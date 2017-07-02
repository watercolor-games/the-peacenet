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
    [Launcher("{TITLE_PONG}", true, "al_pong", "{AL_GAMES}")]
    [WinOpen("{WO_PONG}")]
    [DefaultTitle("{TITLE_PONG}")]
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
#if DEBUG
            this.KeyDown += (o, a) =>
            {
                if(a.KeyCode == Keys.D)
                {
                    drawAiBall = !drawAiBall;                
                }
            };
#endif
        }

        private double ballX = 0.0f;
        private double ballY = 0.0f;

        private double aiBallX = 0.0f;
        private double aiBallY = 0.0f;


        private double speedFactor = 0.0125;

        private double xVel = 1;
        private double yVel = 1;

        private double aiXVel = 1;
        private double aiYVel = 1;


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

        private void Bounce(Rectangle ball, Rectangle paddle)
        {
            // reverse x velocity to send the ball the other way
            xVel = -xVel;

            // adjust y velocity based on where the ball hit the paddle
            yVel = linear((ball.Top + (ball.Height / 2)), paddle.Top, paddle.Bottom, -1, 1);
        }

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

                //top and bottom walls - ai
                if (aiBallRect.Top <= 0 || aiBallRect.Bottom >= pnlcanvas.Height)
                    aiYVel = -aiYVel; //reverse the Y velocity


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
                        Bounce(ballRect, opponentRect);
                        aiXVel = xVel;
                        aiYVel = yVel;
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
                        Bounce(ballRect, playerRect);
                        aiXVel = xVel;
                        aiYVel = yVel;

                        //reset the ai location
                        aiBallX = ballX;
                        aiBallY = ballY;
                        doAi = true;
                    }

                }





                ballX += xVel * speedFactor;
                ballY += yVel * speedFactor;

                aiBallX += aiXVel * (speedFactor * 1.5);
                aiBallY += aiYVel * (speedFactor * 1.5);

                if (doAi == true)
                {
                    if (opponentY != aiBallY)
                    {
                        if (opponentY < aiBallY)
                        {
                            if (opponentY < 0.9)
                                opponentY += speedFactor * level;
                        }
                        else
                        {
                            if (opponentY > -0.9)
                                opponentY -= speedFactor * level;
                        }
                    }
                }
            }
        }

        public void Lose()
        {
            InitializeCoordinates();
            counterTimer.Stop();
            secondsleft = 60;
            level = 1;
            speedFactor = 0.0125;
            pnlgamestart.Show();
            pnlgamestart.BringToFront();
            pnlgamestart.CenterParent();
            Infobox.Show("{TITLE_PONG_YOULOSE}", "{PROMPT_PONGLOST}");
            doAi = false;
            doBallCalc = false;
        }

        public void LevelComplete()
        {
            level++;
            doAi = false;
            doBallCalc = false;
            counterTimer.Stop();
            ballX = 0;
            ballY = 0;
            aiBallX = 0;
            aiBallY = 0;
            pnllevelwon.CenterParent();
            pnllevelwon.Show();
            pnllevelwon.BringToFront();
            lbltitle.Text = Localization.Parse("{PONG_LEVELREACHED}", new Dictionary<string, string>
            {
                ["%level"] = level.ToString()
            });
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
            header = Localization.Parse("{PONG_BEATAI}", new Dictionary<string, string>
            {
                ["%amount"] = CalculateAIBeatCP().ToString()
            });
            InitializeCoordinates();
            counterTimer.Stop();
            new System.Threading.Thread(() =>
            {
                doBallCalc = false;
                for (int i = 3; i > 0; i--)
                {
					counter = i.ToString();
					Engine.AudioManager.PlayStream(Properties.Resources.writesound);
					System.Threading.Thread.Sleep(1000);
				}
                doBallCalc = true;
                header = "";
                counter = "";
                Desktop.InvokeOnWorkerThread(() =>
                {
                    counterTimer.Start();
                });
            }).Start();
        }

        public void InitializeCoordinates()
        {
            ballX = 0;
            ballY = 0;
            opponentY = 0;
            xVel = 1;
            aiBallX = 0;
            aiBallY = 0;
            aiXVel = xVel;
            aiYVel = yVel;
            doAi = true;
        }

        private bool drawAiBall = false;

        private void pnlcanvas_Paint(object sender, PaintEventArgs e)
        {

            paddleWidth = pnlcanvas.Width / 30;
            double ballXLocal = linear(ballX, -1.0, 1.0, 0, pnlcanvas.Width);
            double ballYLocal = linear(ballY, -1.0, 1.0, 0, pnlcanvas.Height);

            ballXLocal -= ((double)paddleWidth / 2);
            ballYLocal -= ((double)paddleWidth / 2);

            double aiballXLocal = linear(aiBallX, -1.0, 1.0, 0, pnlcanvas.Width);
            double aiballYLocal = linear(aiBallY, -1.0, 1.0, 0, pnlcanvas.Height);

            aiballXLocal -= ((double)paddleWidth / 2);
            aiballYLocal -= ((double)paddleWidth / 2);


            e.Graphics.Clear(pnlcanvas.BackColor);

            //draw the ai ball
            if (drawAiBall)
                e.Graphics.FillEllipse(new SolidBrush(Color.Gray), new RectangleF((float)aiballXLocal, (float)aiballYLocal, (float)paddleWidth, (float)paddleWidth));


            //draw the ball
            if (doBallCalc)
            e.Graphics.FillEllipse(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)ballXLocal, (float)ballYLocal, (float)paddleWidth, (float)paddleWidth));

            double playerYLocal = linear(playerY, -1.0, 1.0, 0, pnlcanvas.Height);
            double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, pnlcanvas.Height);

            int paddleHeight = pnlcanvas.Height / 5;

            int paddleStart = paddleWidth;

            //draw player paddle
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)paddleWidth, (float)(playerYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

            //draw opponent
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)(pnlcanvas.Width - (paddleWidth*2)), (float)(opponentYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

            string cp_text = Localization.Parse("{PONG_STATUSCP}", new Dictionary<string, string>
            {
                ["%cp"] = codepointsToEarn.ToString()
            });

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

            string l = Localization.Parse("{PONG_STATUSLEVEL}", new Dictionary<string, string>
            {
                ["%level"] = level.ToString(),
                ["%time"] = secondsleft.ToString()
            });
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
            pnlgamestart.CenterParent();
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
            Infobox.Show("{TITLE_CODEPOINTSTRANSFERRED}", Localization.Parse("{PROMPT_CODEPOINTSTRANSFERRED}", new Dictionary<string, string>
            {
                ["%transferrer"] = "Pong",
                ["%amount"] = codepointsToEarn.ToString()
            }));
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
