using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plex.Objects
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UsageStringAttribute : Attribute
    {
        public UsageStringAttribute(string usage)
        {
            Usage = usage;
        }

        public string Usage { get; private set; }
    }

}
