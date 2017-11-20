using System;
namespace Plex.Server.Networking.IP
{
    public struct Packet
    {
        public readonly IpAddress Destination;
        public readonly IpAddress Source;


        public readonly byte[] Content;

        public Packet(IpAddress dest, IpAddress src, byte[] content)
        {
            Destination = dest;
            Source = src;
            Content = content;
        }
    }
}
