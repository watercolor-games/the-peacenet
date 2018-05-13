using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;
using Plex.Engine;

namespace Peacenet.TerminalCommands
{
    /// <summary>
    /// Lists all available commands and their descriptions.
    /// </summary>
    public class HelpCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Retrieves a list of commands.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "help";
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
            console.SetBold(true);
            console.WriteLine("Command help");
            console.WriteLine("------------------");
            console.WriteLine("");
            console.SetBold(false);
            foreach(var command in _terminal.GetCommandList().OrderBy(x=>x.Name))
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                console.Write(" - ");
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Yellow);
                console.SetBold(true);
                console.Write(command.Name);
                console.SetBold(false);
                if(!string.IsNullOrWhiteSpace(command.Description))
                {
                    console.Write(": ");
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                    console.SetItalic(true);
                    console.WriteLine(command.Description);
                    console.SetItalic(false);
                }
                else
                {
                    console.WriteLine("");
                }
            }
        }
    }

    /// <summary>
    /// Prints the manual page for a command
    /// </summary>
    public class ManCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;
        
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "View a command's manual page."; //I tried to copy that of the Linux manpage for 'man'. I don't feel like booting a vm or using SSH so...no.
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "man";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<command>";
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string command = arguments["<command>"].ToString();
            var commandDescriptor = _terminal.GetCommandList().FirstOrDefault(x => x.Name == command);
            if(commandDescriptor == null || commandDescriptor?.ManPage == null)
            {
                console.WriteLine("No manpage found for this command.");
                return;
            }
            console.WriteLine(commandDescriptor.ManPage);
        }
    }
}
