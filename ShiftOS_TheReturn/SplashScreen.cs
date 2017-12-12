using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plex.Engine
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public void SetProgressType(ProgressBarStyle style)
        {
            this.Invoke(new Action(() =>
            {
                pgprogress.Style = style;
            }));
        }

        public void SetProgress(int val, int max, string status)
        {
            this.Invoke(new Action(() =>
            {
                pgprogress.Maximum = max;
                pgprogress.Value = val;
                lbstatus.Text = status;
            }));
        }
    }
}
