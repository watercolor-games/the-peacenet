using System;
namespace Plex.Server.Networking.IP
{
    public struct IpAddress
    {
        public readonly uint Address;

        public IpAddress(uint addr)
        {
            Address = addr;
        }

                public bool IsInNetwork(IpAddress ipaddr, uint netmask)
        {
            return (ipaddr.Address & netmask) == (Address & netmask);
        }

        public static IpAddress Parse(string ip)
        {
            var octets = ip.Split('.');

            var oct1 = byte.Parse(octets[0]);
            var oct2 = byte.Parse(octets[1]);
            var oct3 = byte.Parse(octets[2]);
            var oct4 = byte.Parse(octets[3]);

            var addr = (uint)(oct4 | (oct3 << 8) | (oct2 << 16) | (oct1 << 24));

            return new IpAddress(addr);
        }



        public override string ToString()
        {

            var oct1 = Address & 0xFF;
            var oct2 = (Address & 0xFF00) >> 8;
            var oct3 = (Address & 0xFF0000) >> 16;
            var oct4 = (Address & 0xFF000000) >> 24;

            return string.Format("{0}.{1}.{2}.{3}", oct4, oct3, oct2, oct1);
        }

        public static bool operator ==(IpAddress left, IpAddress right)
        {
            return left.Address == right.Address;
        }

        public static bool operator !=(IpAddress left, IpAddress right)
        {
            return left.Address != right.Address;
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

    }
}
