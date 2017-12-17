using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Config;
using Plex.Engine.Server;
using System.IO;
using Plex.Objects.PlexFS;
using Plex.Objects.ShiftFS;

namespace Plex.Engine.Filesystem
{
    public class FSManager : IEngineComponent
    {
        [Dependency]
        private AppDataManager _appdata = null;

        [Dependency]
        private AsyncServerManager _server = null;

        private string _fsDirectory = null;

        private PlexFAT _localFAT = null;
        private Stream fobj = null;

        public void CreateDirectory(string path)
        {
            CreateDirectoryInternal(path);
        }

        public bool DirectoryExists(string path)
        {
            return DirectoryExistsInternal(path);
        }

        public bool FileExists(string path)
        {
            return FileExistsInternal(path);
        }

        public FileRecord GetFileRecord(string path)
        {
            return GetFileInfoInternal(path);
        }

        public string[] GetFiles(string path)
        {
            return GetFilesInternal(path);
        }

        public string[] GetDirectories(string path)
        {
            return GetDirectoriesInternal(path);
        }

        public void Initiate()
        {
            _fsDirectory = Path.Combine(_appdata.GamePath, "filesystem");
            if (!System.IO.Directory.Exists(_fsDirectory))
                System.IO.Directory.CreateDirectory(_fsDirectory);
            string fsLoc = Path.Combine(_fsDirectory, "filesystem.pfat");
            bool extant = System.IO.File.Exists(fsLoc);
            fobj = System.IO.File.Open(fsLoc, FileMode.OpenOrCreate);
            _localFAT = extant ? PlexFAT.FromStream(fobj) : PlexFAT.New(fobj);

        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            _localFAT = null;
            fobj.Close();
        }

        private PlexFAT.Directory getDirectory(string path)
        {
            lock (_localFAT)
            {
                //No need for this check. Server already does this.
                //string pathvnum = path.Split(new[] {':'})[0];
                //if (DriveNumber.ToString() != pathvnum)
                //    throw new IO.IOException($"This is drive {DriveNumber}, the path specifies {pathvnum}.");
                string[] components = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                PlexFAT.Directory ret = _localFAT.Root;
                if (components.Length >= 1)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        ret = ret.GetSubdirectory(components[i]);
                    }
                }
                return ret;
            }
        }

        private void getParent(string path, out PlexFAT.Directory parent, out string fname)
        {
            lock (_localFAT)
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
                    parent = _localFAT.Root;
                }
            }
        }

        private FileRecord GetFileInfoInternal(string path)
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

        private void CreateDirectoryInternal(string path)
        {
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            if (parent.Contents.Contains(dname))
                throw new IOException($"'{path}' already exists.");
            parent.GetSubdirectory(dname, OpenMode.OpenOrCreate);
        }

        private void DeleteDirectoryInternal(string path)
        {
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            EntryType type = parent.TypeOf(dname);
            if (type == EntryType.DIRECTORY)
                parent.Delete(dname);
        }

        private void DeleteFileInternal(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            EntryType type = parent.TypeOf(fname);
            if (type == EntryType.FILE)
                parent.Delete(fname);
        }

        private bool DirectoryExistsInternal(string path)
        {
            if (!path.Contains("/"))
                return true; //obvi. root
            PlexFAT.Directory parent;
            string dname;
            getParent(path, out parent, out dname);
            EntryType type = parent.TypeOf(dname);
            return type == EntryType.DIRECTORY;
        }

        private bool FileExistsInternal(string path)
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
            foreach (var entry in arr)
            {
                lst.Add(path + "/" + entry);
            }

            return lst.ToArray();
        }

        private string[] GetDirectoriesInternal(string path)
        {
            return searchType(path, EntryType.DIRECTORY);
        }

        private string[] GetFilesInternal(string path)
        {
            return searchType(path, EntryType.FILE);
        }

        private byte[] ReadAllBytesInternal(string path)
        {
            PlexFAT.Directory parent;
            string fname;
            getParent(path, out parent, out fname);
            var bytes = new byte[parent.SizeOf(fname)];
            using (Stream fobj = parent.OpenFile(fname))
                fobj.Read(bytes, 0, bytes.Length);
            return bytes;
        }


        private void WriteAllBytesInternal(string path, byte[] bytes)
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

    }
}
