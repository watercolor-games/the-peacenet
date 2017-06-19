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
        private void StartGame()
        {
            new Loading().Show();
        }

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
            CategoryText = Localization.Parse("{MAINMENU_TITLE}");
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
            CategoryText = Localization.Parse("{MAINMENU_TITLE}");

        }

        private Random rnd = new Random();

        private string GetTickerMessage()
        {
            return Localization.Parse("{MAINMENU_TIPTEXT_" + rnd.Next(10) + "}");
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
            CategoryText = Localization.Parse("{MAINMENU_CAMPAIGN}");

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (Desktop.CurrentDesktop as WinformsDesktop).IsSandbox = true;
            StartGame();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var conf = ShiftOS.Objects.UserConfig.Get();

            txtdsaddress.Text = conf.DigitalSocietyAddress;
            txtdsport.Text = conf.DigitalSocietyPort.ToString();

            cblanguage.Items.Clear();
            foreach(var lang in Localization.GetAllLanguages())
            {
                var finf = new System.IO.FileInfo(lang);
                int nameindex = finf.Name.Length - 5;
                cblanguage.Items.Add(finf.Name.Remove(nameindex, 5));
            }

            cblanguage.Text = conf.Language;

            pnloptions.Show();
            pnloptions.BringToFront();
            pnloptions.CenterParent();
            currentMenu = pnloptions;
            CategoryText = Localization.Parse("{GEN_SETTINGS}");

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

            if(int.TryParse(txtdsport.Text, out p) == false || p < 0 || p > 65535)
            {
                Infobox.Show("{TITLE_INVALIDPORT}", "{PROMPT_INVALIDPORT}");
                return;
            }

            conf.DigitalSocietyPort = p;

            bool requiresRestart = (conf.Language != cblanguage.Text);
            conf.Language = cblanguage.Text;
            

            System.IO.File.WriteAllText("servers.json", Newtonsoft.Json.JsonConvert.SerializeObject(conf, Newtonsoft.Json.Formatting.Indented));

            HideOptions();
            if(requiresRestart == true)
            {
                Infobox.Show("{TITLE_RESTARTREQUIRED}", "{PROMPT_RESTARTREQUIRED}", () =>
                {
                    Application.Restart();
                });
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            flcampaign.Hide();
            flmenu.Show();
            flmenu.BringToFront();
            flmenu.CenterParent();
            currentMenu = flmenu;
            CategoryText = Localization.Parse("{MAINMENU_TITLE}");
        }

        private void btncontinue_Click(object sender, EventArgs e)
        {
            StartGame();

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
                        StartGame();
                    }
                });
            }
            else
            {
                StartGame();
            }
        }
    }
}
