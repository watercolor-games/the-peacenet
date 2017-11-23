using System;
using Plex.Server.Networking.Interfaces;
using Plex.Server.Networking.IP;
using System.Collections.Generic;
using System.Linq;

namespace Plex.Server.Networking.Routing
{
    public class Switch : Router
    {
        public readonly IpNetwork Network;

        uint lastAssignedIp = 2;

        readonly Router gateway;
        readonly List<INetworkDevice> devices = new List<INetworkDevice>();
          
        public Switch(IpNetwork network, Router gateway)
        {
            Network = network;
            this.gateway = gateway;
        }


        public void AddDevice(INetworkDevice device)
        {
            devices.Add(device);
        }

        public override void ForwardPacket(IInterface originatingIface, Packet packet)
        {

            if (Network.Contains(packet.Source))
            {
                Console.WriteLine(packet.Source);

                gateway.ForwardPacket(originatingIface, packet);

                return;
            }

            if (Network.Contains(packet.Destination))
            {
                var suitableInterfaces = devices.SelectMany(p => p.Interfaces).Where(p => p.Address == packet.Destination).FirstOrDefault();

                suitableInterfaces.ReceivePacket(null, packet);

                return;
            }
        }

        public IpAddress GetFreeIpAddress()
        {
            return new IpAddress(Network.Address + lastAssignedIp++);
        }
    }
}
