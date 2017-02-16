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
    [Launcher("Experience Shifter", false, "", "Customization")]
    [DefaultTitle("Experience Shifter")]
    [DefaultIcon("iconShifter")]
    public partial class ExperienceShifter : UserControl, IShiftOSWindow
    {
        public ExperienceShifter()
        {
            InitializeComponent();
        }

        private string currentUI = "desktop";

        public void SetupUI()
        {
            switch (currentUI)
            {
                case "desktop":
                    pnldesktop.BringToFront();
                    PopulateDesktops();
                    break;
                case "applauncher":
                    pnlapplauncher.BringToFront();
                    PopulateLaunchers();
                    break;
            }
        }

        public void PopulateDesktops()
        {
            lbdesktops.Items.Clear();
            lbdesktops.Items.Add("ShiftOS Desktop");
        }

        public void PopulateLaunchers()
        {
            lblaunchers.Items.Clear();
            lbdesktops.Items.Add("ShiftOS App Launcher");
        }

        public void OnLoad()
        {
            SetupUI();
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

        private void desktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentUI = "desktop";
            SetupUI();
        }

        private void appLauncherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentUI = "applauncher";
            SetupUI();
        }
    }
}
