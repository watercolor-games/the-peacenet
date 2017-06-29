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
using Newtonsoft.Json;
using ShiftOS.Engine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("{TITLE_SHIFTLETTERS}", false, null, "{AL_GAMES}")]
    [AppscapeEntry("shiftletters", "{TITLE_SHIFTLETTERS}", "{DESC_SHIFTLETTERS}", 300, 150, null, "{AL_GAMES}")]
    [WinOpen("{WO_SHIFTLETTERS}")]
    [DefaultIcon("iconShiftLetters")]
    public partial class ShiftLetters : UserControl, IShiftOSWindow
    {
        int lives = 7;
        string word = "";
        static Random rng = new Random();
        string guessedCharacters = "";
        List<String> shiftWordlist = new List<string> { "shiftos", "devx", "artpad", "shifter", "pong",
                "shiftorium", "codepoints", "shiftletters", "shops", "mud", "notification", "namechanger",
                "skinning", "skinloader", "calculator", "fileskimmer", "lua", "shiftnet", "terminal", "textpad"};
        List<String> contributorsWordlist = new List<string> { "philipadams", "carverh", "computelinux", "lempamo",
                "wowmom", "michaeltheshifter", "arencllc", "therandommelon", "pfg", "craftxbox", "ashifter"};

        List<string> osWordlist = new List<string>
        {
            "windows",
            "longhorn",
            "memphis",
            "neptune",
            "vista",
            "visopsys",
            "ubuntu",
            "linux",
            "arch",
            "debian",
            "redhat",
            "fedora",
            "opensuse",
            "kubuntu",
            "lubuntu",
            "xubuntu",
            "mythbuntu",
            "ubuntumate",
            "zorin",
            "lindows",
            "msdos",
            "freedos",
            "freebsd",
            "netbsd",
            "pcbsd",
            "android",
            "ios",
            "macos",
            "mint",
            "mikeos",
            "raspbian",
            "cosmos",
            "chicago",
            "vienna",
            "whistler",
            "windowsxp",
            "windowsforworkgroups",
            "shiftos"
        };

        public ShiftLetters()
        {
            InitializeComponent();
        }
        
        private void StartGame()
        {
            guessedCharacters = "";
            lives = 7;
            comboBox1.Visible = false;
            tbguess.Visible = true;
            lbllives.Visible = true;
            lblword.Visible = true;
            btnrestart.Visible = false;

            bool isShiftOS = comboBox1.SelectedItem.ToString().ToLower() == "shiftos";
            bool isContributors = comboBox1.SelectedItem.ToString().ToLower() == "contributors";
            bool isOSes = comboBox1.SelectedItem.ToString().ToLower() == "operating systems";


            var wordlist = new List<string>();
            if (isOSes)
            {
                foreach (var w in osWordlist)
                {
                    if (!wordlist.Contains(w.ToLower()))
                    {
                        wordlist.Add(w.ToLower());
                    }
                }
            }
            if (isShiftOS)
            {
                foreach (var w in shiftWordlist)
                {
                    if (!wordlist.Contains(w.ToLower()))
                    {
                        wordlist.Add(w.ToLower());
                    }
                }
            }
            else if (isContributors)
            {
                foreach (var w in contributorsWordlist)
                {
                    if (!wordlist.Contains(w.ToLower()))
                    {
                        wordlist.Add(w.ToLower());
                    }
                }
            }
            if (isShiftOS)
            {
                //This can diversify the amount of ShiftOS-related words in the game.
                foreach (var upg in Shiftorium.GetDefaults())
                {
                    foreach (var w in upg.Name.Split(' '))
                    {
                        if (!wordlist.Contains(w.ToLower()))
                            wordlist.Add(w.ToLower());
                    }
                }
            }
            word = wordlist[rng.Next(wordlist.Count)];
            while(word == lastword)
            {
                word = wordlist[rng.Next(wordlist.Count)];
            }
            lastword = word; //to make the game not choose the same word twice or more in a row
            lbllives.Text = "You have 7 lives left!";
            lblword.Text = "";
            for (int i=0; i<word.Length; i++)
            {
                lblword.Text = lblword.Text + "_ ";
            }
        }

        public void OnLoad()
        {
            tbguess.Visible = false;
            comboBox1.Items.Add("ShiftOS");
            if (ShiftoriumFrontend.UpgradeInstalled("sl_contributors_wordlist")) comboBox1.Items.Add("Contributors");
            if (Shiftorium.UpgradeInstalled("sl_operating_systems_wordlist")) comboBox1.Items.Add("Operating Systems");
            btnrestart.Visible = true;
            lblword.Left = (this.Width - lblword.Width) / 2;
            comboBox1.SelectedIndex = 0;
            tmrcenter.Tick += (o, a) =>
            {
                this.tbguess.CenterParent();
                this.tbguess.Parent.CenterParent();
            };
            tmrcenter.Interval = 50;
            tmrcenter.Start();
        }

        Timer tmrcenter = new Timer();

        public void OnUpgrade()
        {

        }

        public bool OnUnload()
        {
            tmrcenter.Stop();
            return true;
        }

        public void OnSkinLoad()
        {
            lblword.Left = (this.Width - lblword.Width) / 2;
        }

        string lastword = "";

        private void tbguess_TextChanged(object sender, EventArgs e)
        {
            if (this.tbguess.Text.Length == 1)
            {
                var charGuessed = this.tbguess.Text.ToLower();
                bool correct = false;
                this.tbguess.Text = "";
                for (int i=0; i < word.Length; i++)
                {
                    if (word[i] == System.Convert.ToChar(charGuessed) & lives > 0)
                    {
                        char[] letters = lblword.Text.ToCharArray();
                        correct = true;
                        letters[i * 2] = System.Convert.ToChar(charGuessed);
                        lblword.Text = string.Join("", letters);
                        if (!lblword.Text.Contains("_"))
                        {
                            int oldlives = lives;
                            ulong cp = (ulong)(word.Length * oldlives) * 2; //drunky michael made this 5x...
                            SaveSystem.TransferCodepointsFrom("shiftletters", cp);
                            StartGame();
                        }
                    }
                }
                if (correct == false & lives > 0 & !guessedCharacters.Contains(charGuessed))
                {
                    guessedCharacters = guessedCharacters + charGuessed;
                    lives--;
                    if (lives == 1)
                    {
                        lbllives.Text = "You have 1 life left! Be careful...";
                    } else {
                        lbllives.Text = "You have " + lives + " lives left!";
                    }
                    if (lives == 0)
                    {
                        tbguess.Visible = false;
                        lbllives.Visible = false;
                        btnrestart.Visible = true;
                        btnrestart.Text = "Restart";
                        comboBox1.Visible = true;
                    }
                }
            }
        }

        private void btnrestart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private class NullWordlistException : Exception
        {
            public NullWordlistException(string message) : base("ShiftLetters tried to use a Null Wordlist.")
            {

            }
        }

        private void lblword_TextChanged(object sender, EventArgs e)
        {
            lblword.Left = (this.Width - lblword.Width) / 2;
        }
    }
}
