#define DISABLED_MP

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
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Cutscenes;
using System.IO;
using System.Threading;
using Peacenet.DesktopUI;
using Plex.Objects;
using Peacenet.PeacegateThemes;
using Peacenet.GameState;

namespace Peacenet.MainMenu
{
    /// <summary>
    /// A Peace engine entity which displays the Watercolor Games splash screen and Peacenet's main menu.
    /// </summary>
    public class SplashEntity : IEntity, ILoadable, IDisposable
    {
        public void OnGameExit() { }
        private SpriteFont _Font = null;

        private const string _grubHeader = "GNU GRUB  version 2.00-7peacegate1";
        private const string _helptext = @"Use the [UP] and [DOWN] keys to select which item is highlighted.
Press enter to boot the selected OS, 'e' to edit the commands
before boot or 'c' for a command line.";
        private const string _helptextSubmenu = @"Use the [UP] and [DOWN] keys to select which item is highlighted.
Press enter to boot the selected OS, 'e' to edit the commands
before boot or 'c' for a command line. Press [BACKSPACE] or [ESC] to go back.";


        private const int _rectThickness = 4;

        private string[] _mainEntries = new string[] { "Peacegate OS", "Memtest86+ (RAM test)", "System Configuration", "Power Off" };
        private int _selectedEntry = 0;
        private int _menuState = 0;
        private string[] _empty = new string[0];
        private string[] _osBootMenu = null;

        private string[] GetMenuEntries()
        {
            switch(_menuState)
            {
                case 0:
                    return _mainEntries;
                case 1:
                    return _osBootMenu;
                default:
                    return _empty;
            }
        }


        #region Engine dependencies

        [Dependency]
        private PeacenetThemeManager _pn = null;

        [Dependency]
        private UIManager _uimanager = null;

        [Dependency]
        private ThemeManager _thememgr = null;

        [Dependency]
        private GameLoop _plexgate = null;

        [Dependency]
        private WindowSystem _windowManager = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private SaveManager _saveManager = null;

        [Dependency]
        private SplashScreenComponent _splash = null;

        [Dependency]
        private CutsceneManager _cutscene = null;

        [Dependency]
        private ItchOAuthClient _api = null;

        [Dependency]
        private OS _os = null;

        #endregion

        [Dependency]
        private GameManager _game = null;

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        private bool _isWaitingForSignin = false;

        private void startSinglePlayer()
        {
            _game.BeginGame<SinglePlayerStateInfo>();
            _gameStarting = true;

        }

        private bool _gameStarting = false;

        private bool _gameExists = false;

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _Font = content.Load<SpriteFont>("ThemeAssets/Fonts/Mono/Large");
            _gameExists = System.IO.Directory.Exists(_os.SinglePlayerSaveDirectory);
        }

        private void login()
        {
            _api.Login("17a4b2de3caf06c14a524936d88402c1", () =>
            {
                _isWaitingForSignin = true;
            }, () =>
            {
                _isWaitingForSignin = false;
            });
        }

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if (SettingsOpen)
                return;
            var entryCount = GetMenuEntries().Length;
            if(e.Key == Keys.Up)
            {
                if (_selectedEntry > 0)
                    _selectedEntry--;
            }
            if(e.Key == Keys.Down)
            {
                if (_selectedEntry < entryCount - 1)
                    _selectedEntry++;
            }
            if(e.Key == Keys.Enter)
            {
                ProcessEntry(_menuState, _selectedEntry);
            }
            if(e.Key == Keys.Back || e.Key == Keys.Escape)
            {
                _menuState = 0;
                _selectedEntry = 0;
            }
            if(e.Key == Keys.C)
            {
                _splash.MakeHidden();
                _cutscene.Play("credits_00", () =>
                {
                    _splash.MakeVisible();
                });
            }
        }

        private bool SettingsOpen => _windowManager.WindowList.Any(x => x.Border.Window is GameSettings);

        private void ProcessEntry(int state, int item)
        {
            switch(state)
            {
                case 0:
                    switch(item)
                    {
                        case 0:
                            _menuState = 1;
                            if (_gameExists)
                                _osBootMenu = new string[] { "New installation", "Existing OS" };
                            else
                                _osBootMenu = new string[] { "New installation" };
                            break;
                        case 2:
                            var settings = new GameSettings(_windowManager);
                            settings.SetWindowStyle(WindowStyle.NoBorder);
                            settings.Show();
                            break;
                        case 3:
                            _plexgate.Exit();
                            break;
                    }
                    break;
                case 1:
                    switch(item)
                    {
                        case 0:
                            if (System.IO.Directory.Exists(_os.SinglePlayerSaveDirectory))
                                Directory.Delete(_os.SinglePlayerSaveDirectory, true);
                            startSinglePlayer();
                            _splash.MakeHidden();
                            _os.OnReady();
                            break;
                        case 1:
                            startSinglePlayer();
                            _splash.MakeHidden();
                            _os.OnReady();
                            break;
                    }
                    break;
            }
            _selectedEntry = 0;
        }

        [Dependency]
        private DiscordRPCModule _discord = null;

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext gfx)
        {
            if (SettingsOpen)
                return;
            string help = (_menuState == 0) ? _helptext : _helptextSubmenu;
            var charsize = _Font.MeasureString("#");
            var grubSize = _Font.MeasureString(_grubHeader);
            var helpSize = _Font.MeasureString(help);
            gfx.DrawString(_Font, _grubHeader, new Vector2((gfx.Width - grubSize.X) / 2, charsize.Y), Color.White);
            gfx.DrawString(_Font, help, new Vector2((gfx.Width - helpSize.X) / 2, (gfx.Height - charsize.Y) - helpSize.Y), Color.White);

            float rectY = 2 * charsize.Y + (charsize.Y + _rectThickness);
            float rectX = 3 * charsize.X + (charsize.X - _rectThickness);
            float rectWidth = gfx.Width - (rectX * 2);
            float rectBottom = (gfx.Height - (4 * charsize.Y)) - (charsize.Y - _rectThickness);
            float rectHeight = rectBottom - rectY;
            gfx.FillRectangle(rectX, rectY, rectWidth, rectHeight, Color.White);
            gfx.FillRectangle(rectX + _rectThickness, rectY + _rectThickness, rectWidth - (_rectThickness * 2), rectHeight - (_rectThickness * 2), Color.Black);

            float rectInnerX = rectX + _rectThickness;
            float rectInnerY = rectY + _rectThickness;
            float rectInnerWidth = rectWidth - (_rectThickness * 2);

            var items = GetMenuEntries();

            for(int i = 0; i < items.Length; i++)
            {
                string text = items[i];
                Color bg = Color.Black;
                if (i == _selectedEntry)
                    bg = Color.Blue.Darken(0.5F);
                float y = rectInnerY + (charsize.Y * i);
                gfx.FillRectangle(rectInnerX, y, rectInnerWidth, charsize.Y, bg);
                gfx.DrawString(_Font, text, new Vector2(rectInnerX, y), Color.White);
            }
        }

    }
}
