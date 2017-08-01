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
            
            ctl.WriteLine("");
            ctl.WriteLine("Any time you are unsure of a command to run, type the help command.");
            ctl.WriteLine("Now, try typing the \"status\" command to see your current system status.");
        }
    }
}
