/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#define MUD_RAPIDDEV

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static ShiftOS.Engine.SaveSystem;

namespace ShiftOS.Engine
{
    // Provides functionality for managing windows within ShiftOS.
    public static class AppearanceManager
    {
        [Obsolete("Please use Localization.GetAllLanguages().")]
        public static string[] GetLanguages()
        {
            return Localization.GetAllLanguages();
        }

        // Sets the title text of the specified window.
        public static void SetWindowTitle(IShiftOSWindow window, string title)
        {
            if (window == null)
                throw new ArgumentNullException("window", "The window cannot be null.");
            winmgr.SetTitle(window, title);
        }

        //HEY LETS FIND THE WINDOWS
        public static IEnumerable<Type> GetAllWindowTypes()
        {
            return Array.FindAll(ReflectMan.Types, t => t.GetInterfaces().Contains(typeof(IShiftOSWindow)));
        }

        // hey you know that window we just made appear? well give it its title
        public static string GetDefaultTitle(Type winType)
        {
            if (winType == null)
                throw new ArgumentNullException("winType");
            foreach(var attrib in winType.GetCustomAttributes(false))
            {
                if(attrib is DefaultTitleAttribute)
                {
                    return (attrib as DefaultTitleAttribute).Title;
                }
            }
            return winType.Name;
        }

        // Current cursor position of the console
        public static int CurrentPosition { get; set; }

        // We don't know what this does. It may be gone if it does nothing.
        public static int LastLength { get; set; }


        // Minimize a window.
        public static void Minimize(IWindowBorder form)
        {
            if (form == null)
                //FUCK WHY THE FUCK IS THIS NULL
                throw new ArgumentNullException("form");
            if (winmgr == null)
                //FUCK THIS PART OF THE ENGINE WASNT TURNED ON YET
                throw new EngineModuleDisabledException();
            winmgr.Minimize(form);
        }

        // Maximizes a window! :D
        public static void Maximize(IWindowBorder form)
        {
            if (form == null)
                //AHHHH SHOULDNT BE NULLLLLL
                throw new ArgumentNullException("form");
            if (winmgr == null)
                //WHY ARE YOU DOING THIS PART OF THE ENGINE IT WASNT ENABLED FUCK
                throw new EngineModuleDisabledException();
            winmgr.Maximize(form);
        }



        // Provides a list of all open ShiftOS windows.
        public static List<IWindowBorder> OpenForms = new List<IWindowBorder>();

        // Decorates a window with a border, then shows the window.
        public static void SetupWindow(IShiftOSWindow form)
        {
            if (form == null)
                //YOU GET THE POINT THIS REALLY SHOULDNT BE NULL
                throw new ArgumentNullException("form");
            if (winmgr == null)
                //SAME HERE
                throw new EngineModuleDisabledException();
            winmgr.SetupWindow(form);
            Desktop.ResetPanelButtons();
        }

        // Closes the specified window. SHOCKED YOU ARE I KNOW, HOW COULD YOU HAVE GUESSED
        public static void Close(IShiftOSWindow win)
        {
            if (win == null)
                //NOPE SHOULDNT BE NULL
                throw new ArgumentNullException("win");
            if (winmgr == null)
                //WHY IS THIS NULL
                throw new EngineModuleDisabledException();
            winmgr.Close(win);
            Desktop.ResetPanelButtons();
        }


        // Decorates a window with a border, then shows the window, as a dialog box.
        public static void SetupDialog(IShiftOSWindow form)
        {
            if (form == null)
                //NULLLLLLLLL
                throw new ArgumentNullException("form");
            if (winmgr == null)
                //ASGDFHASDGF
                throw new EngineModuleDisabledException();
            winmgr.SetupDialog(form);
            Desktop.ResetPanelButtons();
        }

        // The underlying window manager for this engine module
        private static WindowManager winmgr = null;

        // Initiate this engine module, and perform mandatory configuration.
        public static void Initiate(WindowManager mgr)
        {
            winmgr = mgr; // A working, configured window manager to use as a backend for this module
        }


        // Raised when the engine is entering its shutdown phase. Save your work!
        public static event EmptyEventHandler OnExit;

        // Starts the engine's exit routine, firing the OnExit event.
        public static void Exit()
        {
            OnExit?.Invoke();
            //disconnect from MUD
            ServerManager.Disconnect();
            Desktop.InvokeOnWorkerThread(() =>
            {
                Process.GetCurrentProcess().Kill(); //bye bye
            });
        }

        // The current terminal body control.
        public static ITerminalWidget ConsoleOut { get; set; }

        // Redirects the .NET to a new TerminalTextWriter instance.  
        public static void StartConsoleOut()
        {
            Console.SetOut(new TerminalTextWriter()); //"plz start writing text .NET kthx"
        }

        // Invokes an action on the window management thread.
        public static void Invoke(Action act)
        {
            winmgr.InvokeAction(act);
        }
    }

    // Provides the base functionality for a ShiftOS terminal.
    public interface ITerminalWidget
    {
        void Write(string text); // Actually write text to this Terminal! :D:D:D:D
        void WriteLine(string text); // Write text to this Terminal, followed by a newline.
        void Clear(); // Clear the contents of this Terminal, i bet you wouldve never guessed that
        void SelectBottom(); // Move the cursor to the last character in the Terminal.
    }

    // makes the window manager actually do its job
    public abstract class WindowManager
    {
        public abstract void Minimize(IWindowBorder border); // guess what this does
        public abstract void Maximize(IWindowBorder border); // ooh this too
        public abstract void Close(IShiftOSWindow win); // omg this probably does something
        public abstract void SetupWindow(IShiftOSWindow win); // i cant think of what this does
        public abstract void SetupDialog(IShiftOSWindow win); // how about this???????
        public abstract void InvokeAction(Action act); // i wonder what this invokes
        public abstract void SetTitle(IShiftOSWindow win, string title); // what is a title again
    }

    // Provides the base functionality for a typical ShiftOS window border, what did you expect
    public interface IWindowBorder
    {
        void Close(); // CLOSES THE BORDER ALONG WITH ITS WINDOW!!!!!!! HOLY SHIT I DIDNT EXPECT THAT
        string Text { get; set; } // title text exists now
        IShiftOSWindow ParentWindow { get; set; } // Gets or sets the underlying for this border. 
    }

    // Provides a way of setting default title text for classes. 
    public class DefaultTitleAttribute : Attribute
    {
        // oy if you cant find a title this is the one you should use
        public DefaultTitleAttribute(string title)
        {
            Title = title;
        }
        
        public string Title { get; private set; }
    }

    // An exception that is thrown when mandatory configuration to run a specific method or module hasn't been done yet.
    public class EngineModuleDisabledException : Exception
    {
        // FUCK WE DIDNT ORDER THINGS RIGHT
        public EngineModuleDisabledException() : base("This engine module has not yet been enabled.")
        {
            //FUCK
        }
    }
}
