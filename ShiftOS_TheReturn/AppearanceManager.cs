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
using Plex.Objects;
using static Plex.Engine.SaveSystem;

namespace Plex.Engine
{
    // Provides functionality for managing windows within Plex.
    public static class AppearanceManager
    {
        // Sets the title text of the specified window.
        public static void SetWindowTitle(IPlexWindow window, string title)
        {
            if (window == null)
                throw new ArgumentNullException("window", "The window cannot be null.");
            winmgr.SetTitle(window, title);
        }

        //HEY LETS FIND THE WINDOWS
        public static IEnumerable<Type> GetAllWindowTypes()
        {
            return ReflectMan.Types.Where(t => t.GetInterfaces().Contains(typeof(IPlexWindow)));
        }

        // Provides a list of all open Plex windows.
        public static List<IWindowBorder> OpenForms = new List<IWindowBorder>();

        // Decorates a window with a border, then shows the window.
        public static void SetupWindow(IPlexWindow form)
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
        public static void Close(IPlexWindow win)
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
        public static void SetupDialog(IPlexWindow form)
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
            ServerManager.Disconnect( DisconnectType.EngineShutdown);
            Desktop.InvokeOnWorkerThread(() =>
            {
                Process.GetCurrentProcess().Kill(); //bye bye
            });
        }
    }

    // makes the window manager actually do its job
    public abstract class WindowManager
    {
        public abstract void Minimize(IWindowBorder border); // guess what this does
        public abstract void Maximize(IWindowBorder border); // ooh this too
        public abstract void Close(IPlexWindow win); // omg this probably does something
        public abstract void SetupWindow(IPlexWindow win); // i cant think of what this does
        public abstract void SetupDialog(IPlexWindow win); // how about this???????
        public abstract void InvokeAction(Action act); // i wonder what this invokes
        public abstract void SetTitle(IPlexWindow win, string title); // what is a title again
    }

    // Provides the base functionality for a typical Plex window border, what did you expect
    public interface IWindowBorder
    {
        bool Close(); // CLOSES THE BORDER ALONG WITH ITS WINDOW!!!!!!! HOLY SHIT I DIDNT EXPECT THAT
        string Text { get; set; } // title text exists now
        IPlexWindow ParentWindow { get; set; } // Gets or sets the underlying for this border. 
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingleInstanceAttribute : Attribute
    {

    }
}
