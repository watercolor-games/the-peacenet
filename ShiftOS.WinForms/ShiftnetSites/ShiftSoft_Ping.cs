using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/shiftsoft/ping", "Ping", "A site listing hosted by ShiftSoft.")]
    public partial class ShiftSoft_Ping : UserControl, IShiftnetSite
    {
        public ShiftSoft_Ping()
        {
            InitializeComponent();
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
            SetupListing();
        }

        public void SetupListing()
        {
            fllist.Controls.Clear();
            foreach (var type in Array.FindAll(ReflectMan.Types, t => t.GetInterfaces().Contains(typeof(IShiftnetSite))))
            {
                var attr = type.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftnetSiteAttribute) as ShiftnetSiteAttribute;
                if (attr != null)
                {
                    if (attr.Url.StartsWith("shiftnet/"))
                    {
                        var lnk = new LinkLabel();
                        lnk.LinkColor = SkinEngine.LoadedSkin.ControlTextColor;
                        lnk.Text = attr.Name;
                        var desc = new Label();
                        desc.AutoSize = true;
                        lnk.AutoSize = true;
                        desc.MaximumSize = new Size(this.Width / 3, 0);
                        desc.Text = attr.Description;
                        desc.Padding = new Padding
                        {
                            Bottom = 25,
                            Top = 0,
                            Left = 10,
                            Right = 10
                        };
                        lnk.Click += (o, a) =>
                        {
                            GoToUrl?.Invoke(attr.Url);
                        };
                        fllist.Controls.Add(lnk);
                        fllist.Controls.Add(desc);
                        ControlManager.SetupControls(lnk);
                        lnk.Show();
                        desc.Show();
                    }
                }
            }
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {
            SetupListing();
        }
    }
}
