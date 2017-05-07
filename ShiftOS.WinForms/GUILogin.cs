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
using ShiftOS.Objects;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    public partial class GUILogin : Form
    {
        public GUILogin()
        {
            InitializeComponent();
            uiTimer.Tick += (o, a) =>
            {
                btnlogin.Left = txtusername.Left + ((txtusername.Width - btnlogin.Width) / 2);
                btnlogin.Top = txtpassword.Top + txtpassword.Height + 5;

                lblogintitle.Left = pnlloginform.Left + ((pnlloginform.Width - lblogintitle.Width) / 2);
                lblogintitle.Top = pnlloginform.Top - lblogintitle.Width - 5;

            };
            uiTimer.Interval = 100;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
        }

        Timer uiTimer = new Timer();

        public event Action<ClientSave> LoginComplete;

        private void GUILogin_Load(object sender, EventArgs e)
        {
            uiTimer.Start();
            ControlManager.SetupControl(lblogintitle);
            ControlManager.SetupControls(pnlloginform);
            ControlManager.SetupControl(btnshutdown);
            pnlloginform.CenterParent();
            txtusername.CenterParent();
            txtusername.Location = new System.Drawing.Point(txtusername.Location.X, 77);
            txtpassword.CenterParent();
            btnlogin.CenterParent();
            btnlogin.Location = new System.Drawing.Point(btnlogin.Location.X, 143);
            this.BackColor = SkinEngine.LoadedSkin.LoginScreenColor;
            this.BackgroundImage = SkinEngine.GetImage("login");
            this.BackgroundImageLayout = SkinEngine.GetImageLayout("login");
        }

        private ClientSave User = null;

        bool userRequestClose = true;
        bool shuttingdown = false;

        private void GUILogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = userRequestClose;
            if (!e.Cancel)
            {
                uiTimer.Stop();
                if (shuttingdown == false)
                {
                    LoginComplete?.Invoke(User);
                }
            }
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtusername.Text))
            {
                Infobox.Show("Enter a username", "You must enter your username to login.");
                return;
            }

            //Don't check for blank passwords.

            var user = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == txtusername.Text);
            if(user == null)
            {
                Infobox.Show("Invalid username", "That username was not found on your system.");
                return;
            }

            if (user.Password != txtpassword.Text)
            {
                Infobox.Show("Access denied.", "That password didn't work. Please try a different one.");
                return;
            }

            User = user;
            userRequestClose = false;
            shuttingdown = false;
            this.Close();
        }

        private void btnshutdown_Click(object sender, EventArgs e)
        {
            userRequestClose = false;
            shuttingdown = true;
            this.Close();
            SaveSystem.CurrentUser = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == "root");
            TerminalBackend.InvokeCommand("sos.shutdown");
        }
    }

    public class GUILoginFrontend : ILoginFrontend
    {
        public bool UseGUILogin
        {
            get
            {
                return Shiftorium.UpgradeInstalled("gui_based_login_screen");
            }
        }

        public event Action<ClientSave> LoginComplete;

        public void Login()
        {
            var lform = new GUILogin();
            lform.LoginComplete += (user) =>
            {
                LoginComplete?.Invoke(user);
            };
            lform.Show();
        }
    }
}
