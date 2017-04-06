using ShiftOS.Engine;
using ShiftOS.WinForms.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.WinForms
{
    public partial class LanguageSelector : Form
    {
        public string languageID;
        public bool rdy = false;

        public LanguageSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Localization.SetLanguageID((string)comboBox1.SelectedItem);

            rdy = true;

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void closing(object sender, FormClosingEventArgs e)
        {
            if (!rdy) Environment.Exit(0);
        }
    }
}
