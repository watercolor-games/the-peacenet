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
using Plex.Engine.Server;

namespace Peacenet
{
    public class DesktopWindow : Window
    {
        private int _animState = 0;
        private float _scaleAnim = 0;
        private Texture2D _wallpaper = null;
        private float _panelAnim = 0;

        private DesktopPanel _topPanel = null;
        private DesktopPanel _bottomPanel = null;

        private Label _timeLabel = new Label();

        private PictureBox _showDesktopIcon = new PictureBox();

        private MenuItem _applauncher = null;

        private Label _appLauncherText = new Label();

        private DesktopPanelItemGroup _windowList = new DesktopPanelItemGroup();

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private SplashScreenComponent _splash = null;

        [Dependency]
        private AppLauncherManager _applaunchermgr = null;

        private SoundEffect _tutorialBgm = null;
        private SoundEffectInstance _tutorialBgmInstance = null;

        private int _tutorialBgmAnim = -1;

        private WindowSystem winsys = null; //why isn't the current winmgr a property of all Window objects

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private AsyncServerManager _server = null;

        public DesktopWindow(WindowSystem _winsys) : base(_winsys)
        {
            _tutorialBgm = _plexgate.Content.Load<SoundEffect>("Audio/Tutorial/TutorialBGM");
            _tutorialBgmInstance = _tutorialBgm.CreateInstance();

            if (!_server.IsMultiplayer)
            {
                if(_save.GetValue("oobe.tutorial", false) == false)
                {
                    _tutorialBgmAnim = 0;
                }
            }

            winsys = _winsys;
            SetWindowStyle(WindowStyle.NoBorder);
            _wallpaper = _plexgate.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage");
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

            _applauncher = new MenuItem(_winsys);

            _topPanel.AddChild(_appLauncherText);
            _appLauncherText.AutoSize = true;
            _appLauncherText.FontStyle = Plex.Engine.Themes.TextFontStyle.Custom;
            _appLauncherText.Click += (o, a) =>
            {
                if (_applauncher.IsOpen == false)
                {
                    _applauncher.Show(0, _topPanel.Y + _topPanel.Height);
                }
                else
                {
                    _applauncher.Hide();
                }
            };
            _applauncher.Visible = false;
            
            _bottomPanel.AddChild(_windowList);

            ResetAppLauncher(_winsys);
            ResetWindowList(_winsys);
            _winsys.WindowListUpdated += WindowSystemUpdated;
        }

        public void WindowSystemUpdated (object o, EventArgs a)
        {
            ResetWindowList((WindowSystem)o);
        }

        public void ResetAppLauncher(WindowSystem winsys)
        {
            _applauncher.ClearItems();
            bool catAdded = false;
            foreach (var cat in _applaunchermgr.GetAllCategories())
            {
                catAdded = true;
                var catitem = new MenuItem(winsys);
                catitem.Text = cat;
                bool itemAdded = false;
                foreach (var item in _applaunchermgr.GetAllInCategory(cat))
                {
                    itemAdded = true;
                    var subitem = new MenuItem(winsys);
                    subitem.Text = item.Attribute.Name;
                    subitem.Activated += (o, a) =>
                    {
                        var win = (Window)Activator.CreateInstance(item.WindowType, new object[] { winsys });
                        win.Show();
                    };
                    catitem.AddItem(subitem);
                }
                if(itemAdded == false)
                {
                    var noitems = new MenuItem(winsys);
                    noitems.Text = "No items found";
                    catitem.AddItem(noitems);
                }
                _applauncher.AddItem(catitem);
            }
            if(catAdded == false)
            {
                var noitems = new MenuItem(winsys);
                noitems.Text = "No items found";
                _applauncher.AddItem(noitems);

            }
            var shutdown = new MenuItem(winsys);
            shutdown.Activated += (o, a) =>
            {
                foreach(var win in winsys.WindowList.ToArray())
                {
                    win.Border.Enabled = false;
                }
                _infobox.ShowYesNo("End Session", "Are you sure you'd like to end your Peacegate session?", (answer) =>
                {
                    foreach (var win in winsys.WindowList.ToArray())
                    {
                        win.Border.Enabled = true;
                    }
                    if (answer)
                    {
                        foreach(var win in winsys.WindowList.ToArray())
                        {
                            if (win.Border != this.Parent)
                                winsys.Close(win.WindowID);
                        }
                        _animState = 3;
                    }
                });
            };
            shutdown.Text = "Shut down";
            _applauncher.AddItem(shutdown);
        }


        protected override void OnUpdate(GameTime time)
        {
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
                    if (_tutorialBgmInstance.State == SoundState.Playing)
                        _tutorialBgmInstance.Volume = MathHelper.Clamp(_scaleAnim, 0, 1);
                    if (_scaleAnim <= 0)
                    {
                        _animState++;
                    }

                    break;
                case 5:
                    winsys.WindowListUpdated -= WindowSystemUpdated;
                    Close();
                    _splash.Reset();
                    _tutorialBgmInstance.Stop();
                    _tutorialBgmInstance.Dispose();
                    break;
            }

            switch (_tutorialBgmAnim)
            {
                case 0:
                    _tutorialBgmInstance.Play();
                    _tutorialBgmInstance.IsLooped = true;
                    _tutorialBgmAnim++;
                    break;
                case 1:

                    break;
                case 2:
                    float vol = _tutorialBgmInstance.Volume;
                    vol = MathHelper.Clamp(vol - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                    if(vol == 0)
                    {
                        _tutorialBgmInstance.Stop();
                        _tutorialBgmAnim = -1;
                    }

                    break;
            }
            Width = (int)MathHelper.Lerp((Manager.ScreenWidth * 0.75f), Manager.ScreenWidth, _scaleAnim);
            Height = (int)MathHelper.Lerp((Manager.ScreenHeight * 0.75f), Manager.ScreenHeight, _scaleAnim);
            Parent.X = (Manager.ScreenWidth - Width) / 2;
            Parent.Y = (Manager.ScreenHeight - Height) / 2;
            Parent.Opacity = _scaleAnim;

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

            base.OnUpdate(time);
        }

        public void ResetWindowList(WindowSystem winsys)
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

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, _wallpaper);
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
            Opacity = MathHelper.Lerp(0.5F, 1, _opacityAnim);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
        }
    }

    public static class TimeExtensions
    {
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

    public class DesktopPanelItemGroup : Control
    {
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }

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
    }

    public class WindowListButton : Button
    {
        private WindowInfo _win = null;
        private bool _lastFocused = false;
        private WindowSystem _winmgr = null;

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
}
