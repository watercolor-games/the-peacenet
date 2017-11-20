using System;
using Plex.Server.Networking.IP;
using Plex.Server.Networking.Interfaces;

namespace Plex.Server.Networking.Routing
{
    public class RouterInterface : IInterface
    {
        readonly Router owner;

        public IInterface AdjacentInterface
        {
            get;
            set;
        }

        public IpAddress Address
        {
            get;
            private set;
        }

        public IpNetwork NetworkMask
        {
            get;
            private set;
        }

        public IGateway DefaultGateway
        {
            get
            {
                return owner;
            }
        }

        public RouterInterface(Router router, IpAddress addr, IpNetwork network)
        {
            owner = router;
            Address = addr;
            NetworkMask = network;
        }

        public void ReceivePacket(IInterface iface, Packet packet)
        {

            if (packet.Destination == Address)
            {
                owner.ReceivePacket(this, packet);
                return;
            }

            if (iface == AdjacentInterface)
            {
                owner.ForwardPacket(this, packet);
            }
            else
            {
                AdjacentInterface.ReceivePacket(this, packet);
            }
        }

        public bool CanReachNetwork(IpAddress addr)
        {
            return false;
        }

    }
}
