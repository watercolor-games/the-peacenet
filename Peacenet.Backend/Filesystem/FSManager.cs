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
    public class FSManager : IBackendComponent
    {
        private List<ADriveMount> _mounts = new List<ADriveMount>();
        private readonly byte[] drivemagic = Encoding.UTF8.GetBytes("f1l3");

        public ADriveMount GetDriveFromPathData(string username, string pathdata, out string path)
        {
            var pdata = JsonConvert.DeserializeObject<PathData>(pathdata);
            path = pdata.Path;
            return _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == pdata.DriveNumber);
        }

        public bool CreateFS(string username, int drivenumber, string label)
        {
            var existing = _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == drivenumber);
            if (existing != null)
                return false;
            existing = new PlexFATDriveMount(new MountInformation
            {
                 DriveNumber = drivenumber,
                ImageFilePath = Path.Combine("drives", $"{username}.{drivenumber}.drv"),
                 Specification = DriveSpec.PlexFAT,
                 VolumeLabel = label
            }, username);
            _mounts.Add(existing);
            Logger.Log($"Creating drive: {drivenumber}:/ ({label}) for {username}");
            return true;
        }

        public ADriveMount GetFS(string username, int drivenum)
        {
            return _mounts.FirstOrDefault(x => x.SessionID == username && x.DriveNumber == drivenum);
        }


        public void Initiate()
        {
            if (!System.IO.Directory.Exists("drives"))
            {
                Logger.Log("Creating drive directory...");
                System.IO.Directory.CreateDirectory("drives");
                Logger.Log("Done.");
            }
            Logger.Log("Now looking for drive mount info.");
            foreach (var file in System.IO.Directory.GetFiles("drives"))
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

        public void Unload()
        {
            Logger.Log("Unmounting filesystems...");
            _mounts.Clear();
            _mounts = null;
            Logger.Log("Done.");
        }
    }

    public class SystemDriveInfo
    {
        public string Username { get; set; }
        public List<ADriveMount> Mounts { get; set; }
    }
}
