using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    // Behold... The ShiftOS program that doesn't use threading.
    [WinOpen("buttonmaster")]
    [Launcher("Button Master", false, "", "Games")]
    [AppscapeEntry("buttonmaster", "ButtonMaster", "Can you click all the buttons in 60 seconds?", 300, 10000)]
    public partial class ButtonMaster : UserControl, IShiftOSWindow
    {
        public ushort Buttons;
        public double Codepoints, Loss;
        private ulong RoundedCodepoints { get { return (ulong) Math.Round(Codepoints); } }
        public Timer clock = new Timer();
        private ButtonObject[] ButtonObjects;
        private static ButtonMasterDifficulty[] difficulties = new ButtonMasterDifficulty[]
        {
            new ButtonMasterDifficulty("Easy", 24, 500),
            new ButtonMasterDifficulty("Medium", 48, 1000),
            new ButtonMasterDifficulty("Hard", 100, 2000),
            new ButtonMasterDifficulty("Super Hard", 500, 10000)
        };
        public ButtonMaster()
        {
            InitializeComponent();
            difficultysel.Items.AddRange(difficulties);
            difficultysel.SelectedIndex = 0;
            clock.Interval = 20;
            clock.Tick += gameloop;
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {

        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {

        }

        private void quitbtn_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }

        private void playbtn_Click(object sender, EventArgs e)
        {
            var difficulty = difficultysel.SelectedItem as ButtonMasterDifficulty;
            Buttons = difficulty.Buttons;
            Codepoints = difficulty.Codepoints;
            Loss = difficulty.Codepoints / 3000.0;
            startpnl.Hide();
            scorecounter.Text = difficulty.Codepoints.ToString();
            scorecounter.Show();
            ButtonObjects = new ButtonObject[Buttons];
            for (ushort i = 0; i < Buttons; i++)
                ButtonObjects[i] = new ButtonObject(this);
            Controls.AddRange(ButtonObjects.Select(b => b.ctl).ToArray());
            clock.Start();
        }

        private void StopGame(string text)
        {
            desclabel.Text = text;
            startpnl.Show();
            clock.Stop();
            scorecounter.Hide();
            foreach (var b in ButtonObjects)
            {
                clock.Tick -= b.move;
                Controls.Remove(b.ctl);
            }
        }

        private void gameloop(object sender, EventArgs e)
        {
            if (Codepoints <= 0)
            {
                Codepoints = 0;
                string text = String.Format("You lost. There were {0} buttons to go.", Buttons);
                if (difficultysel.SelectedIndex > 0)
                    text += " Try an easier difficulty?";
                StopGame(text);
            }
            else if (Buttons <= 0)
            {
                SaveSystem.CurrentSave.Codepoints += RoundedCodepoints;
                string text = String.Format("You won! {0} Codepoints have been transferred to your system.", RoundedCodepoints);
                StopGame(text);
            }
            else
            {
                Codepoints -= Loss;
                scorecounter.Text = RoundedCodepoints.ToString();
            }
        }
    }
    public class ButtonMasterDifficulty
    {
        public string Name { get; private set; }
        public ushort Buttons { get; private set; }
        public uint Codepoints { get; private set; }
        public string Display { get; private set; }
        public override string ToString()
        {
            return Display;
        }
        public ButtonMasterDifficulty(string in_Name, ushort in_Buttons, uint in_Codepoints)
        {
            Name = in_Name;
            Buttons = in_Buttons;
            Codepoints = in_Codepoints;
            Display = String.Format("{0} ({1} Buttons, {2} Codepoints)", Name, Buttons, Codepoints);
        }
    }

    public class ButtonObject
    {
        public Button ctl;
        public sbyte xspeed, yspeed;
        public ButtonMaster parent;
        private static Random rnd = new Random();
        public void move(object sender, EventArgs e)
        {
            ctl.Top += yspeed;
            if (ctl.Top < 0 || ctl.Bottom > parent.Height)
                yspeed = (sbyte)(Math.Abs(yspeed) * -(ctl.Top / Math.Abs(ctl.Top))); // Change the sign of yspeed to be the opposite of that of ctl.Top
            ctl.Left += xspeed;
            if (ctl.Left < 0 || ctl.Right > parent.Width)
                xspeed = (sbyte)(Math.Abs(xspeed) * -(ctl.Left / Math.Abs(ctl.Left)));
        }
        public void click(object sender, EventArgs e)
        {
            parent.Buttons--;
            ctl.Hide();
            parent.clock.Tick -= move;
        }
        public ButtonObject(ButtonMaster in_parent)
        {
            xspeed = (sbyte) rnd.Next(10, 20);
            yspeed = (sbyte) rnd.Next(10, 20);
            parent = in_parent;
            parent.clock.Tick += move;
            ctl = new Button();
            ctl.Click += click;
            ctl.Top = rnd.Next(parent.Height);
            ctl.Left = rnd.Next(parent.Width);
            ctl.Width = 75;
            ctl.Height = 23;
            ctl.Text = "Click";
            // Time to invoke a private method using reflection. What could possibly go wrong?
            // This stops the player from just holding down space to beat the level.
            typeof(Button).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(ctl, new object[] { ControlStyles.Selectable, false });
        }

    }
}
