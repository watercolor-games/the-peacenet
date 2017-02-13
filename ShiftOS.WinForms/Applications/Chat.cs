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
                            
                            var cmsg = JsonConvert.DeserializeObject<ShiftOS.Objects.ChatMessage>(msg.Contents);
                            if(id == cmsg.Channel)
                                rtbchat.AppendText($"[{cmsg.Username}@{cmsg.SystemName}] {cmsg.Message}{Environment.NewLine}");
                        }));
                    }
                    catch { }
                }
            };
        }

        public void SendMessage(string msg)
        {
            ServerManager.SendMessage("chat_send", JsonConvert.SerializeObject(new ShiftOS.Objects.ChatMessage(SaveSystem.CurrentSave.Username, SaveSystem.CurrentSave.SystemName, msg, id)));
        }

        private string id = "";

        public void OnLoad()
        {
            SendMessage("User has joined the chat.");
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            SendMessage("User has left the chat.");
            id = null;
            return true;
        }

        public void OnUpgrade()
        {
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

                var save = SaveSystem.CurrentSave;

                SendMessage(txtuserinput.Text);
                txtuserinput.Text = "";
            }
        }
    }
}
