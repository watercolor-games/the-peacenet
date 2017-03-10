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
                if(value == false)
                {
                    foreach(var win in AppearanceManager.OpenForms)
                    {
                        foreach(var attr in win.ParentWindow.GetType().GetCustomAttributes(true))
                        {
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

                _mudConnected = value;
                Desktop.PopulateAppLauncher();
            }
        }

        public static bool IsSafe(Type type)
        {
            if (InKernelMode == true)
                return true;

            foreach (var attrib in type.GetCustomAttributes(false))
            {
                if (attrib is KernelModeAttribute)
                    return false;
            }
            return true;
        }

        public static bool IsSafe(MethodInfo type)
        {
            if (InKernelMode == true)
                return true;

            foreach (var attrib in type.GetCustomAttributes(false))
            {
                if (attrib is KernelModeAttribute)
                    return false;
            }
            return true;
        }


        public static void EnterKernelMode()
        {
            InKernelMode = true;
            Console.WriteLine("<kernel> Watchdog deactivated, system-level access granted.");
        }

        public static void LeaveKernelMode()
        {
            InKernelMode = false;
            Console.WriteLine("<kernel> Kernel mode disabled.");
        }
    }
}
