using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerCommand : Command
    {
        public ServerCommand(string name, string desc) : base(name, "", desc)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresArgument : Attribute
    {
        public RequiresArgument(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

}
