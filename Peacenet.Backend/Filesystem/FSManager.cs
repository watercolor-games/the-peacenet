//TODO: Use the database backend in some way for storing drive info.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;
using LiteDB;

namespace Peacenet.Backend.Filesystem
{
    /// <summary>
    /// Provides an asynchronous API for interacting with virtual file systems.
    /// </summary>
    public class FSManager : IBackendComponent
    {
        private LiteCollection<EntityMount> _drives = null;

        private List<ADriveMount> _mounts = new List<ADriveMount>();
        private readonly byte[] drivemagic = Encoding.UTF8.GetBytes("f1l3");

        [Dependency]
        private Backend _backend = null;

        [Dependency]
        private DatabaseHolder _database = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        /// <summary>
        /// Retrieves a VFS from the JSON representation of a <see cref="PathData"/> object. 
        /// </summary>
        /// <param name="entityid">The entity ID for the user of which you'd like to query mounts from.</param>
        /// <param name="pathdata">The JSON path data.</param>
        /// <param name="path">The path usable within the mount.</param>
        /// <returns>The mount to which the path data refers to.</returns>
        public ADriveMount GetDriveFromPathData(string entityid, string pathdata, out string path)
        {
            var pdata = JsonConvert.DeserializeObject<PathData>(pathdata);
            path = pdata.Path;
            return _mounts.FirstOrDefault(x => x.SessionID == entityid && x.DriveNumber == pdata.DriveNumber);
        }

        /// <summary>
        /// Retrieves all available mounts for a given user.
        /// </summary>
        /// <param name="entityid">The entity ID to look up.</param>
        /// <returns>The available mounts for the user.</returns>
        public Dictionary<int, string> GetDrivesForUser(string entityid)
        {
            var mounts = _mounts.Where(x => x.SessionID == entityid);
            var dict = new Dictionary<int, string>();
            foreach (var mount in mounts)
                dict.Add(mount.DriveNumber, mount.VolumeLabel);
            return dict;
        }

        /// <summary>
        /// Create a virtual file system for a user.
        /// </summary>
        /// <param name="entityid">The entity ID that the new drive will belong to.</param>
        /// <param name="drivenumber">The drive number (mount point) for the drive.</param>
        /// <param name="label">The volume label of the drive</param>
        /// <returns>Whether the drive could be created.</returns>
        public bool CreateFS(string entityid, int drivenumber, string label)
        {
            var entity = _entityBackend.GetEntity(entityid);
            if (entity == null)
                return false;
            var existingDrive = _drives.FindOne(x => x.EntityId == entityid && x.Mountpoint == drivenumber && x.VolumeLabel == label);
            if (existingDrive != null)
                return false;
            _drives.Insert(new EntityMount
            {
                Id = Guid.NewGuid().ToString(),
                EntityId = entityid,
                ImagePath = $"{entityid}_d{drivenumber}.pfat",
                Mountpoint = drivenumber,
                VolumeLabel = label
            });
            SafetyCheck();
            return true;
        }

        /// <summary>
        /// Get a filesystem for a given user.
        /// </summary>
        /// <param name="entityid">The entity ID to look up.</param>
        /// <param name="drivenum">The drive number (mount point) for the drive.</param>
        /// <returns>The mount that was found. Returns null if none could be found.</returns>
        public ADriveMount GetFS(string entityid, int drivenum)
        {
            return _mounts.FirstOrDefault(x => x.SessionID == entityid && x.DriveNumber == drivenum);
        }

        private string _drivePath = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            _drivePath = Path.Combine(_backend.RootDirectory, "drives");
            if (!System.IO.Directory.Exists(_drivePath))
            {
                Plex.Objects.Logger.Log("Creating drive directory...");
                System.IO.Directory.CreateDirectory(_drivePath);
                Plex.Objects.Logger.Log("Done.");
            }
            Plex.Objects.Logger.Log("Loading and mounting entity drives...");
            this._drives = _database.Database.GetCollection<EntityMount>("entity_drives");
            _drives.EnsureIndex(x => x.Id);
            var noFSCount = _drives.Delete(x => !File.Exists(Path.Combine(_drivePath, x.ImagePath)));
            var noEntityDrives = _drives.Find(x => _entityBackend.GetEntity(x.EntityId) == null);
            foreach(var drive in noEntityDrives)
            {
                Plex.Objects.Logger.Log($"Removing drive: //{drive.EntityId}/{drive.Mountpoint}. Entity not found.");
                File.Delete(Path.Combine(_drivePath, drive.ImagePath));
                _drives.Delete(x => x.Id == drive.Id);
            }
            Plex.Objects.Logger.Log($"{noFSCount} drives deleted from database due to missing PlexFAT images.");
            Plex.Objects.Logger.Log($"{noEntityDrives.Count()} drives deleted from database due to missing NPC or player entities.");
            Plex.Objects.Logger.Log($"{_drives.Count()} drives loaded from database. Mounting...");

            foreach (var drive in _drives.FindAll())
            {
                Plex.Objects.Logger.Log($"Mounting {drive.ImagePath} to //{drive.EntityId}/{drive.Mountpoint}...");
                var fat = new PlexFATDriveMount(new MountInformation
                {
                    DriveNumber = drive.Mountpoint,
                    ImageFilePath = Path.Combine(_drivePath, drive.ImagePath),
                    Specification = DriveSpec.PlexFAT,
                    VolumeLabel = drive.VolumeLabel
                }, drive.EntityId);
                fat.EnsureDriveExistence();
                _mounts.Add(fat);
            }
            Plex.Objects.Logger.Log("Done loading filesystems...");

            _entityBackend.EntitySpawned += (id, entity) =>
            {
                //Create a drive for the entity if they don't have one.
                if (CreateFS(id, 0, "Peacegate OS"))
                    Plex.Objects.Logger.Log($"Created new 'Peacegate OS' drive at //{id}/0.");
            };
            _backend.PlayerJoined += (id, player) =>
            {
                if (CreateFS(_entityBackend.GetPlayerEntityId(id), 0, "Peacegate OS"))
                    Plex.Objects.Logger.Log($"Created new 'Peacegate OS' drive at //{id}/0.");
            };
        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
            Plex.Objects.Logger.Log("Updating drive database...");
            var noEntityDrives = _drives.Find(x => _entityBackend.GetEntity(x.EntityId) == null);
            foreach (var drive in noEntityDrives)
            {
                Plex.Objects.Logger.Log($"Removing drive: //{drive.EntityId}/{drive.Mountpoint}. Entity not found.");
                var mount = _mounts.FirstOrDefault(x => x.DriveNumber == drive.Mountpoint && x.VolumeLabel == drive.VolumeLabel && x.SessionID == drive.EntityId);
                if(mount != null)
                {
                    mount.Dispose();
                    _mounts.Remove(mount);
                }
                File.Delete(Path.Combine(_drivePath, drive.ImagePath));
                _drives.Delete(x => x.Id == drive.Id);
            }
            Plex.Objects.Logger.Log("Mounting newly-created drives...");
            foreach(var drive in _drives.Find(x=>_mounts.FirstOrDefault(y=>y.DriveNumber == x.Mountpoint && y.SessionID == x.EntityId && y.VolumeLabel == x.VolumeLabel) == null))
            {
                Plex.Objects.Logger.Log($"Mounting {drive.ImagePath} to //{drive.EntityId}/{drive.Mountpoint}...");
                var fat = new PlexFATDriveMount(new MountInformation
                {
                    DriveNumber = drive.Mountpoint,
                    ImageFilePath = Path.Combine(_drivePath, drive.ImagePath),
                    Specification = DriveSpec.PlexFAT,
                    VolumeLabel = drive.VolumeLabel
                }, drive.EntityId);
                fat.EnsureDriveExistence();
                _mounts.Add(fat);

            }
        }

        /// <inheritdoc/>
        public void Unload()
        {
            Plex.Objects.Logger.Log("Unmounting filesystems...");
            while (_mounts.Count > 0)
            {
                _mounts[0].Dispose();
                _mounts.RemoveAt(0);
            }
            _mounts.Clear();
            _mounts = null;
            Plex.Objects.Logger.Log("Done.");
        }
    }

    /// <summary>
    /// Contains all drive mounts for a given user.
    /// </summary>
    public class SystemDriveInfo
    {
        /// <summary>
        /// The user ID of the mount list.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The mounts available to the user.
        /// </summary>
        public List<ADriveMount> Mounts { get; set; }
    }

    public class EntityMount
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string ImagePath { get; set; }
        public string VolumeLabel { get; set; }
        public int Mountpoint { get; set; }
    }
}
