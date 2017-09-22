using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerCommand : Attribute
    {
        public ServerCommand(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequiresArgument : Attribute
    {
        public RequiresArgument(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

}
