using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;
using Newtonsoft.Json;

namespace Plex.Server
{
    public static class SaveManager
    {
        [SessionRequired]
        [ServerMessageHandler( ServerMessageType.UPG_GETUPGRADES)]
        public static byte GetDB(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            List<string> upgrades = new List<string>();
            var session = SessionManager.GrabAccount(session_id);
            if(session != null)
            {
                string net = session.SaveID.Substring(0, session.SaveID.IndexOf("."));
                var netdata = Program.GameWorld.Networks.FirstOrDefault(x => x.Name == net);
                if(netdata != null)
                {
                    if (netdata.AvailableUpgrades == null)
                        netdata.AvailableUpgrades = new List<string>();
                    upgrades.AddRange(netdata.AvailableUpgrades);
                }
            }

            writer.Write(upgrades.Count);
            foreach(var upg in upgrades)
            {
                writer.Write(upg);
            }
            return 0x00;
        }
    }
}
