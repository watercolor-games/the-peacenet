using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.MainMenu
{
    public partial class MainMenu : Form
    {
        public MainMenu(IDesktop desk)
        {
            InitializeComponent();
            (desk as WinformsDesktop).ParentMenu = this;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
#if DEBUG
            var asm = Assembly.GetExecutingAssembly();

            string asmName = asm.GetName().Name;
            string vstring = "";
            var version = asm.GetCustomAttributes(true).FirstOrDefault(x => x is AssemblyVersionAttribute) as AssemblyVersionAttribute;
            if (version != null)
                vstring = version.Version;
            lbbuilddetails.Text = $"{asmName} - version number: {vstring} - THIS IS AN UNSTABLE RELEASE.";

#else
            lbbuilddetails.Hide();
#endif
        }

        public void HideOptions()
        {
            pnloptions.Hide();
            flmenu.BringToFront();
            flmenu.CenterParent();
            currentMenu = flmenu;
            CategoryText = "Main menu";
        }

        public string CategoryText
        {
            get
            {
                return lbcurrentui.Text;
            }
            set
            {
                lbcurrentui.Text = value;
                lbcurrentui.CenterParent();
                lbcurrentui.Top = currentMenu.Top - (lbcurrentui.Height * 2);
            }
        }

        private Control currentMenu = null;

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Tools.ControlManager.SetupControls(this);
            shiftos.CenterParent();
            shiftos.Top = 35;

            
            var tickermove = new Timer();
            var tickerreset = new Timer();
            tickermove.Tick += (o, a) =>
            {
                if (lbticker.Left <= (0 - lbticker.Width))
                {
                    tickermove.Stop();
                    tickerreset.Start();
                }
                else
                {
                    lbticker.Top = (this.ClientSize.Height - (lbticker.Height * 2));
                    lbticker.Left -= 2;
                }
            };
            tickerreset.Tick += (o, a) =>
            {
                lbticker.Visible = false;
                lbticker.Text = GetTickerMessage();
                lbticker.Left = this.Width;
                lbticker.Visible = true;
                tickerreset.Stop();
                tickermove.Start();
            };

            tickermove.Interval = 1;
            tickerreset.Interval = 1000;

            pnloptions.Hide();
            flcampaign.Hide();
            flmenu.CenterParent();

            tickerreset.Start();

            currentMenu = flmenu;
            CategoryText = "Main menu";

        }

        private Random rnd = new Random();

        private string GetTickerMessage()
        {
            switch (rnd.Next(0, 10))
            {
                case 0:
                    return "Did you know that you can skin this very menu? Just goes to show how much you can shift it your way.";
                case 1:
                    return "Want to pick up a few skins or mods from the community? Head on over to http://getshiftos.ml/Skins!";
                case 2:
                    return "Sandbox mode is a special version of ShiftOS that allows you to use the operating system without having to deal with Codepoints, the Shiftorium or having to play through the storyline. Handy...";
                case 3:
                    return "ArtPad not good enough? You can use an external image editor to create ShiftOS skin textures. Just save your files to the Shared Directory and they'll be imported into ShiftOS on the 1:/ drive.";
                case 4:
                    return "Terminal too weird for ya? You can use the Format Editor to generate your own Terminal command parser. No coding knowledge needed!";
                case 5:
                    return "Contests are a good way to earn heaps of Codepoints. Head on over to http://getshiftos.ml/Contests for info on current community contests.";
                case 6:
                    return "There's no bugs in this game... But if you find some, please submit them to http://getshiftos.ml/Bugs.";
                case 7:
                    return "SHIFTOS - PROPERTY OF MICHAEL VANOVERBEEK. FOR INTERNAL USE ONLY. Build number = sos_tr_133764 [Just kidding. ShiftOS is open-source. Find the code at http://github.com/shiftos-game/ShiftOS!]";
                case 8:
                    return "Hold your Codepoints against the wall...           when they take everything away. Hold your Codepoints against the wall...";
                default:
                    return "Good God. We don't know what to put here.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(System.IO.File.Exists(System.IO.Path.Combine(Paths.SaveDirectory, "autosave.save")))
            {
                btncontinue.Show();
            }
            else
            {
                btncontinue.Hide();
            }
            flmenu.Hide();
            flcampaign.Show();
            flcampaign.BringToFront();
            flcampaign.CenterParent();
            currentMenu = flcampaign;
            CategoryText = "Campaign";

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (Desktop.CurrentDesktop as WinformsDesktop).IsSandbox = true;
            Desktop.CurrentDesktop.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var conf = ShiftOS.Objects.UserConfig.Get();

            txtubase.Text = conf.UniteUrl;
            txtdsaddress.Text = conf.DigitalSocietyAddress;
            txtdsport.Text = conf.DigitalSocietyPort.ToString();


            pnloptions.Show();
            pnloptions.BringToFront();
            pnloptions.CenterParent();
            currentMenu = pnloptions;
            CategoryText = "Settings";

        }

        private void opt_btncancel_Click(object sender, EventArgs e)
        {
            HideOptions();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            var conf = ShiftOS.Objects.UserConfig.Get();

            conf.DigitalSocietyAddress = txtdsaddress.Text;

            int p = 0;

            if(int.TryParse(txtdsport.Text, out p) == false)
            {
                Infobox.Show("Invalid port number", "The Digital Society Port must be a valid whole number between 0 and 65535.");
                return;
            }
            else
            {
                if(p < 0 || p > 65535)
                {
                    Infobox.Show("Invalid port number", "The Digital Society Port must be a valid whole number between 0 and 65535.");
                    return;
                }
            }

            conf.DigitalSocietyPort = p;

            string unite = txtubase.Text;
            if (unite.EndsWith("/"))
            {
                int len = unite.Length;
                int index = len - 1;
                int end = 1;
                unite = unite.Remove(index, end);
            }
            conf.UniteUrl = unite;

            System.IO.File.WriteAllText("servers.json", Newtonsoft.Json.JsonConvert.SerializeObject(conf, Newtonsoft.Json.Formatting.Indented));

            HideOptions();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            flcampaign.Hide();
            flmenu.Show();
            flmenu.BringToFront();
            flmenu.CenterParent();
            currentMenu = flmenu;
            CategoryText = "Main menu";
        }

        private void btncontinue_Click(object sender, EventArgs e)
        {
            Desktop.CurrentDesktop.Show();

        }

        private void btnnewgame_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.Combine(Paths.SaveDirectory, "autosave.save");
            if (System.IO.File.Exists(path))
            {
                Infobox.PromptYesNo("Campaign", "You are about to start a new game, which will erase any previous progress. Are you sure you want to do this?", (result) =>
                {
                    if (result == true)
                    {
                        System.IO.File.Delete(path);
                        Desktop.CurrentDesktop.Show();
                    }
                });
            }
            else
            {
                Desktop.CurrentDesktop.Show();
            }
        }
    }
}
