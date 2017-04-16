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
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("Pong", true, "al_pong", "Games")]
    [WinOpen("pong")]
    [DefaultIcon("iconPong")]
    public partial class Pong : UserControl, IShiftOSWindow
    {
        //I can assure you guaranteed that there is an acorn somewhere, in this place, and the sailors are looking for it
        int xVel = 7;
        int yVel = 8;
        int computerspeed = 8;
        int level = 1;
        int secondsleft = 60;
        int casualposition;
        double xveldec = 3.0;
        double yveldec = 3.0;
        double incrementx = 0.4;
        double incrementy = 0.2;
        int levelxspeed = 3;
        int levelyspeed = 3;
        int beatairewardtotal;
        int beataireward = 1;
        int[] levelrewards = new int[50];
        int totalreward;
        int countdown = 3;

        bool aiShouldIsbeEnabled = true;

        public Pong()
        {
            InitializeComponent();
        }

        private void Pong_Load(object sender, EventArgs e)
        {
            setuplevelrewards();
        }



        // Move the paddle according to the mouse position.
        private void pongMain_MouseMove(object sender, MouseEventArgs e)
        {
            var loc = this.PointToClient(MousePosition);
            paddleHuman.Location = new Point(paddleHuman.Location.X, (loc.Y) - (paddleHuman.Height / 2));
        }

        private void CenterPanels()
        {
            pnlfinalstats.CenterParent();
            pnlgamestats.CenterParent();
            pnlhighscore.CenterParent();
            pnlintro.CenterParent();
            pnllose.CenterParent();
            lblcountdown.CenterParent();
            lblbeatai.Left = (this.Width - lblbeatai.Width) / 2;
            SetupStats();
        }

        public void SetupStats()
        {
            lblstatsX.Location = new Point(5, this.Height - lblstatsX.Height - 5);
            lblstatsY.Location = new Point(this.Width - lblstatsY.Width - 5, this.Height - lblstatsY.Height - 5);
            lblstatscodepoints.Top = this.Height - lblstatscodepoints.Height - 5;
            lblstatscodepoints.Left = (this.Width - lblstatscodepoints.Width) / 2;
        }
        

        // ERROR: Handles clauses are not supported in C#
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (this.Left < Screen.PrimaryScreen.Bounds.Width)
            {
                ball.BackColor = SkinEngine.LoadedSkin.ControlTextColor;
                paddleComputer.BackColor = SkinEngine.LoadedSkin.ControlTextColor;
                paddleHuman.BackColor = SkinEngine.LoadedSkin.ControlTextColor;

                //Check if paddle upgrade has been bought and change paddles accordingly
                //if (ShiftoriumFrontend.UpgradeInstalled("pong_increased_paddle_size"))
                //{
                 //   paddleHuman.Height = 150;
                  //  paddleComputer.Height = 150;
                //}
                //I don't know the point of this but I'm fucking 86ing it. - Michael

                //Set the computer player to move according to the ball's position.
                if (aiShouldIsbeEnabled)
                    if (ball.Location.X > (this.Width - (this.Width / 3)) - xVel * 10 && xVel > 0)
                    {
                        if (ball.Location.Y > paddleComputer.Location.Y + 50)
                        {
                            paddleComputer.Location = new Point(paddleComputer.Location.X, paddleComputer.Location.Y + computerspeed);
                        }
                        if (ball.Location.Y < paddleComputer.Location.Y + 50)
                        {
                            paddleComputer.Location = new Point(paddleComputer.Location.X, paddleComputer.Location.Y - computerspeed);
                        }
                        casualposition = rand.Next(-150, 201);
                    }
                    else
                    {
                        //used to be me.location.y - except it's fucking C# and this comment is misleading as fuck. OH WAIT! I didn't write it! And none of the current devs did either! - Michael
                        if (paddleComputer.Location.Y > this.Size.Height / 2 - paddleComputer.Height + casualposition)
                        {
                            paddleComputer.Location = new Point(paddleComputer.Location.X, paddleComputer.Location.Y - computerspeed);
                        }
                        //Rylan is hot. Used to be //used to be me.location.y
                        if (paddleComputer.Location.Y < this.Size.Height / 2 - paddleComputer.Height + casualposition)
                        {
                            paddleComputer.Location = new Point(paddleComputer.Location.X, paddleComputer.Location.Y + computerspeed);
                        }
                    }

                //Set Xvel and Yvel speeds from decimal
                if (xVel > 0)
                    xVel = (int)Math.Round(xveldec);
                if (xVel < 0)
                    xVel = (int)-Math.Round(xveldec);
                if (yVel > 0)
                    yVel = (int)Math.Round(yveldec);
                if (yVel < 0)
                    yVel = (int)-Math.Round(yveldec);

                // Move the game ball.
                ball.Location = new Point(ball.Location.X + xVel, ball.Location.Y + yVel);

                // Check for top wall.
                if (ball.Location.Y < 0)
                {
                    ball.Location = new Point(ball.Location.X, 0);
                    yVel = -yVel;
                    ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.typesound);
                }

                // Check for bottom wall.
                if (ball.Location.Y > pgcontents.Height - ball.Height)
                {
                    ball.Location = new Point(ball.Location.X, pgcontents.Height - ball.Size.Height);
                    yVel = -yVel;
                    ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.typesound);
                }

                // Check for player paddle.
                if (ball.Bounds.IntersectsWith(paddleHuman.Bounds))
                {
                    ball.Location = new Point(paddleHuman.Location.X + ball.Size.Width + 1, ball.Location.Y);
                    //randomly increase x or y speed of ball
                    switch (rand.Next(1, 3))
                    {
                        case 1:
                            xveldec = xveldec + incrementx;
                            break;
                        case 2:
                            if (yveldec > 0)
                                yveldec = yveldec + incrementy;
                            if (yveldec < 0)
                                yveldec = yveldec - incrementy;
                            break;
                    }
                    xVel = -xVel;
                    ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.writesound);

                }

                // Check for computer paddle.
                if (ball.Bounds.IntersectsWith(paddleComputer.Bounds))
                {
                    ball.Location = new Point(paddleComputer.Location.X - paddleComputer.Size.Width - 1, ball.Location.Y);
                    xveldec = xveldec + incrementx;
                    xVel = -xVel;
                    ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                }

                // Check for left wall.
                if (ball.Location.X < -100)
                {
                    ball.Location = new Point(this.Size.Width / 2 + 200, this.Size.Height / 2);
                    paddleComputer.Location = new Point(paddleComputer.Location.X, ball.Location.Y);
                    if (xVel > 0)
                        xVel = -xVel;
                    pnllose.Show();
                    gameTimer.Stop();
                    counter.Stop();
                    lblmissedout.Text = Localization.Parse("{YOU_MISSED_OUT_ON}:") + Environment.NewLine + lblstatscodepoints.Text.Replace(Localization.Parse("{CODEPOINTS}: "), "") + Localization.Parse(" {CODEPOINTS}");
                    if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade_2"))
                    {
                        totalreward = levelrewards[level - 1] + beatairewardtotal;
                        double onePercent = (totalreward / 100);
                        lblbutyougained.Show();
                        lblbutyougained.Text = Localization.Parse("{BUT_YOU_GAINED}:") + Environment.NewLine + onePercent.ToString("") + (Localization.Parse(" {CODEPOINTS}"));
                        SaveSystem.TransferCodepointsFrom("pong", (totalreward / 100));
                    }
                    else
                    {
                        lblbutyougained.Hide();
                    }
                }

                // Check for right wall.
                if (ball.Location.X > this.Width - ball.Size.Width - paddleComputer.Width + 100)
                {
                    ball.Location = new Point(this.Size.Width / 2 + 200, this.Size.Height / 2);
                    paddleComputer.Location = new Point(paddleComputer.Location.X, ball.Location.Y);
                    if (xVel > 0)
                        xVel = -xVel;
                    beatairewardtotal = beatairewardtotal + beataireward;
                    lblbeatai.Show();
                    lblbeatai.Text = Localization.Parse($"{{PONG_BEAT_AI_REWARD_SECONDARY}}: {beataireward}");
                    tmrcountdown.Start();
                    gameTimer.Stop();
                    counter.Stop();
                }

                //lblstats.Text = "Xspeed: " & Math.Abs(xVel) & " Yspeed: " & Math.Abs(yVel) & " Human Location: " & paddleHuman.Location.ToString & " Computer Location: " & paddleComputer.Location.ToString & Environment.NewLine & " Ball Location: " & ball.Location.ToString & " Xdec: " & xveldec & " Ydec: " & yveldec & " Xinc: " & incrementx & " Yinc: " & incrementy
                lblstatsX.Text = Localization.Parse("{H_VEL}: ") + xveldec;
                lblstatsY.Text = Localization.Parse("{V_VEL}: ") + yveldec;
                lblstatscodepoints.Text = Localization.Parse("{CODEPOINTS}: ") + (levelrewards[level - 1] + beatairewardtotal).ToString();
                lbllevelandtime.Text = Localization.Parse("{LEVEL}: " + level + " - " + secondsleft + " {SECONDS_LEFT}");

                if (xVel > 20 || xVel < -20)
                {
                    paddleHuman.Width = Math.Abs(xVel);
                    paddleComputer.Width = Math.Abs(xVel);
                }
                else
                {
                    paddleHuman.Width = 20;
                    paddleComputer.Width = 20;
                }

                computerspeed = Math.Abs(yVel);

                //  pgcontents.Refresh()
                // pgcontents.CreateGraphics.FillRectangle(Brushes.Black, ball.Location.X, ball.Location.Y, ball.Width, ball.Height)

            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void counter_Tick(object sender, EventArgs e)
        {
            if (this.Left < Screen.PrimaryScreen.Bounds.Width)
            {
                secondsleft = secondsleft - 1;
                if (secondsleft == 1)
                {
                    secondsleft = 60;
                    level = level + 1;
                    generatenextlevel();
                    pnlgamestats.Show();
                    pnlgamestats.BringToFront();
                    pnlgamestats.Location = new Point((pgcontents.Width / 2) - (pnlgamestats.Width / 2), (pgcontents.Height / 2) - (pnlgamestats.Height / 2));

                    counter.Stop();
                    gameTimer.Stop();
                    SendHighscores();
                }

                lblstatscodepoints.Text = Localization.Parse("{CODEPOINTS}: ") + (levelrewards[level - 1] + beatairewardtotal).ToString();
            }
            SetupStats();
        }

        public void SendHighscores()
        {
            var highscore = new PongHighscore
            {
                UserName = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}",
                HighestLevel = level,
                HighestCodepoints = totalreward
            };
            ServerManager.SendMessage("pong_sethighscore", JsonConvert.SerializeObject(highscore));
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnplayon_Click(object sender, EventArgs e)
        {
            xveldec = levelxspeed;
            yveldec = levelyspeed;

            secondsleft = 60;

            tmrcountdown.Start();
            lblbeatai.Text = Localization.Parse($"{{PONG_BEAT_AI_REWARD}}: {beataireward}");
            pnlgamestats.Hide();
            lblbeatai.Show();
            ball.Location = new Point(paddleHuman.Location.X + paddleHuman.Width + 50, paddleHuman.Location.Y + paddleHuman.Height / 2);
            if (xVel < 0)
                xVel = Math.Abs(xVel);
            lbllevelandtime.Text = Localization.Parse("{LEVEL}: " + level + " - " + secondsleft + " {SECONDS_LEFT}");
        }

        //Increase the ball speed stats for the next level
        private void generatenextlevel()
        {
            lbllevelreached.Text = Localization.Parse("{YOU_REACHED_LEVEL} " + level + "!");

            lblpreviousstats.Text = Localization.Parse("{INITIAL_H_VEL}: " + levelxspeed + Environment.NewLine + "{INITIAL_V_VEL}: " + levelyspeed + Environment.NewLine + "{INC_H_VEL}: " + incrementx + Environment.NewLine + "{INC_V_VEL}: " + incrementy);

            switch (rand.Next(1, 3))
            {
                case 1:
                    levelxspeed = levelxspeed + 1;
                    break;
                case 2:
                    levelxspeed = levelxspeed + 2;
                    break;
            }

            switch (rand.Next(1, 3))
            {
                case 1:
                    levelyspeed = levelyspeed + 1;
                    break;
                case 2:
                    levelyspeed = levelyspeed + 2;
                    break;
            }

            switch (rand.Next(1, 6))
            {
                case 1:
                    incrementx = incrementx + 0.1;
                    break;
                case 2:
                    incrementx = incrementx + 0.2;
                    break;
                case 3:
                    incrementy = incrementy + 0.1;
                    break;
                case 4:
                    incrementy = incrementy + 0.2;
                    break;
                case 5:
                    incrementy = incrementy + 0.3;
                    break;
            }

            lblnextstats.Text = Localization.Parse("{INITIAL_H_VEL}: " + levelxspeed + Environment.NewLine + "{INITIAL_V_VEL}: " + levelyspeed + Environment.NewLine + "{INC_H_VEL}: " + incrementx + Environment.NewLine + "{INC_V_VEL}: " + incrementy);

            if (level < 15)
            {
                if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade"))
                {
                    beataireward = level * 10;
                } else
                {
                    beataireward = level * 5;
                }
            }
            else
            {
                if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade"))
                {
                    double br = levelrewards[level - 1] / 10;
                    beataireward = (int)Math.Round(br) * 10;
                } else
                {
                    double br = levelrewards[level - 1] / 10;
                    beataireward = (int)Math.Round(br) * 5;
                }
            }

            totalreward = levelrewards[level - 1] + beatairewardtotal;

            btncashout.Text = Localization.Parse("{CASH_OUT_WITH_CODEPOINTS}");
            btnplayon.Text = Localization.Parse("{PONG_PLAY_ON_FOR_MORE}");
        }

        private void setuplevelrewards()
        {
            if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade"))
            {
                levelrewards[0] = 0;
                levelrewards[1] = 40;
                levelrewards[2] = 120;
                levelrewards[3] = 280;
                levelrewards[4] = 580;
                levelrewards[5] = 800;
                levelrewards[6] = 1200;
                levelrewards[7] = 1800;
                levelrewards[8] = 2400;
                levelrewards[9] = 3200;
                levelrewards[10] = 4000;
                levelrewards[11] = 5000;
                levelrewards[12] = 6000;
                levelrewards[13] = 8000;
                levelrewards[14] = 10000;
                levelrewards[15] = 12000;
                levelrewards[16] = 16000;
                levelrewards[17] = 20000;
                levelrewards[18] = 26000;
                levelrewards[19] = 32000;
                levelrewards[20] = 40000;
                levelrewards[21] = 50000;
                levelrewards[22] = 64000;
                levelrewards[23] = 80000;
                levelrewards[24] = 100000;
                levelrewards[25] = 120000;
                levelrewards[26] = 150000;
                levelrewards[27] = 180000;
                levelrewards[28] = 220000;
                levelrewards[29] = 280000;
                levelrewards[30] = 360000;
                levelrewards[31] = 440000;
                levelrewards[32] = 540000;
                levelrewards[33] = 640000;
                levelrewards[34] = 800000;
                levelrewards[35] = 1000000;
                levelrewards[36] = 1280000;
                levelrewards[37] = 1600000;
                levelrewards[38] = 2000000;
                levelrewards[39] = 3000000;
                levelrewards[40] = 4000000;
            } else
            {
                levelrewards[0] = 0;
                levelrewards[1] = 20;
                levelrewards[2] = 60;
                levelrewards[3] = 140;
                levelrewards[4] = 290;
                levelrewards[5] = 400;
                levelrewards[6] = 600;
                levelrewards[7] = 900;
                levelrewards[8] = 1200;
                levelrewards[9] = 1600;
                levelrewards[10] = 2000;
                levelrewards[11] = 2500;
                levelrewards[12] = 3000;
                levelrewards[13] = 4000;
                levelrewards[14] = 5000;
                levelrewards[15] = 6000;
                levelrewards[16] = 8000;
                levelrewards[17] = 10000;
                levelrewards[18] = 13000;
                levelrewards[19] = 16000;
                levelrewards[20] = 20000;
                levelrewards[21] = 25000;
                levelrewards[22] = 32000;
                levelrewards[23] = 40000;
                levelrewards[24] = 50000;
                levelrewards[25] = 60000;
                levelrewards[26] = 75000;
                levelrewards[27] = 90000;
                levelrewards[28] = 110000;
                levelrewards[29] = 140000;
                levelrewards[30] = 180000;
                levelrewards[31] = 220000;
                levelrewards[32] = 270000;
                levelrewards[33] = 320000;
                levelrewards[34] = 400000;
                levelrewards[35] = 500000;
                levelrewards[36] = 640000;
                levelrewards[37] = 800000;
                levelrewards[38] = 1000000;
                levelrewards[39] = 1500000;
                levelrewards[40] = 2000000;
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void countdown_Tick(object sender, EventArgs e)
        {
            if (this.Left < Screen.PrimaryScreen.Bounds.Width)
            {
                switch (countdown)
                {
                    case 0:
                        countdown = 3;
                        lblcountdown.Hide();
                        lblbeatai.Hide();
                        gameTimer.Start();
                        counter.Start();
                        tmrcountdown.Stop();
                        break;
                    case 1:
                        lblcountdown.Text = "1";
                        countdown = countdown - 1;
                        ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.typesound);
                        break;
                    case 2:
                        lblcountdown.Text = "2";
                        countdown = countdown - 1;
                        ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.typesound);
                        break;
                    case 3:
                        lblcountdown.Text = "3";
                        countdown = countdown - 1;
                        lblcountdown.Show();
                        ShiftOS.Engine.AudioManager.PlayStream(Properties.Resources.typesound);
                        break;
                }
            
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btncashout_Click(object sender, EventArgs e)
        {
            pnlgamestats.Hide();
            pnlfinalstats.Show();
            lblfinalcodepointswithtext.Text = Localization.Parse("{YOU_WON} " + totalreward + " {CODEPOINTS}!");
            lblfinallevelreached.Text = Localization.Parse("{CODEPOINTS_FOR_BEATING_LEVEL}: ") + (level - 1).ToString();
            lblfinallevelreward.Text = levelrewards[level - 1].ToString();
            lblfinalcomputerreward.Text = beatairewardtotal.ToString();
            lblfinalcodepoints.Text = totalreward + Localization.Parse(" {CODEPOINTS_SHORT}");
            SaveSystem.TransferCodepointsFrom("pong", totalreward);
        }

        private void newgame()
        {
            pnlfinalstats.Hide();
            pnllose.Hide();
            pnlintro.Hide();

            level = 1;
            totalreward = 0;
            if (ShiftoriumFrontend.UpgradeInstalled("pong_upgrade"))
            {
                beataireward = 10;
            } else
            {
                beataireward = 5;
            }
            beatairewardtotal = 0;
            secondsleft = 60;
            lblstatscodepoints.Text = Localization.Parse("{CODEPOINTS}: ");
            //reset stats text
            lblstatsX.Text = Localization.Parse("{H_VEL}: ");
            lblstatsY.Text = Localization.Parse("{V_VEL}: ");

            levelxspeed = 3;
            levelyspeed = 3;

            incrementx = 0.4;
            incrementy = 0.2;

            xveldec = levelxspeed;
            yveldec = levelyspeed;

            tmrcountdown.Start();
            lblbeatai.Text = Localization.Parse($"{{PONG_BEAT_AI_REWARD}}: {beataireward}");
            pnlgamestats.Hide();
            lblbeatai.Show();
            ball.Location = new Point(paddleHuman.Location.X + paddleHuman.Width + 50, (paddleHuman.Location.Y + paddleHuman.Height) / 2);
            if (xVel < 0)
                xVel = Math.Abs(xVel);
            lbllevelandtime.Text = Localization.Parse("{{LEVEL}}: " + level + " - " + secondsleft + " {SECONDS_LEFT}");
        }

        public void btnhighscore_Click(object s, EventArgs a)
        {
            pnlhighscore.BringToFront();
            SetupHighScores();
        }

        public void SetupHighScores()
        {
            lbhighscore.Items.Clear();
            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "pong_highscores")
                {
                    var hs = JsonConvert.DeserializeObject<List<PongHighscore>>(msg.Contents);

                    var orderedhs = hs.OrderByDescending(i => i.HighestLevel);

                    foreach(var score in orderedhs)
                    {
                        this.Invoke(new Action(() =>
                        {
                            lbhighscore.Items.Add($"{score.UserName}\t\t\t{score.HighestLevel}\t\t{score.HighestCodepoints} CP");
                        }));
                    }
                }
            };
            ServerManager.SendMessage("pong_gethighscores", null);
            pnlhighscore.Show();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnplayagain_Click(object sender, EventArgs e)
        {
            newgame();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnlosetryagain_Click(object sender, EventArgs e)
        {
            newgame();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnstartgame_Click(object sender, EventArgs e)
        {
            newgame();
        }

        Random rand = new Random();
        // ERROR: Handles clauses are not supported in C#
        private void tmrstoryline_Tick(object sender, EventArgs e)
        {
            // Random chance of showing getshiftnet storyline
            int i = rand.Next(0, 100);

            if (i >= 25 && i <= 50)
            {
                tmrstoryline.Stop();
            }

        }

        // ERROR: Handles clauses are not supported in C#
        private void me_closing(object sender, FormClosingEventArgs e)
        {
            tmrstoryline.Stop();
        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void pgcontents_Paint(object sender, PaintEventArgs e) {

        }

        private void ball_MouseEnter(object sender, EventArgs e) {
            aiShouldIsbeEnabled = false;
        }

        private void ball_MouseLeave(object sender, EventArgs e) {
            aiShouldIsbeEnabled = true;
        }

        public void OnLoad()
        {
            pnlintro.BringToFront();
            pnlintro.Show();
            pnlhighscore.Hide();
            pnlgamestats.Hide();
            pnlfinalstats.Hide();
            CenterPanels();
            lblbeatai.Hide();
        }

        public void OnSkinLoad()
        {
            CenterPanels();
            this.SizeChanged += (o, a) =>
            {
                CenterPanels();
            };
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            CenterPanels();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnlhighscore.Hide();
        }
    }
}
