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
    [DesktopWidget("Codepoints Display", "Show how many Codepoints you have.")]
    public partial class CodepointsWidget : UserControl, IDesktopWidget
    {
        public CodepointsWidget()
        {
            InitializeComponent();
            tmr.Tick += (o, a) =>
            {
                try
                {
                    lbcodepoints.Text = SaveSystem.CurrentSave.Codepoints.ToString() + " Codepoints";
                }
                catch { }
            };
            tmr.Interval = 350;
            this.VisibleChanged += (o, a) =>
            {
                if (this.Visible == false)
                    tmr.Stop();
            };
        }

        Timer tmr = new Timer();

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
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
