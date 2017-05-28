using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects.Unite
{
    public class ReleaseQuery
    {
        public bool ShowUnstable { get; set; }
        public bool ShowObsolete { get; set; }
        public DateTime CurrentBuildDate { get; set; }
    }
}
