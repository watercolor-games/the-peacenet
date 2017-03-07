using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.WinForms.Applications;

namespace ShiftOS.WinForms
{
    [Namespace("l337")]
    [RequiresUpgrade("hacker101_deadaccts")]
    public static class HackerCommands
    {
        private static void writeSlow(string text)
        {
            Console.Write("[hacker101@undisclosed]: ");
            foreach(var c in text.ToCharArray())
            {
                Console.Write(c);
                Thread.Sleep(125);
            }
            Console.WriteLine();
            Thread.Sleep(1000);
        }

        [Story("hacker101_deadaccts")]
        public static void DeadAccountsStory()
        {
            if (!terminalIsOpen())
            {
                AppearanceManager.SetupWindow(new Terminal());
            }

            var t = new Thread(() =>
            {
                Console.WriteLine("[sys@mud]: Warning: User connecting to system...");
                Thread.Sleep(75);
                Console.WriteLine("[sys@mud]: UBROADCAST: Username: hacker101 - Sysname: undisclosed");
                Thread.Sleep(50);
                Console.Write("--locking mud connection resources...");
                Thread.Sleep(50);
                Console.WriteLine("...done.");
                Console.Write("--locking user input... ");
                Thread.Sleep(75);
                TerminalBackend.PrefixEnabled = false;
                TerminalBackend.InStory = true;
                Console.WriteLine("...done.");

                Thread.Sleep(2000);
                writeSlow($"Hello there, fellow multi-user domain user.");
                writeSlow("My name, as you can tell, is hacker101.");
                writeSlow("And yours must be... don't say it... it's " + SaveSystem.CurrentSave.Username + "@" + SaveSystem.CurrentSave.SystemName + ", right?");
                writeSlow("Of course it is.");
                writeSlow("And I bet you 10,000 Codepoints that you have... " + SaveSystem.CurrentSave.Codepoints.ToString() + " Codepoints.");
                writeSlow("Oh, and how much upgrades have you installed since you first started using ShiftOS?");
                writeSlow("That would be... uhh... " + SaveSystem.CurrentSave.CountUpgrades().ToString() + ".");
                writeSlow("I'm probably freaking you out right now. You are probably thinking that you're unsafe and need to lock yourself down.");
                writeSlow("But, don't worry, I mean no harm.");
                writeSlow("In fact, I am a multi-user domain safety activist and security professional.");
                writeSlow("I need your help with something.");
                writeSlow("Inside the multi-user domain, every now and then these 'dead' user accounts pop up.");
                writeSlow("They're infesting everything. They're in every legion, they infest chatrooms, and they take up precious hard drive space.");
                writeSlow("Eventually there's going to be tons of them just sitting there taking over the MUD. We can't have that.");
                writeSlow("It sounds like a conspiracy theory indeed, but it's true, in fact, these dead accounts hold some valuable treasures.");
                writeSlow("I'm talking Codepoints, skins, documents, the possibilities are endless.");
                writeSlow("I'm going to execute a quick sys-script that will show you how you can help get rid of these accounts, and also gain some valuable resources to help you on your digital frontier.");
                writeSlow("This script will also show you the fundamentals of security exploitation and theft of resources - which if you want to survive in the multi-user domain is paramount.");
                writeSlow("Good luck.");
                Thread.Sleep(1000);
                Console.WriteLine("--user disconnected");
                Thread.Sleep(75);
                Console.WriteLine("--commands unlocked - check sos.help.");
                Thread.Sleep(45);
                SaveSystem.SaveGame();
                Console.Write("--unlocking user input...");
                Thread.Sleep(75);
                Console.Write(" ..done");
                TerminalBackend.InStory = false;
                TerminalBackend.PrefixEnabled = true;
                Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                StartHackerTutorial();
            });
            t.IsBackground = true;
            t.Start();
        }

        internal static void StartHackerTutorial()
        {
            //nyi
        }

        private static bool terminalIsOpen()
        {
            foreach(var win in AppearanceManager.OpenForms)
            {
                if (win.ParentWindow is Terminal)
                    return true;
            }
            return false;
        }
    }

    [Namespace("storydev")]
    public static class StoryDevCommands
    {
        [Command("start", description = "Starts a story plot.", usage ="id:string")]
        [RequiresArgument("id")]
        [RemoteLock]
        public static bool StartStory(Dictionary<string, object> args)
        {
            Story.Start(args["id"].ToString());
            return true;
        }

        [Command("unexperience", description = "Marks a story plot as not-experienced yet.", usage ="id:string")]
        [RemoteLock]
        [RequiresArgument("id")]
        public static bool Unexperience(Dictionary<string, object> args)
        {
            string id = args["id"].ToString();
            if (SaveSystem.CurrentSave.StoriesExperienced.Contains(id))
            {
                Console.WriteLine("Unexperiencing " + id + ".");
                SaveSystem.CurrentSave.StoriesExperienced.Remove(id);
                SaveSystem.SaveGame();
            }
            else
            {
                Console.WriteLine("Story ID not found.");
            }

            return true;
        }
    }
}
