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
using Plex.Objects;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Themes;
using Plex.Engine.Saves;

namespace Peacenet
{
    /// <summary>
    /// A <see cref="IEntity"/> which acts as the Peacegate OS bootscreen. 
    /// </summary>
    public class OSEntity : IEntity, ILoadable, IDisposable
    {
        [Dependency]
        private GameLoop _GameLoop = null;

        public event Action SessionStarted;

        /// <inheritdoc/>
        public void OnGameExit()
        {
            _os.Shutdown();
        }

        #region Boot animation

        private int _osIntroState = 0;
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

        [Dependency]
        private WindowSystem _windowSystem = null;

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
                    _wgDeskOpen = true;
                    var desk = new DesktopWindow(_winmgr);
                    desk.Show();
                    _desktop = desk;
                    _osIntroState = -1;
                    SessionStarted?.Invoke();
                    break;
            }
        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _osIntroState = 0;
            if (_GameLoop.QuietMode)
            {
                _osIntroState = 2;
                _term.RunCommand("init", _term.CreateContext(StreamWriter.Null, StreamReader.Null));
            }
        }
    }
}
