using System;
using Plex.Objects;
using System.Collections.Generic;
using System.IO;
using Whoa;
using System.Text;
using System.Linq;

namespace Peacenet.Backend.Saves
{
    public class SaveManager : IBackendComponent
    {
        private List<Save> _userSaves = new List<Save>();
        private readonly byte[] savemagic = Encoding.UTF8.GetBytes("S4V3");

        public void AddSave(string username)
        {
            var existing = _userSaves.FirstOrDefault(x => x.Username == username);
            if (existing != null)
                throw new InvalidOperationException("A save with that username already exists.");
            Logger.Log($"Creating new save for {username}.");
            existing = new Save
            {
                Cash = 0,
                Experience = 0,
                IsSandbox = false,
                LoadedUpgrades = new List<string>(),
                MaxLoadedUpgrades = 5,
                NetworkTasks = null,
                PickupPoint = "",
                Rank = 0,
                StoriesExperienced = new List<string>(),
                SystemName = "deprecated",
                Transactions = new List<CashTransaction>(),
                Upgrades = new Dictionary<string, bool>(),
                Username = username
            };
            _userSaves.Add(existing);
            Logger.Log("Done.");
        }

        public Save GetSave(string username)
        {
            Logger.Log("Retrieving save for " + username + "...");
            return _userSaves.FirstOrDefault(x => x.Username == username);
        }

        public void Initiate()
        {
            Logger.Log("Checking for save directory...");
            if (!Directory.Exists("saves"))
            {
                Logger.Log("Creating save directory...");
                Directory.CreateDirectory("saves");
                Logger.Log("Done.");
            }
            Logger.Log("Loading user saves...");
            foreach (var file in Directory.GetFiles("saves"))
            {
                using (var fs = File.OpenRead(file))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        byte[] magic = reader.ReadBytes(4);
                        if (!magic.SequenceEqual(savemagic))
                            continue; //not a save file.
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);
                        using (var ms = new MemoryStream(data))
                        {
                            var save = Whoa.Whoa.DeserialiseObject<Save>(ms);
                            Logger.Log($"Loaded save: {save.Username}");
                            _userSaves.Add(save);
                        }
                    }
                }
            }
            Logger.Log($"Done loading saves. {_userSaves.Count} saves loaded.");
        }

        public void SafetyCheck()
        {
            Logger.Log("Saving user saves to disk...");
            foreach (var save in _userSaves)
            {
                string path = Path.Combine("saves", $"{save.Username}.whoa");
                Logger.Log($"Saving {save.Username} to {path}...");
                using (var fs = File.OpenWrite(path))
                {
                    using (var writer = new BinaryWriter(fs))
                    {
                        writer.Write(savemagic);

                        using (var ms = new MemoryStream())
                        {
                            Whoa.Whoa.SerialiseObject<Save>(ms, save);
                            var arr = ms.ToArray();
                            writer.Write(arr.Length);
                            writer.Write(arr);
                        }
                    }
                }
                Logger.Log("Done.");
            }
        }

        public void Unload()
        {
            Logger.Log("Unloading user saves...");
            _userSaves = null;
            Logger.Log("Done.");
        }
    }
}
