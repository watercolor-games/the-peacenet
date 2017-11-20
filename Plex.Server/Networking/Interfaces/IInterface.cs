using System;
using Plex.Server.Networking.IP;
namespace Plex.Server.Networking.Interfaces
{
    public interface IInterface
    {
        IpAddress Address { get; }
        IpNetwork NetworkMask { get; }
        IGateway DefaultGateway { get; }

        void ReceivePacket(IInterface iface, Packet packet);
    }
}
