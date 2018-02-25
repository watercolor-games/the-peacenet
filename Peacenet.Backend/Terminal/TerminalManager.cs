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
    /// <summary>
    /// Provides terminal command support in the Peacenet server.
    /// </summary>
    public class TerminalManager : IBackendComponent
    {
        private List<ITerminalCommand> _commands = null;
        Dictionary<string, string> _usages = new Dictionary<string, string>();

        /// <summary>
        /// Get a list of all commands.
        /// </summary>
        /// <returns>A list containing all commands plus their descriptions.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetCommandList()
        {
            foreach (var cmd in _commands)
            {
                yield return new KeyValuePair<string, string>(cmd.Name, cmd.Description);
            }
        }

        [Dependency]
        private Backend _backend = null;

        /// <inheritdoc/>
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
                    _backend.Inject(command);
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
                var usages = (cmd.UsageStrings != null) ? cmd.UsageStrings.ToArray() : new string[0];
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

        /// <summary>
        /// Retrieve the manual page of a command.
        /// </summary>
        /// <param name="cmdname">The name of the command</param>
        /// <returns>The command's manual page</returns>
        public string GetManpage(string cmdname)
        {
            if (_usages.ContainsKey(cmdname))
                return _usages[cmdname];
            return null;
        }

        /// <summary>
        /// Run a command.
        /// </summary>
        /// <param name="backend">The server backend environment</param>
        /// <param name="commandname">The command name</param>
        /// <param name="args">The tokenized arguments for the command</param>
        /// <param name="session">The user ID of the caller</param>
        /// <param name="stdin">The standard input stream</param>
        /// <param name="stdout">The standard output stream</param>
        /// <returns>Whether the command was found</returns>
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
                ctx.WriteLine(ex.StackTrace);
                ctx.WriteLine(ex.Message);
            }
            return true;
        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
        }

        /// <inheritdoc/>
        public void Unload()
        {
            Logger.Log("Clearing commands...");
            _commands.Clear();
            Logger.Log("Done.");
        }
    }

    /// <summary>
    /// Handler for retrieving terminal command lists
    /// </summary>
    public class TerminalHelpRetriever : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.TRM_GETCMDS;
            }
        }

        /// <inheritdoc/>
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

    /// <summary>
    /// Provides an API for creating server-side terminal commands.
    /// </summary>
    public interface ITerminalCommand
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The description of the command
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The syntax of the command
        /// </summary>
        IEnumerable<string> UsageStrings { get; }
        /// <summary>
        /// The entry of the command.
        /// </summary>
        /// <param name="backend">The server backend environment.</param>
        /// <param name="console">The console context for the command</param>
        /// <param name="sessionid">The user ID of the caller.</param>
        /// <param name="args">The arguments for the command.</param>
        void Run(Backend backend, ConsoleContext console,  string sessionid, Dictionary<string, object> args);
    }

    /// <summary>
    /// Handler for retrieving manual pages.
    /// </summary>
    public class ManpageRetriever : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.TRM_MANPAGE;
            }
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Handler for running commands
        /// </summary>
        public class CommandRunner : IMessageHandler
        {
            /// <inheritdoc/>
            public ServerMessageType HandledMessageType
            {
                get
                {
                    return ServerMessageType.TRM_INVOKE;
                }
            }

            /// <inheritdoc/>
            public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
            {
                //TODO: Persistent connection to client so that stdin reads from client and stdout sends to client.
                //This will allow server-side commands to ask the in-game user for input while the command is running.
                //For now, we'll pipe stdout and stdin to a MemoryStream.
                MemoryStream std = new MemoryStream();
                var stdout = new StreamWriter(std);
                stdout.AutoFlush = true;
                var stdin = new StreamReader(std);

                string commandname = datareader.ReadString();
                int argCount = datareader.ReadInt32();
                string[] argv = new string[argCount];
                for(int i = 0; i < argCount;i++)
                {
                    argv[i] = datareader.ReadString();
                }

                var trmmgr = backend.GetBackendComponent<TerminalManager>();
                bool result = trmmgr.RunCommand(backend, commandname, argv, session, stdin, stdout);
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
