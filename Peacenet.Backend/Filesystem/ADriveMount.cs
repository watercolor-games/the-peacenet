using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Plex.Objects;
using Plex.Objects.PlexFS;
using Plex.Objects.ShiftFS;

namespace Peacenet.Backend.Filesystem
{
    public abstract class ADriveMount
    {
        private int _volumenum = 0;
        private string _diskLoc = "";
        private string _usersession = "";
        private string _label = null;

        public ADriveMount(MountInformation mountdata, string usersession)
        {
            _volumenum = mountdata.DriveNumber;
            _diskLoc = mountdata.ImageFilePath;
            _usersession = usersession;
            _label = mountdata.VolumeLabel;
            EnsureDriveExistence();
        }

        public string VolumeLabel
        {
            get
            {
                return _label;
            }
        }

        /// <summary>
        /// Gets the drive number associated with this volume.
        /// </summary>
        public int DriveNumber
        {
            get
            {
                return _volumenum;
            }
        }

        /// <summary>
        /// Gets the volume's disk image filepath for storage on the system HDD/SSD.
        /// </summary>
        public string ImageLocation
        {
            get
            {
                return _diskLoc;
            }
        }

        /// <summary>
        /// Gets the usersession ID for this drive.
        /// </summary>
        public string SessionID
        {
            get
            {
                return _usersession;
            }
        }

        /// <summary>
        /// Ensure proper existence of the drive. Use this to create any required data if it doesn't already exist.
        /// </summary>
        public abstract void EnsureDriveExistence();

        /// <summary>
        /// Write binary data to a file.
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="bytes">The data to write</param>
        public abstract void WriteAllBytes(string path, byte[] bytes);

        /// <summary>
        /// Get information about a file or directory such as the name, size, etc.
        /// </summary>
        /// <param name="path">The path to retrieve info about.</param>
        /// <returns>The file record data.</returns>
        public abstract FileRecord GetFileInfo(string path);

        /// <summary>
        /// Write text to a file.
        /// </summary>
        /// <param name="path">The file path to write to</param>
        /// <param name="text">The text to write</param>
        public abstract void WriteAllText(string path, string text);

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="path">The file to delete.</param>
        public abstract void DeleteFile(string path);

        /// <summary>
        /// Delete a directory.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        public abstract void DeleteDirectory(string path);

        /// <summary>
        /// Check if a file exists at this path.
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>Whether the file exists</returns>
        public abstract bool FileExists(string path);

        /// <summary>
        /// Check if a directory exists at this path
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>Whether the directory exists</returns>
        public abstract bool DirectoryExists(string path);

        /// <summary>
        /// Create a directory.
        /// </summary>
        /// <param name="path">The path to create the directory at</param>
        public abstract void CreateDirectory(string path);

        /// <summary>
        /// Read the text of a file
        /// </summary>
        /// <param name="path">The file to read</param>
        /// <returns>The text to read</returns>
        public abstract string ReadAllText(string path);

        /// <summary>
        /// Read the binary data of a file
        /// </summary>
        /// <param name="path">The file to read</param>
        /// <returns>The binary data</returns>
        public abstract byte[] ReadAllBytes(string path);

        /// <summary>
        /// Get a list of files in a directory.
        /// </summary>
        /// <param name="path">The directory to search</param>
        /// <returns>All files found in the directory.</returns>
        public abstract string[] GetFiles(string path);

        /// <summary>
        /// Get all subdirectories in a directory.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <returns>A list of all found subdirectories.</returns>
        public abstract string[] GetDirectories(string path);

        public abstract void Dispose();
    }


    public class PlexFATDriveMount : ADriveMount
    {
        private PlexFAT.Directory getDirectory(string path)
        {
            //No need for this check. Server already does this.
            //string pathvnum = path.Split(new[] {':'})[0];
            //if (DriveNumber.ToString() != pathvnum)
            //    throw new IO.IOException($"This is drive {DriveNumber}, the path specifies {pathvnum}.");
            string[] components = path.Split(new[] { '/' });
            PlexFAT.Directory ret = vol.Root;
            if (components.Length >= 2)
            {
                for (int i = 1; i < components.Length; i++)
                {
                    ret = ret.GetSubdirectory(components[i]);
                }
            }
            return ret;
        }

        private void getParent(string path, out PlexFAT.Directory parent, out string fname)
        {
            if (path.EndsWith("/"))
            {
                //Remove last slash if it's the last char in the string.
                path = path.Remove(path.LastIndexOf("/"), 1);
            }
            if (path.Contains("/"))
            {
                int slashpos = path.LastIndexOf("/");
                fname = path.Substring(slashpos + 1);
                parent = getDirectory(path.Substring(0, slashpos));
            }
            else
            {
                fname = "";
                parent = vol.Root;
            }
        }

        private Stream fobj = null;
        private PlexFAT vol = null;

        public override void Dispose()
        {
            vol = null;
            fobj.Close();
            fobj = null;
        }

        public PlexFATDriveMount(MountInformation mountdata, string usersession) : base(mountdata, usersession)
        {
            Console.WriteLine("PlexFAT mount started at {0}:", mountdata.DriveNumber);
        }

        public override FileRecord GetFileInfo(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            EntryType type = parent.TypeOf(fname);
            if (type == EntryType.NONEXISTENT)
                return null;

            var ret = new FileRecord();
            ret.Name = fname;
            ret.IsDirectory = type == EntryType.DIRECTORY;
            if (!ret.IsDirectory)
                ret.SizeBytes = parent.SizeOf(fname);
            return ret;
        }

        public override void CreateDirectory(string path)
        {
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            if (parent.Contents.Contains(dname))
                throw new IOException($"'{path}' already exists.");
            parent.GetSubdirectory(dname, OpenMode.OpenOrCreate);
        }

        public override void DeleteDirectory(string path)
        {
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            EntryType type = parent.TypeOf(dname);
            if (type == EntryType.DIRECTORY)
                parent.Delete(dname);
        }

        public override void DeleteFile(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            EntryType type = parent.TypeOf(fname);
            if (type == EntryType.FILE)
                parent.Delete(fname);
        }

        public override bool DirectoryExists(string path)
        {
            if (!path.Contains("/"))
                return true; //obvi. root
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            EntryType type = parent.TypeOf(dname);
            return type == EntryType.DIRECTORY;
        }

        public override void EnsureDriveExistence()
        {
            fobj?.Close();
            bool extant = System.IO.File.Exists(ImageLocation);
            fobj = System.IO.File.Open(ImageLocation, FileMode.OpenOrCreate);
            vol = extant ? PlexFAT.FromStream(fobj) : PlexFAT.New(fobj);
        }

        public override bool FileExists(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            EntryType type = parent.TypeOf(fname);
            return type == EntryType.FILE;
        }

        private string[] searchType(string path, EntryType type)
        {
            PlexFAT.Directory dir = getDirectory(path);
            var arr = dir.Contents.Where(n => dir.TypeOf(n) == type).ToArray();
            var lst = new List<string>();
            if (path.EndsWith("/"))
                path = path.Remove(path.LastIndexOf("/"), 1);
            if(type == EntryType.DIRECTORY)
                lst.Add(DriveNumber + path + "/.");
            foreach (var entry in arr)
            {
                lst.Add(DriveNumber + path + "/" + entry);
            }

            return lst.ToArray();
        }

        public override string[] GetDirectories(string path)
        {
            List<string> types = new List<string>();
            types.AddRange(searchType(path, EntryType.DIRECTORY));
            return types.ToArray();
        }

        public override string[] GetFiles(string path)
        {
            return searchType(path, EntryType.FILE);
        }

        public override byte[] ReadAllBytes(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            var bytes = new byte[parent.SizeOf(fname)];
            using (Stream fobj = parent.OpenFile(fname))
                fobj.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public override string ReadAllText(string path)
        {
            // This really should be defined by ADriveMount.
            return Encoding.UTF8.GetString(ReadAllBytes(path));
        }

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            using (Stream fobj = parent.OpenFile(fname, OpenMode.OpenOrCreate))
            {
                fobj.SetLength(bytes.Length);
                fobj.Write(bytes, 0, bytes.Length);
            }
        }

        public override void WriteAllText(string path, string text)
        {
            // ... and this.
            WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }
    }

}
