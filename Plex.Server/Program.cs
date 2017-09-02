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

namespace Plex.Server
{
    public class Program
    {
        private static UdpClient _server = null;
        internal static bool IsMultiplayerServer = true;
        public static List<string> BannedIPs = new List<string>();

        public static void SendMessage(PlexServerHeader header)
        {
            var ip = IPAddress.Parse(header.IPForwardedBy);
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            _server.Send(bytes, bytes.Length, new IPEndPoint(ip, _port));
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

        public static void Main(string[] args)
        {
            if (!IsMultiplayerServer)
            {
                ServerLoop();
            }
            else
            {
                Console.SetOut(new LocalizedTextWriter());
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
            var _ipEP = new IPEndPoint(IPAddress.Any, 62252);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, 62252));
            _server.Client = sock;
            while (true)
            {
                _ipEP = new IPEndPoint(IPAddress.Any, 62252);
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
                }
                else
                {
                    var header = JsonConvert.DeserializeObject<PlexServerHeader>(data);
                    IPAddress test = null;
                    if (IPAddress.TryParse(header.IPForwardedBy, out test) == false)
                        header.IPForwardedBy = _ipEP.Address.ToString();
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
                Console.WriteLine(localized);
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
