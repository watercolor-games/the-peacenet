using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class Hackable
    {
        public string SystemName { get; set; }
        public string FriendlyName { get; set; }
        public string Password { get; set; }
        public string PasswordHint { get; set; }
        public string WelcomeMessage { get; set; }

        public string Description { get; set; }

        public int FirewallStrength { get; set; }
        public int LootRarity { get; set; }
        public int LootAmount { get; set; }
        public int ConnectionTimeoutLevel { get; set; }
        public int LockTier { get; set; }

        public SystemType SystemType { get; set; }

        public string OnHackCompleteStoryEvent { get; set; }
        public string OnHackFailedStoryEvent { get; set; }
        
        public string Dependencies { get; set; }


        public string ID
        {
            get
            {
                return SystemName.ToLower().Replace(" ", "_");
            }
        }

        public override string ToString()
        {
            return $"{FriendlyName} ({SystemName})";
        }
    }
    
    [Flags]
    public enum SystemType
    {
        FileServer,
        SSHServer,
        EmailServer,
        Database
    }

    [Serializable]
    public class ServerMessage
    {
        public string Name { get; set; }
        public string GUID { get; set; }
        public string Contents { get; set; }
    }

}
