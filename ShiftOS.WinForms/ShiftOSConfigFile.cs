using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.WinForms
{
    public class ShiftOSConfigFile
    {
        public ShiftOSConfigFile()
        {
            AudioVolume = 0.5F;
            Desktops = new List<string>();
        }

        public float AudioVolume { get; set; }
        public List<string> Desktops { get; set; }
    }
}
