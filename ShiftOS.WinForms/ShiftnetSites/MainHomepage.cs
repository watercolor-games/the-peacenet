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
using System.Reflection;
using System.IO;

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/main", "Main Site", "The main Shiftnet hub.")]
    public partial class MainHomepage : UserControl, IShiftnetSite
    {
        public MainHomepage()
        {
            InitializeComponent();
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {
            
        }

        public void Setup()
        {
            //Get the Fundamentals List
            flfundamentals.Controls.Clear();
            foreach (var type in Array.FindAll(ReflectMan.Types, t => t.GetInterfaces().Contains(typeof(IShiftnetSite)) && t.BaseType == typeof(UserControl) && Shiftorium.UpgradeAttributesUnlocked(t)))
            {
                var attrs = type.GetCustomAttributes(false);
                var attribute = attrs.FirstOrDefault(x => x is ShiftnetSiteAttribute) as ShiftnetSiteAttribute;
                if (attribute != null)
                {
                    if (attrs.OfType<ShiftnetFundamentalAttribute>().Any())
                    {
                        var dash = new Label();
                        dash.Text = " - ";
                        dash.AutoSize = true;
                        flfundamentals.Controls.Add(dash);
                        dash.Show();
                        var link = new LinkLabel();
                        link.Text = attribute.Name;
                        link.Click += (o, a) =>
                        {
                            GoToUrl?.Invoke(attribute.Url);
                        };
                        flfundamentals.Controls.Add(link);
                        flfundamentals.SetFlowBreak(link, true);
                        link.Show();
                        link.LinkColor = SkinEngine.LoadedSkin.ControlTextColor;
                    }
                }
            }

        }
    }
}

//I COMMITTED MYSELF, RYLAN!