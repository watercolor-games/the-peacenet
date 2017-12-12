using Plex.Engine.Interfaces;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using System.IO;
using Plex.Engine.Server;
using Newtonsoft.Json;

namespace Plex.Engine
{
    public class TerminalManager : IEngineComponent
    {
        private List<ITerminalCommand> _localCommands = null;
        private Dictionary<string, string> _usages = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private AsyncServerManager _server = null;

        public void Initiate()
        {
            _localCommands = new List<ITerminalCommand>();
            _usages = new Dictionary<string, string>();
            Logger.Log("Looking for terminal commands...", LogType.Info, "terminal");
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(ITerminalCommand))))
            {
                var command = (ITerminalCommand)Activator.CreateInstance(type, null);
                Logger.Log($"Found: {command.Name} (from {type.FullName})", LogType.Info, "terminal");
                //Avoid commands with the same name!
                if(_localCommands.FirstOrDefault(x=>x.Name == command.Name) != null)
                {
                    Logger.Log($"COMMAND CONFLICT: Two commands with the same name: {command.Name} (from {type.FullName}) and {_localCommands.FirstOrDefault(y => y.Name == command.Name).Name} (from {_localCommands.FirstOrDefault(y => y.Name == command.Name).GetType().FullName}). Skipping.", LogType.Error, "terminal");
                    continue;
                }

                _localCommands.Add(command);
                Logger.Log("Injecting dependencies for the command...");
                _plexgate.Inject(command);
                Logger.Log("Done.");
                Logger.Log("Creating usage string...");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{command.Name}");
                sb.AppendLine();
                sb.AppendLine("Summary:");
                sb.AppendLine($"  {command.Description}");
                sb.AppendLine();
                sb.AppendLine("Usage:");
                //This is the tough part.
                if(command.Usages == null || command.Usages.Count() == 0)
                {
                    //No arguments for the command, just add the command name as a usage string so Docopt doesn't get confused.
                    sb.AppendLine($"  {command.Name}");
                }
                else
                {
                    foreach(var usage in command.Usages)
                    {
                        sb.AppendLine($"  {command.Name} {usage}");
                    }
                }
                //Add usage string to the database.
                _usages.Add(command.Name, sb.ToString());
                Logger.Log("Done.");
            }
            Logger.Log("Successfully loaded all Terminal commands.", LogType.Info, "terminal");
        }

        /// <summary>
        /// Create a console context object from a standard output and standard input stream.
        /// </summary>
        /// <param name="stdout">The writer for the standard output stream.</param>
        /// <param name="stdin">The reader for the standard input stream.</param>
        /// <returns>A console context wrapping the specified streams.</returns>
        public ConsoleContext CreateContext(StreamWriter stdout, StreamReader stdin)
        {
            return new ConsoleContext(stdout, stdin);
        }

        public IEnumerable<CommandDescriptor> GetCommandList()
        {
            CommandDescriptor[] cmds = null;
            if (_server.Connected)
            {
                _server.SendMessage(ServerMessageType.TRM_GETCMDS, null, (res, reader) =>
                {
                    int len = reader.ReadInt32();

                    cmds = new CommandDescriptor[len];
                    for(int i = 0; i < len; i++)
                    {
                        cmds[i] = new CommandDescriptor(reader.ReadString(), reader.ReadString(), null);
                    }
                }).Wait();
                foreach (var cmd in cmds)
                    yield return cmd;
            }
            foreach (var cmd in _localCommands)
            {
                yield return new CommandDescriptor(cmd.Name, cmd.Description, _usages[cmd.Name]);
            }
        }

        public bool RunCommand(string command, ConsoleContext console)
        {
            //First we need to parse the command string into something we can actually use as a query.
            var query = ParseCommand(command);
            //If query is null, don't try to run a command. There was no command.
            if (query == null)
                return false;
            //If we get to this stage, the query is OK. Let's find a local command.
            var local = _localCommands.FirstOrDefault(x => x.Name == query.Name);
            //If this is null we'll send it off to the server. Well, I haven't implemented the server yet so we'll just return false.
            if(local == null)
            {
                if (!_server.Connected)
                    return false;
                using(var memstr = new MemoryStream())
                {
                    using(var writer = new BinaryWriter(memstr, Encoding.UTF8))
                    {
                        writer.Write(JsonConvert.SerializeObject(new
                        {
                            cmd = query.Name,
                            args = query.ArgumentTokens,
                            sessionfwd = ""
                        }));
                        writer.Flush();
                        _server.SendMessage(ServerMessageType.TRM_INVOKE, memstr.ToArray(), (res, reader) =>
                        {
                            string output;
                            while((output = reader.ReadString()) != "\u0013\u0014")
                            {
                                console.Write(output);
                            }
                            console.WriteLine("");
                        }).Wait();
                    }
                }
                return true;
            }
            else
            {
                //So we have a command query, a console context, and a command.
                //What do we need next? Well, we need our argument dictionary.
                //We can use Docopt.Net and the ITerminalCommand.Usages property to get that.
                //First let's create a Dictionary<string, object> to store our arguments.
                var argumentStore = new Dictionary<string, object>();

                //Next, we retrieve the usage string from the command.
                //The engine creates usage strings during initialization.
                var usage = _usages[local.Name];

                //Then we pass it off to Docopt.
                var docoptArgs = new DocoptNet.Docopt().Apply(usage, query.ArgumentTokens, true, local.Name, false, false);
                //Now we just CTRL+C and CTRL+V from the debug console.
                foreach (var arg in docoptArgs)
                {
                    if (arg.Value != null)
                        argumentStore.Add(arg.Key, arg.Value.Value);
                    else
                        argumentStore.Add(arg.Key, null);
                }
                //Now we run the command - check for errors!
                try
                {
                    local.Run(console, argumentStore);
                }
                catch (Exception ex)
                {
                    console.WriteLine($"Command error: {ex.Message}");
                }
                return true;
            }
        }

        public CommandQuery ParseCommand(string command)
        {
            //We can use the Watercolor tokenizer just like we do for the server console and the Debug console.
            //That'll tokenize the entire command string.
            var tokens = WatercolorGames.CommandLine.Tokenizer.TokenizeString(command);
            if(tokens.Length == 0)
            {
                //If the length of the token array is 0, the player didn't supply an actual command. This should be handled by the terminal frontend itself...
                //but because I'm nice, I'll add a failsafe.
                return null;
            }
            //Now we create an argument array which has one element less than the Watercolor tokenizer's result.
            var arguments = new string[tokens.Length - 1];
            //And we iterate through with a for() loop...
            for(int i = 0; i < arguments.Length; i++)
            {
                //The general rule is we don't want the command name inside the argument tokens. So, this gets rid of it in our target array.
                arguments[i] = tokens[i + 1]; //Bam.
            }
            //There's probably a better way to do that...
            //Anyway now we can return the command query.
            return new CommandQuery(tokens[0], arguments); //First element of tokens is always the command name
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            Logger.Log("Terminal is shutting down...");
            while (_localCommands.Count > 0)
            {
                //add any disposing code here if needed.

                //remove from list.
                _localCommands.RemoveAt(0);
                
            }
            Logger.Log("Done.");
        }
    }

    public interface ITerminalCommand
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> Usages { get; }

        void Run(ConsoleContext console, Dictionary<string, object> arguments); 
    }

    public class CommandDescriptor
    {
        public CommandDescriptor(string name, string desc, string man)
        {
            Name = name;
            Description = desc;
            ManPage = man;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ManPage { get; private set; }
    }


    public class ClearCommand : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Clears the screen";
            }
        }

        public string Name
        {
            get
            {
                return "clear";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.Clear();
        }
    }
}
