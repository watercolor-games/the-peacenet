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

namespace ShiftOS.WinForms.MainMenu
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        private void Loading_FormShown(object sender, EventArgs e)
        {
            // This hideous timer thing is the most reliable way to make the form update
            // before it starts doing stuff.
            var callback = new Timer();
            callback.Tick += (o, a) => { Desktop.CurrentDesktop.Show(); Hide(); callback.Stop(); };
            callback.Interval = 1;
            callback.Start();
        }
    }
}
