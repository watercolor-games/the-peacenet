using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using attribute = System.Attribute;

namespace ShiftOS.Objects
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MudRequestAttribute : attribute
    {
        /// <summary>
        /// This attribute can be used on a static method to make the multi-user domain server software see this method as a MUD request handler.
        /// </summary>
        /// <param name="rName">The header ID of the request this method should handle.</param>
        public MudRequestAttribute(string rName)
        {
            RequestName = rName;
        }

        public string RequestName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MudResponseAttribute : attribute
    {
        /// <summary>
        /// Clients will look for static methods marked with this attribute and run them first. If no attribute is found with the given header ID, the client may invoke a delegate with the message information.
        /// </summary>
        /// <param name="rName">The header ID of the response that this method will handle.</param>
        public MudResponseAttribute(string rName)
        {
            ResponseName = rName;
        }

        public string ResponseName { get; private set; }
    }


}

