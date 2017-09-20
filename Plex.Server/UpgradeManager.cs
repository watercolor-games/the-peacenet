using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Server
{
    public static class UpgradeManager
    {
        [SessionRequired]
        [ServerMessageHandler("upgrades_getinstalled")]
        public static void GetInstalled(string session_id, string content, string ip)
        {
            var session = SessionManager.GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            int result = 0;
            if (save != null)
            {
                if(save.SystemDescriptor.Upgrades != null)
                {
                    save.SystemDescriptor.Upgrades = new Dictionary<string, bool>();
                    Program.SaveWorld();
                }
                if(!save.SystemDescriptor.Upgrades.ContainsKey(content))
                {
                    save.SystemDescriptor.Upgrades.Add(content, false);
                    Program.SaveWorld();
                }
                result = save.SystemDescriptor.Upgrades[content] ? 1 : 0;
            }
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_installed",
                SessionID = session_id
            });
        }

        [SessionRequired]
        [ServerMessageHandler("upgrades_getcount")]
        public static void GetCount(string session_id, string content, string ip)
        {
            var session = SessionManager.GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            int result = 0;
            if (save != null)
            {
                if (save.SystemDescriptor.Upgrades != null)
                {
                    save.SystemDescriptor.Upgrades = new Dictionary<string, bool>();
                    Program.SaveWorld();
                }
                result = save.SystemDescriptor.Upgrades.Where(x => x.Value == true).Count();
            }
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_count",
                SessionID = session_id
            });
        }


        [SessionRequired]
        [ServerMessageHandler("upgrades_getloaded")]
        public static void GetLoaded(string session_id, string content, string ip)
        {
            var session = SessionManager.GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(session.SaveID);
            int result = 0;
            if (save != null)
            {
                if (save.SystemDescriptor.LoadedUpgrades == null)
                {
                    save.SystemDescriptor.LoadedUpgrades = new List<string>();
                    Program.SaveWorld();
                }
                result = save.SystemDescriptor.LoadedUpgrades.Contains(content) ? 1 : 0;
            }
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_loaded",
                SessionID = session_id
            });
        }

    }
}
