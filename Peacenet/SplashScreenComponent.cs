using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Themes;
using Plex.Engine.GUI;
using Peacenet.Applications;

namespace Peacenet
{
    public class SplashScreenComponent : IEngineComponent
    {
        [Dependency]
        private UIManager _uimanager = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ThemeManager _themeManager = null;

        [Dependency]
        private WindowSystem _windowManager = null;

        private Texture2D _watercolor = null;
        private Texture2D _peacenet = null;
        private Texture2D _welcome = null;

        private float _wgFade = 0.0f;
        private double _wgRide = 0.0;
        private float _peacenetSlideLeft = 0.0f;
        private float _peacenetOpacity = 0.0f;

        private float _progressBGFade = 0.0f;
        private float _progressFGAmount = 0.0f;
        private float _progressFGPos = 0.0f;

        private const int _progressHeight = 26;

        //Textures for menu items
        private Texture2D _singleplayer = null;
        private Texture2D _multiplayer = null;
        private Texture2D _settings = null;

        //Hitboxes for menu items
        private Hitbox _hbSingleplayer = null;
        private Hitbox _hbMultiplayer = null;
        private Hitbox _hbSettings = null;

        //Anim values for menu items
        private float _spSlideUp = 0.0F;
        private float _mpSlideUp = 0.0F;
        private float _seSlideUp = 0.0F;

        private float _menuLabelOpacity = 0.0f;

        private GameSettings _settingsApp = null;

        private int animState = 0;

        public void Initiate()
        {
            _watercolor = _plexgate.Content.Load<Texture2D>("Splash/Watercolor");
            _peacenet = _plexgate.Content.Load<Texture2D>("Splash/Peacenet");
            _welcome = _plexgate.Content.Load<Texture2D>("Splash/Welcome");

            _singleplayer = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/SinglePlayer");
            _multiplayer = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/MultiPlayer");
            _settings = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/Settings");

            _hbSingleplayer = new Hitbox();
            _hbMultiplayer = new Hitbox();
            _hbSettings = new Hitbox();

            _uimanager.Add(_hbSettings);
            _uimanager.Add(_hbSingleplayer);
            _uimanager.Add(_hbMultiplayer);

            _settingsApp = new GameSettings(_windowManager);

            _hbSettings.Click += (o, a) =>
            {
                if (_settingsApp.Disposed)
                    _settingsApp = new GameSettings(_windowManager);

                _settingsApp.Show();

            };
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            ctx.BeginDraw();
            ctx.DrawRectangle(0, 0, _uimanager.ScreenWidth, _uimanager.ScreenHeight, _watercolor, Color.White * _wgFade, System.Windows.Forms.ImageLayout.Zoom, true);

            int peacenet_y = (_uimanager.ScreenHeight - _peacenet.Height) / 2;
            int peacenet_x_min = (0 - _peacenet.Width);
            int peacenet_x_max = (_uimanager.ScreenWidth - _peacenet.Width) / 2;

            int welcome_x = peacenet_x_max;
            int welcome_y_max = peacenet_y - _welcome.Height;
            int welcome_y_min = welcome_y_max - (int)(_uimanager.ScreenHeight * 0.15);

            int progressWidth = (_peacenet.Width - 50);
            int progressX = (_uimanager.ScreenWidth - progressWidth) / 2;
            int progressY = (peacenet_y + _peacenet.Height + 30);


            ctx.DrawRectangle((int)MathHelper.Lerp(peacenet_x_min, peacenet_x_max, _peacenetSlideLeft), peacenet_y, _peacenet.Width, _peacenet.Height, _peacenet, Color.White * _peacenetOpacity, System.Windows.Forms.ImageLayout.Zoom, true);
            ctx.DrawRectangle(welcome_x, (int)MathHelper.Lerp(welcome_y_min, welcome_y_max, _peacenetSlideLeft), _welcome.Width, _welcome.Height, _welcome, Color.White * _peacenetOpacity, System.Windows.Forms.ImageLayout.Zoom, false);

            //fake progress bar
            if (_progressBGFade > 0)
            {
                ctx.DrawRectangle(progressX, progressY, progressWidth, _progressHeight, Color.White * _progressBGFade);
                ctx.DrawRectangle(progressX + 2, (int)MathHelper.Lerp(progressY + 2, progressY - (int)(_uimanager.ScreenHeight * 0.1), _progressFGPos), (int)MathHelper.Lerp(0, progressWidth - 4, _progressFGAmount), _progressHeight - 4, _themeManager.Theme.GetAccentColor() * _progressBGFade);
            }

            //"Press ENTER" prompt
            var fnt = new System.Drawing.Font("Monda", 15F);
            if (_progressFGPos > 0)
            {
                string _enter = "Press ENTER to continue";
                var measure = TextRenderer.MeasureText(_enter, fnt, _peacenet.Width, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);

                int textX = (int)(_uimanager.ScreenWidth - measure.X) / 2;
                int textYMin = progressY + (int)(_uimanager.ScreenHeight * 0.1);

                ctx.DrawString(_enter, textX, (int)MathHelper.Lerp(textYMin, progressY, _progressFGPos), Color.White * _progressFGPos, fnt, TextAlignment.Middle, (int)measure.X, Plex.Engine.TextRenderers.WrapMode.Words);
            }

            //Draw menu items.

            //Layout multiplayer
            _hbMultiplayer.Width = _multiplayer.Width;
            _hbMultiplayer.Height = _multiplayer.Height;
            _hbMultiplayer.X = (_uimanager.ScreenWidth - _hbMultiplayer.Width) / 2;
            int proposedMultiplayerY = (_uimanager.ScreenHeight - _hbMultiplayer.Height) / 2;
            _hbMultiplayer.Y = (int)MathHelper.Lerp(proposedMultiplayerY + (int)(_uimanager.ScreenHeight * 0.1), proposedMultiplayerY, _mpSlideUp);

            //Singleplayer layout
            _hbSingleplayer.Width = _singleplayer.Width;
            _hbSingleplayer.Height = _singleplayer.Height;
            _hbSingleplayer.X = (_hbMultiplayer.X - 5) - _hbSettings.Width;
            _hbSingleplayer.Y = (int)MathHelper.Lerp(proposedMultiplayerY + (int)(_uimanager.ScreenHeight * 0.1), proposedMultiplayerY, _spSlideUp);

            //Settings layout.

            //Singleplayer layout
            _hbSettings.Width = _settings.Width;
            _hbSettings.Height = _settings.Height;
            _hbSettings.X = _hbMultiplayer.X + _hbMultiplayer.Width + 5;
            _hbSettings.Y = (int)MathHelper.Lerp(proposedMultiplayerY + (int)(_uimanager.ScreenHeight * 0.1), proposedMultiplayerY, _seSlideUp);

            //Now draw the glyphs.
            var colorIdle = new Color(191, 191, 191, 255);
            var colorHover = Color.White;
            if(_spSlideUp>0)
                ctx.DrawRectangle(_hbSingleplayer.X, _hbSingleplayer.Y, _hbSingleplayer.Width, _hbSingleplayer.Height, _singleplayer, ((_hbSingleplayer.ContainsMouse) ? colorHover : colorIdle)*_spSlideUp);
            if (_mpSlideUp > 0)
                ctx.DrawRectangle(_hbMultiplayer.X, _hbMultiplayer.Y, _hbMultiplayer.Width, _hbMultiplayer.Height, _multiplayer, ((_hbMultiplayer.ContainsMouse) ? colorHover : colorIdle)*_mpSlideUp);
            if (_seSlideUp > 0)
                ctx.DrawRectangle(_hbSettings.X, _hbSettings.Y, _hbSettings.Width, _hbSettings.Height, _settings, ((_hbSettings.ContainsMouse) ? colorHover : colorIdle)*_seSlideUp);
            if (_menuLabelOpacity > 0)
            {
                string text_sp = "Singleplayer";
                string text_mp = "Multiplayer";
                string text_settings = "Settings";

                var sp_measure = TextRenderer.MeasureText(text_sp, fnt, _hbSingleplayer.Width, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);
                var mp_measure = TextRenderer.MeasureText(text_mp, fnt, _hbMultiplayer.Width, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);
                var settings_measure = TextRenderer.MeasureText(text_settings, fnt, _hbSettings.Width, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.Words);

                int proposedMenuTextY = _hbMultiplayer.Y + _hbMultiplayer.Height + 30;
                int realTextY = (int)MathHelper.Lerp(proposedMenuTextY + (int)(_uimanager.ScreenHeight * 0.10), proposedMenuTextY, _menuLabelOpacity);

                int spX = _hbSingleplayer.X + ((_hbSingleplayer.Width - (int)sp_measure.X) / 2);
                int mpX = _hbMultiplayer.X + ((_hbMultiplayer.Width - (int)mp_measure.X) / 2);
                int seX = _hbSettings.X + ((_hbSettings.Width - (int)settings_measure.X) / 2);

                ctx.DrawString(text_sp, spX, realTextY, ((_hbSingleplayer.ContainsMouse) ? colorHover : colorIdle) * _menuLabelOpacity, fnt, TextAlignment.Middle, (int)sp_measure.X, Plex.Engine.TextRenderers.WrapMode.Words);
                ctx.DrawString(text_mp, mpX, realTextY, ((_hbMultiplayer.ContainsMouse) ? colorHover : colorIdle) * _menuLabelOpacity, fnt, TextAlignment.Middle, (int)mp_measure.X, Plex.Engine.TextRenderers.WrapMode.Words);
                ctx.DrawString(text_settings, seX, realTextY, ((_hbSettings.ContainsMouse) ? colorHover : colorIdle) * _menuLabelOpacity, fnt, TextAlignment.Middle, (int)settings_measure.X, Plex.Engine.TextRenderers.WrapMode.Words);
            }
            ctx.EndDraw();
        }

        public void OnGameUpdate(GameTime time)
        {
            switch (animState)
            {
                case 0:
                    _hbSingleplayer.Visible = false;
                    _hbSettings.Visible = false;
                    _hbMultiplayer.Visible = false;

                    _wgFade += (float)(time.ElapsedGameTime.TotalSeconds*2.5);
                    if(_wgFade >= 1)
                    {
                        animState++;
                    }
                    break;
                case 1:
                    _wgRide += time.ElapsedGameTime.TotalSeconds;
                    if (_wgRide >= 2.5F)
                    {
                        animState++;
                    }
                    break;
                case 2:
                    _wgFade -= (float)(time.ElapsedGameTime.TotalSeconds*2.5);
                    if (_wgFade <= 0)
                    {
                        animState++;
                    }
                    break;
                case 3:
                    _peacenetOpacity += (float)(time.ElapsedGameTime.TotalSeconds*2.5);
                    _peacenetOpacity = MathHelper.Clamp(_peacenetOpacity, 0, 1);
                    _peacenetSlideLeft = _peacenetOpacity;
                    if (_peacenetOpacity >= 1)
                    {
                        animState++;
                    }
                    break;
                case 4:
                    _progressBGFade += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressBGFade >= 1)
                        animState++;
                    break;
                case 5:
                    _progressFGAmount += (float)time.ElapsedGameTime.TotalSeconds / 2.5F;
                    if (_progressFGAmount >= 1)
                        animState++;
                    break;
                case 6:
                    _progressBGFade -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    _progressFGPos += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos >= 1)
                        animState++;
                    break;
                case 8:
                    _progressFGPos -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos <= 0)
                        animState++;
                    break;
                case 9:
                    _peacenetOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_peacenetOpacity <= 0)
                        animState++;

                    break;
                case 10:
                    _spSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_spSlideUp >= 1)
                        animState++;
                    break;
                case 11:
                    _mpSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_mpSlideUp >= 1)
                        animState++;
                    break;
                case 12:
                    _seSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_seSlideUp >= 1)
                        animState++;
                    break;
                case 13:
                    _hbSettings.Visible = true;
                    _hbSingleplayer.Visible = true;
                    _hbMultiplayer.Visible = true;
                    animState++;
                    break;
                case 14:
                    _menuLabelOpacity += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_menuLabelOpacity >= 1)
                        animState++;
                    break;


            }
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (animState == 7)
                {
                    animState++;
                }
            }
        }

        public void Unload()
        {
            _watercolor.Dispose();
            _peacenet.Dispose();
            _welcome.Dispose();
            _uimanager.Remove(_hbSingleplayer);
            _uimanager.Remove(_hbMultiplayer);
            _uimanager.Remove(_hbSettings);
            _hbSingleplayer = null;
            _hbMultiplayer = null;
            _hbSettings = null;
        }
    }
}
