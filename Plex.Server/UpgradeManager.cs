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
            int result = IsUpgradeInstalled(content, session_id) ? 1 : 0;
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
                if (save.SystemDescriptor.Upgrades == null) //FOR FUCK SAKES WHY DO I KEEP DOING THIS
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
            int result = IsUpgradeLoaded(content, session_id) ? 1 : 0;
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_loaded",
                SessionID = session_id
            });
        }

        internal static bool IsUpgradeInstalled(string upg, string session_id)
        {
            if (string.IsNullOrWhiteSpace(upg))
                return true;

            if(upg.Contains(";"))
            {
                string[] ids = upg.Split(';');
                foreach (var id in ids)
                    if (!IsUpgradeInstalled(id, session_id))
                        return false;
                return true;
            }

            var session = SessionManager.GrabAccount(session_id);
            if (session == null)
                return false;

            var save = Program.GetSaveFromPrl(session.SaveID);
            bool save_world = false;
            if (save.SystemDescriptor.Upgrades == null)
            {
                save.SystemDescriptor.Upgrades = new Dictionary<string, bool>();
                save_world = true;
            }
            if(!save.SystemDescriptor.Upgrades.ContainsKey(upg))
            {
                save.SystemDescriptor.Upgrades.Add(upg, false);
                save_world = true;
            }

            if (save_world)
                Program.SaveWorld();

            return save.SystemDescriptor.Upgrades[upg];
        }

        internal static bool IsUpgradeLoaded(string upg, string session_id)
        {
            if (string.IsNullOrWhiteSpace(upg))
                return true;

            if (upg.Contains(";"))
            {
                string[] ids = upg.Split(';');
                foreach (var id in ids)
                    if (!IsUpgradeLoaded(id, session_id))
                        return false;
                return true;
            }

            if (!IsUpgradeInstalled(upg, session_id))
                return false;

            var session = SessionManager.GrabAccount(session_id);
            if (session == null)
                return false;

            var save = Program.GetSaveFromPrl(session.SaveID);
            bool save_world = false;
            if (save.SystemDescriptor.LoadedUpgrades == null)
            {
                save.SystemDescriptor.LoadedUpgrades = new List<string>();
                save_world = true;
            }

            if (save_world)
                Program.SaveWorld();

            return save.SystemDescriptor.LoadedUpgrades.Contains(upg);
        }

    }
}
