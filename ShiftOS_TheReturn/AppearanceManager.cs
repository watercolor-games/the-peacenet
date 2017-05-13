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
    /// <summary>
    /// Provides functionality for managing windows within ShiftOS.
    /// </summary>
    public static class AppearanceManager
    {
        [Obsolete("Please use Localization.GetAllLanguages().")]
        public static string[] GetLanguages()
        {
            return Localization.GetAllLanguages();
        }

        /// <summary>
        /// Sets the title text of the specified window.
        /// </summary>
        /// <param name="window">The window to modify</param>
        /// <param name="title">The title text to use</param>
        /// <exception cref="ArgumentNullException">Thrown if the window is null.</exception>
        public static void SetWindowTitle(IShiftOSWindow window, string title)
        {
            if (window == null)
                throw new ArgumentNullException("window", "The window cannot be null.");
            winmgr.SetTitle(window, title);
        }

        public static IEnumerable<Type> GetAllWindowTypes()
        {
            List<Type> types = new List<Type>();
            foreach(var file in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                if(file.EndsWith(".exe") || file.EndsWith(".dll"))
                {
                    try
                    {
                        var asm = Assembly.LoadFile(file);
                        foreach(var type in asm.GetTypes())
                        {
                            if (type.GetInterfaces().Contains(typeof(IShiftOSWindow)))
                                types.Add(type);
                        }
                    }
                    catch { }
                }
            }
            return types;
        }

        /// <summary>
        /// Returns the default window title for a specified <see cref="IShiftOSWindow"/>-inheriting type. 
        /// </summary>
        /// <param name="winType">The type to scan</param>
        /// <returns>The default title</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="winType"/> is null.</exception>
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

        /// <summary>
        /// Current cursor position of the console
        /// </summary>
        public static int CurrentPosition { get; set; }

        /// <summary>
        /// We don't know what this does. It may be gone if it does nothing.
        /// </summary>
        public static int LastLength { get; set; }


        /// <summary>
        /// Minimize a window.
        /// </summary>
        /// <param name="form">The window border to minimize.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception> 
        /// <exception cref="EngineModuleDisabledException">Thrown if this part of the engine hasn't been enabled.</exception> 
        public static void Minimize(IWindowBorder form)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (winmgr == null)
                throw new EngineModuleDisabledException();
            winmgr.Minimize(form);
        }

        /// <summary>
        /// Maximizes a window.
        /// </summary>
        /// <param name="form">The window border to maximize.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception> 
        /// <exception cref="EngineModuleDisabledException">Thrown if this engine module hasn't been enabled.</exception>
        public static void Maximize(IWindowBorder form)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (winmgr == null)
                throw new EngineModuleDisabledException();
            winmgr.Maximize(form);
        }


        /// <summary>
        /// Provides a list of all open ShiftOS windows.
        /// </summary>
        public static List<IWindowBorder> OpenForms = new List<IWindowBorder>();

        /// <summary>
        /// Decorates a window with a border, then shows the window.
        /// </summary>
        /// <param name="form">The window to decorate and show.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null. </exception>
        /// <exception cref="EngineModuleDisabledException">Thrown if this engine module has not been initiated yet.</exception> 
        public static void SetupWindow(IShiftOSWindow form)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (winmgr == null)
                throw new EngineModuleDisabledException();
            winmgr.SetupWindow(form);
            Desktop.ResetPanelButtons();
        }

        /// <summary>
        /// Closes the specified window.
        /// </summary>
        /// <param name="win">The window to close.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="win"/> is null. </exception>
        /// <exception cref="EngineModuleDisabledException">Thrown if this engine module has not been initiated yet.</exception> 
        public static void Close(IShiftOSWindow win)
        {
            if (win == null)
                throw new ArgumentNullException("win");
            if (winmgr == null)
                throw new EngineModuleDisabledException();
            winmgr.Close(win);
            Desktop.ResetPanelButtons();
        }

        /// <summary>
        /// Decorates a window with a border, then shows the window, as a dialog box.
        /// </summary>
        /// <param name="form">The window to decorate and show.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null. </exception>
        /// <exception cref="EngineModuleDisabledException">Thrown if this engine module has not been initiated yet.</exception> 
        public static void SetupDialog(IShiftOSWindow form)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (winmgr == null)
                throw new EngineModuleDisabledException();
            winmgr.SetupDialog(form);
            Desktop.ResetPanelButtons();
        }

        /// <summary>
        /// The underlying window manager for this engine module
        /// </summary>
        private static WindowManager winmgr = null;

        /// <summary>
        /// Initiate this engine module, and perform mandatory configuration.
        /// </summary>
        /// <param name="mgr">A working, configured <see cref="WindowManager"/> to use as a backend for this module </param>
        public static void Initiate(WindowManager mgr)
        {
            winmgr = mgr;
        }

        /// <summary>
        /// Raised when the engine is entering its shutdown phase. Save your work!
        /// </summary>
        public static event EmptyEventHandler OnExit;

        /// <summary>
        /// Starts the engine's exit routine, firing the OnExit event.
        /// </summary>
        internal static void Exit()
        {
            OnExit?.Invoke();
            //disconnect from MUD
            ServerManager.Disconnect();
            Desktop.InvokeOnWorkerThread(() =>
            {
                Environment.Exit(0);
            });
        }

        /// <summary>
        /// The current terminal body control.
        /// </summary>
        public static ITerminalWidget ConsoleOut { get; set; }

        /// <summary>
        /// Redirects the .NET <see cref="Console"/> to a new <see cref="TerminalTextWriter"/> instance.  
        /// </summary>
        public static void StartConsoleOut()
        {
            Console.SetOut(new TerminalTextWriter());
        }

        /// <summary>
        /// Invokes an action on the window management thread.
        /// </summary>
        /// <param name="act">The action to invoke</param>
        public static void Invoke(Action act)
        {
            winmgr.InvokeAction(act);
        }
    }

    /// <summary>
    /// Provides the base functionality for a ShiftOS terminal.
    /// </summary>
    public interface ITerminalWidget
    {
        /// <summary>
        /// Write text to this Terminal.
        /// </summary>
        /// <param name="text">Text to write</param>
        void Write(string text);
        /// <summary>
        /// Write text to this Terminal, followed by a newline.
        /// </summary>
        /// <param name="text">Text to write.</param>
        void WriteLine(string text);
        /// <summary>
        /// Clear the contents of this Terminal.
        /// </summary>
        void Clear();
        /// <summary>
        /// Move the cursor to the last character in the Terminal.
        /// </summary>
        void SelectBottom();
    }

    /// <summary>
    /// Provides the base functionality for a ShiftOS window manager.
    /// </summary>
    public abstract class WindowManager
    {
        /// <summary>
        /// Minimizes a window
        /// </summary>
        /// <param name="border">The window border to minimize</param>
        public abstract void Minimize(IWindowBorder border);

        /// <summary>
        /// Maximizes a window
        /// </summary>
        /// <param name="border">The window border to maximize</param>
        public abstract void Maximize(IWindowBorder border);

        /// <summary>
        /// Closes a window
        /// </summary>
        /// <param name="win">The window to close</param>
        public abstract void Close(IShiftOSWindow win);

        /// <summary>
        /// Decorates a window with a window border, then shows it to the user.
        /// </summary>
        /// <param name="win">The window to decorate.</param>
        public abstract void SetupWindow(IShiftOSWindow win);

        /// <summary>
        /// Decorates a window with a border, then shows it to the user as a dialog box.
        /// </summary>
        /// <param name="win">The window to decorate</param>
        public abstract void SetupDialog(IShiftOSWindow win);

        /// <summary>
        /// Invokes an action on the window management thread.
        /// </summary>
        /// <param name="act">The action to invoke.</param>
        public abstract void InvokeAction(Action act);

        /// <summary>
        /// Sets the title text of a window.
        /// </summary>
        /// <param name="win">The window to modify.</param>
        /// <param name="title">The new title text.</param>
        public abstract void SetTitle(IShiftOSWindow win, string title);
    }

    /// <summary>
    /// Provides the base functionality for a typical ShiftOS window border.
    /// </summary>
    public interface IWindowBorder
    {
        /// <summary>
        /// Closes the border along with its window. Unload events should be invoked here.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets or sets the title text for the window border.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the underlying <see cref="IShiftOSWindow"/> for this border. 
        /// </summary>
        IShiftOSWindow ParentWindow { get; set; }
    }
    
    /// <summary>
    /// Provides a way of setting default title text for <see cref="IShiftOSWindow"/> classes. 
    /// </summary>
    public class DefaultTitleAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DefaultTitleAttribute"/>. 
        /// </summary>
        /// <param name="title">A default title to associate with this attribute.</param>
        public DefaultTitleAttribute(string title)
        {
            Title = title;
        }
        
        public string Title { get; private set; }
    }

    /// <summary>
    /// An exception that is thrown when mandatory configuration to run a specific method or module hasn't been done yet.
    /// </summary>
    public class EngineModuleDisabledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineModuleDisabledException"/>. 
        /// </summary>
        public EngineModuleDisabledException() : base("This engine module has not yet been enabled.")
        {

        }
    }
}
