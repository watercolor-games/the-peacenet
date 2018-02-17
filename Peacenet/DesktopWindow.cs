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
using Plex.Engine.Filesystem;
using Peacenet.CoreUtils;
using Peacenet.Server;
using Peacenet.DesktopUI;

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

            _appLauncherText.Click += (o, a) =>
            {
                if(_winsys.WindowList.FirstOrDefault(x => x.Border == _applauncher.Parent) != null)
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

            AddChild(_notificationTitle);
            AddChild(_notificationDescription);

            _notificationTitle.AutoSize = true;
            _notificationTitle.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _notificationDescription.AutoSize = true;
            _notificationDescription.MaxWidth = 450;
            _notificationTitle.MaxWidth = _notificationDescription.MaxWidth;
        }

        /// <summary>
        /// Show a notification banner on the desktop.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="description">The message for the notification.</param>
        public void ShowNotification(string title, string description)
        {
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
                _infobox.Show("Not yet implemented", "Files cannot yet be opened from the Desktop.");
            }
        }

        private bool _needsDesktopReset = true;
        private double _desktopResetTimer = 0.0;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private FileUtils _futils = null;

        /// <summary>
        /// Repopulates the desktop icon list view.
        /// </summary>
        public void SetupIcons()
        {
            _desktopIconsView.Clear();
            if (!_fs.DirectoryExists("/home/Desktop"))
                _fs.CreateDirectory("/home/Desktop");
            foreach(var dir in _fs.GetDirectories("/home/Desktop"))
            {
                if (_futils.GetNameFromPath(dir).StartsWith("."))
                    continue;
                var diritem = new ListViewItem(_desktopIconsView);
                diritem.Tag = dir;
                diritem.Value = _futils.GetNameFromPath(dir);
                diritem.ImageKey = "folder";
            }
            foreach (var dir in _fs.GetFiles("/home/Desktop"))
            {
                if (_futils.GetNameFromPath(dir).StartsWith("."))
                    continue;
                var diritem = new ListViewItem(_desktopIconsView);
                diritem.Tag = dir;
                diritem.Value = _futils.GetNameFromPath(dir);
                diritem.ImageKey = _futils.GetMimeType(dir);
                if(_desktopIconsView.GetImage(diritem.ImageKey) == null)
                {
                    _desktopIconsView.SetImage(diritem.ImageKey, _futils.GetMimeIcon(diritem.ImageKey));
                }
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
                    Close();
                    _os.Shutdown();
                    break;
            }

            switch(_notificationAnimState)
            {
                case 0:
                    _notificationBannerFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
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
            _showDesktopIcon.Tint = (_showDesktopIcon.ContainsMouse) ? Color.White : new Color(191, 191, 191, 255);

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
                    _desktopResetTimer = 0;
                    Task.Run(() =>
                    {
                        SetupIcons();
                    });
                    _needsDesktopReset = false;
                }
                else
                {
                    _desktopResetTimer += time.ElapsedGameTime.TotalSeconds;
                    if (_desktopResetTimer >= 10)
                    {
                        _needsDesktopReset = true;
                    }
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
        private int _animState = -1;
        private bool? _lastFocus = null;
        private float _opacityAnim = 0;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            bool focused = ContainsMouse;
            if(focused != _lastFocus)
            {
                _animState = 0;
                _lastFocus = focused;
            }
            switch (_animState)
            {
                case 0:
                    if (_lastFocus == true)
                    {
                        _opacityAnim += (float)time.ElapsedGameTime.TotalSeconds * 8;
                        if (_opacityAnim >= 1)
                        {
                            _animState++;
                        }
                    }
                    else
                    {
                        _opacityAnim -= (float)time.ElapsedGameTime.TotalSeconds * 8;
                        if (_opacityAnim <= 0)
                        {
                            _animState++;
                        }

                    }
                    break;
            }
            Opacity = MathHelper.Lerp(0.75F, 1, _opacityAnim);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Theme.GetAccentColor().Darken(0.35F));
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
                x += control.Width;
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
    public class WindowListButton : Button
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
            Text = _win.Border?.Title;
            ShowImage = false;
            if(_lastFocused != _win.Border?.HasFocused)
            {
                _lastFocused = (bool)_win.Border?.HasFocused;
                Invalidate();
            }
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var state = Plex.Engine.Themes.UIButtonState.Idle;
            if (_lastFocused)
                state = Plex.Engine.Themes.UIButtonState.Pressed;
            if (ContainsMouse)
                state = Plex.Engine.Themes.UIButtonState.Hover;
            Theme.DrawButton(gfx, Text, Image, state, ShowImage, ImageRect, TextRect);
        }
    }

    /// <summary>
    /// An overlay element that is used for showing the player around the user interface.
    /// </summary>
    public class TutorialOverlay : Control
    {
        private Label _header = new Label();
        private Label _description = new Label();
        private Button _okay = new Button();

        private Rectangle _unshroudedRegion;

        /// <summary>
        /// Creates a new instance of the <see cref="TutorialOverlay"/> control. 
        /// </summary>
        public TutorialOverlay()
        {
            AddChild(_header);
            _header.AutoSize = true;
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            AddChild(_description);
            _description.AutoSize = true;
            _description.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            AddChild(_okay);
            _okay.Click += (o, a) =>
            {
                OkayButtonClicked?.Invoke(this, a);
            };
        }

        /// <summary>
        /// Occurs when the player clicks the "OK" button on the overlay.
        /// </summary>
        public event EventHandler OkayButtonClicked;

        /// <summary>
        /// Gets or sets the title of the overlay.
        /// </summary>
        public string HeaderText
        {
            get
            {
                return _header.Text;
            }
            set
            {
                _header.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the description of the overlay.
        /// </summary>
        public string DescriptionText
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

        /// <summary>
        /// Gets or sets a region on the overlay where the control is completely transparent, thus showing other controls rendering behind the overlay.
        /// </summary>
        public Rectangle Region
        {
            get
            {
                return _unshroudedRegion;
            }
            set
            {
                if (_unshroudedRegion == value)
                    return;
                _unshroudedRegion = value;
                Invalidate(true);
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _header.MaxWidth = Width / 3;
            _description.MaxWidth = Width / 2;
            _header.Alignment = TextAlignment.Top;
            _description.Alignment = TextAlignment.Center;

            int combinedHeight = _header.Height + 10 + _description.Height + 5 + _okay.Height;
            _header.X = (Width - _header.Width) / 2;
            _header.Y = (Height - combinedHeight) / 2;
            _description.X = (Width - _description.Width) / 2;
            _description.Y = _header.Y + _header.Height + 10;
            _okay.X = (Width - _okay.Width) / 2;
            _okay.Y = _description.Y + _description.Height + 5;

            _okay.Text = "OK";
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var color = Color.Black * 0.5F;
            gfx.Clear(Color.Transparent);
            gfx.DrawRectangle(0, 0, _unshroudedRegion.Left, Height, color);
            gfx.DrawRectangle(_unshroudedRegion.Left, 0, Width - _unshroudedRegion.Left, _unshroudedRegion.Top, color);
            gfx.DrawRectangle(_unshroudedRegion.Left, _unshroudedRegion.Top + _unshroudedRegion.Height, Width - _unshroudedRegion.Left, Height - (_unshroudedRegion.Top + _unshroudedRegion.Height), color);
            gfx.DrawRectangle(_unshroudedRegion.Left + _unshroudedRegion.Width, _unshroudedRegion.Top, Width - (_unshroudedRegion.Left + _unshroudedRegion.Width), _unshroudedRegion.Height, color);

        }
    }
}
