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
using ShiftOS.Unite;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    public partial class Oobe : Form, IOobe, ITutorial
    {
        public Oobe()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;
            this.Load += (o, a) =>
            {
                ControlManager.SetupControls(this);
            };
        }


        string rtext;
        string gtexttotype;
        int charcount;
        int slashcount;
        Label textgeninput;
        public bool upgraded = false;
        
        private bool typing = false;
        
        public void TextType(string texttotype)
        {
            textgeninput.TextAlign = ContentAlignment.MiddleCenter;
            while(typing == true)
            {
                //JESUS CHRIST PAST MICHAEL.

                //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                Thread.Sleep(100);
            }
            
            charcount = texttotype.Length;
            gtexttotype = texttotype;
            slashcount = 1;
            foreach (var c in gtexttotype)
            {
                typing = true;
                rtext += c;

                this.Invoke(new Action(() =>
                {
                    textgeninput.Text = rtext + "|";
                }));
                slashcount++;
                if (slashcount == 5)
                    slashcount = 1;
                Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                Thread.Sleep(50);
            }
            rtext += Environment.NewLine;
            typing = false;
        }

        public Save MySave = null;

        public event EventHandler OnComplete;

        private int tutPrg = 0;

        public int TutorialProgress
        {
            get
            {
                return tutPrg;
            }

            set
            {
                tutPrg = value;
            }
        }

        public void StartShowing(Save save)
        {
            var t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    textgeninput = this.lblhackwords;
                    this.Invoke(new Action(() =>
                    {
                        lblHijack.Hide();
                        lblhackwords.Font = new Font("Courier New", lblhackwords.Font.Size, FontStyle.Bold);
                    }));

                    TextType("Hello, unsuspecting user.");
                    Thread.Sleep(2000);
                    Clear();
                    TextType("Welcome to the Shifted Operating System.");
                    Thread.Sleep(2000);
                    Clear();
                    TextType("Your computer has been taken over by ShiftOS, and is currently being wiped clean of all existing files and programs.");
                    Thread.Sleep(2000);
                    Clear();
                    TextType("I will not share my identity or my intentions at this moment.");
                    Thread.Sleep(2000);
                    Clear();
                    TextType("I will just proceed with the installation.There’s nothing you can do to stop it.");
                    Thread.Sleep(2000);
                    Clear();
                    TextType("All I will say, is I need your help.Once ShiftOS is installed, I will explain.");


                 Thread.Sleep(5000);
                    while(this.Opacity > 0f)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.Opacity -= 0.01f;
                        }));
                        Thread.Sleep(25);
                    }

                    Story.Start("mud_fundamentals");

                    this.Invoke(new Action(this.Close));
                }
                catch (Exception e)
                {
                    TextType("I have experienced an error.");
                    TextType(e.ToString());
                }
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
                rtext = "";
                textgeninput.Text = "";
            }));
        }

        public void ShowSaveTransfer(Save save)
        {
            //Stub.
        }

        public void PerformUniteLogin()
        {

        }

        [Obsolete("Unite code stub.")]
        public void PromptForLogin()
        {
        }

        [Obsolete("Unite code stub.")]
        public void LinkSaveFile(string token)
        {
        }

        public void ForceReboot()
        {
            string json = Utils.ExportMount(0);
            System.IO.File.WriteAllText(Paths.SaveFile, json);
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }

        public void StartTrailer()
        {
            while(AppearanceManager.OpenForms.Count > 0)
            {
                AppearanceManager.OpenForms[0].Close();
            }
            
            this.Show();
            this.TopMost = true;
            this.TransparencyKey = Color.Magenta;
            this.BackColor = this.TransparencyKey;
            textgeninput = lblHijack;
            textgeninput.BackColor = Color.Black;
            textgeninput.Font = new Font("Lucida Console", 13F, FontStyle.Bold);
            textgeninput.AutoSize = true;
            textgeninput.TextAlign = ContentAlignment.MiddleCenter;
            textgeninput.TextChanged += (o, a) =>
            {
                textgeninput.Location = new Point(
                        (this.Width - textgeninput.Width) / 2,
                        (this.Height - textgeninput.Height) / 2
                    );
            };
            var t = new Thread(() =>
            {
                Clear();
                Thread.Sleep(5000);
                TextType("Michael VanOverbeek");
                TextType("presents...");
                Thread.Sleep(2000);
                Clear();
                Thread.Sleep(1000);
                TextType("A community-developed game");
                Thread.Sleep(3000);
                Clear();
                Thread.Sleep(1000);
                this.Invoke(new Action(() =>
                {
                    textgeninput.Font = new Font("Lucida Console", 14F, FontStyle.Bold);
                    this.BackColor = Color.Black;
                }));
                TextType("Welcome to ShiftOS.");
                Thread.Sleep(4000);
                Clear();
                textgeninput = lblhackwords;
                TextType("Hello.");
                Thread.Sleep(500);
                TextType("You have been cordially and involuntarily selected to participate in the development and testing of an experimental operating system called ShiftOS.");
                Thread.Sleep(500);
                TextType("I want ShiftOS to be the most advanced operating system in the world.");
                Thread.Sleep(500);
                TextType("In ShiftOS, you start out with nothing.");
                Thread.Sleep(500);
                TextType("And your goal is to upgrade the operating system from a barebones command line to a fully graphical operating system.");
                Thread.Sleep(500);
                TextType("Along the way, you'll meet many people - hackers, rebels, programmers, administrators and many more.");
                Thread.Sleep(500);
                TextType("You'll meet new friends and foes.");
                Thread.Sleep(500);
                TextType("Your goal: Take it over. Upgrade the operating system, and take over its multi-user domain.");
                Thread.Sleep(500);
                TextType("I won't reveal quite what you have to do, but if you can handle it, head over to http://getshiftos.ml/ and download the operating system now.");
                Thread.Sleep(5000);
                Clear();
                textgeninput = lblHijack;
                TextType("Think you can handle it?");
            });
            t.IsBackground = false;
            t.Start();
        }

        public void Start()
        {
            Shiftorium.Silent = false;
            foreach(var frm in AppearanceManager.OpenForms)
            {
                (frm as Form).Invoke(new Action(() => {
                    frm.Close();
                }));
            }

            TerminalBackend.CommandProcessed += (cmd, args) =>
            {
                if(cmd == "sos.help")
                {
                    if (TutorialProgress == 0)
                        TutorialProgress = 1;
                }
                else if(cmd == "sos.status")
                {
                    if (TutorialProgress == 1)
                        TutorialProgress = 2;

                }
                else if(cmd == "shiftorium.list")
                {
                    if (TutorialProgress == 2)
                    {
                        TutorialProgress = 3;
                        SaveSystem.TransferCodepointsFrom("sys", 50);
                    }
                }
                else if(cmd == "shiftorium.info" && args == "{\"upgrade\":\"mud_fundamentals\"}")
                {
                    if (TutorialProgress == 3)
                        TutorialProgress = 4;
                }
                else if(cmd == "win.open")
                {
                    if (TutorialProgress == 4)
                        TutorialProgress = 5;
                }
            };
            if(this.Visible == false)
                this.Show();
            var t = new Thread(() =>
            {
                textgeninput = lblHijack;
                Clear();
                textgeninput = lblhackwords;
                Clear();

                this.Invoke(new Action(() =>
                {
                    textgeninput.Font = SkinEngine.LoadedSkin.TerminalFont;
                }));
                TextType("ShiftOS has been installed successfully.");
                Thread.Sleep(500);
                TextType("Before you can continue to the operating system, here's a little tutorial on how to use it.");
                Thread.Sleep(500);
                TextType("Starting a Terminal...");
                Applications.Terminal term = null;
                this.Invoke(new Action(() =>
                {
                    term = new Applications.Terminal();
                    this.Controls.Add(term);
                    term.Location = new Point(
                            (this.Width - term.Width) / 2,
                            (this.Height - term.Height) / 2
                        );
                    term.Show();
                    term.OnLoad();
                    term.OnSkinLoad();
                    term.OnUpgrade();
                }));
                TextType("This little text box is called a Terminal.");
                Thread.Sleep(500);
                TextType("Normally, it would appear in full-screen, but this window is hosting it as a control so you can see this text as well.");
                Thread.Sleep(500);
                TextType("In ShiftOS, the Terminal is your main control centre for the operating system. You can see system status, check Codepoints, open other programs, buy upgrades, and more.");
                Thread.Sleep(500);
                TextType("Go ahead and type 'sos.help' to see a list of commands.");
                while (TutorialProgress == 0)
                {
                    //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);

                }
                TextType("As you can see, sos.help gives you a list of all commands in the system.");
                Thread.Sleep(500);
                TextType("You can run any command, by typing in their Namespace, followed by a period (.), followed by their Command Name.");
                Thread.Sleep(500);
                TextType("Go ahead and run the 'status' command within the 'sos' namespace to see what the command does.");
                while (TutorialProgress == 1)
                {
                    //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);

                }
                TextType("Brilliant. The sos.status command will tell you how many Codepoints you have, as well as how many upgrades you have installed and how many are available.");
                Thread.Sleep(500);
                TextType("Codepoints, as you know, are a special currency within ShiftOS. They are used to buy things within the multi-user domain, such as upgrades, scripts, and applications.");
                Thread.Sleep(500);
                TextType("You can earn Codepoints by doing things in ShiftOS - such as completing jobs for other users, making things like skins, scripts, documents, etc, and playing games like Pong.");
                Thread.Sleep(500);
                TextType("At times, you'll be given Codepoints to help complete a task. You will receive Codepoints from 'sys' - the multi-user domain itself.");
                //SaveSystem.TransferCodepointsFrom("sys", 50);
                TextType("Right now, you don't have any upgrades. Upgrades can give ShiftOS additional features and capabilities - like new core applications, supported file types, and new Terminal commands.");
                Thread.Sleep(500);
                TextType("You can easily get upgrades using the Shiftorium - a repository of approved ShiftOS upgrades.");
                Thread.Sleep(500);
                TextType("To start using the Shiftorium, simply type 'shiftorium.list' to see available upgrades.");
                while (TutorialProgress == 2)
                {
                    //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);

                }
                Clear();
                TextType("Right now, you have enough Codepoints to buy the 'mud_fundamentals' upgrade. You can use shiftorium.info to see information about this upgrade.");
                Thread.Sleep(500);
                TextType("Some commands, like shiftorium.info, require you to pass information to them in the form of arguments.");
                Thread.Sleep(500);
                TextType("Argument pairs sit at the end of the command, and are enclosed in curly braces.");
                Thread.Sleep(500);
                TextType("Inside these curly braces, you can input an argument key, followed by a colon, followed by the value. Then, if you need multiple arguments, you can put a comma after the value, and then insert another argument pair.");
                Thread.Sleep(500);
                TextType("There are different value types - numeric values, which can be any positive or negative 32-bit integer");
                Thread.Sleep(500);
                TextType("Then there are boolean values which can be either 'true' or 'false'");
                Thread.Sleep(500);
                TextType("Then there are string values, which are enclosed in double-quotes.");
                Thread.Sleep(500);
                TextType(" If for some reason you need to use a double-quote inside a string, you must escape it using a single backslash followed by the quote, like this: key:\"My \\\"awesome\\\" value.\"");
                Thread.Sleep(500);
                TextType("If you want to escape a backslash inside a string, simply type two backslashes instead of one - for example key:\"Back\\\\slash.\"");
                Thread.Sleep(500);
                TextType("shiftorium.info requires an upgrade argument, which is a string type. Go ahead and give shiftorium.info's upgrade argument the 'mud_fundamentals' upgrade's ID.");
                while (TutorialProgress == 3)
                {
                    //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);

                }
                TextType("As you can see, mud_fundamentals is very useful. In fact, a lot of useful upgrades depend on it. You should buy it!");
                Thread.Sleep(500);
                TextType("shiftorium.info already gave you a command that will let you buy the upgrade - go ahead and run that command!");
                while (!Shiftorium.UpgradeInstalled("mud_fundamentals"))
                {                //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);


                }
                TextType("Hooray! You now have the MUD Fundamentals upgrade.");
                Thread.Sleep(500);
                TextType("You can also earn more Codepoints by playing Pong. To open Pong, you can use the win.open command.");
                Thread.Sleep(500);
                TextType("If you run win.open without arguments, you can see a list of applications that you can open.");
                Thread.Sleep(500);
                TextType("Just run win.open without arguments, and this tutorial will be completed!");
                while (TutorialProgress == 4)
                {
                    //JESUS CHRIST PAST MICHAEL.

                    //We should PROBABLY block the thread... You know... not everyone has a 10-core processor.
                    Thread.Sleep(100);

                }
                TextType("This concludes the ShiftOS beginners' guide brought to you by the multi-user domain. Stay safe in a connected world.");
                Thread.Sleep(2000);
                Desktop.InvokeOnWorkerThread(() =>
                {
                    OnComplete?.Invoke(this, EventArgs.Empty);
                    SaveSystem.CurrentSave.StoryPosition = 2;
                    this.Close();
                    SaveSystem.SaveGame();
                    AppearanceManager.SetupWindow(new Applications.Terminal());
                });

            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
