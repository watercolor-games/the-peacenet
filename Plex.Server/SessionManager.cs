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
        public static byte GetSysname(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            writer.Write(session.SaveID);
            return 0x00;
        }

        [ServerMessageHandler( ServerMessageType.USR_GETXP)]
        [SessionRequired]
        public static byte GetXP(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            writer.Write(save.SystemDescriptor.Experience);
            return 0x00;
        }

        [ServerMessageHandler( ServerMessageType.USR_GETUSERNAME)]
        [SessionRequired]
        public static byte GetUsername(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var session = GrabAccount(session_id);
            writer.Write(session.Username);
            return 0x00;
            
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
        public static byte SessionVerify(string session_id, BinaryReader reader, BinaryWriter writer)
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
                    return (byte)ServerResponseType.REQ_LOGINREQUIRED;
                }
                writer.Write(session_id); //we do this twice, once because lol, again because lol
                return 0x00;

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
                writer.Write(accts[0].SessionID);
                return 0x00;
            }
        }

        [ServerMessageHandler( ServerMessageType.USR_REGISTER)]
        public static byte CreateAccount(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            
            var sessions = getAccts();
            if(sessions.FirstOrDefault(x=>x.Username == username) != null)
            {
                return (byte)ServerResponseType.REQ_ERROR;
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
            writer.Write(acct.SessionID);
            return 0x00;
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
        public static byte AccountGetKey(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            if (Program.IsMultiplayerServer)
            {
                var user = getAccts().FirstOrDefault(x => x.Username == username);
                if (user == null)
                {
                    return (byte)ServerResponseType.REQ_ERROR;
                }
                var hashedpass = PasswordHasher.Hash(password, user.PasswordSalt);
                if (hashedpass != user.PasswordHash)
                {
                    return (byte)ServerResponseType.REQ_ERROR;

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

               writer.Write(user.SessionID);
                return 0x00;
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

                writer.Write(getAccts()[0].SessionID);
                return 0x00;
            }

        }

        [ServerMessageHandler( ServerMessageType.SP_COMPLETESTORY)]
        [SessionRequired]
        public static byte CompleteStory(string session_id, BinaryReader reader, BinaryWriter writer)
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
            return 0x00;
        }
    }
}
