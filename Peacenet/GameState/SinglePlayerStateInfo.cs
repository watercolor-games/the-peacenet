using LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Peacenet.Applications;
using Peacenet.CoreUtils;
using Peacenet.Email;
using Peacenet.Filesystem;
using Peacenet.WorldGenerator;
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
using System.Threading;
using System.Threading.Tasks;

namespace Peacenet.GameState
{
    public class SinglePlayerStateInfo : IGameStateInfo, IEntity, ISaveBackend
    {
        private float _alertLevel = 0f;
        private bool _alertFalling = false;

        //Amount of upgrade slots granted to a Skill Level 0 player
        private const int _baseUpgradeSlots = 5;
        //Amount of additional upgrade slots granted per skill level.
        private const int _upgradesPerSkillLevel = 3;

        public float AlertLevel => _alertLevel;
        public bool AlertFalling => _alertFalling;

        public Country CurrentCountry => GetValue("country.current", Country.Elytrose);
        public bool IsCountryUnlocked(Country country)
        {
            return GetValue($"country.{country.ToString().ToLower()}.unlocked", (country == Country.Elytrose) ? true : false);
        }

        public void SwitchCountry(Country country)
        {
            if (!IsCountryUnlocked(country)) return;
            SetValue("country.current", country);

            EnsureCountryDataExists();
        }

        private int[] _npcTypeMap = null;

        private void LoadCountryData(Country country)
        {
            if (_countryTexture != null)
            {
                _countryTexture.Dispose();
                _countryTexture = null;
            }

            _npcTypeMap = null;

            string terrainID = $"{country.ToString().ToLower()}_terrain";

            if (_saveDB.FileStorage.Exists(terrainID))
            {
                using (var s = _saveDB.FileStorage.OpenRead(terrainID))
                {
                    using (var r = new BinaryReader(s))
                    {
                        uint[] u = new uint[r.ReadInt32()];
                        for (int i = 0; i < u.Length; i++)
                            u[i] = r.ReadUInt32();
                        _countryTexture = new Texture2D(_GameLoop.GraphicsDevice, 512, 512);
                        _countryTexture.SetData(u);
                        _npcTypeMap = new int[r.ReadInt32()];
                        for (int i = 0; i < _npcTypeMap.Length; i++)
                            _npcTypeMap[i] = r.ReadInt32();
                    }
                }
            }

            int playerSlot = Array.IndexOf(_npcTypeMap, 8);
            var player = _sentiences.FindOne(x => x.Id == $"{country}_player");
            if (player != null)
            {
                player.Hostname = _os.Hostname;
                player.XP = this.TotalXP;
                player.SkillLevel = _level;
                _sentiences.Update(player);
            }
            else
            {
                var loc = getMapLoc(playerSlot);
                player = new Sentience
                {
                    FactionID = null,
                    Hostname = _os.Hostname,
                    Country = country,
                    Id = $"{country}_player",
                    IPAddress = genIP(country),
                    X = loc.X,
                    Y = loc.Y,
                    Reputation = 0,
                    SkillLevel = _level,
                    XP = TotalXP
                };
                _sentiences.Insert(player);
            }

            Task.Run(() =>
            {
                var added = new ManualResetEvent(false);

                List<NPCDefinition> _def = new List<NPCDefinition>();
                for (int i = 0; i < _npcTypeMap.Length; i++)
                {
                    if (_npcTypeMap[i] > 0 && _npcTypeMap[i] < 8)
                        _def.Add(new NPCDefinition(i, _npcTypeMap[i]));
                }

                var storyQueue = new Queue<StoryNPC>(_storyNPCs.Where(x => x.Country == CurrentCountry));

                foreach (var npc in _def.OrderBy(x => x.Type))
                {
                    added.Reset();

                    if (_sentiences.FindOne(x => x.Id == npc.Index.ToString() && x.Country == country) != null)
                        continue;
                    StoryNPC story = null;
                    double rep = 0;
                    switch (npc.Type)
                    {
                        case 1:
                            story = storyQueue.Dequeue();
                            rep = story.Reputation;
                            break;
                        case 2:
                        case 3:
                            rep = -_rnd.NextDouble();
                            break;
                        case 6:
                        case 7:
                            rep = _rnd.NextDouble();
                            break;
                    }

                    var nloc = getMapLoc(npc.Index);
                    if (npc.Type == 1)
                    {
                        int skill = story.SkillLevel;
                        int min = _levels[skill];
                        int max = (skill + 1 >= _levels.Length) ? min + (min / 2) : _levels[skill + 1] - 1;
                        int xp = _rnd.Next(min, max);

                        var npcData = new Sentience
                        {
                            Id = npc.Index.ToString(),
                            Country = country,
                            FactionID = null,
                            Hostname = story.Hostname,
                            IPAddress = genIP(country),
                            Reputation = story.Reputation,
                            SkillLevel = story.SkillLevel,
                            XP = xp,
                            X = nloc.X,
                            Y = nloc.Y
                        };
                        var hackable = new Hackable
                        {
                            Credentials = story.Credentials ?? new Dictionary<string, string>(),
                            Loot = story.Loot ?? new string[0],
                            Dependencies = story.Dependencies ?? new string[0],
                            GovernmentAlert = story.GovernmentAlert,
                            Id = npcData.Id
                        };
                        _GameLoop.Invoke(() =>
                        {
                            Plex.Objects.Logger.Log($"Spawning {country} NPC '{npcData.Id} at {npcData.MapLocation}");
                            _sentiences.Insert(npcData);
                            _hackables.Insert(hackable);
                            added.Set();
                        });
                        added.WaitOne();
                    }
                    else if (npc.Type % 2 == 0)
                    {
                        int skill = _rnd.Next(_levels.Length);
                        int min = _levels[skill];
                        int max = (skill + 1 >= _levels.Length) ? min + (min / 2) : _levels[skill + 1] - 1;
                        int xp = _rnd.Next(min, max);
                        //singular sentience.

                        var npcData = new Sentience
                        {
                            Id = npc.Index.ToString(),
                            Country = country,
                            FactionID = null,
                            Hostname = null,
                            IPAddress = genIP(country),
                            Reputation = (float)rep,
                            SkillLevel = skill,
                            XP = xp,
                            X = nloc.X,
                            Y = nloc.Y
                        };
                        _GameLoop.Invoke(() =>
                        {
                            Plex.Objects.Logger.Log($"Spawning {country} NPC '{npcData.Id} at {npcData.MapLocation}");
                            _sentiences.Insert(npcData);
                            added.Set();
                        });
                        added.WaitOne();
                    }
                    else
                    {
                        _GameLoop.Invoke(() =>
                        {
                            Plex.Objects.Logger.Log("Skipping spawn of NPC '" + npc.Index + ". Factions are NYI.");
                        });
                    }
                }
                Plex.Objects.Logger.Log("Done spawning NPCs for " + country + ".");
            });
        }

        public Sentience Player => _sentiences.FindOne(x => x.Id == $"{CurrentCountry}_player");
        public IEnumerable<Sentience> SingularSentiences => _sentiences.Find(x => x.Country == CurrentCountry && x.FactionID == null);
        public IEnumerable<Faction> Factions => _factions.Find(x => x.Country == CurrentCountry);
        public IEnumerable<Sentience> GetSentiences(Faction faction)
        {
            if (faction == null)
                return null;
            return _sentiences.Find(x => x.FactionID == faction.Id);
        }

        private Random _rnd = new Random();

        private Vector2 getMapLoc(int index)
        {
            return new Vector2((float)index % 512, (float)index / 512);
        }

        private uint genIP(Country country)
        {
            byte cByte1 = (byte)((255 / 2) - (64 / ((int)country + 1)));
            byte cByte2 = (byte)((255 / 2) + (16 * ((int)country + 1)));
            byte eByte1 = (byte)_rnd.Next(255);
            byte eByte2 = (byte)_rnd.Next(255);

            return (uint)((cByte1 << 24) + (cByte2 << 16) + (eByte1 << 8) + eByte2);
        }

        public bool IsUpgradeInstalled(string upgradeID)
        {
            return GetValue($"upgrade.{upgradeID}.enabled", false);
        }

        public IEnumerable<string> UpgradeIDs => _upgrades.Select(x => x.Id);

        public bool DisableUpgrade(string upgradeID)
        {
            if (!IsUpgradeInstalled(upgradeID))
                return false;
            var children = _upgrades.Where(x => x.Dependencies != null && x.Dependencies.Contains(upgradeID) && IsUpgradeInstalled(x.Id));
            foreach (var upgrade in children)
                DisableUpgrade(upgrade.Id);
            SetValue($"upgrade.{upgradeID}.enabled", false);
            return true;
        }

        public bool EnableUpgrade(string upgradeID)
        {
            if (IsUpgradeInstalled(upgradeID))
                return false;
            var children = _upgrades.Where(x => x.Dependencies != null && x.Dependencies.Contains(upgradeID) && !IsUpgradeInstalled(x.Id));
            if (children.Count() > 0)
                return false;
            int enabledCount = _upgrades.Where(x => IsUpgradeInstalled(x.Id)).Count();
            if (enabledCount + 1 > _maxUpgradeSlots)
                return false;
            SetValue($"upgrade.{upgradeID}.enabled", true);
            return true;
        }

        public Upgrade GetUpgradeInfo(string upgradeID)
        {
            return _upgrades.FirstOrDefault(x => x.Id == upgradeID);
        }

        private int _level = 0;
        private float _levelPercentage = 0;

        public int SkillLevel => _level;
        public float SkillLevelPercentage => _levelPercentage;


        private double _timeInAlert = 0;
        private float _lastAlert = 0f;

        public float GameCompletion { get => GetValue("game.completion", 0f); set => SetValue("game.completion", value); }

        public float Reputation { get => GetValue("player.rep", 0f); set => SetValue("player.rep", value); }

        public event Action<string> MissionCompleted;

        public int TotalXP => GetValue<int>("sys.xp", 0);

        public void AddXP(int xp)
        {
            SetValue("sys.xp", TotalXP + xp);
            updateSkillLevel();
        }

        private int _maxUpgradeSlots = 0;

        public int UpgradeSlotCount => _maxUpgradeSlots;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private GUIUtils _gui = null;

        public void Highlight(string hostname)
        {
            _highlighted = SingularSentiences.FirstOrDefault(x => x.Hostname == hostname);
        }


        private int _unread = 0;

        private Sentience _highlighted = null;

        public Sentience Highlighted => _highlighted;

        private Upgrade[] _upgrades = null;

        private LiteDatabase _saveDB = null;

        private List<Connection> _connections = new List<Connection>();

        private LiteCollection<Snapshot> _snapshots = null;
        private LiteCollection<SaveValue> _values = null;
        private LiteCollection<EmailThread> _threads = null;
        private LiteCollection<EmailMessage> _messages = null;
        private LiteCollection<Hackable> _hackables = null;
        private StoryNPC[] _storyNPCs = null;
        private int[] _levels = null;

        public SinglePlayerStateInfo()
        {
        }

        public IEnumerable<Connection> ActiveConnections => _connections;

        public bool StartConnection(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return false;
            var sentience = _sentiences.FindOne(x => x.Country == CurrentCountry && (x.Hostname == host || x.IPAddress.ToIPv4String() == host));
            if (sentience == null)
                return false;
            var hackable = _hackables.FindOne(x => x.Id == sentience.Id);
            if (hackable == null)
                return false;

            var connection = new Connection(this, hackable, sentience);

            _connections.Add(connection);

            return true;
        }

        public bool EndConnection(string host)
        {
            var connection = _connections.FirstOrDefault(x => x.Sentience.Hostname == host || x.Sentience.IPAddress.ToIPv4String() == host);
            if (connection == null)
                return false;
            connection.End();
            _connections.Remove(connection);
            return true;
        }

        private class NPCDefinition
        {
            public int Index { get; private set; }
            public int Type { get; private set; }

            public NPCDefinition(int index, int type)
            {
                Index = index;
                Type = type;
            }
        }

        private bool _doSafeExit = true;

        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        public void EndGame()
        {
            if (_doSafeExit)
            {
                _GameLoop.GetLayer(LayerType.NoDraw)?.RemoveEntity(this);
                _fs.SetBackend(null);
                _save.SetBackend(null);
            }
            _saveDB.Shrink();
            _saveDB.Dispose();
            _saveDB = null;
            _values = null;
            _snapshots = null;
            _threads = null;
            _messages = null;
            _hackables = null;
        }

        private Texture2D _countryTexture = null;

        public Texture2D CountryTexture => _countryTexture;

        public void OnGameExit()
        {
            _doSafeExit = false;
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
                        _gui.AskForFile(true, null, (path) =>
                        {
                            _fs.WriteAllBytes(path, data);
                        });
                    }
                }
            }
            #endif
        }

        private LiteCollection<Faction> _factions = null;
        private LiteCollection<Sentience> _sentiences { get; set; }

        private const string _seed = "PeacenetSingleplayer";

        public void StartGame()
        {
            _storyNPCs = JsonConvert.DeserializeObject<StoryNPC[]>(Properties.Resources.StoryNPCs);

            if (!Directory.Exists(_os.SinglePlayerSaveDirectory))
                Directory.CreateDirectory(_os.SinglePlayerSaveDirectory);
            _GameLoop.GetLayer(LayerType.NoDraw).AddEntity(this);
            _fs.SetBackend(_GameLoop.New<SinglePlayerFilesystem>());

            _saveDB = new LiteDatabase(Path.Combine(_os.SinglePlayerSaveDirectory, "savefile.db"));

            _values = _saveDB.GetCollection<SaveValue>("values");
            _snapshots = _saveDB.GetCollection<Snapshot>("snapshots");
            _values.EnsureIndex(x => x.ID);
            _snapshots.EnsureIndex(x => x.ID);

            _threads = _saveDB.GetCollection<EmailThread>("emailthreads");
            _messages = _saveDB.GetCollection<EmailMessage>("emailmessages");
            _hackables = _saveDB.GetCollection<Hackable>("hackables");

            _save.SetBackend(this);
            _os.EnsureProperEnvironment();

            _unread = _messages.Find(x => x.IsUnread).Count();

            if(_levels == null)
            {
                if(_saveDB.FileStorage.Exists("levels"))
                {
                    using (var s = _saveDB.FileStorage.OpenRead("levels"))
                    {
                        byte[] data = new byte[s.Length];
                        s.Read(data, 0, data.Length);
                        _levels = JsonConvert.DeserializeObject<int[]>(Encoding.UTF8.GetString(data));
                    }
                }
                else
                {
                    using (var s = _saveDB.FileStorage.OpenWrite("levels", "levels"))
                    {
                        var contentLevels = _GameLoop.Content.Load<int[]>("SkillLevels");
                        _levels = contentLevels;
                        string json = JsonConvert.SerializeObject(_levels);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        s.Write(data, 0, data.Length);
                    }
                }
            }

            //Upgrades are localizable and should NEVER be altered by the player, so we'll deserialize them from an embedded resource.
            _upgrades = JsonConvert.DeserializeObject<Upgrade[]>(Properties.Resources.Upgrades);

            updateSkillLevel();

            _factions = _saveDB.GetCollection<Faction>("factions");
            _sentiences = _saveDB.GetCollection<Sentience>("sentiences");

            EnsureCountryDataExists();
        }

        public bool SentienceHacked(string id)
        {
            return _save.GetValue($"sentience.{id}.hacked", false);
        }

        private void EnsureCountryDataExists()
        {
            //Do we have the Elytrose terrain map?
            if (!_saveDB.FileStorage.Exists($"{CurrentCountry.ToString().ToLower()}_terrain"))
            {
                var hmap = new Heightmap(Heightmap.GetSeed(_seed + "." + CurrentCountry.ToString().ToLower()));
                hmap.Width = 512;
                hmap.Height = 512;
                
                Task.Run(() =>
                {
                    _os.PreventStartup = true;

                    var m = hmap.Generate();
                    uint[] pdata = new uint[hmap.Width * hmap.Height];
                    for (int i = 0; i < pdata.Length; i++)
                    {
                        double h = m[i];
                        if (h > 0)
                        {
                            pdata[i] = uint.MaxValue;
                        }
                    }

                    var npc = new NPCMap(hmap.Seed, m);
                    npc.SpawnStoryNPCs = true;
                    npc.MinStoryNPCs = _storyNPCs.Where(x => x.Country == CurrentCountry).Count();
                    var densityMap = npc.GenerateDensityMap();
                    var typeMap = npc.GetTypeMap(densityMap);

                    _GameLoop.Invoke(() =>
                    {
                        using (var s = _saveDB.FileStorage.OpenWrite($"{CurrentCountry.ToString().ToLower()}_terrain", $"{CurrentCountry.ToString().ToLower()}_terrain"))
                        {
                            using (var w = new BinaryWriter(s, Encoding.UTF8))
                            {
                                w.Write(pdata.Length);
                                foreach (var i in pdata)
                                    w.Write(i);
                                w.Write(typeMap.Length);
                                foreach (var t in typeMap)
                                    w.Write(t);
                            }
                        }
                    
                        _os.PreventStartup = false;
                        LoadCountryData(CurrentCountry);
                    });

                });
            }
            else
            {
                LoadCountryData(CurrentCountry);
            }

        }

        private void updateSkillLevel()
        {
            int xp = TotalXP;
            int levelIndex = Array.IndexOf(_levels, _levels.Last(x => xp >= x));
            if (levelIndex + 1 >= _levels.Length)
            {
                //We're on the last skill level.
                _level = levelIndex + 1;
                _levelPercentage = 1f;
                return;
            }

            int levelStart = _levels[levelIndex];
            int levelEnd = _levels[levelIndex + 1];

            int rangeUpper = levelEnd - levelStart;
            int rangeLower = xp - levelStart;

            _level = levelIndex;
            _levelPercentage = (float)rangeLower / rangeUpper;

            _maxUpgradeSlots = _baseUpgradeSlots + (_upgradesPerSkillLevel * _level);
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
                this.sendNPCMail(npcMail.Subject, npcMail.From, npcMail.Message, npcMail.MissionID);
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

        private void sendNPCMail(string subject, string from, string message, string mission)
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
                To = "{you}",
                MissionID = mission
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
            if (DirectoryExists(path))
                throw new IOException("Is a directory.");
            if (!FileExists(path) && mode != OpenMode.OpenOrCreate)
                throw new IOException("File not found.");
            string dir = path.Substring(0, path.LastIndexOf("/"));
            if (!DirectoryExists(dir))
                CreateDirectory(dir);
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

    public class StoryNPC
    {
        public string Hostname { get; set; }
        public int SkillLevel { get; set; }
        public float Reputation { get; set; }
        public Country Country { get; set; }
        public string[] Dependencies { get; set; }
        public string[] Loot { get; set; }
        public Dictionary<string, string> Credentials { get; set; }
        public bool GovernmentAlert { get; set; } = true;
    }

    public class Hackable
    {
        public string Id { get; set; }
        public string[] Loot { get; set; }
        public string[] Dependencies { get; set; }
        public bool GovernmentAlert { get; set; }
        public Dictionary<string, string> Credentials { get; set; }
    }

    public class Connection : IAsyncFSBackend
    {
        private Hackable _hackable = null;
        private IGameStateInfo _state = null;

        public Connection(IGameStateInfo state, Hackable hackable, Sentience sentience)
        {
            Sentience = sentience;
            _hackable = hackable;
            _state = state;
        }

        public Sentience Sentience { get; private set; }
        public bool Authenticated { get; private set; } = false;

        public string AuthenticatedUsername { get; private set; }

        public bool Authenticate(string user, string pass)
        {
            if (!_hackable.Credentials.ContainsKey(user))
                return false;
            if (_hackable.Credentials[user] != pass)
                return false;
            Authenticated = true;
            AuthenticatedUsername = user;
            return true;
        }
        
        public void End()
        {

        }

        public void CreateDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public string[] GetDirectories(string path)
        {
            throw new NotImplementedException();
        }

        public FileRecord GetFileRecord(string path)
        {
            throw new NotImplementedException();
        }

        public string[] GetFiles(string path)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public Stream Open(string path, OpenMode mode)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadAllBytes(string path)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void WriteAllBytes(string path, byte[] data)
        {
            throw new NotImplementedException();
        }
    }


}
