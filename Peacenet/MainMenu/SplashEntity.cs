using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Content;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GUI;
using floaty = System.Single;
using Plex.Engine.Themes;
using Peacenet.Applications;
using Plex.Engine.Saves;
using Plex.Engine.Cutscene;
using Peacenet.RichPresence;
using Peacenet.Server;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Cutscenes;
using System.IO;

namespace Peacenet.MainMenu
{
    /// <summary>
    /// A Peace engine entity which displays the Watercolor Games splash screen and Peacenet's main menu.
    /// </summary>
    public class SplashEntity : IEntity, ILoadable, IDisposable
    {
        /// <inheritdoc/>
        public void OnGameExit() { }

        #region Animation state

        private IVideoFormat vid;
        private Stream vidf;
        private VideoPlayer vidp;
        private int animState = -1;
        private floaty _wgFade = 0;
        private floaty _peacenetSlideLeft = 0;
        private floaty _peacenetOpacity = 0;
        private floaty _progressFGPos = 0;
        private floaty _mpSlideUp = 0;
        private floaty _spSlideUp = 0;
        private floaty _seSlideUp = 0;
        private bool _hasEnteredMenu = false;
        private double _wgRide = 0;
        private floaty _progressFGFade = 0;
        private floaty _progressBGFade = 0;
        private floaty _menuLabelOpacity = 0;
        private floaty _wgUserFadeIn = 0;
        private System.Drawing.Font _titlefont = new System.Drawing.Font("Monda", 15F);

        #endregion

        #region Textures

        private Texture2D _watercolor = null;
        private Texture2D _peacenet = null;
        private Texture2D _welcome = null;
        private Texture2D _multiplayer = null;
        private Texture2D _singleplayer = null;
        private Texture2D _settings = null;

        #endregion

        #region Sound Effects

        private SoundEffect _watercolorSplash = null;
        private SoundEffectInstance _wgsInstance = null;

        #endregion

        #region Engine dependencies

        [Dependency]
        private UIManager _uimanager = null;

        [Dependency]
        private ThemeManager _thememgr = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private WindowSystem _windowManager = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private SaveManager _saveManager = null;

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private SplashScreenComponent _splash = null;

        [Dependency]
        private CutsceneManager _cutscene = null;

        [Dependency]
        private ItchOAuthClient _api = null;

        [Dependency]
        private OS _os = null;

        #endregion

        #region Hitboxes

        private Hitbox _hbSingleplayer = null;
        private Hitbox _hbMultiplayer = null;
        private Hitbox _hbSettings = null;

        #endregion

        #region Text labels

        private Label _username = null;
        private Label _realname = null;
        private Label _lbSingleplayer = null;
        private Label _lbMultiplayer = null;
        private Label _lbSettings = null;

        #endregion

        #region Buttons

        private Button _wgButton = null;
        private Button _credits = null;
        private Button _exitButton = null;

        #endregion

        #region Window entities

        private GameSettings _settingsApp = null;
        
        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            (vid as IDisposable)?.Dispose();
            vidf?.Dispose();
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

        private bool _isWaitingForSignin = false;

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext ctx)
        {
            ctx.BeginDraw();

            int peacenet_y = (_uimanager.ScreenHeight - _peacenet.Height) / 2;
            int peacenet_x_min = (0 - _peacenet.Width);
            int peacenet_x_max = (_uimanager.ScreenWidth - _peacenet.Width) / 2;

            int welcome_x = peacenet_x_max;
            int welcome_y_max = peacenet_y - _welcome.Height;
            int welcome_y_min = welcome_y_max - (int)(_uimanager.ScreenHeight * 0.15);

            ctx.DrawRectangle(welcome_x, (int)MathHelper.Lerp(welcome_y_min, welcome_y_max, _peacenetOpacity), _welcome.Width, _welcome.Height, _welcome, Color.White * _peacenetOpacity, System.Windows.Forms.ImageLayout.Stretch);
            ctx.DrawRectangle((int)MathHelper.Lerp(peacenet_x_min, peacenet_x_max, _peacenetOpacity), peacenet_y, _peacenet.Width, _peacenet.Height, _peacenet, Color.White * _peacenetOpacity, System.Windows.Forms.ImageLayout.Stretch);

            //"Press ENTER" prompt
            var fnt = _thememgr.Theme.GetFont(TextFontStyle.Header1);
            if (_progressFGPos > 0)
            {
                string _enter = "Press ENTER to continue";
                var measure = TextRenderer.MeasureText(_enter, fnt, (this._uimanager.ScreenWidth/2), Plex.Engine.TextRenderers.WrapMode.Words);

                int textX = (int)(_uimanager.ScreenWidth - measure.X) / 2;
                int textYMin = peacenet_y + _peacenet.Height + 15 + (int)(_uimanager.ScreenHeight * 0.1);

                ctx.DrawString(_enter, textX, (int)MathHelper.Lerp(textYMin, peacenet_y + _peacenet.Height + 15, _progressFGPos), Color.White * _progressFGPos, fnt, TextAlignment.Center, (int)measure.X, Plex.Engine.TextRenderers.WrapMode.Words);
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
            if (_spSlideUp > 0)
                ctx.DrawRectangle(_hbSingleplayer.X, _hbSingleplayer.Y, _hbSingleplayer.Width, _hbSingleplayer.Height, _singleplayer, ((_hbSingleplayer.ContainsMouse) ? colorHover : colorIdle) * _spSlideUp);
            if (_mpSlideUp > 0)
                ctx.DrawRectangle(_hbMultiplayer.X, _hbMultiplayer.Y, _hbMultiplayer.Width, _hbMultiplayer.Height, _multiplayer, ((_hbMultiplayer.ContainsMouse) ? colorHover : colorIdle) * _mpSlideUp);
            if (_seSlideUp > 0)
                ctx.DrawRectangle(_hbSettings.X, _hbSettings.Y, _hbSettings.Width, _hbSettings.Height, _settings, ((_hbSettings.ContainsMouse) ? colorHover : colorIdle) * _seSlideUp);

            //If we're waiting for connection to a server, draw the shroud.
            if(_connecting)
            {
                ctx.Clear(Color.Black * 0.5F);
                var cFont = _thememgr.Theme.GetFont(TextFontStyle.Header1);
                var cColor = _thememgr.Theme.GetFontColor(TextFontStyle.Header1);

                var cMeasure = cFont.MeasureString(_connectingText);
                ctx.Batch.DrawString(cFont, _connectingText, new Vector2((ctx.Width - cMeasure.X) / 2, (ctx.Height - cMeasure.Y) / 2), cColor);
            }

            ctx.EndDraw();
        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _discord.GameState = "Watercolor Games presents...";
            _discord.GameDetails = "The Peacenet";

            ((PeacenetTheme)_thememgr.Theme).SetAccentColor(_plexgate.GraphicsDevice, content, PeacenetAccentColor.Blueberry);
            _uimanager.InvalidateAll();

            _watercolorSplash = content.Load<SoundEffect>("Audio/MainMenu/WatercolorSplash");
            _wgsInstance = _watercolorSplash.CreateInstance();

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

            _username = new Label();
            _realname = new Label();
            _wgButton = new Button();
            _username.AutoSize = true;
            _username.FontStyle = TextFontStyle.Header3;
            _realname.AutoSize = true;
            _realname.FontStyle = TextFontStyle.System;

            _uimanager.Add(_username);
            _uimanager.Add(_realname);
            _uimanager.Add(_wgButton);

            _settingsApp = new GameSettings(_windowManager);

            _hbSettings.Click += (o, a) =>
            {
                if (_settingsApp.Disposed)
                    _settingsApp = new GameSettings(_windowManager);

                _settingsApp.Show();

            };
            _hbMultiplayer.Click += (o, a) =>
            {
                if (animState < 12)
                {
                    _infobox.PromptText("Connect to server", "Please enter a hostname and port for a server to connect to.", (address) =>
                    {
                        if (address.Split(':').Length == 1)
                            address += ":3251";
                        _server.Connect(address, () =>
                        {
                            _saveManager.SetBackend(new ServerSideSaveBackend());
                            
                            animState = 12;
                        }, (error) =>
                        {
                            _infobox.Show("Connection error", $"Could not connect:{Environment.NewLine}{Environment.NewLine}{error}");
                            _splash.Reset();
                        });
                    });
                }
            };
_hbSingleplayer.Click += (o, a) =>
{
    if (_connecting)
        return;
    if (animState < 12)
    {
        Task.Run(() =>
        {
            _connecting = true;
            _connectingText = "Starting internal server...";
            try
            {
                _os.StartLocalServer();
                _saveManager.SetBackend(new ServerSideSaveBackend());
                animState = 12;
            }
            catch (Exception ex)
            {
                _infobox.Show("Error starting internal server", "An error has occurred while starting the internal Peacenet single-player server.\n\n" + ex.Message);
            }
            _connecting = false;
        });
    }

};

            _lbSingleplayer = new Label();
            _lbMultiplayer = new Label();
            _lbSettings = new Label();

            _uimanager.Add(_lbSingleplayer);
            _uimanager.Add(_lbMultiplayer);
            _uimanager.Add(_lbSettings);


            _credits = new Button();
            _uimanager.Add(_credits);
            _credits.Text = "Credits";
            _credits.ShowImage = true;
            _credits.Image = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/Credits");
            _credits.Click += (o, a) =>
            {
                _cutscene.Play("credits_00");
            };


            _wgButton.Click += (o, a) =>
            {
                if (_api.LoggedIn)
                {
                    _infobox.ShowYesNo("Log out", "Are you sure you want to log out of your Watercolor account?",
                        (answer) =>
                        {
                            if (answer)
                                _api.Logout();
                        });


                }
                else
                {
                    _api.Login("17a4b2de3caf06c14a524936d88402c1", ()=>
                    {
                        _isWaitingForSignin = true;
                    }, ()=>
                    {
                        _isWaitingForSignin = false;
                    });
                }
            };

            _exitButton = new Button();
            _uimanager.Add(_exitButton);
            _exitButton.Click += (o, a) =>
            {
                _infobox.ShowYesNo("Exit The Peacenet", "Are you sure you'd like to quit to your desktop?", (answer) =>
                {
                    if (answer)
                        _plexgate.Exit();
                });
            };

            try
            {
                vidf = File.OpenRead("Content/Cutscenes/PeaceEngine1080p.pnv");
                vid = new PNV(vidf);
                vidp = new VideoPlayer(vid);
                _plexgate.GetLayer(LayerType.Foreground).AddEntity(vidp);
                vidp.Finished += (sender, e) => { _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(vidp); (vid as IDisposable)?.Dispose(); vidf?.Dispose(); animState = 0; };
                Logger.Log("Loaded intro video");
            }
            catch (FileNotFoundException)
            {
                animState = 0;
                Logger.Log("Intro video not found, skipping", LogType.Warning);
            }
        }

        private string _connectingText = "";

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (animState == 2)
                {
                    _hasEnteredMenu = true;
                    animState++;
                }
            }

        }

        private bool _connecting = false;

        [Dependency]
        private DiscordRPCModule _discord = null;

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            switch (animState)
            {
                case -1:
                    _lbSingleplayer.Visible = false;
                    _lbMultiplayer.Visible = false;
                    _lbSettings.Visible = false;
                    _hbSingleplayer.Visible = false;
                    _hbSettings.Visible = false;
                    _hbMultiplayer.Visible = false;
                    _credits.Visible = false;
                    _exitButton.Visible = false;
                    _wgButton.Visible = false;
                    _username.Visible = false;
                    _realname.Visible = false;
                    break;
                case 0: //Start Watercolor splash
                    _peacenetOpacity += (float)(time.ElapsedGameTime.TotalSeconds * 2.5);
                    _peacenetOpacity = MathHelper.Clamp(_peacenetOpacity, 0, 1);
                    _peacenetSlideLeft = _peacenetOpacity;
                    if (_peacenetOpacity >= 1)
                    {
                        animState++;
                    }
                    break;
                case 1: //End Peacenet splash, start Enter wait.
                    _progressBGFade -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    _progressFGPos += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos >= 1)
                    {
                        animState++;
                        _discord.GameState = "In Main Menu";
                        _discord.GameDetails = "Welcome to The Peacenet.";
                    }
                    break;
                case 3: //End Enter wait, start Peacenet splash -> Menu transition.
                    _progressFGPos -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_progressFGPos <= 0)
                        animState++;
                    break;
                case 4:
                    _peacenetOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_peacenetOpacity <= 0)
                        animState++;

                    break;
                case 5: //Start Menu animation.
                    _spSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_spSlideUp >= 1)
                        animState++;
                    break;
                case 6:
                    _mpSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_mpSlideUp >= 1)
                        animState++;
                    break;
                case 7:
                    _seSlideUp += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_seSlideUp >= 1)
                        animState++;
                    break;
                case 8:
                    _hbSettings.Visible = true;
                    _hbSingleplayer.Visible = true;
                    _hbMultiplayer.Visible = true;
                    _lbSingleplayer.Visible = true;
                    _lbMultiplayer.Visible = true;
                    _lbSettings.Visible = true;
                    _realname.Visible = true;
                    _username.Visible = true;
                    _wgButton.Visible = true;
                    _exitButton.Visible = true;
                    _discord.GameDetails = "Selecting game mode";
                    animState++;
                    break;
                case 9: //End Menu animation.
                    _menuLabelOpacity += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_menuLabelOpacity >= 1)
                    {
                        animState++;
                        _credits.Visible = true;
                    }
                    break;
                case 10:
                    _wgUserFadeIn += (float)time.ElapsedGameTime.TotalSeconds * 8;
                    if (_wgUserFadeIn >= 1)
                    {
                        animState++;
                    }
                    break;
                case 12:
                    _wgUserFadeIn -= (float)time.ElapsedGameTime.TotalSeconds * 8;
                    if (_wgUserFadeIn <= 0)
                    {
                        animState++;
                    }
                    break;
                case 13: //Start Menu Unload Animation.
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
                        _username.Visible = false;
                        _realname.Visible = false;
                        _wgButton.Visible = false;
                        _exitButton.Visible = false;
                    }

                    break;
                case 14:
                    _spSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_spSlideUp <= 0)
                        animState++;

                    break;
                case 15:
                    _mpSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_mpSlideUp <= 0)
                        animState++;

                    break;
                case 16:
                    _seSlideUp -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_seSlideUp <= 0)
                        animState++;

                    break;
                case 17:
                    animState++;
                    break;
                case 18:
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
            _lbSingleplayer.FontStyle = TextFontStyle.Header2;
            
            _lbMultiplayer.AutoSize = true;
            _lbMultiplayer.FontStyle = TextFontStyle.Header2;

            _lbSettings.AutoSize = true;
            _lbSettings.FontStyle = TextFontStyle.Header2;

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

            int _userYMax = _lbMultiplayer.Y + _lbMultiplayer.Height + 30;
            int _userYMin = _userYMax + (int)(_uimanager.ScreenHeight * 0.1);
            _username.Opacity = _wgUserFadeIn;
            _username.Y = (int)MathHelper.Lerp(_userYMin, _userYMax, _wgUserFadeIn);
            _realname.Opacity = _wgUserFadeIn;
            _realname.Y = _username.Y + _username.Height+5;
            _wgButton.Opacity = _wgUserFadeIn;
            _wgButton.Y = _realname.Y + _realname.Height+10;
            _username.X = (_uimanager.ScreenWidth - _username.Width) / 2;
            _realname.X = (_uimanager.ScreenWidth - _realname.Width) / 2;
            _wgButton.X = (_uimanager.ScreenWidth - _wgButton.Width) / 2;
            if (_api.LoggedIn)
            {
                _realname.Text = _api.User.username;
                _username.Text = _api.User.display_name;
                _wgButton.Text = "Log out";
            }
            else
            {
                _username.Text = "Not logged in";
                _realname.Text = "Sign into itch.io to play Peacenet in multiplayer and gain other useful perks!";
                _wgButton.Text = "Sign in";
            }

            _exitButton.Text = "Quit to desktop";
            _exitButton.Y = (_uimanager.ScreenHeight - _exitButton.Height) - 15;
            _exitButton.X = (_uimanager.ScreenWidth - _exitButton.Width) - 15;

        }
    }
}
