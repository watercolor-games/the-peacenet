using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    public class date : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Print the current date to the console.";
            }
        }

        public string Name
        {
            get
            {
                return "date";
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
            console.WriteLine(DateTime.Now.ToLongDateString());
        }
    }

    public class time : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Print the current time to the console.";
            }
        }

        public string Name
        {
            get
            {
                return "time";
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
            console.WriteLine(DateTime.Now.ToLongTimeString());
        }
    }
}
