using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// Contains a command name and a list of tokenized arguments.
    /// </summary>
    public class CommandQuery
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandQuery"/> class. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="tokens">The tokenized arguments.</param>
        public CommandQuery(string name, string[] tokens)
        {
            Name = name;
            ArgumentTokens = tokens;
        }

        /// <summary>
        /// Retrieves the name of the command.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Retrieves the tokenized arguments for the command.
        /// </summary>
        public string[] ArgumentTokens { get; private set; }
    }
}
