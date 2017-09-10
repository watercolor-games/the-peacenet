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
    }
}
