using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class Plexnet
    {
        public List<Network> Networks { get; set; }
    }

    public class Network
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }

        public List<Device> Devices { get; set; }
    }

    public class Device
    {
        public string SystemName { get; set; }
        public SystemType SystemType { get; set; }
        public int Rank { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
