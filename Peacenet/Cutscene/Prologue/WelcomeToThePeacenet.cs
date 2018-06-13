using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Cutscene.Prologue
{
    public class WelcomeToThePeacenet : Plex.Engine.Cutscene.Cutscene
    {
        private float _welcomeSlide = 0f;
        private float _whiteshade = 0f;
        private SpriteFont _bigOxygen = null;
        private float _logoFade = 0f;
        private float _logoSlide = 0f;
        private Texture2D _logo = null;
        private Texture2D _wordmark = null;
        private bool _doneIntro = false;
        private double _ride = 0;

        public override string Name => "m00_welcome";

        public override void Update(GameTime time)
        {
            if(_doneIntro==false)
            {
                if(_welcomeSlide<1)
                {
                    _welcomeSlide = MathHelper.Clamp(_welcomeSlide + ((float)time.ElapsedGameTime.TotalSeconds * 3f), 0, 1);
                    _logoFade = _welcomeSlide;
                }
                else
                {
                    if (_logoSlide < 1f)
                    {
                        _logoSlide = MathHelper.Clamp(_logoSlide + ((float)time.ElapsedGameTime.TotalSeconds * 4f), 0, 1);
                    }
                    else
                    {
                        if(_ride < 2)
                        {
                            _ride += time.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            if (_whiteshade < 1f)
                            {
                                _whiteshade = MathHelper.Clamp(_whiteshade + ((float)time.ElapsedGameTime.TotalSeconds * 4f), 0, 1);
                            }
                            else
                            {
                                _doneIntro = true;
                            }
                        }
                    }
                }
            }
            else
            {
                _welcomeSlide = 0;
                _logoSlide = 0;
                _logoFade = 0;
                if (_whiteshade >0)
                {
                    _whiteshade = MathHelper.Clamp(_whiteshade - ((float)time.ElapsedGameTime.TotalSeconds * 4f), 0, 1);
                }
                else
                {
                    NotifyFinished();
                }
            }

            base.Update(time);
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            float iconWidth = _logo.Width/1.5f;
            float iconHeight = _logo.Height/1.5f;
            float wordWidth = _wordmark.Width/1.5f;
            float wordHeight = _wordmark.Height/1.5f;

            float totalPeacenetWidth = iconWidth + wordWidth;
            
            float iconXMin = (gfx.Width - iconWidth) / 2;
            float iconXMax = (gfx.Width - totalPeacenetWidth) / 2;

            float peacenetY = (gfx.Height - iconHeight) / 2;
            float iconX = MathHelper.Lerp(iconXMin, iconXMax, _logoSlide);

            string welcome = "Welcome to";

            var welcomeMeasure = _bigOxygen.MeasureString(welcome);

            float welcomeX = MathHelper.Lerp(-welcomeMeasure.X, iconXMax, _welcomeSlide);
            float welcomeY = peacenetY - (welcomeMeasure.Y + 15);

            gfx.DrawString(_bigOxygen, welcome, new Vector2(welcomeX, welcomeY), Color.White * _welcomeSlide);
            gfx.FillRectangle(iconX + iconWidth, peacenetY, wordWidth, wordHeight, _wordmark, Color.White * _logoSlide);
            gfx.FillRectangle(iconX, peacenetY, iconWidth, iconHeight, _logo, Color.White * _logoFade);
            


            gfx.FillRectangle(0, 0, gfx.Width, gfx.Height, Color.White * _whiteshade);
        }

        public override void Load(ContentManager content)
        {
            _bigOxygen = content.Load<SpriteFont>("Fonts/EvenHugerOxygen");
            _logo = content.Load<Texture2D>("Cutscenes/Prologue/peacenet icon");
            _wordmark = content.Load<Texture2D>("Cutscenes/Prologue/peacenet word");

        }
    }
}
