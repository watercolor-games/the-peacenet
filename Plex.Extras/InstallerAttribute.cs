using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;

namespace Plex.Extras
{
    public class InstallerAttribute : ShiftoriumUpgradeAttribute
    {
        public InstallerAttribute(string name, string description, long bytescount, string dependencies = "") : base(name, 0, description, dependencies, "Installer files", false)
        {
            Bytes = bytescount;
        }

        public long Bytes { get; private set; }
    }
}
