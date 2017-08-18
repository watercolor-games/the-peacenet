using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
namespace Plex.Frontend.Stories
{
    public static class BeginTutorials
    {
        [Story("tutorial1")]
        public static void SystemTutorial()
        {
            while (AppearanceManager.OpenForms.Count > 0)
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
                        if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
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
                        if (text == "help")
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
            ctl.WriteLine("<plexkrnl> System installation completed successfully.");
            ctl.WriteLine("<plexgate> Starting system tutorial...");
            Thread.Sleep(500);
            ctl.WriteLine("");
            ctl.WriteLine("");
            ctl.WriteLine("Welcome to the Plexgate Terminal. You are now running the system usage tutorial.");
            ctl.WriteLine("This tutorial will teach you the basic skills of using Plex and the Plexgate Desktop.");
            ctl.WriteLine("When you are ready, strike the [ENTER] key.");
            ctl.WriteLine("");
            while (position == 0)
                Thread.Sleep(10);
            ctl.WriteLine("Enter keypress detected.");
            Thread.Sleep(244);
            ctl.WriteLine("");
            ctl.WriteLine("<plexshell> Starting command shell on tty0.");
            Thread.Sleep(100);
            ctl.WriteLine("The below prompt is a Command Shell. This shell allows you to input commands into Plexgate Terminal to tell it what to do.");
            ctl.WriteLine("To get a list of usable Plexgate commands, type the \"help\" command.");
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
            ctl.WriteLine("As you can see, your system doesn't have much value within the usenet.");
            Thread.Sleep(1000);
            ctl.WriteLine($"You have {SaveSystem.CurrentSave.Experience} Experience Points - and {SaveSystem.CurrentSave.CountUpgrades()} system upgrades.");
            ctl.WriteLine("");
            ctl.WriteLine("");
            Thread.Sleep(500);
            ctl.WriteLine("Experience can be earned by completing objectives within Plex or by playing minigames.");
            Thread.Sleep(250);
            ctl.WriteLine("When you have Experience, you can use them to buy system upgrades from the Upgrade Centre, to add new programs and enhancements to Plexgate.");
            Thread.Sleep(200);
            ctl.WriteLine("Tasks that can give you Experience can be found using the missions command.");
            Thread.Sleep(200);
            ctl.WriteLine("You can start a mission using the startmission command, and specifying the mission ID as a command-line argument.");
            Thread.Sleep(750);
            ctl.WriteLine("<plexkrnl> Careful. The user doesn't know how arguments work in Plex.");
            Thread.Sleep(490);
            ctl.WriteLine("That reminds me... Command-line arguments are pretty easy in Plex.");
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
            ctl.WriteLine("<plexgate> Basic system usage tutorial complete.");
            Thread.Sleep(500);
            ctl.WriteLine("<cpd> 200 Experience earned.");
            SaveSystem.CurrentSave.Experience += 200;
            SaveSystem.SaveGame();
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.InStory = false;
            Story.Context.MarkComplete();
            TerminalBackend.PrintPrompt();
        }
        
        [Mission("gcc", "GUI Crash Course", "Welcome to the Plexgate Desktop. Now, it's time to learn how to use it.", 450, "plexkrnl")]
        [RequiresUpgrade("tutorial1")]
        public static void GUICrashCourse()
        {
            Story.Context.AutoComplete = false;
            while (AppearanceManager.OpenForms.Count > 0)
            {
                var frm = AppearanceManager.OpenForms[0];
                AppearanceManager.Close(frm.ParentWindow);
            }
            TerminalBackend.PrefixEnabled = false;
            TerminalBackend.InStory = true;
            var term = new Apps.Terminal();
            AppearanceManager.SetupWindow(term);
            Console.WriteLine("Welcome to Plexgate.");
            Thread.Sleep(2000);
            Console.WriteLine("This guide will teach you the basics of using the Plexgate Desktop. Just follow the interactive tutorial and you'll be an expert in no time.");
            int secondsleft = 5;
            while(secondsleft > 0)
            {
                Console.WriteLine("Starting tutorial in {0} seconds", secondsleft);
                Thread.Sleep(1000);
                secondsleft--;
            }
            UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(50, 50, 50, 50), "Welcome to the Plexgate Desktop tutorial! My name is Alkaline. I'll help you out by blanking out the screen and showing you where to go by keeping that area onscreen, just like this box over here!\r\n\r\nJust click it to continue!", () =>
               {
                   var now = DateTime.Now.TimeOfDay;
                   var newDateTimeString = $"{now.Hours}:{now.Minutes}:{now.Seconds} - {SaveSystem.CurrentSave.SystemName}";
                   var dtmeasure = GraphicsContext.MeasureString(newDateTimeString, SkinEngine.LoadedSkin.DesktopPanelClockFont);
                   int dp_height = SkinEngine.LoadedSkin.DesktopPanelHeight;
                   int dp_start = (UIManager.Viewport.Height - dp_height) * SkinEngine.LoadedSkin.DesktopPanelPosition;
                   int al_left = SkinEngine.LoadedSkin.AppLauncherFromLeft.X;
                   int al_width = SkinEngine.LoadedSkin.AppLauncherHolderSize.Width;
                   int dp_width = UIManager.Viewport.Width;
                   int pc_left = dp_width - SkinEngine.LoadedSkin.DesktopPanelClockFromRight.X - (int)dtmeasure.X;
                   int pc_width = dp_width - pc_left;
                   int pb_width = dp_width - al_width - pc_width;
                   int pb_left = al_width;
                   UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(0, dp_start, dp_width, dp_height), "That's how you do it. I've highlighted another area - this is called your Action Panel. Up here, you can switch between open windows, launch new programs, and see the time and the system you are connected to.", ()=>
                   {
                       UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(pc_left, dp_start, pc_width, dp_height), "Over here is your Connection Status. It tells you the current system time, and the hostname of the system you are currently connected to.", () =>
                       {
                           UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(pb_left, dp_start, pb_width, dp_height), "And here is your Task Switcher. It shows a list of all open windows, and clicking one of those buttons will bring the window to the front of the screen.", () =>
                           {
                               UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(0, dp_start, al_width, dp_height), "And over here? This mysterious do-hickey? This is your Launcher. Clicking it will open a menu full of programs you can launch.", () =>
                                {
                                    var al_item_to_click = (Engine.Desktop.CurrentDesktop as Desktop.Desktop).LauncherItems.FirstOrDefault(x => x.Data.LaunchType == typeof(Apps.SystemStatus));
                                    int al_container_height = ((Engine.Desktop.CurrentDesktop as Desktop.Desktop).LauncherItems.Count * al_item_to_click.Height) + 2;
                                    int al_item_width = al_item_to_click.Width;
                                    int al_item_height = al_item_to_click.Height;
                                    int al_c_y = (SkinEngine.LoadedSkin.DesktopPanelPosition == 0) ? dp_start + dp_height : 720 - dp_height - al_container_height;
                                    UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(al_item_to_click.X+1, al_c_y + al_item_to_click.Y+1, al_item_width-2, al_item_height), "You just opened your Launcher... I guess that was a bug with my tutorial overlay...it's not REALLY supposed to send click events directly to Plexgate...\r\n\r\nAhh well, I guess while you're here we might as well go over the System Status window. Go ahead and click here to open it.", () =>
                                    {
                                        var sstatus = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is Apps.SystemStatus) as Desktop.WindowBorder;
                                        var parent = (sstatus.ParentWindow as Apps.SystemStatus);
                                        UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(sstatus.X + parent.X, sstatus.Y + parent.Y, parent.Width, parent.Height), "This is the System Status window. It does exactly what you think - it tells you about your system's status.\r\n\r\nJust reading this window, you can see how much experience you've gained within the Plex Usenet, how many programs are open, and more.\r\n\r\nIt's a pretty useful utility, and there are many more utilities hiding in your Launcher waiting for you to use! I'll let you explore the Plexgate and its wonderful tools, but first, click this area to confirm System Status is on your screen.\r\n\r\nIf you want to close the window, you can do so by clicking the red box on the window's titlebar at the very far right.", () =>
                                        {
                                            AppearanceManager.Close(parent);
                                            UIManager.SetTutorialOverlay(new Microsoft.Xna.Framework.Rectangle(0, 0, 1280, 720), "This concludes the Alkaline Tutorial for the Plexgate Desktop.\r\n\r\nClick anywhere you'd like to begin using your new desktop environment.", () =>
                                               {
                                                   Story.Context.MarkComplete();
                                               });
                                        });
                                    });
                                });
                           });
                       });
                   });
               });
        }
    }
}
