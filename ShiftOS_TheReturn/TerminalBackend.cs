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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShiftOS.Engine.SaveSystem;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Backend for the ShiftOS terminal.
    /// </summary>
    public static class TerminalBackend
    {
        /// <summary>
        /// Occurs when a command is processed.
        /// </summary>
        public static event Action<string, string> CommandProcessed;

        /// <summary>
        /// Gets or sets whether the current command is elevated.
        /// </summary>
        public static bool Elevated { get; set; }

        /// <summary>
        /// Parses command-line arguments using the ShiftOS syntax and stores them in a <see cref="Dictionary{string, string}"/>, removing the parsed text from the original string.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns><see cref="Dictionary{string, string}"/> containing the parsed arguments.</returns>
        public static Dictionary<string, string> GetArgs(ref string text)
        {
            bool shouldParse = false;
            int argStart = 0;
            if (text.Contains("{"))
            {
                shouldParse = true;
                argStart = text.IndexOf('{');
            }

            if (shouldParse == false)
            {
                string replacement = Regex.Replace(text, @"\t|\n|\r", "");
                text = replacement + "{}";
                shouldParse = true;
                argStart = text.IndexOf('{');
            }

            string args = text.Substring(argStart, text.Length - argStart);

            text = text.Remove(argStart, text.Length - argStart).Replace(" ", "");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(args);
        }

        /// <summary>
        /// String representing the last command entered by the user.
        /// </summary>
        public static string LastCommand = "";

        /// <summary>
        /// Invokes a ShiftOS terminal command.
        /// </summary>
        /// <param name="ns">The command's namespace.</param>
        /// <param name="command">The command name.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string ns, string command, Dictionary<string, string> arguments, bool isRemote = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ns))
                    return;


                bool commandWasClient = RunClient(ns, command, arguments, isRemote);

                if (!commandWasClient && !string.IsNullOrWhiteSpace(ns))
                {
                    PrefixEnabled = false;

                    ServerManager.SendMessage("script", $@"{{
    user: ""{ns}"",
    script: ""{command}"",
    args: ""{GetSentArgs(arguments)}""
}}");
                }

                CommandProcessed?.Invoke(ns + "." + command, JsonConvert.SerializeObject(arguments));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}"); // This shouldn't ever be called now
                PrefixEnabled = true;

            }
        }

        /// <summary>
        /// Transforms a <see cref="Dictionary{String, String}"/> of arguments to a <see cref="Dictionary{String, Object}"/>.  
        /// </summary>
        /// <param name="argss">The original argument dictionary to convert.</param>
        /// <returns>The converted dictionary.</returns>
        public static string GetSentArgs(Dictionary<string, string> argss)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return JsonConvert.SerializeObject(args);
        }

        /// <summary>
        /// Invokes a ShiftOS terminal command.
        /// </summary>
        /// <param name="text">The full command text in regular ShiftOS syntax</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string text, bool isRemote = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var args = GetArgs(ref text);

                bool commandWasClient = RunClient(text, args, isRemote);

                if (!commandWasClient)
                {
                    PrefixEnabled = false;

                    ServerManager.SendMessage("script", $@"{{
    user: ""{text.Split('.')[0]}"",
    script: ""{text.Split('.')[1]}"",
    args: ""{GetSentArgs(args)}""
}}");
                }
                CommandProcessed?.Invoke(text, GetSentArgs(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}");
                PrefixEnabled = true;

            }
        }

        /// <summary>
        /// Gets or sets whether the user prefix is printed after a command completes.
        /// </summary>
        public static bool PrefixEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the user is in a story plot, and thus, the terminal input should be disabled.
        /// </summary>
        public static bool InStory { get; set; }

        /// <summary>
        /// Another latest command string.
        /// </summary>
        public static string latestCommmand = "";

        /// <summary>
        /// Occurs when the engine requests a Terminal to be open.
        /// </summary>
        public static event EmptyEventHandler TerminalRequested;

        /// <summary>
        /// Opens a Terminal.
        /// </summary>
        internal static void OpenTerminal()
        {
            TerminalRequested?.Invoke();
        }

        /// <summary>
        /// Determines if the specified command method can be ran in RTS
        /// </summary>
        /// <param name="mth">The method to scan</param>
        /// <param name="isRemote">Is the user in an RTS session?</param>
        /// <returns>Whether the command can be run.</returns>
        public static bool CanRunRemotely(MethodInfo mth, bool isRemote)
        {
            if (!isRemote)
                return true;

            foreach (var attr in mth.GetCustomAttributes(false))
            {
                if (attr is RemoteLockAttribute)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Runs a command on the client-side.
        /// </summary>
        /// <param name="ns">The command's namespace.</param>
        /// <param name="cmd">The command name.</param>
        /// <param name="args">The command's arguments.</param>
        /// <param name="isRemote">Whether the command should be ran through RTS.</param>
        /// <returns>Whether the command ran successfully.</returns>
        public static bool RunClient(string ns, string cmd, Dictionary<string, string> args, bool isRemote = false)
        {
            return RunClient(ns + "." + cmd, args, isRemote);
        }

        /// <summary>
        /// Runs a command on the client.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="argss">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be ran through RTS.</param>
        /// <returns>Whether the command ran successfully.</returns>
        public static bool RunClient(string text, Dictionary<string, string> argss, bool isRemote = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return RunClient(text, args, isRemote);
        }

        /// <summary>
        /// Runs a command on the client.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="args">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be run in RTS.</param>
        /// <returns>Whether the command ran successfully.</returns>
        public static bool RunClient(string text, Dictionary<string, object> args, bool isRemote = false)
        {
            latestCommmand = text;

            //Console.WriteLine(text + " " + "{" + string.Join(",", args.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}" + " " + isRemote);

            foreach (var asmExec in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                try
                {
                    var asm = Assembly.LoadFile(asmExec);

                    var types = asm.GetTypes();
                    foreach (var type in types)
                    {
                        if (Shiftorium.UpgradeAttributesUnlocked(type))
                        {
                            foreach (var a in type.GetCustomAttributes(false))
                            {
                                if (a is Namespace)
                                {
                                    var ns = a as Namespace;
                                    if (text.Split('.')[0] == ns.name)
                                    {
                                        if (KernelWatchdog.IsSafe(type))
                                        {
                                            if (KernelWatchdog.CanRunOffline(type))
                                            {
                                                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                                {
                                                    if (Shiftorium.UpgradeAttributesUnlocked(method))
                                                    {
                                                        foreach (var ma in method.GetCustomAttributes(false))
                                                        {
                                                            if (ma is Command)
                                                            {
                                                                var cmd = ma as Command;
                                                                if (text.Split('.')[1] == cmd.name)
                                                                {
                                                                    if (KernelWatchdog.IsSafe(method))
                                                                    {
                                                                        if (KernelWatchdog.CanRunOffline(method))
                                                                        {
                                                                            var attr = method.GetCustomAttribute<CommandObsolete>();

                                                                            if (attr != null)
                                                                            {
                                                                                string newcommand = attr.newcommand;
                                                                                if (attr.warn)
                                                                                {
                                                                                    Console.WriteLine(Localization.Parse((newcommand == "" ? "{ERROR}" : "{WARN}") + attr.reason, new Dictionary<string, string>() {
                                                                {"%newcommand", newcommand}
                                                            }));
                                                                                }
                                                                                if (newcommand != "")
                                                                                {
                                                                                    // redo the entire process running newcommand

                                                                                    return RunClient(newcommand, args);
                                                                                }
                                                                            }

                                                                            var requiresArgs = method.GetCustomAttributes<RequiresArgument>();
                                                                            bool error = false;
                                                                            bool providedusage = false;

                                                                            foreach (RequiresArgument argument in requiresArgs)
                                                                            {
                                                                                if (!args.ContainsKey(argument.argument))
                                                                                {

                                                                                    if (!providedusage)
                                                                                    {
                                                                                        string usageparse = "{COMMAND_" + ns.name.ToUpper() + "_" + cmd.name.ToUpper() + "_USAGE}";
                                                                                        if (usageparse == Localization.Parse(usageparse))
                                                                                            usageparse = "";
                                                                                        else
                                                                                            usageparse = Shiftorium.UpgradeInstalled("help_usage") ? Localization.Parse("{ERROR}{USAGE}" + usageparse, new Dictionary<string, string>() {
                                                                        {"%ns", ns.name},
                                                                        {"%cmd", cmd.name}
                                                                    }) : "";

                                                                                        Console.WriteLine(usageparse);

                                                                                        providedusage = true;
                                                                                    }
                                                                                    if (Shiftorium.UpgradeInstalled("help_usage"))
                                                                                    {
                                                                                        Console.WriteLine(Localization.Parse("{ERROR_ARGUMENT_REQUIRED}", new Dictionary<string, string>() {
                                                                    {"%argument", argument.argument}
                                                                }));
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine(Localization.Parse("{ERROR_ARGUMENT_REQUIRED_NO_USAGE}"));
                                                                                    }

                                                                                    error = true;
                                                                                }
                                                                            }

                                                                            if (error)
                                                                            {
                                                                                throw new Exception("{ERROR_COMMAND_WRONG}");
                                                                            }

                                                                            try
                                                                            {
                                                                                return (bool)method.Invoke(null, new[] { args });
                                                                            }
                                                                            catch (TargetInvocationException e)
                                                                            {
                                                                                Console.WriteLine(Localization.Parse("{ERROR_EXCEPTION_THROWN_IN_METHOD}"));
                                                                                Console.WriteLine(e.InnerException.Message);
                                                                                Console.WriteLine(e.InnerException.StackTrace);
                                                                                return true;
                                                                            }
                                                                            catch
                                                                            {
                                                                                return (bool)method.Invoke(null, new object[] { });
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.Write("<");
                                                                            ConsoleEx.Bold = true;
                                                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                                            Console.Write("session_mgr");
                                                                            ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                                            ConsoleEx.Bold = false;
                                                                            Console.Write(">");
                                                                            ConsoleEx.Italic = true;
                                                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                                            Console.WriteLine(" You cannot run this command while disconnected from the multi-user domain..");
                                                                            return true;

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Admin)
                                                                        {
                                                                            Infobox.PromptText("Elevate to root mode", "This command cannot be run as a regular user. To run this command, please enter your password to elevate to root mode temporarily.", (pass) =>
                                                                            {
                                                                                if (pass == SaveSystem.CurrentUser.Password)
                                                                                {
                                                                                    KernelWatchdog.EnterKernelMode();
                                                                                    RunClient(text, args, isRemote);
                                                                                    KernelWatchdog.LeaveKernelMode();
                                                                                }
                                                                                else
                                                                                {
                                                                                    Infobox.Show("Access denied.", "You did not type in the correct password.");
                                                                                }
                                                                            }, true);
                                                                            return true;
                                                                        }
                                                                        Console.Write("<");
                                                                        ConsoleEx.Bold = true;
                                                                        ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                                        Console.Write("watchdog");
                                                                        ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                                        ConsoleEx.Bold = false;
                                                                        Console.Write(">");
                                                                        ConsoleEx.Italic = true;
                                                                        ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                                        Console.WriteLine(" You cannot run this command. You do not have permission. Incident reported.");
                                                                        KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                                                        return true;
                                                                    }
                                                                }
                                                            }


                                                        }
                                                    }

                                                    else
                                                    {
                                                        Console.Write("<");
                                                        ConsoleEx.Bold = true;
                                                        ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                        Console.Write("session_mgr");
                                                        ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                        ConsoleEx.Bold = false;
                                                        Console.Write(">");
                                                        ConsoleEx.Italic = true;
                                                        ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                        Console.WriteLine(" You cannot run this command while disconnected from the multi-user domain..");
                                                        return true;

                                                    }

                                                }
                                            } 
                                            else
                                            {
                                                if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Admin)
                                                {
                                                    Infobox.PromptText("Elevate to root mode", "This command cannot be run as a regular user. To run this command, please enter your password to elevate to root mode temporarily.", (pass) =>
                                                    {
                                                        if (pass == SaveSystem.CurrentUser.Password)
                                                        {
                                                            KernelWatchdog.EnterKernelMode();
                                                            RunClient(text, args, isRemote);
                                                            KernelWatchdog.LeaveKernelMode();
                                                        }
                                                        else
                                                        {
                                                            Infobox.Show("Access denied.", "You did not type in the correct password.");
                                                        }
                                                    }, true);
                                                    return true;
                                                }
                                                Console.Write("<");
                                                ConsoleEx.Bold = true;
                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                Console.Write("watchdog");
                                                ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                ConsoleEx.Bold = false;
                                                Console.Write(">");
                                                ConsoleEx.Italic = true;
                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                Console.WriteLine(" You cannot run this command. You do not have permission. Incident reported.");
                                                KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Prints the user prompt to the terminal.
        /// </summary>
        public static void PrintPrompt()
        {
            if (SaveSystem.CurrentSave != null && CurrentUser != null)
            {
                Desktop.InvokeOnWorkerThread(() =>
                {
                    ConsoleEx.BackgroundColor = SkinEngine.LoadedSkin.TerminalBackColorCC;
                    ConsoleEx.Italic = false;
                    ConsoleEx.Underline = false;

                    ConsoleEx.ForegroundColor = ConsoleColor.Magenta;
                    ConsoleEx.Bold = true;

                    Console.Write(SaveSystem.CurrentUser.Username);
                    ConsoleEx.Bold = false;
                    ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("@");
                    ConsoleEx.Italic = true;
                    ConsoleEx.Bold = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(SaveSystem.CurrentSave.SystemName);
                    ConsoleEx.Italic = false;
                    ConsoleEx.Bold = false;
                    ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(":~");
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleEx.Italic = true;
                    if (KernelWatchdog.InKernelMode == true)
                        Console.Write("#");
                    else
                        Console.Write("$");
                    ConsoleEx.Italic = false;
                    ConsoleEx.Bold = false;
                    ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                    Console.Write(" ");
                });
            }
        }

        /// <summary>
        /// Static constructor for <see cref="TerminalBackend"/>. 
        /// </summary>
        static TerminalBackend()
        {
            ServerMessageReceived onMessageReceived = (msg) =>
            {
                if (msg.Name == "trm_invokecommand")
                {
                    string text3 = "";
                    string text4 = msg.Contents;

                    if (TerminalBackend.PrefixEnabled)
                    {
                        text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                    }
                    IsForwardingConsoleWrites = true;
                    if (TerminalBackend.InStory == false)
                    {
                        TerminalBackend.InvokeCommand(text3, true);
                    }
                    if (TerminalBackend.PrefixEnabled)
                    {
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                    }
                    IsForwardingConsoleWrites = false;
                }
                else if (msg.Name == "pleasewrite")
                {
                    Console.Write(msg.Contents);
                }
                else if (msg.Name == "handshake_from")
                {
                    var a = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                    string uName = a["username"] as string;
                    string pass = a["password"] as string;
                    string sys = a["sysname"] as string;
                    string guid = msg.GUID;
                    if (SaveSystem.CurrentSave.Username == uName && SaveSystem.CurrentSave.Password == pass && CurrentSave.SystemName == sys)
                    {
                        ForwardGUID = guid;
                        ServerManager.SendMessage("trm_handshake_accept", $@"{{
    guid: ""{ServerManager.thisGuid}"",
    target: ""{guid}""
}}");

                        IsForwardingConsoleWrites = true;
                        InvokeCommand("sos.status");
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                        IsForwardingConsoleWrites = false;
                    }
                }
            };

            ServerManager.MessageReceived += onMessageReceived;
        }

        /// <summary>
        /// Gets whether the terminal backend is forwarding console write requests through RTS to a remote client.
        /// </summary>
        public static bool IsForwardingConsoleWrites { get; internal set; }

        /// <summary>
        /// Gets the RTS forward GUID.
        /// </summary>
        public static string ForwardGUID { get; internal set; }

        /// <summary>
        /// Occurs when the user inputs text in a Terminal.
        /// </summary>
        public static event TextSentEventHandler TextSent;

        /// <summary>
        /// Fakes the user inputting text to a Terminal.
        /// </summary>
        /// <param name="text">The text to input.</param>
        public static void SendText(string text)
        {
            TextSent?.Invoke(text);
        }

    }
}
