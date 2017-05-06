using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.WinForms.Tools;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.DesktopWidgets
{
    [DesktopWidget("Server ping", "See the time spent between client requests and server replies in the digital society.")]
    public partial class HeartbeatWidget : UserControl, IDesktopWidget
    {
        public HeartbeatWidget()
        {
            InitializeComponent();
            tmr.Interval = 1;
            tmr.Tick += (o, a) =>
            {
                if(ts != ServerManager.DigitalSocietyPing)
                {
                    ts = ServerManager.DigitalSocietyPing;
                    lbheartbeat.Text = "Server ping: " + ts.ToString() + " MS";
                }
            };
        }

        //Fun fact. I misspelled this as "TimeSpam."
        long ts = 0;

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {
        }

        Timer tmr = new Timer();

        public void Setup()
        {
            tmr.Start();
        }
    }
}
