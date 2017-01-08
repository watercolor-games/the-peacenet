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
    [Launcher("MUD Chat", true, "al_mud_chat")]
    [RequiresUpgrade("mud_fundamentals")]
    [WinOpen("chat")]
    public partial class Chat : UserControl, IShiftOSWindow
    {
        public Chat(string chatId)
        {
            InitializeComponent();
            id = chatId;
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "cbroadcast")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            rtbchat.Text += msg.Contents + Environment.NewLine;
                        }));
                    }
                    catch { }
                }
            };
        }

        private string id = "";

        public void OnLoad()
        {
            var save = SaveSystem.CurrentSave;
            ServerManager.SendMessage("chat_join", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)}
}}");
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            var save = SaveSystem.CurrentSave;
            ServerManager.SendMessage("chat_leave", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)}
}}");

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

                ServerManager.SendMessage("chat", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)},
    msg: ""{txtuserinput.Text}""
}}");
                txtuserinput.Text = "";
            }
        }
    }
}
