using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Frontend
{
    class HackingCommands
    {
        //TODO: Implement firewall cracking
        [Command("connect")]
        [RequiresArgument("id")]
        public static void Connect(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            var hackable = Hacking.AvailableToHack.FirstOrDefault(x => x.ID == id);
            if (hackable == null)
            {
                Console.WriteLine("[sploitset] device not found on network.");
                return;
            }
            Hacking.InitHack(hackable);
        }

        [Command("exploit")]
        [RequiresArgument("id")]
        [RequiresArgument("port")]
        public static void Exploit(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            string Port = args["port"].ToString();
            string ExploitName = args["id"].ToString();
            var ExploitID = Hacking.AvailableExploits.FirstOrDefault(x => x.ID == ExploitName);
            Console.WriteLine(ExploitID.ExploitName);
            if (ExploitID == null)
            {
                Console.WriteLine("[sploitset] invalid exploit.");
                return;
            }
            var ExploitTarget = Hacking.CurrentHackable.PortsToUnlock.First(x => x.Value.ToString() == Port);
            if (ExploitTarget == null)
            {
                Console.WriteLine("[sploitset] port not open");
                return;
            }   
            if (!ExploitTarget.AttachTo.HasFlag(ExploitID.EffectiveAgainst))
            {
                Console.WriteLine("[sploitset] port not exploitable using this exploit");
                return;
            }
            Hacking.CurrentHackable.VectorsUnlocked.Add(ExploitTarget.AttachTo);
            Console.WriteLine("[sploitset] exploited service");
        }

        [Command("inject")]
        [RequiresArgument("id")]
        public static void InjectPayload(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            string PayloadName = args["id"].ToString();
            var PayloadID = Hacking.AvailablePayloads.FirstOrDefault(x => x.ID == PayloadName);
            if (PayloadID == null)
            {
                Console.WriteLine("[sploitset] invalid payload.");
                return;
            }
            if (!Hacking.CurrentHackable.VectorsUnlocked.Contains(PayloadID.EffectiveAgainst))
            {
                Console.WriteLine("[sploitset] the connected machine doesn't have that service exploited.");
                return;
            }
            PayloadFunc.DoHackFunction(PayloadID.Function);
            Hacking.CurrentHackable.PayloadExecuted.Add(PayloadID);
            Console.WriteLine("[sploitset] injected payload");
        }

        [Command("listports")]
        public static void ListPorts(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            foreach (var port in Hacking.CurrentHackable.PortsToUnlock)
            {
                Console.WriteLine(port.Value + ": " + port.FriendlyName);
            }
        }

        [Command("devicescan")]
        public static void ScanDevices()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailableToHack.Length + " devices on the network");
            foreach (var hackable in Hacking.AvailableToHack)
            {
                Console.WriteLine(hackable.ID + ": " + hackable.FriendlyName);
            }
        }

        [Command("exploits")]
        public static void ScanExploits()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailableExploits.Length + " exploits installed");
            foreach (var exploit in Hacking.AvailableExploits)
            {
                Console.WriteLine(exploit.ID + ": " + exploit.FriendlyName);
            }
        }

        [Command("payloads")]
        public static void ListAllPayloads()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailablePayloads.Length + " payloads");
            foreach (var exploit in Hacking.AvailablePayloads)
            {
                Console.WriteLine(exploit.ID + ": " + exploit.FriendlyName);
            }
        }

        [Command("disconnect")]
        public static void Disconnect(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            if (Hacking.CurrentHackable.PayloadExecuted.Count == 0)
            {
                Hacking.FailHack();
                return;
            }
            Hacking.FinishHack();
        }
    }
}
