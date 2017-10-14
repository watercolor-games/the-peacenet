using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Objects;

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
            Action<string, Dictionary<string, object>> commandListener = (text, args) =>
            {
                Thread.Sleep(25);
                switch (position)
                {
                    case 0:
                        if (text == "help")
                        {
                            position++;

                        }
                        break;
                    case 1:
                        if (text == "status")
                            position++;
                        break;
                    case 2:
                        if (text == "upgrades")
                            position++;
                        break;
                }
            };
            TerminalBackend.CommandFinished += commandListener;

            Engine.Story.Context.AutoComplete = false;
            TerminalBackend.PrefixEnabled = false;
            TerminalBackend.InStory = true;
            UIManagerTools.EnterTextMode();
            Thread.Sleep(1000);
            var ctl = UIManager.TopLevels.FirstOrDefault(x => x is Apps.TerminalControl) as Apps.TerminalControl;
            ctl.Clear();
            Console.WriteLine("Welcome to the Plexnet.");
            Thread.Sleep(4000);
            Console.WriteLine("You need not worry, as your questions will be answered in due time.");
            Thread.Sleep(4000);
            Console.WriteLine("First, we must begin the training sequence.");
            Thread.Sleep(4000);
            Console.WriteLine("Below is what is called a Command Shell. It is an indicator that the system is waiting for a command. You can type a command into the shell and hit [ENTER] to confirm it.");
            Thread.Sleep(4000);
            Console.WriteLine("Please run the 'help' command to confirm your understanding.");
            Thread.Sleep(4000);
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.PrintPrompt();
            while (position == 0)
                Thread.Sleep(10);
            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;
            Console.WriteLine("The Plexgate recognizes your will to obey orders.");
            Thread.Sleep(4000);
            Console.WriteLine("The 'help' command is context-sensitive. It will list all available commands in the current shell.");
            Thread.Sleep(4000);
            Console.WriteLine("You want to know more about your environment, but first, you must know about your system.");
            Thread.Sleep(4000);
            Console.WriteLine("Firstly, when in the System Shell, you'll always know what system you are connected to and what user you're authenticated as");
            Thread.Sleep(4000);
            Console.WriteLine("because it will display on your shell prompt. Currently, you are logged in as {0} on the {1} system.", SaveSystem.GetUsername(), SaveSystem.GetSystemName());
            Thread.Sleep(4000);
            Console.WriteLine("AKA, you're logged in as yourself, on your own system.");
            Thread.Sleep(4000);
            Console.WriteLine("To know more about your system, run the 'status' command.");
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.PrintPrompt();
            while (position == 1)
                Thread.Sleep(10);
            
            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;
            Console.WriteLine("Good job. You seem to be quite literate compared to the others.");
            Thread.Sleep(4000);

            Console.WriteLine("You are currently in Text mode, but there is more to this operating system than just a terminal.");
            Thread.Sleep(4000);
            Console.WriteLine("Before I can show you this stuff, you need to learn how to purchase and load system upgrades.");
            Thread.Sleep(4000);
            Console.WriteLine("System upgrades are a purchasable resource that enhance Plexgate and your system's code to allow you to access more features.");
            Thread.Sleep(4000);
            Console.WriteLine("To list all available upgrades, simply type the 'upgrades' command.");
            Thread.Sleep(4000);
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            TerminalBackend.PrintPrompt();
            while (position == 2)
                Thread.Sleep(10);

            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;

        }

        public static PlexSkin LoadedSkin
        {
            get
            {
                return (PlexSkin)SkinEngine.LoadedSkin;
            }
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
                   var newDateTimeString = $"{now.Hours}:{now.Minutes}:{now.Seconds}";
                   var dtmeasure = GraphicsContext.MeasureString(newDateTimeString, LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopRight);
                   int dp_height = LoadedSkin.DesktopPanelHeight;
                   int dp_start = (UIManager.Viewport.Height - dp_height) * LoadedSkin.DesktopPanelPosition;
                   int al_left = LoadedSkin.AppLauncherFromLeft.X;
                   int al_width = LoadedSkin.AppLauncherHolderSize.Width;
                   int dp_width = UIManager.Viewport.Width;
                   int pc_left = dp_width - LoadedSkin.DesktopPanelClockFromRight.X - (int)dtmeasure.X;
                   int pc_width = dp_width - pc_left;
                   int pb_width = dp_width - al_width - pc_width;
                   int pb_left = al_width;

               });
        }
    }
}
