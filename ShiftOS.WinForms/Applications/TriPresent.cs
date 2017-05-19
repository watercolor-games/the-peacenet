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
    [WinOpen("tripresent")]
    [AppscapeEntry("TriPresent", "Part of the trilogy of office applications for enhancement of your system. TriPresent is easliy the best presentation creator out there for ShiftOS.", 1024, 750, "file_skimmer", "Office")]
    [DefaultTitle("TriPresent")]
    [Launcher("TriPresent", false, null, "Office")]
    public partial class TriPresent : UserControl
    {
        public TriPresent()
        {
            InitializeComponent();
        }

        private void addLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addItemLabel.Text = "Add Label";
            AddItem.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddItem.Hide();
        }
    }
}
