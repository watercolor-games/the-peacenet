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
    [WinOpen("simplesrc")]
    [Launcher("SimpleSRC Client", false, null, "Networking")]
    [DefaultTitle("SimpleSRC Client")]
    [AppscapeEntry("SimpleSRC", "A simple ShiftOS Relay Chat client that allows you to talk with other ShiftOS users from all over the world.", 300, 145, "file_skimmer", "Networking")]
    public partial class Chat : UserControl, IShiftOSWindow
    {
        public Chat()
        {
            InitializeComponent();
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
            tschatid.Text = ChatID;
            AppearanceManager.SetWindowTitle(this, tschatid.Text + " - SimpleSRC Client");
            tsuserdata.Text = $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}";
            RefreshUserInput();
        }

        public static readonly List<Chat> AllInstances = new List<Chat>();

        public static void SendMessage(string user, string destination, string msg)
        {
            foreach(var chat in AllInstances)
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

        [Story("story_thefennfamily")]
        public static void Story_TheFennFamily()
        {
            bool complete = false;
            Infobox.Show("SimpleSRC", "A direct message has been sent to you on SimpleSRC from user \"maureenfenn@trisys\".", () =>
            {
                string ch = "maureenfenn@trisys";
                var c = new Chat();
                c.ChatID = ch;
                AppearanceManager.SetupWindow(c);
                string you = $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}";

                var t = new Thread(() =>
                {
                    SendMessage(you, ch, "User has joined the chat.");
                    Thread.Sleep(2000);
                    SendMessage(ch, ch, "Hello, " + you + ". My name is Maureen. Maureen Fenn.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "I am the author of the various Tri applications you may see on Appscape.");
                    Thread.Sleep(2000);
                    SendMessage(ch, ch, "I need your help with something...");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "Firstly, a little backstory. There was a time in ShiftOS when none of us were connected.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "There wasn't a Digital Society, we didn't have chat applications or anything...");
                    Thread.Sleep(2000);
                    SendMessage(ch, ch, "All we had was the Shiftnet.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "However, in 2016, something happened called the \"connected revolution\". It was like, the invention of the Internet - it was huge for the world of ShiftOS.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "Before this, the only way you could earn Codepoints was through playing games in ShiftOS.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "I was the one who coded those games, and I would put them on a Shiftnet website that you can still access today, shiftnet/main/shiftgames.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "But when the Connected Revolution took place, things got difficult. My son, Nalyr Fenn, was born, and people stopped using my software and instead moved on to hacking eachother and stealing peoples' Codepoints.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "When Nalyr's sentience levels reached near human - i.e, he grew up, we decided to start TriOffice. It was a huge success, thanks to Aiden Nirh, the guy who runs Appscape.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "However... a few months ago he cut contact with us and we stopped receiving Codepoints from TriOffice.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "I'm running low - I can't afford to keep my system running much longer. You have to help!");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "Perhaps, you could breach Aiden's server and look for clues as to why he's against us? I'll reward you with the last Codepoints I have.");
                    Thread.Sleep(2500);
                    SendMessage(you, ch, "Alright, I'm in - but I don't know where to begin...");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "A little birdie tells me you know about the RTS exploits going around... Try using that on Aiden's server. You can find his systemname on Appscape under \"Contact Us.\" He has a mailserver on Appscape - and also has RTS on the same server.");
                    Thread.Sleep(2500);
                    SendMessage(ch, ch, "Good luck... My life depends on you!");
                    complete = true;
                });
                t.IsBackground = true;
                t.Start();
            });
            while (!complete)
                Thread.Sleep(10);
        }
    }
}
