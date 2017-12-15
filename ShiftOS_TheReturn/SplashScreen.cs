using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Plex.Engine
{
    public partial class SplashScreen : Form
    {
        EventWaitHandle handleEv = new AutoResetEvent(false);
        volatile bool handleCreated = false;
        public SplashScreen()
        {
            InitializeComponent();
            HandleCreated += SplashScreen_HandleCreated;
            Disposed += SplashScreen_Disposed;
        }

        void SplashScreen_HandleCreated(object sender, EventArgs e)
        {
            handleCreated = true;
            handleEv.Set();
        }

        void SplashScreen_Disposed(object sender, EventArgs e)
        {
            // This language sucks
            handleEv.Dispose();
        }

        public void SetProgressType(ProgressBarStyle style)
        {
            while (!handleCreated)
                handleEv.WaitOne();
            this.Invoke(new Action(() =>
            {
                pgprogress.Style = style;
            }));
        }

        public void SetProgress(int val, int max, string status)
        {
            while (!handleCreated)
                handleEv.WaitOne();
            this.Invoke(new Action(() =>
            {
                pgprogress.Maximum = max;
                pgprogress.Value = val;
                lbstatus.Text = status;
            }));
        }
    }
}
