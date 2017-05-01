using ShiftOS.Objects.ShiftFS;
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

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("triwrite")]
    [AppscapeEntry("TriWrite", "Part of the trilogy of office applications for enhancement of your system. TriWrite is easliy the best text editor out there for ShiftOS.", 1024, 750, null, "Office")]
    [DefaultTitle("TriWrite")]
    [Launcher("TriWrite", false, null, "Office")]
    public partial class TriWrite : UserControl, IShiftOSWindow
    {

        public TriWrite()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtcontents.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void LoadFile(string file)
        {
            txtcontents.Text = Utils.ReadAllText(file);
        }

        public void SaveFile(string file)
        {
            Utils.WriteAllText(file, txtcontents.Text);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

    }
}