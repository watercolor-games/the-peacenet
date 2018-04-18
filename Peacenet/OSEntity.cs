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
using Peacenet.Applications;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using floaty = System.Single;
using Plex.Engine.GUI;
using doublefloaty = System.Double;
using Microsoft.Xna.Framework.Content;
using Peacenet.RichPresence;
using Peacenet.Server;
using Plex.Objects;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Themes;
using Plex.Engine.Saves;
using Peacenet.Missions.Prologue;

namespace Peacenet
{
    /// <summary>
    /// A <see cref="IEntity"/> which acts as the Peacegate OS bootscreen. 
    /// </summary>
    public class OSEntity : IEntity, ILoadable, IDisposable
    {
        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private SoundEffect _hackedBgm = null;
        private SoundEffectInstance _hackedBgmInstance = null;

        private HackedAnimationEntity _anim = null;
        
        public bool IsReceivingConnection
        {
            get
            {
                if (_anim == null)
                    return false;
                return _plexgate.GetLayer(LayerType.Foreground).HasEntity(_anim);
            }
        }

        /// <inheritdoc/>
        public void OnGameExit()
        {
            _os.Shutdown();
        }

        #region Boot animation

        private int _osIntroState = 0;
        private SpriteFont _bootFont = null;
        private floaty _peacegateIconOpacity = 0;
        private doublefloaty _peacegateRide = 0;
        private bool _wgDeskOpen = false;
        private DesktopWindow _desktop = null;

        #endregion

        #region Window entities

        private Terminal _init = null;

        #endregion

        #region Engine components

        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private DiscordRPCModule _discord = null;

        [Dependency]
        private WindowSystem _winmgr = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private TerminalManager _term = null;

        [Dependency]
        private ThemeManager _theme = null;

        #endregion

        #region Textures

        private Texture2D _peacegate = null;

        #endregion



        /// <inheritdoc/>
        public void Dispose()
        {
            if(_desktop != null)
            {
                if(_desktop.Visible)
                {
                    _desktop.Close();
                }
                _desktop.Dispose();
                _desktop = null;
            }
            _server.BroadcastReceived -= _server_BroadcastReceived;
        }

        private int _connectionCount = 0;

        /// <summary>
        /// Retrieves this OS entity's desktop.
        /// </summary>
        public DesktopWindow Desktop
        {
            get
            {
                return _desktop;
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext ctx)
        {
            int peacegateX = (_ui.ScreenWidth - _peacegate.Width) / 2;
            int peacegateYMax = (_ui.ScreenHeight - _peacegate.Height) / 2;
            int peacegateYMin = peacegateYMax + (int)(_ui.ScreenHeight * 0.15);
            int peacegateY = (int)MathHelper.Lerp(peacegateYMin, peacegateYMax, _peacegateIconOpacity);
            ctx.BeginDraw();
            ctx.DrawRectangle(peacegateX, peacegateY, _peacegate.Width, _peacegate.Height, _peacegate, Color.White * _peacegateIconOpacity);

            int _textY = peacegateY + _peacegate.Height + 25;
            string text = "Welcome to Peacegate.";
            var measure = TextRenderer.MeasureText(text, _bootFont, int.MaxValue, Plex.Engine.TextRenderers.WrapMode.None);
            int _textX = ((_ui.ScreenWidth - (int)measure.X) / 2);
            ctx.DrawString(text, _textX, _textY, Color.White * _peacegateIconOpacity, _bootFont, TextAlignment.Left, int.MaxValue, Plex.Engine.TextRenderers.WrapMode.None);

            ctx.EndDraw();
        }

        private bool _allowControlT = true;

        /// <summary>
        /// Gets or sets whether the player may use CTRL+T to open the Terminal.
        /// </summary>
        public bool AllowTerminalHotkey
        {
            get
            {
                return _allowControlT;
            }
            set
            {
                _allowControlT = value;
            }
        }

        [Dependency]
        private InfoboxManager _infobox = null;

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if (_wgDeskOpen)
            {
                if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Microsoft.Xna.Framework.Input.Keys.T)
                {
                    if(!_allowControlT)
                    {
                        _infobox.Show("Feature not unlocked", "You cannot do this right now. Continue playing to unlock this feature.");
                        return;
                    }
                    var term = new Applications.Terminal(_winmgr);
                    term.Show();
                }
                if (e.Key == Keys.F6)
                {
                    var missionMenu = new MissionMenu(_winmgr);
                    missionMenu.Show();
                }
            }
        }

        private CrashEntity _crash = null;

        [Dependency]
        private WindowSystem _windowSystem = null;

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            switch (_osIntroState)
            {
                case 0:
                    _init = new Applications.SystemInitTerminal(_winmgr);
                    _init.Show();
                    _discord.GameState = "Peacegate OS version 1.4";
                    _discord.GameDetails = "Starting kernel...";
                    _osIntroState++;
                    break;

                case 1:
                    if (_os.PreventStartup == false && (_init.Visible == false || _init.Disposed == true))
                        _osIntroState++;
                    break;
                case 2:
                    _peacegateIconOpacity += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacegateIconOpacity >= 1F)
                    {
                        _peacegateRide = 0;
                        _osIntroState++;
                        _discord.GameState = "Peacegate OS version 1.4";
                        _discord.GameDetails = "System initializing.";

                    }
                    break;
                case 3:
                    _peacegateRide += time.ElapsedGameTime.TotalSeconds;
                    if(_peacegateRide>=5)
                    {
                        _osIntroState++;
                    }
                    break;
                case 4:
                    _peacegateIconOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacegateIconOpacity <= 0)
                    {
                        _osIntroState++;
                    }
                    break;
                case 5:
                    _wgDeskOpen = true;
                    var desk = new DesktopWindow(_winmgr);
                    desk.Show();
                    _desktop = desk;
                    _osIntroState = -1;
                    break;
                case 7:
                    if (_hackedBgmInstance.Pitch > -1F)
                    {
                        _hackedBgmInstance.Pitch = MathHelper.Clamp(_hackedBgmInstance.Pitch - ((float)time.ElapsedGameTime.TotalSeconds * 0.25f), -1, 0);
                    }
                    else
                    {
                        while (_windowSystem.WindowList.Length > 0)
                            _windowSystem.Close(_windowSystem.WindowList[0].WindowID);
                        _crash = _plexgate.New<CrashEntity>();
                        _wgDeskOpen = false;
                        _desktop = null;
                        _plexgate.GetLayer(LayerType.Foreground).AddEntity(_crash);
                        _hackedBgmInstance.Stop();
                        _hackedBgmInstance.Pitch = 0;
                        _hackedBgmInstance.Volume = 1;
                        _osIntroState++;
                    }
                    break;
                case 8:
                    if(!_plexgate.GetLayer(LayerType.Foreground).HasEntity(_crash))
                    {
                        _crash = null;
                        _osIntroState = 0;
                    }
                    break;
            }

            if (_anim == null)
                return;
            if(_anim.IsShowingTutorial)
            {
                _hackedBgmInstance.Volume = MathHelper.Clamp(_hackedBgmInstance.Volume - ((float)time.ElapsedGameTime.TotalSeconds * 3), 0.1F, 1F);
            }
            else
            {
                _hackedBgmInstance.Volume = MathHelper.Clamp(_hackedBgmInstance.Volume + ((float)time.ElapsedGameTime.TotalSeconds), 0.1F, 1F);
            }
            if(_isInConnection)
            {
                if(!_plexgate.GetLayer(LayerType.Foreground).HasEntity(_anim))
                {
                    if(!_plexgate.GetLayer(LayerType.Foreground).HasEntity(_countdown))
                    {
                        _plexgate.GetLayer(LayerType.Foreground).AddEntity(_countdown);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _hackedBgm = content.Load<SoundEffect>("Audio/IncomingConnectionBgm");
            _hackedBgmInstance = _hackedBgm.CreateInstance();
            _hackedBgmInstance.IsLooped = true;

            _bootFont = _theme.Theme.GetFont(TextFontStyle.Header3);
            _osIntroState = 0;
            if (_plexgate.QuietMode)
            {
                _osIntroState = 5;
                _term.RunCommand("init", _term.CreateContext(StreamWriter.Null, StreamReader.Null));
            }
            _peacegate = content.Load<Texture2D>("Desktop/UIIcons/Peacegate");
            _server.BroadcastReceived += _server_BroadcastReceived;
        }

        private bool _isInConnection = false;

        private ObjectiveCountdownEntity _countdown = null;

        [Dependency]
        private MissionManager _mission = null;

        private void _server_BroadcastReceived(Plex.Objects.ServerBroadcastType arg1, System.IO.BinaryReader arg2)
        {
            if(arg1 == Plex.Objects.ServerBroadcastType.SYSTEM_CONNECTED)
            {
                if (_isInConnection == true)
                    return;
                _countdown = _plexgate.New<ObjectiveCountdownEntity>();
                _countdown.TimedOut += () =>
                {
                    if(_isInConnection)
                    {
                        if (_mission.IsPlayingMission)
                            _mission.AbandonMission();
                        _osIntroState = 7;
                        _isInConnection = false;
                        Logger.Log($"{_hackedBgmInstance.Pitch}");
                    }
                };
                _isInConnection = true;
                if(_hackedBgmInstance.State != SoundState.Playing)
                    _hackedBgmInstance.Play();
                _anim = _plexgate.New<HackedAnimationEntity>();
                _plexgate.GetLayer(LayerType.Foreground).AddEntity(_anim);
            }
        }
    }

    [HideInHelp]
    public class simulate_connection : ITerminalCommand
    {
        [Dependency]
        private AsyncServerManager _server = null;

        public string Description
        {
            get
            {
                return "";
            }
        }

        public string Name
        {
            get
            {
                return "simulate_connection";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<ip>";
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string ip = arguments["<ip>"].ToString();
            try
            {
                uint ipaddr = GetIPFromString(ip);
                using (var memstr = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(memstr, Encoding.UTF8, true))
                    {
                        writer.Write(ipaddr);
                        writer.Flush();
                        _server.SendMessage(ServerMessageType.SP_SIMULATE_CONNECTION_TO_PLAYER, memstr.ToArray(), (res, reader) =>
                        {
                            if (res == ServerResponseType.REQ_ERROR)
                                console.WriteLine("An error has occurred.");
                        }).Wait();
                    }
                }
            }
            catch(Exception ex)
            {
                console.WriteLine(ex.Message);
            }
        }

        public uint CombineToUint(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 4)
                throw new ArgumentException($"You cannot convert a {values.Length} byte array to an unsigned integer.");
            int result = 0;
            result = values[0] + (values[1] << 8) + (values[2] << 16) + (values[3] << 24);
            return (uint)result;
        }

        public uint GetIPFromString(string iPAddress)
        {
            if (string.IsNullOrWhiteSpace(iPAddress))
                throw new ArgumentException("IP string cannot be empty.");
            if (!iPAddress.Contains("."))
                throw new FormatException();
            string[] segments = iPAddress.Split('.');
            if (segments.Length != 4)
                throw new FormatException();
            byte seg1 = Convert.ToByte(segments[0]);
            byte seg2 = Convert.ToByte(segments[1]);
            byte seg3 = Convert.ToByte(segments[2]);
            byte seg4 = Convert.ToByte(segments[3]);

            return this.CombineToUint(new byte[] { seg1, seg2, seg3, seg4 });
        }
    }

    public class HackedAnimationEntity : IEntity, ILoadable
    {
        private Texture2D _warning = null;
        private string _text = "[SYSTEM COMPROMISED]";
        private string _desc = "An unauthorized connection to your system has been detected by Peacegate OS.";

        private float _shroudOpacity = 1f;
        private const double _totalSplashLengthSeconds = 6F;
        private double _splashProgress = 0f;
        private int _splashState = 0;

        private float _textOpacity = 0f;
        private float _descOpacity = 0f;

        private float _tutorialShroudOpacity = 0f;
        private float _tutorialTextOpacity = 0f;

        private float _warningOpacity = 0f;

        private bool _willShowHint = false;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private UIManager _ui = null;

        private string _tutorialHead = "Counter-hacking and System Compromisation";
        private string _tutorialBody = @"The Peacenet is in a time of digital warfare. This means that there is always a chance your system will be hacked by another system. It just so happens that this has happened right now.

When Peacegate OS sees a new connection to your system, you'll be given a chance to COUNTER-HACK.

To COUNTER-HACK, grab the IP address of the connected system using the 'connections' command, use 'nmap' to see what ports are open on the target system, and exploit the correct service to gain a reverse shell to the system. Once you're in, use the 'disconnect' command on the remote Terminal to disconnect the target from you. Remember to clear logs in /var/log.

NOTE: YOU ONLY HAVE ONE MINUTE TO DO THIS. If you run out of TIME, your system will crash and you will fail any current MISSIONS. BE CAREFUL.

Press ENTER to dismiss.";

        


        public bool IsShowingTutorial
        {
            get
            {
                return _splashState == 2;
            }
        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            gfx.BeginDraw();
            gfx.Clear((Color.Black * 0.75F) * _shroudOpacity);
            gfx.Clear((Color.Red * 0.25F) * _shroudOpacity);


            int warnHeight = _warning.Height * 2;

            var textFont = _theme.Theme.GetFont(TextFontStyle.Header1);
            var descFont = _theme.Theme.GetFont(TextFontStyle.Header3);

            var textColor = Color.Red.Lighten(0.25F);
            var descColor = Color.White;

            int screenRealEstate = (gfx.Width / 2);

            var textMeasure = TextRenderer.MeasureText(_text, textFont, screenRealEstate, Plex.Engine.TextRenderers.WrapMode.Words);
            var descMeasure = TextRenderer.MeasureText(_desc, descFont, screenRealEstate, Plex.Engine.TextRenderers.WrapMode.Words);

            int totalHeight = warnHeight + 30 + (int)textMeasure.Y + 10 + (int)descMeasure.Y;

            gfx.DrawRectangle((gfx.Width - (_warning.Width * 2)) / 2, (gfx.Height - totalHeight) / 2, _warning.Width * 2, warnHeight, _warning, Color.Red * _warningOpacity);

            int initialY = (gfx.Height - totalHeight) / 2;

            gfx.DrawString(_text, (gfx.Width - screenRealEstate) / 2, initialY + warnHeight + 30, textColor * _textOpacity, textFont, TextAlignment.Center, screenRealEstate, Plex.Engine.TextRenderers.WrapMode.Words);

            gfx.DrawString(_desc, (gfx.Width - screenRealEstate) / 2, initialY + warnHeight + 30 + (int)textMeasure.Y + 10, descColor * _descOpacity, descFont, TextAlignment.Center, screenRealEstate, Plex.Engine.TextRenderers.WrapMode.Words);

            gfx.Clear((Color.Black * 0.95F) * _tutorialShroudOpacity);

            var tutTextMeasure = TextRenderer.MeasureText(_tutorialHead, textFont, (gfx.Width - 30), Plex.Engine.TextRenderers.WrapMode.Words);
            var tutBodyMeasure = TextRenderer.MeasureText(_tutorialBody, descFont, (gfx.Width - 30), Plex.Engine.TextRenderers.WrapMode.Words);

            gfx.DrawString(_tutorialHead, 15, 15, textColor * _tutorialTextOpacity, textFont, TextAlignment.Left, (gfx.Width - 30), Plex.Engine.TextRenderers.WrapMode.Words);
            gfx.DrawString(_tutorialBody, 15, 15 + (int)tutTextMeasure.Y + 10, descColor * _tutorialTextOpacity, descFont, TextAlignment.Left, (gfx.Width - 30), Plex.Engine.TextRenderers.WrapMode.Words);


            gfx.EndDraw();
        }

        public void Load(ContentManager content)
        {
            _willShowHint = !_save.GetValue<bool>("hints.counterhack", false);
            _warning = content.Load<Texture2D>("Infobox/warning");
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Keys.Enter)
            {
                if (_splashState == 2)
                    _splashState = 3;
            }
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void Update(GameTime time)
        {
            switch(_splashState)
            {
                case 0:
                    _splashProgress += time.ElapsedGameTime.TotalSeconds;
                    if(_splashProgress >= _totalSplashLengthSeconds)
                    {
                        _warningOpacity = 0f;
                        _splashState++;
                        return;
                    }
                    _shroudOpacity = MathHelper.Lerp(1f, 0f, (float)(_splashProgress / _totalSplashLengthSeconds));
                    _warningOpacity = (((int)Math.Round(_splashProgress*2) % 2) == 0) ? 1F : 0F;

                    if(_textOpacity>=1F)
                    {
                        if(_descOpacity<=1F)
                        {
                            _descOpacity += (float)time.ElapsedGameTime.TotalSeconds * 4;
                        }
                        else
                        {
                            _descOpacity = 1f;
                        }
                    }
                    else
                    {
                        _textOpacity = MathHelper.Clamp(_textOpacity + ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
                    }
                    break;
                case 1:
                    if (_descOpacity <= 0F)
                    {
                        if (_textOpacity >= 0F)
                        {
                            _textOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                        }
                        else
                        {
                            if(_willShowHint)
                            {
                                _splashState++;
                            }
                            else
                            {
                                _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
                            }
                            _textOpacity = 0f;

                        }
                    }
                    else
                    {
                        _descOpacity = MathHelper.Clamp(_descOpacity - ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
                    }

                    break;
                case 2:
                    _ui.DoInput = false;
                    if(_tutorialShroudOpacity>=1F)
                    {
                        if(_tutorialTextOpacity<1F)
                        {
                            _tutorialTextOpacity = MathHelper.Clamp(_tutorialTextOpacity + ((float)time.ElapsedGameTime.TotalSeconds * 2), 0f, 1f);
                        }
                    }
                    else
                    {
                        _tutorialShroudOpacity = MathHelper.Clamp(_tutorialShroudOpacity + ((float)time.ElapsedGameTime.TotalSeconds * 2), 0f, 1f);
                    }
                    break;
                case 3:
                    if (_tutorialTextOpacity <= 0F)
                    {
                        if (_tutorialShroudOpacity > 0F)
                        {
                            _tutorialShroudOpacity = MathHelper.Clamp(_tutorialShroudOpacity - ((float)time.ElapsedGameTime.TotalSeconds * 2), 0f, 1f);
                        }
                        else
                        {
                            _ui.DoInput = true;
                            _save.SetValue<bool>("hints.counterhack", true);
                            _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
                        }
                    }
                    else
                    {
                        _tutorialTextOpacity = MathHelper.Clamp(_tutorialTextOpacity - ((float)time.ElapsedGameTime.TotalSeconds * 2), 0f, 1f);
                    }

                    break;
            }
        }
    }

    public class CrashEntity : IEntity
    {
        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private bool _isTextShowing = false;

        private string _peacegateError = @"Peacegate OS has suffered a fatal error and needs to be rebooted.

Error code: ERR_OUTOFMEMORY_SWAPSPACEFULL

Any unsaved work has been lost. We apologize for that. However, we will reboot your system shortly and the desktop should load up just fine. Be sure to check /var/log for any further errors that could have caused this.";

        private float _bgWipeAnim = 0f;

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            gfx.BeginDraw();

            var bg = _theme.Theme.GetAccentColor().Darken(0.5F);
            gfx.DrawRectangle(0, 0, gfx.Width, (int)MathHelper.Lerp(1, gfx.Height, _bgWipeAnim), bg);
            if(_isTextShowing)
            {
                gfx.DrawString(_peacegateError, 0, 0, Color.White, _theme.Theme.GetFont(TextFontStyle.Mono), TextAlignment.Left, gfx.Width, Plex.Engine.TextRenderers.WrapMode.Words);
            }

            gfx.EndDraw();
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        private double _flashCounter = 0f;
        private int _flashes = 0;

        private double _despawnTime = 0;

        public void Update(GameTime time)
        {
            _ui.DoInput = false;
            if(_bgWipeAnim>=1f)
            {
                _flashCounter += time.ElapsedGameTime.TotalMilliseconds;
                if(_flashCounter>=50)
                {
                    if(_flashes<3)
                    {
                        _isTextShowing = !_isTextShowing;
                        _flashes++;
                        _flashCounter = 0;
                    }
                }
                if(_flashes>=3)
                {
                    _despawnTime += time.ElapsedGameTime.TotalSeconds;
                    if(_despawnTime>=10)
                    {
                        _ui.DoInput = true;
                        _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
                    }
                }
            }
            else
            {
                _bgWipeAnim += (float)time.ElapsedGameTime.TotalSeconds / 2;
            }
        }
    }
}
