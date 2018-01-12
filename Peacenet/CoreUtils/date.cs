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

    /// <summary>
    /// Contains extension methods for converting DateTime objects to other time formats
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the <see cref="DateTime"/> object to a <see cref="Int64"/> representing the equivalent UNIX epoch timestamp.  
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> object to convert</param>
        /// <returns>The Epoch time represented by the specified DateTime object.</returns>
        public static long Epoch(this DateTime datetime)
        {
            var epoch = (long)(datetime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return epoch;
        }
    }
}
