using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Stories
{
    public static class LegionStory
    {
        private static string CharacterName = "DevX";
        private static string SysName = "mud";

        [Story("victortran_shiftnet")]
        public static void ShiftnetStoryFeaturingTheBlueSmileyFaceHolyFuckThisFunctionNameIsLong()
        {
            CharacterName = "aiden";
            SysName = "appscape_main";
            bool waiting = false;
            var installer = new Applications.Installer();
            installer.InstallCompleted += () =>
            {
                Desktop.InvokeOnWorkerThread(() =>
                {
                    AppearanceManager.Close(installer);
                });
                waiting = false;
            };

            if (!terminalOpen())
            {
                var term = new Applications.Terminal();
                AppearanceManager.SetupWindow(term);
            }

            WriteLine("aiden@appscape_main - user connecting to your system.", false);
            Thread.Sleep(2000);
            WriteLine("Hello there! My name's Aiden Nirh.");
            WriteLine("I run a small Shiftnet website known simply as \"Appscape\".");
            WriteLine("Oh - wait... you don't know what the Shiftnet is...");
            WriteLine("Well, the Shiftnet is like... a private Internet, only accessible through ShiftOS.");
            WriteLine("It has many sites and companies on it - banks, software centres, service providers, you name it.");
            WriteLine("Appscape is one of them. I host many applications on Appscape, from games to utilities to productivity programs, and anything in between. If it exists as a ShiftOS program, it's either on the Shiftorium or Appscape.");
            WriteLine("I'm going to assume you're interested... and I'll install the Shiftnet just in case.");
            WriteLine("Beginning installation of Shiftnet...");
            //Set up an Installer.
            waiting = true;
            Desktop.InvokeOnWorkerThread(() =>
            {
                SaveSystem.CurrentSave.StoriesExperienced.Add("installer");
                SaveSystem.CurrentSave.StoriesExperienced.Add("downloader");

                while (!Shiftorium.UpgradeInstalled("installer"))
                    Thread.Sleep(20);
                AppearanceManager.SetupWindow(installer);
                installer.InitiateInstall(new ShiftnetInstallation());
            });
            while (waiting == true)
                Thread.Sleep(25);

            WriteLine("All good to go! Once I disconnect, type win.open to see a list of your new apps.");
            WriteLine("I've installed everything you need, for free.");
            WriteLine("You've got the Downloader, a simple application to help you track Shiftnet file downloads...");
            WriteLine("...the Installer which will help you unpack programs from .stp files and install them to your system...");
            WriteLine("...and lastly, the Shiftnet browser itself. This program lets you browse the Shiftnet, much like you would the real Internet.");
            WriteLine("I'd stay on the shiftnet/ cluster though, because although there are many services on the Shiftnet, some of them may try to scam you into losing loads of Codepoints, or worse, wrecking your system with viruses.");
            WriteLine("If you want a nice list of safe Shiftnet services, head to ShiftSoft's \"Ping\" site, at shiftnet/shiftsoft/ping.");
            WriteLine("Oh, also, the Shiftnet, much like the real internet, is not free.");
            WriteLine("It requires a service provider. Service providers cost a fair amount of Codepoints, but if you want to get faster speeds and more reliable connections on the Shiftnet, finding a good service provider is a necessity.");
            WriteLine("Right now, you are on ShiftSoft's free trial plan, Freebie Solutions, which gives you access to the Shiftnet however you are locked at 256 bytes per second file downloads and can't leave the shiftnet/ cluster.");
            WriteLine("It's enough to get you started - you'll want to find a faster provider though..");
            WriteLine("Anyways, that's all I'll say for now. Have fun on the Shiftnet. I have to go work on something.");
            WriteLine("One of my friends'll contact you once you've gotten a new service provider.");



            Story.Context.MarkComplete();
            Story.Start("aiden_shiftnet2");
        }

        [Story("hacker101_breakingbonds_1")]
        public static void BreakingTheBondsIntro()
        {
            CharacterName = "hacker101";
            SysName = "pebcak";
            
            if (!terminalOpen())
            {
                var term = new Applications.Terminal();
                AppearanceManager.SetupWindow(term);
            }

            WriteLine("hacker101@pebcak - user connecting to your system.", false);
            Thread.Sleep(2000);
            WriteLine("Greetings, user.");
            WriteLine("My name is hacker101. I have a few things to show you.");
            WriteLine("Before I can do that, however, I need you to do a few things.");
            WriteLine("I'll assign what I need you to do as an objective. When you're done, I'll tell you what you need to know.");

            Story.Context.MarkComplete();
            TerminalBackend.PrefixEnabled = true;

            Story.PushObjective("Breaking the Bonds: Errand Boy", @"hacker101 has something he needs to show you, however before he can, you need to do the following:

 - Buy ""TriWrite"" from Appscape
 - Buy ""Address Book"" from Appscape
 - Buy ""SimpleSRC"" from Appscape", () =>
            {
                bool flag1 = Shiftorium.UpgradeInstalled("address_book");
                bool flag2 = Shiftorium.UpgradeInstalled("triwrite");
                bool flag3 = Shiftorium.UpgradeInstalled("simplesrc");
                return flag1 && flag2 && flag3;
            }, () =>
            {
                Story.Context.MarkComplete();
                SaveSystem.SaveGame();
                Story.Start("hacker101_breakingbonds_2");
            });
            Story.Context.AutoComplete = false;
        }

        [Story("aiden_shiftnet2")]
        public static void AidenShiftnet2()
        {
            Story.PushObjective("Register with a new Shiftnet service provider.", "You've just unlocked the Shiftnet, which has opened up a whole new world of applications and features for ShiftOS. Before you go nuts with it, you may want to register with a better service provider than Freebie Solutions.", () =>
            {
                return SaveSystem.CurrentSave.ShiftnetSubscription != 0;
            },
            () =>
            {
                Story.Context.MarkComplete();
                SaveSystem.SaveGame();
                TerminalBackend.PrintPrompt();
                Story.Start("hacker101_breakingbonds_1");
            });
            Story.Context.AutoComplete = false;
        }

        private static void WriteLine(string text, bool showCharacterName=true)
        {
            Console.WriteLine();
            if (showCharacterName == true)
            {
                ConsoleEx.Bold = true;
                ConsoleEx.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(CharacterName);
                ConsoleEx.OnFlush?.Invoke();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("@");
                ConsoleEx.OnFlush?.Invoke();
                ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(SysName + ": ");
                ConsoleEx.OnFlush?.Invoke();
            }
            ConsoleEx.ForegroundColor = ConsoleColor.Gray;
            ConsoleEx.Bold = false;

            foreach(var c in text)
            {
                Console.Write(c);
                ConsoleEx.OnFlush?.Invoke();
                Thread.Sleep(5);
            }
            Thread.Sleep(1000);
        }

        public static bool terminalOpen()
        {
            foreach(var win in AppearanceManager.OpenForms)
            {
                if (win.ParentWindow is Applications.Terminal)
                    return true;
            }
            return false;
        }

        [Story("mud_control_centre")]
        public static void MCC_Placeholder()
        {
            //What a way to create unbuyable, engine-managed Shiftorium Upgrades... :P
        }

        /// <summary>
        /// Stub: Used for story-driven Shiftorium dependency "installer".
        /// </summary>
        [Story("installer")]
        public static void InstallerPlaceholder()
        {

        }

        /// <summary>
        /// Stub: Used for story-driven Shiftorium dependency: "downloader"
        /// </summary>
        [Story("downloader")]
        public static void DownloaderPlaceholder()
        {

        }

        public class ShiftnetInstallation : Applications.Installation
        {
            protected override void Run()
            {
                SetStatus("Preparing to install dependency: installer_user_agent");
                SetProgress(0);
                Thread.Sleep(5000);
                for(int i = 0; i < 100; i++)
                {
                    SetStatus("Installing installer_user_agent");
                    SetProgress(i);
                    Thread.Sleep(50);
                }
                SetProgress(0);
                SetStatus("Preparing to install dependency: downloader");
                Thread.Sleep(3500);
                for(int i = 0; i < 100; i++)
                {
                    SetStatus("Installing dependency: downloader");
                    SetProgress(i);
                    Thread.Sleep(100);
                }
                SaveSystem.CurrentSave.StoriesExperienced.Add("downloader");
                SetProgress(0);
                SetStatus("Dependencies installed.");
                Thread.Sleep(2000);
                SetStatus("Installing Shiftnet.");
                Thread.Sleep(3000);
                for(int i = 0; i < 100; i++)
                {
                    SetProgress(i);
                    string dots = "";
                    if ((i % 2) == 0)
                        dots = ".";
                    if ((i % 3) == 0)
                        dots = "..";
                    if ((i % 4) == 0)
                        dots = "...";
                    SetStatus($"Installing Shiftnet{dots}");
                    Thread.Sleep(100);
                }

            }
        }
    }
}
