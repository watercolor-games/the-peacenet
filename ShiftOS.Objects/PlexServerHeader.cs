using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class PlexServerHeader
    {
        public string Message { get; set; }
        public string Content { get; set; }
        public string IPForwardedBy { get; set; }
        public string PlexSysname { get; set; }
        public string PlexUser { get; set; }
    }
}
