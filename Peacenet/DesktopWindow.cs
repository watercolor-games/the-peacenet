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
using Peacenet.DesktopUI;
using Peacenet.Filesystem;
using Peacenet.PeacegateThemes.PanelThemes;
using Plex.Engine.Themes;
using Plex.Engine.TextRenderers;

namespace Peacenet
{
    /// <summary>
    /// The Peacegate OS Desktop Environment user interface.
    /// </summary>
    public class DesktopWindow : Window
    {
        #region Notification buttons.

        private Button _emailButton = new Button();

        #endregion

        #region Animation and state

        private int _animState = 0;
        private float _scaleAnim = 0;
        private float _panelAnim = -1;
        private bool _showPanels = true;
        private float _notificationBannerFade = 0f;
        private double _notificationRide = 0;
        private int _notificationAnimState = 0;
        private IEnumerable<int> hiddenWindows = null;
        private bool _appLauncherClosesWhenFocusLost = true;
        private bool _needsDesktopReset = true;
        private bool _appLauncherButtonVisible = true;

        #endregion

        #region Textures

        private Texture2D _wallpaper = null;
        private Texture2D _iconEmail = null;
        private Texture2D _iconEmailUnread = null;

        #endregion

        #region UI elements

        private HorizontalStacker _notificationTray = new HorizontalStacker();
        private ContextMenu _desktopRightClick = null;
        private Label _notificationTitle = new Label();
        private Label _notificationDescription = new Label();
        private PictureBox _showDesktopIcon = new PictureBox();
        private DesktopPanelItemGroup _windowList = new DesktopPanelItemGroup();
        private ListView _desktopIconsView = null;
        private AppLauncherMenu _applauncher = null;

        private Hitbox _topPanel = new Hitbox();
        private Hitbox _bottomPanel = new Hitbox();
        private Hitbox _applauncherHitbox = new Hitbox();
        private Hitbox _hbTime = new Hitbox();

        #endregion

        #region Engine dependencies

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FileUtilities _utils = null;

        #endregion

        #region Audio

        private SoundEffect _noteSound = null;

        #endregion

        #region Properties

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
        /// Gets the App Launcher Menu for this desktop.
        /// </summary>
        public AppLauncherMenu AppLauncher
        {
            get
            {
                return _applauncher;
            }
        }

        /// <summary>
        /// Gets or sets whether the App Launcher button is visible.
        /// </summary>
        public bool ShowAppLauncherButton
        {
            get
            {
                return _appLauncherButtonVisible;
            }
            set
            {
                _appLauncherButtonVisible = value;
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

        #endregion

        #region Private functions

        private void ResetWindowList(WindowSystem winsys)
        {
            while (_windowList.Children.Length > 0)
                _windowList.RemoveChild(_windowList.Children[0]);
            foreach (var win in winsys.WindowList)
            {
                if (win.Border.WindowStyle == WindowStyle.Default)
                {
                    var btn = new WindowListButton(WindowSystem, win, _pn);
                    _windowList.AddChild(btn);
                }
            }
        }


        /// <summary>
        /// Repopulates the desktop icon list view.
        /// </summary>
        private void SetupIcons()
        {
            _desktopIconsView.ClearItems();
            if (!_fs.DirectoryExists("/home/Desktop"))
                _fs.CreateDirectory("/home/Desktop");
            foreach (var dir in _fs.GetDirectories("/home/Desktop"))
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
                if (_desktopIconsView.GetImage(diritem.ImageKey) == null)
                {
                    _desktopIconsView.SetImage(diritem.ImageKey, _futils.GetMimeIcon(diritem.ImageKey));
                }
                _desktopIconsView.AddItem(diritem);
            }
        }

        private void WindowSystemUpdated(object o, EventArgs a)
        {
            ResetWindowList((WindowSystem)o);
        }

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
            if (_desktopIconsView.SelectedItem != null)
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
                        if (answer)
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


        #endregion

        #region Constructors

        /// <inheritdoc/>
        public DesktopWindow(WindowSystem _winsys) : base(_winsys)
        {
            AddChild(_applauncherHitbox);
            AddChild(_hbTime);

            _iconEmail = _plexgate.Content.Load<Texture2D>("UIIcons/NotificationTray/Email");
            _iconEmailUnread = _plexgate.Content.Load<Texture2D>("UIIcons/NotificationTray/EmailUnread");


            _notificationTray.AddChild(_emailButton);

            _desktopRightClick = new ContextMenu(_winsys);

            _noteSound = _plexgate.Content.Load<SoundEffect>("SFX/DesktopNotification");

            _applauncher = new AppLauncherMenu(_winsys, this);

            SetWindowStyle(WindowStyle.NoBorder);
            ResetWallpaper();
            _os.WallpaperChanged += ResetWallpaper;
            
            _showDesktopIcon.Texture = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/ShowDesktop");
            _showDesktopIcon.AutoSize = true;
            
            ResetWindowList(_winsys);
            _winsys.WindowListUpdated += WindowSystemUpdated;
            _desktopIconsView = new ListView();
            AddChild(_desktopIconsView);
            _desktopIconsView.IconFlow = IconFlowDirection.TopDown;

            _desktopIconsView.ItemClicked += _desktopIconsView_ItemClicked;
            _desktopIconsView.SetImage("folder", _plexgate.Content.Load<Texture2D>("UIIcons/folder"));
            
            RightClick += Desktop_RightClick;
            _desktopIconsView.RightClick += Desktop_RightClick;

            _emailButton.Click += (o, a) =>
            {
            };


            _hbTime.Click += (o, a) =>
            {
                var clock = new Applications.Clock(WindowSystem);
                clock.Show();
            };

            _applauncherHitbox.Click += (o, a) =>
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
                    Manager.SetFocus(_applauncher);
                }
            };

            EventHandler wscallback = null;
            wscallback = (o, a) => { hiddenWindows = null; WindowSystem.WindowStateChanged -= wscallback; };
            _showDesktopIcon.Click += (o, a) =>
            {
                if (hiddenWindows == null) // show desktop
                {
                    hiddenWindows = WindowSystem.WindowList.Where(w => w.Border.Visible && !(w.Border.Window is DesktopWindow)).Select(w => w.WindowID).ToList();
                    foreach (var id in hiddenWindows)
                        WindowSystem.Hide(id);
                    WindowSystem.WindowStateChanged += wscallback;
                }
                else // restore windows
                {
                    WindowSystem.WindowStateChanged -= wscallback;
                    foreach (var id in hiddenWindows)
                        WindowSystem.Show(id);
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

            AddChild(this._showDesktopIcon);
            AddChild(_notificationTray);
            _notificationTray.AddChild(_emailButton);

            AddChild(_windowList);
        }

        #endregion

        #region Public functions

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

        /// <summary>
        /// Plays the Desktop Close animation and shuts down the session, returning to main menu.
        /// </summary>
        public void Shutdown()
        {
            _animState = 3;
        }

        #endregion
        
        #region Update and Draw methods

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _emailButton.Text = "";
            _emailButton.ShowImage = true;
            _emailButton.Image = _iconEmail;

            

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
                    WindowSystem.WindowListUpdated -= WindowSystemUpdated;
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

            _topPanel.X = 0;
            _topPanel.Width = Width;
            _topPanel.Height = _pn.PanelTheme.DesktopPanelHeight;
            _topPanel.Y = (int)MathHelper.Lerp(0 - _topPanel.Height, 0, _panelAnim);

            _bottomPanel.X = 0;
            _bottomPanel.Width = Width;
            _bottomPanel.Height = _topPanel.Height;
            _bottomPanel.Y = (int)MathHelper.Lerp(Height, Height - _bottomPanel.Height, _panelAnim);

            _applauncherHitbox.Visible = (_showPanels && _appLauncherButtonVisible);

            _applauncherHitbox.X = _topPanel.X + _pn.PanelTheme.AppLauncherRectangle.X;
            _applauncherHitbox.Y = _topPanel.Y + _pn.PanelTheme.AppLauncherRectangle.Y;
            _applauncherHitbox.Width = _pn.PanelTheme.AppLauncherRectangle.Width;
            _applauncherHitbox.Height = _pn.PanelTheme.AppLauncherRectangle.Height;
            
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

            _showDesktopIcon.X = 2;
            _showDesktopIcon.Y = _bottomPanel.Y + ((_bottomPanel.Height - _showDesktopIcon.Height)/2);
            _showDesktopIcon.Tint = (_showDesktopIcon.ContainsMouse ^ (hiddenWindows != null)) ? Color.White : new Color(191, 191, 191, 255);

            _windowList.Y = 0;
            _windowList.Height = _bottomPanel.Height;
            _windowList.X = _showDesktopIcon.X + _showDesktopIcon.Width + 2;
            _windowList.Width = _bottomPanel.Width - _windowList.X;

            _desktopIconsView.X = 0;
            _desktopIconsView.Y = _topPanel.Y + _topPanel.Height;
            _desktopIconsView.Height = _bottomPanel.Y - _desktopIconsView.Y;

//            if (_server.Connected)
//            {
                if (_needsDesktopReset)
                {
                    SetupIcons();
                    _needsDesktopReset = false;
                }
//            }
//            else
//            {
//                if (_animState < 3)
//                {
//                    foreach (var win in WindowSystem.WindowList.ToArray())
//                    {
//                        if (win.Border != this.Parent)
//                            WindowSystem.Close(win.WindowID);
//                    }
//                    _animState = 3;

//                }
//            }

            string rtime = DateTime.Now.ToShortTimeString();
            var rtmeasure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

            int timePos = _topPanel.Width - ((int)rtmeasure.X + 5);

            _notificationTray.X = (timePos - _notificationTray.Width) - 7;
            _notificationTray.Y = _topPanel.Y + ((_topPanel.Height - _notificationTray.Height) / 2);

            _windowList.X = _showDesktopIcon.X + _showDesktopIcon.Width + 5;
            _windowList.Y = _bottomPanel.Y + ((_bottomPanel.Height - (int)_pn.PanelTheme.PanelButtonSize.Y) / 2);
            _windowList.Width = (_bottomPanel.Width - _windowList.X) - 5;

            var measure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

            _hbTime.Y = _topPanel.Y;
            _hbTime.X = _topPanel.Width - ((int)measure.X + 10);
            _hbTime.Width = (int)measure.X + 10;
            _hbTime.Height = _topPanel.Height;

            WindowSystem.Workspace = new Rectangle(Parent.X, Parent.Y + ((_showPanels) ? _topPanel.Height : 0), Width, Height - ((_showPanels) ? _topPanel.Height + _bottomPanel.Height : 0));

            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, _wallpaper);
            gfx.DrawRectangle(_notificationTitle.X - 15, _notificationTitle.Y - 15, (Math.Max(_notificationTitle.Width, _notificationDescription.Width) + 30), _notificationTitle.Height + 10 + _notificationDescription.Height + 30, Theme.GetAccentColor() * (_notificationBannerFade/2));
            if(_showPanels)
            {
                if(_topPanel.Visible)
                {
                    _pn.PanelTheme.DrawPanel(gfx, _topPanel.Bounds);
                }
                if (_bottomPanel.Visible)
                {
                    _pn.PanelTheme.DrawPanel(gfx, _bottomPanel.Bounds);
                }
                if(_applauncherHitbox.Visible)
                {
                    var alState = UIButtonState.Idle;
                    if (_applauncherHitbox.ContainsMouse)
                        alState = UIButtonState.Hover;
                    if (_applauncherHitbox.LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        alState = UIButtonState.Pressed;
                    _pn.PanelTheme.DrawAppLauncher(gfx, _applauncherHitbox.Bounds, alState);
                }

                string rtime = DateTime.Now.ToShortTimeString();
                var measure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

                gfx.DrawString(rtime, new Vector2(_topPanel.Bounds.Width - ((measure.X + 5)), _topPanel.Y + (((_topPanel.Height - (measure.Y)) / 2))), (_hbTime.ContainsMouse) ? _pn.PanelTheme.StatusTextHoverColor : 
                    _pn.PanelTheme.StatusTextColor, _pn.PanelTheme.StatusTextFont, TextAlignment.Left, int.MaxValue, WrapMode.None);
            }
        }

        #endregion
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

    public class HorizontalStacker : Control
    {
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
                return;
            Height = Parent.Height;
            int maxWidth = MaxWidth == 0 ? int.MaxValue : MaxWidth;
            int width = 2;
            foreach(var ctrl in Children)
            {
                ctrl.X = width;
                ctrl.Y = (Height - ctrl.Height) / 2;
                width += ctrl.Width + 5;
            }

            Width = width - 3;
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

        private PeacenetThemeManager _pn = null;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowListButton"/>. 
        /// </summary>
        /// <param name="winmgr">A <see cref="WindowSystem"/> component for modifying the window state.</param>
        /// <param name="win">The window associated with this control.</param>
        public WindowListButton(WindowSystem winmgr, WindowInfo win, PeacenetThemeManager themeManager)
        {
            //dependency injection won't work here. FUCK.
            _pn = themeManager;

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
            Width = (int)_pn.PanelTheme.PanelButtonSize.X;
            Height = (int)_pn.PanelTheme.PanelButtonSize.Y;
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var state = Plex.Engine.Themes.UIButtonState.Idle;
            if (ContainsMouse)
                state = Plex.Engine.Themes.UIButtonState.Hover;
            if (LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                state = Plex.Engine.Themes.UIButtonState.Pressed;

            var winState = PanelButtonState.Default;
            if (_win.Border.HasFocused)
                winState = PanelButtonState.Active;
            if (_win.Border.Visible == false)
                winState = PanelButtonState.Minimized;
            _pn.PanelTheme.DrawPanelButton(gfx, new Rectangle(0, 0, Width, Height), winState, state, _win.Border.Title);
        }
    }
}
