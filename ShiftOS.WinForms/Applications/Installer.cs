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
using System.Threading;
using static ShiftOS.Objects.ShiftFS.Utils;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("{WO_INSTALLER}")]
    [RequiresUpgrade("installer")]
    [MultiplayerOnly]
    [DefaultTitle("{TITLE_INSTALLER}")]
    [Launcher("{TITLE_INSTALLER}", true, "al_installer", "{AL_UTILITIES}")]
    [FileHandler("Name Pack", ".names", "fileiconnames")]
    public partial class Installer : UserControl, IShiftOSWindow, IFileHandler
    {
        public Installer()
        {
            InitializeComponent();
            lbtitle.Text = "Select file";
        }

        public void OpenFile(string path)
        {
            var stpInstall = new StpInstallation(path);
            AppearanceManager.SetupWindow(this);
            InitiateInstall(stpInstall);
        }

        public void InitiateInstall(Installation install)
        {
            pnlselectfile.Hide();
            pginstall.Show();
            lbprogress.Show();
            lbtitle.Text = install.Name;
            install.ProgressReported += (p) =>
            {
                Desktop.InvokeOnWorkerThread(new Action(() =>
                {
                    pginstall.Value = p;
                }));
            };
            install.StatusReported += (s) =>
            {
                Desktop.InvokeOnWorkerThread(new Action(() =>
                {
                    lbprogress.Text = s;
                }));
            };
            install.InstallCompleted += () =>
            {
                Desktop.InvokeOnWorkerThread(new Action(() =>
                {
                    lbtitle.Text = "Select file";
                    pnlselectfile.Show();
                }));
                isInstalling = false;
                InstallCompleted?.Invoke();
            };
            while (!this.Visible)
            {
                
            }
            isInstalling = true;
            install.Install();
        }

        public void OnLoad()
        {
            btnstart.Hide();
            pginstall.Hide();
            lbprogress.Hide();
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

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".stp" }, FileOpenerStyle.Open, (file) =>
            {
                txtfilepath.Text = file;
                btnstart.Show();
            });
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            if (FileExists(txtfilepath.Text))
            {
                var install = new StpInstallation(txtfilepath.Text);
                InitiateInstall(install);
            }
            else
            {
                Infobox.Show("File not found.", "The file you requested was not found on the system.");
            }
        }
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

    public class StpInstallation : Installation
    {
        public StpInstallation(string stpfile) : base()
        {
            if (!FileExists(stpfile))
                throw new System.IO.FileNotFoundException("An attempt to install a ShiftOS setup package failed because the package was not found.", stpfile);
            string json = ReadAllText(stpfile);
            var contents = JsonConvert.DeserializeObject<StpContents>(json);
            this.Name = contents.Name;
            Contents = contents;
        }

        public StpContents Contents { get; set; }

        protected override void Run()
        {
            if (Shiftorium.UpgradeInstalled(Contents.Upgrade))
            {
                Infobox.Show("Installation failed.", "This package has already been installed.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(Contents.Dependencies))
            {
                SetStatus("Checking Shiftorium for dependencies...");
                string[] dependencies = Contents.Dependencies.Split(';');
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (Shiftorium.UpgradeInstalled(dependencies[i]))
                    {
                        double percent = (i / dependencies.Length) * 100;
                        SetProgress((int)percent);
                    }
                    else
                    {
                        var upg = Shiftorium.GetDefaults().FirstOrDefault(x => x.Id == dependencies[i]);
                        Infobox.Show("Missing dependency!", $"You are missing the following Shiftorium Upgrade: {upg.Name}\r\n\r\nThe installation cannot continue.");
                        return;
                    }
                    Thread.Sleep(250);
                }
            }
            SetStatus("Installing...");
            SetProgress(0);
            for(int i = 0; i < 100; i++)
            {
                SetProgress(i);
                Thread.Sleep(50);
            }
            Desktop.InvokeOnWorkerThread(() =>
            {
                Shiftorium.Buy(Contents.Upgrade, 0);
                Infobox.Show("Install complete.", "The installation has completed successfully.");
                SaveSystem.SaveGame();
            });
        }
    }

    public class StpContents : RequiresUpgradeAttribute
    {
        public StpContents(string name, string author, string dependencies = "") : base(name.ToLower().Replace(" ", "_"))
        {
            Name = name;
            Author = author;
            Dependencies = dependencies;
        }

        public string Name { get; set; }
        public string Author { get; set; }
        public string Dependencies { get; set; }
    }

}
