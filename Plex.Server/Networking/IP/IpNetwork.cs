using System;
using System.Linq;
namespace Plex.Server.Networking.IP
{
    public struct IpNetwork
    {
        public readonly uint Address;
        public readonly uint Mask;


        public int AssignableHosts
        {
            get
            {
                return ~(int)Mask - 2;
            }
        }

        public IpNetwork NextNetwork
        {
            get
            {
                return new IpNetwork(Address + ~Mask + 1, Mask);
            }
        }

        public IpNetwork(uint addr, uint mask)
        {
            Address = addr;
            Mask = mask;
        }


        public bool Contains(IpAddress ipaddr)
        {
            return (ipaddr.Address & Mask) == (Address & Mask);
        }

        public override string ToString()
        {

            var oct1 = Address & 0xFF;
            var oct2 = (Address & 0xFF00) >> 8;
            var oct3 = (Address & 0xFF0000) >> 16;
            var oct4 = (Address & 0xFF000000) >> 24;

            var cidr = Mask - ((Mask >> 1) & 0x55555555);
            cidr = (cidr & 0x33333333) + ((cidr >> 2) & 0x33333333);
            cidr = (((cidr + (cidr >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;

            return string.Format("{0}.{1}.{2}.{3}/{4}", oct4, oct3, oct2, oct1, cidr);
        }

        public static IpNetwork Parse(string ip)
        {

            var ipAndCidr = ip.Split('/');

            if (ipAndCidr.Length != 2)
            {
                throw new FormatException();
            }


            var octets = ipAndCidr.First().Split('.');

            var cidr = byte.Parse(ipAndCidr.Last());

            if (octets.Length != 4)
            {
                throw new FormatException("Expected 4 octets in IP address");
            }

            var oct1 = byte.Parse(octets[0]);
            var oct2 = byte.Parse(octets[1]);
            var oct3 = byte.Parse(octets[2]);
            var oct4 = byte.Parse(octets[3]);

            var mask = (uint)((0xFFFFFFFF << (32 - cidr)));

            var addr = (uint)(oct4 | (oct3 << 8) | (oct2 << 16) | (oct1 << 24));

            return new IpNetwork(addr, mask);

        }
    }
}
