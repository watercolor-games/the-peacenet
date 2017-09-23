using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;
using Newtonsoft.Json;

namespace Plex.Server
{
    public static class SessionManager
    {
        private static readonly string sp_sessionkey = Guid.NewGuid().ToString();

        [ServerMessageHandler("acct_getsysname")]
        [SessionRequired]
        public static void GetSysname(string session_id, string content, string ip, int port)
        {
            var session = GrabAccount(session_id);
            Program.SendMessage(new PlexServerHeader
            {
                Message = "acct_sysname",
                Content = session.SaveID,
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }

        [ServerMessageHandler("acct_getxp")]
        [SessionRequired]
        public static void GetXP(string session_id, string content, string ip, int port)
        {
            var session = GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            Program.SendMessage(new PlexServerHeader
            {
                Content = save.SystemDescriptor.Experience.ToString(),
                IPForwardedBy = ip,
                Message = "acct_xp",
                SessionID = session_id
            }, port);
        }

        [ServerMessageHandler("acct_getusername")]
        [SessionRequired]
        public static void GetUsername(string session_id, string content, string ip, int port)
        {
            var session = GrabAccount(session_id);
            Program.SendMessage(new PlexServerHeader
            {
                Message = "acct_username",
                Content = session.Username,
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }

        private static List<ServerAccount> getAccts()
        {
            if (Program.IsMultiplayerServer)
            {
                if (!File.Exists("accts.json"))
                    return new List<ServerAccount>();
                return JsonConvert.DeserializeObject<List<ServerAccount>>(File.ReadAllText("accts.json"));
            }
            else
            {
                return new List<ServerAccount>
                {
                    new ServerAccount
                    {
                         Expiry = DateTime.Now.AddMonths(60),
                          LastLogin = DateTime.Now,
                           PasswordHash = "",
                            PasswordSalt = new byte[0],
                             SaveID = "main.rogue", //nyi
                              SessionID = sp_sessionkey,
                               Username = "campaign"
                    }
                };
            }
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

        [ServerMessageHandler("session_verify")]
        public static void SessionVerify(string session_id, string content, string ip, int port)
        {
            bool nosession = string.IsNullOrWhiteSpace(session_id);
            if (!nosession)
            {
                nosession = IsExpired(session_id);
            }
            if (nosession)
            {
                Program.SendMessage(new PlexServerHeader
                {
                    Content = "",
                    IPForwardedBy = ip,
                    Message = "login_required",
                    SessionID = session_id
                }, port);
            }
        }

        [ServerMessageHandler("acct_create")]
        public static void CreateAccount(string session_id, string content, string ip, int port)
        {
            var acct = JsonConvert.DeserializeObject<ServerAccount>(content);
            var sessions = getAccts();
            if(sessions.FirstOrDefault(x=>x.Username == acct.Username) != null)
            {
                Program.SendMessage(new PlexServerHeader
                {
                    IPForwardedBy = ip,
                    Message = "acct_taken",
                    SessionID = session_id,
                    Content = ""
                }, port);

                return;
            }
            Console.WriteLine("<acctmgr> New account {0} is being created.", acct.Username);
            Console.WriteLine("<acctmgr> Generating password salt...");
            acct.PasswordSalt = PasswordHasher.GenerateRandomSalt();
            Console.WriteLine("<acctmgr> Done. Now hashing password...");
            acct.PasswordHash = PasswordHasher.Hash(acct.PasswordHash, acct.PasswordSalt);
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
            Program.SendMessage(new PlexServerHeader
            {
                IPForwardedBy = ip,
                Message = "session_accessgranted",
                SessionID = session_id,
                Content = acct.SessionID
            }, port);

        }

        public static ServerAccount GrabAccount(string session_key)
        {
            return getAccts().FirstOrDefault(x => x.SessionID == session_key);
        }

        public static void SetSessionInfo(string session_key, ServerAccount acct)
        {
            var accts = getAccts();
            int index = accts.IndexOf(accts.FirstOrDefault(x => x.SessionID == session_key));
            accts[index] = acct;
            setSessions(accts);
        }


        [ServerMessageHandler("acct_get_key")]
        public static void AccountGetKey(string session_id, string content, string ip, int port)
        {
            if (Program.IsMultiplayerServer)
            {
                string[] split = content.Split('\t');
                if (split.Length != 2)
                {
                    Program.SendMessage(new PlexServerHeader
                    {
                        IPForwardedBy = ip,
                        Message = "malformed_data",
                        SessionID = session_id,
                        Content = ""
                    }, port);
                    return;
                }
                string username = split[0];
                string password = split[1];
                var user = getAccts().FirstOrDefault(x => x.Username == username);
                if (user == null)
                {
                    Program.SendMessage(new PlexServerHeader
                    {
                        IPForwardedBy = ip,
                        Message = "session_accessdenied",
                        SessionID = session_id,
                        Content = ""
                    }, port);
                    return;
                }
                var hashedpass = PasswordHasher.Hash(password, user.PasswordSalt);
                if (hashedpass != user.PasswordHash)
                {
                    Program.SendMessage(new PlexServerHeader
                    {
                        IPForwardedBy = ip,
                        Message = "session_accessdenied",
                        SessionID = session_id,
                        Content = session_id
                    }, port);
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

                Program.SendMessage(new PlexServerHeader
                {
                    IPForwardedBy = ip,
                    Message = "session_accessgranted",
                    SessionID = session_id,
                    Content = user.SessionID
                }, port);
            }
            else
            {
                Program.SendMessage(new PlexServerHeader
                {
                    IPForwardedBy = ip,
                    Message = "session_accessgranted",
                    SessionID = session_id,
                    Content = getAccts()[0].SessionID
                }, port);
            }

        }
    }
}
