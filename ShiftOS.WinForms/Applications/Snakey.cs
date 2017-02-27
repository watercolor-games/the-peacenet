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
    [Launcher("Snakey", true, "al_snakey", "Games")]
    [RequiresUpgrade("snakey")]
    [WinOpen("snakey")]
    [DefaultIcon("iconSnakey")]
    public partial class Snakey : UserControl, IShiftOSWindow
    {
        public Snakey()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            throw new NotImplementedException();
        }

        public void OnSkinLoad()
        {
            throw new NotImplementedException();
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            throw new NotImplementedException();
        }
    }
}
