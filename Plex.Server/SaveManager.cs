using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;

namespace Plex.Server
{
    public static class SaveManager
    {
        private static readonly byte[] rst5 = Encoding.UTF8.GetBytes("p13x");

        private static void save_system_descriptor(string name, Save data)
        {
            if (!Directory.Exists("saves"))
                Directory.CreateDirectory("saves");

            string path = Path.Combine("saves", name);
            using (var fobj = System.IO.File.OpenWrite(path))
            {
                var writer = new BinaryWriter(fobj);
                writer.Write(rst5);
                using (var memory = new MemoryStream())
                {
                    Whoa.Whoa.SerialiseObject(memory, data);
                    byte[] bdata = memory.ToArray();
                    writer.Write(bdata.Length);
                    writer.Write(bdata);
                }
                writer.Close();
            }

        }

        private static Save new_system_descriptor()
        {
            return new Save
            {
                Experience = 0,
                ID = Guid.NewGuid(),
                IsSandbox = false,
                Language = "english",
                MusicEnabled = false,
                MusicVolume = 0,
                PickupPoint = "",
                ShiftnetSubscription = 0,
                SoundEnabled = false,
                StoriesExperienced = new List<string>(),
                StoryPosition = 0,
                SystemName = "",
                Upgrades = new Dictionary<string, bool>(),
                Username = "",
                ViralInfections = new List<ViralInfection>()
            };

        }

        private static Save read_system_descriptor(string name)
        {
            if (!Directory.Exists("saves"))
                Directory.CreateDirectory("saves");
            string savepath = Path.Combine("saves", name);
            if (!File.Exists(savepath))
                return new_system_descriptor();
            using (var fobj = System.IO.File.OpenRead(savepath))
            {
                var magic = new byte[4];
                var reader = new BinaryReader(fobj);
                magic = reader.ReadBytes(4);
                if (magic.SequenceEqual(rst5))
                {
                    int savelength = reader.ReadInt32();
                    using (var memory = new System.IO.MemoryStream())
                    {

                        var savebytes = new byte[savelength];
                        savebytes = reader.ReadBytes(savebytes.Length);
                        memory.Write(savebytes, 0, savebytes.Length);
                        memory.Position = 0;
                        try
                        {
                            return Whoa.Whoa.DeserialiseObject<Save>(memory);
                        }
                        catch
                        {
                            return new_system_descriptor();
                        }
                    }
                }
                else
                {
                    return new_system_descriptor();
                }
            }

        }

        public static Save SinglePlayerSave
        {
            get
            {
                return read_system_descriptor("singleplayer.save");
            }
        }
    }
}
