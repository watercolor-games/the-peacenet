using System;
using Plex.Server.Networking.IP;
using Plex.Server.Networking.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Plex.Server.Networking.Routing
{
    public class Router : INetworkDevice, IGateway
    {
        /// <summary>
        /// A list of interfaces. Not sure what this is for.
        /// </summary>
        readonly List<RouterInterface> interfaces = new List<RouterInterface>();

        /// <summary>
        /// Retrieve the list of interfaces. 
        /// </summary>
        /// <value>The interfaces.</value>
        public IEnumerable<IInterface> Interfaces
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

        public void ReceivePacket(IInterface iface, Packet packet)
        {
            Console.WriteLine("We received a packet!");
        }

        public void AddInterface(RouterInterface iface)
        {
            interfaces.Add(iface);
        }
    }
}
