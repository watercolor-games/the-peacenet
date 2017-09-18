using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Engine
{
    public static class Hacking
    {
        private static List<HackableSystem> _activeConnections = new List<HackableSystem>();
        public static List<Objects.Hackable> Hackables = new List<Objects.Hackable>();
        private static List<Objects.Exploit> Exploits = new List<Objects.Exploit>();
        private static List<Objects.Payload> Payloads = new List<Objects.Payload>();
        private static List<Objects.Port> Ports = new List<Objects.Port>();
        private static List<Objects.Loot> LootFiles = new List<Objects.Loot>();

        public static HackableSystem CurrentHackable { get; private set; } 

        public static Objects.Hackable[] AvailableToHack
        {
            get
            {
                return Hackables.Where(x => Upgrades.UpgradeInstalled(x.Dependencies) && !Upgrades.UpgradeInstalled(x.ID)).ToArray();
            }
        }

        public static Objects.Exploit[] AvailableExploits
        {
            get
            {
                return Exploits.Where(x => Upgrades.UpgradeInstalled(x.Dependencies) && !Upgrades.UpgradeInstalled(x.ID)).ToArray();
            }
        }

        public static Objects.Payload[] AvailablePayloads
        {
            get
            {
                return Payloads.Where(x => Upgrades.UpgradeInstalled(x.Dependencies) && !Upgrades.UpgradeInstalled(x.ID)).ToArray();
            }
        }

        public static Objects.Port[] AvailablePorts
        {
            get
            {
                return Ports.ToArray();
            }
        }

        public static Objects.Loot[] AvailableLoot
        {
            get
            {
                return LootFiles.ToArray();
            }
        }

        public static HackableSystem[] ActiveConnections
        {
            get
            {
                return _activeConnections.ToArray();
            }
        }

        public static HackableSystem[] PwnedConnections
        {
            get
            {
                if (SaveSystem.CurrentSave.CompletedHacks == null)
                    SaveSystem.CurrentSave.CompletedHacks = new List<HackableSystem>();
                return SaveSystem.CurrentSave.CompletedHacks.Where(x => x.IsPwn3d).ToArray();
            }
        }

        public static byte[] GetLootBytes(string lootid)
        {
            foreach (var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IHackableProvider))))
            {
                var @interface = (IHackableProvider)Activator.CreateInstance(type, null);
                var bytes = @interface.FindLootBytes(lootid);
                if (bytes != null)
                    return bytes;
            }
            throw new NaughtyDeveloperException("Seems like the loot system's trying to work without any loot. This isn't normal.");
        }

        public static void BeginHack(HackableSystem _hackable)
        {
            CurrentHackable = _hackable;
        }

        public static void InitHack(Objects.Hackable data)
        {
            if (SaveSystem.CurrentSave.CompletedHacks == null)
                SaveSystem.CurrentSave.CompletedHacks = new List<HackableSystem>();
            var sys = SaveSystem.CurrentSave.CompletedHacks.FirstOrDefault(x => x.SystemDescriptor.SystemName == data.SystemName);
            if (sys == null)
            {
                sys = ProcgenHackable(data);
                SaveSystem.CurrentSave.CompletedHacks.Add(sys);
            }
            CurrentHackable = sys;
        }

        public static void FailHack()
        {
            if (CurrentHackable == null)
                throw new NaughtyDeveloperException("Someone tried to fail a non-existent hack.");
            if (CurrentHackable.IsPwn3d)
                throw new NaughtyDeveloperException("A developer tried to un-pwn a pwn3d hackable.");
            Console.WriteLine();
            Console.WriteLine("[sploitset] [FAIL] disconnected - connection terminated by remote machine ");
            if (!string.IsNullOrWhiteSpace(GetData(CurrentHackable).OnHackFailedStoryEvent))
                Story.Start(GetData(CurrentHackable).OnHackFailedStoryEvent);
            CurrentHackable = null;
            TerminalBackend.SetShellOverride("");
            TerminalBackend.PrintPrompt();
        }

        public static Hackable GetData(HackableSystem system)
        {
            var data = Hacking.Hackables.FirstOrDefault(x => x.SystemName == system.SystemDescriptor.SystemName);
            return data;
        }

        public static HackableSystem ProcgenHackable(Hackable data)
        {
            var sys = new HackableSystem();
            var save = new Save();
            //First we fill in the values we KNOW about this system.
            save.SystemName = data.SystemName;
            save.Rank = data.Rank;

            //Now let's set the systemtype of this system from the data
            sys.SystemType = data.SystemType;

            //Now we get the min and max XP range from the rank
            //this is done by getting the XP value for current rank, and
            //XP value for next rank
            var currentRank = RankManager.GetRankData(save.Rank);
            var nextRank = RankManager.GetRankData(save.Rank + 1); //will return current rank if next rank doesn't exist.
            //We can also get the previous rank for cash gen.
            var prevRank = RankManager.GetRankData(save.Rank - 1);

            //create random number generator
            var rnd = new Random();

            //generate XP
            save.Experience = (ulong)rnd.Next((int)currentRank.Experience, (int)nextRank.Experience);

            //We can do the same for cash.
            save.Cash = (long)rnd.Next((int)prevRank.MaximumCash, (int)currentRank.MaximumCash);

            //Now, using the rank system we can determine
            //what upgrades this system will have loaded.

            //We do this by first generating a random number between 0 and the rank's max upgrade slots...
            int maxUpgrades = rnd.Next(currentRank.UpgradeMax);
            //We'll use this later. Next, we need to initiate this save's
            //Upgrade DB.

            //...Algorithmically.

            save.Upgrades = new Dictionary<string, bool>();

            //So here's how this algorithm will work.

            //While the upgrade DB's count is below that of the upgrades in the engine...
            while(save.Upgrades.Count < Upgrades.AllUpgrades.Count)
            {
                //Grab a list of all upgrades whose dependencies are satisfied by the upgrade DB...
                var satisfied = Upgrades.AllUpgrades.Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.Dependencies))
                        return true;
                    string[] ids = x.Dependencies.Split(';');
                    bool isSatisfied = true;
                    foreach(var id in ids)
                    {
                        if (!save.Upgrades.ContainsKey(id))
                            isSatisfied = false;
                        else if (save.Upgrades[id] == false)
                            isSatisfied = false;
                        if (isSatisfied == false)
                            break;
                    }
                    return isSatisfied;
                });
                //Now we populate the upgrade DB with these upgrades, randomly choosing whether they're installed.
                foreach (var upg in satisfied)
                {
                    bool installed = rnd.Next(0, 100) > 50;
                    if (!save.Upgrades.ContainsKey(upg.ID))
                        save.Upgrades.Add(upg.ID, false);
                    save.Upgrades[upg.ID] = installed;

                }
            }
            //This algorithm is a very neat way of procedurally generating an upgrade DB as if
            //an actual player had bought these upgrades.
            //
            //It only chooses upgrades whose dependencies are satisfied, making it feel more
            //natural to the player.
            //
            //I wrote it myself :D
            //Now we pick random upgrades to actually load.
            save.LoadedUpgrades = new List<string>();
            for (int i = 0; i < maxUpgrades; i++)
            {
                var available = save.Upgrades.Where(x => x.Value == true);
                string id = null;
                while(id == null || !save.LoadedUpgrades.Contains(id))
                {
                    id = available.ToArray()[rnd.Next(0, available.Count())].Key;
                }
                save.LoadedUpgrades.Add(id);
            }
            //set system descriptor
            sys.SystemDescriptor = save;

            //We've procgen'd all we really can.

            return sys;
        }

        public static void EndHack()
        {
            if (CurrentHackable == null)
                throw new NaughtyDeveloperException("Someone tried to end a non-existent hack.");
            Console.WriteLine("[sploitset] [FAIL] disconnected for unknown reason");
            CurrentHackable = null;
        }

        public static void FinishHack()
        {
            if (CurrentHackable == null)
                throw new NaughtyDeveloperException("Someone tried to finish a non-existent hack.");
            if (!string.IsNullOrWhiteSpace(GetData(CurrentHackable).OnHackCompleteStoryEvent))
                Story.Start(GetData(CurrentHackable).OnHackCompleteStoryEvent);
            Console.WriteLine("[sploitset] disconnected with payload applied");
            CurrentHackable = null;
        }

        public static void Initiate()
        {
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IHackableProvider))))
            {
                var @interface = (IHackableProvider)Activator.CreateInstance(type, null);
                Hackables.AddRange(@interface.GetHackables());
                Ports.AddRange(@interface.GetPorts());
                Payloads.AddRange(@interface.GetPayloads());
                Exploits.AddRange(@interface.GetExploits());
                LootFiles.AddRange(@interface.GetLoot());
            }

            var hackable = Hackables.FirstOrDefault(x => Hackables.Where(y => x.SystemName == y.SystemName).Count() > 1);
            if(hackable != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more hackables were found with the same hostname \"" + hackable.SystemName + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
            var ports = Ports.FirstOrDefault(x => Ports.Where(y => x.Name == y.Name).Count() > 1);
            if (ports != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more ports were found with the same name \"" + ports.Name + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
            var payloads = Payloads.FirstOrDefault(x => Payloads.Where(y => x.PayloadName == y.PayloadName).Count() > 1);
            if (payloads != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more payloads were found with the same name \"" + payloads.PayloadName + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
            var exploits = Exploits.FirstOrDefault(x => Exploits.Where(y => x.ExploitName == y.ExploitName).Count() > 1);
            if (exploits != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more exploits were found with the same name \"" + exploits.ExploitName + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
            var loot = LootFiles.FirstOrDefault(x => LootFiles.Where(y => x.LootName == y.LootName).Count() > 1);
            if (loot != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more loot files were found with the same name \"" + loot.LootName + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
        }
    }

    /// <summary>
    /// An exception which is thrown when a developer deliberately tries to cause a bug.
    /// </summary>
    public class NaughtyDeveloperException : Exception
    {
        /// <summary>
        /// Create a new instance of the <see cref="NaughtyDeveloperException"/>, with the specified message, which will cause Visual Studio to call the person who caused the exception a scrotum (and tell them to fix it). 
        /// </summary>
        /// <param name="message">The message you want to yell at the user.</param>
        public NaughtyDeveloperException(string message) : base(message + " - FIX IT, YOU SCROTUM")
        {

        }
    }

    public class DataConflictException : Exception
    {
        public DataConflictException(string message) : base(message)
        {

        }
    }

    public interface IHackableProvider
    {
        Objects.Hackable[] GetHackables();
        Objects.Exploit[] GetExploits();
        Objects.Payload[] GetPayloads();
        Objects.Port[] GetPorts();
        Objects.Loot[] GetLoot();
        byte[] FindLootBytes(string lootid);
    }

}
