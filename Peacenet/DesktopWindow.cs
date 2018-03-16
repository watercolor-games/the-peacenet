using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Saves;
using Peacenet.Filesystem;
using Peacenet.CoreUtils;
using Peacenet.Server;
using Peacenet.DesktopUI;
using Peacenet.Filesystem;

namespace Peacenet
{
    /// <summary>
    /// The Peacegate OS Desktop Environment user interface.
    /// </summary>
    public class DesktopWindow : Window
    {
        private int _animState = 0;
        private float _scaleAnim = 0;
        private Texture2D _wallpaper = null;
        private float _panelAnim = -1;

        private ContextMenu _desktopRightClick = null;

        private SoundEffect _noteSound = null;

        private bool _showPanels = true;

        private float _notificationBannerFade = 0f;
        private double _notificationRide = 0;
        private int _notificationAnimState = 0;
        private Label _notificationTitle = new Label();
        private Label _notificationDescription = new Label();


        public bool ShowPanels
        {
            get
            {
                return _showPanels;
            }
            set
            {
                if (_showPanels == value)
                    return;
                _showPanels = value;
                if(value)
                {
                    _panelAnim = 0;
                    _animState = 1;
                }
            }
        }

        private DesktopPanel _topPanel = null;
        private DesktopPanel _bottomPanel = null;

        private Label _timeLabel = new Label();

        private PictureBox _showDesktopIcon = new PictureBox();

        private Label _appLauncherText = new Label();

        private DesktopPanelItemGroup _windowList = new DesktopPanelItemGroup();

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        private WindowSystem winsys = null; //why isn't the current winmgr a property of all Window objects

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private AsyncServerManager _server = null;

        private Button _missionButton = new Button();

        private ListView _desktopIconsView = null;

        private AppLauncherMenu _applauncher = null;

        IEnumerable<int> hiddenWindows = null;

        private void ResetWallpaper()
        {
            string wallpaperId = _save.GetValue("desktop.wallpaper", "DesktopBackgroundImage2");
            try
            {
                _wallpaper = _plexgate.Content.Load<Texture2D>("Desktop/" + wallpaperId);
            }
            catch
            {
                //ContentManager throws if the asset path isn't found or is invalid.
                wallpaperId = "DesktopBackgroundImage2";
                _save.SetValue("desktop.wallpaper", wallpaperId);
                _wallpaper = _plexgate.Content.Load<Texture2D>("Desktop/" + wallpaperId);
            }
            Invalidate(true);
        }

        /// <inheritdoc/>
        public DesktopWindow(WindowSystem _winsys) : base(_winsys)
        {
            _desktopRightClick = new ContextMenu(_winsys);

            _noteSound = _plexgate.Content.Load<SoundEffect>("SFX/DesktopNotification");

            _applauncher = new AppLauncherMenu(_winsys, this);

            _missionButton.Image = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/flag");
            _missionButton.ShowImage = true;
            _missionButton.Text = "NYI";

            winsys = _winsys;
            SetWindowStyle(WindowStyle.NoBorder);
            ResetWallpaper();
            _os.WallpaperChanged += ResetWallpaper;
            _topPanel = new DesktopPanel();
            _bottomPanel = new DesktopPanel();
            AddChild(_topPanel);
            AddChild(_bottomPanel);
            _timeLabel = new Label();
            _timeLabel.AutoSize = true;
            _topPanel.AddChild(_timeLabel);

            _showDesktopIcon.Texture = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/ShowDesktop");
            _showDesktopIcon.AutoSize = true;
            _bottomPanel.AddChild(_showDesktopIcon);

            _topPanel.AddChild(_appLauncherText);
            _appLauncherText.AutoSize = true;
            _appLauncherText.FontStyle = Plex.Engine.Themes.TextFontStyle.Custom;

            _bottomPanel.AddChild(_windowList);

            ResetWindowList(_winsys);
            _winsys.WindowListUpdated += WindowSystemUpdated;
            _desktopIconsView = new ListView();
            AddChild(_desktopIconsView);
            _desktopIconsView.IconFlow = IconFlowDirection.TopDown;

            _desktopIconsView.ItemClicked += _desktopIconsView_ItemClicked;
            _desktopIconsView.SetImage("folder", _plexgate.Content.Load<Texture2D>("UIIcons/folder"));
            _topPanel.AddChild(_missionButton);

            _missionButton.Click += (o, a) =>
            {
            };

            RightClick += Desktop_RightClick;
            _desktopIconsView.RightClick += Desktop_RightClick;



            _appLauncherText.Click += (o, a) =>
            {
                if (_winsys.WindowList.FirstOrDefault(x => x.Border == _applauncher.Parent) != null)
                {
                    _applauncher.Close();
                }
                else
                {
                    if (_applauncher.Disposed)
                        _applauncher = new AppLauncherMenu(_winsys, this);
                    _applauncher.Show(0, _topPanel.Height);
                }
            };

            EventHandler wscallback = null;
            wscallback = (o, a) => { hiddenWindows = null; winsys.WindowStateChanged -= wscallback; };
            _showDesktopIcon.Click += (o, a) =>
            {
                if (hiddenWindows == null) // show desktop
                {
                    hiddenWindows = winsys.WindowList.Where(w => w.Border.Visible && !(w.Border.Window is DesktopWindow)).Select(w => w.WindowID).ToList();
                    foreach (var id in hiddenWindows)
                        winsys.Hide(id);
                    winsys.WindowStateChanged += wscallback;
                }
                else // restore windows
                {
                    winsys.WindowStateChanged -= wscallback;
                    foreach (var id in hiddenWindows)
                        winsys.Show(id);
                    hiddenWindows = null;
                }
            };

            AddChild(_notificationTitle);
            AddChild(_notificationDescription);

            _notificationTitle.AutoSize = true;
            _notificationDescription.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            
            _notificationDescription.AutoSize = true;
            _notificationDescription.MaxWidth = 450;
            _notificationTitle.MaxWidth = _notificationDescription.MaxWidth;

            _fs.WriteOperation += (path) =>
            {
                if (path.StartsWith("/home/Desktop"))
                    _needsDesktopReset = true;
            };
        }

        private void Desktop_RightClick(object sender, EventArgs e)
        {
            _desktopRightClick.ClearItems();
            var newFolder = new MenuItem
            {
                Text = "New Folder..."
            };
            newFolder.Activated += () =>
            {
                string folderPath = "/home/Desktop";
                _infobox.PromptText("New folder", "Please enter a name for your new folder.", (name) =>
                {
                    string fullPath = (folderPath.EndsWith("/")) ? folderPath + name : folderPath + "/" + name;
                    _fs.CreateDirectory(fullPath);
                }, (proposedName) =>
                {
                    if (string.IsNullOrWhiteSpace(proposedName))
                    {
                        _infobox.Show("New folder", "Your folder's name must not be blank.");
                        return false;
                    }

                    foreach (char c in proposedName)
                    {
                        if (char.IsLetterOrDigit(c))
                            continue;
                        if (c == '_' || c == ' ' || c == '-' || c == '.')
                            continue;
                        _infobox.Show("Invalid path character", "Your new folder's name contains an invalid character. Valid characters include any letter or number as well as '.', '_', '-' or a space.");
                        return false;
                    }

                    string fullPath = (folderPath.EndsWith("/")) ? folderPath + proposedName : folderPath + "/" + proposedName;
                    if (_fs.DirectoryExists(fullPath) || _fs.FileExists(fullPath))
                    {
                        _infobox.Show("New folder", "A folder or file already exists with that name.");
                        return false;
                    }

                    return true;
                });

            };
            var changeDesktopBackground = new MenuItem
            {
                Text = "Change Desktop Background..."
            };
            changeDesktopBackground.Activated += () =>
            {
                var appearance = new Applications.Appearance(WindowSystem);
                appearance.Show();
            };
            _desktopRightClick.AddItem(newFolder);
            if(_desktopIconsView.SelectedItem!=null)
            {
                var path = _desktopIconsView.SelectedItem.Tag.ToString();
                var name = _desktopIconsView.SelectedItem.Value;

                var delete = new MenuItem
                {
                    Text = (_fs.DirectoryExists(path)) ? "Delete folder" : "Delete file"
                };
                delete.Activated += () =>
                {
                    _infobox.ShowYesNo(delete.Text, "Are you sure you want to delete \"" + path + "\"?", (answer) =>
                    {
                        if(answer)
                        {
                            _fs.Delete(path);
                        }
                    });
                };
                _desktopRightClick.AddItem(delete);
            }
            _desktopRightClick.AddItem(changeDesktopBackground);

            _desktopRightClick.Show(MouseX, MouseY);
        }

        /// <summary>
        /// Show a notification banner on the desktop.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="description">The message for the notification.</param>
        public void ShowNotification(string title, string description)
        {
            _noteSound.Play();
            _notificationTitle.Text = title;
            _notificationDescription.Text = description;
            _notificationAnimState = 0;
            _notificationBannerFade = 0;
        }


        private bool _appLauncherClosesWhenFocusLost = true;

        /// <summary>
        /// Gets or sets whether the App Launcher should close when its focus is lost.
        /// </summary>
        public bool CloseALOnFocusLoss
        {
            get
            {
                return _appLauncherClosesWhenFocusLost;
            }
            set
            {
                _appLauncherClosesWhenFocusLost = value;
            }
        }

        /// <summary>
        /// Retrieves whether the App Launcher menu is open.
        /// </summary>
        public bool IsAppLauncherOpen
        {
            get
            {
                return WindowSystem.WindowList.FirstOrDefault(x => x.Border == _applauncher.Parent) != null;
            }
        }

        /// <summary>
        /// Retrieves whether the tutorial overlay is showing.
        /// </summary>
        public bool IsTutorialOpen
        {
            get
            {
                return false;
            }
        }
   
        private void _desktopIconsView_ItemClicked(ListViewItem obj)
        {
            if (_fs.DirectoryExists(obj.Tag.ToString()))
            {
                var browser = new Applications.FileManager(WindowSystem);
                browser.SetCurrentDirectory(obj.Tag.ToString());
                browser.Show();
            }
            else
            {
                if (!_utils.OpenFile(obj.Tag.ToString()))
                {
                    _infobox.Show("Can't open file", "File Manager couldn't find a program that can open that file!");
                }
            }
        }

        [Dependency]
        private FileUtilities _utils = null;

        private bool _needsDesktopReset = true;
        
        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private FileUtils _futils = null;

        /// <summary>
        /// Repopulates the desktop icon list view.
        /// </summary>
        public void SetupIcons()
        {
            _desktopIconsView.ClearItems();
            if (!_fs.DirectoryExists("/home/Desktop"))
                _fs.CreateDirectory("/home/Desktop");
            foreach(var dir in _fs.GetDirectories("/home/Desktop"))
            {
                if (_futils.GetNameFromPath(dir).StartsWith("."))
                    continue;
                var diritem = new ListViewItem();
                diritem.Tag = dir;
                diritem.Value = _futils.GetNameFromPath(dir);
                diritem.ImageKey = "folder";
                _desktopIconsView.AddItem(diritem);
            }
            foreach (var dir in _fs.GetFiles("/home/Desktop"))
            {
                if (_futils.GetNameFromPath(dir).StartsWith("."))
                    continue;
                var diritem = new ListViewItem();
                diritem.Tag = dir;
                diritem.Value = _futils.GetNameFromPath(dir);
                diritem.ImageKey = _futils.GetMimeType(dir);
                if(_desktopIconsView.GetImage(diritem.ImageKey) == null)
                {
                    _desktopIconsView.SetImage(diritem.ImageKey, _futils.GetMimeIcon(diritem.ImageKey));
                }
                _desktopIconsView.AddItem(diritem);
            }
        }

        private void WindowSystemUpdated (object o, EventArgs a)
        {
            ResetWindowList((WindowSystem)o);
        }

        /// <summary>
        /// Gets the App Launcher Menu for this desktop.
        /// </summary>
        public AppLauncherMenu AppLauncher
        {
            get
            {
                return _applauncher;
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _topPanel.Visible = _showPanels;
            _bottomPanel.Visible = _showPanels;

            if (IsAppLauncherOpen)
                _applauncher.CloseOnFocusLoss = _appLauncherClosesWhenFocusLost;

            switch (_animState)
            {
                case 0:
                    _scaleAnim += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_scaleAnim >= 1)
                    {
                        _animState++;
                    }
                    break;
                case 1:
                    _panelAnim += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_panelAnim >= 1)
                    {
                        _animState++;
                    }
                    break;
                case 3:
                    _panelAnim -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_panelAnim <= 0)
                    {
                        _animState++;
                    }

                    break;
                case 4:
                    _scaleAnim -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_scaleAnim <= 0)
                    {
                        _animState++;
                    }

                    break;
                case 5:
                    winsys.WindowListUpdated -= WindowSystemUpdated;
                    _os.Shutdown();
                    Close();
                    break;
            }

            switch(_notificationAnimState)
            {
                case 0:
                    _notificationBannerFade += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if(_notificationBannerFade>=1)
                    {
                        _notificationBannerFade = 1;
                        _notificationAnimState++;
                        _notificationRide = 0;
                    }
                    Invalidate(true);
                    break;
                case 1:
                    _notificationRide += time.ElapsedGameTime.TotalSeconds;
                    Invalidate(true);
                    if(_notificationRide>=5)
                    {
                        _notificationAnimState++;
                    }
                    break;
                case 2:
                    _notificationBannerFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_notificationBannerFade <= 0)
                    {
                        _notificationBannerFade = 0;
                        _notificationAnimState = -1;
                        _notificationRide = 0;
                    }
                    Invalidate(true);
                    break;
            }
           Width = (int)MathHelper.Lerp((Manager.ScreenWidth * 0.75f), Manager.ScreenWidth, _scaleAnim);
            Height = (int)MathHelper.Lerp((Manager.ScreenHeight * 0.75f), Manager.ScreenHeight, _scaleAnim);
            Parent.X = (Manager.ScreenWidth - Width) / 2;
            Parent.Y = (Manager.ScreenHeight - Height) / 2;
            Parent.Opacity = _scaleAnim;

            int noteYMin = 0;
            int noteYMax = _topPanel.Y + _topPanel.Height + 15;
            int noteY = (int)MathHelper.Lerp(noteYMin, noteYMax, _notificationBannerFade);
            _notificationTitle.Opacity = _notificationBannerFade;
            _notificationDescription.Opacity = _notificationTitle.Opacity;
            _notificationTitle.Y = noteY;
            _notificationDescription.Y = _notificationTitle.Y + _notificationTitle.Height + 10;
            int noteWidthMax = Math.Max(_notificationTitle.Width, _notificationDescription.Width);
            _notificationTitle.X = Width - noteWidthMax - 15;
            _notificationDescription.X = _notificationTitle.X;

            _topPanel.Height = 24;
            _topPanel.Width = Width;
            _bottomPanel.Height = 24;
            _bottomPanel.Width = Width;
            _topPanel.X = 0;
            _bottomPanel.X = 0;
            _topPanel.Y = (int)MathHelper.Lerp(0 - _topPanel.Height, 0, _panelAnim);
            _bottomPanel.Y = (int)MathHelper.Lerp(Height, Height - _bottomPanel.Height, _panelAnim);

            _timeLabel.Y = (_topPanel.Height - _timeLabel.Height) / 2;
            _timeLabel.X = (_topPanel.Width - _timeLabel.Width) - 2;
            _timeLabel.Text = DateTime.Now.TimeOfDay.ToPresentableString();

            _showDesktopIcon.X = 2;
            _showDesktopIcon.Y = 2;
            _showDesktopIcon.Tint = (_showDesktopIcon.ContainsMouse ^ (hiddenWindows != null)) ? Color.White : new Color(191, 191, 191, 255);

            _appLauncherText.X = 2;
            _appLauncherText.CustomFont = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);
            _appLauncherText.CustomColor = (_appLauncherText.ContainsMouse) ? Color.White : new Color(191, 191, 191, 255);
            _appLauncherText.Text = "Peacegate";
            _appLauncherText.Y = (_topPanel.Height - _appLauncherText.Height) / 2;

            _windowList.Y = 0;
            _windowList.Height = _bottomPanel.Height;
            _windowList.X = _showDesktopIcon.X + _showDesktopIcon.Width + 2;
            _windowList.Width = _bottomPanel.Width - _windowList.X;

            _desktopIconsView.X = 0;
            _desktopIconsView.Y = _topPanel.Y + _topPanel.Height;
            _desktopIconsView.Height = _bottomPanel.Y - _desktopIconsView.Y;

            if (_server.Connected)
            {
                if (_needsDesktopReset)
                {
                    SetupIcons();
                    _needsDesktopReset = false;
                }
            }
            else
            {
                if (_animState < 3)
                {
                    foreach (var win in winsys.WindowList.ToArray())
                    {
                        if (win.Border != this.Parent)
                            winsys.Close(win.WindowID);
                    }
                    _animState = 3;

                }
            }

            _missionButton.Y = (_topPanel.Height - _missionButton.Height) / 2;
            _missionButton.X = _timeLabel.X - _missionButton.Width - 3;

            base.OnUpdate(time);
        }

        /// <summary>
        /// Plays the Desktop Close animation and shuts down the session, returning to main menu.
        /// </summary>
        public void Shutdown()
        {
            _animState = 3;
        }

        [Dependency]
        private OS _os = null;

        /// <summary>
        /// Gets or sets whether the App Launcher button is visible.
        /// </summary>
        public bool ShowAppLauncherButton
        {
            get
            {
                return _appLauncherText.Visible;
            }
            set
            {
                _appLauncherText.Visible = value;
            }
        }

        private void ResetWindowList(WindowSystem winsys)
        {
            while (_windowList.Children.Length > 0)
                _windowList.RemoveChild(_windowList.Children[0]);
            foreach(var win in winsys.WindowList)
            {
                if(win.Border.WindowStyle == WindowStyle.Default)
                {
                    var btn = new WindowListButton(winsys, win);
                    _windowList.AddChild(btn);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, _wallpaper);
            gfx.DrawRectangle(_notificationTitle.X - 15, _notificationTitle.Y - 15, (Math.Max(_notificationTitle.Width, _notificationDescription.Width) + 30), _notificationTitle.Height + 10 + _notificationDescription.Height + 30, Theme.GetAccentColor() * (_notificationBannerFade/2));
        }
    }

    /// <summary>
    /// A panel that is skinned for use in the desktop UI.
    /// </summary>
    public class DesktopPanel : Control
    {
        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlBG(gfx, 0, 0, Width, Height);
        }
    }

    /// <summary>
    /// Provides extensions for formatting <see cref="DateTime"/> objects into strings. 
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>
        /// Transforms a <see cref="DateTime"/> into a presentable Peacegate time string. 
        /// </summary>
        /// <param name="timeSpan">The <see cref="DateTime"/> to transform</param>
        /// <returns>The resulting presentable string</returns>
        public static string ToPresentableString(this TimeSpan timeSpan)
        {
            string hour = "00";
            string minute = "00";
            string second = "00";
            string m = "AM";

            if(timeSpan.Hours > 12)
            {
                m = "PM";
                hour = (timeSpan.Hours - 12).ToString();
            }
            else
            {
                hour = timeSpan.Hours.ToString();
            }
            if(timeSpan.Minutes < 10)
            {
                minute = $"0{timeSpan.Minutes}";
            }
            else
            {
                minute = timeSpan.Minutes.ToString();
            }

            if (timeSpan.Seconds < 10)
            {
                second = $"0{timeSpan.Seconds}";
            }
            else
            {
                second = timeSpan.Seconds.ToString();
            }


            return $"{hour}:{minute}:{second} {m}";
        }
    }

    /// <summary>
    /// An item group primarly used in <see cref="DesktopPanel"/> elements for displaying the window list. 
    /// </summary>
    public class DesktopPanelItemGroup : Control
    {
        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            int x = 2;
            int y = 0;
            int lineheight = 0;
            foreach(var control in Children)
            {
                if(x + control.Width + 2 > Width)
                {
                    x = 0;
                    y += lineheight + 2;
                    lineheight = 0;
                }
                control.X = x;
                control.Y = y;
                lineheight = Math.Max(lineheight, control.Height);
                x += control.Width+3;
            }
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }
    }

    /// <summary>
    /// A special <see cref="Button"/> capable of sticking in the "Pressed" state while an associated <see cref="Window"/> is active.  
    /// </summary>
    public class WindowListButton : Control
    {
        private WindowInfo _win = null;
        private bool _lastFocused = false;
        private WindowSystem _winmgr = null;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowListButton"/>. 
        /// </summary>
        /// <param name="winmgr">A <see cref="WindowSystem"/> component for modifying the window state.</param>
        /// <param name="win">The window associated with this control.</param>
        public WindowListButton(WindowSystem winmgr, WindowInfo win)
        {
            if (winmgr == null)
                throw new ArgumentNullException();
            if (win == null)
                throw new ArgumentNullException();
            _win = win;
            _winmgr = winmgr;
            Click += (o, a) =>
            {
                var brdr = _win?.Border;
                if(brdr.Visible == false)
                {
                    _winmgr.Show(_win.WindowID);
                    brdr.Manager.SetFocus(brdr);
                }
                else
                {
                    if (!brdr.HasFocused)
                    {
                        brdr.Manager.SetFocus(brdr);
                    }
                    else
                    {
                        _winmgr.Hide(_win.WindowID);
                    }
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            Width = 192;
            Height = 24;

        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var state = Plex.Engine.Themes.UIButtonState.Idle;
            if (ContainsMouse)
                state = Plex.Engine.Themes.UIButtonState.Hover;
            if (_win.Border.HasFocused || LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                state = Plex.Engine.Themes.UIButtonState.Pressed;
            switch(state)
            {
                case Plex.Engine.Themes.UIButtonState.Idle:
                    Theme.DrawControlLightBG(gfx, 0, 0, Width, Height);
                    break;
                case Plex.Engine.Themes.UIButtonState.Hover:
                    gfx.DrawRectangle(0, 0, Width, Height, Theme.GetAccentColor());
                    break;
                case Plex.Engine.Themes.UIButtonState.Pressed:
                    Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
                    break;
            }
            var font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);
            var measure = font.MeasureString(_win.Border.Title);
            gfx.DrawString(_win.Border.Title, 5, (Height - (int)measure.Y) / 2, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), font, TextAlignment.Left);
        }
    }
}
