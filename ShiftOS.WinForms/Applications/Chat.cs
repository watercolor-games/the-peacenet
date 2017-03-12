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

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    public partial class Chat : UserControl, IShiftOSWindow
    {
        public Chat(string chatId)
        {
            InitializeComponent();
            id = chatId;
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "chat_msgreceived")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            try
                            {
                                var args = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Contents);
                                var cmsg = new ShiftOS.Objects.ChatMessage(args["Username"] as string, args["SystemName"] as string, args["Message"] as string, args["Channel"] as string);
                                if (id == cmsg.Channel)
                                    rtbchat.AppendText($"[{cmsg.Username}@{cmsg.SystemName}]: {cmsg.Message}{Environment.NewLine}");
                            }
                            catch (Exception ex)
                            {
                                rtbchat.AppendText($"[system@multiuserdomain] Exception thrown by client: {ex}");
                            }
                        }));
                    }
                    catch { }
                }
                else if(msg.Name == "chatlog")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            rtbchat.AppendText(msg.Contents);
                        }));
                    }
                    catch { }
                }
            };
        }

        public void SendMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                rtbchat.AppendText($"[{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}] {msg}{Environment.NewLine}");

                ServerManager.SendMessage("chat_send", JsonConvert.SerializeObject(new ShiftOS.Objects.ChatMessage(SaveSystem.CurrentSave.Username, SaveSystem.CurrentSave.SystemName, msg, id)));
            }
            else
            {
                rtbchat.AppendText($"[sys@multiuserdomain] You can't send blank messages. (only you can see this)");
            }
        }


        private string id = "";

        public void OnLoad()
        {
            ServerManager.SendMessage("chat_getlog", JsonConvert.SerializeObject(new ShiftOS.Objects.ChatLogRequest(id, 50)));

            SendMessage("User has joined the chat.");
            RefreshUserInput();
        }

        public void OnSkinLoad()
        {
            RefreshUserInput();
        }

        public bool OnUnload()
        {
            SendMessage("User has left the chat.");
            id = null;
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
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                btnsend_Click(sender, EventArgs.Empty);
            }
        }

        private void rtbchat_TextChanged(object sender, EventArgs e)
        {
            rtbchat.SelectionStart = rtbchat.Text.Length;
            rtbchat.ScrollToCaret();
            tschatid.Text = id;
            tsuserdata.Text = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}";
            RefreshUserInput();
        }

        private void btnsend_Click(object sender, EventArgs e)
        {
            SendMessage(txtuserinput.Text);
            txtuserinput.Text = "";
        }
    }
}
