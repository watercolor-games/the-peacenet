using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Plex.Objects.ShiftFS
{

    public class File
    {
        public string Name;
        public byte[] Data;
        public byte[] HeaderData;
        public bool ReadAccessToLowUsers;
        public System.IO.Stream GetStream()
        {
            return new System.IO.MemoryStream(Data);
        }

        public File(string name, byte[] data, bool ReadAccess_to_low_users)
        {
            Name = name;
            Data = data;
            ReadAccessToLowUsers = ReadAccess_to_low_users;
        }
    }
    public class Directory
    {
        public string Name;
        public List<File> Files = new List<File>();
        public List<Directory> Subdirectories = new List<Directory>();
        public bool ReadAccessToLowUsers;
        public void AddFile(File file)
        {
            Files.Add(file);
        }

        public void RemoveFile(string name)
        {
            Files.Remove(Files.Find(x => x.Name == name));
        }

        public void RemoveFile(File file)
        {
            Files.Remove(file);
        }

        public File FindFileByName(string name)
        {
            return Files.Find(x => x.Name == name);
        }

        public void AddDirectory(Directory dir)
        {
            Subdirectories.Add(dir);
        }

        public void RemoveDirectory(string name)
        {
            Subdirectories.Remove(Subdirectories.Find(x => x.Name == name));
        }

        public void RemoveDirectory(Directory dir)
        {
            Subdirectories.Remove(dir);
        }

        public Directory FindDirectoryByName(string name)
        {
            return Subdirectories.Find(x => x.Name == name);
        }
    }

    public class PathData
    {
        /// <summary>
        /// The drive number that the server uses to find the right FS
        /// </summary>
        public int DriveNumber { get; set; }

        /// <summary>
        /// The path within the drive.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Contextual data for the server. If you are requesting a file write, place the contents here.
        /// </summary>
        public string AdditionalData { get; set; }
    }

    public class FileRecord
    {
        public string Name { get; set; }
        public long SizeBytes { get; set; }
        public bool IsDirectory { get; set; }
    }
}
