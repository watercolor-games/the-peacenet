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
        [Command("lshackables")]
        public static void ListAllHackables()
        {
            foreach(var hackable in Hacking.AvailableToHack)
            {
                Console.WriteLine(hackable.ID + ": " + hackable.FriendlyName);
            }
        }

        [Command("describebackable")]
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

        [Command("inithack")]
        [RequiresArgument("id")]
        public static void InitHack(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            var hackable = Hacking.AvailableToHack.FirstOrDefault(x => x.ID == id);
            if (hackable == null)
            {
                Console.WriteLine("Hackable not found.");
                return;
            }
            Hacking.InitHack(hackable);
        }
    }
}
#endif