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
        public Save MySave { get; private set; }

        public bool IsTransferMode = false;

        public Oobe()
        {
            InitializeComponent();
        }

        public void HideAll()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != this.pnlshadow)
                {
                    ctrl.Hide();
                }
            }
        }

        public void SetupUI()
        {
            HideAll();
            Panel shown = null;
            if (IsTransferMode == false)
            {
                switch (MySave.StoryPosition)
                {
                    case 0:
                        shown = pnllanguage;
                        SetupLanguages();
                        break;
                    case 1:
                        shown = pnluserinfo;
                        break;
                    case 2:
                        shown = pnldiscourse;
                        break;
                    default:
                        shown = pnlterminaltutorial;
                        SetupTerminal();
                        break;
                }
            }
            else
            {
                switch (TransferProgress)
                {
                    case 0:
                        shown = pnlreenteruserinfo;
                        break;
                    case 1:
                        shown = pnlauthfail;
                        break;
                    case 2:
                        shown = pnlauthdone;
                        break;
                    case 4:
                        shown = pnlrelogin;
                        break;
                    case 5:
                        shown = pnlrelogerror;
                        break;
                }
            }
            
            if (shown != null)
            {
                shown.Location = new Point((int)(shown.Parent?.Width - shown.Width) / 2, (int)(shown?.Parent.Height - shown.Height) / 2);
                pnlshadow.Size = (Size)shown.Size;
                pnlshadow.Location = new Point((int)shown.Left + 15, (int)shown.Top + 15);
                shown.Show();
            }
        }

        private int TransferProgress = 0;

        public void SetupTerminal()
        {
            //just so that the terminal can access our save
            SaveSystem.CurrentSave = MySave;

           Applications.Terminal.MakeWidget(txtterm);
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = true;
            Console.WriteLine("{TERMINAL_TUTORIAL_1}");
            SaveSystem.TransferCodepointsFrom("oobe", 50);

            Shiftorium.Installed += () =>
            {
                if (SaveSystem.CurrentSave.StoryPosition < 5)
                {
                    if (Shiftorium.UpgradeInstalled("mud_fundamentals"))
                    {
                        Console.WriteLine("{TERMINAL_TUTORIAL_2}");
                        txtterm.Height -= pgsystemstatus.Height - 4;
                        pgsystemstatus.Show();
                        
                        StartInstall();
                        
                    }
                }
            };

        }

        public int thingsDone
        {
            get
            {
                return pgsystemstatus.Value;
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    pgsystemstatus.Value = value;
                }));
            }
        }

        public int thingsToDo
        {
            get
            {
                return pgsystemstatus.Maximum;
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    pgsystemstatus.Maximum = value;
                }));
            }
        }

        public void StartInstall()
        {
            Dictionary<string, string> Aliases = new Dictionary<string, string>();
            Aliases.Add("help", "sos.help");
            Aliases.Add("save", "sos.save");
            Aliases.Add("shutdown", "sys.shutdown");
            Aliases.Add("status", "sos.status");
            Aliases.Add("pong", "win.open{app:\"pong\"}");
            Aliases.Add("programs", "sos.help{topic:\"programs\"}");

            foreach (var path in Paths.GetAll())
            {
                Console.WriteLine("{CREATE}: " + path);
                thingsDone += 1;
                Thread.Sleep(500);
            }
            thingsToDo = Aliases.Count + Paths.GetAll().Length + 2;

            Console.WriteLine("{INSTALL}: {PONG}");
            thingsDone++;
            Thread.Sleep(200);

            Console.WriteLine("{INSTALL}: {TERMINAL}");
            thingsDone++;
            Thread.Sleep(200);

            foreach (var kv in Aliases)
            {
                Console.WriteLine($"{{ALIAS}}: {kv.Key} => {kv.Value}");
                thingsDone++;
                Thread.Sleep(450);
            }

            SaveSystem.CurrentSave.StoryPosition = 5;
            SaveSystem.SaveGame();
        }

        List<string> supportedLangs
        {
            get
            {
                //return JsonConvert.DeserializeObject<List<string>>(Properties.Resources.languages);

                return new List<string>(new[] { "english" });
            }
        }

        public void SetupLanguages()
        {
            lblanguage.Items.Clear();

            foreach (var supportedLang in supportedLangs)
            {
                lblanguage.Items.Add(supportedLang);
            }
        }

        public void SetupAllControls()
        {
            foreach (Control ctrl in this.Controls)
            {
                SetupControl(ctrl);
            }
        }

        public void SetupControl(Control ctrl)
        {
            if (!string.IsNullOrWhiteSpace(ctrl.Text))
                ctrl.Text = Localization.Parse(ctrl.Text);
            try
            {
                foreach (Control child in ctrl.Controls)
                {
                    SetupControl(child);
                }
            }
            catch
            {
            }

        }

        private void btnselectlang_Click(object sender, EventArgs e)
        {
            if (lblanguage.SelectedItem != null)
            {
                string l = lblanguage.SelectedItem as string;
                MySave.Language = l;
                MySave.StoryPosition = 1;
                SetupUI();
            }
        }

        private void btnsetuserinfo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtusername.Text))
            {
                MySave.Username = txtusername.Text;

                MySave.Password = txtpassword.Text;

                MySave.SystemName = (string.IsNullOrWhiteSpace(txtsysname.Text)) ? "shiftos" : txtsysname.Text;

                MySave.StoryPosition = 2;

                SetupUI();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtdiscoursename.Text))
            {
                MySave.DiscourseName = txtdiscoursename.Text;
                MySave.DiscoursePass = txtdiscoursepass.Text;
            }
            MySave.StoryPosition = 3;
            SetupUI();
        }

        public void StartShowing(Save save)
        {
            IsTransferMode = false;
            MySave = save;
            SetupAllControls();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            this.Load += (o, a) =>
            {
                SetupUI();
            };
            SaveSystem.GameReady += () =>
            {
                this.Invoke(new Action(() =>
                {
                    this.Close();
                    (AppearanceManager.OpenForms[0] as WindowBorder).BringToFront();
                    Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                }));

            };
            this.Show();
        }

        public void ShowSaveTransfer(Save save)
        {
            MySave = save;
            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "mud_notfound")
                {
                    TransferProgress = 2;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if(msg.Name == "mud_found")
                {
                    TransferProgress = 1;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if(msg.Name == "mud_saved")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            SaveSystem.CurrentSave = MySave;
                            this.Close();
                            Utils.Delete(Paths.SaveFileInner);
                        }));
                    }
                    catch { }
                }
            };
            IsTransferMode = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Show();
            SetupUI();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnlreenteruserinfo.Hide();
            pnlshadow.Size = new Size(0, 0);
            ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtruser.Text}"",
    password: ""{txtrpass.Text}""
}}");
            MySave.Username = txtruser.Text;
            MySave.Password = txtrpass.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TransferProgress = 0;
            SetupUI();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("mud_save", JsonConvert.SerializeObject(MySave));
        }

        public void PromptForLogin()
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "mud_notfound")
                {
                    TransferProgress = 5;
                    this.Invoke(new Action(() => { SetupUI(); }));
                }
                else if (msg.Name == "mud_found")
                {
                    this.Invoke(new Action(() =>
                    {
                        Utils.WriteAllText(Paths.GetPath("user.dat"), $@"{{
    username: ""{txtluser.Text}"",
    password: ""{txtlpass.Text}""
}}");
                        SaveSystem.ReadSave();
                        this.Close();
                    }));
                }
            };
            IsTransferMode = true;
            TransferProgress = 4;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Show();
            SetupUI();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TransferProgress = 4;
            SetupUI();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtluser.Text}"",
    password: ""{txtlpass.Text}""
}}");
        }
    }
}
