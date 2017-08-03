using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
namespace ShiftOS.Frontend.Stories
{
    public static class BeginTutorials
    {
        [Story("tutorial1")]
        public static void SystemTutorial()
        {
            while(AppearanceManager.OpenForms.Count > 0)
            {
                var frm = AppearanceManager.OpenForms[0];
                AppearanceManager.Close(frm.ParentWindow);
            }
            int position = 0;
            Action<KeyEvent> keylistener = (e) =>
            {
                switch (position)
                {
                    case 0:
                        if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                        {
                            position = 1;
                        }
                        break;
                }
            };
            
            Action<string, Dictionary<string, object>> commandListener = (text, args) =>
            {
                Thread.Sleep(25);
                switch (position)
                {
                    case 1:
                        if(text == "help")
                        {
                            position++;
                            
                        }
                        break;
                    case 2:
                        if (text == "status")
                            position++;
                        break;
                }
            };
            TerminalBackend.CommandFinished += commandListener;

            Engine.Story.Context.AutoComplete = false;
            TerminalBackend.PrefixEnabled = false;
            TerminalBackend.InStory = true;
            var term = new Apps.Terminal();
            AppearanceManager.SetupWindow(term);
            var ctl = term.TerminalControl;
            ctl.KeyEvent += keylistener;
            ctl.WriteLine("<kernel> System installation completed successfully.");
            ctl.WriteLine("<cinemgr> Starting system tutorial...");
            Thread.Sleep(500);
            ctl.WriteLine("");
            ctl.WriteLine("");
            ctl.WriteLine("Hey there, and welcome to ShiftOS. You are now running the system usage tutorial.");
            ctl.WriteLine("This tutorial will guide you through the bare minimum basics of using ShiftOS.");
            ctl.WriteLine("When you are ready, strike the [ENTER] key.");
            ctl.WriteLine("");
            while (position == 0)
                Thread.Sleep(10);
            ctl.WriteLine("Enter keypress detected.");
            Thread.Sleep(244);
            ctl.WriteLine("");
            ctl.WriteLine("<shd> Starting command shell on tty0.");
            Thread.Sleep(100);
            ctl.WriteLine("The below prompt is a Command Shell. This shell allows you to input commands into ShiftOS to tell it what to do.");
            ctl.WriteLine("To get a list of usable ShiftOS commands, type the \"help\" command.");
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.PrintPrompt();
            while (position == 1)
                Thread.Sleep(10);
            TerminalBackend.PrefixEnabled = false;
            Thread.Sleep(1000);
            ctl.WriteLine("");
            ctl.WriteLine("Any time you are unsure of a command to run, type the help command.");
            ctl.WriteLine("Now, try typing the \"status\" command to see your current system status.");

            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.PrintPrompt();
            while (position == 2)
                Thread.Sleep(10);
            Thread.Sleep(1000);
            ctl.WriteLine("");
            ctl.WriteLine("");
            TerminalBackend.PrefixEnabled = false;
            TerminalBackend.InStory = true;
            ctl.WriteLine("As you can see, your system doesn't have much value within the Digital Society.");
            Thread.Sleep(1000);
            ctl.WriteLine($"You have {SaveSystem.CurrentSave.Codepoints} Codepoints - and {SaveSystem.CurrentSave.CountUpgrades()} system upgrades.");
            ctl.WriteLine("");
            ctl.WriteLine("");
            Thread.Sleep(500);
            ctl.WriteLine("Codepoints can be earned by completing objectives within ShiftOS or by playing minigames.");
            Thread.Sleep(250);
            ctl.WriteLine("When you have Codepoints, you can use them to buy system upgrades from the Shiftorium, to add new programs and enhancements to ShiftOS.");
            Thread.Sleep(200);
            ctl.WriteLine("Tasks that can give you Codepoints can be found using the missions command.");
            Thread.Sleep(200);
            ctl.WriteLine("You can start a mission using the startmission command, and specifying the mission ID as a command-line argument.");
            Thread.Sleep(750);
            ctl.WriteLine("<kernel> Careful. The user doesn't know how arguments work in ShiftOS.");
            Thread.Sleep(490);
            ctl.WriteLine("That reminds me... Command-line arguments are pretty easy in ShiftOS.");
            Thread.Sleep(200);
            ctl.WriteLine("Most commands don't require arguments at all, like \"help\", \"status\" and \"missions\". However, others like \"buy\" and \"close\" will.");
            Thread.Sleep(200);
            ctl.WriteLine("Most commands that require arguments will take the implicit syntax, i.e \"open pong\".");
            Thread.Sleep(200);
            ctl.WriteLine("Others will require the explicit syntax, for example \"fileskimmer --dir 0:/home\".");
            Thread.Sleep(200);
            ctl.WriteLine("And others will accept both, for example \"inject ftpwn --port 21\".");
            Thread.Sleep(200);
            ctl.WriteLine("When you run a command and you have forgotten to supply its arguments, the Shell will tell you which arguments you are missing.");
            Thread.Sleep(200);
            ctl.WriteLine("If the Shell says you are missing an \"id\" argument, you can pass it using either the implicit syntax, i.e \"command value\", or the explicit syntax, i.e \"command --id value\". Both will be accepted by the command interpreter.");
            Thread.Sleep(200);
            ctl.WriteLine("However, for other arguments, the explicit syntax is required - the command interpreter won't be able to tell what argument you're supplying data to if you use the implicit syntax, and thus it will assume you are supplying data for the \"id\" argument.");
            Thread.Sleep(1000);
            ctl.WriteLine("<cinemgr> Basic system usage tutorial complete.");
            Thread.Sleep(500);
            ctl.WriteLine("<cpd> 200 Codepoints earned.");
            SaveSystem.CurrentSave.Codepoints += 200;
            SaveSystem.SaveGame();
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.InStory = false;
            Story.Context.MarkComplete();
            TerminalBackend.PrintPrompt();
        }

        [RequiresUpgrade("tutorial1")]
        [Mission("tutorial_hacking_basics", "Hacking Basics", "Welcome to ShiftOS. You know how to use your terminal, so let's teach you the sploitset shell and how to use OTHERS' terminals.", 250, "root")]
        public static void HackingBasics()
        {
            var term = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is Apps.Terminal);
            if(term == null)
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            }

            Console.WriteLine("");
            Story.Context.AutoComplete = false;
            Thread.Sleep(2000);
            Console.WriteLine("Hello there. I see you've come for more learning.");
            Thread.Sleep(2000);
            Console.WriteLine("In the Digital Society, if you're going to survive, the ability to breach and not get breached is paramount.");
            Thread.Sleep(2000);
            Console.WriteLine("First, let's teach you how to breach others using the sploitset tool.");
            Thread.Sleep(2000);
            Console.WriteLine("Start by running \"sploitset\". Also, any time you are assigned an objective, you can see it by typing \"status\" or in the System Status window.");
            Story.PushObjective("Open the sploitset shell.", "Open the sploitset shell to begin your first hack.",
                () => { return TerminalBackend.ShellOverride == "sploitset> "; },
                () =>
                {
                    Console.WriteLine("Sploitset tool started - objective complete.");
                    Console.WriteLine("Next, type \"devicescan\" to see a list of systems near you on the Digital Society.");
                    bool devicescanrun = false;
                    TerminalBackend.CommandFinished += (cmd, args) =>
                    {
                        if (cmd == "devicescan")
                            devicescanrun = true;
                    };
                    Story.PushObjective("Scan for nearby devices.", "Use sploitset's devicescan command to scan for devices near yours on the Digital Society.",
                        () => { return devicescanrun; },
                        () =>
                        {
                            Console.WriteLine("Alright, one device - shiftsyndicate_main. Let's try breaching it.");
                            Console.WriteLine("To breach a system, we first need to initiate a TCP handshake. Sploitset can do this with the \"connect\" command.");
                            Console.WriteLine("This is where your knowledge on Terminal's syntax comes in handy!");
                            bool connectrun = false;
                            TerminalBackend.CommandFinished += (cmd, args) =>
                            {
                                if (cmd == "connect")
                                    if (args.ContainsKey("id"))
                                        if (args["id"] as string == "shiftsyndicate_main")
                                            connectrun = true;
                            };
                            Story.PushObjective("Initiate a TCP handshake towards shiftsyndicate_main.", "We need to start a TCP handshake on this system if we wanna be able to do anything with it. You can do this with sploitset's \"connect\" command.",
                                () => connectrun,
                                () =>
                                {
                                    Console.WriteLine("Connection started? Alright. This one's a feisty one. You have a limited amount of time to connect and use the system.");
                                    Console.WriteLine("Because this is a tutorial, when you've established the connection, I'll apply a force heartbeat payload to keep the connection alive.");
                                    Console.WriteLine();
                                    Console.WriteLine("Next, we need to find out the ports that are being listened to on this device. You can do this by running \"listports\".");
                                    bool listedports = false;
                                    TerminalBackend.CommandFinished += (cmd, args) =>
                                    {
                                        if (cmd == "listports")
                                            listedports = true;
                                    };

                                    Story.PushObjective("List the online ports.", "Use sploitset to list the open ports on the remote system. It should give us an idea on what could be on the other side...",
                                        () => listedports,
                                        () =>
                                        {
                                            Console.WriteLine("This one looks like an FTP server, with SSH open for administration. The SSH one won't be much use as you don't have an SSH client, but let's see what's on that FTP server.");
                                            Console.WriteLine("Before we can connect, we need to use an FTP exploit to gain access to the remote server without needing to brute-force a username or password.");
                                            Console.WriteLine("Of course, you CAN do it the brute-force way, but it'll take a long time, and we don't have much time to do that, so it's better to let a program do it.");
                                            Console.WriteLine("You already have an FTP exploitation program installed on sploitset, called ftpwn. You can use it with the exploit command - \"exploit ftpwn --port 21\".");
                                            Console.WriteLine("You can also run \"exploits\" to see a list of all your exploit programs.");
                                            bool exploitrun = false;
                                            TerminalBackend.CommandFinished += (cmd, args) =>
                                            {
                                                if (cmd == "exploit")
                                                    if (args.ContainsKey("id") && args.ContainsKey("port"))
                                                        if (args["id"] as string == "ftpwn" && Convert.ToInt32(args["port"].ToString()) == 21)
                                                            exploitrun = true;
                                            };
                                            Story.PushObjective("Run the ftpwn exploit.", "It's time we get onto that port. This server's firewall isn't very strong but the port requires authentication. ftpwn will bypass the authentication and the pitiful firewall software on this device.",
                                                () => exploitrun,
                                                () =>
                                                {
                                                    Console.WriteLine("Exploitation of insecure FTP server successful? Alright. Deploying the force-heartbeat payload.");
                                                    Console.WriteLine("<sploitset> connection timeout deactivated.");
                                                    Hacking.CurrentHackable.DoConnectionTimeout = false;
                                                    Thread.Sleep(200);
                                                    Console.WriteLine("Done. Now, we may be connected to the remote FTP server, but we can't really do much with it.");
                                                    Console.WriteLine("Let's inject a payload that'll dump the contents of the remote system's home directory to yours.");
                                                    Console.WriteLine("The payload is called ftpull. You can inject it using the \"inject ftpull\" command.");
                                                    bool ftpull = false;
                                                    TerminalBackend.CommandFinished += (cmd, args) =>
                                                    {
                                                        if (cmd == "inject")
                                                            if (args.ContainsKey("id"))
                                                                if (args["id"] as string == "ftpull")
                                                                    ftpull = true;
                                                    };
                                                    Story.PushObjective("Inject the ftpull payload.", "You're connected to FTP, but you can't really do many things with a sploitset shell. Let's grab a list of users on the server so we can dump some useful files.",
                                                        () => ftpull && Hacking.CurrentHackable == null,
                                                        () =>
                                                        {
                                                            Console.WriteLine("The payload's done, and the connection's been dropped. You should've been booted out of sploitset. Check your Documents directory in File Skimmer!");

                                                            Console.WriteLine("You just looted an FTP server. This is a quick and easy way to gain new programs and other files for your system.");
                                                            Console.WriteLine("Look out for .stp files - they can be opened and contain new programs, upgrades, or additional features.");
                                                            Console.WriteLine("The harder the server is to crack, the more useful the loot will be.");
                                                            Console.WriteLine("This concludes the ShiftOS hacking tutorial!");
                                                            Story.Context.MarkComplete();
                                                        });
                                                });

                                        });
                                });
                        });
                });
        }
    }
}
