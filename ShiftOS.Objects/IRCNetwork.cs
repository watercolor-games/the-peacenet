using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class IRCNetwork
    {
        public string SystemName { get; set; }
        public string FriendlyName { get; set; }
        public string MOTD { get; set; }
        public IRCChannel Channel { get; set; }
    }

    public class IRCChannel
    {
        public string Tag { get; set; }
        public string Topic { get; set; }
        public List<IRCUser> OnlineUsers { get; set; }
        
    }

    public class IRCUser
    {
        public string Nickname { get; set; }
        public IRCPermission Permission { get; set; }
    }

    public enum IRCPermission
    {
        User,
        ChanOp,
        NetOp,
    }
}
