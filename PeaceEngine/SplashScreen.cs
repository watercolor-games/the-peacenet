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
    /// <summary>
    /// A <see cref="Form"/> with a simple GUI that shows the progress of loading all engine components. 
    /// </summary>
    public partial class SplashScreen : Form
    {
        EventWaitHandle handleEv = new AutoResetEvent(false);
        volatile bool handleCreated = false;
        /// <summary>
        /// Creates a new instance of the <see cref="SplashScreen"/> form. 
        /// </summary>
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

        /// <summary>
        /// Set the style of the progress bar.
        /// </summary>
        /// <param name="style">The style of the progress bar.</param>
        public void SetProgressType(ProgressBarStyle style)
        {
            while (!handleCreated)
                handleEv.WaitOne();
            this.Invoke(new Action(() =>
            {
                pgprogress.Style = style;
            }));
        }

        /// <summary>
        /// Set the value and range of the progress bar.
        /// </summary>
        /// <param name="val">The value of the progress bar.</param>
        /// <param name="max">The maximum value of the progress bar.</param>
        /// <param name="status">The status text shown above the progress bar.</param>
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
