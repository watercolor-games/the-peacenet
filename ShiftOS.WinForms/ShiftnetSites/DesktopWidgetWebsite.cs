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
using ShiftOS.WinForms.Applications;
using Newtonsoft.Json;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/superdesk", "SuperDesk", "Empower the ShiftOS Desktop like never before.")]
    [ShiftnetFundamental]
    public partial class DesktopWidgetWebsite : UserControl, Engine.IShiftnetSite
    {
        public DesktopWidgetWebsite()
        {
            InitializeComponent();
            
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
            this.pictureBox1.Left = (this.Width - pictureBox1.Width) / 2;
            ControlManager.ControlSetup += (ctrl) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        lbwhatissuperdesk.Left = (this.Width - lbwhatissuperdesk.Width) / 2;
                        lbthisissuperdesk.Top = lbwhatissuperdesk.Top + lbwhatissuperdesk.Height + 10;
                        lbthisissuperdesk.Left = (this.Width - lbthisissuperdesk.Width) / 2;

                        lbgetthepackage.Top = lbthisissuperdesk.Top + lbthisissuperdesk.Height + 10;
                        lbgetthepackage.Left = (this.Width - lbgetthepackage.Width) / 2;

                        lbpackagedesc.Top = lbgetthepackage.Top + lbgetthepackage.Height + 10;
                        lbpackagedesc.Left = (this.Width - lbpackagedesc.Width) / 2;

                        lnkdownload.Top = lbpackagedesc.Top + lbpackagedesc.Height + 10;
                        lnkdownload.Left = (this.Width - lnkdownload.Width) / 2;

                        lnkdownload.LinkColor = lbpackagedesc.ForeColor;
                    }));
                }
                catch
                {
                }
            };
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {
            this.HorizontalScroll.Maximum = 0;
            this.AutoScroll = false;
            this.VerticalScroll.Visible = false;
            this.AutoScroll = true;

        }

        private void lnkdownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var attrib = typeof(Applications.WidgetManagerFrontend).GetCustomAttributes(false).FirstOrDefault(x => x is StpContents) as StpContents;
            if(attrib != null)
            {
                FileSkimmerBackend.GetFile(new[] { ".stp" }, FileOpenerStyle.Save, (file) =>
                 {
                     WriteAllText(file, JsonConvert.SerializeObject(attrib));
                 });
            }
            else
            {
                Infobox.Show("Service not available.", "The Shiftnet service you requested is not available.");
            }
        }
    }
}
