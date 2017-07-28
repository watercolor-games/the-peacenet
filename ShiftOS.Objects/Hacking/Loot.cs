using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class Loot
    {
        public string FriendlyName { get; set; }
        public string LootName { get; set; }
        public int Rarity { get; set; }
        public string PointTo { get; set; }

        public string ID
        {
            get
            {
                return LootName.ToLower().Replace(" ", "_");
            }
        }

        public override string ToString()
        {
            return $"{FriendlyName} ({LootName})";
        }
    }

}
