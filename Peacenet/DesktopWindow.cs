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

        [Dependency]
        private Plexgate _plexgate = null;

        public DesktopWindow(WindowSystem _winsys) : base(_winsys)
        {
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
                _applauncher.X = X;
                _applauncher.Y = _topPanel.Y + _topPanel.Height;
                _applauncher.Visible = !_applauncher.Visible;
            };
            _applauncher.Visible = false;
            AddChild(_applauncher);
            for(int i = 0; i < 3; i++)
            {
                var item = new MenuItem(_winsys);
                item.Text = $"Item {i + 1}";
                _applauncher.AddItem(item);
            }
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

            base.OnUpdate(time);
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
                        _opacityAnim += (float)time.ElapsedGameTime.TotalSeconds * 4;
                        if (_opacityAnim >= 1)
                        {
                            _animState++;
                        }
                    }
                    else
                    {
                        _opacityAnim -= (float)time.ElapsedGameTime.TotalSeconds * 3;
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
}
