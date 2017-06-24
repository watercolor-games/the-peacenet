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

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/sol/subscription", "ShiftOS Online", "SOL is the Shiftnet.")]
    [ShiftnetFundamental]
    public partial class ShiftOSOnline : UserControl, IShiftnetSite
    {
        public ShiftOSOnline()
        {
            InitializeComponent();
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            Tools.ControlManager.SetupControls(this);
            lbtitle.CenterParent();
            lbtitle.Top = 15;
            label1.CenterParent();
            btnsubscribe.CenterParent();
            btnsubscribe.Top = (label1.Top + label1.Height) + 15;
        }

        public void OnUpgrade()
        {
            
        }

        public string SOL_YOUARESUBSCRIBED
        {
            get
            {
                return Localization.Parse("You're already subscribed! Unsubscribe here.");
            }
        }

        public string SOL_SUBSCRIBE
        {
            get
            {
                return Localization.Parse("Subscribe today!");
            }
        }

        public void Setup()
        {
            if(SaveSystem.CurrentSave.ShiftnetSubscription == 3)
            {
                btnsubscribe.Text = SOL_YOUARESUBSCRIBED;
            }
            else
            {
                btnsubscribe.Text = SOL_SUBSCRIBE;
            }
        }

        private void btnsubscribe_Click(object sender, EventArgs e)
        {
            if(btnsubscribe.Text == SOL_YOUARESUBSCRIBED)
            {
                Infobox.PromptYesNo("Unsubscribe", "Are you sure you want to unsubscribe from ShiftOS Online?", (result) =>
                {
                    if (result == true)
                    {
                        SaveSystem.CurrentSave.ShiftnetSubscription = 0;
                        Setup();
                        OnSkinLoad();
                    }
                });
            }
            else
            {
                Infobox.PromptYesNo("Subscribe?", "Would you like to subscribe to ShiftOS Online to get 768 kb/s for 300 Codepoints?", (result) =>
                {
                    if(result == true)
                    {
                        if(SaveSystem.CurrentSave.Codepoints >= 300)
                        {
                            SaveSystem.CurrentSave.Codepoints -= 300;
                            SaveSystem.CurrentSave.ShiftnetSubscription = 3;
                            Infobox.Show("Subscribed.", "You have sent 300 Codepoints to ShiftOS Online and have successfully subscribed to their Shiftnet Service.");
                        }
                        else
                        {
                            Infobox.Show("Insufficient Codepoints", "You do not have enough Codepoints to complete this operation.");
                        }
                    }
                });
            }
        }
    }
}
