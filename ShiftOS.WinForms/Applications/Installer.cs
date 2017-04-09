using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    public partial class Installer : UserControl, IShiftOSWindow
    {
        public Installer()
        {
            InitializeComponent();
            lbtitle.Text = "Select file";
        }

        public void InitiateInstall(Installation install)
        {
            pnlselectfile.Hide();
            install.ProgressReported += (p) =>
            {
                this.Invoke(new Action(() =>
                {
                    pginstall.Value = p;
                }));
            };
            install.StatusReported += (s) =>
            {
                this.Invoke(new Action(() =>
                {
                    lbprogress.Text = s;
                }));
            };
            install.InstallCompleted += () =>
            {
                this.Invoke(new Action(() =>
                {
                    lbtitle.Text = "Select file";
                    pnlselectfile.Show();
                }));
                isInstalling = false;
                InstallCompleted?.Invoke();
            };
            isInstalling = true;
            install.Install();
        }

        public void OnLoad()
        {
        }

        private bool isInstalling = false;

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return !isInstalling; //Don't close if an install is running.
        }

        public void OnUpgrade()
        {

        }

        private void pnlselectfile_VisibleChanged(object sender, EventArgs e)
        {
            if(this.ParentForm != null)
            {
                this.ParentForm.Height = (pnlselectfile.Visible == true) ? this.ParentForm.Height + pnlselectfile.Height : this.ParentForm.Height - pnlselectfile.Height;
            }
        }
        public event Action InstallCompleted;
    }

    public abstract class Installation
    {
        /// <summary>
        /// The display name of the installation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Occurs when the installation updates its status.
        /// </summary>
        public event Action<string> StatusReported;
        /// <summary>
        /// Occurs when the installation updates its progress percentage.
        /// </summary>
        public event Action<int> ProgressReported;
        /// <summary>
        /// Occurs when the installation completes.
        /// </summary>
        public event Action InstallCompleted;

        /// <summary>
        /// Start the installation.
        /// </summary>
        public void Install()
        {
            var t = new System.Threading.Thread(() =>
            {
                ProgressReported?.Invoke(0);
                StatusReported?.Invoke("");
                Run();
                ProgressReported?.Invoke(100);
                StatusReported?.Invoke("Installation completed.");
                InstallCompleted?.Invoke();
            });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Sets the install progress percentage.
        /// </summary>
        /// <param name="value">The installation percentage.</param>
        protected void SetProgress(int value)
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("value", "A percentage is typically between 0 and 100.... derp...");
            ProgressReported?.Invoke(value);
        }

        /// <summary>
        /// Sets the install status text.
        /// </summary>
        /// <param name="status">Text to display as status.</param>
        protected void SetStatus(string status)
        {
            StatusReported?.Invoke(status);
        }

        /// <summary>
        /// User-defined code to run during install. Once this code is ran, the installation is complete.
        /// </summary>
        protected abstract void Run();
    }




}
