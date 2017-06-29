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
using Microsoft.VisualBasic;

namespace ShiftOS.WinForms.Applications
{
    [AppscapeEntry("dodge", "Dodge", "Dodge falling objects and stay alive as long as you can. Play this fun and exciting game while earning code points!", 500, 1590, "", "Games")]
    [WinOpen("dodge")]
    [Launcher("Dodge", false, "", "Games")]
    public partial class Dodge : UserControl, IShiftOSWindow
    {
        decimal speed; //the speed the game runs at
        int score; //the score/code points the player gets
        bool usingkeys = false; //user can use mouse or keyboard, mouse by default, chnages to true if key pressed
        decimal time = 0; //Records the time spent playing, used for codepoints formula
        int bonusesfound; //Number or bonus play collects
        int keyboardinput = 0; //for smooth keyboard gameplay, 1=left, 2=right, 0=none
        Point LocalMousePosition = new Point(0, 0); // the position of the mouse relative to our control
        public Dodge()
        {
            InitializeComponent();
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

        private void QuitButton_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this); //quits the game (In case user donsn't have close button)
        }

        private void Dodge_KeyUp(object sender, KeyEventArgs e)
        {
            keyboardinput = 0;
        }

        public void OnLoad()
        {
            player.Visible = false; // hide player and score until game starts
            scorelabel.Visible = false;
            DescriptionLabel.Text = "Welcome to Dodge. Dodge is a simple arcade game with one objective: survive the falling objects! Use the arrow or mouse to move the player and avoid as many objects as you can. The longer you survive, the more code point you will be rewarded with. Beware, it gets harder..."; // set the description text

            //to impliment skinning, simply set the picturebox to the new skinned image.
            //For example:
            //player.Image = Image.FromFile("PATH TO SKINNED IMAGE");
        }

        private void BeginButton_Click(object sender, EventArgs e)
        {

            //Hide buttons
            BeginButton.Visible = false;
            QuitButton.Visible = false;
            DescriptionLabel.Visible = false;

            player.Visible = true; // show the player
            speed = 2; // controls speed of game, will increase as game progresses
            scorelabel.Visible = true; // show score label
            bonusesfound = 0;

            //Make sure all objects are in the correct position
            object_small.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), 300);
            object_small2.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), 49);
            object_mid.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), 377);
            object_mid2.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), 140);
            object_large.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), 214);
            PicBonus.Location = new Point(((int)Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);

            //Reset time
            time = 0;

            usingkeys = false;

            System.Threading.Thread.Sleep(100); // slight delay before game starts (in milliseconds)

            main(); // start the main game function

            //sig() //infobox sigs -COMMENT THIS OUT
        }

        private void Dodge_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: // detect right key press
                    usingkeys = true; // turn off mouse control
                    keyboardinput = 2;
                    break;
                case Keys.Left:
                    usingkeys = true;
                    keyboardinput = 1;
                    break;
            }

        }

        private void main()
        {
            clock.Start(); //the timer restart this sub every tick, making an endless loop between them.

            //score system
            scorelabel.Text = score.ToString();
            score = (int)((speed / 10) + (time / 20) + bonusesfound);

            //Speed increase
            speed = speed + (speed * (decimal)0.001);

            //increase time
            time = time + (decimal)0.05; //loops every 0.05 seconds so time increases by 1 every second (have I done the maths correctly?)

            //Make objects fall
            object_large.Location = new Point(object_large.Location.X, (int)(object_large.Location.Y + speed));
            object_mid.Location = new Point(object_mid.Location.X, (int)(object_mid.Location.Y + speed));
            object_mid2.Location = new Point(object_mid2.Location.X, (int)(object_mid2.Location.Y + speed));
            object_small.Location = new Point(object_small.Location.X, (int)(object_small.Location.Y + speed));
            object_small2.Location = new Point(object_small2.Location.X, (int)(object_small2.Location.Y + speed));

            //mouse controls
            if (!usingkeys) // tests if mouse control is enabled
            {
                player.Left = LocalMousePosition.X - (player.Width / 2) - 5; //sets the x location to that of the mouse
            }
            //keyboard controls
            if (usingkeys)
            {
                if (keyboardinput == 1)
                    player.Location = new Point((int)(player.Location.X - (speed * 4)), player.Location.Y);
                if (keyboardinput == 2)
                    player.Location = new Point((int)(player.Location.X + (speed * 4)), player.Location.Y); // move right
            }

            //move object back to the top of the screen
            if (object_small.Location.Y > 522)
                object_small.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20); //picks a random number between 0 and the window width and sets the x position to this value. uses -20 for y as it is above the top of window
            if (object_small2.Location.Y > 522)
                object_small2.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);
            if (object_mid.Location.Y > 522)
                object_mid.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);
            if (object_mid2.Location.Y > 522)
                object_mid2.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);
            if (object_large.Location.Y > 522)
                object_large.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);

            //Makes sure the player is on the screen (Anti-cheating)
            if (player.Location.X > pgcontents.Width)
                player.Location = new Point(385, player.Location.Y);
            if (player.Location.X < 0)
                player.Location = new Point(0, player.Location.Y);

            //Bonus
            if (!PicBonus.Visible)
            {
                int ran = (int)Math.Ceiling(VBMath.Rnd() * 300); //random 1 in 500 chance
                if (ran == 1)
                    PicBonus.Visible = true;
            }
            else
            {
                PicBonus.Location = new Point(PicBonus.Location.X, (int)(PicBonus.Location.Y + speed));
                if (PicBonus.Location.Y > 522)
                    PicBonus.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);
                PicBonus.Visible = false;
            }

            //check collisions
            if (player.Bounds.IntersectsWith(object_mid.Bounds) || player.Bounds.IntersectsWith(object_mid2.Bounds) || player.Bounds.IntersectsWith(object_large.Bounds) || player.Bounds.IntersectsWith(object_small.Bounds) || player.Bounds.IntersectsWith(object_small2.Bounds))
            {
                clock.Stop(); //breaks loop
                System.Threading.Thread.Sleep(333); //delay for a third of a second
                player.Visible = false; //hide game
                DescriptionLabel.Text = "Sorry, you just lost the game, however, you earnt a total of " + score.ToString() + " code points. To earn more code points, press the begin button now. To exit, press the quit button"; // change the description to the die message
                SaveSystem.CurrentSave.Codepoints += (ulong) score;
                DescriptionLabel.Visible = true; //show non-game elements
                BeginButton.Visible = true;
                QuitButton.Visible = true;
                scorelabel.Visible = false;
            }
            if (player.Bounds.IntersectsWith(PicBonus.Bounds))
            {
                PicBonus.Visible = false;
                bonusesfound = bonusesfound + 1;
                PicBonus.Location = new Point((int)(Math.Ceiling(VBMath.Rnd() * pgcontents.Width)), -20);
            }

        }
        private void clock_Tick(object sender, EventArgs e)
        {
            main(); //repeat the main function (endless loop)
        }

        private void sig()
        {
            Infobox.Show("FLAG", "There is no foul on the play, the punt was blocked.");
        }

        private void Dodge_MouseMove(object sender, MouseEventArgs e)
        {
            LocalMousePosition = e.Location;
        }

        private void pgcontents_MouseMove(object sender, MouseEventArgs e)
        {
            LocalMousePosition = e.Location;
        }
    }
}
