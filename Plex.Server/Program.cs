using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Plex.Objects;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using WatercolorGames.CommandLine;

namespace Plex.Server
{
    public class Program
    {
        private static TcpListener _tcpListener = null;

        public enum ServerThreadState
        {
            Waiting,
            Active,
            Stopped
        }

        public class ServerThread
        {
            private Thread _thread = null;
            private Queue<Action> _messageQueue = new Queue<Action>();
            private EventWaitHandle _messageQueueHasMessages = new AutoResetEvent(false);
            private ServerThreadState __state = ServerThreadState.Stopped;
            private EventWaitHandle _stateChanged = new AutoResetEvent(false);
            private ServerThreadState _state
            {
                get
                {
                    return __state;
                }
                set
                {
                    __state = value;
                    _stateChanged.Set();
                }
            }

            public ServerThread()
            {
                _thread = new Thread(() =>
                {
                    while(true)
                    {
                        _state = ServerThreadState.Waiting;
                        _messageQueueHasMessages.WaitOne();
                        lock (_messageQueue)
                        {
                            if (_messageQueue.Count > 0)
                            {
                                _state = ServerThreadState.Active;
                                _messageQueue.Dequeue().Invoke();
                            }
                        }
                    }
                });
            }

            public int Count
            {
                get
                {
                    return _messageQueue.Count;
                }
            }

            public ServerThreadState State
            {
                get
                {
                    return _state;
                }
            }

            public void Queue(Action action)
            {
                if (action != null)
                {
                    lock (_messageQueue)
                        _messageQueue.Enqueue(action);
                    _messageQueueHasMessages.Set();
                }
            }

            public void Start()
            {
                if(_state == ServerThreadState.Stopped)
                {
                    _thread.Start();
                }
                else
                {
                    throw new Exception("The thread is already running.");
                }
            }

            public void Stop()
            {
                if (_state == ServerThreadState.Stopped)
                    throw new Exception("The thread is not running.");
                while (_state == ServerThreadState.Active)
                    _stateChanged.WaitOne();
                _thread.Abort();
                _state = ServerThreadState.Stopped;
            }
        }

        private static List<ServerThread> threads = new List<ServerThread>(Environment.ProcessorCount);

        private static readonly string[] NATOCodeNames = { "alfa", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india", "juliett", "kilo", "lima", "mike", "november", "oscar", "papa", "quebec", "romeo", "sierra", "tango", "uniform", "victor", "whiskey", "xray", "yankee", "zulu" };
        private const int _MyPort = 3251;
        internal static bool IsMultiplayerServer = true;
        public static List<string> BannedIPs = new List<string>();

        [ServerMessageHandler(ServerMessageType.U_CONF)]
        public static byte UConf(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            writer.Write(IsMultiplayerServer);
            return 0x00;
        }

        public static World GameWorld = null;

        private static int _port = 3251;

        public static void SetServerPort(int port)
        {
            _port = port;
        }

        public static void StartFromClient(string[] args, bool isMP)
        {
            IsMultiplayerServer = isMP;
            Main(args);
        }

        public static event Action ServerStarted;

        public static void LoadWorld()
        {
            if (!File.Exists("world.whoa"))
            {
                var world = new World();
                if (UpgradeManager.Upgrades == null)
                    UpgradeManager.Initiate();
                world.MaxRank = UpgradeManager.Upgrades.OrderBy(x => x.Rank).Last().Rank;

                var rogue = new Subnet();
                rogue.Name = "main";
                rogue.FriendlyName = "Peacenet Central Hub";
                rogue.AvailableUpgrades = new List<string>();
                rogue.FriendlyDescription = "The central hub for the Peacenet which manages economy, society and government.";
                rogue.WorldX = 0;
                rogue.WorldY = 0;
                rogue.NPCs = new List<HackableSystem>();
                var bank = GenerateSystem(world.MaxRank, SystemType.Bank, "centralbank");
                var repo = GenerateSystem(world.MaxRank, SystemType.Bank, "centralupgraderepo");
                var worker = GenerateSystem(world.MaxRank, SystemType.Computer, "centralwork");
                worker.X = 0;
                worker.Y = 0;
                bank.X = -100f;
                bank.Y = 0;
                repo.X = 100;
                repo.Y = 0;
                rogue.NPCs.Add(worker);
                rogue.NPCs.Add(bank);
                rogue.NPCs.Add(repo);
                world.Rogue = rogue;

                int maxnetcount = (world.MaxRank + 1)*5;
                world.Networks = new List<Subnet>();
                for(int i = 0; i <= world.MaxRank; i++)
                {
                    int netcount = (maxnetcount / (i + 1));
                    for (int j = 0; j < netcount; j++)
                    {
                        var net = new Subnet();
                        net.NPCs = new List<HackableSystem>();
                        NameGen:
                        string name = "";
                        for (int k = 0; k < 3; k++)
                        {
                            string nato = NATOCodeNames[rnd.Next(0, NATOCodeNames.Length)];
                            name += nato + "_";
                        }
                        if (name.EndsWith("_"))
                            name = name.Remove(name.Length - 1, 1);
                        if (world.Networks.FirstOrDefault(x => x.Name == name) != null)
                            goto NameGen;
                        net.Name = name;
                        net.FriendlyName = name + " subnet";

                        var nbank = GenerateSystem(i, SystemType.Bank, "netbank");
                        var nrepo = GenerateSystem(i, SystemType.UpgradeDB, "netupgrades");

                        var posForRepo = GetPositionFromRadius(new PointF(0, 0), (250 * (i + 2)), rnd.Next(360));
                        var posForBank = GetPositionFromRadius(new PointF(0, 0), (250 * (i + 2)), rnd.Next(360));

                        nbank.X = posForBank.X;
                        nbank.Y = posForBank.Y;
                        nrepo.X = posForRepo.X;
                        nrepo.Y = posForRepo.Y;

                        net.NPCs.Add(nrepo);
                        net.NPCs.Add(nbank);

                        var availableenums = Enum.GetNames(typeof(SystemType)).Where(x => x != "Bank" && x != "UpgradeDB").ToArray();
                        for (int k = 0; k <= i; k++)
                        {
                            int npccount = rnd.Next(((i + 1) * 5) / (k + 1));
                            for (int l = 0; l < npccount; l++)
                            {
                                string type = availableenums[rnd.Next(availableenums.Length)];
                                SystemType real = (SystemType)Enum.Parse(typeof(SystemType), type);
                                var npc = GenerateSystem(k, real, GenerateSystemName(net));
                                var loc = GetPositionFromRadius(new PointF(0, 0), (250) * (k + 1), rnd.Next(360));
                                npc.X = loc.X;
                                npc.Y = loc.Y;
                                net.NPCs.Add(npc);
                            }
                        }
                        world.Networks.Add(net);
                    }
                }
                GameWorld = world;
                SaveWorld();
            }
            else
            {
                try
                {
                    bool tryJSON = false;
                    Console.WriteLine("<worldgen> Loading from world.whoa...");
                    using (var fobj = File.OpenRead("world.whoa"))
                    {
                        using (var reader = new BinaryReader(fobj))
                        {
                            var magic = new byte[4];
                            magic = reader.ReadBytes(4);
                            if (magic.SequenceEqual(worldmagic))
                            {
                                int worldDataCount = reader.ReadInt32();
                                byte[] worldData = reader.ReadBytes(worldDataCount);
                                using (var memory = new MemoryStream(worldData))
                                {
                                    GameWorld = Whoa.Whoa.DeserialiseObject<World>(memory);
                                }
                            }
                            else
                            {
                                tryJSON = true;
                            }
                        }
                    }
                    if (tryJSON == true)
                    {
                        GameWorld = JsonConvert.DeserializeObject<World>(File.ReadAllText("world.whoa"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("<worldgen> Bad world file. Regenerating...");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.Data);
                    int i = 0;
                    string digit = "0";
                    while (File.Exists($"world.bak{digit}"))
                    {
                        i++;
                        if (i >= 16)
                        {
                            Console.WriteLine("<worldgen> Wow, that's a lot of bad world files.");
                            for (int j = 0; j < 16; j++)
                            {
                                digit = j.ToString("X");
                                File.Delete($"world.bak{digit}");
                            }
                            i = 0;
                            break;
                        }
                        digit = i.ToString("X");
                    }
                    File.Copy("world.whoa", $"world.bak{digit}");
                    File.Delete("world.whoa");
                    LoadWorld();
                }
            }
        }

        public static void SaveWorld()
        {
            using (var fobj = File.OpenWrite("world.whoa"))
            {
                using (var writer = new BinaryWriter(fobj))
                {
                    writer.Write(worldmagic);
                    byte[] worldbytes = null;
                    using (var memory = new MemoryStream())
                    {
                        Whoa.Whoa.SerialiseObject(memory, GameWorld);
                        worldbytes = memory.ToArray();
                    }
                    writer.Write(worldbytes.Length);
                    writer.Write(worldbytes);
                }
            }
        }

        [ServerCommand("worldinfo", "Displays world information.")]
        public static void WorldInfo()
        {
            Console.WriteLine("World info");
            Console.WriteLine("===============");
            Console.WriteLine();
            Console.WriteLine("Subnet count (excluding rogue): {0}", GameWorld.Networks.Count);
            List<HackableSystem> systems = new List<HackableSystem>();
            foreach (var net in GameWorld.Networks)
            {
                systems.AddRange(net.NPCs);
            }
            Console.WriteLine("Systems: {0}", systems.Count);
            foreach (SystemType stype in Enum.GetValues(typeof(SystemType)).Cast<SystemType>())
            {
                Console.WriteLine("  {0} of which are a {1}", systems.Where(x => x.SystemType == stype).Count(), stype);
            }
            long cash = 0;
            foreach (var system in systems)
            {
                cash += system.SystemDescriptor.Cash;
            }
            Console.WriteLine("This world is worth ${0}", ((double)cash / 100));
        }


        private static readonly byte[] worldmagic = Encoding.UTF8.GetBytes("wR1d");

        public static int AverageRank(List<HackableSystem> systems)
        {
            int rank = 0;
            foreach (var sys in systems)
                rank += sys.SystemDescriptor.Rank;
            return rank / systems.Count;
        }

        public static PointF GetPositionFromRadius(PointF centre, float radius, float degrees)
        {
            float x = (float)(centre.X + radius * Math.Cos(degrees * Math.PI / 180));
            float y = (float)(centre.Y + radius * Math.Sin(degrees * Math.PI / 180));
            return new PointF(x, y);
        }


        private const string letters = "abcdefghijklmnopqrstuvwxyz";

        public static string GenerateSystemName(Subnet subnet)
        {
            string sysname = null;
            while (sysname == null || subnet.NPCs.FirstOrDefault(x => x.SystemDescriptor.SystemName == sysname) != null)
            {
                string nato = NATOCodeNames[rnd.Next(0, NATOCodeNames.Length)];
                char c = letters[rnd.Next(letters.Length)];
                int num = rnd.Next(0, 9);
                sysname = $"{nato}-{c}{num}";
            }
            return sysname;
        }

        public static Subnet CreateSubnet(string name, string friendlyname, string friendlydescription)
        {
            var subnet = new Subnet();

            subnet.Name = name;
            subnet.FriendlyName = friendlyname;
            subnet.FriendlyDescription = friendlydescription;
            subnet.NPCs = new List<HackableSystem>();

            return subnet;
        }

        static Random rnd = new Random();

        public static HackableSystem GenerateSystem(int rank, SystemType type, string sysname)
        {
            Console.WriteLine("<worldgen> Creating hackable...");
            var hackable = new HackableSystem();
            hackable.IsNPC = true;
            Console.WriteLine("<worldgen> Creating system descriptor...");
            var save = new Save();
            save.SystemName = sysname;
            Console.WriteLine("<worldgen> System name: " + sysname);
            save.Rank = rank;
            Console.WriteLine("<worldgen> Rank: " + rank);

            if (rank == 1000)
            {
                save.Cash = 0;
                save.Experience = long.MaxValue;
                save.MaxLoadedUpgrades = int.MaxValue;
            }
            save.PickupPoint = "";
            save.StoriesExperienced = new List<string>();
            save.Transactions = new List<CashTransaction>();
            save.Upgrades = new Dictionary<string, bool>();
            save.LoadedUpgrades = new List<string>();
            save.NetworkTasks = new Dictionary<string, long>();
            hackable.SystemDescriptor = save;
            hackable.IsPwn3d = false;
            hackable.SystemType = type;
            hackable.Filesystems = new List<MountInformation>();
            Console.WriteLine("<worldgen> System type: {0}", type);
            return hackable;
        }

        /// <summary>
        /// Gets the hackable at the specified Plexnet Resource Link (net.sysname).
        /// </summary>
        /// <param name="prl"></param>
        /// <returns></returns>
        public static HackableSystem GetSaveFromPrl(string prl)
        {
            string[] split = prl.Split('.');
            return GameWorld.Networks.FirstOrDefault(x => x.Name == split[0])?.NPCs.FirstOrDefault(x => x.SystemDescriptor.SystemName == split[1]);
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static Subnet GetRandomSubnet()
        {
            return GameWorld.Networks[rnd.Next(0, GameWorld.Networks.Count)];
        }

        public static void Main(string[] args)
        {
            if (!Directory.Exists("userdrives"))
                Directory.CreateDirectory("userdrives");

            if (!IsMultiplayerServer)
            {
                ServerLoop();
            }
            else
            {
                Console.WriteLine("PROJECT: PLEX SERVER SOFTWARE - Copyright (c) 2017 Watercolor Games - Licensed under MIT");
                Console.WriteLine("===============================");
                Console.WriteLine();

                Console.WriteLine("Loading configuration....");

                if (System.IO.File.Exists("serverconfig.json"))
                {
                    _conf = JsonConvert.DeserializeObject<ServerConfiguration>(File.ReadAllText("serverconfig.json"));
                    Console.WriteLine("Configuration loaded.");
                }
                else
                {
                    Console.WriteLine("No config found.");
                    Console.WriteLine("Name your server: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("Discord payload URL (leave blank if you don't want chat to go through discord)");
                    string chaturl = Console.ReadLine();
                    Console.WriteLine("Saving config to serverconfig.json.");
                    _conf = new ServerConfiguration
                    {
                        DiscordPayloadURL = chaturl,
                        ServerName = name
                    };
                    File.WriteAllText("serverconfig.json", JsonConvert.SerializeObject(_conf, Formatting.Indented));
                    Console.WriteLine("At any time you may edit this file to alter the server behaviour.");

                }


                LoadWorld();

                Console.WriteLine("Starting HTTP API on port 3253...");
                var httpThread = new ServerThread();
                httpThread.Queue(() =>
                {
                    new APIHTTPServer().listen();

                });
                httpThread.Start();

                Console.WriteLine("<plexsrv> Starting server...");
                var t = new System.Threading.Thread(ServerLoop);
                t.Start();
                Console.WriteLine("Server thread running and listening for requests.");
                Console.WriteLine("Reading banned IP addresses...");
                if (System.IO.File.Exists("banned-ips.json"))
                {
                    BannedIPs = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText("banned-ips.json"));
                }
                Console.WriteLine("{0} IP addresses have been banned.", BannedIPs.Count);
                Console.WriteLine("Starting server shell. Type 'help' for a list of commands.");
                Terminal.Populate();
                while (true)
                {
                    Console.Write("> ");
                    string cmd = Console.ReadLine();
                    if (cmd == "exit")
                    {
                        Console.WriteLine("<worldgen> Saving world...");
                        SaveWorld();
                        Environment.Exit(-1);
                    }
                    else
                    {
                        try
                        {
                            
                            string[] argv = Tokenizer.TokenizeString(cmd);
                            if (argv.Length > 0)
                            {
                                string cmdname = argv[0];
                                string[] argsarr = new string[0];
                                if(argv.Length > 1)
                                {
                                    var alist = argv.ToList();
                                    alist.RemoveAt(0);
                                    argsarr = alist.ToArray();
                                }
                                if (!Terminal.RunClient(cmdname, argsarr, "", (StreamWriter)Console.Out, (StreamReader)Console.In, true))
                                {
                                    Console.WriteLine("Command not found.");
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("Parse error: " + ex.Message);
                        }
                    }
                }
            }

        }

        [ServerCommand("banip", "Ban an IP address from this server.", true)]
        [UsageString("<ipaddr>")]
        public static void BanIP(Dictionary<string, object> args)
        {
            string ip = args["<ipaddr>"].ToString();
            if (BannedIPs.Contains(ip))
            {
                Console.WriteLine("This IP address is already banned.");
                return;
            }
            IPAddress _ban = null;
            if (IPAddress.TryParse(ip, out _ban) == false)
            {
                Console.WriteLine("Parse error: Input is not a valid IP address.");
                return;
            }

            BannedIPs.Add(ip);
            System.IO.File.WriteAllText("banned-ips.json", JsonConvert.SerializeObject(BannedIPs, Formatting.Indented));
            Console.WriteLine("The IP address {0} has been banned. Any attempts to query the server from this address will result in a 'server_banned' result. Banlist saved.");
        }

        public static void WorldManager()
        {

        }

        private static ServerConfiguration _conf = null;

        public static ServerConfiguration ServerConfig
        {
            get
            {
                return _conf ?? new ServerConfiguration();
            }
        }

        public static List<Connection> connections = new List<Connection>();

        public class Connection
        {
            public BinaryWriter Writer { get; set; }
            public BinaryReader Reader { get; set; }
            public TcpClient Client { get; set; }
        }

        public static void TcpLoop()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();
            ServerStarted?.Invoke();
            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                if(IsMultiplayerServer)
                {
                    if(connections.Count + 1 > ServerConfig.MaxPlayers)
                    {
                        client.Close();
                        continue;
                    }
                }
                var connectThread = new ServerThread();
                if(IsMultiplayerServer)
                    Console.WriteLine($"{client.Client.LocalEndPoint} has connected through TCP.");
                connectThread.Queue(() =>
                {
                    try
                    {
                        var stream = client.GetStream();
                        var reader = new BinaryReader(stream, Encoding.UTF8, true);
                        var writer = new BinaryWriter(stream, Encoding.UTF8, true);
                        var connection = new Connection
                        {
                            Client = client,
                            Writer = writer,
                            Reader = reader
                        };
                        connections.Add(connection);
                        while (client.Connected)
                        {
                            var muid = reader.ReadString();
                            var _messagetype = reader.ReadInt32();
                            
                            string session_id = reader.ReadString();
                            byte[] content = new byte[] { };
                            int contentLength = reader.ReadInt32();
                            if (contentLength > 0)
                                content = reader.ReadBytes(contentLength);
                            
                            using (var mstr = new MemoryStream())
                            {
                                byte r = 0x00;
                                using (var bwriter = new BinaryWriter(mstr, Encoding.UTF8, true))
                                {
                                    r = ServerManager.HandleTcpMessage(new PlexServerHeader
                                    {
                                        Message = (byte)_messagetype,
                                        SessionID = session_id,
                                        Content = content
                                    }, bwriter);

                                }
                                writer.Write(muid);
                                writer.Flush();
                                writer.Write((int)r);
                                writer.Flush();
                                writer.Write(session_id);
                                writer.Flush();
                                byte[] bc = mstr.ToArray();
                                writer.Write((int)bc.Length);
                                writer.Flush();
                                if (bc.Length > 0)
                                    writer.Write(bc);
                                writer.Flush();
                            }
                        }
                        if (IsMultiplayerServer)
                            Console.WriteLine($"{client.Client.LocalEndPoint} has disconnected from TCP.");
                        connections.Remove(connection);
                    }
                    catch { }
                });
                connectThread.Start();
            }
        }

        public static void ServerLoop()
        {
            Console.WriteLine("Loading upgrade data...");
            UpgradeManager.Initiate();
            Console.WriteLine("Done.");

            ServerManager.LocateHandlers();
            Console.WriteLine("Validating npc/player filesystems...");
            bool requireSave = false;
            foreach (var net in GameWorld.Networks)
            {
                foreach (var npc in net.NPCs)
                {
                    if (npc.Filesystems == null)
                    {
                        npc.Filesystems = new List<MountInformation>();
                        requireSave = true;
                    }
                }
            }

            if (requireSave)
                SaveWorld();

            var worldThread = new ServerThread();
            Console.WriteLine("Starting world manager thread...");
            worldThread.Queue(WorldManager);
            worldThread.Start();

            var tcpThread = new ServerThread();
            Console.WriteLine("Starting TCP listener...");
            TcpLoop();
        }

        /// <summary>
        /// Block the current thread for the specified amount of time.
        /// </summary>
        /// <param name="span">The amount of time to block.</param>
        public static void Sleep(TimeSpan span)
        {
            Thread.Sleep((int)span.TotalMilliseconds);
        }

        [ServerCommand("broadcast_test", "Broadcasts a test announcement")]
        public static void BroadcastTest()
        {
            using(var bstr = new BroadcastStream(BroadcastType.SRV_ANNOUNCEMENT))
            {
                bstr.Write("Test announcement");
                bstr.Write("This is a test announcement.");
                bstr.Send();
            }
            Console.WriteLine("Broadcast sent.");
        }

        public static void Broadcast(BroadcastType type, byte[] content)
        {
            foreach (var client in connections)
            {
                client.Writer.Write("broadcast");
                client.Writer.Write((int)type);
                client.Writer.Write(content.Length);
                if (content.Length > 0)
                    client.Writer.Write(content);
            }
        }

    }


    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    internal static class ServerManager
    {
        private static Dictionary<ServerMessageType, MethodInfo> _handlers = new Dictionary<ServerMessageType, MethodInfo>();


        internal static void LocateHandlers()
        {
            _handlers.Clear();
            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is ServerMessageHandlerAttribute) != null))
                {
                    var attribute = method.GetCustomAttributes(false).FirstOrDefault(x => x is ServerMessageHandlerAttribute) as ServerMessageHandlerAttribute;
                    if (!_handlers.ContainsKey(attribute.ID))
                    {
                        _handlers.Add(attribute.ID, method);
                        Console.WriteLine("{0}: {1}", attribute.ID, method.Name);
                        
                    }
                    else
                    {
                        throw new InvalidOperationException("Two or more message handlers were found with the same ID: " + attribute.ID + " - This is not allowed.");
                    }
                }
            }

        }



        internal static byte HandleTcpMessage(PlexServerHeader header, BinaryWriter tcpStreamWriter)
        {
            if (_handlers.ContainsKey((ServerMessageType)header.Message))
            {
                var method = _handlers[(ServerMessageType)header.Message];

                var sessionRequired = method.GetCustomAttributes(false).FirstOrDefault(x => x is SessionRequired) as SessionRequired;
                if (sessionRequired != null)
                {
                    bool nosession = string.IsNullOrWhiteSpace(header.SessionID);
                    if (nosession == false)
                    {
                        nosession = SessionManager.IsExpired(header.SessionID);
                    }

                    if (nosession)
                    {
                        return (byte)ServerResponseType.REQ_LOGINREQUIRED;
                    }
                    else
                    {
                        var acct = SessionManager.GrabAccount(header.SessionID);
                        if (acct.IsBanned)
                        {
                            tcpStreamWriter.Write(acct.BanLiftDate.ToString());

                            return (byte)ServerResponseType.REQ_BANNED;
                        }
                    }
                }
                using (var mstr = new MemoryStream(header.Content))
                {
                    using (var reader = new BinaryReader(mstr, Encoding.UTF8, true))
                    {
                        var result = method.Invoke(null, new object[] { header.SessionID, reader, tcpStreamWriter });
                        if(result == null)
                        {
                            System.Diagnostics.Debug.Print($"[server] [WARN] {method.Name} returns void. Byte required.");
                            return (byte)ServerResponseType.REQ_ERROR;
                        }
                        return (byte)result;
                    }
                }
            }
            else
            {
                return (byte)ServerResponseType.REQ_ERROR;
            }
        }


    }

    public class BroadcastStream : BinaryWriter
    {
        public BroadcastType Type { get; private set; }

        public BroadcastStream(BroadcastType type) : base(new MemoryStream())
        {
            Type = type;
        }

        public void Send()
        {
            Program.Broadcast(Type, (BaseStream as MemoryStream).ToArray());
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SessionRequired : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerMessageHandlerAttribute : Attribute
    {
        public ServerMessageHandlerAttribute(ServerMessageType id)
        {
            ID = id;
        }

        public ServerMessageType ID { get; private set; }
    }


    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            try
            {
                outputStream.Flush();
            }
            catch { }// bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writePngImage(byte[] data)
        {
            outputStream.Write("HTTP/1.0 200 OK\r\n");
            outputStream.Write("Content-Type: image/bmp\r\n");
            outputStream.Write("Content-Length: " + data.Length + "\r\n");
            outputStream.Write("Connection: close\r\n");
            outputStream.Write("\r\n");
            outputStream.Flush();
            for (int i = 0; i < data.Length; i++)
                outputStream.BaseStream.WriteByte(data[i]);
        }

        public void writeSuccess(string content_type = "text/html")
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }

    public abstract class HttpServer
    {

        protected int port;
        TcpListener listener;
        bool is_active = true;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public void listen()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public sealed class APIHTTPServer : HttpServer
    {
        public APIHTTPServer() : base(3253)
        {

        }

        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine(p.http_url);
            string[] split = p.http_url.Remove(0, 1).Split('/');
            if (split.Length > 0)
            {
                string first = split[0];
                switch (first.ToLower())
                {
                    case "servericon":
                        if (System.IO.File.Exists("servericon.bmp"))
                        {
                            p.writePngImage(File.ReadAllBytes("servericon.bmp"));
                            return;
                        }
                        using(var memstr = new MemoryStream())
                        {
                            Properties.Resources.server.Save(memstr, System.Drawing.Imaging.ImageFormat.Bmp);
                            p.writePngImage(memstr.ToArray());
                        }
                        return;
                    case "serverinfo":
                        string json = JsonConvert.SerializeObject(new
                        {
                            server_name = Program.ServerConfig?.ServerName,
                            online_players = Program.connections.Count,
                            max_players = Program.ServerConfig?.MaxPlayers
                        });
                        p.writeSuccess("application/json");
                        p.outputStream.Write(json);
                        return;
                    case "avatar":
                        try
                        {
                            string username = split[1];
                            var session = SessionManager.GetSessions().FirstOrDefault(x => x.Username == username);
                            if(session != null)
                            {
                                p.writePngImage(Avatars.GetAvatar(session.Username));
                            }
                        }
                        catch { }
                        break;
                }
            }
            p.writeFailure();
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            
        }
    }

    public static class Avatars
    {
        public class AvatarCache
        {
            public string Username { get; set; }
            public byte[] Data { get; set; }
            public DateTime Expiry { get; set; }
        }

        private static readonly List<AvatarCache> caches = new List<AvatarCache>();

        public static byte[] GetAvatar(string username)
        {
            if (!System.IO.Directory.Exists("avatars"))
                System.IO.Directory.CreateDirectory("avatars");

            var cache = caches.FirstOrDefault(x => x.Username == username);
            bool cacheRedo = cache == null;
            if (!cacheRedo)
                cacheRedo = DateTime.Now > cache.Expiry;
            if (cacheRedo)
            {
                string path = Path.Combine("avatars", username + ".png");
                if(File.Exists(path))
                {
                    cache = new AvatarCache
                    {
                        Username = username,
                        Data = File.ReadAllBytes(path),
                        Expiry = DateTime.Now.AddMinutes(5)
                    };
                    caches.Add(cache);
                }
                else
                {
                    byte[] imgdata = null;
                    using (var memstr = new MemoryStream())
                    {
                        Properties.Resources.user.Save(memstr, System.Drawing.Imaging.ImageFormat.Bmp);
                        imgdata = memstr.ToArray();
                    }
                    cache = new AvatarCache
                    {
                        Username = username,
                        Data = imgdata,
                        Expiry = DateTime.Now.AddMinutes(5)
                    };
                    File.WriteAllBytes(path, imgdata);
                }

            }
            return cache.Data;
        }
    }

}
