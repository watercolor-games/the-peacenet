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
using System.Diagnostics;
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
        /// Gets the output of the last command.
        /// </summary>
        public static string LastCommandBuffer { get; private set; }

        /// <summary>
        /// Invokes a ShiftOS terminal command.
        /// </summary>
        /// <param name="command">The command name.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string command, Dictionary<string, string> arguments, bool isRemote = false)
        {
            try
            {
                bool commandWasClient = RunClient(command, arguments, isRemote);

                if (!commandWasClient)
                {
                    Console.WriteLine("{ERR_COMMANDNOTFOUND}");
                }

                CommandProcessed?.Invoke(command, JsonConvert.SerializeObject(arguments));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{ERR_SYNTAXERROR}");
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

        public class TerminalCommand
        {
            public override int GetHashCode()
            {
                int hash = 0;
                foreach (char c in ToString())
                {
                    hash += (int)c;
                }
                return hash;
            }

            public Command CommandInfo { get; set; }

            public List<string> RequiredArguments { get; set; }
            public string Dependencies { get; set; }

            public MethodInfo CommandHandler;

            public Type CommandType;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.CommandInfo.name);
                if (this.RequiredArguments.Count > 0)
                {
                    sb.Append(" ");
                    foreach (var arg in RequiredArguments)
                    {
                        sb.Append("--" + arg);
                        sb.Append(" ");
                        if (RequiredArguments.IndexOf(arg) < RequiredArguments.Count - 1)
                            sb.Append(',');
                    }
                    sb.Append("}");
                }
                sb.Append("|");
                sb.Append(CommandHandler.Name + "()");
                return sb.ToString();
            }

            public bool RequiresElevation { get; set; }

            public virtual void Invoke(Dictionary<string, object> args)
            {
                List<string> errors = new List<string>();
                bool requiresAuth = false;
                if (!KernelWatchdog.IsSafe(this))
                {
                    if (SaveSystem.CurrentUser.Permissions == Objects.UserPermissions.Admin)
                        requiresAuth = true;
                    else
                        errors.Add("You can't run this command - you do not have permission.");
                }
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine("Command error: " + error);
                    }
                    return;
                }
                if (requiresAuth)
                {
                    Infobox.PromptText("Enter your password.", "This command requires you to have elevated permissions. Please enter your password to confirm this action.", (pass) =>
                    {
                        if (pass == SaveSystem.CurrentUser.Password)
                        {
                            var uname = SaveSystem.CurrentUser.Username;
                            SaveSystem.CurrentUser = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == "root");
                            try
                            {
                                var h = CommandHandler;
                                h.Invoke(null, new[] { args });
                            }
                            catch
                            {
                                var h = CommandHandler;
                                h.Invoke(null, null);
                            }
                            SaveSystem.CurrentUser = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == uname);
                        }
                        else
                        {
                            Infobox.Show("Access denied.", "Incorrect password provided. The command will not run.");
                        }
                    }, true);
                }

                try
                {
                    CommandHandler.Invoke(null, new[] { args });
                }
                catch
                {
                    CommandHandler.Invoke(null, null);
                }
            }
        }

        public class WinOpenCommand : TerminalCommand
        {
            public Type ShiftOSWindow { get; set; }


            public override void Invoke(Dictionary<string, object> args)
            {
                AppearanceManager.SetupWindow((IShiftOSWindow)Activator.CreateInstance(ShiftOSWindow, null));
            }


        }

        public class MemoryTextWriter : System.IO.TextWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.Unicode;
                }
            }

            private StringBuilder sb = null;

            public MemoryTextWriter()
            {
                sb = new StringBuilder();
            }

            public override string ToString()
            {
                return sb.ToString();
            }

            public override void Write(char value)
            {
                sb.Append(value);
            }

            public override void WriteLine()
            {
                sb.AppendLine();
            }

            public override void Write(string value)
            {
                sb.Append(value);
            }

            public override void Close()
            {
                sb.Clear();
                sb = null;
                base.Close();
            }

            public override void WriteLine(string value)
            {
                sb.AppendLine(value);
            }
        }

        public static List<TerminalCommand> Commands { get; private set; }

        public static void PopulateTerminalCommands()
        {
            Commands = new List<TerminalCommand>();
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IShiftOSWindow)))
                {
                    var winopenattrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is WinOpenAttribute) as WinOpenAttribute;
                    if(winopenattrib != null)
                    {
                        var winc = new WinOpenCommand();
                        winc.CommandType = type;
                        var rupg = type.GetCustomAttributes().FirstOrDefault(x => x is RequiresUpgradeAttribute) as RequiresUpgradeAttribute;
                        if (rupg != null)
                            winc.Dependencies = rupg.Upgrade;
                        winc.CommandInfo = new Engine.Command(winopenattrib.ID, "", "Opens the \"" + winopenattrib.ID + " program.");
                        winc.RequiredArguments = new List<string>();
                        winc.RequiresElevation = false;
                        winc.ShiftOSWindow = type;

                        var ambiguity = Commands.FirstOrDefault(x => x.CommandInfo.name == winc.CommandInfo.name);
                        if (ambiguity != null)
                            throw new Exception("Ambiguity error. The program " + winc.CommandInfo.name + " collides with another program or terminal command with the same name. Please either change the already-existing program/command, or change this one's WinOpenAttribute value to compensate.");
                        Commands.Add(winc);
                    }
                }
                foreach (var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var cmd = mth.GetCustomAttributes(false).FirstOrDefault(x => x is Command);
                    if (cmd != null)
                    {
                        var tc = new TerminalCommand();
                        tc.RequiresElevation = !(type.GetCustomAttributes(false).FirstOrDefault(x => x is KernelModeAttribute) == null);


                        tc.CommandInfo = cmd as Command;
                        tc.RequiresElevation = tc.RequiresElevation || !(mth.GetCustomAttributes(false).FirstOrDefault(x => x is KernelModeAttribute) == null);
                        tc.RequiredArguments = new List<string>();
                        foreach (var arg in mth.GetCustomAttributes(false).Where(x => x is RequiresArgument))
                        {
                            var rarg = arg as RequiresArgument;
                            tc.RequiredArguments.Add(rarg.argument);
                        }
                        var rupg = mth.GetCustomAttributes(false).FirstOrDefault(x => x is RequiresUpgradeAttribute) as RequiresUpgradeAttribute;
                        if (rupg != null)
                            tc.Dependencies = rupg.Upgrade;
                        else
                            tc.Dependencies = "";
                        tc.CommandType = type;
                        tc.CommandHandler = mth;

                        var ambiguity = Commands.FirstOrDefault(x => x.CommandInfo.name == tc.CommandInfo.name);
                        if (ambiguity != null)
                            throw new Exception("Command ambiguity error. You can't have two commands with the same name: " + $"{tc} == {ambiguity}");

                        if (!Commands.Contains(tc))
                            Commands.Add(tc);
                    }
                }

            }
            Console.WriteLine("[termdb] " + Commands.Count + " commands found.");
        }

        /// <summary>
        /// Invokes a ShiftOS terminal command.
        /// </summary>
        /// <param name="text">The full command text in regular ShiftOS syntax</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string text, bool isRemote = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            var tw = new MemoryTextWriter();
            Console.SetOut(tw);
            try
            {
                var args = GetArgs(ref text);

                bool commandWasClient = RunClient(text, args, isRemote);

                if (!commandWasClient)
                {
                    Console.WriteLine("Error: Command not found.");
                    
                }
                CommandProcessed?.Invoke(text, GetSentArgs(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}");
                PrefixEnabled = true;

            }
            string buffer = tw.ToString();
            LastCommandBuffer = buffer;
            Console.SetOut(new TerminalTextWriter());
            if(!isRemote)
                Console.Write(buffer);

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


            var cmd = Commands.FirstOrDefault(x => Localization.Parse(x.CommandInfo.name) == text);
            if (cmd == null)
                return false;
            if (!Shiftorium.UpgradeInstalled(cmd.Dependencies))
                return false;
            bool res = false;
            foreach (var arg in cmd.RequiredArguments)
            {
                if (!args.ContainsKey(arg))
                {
                    res = true;
                    Console.WriteLine("You are missing an argument with the key \"" + arg + "\".");
                }
            }
            if (res == true)
                return true;
            try
            {
                cmd.Invoke(args);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Command error: " + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Prints the user prompt to the terminal.
        /// </summary>
        public static void PrintPrompt()
        {
            Console.WriteLine();
            if (SaveSystem.CurrentSave != null && CurrentUser != null)
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
                ConsoleEx.Flush();
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
                        text3 = text4.Remove(0, $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                    }
                    IsForwardingConsoleWrites = true;
                    if (TerminalBackend.InStory == false)
                    {
                        TerminalBackend.InvokeCommand(text3, true);
                    }
                    if (TerminalBackend.PrefixEnabled)
                    {
                        Console.Write($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
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
                    if (SaveSystem.CurrentUser.Username == uName && SaveSystem.CurrentSave.Password == pass && CurrentSave.SystemName == sys)
                    {
                        ForwardGUID = guid;
                        ServerManager.SendMessage("trm_handshake_accept", $@"{{
    guid: ""{ServerManager.thisGuid}"",
    target: ""{guid}""
}}");

                        IsForwardingConsoleWrites = true;
                        InvokeCommand("sos.status");
                        Console.Write($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
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
