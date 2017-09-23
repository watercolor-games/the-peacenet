using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class WorldManager
    {
        [SessionRequired]
        [ServerMessageHandler("get_world")]
        public static void GetWorld(string session_id, string content, string ip, int port)
        {
            var world = new Plexnet();
            world.Networks = new List<Network>();
            foreach(var net in Program.GameWorld.Networks)
            {
                var wnet = new Network();
                wnet.Name = net.Name;
                wnet.FriendlyName = net.FriendlyName;
                wnet.Description = net.FriendlyDescription;
                wnet.Devices = new List<Device>();
                wnet.X = net.WorldX;
                wnet.Y = net.WorldY;
                foreach(var sys in net.NPCs)
                {
                    wnet.Devices.Add(new Device
                    {
                         Rank = sys.SystemDescriptor.Rank,
                          SystemName = sys.SystemDescriptor.SystemName,
                           SystemType = sys.SystemType,
                           X = sys.X,
                           Y = sys.Y
                    });
                }
                world.Networks.Add(wnet);
            }
            var rnet = Program.GameWorld.Rogue;
            var rrnet = new Network();
            rrnet.Name = rnet.Name;
            rrnet.FriendlyName = rnet.FriendlyName;
            rrnet.Description = rnet.FriendlyDescription;
            rrnet.Devices = new List<Device>();
            rrnet.X = rnet.WorldX;
            rrnet.Y = rnet.WorldY;
            foreach (var sys in rnet.NPCs)
            {
                rrnet.Devices.Add(new Device
                {
                    Rank = sys.SystemDescriptor.Rank,
                    SystemName = sys.SystemDescriptor.SystemName,
                    SystemType = sys.SystemType,
                    X = sys.X,
                    Y = sys.Y
                });
            }
            world.Networks.Add(rrnet);

            Program.SendMessage(new PlexServerHeader
            {
                Message = "world",
                Content = JsonConvert.SerializeObject(world),
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }
    }
}
