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
    [WinOpen("web_browser")]
    [AppscapeEntry("web_browser", "Web Browser", "We're going surfing on the Internet! This application allows you to break the bounds of the Digital Society and connect to the outer Internet inside ShiftOS.",
        4096, 10000, "color_depth_24_bits", "Networking")]
    [Launcher("Web Browser", false, null, "Networking")]
    [DefaultTitle("Web Browser")]
    [DefaultIcon("iconShiftnet")]
    public partial class WebBrowser : UserControl, IShiftOSWindow
    {
        public WebBrowser()
        {
            InitializeComponent();
            uiupdate = new Timer();
            uiupdate.Tick += (o, a) =>
            {
                btnback.Location = new Point(2, 2);
                btnforward.Location = new Point(btnback.Left + btnback.Width + 2, 2);
                txturl.Location = new Point(btnforward.Left + btnforward.Width + 2, 2);
                txturl.Width = flcontrols.Width - btnback.Width - 2 - btnforward.Width - 2 - (btngo.Width * 2) - 2;
                btngo.Location = new Point(flcontrols.Width - btngo.Width - 2, 2);
                btnback.Enabled = wbmain.CanGoBack;
                btnforward.Enabled = wbmain.CanGoForward;
            };
            uiupdate.Interval = 100;
        }

        Timer uiupdate = null;

        public void OnLoad()
        {
            uiupdate.Start();
            wbmain.Url = new Uri("http://getshiftos.ml/");
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            uiupdate.Stop();
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            wbmain.GoBack();
        }

        private void btnforward_Click(object sender, EventArgs e)
        {
            wbmain.GoForward();
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            wbmain.Navigate(txturl.Text);
        }

        private void wbmain_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (Shiftorium.UpgradeInstalled("web_browser_new_window"))
            {
                AppearanceManager.SetupWindow(new WebBrowser());
            }
        }

        private void txturl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btngo_Click(sender, EventArgs.Empty);
        }

        private void wbmain_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            txturl.Text = wbmain.Url.ToString();
            AppearanceManager.SetWindowTitle(this, wbmain.DocumentTitle + " - " + NameChangerBackend.GetNameRaw(this.GetType()));
        }
    }
}
