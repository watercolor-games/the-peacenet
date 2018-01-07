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
    /// A command which prints the current date to the console.
    /// </summary>
    public class date : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Print the current date to the console.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "date";
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
            console.WriteLine(DateTime.Now.ToLongDateString());
        }
    }

    /// <summary>
    /// Writes the current time to the console.
    /// </summary>
    public class time : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Print the current time to the console.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "time";
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
            console.WriteLine(DateTime.Now.ToLongTimeString());
        }
    }
}
