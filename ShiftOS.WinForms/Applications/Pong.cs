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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Pong", true, "al_pong", "Games")]
    [WinOpen("pong")]
    [DefaultTitle("Pong")]
    [DefaultIcon("iconPong")]
    public partial class Pong : UserControl, IShiftOSWindow
    {
        public Pong()
        {
            InitializeComponent();
            paddleWidth = pnlcanvas.Width / 30;
            drawTimer = new Timer();
            drawTimer.Interval = 1;
            drawTimer.Tick += (o, a) =>
            {
                UpdateBall();
                pnlcanvas.Refresh();
            };
            counterTimer = new Timer();
            counterTimer.Interval = 1000;
            counterTimer.Tick += (o, a) =>
            {
                if(secondsleft > 0)
                {
                    secondsleft--;
                }
                else
                {
                    LevelComplete();
                }
            };
        }

        private double ballX = 0.0f;
        private double ballY = 0.0f;

        private double aiBallX = 0.0f;
        private double aiBallY = 0.0f;


        private double speedFactor = 0.0125;

        private double xVel = 1;
        private double yVel = 1;

        private int paddleWidth;

        Timer counterTimer = null;

        private long codepointsToEarn = 0;
        private int level = 1;


        private double playerY = 0.0;
        private double opponentY = 0.0;
        private int secondsleft = 60;
        bool doAi = true;
        bool doBallCalc = true;

        private string header = "";

        private string counter = "";

        public void UpdateBall()
        {
            if (doBallCalc)
            {
                double ballXLocal = linear(ballX, -1.0, 1.0, 0, pnlcanvas.Width);
                double ballYLocal = linear(ballY, -1.0, 1.0, 0, pnlcanvas.Height);

                ballXLocal -= ((double)paddleWidth / 2);
                ballYLocal -= ((double)paddleWidth / 2);

                double aiBallXLocal = linear(aiBallX, -1.0, 1.0, 0, pnlcanvas.Width);
                double aiBallYLocal = linear(aiBallY, -1.0, 1.0, 0, pnlcanvas.Height);

                aiBallXLocal -= ((double)paddleWidth / 2);
                aiBallYLocal -= ((double)paddleWidth / 2);


                double playerYLocal = linear(playerY, -1.0, 1.0, 0, pnlcanvas.Height);
                double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, pnlcanvas.Height);

                int paddleHeight = pnlcanvas.Height / 5;


                Rectangle ballRect = new Rectangle((int)ballXLocal, (int)ballYLocal, paddleWidth, paddleWidth);

                Rectangle aiBallRect = new Rectangle((int)aiBallXLocal, (int)aiBallYLocal, paddleWidth, paddleWidth);


                Rectangle playerRect = new Rectangle((int)paddleWidth, (int)(playerYLocal - (int)(paddleHeight / 2)), (int)paddleWidth, (int)paddleHeight);
                Rectangle opponentRect = new Rectangle((int)(pnlcanvas.Width - (paddleWidth * 2)), (int)(opponentYLocal - (int)(paddleHeight / 2)), (int)paddleWidth, (int)paddleHeight);

                //Top and bottom walls:
                if (ballRect.Top <= 0 || ballRect.Bottom >= pnlcanvas.Height)
                    yVel = -yVel; //reverse the Y velocity


                //Left wall
                if (ballRect.Left <= 0)
                {
                    //You lose.
                    codepointsToEarn = 0;
                    Lose();
                }

                //Right wall
                if (ballRect.Right >= pnlcanvas.Width)
                {
                    //You win.
                    codepointsToEarn += CalculateAIBeatCP();
                    Win();
                }

                //Enemy paddle:
                if (ballRect.IntersectsWith(opponentRect))
                {
                    //check if the ball x is greater than the player paddle's middle coordinate
                    if (ballRect.Right >= opponentRect.Left)
                    {
                        //reverse x velocity to send the ball the other way
                        xVel = -xVel;

                        //set y velocity based on where the ball hit the paddle
                        yVel = linear((ballRect.Top + (ballRect.Height / 2)), opponentRect.Top, opponentRect.Bottom, -1, 1);
                        doAi = false;
                    }

                }


                //Enemy paddle - AI:
                if (aiBallRect.IntersectsWith(opponentRect))
                {
                    //check if the ball x is greater than the player paddle's middle coordinate
                    if (aiBallRect.Right >= opponentRect.Left)
                    {
                        doAi = false;
                    }

                }


                //Player paddle:
                if (ballRect.IntersectsWith(playerRect))
                {
                    if (ballRect.Left <= playerRect.Right)
                    {
                        //reverse x velocity to send the ball the other way
                        xVel = -xVel;

                        //set y velocity based on where the ball hit the paddle
                        yVel = linear((ballRect.Top + (ballRect.Height / 2)), playerRect.Top, playerRect.Bottom, -1, 1);

                        //reset the ai location
                        aiBallX = ballX;
                        aiBallY = ballY;
                        doAi = true;
                    }

                }





                ballX += xVel * speedFactor;
                ballY += yVel * speedFactor;

                aiBallX += xVel * (speedFactor * 2);
                aiBallY += yVel * (speedFactor * 2);

                if (doAi == true)
                {
                    if (opponentY != aiBallY)
                    {
                        if (opponentY < aiBallY)
                        {
                            if (opponentY < 0.9)
                                opponentY += speedFactor;
                        }
                        else
                        {
                            if (opponentY > -0.9)
                                opponentY -= speedFactor;
                        }
                    }
                }
            }
        }

        public void Lose()
        {
            InitializeCoordinates();
        }

        public void LevelComplete()
        {
            level++;
            doAi = false;
            doBallCalc = false;
            counterTimer.Stop();
            pnllevelwon.CenterParent();
            pnllevelwon.Show();
            pnllevelwon.BringToFront();
            lbltitle.Text = "You've reached level " + level + "!";
            lbltitle.Height = (int)CreateGraphics().MeasureString(lbltitle.Text, lbltitle.Font).Height;
            codepointsToEarn += CalculateAIBeatCP() * 2;
            speedFactor += speedFactor / level;
            secondsleft = 60;
        }

        public long CalculateAIBeatCP()
        {
            return 2 * (10 * level);
        }

        public void Win()
        {
            header = "You beat the AI! " + CalculateAIBeatCP() + " Codepoints!.";
            InitializeCoordinates();
            new System.Threading.Thread(() =>
            {
                doBallCalc = false;
                counter = "3";
                Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                System.Threading.Thread.Sleep(1000);
                counter = "2";
                Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                System.Threading.Thread.Sleep(1000);
                counter = "1";
                Engine.AudioManager.PlayStream(Properties.Resources.writesound);
                System.Threading.Thread.Sleep(1000);
                doBallCalc = true;
                header = "";
                counter = "";
            }).Start();
        }

        public void InitializeCoordinates()
        {
            ballX = 0;
            ballY = 0;
            xVel = 1;
            yVel = 1;
            opponentY = 0;
            aiBallX = 0;
            aiBallY = 0;
            doAi = true;
        }

        private void pnlcanvas_Paint(object sender, PaintEventArgs e)
        {

            paddleWidth = pnlcanvas.Width / 30;
            double ballXLocal = linear(ballX, -1.0, 1.0, 0, pnlcanvas.Width);
            double ballYLocal = linear(ballY, -1.0, 1.0, 0, pnlcanvas.Height);

            ballXLocal -= ((double)paddleWidth / 2);
            ballYLocal -= ((double)paddleWidth / 2);


            e.Graphics.Clear(pnlcanvas.BackColor);

            //draw the ball
            if(doBallCalc)
            e.Graphics.FillEllipse(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)ballXLocal, (float)ballYLocal, (float)paddleWidth, (float)paddleWidth));

            double playerYLocal = linear(playerY, -1.0, 1.0, 0, pnlcanvas.Height);
            double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, pnlcanvas.Height);

            int paddleHeight = pnlcanvas.Height / 5;

            int paddleStart = paddleWidth;

            //draw player paddle
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)paddleWidth, (float)(playerYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

            //draw opponent
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)(pnlcanvas.Width - (paddleWidth*2)), (float)(opponentYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

            string cp_text = Localization.Parse("{CODEPOINTS}: " + codepointsToEarn);

            var tSize = e.Graphics.MeasureString(cp_text, SkinEngine.LoadedSkin.Header3Font);

            var tLoc = new PointF((pnlcanvas.Width - (int)tSize.Width) / 2,
                (pnlcanvas.Height - (int)tSize.Height)
                
                );
            e.Graphics.DrawString(cp_text, SkinEngine.LoadedSkin.Header3Font, new SolidBrush(pnlcanvas.ForeColor), tLoc);
            tSize = e.Graphics.MeasureString(counter, SkinEngine.LoadedSkin.Header2Font);

            tLoc = new PointF((pnlcanvas.Width - (int)tSize.Width) / 2,
                (pnlcanvas.Height - (int)tSize.Height) / 2

                );
            e.Graphics.DrawString(counter, SkinEngine.LoadedSkin.Header2Font, new SolidBrush(pnlcanvas.ForeColor), tLoc);
            tSize = e.Graphics.MeasureString(header, SkinEngine.LoadedSkin.Header2Font);

            tLoc = new PointF((pnlcanvas.Width - (int)tSize.Width) / 2,
                (pnlcanvas.Height - (int)tSize.Height) / 4

                );
            e.Graphics.DrawString(header, SkinEngine.LoadedSkin.Header2Font, new SolidBrush(pnlcanvas.ForeColor), tLoc);

            string l = Localization.Parse("{LEVEL}: " + level + " - " + secondsleft + " {SECONDS_LEFT}");
            tSize = e.Graphics.MeasureString(l, SkinEngine.LoadedSkin.Header3Font);

            tLoc = new PointF((pnlcanvas.Width - (int)tSize.Width) / 2,
                (tSize.Height)
                );
            e.Graphics.DrawString(l, SkinEngine.LoadedSkin.Header3Font, new SolidBrush(pnlcanvas.ForeColor), tLoc);


        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        Timer drawTimer = null;

        public void OnLoad()
        {
            doAi = false;
            doBallCalc = false;
            pnlgamestart.Show();
            pnlgamestart.BringToFront();
            pnlgamestart.CenterParent();
            drawTimer.Start();
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            drawTimer.Stop();
            counterTimer.Stop();
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void pnlcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            playerY = linear(e.Y, 0, pnlcanvas.Height, -1, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnllevelwon.Hide();
            doAi = true;
            doBallCalc = true;
            counterTimer.Start();
        }

        private void btncashout_Click(object sender, EventArgs e)
        {
            pnllevelwon.Hide();
            SaveSystem.CurrentSave.Codepoints += (ulong)codepointsToEarn;
            level = 1;
            speedFactor = 0.0125;
            Infobox.Show("Codepoints transferred.", "Pong has transferred " + codepointsToEarn + " Codepoints to your system.");
            codepointsToEarn = 0;
            pnlgamestart.Show();
            pnlgamestart.BringToFront();
            pnlgamestart.CenterParent();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            counterTimer.Start();
            doAi = true;
            doBallCalc = true;
            pnlgamestart.Hide();
        }
    }
}
