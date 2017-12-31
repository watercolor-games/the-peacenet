using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    /// <summary>
    /// Specifies that this <see cref="ITerminalCommand"/> shouldn't ever be displayed in the command list presented by the "help" command. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HideInHelpAttribute : Attribute
    {
    }
}
