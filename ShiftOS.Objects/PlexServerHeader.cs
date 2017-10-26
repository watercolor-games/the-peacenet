using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class PlexServerHeader
    {
        public byte Message { get; set; }
        public string SessionID { get; set; }
        public byte[] Content { get; set; }
        public string TransactionKey { get; set; }
    }

    public class ServerAccount
    {
        public string SessionID { get; set; }
        public string SaveID { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime LastLogin { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsNPC = false;

        public bool IsBanned = false;
        public DateTime BanLiftDate { get; set; }

        public ACLPermission Permission = ACLPermission.User;

    }

    public class SessionInfo
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string SessionID { get; set; }
    }


}
