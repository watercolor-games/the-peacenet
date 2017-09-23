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

namespace Plex.Server
{
    public class Program
    {
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
            private ServerThreadState _state = ServerThreadState.Stopped;

            public ServerThread()
            {
                _thread = new Thread(() =>
                {
                    while(true)
                    {
                        while (_messageQueue.Count == 0)
                        {
                            Thread.Sleep(2);
                            _state = ServerThreadState.Waiting;
                        }
                        _state = ServerThreadState.Active;
                        _messageQueue.Dequeue().Invoke();
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
                    _messageQueue.Enqueue(action);
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
                    Thread.Sleep(10);
                _thread.Abort();
                _state = ServerThreadState.Stopped;
            }
        }

        private static List<ServerThread> threads = new List<ServerThread>();
        private const int threadCount = 16;

        private static readonly string[] NATOCodeNames = { "alfa", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india", "juliett", "kilo", "lima", "mike", "november", "oscar", "papa", "quebec", "romeo", "sierra", "tango", "uniform", "victor", "whiskey", "xray", "yankee", "zulu" };
        private const int _MyPort = 3251;
        private static UdpClient _server = null;
        internal static bool IsMultiplayerServer = true;
        public static List<string> BannedIPs = new List<string>();

        public static List<Rank> Ranks { get; set; }

        public static World GameWorld = null;

        public static void SendMessage(PlexServerHeader header, int port)
        {
            var ip = IPAddress.Parse(header.IPForwardedBy);
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            var _ipEP = new IPEndPoint(ip, port);
            _server.Send(bytes, bytes.Length, _ipEP);
            if (IsMultiplayerServer)
                Console.WriteLine("<server> me -> {0}: {1} (session id: \"{2}\", content: {3} chars long)", _ipEP.ToString(), header.Message, header.SessionID, header.Content.Length);
        }

        public static void Broadcast(PlexServerHeader header)
        {
            var ip = IPAddress.Broadcast;
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            foreach (var port in ports)
            {
                _server.Send(bytes, bytes.Length, new IPEndPoint(ip, port));
            }
        }

        private static List<int> ports = new List<int>();

        public static void Main(string[] args, bool isMP)
        {
            IsMultiplayerServer = isMP;
            Main(args);
        }

        public static void LoadWorld()
        {
            if (!File.Exists("world.whoa"))
            {
                Console.WriteLine("<worldgen> No world detected on filesystem. Generating world...");

                var world = new World();
                Console.WriteLine("<worldgen> Creating subnet list...");
                world.Networks = new List<Subnet>();


                Console.WriteLine("<worldgen> Creating MAIN subnet...");

                var main = CreateSubnet("main", "Main subnet", "This is the maintenance subnet of the Plexnet. The health of this network is vital for proper operation of the Plexnet.");

                Console.WriteLine("<rogue> Generating rogue system...");

                var rogue = GenerateSystem(1000, SystemType.Computer, "rogue");

                Console.WriteLine("<rogue> Joining main subnet");
                main.NPCs.Add(rogue);

                Console.WriteLine("<main> Joining world network...");

                world.Rogue = main;

                int subnets = NATOCodeNames.Length;
                Console.WriteLine("<worldgen> This world will have {0} sub-networks.", subnets);

                for (int i = 0; i < subnets; i++)
                {
                    Console.WriteLine("<worldgen> Generating sub-network {0}...", i);
                    string name = null;
                    while (name == null || world.Networks.FirstOrDefault(x => x.Name == name) != null)
                    {
                        name = NATOCodeNames[rnd.Next(0, NATOCodeNames.Length)];
                    }
                    Console.WriteLine("<{0}> Subnet generated.", name);

                    var subnet = CreateSubnet(name, $"{name} subnet", "");

                    Console.WriteLine("<{0}> I need a router.", name);

                    var router = GenerateSystem(rnd.Next(1, Ranks.Count), SystemType.Router, GenerateSystemName(subnet));

                    Console.WriteLine("<{0}.{1}> Joining {0}...", name, router.SystemDescriptor.SystemName);

                    subnet.NPCs.Add(router);

                    for (int j = 1; j <= router.SystemDescriptor.Rank; j++)
                    {
                        int max_systems = 20 / j;
                        int generatedsystems = rnd.Next(1, max_systems);

                        Console.WriteLine("<{0}> Generating {1} rank {2} systems...", subnet.Name, generatedsystems, j);

                        string stype = null;
                        string[] names = Enum.GetNames(typeof(SystemType));
                        while (stype == null || stype == "Router")
                            stype = names[rnd.Next(names.Length)];
                        SystemType systemtype = (SystemType)Enum.Parse(typeof(SystemType), stype);
                        var system = GenerateSystem(j, systemtype, GenerateSystemName(subnet));
                        Console.WriteLine("<{0}.{1}> Joining {0}...", subnet.Name, system.SystemDescriptor.SystemName);
                        subnet.NPCs.Add(system);
                    }


                    Console.WriteLine("<{0}> Joining world...", subnet.Name);

                    world.Networks.Add(subnet);
                }

                Console.WriteLine("<worldgen> SUBNETS CREATED. Now let's position them.");
                PointF _mainPos = new PointF(0, 0);
                foreach (var net in world.Networks)
                {
                    int averagerank = AverageRank(net.NPCs);
                    float radius = 125 * averagerank;
                    int degrees = rnd.Next(360);
                    var point = GetPositionFromRadius(_mainPos, radius, degrees);
                    Console.WriteLine("<{0}> Average rank: {1} - Radius from main net: {2} - Absolute World Position: {3}, {4}", net.Name, averagerank, radius, point.X, point.Y);
                    net.WorldX = point.X;
                    net.WorldY = point.Y;
                    Console.WriteLine("<{0}> Grabbing router...", net.Name);
                    var router = net.NPCs.FirstOrDefault(x => x.SystemType == SystemType.Router);
                    Console.WriteLine("<{0}> Router is {0}.{1}", net.Name, router.SystemDescriptor.SystemName);
                    foreach (var npc in net.NPCs.Where(x => x.SystemType != SystemType.Router))
                    {
                        int nradius = 50 * npc.SystemDescriptor.Rank;
                        int ndegrees = rnd.Next(360);
                        var npoint = GetPositionFromRadius(new PointF(router.X, router.Y), nradius, ndegrees);
                        npc.X = npoint.X;
                        npc.Y = npoint.Y;
                        Console.WriteLine("<{0}.{1}> New location: {2}, {3}", net.Name, npc.SystemDescriptor.SystemName, npoint.X, npoint.Y);

                    }
                }
                Console.WriteLine("---------------------------");
                Console.WriteLine("World generation complete.");
                Console.WriteLine("Generating world economy...");
                Console.WriteLine("---------------------------");

                foreach (var net in world.Networks)
                {
                    var computers = net.NPCs.Where(x => x.SystemType != SystemType.Router);
                    var routerRank = net.NPCs.FirstOrDefault(x => x.SystemType == SystemType.Router).SystemDescriptor.Rank;
                    for (int i = 1; i < routerRank; i++)
                    {
                        var rank = Ranks[i];
                        var systemsWithRank = computers.Where(x => x.SystemDescriptor.Rank == i);
                        if (systemsWithRank.Count() == 0)
                            continue;
                        ulong cashPerSystem = rank.MaximumCash / (ulong)systemsWithRank.Count();
                        foreach (var system in systemsWithRank)
                        {
                            Console.WriteLine("<{0}.{1}> Adding cash: ${2}", net.Name, system.SystemDescriptor.SystemName, ((double)cashPerSystem / 100));
                            system.SystemDescriptor.Cash = (long)cashPerSystem;
                        }
                    }
                }
                Console.WriteLine(@"============
Done generating economy.
Now generating defenses...
============");
                Console.WriteLine("<worldgen> Scanning for firewall puzzles...");
                List<PuzzleAttribute> puzzles = new List<PuzzleAttribute>();
                foreach (var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IPuzzle))))
                {
                    var attr = type.GetCustomAttributes(false).FirstOrDefault(x => x is PuzzleAttribute) as PuzzleAttribute;
                    if (attr != null)
                    {
                        Console.WriteLine("<worldgen> Puzzle found: {0} - Rank {1}", type.Name, attr.Rank);
                        puzzles.Add(attr);
                    }
                }
                Console.WriteLine("<worldgen> {0} puzzles found", puzzles.Count);
                foreach (var net in world.Networks)
                {
                    foreach (var sys in net.NPCs)
                    {
                        int chance = sys.SystemDescriptor.Rank * (100 / Ranks.Count);
                        bool hasFirewall = rnd.Next(0, 100) <= chance;
                        var availablePuzzles = puzzles.Where(x => x.Rank <= sys.SystemDescriptor.Rank).ToArray();
                        if (availablePuzzles.Length == 0)
                            hasFirewall = false;
                        if (hasFirewall)
                        {
                            int puzzleCount = rnd.Next(1, availablePuzzles.Length);
                            while (sys.Puzzles.Count < puzzleCount)
                            {
                                PuzzleAttribute atr = availablePuzzles[rnd.Next(0, availablePuzzles.Length)];
                                while (sys.Puzzles.FirstOrDefault(x => x.ID == atr.ID && x.Rank == atr.Rank) != null)
                                {
                                    atr = availablePuzzles[rnd.Next(0, availablePuzzles.Length)];
                                }
                                sys.Puzzles.Add(new Objects.Hacking.Puzzle
                                {
                                    ID = atr.ID,
                                    Rank = atr.Rank,
                                    Completed = false
                                });
                                Console.WriteLine("<worldgen> {0} - new puzzle: {1} {2}", sys.SystemDescriptor.SystemName, atr.ID, atr.Rank);
                            }
                        }
                        sys.HasFirewall = hasFirewall;
                    }
                }
                Program.GameWorld = world;
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
            else
            {
                var current = Ranks[rank];
                save.Cash = 0;
                save.Experience = current.Experience;
                save.MaxLoadedUpgrades = current.UpgradeMax;
            }
            save.CompletedHacks = new List<HackableSystem>();
            save.Language = "english";
            save.PickupPoint = "";
            save.StoriesExperienced = new List<string>();
            save.Transactions = new List<CashTransaction>();
            save.ViralInfections = new List<ViralInfection>();
            save.Upgrades = new Dictionary<string, bool>();
            save.LoadedUpgrades = new List<string>();
            save.ID = Guid.NewGuid();
            hackable.SystemDescriptor = save;
            hackable.IsPwn3d = false;
            hackable.SystemType = type;
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
            if (!IsMultiplayerServer)
            {
                ServerLoop();
            }
            else
            {


                if (!File.Exists("ranks.json"))
                {
                    Console.WriteLine("<ranksys> No ranks found on server. Please configure the ranks. An example file has been written to ranks.example.json. Press a key to continue.");
                    string comment = @"/* Rank example
 *
 * Ranks are used by the server to give players a sense of progression. As the user ranks up,
 * they earn perks such as more upgrade slots and a bigger MoneyMate budget.
 *
 * The rank system is also used by the server's world generator to generate cash and resources
 * for each NPC system in the world based on their rank.
 * 
 * Your server's economy ultimately depends on the budget given to each rank, and the amount of NPCs
 * generated for each rank, so we let you decide the values for each rank in this file.
 *
 * Note that ranks are ordered by experience, starting at 0. Rank 0 is the base rank, and its experience value is
 * completely ignored. All new players start at rank 0, so use it to set what your players can and can't do until they rank up.
 *
 * When you're ready, rename this file to 'ranks.json', and fire up your server, and the next phase of setup will begin.
 *
 *
 * Note: All cash values are expressed in cents. 500 = $5.
 */";
                    var ranksex = new List<Rank>
                    {
                        new Rank
                        {
                            Experience = 0,
                             MaximumCash = 500000,
                              Name = "Inexperienced",
                               UnlockedUpgrades = null,
                                UpgradeMax = 5
                        }
                    };
                    comment += Environment.NewLine;
                    comment += JsonConvert.SerializeObject(ranksex, Formatting.Indented);
                    if (!File.Exists("ranks.example.json"))
                        File.WriteAllText("ranks.example.json", comment);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                Ranks = JsonConvert.DeserializeObject<List<Rank>>(File.ReadAllText("ranks.json"));

                Console.WriteLine("PROJECT: PLEX SERVER SOFTWARE - Copyright (c) 2017 Watercolor Games - Licensed under MIT");
                Console.WriteLine("===============================");
                Console.WriteLine();
                LoadWorld();

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
                var parser = CommandParser.GenerateSample();
                while (true)
                {
                    Console.Write("> ");
                    string cmd = Console.ReadLine();
                    if (cmd == "exit")
                    {
                        Console.WriteLine("<worldgen> Saving world...");
                        SaveWorld();
                        Broadcast(new PlexServerHeader
                        {
                            Message = "server_shutdown",
                            SessionID = "",
                            IPForwardedBy = "",
                            Content = ""
                        });
                        Console.WriteLine("<plexsrv> Broadcasting shutdown message to connected clients... waiting 5 seconds before closing.");
                        System.Threading.Thread.Sleep(5000);
                        Environment.Exit(-1);
                    }
                    else
                    {
                        var parsed = parser.ParseCommand(cmd);
                        Dictionary<string, object> cargs = new Dictionary<string, object>();
                        foreach (var arg in parsed.Value)
                            cargs.Add(arg.Key, arg.Value);
                        if (!Terminal.RunClient(parsed.Key, cargs, "server"))
                        {
                            Console.WriteLine("Command not found.");
                        }
                    }
                }
            }

        }

        [ServerCommand("banip", "Ban an IP address from this server.")]
        [RequiresArgument("id")]
        public static void BanIP(Dictionary<string, object> args)
        {
            string ip = args["id"].ToString();
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

        public static void ServerLoop()
        {
            for(int i = 0; i < threadCount; i++)
            {
                var thread = new ServerThread();
                Console.WriteLine("Starting server thread {0}...", i);
                thread.Start();
                threads.Add(thread);
            }

            _server = new UdpClient();
            var _ipEP = new IPEndPoint(IPAddress.Any, _MyPort);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Any, _MyPort));
            _server.Client = sock;

            while (true)
            {
                _ipEP = new IPEndPoint(IPAddress.Any, _MyPort);
                var receive = _server.Receive(ref _ipEP);
                int port = _ipEP.Port;
                string address = _ipEP.Address.ToString();

                var thread = threads.Aggregate((curMin, x) => (curMin == null || (x.Count) < curMin.Count ? x : curMin));
                thread.Queue(() =>
                {
                    if (BannedIPs.Contains(_ipEP.Address.ToString()))
                    {
                        
                        SendMessage(new PlexServerHeader
                        {
                            IPForwardedBy = _ipEP.Address.ToString(),
                            Message = "server_banned",
                            SessionID = "",
                            Content = ""
                        }, port);
                        if (IsMultiplayerServer)
                            Console.WriteLine("<server/banhammer> Attempted connection from banned IP address {0} has been blocked.", _ipEP.ToString());

                        return;
                    }
                    if (!ports.Contains(port))
                        ports.Add(port);
                    string data = Encoding.UTF8.GetString(receive);
                    if (data == "heart")
                    {
                        var beat = Encoding.UTF8.GetBytes("beat");
                        _server.Send(beat, beat.Length, new IPEndPoint(IPAddress.Parse(address), port));

                    }
                    else if (data == "ismp")
                    {
                        int value = (IsMultiplayerServer) ? 1 : 0;
                        _server.Send(new byte[] { (byte)value }, 1, new IPEndPoint(IPAddress.Parse(address), port));
                        if (IsMultiplayerServer)
                            Console.WriteLine("<server> {0} -> me: Are you a multiplayer server?", _ipEP.ToString());
                        if (IsMultiplayerServer)
                            Console.WriteLine("<server> me -> {0}: {1}", _ipEP.ToString(), (value == 1) ? "Yes." : "No.");

                    }
                    else
                    {
                        var header = JsonConvert.DeserializeObject<PlexServerHeader>(data);
                        IPAddress test = null;
                        if (IPAddress.TryParse(header.IPForwardedBy, out test) == false)
                            header.IPForwardedBy = address;
                        if (IsMultiplayerServer)
                            Console.WriteLine("<server> {0} -> me: {1} (session id: \"{2}\", content: {3} chars long)", _ipEP.ToString(), header.Message, header.SessionID, header.Content.Length);

                        ServerManager.HandleMessage(header, port);
                    }
                });
            }

        }
    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        internal static void HandleMessage(PlexServerHeader header, int port)
        {
            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is ServerMessageHandlerAttribute) != null))
                {
                    var attribute = method.GetCustomAttributes(false).FirstOrDefault(x => x is ServerMessageHandlerAttribute) as ServerMessageHandlerAttribute;
                    if (attribute.ID == header.Message)
                    {
                        var sessionRequired = method.GetCustomAttributes(false).FirstOrDefault(x => x is SessionRequired) as SessionRequired;
                        if(sessionRequired != null)
                        {
                            bool nosession = string.IsNullOrWhiteSpace(header.SessionID);
                            if(nosession == false)
                            {
                                nosession = SessionManager.IsExpired(header.SessionID);
                            }

                            if (nosession)
                            {
                                Program.SendMessage(new PlexServerHeader
                                {
                                    IPForwardedBy = header.IPForwardedBy,
                                    Message = "login_required",
                                    Content = ""
                                }, port);
                                return;
                            }
                        }
                        method.Invoke(null, new object[] { header.SessionID, header.Content, header.IPForwardedBy, port });
                        return;
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SessionRequired : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerMessageHandlerAttribute : Attribute
    {
        public ServerMessageHandlerAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }

}
