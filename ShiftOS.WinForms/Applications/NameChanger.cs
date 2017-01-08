using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications {

    [Launcher("Name Changer", true, "al_name_changer")]
    [RequiresUpgrade("name_changer")]
    [WinOpen("name_changer")]
    public partial class NameChanger : UserControl, IShiftOSWindow {
        public NameChanger() {
            InitializeComponent();
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void NameChanger_Load(object sender, EventArgs e) {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
