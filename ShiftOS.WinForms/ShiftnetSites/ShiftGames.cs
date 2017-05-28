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
    [ShiftnetSite("shiftnet/main/shiftgames", "ShiftGames (formerly Minimatch)", "The one true Shiftnet site for virus-free, quality games.")]
    public partial class ShiftGames : UserControl, IShiftnetSite
    {
        public ShiftGames()
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
        }
    }
}
