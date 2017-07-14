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

        public static void Initiate()
        {
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IHackableProvider))))
            {
                var @interface = (IHackableProvider)Activator.CreateInstance(type, null);
                Hackables.AddRange(@interface.GetHackables());

            }

            var hackable = Hackables.FirstOrDefault(x => Hackables.Where(y => x.SystemName == y.SystemName).Count() > 1);
            if(hackable != null)
            {
                throw new DataConflictException("Data conflict encountered while initiating the hacking engine. Two or more hackables were found with the same hostname \"" + hackable.SystemName + "\". This is a direct violation of the ShiftOS save system and Shiftorium backend.");
            }
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
    }

    public class HackableSystem
    {
        public Objects.Hackable Data { get; set; }
        public List<Port> PortsToUnlock { get; set; }
        public bool FirewallCracked { get; set; }
        public Objects.ShiftFS.Directory Filesystem { get; set; }
        public int MillisecondsCountdown { get; set; }
        public bool IsPwn3d { get; set; }
    }

    public class Port
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public int Difficulty { get; set; }
        public bool Cracked { get; set; }
    }
}
