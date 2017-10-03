using System;
using IO = System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Plex.Objects;
using ShiftFS = Plex.Objects.ShiftFS; // mike move FileRecord :angery:
using Plex.Objects.PlexFS;


namespace Plex.Server.DriveSpecs
{
    [FSHandler(Objects.DriveSpec.PlexFAT)]
    public class PlexFATDriveMount: ADriveMount
    {
        private PlexFAT.Directory getDirectory(string path)
        {
            //No need for this check. Server already does this.
            //string pathvnum = path.Split(new[] {':'})[0];
            //if (DriveNumber.ToString() != pathvnum)
            //    throw new IO.IOException($"This is drive {DriveNumber}, the path specifies {pathvnum}.");
            string[] components = path.Split(new[] {'/'});
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
            if(path.EndsWith("/"))
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
        
        private IO.Stream fobj = null;
        private PlexFAT vol = null;
        
        public PlexFATDriveMount(MountInformation mountdata, string usersession) : base(mountdata, usersession)
        {
            Console.WriteLine("PlexFAT mount started at {0}:", mountdata.DriveNumber);
        }
        
        public override ShiftFS.FileRecord GetFileInfo(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            EntryType type = parent.TypeOf(fname);
            if (type == EntryType.NONEXISTENT)
                return null;
            
            var ret = new ShiftFS.FileRecord();
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
                throw new IO.IOException($"'{path}' already exists.");
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
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            EntryType type = parent.TypeOf(dname);
            return type == EntryType.DIRECTORY;
        }
        
        public override void EnsureDriveExistence()
        {
            fobj?.Close();
            bool extant = IO.File.Exists(ImageLocation);
            fobj = IO.File.Open(ImageLocation, IO.FileMode.OpenOrCreate);
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
            return dir.Contents.Where(n => dir.TypeOf(n) == type).ToArray();
        }
        
        public override string[] GetDirectories(string path)
        {
            return searchType(path, EntryType.DIRECTORY);
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
            using (IO.Stream fobj = parent.OpenFile(fname))
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
            using (IO.Stream fobj = parent.OpenFile(fname, OpenMode.OpenOrCreate))
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

