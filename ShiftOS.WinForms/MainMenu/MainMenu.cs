using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

            var tickermove = new Timer();
            var tickerreset = new Timer();
            tickermove.Tick += (o, a) =>
            {
                if(lbticker.Left <= (0 - lbticker.Width))
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

            flmenu.CenterParent();

            tickerreset.Start();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Tools.ControlManager.SetupControls(this);

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
                default:
                    return "Good God. We don't know what to put here.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Desktop.CurrentDesktop.Show();
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
    }
}
