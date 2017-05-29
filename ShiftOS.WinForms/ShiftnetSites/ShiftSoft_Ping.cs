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

        public void OnLoad()
        {
            SetupListing();
        }

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
            SetupListing();
        }

        public void SetupListing()
        {
            foreach(var exec in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if(exec.ToLower().EndsWith(".exe") || exec.ToLower().EndsWith(".dll"))
                {
                    try
                    {
                        var asm = Assembly.LoadFile(exec);
                        var types = asm.GetTypes();
                        foreach (var type in types)
                        {
                            if (type.GetInterfaces().Contains(typeof(IShiftnetSite)))
                            {
                                var attr = type.GetCustomAttributes(false).Where(x => x is ShiftnetSiteAttribute) as ShiftnetSiteAttribute;
                                if (attr != null)
                                {
                                    var lnk = new LinkLabel();
                                    lnk.LinkColor = SkinEngine.LoadedSkin.ControlTextColor;
                                    lnk.Tag = "header3";
                                    lnk.Text = attr.Name;
                                    var desc = new Label();
                                    desc.AutoSize = true;
                                    lnk.AutoSize = true;
                                    desc.MaximumSize = new Size(this.Width / 3, 0);
                                    desc.Text = attr.Description;
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
                    catch { }
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
