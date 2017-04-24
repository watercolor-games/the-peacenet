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
    [ShiftnetSite("shiftnet/superdesk", "SuperDesk", "Empower the ShiftOS Desktop like never before.")]
    [ShiftnetFundamental]
    public partial class DesktopWidgetWebsite : UserControl, Engine.IShiftnetSite
    {
        public DesktopWidgetWebsite()
        {
            InitializeComponent();
            this.Click += (o, a) =>
            {
                GoToUrl?.Invoke("shiftnet/main");
                Infobox.Show("Haha!", "You thought this would do something else. But no!");
            };
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {

        }
    }
}
