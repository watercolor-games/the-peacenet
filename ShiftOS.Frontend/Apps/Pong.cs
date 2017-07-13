using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend.Apps
{
    [Launcher("{TITLE_PONG}", false, null, "{AL_GAMES}")]
    [WinOpen("{WO_PONG}")]
    [DefaultTitle("{TITLE_PONG}")]
    [DefaultIcon("iconPong")]
    public class Pong : GUI.Control, IShiftOSWindow
    {
        public Pong()
        {
            Width = 720;
            Height = 480;
            MouseMove += (loc) =>
            {
                double _y = linear(loc.Y, 0, Height, -1, 1);
                if(_y != playerY)
                {
                    playerY = _y;
                    Invalidate();
                }
            };
        }

        #region Private variables
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
        private long codepointsToEarn = 0;
        private int level = 1;
        private double playerY = 0.0;
        private double opponentY = 0.0;
        private int secondsleft = 60;
        bool doAi = true;
        bool doBallCalc = true;
        private string header = "";
        private string counter = "";
        #endregion

        #region Control behaviour overrides

        protected override void OnPaint(GraphicsContext gfx)
        {
            //This is where we'll dump the winforms painting code
            //By now, Layout() would have calculated the game's state

            paddleWidth = Width / 30;
            double ballXLocal = linear(ballX, -1.0, 1.0, 0, Width);
            double ballYLocal = linear(ballY, -1.0, 1.0, 0, Height);

            ballXLocal -= ((double)paddleWidth / 2);
            ballYLocal -= ((double)paddleWidth / 2);

            double aiballXLocal = linear(aiBallX, -1.0, 1.0, 0, Width);
            double aiballYLocal = linear(aiBallY, -1.0, 1.0, 0, Height);

            aiballXLocal -= ((double)paddleWidth / 2);
            aiballYLocal -= ((double)paddleWidth / 2);


            base.OnPaint(gfx);

            //draw the ball
            if (doBallCalc)
            {
                gfx.DrawRectangle((int)ballXLocal, (int)ballYLocal, paddleWidth, paddleWidth, UIManager.SkinTextures["ControlTextColor"]);
            }
            double playerYLocal = linear(playerY, -1.0, 1.0, 0, Height);
            double opponentYLocal = linear(opponentY, -1.0, 1.0, 0, Height);

            int paddleHeight = Height / 5;

            int paddleStart = paddleWidth;

            //draw player paddle
            gfx.DrawRectangle(paddleWidth, (int)playerYLocal - (paddleHeight / 2), paddleWidth, paddleHeight, UIManager.SkinTextures["ControlTextColor"]);


            //draw opponent
            gfx.DrawRectangle(Width - (paddleWidth*2), (int)opponentYLocal - (paddleHeight / 2), paddleWidth, paddleHeight, UIManager.SkinTextures["ControlTextColor"]);

            string cp_text = Localization.Parse("{PONG_STATUSCP}", new Dictionary<string, string>
            {
                ["%cp"] = codepointsToEarn.ToString()
            });

            var tSize = gfx.MeasureString(cp_text, SkinEngine.LoadedSkin.Header3Font);

            var tLoc = new Vector2((Width - (int)tSize.X) / 2,
                (Height - (int)tSize.Y)

                );

            gfx.DrawString(cp_text, (int)tLoc.X, (int)tLoc.Y, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), SkinEngine.LoadedSkin.Header3Font);

            tSize = gfx.MeasureString(counter, SkinEngine.LoadedSkin.Header2Font);

            tLoc = new Vector2((Width - (int)tSize.X) / 2,
                (Height - (int)tSize.Y) / 2

                );
            gfx.DrawString(counter, (int)tLoc.X, (int)tLoc.Y, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), SkinEngine.LoadedSkin.Header2Font);
            tSize = gfx.MeasureString(header, SkinEngine.LoadedSkin.Header2Font);

            tLoc = new Vector2((Width - (int)tSize.X) / 2,
                (Height - (int)tSize.Y) / 4

                );
            gfx.DrawString(header, (int)tLoc.X, (int)tLoc.Y, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), SkinEngine.LoadedSkin.Header2Font);

            string l = Localization.Parse("{PONG_STATUSLEVEL}", new Dictionary<string, string>
            {
                ["%level"] = level.ToString(),
                ["%time"] = secondsleft.ToString()
            });
            tSize = gfx.MeasureString(l, SkinEngine.LoadedSkin.Header3Font);

            tLoc = new Vector2((Width - (int)tSize.X) / 2,
                (tSize.Y)
                );
            gfx.DrawString(l, (int)tLoc.X, (int)tLoc.Y, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), SkinEngine.LoadedSkin.Header3Font);


        }

        #endregion


        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        public void OnLoad()
        {
            doBallCalc = true;
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
    }
}
