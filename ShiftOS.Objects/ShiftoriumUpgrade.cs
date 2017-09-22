using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class ShiftoriumUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ulong Cost { get; set; }
        public string ID { get { return (Name.ToLower().Replace(" ", "_")); } }
        public string Category { get; set; }
        public bool Purchasable { get; set; }
        public string Tutorial { get; set; }
        public string Dependencies { get; set; }
    }
}
