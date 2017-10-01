using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Objects.ShiftFS;
using Newtonsoft.Json;

namespace Plex.Server.DriveSpecs
{
    [FSHandler(Objects.DriveSpec.ShiftFS)]
    public class ShiftFSDriveMount : ADriveMount
    {
        private Directory _root = null;

        public ShiftFSDriveMount(MountInformation mountdata, string usersession) : base(mountdata, usersession)
        {
            Console.WriteLine("ShiftFS mount started at {0}:", mountdata.DriveNumber);
        }

        public override FileRecord GetFileInfo(string path)
        {
            if (!FileExists(path) && !DirectoryExists(path))
                return null; //no file.
            var fr = new FileRecord();
            fr.Name = path.Split('/').Last();
            fr.IsDirectory = DirectoryExists(path);
            if (!fr.IsDirectory)
                fr.SizeBytes = ReadAllBytes(path).LongLength;
            return fr;
        }

        private void SaveToDisk()
        {
            System.IO.File.WriteAllText(ImageLocation, JsonConvert.SerializeObject(_root));
        }

        public override void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
            {
                string[] pathlist = path.Split('/');
                var dir = _root;
                for (int i = 1; i <= pathlist.Length - 2; i++)
                {
                    dir = dir.FindDirectoryByName(pathlist[i]);
                }
                dir.AddDirectory(new Directory
                {
                    Name = pathlist[pathlist.Length - 1],

                });
                SaveToDisk();
            }
            else
            {
                throw new Exception("The directory \"" + path + "\" already exists.");
            }
        }

        public override void DeleteDirectory(string path)
        {
            if (DirectoryExists(path))
                DeleteInternal(path);
        }
        
        //For some reason shiftfs only has one delete call that handles both directories and files
        private void DeleteInternal(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }

            if (FileExists(path))
                dir.RemoveFile(pathlist[pathlist.Length - 1]);
            else
                dir.RemoveDirectory(pathlist[pathlist.Length - 1]);

        }

        public override void DeleteFile(string path)
        {
            if (DirectoryExists(path))
                DeleteInternal(path);
        }

        public override bool DirectoryExists(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;

            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir?.FindDirectoryByName(pathlist[i]);
                if (dir == null)
                    return false;
            }
            return dir != null;

        }

        public override void EnsureDriveExistence()
        {
            _root = new Directory();
            if (System.IO.File.Exists(ImageLocation))
            {
                //ShiftFS isn't streamable...this is gonna use a lot of ram
                _root = JsonConvert.DeserializeObject<Directory>(System.IO.File.ReadAllText(ImageLocation));
            }
            System.IO.File.WriteAllText(ImageLocation, JsonConvert.SerializeObject(_root));
        }

        public override bool FileExists(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
                if (dir == null)
                    return false;
            }
            return dir.FindFileByName(pathlist[pathlist.Length - 1]) != null;

        }

        public override string[] GetDirectories(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);

            List<string> paths = new List<string>();

            foreach (var subdir in dir.Subdirectories)
            {
                paths.Add(DriveNumber + path + "/" + subdir.Name);
            }
            paths.Sort();
            return paths.ToArray();
        }

        public override string[] GetFiles(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 1; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1, 1);

            List<string> paths = new List<string>();

            foreach (var subdir in dir.Files)
            {
                paths.Add(DriveNumber + path + "/" + subdir.Name);
            }
            paths.Sort();
            return paths.ToArray();
        }

        public override byte[] ReadAllBytes(string path)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
            }
            var file = dir.FindFileByName(pathlist[pathlist.Length - 1]);
            return file.Data;
        }

        public override string ReadAllText(string path)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(path));
        }

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            string[] pathlist = path.Split('/');
            var dir = _root;
            for (int i = 1; i <= pathlist.Length - 2; i++)
            {
                dir = dir.FindDirectoryByName(pathlist[i]);
                if (dir == null)
                    throw new System.IO.IOException("Parent directory not found.");
            }

            if (!FileExists(path))
            {
                dir.AddFile(new File(pathlist[pathlist.Length - 1], bytes, false));
            }
            else
            {
                var f = dir.FindFileByName(pathlist[pathlist.Length - 1]);
                f.Data = bytes;
            }
            SaveToDisk();
        }

        public override void WriteAllText(string path, string text)
        {
            WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }
    }
}
