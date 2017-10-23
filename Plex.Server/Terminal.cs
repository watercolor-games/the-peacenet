using System;
using System.Collections.Generic;
using System.IO;
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

        public static string SessionID { get; private set; }

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


        public static bool RunClient(string text, Dictionary<string, object> args, string session_id, bool isServerAdmin = false)
        {
            SessionID = session_id;
            var cmd = Commands.FirstOrDefault(x => x.CommandInfo.name == text);
            if (cmd == null)
                return false;
            if(((ServerCommand)cmd.CommandInfo).ServerOnly == true && isServerAdmin == false)
            {
                Console.WriteLine("You can't run this command as you are not a server admin.");
                return true;
            }
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

        [ServerCommand("echo", "Prints the desired text on-screen.")]
        [RequiresArgument("id")]
        public static void Echo(Dictionary<string, object> args)
        {
            Console.WriteLine(args["id"].ToString());
        }

        public static RequestInfo SessionInfo { get; private set; }

        [ServerMessageHandler( ServerMessageType.TRM_GETCMDS)]
        [SessionRequired]
        public static void GetHelp(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            Dictionary<string, string> commands = new Dictionary<string, string>();
            foreach (var cmd in Terminal.Commands)
            {
                if (UpgradeManager.IsUpgradeLoaded(cmd.Dependencies, session_id))
                    commands.Add(cmd.CommandInfo.name, cmd.CommandInfo.description);
            }
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            writer.Write(commands.Count);
            foreach(var cmd in commands)
            {
                writer.Write(cmd.Key);
                writer.Write(cmd.Value);
            }
        }


        [ServerMessageHandler( ServerMessageType.TRM_INVOKE)]
        [SessionRequired]
        public static void InvokeCMD(string session_id, BinaryReader reader, BinaryWriter writer)
        {
            string datajson = reader.ReadString();
            SessionInfo = new RequestInfo
            {
                SessionID = session_id
            };
            var outstream = Console.Out;
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(datajson);
            var memwriter = new RemoteTextWriter(writer, session_id); //We use this to forward all console writes to the client that ran this command.
            Console.SetOut(memwriter);
            SetShellOverride(data["shell"].ToString());
            string sessionfwd = (string.IsNullOrWhiteSpace(data["sessionfwd"] as string)) ? session_id : data["sessionfwd"].ToString();
            writer.Write((int)ServerResponseType.REQ_SUCCESS);
            writer.Write(session_id);
            bool result = RunClient(data["cmd"].ToString(), JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(data["args"])), sessionfwd);
            SetShellOverride("");
            Console.SetOut(outstream);
            writer.Write("\u0013\u0014");
        }
    }

    public class RequestInfo
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string SessionID { get; set; }
    }

    public class RemoteTextWriter : System.IO.TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        private BinaryWriter _writer = null;
        private string _session = "";

        public RemoteTextWriter(BinaryWriter writer, string session)
        {
            _writer = writer;
            _session = session;
        }

        public override void Write(string value)
        {
            _writer.Write(value);
        }

        public override void WriteLine(string value)
        {
            Write(value + Environment.NewLine);
        }

    }
}
