using System;
using System.Collections.Generic;
using System.Linq;

namespace Peacenet.Backend
{
    public class WorldBackend : IBackendComponent
    {
        Random _random = new Random();

        public void Initiate()
        {
            Logger.Log("World manager is starting...");
            int _isps = 30;
            int _customers = 100;
            var worldHub = new Router(Guid.NewGuid().ToString());

            for (int i = 0; i < _isps; i++)
            {
                var isp = IpNetwork.Parse($"{14 + i}.100.0.0/16");
                Logger.Log($"Generating ISP: {isp}");
                _createResidential(isp, worldHub, _customers);
            }
        }

        private Router _createResidential(IpNetwork net, Router link, int netCount)
        {
            var mask = IpAddress.Parse("255.255.255.240").Address;

            var hub = new Router(Guid.NewGuid().ToString());

            var custNet = new IpNetwork(Guid.NewGuid().ToString(), net.Address, mask);
            for (int i = 0; i < netCount; i++)
            {
                var customerRouterLink = custNet;
                var customerLinkNet = new IpNetwork(Guid.NewGuid().ToString(), custNet.Address, 0xFFFFFFFC);
                var customerPortion = new IpNetwork(Guid.NewGuid().ToString(), custNet.Address + 8, 0xFFFFFFF8);

                var homeRouter = new Switch(Guid.NewGuid().ToString(), customerPortion, hub);

                homeRouter.LinkRouters(hub, customerLinkNet);

                custNet = custNet.NextNetwork;

            }

            var ispLinkNetwork = new IpNetwork(Guid.NewGuid().ToString(), custNet.Address, 0xFFFFFFFC);


            link.LinkRouters(hub, ispLinkNetwork);

            return hub;

        }

        public void SafetyCheck()
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }

    public interface IGateway
    {
        void ForwardPacket(IInterface origin, Packet packet);
    }

    public interface IInterface
    {
        IpAddress Address { get; }
        IpNetwork NetworkMask { get; }
        IGateway DefaultGateway { get; }

        void ReceivePacket(IInterface iface, Packet packet);
    }

    public abstract class NetworkDevice
    {
        public string EntityID { get; private set; }

        public NetworkDevice(string entityid)
        {
            EntityID = entityid;
        }

        public abstract IEnumerable<IInterface> Interfaces { get; }

        public abstract void ReceivePacket(IInterface iface, Packet packet);
    }

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

    public struct IpNetwork
    {
        public readonly string EntityID;

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
                return new IpNetwork(Guid.NewGuid().ToString(), Address + ~Mask + 1, Mask);
            }
        }

        public IpNetwork(string entityid, uint addr, uint mask)
        {
            EntityID = entityid;
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

            return new IpNetwork(Guid.NewGuid().ToString(), addr, mask);

        }
    }

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

    public class Router : NetworkDevice, IGateway
    {
        public Router(string entityid) : base(entityid)
        {

        }

        /// <summary>
        /// A list of interfaces. Not sure what this is for.
        /// </summary>
        readonly List<RouterInterface> interfaces = new List<RouterInterface>();

        /// <summary>
        /// Retrieve the list of interfaces. 
        /// </summary>
        /// <value>The interfaces.</value>
        public override IEnumerable<IInterface> Interfaces
        {
            get
            {
                return interfaces;
            }
        }

        /// <summary>
        /// The routing table for this router.
        /// </summary>
        List<RoutingRule> routingTable = new List<RoutingRule>();

        public IEnumerable<IpNetwork> BuildRoutingTable(List<IGateway> blacklist = null)
        {
            var ret = new List<IpNetwork>();

            if (blacklist != null && blacklist.Contains(this))
            {
                return ret;
            }

            if (blacklist == null)
            {
                blacklist = new List<IGateway>();
            }

            blacklist.Add(this);

            for (int i = 0; i < interfaces.Count; i++)
            {
                var iface = interfaces[i];
                var dg = iface.AdjacentInterface.DefaultGateway;

                if (dg == this || !(dg is Router))
                {
                    continue;
                }

                var router = dg as Router;

                routingTable.Add(new RoutingRule(iface.NetworkMask, i));

                foreach (var network in router.BuildRoutingTable(blacklist))
                {
                    routingTable.Add(new RoutingRule(network, i));

                    ret.Add(network);
                }


                if (router is Switch)
                {
                    var network = ((Switch)router).Network;
                    var rule = new RoutingRule(network, i);

                    ret.Add(network);
                    routingTable.Add(rule);
                }

                ret.Add(iface.NetworkMask);
            }

            return ret;

        }

        public void PrintRoutingTable()
        {
            foreach (RoutingRule rule in routingTable)
            {
                Console.WriteLine("{0} -> interface{1}", rule.Network, rule.InterfaceNum);
            }
            Console.WriteLine();
        }

        public virtual void ForwardPacket(IInterface originatingIface, Packet packet)
        {
            int ifaceNum = 0;




            var matchingRules = routingTable.Where(p => p.Network.Contains(packet.Destination));

            foreach (RoutingRule rule in routingTable)
            {
                if (rule.Network.Contains(packet.Destination))
                {
                    var iface = interfaces[rule.InterfaceNum];

                    Console.WriteLine("Hop on {0} (iface #{1})", iface.Address, ifaceNum++);

                    iface.ReceivePacket(originatingIface, packet);
                }
            }
        }

        public void LinkRouters(Router router, IpNetwork address)
        {
            var ip1 = new IpAddress(address.Address + 1);
            var ip2 = new IpAddress(address.Address + 2);


            var iface1 = new RouterInterface(this, ip1, address);
            var iface2 = new RouterInterface(router, ip2, address);

            iface1.AdjacentInterface = iface2;
            iface2.AdjacentInterface = iface1;

            AddInterface(iface1);

            router.AddInterface(iface2);
        }

        public override void ReceivePacket(IInterface iface, Packet packet)
        {
            Console.WriteLine("We received a packet!");
        }

        public void AddInterface(RouterInterface iface)
        {
            interfaces.Add(iface);
        }
    }

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

    public class Switch : Router
    {
        public readonly IpNetwork Network;

        uint lastAssignedIp = 2;

        readonly Router gateway;
        readonly List<NetworkDevice> devices = new List<NetworkDevice>();

        public Switch(string entityid, IpNetwork network, Router gateway) : base(entityid)
        {
            Network = network;
            this.gateway = gateway;
        }


        public void AddDevice(NetworkDevice device)
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
