using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.Engine
{
    public static class KernelWatchdog
    {
        //store logs into a file
        public static void Log(string e, string desc)
        {
            string line = $"[{DateTime.Now}] <{e}> {desc}";
            if (FileExists("0:/system/data/kernel.log"))
            {
                string contents = ReadAllText("0:/system/data/kernel.log");
                contents += Environment.NewLine + line;
                WriteAllText("0:/system/data/kernel.log", contents);
            }
            else
            {
                WriteAllText("0:/system/data/kernel.log", line);
            }
        }

        private static bool _mudConnected = true;

        public static bool InKernelMode { get; private set; }
        public static bool MudConnected
        {
            get
            {
                return _mudConnected;
            }
            set
            {
                if(value == false) // hey game if you want to disconnect from mud do this:
                {
                    foreach(var win in AppearanceManager.OpenForms)
                    {
                        foreach(var attr in win.ParentWindow.GetType().GetCustomAttributes(true))
                        {
                            // prevents disconnect from mud if an application that needs a connection is open
                            if(attr is MultiplayerOnlyAttribute)
                            {
                                ConsoleEx.Bold = true;
                                ConsoleEx.Underline = false;
                                ConsoleEx.Italic = true;
                                ConsoleEx.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Error:");
                                ConsoleEx.Bold = false;
                                ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine("Cannot disconnect from multi-user domain because an app that depends on it is open.");
                                TerminalBackend.PrintPrompt();
                                return;
                            }
                        }
                    }
                }

                _mudConnected = value; // connects or disconnects from mud
                Desktop.PopulateAppLauncher();
            }
        }


        static string regularUsername = ""; //put regular username in here later


        //determines if you can disconnect from mud if there are no applications that currently need to
        internal static bool CanRunOffline(Type method)
        {
            if (MudConnected)
                return true;

            foreach (var attr in method.GetCustomAttributes(false))
            {
                if (attr is MultiplayerOnlyAttribute)
                    return false;
            }
            return true;
        }
        
        //same as above but this time for methods
        internal static bool CanRunOffline(MethodInfo method)
        {
            if (MudConnected)
                return true;

            foreach(var attr in method.GetCustomAttributes(false))
            {
                if (attr is MultiplayerOnlyAttribute)
                    return false;
            }
            return true;
        }
    }
}
