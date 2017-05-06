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
            if (IsMultiplayerSession)
            {
                if (IsLeader)
                {
                    paddleHuman.Location = new Point(paddleHuman.Location.X, (loc.Y) - (paddleHuman.Height / 2));
                    ServerManager.Forward(OpponentGUID, "pong_mp_setopponenty", paddleHuman.Top.ToString());
                }
                else
                {
                    paddleComputer.Location = new Point(paddleComputer.Location.X, (loc.Y) - (paddleComputer.Height / 2));
                    ServerManager.Forward(OpponentGUID, "pong_mp_setopponenty", paddleComputer.Top.ToString());
                }
            }
            else
            {
                paddleHuman.Location = new Point(paddleHuman.Location.X, (loc.Y) - (paddleHuman.Height / 2));
            }
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

        int OpponentY = 0;

        bool IsLeader = true;

        int LeaderX = 0;
        int LeaderY = 0;

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
                if (IsMultiplayerSession == true)
                {
                    //If we're multiplayer, then we want to set the computer Y to the opponent's Y.
                    //If we're the leader, we set the AI paddle, else we set the player paddle.
                    if (IsLeader)
                        paddleComputer.Top = OpponentY;
                    else
                        paddleHuman.Top = OpponentY;
                }
                else
                {
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

                bool BallPhysics = true;

                if (IsMultiplayerSession)
                {
                    //Logic for moving the ball in Multiplayer.
                    if (IsLeader)
                    {
                        ball.Location = new Point(ball.Location.X + xVel, ball.Location.Y + yVel);
                    }
                    else
                    {
                        //Move it to the leader's ball position.
                        ball.Location = new Point(LeaderX, LeaderY);
                        BallPhysics = false;
                    }
                }
                else
                {// Move the game ball.
                    ball.Location = new Point(ball.Location.X + xVel, ball.Location.Y + yVel);
                }
                if (BallPhysics)
                {
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
                }



                // Check for left wall.
                if (ball.Location.X < -100)
                {
                    //If we are in multiplayer, and not the leader, we won.
                    if (IsMultiplayerSession)
                    {
                        if (IsLeader)
                        {
                            //We lost.
                            NotifyLoseToTarget();
                        }
                        else
                        {
                            //We won.
                            NotifyWinToTarget();
                            Win();
                        }
                    }
                    else
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
                }

                // Check for right wall.
                if (ball.Location.X > this.Width - ball.Size.Width - paddleComputer.Width + 100)
                {
                    if (IsMultiplayerSession)
                    {
                        //If we are the leader we won.
                        if (IsLeader)
                        {
                            NotifyWinToTarget();
                            Win();
                        }
                        else
                        {
                            NotifyLoseToTarget();
                        }
                    }
                    else
                    {
                        Win();
                    }
                }

                if (IsMultiplayerSession)
                {
                    if (IsLeader)
                    {
                        ServerManager.Forward(OpponentGUID, "pong_mp_setballpos", JsonConvert.SerializeObject(ball.Location));
                    }
                }

                if (IsLeader)
                {
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
                }
                if (!IsMultiplayerSession)
                {
                    computerspeed = Math.Abs(yVel);
                }
            }
        }

        public void ServerMessageReceivedHandler(ServerMessage msg)
        {
            if (IsMultiplayerSession)
            {
                if (msg.Name == "pong_mp_setballpos")
                {
                    var pt = JsonConvert.DeserializeObject<Point>(msg.Contents);
                    LeaderX = pt.X;
                    LeaderY = pt.Y;
                }
                else if (msg.Name == "pong_mp_left")
                {
                    this.Invoke(new Action(() =>
                    {
                        AppearanceManager.Close(this);
                    }));
                    Infobox.Show("Opponent has closed Pong.", "The opponent has closed Pong, therefore the connection between you two has dropped.");
                }
                else if (msg.Name == "pong_mp_youlose")
                {
                    this.Invoke(new Action(LoseMP));
                }
                else if (msg.Name == "pong_mp_setopponenty")
                {
                    int y = Convert.ToInt32(msg.Contents);
                    OpponentY = y;
                }
                else if (msg.Name == "pong_handshake_matchmake")
                {
                    if (!PossibleMatchmakes.Contains(msg.Contents))
                        PossibleMatchmakes.Add(msg.Contents);
                    this.Invoke(new Action(ListMatchmakes));
                }
                else if (msg.Name == "pong_handshake_resendid")
                {
                    ServerManager.Forward("all", "pong_handshake_matchmake", YouGUID);
                }
                else if (msg.Name == "pong_handshake_complete")
                {
                    IsLeader = true;

                    OpponentGUID = msg.Contents;
                    LeaveMatchmake();
                    this.Invoke(new Action(() =>
                    {
                        pnlmultiplayerhandshake.Hide();
                        StartLevel();
                    }));
                }
                else if(msg.Name == "pong_mp_levelcompleted")
                {
                    level = Convert.ToInt32(msg.Contents) + 1;
                    this.Invoke(new Action(CompleteLevel));
                }
                else if (msg.Name == "pong_handshake_chosen")
                {
                    IsLeader = false;
                    LeaveMatchmake();
                    OpponentGUID = msg.Contents;
                    YouGUID = ServerManager.thisGuid.ToString();
                    //Start the timers.
                    counter.Start();
                    SendFollowerGUID();
                    this.Invoke(new Action(() =>
                    {
                        pnlmultiplayerhandshake.Hide();
                    }));
                }
                else if(msg.Name == "pong_mp_cashedout")
                {
                    this.Invoke(new Action(() =>
                    {
                        btncashout_Click(this, EventArgs.Empty);
                    }));
                    Infobox.Show("Cashed out.", "The other player has cashed out their Codepoints. Therefore, we have automatically cashed yours out.");
                }
                else if(msg.Name == "pong_mp_startlevel")
                {
                    OpponentAgrees = true;
                    if(YouAgree == false)
                    {
                        Infobox.PromptYesNo("Play another level?", "The opponent wants to play another level. Would you like to as well?", (answer)=>
                        {
                            YouAgree = answer;
                            ServerManager.Forward(OpponentGUID, "pong_mp_level_callback", YouAgree.ToString());
                        });
                    }
                }
                else if(msg.Name == "pong_mp_level_callback")
                {
                    bool agreed = bool.Parse(msg.Contents);
                    OpponentAgrees = agreed;
                    if (OpponentAgrees)
                    {
                        if (IsLeader)
                        {
                            //this.Invoke(new Action(()))
                        }
                    }
                }
                else if (msg.Name == "pong_handshake_left")
                {
                    if (this.PossibleMatchmakes.Contains(msg.Contents))
                        this.PossibleMatchmakes.Remove(msg.Contents);
                    this.Invoke(new Action(ListMatchmakes));
                }
                else if(msg.Name == "pong_mp_clockupdate")
                {
                    secondsleft = Convert.ToInt32(msg.Contents);
                }
                else if (msg.Name == "pong_mp_youwin")
                {
                    this.Invoke(new Action(Win));
                }
            }
        }

        bool OpponentAgrees = false;
        bool YouAgree = false;

        public void ListMatchmakes()
        {
            lvotherplayers.Items.Clear();
            var c = new ColumnHeader();
            c.Width = lvotherplayers.Width;
            c.Text = "Player";
            lvotherplayers.Columns.Clear();
            lvotherplayers.Columns.Add(c);

            lvotherplayers.FullRowSelect = true;
            foreach (var itm in PossibleMatchmakes)
            {
                if (itm != YouGUID)
                {
                    var l = new ListViewItem();
                    l.Text = itm;
                    lvotherplayers.Items.Add(l);
                }
            }

            if (PossibleMatchmakes.Count > 0)
            {
                lbmpstatus.Text = "Select a player.";
            }
            else
            {
                lbmpstatus.Text = "Waiting for players...";
            }
        }

        public void NotifyLoseToTarget()
        {
            ServerManager.Forward(OpponentGUID, "pong_mp_youwin", null);
        }

        public void NotifyWinToTarget()
        {
            ServerManager.Forward(OpponentGUID, "pong_mp_youlose", null);
        }

        public void LeaveMatchmake()
        {
            ServerManager.Forward("all", "pong_handshake_left", YouGUID);
        }

        List<string> PossibleMatchmakes = new List<string>();

        public void SendLeaderGUID(string target)
        {
            ServerManager.Forward(target, "pong_handshake_chosen", YouGUID);
        }


        public void StartMultiplayer()
        {
            IsMultiplayerSession = true;
            YouGUID = ServerManager.thisGuid.ToString();
            ServerManager.SendMessage("pong_handshake_matchmake", YouGUID);
            StartMatchmake();
        }

        public void StartMatchmake()
        {
            pnlmultiplayerhandshake.Show();
            pnlmultiplayerhandshake.CenterParent();
            pnlmultiplayerhandshake.BringToFront();

            ServerManager.Forward("all", "pong_handshake_resendid", null);

        }


        public void SendFollowerGUID()
        {
            ServerManager.Forward(OpponentGUID, "pong_handshake_complete", YouGUID);
        }

        public void LoseMP()
        {
            ball.Location = new Point(this.Size.Width / 2 + 200, this.Size.Height / 2);
            if(IsLeader)
                if (xVel > 0)
                    xVel = -xVel;
            lblbeatai.Show();
            lblbeatai.Text = "The opponent has beaten you!";
            tmrcountdown.Start();
            gameTimer.Stop();
            counter.Stop();

        }

        public void Win()
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

        public void CompleteLevel()
        {
            if (SaveSystem.CurrentSave.UniteAuthToken != null)
            {
                try
                {
                    var unite = new ShiftOS.Unite.UniteClient("http://getshiftos.ml", SaveSystem.CurrentSave.UniteAuthToken);
                    if (unite.GetPongLevel() < level)
                        unite.SetPongLevel(level);
                }
                catch { }
            }
            //Only set these stats if the user is the leader.
            if (IsLeader)
            {
                secondsleft = 60;
                level = level + 1;
                generatenextlevel();
            }

            pnlgamestats.Show();
            pnlgamestats.BringToFront();
            pnlgamestats.Location = new Point((pgcontents.Width / 2) - (pnlgamestats.Width / 2), (pgcontents.Height / 2) - (pnlgamestats.Height / 2));

            counter.Stop();
            gameTimer.Stop();

        }

        // ERROR: Handles clauses are not supported in C#
        private void counter_Tick(object sender, EventArgs e)
        {
            if (IsLeader)
            {
                if (this.Left < Screen.PrimaryScreen.Bounds.Width)
                {
                    secondsleft = secondsleft - 1;
                    if (secondsleft == 1)
                    {
                        CompleteLevel();
                    }

                    lblstatscodepoints.Text = Localization.Parse("{CODEPOINTS}: ") + (levelrewards[level - 1] + beatairewardtotal).ToString();
                }
            }
            SetupStats();
        }

        [Obsolete("This method does nothing. Use UniteClient for highscore queries.")]
        public void SendHighscores()
        {
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
            if (!string.IsNullOrWhiteSpace(SaveSystem.CurrentSave.UniteAuthToken))
            {
                var unite = new ShiftOS.Unite.UniteClient("http://getshiftos.ml", SaveSystem.CurrentSave.UniteAuthToken);
                if (unite.GetPongCP() < totalreward)
                {
                    unite.SetPongCP(totalreward);
                }
            }
            if (IsMultiplayerSession)
            {
                ServerManager.Forward(OpponentGUID, "pong_mp_cashedout", null);
                StopMultiplayerSession();
            }
        }

        public void StopMultiplayerSession()
        {
            IsMultiplayerSession = false;
            IsLeader = true;
            OpponentGUID = "";
            YouGUID = "";
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

        bool IsMultiplayerSession = false;

        string YouGUID = "";
        string OpponentGUID = "";

        public void SetupHighScores()
        {
            lbhighscore.Items.Clear();
            lbhighscore.View = View.Details;
            lbhighscore.FullRowSelect = true;
            lbhighscore.Columns.Clear();
            var n = new ColumnHeader();
            n.Text = "Player";
            n.Width = lbhighscore.Width / 3;
            var l = new ColumnHeader();
            l.Text = "Level";
            l.Width = n.Width;
            var c = new ColumnHeader();
            c.Text = "Codepoints";
            c.Width = n.Width;
            lbhighscore.Columns.Add(n);
            lbhighscore.Columns.Add(l);
            lbhighscore.Columns.Add(c);

            var t = new Thread(() =>
            {
                try
                {

                    var unite = new ShiftOS.Unite.UniteClient("http://getshiftos.ml", SaveSystem.CurrentSave.UniteAuthToken);
                    var hs = unite.GetPongHighscores();
                    foreach (var score in hs.Highscores)
                    {
                        if(this.ParentForm.Visible == false)
                        {
                            Thread.CurrentThread.Abort();
                        }
                        string username = unite.GetDisplayNameId(score.UserId);
                        this.Invoke(new Action(() =>
                        {
                            var name_item = new ListViewItem();
                            name_item.Text = username;
                            lbhighscore.Items.Add(name_item);
                            name_item.SubItems.Add(score.Level.ToString());
                            name_item.SubItems.Add(score.CodepointsCashout.ToString());
                        }));
                    }
                }
                catch
                {
                    try
                    {
                        if (this.ParentForm.Visible == true)
                        {
                            Infobox.Show("Service unavailable.", "The Pong Highscore service is unavailable at this time.");
                            this.Invoke(new Action(pnlgamestats.BringToFront));
                        }
                    }
                    catch { } //JUST. ABORT. THE. FUCKING. THREAD.
                    return;
                }
            });
            t.Start();
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

        public void StartLevel()
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
            ServerManager.MessageReceived += this.ServerMessageReceivedHandler;
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
            if(IsMultiplayerSession == true)
            {
                if(!string.IsNullOrWhiteSpace(OpponentGUID))
                    ServerManager.Forward(OpponentGUID, "pong_mp_left", null);
                LeaveMatchmake();
            }
            ServerManager.MessageReceived -= this.ServerMessageReceivedHandler;

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

        private void btnmatchmake_Click(object sender, EventArgs e)
        {
            this.StartMultiplayer();
            pnlintro.Hide();
            lblbeatai.Text = "Beat the other player to earn Codepoints.";
            lblcountdown.Text = "Waiting for another player...";
            lblcountdown.Left = (this.Width - lblcountdown.Width) / 2;
        }

        private void lvotherplayers_DoubleClick(object sender, EventArgs e)
        {
            if(lvotherplayers.SelectedItems.Count > 0)
            {
                SendLeaderGUID(lvotherplayers.SelectedItems[0].Text);
            }
        }
    }
}
