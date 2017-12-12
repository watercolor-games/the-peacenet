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
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Cutscene;
using Plex.Engine.Saves;
using Plex.Engine.Server;

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

        [Dependency]
        private CutsceneManager _cutscene = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private AsyncServerManager _server = null;


        private SoundEffect _andersJensenDreamsInc = null;
        private SoundEffectInstance _dreamsIncInstance = null;
        private bool _hasEnteredMenu = false;
        private SoundEffect _introEffect = null;
        private SoundEffectInstance _introInstance = null;
        private bool _alreadyPlayedIntro = false;
        private SoundEffect _mainSong = null;
        private SoundEffectInstance _mainSongInstance = null;
        private bool _playedMainSong = false;

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

        private Label _lbSingleplayer = null;
        private Label _lbMultiplayer = null;
        private Label _lbSettings = null;


        //Anim values for menu items
        private float _spSlideUp = 0.0F;
        private float _mpSlideUp = 0.0F;
        private float _seSlideUp = 0.0F;

        private float _menuLabelOpacity = 0.0f;

        private GameSettings _settingsApp = null;

        private int animState = 0;

        private bool _hasVolumeBeenAdjusted = true;

        [Dependency]
        private SaveManager _saveManager = null;

        [Dependency]
        private OS _os = null;

        public void Reset()
        {
            animState = 0;
            _alreadyPlayedIntro = false;
            _hasEnteredMenu = false;
            _wgRide = 0;
            _progressFGAmount = 0;
            _os.Shutdown();
        }

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
            _hbMultiplayer.Click += (o, a) =>
            {
                _infobox.PromptText("Connect to server", "Please enter a hostname and port for a server to connect to.", (address) =>
                {
                    if (address.Split(':').Length == 1)
                        address += ":3251";
                    _server.Connect(address, () =>
                    {
                        _infobox.Show("Connection successful.", $"You have established a TCP connection to {address} :D");
                    }, (error) =>
                    {
                        _infobox.Show("Connection error", $"Could not connect:{Environment.NewLine}{Environment.NewLine}{error}");
                    });
                });
            };
            _hbSingleplayer.Click += (o,a) =>
            {
                if (animState < 16)
                {
                    var savefiles = _saveManager.GetSavePaths();
                    if(savefiles.Length < 1)
                    {
                        _saveManager.CreateSinglePlayerSave();
                    }
                    else
                    {
                        _saveManager.StartSinglePlayerSession(savefiles[0]);
                    }
                    animState = 16;
                }

            };

            _andersJensenDreamsInc = _plexgate.Content.Load<SoundEffect>("Audio/MainMenu/PressEnter2");
            _dreamsIncInstance = _andersJensenDreamsInc.CreateInstance();

            _introEffect = _plexgate.Content.Load<SoundEffect>("Audio/MainMenu/PressEnter1");
            _introInstance = _introEffect.CreateInstance();

            _mainSong = _plexgate.Content.Load<SoundEffect>("Audio/MainMenu/MainSong");
            _mainSongInstance = _mainSong.CreateInstance();

            _lbSingleplayer = new Label();
            _lbMultiplayer = new Label();
            _lbSettings = new Label();

            _uimanager.Add(_lbSingleplayer);
            _uimanager.Add(_lbMultiplayer);
            _uimanager.Add(_lbSettings);

            _uimanager.Add(_credits);
            _credits.Text = "Credits";
            _credits.ShowImage = true;
            _credits.Image = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/Credits");
            _credits.Click += (o, a) =>
            {
                _cutscene.Play("credits_00");
            };
        }

        private Button _credits = new Button();

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            if (_cutscene.IsPlaying == true)
                return;
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
            ctx.EndDraw();
        }

        private System.Drawing.Font _titlefont = new System.Drawing.Font("Monda", 15F);



        public void OnGameUpdate(GameTime time)
        {
            if (_cutscene.IsPlaying)
            {
                if (_hasVolumeBeenAdjusted)
                {
                    float volume = _dreamsIncInstance.Volume;
                    volume -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (volume <= 0)
                        _hasVolumeBeenAdjusted = false;
                    volume = MathHelper.Clamp(volume, 0, 1);
                    _dreamsIncInstance.Volume = volume;
                    _introInstance.Volume = volume;
                    this._mainSongInstance.Volume = volume;
                }
                
            }
            else
            {
                if (_hasVolumeBeenAdjusted == false)
                {
                    float volume = _dreamsIncInstance.Volume;
                    volume += (float)time.ElapsedGameTime.TotalSeconds;
                    if (volume >= 1)
                        _hasVolumeBeenAdjusted = true;
                    volume = MathHelper.Clamp(volume, 0, 1);
                    _dreamsIncInstance.Volume = volume;
                    _introInstance.Volume = volume;
                    this._mainSongInstance.Volume = volume;
                }
            }

            if (_introInstance.State == SoundState.Stopped)
            {
                if (_alreadyPlayedIntro)
                {
                    if(_dreamsIncInstance.State == SoundState.Stopped)
                    {
                        if (_hasEnteredMenu == false)
                        {
                            _dreamsIncInstance.Play();
                        }
                        else
                        {
                            if (_playedMainSong == false)
                            {
                                _mainSongInstance.Play();
                                _playedMainSong = true;
                            }
                        }
                    }
                }
                else
                {
                    _introInstance.Play();
                    _alreadyPlayedIntro = true;
                }
            }

            switch (animState)
            {
                case 0: //Start Watercolor splash
                    _lbSingleplayer.Visible = false;
                    _lbMultiplayer.Visible = false;
                    _lbSettings.Visible = false;
                    _hbSingleplayer.Visible = false;
                    _hbSettings.Visible = false;
                    _hbMultiplayer.Visible = false;
                    _credits.Visible = false;
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
                case 3: //End Watercolor splash, start Peacenet splash
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
                case 6: //End Peacenet splash, start Enter wait.
                    _progressBGFade -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    _progressFGPos += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos >= 1)
                    {
                        animState++;
                    }
                    break;
                case 8: //End Enter wait, start Peacenet splash -> Menu transition.
                    _progressFGPos -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos <= 0)
                        animState++;
                    break;
                case 9:
                    _peacenetOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_peacenetOpacity <= 0)
                        animState++;

                    break;
                case 10: //Start Menu animation.
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
                    _lbSingleplayer.Visible = true;
                    _lbMultiplayer.Visible = true;
                    _lbSettings.Visible = true;
                    animState++;
                    break;
                case 14: //End Menu animation.
                    _menuLabelOpacity += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_menuLabelOpacity >= 1)
                    {
                        animState++;
                        _credits.Visible = true;
                    }
                    break;
                case 16: //Start Menu Unload Animation.
                    _menuLabelOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_menuLabelOpacity <= 0)
                    {
                        animState++;
                        _credits.Visible = false;
                        _hbSettings.Visible = false;
                        _hbSingleplayer.Visible = false;
                        _hbMultiplayer.Visible = false;
                        _lbSingleplayer.Visible = false;
                        _lbMultiplayer.Visible = false;
                        _lbSettings.Visible = false;

                    }

                    break;
                case 17:
                    _spSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_spSlideUp <= 0)
                        animState++;

                    break;
                case 18:
                    _mpSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_mpSlideUp <= 0)
                        animState++;

                    break;
                case 19:
                    _seSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_seSlideUp <= 0)
                        animState++;

                    break;
                case 20:
                    float bgmVolume = _dreamsIncInstance.Volume;
                    bgmVolume = MathHelper.Clamp(bgmVolume - (float)time.ElapsedGameTime.TotalSeconds * 4, 0, 1);
                    _dreamsIncInstance.Volume = bgmVolume;
                    _introInstance.Volume = bgmVolume;
                    this._mainSongInstance.Volume = bgmVolume;
                    if(bgmVolume <= 0)
                    {
                        animState++;
                    }
                    break;
                case 21:
                    _dreamsIncInstance.Stop();
                    _mainSongInstance.Stop();
                    _introInstance.Stop();
                    _os.OnReady();
                    animState++;
                    break;

            }

            _lbSingleplayer.Opacity = _menuLabelOpacity;
            _lbMultiplayer.Opacity = _menuLabelOpacity;
            _lbSettings.Opacity = _menuLabelOpacity;

            int labelYMax = _hbMultiplayer.Y + _hbMultiplayer.Height + 25;
            int labelYMin = labelYMax + (int)(_uimanager.ScreenHeight * 0.1);
            int labelY = (int)MathHelper.Lerp(labelYMin, labelYMax, _menuLabelOpacity);

            _lbSingleplayer.Y = labelY;
            _lbMultiplayer.Y = labelY;
            _lbSettings.Y = labelY;

            _lbSingleplayer.AutoSize = true;
            _lbSingleplayer.FontStyle = TextFontStyle.Custom;
            _lbSingleplayer.CustomFont = _titlefont;

            _lbMultiplayer.AutoSize = true;
            _lbMultiplayer.FontStyle = TextFontStyle.Custom;
            _lbMultiplayer.CustomFont = _titlefont;

            _lbSettings.AutoSize = true;
            _lbSettings.FontStyle = TextFontStyle.Custom;
            _lbSettings.CustomFont = _titlefont;

            _lbSingleplayer.Text = "Single Player";
            _lbMultiplayer.Text = "Multiplayer";
            _lbSettings.Text = "Settings";

            _lbSingleplayer.MaxWidth = _hbSingleplayer.Width;
            _lbSingleplayer.X = _hbSingleplayer.X + ((_hbSingleplayer.Width - _lbSingleplayer.Width) / 2);

            _lbMultiplayer.MaxWidth = _hbMultiplayer.Width;
            _lbMultiplayer.X = _hbMultiplayer.X + ((_hbMultiplayer.Width - _lbMultiplayer.Width) / 2);

            _lbSettings.MaxWidth = _hbSettings.Width;
            _lbSettings.X = _hbSettings.X + ((_hbSettings.Width - _lbSettings.Width) / 2);

            var colorIdle = new Color(191, 191, 191, 255);
            var colorHover = Color.White;

            _lbSingleplayer.CustomColor = (_hbSingleplayer.ContainsMouse) ? colorHover : colorIdle;
            _lbMultiplayer.CustomColor = (_hbMultiplayer.ContainsMouse) ? colorHover : colorIdle;
            _lbSettings.CustomColor = (_hbSettings.ContainsMouse) ? colorHover : colorIdle;

            _credits.X = 15;
            _credits.Y = (_uimanager.ScreenHeight - _credits.Height) - 15;
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (animState == 7)
                {
                    _hasEnteredMenu = true;
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
