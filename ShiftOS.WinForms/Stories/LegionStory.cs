using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;

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
        
        [RequiresUpgrade("appscape_troubles")]
        [Mission("appscape_troubles_end", "Appscape Troubles: Lifting The Ban", "Maureen's been banned from Appscape. Let's see if we can get Aiden to reverse that.", 1200l, "maureen_fenn")]
        public static void AppscapeTroublesEnd()
        {
            Applications.Chat chat = null;
            Desktop.InvokeOnWorkerThread(() =>
            {
                chat = OpenChat();
            });
            while (chat == null)
                Thread.Sleep(10);
            chat.ShowChat();
            chat.ChatID = "maureen_fenn@trisys";
            CurrentChat = chat;
            SendChatMessage("maureen_fenn", "I just talked to hacker101 about our little issue here...");
            SendChatMessage("maureen_fenn", "He tried to get Aiden to lighten up, but he's too hell-bent on making DevX happy.");
            SendChatMessage("maureen_fenn", "But, I think we've gotten a plan.");
            SendChatMessage("hacker101", "<user joined chat>");
            chat.ChatID = "maureen_fenn@trisys, hacker101@pebcak";
            SendChatMessage("hacker101", "We meet again, " + SaveSystem.CurrentUser.Username + "...");
            SendChatMessage("hacker101", "Maureen tells me you've used your hacking skills to steal a document from Aiden Nirh.");
            SendChatMessage("hacker101", "I hope he didn't find out about brute...");
            SendChatMessage("maureen_fenn", "Ugh... brute? That thing? That's not hacking.");
            SendChatMessage("maureen_fenn", "You're just turning " + SaveSystem.CurrentUser.Username + " into a script-kiddie.");
            SendChatMessage("maureen_fenn", SaveSystem.CurrentUser.Username + ", you want REAL hacking? Why don't you come to me after we're done here. I'll show you how REAL sentiences get their way.");
            SendChatMessage("hacker101", "HEY. You gotta give Brute some credit. It's good at cracking passwords.");
            SendChatMessage("maureen_fenn", "Well, what happens if the user runs into a firewall block? Brute ain't going to help with that.");
            SendChatMessage("hacker101", "....whatever. Let's just get on with this.");
            SendChatMessage("hacker101", "We need to get rid of that cease and desist ban.");
            SendChatMessage("hacker101", "Firstly, let's review the document.");
            SendChatMessage("hacker101", "<sent a file: maureen_fenn.txt>");
            var bytes = Convert.FromBase64String(Properties.Resources.AppscapeWantedFile);
            chat.PostMessage("maureen_fenn.txt", chat.ChatID, Encoding.UTF8.GetString(bytes));
            SendChatMessage("hacker101", "I ain't no lawyer but I can tell you right now that C&D is bull.");
            SendChatMessage("hacker101", "Guess this is just DevX's way of saying he can't stand you, Maureen.");
            SendChatMessage("hacker101", "I want you to know that unlike him, I can. I... I...");
            SendChatMessage("maureen_fenn", "..Nope, nope. Let's not go there. You're getting a bit creepy, hacker.");
            SendChatMessage("hacker101", "Whoops. Oh well... We need to show DevX who's boss.");
            SendChatMessage("hacker101", "Maybe, " + SaveSystem.CurrentUser.Username + " can help us.");
            SendChatMessage("maureen_fenn", "Maybe they can... and meybe Aiden can as well.");
            SendChatMessage("hacker101", "No, he's convinced that we're the bad guys.");
            SendChatMessage("hacker101", "He's got me banned because of brute.");
            SendChatMessage("maureen_fenn", "Lol, banned... for BRUTE? That little harmless tool? Bahahahaha.");
            SendChatMessage(SaveSystem.CurrentUser.Username, "Will you two stop bickering at eachother about Brute and hacker101's love for Maureen and let us get on with this? I could be playing Pong right now.");
            SendChatMessage("maureen_fenn", "Right. We need to gather as much evidence against DevX as possible.");
            SendChatMessage("hacker101", "Then, we need to send it all to Aiden, right? We'll need " + SaveSystem.CurrentUser.Username + " to do that.");
            SendChatMessage("maureen_fenn", "Alrighty, " + SaveSystem.CurrentUser.Username + ". You're on our team. We'll have any tasks we need you to do in your missions list.");
            SendChatMessage("maureen_fenn", "Oh yeah, and here's a couple Codepoints as compensation for helping us out.");
            Story.Context.MarkComplete();
            Thread.Sleep(5000);
            Desktop.InvokeOnWorkerThread(() =>
            {
                AppearanceManager.Close(chat);
            });
        }

        [RequiresUpgrade("hacker101_breakingbonds_3")]
        [Mission("appscape_troubles", "Appscape Troubles", "You know how to do some basic hacking, now you've got a chance to exercise it.", 750l, "maureen_fenn")]
        public static void AppscapeTroubles()
        {
            Applications.Chat chat = null;
            Desktop.InvokeOnWorkerThread(() =>
            {
                chat = OpenChat();
            });
            while (chat == null)
                Thread.Sleep(10);
            chat.ShowChat();
            chat.ChatID = "maureen_fenn@trisys";
            CurrentChat = chat;
            SendChatMessage("maureen_fenn", "Hello there, " + SaveSystem.CurrentUser.Username + ". My name is Maureen.");
            SendChatMessage("maureen_fenn", "I'm the developer of the various Tri apps you may've seen around the Shiftnet.");
            Story.Context.AutoComplete = false;
            SendChatMessage("maureen_fenn", "I have a bit of a problem that may need your assistance..");
            SendChatMessage("maureen_fenn", "I struck a deal with Aiden Nirh to put my software on his site and split the profits in half with me and him.");
            SendChatMessage("maureen_fenn", "But lately, even though many people have been buying my software, I've been getting nothing for it.");
            SendChatMessage("maureen_fenn", "Now I have barely enough Codepoints to keep my development environment online...");
            SendChatMessage("maureen_fenn", "My friend, you know him as hacker101, he's told me that you know how to hack.");
            SendChatMessage("maureen_fenn", "Can you bust into Aiden's server and see if he's gotten any documents or anything indicating why he's not paying me?");
            SendChatMessage("maureen_fenn", "He likes to document a lot of things about what he does and he likes to store those docs on his central Appscape server. Maybe there's something on there about me...");
            SendChatMessage("maureen_fenn", "You can find connection details for his server on Appscape's Contact page.");
            SendChatMessage("maureen_fenn", "The guy has pretty good security though... so be careful.");
            SendChatMessage("maureen_fenn", "When you find the right file, I'd download it to your system and just send it through SimpleSRC.");


            VirtualEnvironments.Create("appscape_main", new List<ClientSave>
            {
                new ClientSave
                {
                    Username = "aiden",
                    Password = GenRandomPassword(),
                    Permissions = UserPermissions.Root,
                },
                new ClientSave
                {
                    Username = "feedback",
                    Password = "",
                    Permissions = UserPermissions.Guest
                }
            }, 15000l, JsonConvert.DeserializeObject<ShiftOS.Objects.ShiftFS.Directory>(Properties.Resources.AppscapeServerFS));

            bool validFileSent = false;
            chat.FileSent += (file) =>
            {
                if (Convert.ToBase64String(file.Data) == Properties.Resources.AppscapeWantedFile)
                    validFileSent = true;
            };

            Story.Context.AutoComplete = false;

            Story.PushObjective("Appscape Troubles: The Secret", "Maureen has asked you to find out why Aiden Nirh, the maintainer of Appscape, isn't paying her half the profits for her TriOffice suite. Time to use your hacking skills... just like before.",
                () => { return validFileSent; },
                () =>
                {
                    SendChatMessage("maureen_fenn", "File received.");
                    SendChatMessage("maureen_fenn", "Awwww, come on, man! You can't tell me I got banned due to a cease and desist from DevX... That bastard... DevX, I mean.");
                    SendChatMessage("maureen_fenn", "Anyways, I'll talk to Aiden about that... if I can... or maybe you can? Either way, here's your Codepoints.");
                    Story.Context.MarkComplete();
                    Thread.Sleep(5000);
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        AppearanceManager.Close(chat);
                    });

                });


        }

        public static Applications.Chat CurrentChat;

        public static void SendChatMessage(string who, string msg)
        {
            CurrentChat.Typing = who;
            foreach(var c in msg)
            {
                Thread.Sleep(75);
            }
            CurrentChat?.PostMessage(who, CurrentChat?.ChatID, msg);
            CurrentChat.Typing = "";
            Thread.Sleep(500);
        }

        public static Applications.Chat OpenChat()
        {
            var chatbrd = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is Applications.Chat);
            Applications.Chat chat = null;
            if(chatbrd == null)
            {
                chat = new Applications.Chat();
                AppearanceManager.SetupWindow(chat);
            }
            else
            {
                chat = chatbrd.ParentWindow as Applications.Chat;
            }
            return chat;    
        }


        [Mission("hacker101_breakingbonds_3", "Breaking the Bonds", "It's time you've learned how to hack.", 500l, "hacker101")]
        public static void BreakingTheBonds_Outro()
        {
            Story.Context.AutoComplete = false;
            CharacterName = "hacker101";
            SysName = "pebcak";

            if (!terminalOpen())
            {
                var term = new Applications.Terminal();
                AppearanceManager.SetupWindow(term);
            }

            WriteLine("Now let's teach you how to hack others' systems.");
            WriteLine("I'll start up a safe virtual environment for you to mess with.");
            VirtualEnvironments.Create("pebcak_devel", new List<Objects.ClientSave>
            {
                new Objects.ClientSave
                {
                    Username = "root",
                    Password = GenRandomPassword(),
                    Permissions = Objects.UserPermissions.Root
                },
                new Objects.ClientSave
                {
                    Username = "user",
                    Password = "",
                    Permissions = Objects.UserPermissions.Admin,
                }
            }, 6500l, JsonConvert.DeserializeObject<ShiftOS.Objects.ShiftFS.Directory>(Properties.Resources.PebcakDevelFS));
            Thread.Sleep(2000);
            WriteLine("It's all set up now. The system name for this environment is \"pebcak_devel\".");
            WriteLine("This server allows FTP connections through your File Skimmer.");
            WriteLine("Other systems can allow RTS connections (which allows you to control their terminals remotely), SMTP (mail transfer), etc. It depends on the system's role.");

            bool isFsCopyInstalled = Shiftorium.UpgradeInstalled("fs_copy");
            if (isFsCopyInstalled == false)
            {
                WriteLine("Before we can begin, one last thing. You need the FS Copy Shiftorium upgrade.");
                Story.PushObjective("Breaking The Bonds: Preparations", "You need to buy the FS Copy upgrade from the Shiftorium.", () => { return Shiftorium.UpgradeInstalled("fs_copy"); }, () =>
                {
                    isFsCopyInstalled = true;
                });
            }
            while (isFsCopyInstalled == false)
                Thread.Sleep(10);

            WriteLine("Alright, open your File Skimmer, click \"Start Remote Session\", and connect to the system name \"pebcak_devel\" with user name \"user\" and no password.");

            Story.PushObjective("Breaking The Bonds: A little practice...", "hacker101 has set up a virtual environment for you to connect to. Its system name is \"pebcak_local\", and has an unsecured user account with the name \"user\". Log into that user using your File Skimmer.",
                () => { return Applications.FileSkimmer.OpenConnection.SystemName == "pebcak_devel"; },
                () =>
                {
                    WriteLine("Good work. You're in. This user only has Admin privileges, and doesn't have anything useful on it. This is where hacking comes in.");
                    WriteLine("See that \"super private personal stuff\" folder? It can only be accessed as a root user. You'll need the root password for pebcak_devel to get in there.");
                    WriteLine("I'll send you a password cracking utility that can use open ShiftOS connections to sniff out all the users on the system and allow you to brute-force into an account.");
                    Console.WriteLine("New program unlocked: brute");
                    SaveSystem.CurrentSave.StoriesExperienced.Add("brute");
                    WriteLine("Go ahead and open it! Use it to breach the root user on pebcak_devel.");
                    WriteLine("Once you've got the root password, click the Reauthenticate button in File Skimmer and it will ask you to log in as a new user.");
                    WriteLine("Use the new credentials to log in.");
                    Story.PushObjective("Breaking The Bonds: The Brute", "Use your new \"brute\" application to breach the root account on pebcak_local so you can access the super secret folder and download its contents.", () =>
                    {
                        return Applications.FileSkimmer.CurrentRemoteUser == Applications.FileSkimmer.OpenConnection.Users.FirstOrDefault(x => x.Username == "root");
                    },
                    () =>
                    {
                        WriteLine("Now, open the folder and you can copy files and folders from it to your system.");
                        WriteLine("You've got 60 seconds before ShiftOS's internet daemon terminates this connection.");
                        int counter = 60;
                        while(counter > 0 && Applications.FileSkimmer.OpenConnection.SystemName == "pebcak_devel")
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("Connection termination in " + counter + " seconds...");
                            if (counter == 30 || counter == 15)
                                Engine.AudioManager.PlayStream(Properties.Resources._3beepvirus);
                            if (counter <= 10)
                                Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                            counter--;
                        }
                        VirtualEnvironments.Clear();
                        Applications.FileSkimmer.DisconnectRemote();
                        WriteLine("Connections terminated I see.. Alright. Have fun with those dummy documents - you can keep them if you'd like. There's nothing important in them.");
                        WriteLine("That's one thing you can do with brute and other hacking utilities. I'd recommend buying some of brute's Shiftorium upgrades to make it faster and more efficient.");
                        WriteLine("Also, along the way, you're going to find a lot of new tricks. Some of them will require more than just brute to get into.");
                        WriteLine("So be on the lookout on the Shiftnet for other hacking-related tools. You won't find any on Appscape, however...");
                        WriteLine("That darn Aiden Nirh guy can't stand hackers.");
                        WriteLine("Looking at your logs, I see he's contacted you before... Seriously... don't let him find out about brute. He'll report it directly to DevX.");
                        WriteLine("Oh yeah, one more thing... that virus scanner... you may want to scan any files that you transfer from other systems with it.");
                        WriteLine("You never know what sorts of digital filth is hidden within such innocent-looking files.");
                        WriteLine("ALWAYS scan before opening - because if you open a file containing malicious code, it'll get run. It's just how ShiftOS's kernel works.");
                        WriteLine("Oh yeah, one last thing. Things are going to start getting pretty open in the Digital Society..");
                        WriteLine("Multiple people are going to want you to help them out with multiple things.");
                        WriteLine("I've written a little command-line utility that'll help you keep track of these missions and see how far you've gotten.");
                        WriteLine("Use the missions command to list out all the available missions, then use the startmission command to start one.");
                        WriteLine("When you complete a mission, you'll earn Codepoints depending on the mission.");
                        WriteLine("Allow me to demonstrate...");
                        Console.WriteLine("User has disconnected.");



                        Story.Context.MarkComplete();
                        TerminalBackend.PrefixEnabled = true;
                        SaveSystem.SaveGame();
                        TerminalBackend.PrintPrompt();
                    });
                });
        }

        [Story("brute")]
        public static void BreakingBondsStubStory()
        {
            //just to annoy victor tran
        }

        const string VALID_PASSWORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";

        public static string GenRandomPassword()
        {
            var rnd = new Random();
            int len = rnd.Next(5, 15);
            string pass = "";
            for(int i = 0; i < len; i++)
            {
                char c = VALID_PASSWORD_CHARS[rnd.Next(VALID_PASSWORD_CHARS.Length)];
                pass += c;
            }
            return pass;
        }

        [Story("hacker101_breakingbonds_2")]
        public static void BreakingTheBonds_Patchwork()
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
            WriteLine("Alright, you've gotten the applications you need.");
            WriteLine("Now, I know you're wondering, why do you need these three applications on your system?");
            WriteLine("Well, you're going to be doing some shady things and we need secure ways of storing the things you learn.");
            WriteLine("TriWrite is also needed so you can view rich-formatted text documents without them being garbled in TextPad.");
            WriteLine("Address Book is a secure way of storing information about the people you meet and learn about in the Digital Society.");
            WriteLine("And SimpleSRC is a chat system, much more advanced and secure than this remote terminal stuff you may have seen people doing to you.");
            WriteLine("ALL further operations with me will be done on SimpleSRC. But, for now, let's get you set up with your task.");
            WriteLine("You're going to be learning how to hack and crack systems in the Digital Society.");
            WriteLine("And this ain't no hippy DDoS stuff. Pfft, that crap is boring as hell.");
            WriteLine("I'm talking the ability to steal people's files remotely, read them on your system, and also, gain desktop-level and even root-level access to their ShiftOS installations, without them even knowing.");
            WriteLine("You'll be able to steal documents, programs, Codepoints and even more.");
            WriteLine("Of course, there's going to be defenses in place on other people's systems, such as secure passwords, advanced firewalls, network monitors, virus scanners, etc. You should get those kinds of things going on your system before we continue.");
            Story.Context.AutoComplete = false;
            WriteLine("I'll push out a sequence of objectives for you to follow to get your system secure.");
            Story.PushObjective("Breaking The Bonds: Patchwork - Get a virus scanner.", "Viruses are programs with the intent to harm users. They spread across the Digital Society infecting whoever they can find. A virus scanner can help you fight them off. There's a minimal one in the Shiftorium. Go get it!",

                () => { return Shiftorium.UpgradeInstalled("virus_scanner"); },
                () =>
                {
                    WriteLine("Alright, you've got a virus scanning program.");
                    WriteLine("Now, let's take care of your system's biggest vulnerability, your root account.");
                    bool isRootVulnerable = false;
                    if (string.IsNullOrWhiteSpace(SaveSystem.Users.FirstOrDefault(x => x.Username == "root").Password))
                    {
                        isRootVulnerable = true;
                        WriteLine("I was able to authenticate as root on your system without a password. Use the passwd command when logged in as root to change that.");
                        Story.PushObjective("Breaking The Bonds: Patchwork - Set a root password.", "If you aren't already, login as root using the su command. Then, Jesus Christ, set a root password!",
                            () => { return string.IsNullOrWhiteSpace(SaveSystem.Users.FirstOrDefault(x => x.Username == "root").Password); },
                            () => {
                                WriteLine("Man, oh man. My connection got terminated. That means you did it.");
                                WriteLine("Be lucky that was me and not someone who wanted to harm you.");
                                WriteLine("In ShiftOS, as well as most other Unix-likes, the root account has full permissions to everything on your system, no matter what.");
                                WriteLine("If someone gains access to your root system remotely, you must change its password immediately or you can call that system toast.");

                                isRootVulnerable = false; });
                    }
                    while (isRootVulnerable)
                        Thread.Sleep(10);

                    WriteLine("Alright, now let's make you another user account.");
                    WriteLine("This user account will have administrative permissions, but in order for you to use them, you'll need to type your root password to confirm any administrative task.");
                    WriteLine("Use the adduser command to add a new user. Give it a name, log into it, set a password if you'd like, then log back into root using su...");
                    WriteLine("Then, in root, run \"setuserpermissions --user \"yourusername\" --val 2\". This will give the specified user \"admin\" permissions.");
                    Story.PushObjective("Breaking The Bonds: Patchwork - Create an admin user", "Your root account looks nice and safe, but it's good practice on any Unix-like operating system, including ShiftOS, to have a user with slightly lower permissions called an Admin user. This user can do all the things that root can, but it requires you to enter your root password to verify administrative tasks.",

                        () =>
                        {
                            bool success = false;
                            if(SaveSystem.Users.Count() > 1)
                            {
                                success = SaveSystem.Users.FirstOrDefault(x => x.Username != "root" && x.Permissions == Objects.UserPermissions.Admin) != null;
                            }
                            return success;
                        },
                        () =>
                        {
                            WriteLine("It's as secure as you need now. There are a few other things you'll want to do, like setting up a firewall and a network monitor, but we'll save that for later.");
                            Story.Context.MarkComplete();
                            Story.Start("hacker101_breakingbonds_3");
                        });
                });
            
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

            TerminalBackend.PrefixEnabled = true;

            Story.PushObjective("Breaking the Bonds: Errand Boy", @"hacker101 has something he needs to show you, however before he can, you need to do the following:

 - Buy ""TriWrite"" from Appscape
 - Buy ""Address Book"" from Appscape
 - Buy ""SimpleSRC"" from Appscape", () =>
            {
                bool flag1 = Shiftorium.UpgradeInstalled("address_book");
                bool flag2 = Shiftorium.UpgradeInstalled("triwrite");
                bool flag3 = Shiftorium.UpgradeInstalled("simplesrc_client");
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
