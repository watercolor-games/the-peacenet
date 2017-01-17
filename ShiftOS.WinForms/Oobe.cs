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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms
{
    public partial class Oobe : Form, IOobe
    {
        public Oobe()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

        }


        string rtext;
        string gtexttotype;
        int charcount;
        int currentletter;
        int slashcount;
        int conversationcount = 0;
        Label textgeninput;
        bool needtoclose = false;
        public bool upgraded = false;
        int hackeffect;
        int percentcount;

        
        public void TextType(string texttotype)
        {
            charcount = texttotype.Length;
            gtexttotype = texttotype;
            currentletter = 0;
            slashcount = 1;
            foreach (var c in gtexttotype)
            {
                rtext += c;
                this.Invoke(new Action(() =>
                {
                    textgeninput.Text = rtext + "|";
                }));
                Console.Beep(750, 50);
                slashcount++;
                if (slashcount == 5)
                    slashcount = 1;
            }
            rtext += Environment.NewLine;
        }

        public Save MySave = null;

        public void StartShowing(Save save)
        {
            var t = new Thread(new ThreadStart(() =>
            {
                textgeninput = this.lblHijack;
                TextType("Your system is now being hijacked.");
                rtext = "";
                Thread.Sleep(1000);
                textgeninput = this.lblhackwords;
                this.Invoke(new Action(() =>
                {
                    lblHijack.Hide();
                }));
                TextType("Hello, and welcome to ShiftOS.");
                Thread.Sleep(500);
                TextType("You have been cordially and involuntarily selected to participate in the development and testing of this operating system.");
                Thread.Sleep(500);
                TextType("My identity shall remain secret, but if you've been through this before, you'll know exactly who I am.");
                Thread.Sleep(500);
                TextType("But that doesn't matter.");
                Thread.Sleep(500);
                TextType("I will now begin to prepare your system for the installation of ShiftOS.");
                Thread.Sleep(1000);
                FakeSetupScreen fakeForm = null;
                this.Invoke(new Action(() =>
                {
                    fakeForm = new FakeSetupScreen(this);
                    fakeForm.Show();
                    MySave = save;
                    lblhackwords.GotFocus += (o, a) =>
                    {
                        try
                        {
                            fakeForm.Invoke(new Action(() =>
                            {
                                fakeForm.Focus();
                                fakeForm.BringToFront();
                            }));
                        }
                        catch { }
                    };
                    fakeForm.TextSent += (txt) =>
                    {
                        TextType(txt);
                    };
                }));
                while(fakeForm?.Visible == true)
                {

                }
                TextType("That's all the information I need for now.");
                Thread.Sleep(2000);
                TextType("Beginning installation of ShiftOS on " + MySave.SystemName + ".");
                Thread.Sleep(500);
                TextType("Creating new user: " + MySave.Username);
                TextType("...with 0 Codepoints, 0 installed upgrades, no legion, and no user shops...");
                MySave.Codepoints = 0;
                MySave.Upgrades = new Dictionary<string, bool>();
                MySave.CurrentLegions = new List<string>();
                MySave.MyShop = "";
                TextType("User created successfully.");
                Thread.Sleep(450);
                TextType("You may be wondering what all that meant... You see, in ShiftOS, your user account holds everything I need to know about you.");
                Thread.Sleep(640);
                TextType("It holds the amount of Codepoints you have - Codepoints are a special currency you can get by doing various tasks in ShiftOS.");
                Thread.Sleep(500);
                TextType("It also holds all the upgrades you've installed onto ShiftOS - features, applications, enhancements, patches, all that stuff.");
                Thread.Sleep(500);
                TextType("As for the legions and the shop thing, I'll reveal that to you when it becomes necessary.");
                Thread.Sleep(500);
                TextType("Your user account is stored on a server of mine called the multi-user domain. It holds every single user account, every script, every application, every thing within ShiftOS.");
                Thread.Sleep(600);
                TextType("Every time you boot ShiftOS, if you are connected to the Internet, you will immediately connect to the multi-user domain and ShiftOS will attempt to authenticate using the last successful username and password pair.");
                Thread.Sleep(500);
                TextType("When you are in the MUD, you are in the middle of a free-for-all. I don't want it to be this way, it just is. I've employed you to help me develop and test the MUD and ShiftOS, but you have a secondary task if you choose to accept it.");
                Thread.Sleep(500);
                TextType("There have been a few rebelious groups in the MUD - who have cracked ShiftOS's security barriers - and they're using these exploits to steal others' Codepoints, upgrades, and even spread damaging viruses.");
                Thread.Sleep(500);
                TextType("I want you to stop them.");
                Thread.Sleep(500);
                TextType("Whoever can stop these hackers will gain eternal control over the multi-user domain. They will be given the ability to do as they please, so long as it doesn't interfere with my experiments.");
                Thread.Sleep(500);
                TextType("I have been installing ShiftOS on your system in the background as I was talking with you. Before I can set you free, I need to give you a tutorial on how to use the system.");
                Thread.Sleep(500);
                TextType("I will reboot your system in Tutorial Mode now. Complete the tutorial, and you shall be on your way.");
                SaveSystem.CurrentSave = MySave;
                SaveSystem.SaveGame();
                this.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }));
            this.Show();
            this.BringToFront();
            this.TopMost = true;
            t.IsBackground = true;
            t.Start();
        }

        public void Clear()
        {
            this.Invoke(new Action(() =>
            {
                textgeninput.Text = "";
            }));
        }

        public void ShowSaveTransfer(Save save)
        {
            throw new NotImplementedException();
        }

        public void PromptForLogin()
        {
            throw new NotImplementedException();
        }
    }
}
