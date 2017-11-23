using System;
using Plex.Server.Networking.IP;
namespace Plex.Server.Networking.Interfaces
{
    public interface IGateway
    {
        void ForwardPacket(IInterface origin, Packet packet);
    }
}
