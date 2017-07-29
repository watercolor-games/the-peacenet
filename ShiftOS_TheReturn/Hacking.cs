using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public static class Hacking
    {
        private static List<HackableSystem> _activeConnections = new List<HackableSystem>();
        private static List<Objects.Hackable> Hackables = new List<Objects.Hackable>();
        private static List<Objects.Exploit> Exploits = new List<Objects.Exploit>();
        private static List<Objects.Payload> Payloads = new List<Objects.Payload>();
        private static List<Objects.Port> Ports = new List<Objects.Port>();
        private static List<Objects.Loot> LootFiles = new List<Objects.Loot>();

        public static HackableSystem CurrentHackable { get; private set; } 

        public static Objects.Hackable[] AvailableToHack
        {
            get
            {
                return Hackables.Where(x => Shiftorium.UpgradeInstalled(x.Dependencies) && !Shiftorium.UpgradeInstalled(x.ID)).ToArray();
            }
        }

        public static Objects.Exploit[] AvailableExploits
        {
            get
            {
                return Exploits.Where(x => Shiftorium.UpgradeInstalled(x.Dependencies) && !Shiftorium.UpgradeInstalled(x.ID)).ToArray();
            }
        }

        public static Objects.Payload[] AvailablePayloads
        {
            get
            {
                return Payloads.Where(x => Shiftorium.UpgradeInstalled(x.Dependencies) && !Shiftorium.UpgradeInstalled(x.ID)).ToArray();
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
                return _activeConnections.Where(x => x.IsPwn3d).ToArray();
            }
        }

        public static HackableSystem[] TimedConnections
        {
            get
            {
                return _activeConnections.Where(x => x.Data.ConnectionTimeoutLevel > 0&&!x.IsPwn3d).ToArray();
            }
        }

        public static void InitHack(Objects.Hackable data)
        {
            var hsys = new HackableSystem();
            hsys.Data = data;
            hsys.IsPwn3d = false;
            hsys.FirewallCracked = (data.FirewallStrength == 0);
            hsys.DoConnectionTimeout = (data.ConnectionTimeoutLevel > 0);
            if (hsys.DoConnectionTimeout)
            {
                hsys.MillisecondsCountdown = 1000 * (240 / data.ConnectionTimeoutLevel);
            }
            else
            {
                hsys.MillisecondsCountdown = 0;
            }
            hsys.PortsToUnlock = new List<Objects.Port>();
            hsys.VectorsUnlocked = new List<Objects.SystemType>();
            hsys.PayloadExecuted = new List<Objects.Payload>();
            hsys.ServerFTPLoot = new List<Objects.Loot>();
            foreach(Objects.Port porttocheck in Ports)
            {
                if (data.SystemType.HasFlag(porttocheck.AttachTo))
                    hsys.PortsToUnlock.Add(porttocheck);
            }
            var rnd = new Random();
            List<Objects.Loot> loot = new List<Objects.Loot>();
            loot.AddRange(LootFiles.Where(x => x.Rarity <= hsys.Data.LootRarity));
            var amount = data.LootAmount;
            for (int i = 0; i < amount; i++)
            {
                int idx = rnd.Next(0, loot.Count - 1);
                hsys.ServerFTPLoot.Add(loot[idx]);
                loot.RemoveAt(idx);
            }
            CurrentHackable = hsys;
        }

        public static void FailHack()
        {
            if (CurrentHackable == null)
                throw new NaughtyDeveloperException("Someone tried to fail a non-existent hack.");
            if (CurrentHackable.IsPwn3d)
                throw new NaughtyDeveloperException("A developer tried to un-pwn a pwn3d hackable.");
            Console.WriteLine("[sploitset] [FAIL] disconnected - connection terminated by remote machine ");
            if (!string.IsNullOrWhiteSpace(CurrentHackable.Data.OnHackFailedStoryEvent))
                Story.Start(CurrentHackable.Data.OnHackFailedStoryEvent);
            CurrentHackable = null;
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
            if (!string.IsNullOrWhiteSpace(CurrentHackable.Data.OnHackCompleteStoryEvent))
                Story.Start(CurrentHackable.Data.OnHackCompleteStoryEvent);
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
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more hackables were found with the same hostname \"" + hackable.SystemName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            var ports = Ports.FirstOrDefault(x => Ports.Where(y => x.Name == y.Name).Count() > 1);
            if (ports != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more ports were found with the same name \"" + ports.Name + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            var payloads = Payloads.FirstOrDefault(x => Payloads.Where(y => x.PayloadName == y.PayloadName).Count() > 1);
            if (payloads != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more payloads were found with the same name \"" + payloads.PayloadName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            var exploits = Exploits.FirstOrDefault(x => Exploits.Where(y => x.ExploitName == y.ExploitName).Count() > 1);
            if (exploits != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more exploits were found with the same name \"" + exploits.ExploitName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            var loot = LootFiles.FirstOrDefault(x => LootFiles.Where(y => x.LootName == y.LootName).Count() > 1);
            if (loot != null)
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more loot files were found with the same name \"" + loot.LootName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
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
    }

    public class HackableSystem
    {
        public Objects.Hackable Data { get; set; }
        public List<Objects.Port> PortsToUnlock { get; set; }
        public List<Objects.SystemType> VectorsUnlocked { get; set; }
        public List<Objects.Payload> PayloadExecuted { get; set; }
        public bool FirewallCracked { get; set; }
        public List<Objects.Loot> ServerFTPLoot { get; set; }
        public double MillisecondsCountdown { get; set; }
        public bool DoConnectionTimeout { get; set; }
        public bool IsPwn3d { get; set; }
    }
}
