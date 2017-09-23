using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plex.Server
{
    public static class UpgradeManager
    {
        [SessionRequired]
        [ServerMessageHandler("upgrades_getinstalled")]
        public static void GetInstalled(string session_id, string content, string ip, int port)
        {
            int result = IsUpgradeInstalled(content, session_id) ? 1 : 0;
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_installed",
                SessionID = session_id
            }, port);
        }

        [SessionRequired]
        [ServerMessageHandler("upgrades_load")]
        public static void LoadUpgrade(string session_id, string content, string ip, int port)
        {
            string result = "uncaught_error";

            bool installed = IsUpgradeInstalled(content, session_id);
            if(installed == false)
            {
                result = "missing_upgrade";
            }
            else
            {
                bool loaded = IsUpgradeLoaded(content, session_id);
                if(loaded == true)
                {
                    result = "already_loaded";
                }
                else
                {
                    var session = SessionManager.GrabAccount(session_id);
                    var save = Program.GetSaveFromPrl(session.SaveID);
                    var upgcount = save.SystemDescriptor.MaxLoadedUpgrades;
                    if(save.SystemDescriptor.LoadedUpgrades.Count + 1 >= upgcount)
                    {
                        result = "no_slots";
                    }
                    else
                    {
                        save.SystemDescriptor.LoadedUpgrades.Add(content);
                        result = "loaded";
                        Program.SaveWorld();
                    }
                }
            }

            Program.SendMessage(new Objects.PlexServerHeader
            {
                Message = "upgrades_loadresult",
                Content = result,
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }

        [SessionRequired]
        [ServerMessageHandler("upgrades_unload")]
        public static void UnloadUpgrade(string session_id, string content, string ip, int port)
        {
            string result = "uncaught_error";

            bool installed = IsUpgradeInstalled(content, session_id);
            if (installed == false)
            {
                result = "missing_upgrade";
            }
            else
            {
                bool loaded = !IsUpgradeLoaded(content, session_id);
                if (loaded == true)
                {
                    result = "already_unloaded";
                }
                else
                {
                    var session = SessionManager.GrabAccount(session_id);
                    var save = Program.GetSaveFromPrl(session.SaveID);
                    var upgcount = save.SystemDescriptor.MaxLoadedUpgrades;
                    save.SystemDescriptor.LoadedUpgrades.Remove(content);
                    result = "unloaded";
                    Program.SaveWorld();
                }
            }

            Program.SendMessage(new Objects.PlexServerHeader
            {
                Message = "upgrades_unloadresult",
                Content = result,
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }


        [SessionRequired]
        [ServerMessageHandler("upgrades_set")]
        public static void SetUpgradeValue(string session_id, string content, string ip, int port)
        {
            var args = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            bool value = (bool)args["value"];
            string id = args["id"].ToString();
            bool isinstalled = IsUpgradeInstalled(id, session_id);
            if(isinstalled != value)
            {
                var session = SessionManager.GrabAccount(session_id);
                var save = Program.GetSaveFromPrl(session.SaveID);
                save.SystemDescriptor.Upgrades[id] = value;
                Program.SaveWorld();
            }
        }


        [SessionRequired]
        [ServerMessageHandler("upgrades_getcount")]
        public static void GetCount(string session_id, string content, string ip, int port)
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
            }, port);
        }


        [SessionRequired]
        [ServerMessageHandler("upgrades_getloaded")]
        public static void GetLoaded(string session_id, string content, string ip, int port)
        {
            int result = IsUpgradeLoaded(content, session_id) ? 1 : 0;
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Content = result.ToString(),
                IPForwardedBy = ip,
                Message = "upgrades_loaded",
                SessionID = session_id
            }, port);
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
            if (save == null)
                return false;
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
            if (save == null)
                return false;
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
