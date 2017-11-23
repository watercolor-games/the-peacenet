using System;
using Plex.Server.Networking.IP;

namespace Plex.Server.Networking.Routing
{
    public struct RoutingRule
    {
        public readonly IpNetwork Network;
        public readonly int InterfaceNum;

        public RoutingRule(IpNetwork network, int interfaceNum)
        {
            Network = network;
            InterfaceNum = interfaceNum;
        }
    }
}
