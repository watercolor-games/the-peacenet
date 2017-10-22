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
        public static void GetDB(string session_id, BinaryReader reader, BinaryWriter writer)
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
            writer.Write((byte)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(JsonConvert.SerializeObject(upgDb));
        }
    }
}
