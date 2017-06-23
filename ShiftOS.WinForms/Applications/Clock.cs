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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [RequiresUpgrade("clock")]
    [WinOpen("clock")]
    [Launcher("Clock", false, null, "Accessories")]
    [DefaultTitle("Clock")]
    public partial class Clock : UserControl, IShiftOSWindow
    {
        public Clock()
        {
            InitializeComponent();
            clocktimer = new Timer();
            clocktimer.Interval = 100;
            clocktimer.Tick += (o, a) =>
            {
                lbheader.CenterParent();
                lbheader.Top = 15;
                lbcurrenttime.Text = Terminal.GetTime();
                lbcurrenttime.CenterParent();
            };
        }

        private Timer clocktimer = null;

        public void OnLoad()
        {
            clocktimer.Start();
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
