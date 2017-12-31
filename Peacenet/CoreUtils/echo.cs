using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    public class echo : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Write a line of text to the screen.";
            }
        }

        public string Name
        {
            get
            {
                return "echo";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<text>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.Write(arguments["<text>"].ToString());
        }
    }
}
