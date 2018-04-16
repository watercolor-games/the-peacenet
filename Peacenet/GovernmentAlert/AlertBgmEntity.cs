using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine;
using Plex.Engine.Themes;

namespace Peacenet.GovernmentAlert
{
    public class AlertBgmEntity : IEntity, ILoadable
    {
        private SoundEffect[] _bgm = null;
        private SoundEffectInstance _current = null;

        [Dependency]
        private AlertService _alert = null;

        [Dependency]
        private ThemeManager _theme = null;

        private bool _doneIntro = false;

        private Random _rnd = new Random();

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            var headerFont = _theme.Theme.GetFont(TextFontStyle.Header1);
            var highlightFont = _theme.Theme.GetFont(TextFontStyle.Highlight);

            string header = "Government Alert";
            string highlight = "Your system has alerted the Government due to frequent notorious actions.";

            var headerMeasure = gfx.TextRenderer.MeasureText(header, headerFont, gfx.Width / 2, WrapMode.Words);
            var highlightMeasure = gfx.TextRenderer.MeasureText(highlight, highlightFont, gfx.Width / 2, WrapMode.Words);

            float y = (gfx.Height - (headerMeasure.Y + highlightMeasure.Y + 5)) / 2;


            gfx.BeginDraw();
            gfx.DrawString(header, (gfx.Width - (gfx.Width / 2)) / 2, (int)y, _theme.Theme.GetFontColor(TextFontStyle.Header1) * _screenShroudOpacity, headerFont, TextAlignment.Center, gfx.Width / 2, WrapMode.Words);
            gfx.DrawString(highlight, (gfx.Width - (gfx.Width / 2)) / 2, (int)(y+headerMeasure.Y+5), _theme.Theme.GetFontColor(TextFontStyle.Highlight) * _screenShroudOpacity, highlightFont, TextAlignment.Center, gfx.Width / 2, WrapMode.Words);
            gfx.EndDraw();
        }

        public void Load(ContentManager content)
        {
            _bgm = content.LoadAllIn<SoundEffect>("Audio/GovernmentAlert");

        }

        private float _screenShroudOpacity = 0f;

        public void OnGameExit()
        {
            if(_current != null)
            {
                _current.Stop();
                _current = null;
            }
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void Update(GameTime time)
        {
            if(_alert.AlertLevel > 1)
            {
                if(_current == null)
                {
                    _current = _bgm[_rnd.Next(_bgm.Length)].CreateInstance();
                    _current.Play();
                }
                else
                {
                    if(_current.State != SoundState.Playing)
                    {
                        _current = null;
                    }
                }
                if(_doneIntro==false)
                {
                    _doneIntro = true;
                    _screenShroudOpacity = 1f;
                }
                
            }
            else
            {
                if (_alert.AlertLevel == 0)
                {
                    if (_current != null)
                    {
                        _current.Volume = MathHelper.Clamp(_current.Volume - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                        if (_current.Volume <= 0)
                        {
                            _current.Stop();
                            _current = null;
                        }
                    }
                }
                _doneIntro = false;
            }
            _screenShroudOpacity = MathHelper.Clamp(_screenShroudOpacity - ((float)time.ElapsedGameTime.TotalSeconds / 5), 0, 1);
        }
    }
}
