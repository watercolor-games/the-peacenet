using System;
using Plex.Server.Networking.Routing;
using Plex.Server.Networking.IP;
using Plex.Server.Networking.Interfaces;
using System.Collections.Generic;
using Plex.Objects;

namespace Plex.Server
{
    public static class WorldGenerator
    {
        private static NewWorld _world = null;
        private static Random random = new Random();

        public static void Initiate()
        {
            if (_world == null)
            {
                int isps = 5; //start out with 5 ISP nets for now.
                int homesPerISP = 3;

                var w = new NewWorld();
                w.ISPs = new List<ISPInfo>();
                w.Hub = new Router();

                for (int i = 0; i < isps; i++)
                {
                    var isp = IpNetwork.Parse($"{17 + i}.100.0.0/16");
                    var ispInfo = new ISPInfo
                    {
                        Network = $"{17 + i}.100.0.0/16",
                        Name = $"Internet Service Provider {i + 1}",
                        ID = Guid.NewGuid().ToString()
                    };

                    createResidential(isp, w.Hub, homesPerISP);
                    w.ISPs.Add(ispInfo);
                }

                _world = w;
                buildWorldRoutingTables();
            }


        }

        private static void buildWorldRoutingTables()
        {
            var list = new List<IGateway>();

            _world.Hub.BuildRoutingTable(list);

            foreach (IGateway gateway in list)
            {
                var router = gateway as Router;

                if (router != null)
                {
                    router.BuildRoutingTable();
                }
            }

        }

        private static Router createResidential(IpNetwork net, Router link, int netCount)
        {
            var mask = IpAddress.Parse("255.255.255.240").Address;

            var hub = new Router();

            var custNet = new IpNetwork(net.Address, mask);
            for (int i = 0; i < netCount; i++)
            {
                var customerRouterLink = custNet;

                var customerLinkNet = new IpNetwork(custNet.Address, 0xFFFFFFFC);
                var customerPortion = new IpNetwork(custNet.Address + 8, 0xFFFFFFF8);

                var homeRouter = new Switch(customerPortion, hub);

                homeRouter.LinkRouters(hub, customerLinkNet);

                custNet = custNet.NextNetwork;

            }

            var ispLinkNetwork = new IpNetwork(custNet.Address, 0xFFFFFFFC);


            link.LinkRouters(hub, ispLinkNetwork);

            return hub;
        }

    }

    //TODO: MOVE THIS TO PLEX.OBJECTS.WORLD EVENTUALLY PLEASE
    public class NewWorld
    {
        public Router Hub { get; set; }

        public List<ISPInfo> ISPs { get; set; }

        public List<Device> Devices { get; set; }
    }

    public class ISPInfo
    {
        public string ID { get; set; }
        public string Network { get; set; }
        public string Name { get; set; }
    }

    public class Device
    {
        public Save SystemDescriptor { get; set; }
        public SystemType SystemType { get; set; }
        public string IpAddress { get; set; }
        public string ID { get; set; }
    }

    public class UserNetworkDevice : INetworkDevice
    {
        private List<IInterface> interfaces = null;

        public IEnumerable<IInterface> Interfaces
        {
            get
            {
                throw new NotImplementedException();
            }
        }



        public void ReceivePacket(IInterface iface, Packet packet)
        {
            throw new NotImplementedException();
        }
    }

}
