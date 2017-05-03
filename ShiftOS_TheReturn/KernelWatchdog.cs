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
            if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Root)
                return true;

            foreach (var attrib in type.GetCustomAttributes(false))
            {
                if (attrib is KernelModeAttribute)
                {
                    if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Root)
                        return true;
                    return false;
                }
            }
            return true;
        }

        public static bool IsSafe(MethodInfo type)
        {
            if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Root)
                return true;

            foreach (var attrib in type.GetCustomAttributes(false))
            {
                if (attrib is KernelModeAttribute)
                {
                    if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Root)
                        return true;
                    return false;
                }
            }
            return true;
        }

        static string regularUsername = "";


        public static void EnterKernelMode()
        {
            regularUsername = SaveSystem.CurrentUser.Username;
            SaveSystem.CurrentUser = SaveSystem.Users.FirstOrDefault(x => x.Username == "root");

        }

        public static void LeaveKernelMode()
        {
            var user = SaveSystem.Users.FirstOrDefault(x => x.Username == regularUsername);
            if (user == null)
                throw new Exception("User not in root mode.");
            SaveSystem.CurrentUser = user;
        }

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
