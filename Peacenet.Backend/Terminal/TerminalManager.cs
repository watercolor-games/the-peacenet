using System;
using System.Reflection;
using Plex.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WatercolorGames.CommandLine;
using DocoptNet;
using Newtonsoft.Json;

namespace Peacenet.Backend
{
    public class TerminalManager : IBackendComponent
    {
        private List<ITerminalCommand> _commands = null;
        Dictionary<string, string> _usages = new Dictionary<string, string>();

        public IEnumerable<KeyValuePair<string, string>> GetCommandList()
        {
            foreach (var cmd in _commands)
            {
                yield return new KeyValuePair<string, string>(cmd.Name, cmd.Description);
            }
        }

        public void Initiate()
        {
            Logger.Log("Initiating Terminal module...");
            _commands = new List<ITerminalCommand>();
            Logger.Log("Searching for commands...");
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(ITerminalCommand)))
                {
                    var command = (ITerminalCommand)Activator.CreateInstance(type, null);
                    if (_commands.FirstOrDefault(x => x.Name == command.Name) != null)
                    {
                        Logger.Log($"Duplicate: {command.Name} ({type.Name}). Skipping.");
                        continue;
                    }
                    Logger.Log($"Found: {command.Name} ({type.Name})");
                    _commands.Add(command);
                }
            }
            Logger.Log($"Done. {_commands.Count} commands found.");
            Logger.Log("Building manpages...");
            _usages = new Dictionary<string, string>();
            foreach (var cmd in _commands)
            {
                string name = cmd.Name;
                string desc = cmd.Description;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Command:");
                sb.AppendLine($"  {name}");
                sb.AppendLine("");
                sb.AppendLine("Summary:");
                sb.AppendLine($"  {desc}");
                sb.AppendLine();
                sb.AppendLine("Usage:");
                var usages = cmd.UsageStrings.ToArray();
                if (usages.Length == 0)
                {
                    sb.AppendLine($"  {cmd.Name}");
                }
                else
                {
                    foreach (var ustr in usages)
                    {
                        sb.AppendLine($"  {cmd.Name} {ustr}");
                    }
                }
                _usages.Add(name, sb.ToString());
                Logger.Log(_usages[name]);
            }
            Logger.Log("Manpages built.");
        }

        public string GetManpage(string cmdname)
        {
            if (_usages.ContainsKey(cmdname))
                return _usages[cmdname];
            return null;
        }

        public bool RunCommand(Backend backend, string commandname, string[] args, string session, StreamReader stdin, StreamWriter stdout)
        {
            if (backend == null)
                throw new ArgumentNullException(nameof(backend));
            if (string.IsNullOrWhiteSpace(commandname))
                return false;
            if (args == null)
                args = new string[] { };
            var cmd = _commands.FirstOrDefault(x => x.Name == commandname);
            if (cmd == null)
                return false;
            string man = GetManpage(commandname);
            var ctx = new ConsoleContext(stdout, stdin);
            //Validate and run the command.
            try
            {
                Dictionary<string, object> argss = new Dictionary<string, object>();
                var docopt = new Docopt();
                var argsv = docopt.Apply(man, args, version: commandname, exit: false);
                foreach (var arg in argsv)
                {
                    if (arg.Value != null)
                        argss.Add(arg.Key, arg.Value.Value);
                    else
                        argss.Add(arg.Key, null);
                }
                cmd.Run(backend, ctx, session, argss);
            }
            catch (Exception ex)
            {
                ctx.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Red);
                ctx.WriteLine($"{commandname}: Error");
                ctx.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                ctx.WriteLine(ex.Message);
            }
            return true;
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
            Logger.Log("Clearing commands...");
            _commands.Clear();
            Logger.Log("Done.");
        }
    }

    public class TerminalHelpRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.TRM_GETCMDS;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var trmmgr = backend.GetBackendComponent<TerminalManager>();
            var cmds = trmmgr.GetCommandList().ToArray();
            datawriter.Write(cmds.Length);
            foreach (var cmd in cmds)
            {
                datawriter.Write(cmd.Key);
                datawriter.Write(cmd.Value);
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    public interface ITerminalCommand
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> UsageStrings { get; }
        void Run(Backend backend, ConsoleContext console,  string sessionid, Dictionary<string, object> args);
    }

    public class TestCommand : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "This is a test server-side command.";
            }
        }

        public string Name
        {
            get
            {
                return "hello";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                yield return "[--echo <echotext>]";
            }
        }

        public void Run(Backend backend, ConsoleContext console, string sessionid, Dictionary<string, object> args)
        {
            if ((bool)args["--echo"] == true)
            {
                console.WriteLine(args["<echotext>"].ToString());
            }
            else
            {
                console.WriteLine("Hello world!");
            }
        }
    }

    public class ManpageRetriever : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.TRM_MANPAGE;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var termmgr = backend.GetBackendComponent<TerminalManager>();
            string cmd = datareader.ReadString();
            string manpage = termmgr.GetManpage(cmd);
            if (manpage != null)
            {
                datawriter.Write(manpage);
            }
            else
            {
                datawriter.Write("No manpage found for this command.");
            }
            return ServerResponseType.REQ_SUCCESS;
        }

        public class CommandRunner : IMessageHandler
        {
            public ServerMessageType HandledMessageType
            {
                get
                {
                    return ServerMessageType.TRM_INVOKE;
                }
            }

            public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
            {
                //TODO: Persistent connection to client so that stdin reads from client and stdout sends to client.
                //This will allow server-side commands to ask the in-game user for input while the command is running.
                //For now, we'll pipe stdout and stdin to a MemoryStream.
                MemoryStream std = new MemoryStream();
                var stdout = new StreamWriter(std);
                stdout.AutoFlush = true;
                var stdin = new StreamReader(std);

                string datajson = datareader.ReadString();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(datajson);
                string sessionfwd = (string.IsNullOrWhiteSpace(data["sessionfwd"] as string)) ? session : data["sessionfwd"].ToString();
                string cmdname = data["cmd"].ToString();
                var args = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(data["args"]));
                var trmmgr = backend.GetBackendComponent<TerminalManager>();
                bool result = trmmgr.RunCommand(backend, cmdname, args, session, stdin, stdout);
                if (result)
                {
                    datawriter.Write(Encoding.UTF8.GetString(std.ToArray()));
                }
                else
                {
                    datawriter.Write("Command not found.");
                }
                stdout.Close();
                stdin.Close();
                std.Close();
                datawriter.Write("\u0013\u0014");
                return ServerResponseType.REQ_SUCCESS;

            }
        }
    }
}
