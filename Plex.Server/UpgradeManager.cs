using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class UpgradeManager
    {
        [SessionRequired]
        [ServerMessageHandler( Objects.ServerMessageType.UPG_ISINSTALLED)]
        public static byte GetInstalled(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string upgid = reader.ReadString();
            bool result = IsUpgradeInstalled(upgid, session_id);
            writer.Write(result);
            return 0x00;
        }


        [SessionRequired]
        [ServerMessageHandler( ServerMessageType.UPG_LOAD)]
        public static byte LoadUpgrade(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            byte result = (byte) UpgradeResult.UNCAUGHT_ERROR;
            string content = reader.ReadString();
            bool installed = IsUpgradeInstalled(content, session_id);
            if(installed == false)
            {
                result = (byte)UpgradeResult.MISSING_UPGRADE;
            }
            else
            {
                bool loaded = IsUpgradeLoaded(content, session_id);
                if(loaded == true)
                {
                    result = (byte)UpgradeResult.ALREADY_LOADED;
                }
                else
                {
                    var session = SessionManager.GrabAccount(session_id);
                    var save = Program.GetSaveFromPrl(session.SaveID);
                    var upgcount = save.SystemDescriptor.MaxLoadedUpgrades;
                    if(save.SystemDescriptor.LoadedUpgrades.Count + 1 >= upgcount)
                    {
                        result = (byte)UpgradeResult.NO_SLOTS;
                    }
                    else
                    {
                        save.SystemDescriptor.LoadedUpgrades.Add(content);
                        result = (byte)UpgradeResult.LOADED;
                        Program.SaveWorld();
                    }
                }
            }

            writer.Write((int)result);
            return 0x00;
        }

        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.UPG_UNLOAD)]
        public static byte UnloadUpgrade(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            var result = UpgradeResult.UNCAUGHT_ERROR;
            string content = reader.ReadString();
            bool installed = IsUpgradeInstalled(content, session_id);
            if (installed == false)
            {
                result = UpgradeResult.MISSING_UPGRADE;
            }
            else
            {
                bool loaded = !IsUpgradeLoaded(content, session_id);
                if (loaded == true)
                {
                    result = UpgradeResult.ALREADY_UNLOADED;
                }
                else
                {
                    var session = SessionManager.GrabAccount(session_id);
                    var save = Program.GetSaveFromPrl(session.SaveID);
                    var upgcount = save.SystemDescriptor.MaxLoadedUpgrades;
                    save.SystemDescriptor.LoadedUpgrades.Remove(content);
                    result = UpgradeResult.UNLOADED;
                    Program.SaveWorld();
                }
            }

            writer.Write((int)result);
            return 0x00;
        }

        [ServerCommand("upgload", "Load the specified upgrade.")]
        [RequiresArgument("id")]
        public static void LoadUpgradeCMD(Dictionary<string, object> args)
        {
            string content = args["id"].ToString();
            string session_id = Terminal.SessionID;
            string result = "Upgrade loaded.";

            bool installed = IsUpgradeInstalled(content, session_id);
            if (installed == false)
            {
                result = "That upgrade was not found on your system.";
            }
            else
            {
                bool loaded = IsUpgradeLoaded(content, session_id);
                if (loaded == true)
                {
                    result = "This upgrade has already been loaded.";
                }
                else
                {
                    var session = SessionManager.GrabAccount(session_id);
                    var save = Program.GetSaveFromPrl(session.SaveID);
                    var upgcount = save.SystemDescriptor.MaxLoadedUpgrades;
                    if (save.SystemDescriptor.LoadedUpgrades.Count + 1 >= upgcount)
                    {
                        result = "You do not have any more upgrade slots.";
                    }
                    else
                    {
                        save.SystemDescriptor.LoadedUpgrades.Add(content);
                        result = "Upgrade loaded.";
                        Program.SaveWorld();
                    }
                }
                
            }

            Console.WriteLine(result);
        }


        [SessionRequired]
        [ServerMessageHandler( ServerMessageType.UPG_SETINSTALLED)]
        public static byte SetUpgradeValue(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string content = reader.ReadString();
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
            return 0x00;
        }


        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.UPG_GETCOUNT)]
        public static byte GetCount(string session_id, BinaryReader reader, BinaryWriter writer)
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
            writer.Write(result);
            return 0x00;
        }


        [SessionRequired]
        [ServerMessageHandler( ServerMessageType.UPG_ISLOADED)]
        public static byte GetLoaded(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string upgid = reader.ReadString();
            bool result = IsUpgradeLoaded(upgid, session_id);
            writer.Write(result);
            return 0x00;
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
            if(save.SystemDescriptor.StoriesExperienced == null)
            {
                save.SystemDescriptor.StoriesExperienced = new List<string>();
                save_world = true;
            }
            if (save.SystemDescriptor.StoriesExperienced.Contains(upg))
            {
                if (save_world)
                    Program.SaveWorld();
                return true;
            }
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
