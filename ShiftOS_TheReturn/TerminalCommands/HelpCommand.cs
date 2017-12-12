using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine.Server;
using System.IO;

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
            foreach(var command in _terminal.GetCommandList().OrderBy(x=>x.Name))
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
        
        [Dependency]
        private AsyncServerManager _server = null;

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
            if(commandDescriptor == null || commandDescriptor?.ManPage == null)
            {
                if (_server.Connected)
                {
                    using(var memstr = new MemoryStream())
                    {
                        using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                        {
                            writer.Write(command);
                            _server.SendMessage(ServerMessageType.TRM_MANPAGE, memstr.ToArray(), (res, reader) =>
                            {
                                if(res == ServerResponseType.REQ_SUCCESS)
                                {
                                    console.WriteLine(reader.ReadString());
                                }
                            }).Wait();
                        }
                    }
                }
                else
                {
                    console.WriteLine("No manpage found for this command.");
                }
                return;
            }
            console.WriteLine(commandDescriptor.ManPage);
        }
    }
}
