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
        private static List<Objects.Loot> Loot = new List<Objects.Loot>();

        public static HackableSystem CurrentHackable { get; private set; } 

        public static Objects.Hackable[] AvailableToHack
        {
            get
            {
                return Hackables.Where(x => Shiftorium.UpgradeInstalled(x.Dependencies) && !Shiftorium.UpgradeInstalled(x.ID)).ToArray();
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
            var fs = new Objects.ShiftFS.Directory();
            fs.Name = data.FriendlyName;
            Objects.ShiftFS.Utils.Mounts.Add(fs);
            var mountid = Objects.ShiftFS.Utils.Mounts.IndexOf(fs);
            Objects.ShiftFS.Utils.Mounts.Remove(fs);
            hsys.Filesystem = fs;
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
            hsys.PortsToUnlock = new List<Port>();
            if (data.SystemType.HasFlag(Objects.SystemType.EmailServer))
                hsys.PortsToUnlock.Add(new Port
                {
                    Value = 25,
                    Name = "SMTP mailserver (unencrypted)",
                    Tier = hsys.Data.LockTier,
                    Locks = GetLocks(hsys.Data.LockTier, hsys.Data.FirewallStrength),
                });
            if (data.SystemType.HasFlag(Objects.SystemType.FileServer))
                hsys.PortsToUnlock.Add(new Port
                {
                    Value = 22,
                    Name = "File Transfer Protocol",
                    Tier = hsys.Data.LockTier,
                    Locks = GetLocks(hsys.Data.LockTier, hsys.Data.FirewallStrength),
                });
            if (data.SystemType.HasFlag(Objects.SystemType.SSHServer))
                hsys.PortsToUnlock.Add(new Port
                {
                    Value = 21,
                    Name = "ShiftSSH server",
                    Tier = hsys.Data.LockTier,
                    Locks = GetLocks(hsys.Data.LockTier, hsys.Data.FirewallStrength),
                });
            if (data.SystemType.HasFlag(Objects.SystemType.Database))
                hsys.PortsToUnlock.Add(new Port
                {
                    Value = 3306,
                    Name = "MySQL database",
                    Tier = hsys.Data.LockTier,
                    Locks = GetLocks(hsys.Data.LockTier, hsys.Data.FirewallStrength),
                });

            CurrentHackable = hsys;
        }

        public static List<PortLock> GetLocks(int tier, int fwallstrength)
        {
            var locks = new List<PortLock>();
            var lckTypes = new List<Type>();
            foreach(var lck in ReflectMan.Types.Where(x=>x.BaseType == typeof(PortLock)))
            {
                var lckAttrib = lck.GetCustomAttributes(false).FirstOrDefault(x => x is LockAttribute) as LockAttribute;
                if(lckAttrib != null)
                {
                    if(lckAttrib.Tier == tier)
                    {
                        lckTypes.Add(lck);
                    }
                }
            }
            if (lckTypes.Count > 0)
            {
                var rnd = new Random();
                for (int i = 0; i < fwallstrength; i++)
                {
                    int _typeindex = rnd.Next(lckTypes.Count);
                    var type = (PortLock)Activator.CreateInstance(lckTypes[_typeindex], lckTypes[_typeindex].GetCustomAttributes(false).FirstOrDefault(x => x is LockAttribute));
                    lckTypes.RemoveAt(_typeindex);
                    locks.Add(type);
                }
            }
            return locks;
        }

        public static void FailHack()
        {
            if (CurrentHackable == null)
                throw new NaughtyDeveloperException("Someone tried to fail a non-existent hack.");
            if (CurrentHackable.IsPwn3d)
                throw new NaughtyDeveloperException("A developer tried to un-pwn a pwn3d hackable.");
            if (!string.IsNullOrWhiteSpace(CurrentHackable.Data.OnHackFailedStoryEvent))
                Story.Start(CurrentHackable.Data.OnHackFailedStoryEvent);
            if (Objects.ShiftFS.Utils.Mounts.Contains(CurrentHackable.Filesystem))
                Objects.ShiftFS.Utils.Mounts.Remove(CurrentHackable.Filesystem);
            CurrentHackable = null;
        }

        public static void Initiate()
        {
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IHackableProvider))))
            {
                var @interface = (IHackableProvider)Activator.CreateInstance(type, null);
                Hackables.AddRange(@interface.GetHackables());
                var lootinfo = @interface.GetLootInfo();
                foreach(var loot in lootinfo)
                {
                    var existing = Loot.FirstOrDefault(x => x.Info.Filename == loot.Filename);
                    if (existing != null)
                        throw new DataConflictException("Data conflict encountered while reading loot data. Two or more loot resources with the filename \"" + loot.Filename + "\" were found. This can cause major bugs and confusion in the game.");
                    var @new = new Objects.Loot(loot, @interface.GetLootFromResource(loot.ResourceId));
                    Loot.Add(@new);
                }
            }

            var hackable = Hackables.FirstOrDefault(x => Hackables.Where(y => x.SystemName == y.SystemName).Count() > 1);
            if(hackable != null)
            {
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more hackables were found with the same hostname \"" + hackable.SystemName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            }
        }
    }

    /// <summary>
    /// An exception which is thrown when a developer deliberately tries to cause a bug.
    /// </summary>
    public class NaughtyDeveloperException : Exception
    {
        /// <summary>
        /// Create a new instance of the <see cref="NaughtyDeveloperException"/>, with the specified message, which will cause Visual Studio to call the person who caused the exception a scrotem. 
        /// </summary>
        /// <param name="message">The message you want to yell at the user.</param>
        public NaughtyDeveloperException(string message) : base(message + " - FIX IT, YOU SCROTEM")
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
        Objects.LootInfo[] GetLootInfo();
        byte[] GetLootFromResource(string resId);
    }

    public class HackableSystem
    {
        public Objects.Hackable Data { get; set; }
        public List<Port> PortsToUnlock { get; set; }
        public bool FirewallCracked { get; set; }
        public Objects.ShiftFS.Directory Filesystem { get; set; }
        public double MillisecondsCountdown { get; set; }
        public bool DoConnectionTimeout { get; set; }
        public bool IsPwn3d { get; set; }
    }

    public class Port
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public int Tier { get; set; }
        public List<PortLock> Locks { get; set; } //not a hackmud thing i promise
    }

    public abstract class PortLock
    {
        public PortLock(LockAttribute attrib)
        {
            Attribute = attrib;
        }

        public LockAttribute Attribute { get; private set; }

        public abstract bool Evaluate(Dictionary<string, object> args);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class LockAttribute : Attribute
    {
        public LockAttribute(string name, string company, int tier)
        {
            Name = name;
            Company = company;
            Tier = tier;
        }

        public int Tier { get; private set; }
        public string Name { get; private set; }
        public string Company { get; private set; }
    }
}
