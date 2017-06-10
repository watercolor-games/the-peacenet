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
    [ShiftnetSite("shiftnet/shiftsoft", "ShiftSoft", "What do you want to shift today?")]
    [ShiftnetFundamental]
    public partial class ShiftSoft : UserControl, IShiftnetSite
    {
        public ShiftSoft()
        {
            InitializeComponent();
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            pnldivider.Tag = "keepbg";
            pnldivider.BackColor = SkinEngine.LoadedSkin.ControlTextColor;
            Tools.ControlManager.SetupControls(flbuttons);
            Tools.ControlManager.SetupControls(pnlhome);
            Tools.ControlManager.SetupControls(pnlservices);

            lbfreebiedesc.Top = lbfreebie.Top + lbfreebie.Height + 5;
            btnjoinfreebie.Top = lbfreebiedesc.Top + lbfreebiedesc.Height + 5;

            SetupFreebieUI();

            lbdesc.Top = lbwhere.Top + lbwhere.Height + 5;
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {
            pnlhome.BringToFront();
        }

        private void btnping_Click(object sender, EventArgs e)
        {
            GoToUrl?.Invoke("shiftnet/shiftsoft/ping");
        }

        private void btnhome_Click(object sender, EventArgs e)
        {
            pnlhome.BringToFront();
        }

        private void btnservices_Click(object sender, EventArgs e)
        {
            pnlservices.BringToFront();
            SetupFreebieUI();
        }

        public void SetupFreebieUI()
        {
            if(SaveSystem.CurrentSave.ShiftnetSubscription == 0)
            {
                btnjoinfreebie.Enabled = false;
                btnjoinfreebie.Text = "You are already subscribed to Freebie Solutions.";
            }
            else
            {
                btnjoinfreebie.Enabled = true;
                btnjoinfreebie.Text = "Join Freebie Solutions";
            }
            btnjoinfreebie.Left = (lbfreebiedesc.Left + lbfreebiedesc.Width) - btnjoinfreebie.Width; 
        }

        private void btnjoinfreebie_Click(object sender, EventArgs e)
        {
            Infobox.PromptYesNo("Switch providers", "Would you like to switch from your current Shiftnet provider, " + Applications.DownloadManager.GetAllSubscriptions()[SaveSystem.CurrentSave.ShiftnetSubscription].Name + ", to Freebie Solutions by ShiftSoft?", (res) =>
            {
                if(res == true)
                {
                    SaveSystem.CurrentSave.ShiftnetSubscription = 0;
                    Infobox.Show("Switch providers", "The operation has completed successfully.");
                }
            });
        }
    }
}
