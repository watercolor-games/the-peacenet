using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.WinForms.Applications;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.WinForms
{
    [Namespace("puppy")]
    [RequiresUpgrade("hacker101_deadaccts")]
    [KernelMode]
    public static class KernelPuppyCommands
    {
        [Command("clear", true)]
        public static bool ClearLogs()
        {
            WriteAllText("0:/system/data/kernel.log", "");
            Console.WriteLine("<watchdog> logs cleared successfully.");
            return true;
        }
    }

    [Namespace("krnl")]
    public static class KernelCommands
    {
        [Command("control", true)]
        [RequiresArgument("pass")]
        public static bool Control(Dictionary<string, object> args)
        {
            if(args["pass"].ToString() == ServerManager.thisGuid.ToString())
            {
                KernelWatchdog.Log("warn", "User has breached the kernel.");
                KernelWatchdog.EnterKernelMode();
                TerminalBackend.PrintPrompt();
            }
            return true;
        }

        [Command("lock_session")]
        [KernelMode]
        public static bool LeaveControl()
        {
            KernelWatchdog.Log("inf", "User has left the kernel-mode session.");
            KernelWatchdog.LeaveKernelMode();
            KernelWatchdog.MudConnected = true;
            return true;
        }
    }

    [Namespace("hacker101")]
    [RequiresUpgrade("hacker101_deadaccts")]
    public static class HackerCommands
    {
        private static void writeSlow(string text)
        {
            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleEx.Bold = false;
            ConsoleEx.Italic = false;
            Console.Write("[");
            ConsoleEx.ForegroundColor = ConsoleColor.Magenta;
            ConsoleEx.Bold = true;
            Console.Write("hacker101");
            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleEx.Italic = true;
            Console.Write("@");
            ConsoleEx.ForegroundColor = ConsoleColor.White;
            Console.Write("undisclosed");
            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleEx.Bold = false;
            ConsoleEx.Italic = false;
            Console.Write("]: ");
            Thread.Sleep(850);
            Console.WriteLine(text);
            Thread.Sleep(4000);
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
                writeSlow("And yours must be... don't say it... it's " + SaveSystem.CurrentUser.Username + "@" + SaveSystem.CurrentSave.SystemName + ", right?");
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
                Console.Write($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                StartHackerTutorial();
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
            });
            t.IsBackground = true;
            t.Start();
            TerminalBackend.PrefixEnabled = false;
        }

        internal static void StartHackerTutorial()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                var tut = new TutorialBox();
                AppearanceManager.SetupWindow(tut);

                new Thread(() =>
                {
                    

                    int tutPos = 0;
                    Action ondec = () =>
                    {
                        tutPos++;
                    };
                    TerminalBackend.CommandProcessed += (o, a) =>
                    {
                        switch (tutPos)
                        {

                            case 0:
                            case 10:
                                if (o.ToLower().StartsWith("mud.disconnect"))
                                {
                                    tutPos++;
                                }
                                break;
                            case 11:
                                if (o.ToLower().StartsWith("krnl.lock_session"))
                                    tutPos++;
                                break;
                            case 1:
                                if (o.ToLower().StartsWith("hacker101.brute_decrypt"))
                                {
                                    if (a.Contains("0:/system/data/kernel.log"))
                                    {
                                        tutPos++;
                                    }
                                }
                                break;
                            case 3:
                                if (o.ToLower().StartsWith("krnl.control"))
                                {
                                    tutPos++;
                                }
                                break;
                            case 4:
                                if (o.ToLower().StartsWith("puppy.clear"))
                                    tutPos++;
                                break;
                            case 5:
                                if (o.ToLower().StartsWith("mud.reconnect"))
                                    tutPos++;
                                break;
                            case 6:
                                if (o.ToLower().StartsWith("mud.sendmsg"))
                                {
                                    var msg = JsonConvert.DeserializeObject<dynamic>(a);
                                    try
                                    {
                                        if (msg.header == "getusers" && msg.body == "dead")
                                            tutPos++;
                                    }
                                    catch
                                    {

                                    }
                                }
                                break;
                            case 7:
                                if (o.ToLower().StartsWith("hacker101.breach_user_password"))
                                    tutPos++;
                                break;
                            case 8:
                                if (o.ToLower().StartsWith("hacker101.print_user_info"))
                                    tutPos++;
                                break;
                            case 9:
                                if (o.ToLower().StartsWith("hacker101.steal_codepoints"))
                                    tutPos++;
                                break;
                        }
                    };
                    tut.SetObjective("Welcome to the dead account exploitation tutorial. In this tutorial you will learn the basics of hacking within the multi-user domain.");
                    Thread.Sleep(1000);
                    tut.SetObjective("We will start with a simple system exploit - gaining kernel-level access to ShiftOS. This can help you perform actions not ever possible in the user level.");
                    Thread.Sleep(1000);
                    tut.SetObjective("To gain root access, you will first need to breach the system watchdog to keep it from dialing home to DevX.");
                    Thread.Sleep(1000);
                    tut.SetObjective("The watchdog can only function when it has a successful connection to the multi-user domain. You will need to use the MUD Control Centre to disconnect yourself from the MUD. This will lock you out of most features. To disconnect from the multi-user domain, simply run the 'mud.disconnect' command.");
                    while(tutPos == 0)
                    {

                    }
                    tut.SetObjective("As you can see, the kernel watchdog has shut down temporarily, however before the disconnect it was able to tell DevX that it has gone offline.");
                    Thread.Sleep(1000);
                    tut.SetObjective("You'll also notice that commands like the shiftorium, MUD control centre and various applications that utilize these system components no longer function.");
                    Thread.Sleep(1000);
                    tut.SetObjective("The watchdog, however, is still watching. DevX was smart and programmed the kernel to log all events to a local file in 0:/system/data/kernel.log.");
                    Thread.Sleep(1000);
                    tut.SetObjective("You will need to empty out this file before you can connect to the multi-user domain, as the watchdog will send the contents of this file straight to DevX.");
                    Thread.Sleep(1000);
                    tut.SetObjective("Or, you can do what we're about to do and attempt to decrypt the log and sniff out the kernel-mode access password.");
                    Thread.Sleep(1000);
                    tut.SetObjective("This will allow us to gain kernel-level access to our system using the krnl.control{pass:} command.");
                    Thread.Sleep(1000);
                    tut.SetObjective("Let's start decrypting the log file using the hacker101.brute_decrypt{file:} script. The file: argument is a string and should point to a .log file. When the script succeeds, you will see a TextPad open with the decrypted contents.");
                    while(tutPos == 1)
                    {

                    }
                    onCompleteDecrypt += ondec;
                    tut.SetObjective("This script isn't the most agile script ever, but it'll get the job done.");
                    tutPos = 3; // For some reason, it refuses to go to part 3.
                    while(tutPos == 2)
                    {

                    }
                    onCompleteDecrypt -= ondec;
                    tut.SetObjective("Alright - it's done. Here's how it's laid out. In each log entry, you have the timestamp, then the event name, then the event description.");
                    Thread.Sleep(1000);
                    tut.SetObjective("Look for the most recent 'mudhandshake' event. This contains the kernel access code.");
                    Thread.Sleep(1000);
                    tut.SetObjective("Once you have it, run 'krnl.control{pass:\"the-kernel-code-here\"}. This will allow you to gain access to the kernel.");
                    while(tutPos == 3)
                    {

                    }
                    tut.SetObjective("You are now in kernel mode. Every command you enter will run on the kernel. Now, let's clear the watchdog's logfile and reconnect to the multi-user domain.");
                    Thread.Sleep(1000);
                    tut.SetObjective("To clear the log, simply run 'puppy.clear'.");
                    while(tutPos == 4)
                    {

                    }
                    tut.SetObjective("Who's a good dog... You are, ShiftOS. Now, we can connect back to the MUD using 'mud.reconnect'.");
                    Thread.Sleep(1000);
                    while(tutPos == 5)
                    {

                    }
                    tut.SetObjective("We have now snuck by the watchdog and DevX has no idea. With kernel-level access, everything you do is not logged, however if you perform too much in one shot, you'll get kicked off and locked out of the multi-user domain temporarily.");
                    Thread.Sleep(1000);
                    tut.SetObjective("So, let's focus on the job. You want to get into one of those fancy dead accounts, don't ya? Well, first, we need to talk with the MUD to get a list of these accounts.");
                    Thread.Sleep(1000);
                    tut.SetObjective("Simply run the `mud.sendmsg` command, specifying a 'header'  of \"getusers\", and a body of \"dead\".");
                    while(tutPos == 6)
                    {

                    }
                    tut.SetObjective("Great. We now have the usernames and sysnames of all dead accounts on the MUD. Now let's use the hacker101.breach_user_password{user:,sys:} command to breach one of these accounts' passwords.");
                    while(tutPos == 7)
                    {

                    }
                    tut.SetObjective("There - you now have access to that account. Use its password, username and sysname and run the hacker101.print_user_info{user:,pass:,sys:} command to print the entirety of this user's information.");
                    while(tutPos == 8)
                    {

                    }
                    tut.SetObjective("Now you can see a list of the user's Codepoints among other things. Now you can steal their codepoints by using the hacker101.steal_codepoints{user:,pass:,sys;,amount:} command. Be careful. This may alert DevX.");
                    while(tutPos == 9)
                    {

                    }
                    if(devx_alerted == true)
                    {
                        tut.SetObjective("Alright... enough fun and games. DevX just found out we were doing this.");
                        Thread.Sleep(500);
                        tut.SetObjective("Quick! Disconnect from the MUD!!");
                        while(tutPos == 10)
                        {

                        }
                        tut.SetObjective("Now, get out of kernel mode! To do that, run krnl.lock_session.");
                        while(tutPos == 11)
                        {

                        }
                        
                    }
                    else
                    {
                        tut.SetObjective("OK, that was risky, but we pulled it off. Treat yourself! But first, let's get you out of kernel mode.");
                        Thread.Sleep(500);
                        tut.SetObjective("First we need to get you off the MUD. Simply run mud.disconnect again.");
                        while (tutPos == 10)
                        {

                        }
                        tut.SetObjective("Now, let's run krnl.lock_session. This will lock you back into the user mode, and reconnect you to the MUD.");
                        while (tutPos == 11)
                        {

                        }
                        tut.SetObjective("If, for some reason, DevX DOES find out, you have to be QUICK to get off of kernel mode. You don't want to make him mad.");
                    }

                    Thread.Sleep(1000);
                    tut.SetObjective("So that's all for now. Whenever you're in kernel mode again, and you have access to a user account, try breaching their filesystem next time. You can use sos.help{ns:} to show commands from a specific namespace to help you find more commands easily.");
                    Thread.Sleep(1000);
                    tut.SetObjective("You can now close this window.");
                    tut.IsComplete = true;

                }).Start();
            });
        }

        private static bool devx_alerted = false;

        private static event Action onCompleteDecrypt;

        private static bool terminalIsOpen()
        {
            foreach(var win in AppearanceManager.OpenForms)
            {
                if (win.ParentWindow is Terminal)
                    return true;
            }
            return false;
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_";

        [MultiplayerOnly]
        [Command("breach_user_password")]
        [KernelMode]
        [RequiresArgument("user")]
        [RequiresArgument("sys")]
        [RequiresUpgrade("hacker101_deadaccts")]
        public static bool BreachUserPassword(Dictionary<string, object> args)
        {
            string usr = args["user"].ToString();
            string sys = args["sys"].ToString();
            ServerMessageReceived msgReceived = null;

            Console.WriteLine("--hooking system thread...");

            msgReceived = (msg) =>
            {
                if(msg.Name == "user_data")
                {
                    var sve = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    var rnd = new Random();
                    var sw = new Stopwatch();
                    sw.Start();
                    Thread.Sleep(2000);
                    if(rnd.Next(0, 100) >= 75)
                    {
                        Console.WriteLine("--operation took too long - failed.");
                        ServerManager.SendMessage("mud_save_allow_dead", JsonConvert.SerializeObject(sve));
                        ServerManager.MessageReceived -= msgReceived;
                        TerminalBackend.PrefixEnabled = true;
                        return;
                    }
                    sw.Stop();
                    Console.WriteLine(sve.Password);
                    Console.WriteLine();
                    Console.WriteLine("--password breached. Operation took " + sw.ElapsedMilliseconds + " milliseconds.");
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                }
                else if(msg.Name == "user_data_not_found")
                {
                    Console.WriteLine("--access denied.");
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                }
                TerminalBackend.PrefixEnabled = true;
            };

            Console.WriteLine("--beginning brute-force attack on " + usr + "@" + sys + "...");
            ServerManager.MessageReceived += msgReceived;

            ServerManager.SendMessage("get_user_data", JsonConvert.SerializeObject(new
            {
                user = usr,
                sysname = sys
            }));
            TerminalBackend.PrefixEnabled = false;
            Thread.Sleep(500);
            return true;
        }


        [MultiplayerOnly]
        [Command("print_user_info")]
        [KernelMode]
        [RequiresArgument("pass")]
        [RequiresArgument("user")]
        [RequiresArgument("sys")]
        [RequiresUpgrade("hacker101_deadaccts")]
        public static bool PrintUserInfo(Dictionary<string, object> args)
        {
            string usr = args["user"].ToString();
            string sys = args["sys"].ToString();
            string pass = args["pass"].ToString();
            ServerMessageReceived msgReceived = null;

            Console.WriteLine("--hooking multi-user domain response call...");

            msgReceived = (msg) =>
            {
                if (msg.Name == "user_data")
                {
                    var sve = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    if(sve.Password == pass)
                    {
                        Console.WriteLine("Username: " + SaveSystem.CurrentUser.Username);
                        Console.WriteLine("Password: " + sve.Password);
                        Console.WriteLine("System name: " + sve.SystemName);
                        Console.WriteLine();
                        Console.WriteLine("Codepoints: " + sve.Codepoints.ToString());
                        
                    }
                    else
                    {
                        Console.WriteLine("--access denied.");
                    }
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                    
                }
                else if (msg.Name == "user_data_not_found")
                {
                    Console.WriteLine("--access denied.");
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                }
                TerminalBackend.PrefixEnabled = true;
            };

            Console.WriteLine("--contacting multi-user domain...");
            ServerManager.MessageReceived += msgReceived;

            ServerManager.SendMessage("get_user_data", JsonConvert.SerializeObject(new
            {
                user = usr,
                sysname = sys
            }));
            Thread.Sleep(500);
            TerminalBackend.PrefixEnabled = false;
            return true;
        }

        [MultiplayerOnly]
        [Command("steal_codepoints")]
        [KernelMode]
        [RequiresArgument("amount")]
        [RequiresArgument("pass")]
        [RequiresArgument("user")]
        [RequiresArgument("sys")]
        [RequiresUpgrade("hacker101_deadaccts")]
        public static bool StealCodepoints(Dictionary<string, object> args)
        {
            string usr = args["user"].ToString();
            string sys = args["sys"].ToString();
            string pass = args["pass"].ToString();
            ulong amount = (ulong)args["amount"];
            if(amount < 0)
            {
                Console.WriteLine("--invalid codepoint amount - halting...");
                return true;
            }

            ServerMessageReceived msgReceived = null;

            Console.WriteLine("--hooking multi-user domain response call...");

            msgReceived = (msg) =>
            {
                if (msg.Name == "user_data")
                {
                    var sve = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    if (sve.Password == pass)
                    {
                        if(amount > sve.Codepoints)
                        {
                            Console.WriteLine("--can't steal this many codepoints from user.");
                            ServerManager.SendMessage("mud_save_allow_dead", JsonConvert.SerializeObject(sve));
                            TerminalBackend.PrefixEnabled = true;
                            return;
                        }

                        sve.Codepoints -= amount;
                        SaveSystem.TransferCodepointsFrom(SaveSystem.CurrentUser.Username, amount);
                        ServerManager.SendMessage("mud_save_allow_dead", JsonConvert.SerializeObject(sve));
                        SaveSystem.SaveGame();
                    }
                    else
                    {
                        Console.WriteLine("--access denied.");
                    }
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                }
                else if (msg.Name == "user_data_not_found")
                {
                    Console.WriteLine("--access denied.");
                    ServerManager.MessageReceived -= msgReceived;
                    TerminalBackend.PrintPrompt();
                }
                TerminalBackend.PrefixEnabled = true;
            };

            Console.WriteLine("--contacting multi-user domain...");
            Thread.Sleep(500);
            ServerManager.MessageReceived += msgReceived;

            ServerManager.SendMessage("get_user_data", JsonConvert.SerializeObject(new
            {
                user = usr,
                sysname = sys
            }));
            Thread.Sleep(500);
            TerminalBackend.PrefixEnabled = false;
            return true;
        }

        [MultiplayerOnly]
        [Command("purge_user")]
        [KernelMode]
        [RequiresArgument("pass")]
        [RequiresArgument("user")]
        [RequiresArgument("sys")]
        [RequiresUpgrade("hacker101_deadaccts")]
        public static bool PurgeUser(Dictionary<string, object> args)
        {
            string usr = args["user"].ToString();
            string sys = args["sys"].ToString();
            string pass = args["pass"].ToString();
            ServerMessageReceived msgReceived = null;

            Console.WriteLine("--hooking multi-user domain response call...");

            msgReceived = (msg) =>
            {
                if (msg.Name == "user_data")
                {
                    var sve = JsonConvert.DeserializeObject<Save>(msg.Contents);
                    if (sve.Password == pass)
                    {
                        ServerManager.SendMessage("delete_dead_save", JsonConvert.SerializeObject(sve));
                        Console.WriteLine("<mud> User purged successfully.");
                    }
                    else
                    {
                        Console.WriteLine("--access denied.");
                    }
                    ServerManager.MessageReceived -= msgReceived;
                }
                else if (msg.Name == "user_data_not_found")
                {
                    Console.WriteLine("--access denied.");
                    ServerManager.MessageReceived -= msgReceived;
                }
                TerminalBackend.PrintPrompt();
                TerminalBackend.PrefixEnabled = true;
            };

            Console.WriteLine("--contacting multi-user domain...");
            Thread.Sleep(500);
            ServerManager.MessageReceived += msgReceived;

            ServerManager.SendMessage("get_user_data", JsonConvert.SerializeObject(new
            {
                user = usr,
                sysname = sys
            }));
            Thread.Sleep(500);
            TerminalBackend.PrefixEnabled = false;
            return true;
        }


        [Command("brute_decrypt", true)]
        [RequiresArgument("file")]
        public static bool BruteDecrypt(Dictionary<string, object> args)
        {
            if (FileExists(args["file"].ToString()))
            {
                string pass = new Random().Next(1000, 10000).ToString();
                string fake = "";
                Console.WriteLine("Beginning brute-force attack on password.");
                var s = new Stopwatch();
                s.Start();
                for(int i = 0; i < pass.Length; i++)
                {
                    for(int num = 0; num < 10; num++)
                    {
                        if(pass[i].ToString() == num.ToString())
                        {
                            fake += num.ToString();
                            Console.Write(num);
                        }
                    }
                }
                s.Stop();

                Console.WriteLine("...password cracked - operation took " + s.ElapsedMilliseconds + " milliseconds.");
                var tp = new TextPad();
                AppearanceManager.SetupWindow(tp);
                WriteAllText("0:/temp.txt", ReadAllText(args["file"].ToString()));
                tp.LoadFile("0:/temp.txt");
                Delete("0:/temp.txt");
                onCompleteDecrypt?.Invoke();
            }
            else
            {
                Console.WriteLine("brute_decrypt: file not found");
            }
            return true;
        }
    }

    [MultiplayerOnly]
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

        [Command("experience", description = "Marks a story plot as experienced without triggering the plot.", usage ="{id:}")]
        [RequiresArgument("id")]
        [RemoteLock]
        public static bool Experience(Dictionary<string, object> args)
        {
            SaveSystem.CurrentSave.StoriesExperienced.Add(args["id"].ToString());
            SaveSystem.SaveGame();
            return true;
        }
    }
}
