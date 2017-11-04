using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Engine
{
    [Obsolete("Hacking is mostly a server-side feature.")]
    public static class Hacking
    {
        private static List<HackableSystem> _activeConnections = new List<HackableSystem>();
        public static List<Objects.Hackable> Hackables = new List<Objects.Hackable>();
        private static List<Objects.Exploit> Exploits = new List<Objects.Exploit>();
        private static List<Objects.Payload> Payloads = new List<Objects.Payload>();
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
                return new HackableSystem[] { };//Todo: This should be a server-side call.
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

        public static Hackable GetData(HackableSystem system)
        {
            var data = Hacking.Hackables.FirstOrDefault(x => x.SystemName == system.SystemDescriptor.SystemName);
            return data;
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
                Payloads.AddRange(@interface.GetPayloads());
                Exploits.AddRange(@interface.GetExploits());
                LootFiles.AddRange(@interface.GetLoot());
            }

            var hackable = Hackables.FirstOrDefault(x => Hackables.Where(y => x.SystemName == y.SystemName).Count() > 1);
            if(hackable != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more hackables were found with the same hostname \"" + hackable.SystemName + "\". This is a direct violation of the Plex save system and Shiftorium backend.");
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
    [Obsolete("Hacking is mostly a server-side feature.")]
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

    [Obsolete("Hacking is mostly a server-side feature.")]
    public class DataConflictException : Exception
    {
        public DataConflictException(string message) : base(message)
        {

        }
    }

    [Obsolete("Hacking is mostly a server-side feature.")]
    public interface IHackableProvider
    {
        Objects.Hackable[] GetHackables();
        Objects.Exploit[] GetExploits();
        Objects.Payload[] GetPayloads();
        Objects.Loot[] GetLoot();
        byte[] FindLootBytes(string lootid);
    }

}
