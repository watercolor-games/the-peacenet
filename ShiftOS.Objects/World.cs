using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whoa;

namespace Plex.Objects
{
    public class World
    {
        [Order]
        public Subnet Rogue { get; set; }


        [Order]
        public List<Subnet> Networks { get; set; }

        [Order]
        public int MaxRank { get; set; }
    }

    public class Subnet
    {
        [Order]
        public string Name { get; set; }
        [Order]
        public string FriendlyName { get; set; }
        [Order]
        public string FriendlyDescription { get; set; }

        [Order]
        public float WorldX { get; set; }
        [Order]
        public float WorldY { get; set; }

        [Order]
        public List<HackableSystem> NPCs { get; set; }

        public HackableSystem Bank
        {
            get
            {
                return NPCs.FirstOrDefault(x => x.SystemType.HasFlag(SystemType.Bank));
            }
        }


        public HackableSystem UpgradeRepo
        {
            get
            {
                return NPCs.FirstOrDefault(x => x.SystemType.HasFlag(SystemType.UpgradeDB));
            }
        }
        
        [Order]
        public List<string> AvailableUpgrades { get; set; }

        [Order]
        public List<NetworkTask> Tasks { get; set; }
    }

    public class NetworkTask
    {
        [Order]
        public string TaskID { get; set; }

        [Order]
        public TaskType Type { get; set; }

        [Order]
        public long CompletionValue { get; set; }
    }

    

    public enum TaskType
    {
        GetXUpgrades,
        GetXExperience,
        HackXSystems,
    }
}
