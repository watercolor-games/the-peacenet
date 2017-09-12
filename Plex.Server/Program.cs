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
using Plex.Engine;
using System.IO;
using System.Drawing;

namespace Plex.Server
{
    public class Program
    {
        private static readonly string[] NATOCodeNames = { "alfa", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india", "juliett", "kilo", "lima", "mike", "november", "oscar", "papa", "quebec", "romeo", "sierra", "tango", "uniform", "victor", "whiskey", "xray", "yankee", "zulu" };
        private const int _MyPort = 3251;
        private static UdpClient _server = null;
        internal static bool IsMultiplayerServer = true;
        public static List<string> BannedIPs = new List<string>();

        public static List<Rank> Ranks { get; set; }

        public static World GameWorld = null;

        public static void SendMessage(PlexServerHeader header)
        {
            var ip = IPAddress.Parse(header.IPForwardedBy);
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            var _ipEP = new IPEndPoint(ip, _port);
            _server.Send(bytes, bytes.Length, _ipEP);
            if (IsMultiplayerServer)
                Console.WriteLine("<server> me -> {0}: {1} (session id: \"{2}\", content: {3})", _ipEP.ToString(), header.Message, header.SessionID, header.Content);
        }

        public static void Broadcast(PlexServerHeader header)
        {
            var ip = IPAddress.Broadcast;
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            _server.Send(bytes, bytes.Length, new IPEndPoint(ip, _port));
        }

        private static int _port = 0;

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

                int subnets = rnd.Next(0, NATOCodeNames.Length - 1);
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
                Program.GameWorld = world;
                SaveWorld();
            }
            else
            {
                try
                {
                    Console.WriteLine("<worldgen> Loading from world.whoa...");
                    GameWorld = JsonConvert.DeserializeObject<World>(File.ReadAllText("world.whoa"));
                }
                catch
                {
                    Console.WriteLine("<worldgen> Bad world file. Regenerating...");
                    File.Delete("world.whoa");
                    LoadWorld();
                }
            }
        }

        public class ServerSkin : Skin { }

        public class ServerSkinProvider : ISkinProvider
        {
            public Skin GetDefaultSkin()
            {
                return new ServerSkin();
            }

            public Skin GetEasterEggSkin()
            {
                return new ServerSkin();
            }

            public Skin ReadSkin(string pfsPath)
            {
                throw new NotImplementedException();
            }
        }

        public static void SaveWorld()
        {
            File.WriteAllText("world.whoa", JsonConvert.SerializeObject(GameWorld, Formatting.Indented));
        }

        [Command("worldinfo")]
        public static void WorldInfo()
        {
            Console.WriteLine("World info");
            Console.WriteLine("===============");
            Console.WriteLine();
            Console.WriteLine("Subnet count (excluding rogue): {0}", GameWorld.Networks.Count);
            List<HackableSystem> systems = new List<HackableSystem>();
            foreach(var net in GameWorld.Networks)
            {
                systems.AddRange(net.NPCs);
            }
            Console.WriteLine("Systems: {0}", systems.Count);
            foreach(SystemType stype in Enum.GetValues(typeof(SystemType)).Cast<SystemType>())
            {
                Console.WriteLine("  {0} of which are a {1}", systems.Where(x => x.SystemType == stype).Count(), stype);
            }
            long cash = 0;
            foreach(var system in systems)
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
            while(sysname == null || subnet.NPCs.FirstOrDefault(x=>x.SystemDescriptor.SystemName == sysname) != null)
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
            Console.WriteLine("<worldgen> Creating system descriptor...");
            var save = new Save();
            save.SystemName = sysname;
            Console.WriteLine("<worldgen> System name: " + sysname);
            save.Rank = rank;
            Console.WriteLine("<worldgen> Rank: " + rank);

            if(rank == 1000)
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

            hackable.SystemDescriptor = save;
            hackable.IsPwn3d = false;
            hackable.SystemType = type;
            Console.WriteLine("<worldgen> System type: {0}", type);
            return hackable;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static void Main(string[] args)
        {
            if (!IsMultiplayerServer)
            {
                ServerLoop();
            }
            else
            {
                
                Console.SetOut(new LocalizedTextWriter());

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
                SkinEngine.SetSkinProvider(new ServerSkinProvider());
                Console.WriteLine();
                LoadWorld();

                Console.WriteLine("<plexsrv> Starting server...");
                var t = new System.Threading.Thread(ServerLoop);
                t.Start();
                Localization.RegisterProvider(new ServerLanguageProvider());
                Console.WriteLine("Server thread running and listening for requests.");
                Console.WriteLine("Reading banned IP addresses...");
                if (System.IO.File.Exists("banned-ips.json"))
                {
                    BannedIPs = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText("banned-ips.json"));
                }
                Console.WriteLine("{0} IP addresses have been banned.", BannedIPs.Count);
                Console.WriteLine("Starting server shell. Type 'help' for a list of commands.");
                TerminalBackend.PopulateTerminalCommands();
                while (true)
                {
                    TerminalBackend.PrefixEnabled = false;
                    TerminalBackend.InStory = false;
                    Console.Write("> ");
                    string cmd = Console.ReadLine();
                    if(cmd == "exit")
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
                        var parsed = SkinEngine.LoadedSkin.CurrentParser.ParseCommand(cmd);
                        TerminalBackend.InvokeCommand(parsed.Key, parsed.Value);
                    }
                }
            }

        }

        public class ServerLanguageProvider : ILanguageProvider
        {
            public string[] GetAllLanguages()
            {
                return new string[] { "server" };
            }

            public string GetCurrentTranscript()
            {
                WriteDefaultTranscript();
                return File.ReadAllText("server.lang");
            }

            public List<string> GetJSONTranscripts()
            {
                return new List<string> { "server.lang" };
            }

            public void WriteDefaultTranscript()
            {
                File.WriteAllText("server.lang", Properties.Resources.server_lang);
            }

            public void WriteTranscript()
            {
                File.WriteAllText("server.lang", Properties.Resources.server_lang);
            }
        }

        [Command("banip")]
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
            if(IPAddress.TryParse(ip, out _ban) == false)
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
            _server = new UdpClient();
            var _ipEP = new IPEndPoint(IPAddress.Any, _MyPort);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, _MyPort));
            _server.Client = sock;
            
            while (true)
            {
                _ipEP = new IPEndPoint(IPAddress.Any, _MyPort);
                var receive = _server.Receive(ref _ipEP);
                _port = _ipEP.Port;
                if (BannedIPs.Contains(_ipEP.Address.ToString()))
                {
                    SendMessage(new PlexServerHeader
                    {
                        IPForwardedBy = _ipEP.Address.ToString(),
                        Message = "server_banned",
                        SessionID = "",
                        Content = ""
                    });
                    if (IsMultiplayerServer)
                        Console.WriteLine("<server/banhammer> Attempted connection from banned IP address {0} has been blocked.", _ipEP.ToString());

                    continue;
                }
                string data = Encoding.UTF8.GetString(receive);
                if (data == "heart")
                {
                    var beat = Encoding.UTF8.GetBytes("beat");
                    _server.Send(beat, beat.Length, new IPEndPoint(_ipEP.Address, _ipEP.Port));
                    
                }
                else if (data == "ismp")
                {
                    int value = (IsMultiplayerServer) ? 1 : 0;
                    _server.Send(new byte[] { (byte)value }, 1, new IPEndPoint(_ipEP.Address, _ipEP.Port));
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
                        header.IPForwardedBy = _ipEP.Address.ToString();
                    if (IsMultiplayerServer)
                        Console.WriteLine("<server> {0} -> me: {1} (session id: \"{2}\", content: {3})", _ipEP.ToString(), header.Message, header.SessionID, header.Content);

                    ServerManager.HandleMessage(header);
                }
            }

        }

        public class LocalizedTextWriter : System.IO.TextWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.ASCII;
                }
            }

            public override void Write(string value)
            {
                string localized = Localization.Parse(value);
                Reset();
                Console.Write(localized);
                Console.SetOut(this);
            }

            public override void WriteLine()
            {
                Reset();
                Console.WriteLine();
                Console.SetOut(this);
            }

            public void Reset()
            {
                StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);

            }

            public override void WriteLine(string value)
            {
                string localized = Localization.Parse(value);
                Reset();
                Console.WriteLine($"[{DateTime.Now}] {localized}");
                Console.SetOut(this);
            }
        }
    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        internal static void HandleMessage(PlexServerHeader header)
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
                                });
                                return;
                            }
                        }
                        method.Invoke(null, new[] { header.SessionID, header.Content, header.IPForwardedBy });
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
