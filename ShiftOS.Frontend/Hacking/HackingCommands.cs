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
        [RequiresArgument("exploit")]
        [RequiresArgument("port")]
        public static void Exploit(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            string Port = args["port"].ToString();
            string ExploitName = args["exploit"].ToString();
            var Exploit = Hacking.AvailableExploits.FirstOrDefault(x => x.ID == ExploitName);
            if (Exploit == null)
            {
                Console.WriteLine("[sploitset] invalid exploit.");
                return;
            }
            var ExploitTarget = Hacking.CurrentHackable.PortsToUnlock.FirstOrDefault(x => x.AttachTo == Exploit.EffectiveAgainst);
            if (ExploitTarget == null)
            {
                Console.WriteLine("[sploitset] the connected machine doesn't have that service running.");
                return;
            }
            if (ExploitTarget.Value.ToString() != Port)
            {
                Console.WriteLine("[sploitset] port not open");
                return;
            }
            Hacking.CurrentHackable.VectorsUnlocked.Add(ExploitTarget.AttachTo);
            Console.WriteLine("[sploitset] exploited service");
        }

        [Command("inject")]
        [RequiresArgument("payload")]
        public static void InjectPayload(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[sploitset] not connected");
            }
            string PayloadName = args["payload"].ToString();
            var Payload = Hacking.AvailablePayloads.FirstOrDefault(x => x.ID == PayloadName);
            if (Payload == null)
            {
                Console.WriteLine("[sploitset] invalid payload.");
                return;
            }
            if (!Hacking.CurrentHackable.VectorsUnlocked.Contains(Payload.EffectiveAgainst))
            {
                Console.WriteLine("[sploitset] the connected machine doesn't have that service exploited.");
                return;
            }
            PayloadFunc.DoHackFunction(Payload.Function);
            Hacking.CurrentHackable.PayloadExecuted.Add(Payload);
            Console.WriteLine("[sploitset] injected payload");
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
