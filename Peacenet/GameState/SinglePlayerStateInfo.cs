using LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Peacenet.CoreUtils;
using Peacenet.Filesystem;
using Plex.Engine;
using Plex.Engine.Config;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using Plex.Engine.Saves;
using Plex.Objects.PlexFS;
using Plex.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.GameState
{
    public class SinglePlayerStateInfo : IGameStateInfo, IEntity, ISaveBackend
    {
        private float _alertLevel = 0f;
        private bool _alertFalling = false;

        public float AlertLevel => _alertLevel;
        public bool AlertFalling => _alertFalling;

        private double _timeInAlert = 0;
        private float _lastAlert = 0f;

        public float GameCompletion => 0f;

        public float Reputation => 0f;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private GUIUtils _gui = null;

        private LiteDatabase _saveDB = null;

        private LiteCollection<Snapshot> _snapshots = null;
        private LiteCollection<SaveValue> _values = null;

        public SinglePlayerStateInfo()
        {
        }



        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        public void EndGame()
        {
            _plexgate.GetLayer(LayerType.NoDraw).RemoveEntity(this);
            _fs.SetBackend(null);
            _save.SetBackend(null);
            _saveDB.Shrink();
            _saveDB.Dispose();
            _saveDB = null;
            _values = null;
            _snapshots = null;
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
#if DEBUG
            if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.I)
            {
                var opener = new System.Windows.Forms.OpenFileDialog();
                opener.Filter = "All files|*.*";
                opener.Title = "Import file into Peacegate OS";
                if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var fileStream = File.OpenRead(opener.FileName))
                    {
                        byte[] data = new byte[fileStream.Length];
                        fileStream.Read(data, 0, data.Length);
                        _gui.AskForFile(true, (path) =>
                        {
                            _fs.WriteAllBytes(path, data);
                        });
                    }
                }
            }
            #endif
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void StartGame()
        {
            if (!Directory.Exists(_os.SinglePlayerSaveDirectory))
                Directory.CreateDirectory(_os.SinglePlayerSaveDirectory);
            _plexgate.GetLayer(LayerType.NoDraw).AddEntity(this);
            _fs.SetBackend(_plexgate.New<SinglePlayerFilesystem>());

            _saveDB = new LiteDatabase(Path.Combine(_os.SinglePlayerSaveDirectory, "savefile.db"));

            _values = _saveDB.GetCollection<SaveValue>("values");
            _snapshots = _saveDB.GetCollection<Snapshot>("snapshots");
            _values.EnsureIndex(x => x.ID);
            _snapshots.EnsureIndex(x => x.ID);

            _save.SetBackend(this);
            _os.EnsureProperEnvironment();
        }

        public void Update(GameTime time)
        {
            if(_alertLevel > 0 && _alertLevel < 0.8F)
            {
                if(_alertFalling)
                {
                    if(_alertLevel>_lastAlert)
                    {
                        _timeInAlert = 0;
                        _alertFalling = false;
                        _lastAlert = _alertLevel;
                        return;
                    }
                    _alertLevel = MathHelper.Clamp(_alertLevel - ((float)time.ElapsedGameTime.TotalSeconds / 20), 0, 1);
                    _lastAlert = _alertLevel;
                }
                else
                {
                    if (_lastAlert != _alertLevel)
                    {
                        _lastAlert = _alertLevel;
                        _timeInAlert = 0;
                    }
                    _timeInAlert += time.ElapsedGameTime.TotalSeconds;
                    if(_timeInAlert>=30)
                    {
                        _alertFalling = true;
                        _timeInAlert = 0;
                    }
                }
            }
            else
            {
                _alertFalling = false;
                _timeInAlert = 0;
                _lastAlert = 0;
            }
        }

        public bool IsMissionComplete(string missionID)
        {
            return false;
        }

        public bool IsCountryUnlocked(Country country)
        {
            return false;
        }

        public bool IsPackageInstalled(string packageID)
        {
            return false;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            var val = _values.FindOne(x => x.ID == key);
            if(val == null)
            {
                val = new SaveValue
                {
                    ID = key,
                    Value = JsonConvert.SerializeObject(defaultValue)
                };
                _values.Insert(val);
                return defaultValue;
            }
            return JsonConvert.DeserializeObject<T>(val.Value);
        }

        public bool TutorialCompleted
        {
            get
            {
                return GetValue<bool>("peacegate.tutorialCompleted", false);
            }
            set
            {
                SetValue("peacegate.tutorialCompleted", true);
            }
        }

        public void SetValue<T>(string key, T value)
        {
            var val = _values.FindOne(x => x.ID == key);
            if (val == null)
            {
                val = new SaveValue
                {
                    ID = key,
                    Value = JsonConvert.SerializeObject(value)
                };
                _values.Insert(val);
                return;
            }
            val.Value = JsonConvert.SerializeObject(value);
            _values.Update(val);
        }

        public string CreateSnapshot()
        {
            var snapshotJSON = JsonConvert.SerializeObject(_values.FindAll());
            var snapshot = new Snapshot { ID = Guid.NewGuid().ToString() };
            using (var stream = _saveDB.FileStorage.OpenWrite(snapshot.ID, snapshot.ID))
            {
                byte[] unicode = Encoding.UTF8.GetBytes(snapshotJSON);
                stream.Write(unicode, 0, unicode.Length);
            }
            _snapshots.Insert(snapshot);
            return snapshot.ID;
        }

        public void RestoreSnapshot(string id)
        {
            var snapshot = _snapshots.FindOne(x => x.ID == id);
            if (snapshot == null)
                throw new InvalidOperationException("Snapshot not found.");
            using (var stream = _saveDB.FileStorage.OpenRead(snapshot.ID))
            {
                byte[] unicode = new byte[stream.Length];
                stream.Read(unicode, 0, unicode.Length);
                var data = JsonConvert.DeserializeObject<List<SaveValue>>(Encoding.UTF8.GetString(unicode));
                _values.Delete(x => true);
                _values.InsertBulk(data);
            }
            _saveDB.FileStorage.Delete(snapshot.ID);
            _snapshots.Delete(x => x.ID == id);
        }

        private class SaveValue
        {
            public string ID { get; set; }
            public string Value { get; set; }
        }

        public class Snapshot
        {
            public string ID { get; set; }
        }
    }

    public class SinglePlayerFilesystem : IAsyncFSBackend
    {
        [Dependency]
        private OS _os = null;

        private readonly DirectoryEntry _root = new DirectoryEntry
        {
            ID = null,
            Name = "Root",
            ParentID = null
        };

        private LiteDatabase _db = null;

        private LiteCollection<DirectoryEntry> _dirs = null;
        private LiteCollection<FileEntry> _files = null;

        private string[] getComponents(string path)
        {
            while (path.StartsWith("/"))
                path = path.Remove(0, 1);
            while (path.EndsWith("/"))
                path = path.Remove(path.LastIndexOf("/"), 1);
            return path.Split('/');
        }

        private string getParent(string path)
        {
            if (path == "/")
                return null;
            while (path.EndsWith("/"))
                path = path.Remove(path.LastIndexOf("/"), 1);
            if (path.LastIndexOf("/") == 0)
                return "/";
            return path.Substring(0, path.LastIndexOf("/"));
        }

        private DirectoryEntry findDirectory(string path)
        {
            if (path == "/")
                return _root;
            string[] components = getComponents(path);
            string parent = null;
            DirectoryEntry entry = null;
            foreach(var component in components)
            {
                entry = _dirs.FindOne(x => x.Name == component && x.ParentID == parent);
                if (entry == null)
                    return null;
                parent = entry.ID;
                
            }
            return entry;
        }

        private FileEntry findFile(string path)
        {
            string parent = getParent(path);
            var dir = findDirectory(parent);
            if (dir == null)
                return null;
            return _files.FindOne(x => x.ParentDirectory == dir.ID && x.Name == path.Substring(parent.Length));
        }

        public void CreateDirectory(string path)
        {
            if (DirectoryExists(path))
                return;
            if (path == "/")
                return;
            string[] components = getComponents(path);
            string parent = null;
            foreach (var c in components)
            {
                if (c == ".." || c == ".")
                    throw new IOException("A directory cannot be named '.' or '..'.");
                var dir = _dirs.FindOne(x => x.Name == c && x.ParentID == parent);
                if(dir == null)
                {
                    dir = new DirectoryEntry
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = c,
                        ParentID = parent
                    };
                    _dirs.Insert(dir);
                }
                parent = dir.ID;
            }
        }

        private void recursiveDelete(DirectoryEntry entry)
        {
            foreach(var dir in _dirs.Find(x=>x.ParentID==entry.ID).ToArray())
            {
                recursiveDelete(dir);
            }
            foreach(var file in _files.Find(x=>x.ParentDirectory==entry.ID))
            {
                _db.FileStorage.Delete(file.ID);
            }
            _files.Delete(x => x.ParentDirectory == entry.ID);
            _dirs.Delete(x => x.ID == entry.ID);
        }

        public void Delete(string path)
        {
            if (path == "/")
                throw new IOException("You cannot delete the root of the filesystem.");
            if(DirectoryExists(path))
            {
                recursiveDelete(findDirectory(path));
            }
            else if(FileExists(path))
            {
                var file = findFile(path);
                _db.FileStorage.Delete(file.ID);
                _files.Delete(x => x.ID == file.ID);
            }
            else
            {
                throw new IOException("The file or directory was not found in the file system.");
            }
        }

        public bool DirectoryExists(string path)
        {
            return findDirectory(path) != null;
        }

        public bool FileExists(string path)
        {
            return findFile(path) != null;
        }

        private string combine(params string[] paths)
        {
            string path = "";
            foreach (var c in paths)
            {
                if (c == null)
                    continue;
                if (c.EndsWith("/") || Array.LastIndexOf(paths, c) == paths.Length - 1)
                {
                    path += c;
                }
                else
                {
                    path += c + "/";
                }
            }
            return path;
        }

        public string[] GetDirectories(string path)
        {
            List<string> dirs = new List<string>();
            var entry = findDirectory(path);
            if (entry == null)
                return null;
            foreach (var child in _dirs.Find(x => x.ParentID == entry.ID))
            {
                if (string.IsNullOrWhiteSpace(child.Name))
                    continue;
                dirs.Add(combine(path, child.Name));
            }
            return dirs.ToArray();
        }

        public FileRecord GetFileRecord(string path)
        {
            if(DirectoryExists(path))
            {
                var dir = findDirectory(path);
                return new FileRecord
                {
                    IsDirectory = true,
                    Name = dir.Name,
                    SizeBytes = 0
                };
            }
            else if(FileExists(path))
            {
                var file = findFile(path);
                var meta = _db.FileStorage.FindById(file.ID);
                return new FileRecord
                {
                    IsDirectory = false,
                    Name = file.Name,
                    SizeBytes = meta.Length
                };
            }
            return null;
        }

        public string[] GetFiles(string path)
        {
            List<string> dirs = new List<string>();
            var entry = findDirectory(path);
            if (entry == null)
                return null;
            foreach (var child in _files.Find(x => x.ParentDirectory == entry.ID))
            {
                dirs.Add(combine(path, child.Name));
            }
            return dirs.ToArray();
        }

        public void Initialize()
        {
            Plex.Objects.Logger.Log("Loading client-side filesystem database");
            _db = new LiteDatabase(Path.Combine(_os.SinglePlayerSaveDirectory, "filesystem.db"));
            Plex.Objects.Logger.Log("Gatekeeper is shrinking the database...");
            _db.Shrink();
            _dirs = _db.GetCollection<DirectoryEntry>("directories");
            _files = _db.GetCollection<FileEntry>("fileEntries");
            _dirs.EnsureIndex(x => x.ID);
            _files.EnsureIndex(x => x.ID);
            Plex.Objects.Logger.Log("Gatekeeper is now removing invalid file directory entries...");
            var deletedDirs = _dirs.Delete(x => string.IsNullOrEmpty(x.Name));
            var deletedFiles = _files.Delete(x => string.IsNullOrEmpty(x.Name));
            Plex.Objects.Logger.Log($"Gatekeeper has removed {deletedDirs} directory entries and {deletedFiles} file entries with no names.");
            Plex.Objects.Logger.Log($"Gatekeeper has removed {_dirs.Delete(x => (x.ParentID != null) && (_dirs.FindOne(y => y.ID == x.ParentID) == null))} orphaned directory entries.");
            Plex.Objects.Logger.Log($"Gatekeeper has removed {_files.Delete(x => (x.ParentDirectory != null) && (_dirs.FindOne(y => y.ID == x.ParentDirectory) == null))} orphaned file entries.");

        }

        public Stream Open(string path, FileOpenMode mode)
        {
            var record = findFile(path);
            if (record == null)
            {
                switch (mode)
                {
                    case FileOpenMode.Read:
                        return null;
                    case FileOpenMode.Write:
                        var parentDir = getParent(path);
                        if (string.IsNullOrWhiteSpace(parentDir))
                            parentDir = "/";
                        if (!DirectoryExists(parentDir))
                            CreateDirectory(parentDir);
                        var direntry = findDirectory(parentDir);
                        record = new FileEntry
                        {
                            ID = Guid.NewGuid().ToString(),
                            Name = path.Substring(parentDir.Length),
                            ParentDirectory = direntry.ID
                        };
                        _files.Insert(record);
                        break;
                }
            }

            if (mode == FileOpenMode.Read)
                return _db.FileStorage.OpenRead(record.ID);
            else
                return _db.FileStorage.OpenWrite(record.ID, record.ID);
        }

        public byte[] ReadAllBytes(string path)
        {
            var stream = Open(path, FileOpenMode.Read);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        public void Unload()
        {
            _dirs = null;
            _files = null;
            _db.Shrink();
            _db.Dispose();
            _db = null;
        }

        public void WriteAllBytes(string path, byte[] data)
        {
            if (FileExists(path))
                Delete(path);
            var stream = Open(path, FileOpenMode.Write);
            stream.Write(data, 0, data.Length);
        }

        private class DirectoryEntry
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string ParentID { get; set; }
        }

        public class FileEntry
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string ParentDirectory { get; set; }
        }
    }
}
