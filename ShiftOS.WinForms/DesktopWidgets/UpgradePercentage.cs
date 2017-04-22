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

namespace ShiftOS.WinForms.DesktopWidgets
{
    [DesktopWidget("Shiftorium Status", "Show how much upgrades you have and how much are available.")]
    public partial class UpgradePercentage : UserControl, IDesktopWidget
    {
        public UpgradePercentage()
        {
            InitializeComponent();
        }

        public void OnSkinLoad()
        {
            ControlManager.SetupControl(lbstatus);
            pgupgrades.Refresh();
        }

        public void OnUpgrade()
        {
            
            pgupgrades.Maximum = Shiftorium.GetDefaults().Count;
            pgupgrades.Value = SaveSystem.CurrentSave.CountUpgrades();
            lbstatus.Text = $"You have unlocked {pgupgrades.Value} upgrades out of the {pgupgrades.Maximum} available.";
        }

        public void Setup()
        {
        }
    }
}
