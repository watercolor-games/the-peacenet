using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class Rank
    {
        public string Name { get; set; }
        public ulong Experience { get; set; }
        public int UpgradeMax { get; set; }
        public string[] UnlockedUpgrades { get; set; }
        public ulong MaximumCash { get; set; }
    }
}
