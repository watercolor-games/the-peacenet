/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
    [Launcher("MUD Administrator", true, "al_mud_cracker", "Administration")]
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
