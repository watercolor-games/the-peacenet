using System;
using Plex.Server.Networking.IP;
using System.Collections.Generic;
namespace Plex.Server.Networking.Interfaces
{
    public interface INetworkDevice
    {
        IEnumerable<IInterface> Interfaces { get; }

        void ReceivePacket(IInterface iface, Packet packet);
    }
}
