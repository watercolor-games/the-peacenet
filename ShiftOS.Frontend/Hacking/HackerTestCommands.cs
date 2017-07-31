using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

#if DEBUG
namespace ShiftOS.Frontend
{
    public static class HackerTestCommands
    {
        [ShellConstraint("shiftos_debug> ")]
        [Command("lsports")]
        public static void ListAllPorts()
        {
            foreach (var exploit in Hacking.AvailablePorts)
            {
                Console.WriteLine(exploit.ID + ": " + exploit.FriendlyName);
            }
        }

        [ShellConstraint("shiftos_debug> ")]
        [Command("describehackable")]
        [RequiresArgument("id")]
        public static void DescribeHackable(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            var hackable = Hacking.AvailableToHack.FirstOrDefault(x => x.ID == id);
            if(hackable == null)
            {
                Console.WriteLine("Hackable not found.");
                return;
            }
            Console.WriteLine(hackable.FriendlyName);
            Console.WriteLine("------------------------");
            Console.WriteLine();
            Console.WriteLine("System name: " + hackable.SystemName);
            Console.WriteLine("Loot rarity: " + hackable.LootRarity);
            Console.WriteLine("Loot amount: " + hackable.LootAmount);
            Console.WriteLine("Connection timeout level: " + hackable.ConnectionTimeoutLevel);
            Console.WriteLine();
            Console.WriteLine(hackable.WelcomeMessage);
        }

        [ShellConstraint("shiftos_debug> ")]
        [Command("describeport")]
        [RequiresArgument("id")]
        public static void DescribePort(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            var port = Hacking.AvailablePorts.FirstOrDefault(x => x.ID == id);
            if (port == null)
            {
                Console.WriteLine("Port not found.");
                return;
            }
            Console.WriteLine(port.FriendlyName);
            Console.WriteLine("------------------------");
            Console.WriteLine();
            Console.WriteLine("Port: " + port.Value.ToString());
            Console.WriteLine("Name: " + port.Name);
        }
    }
}
#endif