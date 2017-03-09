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
    public static class TerminalBackend
    {
        public static event Action<string, string> CommandProcessed;

        public static bool Elevated { get; set; }

        public static Dictionary<string, object> GetArgs(ref string text)
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
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(args);
        }

        public static string LastCommand = "";

        public static void InvokeCommand(string text, bool isRemote = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var args = GetArgs(ref text);

                bool commandWasClient = RunClient(text, args, isRemote);

                if (!commandWasClient && !string.IsNullOrWhiteSpace(text))
                {
                    PrefixEnabled = false;
                    ServerManager.SendMessage("script", $@"{{
    user: ""{text.Split('.')[0]}"",
    script: ""{text.Split('.')[1]}"",
    args: ""{JsonConvert.SerializeObject(args)}""
}}");
                }
                CommandProcessed?.Invoke(text, JsonConvert.SerializeObject(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}");
                PrefixEnabled = true;

            }
        }

        public static bool PrefixEnabled { get; set; }

        public static bool InStory { get; set; }

        public static string latestCommmand = "";

        public static event EmptyEventHandler TerminalRequested;

        internal static void OpenTerminal()
        {
            TerminalRequested?.Invoke();
        }

        public static bool CanRunRemotely(MethodInfo mth, bool isRemote)
        {
            if (!isRemote)
                return true;

            foreach(var attr in mth.GetCustomAttributes(false))
            {
                if (attr is RemoteLockAttribute)
                    return false;
            }

            return true;
        }

        public static bool RunClient(string text, Dictionary<string, object> args, bool isRemote = false)
        {
            latestCommmand = text;

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
                                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                            {
                                                if (Shiftorium.UpgradeAttributesUnlocked(method))
                                                {
                                                    if (CanRunRemotely(method, isRemote))
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
                                                                        Console.WriteLine("<watchdog> You cannot run this command.");
                                                                        KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                                                        return true;
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine(text + " cannot be ran in a remote session");
                                                        return true;
                                                    }
                                                }

                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("<watchdog> You cannot run this command.");
                                            KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                            return true;
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

        public static void PrintPrompt()
        {
            ConsoleEx.Italic = false;
            ConsoleEx.Underline = false;

            ConsoleEx.ForegroundColor = ConsoleColor.Magenta;
            ConsoleEx.Bold = true;
            Console.Write(SaveSystem.CurrentSave.Username);
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
            ConsoleEx.ForegroundColor = ConsoleColor.White;
            Console.Write(" ");
        }


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
                else if(msg.Name == "handshake_from")
                {
                    var a = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                    string uName = a["username"] as string;
                    string pass = a["password"] as string;
                    string sys = a["sysname"] as string;
                    string guid = msg.GUID;
                    if(SaveSystem.CurrentSave.Username == uName && SaveSystem.CurrentSave.Password == pass && CurrentSave.SystemName == sys)
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

        public static bool IsForwardingConsoleWrites { get; internal set; }
        public static string ForwardGUID { get; internal set; }
        
    }
}
