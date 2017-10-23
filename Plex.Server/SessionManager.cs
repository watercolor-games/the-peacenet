using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace Plex.Server
{
    public static class SessionManager
    {
        private static readonly string sp_sessionkey = Guid.NewGuid().ToString();

        public static List<ServerAccount> GetSessions()
        {
            return getAccts();
        }

        [ServerMessageHandler( ServerMessageType.USR_GETSYSNAME)]
        [SessionRequired]
        public static void GetSysname(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(session.SaveID);
        }

        [ServerMessageHandler( ServerMessageType.USR_GETXP)]
        [SessionRequired]
        public static void GetXP(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(save.SystemDescriptor.Experience);
        }

        [ServerMessageHandler( ServerMessageType.USR_GETUSERNAME)]
        [SessionRequired]
        public static void GetUsername(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(session.Username);
            
        }

        private static List<ServerAccount> getAccts()
        {
            if (!File.Exists("accts.json"))
            {
                return new List<ServerAccount>();
            }
            return JsonConvert.DeserializeObject<List<ServerAccount>>(File.ReadAllText("accts.json"));
        }

        private static void setSessions(List<ServerAccount> info)
        {
            File.WriteAllText("accts.json", JsonConvert.SerializeObject(info));
        }

        public static bool IsExpired(string session_id)
        {
            var acct = getAccts().FirstOrDefault(x => x.SessionID == session_id);
            if (acct == null)
                return true;
            return DateTime.Now > acct.Expiry;
        }

        [ServerMessageHandler( ServerMessageType.USR_VALIDATEKEY)]
        public static void SessionVerify(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            if (Program.IsMultiplayerServer)
            {
                bool nosession = string.IsNullOrWhiteSpace(session_id);
                if (!nosession)
                {
                    nosession = IsExpired(session_id);
                }
                if (nosession)
                {
                    writer.Write((int)ServerResponseType.REQ_LOGINREQUIRED);
                    writer.Write(session_id);
                    return;
                }
            }
            else
            {
                var accts = getAccts();
                if (accts.Count == 0)
                {
                    accts.Add(new ServerAccount
                    {
                        Expiry = DateTime.MaxValue,
                        LastLogin = DateTime.Now,
                        PasswordHash = "",
                        PasswordSalt = null,
                        SaveID = "alfa.system",
                        SessionID = "singleplayers",
                        Username = "user"
                    });
                    setSessions(accts);

                }
                var save = Program.GetSaveFromPrl("alfa.system");
                if (save == null)
                {
                    var subnet = Program.GameWorld.Networks.FirstOrDefault(x => x.Name == "alfa");
                    var sys = Program.GenerateSystem(0, SystemType.Computer, "system");
                    sys.IsNPC = false;
                    subnet.NPCs.Add(sys);
                    Program.SaveWorld();
                }
                writer.Write((int)ServerResponseType.REQ_SUCCESS);
                writer.Write(session_id);
                writer.Write(accts[0].SessionID);
                return;
            }
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(session_id); //we do this twice, once because lol, again because lol

        }

        [ServerMessageHandler( ServerMessageType.USR_REGISTER)]
        public static void CreateAccount(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            
            var sessions = getAccts();
            if(sessions.FirstOrDefault(x=>x.Username == username) != null)
            {
                writer.Write((int)ServerResponseType.REQ_ERROR);
                writer.Write(session_id);
                return;
            }
            var acct = new ServerAccount();
            acct.Username = username;
            Console.WriteLine("<acctmgr> New account {0} is being created.", username);
            Console.WriteLine("<acctmgr> Generating password salt...");
            acct.PasswordSalt = PasswordHasher.GenerateRandomSalt();
            Console.WriteLine("<acctmgr> Done. Now hashing password...");
            acct.PasswordHash = PasswordHasher.Hash(password, acct.PasswordSalt);
            Console.WriteLine("<acctmgr> Yay. Everything's nice and secure......for now.");
            Console.WriteLine("<sessions> Setting expiry date...");
            acct.Expiry = DateTime.Now.AddDays(7);
            Console.WriteLine("<sessions> Expiry: {0}", acct.Expiry);
            Console.WriteLine("<sessions> Generating new session key...");
            acct.LastLogin = DateTime.Now;
            acct.SessionID = Guid.NewGuid().ToString();
            //Create the save file
            var net = Program.GetRandomSubnet();
            string sysname = Program.GenerateSystemName(net);

            var sys = Program.GenerateSystem(0, SystemType.Computer, sysname);
            sys.IsNPC = false;
            net.NPCs.Add(sys);

            acct.SaveID = net.Name + "." + sys.SystemDescriptor.SystemName;
            Program.SaveWorld();

            sessions.Add(acct);
            setSessions(sessions);
            Console.WriteLine("<sessions> Account data updated.");
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(acct.SessionID);
        }

        [ServerCommand("connect", "Attempt to connect to a specific port on the current system.")]
        [RequiresArgument("id")]
        public static void ConnectToPort(Dictionary<string, object> args)
        {
            string sysid = args["id"].ToString();
            string user = "";
            if (args.ContainsKey("u"))
                user = args["u"].ToString();
            string pass = "";
            if (args.ContainsKey("p"))
                pass = args["p"].ToString();

            bool listPorts = !sysid.Contains(":");
            if (listPorts)
            {
                if (sysid.Contains("."))
                {
                    var sys = Program.GetSaveFromPrl(sysid);
                    if (sys != null)
                    {
                        Console.WriteLine("Port #: Name");
                        Console.WriteLine("=====================");
                        Console.WriteLine();
                        foreach (var port in Hacking.GetPorts(sys.SystemType))
                        {
                            Console.WriteLine($"{port.Value}: {port.FriendlyName}");
                        }

                        return;
                    }
                }
            }
            else
            {
                if (sysid.Contains("."))
                {
                    int portstart = sysid.IndexOf(":");
                    int len = sysid.Length - portstart;
                    string sysaddress = sysid.Remove(portstart, len);
                    int port = 0;
                    if (int.TryParse(sysid.Remove(0, sysaddress.Length + 1), out port) == true)
                    {
                        var sys = Program.GetSaveFromPrl(sysaddress);
                        if (sys != null)
                        {
                            Console.WriteLine("Connecting to {0}...", sysaddress);
                            var portdata = Hacking.GetPorts(sys.SystemType).FirstOrDefault(x => x.Value == port);
                            if (portdata == null)
                            {
                                Console.WriteLine("Connection refused. Port not open.");
                                return;
                            }
                            if (sys.HasFirewall == true)
                            {
                                bool hasPuzzle = sys.Puzzles.FirstOrDefault(x => x.Completed == false) != null;
                                if (hasPuzzle)
                                {
                                    Console.WriteLine("Port is open, but connection blocked by firewall.");
                                    Console.WriteLine("Initiating firewall breach console...");
                                    Thread.Sleep(750);
                                    Console.WriteLine("Complete the puzzles provided by the firewall to continue connection.");
                                    Console.WriteLine("When you have completed them all, the connection attempt will continue.");
                                    //Hacking.StartHack(Terminal.SessionInfo.SessionID, sysaddress, Terminal.SessionInfo.IPAddress, Terminal.SessionInfo.Port);
                                    return;
                                }
                                Console.WriteLine("Firewall detected, but is offline.");
                            }
                            string auth_token = "";
                            if(AuthenticationManager.Authenticate(Terminal.SessionInfo.SessionID, Terminal.SessionInfo.IPAddress, Terminal.SessionInfo.Port, sysaddress, port, user, pass, out auth_token) == true)
                            {
                                Console.WriteLine("Connection successful.");
                                Hacking.PortConnect(auth_token, port, Terminal.SessionInfo.SessionID, Terminal.SessionInfo.IPAddress, Terminal.SessionInfo.Port);

                            }
                            else
                            {
                                Console.WriteLine("Access denied: authentication required");
                            }
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid port value! Must be a valid integer.");
                        return;
                    }
                }
            }

            Console.WriteLine("Error: Can't find the system on the address '{0}'. Remember that system address follow the 'subnet.device:port' syntax.\r\n\r\nYou can omit the :port if you want to see a list of ports.");
        }

        [ServerCommand("status", "Shows system status.")]
        public static void Status()
        {
            if(Terminal.SessionInfo == null)
            {
                Console.WriteLine("Usersession required.");
                return;
            }

            var session = SessionManager.GrabAccount(Terminal.SessionID);
            var save = Program.GetSaveFromPrl(session.SaveID);
            Console.WriteLine("System status");
            Console.WriteLine("-----------------------");
            Console.WriteLine();
            Console.WriteLine("Experience: {0}", save.SystemDescriptor.Experience);
            Console.WriteLine("Rank: {0}", save.SystemDescriptor.Rank);
            Console.WriteLine("Cash: ${0}", (double)save.SystemDescriptor.Cash / 100);

        }

        public static ServerAccount GrabAccount(string session_key)
        {
            return getAccts().FirstOrDefault(x => x.SessionID == session_key);
        }

        public static void SetSessionInfo(string session_key, ServerAccount acct)
        {
            var accts = getAccts();
            try
            {
                int index = accts.IndexOf(accts.FirstOrDefault(x => x.SessionID == session_key));
                accts[index] = acct;
            }
            catch
            {
                accts.Add(acct);
            }
            setSessions(accts);
        }


        [ServerMessageHandler( ServerMessageType.USR_LOGIN)]
        public static void AccountGetKey(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            if (Program.IsMultiplayerServer)
            {
                var user = getAccts().FirstOrDefault(x => x.Username == username);
                if (user == null)
                {
                    writer.Write((int)ServerResponseType.REQ_ERROR);
                    writer.Write(session_id);
                    return;
                }
                var hashedpass = PasswordHasher.Hash(password, user.PasswordSalt);
                if (hashedpass != user.PasswordHash)
                {
                    writer.Write((int)ServerResponseType.REQ_ERROR);
                    writer.Write(session_id);
                    return;

                }
                var now = DateTime.Now;
                var expiry = user.Expiry;
                if (now > expiry)
                {
                    string sessionkey = Guid.NewGuid().ToString();
                    var accts = getAccts();
                    accts.FirstOrDefault(x => x.Username == user.Username).Expiry = DateTime.Now.AddDays(7);
                    accts.FirstOrDefault(x => x.Username == user.Username).SessionID = sessionkey;
                    setSessions(accts);
                    user.SessionID = sessionkey;
                }

                var usersys = Program.GetSaveFromPrl(user.SaveID);
                if(usersys == null)
                {
                    if (string.IsNullOrWhiteSpace(user.SaveID))
                    {
                        var sn = Program.GetRandomSubnet();
                        string sysname = Program.GenerateSystemName(sn);
                        var sys = Program.GenerateSystem(0, SystemType.Computer, sysname);
                        sys.IsNPC = false;
                        sn.NPCs.Add(sys);
                        user.SaveID = $"{sn.Name}.{sysname}";
                        SetSessionInfo(user.SessionID, user);
                        Program.SaveWorld();
                    }
                    else
                    {
                        string[] usplit = user.SaveID.Split('.');
                        var sn = Program.GameWorld.Networks.FirstOrDefault(x => x.Name == usplit[0]);
                        var sys = Program.GenerateSystem(0, SystemType.Computer, usplit[1]);
                        sys.IsNPC = false;
                        sn.NPCs.Add(sys);
                        Program.SaveWorld();
                    }
                }

                writer.Write((int)ServerResponseType.REQ_SUCCESS);
                writer.Write(session_id);
                writer.Write(user.SessionID);
            }
            else
            {
                var accts = getAccts();
                if(accts.Count == 0)
                {
                    accts.Add(new ServerAccount
                    {
                        Expiry = DateTime.MaxValue,
                        LastLogin = DateTime.Now,
                        PasswordHash = "",
                        PasswordSalt = null,
                        SaveID = "alfa.system",
                        SessionID = "singleplayers",
                        Username = "user"
                    });
                    setSessions(accts);
                    
                }
                var save = Program.GetSaveFromPrl("alfa.system");
                if (save == null)
                {
                    var subnet = Program.GameWorld.Networks.First(x => x.Name == "alfa");
                    var sys = Program.GenerateSystem(0, SystemType.Computer, "system");
                    sys.IsNPC = false;
                    subnet.NPCs.Add(sys);
                    Program.SaveWorld();
                }

                writer.Write((int)ServerResponseType.REQ_SUCCESS);
                writer.Write(session_id);
                writer.Write(getAccts()[0].SessionID);
            }

        }

        [ServerMessageHandler( ServerMessageType.SP_COMPLETESTORY)]
        [SessionRequired]
        public static void CompleteStory(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
            var session = GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            if(save != null)
            {
                var sys = save.SystemDescriptor;
                if (sys.StoriesExperienced == null)
                    sys.StoriesExperienced = new List<string>();
                if (!sys.StoriesExperienced.Contains(content))
                {
                    sys.StoriesExperienced.Add(content);
                    Program.SaveWorld();
                }
            }
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
        }
    }
}
