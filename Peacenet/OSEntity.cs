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

namespace Peacenet
{
    /// <summary>
    /// A <see cref="IEntity"/> which acts as the Peacegate OS bootscreen. 
    /// </summary>
    public class OSEntity : IEntity, ILoadable, IDisposable
    {
        [Dependency]
        private AsyncServerManager _server = null;

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
            }
        }

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
            }

        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _bootFont = content.Load<SpriteFont>("ThemeAssets/Fonts/Head1");
            _osIntroState = 0;
            _peacegate = content.Load<Texture2D>("Desktop/UIIcons/Peacegate");
            _server.BroadcastReceived += _server_BroadcastReceived;
        }

        private void _server_BroadcastReceived(Plex.Objects.ServerBroadcastType arg1, System.IO.BinaryReader arg2)
        {
            if(arg1 == Plex.Objects.ServerBroadcastType.SYSTEM_CONNECTED)
            {
                Desktop.ShowNotification("New connection.", "Someone has connected to your Peacenet node. Run the 'connections' Terminal Command for info.");
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
}
