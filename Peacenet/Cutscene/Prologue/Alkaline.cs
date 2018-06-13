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
    public class Alkaline : Plex.Engine.Cutscene.Cutscene
    {
        private SpriteFont _bigOxygen = null;
        private SpriteFont _hugeOxygen = null;

        private float _bigFade = 0f;
        private float _hugeFade = 0f;
        private float _bigFade2 = 0f;
        private double _ride = 0;

        public override string Name => "m00_alkaline";

        public override void Update(GameTime time)
        {
            if (_ride < 3)
            {
                if (_bigFade2 < 0.5F)
                {
                    _bigFade2 = MathHelper.Clamp(_bigFade2 + ((float)time.ElapsedGameTime.TotalSeconds), 0, 0.5f);
                }
                else
                {
                    if (_hugeFade < 0.5f)
                    {
                        _hugeFade = MathHelper.Clamp(_hugeFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 0.5f);
                    }
                    else
                    {
                        if (_bigFade < 0.5f)
                        {
                            _bigFade = MathHelper.Clamp(_bigFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 0.5f);
                        }
                        else
                        {
                            _ride += time.ElapsedGameTime.TotalSeconds;
                        }
                    }
                }
            }
            else
            {
                if (_bigFade2 < 1F)
                {
                    _bigFade2 = MathHelper.Clamp(_bigFade2 + ((float)time.ElapsedGameTime.TotalSeconds), 0, 1f);
                }
                else
                {
                    if (_bigFade < 1f)
                    {
                        _bigFade = MathHelper.Clamp(_bigFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 1f);
                    }
                    else
                    {
                        if (_hugeFade < 1f)
                        {
                            _hugeFade = MathHelper.Clamp(_hugeFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 1f);
                        }
                        else
                        {
                            NotifyFinished();
                        }
                    }
                }
            }

            base.Update(time);
        }

        public override void Load(ContentManager content)
        {
            _hugeOxygen = content.Load<SpriteFont>("Fonts/EvenHugerOxygen");
            _bigOxygen = content.Load<SpriteFont>("Fonts/HugeOxygen");
            base.Load(content);
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            string an = "An";
            string watercolor = "Alkaline Thunder";
            string presents = "game";

            var wMeasure = _hugeOxygen.MeasureString(watercolor);
            var aMeasure = _bigOxygen.MeasureString(an);
            var pMeasure = _bigOxygen.MeasureString(presents);

            float height = aMeasure.Y + 3 + wMeasure.Y + 3 + pMeasure.Y;

            float wCenter = (gfx.Height - height) / 2;
            float aCenter = wCenter - (aMeasure.Y + 3);
            float pCenter = wCenter + wMeasure.Y + 3;

            float wY = MathHelper.Lerp(wCenter + (height / 2), wCenter - (height / 2), _hugeFade);
            float aY = MathHelper.Lerp(aCenter + (height / 2), aCenter - (height / 2), _bigFade2);
            float pY = MathHelper.Lerp(pCenter + (height / 2), pCenter - (height / 2), _bigFade);

            float wOpacity = (_hugeFade > 0.5F) ? (2 - (_hugeFade * 2)) : (_hugeFade * 2);
            float aOpacity = (_bigFade2 > 0.5F) ? (2 - (_bigFade2 * 2)) : (_bigFade2 * 2);
            float pOpacity = (_bigFade > 0.5F) ? (2 - (_bigFade * 2)) : (_bigFade * 2);

            float wX = (gfx.Width - wMeasure.X) / 2;
            float aX = (gfx.Width - aMeasure.X) / 2;
            float pX = (gfx.Width - pMeasure.X) / 2;

            gfx.DrawString(_bigOxygen, an, new Vector2(aX, aY), Color.White * aOpacity);
            gfx.DrawString(_hugeOxygen, watercolor, new Vector2(wX, wY), new Color(0x40, 0x80, 0xFF) * wOpacity);
            gfx.DrawString(_bigOxygen, presents, new Vector2(pX, pY), Color.White * pOpacity);



        }
    }
}
