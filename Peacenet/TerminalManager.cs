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
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Objects.Pty;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Peacenet
{
    /// <summary>
    /// Provides an engine component facilitating the execution of Peacenet terminal commands.
    /// </summary>
    public class TerminalManager : IEngineComponent, IDisposable
    {


        private List<ITerminalCommand> _localCommands = null;
        private Dictionary<string, string> _usages = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            _localCommands = new List<ITerminalCommand>();
            _usages = new Dictionary<string, string>();
            Logger.Log("Looking for terminal commands...");
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(ITerminalCommand))))
            {
                if (type.GetCustomAttributes(true).Any(x => x is TerminalSkipAutoloadAttribute))
                    continue;
                var command = (ITerminalCommand)_GameLoop.New(type);
                Logger.Log($"Found: {command.Name} (from {type.FullName})");
                //Avoid commands with the same name!
                if(_localCommands.FirstOrDefault(x=>x.Name == command.Name) != null)
                {
                    Logger.Log($"COMMAND CONFLICT: Two commands with the same name: {command.Name} (from {type.FullName}) and {_localCommands.FirstOrDefault(y => y.Name == command.Name).Name} (from {_localCommands.FirstOrDefault(y => y.Name == command.Name).GetType().FullName}). Skipping.", System.ConsoleColor.DarkYellow);
                    continue;
                }

                _localCommands.Add(command);
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
            Logger.Log("Successfully loaded all Terminal commands.");
        }

        internal void LoadCommand(ITerminalCommand command)
        {
            if (command == null)
                return;
            Logger.Log($"Found: {command.Name} (from {command.GetType().FullName})");
            //Avoid commands with the same name!
            if (_localCommands.FirstOrDefault(x => x.Name == command.Name) != null)
            {
                Logger.Log($"COMMAND CONFLICT: Two commands with the same name: {command.Name} (from {command.GetType().FullName}) and {_localCommands.FirstOrDefault(y => y.Name == command.Name).Name} (from {_localCommands.FirstOrDefault(y => y.Name == command.Name).GetType().FullName}). Skipping.", System.ConsoleColor.DarkYellow);
                throw new ArgumentException("A command with the same name already exists in the database.");

            }

            _localCommands.Add(command);
            Logger.Log("Injecting dependencies for the command...");
            _GameLoop.Inject(command);
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
            if (command.Usages == null || command.Usages.Count() == 0)
            {
                //No arguments for the command, just add the command name as a usage string so Docopt doesn't get confused.
                sb.AppendLine($"  {command.Name}");
            }
            else
            {
                foreach (var usage in command.Usages)
                {
                    sb.AppendLine($"  {command.Name} {usage}");
                }
            }
            //Add usage string to the database.
            _usages.Add(command.Name, sb.ToString());
            Logger.Log("Done.");
        }

        public void UnloadCommand(string name)
        {
            var command = _localCommands.FirstOrDefault(x => x.Name == name);
            if (command == null)
                throw new ArgumentException("A command with that name has not been found.");
            _usages.Remove(name);
            _localCommands.Remove(command);
            Logger.Log("Unloaded command: " + name);
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

        /// <summary>
        /// Retrieves a list of available commands.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{CommandDescriptor}"/> containing a list of command manual entries.</returns>
        public IEnumerable<CommandDescriptor> GetCommandList()
        {
            foreach (var cmd in _localCommands)
            {
                if (cmd.GetType().GetCustomAttributes(true).Any(x => x is HideInHelpAttribute))
                    continue;
                yield return new CommandDescriptor(cmd.Name, cmd.Description, _usages[cmd.Name]);
            }
        }

        public event Action<string> CommandSucceeded;

        /// <summary>
        /// Runs a command with the given console context.
        /// </summary>
        /// <param name="command">The command (including raw arguments) to parse and run.</param>
        /// <param name="console">The <see cref="ConsoleContext"/> to run the command in.</param>
        /// <returns>Whether the command was actually found.</returns>
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
                return false;
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
                    CommandSucceeded?.Invoke(query.Name);
                }
                catch (TerminationRequestException)
                {
                    console.WriteLine("Killed.");
                    return true;
                }
                return true;
            }
        }

        /// <summary>
        /// Parse a command string into a command name and list of arguments.
        /// </summary>
        /// <param name="command">The raw command string to parse.</param>
        /// <returns>The parsed <see cref="CommandQuery"/>. Returns null if a null or empty command string was provided.</returns>
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

        /// <inheritdoc/>
        public void Dispose()
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

    /// <summary>
    /// Provides an API for creating in-game Terminal commands.
    /// </summary>
    public interface ITerminalCommand
    {
        /// <summary>
        /// Retrieves the name of the command. Implementing classes should note that command names may not contain spaces, and are case-sensitive. "Cat" is not the same as "cat". You also cannot have two <see cref="ITerminalCommand"/>s with the same name. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Retrieves the description of the command. This is used to construct a command manual page.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Retrieves the syntaxes of the command. Return null if your command doesn't have a syntax. Each element in the <see cref="IEnumerable{String}"/> represents a new line in the compiled usage string. See <see cref="DocoptNet.Docopt"/> and http://docopt.org/ for information on how to write your command-ilne syntax.   
        /// </summary>
        IEnumerable<string> Usages { get; }

        /// <summary>
        /// The entry point of the command.
        /// </summary>
        /// <param name="console">A <see cref="ConsoleContext"/> providing methods for receiving user input and writing command output to and from the console.</param>
        /// <param name="arguments">A <see cref="Dictionary{String, Object}"/> containing the command-line arguments provided by the player.</param>
        void Run(ConsoleContext console, Dictionary<string, object> arguments); 
    }

    /// <summary>
    /// Contains the data necessary to construct a manual entry for an <see cref="ITerminalCommand"/>. 
    /// </summary>
    public class CommandDescriptor
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandDescriptor"/> class. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="desc">The description of the command.</param>
        /// <param name="man">The command's manual page.</param>
        public CommandDescriptor(string name, string desc, string man)
        {
            Name = name;
            Description = desc;
            ManPage = man;
        }

        /// <summary>
        /// Retrieves the name of the command.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Retrieves the command's description.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Retrieves the full compiled manual page for the command.
        /// </summary>
        public string ManPage { get; private set; }
    }

    /// <summary>
    /// A command which clears the console.
    /// </summary>
    public class ClearCommand : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Clears the screen";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "clear";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.Clear();
        }
    }

    /// <summary>
    /// Indicates that this <see cref="ITerminalCommand"/> should be skipped by the <see cref="TerminalManager"/>'s autoload routine, making it unavailable to a player unless explicitly instantiated and run by another piece of code. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TerminalSkipAutoloadAttribute : Attribute
    {

    }

    class RemotePtyEntity : IEntity
    {
        private ConsoleContext _localContext = null;
        private Stream _remoteMaster = null;
        private Stream _remoteSlave = null;

        public RemotePtyEntity(ConsoleContext localContext, Stream remoteMaster, Stream remoteSlave)
        {
            _localContext = localContext;
            _remoteMaster = remoteMaster;
            _remoteSlave = remoteSlave;
        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        public void OnGameExit()
        {
            _remoteMaster.Close();
            _remoteSlave.Close();
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void Update(GameTime time)
        {
        }
    }
}
