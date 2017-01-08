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

        public static void InvokeCommand(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var args = GetArgs(ref text);

                bool commandWasClient = RunClient(text, args);

                if (!commandWasClient && !string.IsNullOrWhiteSpace(text))
                {
                    PrefixEnabled = false;
                    ServerManager.SendMessage("script", $@"{{
    user: ""{text.Split('.')[0]}"",
    script: ""{text.Split('.')[1]}"",
    args: ""{JsonConvert.SerializeObject(args)}""
}}");
                }
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

        public static bool RunClient(string text, Dictionary<string, object> args)
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
                                                            catch
                                                            {
                                                                return (bool)method.Invoke(null, new object[] { });
                                                            }
                                                        }
                                                    }
                                                }
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
        

    }
}
