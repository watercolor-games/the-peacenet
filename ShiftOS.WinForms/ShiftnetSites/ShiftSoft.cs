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
        }

        public void OnUpgrade()
        {
        }

        public void Setup()
        {
        }
    }
}
