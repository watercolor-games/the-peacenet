using System;
using System.Collections.Generic;
using Plex.Objects;
using System.Linq;

namespace Plex.Server
{
    public static class AuthenticationManager
    {
        private static string GetSessionID(string address)
        {
            var session = SessionManager.GetSessions().FirstOrDefault(x => x.SaveID == address);

            if (session == null)
            {
                //obvi. NPC
                var save = Program.GetSaveFromPrl(address);
                if (save != null)
                {
                    session = new ServerAccount
                    {
                        Expiry = DateTime.Now.AddDays(1),
                        IsNPC = true,
                        LastLogin = DateTime.Now,
                        PasswordHash = "",
                        PasswordSalt = null,
                        SaveID = address,
                        SessionID = Guid.NewGuid().ToString(),
                        Username = save.SystemDescriptor.Username
                    };
                    SessionManager.SetSessionInfo(session.SessionID, session);
                    return session.SessionID;
                }
                throw new Exception("System not found.");
            }
            return session.SessionID;
        }

        public static bool Authenticate(string usersession, string source_ip, int source_port, string sysaddress, int port, string user, string pass, out string auth_token)
        {
            var sessiondata = SessionManager.GrabAccount(usersession);
            if(sessiondata == null)
            {
                auth_token = null;
                return false;
            }
            var save = Program.GetSaveFromPrl(sessiondata.SaveID);
            if (save == null)
            {
                auth_token = null;
                return false;
            }
            if(save.SystemDescriptor.UsedCredentials == null)
            {
                save.SystemDescriptor.UsedCredentials = new List<UsedCredential>();
                Program.SaveWorld();
            }
            bool useexistingcreds = false;
            UsedCredential creds = new UsedCredential();
            if (string.IsNullOrWhiteSpace(user) && string.IsNullOrWhiteSpace(pass))
            {
                useexistingcreds = true;
                creds = save.SystemDescriptor.UsedCredentials.FirstOrDefault(x => x.Address == sysaddress && x.Port == port);
            }
            if(useexistingcreds == true)
            {
                if(creds == null)
                {
                    auth_token = null;
                    return false;
                }
                else
                {
                    auth_token = GetSessionID(sysaddress);
                    return true;
                }
            }
            else
            {
                var nsave = Program.GetSaveFromPrl(sysaddress);
                if(nsave == null)
                {
                    auth_token = null;
                    return false;
                }
                if(nsave.SystemDescriptor.Username == user && nsave.SystemDescriptor.Password == pass)
                {
                    auth_token = GetSessionID(sysaddress);
                    save.SystemDescriptor.UsedCredentials.Add(new UsedCredential
                    {
                        Username = user,
                        Password = pass,
                        Port = port,
                        Address = sysaddress
                    });
                    Program.SaveWorld();
                    return true;
                }
                else
                {
                    auth_token = null;
                    return false;
                }

            }
        }
    }
}