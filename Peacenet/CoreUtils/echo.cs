using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    /// <summary>
    /// A command which prints user-specified text to the console.
    /// </summary>
    public class echo : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Write a line of text to the screen.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "echo";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<text>";
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.WriteLine(arguments["<text>"].ToString());
        }
    }
}
