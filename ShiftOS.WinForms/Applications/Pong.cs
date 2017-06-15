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
            drawTimer.Interval = 16;
            drawTimer.Tick += (o, a) =>
            {
                UpdateBall();
                pnlcanvas.Refresh();
            };
        }

        private double ballX = 0.0f;
        private double ballY = 0.0f;


        private double speedFactor = 0.025;

        private double xVel = 1;
        private double yVel = 1;

        private int paddleWidth;

        private double playerY = 0.0;
        private double opponentY = 0.0;

        public void UpdateBall()
        {
            double ballXLocal = linear(ballX, -1.0, 1.0, 0, pnlcanvas.Width);
            double ballYLocal = linear(ballY, -1.0, 1.0, 0, pnlcanvas.Height);

            ballXLocal -= ((double)paddleWidth / 2);
            ballYLocal -= ((double)paddleWidth / 2);

            double playerYLocal = linear(playerY, -1.0, 1.0, 0, pnlcanvas.Height);
            double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, pnlcanvas.Height);

            int paddleHeight = pnlcanvas.Height / 5;


            Rectangle ballRect = new Rectangle((int)ballXLocal, (int)ballYLocal, paddleWidth, paddleWidth);

            Rectangle playerRect = new Rectangle((int)paddleWidth, (int)(playerYLocal - (int)(paddleHeight / 2)), (int)paddleWidth, (int)paddleHeight);
            Rectangle opponentRect = new Rectangle((int)(pnlcanvas.Width - (paddleWidth * 2)), (int)(opponentYLocal - (int)(paddleHeight / 2)), (int)paddleWidth, (int)paddleHeight);

            //Top and bottom walls:
            if (ballRect.Top <= 0 || ballRect.Bottom >= pnlcanvas.Height)
                yVel = -yVel; //reverse the Y velocity

            //Left and right walls (NOTE: TEMPORARY)
            if (ballRect.Left <= 0 || ballRect.Right >= pnlcanvas.Width)
                xVel = -xVel; //reverse the Y velocity



            //Enemy paddle:
            if (ballRect.IntersectsWith(opponentRect))
            {
                //check if the ball x is greater than the player paddle's middle coordinate
                if (ballRect.Right <= opponentRect.Right - (opponentRect.Width / 2))
                {
                    //reverse x velocity to send the ball the other way
                    xVel = -xVel;

                    //set y velocity based on where the ball hit the paddle
                    yVel = linear((ballRect.Top + (ballRect.Height / 2)), opponentRect.Top, opponentRect.Bottom, -1, 1);

                }

            }




            //Player paddle:
            if (ballRect.IntersectsWith(playerRect))
            {
                //check if the ball x is greater than the player paddle's middle coordinate
                if(ballRect.Left >= playerRect.Left + (playerRect.Width / 2))
                {
                    //reverse x velocity to send the ball the other way
                    xVel = -xVel;

                    //set y velocity based on where the ball hit the paddle
                    yVel = linear((ballRect.Top + (ballRect.Height / 2)), playerRect.Top, playerRect.Bottom, -1, 1);

                }

            }





            ballX += xVel * speedFactor;
            ballY += yVel * speedFactor;

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
            e.Graphics.FillEllipse(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)ballXLocal, (float)ballYLocal, (float)paddleWidth, (float)paddleWidth));

            double playerYLocal = linear(playerY, -1.0, 1.0, 0, pnlcanvas.Height);
            double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, pnlcanvas.Height);

            int paddleHeight = pnlcanvas.Height / 5;

            int paddleStart = paddleWidth;

            //draw player paddle
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)paddleWidth, (float)(playerYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

            //draw opponent
            e.Graphics.FillRectangle(new SolidBrush(pnlcanvas.ForeColor), new RectangleF((float)(pnlcanvas.Width - (paddleWidth*2)), (float)(opponentYLocal - (float)(paddleHeight / 2)), (float)paddleWidth, (float)paddleHeight));

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
            drawTimer.Start();
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            drawTimer.Stop();
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void pnlcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            playerY = linear(e.Y, 0, pnlcanvas.Height, -1, 1);
        }
    }
}
