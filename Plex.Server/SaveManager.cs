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
        [ServerMessageHandler("save_read")]
        public static void ReadSave(string session_id, string content, string ip)
        {
            var acct = SessionManager.GrabAccount(session_id);
            if (string.IsNullOrWhiteSpace(acct.SaveID))
            {
                var subnet = Program.GetRandomSubnet();
                int subnetIndex = Program.GameWorld.Networks.IndexOf(subnet);
                var system = Program.GenerateSystem(0, SystemType.Computer, Program.GenerateSystemName(subnet) + "_" + acct.Username);
                system.IsNPC = false;
                system.SystemDescriptor.Username = acct.Username;
                Program.GameWorld.Networks[subnetIndex].NPCs.Add(system);
                acct.SaveID = subnet.Name + "." + system.SystemDescriptor.SystemName;
                SessionManager.SetSessionInfo(session_id, acct);
                Program.SaveWorld();
            }
            var save = Program.GetSaveFromPrl(acct.SaveID);
            Program.SendMessage(new PlexServerHeader
            {
                Content = JsonConvert.SerializeObject(save.SystemDescriptor),
                IPForwardedBy = ip,
                Message = "save_data",
                SessionID = session_id
            });
        }

        [SessionRequired]
        [ServerMessageHandler("save_write")]
        public static void WriteSave(string session_id, string content, string ip)
        {
            var acct = SessionManager.GrabAccount(session_id);
            Save save = JsonConvert.DeserializeObject<Save>(content);
            if (string.IsNullOrWhiteSpace(acct.SaveID))
            {
                var subnet = Program.GetRandomSubnet();
                int subnetIndex = Program.GameWorld.Networks.IndexOf(subnet);
                var system = Program.GenerateSystem(0, SystemType.Computer, Program.GenerateSystemName(subnet) + "_" + acct.Username);
                system.IsNPC = false;
                save.SystemName = system.SystemDescriptor.SystemName;
                system.SystemDescriptor = save;
                system.SystemDescriptor.Username = acct.Username;
                Program.GameWorld.Networks[subnetIndex].NPCs.Add(system);
                acct.SaveID = subnet.Name + "." + system.SystemDescriptor.SystemName;
                SessionManager.SetSessionInfo(session_id, acct);
                Program.SaveWorld();
            }
            Program.GetSaveFromPrl(acct.SaveID).SystemDescriptor = save;
        }

    }
}
