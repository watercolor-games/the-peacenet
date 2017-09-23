using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class Terminal
    {
        private static string _shelloverride = "";

        public static void SetShellOverride(string value)
        {
            _shelloverride = value;
        }

        public static List<TerminalCommand> Commands { get; private set; }

        public static void Populate()
        {
            Commands = new List<TerminalCommand>();
            foreach (var type in ReflectMan.Types)
            {
                foreach (var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {

                    var cmd = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ServerCommand) as ServerCommand;
                    if (cmd != null)
                    {
                        var tc = new TerminalCommand();
                        tc.RequiresElevation = false;

                        var shellConstraint = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShellConstraintAttribute) as ShellConstraintAttribute;
                        tc.ShellMatch = (shellConstraint == null) ? "" : shellConstraint.Shell;

                        if (mth.GetCustomAttributes(false).FirstOrDefault(x => x is MetaCommandAttribute) != null)
                        {
                            tc.ShellMatch = "metacmd";
                        }

                        tc.CommandInfo = cmd;
                        tc.RequiresElevation = false;
                        tc.RequiredArguments = new List<string>();
                        foreach (var arg in mth.GetCustomAttributes(false).Where(x => x is RequiresArgument))
                        {
                            var rarg = arg as RequiresArgument;
                            tc.RequiredArguments.Add(rarg.Name);
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


        public static bool RunClient(string text, Dictionary<string, object> args, string session_id)
        {
            var cmd = Commands.FirstOrDefault(x => x.CommandInfo.name == text);
            if (cmd == null)
                return false;
            if (!UpgradeManager.IsUpgradeLoaded(cmd.Dependencies, session_id))
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
                cmd.Invoke(args, _shelloverride);
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine("Command error: " + ex.InnerException.Message);
            }

            return true;
        }

        [ServerMessageHandler("trm_invoke")]
        [SessionRequired]
        public static void InvokeCMD(string session_id, string content, string ip, int port)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var memwriter = new MemoryTextWriter();
            Console.SetOut(memwriter);
            SetShellOverride(data["shell"].ToString());
            bool result = RunClient(data["cmd"].ToString(), data["args"] as Dictionary<string, object>, session_id);
            SetShellOverride("");
            var str = Console.OpenStandardOutput();
            var writer = new System.IO.StreamWriter(str);
            writer.AutoFlush = true;
            Console.SetOut(writer);

            Program.SendMessage(new Objects.PlexServerHeader
            {
                Message = "trm_result",
                Content = JsonConvert.SerializeObject(new
                {
                    result = result,
                    message = memwriter.ToString()
                }),
                IPForwardedBy = ip,
                SessionID = session_id
            }, port);
        }
    }
}
