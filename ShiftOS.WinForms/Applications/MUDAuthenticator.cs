using ShiftOS.Objects;
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

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("MUD Administrator", true, "al_mud_cracker")]
    [RequiresUpgrade("mud_cracker")]
    [WinOpen("mud_administrator")]
    public partial class MUDAuthenticator : UserControl, IShiftOSWindow
    {
        public MUDAuthenticator()
        {
            try
            {
                InitializeComponent();
                ServerManager.ServerAccessGranted += () =>
                {
                    this.Invoke(new Action(() => { Granted(); }));
                };
                ServerManager.ServerAccessDenied += () =>
                {
                    this.Invoke(new Action(() => { Denied(); }));
                };
                ServerManager.GUIDReceived += (guid) =>
                {
                    this.guid = guid;
                };
                ServerManager.UsersReceived += (users) =>
                {
                    foreach(var user in users)
                    {
                        if (!this.users.ContainsKey(user.Username))
                        {
                            this.users.Add(user.Username, user);
                        }
                        else
                        {
                            this.users[user.Username].OnlineChat += ";" + user.OnlineChat;
                        }
                    }
                    this.Invoke(new Action(() =>
                    {
                        SetupUserList();
                    }));
                };
            }
            catch
            {
            
            }

            pnllogin.Left = (pnllogin.Parent.Width - pnllogin.Width) / 2;
            pnllogin.Top = (pnllogin.Parent.Height - pnllogin.Height) / 2;

            pnllogin.Parent.SizeChanged += (o, a) =>
            {
                pnllogin.Left = (pnllogin.Parent.Width - pnllogin.Width) / 2;
                pnllogin.Top = (pnllogin.Parent.Height - pnllogin.Height) / 2;
            };

            pnlmain.Left = (pnlmain.Parent.Width - pnlmain.Width) / 2;
            pnlmain.Top = (pnlmain.Parent.Height - pnlmain.Height) / 2;



        }

        public void Granted()
        {
            Infobox.Show("{ACCESS_GRANTED}", "{ACCESS_GRANTED_MUDADMIN_EXP}");
            //This will tell the server to take the granted password off the list of granted passwords
            //so the user must generate and crack a new one.
            ServerManager.SendMessage("mudhack_killpass", "");

            SetupAuthUI();
        }

        public void SetupAuthUI()
        {
            pnllogin.Hide();
            
            pnlmain.Show();

            PopulateUserList();
        }

        Dictionary<string, OnlineUser> users = new Dictionary<string, OnlineUser>();

        public void PopulateUserList()
        {
            users = null;
            ServerManager.SendMessage("mudhack_getallusers", "");
        }

        private string guid = null;

        public void SetupUserList()
        {
            lbusers.Items.Clear();
            foreach(var kv in users)
            {
                guid = null;
                ServerManager.SendMessage("getguid_send", kv.Key);
                while(guid == null)
                {

                }
                users[kv.Key].Guid = guid;
                lbusers.Items.Add(kv.Key);
            }
        }

        public void Denied()
        {
            Infobox.Show("{ACCESS_DENIED}", "{ACCESS_DENIED_MUDADMIN_EXP}");
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("mudhack_verify", "{pass: \"" + txtpassword.Text + "\"}");
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
