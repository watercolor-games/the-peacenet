//TODO: Use the database backend in some way for storing drive info.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;

namespace Peacenet.Backend.Filesystem
{
    /// <summary>
    /// Provides an asynchronous API for interacting with virtual file systems.
    /// </summary>
    public class FSManager : IBackendComponent
    {
        private List<ADriveMount> _mounts = new List<ADriveMount>();
        private readonly byte[] drivemagic = Encoding.UTF8.GetBytes("f1l3");

        [Dependency]
        private Backend _backend = null;

        /// <summary>
        /// Retrieves a VFS from the JSON representation of a <see cref="PathData"/> object. 
        /// </summary>
        /// <param name="username">Misnomer. This is really the user ID for the user of which you'd like to query mounts from.</param>
        /// <param name="pathdata">The JSON path data.</param>
        /// <param name="path">The path usable within the mount.</param>
        /// <returns>The mount to which the path data refers to.</returns>
        public ADriveMount GetDriveFromPathData(string username, string pathdata, out string path)
        {
            var pdata = JsonConvert.DeserializeObject<PathData>(pathdata);
            path = pdata.Path;
            return _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == pdata.DriveNumber);
        }

        /// <summary>
        /// Retrieves all available mounts for a given user.
        /// </summary>
        /// <param name="username">The user ID to look up.</param>
        /// <returns>The available mounts for the user.</returns>
        public Dictionary<int, string> GetDrivesForUser(string username)
        {
            var mounts = _mounts.Where(x => x.SessionID == username);
            var dict = new Dictionary<int, string>();
            foreach (var mount in mounts)
                dict.Add(mount.DriveNumber, mount.VolumeLabel);
            return dict;
        }

        /// <summary>
        /// Create a virtual file system for a user.
        /// </summary>
        /// <param name="username">The user ID that the new drive will belong to.</param>
        /// <param name="drivenumber">The drive number (mount point) for the drive.</param>
        /// <param name="label">The volume label of the drive</param>
        /// <returns>Whether the drive could be created.</returns>
        public bool CreateFS(string username, int drivenumber, string label)
        {
            var existing = _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == drivenumber);
            if (existing != null)
                return false;
            existing = new PlexFATDriveMount(new MountInformation
            {
                 DriveNumber = drivenumber,
                ImageFilePath = Path.Combine(_drivePath, $"{username}.{drivenumber}.drv"),
                 Specification = DriveSpec.PlexFAT,
                 VolumeLabel = label
            }, username);
            _mounts.Add(existing);
            Logger.Log($"Creating drive: {drivenumber}:/ ({label}) for {username}");
            return true;
        }

        /// <summary>
        /// Get a filesystem for a given user.
        /// </summary>
        /// <param name="username">The user ID to look up.</param>
        /// <param name="drivenum">The drive number (mount point) for the drive.</param>
        /// <returns>The mount that was found. Returns null if none could be found.</returns>
        public ADriveMount GetFS(string username, int drivenum)
        {
            return _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == drivenum);
        }

        private string _drivePath = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            _drivePath = Path.Combine(_backend.RootDirectory, "drives");
            if (!System.IO.Directory.Exists(_drivePath))
            {
                Logger.Log("Creating drive directory...");
                System.IO.Directory.CreateDirectory(_drivePath);
                Logger.Log("Done.");
            }
            Logger.Log("Now looking for drive mount info.");
            foreach (var file in System.IO.Directory.GetFiles(_drivePath))
            {
                if (!file.EndsWith(".info", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                Logger.Log("Opening: " + file);
                using (var fs = System.IO.File.OpenRead(file))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        var magic = reader.ReadBytes(drivemagic.Length);
                        if (!magic.SequenceEqual(drivemagic))
                        {
                            Logger.Log("Not a valid drive image. Skipping.");
                            continue;
                        }
                        string username = reader.ReadString();
                        Logger.Log("Loading drive for: " + username);
                        int drivenum = reader.ReadInt32();
                        string drivelabel = reader.ReadString();
                        string drivepath = reader.ReadString();
                        var mountinfo = new MountInformation
                        {
                            DriveNumber = drivenum,
                            ImageFilePath = drivepath,
                            Specification = DriveSpec.PlexFAT,
                            VolumeLabel = drivelabel
                        };
                        var plexfat = new PlexFATDriveMount(mountinfo, username);
                        plexfat.EnsureDriveExistence();
                        _mounts.Add(plexfat);
                        Logger.Log("Mount loaded.");
                    }
                }
            }
            Logger.Log("Done loading filesystems...");
        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
            Logger.Log("Saving filesystems...");
            foreach (var mount in _mounts)
            {
                mount.EnsureDriveExistence();
                string path = mount.ImageLocation + ".info";
                using (var fs = System.IO.File.OpenWrite(path))
                {
                    using (var writer = new BinaryWriter(fs))
                    {
                        writer.Write(drivemagic);
                        writer.Write(mount.SessionID);
                        writer.Write(mount.DriveNumber);
                        writer.Write(mount.VolumeLabel);
                        writer.Write(mount.ImageLocation);
                    }
                }
                Logger.Log("Wrote: " + path);
            }
            Logger.Log("Done.");
        }

        /// <inheritdoc/>
        public void Unload()
        {
            Logger.Log("Unmounting filesystems...");
            while (_mounts.Count > 0)
            {
                _mounts[0].Dispose();
                _mounts.RemoveAt(0);
            }
            _mounts.Clear();
            _mounts = null;
            Logger.Log("Done.");
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
}
