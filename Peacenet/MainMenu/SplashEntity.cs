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
using System.Threading;
using Peacenet.DesktopUI;

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

        private readonly string[] _greetings = new string[]
        {
            "Hey there, ",
            "Hello, ",
            "Welcome, ",
            "Hey, ",
            "Hi, ",
            "Howdy, "
        };

        private Random _rnd = new Random();

        private string _greeting = "";

        private floaty _welcomePosition = 0f;
        private floaty _welcomeFade = 0f;
        private floaty _peacenetFade = 0f;
        private floaty _peacenetPosition = 0f;
        private floaty _pressEnterFade = 0f;
        private floaty _pressEnterPosition = 0f;
        private floaty _greetFade = 0f;
        private floaty _greetPosition = 0f;
        private floaty _wallFade = 0f;
        private floaty _spFade = 0f;
        private floaty _spPosition = 0f;
        private floaty _mpFade = 0f;
        private floaty _mpPosition = 0f;
        private floaty _seFade = 0f;
        private floaty _sePosition = 0f;
        private floaty _signinFade = 0;
        private floaty _signinPosition = 0;
        private floaty _usernameFade = 0f;
        private floaty _usernamePosition = 0f;

        private bool _isMultiplayerGameSelect = false;

        private int _spUIState = -1;
        private int _mpUIState = -1;

        private floaty _mpUIFade = 0f;
        private floaty _mpUIPosition = 0f;

        private floaty _spUIFade = 0f;
        private floaty _spUIPosition = 0f;


        #endregion

        #region Textures

        private Texture2D _peacenet = null;
        private Texture2D _multiplayer = null;
        private Texture2D _singleplayer = null;
        private Texture2D _settings = null;
        private Texture2D _wall = null;

        #endregion

        #region Sound Effects

        private SoundEffectInstance _welcomeToThePeacenet = null;
        private SoundEffectInstance _pressEnter = null;
        private SoundEffectInstance _itchPrep = null;
        private SoundEffectInstance _menuOpenTransition = null;
        private SoundEffectInstance _itchSignin = null;
        private SoundEffectInstance _mainHubLoop = null;
        private SoundEffectInstance _gameSelect = null;
        private SoundEffectInstance _loadingGame = null;
        private bool _isFadingOut = false;

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

        private PictureBox _hbSingleplayer = null;
        private PictureBox _hbMultiplayer = null;
        private PictureBox _hbSettings = null;

        #endregion

        #region Singleplayer UI

        private Label _singlePlayerHead = new Label();
        private Label _singlePlayerDesc = new Label();
        private Button _newGame = new Button();
        private Button _continue = new Button();

        #endregion

        #region Multiplayer UI elements

        private ScrollView _mpServerScroller = null;
        private ListView _serverList = null;
        private Button _addServer = null;
        private Button _removeServer = null;
        private Button _clearServers = null;
        private Button _joinServer = null;

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
        private Button _backButton = null;

        #endregion

        #region Window entities

        private GameSettings _settingsApp = null;

        #endregion

        private void addServers()
        {
            _serverList.ClearItems();
            foreach(var server in _server.SavedServers)
            {
                var item = new ListViewItem
                {
                    Value = server.Name,
                    Tag = server.Address
                };
                _serverList.AddItem(item);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            (vid as IDisposable)?.Dispose();
            vidf?.Dispose();
            _uimanager.Remove(_hbSingleplayer);
            _uimanager.Remove(_hbMultiplayer);
            _uimanager.Remove(_hbSettings);
            _hbSingleplayer = null;
            _hbMultiplayer = null;
            _hbSettings = null;
        }

        private bool _isWaitingForSignin = false;

        private void startSinglePlayer()
        {
            Task.Run(() =>
            {
                _connecting = true;
                _connectingText = "Starting internal server...";
                try
                {
                    _os.StartLocalServer();
                    _saveManager.SetBackend(new ServerSideSaveBackend());
                    _plexgate.Invoke(() =>
                    {
                        animState = 30;
                        _gameStarting = true;
                    });
                }
                catch (Exception ex)
                {
                    _infobox.Show("Error starting internal server", "An error has occurred while starting the internal Peacenet single-player server.\n\n" + ex.Message);
                    Logger.Log(ex.ToString(), LogType.Error);
                }
                _plexgate.Invoke(() =>
                {
                    _connecting = false;
                });
            });

        }

        private bool _gameStarting = false;

        private void connectToMultiplayerServer(string address)
        {
            Task.Run(() =>
            {
                _connectingText = "Connecting to " + address + "...";
                bool error = false;
                _connecting = true;

                var result = _server.Connect(address);

                result.Wait();

                var cResult = result.Result;

                if (cResult.Result == ConnectionResultType.Success)
                {
                    _saveManager.SetBackend(new ServerSideSaveBackend());
                }
                else
                {
                    switch(cResult.Result)
                    {
                        case ConnectionResultType.AlreadyConnected:
                            _infobox.Show("Already connected.", "You are already connected to a server.");
                            break;
                        case ConnectionResultType.BadItchAuth:
                            _infobox.Show("Not signed in", "You must be signed in to an itch.io account to play Multiplayer.");
                            break;
                        case ConnectionResultType.ConnectionTimeout:
                            _infobox.Show("Connection timeout", "Connection to the server failed because the connection has timed out.");
                            break;
                        case ConnectionResultType.Other:
                            _infobox.Show(cResult.Exception.GetType().FullName, cResult.Exception.ToString());
                            break;
                    }
                    error = true;
                }
                _connecting = false;
                if (error == true)
                    return;
                _connecting = true;
                _connectingText = "Setting up environment...";
                _os.EnsureProperEnvironment();
                _connecting = false;
                animState = 30;
                _gameStarting = true;
            });

        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _newGame.Text = "New game";
            _continue.Text = "Continue";
            _uimanager.Add(_newGame);
            _uimanager.Add(_continue);
            _uimanager.Add(_singlePlayerHead);
            _uimanager.Add(_singlePlayerDesc);
            _singlePlayerHead.Text = "Single player";
            _singlePlayerHead.AutoSize = true;
            _singlePlayerHead.FontStyle = TextFontStyle.Header1;
            _singlePlayerDesc.AutoSize = true;

            _backButton = new Button();
            _backButton.Text = "Back";
            _uimanager.Add(_backButton);

            _mpServerScroller = new ScrollView();
            _serverList = new ListView();
            _addServer = new Button();
            _removeServer = new Button();
            _joinServer = new Button();
            _clearServers = new Button();
            
            _serverList.Layout = ListViewLayout.List;
            _uimanager.Add(_mpServerScroller);
            _mpServerScroller.AddChild(_serverList);
            _uimanager.Add(_addServer);
            _uimanager.Add(_removeServer);
            _uimanager.Add(_clearServers);
            _uimanager.Add(_joinServer);

            addServers();

            _joinServer.Click += (o, a) =>
            {
                connectToMultiplayerServer(_serverList.SelectedItem.Tag.ToString());
            };

            _removeServer.Click += (o, a) =>
            {
                if(_serverList.SelectedItem!=null)
                {
                    var server = _server.SavedServers.FirstOrDefault(x => x.Name == _serverList.SelectedItem.Value);
                    if(server != null)
                    {
                        _infobox.ShowYesNo("Remove server", "Are you sure you want to remove the server \"" + server.Name + "\"?", (answer) =>
                        {
                            if(answer)
                            {
                                _server.RemoveServer(server);
                                addServers();
                            }
                        });
                    }
                }
            };

            _clearServers.Click += (o, a) =>
            {
                _server.ClearServers();
                addServers();
            };

            _addServer.Click += (o, a) =>
            {
                _infobox.PromptText("Add server", "Please enter a name for the new server.", (name) =>
                {
                    _infobox.PromptText($"{name} - Add server", "Please enter the IP address or hostname of the server. Note that the default port for Peacenet servers is 3251.", (address) =>
                    {
                        _server.AddServer(new SavedServer
                        {
                            Address = address,
                            Name = name
                        });
                        addServers();
                    }, (address) =>
                    {
                        if(string.IsNullOrWhiteSpace(address))
                        {
                            _infobox.Show("Add server", "You can't enter an empty address!");
                            return false;
                        }
                        return true;
                    });
                }, (name) =>
                {
                    if(string.IsNullOrWhiteSpace(name))
                    {
                        _infobox.Show("Add server", "Your server's name must not be blank!");
                        return false;
                    }
                    if(_server.SavedServers.FirstOrDefault(x=>x.Name == name) != null)
                    {
                        _infobox.Show("Add server", "A server with that name already exists.");
                        return false;
                    }
                    return true;
                });
            };


            _wall = content.Load<Texture2D>("Desktop/DesktopBackgroundImage2");

            _greeting = _greetings[_rnd.Next(_greetings.Length)];

            _peacenet = content.Load<Texture2D>("Splash/Peacenet");

            _welcomeToThePeacenet = content.Load<SoundEffect>("Audio/MainMenu/WelcomeToThePeacenet").CreateInstance();
            _pressEnter = content.Load<SoundEffect>("Audio/MainMenu/PressEnter").CreateInstance();
            _menuOpenTransition = content.Load<SoundEffect>("Audio/MainMenu/MenuOpenTransition").CreateInstance();
            _itchPrep = content.Load<SoundEffect>("Audio/MainMenu/ItchPrep").CreateInstance();
            _itchSignin = content.Load<SoundEffect>("Audio/MainMenu/ItchSignin").CreateInstance();
            _mainHubLoop = content.Load<SoundEffect>("Audio/MainMenu/MainHubLoop").CreateInstance();
            _gameSelect = content.Load<SoundEffect>("Audio/MainMenu/GameSelect").CreateInstance();
            _loadingGame = content.Load<SoundEffect>("Audio/MainMenu/LoadingGame").CreateInstance();
            _pressEnter.IsLooped = true;
            _itchPrep.IsLooped = true;

            _discord.GameState = "Watercolor Games presents...";
            _discord.GameDetails = "The Peacenet";

            ((PeacenetTheme)_thememgr.Theme).SetAccentColor(_plexgate.GraphicsDevice, content, PeacenetAccentColor.Blueberry);
            _uimanager.InvalidateAll();

            _singleplayer = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/SinglePlayer");
            _multiplayer = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/MultiPlayer");
            _settings = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/Settings");

            _hbSingleplayer = new PictureBox();
            _hbMultiplayer = new PictureBox();
            _hbSettings = new PictureBox();

            _uimanager.Add(_hbSettings);
            _uimanager.Add(_hbSingleplayer);
            _uimanager.Add(_hbMultiplayer);

            _hbSettings.Texture = _settings;
            _hbSingleplayer.Texture = _singleplayer;
            _hbMultiplayer.Texture = _multiplayer;


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
                if (_connecting)
                    return;
                if (animState < 22)
                {
                    _isMultiplayerGameSelect = true;
                    animState = 22;
                }
            };
            EventHandler spstart;
            _hbSingleplayer.Click += spstart = (o, a) =>
            {
                if (_connecting)
                    return;
                if (animState < 22)
                {
                    animState = 22;
                    _isMultiplayerGameSelect = false;
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
                _isFadingOut = true;
                _cutscene.Play("credits_00", ()=>
                {
                    _isFadingOut = false;
                });
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
                    login();
                }
            };

            _continue.Click += (o, a) =>
            {
                startSinglePlayer();
            };
            _newGame.Click += (o, a) =>
            {
                if(_continue.Enabled)
                {
                    _infobox.ShowYesNo("Start new game", "Are you sure you want to start a new game? Doing this will wipe all data stored on the current Peacegate instance!", (answer) =>
                    {
                        if(answer)
                        {
                            if(Directory.Exists(_os.SinglePlayerSaveDirectory))
                                Directory.Delete(_os.SinglePlayerSaveDirectory, true);
                            startSinglePlayer();
                        }
                    });
                }
                else
                {
                    startSinglePlayer();
                }
            };

            _backButton.Click += (o, a) =>
            {
                animState = 30;
                _gameStarting = false;
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

            if (_plexgate.GameStarted)
                animState = 3;
            else if (_plexgate.QuietMode)
                spstart.Invoke(this, EventArgs.Empty);
            else
            {
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
        }

        private void login()
        {
            _api.Login("17a4b2de3caf06c14a524936d88402c1", () =>
            {
                _isWaitingForSignin = true;
            }, () =>
            {
                if (animState == 7)
                    animState = 8;
                _isWaitingForSignin = false;
            });
        }

        private string _connectingText = "";

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
            switch(animState)
            {
                case 4:
                    if (e.Key == Keys.Enter)
                        animState++;
                    break;
            }
        }

        private bool _connecting = false;

        [Dependency]
        private DiscordRPCModule _discord = null;

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        private double _greetTime = 0;

        private int soundState = 0;

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            if(_isFadingOut)
            {
                _welcomeToThePeacenet.Volume = MathHelper.Clamp(_welcomeToThePeacenet.Volume - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
            }
            else
            {
                _welcomeToThePeacenet.Volume = MathHelper.Clamp(_welcomeToThePeacenet.Volume + (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
            }
            _pressEnter.Volume = _welcomeToThePeacenet.Volume;
            _menuOpenTransition.Volume = _welcomeToThePeacenet.Volume;
            _itchPrep.Volume = _welcomeToThePeacenet.Volume;
            _itchSignin.Volume = _welcomeToThePeacenet.Volume;
            _mainHubLoop.Volume = _welcomeToThePeacenet.Volume;
            _gameSelect.Volume = _welcomeToThePeacenet.Volume;
            _loadingGame.Volume = _welcomeToThePeacenet.Volume;

            switch (animState)
            {
                case -1:
                    _hbSingleplayer.Visible = false;
                    _hbMultiplayer.Visible = false;
                    _hbSettings.Visible = false;
                    _wgButton.Visible = false;
                    _exitButton.Visible = false;
                    _credits.Visible = false;
                    _backButton.Visible = false;
                    _backButton.Opacity = 0;
                    _realname.Opacity = 0;
                    _username.Opacity = 0;
                    _wgButton.Opacity = 0;
                    _exitButton.Opacity = 0;
                    _credits.Opacity = 0;
                    break;
                case 0:
                    if(_welcomeToThePeacenet.State != SoundState.Playing)
                    {
                        _welcomeToThePeacenet.Play();
                        animState++;
                    }
                    break;
                case 1:
                    _welcomeFade = MathHelper.Clamp(_welcomeFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _welcomePosition = _welcomeFade / 2;
                    if(_welcomeFade>=1)
                    {
                        animState++;
                    }
                    break;
                case 2:
                    _peacenetFade = MathHelper.Clamp(_peacenetFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _peacenetPosition = _peacenetFade / 2;
                    _welcomePosition = 0.5F + _peacenetPosition/2;
                    if(_peacenetFade>=1)
                    {
                        animState++;
                    }
                    break;
                case 3:
                    _pressEnterFade = MathHelper.Clamp(_pressEnterFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _pressEnterPosition = _pressEnterFade / 2;
                    if(_pressEnterFade>=1)
                    {
                        animState++;
                    }
                    break;
                case 5:
                    _pressEnter.IsLooped = false;
                    _pressEnterFade = MathHelper.Clamp(_pressEnterFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    float pressEnterOpposite = 1 - _pressEnterFade;
                    _pressEnterPosition = 0.5f + (pressEnterOpposite / 2);
                    if(_pressEnterFade<=0)
                    {
                        animState++;
                    }
                    break;
                case 6:
                    if (!_api.LoggedIn)
                    {
                        _infobox.ShowYesNo("Sign in to itch.io", "Would you like to sign in to your itch.io account?\n\nSigning in allows your username to be displayed in various areas of Peacegate OS in place of \"player\", and allows you to access multiplayer features of the game.\n\nYou do not need to sign in to play Singleplayer.", (answer) =>
                        {
                            if(!answer)
                            {
                                animState = 8;
                            }
                            else
                            {
                                login();
                            }
                        });
                        animState++;
                    }
                    else
                    {
                        animState = 8;
                    }
                    break;
                case 8:
                    _itchPrep.IsLooped = false;
                    _greetFade = MathHelper.Clamp(_greetFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _greetPosition = _greetFade / 2;
                    if(_greetFade>=1F)
                    {
                        animState++;
                    }
                    break;
                case 9:
                    if(_greetTime<2.5)
                    {
                        _greetTime += time.ElapsedGameTime.TotalSeconds;
                        break;
                    }
                    _greetFade = MathHelper.Clamp(_greetFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float greetOpposite = 1 - _greetFade;
                    _greetPosition = 0.5F + (greetOpposite / 2);
                    if (_greetFade <= 0F)
                    {
                        animState++;
                        _greetTime = 0;
                    }

                    break;
                case 10:
                    _peacenetFade = MathHelper.Clamp(_peacenetFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float peacenetOpposite = 1 - _peacenetFade;
                    _peacenetPosition = 0.5F + (peacenetOpposite / 2);
                    if(_peacenetFade<=0)
                    {
                        animState++;
                    }
                    break;
                case 11:
                    _welcomeFade = MathHelper.Clamp(_welcomeFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float welcomeOpposite = 1 - _welcomeFade;
                    _welcomePosition = 0.775F + (welcomeOpposite / 2);
                    if (_welcomeFade <= 0)
                    {
                        animState++;
                    }
                    break;
                case 12:
                    _wallFade = MathHelper.Clamp(_wallFade + (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                    if(_wallFade>=1F)
                    {
                        animState++;
                    }
                    break;
                case 13:
                    _hbSingleplayer.Visible = true;
                    _spFade = MathHelper.Clamp(_spFade + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    _spPosition = _spFade / 2;
                    if (_spFade>=1F)
                    {
                        animState++;
                    }
                    break;
                case 14:
                    _hbMultiplayer.Visible = true;
                    _mpFade = MathHelper.Clamp(_mpFade + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    _mpPosition = _mpFade / 2;
                    if (_mpFade >= 1F)
                    {
                        animState++;
                    }
                    break;
                case 15:
                    _hbSettings.Visible = true;
                    _seFade = MathHelper.Clamp(_seFade + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    _sePosition = _seFade / 2;
                    if (_seFade >= 1F)
                    {
                        animState++;
                    }
                    break;
                case 16:
                    _hbMultiplayer.Visible = true;
                    _hbSingleplayer.Visible = true;
                    _hbSettings.Visible = true;
                    animState++;
                    break;
                case 17:
                    _signinFade = MathHelper.Clamp(_signinFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _signinPosition = _signinFade / 2;
                    if(_signinFade>=1F)
                    {
                        animState++;
                    }
                    break;
                case 18:
                    _usernameFade = MathHelper.Clamp(_usernameFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _usernamePosition = _usernameFade / 2;
                    if (_usernameFade >= 1F)
                    {
                        animState++;
                        _wgButton.Visible = true;
                    }
                    break;
                case 19:
                    _wgButton.Opacity = MathHelper.Clamp(_wgButton.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    if(_wgButton.Opacity>=1)
                    {
                        animState++;
                        _credits.Visible = true;
                        _exitButton.Visible = true;
                    }
                    break;
                case 20:
                    _credits.Opacity = MathHelper.Clamp(_credits.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    if (_credits.Opacity >= 1)
                    {
                        animState++;
                    }
                    break;

                case 22:
                    _mainHubLoop.IsLooped = false;
                    _wgButton.Opacity = MathHelper.Clamp(_wgButton.Opacity - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _credits.Opacity = _wgButton.Opacity;
                    _exitButton.Opacity = _wgButton.Opacity;
                    if (_wgButton.Opacity <= 0)
                    {
                        animState++;
                        _credits.Visible = false;
                        _exitButton.Visible = false;
                        _wgButton.Visible = false;
                    }
                    break;
                case 23:
                    if(_isMultiplayerGameSelect)
                    {
                        _mpFade = MathHelper.Clamp(_mpFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        float mpOpposite = 1 - _mpFade;
                        _mpPosition = 0.5F + (mpOpposite/2);
                        if (_mpFade <= 0F)
                        {
                            animState++;
                        }
                    }
                    else
                    {
                        _spFade = MathHelper.Clamp(_spFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        float spOpposite = 1 - _spFade;
                        _spPosition = 0.5F + (spOpposite/2);
                        if (_spFade <= 0F)
                        {
                            animState++;
                        }

                    }
                    break;
                case 24:
                    if (!_isMultiplayerGameSelect)
                    {
                        _mpFade = MathHelper.Clamp(_mpFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        float mpOpposite = 1 - _mpFade;
                        _mpPosition = 0.5F + (mpOpposite/2);
                        if (_mpFade <= 0F)
                        {
                            animState++;
                        }
                    }
                    else
                    {
                        _spFade = MathHelper.Clamp(_spFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        float spOpposite = 1 - _spFade;
                        _spPosition = 0.5F + (spOpposite/2);
                        if (_spFade <= 0F)
                        {
                            animState++;
                        }
                    }
                    break;
                case 25:
                    _seFade = MathHelper.Clamp(_seFade - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    float seOpposite = 1 - _seFade;
                    _sePosition = 0.5F + (seOpposite/2);
                    if (_seFade <= 0F)
                    {
                        animState++;
                    }
                    break;
                case 26:
                    _usernameFade = MathHelper.Clamp(_usernameFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float unameOpposite = 1 - _usernameFade;
                    _usernamePosition = 0.5f+(unameOpposite / 2);
                    if (_usernameFade <= 0F)
                    {
                        animState++;
                    }
                    break;
                case 27:
                    _signinFade = MathHelper.Clamp(_signinFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float snOpposite = 1 - _signinFade;
                    _signinPosition = 0.5f + (snOpposite / 2);
                    if (_signinFade <= 0F)
                    {
                        _backButton.Visible = true;
                        animState++;
                    }
                    break;
                case 28:
                    _backButton.Opacity = MathHelper.Clamp(_backButton.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    if(_backButton.Opacity>=1F)
                    {
                        animState++;
                        _hbSingleplayer.Visible = false;
                        _hbMultiplayer.Visible = false;
                        _hbSettings.Visible = false;
                        if(_isMultiplayerGameSelect)
                        {
                            _mpUIState = 0;
                        }
                        else
                        {
                            _spUIState = 0;
                        }
                    }
                    break;
                case 30:
                    _backButton.Opacity = MathHelper.Clamp(_backButton.Opacity - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    if (_backButton.Opacity <= 0F)
                    {
                        _backButton.Visible = false;
                        if(_isMultiplayerGameSelect)
                        {
                            _mpUIState = 2;
                        }
                        else
                        {
                            _spUIState = 2;
                        }
                    }
                    break;
                case 31:
                    _isFadingOut = true;
                    _wallFade = MathHelper.Clamp(_wallFade - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                    if(_wallFade<=0)
                    {
                        _os.OnReady();
                        animState++;
                        
                    }
                    break;
            }

            _hbSettings.Tint = (_hbSettings.ContainsMouse) ? _thememgr.Theme.GetAccentColor() : Color.White;
            _hbSingleplayer.Tint = (_hbSingleplayer.ContainsMouse) ? _thememgr.Theme.GetAccentColor() : Color.White;
            _hbMultiplayer.Tint = (_hbMultiplayer.ContainsMouse) ? _thememgr.Theme.GetAccentColor() : Color.White;


            switch (_spUIState)
            {
                case -1:
                    _continue.Visible = false;
                    _newGame.Visible = false;
                    _singlePlayerDesc.Visible = false;
                    _singlePlayerHead.Visible = false;
                    break;
                case 0:
                    _continue.Visible = true;
                    _newGame.Visible = true;
                    _singlePlayerDesc.Visible = true;
                    _singlePlayerHead.Visible = true;
                    _spUIFade = MathHelper.Clamp(_spUIFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _spUIPosition = _spUIFade / 2;
                    if(_spUIFade>=1f)
                    {
                        _spUIState++;
                        _continue.Enabled = (Directory.Exists(_os.SinglePlayerSaveDirectory));
                        if(_continue.Enabled)
                        {
                            _singlePlayerDesc.Text = "Would you like to start a new game or continue where you left off?";
                        }
                        else
                        {
                            _singlePlayerDesc.Text = "Welcome to The Peacenet. Click \"New Game\" to create a new Peacegate OS instance and begin your adventure.";
                        }
                    }
                    break;
                case 2:
                    _spUIFade = MathHelper.Clamp(_spUIFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float spUIOpposite = 1 - _spUIFade;
                    _spUIPosition = 0.5F + (spUIOpposite / 2);
                    if (_spUIFade <= 0f)
                    {
                        _spUIState = -1;
                        if (_gameStarting)
                        {
                            animState = 31;
                        }
                        else
                        {
                            animState = 13;
                        }
                    }

                    break;
            }

            switch (_mpUIState)
            {
                case -1:
                    _mpServerScroller.Visible = false;
                    _addServer.Visible = false;
                    _joinServer.Visible = false;
                    _clearServers.Visible = false;
                    _removeServer.Visible = false;
                    _serverList.Visible = false;
                    break;
                case 0:
                    _serverList.Visible = true;
                    _mpServerScroller.Visible = true;
                    _addServer.Visible = true;
                    _joinServer.Visible = true;
                    _clearServers.Visible = true;
                    _removeServer.Visible = true;
                    _mpUIFade = MathHelper.Clamp(_mpUIFade + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    _mpUIPosition = (_mpUIFade / 2);
                    if (_mpUIFade >= 1F)
                    {
                        _mpUIState++;
                    }
                    break;
                case 2:
                    _mpUIFade = MathHelper.Clamp(_mpUIFade - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    float mpUIOpposite = 1 - _mpUIFade;
                    _mpUIPosition = 0.5F + (mpUIOpposite / 2);
                    if (_mpUIFade <= 0F)
                    {
                        if (_gameStarting)
                        {
                            animState = 31;
                        }
                        else
                        {
                            animState = 13;
                        }
                        _mpUIState = -1;
                    }
                    break;
            }

            _singlePlayerDesc.MaxWidth = _uimanager.ScreenWidth / 2;
            _singlePlayerDesc.X = (_uimanager.ScreenWidth - _singlePlayerDesc.Width) / 2;
            _singlePlayerDesc.Alignment = TextAlignment.Center;
            int spUICenter = (_uimanager.ScreenHeight - _singlePlayerDesc.Height) / 2;
            _singlePlayerDesc.Y = (int)MathHelper.Lerp(spUICenter - 25, spUICenter + 25, _spUIPosition);
            _singlePlayerDesc.Opacity = _spUIFade;

            _singlePlayerHead.X = (_uimanager.ScreenWidth - _singlePlayerHead.Width) / 2;
            _singlePlayerHead.Y = (_singlePlayerDesc.Y - _singlePlayerHead.Height) - 5;
            _singlePlayerHead.Opacity = _spUIFade;

            int spButtonWidth = _newGame.Width + _continue.Width + 3;
            _newGame.X = (_uimanager.ScreenWidth - spButtonWidth) / 2;
            _continue.X = _newGame.X + _newGame.Width + 3;
            _newGame.Y = _singlePlayerDesc.Y + _singlePlayerDesc.Height + 10;
            _continue.Y = _newGame.Y;
            _newGame.Opacity = _spUIFade;
            _continue.Opacity = _spUIFade;

            _addServer.Text = "Add server";
            _removeServer.Text = "Remove server";
            _clearServers.Text = "Clear servers";
            _joinServer.Text = "Join server";

            _mpServerScroller.Width = 600;
            _mpServerScroller.Height = 400;
            _serverList.Width = 600;
            _serverList.X = 0;
            _serverList.Y = 0;
            _mpServerScroller.X = (_uimanager.ScreenWidth - _mpServerScroller.Width) / 2;
            int mpCenter = (_uimanager.ScreenHeight - _mpServerScroller.Height) / 2;
            _mpServerScroller.Y = (int)MathHelper.Lerp(mpCenter + 200, mpCenter - 200, _mpUIPosition);
            _mpServerScroller.Opacity = _mpUIFade;
            _addServer.X = _mpServerScroller.X;
            _addServer.Y = _mpServerScroller.Y + _mpServerScroller.Height + 15;
            _addServer.Opacity = _mpServerScroller.Opacity;
            _removeServer.Y = _addServer.Y;
            _removeServer.X = _addServer.X + _addServer.Width + 5;
            _removeServer.Opacity = _addServer.Opacity;
            _clearServers.X = _removeServer.X + _removeServer.Width + 5;
            _clearServers.Y = _removeServer.Y;
            _clearServers.Opacity = _removeServer.Opacity;
            _joinServer.X = (_mpServerScroller.X + _mpServerScroller.Width) - _joinServer.Width;
            _joinServer.Y = _clearServers.Y;
            _joinServer.Opacity = _clearServers.Opacity;
            _joinServer.Enabled = (_serverList.SelectedItem != null);
            _removeServer.Enabled = _joinServer.Enabled;

            _hbMultiplayer.Width = 256;
            _hbSingleplayer.Width = 256;
            _hbSettings.Width = 256;
            _hbMultiplayer.Height = 256;
            _hbSingleplayer.Height = 256;
            _hbSettings.Height = 256;


            _hbMultiplayer.X = (_uimanager.ScreenWidth - _hbMultiplayer.Width) / 2;
            _hbSingleplayer.X = (_hbMultiplayer.X - 25) - _hbSingleplayer.Width;
            _hbSettings.X = (_hbMultiplayer.X + _hbMultiplayer.Width) + 25;

            float hbCenterY = (_uimanager.ScreenHeight - _hbMultiplayer.Height) / 2;
            float hbSpY = MathHelper.Lerp(hbCenterY + (_hbSingleplayer.Height / 2), hbCenterY - (_hbSingleplayer.Height / 2), _spPosition);
            float hbMpY = MathHelper.Lerp(hbCenterY + (_hbMultiplayer.Height / 2), hbCenterY - (_hbMultiplayer.Height / 2), _mpPosition);
            float hbSeY = MathHelper.Lerp(hbCenterY + (_hbSettings.Height / 2), hbCenterY - (_hbSettings.Height / 2), _sePosition);

            _hbSingleplayer.Y = (int)hbSpY;
            _hbMultiplayer.Y = (int)hbMpY;
            _hbSettings.Y = (int)hbSeY;

            _hbSettings.Opacity = _seFade;
            _hbMultiplayer.Opacity = _mpFade;
            _hbSingleplayer.Opacity = _spFade;

            _realname.MaxWidth = 550;

            if (_hbSingleplayer.ContainsMouse)
            {
                _username.Text = "Single Player";
                _realname.Text = "Enter the war-torn digital world of The Peacenet and meet many unique personalities and gain valuable resources as you discover the story behind this world and restore its peace.";
                _wgButton.Opacity = 0;
            }
            else if (_hbMultiplayer.ContainsMouse)
            {
                _username.Text = "Multiplayer";
                _realname.Text = "Hack and explore your way as a sentience within The Peacenet, gaining all the resources you can, defending against other sentiences, and emerging victorious as the king of the war.";
                _wgButton.Opacity = 0;
            }
            else if (_hbSettings.ContainsMouse)
            {
                _username.Text = "Settings";
                _realname.Text = "Modify various settings for the game to configure yuor experience.";
                _wgButton.Opacity = 0;
            }
            else
            {
                _username.Text = (_api.LoggedIn) ? "Signed in" : "Not signed in";
                _realname.Text = (_api.LoggedIn) ? ((string.IsNullOrWhiteSpace(_api.User.display_name)) ? _api.User.username : _api.User.display_name) : "Sign in to play Multiplayer and gain other perks!";
                if(animState>19)
                {
                    _wgButton.Opacity = 1;
                }
            }

            _username.AutoSize = true;
            _username.FontStyle = TextFontStyle.Header3;
            _realname.AutoSize = true;
            _realname.FontStyle = TextFontStyle.System;
            _realname.Alignment = TextAlignment.Center;

            _username.X = (_uimanager.ScreenWidth - _username.Width) / 2;
            _realname.X = (_uimanager.ScreenWidth - _realname.Width) / 2;

            //misnomer
            int threeQuartersOfHeight = (int)hbCenterY + _hbMultiplayer.Height + 30;
            int usernameYCenter = threeQuartersOfHeight + _username.Height + 5;

            _username.Y = (int)MathHelper.Lerp(threeQuartersOfHeight + (_username.Height / 2), threeQuartersOfHeight - (_username.Height / 2), _signinPosition);
            _realname.Y = (int)MathHelper.Lerp(usernameYCenter + (_realname.Height / 2), usernameYCenter - (_realname.Height / 2), _usernamePosition);

            _username.Opacity = _signinFade;
            _realname.Opacity = _usernameFade;

            _wgButton.Y = _realname.Y + _realname.Height + 5;
            _wgButton.X = (_uimanager.ScreenWidth - _wgButton.Width) / 2;

            _wgButton.Text = _api.LoggedIn ? "Sign out" : "Sign in";

            _credits.X = 15;
            _credits.Y = (_uimanager.ScreenHeight - _credits.Height) - 15;

            _exitButton.X = (_uimanager.ScreenWidth - _exitButton.Width) - 15;
            _exitButton.Y = _credits.Y;
            _exitButton.Opacity = _credits.Opacity;
            _exitButton.Text = "Exit to Desktop";

            _backButton.X = 15;
            _backButton.Y = (_uimanager.ScreenHeight - _backButton.Height) - 15;
            
            if (animState < 0)
                return;
            switch(soundState)
            {
                case 0:
                    if(_welcomeToThePeacenet.State != SoundState.Playing)
                    {
                        _pressEnter.Play();
                        soundState++;
                    }
                    break;
                case 1:
                    if(_pressEnter.State != SoundState.Playing)
                    {
                        _menuOpenTransition.Play();
                        soundState++;
                    }
                    break;
                case 2:
                    if (_menuOpenTransition.State != SoundState.Playing)
                    {
                        _itchPrep.Play();
                        soundState++;
                    }
                    break;
                case 3:
                    if(_itchPrep.State != SoundState.Playing)
                    {
                        _itchSignin.Play();
                        soundState++;
                    }
                    break;
                case 4:
                    if(_itchSignin.State != SoundState.Playing)
                    {
                        _mainHubLoop.Play();
                        if(animState<22)
                            _mainHubLoop.IsLooped = true;
                        soundState++;
                    }
                    break;
                case 5:
                    if(_mainHubLoop.State != SoundState.Playing)
                    {
                        _gameSelect.Play();
                        _gameSelect.IsLooped = true;
                        soundState++;
                    }
                    break;
                case 6:
                    if (_gameSelect.State != SoundState.Playing)
                    {
                        _loadingGame.Play();
                        _loadingGame.IsLooped = true;
                        soundState = 7;
                    }
                    break;
            }
            
        }

        private bool IsSettingsOpen
        {
            get
            {
                if (_settingsApp == null)
                    return false;
                if (_settingsApp.Disposed)
                    return false;
                return _windowManager.WindowList.Length > 0;
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext gfx)
        {
            var headerFont = _thememgr.Theme.GetFont(TextFontStyle.Header1);
            var highlightFont = _thememgr.Theme.GetFont(TextFontStyle.Highlight);

            string serverSelect = "Select a server";
            string welcomeText = "Welcome to";
            string enterText = "Press ENTER to continue.";
            string greetText = $"{_greeting} " + ((_api.LoggedIn) ? ((string.IsNullOrWhiteSpace(_api.User.display_name)) ? _api.User.username : _api.User.display_name) : "Player") + ".";

            var serverSelectMeasure = headerFont.MeasureString(serverSelect);
            var welcomeMeasure = headerFont.MeasureString(welcomeText);
            var enterMeasure = highlightFont.MeasureString(enterText);
            var greetMeasure = highlightFont.MeasureString(greetText);

            float serverSelectX = (gfx.Width - serverSelectMeasure.X) / 2;
            float serverSelectY = (_mpServerScroller.Y - serverSelectMeasure.Y) - 5;

            float welcomeX = (gfx.Width - welcomeMeasure.X) / 2;
            float welcomeY = MathHelper.Lerp((gfx.Height / 2) + (_peacenet.Height / 2) + 25, (((gfx.Height / 2) - (_peacenet.Height / 2)) - 25), _welcomePosition);

            float peacenetX = (gfx.Width - _peacenet.Width) / 2;
            float peacenetY = MathHelper.Lerp((gfx.Height / 2) + (_peacenet.Height / 2), (gfx.Height / 2) - (_peacenet.Height / 2), _peacenetPosition);

            float enterX = (gfx.Width - enterMeasure.X) / 2;
            float enterYCenter = ((gfx.Height / 2) + (_peacenet.Height / 2) + 40) + (enterMeasure.Y / 2);
            float enterY = MathHelper.Lerp(enterYCenter + (enterMeasure.Y / 2), enterYCenter - (enterMeasure.Y / 2), _pressEnterPosition);

            float greetX = (gfx.Width - greetMeasure.X) / 2;
            float greetYCenter = ((gfx.Height / 2) + (_peacenet.Height / 2) + 40) + (greetMeasure.Y / 2);
            float greetY = MathHelper.Lerp(greetYCenter + (greetMeasure.Y / 2), greetYCenter - (greetMeasure.Y / 2), _greetPosition);

            float wallWidth = MathHelper.Lerp(gfx.Width / 2, gfx.Width, _wallFade);
            float wallHeight = MathHelper.Lerp(gfx.Height / 2, gfx.Height, _wallFade);


            gfx.BeginDraw();

            //gfx.Batch.Draw(_wall, new Rectangle((gfx.Width - (int)wallWidth) / 2, (gfx.Height - (int)wallHeight) / 2, (int)wallWidth, (int)wallHeight), Color.White * _wallFade);
            if (!IsSettingsOpen)
            {
                gfx.Batch.Draw(_peacenet, new Vector2(peacenetX, peacenetY), Color.White * _peacenetFade);
                gfx.Batch.DrawString(headerFont, welcomeText, new Vector2(welcomeX, welcomeY), _thememgr.Theme.GetFontColor(TextFontStyle.Highlight) * _welcomeFade);
            }
            gfx.Batch.DrawString(highlightFont, enterText, new Vector2(enterX, enterY), _thememgr.Theme.GetFontColor(TextFontStyle.Highlight) * _pressEnterFade);
            gfx.Batch.DrawString(highlightFont, greetText, new Vector2(greetX, greetY), _thememgr.Theme.GetFontColor(TextFontStyle.Highlight) * _greetFade);
            gfx.Batch.DrawString(headerFont, serverSelect, new Vector2(serverSelectX, serverSelectY), _thememgr.Theme.GetFontColor(TextFontStyle.Header1) * _mpUIFade);

            if(_connecting)
            {
                gfx.DrawRectangle(0, 0, gfx.Width, gfx.Height, Color.Black * 0.75F);

                var connectingMeasure = headerFont.MeasureString(_connectingText);
                gfx.Batch.DrawString(headerFont, _connectingText, new Vector2((gfx.Width - connectingMeasure.X) / 2, (gfx.Height - connectingMeasure.Y) / 2), _thememgr.Theme.GetFontColor(TextFontStyle.Header1));
            }

            gfx.EndDraw();

        }

    }
}
