using LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Peacenet.CoreUtils;
using Peacenet.Email;
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

        public float GameCompletion { get => GetValue("game.completion", 0f); set => SetValue("game.completion", value); }

        public float Reputation { get => GetValue("player.rep", 0f); set => SetValue("player.rep", value); }

        public event Action<string> MissionCompleted;

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

        private int _unread = 0;

        private LiteDatabase _saveDB = null;

        private LiteCollection<Snapshot> _snapshots = null;
        private LiteCollection<SaveValue> _values = null;
        private LiteCollection<EmailThread> _threads = null;
        private LiteCollection<EmailMessage> _messages = null;


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
            _threads = null;
            _messages = null;

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

            _threads = _saveDB.GetCollection<EmailThread>("emailthreads");
            _messages = _saveDB.GetCollection<EmailMessage>("emailmessages");


            _save.SetBackend(this);
            _os.EnsureProperEnvironment();

            _unread = _messages.Find(x => x.IsUnread).Count();
        }

        public IEnumerable<EmailThread> Emails => _threads.FindAll();

        public IEnumerable<EmailMessage> GetMessages(string threadId)
        {
            return _messages.Find(x => x.EmailId == threadId);
        }

        public void MarkRead(string messageId)
        {
            var message = _messages.FindOne(x => x.Id == messageId);
            if (message == null)
                return;
            message.IsUnread = false;
            _messages.Update(message);
            _unread = _messages.Find(x => x.IsUnread).Count();
        }

        [Dependency]
        private ClientEmailProvider _emailService = null;


        public void Update(GameTime time)
        {
            if(_emailService.Incoming > 0)
            {
                var npcMail = _emailService.Dequeue();
                this.sendNPCMail(npcMail.Subject, npcMail.From, npcMail.Message);
                if(_os.IsDesktopOpen)
                {
                    _os.Desktop.ShowNotification(npcMail.From, npcMail.Subject);
                }
            }

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
            return GetValue($"m.{missionID}.complete", false);
        }

        public bool IsCountryUnlocked(Country country)
        {
            return GetValue($"{country.ToString().ToLower()}.unlocked", false);
        }

        public bool IsPackageInstalled(string packageID)
        {
            return GetValue($"package.{packageID}.installed", false);
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

        public int UnreadEmails
        {
            get
            {
                return _unread;
            }
        }

        private void sendNPCMail(string subject, string from, string message)
        {
            var thread = new EmailThread
            {
                Id = Guid.NewGuid().ToString(),
                Sent = DateTime.Now,
                Subject = subject
            };
            var messageObject = new EmailMessage
            {
                EmailId = thread.Id,
                From = from,
                Id = Guid.NewGuid().ToString(),
                IsUnread = true,
                Message = message,
                Sent = DateTime.Now,
                To = "{you}"
            };
            _threads.Insert(thread);
            _messages.Insert(messageObject);
            _unread = _messages.Find(x => x.IsUnread).Count();
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

        public void UnlockCountry(Country country)
        {
            SetValue($"{country.ToString().ToLower()}.unlocked", true);
        }

        public void CompleteMission(string missionID)
        {
            if (IsMissionComplete(missionID))
                return;
            SetValue($"m.{missionID}.complete", true);
            MissionCompleted?.Invoke(missionID);
        }

        public void InstallPackage(string packageID)
        {
            SetValue($"package.{packageID}.installed", true);
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

        private string _baseDirectory = null;

        private string mapPath(string path)
        {
            while (path.EndsWith("/"))
                path = path.Remove(path.LastIndexOf("/"), 1);
            while (path.StartsWith("/"))
                path = path.Remove(0, 1);
            if (path.Contains("../") || path.Contains("/.."))
                throw new FormatException("This path has not properly been resolved from a relative path to an absolute path. Absolute paths cannot contain '..' or '.' in any component of the path (i.e /home/user/../user2. This is to prevent a possible sandbox breach as this filesystem backend interacts directly with the player's REAL filesystem and you can easily break out of the fake FS by requesting to read /.. (i.e, one level up from root). This is not permitted. Nice try, l337 h4xx0r.");
            return Path.Combine(_baseDirectory, path.Replace('/', Path.AltDirectorySeparatorChar));

        }

        public void CreateDirectory(string path)
        {
            if (DirectoryExists(path))
                throw new IOException("Directory exists");
            Directory.CreateDirectory(mapPath(path));
        }

        public void Delete(string path)
        {
            if (FileExists(path))
                File.Delete(mapPath(path));
            else if (DirectoryExists(path))
                Directory.Delete(mapPath(path), true);
            else
                throw new IOException("File or directory was not found.");
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(mapPath(path));
        }

        public bool FileExists(string path)
        {
            return File.Exists(mapPath(path));
        }

        public string[] GetDirectories(string path)
        {
            if (!DirectoryExists(path))
                throw new IOException("Directory not found.");
            List<string> dirs = new List<string>();
            foreach (var dir in Directory.GetDirectories(mapPath(path)))
            {
                var name = Path.GetFileName(dir);
                if (path.EndsWith("/"))
                    dirs.Add(path + name);
                else
                    dirs.Add(path + "/" + name);
            }
            return dirs.ToArray();
        }

        public FileRecord GetFileRecord(string path)
        {
            if(FileExists(path))
            {
                var finf = new FileInfo(mapPath(path));
                return new FileRecord
                {
                    IsDirectory = false,
                    Name = finf.Name,
                    SizeBytes = finf.Length
                };
            }
            else if(DirectoryExists(path))
            {
                var dinf = new DirectoryInfo(mapPath(path));
                return new FileRecord
                {
                    IsDirectory = true,
                    Name = dinf.Name,
                    SizeBytes = 0
                };
            }
            throw new IOException("Directory or file not found.");
        }

        public string[] GetFiles(string path)
        {
            if (!DirectoryExists(path))
                throw new IOException("Directory not found.");
            List<string> dirs = new List<string>();
            foreach (var dir in Directory.GetFiles(mapPath(path)))
            {
                var name = Path.GetFileName(dir);
                if (path.EndsWith("/"))
                    dirs.Add(path + name);
                else
                    dirs.Add(path + "/" + name);
            }
            return dirs.ToArray();

        }

        public void Initialize()
        {
            _baseDirectory = Path.Combine(_os.SinglePlayerSaveDirectory, "rootfs");
        }

        public Stream Open(string path, OpenMode mode)
        {
            if (!FileExists(path) && mode != OpenMode.OpenOrCreate)
                throw new IOException("File not found.");
            return File.Open(mapPath(path), (mode == OpenMode.Open) ? System.IO.FileMode.Open : System.IO.FileMode.OpenOrCreate);
        }

        public byte[] ReadAllBytes(string path)
        {
            using (var fstream = Open(path, OpenMode.Open))
            {
                byte[] data = new byte[fstream.Length];
                fstream.Read(data, 0, data.Length);
                return data;
            }
        }

        public void Unload()
        {
        }

        public void WriteAllBytes(string path, byte[] data)
        {
            using (var fstream = Open(path, OpenMode.OpenOrCreate))
            {
                fstream.Write(data, 0, data.Length);
            }
        }
    }
}
