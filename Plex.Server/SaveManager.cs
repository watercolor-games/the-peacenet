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
        [ServerMessageHandler("upgrades_getdb")]
        public static void GetDB(string session_id, string content, string ip)
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
                        Purchasable = attrib.Purchasable
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
                            Purchasable = attrib.Purchasable
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
                            Purchasable = attrib.Purchasable
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
                            Purchasable = attrib.Purchasable
                        });

                    }
                }

            }



            foreach (var item in upgDb)
            {
                if (upgDb.Where(x => x.ID == item.ID).Count() > 1)
                    throw new ShiftoriumConflictException(item.ID);
            }
            Program.SendMessage(new PlexServerHeader
            {
                Message = "upgrades_db",
                Content = JsonConvert.SerializeObject(upgDb),
                IPForwardedBy = ip,
                SessionID = session_id
            });
        }

        [SessionRequired]
        [ServerMessageHandler("save_read")]
        public static void ReadSave(string session_id, string content, string ip)
        {
            var acct = SessionManager.GrabAccount(session_id);
            var save = Program.GetSaveFromPrl(acct.SaveID);

            if (string.IsNullOrWhiteSpace(acct.SaveID) || save == null)
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
                save = Program.GetSaveFromPrl(acct.SaveID);
            }
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
            if (string.IsNullOrWhiteSpace(acct.SaveID) || save == null)
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
