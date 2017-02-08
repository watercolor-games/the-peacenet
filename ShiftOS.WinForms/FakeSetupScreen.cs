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

namespace ShiftOS.WinForms
{
    public partial class FakeSetupScreen : Form
    {
        private Oobe oobe = null;

        public Action<bool> MUDUserFound = null;

        public FakeSetupScreen(Oobe _oobe, int page = 0)
        {
            oobe = _oobe;
            InitializeComponent();
            currentPage = page;
            SetupUI();
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "mud_notfound")
                    this.Invoke(new Action(() => { MUDUserFound?.Invoke(false); }));
                else if (msg.Name == "mud_found")
                    this.Invoke(new Action(() => { MUDUserFound?.Invoke(true); }));
            };
        }

        public event Action<string> TextSent;
        bool isTyping = false;

        private int currentPage = 0;

        public void SetupUI()
        {
            btnback.Show();
            pnlheader.Dock = DockStyle.Top;
            pnlheader.Height = 50;
            switch (currentPage)
            {
                case 0:
                    page1.BringToFront();
                    pnlheader.Dock = DockStyle.Left;
                    pnlheader.Width = 100;
                    btnback.Hide();
                    break;
                case 1:
                    btnnext.Hide();
                    btnback.Hide();
                    page2.BringToFront();
                    pgformatprogress.Value = 0;
                    var dinf = new System.IO.DriveInfo("C:");
                    StartWipingInBackground(((dinf.TotalSize / 1024) / 1024) / 1024);
                    TextType($@"So I see you're progressing through.
I really hope you aren't one of those newbies who just next-next-next-finish their way through these things.
I see you've named your drive " + dinf.VolumeLabel + $@"
And it can contain up to {dinf.TotalSize} bytes of information.
And you've formatted this drive with a partition of type " + dinf.DriveFormat + $@".
Interesting...
Very interesting...");
                    break;
                case 2:
                    btnnext.Show();
                    btnback.Hide();
                    TextType(@"Now it's all gone. Now I need some user input so I can install ShiftOS.
Firstly, please enter a username, password, and system name.
Make the username and system name unique so I can identify exactly who you are. You're not the only one here.
Also, make sure the password is secure... There have been people known to breach users' accounts in ShiftOS and steal their stuff.
So make sure your password is secure enough that it can't be guessed, but easy for you to remember, and don't put any personal information in your account.");
                    page3.BringToFront();
                    break;
                case 3:
                    if (string.IsNullOrEmpty(txtnewusername.Text))
                    {
                        TextType("You must specify a valid username!");
                        currentPage--;
                        //SetupUI();
                        break;
                    }

                    if (string.IsNullOrEmpty(txtnewpassword.Text))
                    {
                        TextType("A password would seriously be recommended.");
                        currentPage--;
                        //SetupUI();
                        break;
                    }

                    if (string.IsNullOrEmpty(txtnewsysname.Text))
                    {
                        TextType("You must name your computer.");
                        currentPage--;
                        //SetupUI();
                        break;
                    }


                    MUDUserFound = (val) =>
                    {
                        if(val == true)
                        {
                            TextType("I have just verified that your username and password already exists on my end. Please choose another.");
                            currentPage--;
                            //SetupUI();
                            
                        }
                        else
                        {
                            TextType("I am going to keep that info on my checklist for installing ShiftOS on your system. Now, onto some legal stuff. I highly suggest you read this.");
                            currentPage++;
                            oobe.MySave.Username = txtnewusername.Text;
                            oobe.MySave.Password = txtnewpassword.Text;
                            oobe.MySave.SystemName = txtnewsysname.Text;
                            SetupUI();
                        }
                    };
                    ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtnewusername.Text}"",
    password: ""{txtnewpassword.Text}""
}}");
                    break;
                case 4:
                    page4.BringToFront();
                    txtlicenseagreement.Rtf = Properties.Resources.ShiftOS;
                    break;
                case 5:
                    CanClose = true;
                    this.Close();
                    break;
                case 7:
                    btnnext.Show();
                    btnback.Hide();
                    pgrereg.BringToFront();
                    TextType("You have two choices - either you can migrate your local user file to this multi-user domain, or you can restart with 0 Codepoints, no upgrades, and still keep your files.");
                    break;
                case 8:
                    btnnext.Hide();
                    ServerMessageReceived rc = null;

                    rc = (msg) =>
                    {
                        if(msg.Name == "mud_found")
                        {
                            TextType("That username and password already exists in this multi-user domain. Please choose another.");
                            currentPage = 7;
                        }
                        else if(msg.Name == "mud_notfound")
                        {
                            currentPage = 9;
                            SetupUI();
                        }
                        ServerManager.MessageReceived -= rc;
                    };

                    if (string.IsNullOrEmpty(txtruname.Text))
                    {
                        TextType("You must provide a username.");
                        currentPage = 7;
                    }

                    if (string.IsNullOrEmpty(txtrpass.Text))
                    {
                        TextType("You must provide a password.");
                        currentPage = 7;
                    }

                    if (string.IsNullOrEmpty(txtrsys.Text))
                    {
                        TextType("You must provide a system hostname.");
                        currentPage = 7;
                    }

                    if (currentPage == 7)
                        return;

                    ServerManager.MessageReceived += rc;
                    ServerManager.SendMessage("mud_checkuserexists", $@"{{
    username: ""{txtruname.Text}"",
    password: ""{txtrpass.Text}""
}}");
                    break;
                case 9:
                    UserReregistered?.Invoke(txtruname.Text, txtrpass.Text, txtrsys.Text);
                    this.CanClose = true;
                    this.Close();
                    break;
                case 10:
                    btnnext.Show();
                    btnback.Hide();
                    pglogin.BringToFront();
                    break;
                case 11:
                    btnnext.Hide();
                    ServerMessageReceived login = null;

                    login = (msg) =>
                    {
                        if (msg.Name == "mud_found")
                        {
                            this.Invoke(new Action(() =>
                            {
                                currentPage = 12;
                                SetupUI();
                            }));
                        }
                        else if (msg.Name == "mud_notfound")
                        {
                            this.Invoke(new Action(() =>
                            {
                                currentPage = 10;
                                SetupUI();
                            }));
                        }
                        ServerManager.MessageReceived -= login;
                    };

                    ServerManager.MessageReceived += login;
                    if(!string.IsNullOrWhiteSpace(txtluser.Text) && !string.IsNullOrWhiteSpace(txtlpass.Text))
                    {
                        ServerManager.SendMessage("mud_checkuserexists", JsonConvert.SerializeObject(new
                        {
                            username = txtluser.Text,
                            password = txtlpass.Text
                        }));
                    }
                    break;
                case 12:
                    btnnext.Hide();
                    ServerMessageReceived getsave = null;

                    getsave = (msg) =>
                    {
                        if (msg.Name == "mud_found")
                        {
                            this.Invoke(new Action(() =>
                            {
                                currentPage = 12;
                                SetupUI();
                            }));
                        }
                        else if (msg.Name == "mud_notfound")
                        {
                            this.Invoke(new Action(() =>
                            {
                                currentPage = 10;
                                SetupUI();
                            }));
                        }
                        ServerManager.MessageReceived -= getsave;
                    };

                    ServerManager.MessageReceived += getsave;

                    ServerManager.SendMessage("mud_login", JsonConvert.SerializeObject(new
                    {
                        username = txtluser.Text,
                        password = txtlpass.Text
                    }));

                    DoneLoggingIn?.Invoke();
                    this.CanClose = true;
                    this.Close();
                    break;
            }
        }

        public event Action DoneLoggingIn;

        public event Action<string, string, string> UserReregistered;

        public void StartWipingInBackground(long arbitraryAmountOfBytes)
        {
            pgformatprogress.Maximum = (int)arbitraryAmountOfBytes;
            var t = new Thread(() =>
            {
                for(long i = 0; i <= arbitraryAmountOfBytes; i++)
                {
                    Thread.Sleep(40);
                    this.Invoke(new Action(() =>
                    {
                        pgformatprogress.Value = (int)i;
                        lbbyteszeroed.Text = $"Gigabytes zeroed: {i}/{arbitraryAmountOfBytes}";
                    }));
                }
                this.Invoke(new Action(() =>
                {
                    currentPage++;
                    SetupUI();
                }));
            });
            t.IsBackground = true;
            t.Start();
        }

        

        private void TextType(string text)
        {
            new Thread(() =>
            {
                while(isTyping == true)
                {

                }
                isTyping = true;
                TextSent?.Invoke(text);
                isTyping = false;
            }).Start();
        }

        bool CanClose = false;

        private void FakeSetupScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CanClose)
            {

            }
            else
            {
                e.Cancel = true;
                TextType("Don't try to close the dialog. It's a futile attempt.");
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            currentPage--;
            SetupUI();
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            currentPage++;
            SetupUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShiftOS.Objects.ShiftFS.Utils.Delete(Paths.GetPath("user.dat"));
            System.IO.File.WriteAllText(Paths.SaveFile, ShiftOS.Objects.ShiftFS.Utils.ExportMount(0));
            SaveSystem.NewSave();
            this.CanClose = true;
            this.Close();
        }
    }
}
