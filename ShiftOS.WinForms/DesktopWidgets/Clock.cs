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

namespace ShiftOS.WinForms.DesktopWidgets
{
    [DesktopWidget("Clock", "Show a clock on the desktop.")]
    [RequiresUpgrade("desktop_clock_widget")]
    public partial class Clock : UserControl, IDesktopWidget
    {
        public Clock()
        {
            InitializeComponent();
            tmr = new Timer();
            tmr.Tick += (o, a) =>
            {
                lbtime.Text = Applications.Terminal.GetTime();
            };
            tmr.Interval = 100;
        }

        Timer tmr = new Timer();

        public void OnSkinLoad()
        {
            Tools.ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {
            tmr.Start();
        }
    }
}
