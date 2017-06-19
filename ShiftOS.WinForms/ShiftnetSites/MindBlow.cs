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

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/mindblow", "MindBlow", "World's First IDE for ShiftOS")]
    public partial class MindBlow : UserControl, IShiftnetSite
    {
        public MindBlow()
        {
            InitializeComponent();
        }

        public event Action<string> GoToUrl;
        public event Action GoBack;

        private void DoLayout()
        {
            aboutpnl.Left = tutorialpnl.Left = nav.Width;
            aboutpnl.Width = tutorialpnl.Width = Width - nav.Width;
            tutorialpnl.Height = Height;
        }

        private void aboutbtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            aboutpnl.BringToFront();
        }

        private void tutorialbtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tutorialpnl.BringToFront();
        }

        private void MindBlow_Resize(object sender, EventArgs e)
        {
            DoLayout();
        }

        private void buybtn_Click(object sender, EventArgs e)
        {
            if (Shiftorium.UpgradeInstalled("mindblow"))
                Infobox.Show("Already Purchased", "You have already bought MindBlow.");
            else if (SaveSystem.CurrentSave.Codepoints < 50000)
                Infobox.Show("Not Enough Codepoints", "You do not have enough Codepoints to buy MindBlow.");
            else
            {
                Shiftorium.Buy("mindblow", 50000);
                Infobox.Show("Installation Complete", "MindBlow has been successfully installed.");
            }
        }

        public void Setup()
        {
            DoLayout();
            aboutpnl.BringToFront();
        }

        public void OnSkinLoad()
        {
            Tools.ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {
        }
    }
}
