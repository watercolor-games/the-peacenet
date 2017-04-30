using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using static ShiftOS.Objects.ShiftFS.Utils;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("triwrite")]
    [AppscapeEntry("TriWrite", "Part of the trilogy of office applications for enhancement of your system. TriWrite is easliy the best text editor out there for ShiftOS.", 1024, 750, null, "Office")]
    [DefaultTitle("TriWrite")]
    [Launcher("TriWrite", false, null, "Office")]
    public partial class TriWrite : UserControl, IShiftOSWindow
    {

        public void OnLoad()
        {

        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

    }
}