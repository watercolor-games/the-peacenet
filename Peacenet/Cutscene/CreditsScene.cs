using Plex.Engine.Cutscene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Newtonsoft.Json;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Peacenet.Cutscenes
{
    public class CreditsScene : Cutscene
    {
        private SoundEffect _yesMyGrassIsGreen = null;
        private SoundEffectInstance _grassInstance = null;

        private CreditsFile _creditsFile = null;
        private int _csState = 0;
        private float _componentSlide = 0F;
        private float _personSlide = 0F;
        private float _roleSlide = 0F;

        private int _componentIndex = -1;
        private int _personIndex = -1;


        [Dependency]
        private UIManager _ui = null;

        private System.Drawing.Font _mondaBig;
        private System.Drawing.Font _mondaMedium;
        private System.Drawing.Font _mondaSmall;
        private Color _peace = new Color(64, 128, 255, 255);
        private Color _gray = new Color(191, 191, 191, 255);
        private double _personRide = 0;
        private Texture2D _peacenet = null;

        private float _peacenetFade = 0;
        private float _thanksFade = 0;

        public override string Name
        {
            get
            {
                return "credits_00";
            }
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            gfx.BeginDraw();
            string c = "No component";
            string p = "No person";
            string r = "No role";
            //ha. CPR. Get it?

            if(_componentIndex > -1 && _componentIndex < _creditsFile.Components.Length)
            {
                c = _creditsFile.Components[_componentIndex].Name;
                if(_personIndex > -1 && _personIndex < _creditsFile.Components[_componentIndex].People.Length)
                {
                    p = _creditsFile.Components[_componentIndex].People[_personIndex].Name;
                    r = _creditsFile.Components[_componentIndex].People[_personIndex].Role;
                }
            }

            var cMeasure = TextRenderer.MeasureText(c, _mondaBig, (_ui.ScreenWidth / 4), TextAlignment.TopLeft, Plex.Engine.TextRenderers.WrapMode.Words);
            var pMeasure = TextRenderer.MeasureText(p, _mondaMedium, (_ui.ScreenWidth / 4), TextAlignment.TopLeft, Plex.Engine.TextRenderers.WrapMode.Words);
            var rMeasure = TextRenderer.MeasureText(r, _mondaSmall, (_ui.ScreenWidth / 4), TextAlignment.TopLeft, Plex.Engine.TextRenderers.WrapMode.Words);

            var startX = (_ui.ScreenWidth - (_ui.ScreenWidth / 3)) / 2;
            var startY = (_ui.ScreenHeight - (_ui.ScreenHeight / 3)) / 2;

            var _titleMinX = 0 - (int)cMeasure.X;

            var titleX = (int)MathHelper.Lerp(_titleMinX, startX, _componentSlide);
            gfx.DrawString(c, titleX, startY, _peace * _componentSlide, _mondaBig, TextAlignment.TopLeft, (_ui.ScreenWidth / 3), Plex.Engine.TextRenderers.WrapMode.Words);

            var _personYMax = startY + cMeasure.Y + 25;
            var _personYMin = _personYMax + (_ui.ScreenHeight * 0.1);

            var personY = (int)MathHelper.Lerp((float)_personYMin, _personYMax, _personSlide);
            gfx.DrawString(p, startX, personY, _gray * _personSlide, _mondaMedium, TextAlignment.TopLeft, (_ui.ScreenWidth / 3), Plex.Engine.TextRenderers.WrapMode.Words);

            var roleYMax = _personYMax + pMeasure.Y + 10;
            var roleYMin = roleYMax + (_ui.ScreenHeight * 0.1);
            var roleY = (int)MathHelper.Lerp((float)roleYMin, roleYMax, _roleSlide);
            gfx.DrawString(r, startX, roleY, _gray * _roleSlide, _mondaSmall, TextAlignment.TopLeft, (_ui.ScreenWidth / 3), Plex.Engine.TextRenderers.WrapMode.Words);


            int pnWidth = _peacenet.Width * 2;
            int pnHeight = _peacenet.Height * 2;

            int pnX = (_ui.ScreenWidth - pnWidth) / 2;
            int pnYMax = (_ui.ScreenHeight - pnHeight) / 2;
            int pnYMin = pnYMax + (int)(_ui.ScreenHeight * 0.1);

            int pnY = (int)MathHelper.Lerp(pnYMin, pnYMax, _peacenetFade);
            gfx.DrawRectangle(pnX, pnY, pnWidth, pnHeight, _peacenet, Color.White * _peacenetFade);

            string thanks = "Thanks for playing.";
            var thanksMeasure = TextRenderer.MeasureText(thanks, _mondaBig, int.MaxValue, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);

            int thanksX = (_ui.ScreenWidth - (int)thanksMeasure.X) / 2;
            int thanksYMax = (pnY + pnHeight + 25);
            int thanksYMin = thanksYMax + (int)(_ui.ScreenHeight * 0.1);
            int thanksY = (int)MathHelper.Lerp(thanksYMin, thanksYMax, _thanksFade);
            gfx.DrawString(thanks, thanksX, thanksY, _gray * _thanksFade, _mondaBig, TextAlignment.Middle, (int)thanksMeasure.X, Plex.Engine.TextRenderers.WrapMode.Words);

            gfx.EndDraw();
        }

        public override void LoadResources()
        {
            _yesMyGrassIsGreen = Content.Load<SoundEffect>("Audio/Cutscene/Credits");
            _grassInstance = _yesMyGrassIsGreen.CreateInstance();
            string creditsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "credits.json");
            string json = File.ReadAllText(creditsPath);
            _creditsFile = JsonConvert.DeserializeObject<CreditsFile>(json);
            _mondaBig = new System.Drawing.Font("Monda", 24F, System.Drawing.FontStyle.Bold);
            _mondaMedium = new System.Drawing.Font("Monda", 16F);
            _mondaSmall = new System.Drawing.Font("Monda", 12F);
            _peacenet = Content.Load<Texture2D>("Splash/Peacenet");
        }

        public override void OnFinish()
        {
            _grassInstance.Stop();
        }

        public override void OnPlay()
        {
            _csState = 0;
            _grassInstance.Play();
        }

        public override void UnloadResources()
        {
            _yesMyGrassIsGreen.Dispose();
            _peacenet.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            switch (_csState)
            {
                case 0:
                    _componentIndex++;
                    _csState++;
                    break;
                case 1:
                    _componentSlide += (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_componentSlide >= 1)
                    {
                        _personIndex = -1;
                        _csState++;
                    }
                    break;
                case 2:
                    _personIndex++;
                    _csState++;
                    break;
                case 3:
                    _personSlide += (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_personSlide >= 1)
                    {
                        _csState++;
                    }

                    break;
                case 4:
                    _roleSlide += (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_roleSlide >= 1)
                    {
                        _personRide=0;
                        _csState++;
                    }

                    break;
                case 5:
                    _personRide += gameTime.ElapsedGameTime.TotalSeconds;
                    if(_personRide >= 5)
                    {
                        _csState++;
                    }
                    break;
                case 6:
                    _personSlide -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_personSlide <= 0)
                    {
                        _csState++;
                    }
                    break;
                case 7:
                    _roleSlide -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_roleSlide <= 0)
                    {
                        if (_personIndex < _creditsFile.Components[_componentIndex].People.Length - 1)
                        {
                            _csState = 2;
                        }
                        else
                        {
                            _csState++;
                        }
                    }
                    break;
                case 8:
                    _componentSlide -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    if (_componentSlide <= 0)
                    {
                        if(_componentIndex < _creditsFile.Components.Length - 1)
                        {
                            _csState = 0;
                        }
                        else
                        {
                            _csState++;
                            _personRide = 0;
                        }
                    }

                    break;
                case 9:
                    _personRide += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_personRide > 2.5)
                        _csState++;
                    break;
                case 10:
                    _peacenetFade += (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                    if (_peacenetFade >= 1)
                        _csState++;
                    break;
                case 11:
                    _personRide = 0;
                    _thanksFade += (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                    if (_thanksFade >= 1)
                        _csState++;
                    break;
                case 12:
                    _personRide += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_personRide > 5)
                        _csState++;

                    break;
                case 13:
                    _peacenetFade -= (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                    if (_peacenetFade <= 0)
                        _csState++;

                    break;
                case 14:
                    _thanksFade -= (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                    if (_thanksFade <= 0)
                        _csState++;

                    break;
                case 15:
                    float volume = _grassInstance.Volume;
                    volume = MathHelper.Clamp(volume - (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                    _grassInstance.Volume = volume;
                    if(volume <= 0)
                    {
                        NotifyFinished();
                    }
                    break;
            }
        }
    }

    public class CreditsFile
    {
        public Component[] Components { get; set; }
    }

    public class Component
    {
        public string Name { get; set; }
        public Person[] People { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
