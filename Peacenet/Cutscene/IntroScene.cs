using Plex.Engine.Cutscene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine;

namespace Peacenet.Cutscenes
{
    public class IntroScene : Cutscene
    {
        [Dependency]
        private UIManager _ui = null;


        private SoundEffect _serenity = null;
        private SoundEffectInstance _serenityInstance = null;

        private System.Drawing.Font _monda;

        private int _csState = 0;
        private float _textFade = 0F;
        private int _textIndex = 0;
        private double _textLast = 0;
        private readonly string[] _introMessages =
        {
            "Hello.",
            "I regret to inform you...",
            "That you are dead.",
            "How, why, you don't know.",
            "And I can't tell you.",
            "But I can tell you where you are.",
            "And what you are.",
            "But first, I must know your name.",
        };

        public override string Name
        {
            get
            {
                return "intro_00";
            }
        }

        public override void Draw(GameTime time, GraphicsContext gfx)
        {
            if (_textIndex > -1)
            {
                string _intro = _introMessages[_textIndex];

                var iMeasure = TextRenderer.MeasureText(_intro, _monda, (_ui.ScreenWidth / 2), TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);

                int x = (_ui.ScreenWidth - (int)iMeasure.X) / 2;
                int y = (_ui.ScreenHeight - (int)iMeasure.Y) / 2;
                gfx.BeginDraw();
                gfx.DrawString(_intro, x, y, new Color(191, 191, 191) * _textFade, _monda, TextAlignment.Middle, (int)iMeasure.X, Plex.Engine.TextRenderers.WrapMode.Words);
                gfx.EndDraw();
            }
        }

        public override void LoadResources()
        {
            _serenity = Content.Load<SoundEffect>("Audio/Cutscene/Intro/Serenity");
            _serenityInstance = _serenity.CreateInstance();
            _monda = new System.Drawing.Font("Monda", 28F);
        }

        public override void UnloadResources()
        {
            _serenity.Dispose();
            _serenityInstance = null;
            _serenity = null;
        }

        public override void Update(GameTime gameTime)
        {
            switch (_csState)
            {
                case 0:
                    _serenityInstance.Volume = 0;
                    _serenityInstance.Play();
                    _csState++;
                    break;
                case 1:
                    float volume = _serenityInstance.Volume;
                    volume = MathHelper.Clamp(volume + (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                    _serenityInstance.Volume = volume;
                    if(volume >= 1)
                    {
                        _csState++;
                    }
                    break;
                case 2:
                    _textIndex++;
                    _csState++;
                    break;
                case 3:
                    _textLast = 0;
                    _textFade += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_textFade >= 1)
                    {
                        _csState++;
                    }
                    break;
                case 4:
                    _textLast += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_textLast >= 5)
                    {
                        _csState++;
                    }
                    break;
                case 5:
                    _textFade -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(_textFade <= 0)
                    {
                        if(_textIndex < _introMessages.Length-1)
                        {
                            _csState = 2;
                        }
                        else
                        {
                            _csState++;
                        }
                    }
                    break;
                case 6:
                    float nvolume = _serenityInstance.Volume;
                    nvolume = MathHelper.Clamp(nvolume - (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                    _serenityInstance.Volume = nvolume;
                    if (nvolume <= 0)
                    {
                        _csState++;
                    }

                    break;
                case 7:
                    NotifyFinished();
                    _csState++;
                    break;
            }
        }

        public override void OnPlay()
        {
            _csState = 0;
            _textFade = 0;
            _textIndex = -1;
        }

        public override void OnFinish()
        {
            _serenityInstance.Stop();
        }
    }
}
