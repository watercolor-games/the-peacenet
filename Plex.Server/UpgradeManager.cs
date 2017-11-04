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
        public static List<ShiftoriumUpgrade> Upgrades { get; private set; }

        internal static void Initiate()
        {
            var upgDb = new List<ShiftoriumUpgrade>();
            upgDb.AddRange(JsonConvert.DeserializeObject<ShiftoriumUpgrade[]>(Properties.Resources.upgrades));
            foreach (var type in ReflectMan.Types)
            {


                ShiftoriumUpgradeAttribute attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                if (attrib != null)
                {
                    if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                        throw new ShiftoriumConflictException(attrib.Upgrade);
                    upgDb.Add(new ShiftoriumUpgrade
                    {
                        Name = attrib.Name,
                        Cost = attrib.Cost,
                        Description = attrib.Description,
                        Dependencies = attrib.Dependencies,
                        Category = attrib.Category,
                        Purchasable = attrib.Purchasable,
                        Tutorial = attrib.Tutorial,
                        Rank = attrib.Rank
                    });
                }

                foreach (var mth in type.GetMethods())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable,
                            Tutorial = attrib.Tutorial,
                            Rank = attrib.Rank
                        });

                    }
                }

                foreach (var mth in type.GetFields())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable,
                            Tutorial = attrib.Tutorial,
                            Rank = attrib.Rank
                        });

                    }
                }

                foreach (var mth in type.GetProperties())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable,
                            Tutorial = attrib.Tutorial,
                            Rank = attrib.Rank
                        });

                    }
                }

            }

            foreach (var item in upgDb)
            {
                if (upgDb.Where(x => x.ID == item.ID).Count() > 1)
                    throw new ShiftoriumConflictException(item.ID);
            }
            Upgrades = upgDb;
        }

        [ServerMessageHandler(ServerMessageType.UPG_GETINFO)]
        public static byte GetUpgradeInfo(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string upgid = reader.ReadString();
            var upgdata = Upgrades.FirstOrDefault(x => x.ID == upgid);
            if (upgdata == null)
            {
                writer.Write("The upgrade could not be found.");
                return (byte)ServerResponseType.REQ_ERROR;
            }
            writer.Write(JsonConvert.SerializeObject(upgdata));
            return 0x00;
        }

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
        [UsageString("<id>")]
        public static void LoadUpgradeCMD(Dictionary<string, object> args)
        {
            string content = args["<id>"].ToString();
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
        [ServerMessageHandler(ServerMessageType.UPG_BUY)]
        public static byte BuyUpgrade(string session, BinaryReader reader, BinaryWriter writer)
        {
            string upgradeid = reader.ReadString();
            var upgdata = Upgrades.FirstOrDefault(x => x.ID == upgradeid);
            if (upgdata == null)
            {
                writer.Write("The upgrade could not be found.");
                return (byte)ServerResponseType.REQ_ERROR;
            }

            var sessiondata = SessionManager.GrabAccount(session);
            if (sessiondata == null)
            {
                writer.Write("Generic sessionkeeper error.");
                return (byte)ServerResponseType.REQ_ERROR;
            }
            string net = sessiondata.SaveID.Substring(0, sessiondata.SaveID.IndexOf("."));
            var netdata = Program.GameWorld.Networks.FirstOrDefault(x => x.Name == net);
            if(netdata == null)
            {
                writer.Write("Cannot find network upgrade repository.");
                return (byte)ServerResponseType.REQ_ERROR;
            }
            if (netdata.UpgradeRepo == null)
            {
                writer.Write("Cannot find network upgrade repository.");
                return (byte)ServerResponseType.REQ_ERROR;
            }

            if (netdata.AvailableUpgrades == null)
                netdata.AvailableUpgrades = new List<string>();
            if (!netdata.AvailableUpgrades.Contains(upgradeid))
            {
                writer.Write("The upgrade was not found.");
                return (byte)ServerResponseType.REQ_ERROR;

            }
            string systemtosend = net + "." + netdata.UpgradeRepo.SystemDescriptor.SystemName;
            long amount = (long)upgdata.Cost;
            if(CashManager.CashDeductInternal(session, systemtosend, amount) == false)
            {
                writer.Write("You can't afford this upgrade!");
                return (byte)ServerResponseType.REQ_ERROR;
            }

            netdata.AvailableUpgrades.Remove(upgradeid);

            var save = Program.GetSaveFromPrl(sessiondata.SaveID);

            if (save.SystemDescriptor.Upgrades.ContainsKey(upgradeid))
            {
                save.SystemDescriptor.Upgrades[upgradeid] = true;
            }
            else
            {
                save.SystemDescriptor.Upgrades.Add(upgradeid, true);
            }

            Program.SaveWorld();


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
