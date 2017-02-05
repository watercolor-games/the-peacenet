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
    public partial class Oobe : Form, IOobe, ITutorial
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

        private bool typing = false;
        
        public void TextType(string texttotype)
        {
            while(typing == true)
            {

            }
            
            charcount = texttotype.Length;
            gtexttotype = texttotype;
            currentletter = 0;
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
                    while (fakeForm?.Visible == true)
                    {
                        Thread.Sleep(10);
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
                    SaveSystem.CurrentSave.StoryPosition = 1;
                    Utils.WriteAllText(Paths.GetPath("user.dat"), JsonConvert.SerializeObject(new
                    {
                        username = MySave.Username,
                        password = MySave.Password
                    }));
                    Shiftorium.Silent = true;
                    SaveSystem.SaveGame();
                    
                    Thread.Sleep(3000);
                    this.Invoke(new Action(() =>
                    {
                        TutorialManager.StartTutorial();
                    }));
                }
                catch(Exception e)
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
            this.Show();
            var fSetup = new FakeSetupScreen(this, 7);

            var t = new Thread(() =>
            {
                textgeninput = lblhackwords;
                Clear();
                TextType("Welcome back to ShiftOS.");
                Thread.Sleep(500);
                TextType("Since your last time inside ShiftOS, the operating system has changed. Your user account is no longer stored on your local system.");
                Thread.Sleep(500);
                this.Invoke(new Action(() =>
                {
                    //UPS is drunky heaven over here... it's a liquor store, I think... - Drunk Michael
                    fSetup.UserReregistered += (u, p, s) =>
                    {
                        save.Username = u;
                        save.Password = p;
                        save.SystemName = s;
                        SaveSystem.CurrentSave = save;
                        SaveSystem.CurrentSave.Upgrades = new Dictionary<string, bool>();
                        Shiftorium.Init();

                        SaveSystem.SaveGame();
                        if(Utils.FileExists(Paths.SaveFileInner))
                            Utils.Delete(Paths.SaveFileInner);
                        this.Close();
                    };
                    fSetup.Show();
                }));
            });
            t.IsBackground = true;
            t.Start();
        }

        public void PromptForLogin()
        {
            this.Show();
            this.TopMost = true;
            lblHijack.Text = "";
            textgeninput = lblhackwords;

            var fsw = new FakeSetupScreen(this, 10);
            fsw.Show();
            fsw.TopMost = true;
            fsw.DoneLoggingIn += () =>
            {
                this.Close();
            };
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
                frm.Close();
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
                        TutorialProgress = 3;
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
                while(TutorialProgress == 0)
                {

                }
                TextType("As you can see, sos.help gives you a list of all commands in the system.");
                Thread.Sleep(500);
                TextType("You can run any command, by typing in their Namespace, followed by a period (.), followed by their Command Name.");
                Thread.Sleep(500);
                TextType("Go ahead and run the 'status' command within the 'sos' namespace to see what the command does.");
                while(TutorialProgress == 1)
                {

                }
                TextType("Brilliant. The sos.status command will tell you how many Codepoints you have, as well as how many upgrades you have installed and how many are available.");
                Thread.Sleep(500);
                TextType("Codepoints, as you know, are a special currency within ShiftOS. They are used to buy things within the multi-user domain, such as upgrades, scripts, and applications.");
                Thread.Sleep(500);
                TextType("You can earn Codepoints by doing things in ShiftOS - such as completing jobs for other users, making things like skins, scripts, documents, etc, and playing games like Pong.");
                Thread.Sleep(500);
                TextType("At times, you'll be given Codepoints to help complete a task. You will receive Codepoints from 'sys' - the multi-user domain itself.");
                SaveSystem.TransferCodepointsFrom("sys", 50);
                TextType("Right now, you don't have any upgrades. Upgrades can give ShiftOS additional features and capabilities - like new core applications, supported file types, and new Terminal commands.");
                Thread.Sleep(500);
                TextType("You can easily get upgrades using the Shiftorium - a repository of approved ShiftOS upgrades.");
                Thread.Sleep(500);
                TextType("To start using the Shiftorium, simply type 'shiftorium.list' to see available upgrades.");
                while(TutorialProgress == 2)
                {

                }
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
                TextType("Then there are string values, which are enclosed in double-quotes. If for some reason you need to use a double-quote inside a string, you must escape it using a single backslash followed by the quote, like this: key:\"My \\\"awesome\\\" value.\"");
                Thread.Sleep(500);
                TextType("If you want to escape a backslash inside a string, simply type two backslashes instead of one - for example key:\"Back\\\\slash.\"");
                Thread.Sleep(500);
                TextType("shiftorium.info requires an upgrade argument, which is a string type. Go ahead and give shiftorium.info's upgrade argument the 'mud_fundamentals' upgrade's ID.");
                while(TutorialProgress == 3)
                {

                }
                TextType("As you can see, mud_fundamentals is very useful. In fact, a lot of useful upgrades depend on it. You should buy it!");
                Thread.Sleep(500);
                TextType("shiftorium.info already gave you a command that will let you buy the upgrade - go ahead and run that command!");
                while (!Shiftorium.UpgradeInstalled("mud_fundamentals"))
                {

                }
                TextType("Hooray! You now have the MUD Fundamentals upgrade.");
                Thread.Sleep(500);
                TextType("You can also earn more Codepoints by playing Pong. To open Pong, you can use the win.open command.");
                Thread.Sleep(500);
                TextType("If you run win.open without arguments, you can see a list of applications that you can open.");
                Thread.Sleep(500);
                TextType("Just run win.open without arguments, and this tutorial will be completed!");
                while(TutorialProgress == 4)
                {

                }
                TextType("This concludes the ShiftOS beginners' guide brought to you by the multi-user domain. Stay safe in a connected world.");
                Thread.Sleep(2000);
                this.Invoke(new Action(() =>
                {
                    OnComplete?.Invoke(this, EventArgs.Empty);
                    this.Close();
                    SaveSystem.CurrentSave.StoryPosition = 2;
                    SaveSystem.SaveGame();
                    AppearanceManager.SetupWindow(new Applications.Terminal());
                }));
            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
