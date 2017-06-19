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
            
            //Now let's do it Michael-style.
            label1.Text = Localization.Parse(@"{MISC_BIOSCOPYRIGHT}
{MISC_BIOSVERSION}
{MISC_BIOSCPU}
{MISC_CLOCKSPEED}

{MISC_RUNSETUP}
{MISC_BBSPOPUP}
{MISC_RAMFREQ}
{MISC_CHECKINGNVRAM}

{MISC_RAM}", new Dictionary<string, string>
            {
                                                 ["%year"] = DateTime.Now.Year.ToString(),
                                                 ["%cores"] = Environment.ProcessorCount.ToString()
            });
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
