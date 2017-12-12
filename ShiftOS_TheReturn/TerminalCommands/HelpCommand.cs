using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Engine.TerminalCommands
{
    public class HelpCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        public string Description
        {
            get
            {
                return "Retrieves a list of commands.";
            }
        }

        public string Name
        {
            get
            {
                return "help";
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
            console.WriteLine("Command help");
            console.WriteLine("------------------");
            console.WriteLine("");
            foreach(var command in _terminal.GetCommandList())
            {
                console.Write($" - {command.Name}");
                if(!string.IsNullOrWhiteSpace(command.Description))
                {
                    console.WriteLine(": " + command.Description);
                }
                else
                {
                    console.WriteLine("");
                }
            }
        }
    }

    public class ManCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        public string Description
        {
            get
            {
                return "View a command's manual page."; //I tried to copy that of the Linux manpage for 'man'. I don't feel like booting a vm or using SSH so...no.
            }
        }

        public string Name
        {
            get
            {
                return "man";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<command>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string command = arguments["<command>"].ToString();
            var commandDescriptor = _terminal.GetCommandList().FirstOrDefault(x => x.Name == command);
            if(commandDescriptor == null)
            {
                console.WriteLine("Manpage not found.");
                return;
            }
            console.WriteLine(commandDescriptor.ManPage);
        }
    }
}
