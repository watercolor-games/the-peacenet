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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [WinOpen("{WO_SIMPLESRC}")]
    [Launcher("{TITLE_SIMPLESRC}", false, null, "{AL_NETWORKING}")]
    [DefaultTitle("{TITLE_SIMPLESRC}")]
    [AppscapeEntry("simplesrc_client", "{TITLE_SIMPLESRC}", "{DESC_SIMPLESRC}", 300, 145, "file_skimmer", "{AL_NETWORKING}")]
    public partial class Chat : UserControl, IShiftOSWindow
    {
        public Chat()
        {
            InitializeComponent();
        }

        public event Action<ShiftOS.Objects.ShiftFS.File> FileSent;

        public string Typing
        {
            get
            {
                return lbtyping.Text;
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        lbtyping.Visible = false;
                    }
                    else
                    {
                        lbtyping.Text = value + " is typing...";
                        lbtyping.Visible = true;
                    }
                }));
            }
        }

        public void ShowChat()
        {
            this.Invoke(new Action(() =>
            {
                pnlstart.Hide();
                rtbchat.Show();
                rtbchat.BringToFront();
            }));
        }

        public void OnLoad()
        {
            AllInstances.Add(this);
            if (!string.IsNullOrWhiteSpace(ChatID))
                pnlstart.SendToBack();
            RefreshUserInput();
        }

        public void OnSkinLoad()
        {
            RefreshUserInput();
        }

        public bool OnUnload()
        {
            AllInstances.Remove(this);
            ChatID = null;
            return true;
        }

        public void OnUpgrade()
        {
            RefreshUserInput();
        }

        public void RefreshUserInput()
        {
            txtuserinput.Width = (tsbottombar.Width) - (btnsend.Width) - 15;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void txtuserinput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                btnsend_Click(sender, EventArgs.Empty);
            }
        }

        private void rtbchat_TextChanged(object sender, EventArgs e)
        {
            rtbchat.SelectionStart = rtbchat.Text.Length;
            rtbchat.ScrollToCaret();
            tschatid.Text = ChatID;
            AppearanceManager.SetWindowTitle(this, tschatid.Text + " - SimpleSRC Client");
            tsuserdata.Text = $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}";
            RefreshUserInput();
        }

        public static readonly List<Chat> AllInstances = new List<Chat>();

        public static void SendMessage(string user, string destination, string msg)
        {
            foreach (var chat in AllInstances)
            {
                chat.PostMessage(user, destination, msg);
            }
        }

        public string ChatID = "";

        public void PostMessage(string user, string destination, string message)
        {
            if (ChatID == destination)
            {
                this.Invoke(new Action(() =>
                {
                    rtbchat.SelectionFont = new Font(rtbchat.Font, FontStyle.Bold);
                    rtbchat.AppendText($"[{user}] ");
                    rtbchat.SelectionFont = rtbchat.Font;
                    rtbchat.AppendText(message);
                    rtbchat.AppendText(Environment.NewLine + Environment.NewLine);
                }));
            }
        }

        private void btnsend_Click(object sender, EventArgs e)
        {
            //Update ALL chat windows with this message if they're connected to this chat.
            SendMessage($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}", ChatID, txtuserinput.Text);
            txtuserinput.Text = "";
        }

        private void btnsendfile_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { "" }, FileOpenerStyle.Open, (file) =>
            {
                var finf = ShiftOS.Objects.ShiftFS.Utils.GetFileInfo(file);
                PostMessage($"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}", ChatID, "<user sent a file: " + finf.Name + ">");
                FileSent?.Invoke(finf);
            });
        }
    }
}
