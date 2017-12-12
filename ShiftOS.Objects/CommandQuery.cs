using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class CommandQuery
    {
        public CommandQuery(string name, string[] tokens)
        {
            Name = name;
            ArgumentTokens = tokens;
        }

        public string Name { get; private set; }
        public string[] ArgumentTokens { get; private set; }
    }
}
