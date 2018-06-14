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
using Peacenet.Applications;
using Peacenet.RichPresence;

namespace Peacenet
{
    /// <summary>
    /// The Peacegate OS Desktop Environment user interface.
    /// </summary>
    public class DesktopWindow : Window
    {
        #region Notification buttons.

        #endregion

        #region Animation and state

        private float _objectiveTextFade = 0f;
        private bool _objectiveJustStarted = false;
        private double _objectiveStartedCooldown = 0f;
        private string _objectiveText = "";
        private string _objectiveTime = "";

        private bool _openedInstaller = false;

        private int _animState = 0;
        private float _scaleAnim = 0;
        private float _panelAnim = -1;
        private bool _showPanels = true;
        private IEnumerable<int> hiddenWindows = null;
        private bool _appLauncherClosesWhenFocusLost = true;
        private bool _appLauncherButtonVisible = true;

        #endregion

        #region Textures

        private Texture2D _objectiveTask = null;
        private Texture2D _objectiveTimeout = null;
        private Texture2D _xp = null;
        private Texture2D _wallpaper = null;
        private Texture2D _iconEmail = null;
        private Texture2D _iconEmailUnread = null;

        #endregion

        #region UI elements

        private Queue<NotificationBanner> _banners = new Queue<NotificationBanner>();
        private NotificationBanner _currentBanner = null;

        private Hitbox _objectiveHitbox = new Hitbox();
        private HorizontalStacker _notificationTray = new HorizontalStacker();
        private PictureBox _showDesktopIcon = new PictureBox();
        private DesktopPanelItemGroup _windowList = new DesktopPanelItemGroup();
        private AppLauncherMenu _applauncher = null;

        private Hitbox _topPanel = new Hitbox();
        private Hitbox _bottomPanel = new Hitbox();
        private Hitbox _applauncherHitbox = new Hitbox();
        private Hitbox _hbTime = new Hitbox();
        private Hitbox _mailHitbox = new Hitbox();
        private Hitbox _xpHitbox = new Hitbox();

        private XPDisplay _xpDisplay = new XPDisplay();

        #endregion

        #region Engine dependencies

        [Dependency]
        private DiscordRPCModule _discord = null;

        [Dependency]
        private MissionManager _mission = null;

        [Dependency]
        private GameLoop _plexgate = null;

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

        [Dependency]
        private GameManager _game = null;

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
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public DesktopWindow(WindowSystem _winsys) : base(_winsys)
        {
            AddChild(_objectiveHitbox);

            _objectiveTask = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/check-square");
            _objectiveTimeout = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/clock-o");

            _xpDisplay.HideOnFocusLoss = true;
            AddChild(_xpDisplay);
            _xp = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/flash");
            AddChild(_applauncherHitbox);
            AddChild(_hbTime);

            _iconEmail = _plexgate.Content.Load<Texture2D>("UIIcons/NotificationTray/Email");
            _iconEmailUnread = _plexgate.Content.Load<Texture2D>("UIIcons/NotificationTray/EmailUnread");


            _noteSound = _plexgate.Content.Load<SoundEffect>("SFX/DesktopNotification");

            _applauncher = new AppLauncherMenu(_winsys, this);

            SetWindowStyle(WindowStyle.NoBorder);
            ResetWallpaper();
            _os.WallpaperChanged += ResetWallpaper;
            
            _showDesktopIcon.Texture = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/ShowDesktop");
            _showDesktopIcon.AutoSize = true;
            
            ResetWindowList(_winsys);
            _winsys.WindowListUpdated += WindowSystemUpdated;
            
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

            AddChild(this._showDesktopIcon);
            AddChild(_notificationTray);
            
            AddChild(_windowList);
            AddChild(_mailHitbox);

            _mailHitbox.Click += (o, a) =>
            {
                var mail = new EmailViewer(WindowSystem);
                mail.Show();
            };

            AddChild(_xpHitbox);

            _xpHitbox.Click += (o, a) =>
            {
                if (_xpDisplay.Visible)
                {
                    _xpDisplay.Visible = false;
                }
                else
                {
                    _xpDisplay.Visible = true;
                    Manager.SetFocus(_xpDisplay);
                    _xpDisplay.ApplyAnim();
                    _xpDisplay.TotalXP = _game.State.TotalXP;
                }
            };
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
            var banner = new NotificationBanner();
            banner.Header = title;
            banner.Description = description;
            _banners.Enqueue(banner);
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

        private int _lastObjective = -1;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (IsAppLauncherOpen)
                _applauncher.CloseOnFocusLoss = _appLauncherClosesWhenFocusLost;

            if(_openedInstaller==false)
            {
                bool hasDoneTutorial = _game.State.TutorialCompleted;
                if(hasDoneTutorial)
                {
                    _openedInstaller = true;
                }
                else
                {
                    ShowPanels = false;
                    var installer = new PeacegateInstaller(this, WindowSystem);
                    installer.Show();
                    _openedInstaller = true;
                }
            }

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
            
            _showDesktopIcon.X = 2;
            _showDesktopIcon.Y = _bottomPanel.Y + ((_bottomPanel.Height - _showDesktopIcon.Height)/2);
            _showDesktopIcon.Tint = (_showDesktopIcon.ContainsMouse ^ (hiddenWindows != null)) ? Color.White : new Color(191, 191, 191, 255);

            _windowList.Y = 0;
            _windowList.Height = _bottomPanel.Height;
            _windowList.X = _showDesktopIcon.X + _showDesktopIcon.Width + 2;
            _windowList.Width = _bottomPanel.Width - _windowList.X;

            if(_currentBanner==null)
            {
                if(_banners.Count>0)
                {
                    _currentBanner = _banners.Dequeue();
                    _noteSound.Play();
                    AddChild(_currentBanner);
                }
            }
            else
            {
                if(_currentBanner.BannerState == BannerState.Finished)
                {
                    RemoveChild(_currentBanner);
                    _currentBanner = null;
                }
                else
                {
                    _currentBanner.Y = _topPanel.Y + _topPanel.Height;
                    _currentBanner.X = Width - _currentBanner.Width;
                }
            }



            if (_game.State == null)
                return;
            string rtime = _game.State.CurrentCountry + " - " + DateTime.Now.ToShortTimeString();
            var rtmeasure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

            int timePos = _topPanel.Width - ((int)rtmeasure.X + 5);

            _notificationTray.X = (timePos - _notificationTray.Width) - 7;
            _notificationTray.Y = _topPanel.Y + ((_topPanel.Height - _notificationTray.Height) / 2);

            _windowList.X = _showDesktopIcon.X + _showDesktopIcon.Width + 5;
            _windowList.Y = _bottomPanel.Y + ((_bottomPanel.Height - (int)_pn.PanelTheme.PanelButtonSize.Y) / 2);
            _windowList.Width = (_bottomPanel.Width - _windowList.X) - 5;

            var measure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

            _hbTime.Y = _topPanel.Y;
            _hbTime.X = (_topPanel.Width - _hbTime.Width) / 2;
            _hbTime.Width = (int)measure.X + 10;
            _hbTime.Height = _topPanel.Height;

            WindowSystem.Workspace = new Rectangle(Parent.X, Parent.Y + ((_showPanels) ? _topPanel.Height : 0), Width, Height - ((_showPanels) ? _topPanel.Height + _bottomPanel.Height : 0));
            string unread = (_game.State.UnreadEmails > 0) ? _game.State.UnreadEmails.ToString() : "";
            var unreadMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(unread);

            int totalWidth = 14 + 16 + (int)unreadMeasure.X;
            _mailHitbox.Width = totalWidth;
            _mailHitbox.Height = _topPanel.Height;
            _mailHitbox.Visible = _topPanel.Visible;
            _mailHitbox.Y = _topPanel.Y;
            _mailHitbox.X = _topPanel.Width - _mailHitbox.Width;

            string xp = _game.State.SkillLevel.ToString();
            var xpMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(xp);

            int xpWidth = 14 + 16 + (int)xpMeasure.X;
            _xpHitbox.Width = xpWidth;
            _xpHitbox.Height = _topPanel.Height;
            _xpHitbox.Visible = _topPanel.Visible;
            _xpHitbox.Y = _topPanel.Y;
            _xpHitbox.X = _mailHitbox.X - _xpHitbox.Width;

            _xpDisplay.Y = _xpHitbox.Y + _xpHitbox.Height;
            _xpDisplay.X = (_xpHitbox.X + _xpHitbox.Width) - _xpDisplay.Width;

            _xpDisplay.SkillLevel = _game.State.SkillLevel;
            _xpDisplay.SkillLevelPercentage = _game.State.SkillLevelPercentage;

            if (_mission.IsPlayingMission)
            {
                _objectiveHitbox.Visible = true;
                if (_lastObjective != _mission.CurrentMission.ObjectiveIndex)
                {
                    _lastObjective = _mission.CurrentMission.ObjectiveIndex;
                    _objectiveJustStarted = true;
                    _objectiveStartedCooldown = 5;
                    _objectiveTextFade = 0f;
                }
                _discord.GameState = "In Mission: " + _mission.CurrentMission.Name;
                _discord.GameDetails = _mission.CurrentMission.ObjectiveName;

                _objectiveText = _mission.CurrentMission.ObjectiveName;
                if (_mission.CurrentMission.HasTimeout)
                {
                    switch (_mission.CurrentMission.TimeoutType)
                    {
                        case TimeoutType.Complete:
                            _objectiveTime = (Math.Round((_mission.CurrentMission.TimeoutDuration - _mission.CurrentMission.Timeout) / _mission.CurrentMission.TimeoutDuration, 2)*100).ToString() + "%";
                            break;
                        case TimeoutType.Fail:
                            _objectiveTime = Math.Round(_mission.CurrentMission.Timeout).ToString() + "s";
                            break;
                    }
                }
                else
                {
                    _objectiveTime = "";
                }
            }
            else
            {
                _objectiveHitbox.Visible = false;
                _lastObjective = -1;
                _objectiveJustStarted = false;
                _objectiveStartedCooldown = 0;
                _discord.GameState = "Free roam";
                _discord.GameDetails = $"{_os.Hostname} - Skill Level: {_game.State.SkillLevel} - Alert Level: {_game.State.AlertLevel}";
            }

            if(_objectiveStartedCooldown>0)
            {
                _objectiveStartedCooldown -= time.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                _objectiveJustStarted = false;
            }

            if (_objectiveHitbox.ContainsMouse || _objectiveJustStarted)
            {
                _objectiveTextFade = MathHelper.Clamp(_objectiveTextFade + ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
            }
            else
            {
                _objectiveTextFade = MathHelper.Clamp(_objectiveTextFade - ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
            }

            if(_objectiveHitbox.Visible)
            {
                _objectiveHitbox.Y = _topPanel.Y;
                _objectiveHitbox.Height = _topPanel.Height;
                var objMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(_objectiveTime);
                _objectiveHitbox.Width = 14 + 16 + (int)objMeasure.X;
                _objectiveHitbox.X = _xpHitbox.X - _objectiveHitbox.Width;
            }




            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.FillRectangle(0, 0, Width, Height, _wallpaper, Color.White);
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
                    if (_applauncherHitbox.LeftButtonPressed)
                        alState = UIButtonState.Pressed;
                    _pn.PanelTheme.DrawAppLauncher(gfx, _applauncherHitbox.Bounds, alState);
                }

                string rtime = _game.State.CurrentCountry + " - " + DateTime.Now.ToShortTimeString();
                var measure = _pn.PanelTheme.StatusTextFont.MeasureString(rtime);

                gfx.DrawString(rtime, new Vector2((_topPanel.Bounds.Width - measure.X) / 2, _topPanel.Y + (((_topPanel.Height - (measure.Y)) / 2))), (_hbTime.ContainsMouse) ? _pn.PanelTheme.StatusTextHoverColor : 
                    _pn.PanelTheme.StatusTextColor, _pn.PanelTheme.StatusTextFont, TextAlignment.Left, int.MaxValue, WrapMode.None);

                var mailColor = _mailHitbox.ContainsMouse ? Theme.GetAccentColor() : _pn.PanelTheme.StatusTextColor;

                string unread = (_game.State.UnreadEmails > 0) ? _game.State.UnreadEmails.ToString() : "";
                var unreadMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(unread);

                var icon = (unread.Length > 0) ? _iconEmailUnread : _iconEmail;

                int iconY = _mailHitbox.Y + ((_mailHitbox.Height - 16) / 2);
                int iconX = _mailHitbox.X + 5;
                int mailX = iconX + 20;
                int mailY = _mailHitbox.Y + ((_mailHitbox.Height - (int)unreadMeasure.Y) / 2);
                gfx.FillRectangle(new Vector2(iconX, iconY), new Vector2(16, 16), icon, mailColor);
                gfx.DrawString(unread, new Vector2(mailX, mailY), mailColor, _pn.PanelTheme.StatusTextFont, TextAlignment.Left, int.MaxValue, WrapMode.None);

                string xp = _game.State.SkillLevel.ToString();
                var xpMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(xp);

                var xpColor = _xpHitbox.ContainsMouse ? Theme.GetAccentColor() : _pn.PanelTheme.StatusTextColor;

                var xpIcon = _xp;

                int xpIconY = _xpHitbox.Y + ((_xpHitbox.Height - 16) / 2);
                int xpIconX = _xpHitbox.X + 5;
                int xpX = xpIconX + 20;
                int xpY = _xpHitbox.Y + ((_xpHitbox.Height - (int)xpMeasure.Y) / 2);
                gfx.FillRectangle(new Vector2(xpIconX, xpIconY), new Vector2(16, 16), xpIcon, xpColor);
                gfx.DrawString(xp, new Vector2(xpX, xpY), xpColor, _pn.PanelTheme.StatusTextFont, TextAlignment.Left, int.MaxValue, WrapMode.None);

                if(_objectiveHitbox.Visible && _mission.CurrentMission!=null)
                {
                    var oIcon = (_mission.CurrentMission.HasTimeout) ? _objectiveTimeout : _objectiveTask;
                    int oIconX = _objectiveHitbox.X + 5;
                    int oIconY = _objectiveHitbox.Y + ((_objectiveHitbox.Height - 16) / 2);
                    var color = _objectiveHitbox.ContainsMouse ? Theme.GetAccentColor() : _pn.PanelTheme.StatusTextColor;
                    gfx.FillRectangle(oIconX, oIconY, 16, 16, oIcon, color);

                    var otMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(_objectiveTime);
                    int oTextX = oIconX + 20;
                    int oTextY = _objectiveHitbox.Y + ((_objectiveHitbox.Height - (int)otMeasure.Y) / 2);
                    gfx.DrawString(_objectiveTime, new Vector2(oTextX, oTextY), color, _pn.PanelTheme.StatusTextFont);

                    int len = _objectiveText.Length;
                    float take = (float)len * _objectiveTextFade;
                    int takeInt = (int)Math.Round(take);
                    if (takeInt > 0)
                    {
                        string oName = _objectiveText.Substring(0, takeInt);
                        var oNameMeasure = _pn.PanelTheme.StatusTextFont.MeasureString(oName);
                        int oNameX = (_objectiveHitbox.X - 5) - (int)oNameMeasure.X;
                        int oNameY = _topPanel.Y + ((_topPanel.Height - (int)oNameMeasure.Y) / 2);
                        gfx.DrawString(oName, new Vector2(oNameX, oNameY), _pn.PanelTheme.StatusTextColor * _objectiveTextFade, _pn.PanelTheme.StatusTextFont);
                    }
                }
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
            if (LeftButtonPressed)
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
