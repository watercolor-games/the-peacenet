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
    [Launcher("Calculator", true, "al_calculator", "Accessories")]
    [RequiresUpgrade("calculator")]
    [WinOpen("calculator")]
    [DefaultIcon("iconCalculator")]
    public partial class Calculator : UserControl, IShiftOSWindow
    {
        public bool justopened = false;

        public Calculator()
        {
            InitializeComponent();
        }

        private void Template_Load(object sender, EventArgs e)
        {
            justopened = true;
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
            throw new NotImplementedException();
        }

        public void OnUpgrade()
        {
            throw new NotImplementedException();
        }
    }
}
