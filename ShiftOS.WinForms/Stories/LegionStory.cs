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
            CharacterName = "victor_tran";
            SysName = "theos";
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

            var t = new Thread(() =>
            {
                WriteLine("victortran@theos - user connecting to your system.", false);
                Thread.Sleep(2000);
                WriteLine("Hey there - yes, I am not just a sentient being. I am indeed Victor Tran.");
                WriteLine("I am the creator of a Linux desktop environment called theShell.");
                WriteLine("I'm in the middle of working on a ShiftOS version of it.");
                WriteLine("However, before I start, I feel I need to show you something.");
                WriteLine("You have a lot of ShiftOS applications, and you can earn lots of Codepoints - and you already have lots.");
                WriteLine("Well, you'd be a perfect candidate for me to install the Shiftnet Client on your system.");
                WriteLine("I'll begin the installation right now.");
                //Set up an Installer.
                waiting = true;
                Desktop.InvokeOnWorkerThread(() =>
                {
                    Story.Start("installer");
                    while (!Shiftorium.UpgradeInstalled("installer"))
                        Thread.Sleep(20);
                    AppearanceManager.SetupWindow(installer);
                    installer.InitiateInstall(new ShiftnetInstallation());
                });
                while (waiting == true)
                    Thread.Sleep(25);

                WriteLine("All installed! Once I disconnect, type win.open to see a list of your new apps.");
                WriteLine("The Shiftnet is a vast network of websites only accessible through ShiftOS.");
                WriteLine("Think of it as the DarkNet, but much darker, and much more secretive.");
                WriteLine("There are lots of apps, games, skins and utilities on the Shiftnet.");
                WriteLine("There are also lots of companies offering many services.");
                WriteLine("I'd stay on the shiftnet/ cluster though, others may be dangerous.");
                WriteLine("I'd also stick to the sites listed on shiftnet/shiftsoft/ping - that site is regularly updated with the most safe Shiftnet sites.");
                WriteLine("Oh, that reminds me. I have set you up with ShiftSoft's service provider, Freebie Solutions.");
                WriteLine("It's a free provider, but it offers only 256 bytes per second download and upload speeds.");
                WriteLine("You may want to go look for another provider when you can afford it.");
                WriteLine("Trust me - with this provider, things get slow.");
                WriteLine("Anyways, it was nice meeting you, hopefully someday you'll give theShell a try.");

                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
            });
            t.IsBackground = true;
            t.Start();

            TerminalBackend.PrefixEnabled = false;

        }

        private static void WriteLine(string text, bool showCharacterName=true)
        {
            Console.WriteLine();
            if (showCharacterName == true)
            {
                ConsoleEx.Bold = true;
                ConsoleEx.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(CharacterName);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("@");
                ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(SysName + ": ");
            }
            ConsoleEx.ForegroundColor = ConsoleColor.Gray;
            ConsoleEx.Bold = false;

            Console.WriteLine(text);
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

        [Story("devx_legions")]
        public static void DevXLegionStory()
        {
            CharacterName = "DevX";
            SysName = "mud";
            bool waiting = false;
            //Used for DevX dialogue.
            //Used for legion selection.
            var mud = new Applications.MUDControlCentre();
            //For installing the MCC
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

            var t = new Thread(() =>
            {
                WriteLine("DevX@mud - user connecting to your system.", false);
                Thread.Sleep(2000);
                WriteLine($"Hello, {SaveSystem.CurrentUser.Username}. It's been a while.");
                WriteLine("My intelligence suggests you've installed all GUI-based Shiftorium upgrades.");
                WriteLine("Bet you're liking ShiftOS now that the terminal isn't the only way you can control it.");
                WriteLine("Well, now it's time to introduce your next task.");
                WriteLine("In the multi-user domain, each user has a reputation value associated with them.");
                WriteLine("Right now, you have a reputation of 0, Neutral.");
                WriteLine("This means that nobody has an opinion on you, yet.");
                WriteLine("What a good time to make your presence known?");
                WriteLine("I have an application for you to try, that will help you out in the multi-user domain, and help you make yourself famous.");
                WriteLine("In the digital society, you can't survive long as a lone sentience. You must kongregate with a group, and these groups are called \"legions\".");
                WriteLine("This application will assist you in finding one. A legion is a group of sentiences who carry out actions together. They're like... a pack of wolves, or a family, if you will.");
                WriteLine("They share Codepoints, documents, applications, and other things with each other.");
                WriteLine("And they all have their own goals.");
                WriteLine("Up until now, you've been blindly following my directions, with the goal of upgrading ShiftOS. Now, it's time for you to join a legion, and fulfill their goals, to become known within the digital society.");

                WriteLine("You'll do all of this through the MUD Control Centre.");
                WriteLine("I'll set it up on your system now.");
                //Set up an Installer.
                waiting = true;
                Desktop.InvokeOnWorkerThread(() =>
                {
                    AppearanceManager.SetupWindow(installer);
                    installer.InitiateInstall(new MCCInstallation());
                });
                while (waiting == true)
                    Thread.Sleep(25);

                WriteLine("There, it's all installed, so let's get you set up with a legion, shall we?");
                Desktop.InvokeOnWorkerThread(() =>
                {
                    AppearanceManager.SetupWindow(mud);
                    mud.ShowClasses();
                    mud.ClassChanged += () =>
                    {
                        waiting = false;
                    };
                });
                WriteLine("First, select a class. A class will help define your personality within the multi-user domain. It'll determine the best legions for you.");
                waiting = true;
                while (waiting == true)
                    Thread.Sleep(25);
                WriteLine($"Your class has been selected. You are a(n) {SaveSystem.CurrentSave.Class}.");
                WriteLine("On this screen, you can see a detailed view of your status within the digital society.");
                WriteLine("You'll see this screen everytime you start the MUD Control Centre.");
                WriteLine("Like the sos.status command, it shows your Codepoints, the upgrades you've bought and the upgrades available, but it also shows your reputation, legion, shops, and various other details.");
                WriteLine("Right now, you are not in any legions. This is about to change.");
                WriteLine("I will open the Legion Selector for you. The best legions will be shown at the top of the list.");
                WriteLine("Pay attention to their Perdominent Class and their Collective Reputation values. These values will indicate how morally correct the legion is, and may affect your personal reputation.");
                Desktop.InvokeOnWorkerThread(() =>
                {
                    mud.ShowLegionSelector();
                    mud.LegionChanged += () =>
                    {
                        waiting = false;
                    };
                });
                waiting = true;
                while (waiting == true)
                    Thread.Sleep(25);
                WriteLine($"So, you've joined the [{SaveSystem.CurrentSave.CurrentLegions[0]}] legion.");
                WriteLine("Now you can see a more detailed view of the legion - who's inside, how many Codepoints the legion has, and you can also join their private chat.");
                WriteLine("It's up to you what you do next. Get acquianted with your new team. I've gotta go work on something.");
                WriteLine("I will contact you as you become more well-known.");
                WriteLine("OH, one more thing.");
                WriteLine("You're probably wondering about your reputation. Well, right now you have a Neutral reputation.");
                WriteLine("This means, of course, that people don't have an opinion on you. They don't really know you exist.");
                WriteLine("As you start performing large-scale operations within the digital society, your reputation will raise or lower gradually depending on how morally correct that action was.");
                WriteLine("For example, if you start performing criminal actions, your reputation will start to drop, and people will start to distrust you.");
                WriteLine("And if your rep drops too far, the MUD Safety Task Force, and other safety activists may start going after you and trying to take you off the MUD.");
                WriteLine("However, if you perform morally-correct actions, your reputation will rise, and more people will trust you with more sensitive data and operations.");
                WriteLine("Be careful though, if you have too high of a reputation, lower-rep groups will try to attack you.");
                WriteLine("And, I'd be careful of Investigators. If they suspect anything bad about you, they'll do whatever they can to prove you guilty and dramatically decrease your reputation.");
                WriteLine("Anyways, I've got some other sentiences I need to... have a little...word...with. Keep on shifting.");
                WriteLine("--user has disconnected from your system.--", false);
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
            });
            t.IsBackground = true;
            t.Start();

            TerminalBackend.PrefixEnabled = false;
        }

        public class MCCInstallation : Applications.Installation
        {
            protected override void Run()
            {
                SetStatus("Beginning installation...");
                Thread.Sleep(1270);
                SetProgress(10);
                SetStatus("Installing base application...");
                for(int i = 0; i < 45; i++)
                {
                    Thread.Sleep(25);
                    SetProgress(10 + i);
                }
                SetStatus("Configuring system...");
                //First, we initialize the user's legion value.
                SaveSystem.CurrentSave.CurrentLegions = new List<string>();
                Thread.Sleep(250);
                SetProgress(65);
                //Now we initialize their shop value.
                SaveSystem.CurrentSave.MyShop = null;
                Thread.Sleep(200);
                SetProgress(75);
                //Now for their reputation...
                SaveSystem.CurrentSave.RawReputation = 0.000;
                Thread.Sleep(250);
                SetProgress(90);
                //Now their class.
                SaveSystem.CurrentSave.Class = Objects.UserClass.None;
                Thread.Sleep(200);
                Story.Start("mud_control_centre");
                SaveSystem.SaveGame();
                SetProgress(100);
            }
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
                Story.Start("downloader");
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
