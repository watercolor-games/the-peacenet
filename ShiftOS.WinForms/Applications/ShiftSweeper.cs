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

namespace ShiftOS.WinForms.Applications
{
    [Launcher("ShiftSweeper", false, null, "Games")]
    [RequiresUpgrade("shiftsweeper")]
    [WinOpen("shiftsweeper")]
    [DefaultIcon("iconShiftSweeper")]
    public partial class ShiftSweeper : UserControl, IShiftOSWindow
    {
        private bool gameplayed = false;

        public ShiftSweeper()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            
        }

        public void OnSkinLoad() { }

        public bool OnUnload() { return true; }

        public void OnUpgrade()
        {
            
        }
    }
}
