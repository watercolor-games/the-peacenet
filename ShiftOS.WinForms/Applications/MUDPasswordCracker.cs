using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("MUD cracker 1.0", true, "al_mud_cracker")]
    [RequiresUpgrade("mud_cracker")]
    [WinOpen("mud_cracker")]
    public partial class MUDPasswordCracker : UserControl, IShiftOSWindow
    {
        public MUDPasswordCracker()
        {
            InitializeComponent();
        }

        private string PasswordToCrack { get; set; }
       private string Password { get; set; }


        private void btncrack_Click(object sender, EventArgs e)
        {
            ServerManager.ServerPasswordGenerated += (pass) =>
            {
                PasswordToCrack = pass;
                this.Invoke(new Action(() =>
                {
                    BeginCrack();
                }));
            };
            ServerManager.InitiateMUDHack();
        }

        public void BeginCrack()
        {
            btncrack.Enabled = false;
            btncrack.Text = "Starting crack...";

            var t = new Thread(new ThreadStart(() =>
            {
                int secondsleft = 30;
                while(secondsleft > 0)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            btncrack.Text = $"Cracking... step {secondsleft}.";
                        }));



                        secondsleft -= 1;

                        Thread.Sleep(1000);
                    }
                    catch
                    {

                    }
                }
                this.Invoke(new Action(() => EndCrack()));
                
            }));
            t.IsBackground = true;
            t.Start();
        }

        public void EndCrack()
        {
            lblpassword.Show();
            txtpassword.Show();
            txtpassword.ReadOnly = true;
            txtpassword.Text = PasswordToCrack;

        }

        public void OnLoad()
        {
            
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
