using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Peacenet.Filesystem;
using Peacenet.Applications;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Peacenet.DesktopUI
{
    /// <summary>
    /// The window used for the App Launcher.
    /// </summary>
    public class AppLauncherMenu : Window
    {
        private PictureBox _userIcon = null;
        private Label _userFullName = null;
        private Label _userHostname = null;

        private Stacker _appsCategoryStacker = null;
        private Stacker _appsStacker = null;
        private Stacker _leaveStacker = null;
        private Stacker _computerStacker = null;
        private Stacker _settingsStacker = null;

        private Control _currentPage = null;

        private ScrollView _scroller = null;

        private int _page = 0;

        private bool _closeOnFocusLoss = true;

        /// <summary>
        /// Gets or sets whether the menu will close when it loses focus.
        /// </summary>
        public bool CloseOnFocusLoss
        {
            get
            {
                return _closeOnFocusLoss;
            }
            set
            {
                _closeOnFocusLoss = value;
            }
        }

        //Here are some engine dependencies.
        [Dependency]
        private ItchOAuthClient _itch = null;
        [Dependency]
        private FSManager _fs = null;
        [Dependency]
        private OS _os = null;
        [Dependency]
        private AppLauncherManager _al = null;
        [Dependency]
        private InfoboxManager _infobox = null;
        [Dependency]
        private GameLoop _plexgate = null;
        
        private AppLauncherSectionButton _apps = new AppLauncherSectionButton();
        private AppLauncherSectionButton _computer = new AppLauncherSectionButton();
        private AppLauncherSectionButton _settings = new AppLauncherSectionButton();
        private AppLauncherSectionButton _history = new AppLauncherSectionButton();
        private AppLauncherSectionButton _leave = new AppLauncherSectionButton();


        private DesktopWindow _desktop = null;

        /// <inheritdoc/>
        public AppLauncherMenu(WindowSystem _winsys, DesktopWindow desktop) : base(_winsys)
        {
            _desktop = desktop;

            //Set the width and height
            Width = 484;
            Height = 628;

            //Also, we want to be border-less.
            SetWindowStyle(WindowStyle.NoBorder);
        }

        /// <inheritdoc/>
        public override void Show(int x = -1, int y = -1)
        {
            _scroller = new ScrollView();

            AddChild(_scroller);

            _userIcon = new PictureBox();
            _userFullName = new Label();
            _userHostname = new Label();

            AddChild(_userIcon);
            AddChild(_userFullName);
            AddChild(_userHostname);

            _userFullName.AutoSize = true;
            _userHostname.AutoSize = true;
            _userFullName.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _userIcon.Width = 72;
            _userIcon.Height = 72;

            _userIcon.Texture = _plexgate.Content.Load<Texture2D>("MainMenu/MenuButtons/SinglePlayer");

            _userFullName.Text = (_itch.LoggedIn) ? _itch.User.display_name : "Peacegate OS User";
            if (string.IsNullOrWhiteSpace(_userFullName.Text))
                _userFullName.Text = "Peacegate OS User";
            string itchUsername = (_itch.LoggedIn) ? _itch.User.username : "user";
            string osEdition = "Peacegate OS for Sentient Programs";

            _userHostname.Text = $"{itchUsername}@{_os.Hostname} ({osEdition})";

            _apps = new AppLauncherSectionButton();
            _computer = new AppLauncherSectionButton();
            _settings = new AppLauncherSectionButton();
            _history = new AppLauncherSectionButton();
            _leave = new AppLauncherSectionButton();

            AddChild(_apps);
            AddChild(_computer);
            AddChild(_settings);
            AddChild(_history);
            AddChild(_leave);

            _apps.Name = "Apps";
            _computer.Name = "Computer";
            _settings.Name = "Settings";
            _history.Name = "History";
            _leave.Name = "Leave";

            _apps.Activated += () =>
            {
                _page = 0;
                ResetUI();
            };
            _computer.Activated += () =>
            {
                _page = 2;
                ResetUI();
            };
            _settings.Activated += () =>
            {
                _page = 3;
                ResetUI();
            };
            _history.Activated += () =>
            {
                _page = 4;
                ResetUI();
            };
            _leave.Activated += () =>
            {
                _page = 5;
                ResetUI();
            };


            _appsStacker = new Stacker();
            _appsCategoryStacker = new Stacker();
            _computerStacker = new Stacker();
            _settingsStacker = new Stacker();

            _leaveStacker = new Stacker();

            _appsStacker.AutoSize = true;
            _appsCategoryStacker.AutoSize = true;
            _computerStacker.AutoSize = true;
            _settingsStacker.AutoSize = true;

            _leaveStacker.AutoSize = true;

            var systemSettings = new AppLauncherItem();
            systemSettings.Name = "System Settings";
            systemSettings.Description = "Adjust core settings of the Peacegate OS, such as screen resolution, audio volume, and user interface effects.";
            systemSettings.Activated += () =>
            {
                var gameSettings = new GameSettings(WindowSystem);
                gameSettings.Show();
            };
            _settingsStacker.AddChild(systemSettings);

            var leavePeacegate = new AppLauncherItem();
            leavePeacegate.Name = "Exit Peacegate";
            leavePeacegate.Description = "Exit your Peacegate OS session, closing all your open programs and terminating your connection to The Peacenet.";
            leavePeacegate.Activated += () =>
            {
                foreach (var window in WindowSystem.WindowList)
                    window.Border.Enabled = false;
                _infobox.ShowYesNo("Exit Peacegate", "Are you sure you want to exit Peacegate OS?", (answer) =>
                {
                    foreach (var window in WindowSystem.WindowList)
                        window.Border.Enabled = true;
                    if(answer)
                    {
                        foreach (var window in WindowSystem.WindowList)
                            if (window.Border != _desktop.Parent)
                                WindowSystem.Close(window.WindowID);
                        _desktop.Shutdown();
                    }
                });
            };
            _leaveStacker.AddChild(leavePeacegate);

            foreach(var cat in _al.GetAllCategories().OrderBy(z=>z))
            {
                var item = new AppLauncherItem();
                item.Name = cat;
                item.Activated += () =>
                {
                    _page = 1;
                    SetupCategory(cat);
                    ResetUI();
                };
                _appsStacker.AddChild(item);
            }

            var runCommand = new AppLauncherItem();
            runCommand.Name = "Run command...";
            runCommand.Description = "Run a Terminal Command.";
            runCommand.Activated += () =>
            {
                _infobox.PromptText("Run Command", "Enter a command to run.", (command) =>
                {
                    var terminal = new RunCommandTerminal(WindowSystem, command);
                    terminal.Show();
                });
            };
            _computerStacker.AddChild(runCommand);

            foreach(var dir in _os.GetShellDirs().OrderBy(z=>z.FriendlyName))
            {
                var shellItem = new AppLauncherItem();
                shellItem.Icon = dir.Texture;
                shellItem.Name = dir.FriendlyName;
                shellItem.Description = dir.Path;
                shellItem.Activated += () =>
                {
                    var fm = new FileManager(WindowSystem);
                    fm.CurrentPath = dir.Path;
                    fm.Show();
                };
                _computerStacker.AddChild(shellItem);
            }

            ResetUI();

            base.Show(x, y);
        }

        private void SetupCategory(string category)
        {
            _appsCategoryStacker.Clear();
            foreach(var item in _al.GetAllInCategory(category).OrderBy(x=>x.Attribute.Name))
            {
                var albutton = new AppLauncherItem();
                albutton.Name = item.Attribute.Name;
                albutton.Description = item.Attribute.Description;
                albutton.Activated += () =>
                {
                    var win = (Window)Activator.CreateInstance(item.WindowType, new[] { WindowSystem });
                    win.Show();
                };
                _appsCategoryStacker.AddChild(albutton);
            }
        }

        private void ResetUI()
        {
            _scroller.Clear();
            _currentPage = null;
            switch(_page)
            {
                case 0:
                    _currentPage = _appsStacker;
                    break;
                case 1:
                    _currentPage = _appsCategoryStacker;
                    break;
                case 2:
                    _currentPage = _computerStacker;
                    break;
                case 3:
                    _currentPage = _settingsStacker;
                    break;
                case 5:
                    _currentPage = _leaveStacker;
                    break;
            }
            if (_currentPage != null)
                _scroller.AddChild(_currentPage);

            if(Manager != null)
                Manager.SetFocus(this);
        }

        private bool _allowShutdowns = true;

        /// <summary>
        /// Gets or sets whether the player may shut down the system.
        /// </summary>
        public bool AllowShutdown
        {
            get
            {
                return _allowShutdowns;
            }
            set
            {
                _allowShutdowns = value;
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (!HasFocused)
                if(_closeOnFocusLoss)
                    Close();

            int widthWithPadding = Width - 30;
            int widthAvailableForUserInfo = (widthWithPadding - _userIcon.Width) - 15;

            _userFullName.MaxWidth = widthAvailableForUserInfo;
            _userHostname.MaxWidth = widthAvailableForUserInfo;

            _userIcon.X = 15;
            _userFullName.X = _userIcon.X + _userIcon.Width + 7;
            _userHostname.X = _userFullName.X;

            int userInfoHeight = _userFullName.Height + 5 + _userHostname.Height;

            if(Math.Max(userInfoHeight, _userIcon.Height) == _userIcon.Height)
            {
                _userIcon.Y = (Height - _userIcon.Height) - 15;
            }
            else
            {
                _userIcon.Y = (Height - userInfoHeight) - 15;
            }

            _userFullName.Y = _userIcon.Y;
            _userHostname.Y = _userFullName.Y + _userFullName.Height + 5;

            _apps.X = 2;
            _apps.Y = 2;

            _computer.X = _apps.X + _apps.Width;
            _computer.Y = 2;

            _settings.X = _computer.X + _computer.Width;
            _settings.Y = 2;

            _history.X = _settings.X + _settings.Width;
            _history.Y = 2;

            _leave.X = _history.X + _history.Width;
            _leave.Y = 2;

            _leave.Enabled = _allowShutdowns;

            _apps.Active = (_page == 0 || _page == 1);
            _computer.Active = _page == 2;
            _settings.Active = _page == 3;
            _history.Active = _page == 4;
            _leave.Active = _page == 5;

            _scroller.X = 0;
            _scroller.Y = _apps.Y + _apps.Height;
            if(_currentPage!=null)
                _currentPage.Width = Width;
            _scroller.Height = ((_userFullName.Y - 15) - _scroller.Y);

            base.OnUpdate(time);
        }
    }

    public class AppLauncherSectionButton : Control
    {
        private PictureBox _icon = new PictureBox();
        private Label _name = new Label();

        private bool _active = false;

        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active == value)
                    return;
                _active = value;
            }
        }

        public AppLauncherSectionButton()
        {
            AddChild(_icon);
            AddChild(_name);
            Click += (o, a) =>
            {
                Activated?.Invoke();
            };
            _name.Click += (o, a) =>
            {
                Activated?.Invoke();
            };

            _icon.Click += (o, a) =>
            {
                Activated?.Invoke();
            };
        }

        public Texture2D Icon
        {
            get
            {
                return _icon.Texture;
            }
            set
            {
                _icon.Texture = value;
            }
        }

        public string Name
        {
            get
            {
                return _name.Text;
            }
            set
            {
                _name.Text = value;
            }
        }

        public event Action Activated;

        protected override void OnUpdate(GameTime time)
        {
            Width = 96;
            _icon.Width = 64;
            _icon.Height = 64;
            _icon.X = (Width - _icon.Width) / 2;

            _name.AutoSize = true;
            _name.MaxWidth = Width - 4;
            _name.X = (Width - _name.Width) / 2;
            _name.Alignment = TextAlignment.Center;
            _name.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;

            int totalHeight = _icon.Height + 5 + _name.Height;

            Height = totalHeight + 16;

            _name.Y = (Height - _name.Height) - 5;
            _icon.Y = (_name.Y - _icon.Height) / 2;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if(_active)
            {
                Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
                gfx.FillRectangle(0, Height - 2, Width, 2, Theme.GetAccentColor());
            }
            else
            {
                if(LeftButtonPressed || _name.LeftButtonPressed || _icon.LeftButtonPressed)
                {
                    Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
                }
                else if(ContainsMouse)
                {
                    Theme.DrawControlLightBG(gfx, 0, 0, Width, Height);
                }
                else
                {
                    Theme.DrawControlBG(gfx, 0, 0, Width, Height);

                }
            }
        }
    }

    public class AppLauncherItem : Control
    {
        private PictureBox _icon = new PictureBox();
        private Label _name = new Label();
        private Label _description = new Label();

        public event Action Activated;

        public AppLauncherItem()
        {
            AddChild(_name);
            AddChild(_description);
            AddChild(_icon);

            Click += (o, a) =>
            {
                Activated?.Invoke();
            };
            _icon.Click += (o, a) =>
            {
                Activated?.Invoke();
            };
            _name.Click += (o, a) =>
            {
                Activated?.Invoke();
            };
            _description.Click += (o, a) =>
            {
                Activated?.Invoke();
            };

        }

        public string Name
        {
            get
            {
                return _name.Text;
            }
            set
            {
                _name.Text = value;
            }
        }

        public string Description
        {
            get
            {
                return _description.Text;
            }
            set
            {
                _description.Text = value;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return _icon.Texture;
            }
            set
            {
                _icon.Texture = value;
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
            {
                Visible = false;
                return;
            }
            else
            {
                if (Visible != true)
                    Visible = true;
            }
            Width = Parent.Width;

            _icon.Width = 64;
            _icon.Height = 64;
            _icon.X = 4;

            _name.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;

            _name.AutoSize = true;
            _description.AutoSize = true;

            int widthWithPadding = Width - 8;

            int textWidth = (widthWithPadding - _icon.Width) - 3;

            _name.MaxWidth = textWidth;
            _description.MaxWidth = textWidth;

            _name.X = _icon.X + _icon.Width + 3;
            _description.X = _name.X;

            int textHeight = _name.Height + 3 + _description.Height;

            if(Math.Max(textHeight, _icon.Height) == textHeight)
            {
                Height = textHeight + 8;
            }
            else
            {
                Height = _icon.Height + 8;
            }

            _icon.Y = (Height - _icon.Height) / 2;

            _name.Y = (Height - textHeight) / 2;
            _description.Y = _name.Y + _name.Height + 3;

            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var accent = Theme.GetAccentColor();
            var down = accent.Darken(0.5f);
            var hover = accent.Darken(0.25F);
            if (LeftButtonPressed || _icon.LeftButtonPressed || _name.LeftButtonPressed || _description.LeftButtonPressed)
            {
                gfx.FillRectangle(0, 0, Width, Height, down);
            }
            else if (ContainsMouse)
            {
                gfx.FillRectangle(0, 0, Width, Height, hover);
            }
            else
            {
                Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            }
        }
    }

    public class Stacker : Panel
    {
        protected override void OnUpdate(GameTime time)
        {
            int y = 0;
            foreach(var child in Children)
            {
                child.Width = Width;
                child.Y = y;
                child.X = 0;
                y += child.Height;
            }
            if(AutoSize)
            {
                Height = y;
            }
        }
    }

    public class RunCommandTerminal : Terminal
    {
        private string cmd = "";

        protected override string Shell
        {
            get
            {
                return cmd;
            }
        }

        public RunCommandTerminal(WindowSystem _winsys, string command) : base(_winsys)
        {
            cmd = command;
        }
    }
}
