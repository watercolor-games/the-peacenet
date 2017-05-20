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
    [AppscapeEntry("TriPresent", "Part of the trilogy of office applications for enhancement of your system. TriPresent is easliy the best presentation creator out there for ShiftOS.", 2048, 1500, "file_skimmer", "Office")]
    [DefaultTitle("TriPresent")]
    [Launcher("TriPresent", false, null, "Office")]
    public partial class TriPresent : UserControl, IShiftOSWindow
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

        private void placeAdd_Click(object sender, EventArgs e)
        {
            Label label = new Label();
            label.Parent = designerPanel;
            label.BackColor = Color.Transparent;
            label.ForeColor = panel1.BackColor;
            label.Text = labelContents.Text;
            label.Location = new Point(Convert.ToInt32(xPosition.Value), Convert.ToInt32(yPosition.Value));
            label.Text = labelContents.Text;
            Random rnd = new Random();
            label.Name = labelContents.Text + rnd.Next(0, 500);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new ColorPicker(panel1.BackColor, "Text Color", new Action<Color>((col) =>
            {
                panel1.ForeColor = col;
                panel1.BackColor = col;
            })));
        }
    }
}
