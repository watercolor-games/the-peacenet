using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Plex.Objects;
using System.Linq;

namespace Peacenet.Backend.Sessions
{
    public class SessionManager : IBackendComponent
    {
        private List<ServerAccount> _users = new List<ServerAccount>();

        private Dictionary<string, string> _sessions = new Dictionary<string, string>();

        private const int SaltLength = 128;
        private byte[] GenerateRandomSalt()
        {
            RandomNumberGenerator _generator = RandomNumberGenerator.Create();
            byte[] data = new byte[SaltLength];
            _generator.GetNonZeroBytes(data);
            return data;
        }

private string GenerateToken(int length)
{
    //A string representing the token.
    string token = "";
    //Byte array for RNG
    byte[] arr = new byte[1];
    //RandomNumberGenerator implements IDisposable. It's a good idea to use the 'using' statement in this case.
    using (var generator = RandomNumberGenerator.Create())
    {
        //Do this for each potential character in the token.
        for (int i = 0; i < length; i++)
        {
            //Empty character.
            char c = '\0';
            //Make sure the character can be printed on-screen. Just in case you want the user to copy/paste their token.
            while(!(char.IsLetterOrDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c)))
            {
                //Generate a single random byte.
                generator.GetNonZeroBytes(arr);
                //Convert said byte to a char, assign to the empty char above.
                c = (char)arr[0];
                //Next time this while loop evaluates, if the char is invalid, we'll run it again. If not..
            }
            //Add the character to the token!
            token += c;
        }
    }
    //All good.
    return token;
}

        private string Hash(string password, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public LoginStatus Login(string username, string password, out string sessionid)
        {
            var user = _users.FirstOrDefault(x=>x.Username == username);
            if (user == null)
            {
                sessionid = null;
                return LoginStatus.BadUsername;
            }
            var salt = user.PasswordSalt;
            var hash = Hash(password, salt);
            if (hash != user.PasswordHash)
            {
                sessionid = null;
                return LoginStatus.BadPassword;
            }
            sessionid = Guid.NewGuid().ToString();
            _sessions.Add(sessionid, user.Username);
            return LoginStatus.Success;
        }

        public ServerAccount GetUserFromSession(string sessionid)
        {
            if (_sessions.ContainsKey(sessionid))
                return _users.FirstOrDefault(x => x.Username == _sessions[sessionid]);
            return null;
        }

        public void Logout(string sessionid)
        {
            if (_sessions.ContainsKey(sessionid))
                _sessions.Remove(sessionid);

        }

        public string CreateSinglePlayerSession()
        {
            string session;
            if (Login("user", "", out session) != LoginStatus.Success)
            {
                Register("user", "", out session);
            }
            return session;
        }

        public RegisterStatus Register(string username, string password, out string sessionid)
        {
            var user = _users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                sessionid = null;
                return RegisterStatus.UsernameTaken;
            }
            var salt = GenerateRandomSalt();
            var hash = Hash(password, salt);

            _users.Add(new ServerAccount
            {
                 BanLiftDate = DateTime.Now,
                 Expiry = DateTime.Now,
                 IsBanned = false,
                 IsNPC = false,
                 LastLogin = DateTime.Now,
                 PasswordHash = hash,
                 PasswordSalt = salt,
                 Permission = ACLPermission.User,
                 SaveID = username,
                 Username = username
            });

            sessionid = Guid.NewGuid().ToString();
            _sessions.Add(sessionid, username);
            return RegisterStatus.Success;

        }

        public void SafetyCheck()
        {
            Logger.Log("Saving users to disk...");
            string json = JsonConvert.SerializeObject(_users);
            File.WriteAllText("users.json", json);
            Logger.Log("Done.");
        }

        public void Initiate()
        {
            Logger.Log("Session manager is starting...");
            if (!File.Exists("users.json"))
            {
                Logger.Log("No users.json found! Creating...");
                string json = JsonConvert.SerializeObject(_users);
                File.WriteAllText("users.json", json);
            }
            Logger.Log("Loading user data from disk...");
            _users = JsonConvert.DeserializeObject<List<ServerAccount>>(File.ReadAllText("users.json"));
            Logger.Log($"{_sessions.Count} users loaded into RAM.");
            Logger.Log("Session manager's ready.");
        }

        public void Unload()
        {
            Logger.Log("Destroying cached users list...");
            _sessions = null;
            _users = null;
            Logger.Log("Done.");
        }
    }

    public enum RegisterStatus
    {
        Success,
        UsernameTaken,
        OtherError
    }

    public enum LoginStatus
    {
        Success,
        BadUsername,
        BadPassword,
        OtherError
    }
}
