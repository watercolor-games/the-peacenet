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
        [Command("sploitset")]
        public static void SploitSetEnter(Dictionary<string, object> args)
        {
            TerminalBackend.SetShellOverride("sploitset> ");
        }

        [Command("ftp")]
        public static void FTPEnter(Dictionary<string, object> args)
        {
            TerminalBackend.SetShellOverride("SimplFTP> ");
        }

        //TODO: Implement firewall cracking
        [Command("connect")]
        [MetaCommand]
        [RequiresArgument("id")]
        public static void Connect(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            var hackable = Hacking.AvailableToHack.FirstOrDefault(x => x.ID == id);
            if (hackable == null)
            {
                Console.WriteLine("[connectlib] device not found on network.");
                return;
            }
            Hacking.InitHack(hackable);
        }

        [Command("exploit")]
        [ShellConstraint("sploitset> ")]
        [RequiresArgument("id")]
        [RequiresArgument("port")]
        public static void Exploit(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
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
        [ShellConstraint("sploitset> ")]
        [RequiresArgument("id")]
        public static void InjectPayload(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
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
        [ShellConstraint("sploitset> ")]
        public static void ListPorts(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
            }
            foreach (var port in Hacking.CurrentHackable.PortsToUnlock)
            {
                Console.WriteLine(port.Value + ": " + port.FriendlyName);
            }
        }

        [Command("devicescan")]
        [ShellConstraint("sploitset> ")]
        public static void ScanDevices()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailableToHack.Length + " devices on the network");
            foreach (var hackable in Hacking.AvailableToHack)
            {
                Console.WriteLine(hackable.ID + ": " + hackable.FriendlyName);
            }
        }

        [Command("exploits")]
        [ShellConstraint("sploitset> ")]
        public static void ScanExploits()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailableExploits.Length + " exploits installed");
            foreach (var exploit in Hacking.AvailableExploits)
            {
                Console.WriteLine(exploit.ID + ": " + exploit.FriendlyName);
            }
        }

        [Command("payloads")]
        [ShellConstraint("sploitset> ")]
        public static void ListAllPayloads()
        {
            Console.WriteLine("[sploitset] found " + Hacking.AvailablePayloads.Length + " payloads");
            foreach (var exploit in Hacking.AvailablePayloads)
            {
                Console.WriteLine(exploit.ID + ": " + exploit.FriendlyName);
            }
        }

        [Command("disconnect")]
        [MetaCommand]
        public static void Disconnect(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
            }
            if (Hacking.CurrentHackable.PayloadExecuted.Count == 0)
            {
                Hacking.FailHack();
                return;
            }
            Hacking.FinishHack();
        }

        [Command("list")]
        [ShellConstraint("SimplFTP> ")]
        public static void ListAllFTP(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
            }
            var PayloadID = Hacking.CurrentHackable.PayloadExecuted.FirstOrDefault(x => x.EffectiveAgainst.HasFlag(Objects.SystemType.FileServer));
            if (PayloadID == null)
            {
                Console.WriteLine("[SimplFTP] Not authorised.");
                return;
            }
            foreach (var loot in Hacking.CurrentHackable.ServerFTPLoot)
            {
                Console.WriteLine(loot.LootName + ": (assumed: " + loot.FriendlyName + ")" );
            }
        }

        [Command("download")]
        [ShellConstraint("SimplFTP> ")]
        [RequiresArgument("file")]
        public static void DownloadFTP(Dictionary<string, object> args)
        {
            if (Hacking.CurrentHackable == null)
            {
                Console.WriteLine("[connectlib] not connected");
                return;
            }
            var PayloadID = Hacking.CurrentHackable.PayloadExecuted.FirstOrDefault(x => x.EffectiveAgainst.HasFlag(Objects.SystemType.FileServer));
            if (PayloadID == null)
            {
                Console.WriteLine("[SimplFTP] Not authorised.");
                return;
            }
            string FindName = args["file"].ToString();
            var LootID = Hacking.AvailableLoot.FirstOrDefault(x => x.LootName == FindName);
            if (LootID == null)
            {
                Console.WriteLine("[SimplFTP] file not found on server.");
                return;
            }
            if (!Hacking.CurrentHackable.ServerFTPLoot.Contains(LootID))
            {
                Console.WriteLine("[SimplFTP] file not found on server.");
                return;
            }
            if (!Shiftorium.Buy(FindName, 0))
            {
                Console.WriteLine("[SimplFTP] Could not download file. Either the upgrade does not exist or the user doesn't have 0 codepoints (wat)");
                return;
            }
            Console.WriteLine("[SimplFTP] downloaded file");
        }
    }
}
