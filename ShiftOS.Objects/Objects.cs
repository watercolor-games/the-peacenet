using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    [Obsolete("This feature will be removed in Milestone 2.")]
    public class GUIDRequest
    {
        public string name { get; set; }
        public string guid { get; set; }
    }
}
