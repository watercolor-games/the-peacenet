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
    [Launcher("TextPad", true, "al_textpad")]
    [RequiresUpgrade("textpad")]
    [WinOpen("textpad")]
    public partial class TextPad : UserControl, IShiftOSWindow
    {
        public TextPad()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtcontents.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var types = new List<string>();
            types.Add(".txt");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_lua_support"))
                types.Add(".lua");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_python_support"))
                types.Add(".py");


            AppearanceManager.SetupDialog(new FileDialog(types.ToArray(), FileOpenerStyle.Open, new Action<string>((file) => this.LoadFile(file))));
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
            var types = new List<string>();
            types.Add(".txt");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_lua_support"))
                types.Add(".lua");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_python_support"))
                types.Add(".py");

            AppearanceManager.SetupDialog(new FileDialog(types.ToArray(), FileOpenerStyle.Save, new Action<string>((file) => this.SaveFile(file))));

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

        private void txtcontents_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
