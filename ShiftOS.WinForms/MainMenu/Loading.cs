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
            label.Text = Localization.Parse(label.Text);
            label1.Text = "AMIBIOS(C)"+ DateTime.Now.Year +" Australian Microtrends, Plc.\nShiftsoft 480-L ACPI BIOS Revision 1002\nCPU : VTC 210-N " + Environment.ProcessorCount + " CPU 1.33GHz\n   Speed : 1.337Ghz\n\nPress DEL to run Setup\nPress <F8> for BBS POPUP\nDDR3 Frequency 1337MHz, Dual Channel, Linear Mode\nChecking NVRAM\n\n1337MB OK";
        }

        private void Loading_FormShown(object sender, EventArgs e)
        {
            // This hideous timer thing is the most reliable way to make the form update
            // before it starts doing stuff.
            var callback = new Timer();
            callback.Tick += (o, a) => { Desktop.CurrentDesktop.Show(); Hide(); callback.Stop(); };
            callback.Interval = 1000; //also so the bios screen shows
            callback.Start();
        }

        private void label_Click(object sender, EventArgs e)
        {

        }
    }
}
