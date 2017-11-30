using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.GUI
{
    public class WindowSystem : IEngineComponent
    {
        [Dependency]
        private UIManager _uiman = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private List<WindowInfo> _windows = new List<WindowInfo>();

        public int Width
        {
            get
            {
                return _uiman.ScreenWidth;
            }
        }

        public int Height
        {
            get
            {
                return _uiman.ScreenHeight;
            }
        }

        public void SetWindowTitle(int winid, string title)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
                win.Border.Title = title;

        }

        public void SetWindowStyle(int wid, WindowStyle style)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == wid);
            if (win != null)
                win.Border.WindowStyle = style;
        }

        internal void InjectDependencies(Window window)
        {
            _plexgate.Inject(window);
        }

        private int _totalWindowsOpened = 0;

        public int CreateWindowInfo(Window window, WindowStyle style)
        {
            var info = new WindowInfo
            {
                Border = new GUI.WindowBorder(this, window, style),
                WindowID = _totalWindowsOpened + 1
            };
            _totalWindowsOpened++;
            _windows.Add(info);
            _uiman.Add(info.Border);
            return info.WindowID;
        }

        public void Show(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                win.Border.Visible = true;
            }
        }

        public void Hide(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                win.Border.Visible = false;
            }
        }

        public void Close(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                _uiman.Remove(win.Border);
                _windows.Remove(win);
            }

        }


        public void Initiate()
        {
            Logger.Log("Starting window system.", LogType.Info, "pgwinsys");
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            Logger.Log("Stopping window system.", LogType.Info, "pgwinsys");
        }
    }

    internal class WindowInfo
    {
        public WindowBorder Border { get; set; }
        public int WindowID { get; set; }
    }

    public abstract class Window : Control
    {
        private WindowSystem _winsystem = null;
        private int? _wid = null;
        private WindowStyle _preferredStyle = WindowStyle.Default;
        private string _title = "Peacenet Window";

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_wid == null)
                {
                    if (_title == value)
                        return;
                    _title = value;

                }
                else
                {
                    _winsystem.SetWindowTitle((int)_wid, _title);
                }
            }
        }

        public Window(WindowSystem _winsys)
        {
            _winsystem = _winsys;
            _winsystem.InjectDependencies(this);
        }

        public void SetWindowStyle(WindowStyle style)
        {
            if(_wid == null)
            {
                _preferredStyle = style;
            }
            else
            {
                _winsystem.SetWindowStyle((int)_wid, style);
            }
        }

        public void Show()
        {
            if (_wid == null)
                _wid = _winsystem.CreateWindowInfo(this, _preferredStyle);
            _winsystem.Show((int)_wid);
            _winsystem.SetWindowTitle((int)_wid, _title);
        }

        public void Hide()
        {
            _winsystem.Hide((int)_wid);
        }

        public int? WindowID
        {
            get
            {
                return _wid;
            }
        }
    }

    public enum WindowStyle
    {
        Default,
        Dialog,
        NoBorder
    }

    public class WindowBorder : Control
    {
        private WindowStyle _windowStyle = WindowStyle.Default;
        private Window _child = null;

        private const int _borderWidth = 2;
        private const int _titleHeight = 30;

        private Hitbox _titleHitbox = null;
        private Hitbox _closeHitbox = null;
        private Hitbox _minimizeHitbox = null;
        private Hitbox _maximizeHitbox = null;
        private Hitbox _leftHitbox = null;
        private Hitbox _rightHitbox = null;
        private Hitbox _bottomHitbox = null;
        private Hitbox _bRightHitbox = null;
        private Hitbox _bLeftHitbox = null;
        
        private string _title = "";

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                Invalidate();
            }
        }

        private int _dragAnimState = 1;
        private bool _moving = false;
        private Vector2 _lastPos;
        private int diff_x = 0;
        private int diff_y = 0;

        private WindowSystem _windowManager = null;

        public WindowBorder(WindowSystem winsys, Window child, WindowStyle style)
        {
            _windowManager = winsys;
            _child = child;
            _windowStyle = style;

            AddChild(_child);

            _closeHitbox = new Hitbox();
            _minimizeHitbox = new Hitbox();
            _maximizeHitbox = new Hitbox();
            _titleHitbox = new Hitbox();
            _leftHitbox = new Hitbox();
            _rightHitbox = new Hitbox();
            _bottomHitbox = new Hitbox();
            _bRightHitbox = new Hitbox();
            _bLeftHitbox = new Hitbox();

            _titleHitbox.AddChild(_closeHitbox);
            _titleHitbox.AddChild(_minimizeHitbox);
            _titleHitbox.AddChild(_maximizeHitbox);
            AddChild(_titleHitbox);
            AddChild(_leftHitbox);
            AddChild(_rightHitbox);
            AddChild(_bottomHitbox);
            AddChild(_bRightHitbox);
            AddChild(_bLeftHitbox);

            _closeHitbox.Click += (o, a) =>
            {
                _windowManager.Close((int)_child.WindowID);  
            };

            _titleHitbox.MouseLeftDown += (o, a) =>
            {
                _moving = true;
                _dragAnimState = 2;
                _lastPos = new Vector2(MouseX, MouseY);
            };
            _titleHitbox.MouseMove += (o, pt) =>
            {
                if (_moving == true)
                {
                    var real = new Vector2(MouseX, MouseY);
                    diff_x = (int)(real.X - _lastPos.X);
                    diff_y = (int)(real.Y - _lastPos.Y);

                    X += diff_x;
                    Y += diff_y;
                    _lastPos = new Vector2(real.X - diff_x, real.Y - diff_y);
                }
            };
            _titleHitbox.MouseLeftUp += (o, a) =>
            {
                _dragAnimState = 0;
                _moving = false;
                diff_x = 0;
                diff_y = 0;
            };

            switch (_windowStyle)
            {
                case WindowStyle.NoBorder:
                    _child.X = 0;
                    _child.Y = 0;
                    Width = _child.Width;
                    Height = _child.Height;
                    break;
                case WindowStyle.Default:
                    _child.X = _borderWidth;
                    _child.Y = _titleHeight;
                    Width = _child.X + _child.Width + _borderWidth;
                    Height = _child.Y + _child.Height + _borderWidth;
                    break;
                case WindowStyle.Dialog:
                    _child.X = 0;
                    _child.Y = _titleHeight;
                    Width = _child.Width;
                    Height = _child.Y + _child.Height;
                    break;
            }
            X = (winsys.Width - Width) / 2;
            Y = (winsys.Height - Height) / 2;

        }

        private bool _closeHasMouse = false;

        protected override void OnUpdate(GameTime time)
        {
            switch (_dragAnimState)
            {
                case 0:
                    float opacity = Opacity;
                    opacity = MathHelper.Clamp(opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    Opacity = opacity;
                    if (opacity >= 1)
                    {
                        _dragAnimState++;
                    }
                    break;
                case 2:
                    float opacity2 = Opacity;
                    opacity2 = MathHelper.Clamp(opacity2 - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    Opacity = opacity2;
                    if (opacity2 <= 0.75)
                    {
                        _dragAnimState++;
                    }

                    break;
            }

            switch (_windowStyle)
            {
                case WindowStyle.NoBorder:
                    _child.X = 0;
                    _child.Y = 0;
                    Width = _child.Width;
                    Height = _child.Height;
                    _closeHitbox.Visible = false;
                    _minimizeHitbox.Visible = false;
                    _maximizeHitbox.Visible = false;
                    _titleHitbox.Visible = false;
                    _leftHitbox.Visible = false;
                    _rightHitbox.Visible = false;
                    _bottomHitbox.Visible = false;
                    _bRightHitbox.Visible = false;
                    _bLeftHitbox.Visible = false;

                    break;
                case WindowStyle.Default:
                    _child.X = _borderWidth;
                    _child.Y = _titleHeight;
                    Width = _child.X + _child.Width + _borderWidth;
                    Height = _child.Y + _child.Height + _borderWidth;
                    _closeHitbox.Visible = true;
                    _minimizeHitbox.Visible = true;
                    _maximizeHitbox.Visible = true;
                    _titleHitbox.Visible = true;
                    _leftHitbox.Visible = true;
                    _rightHitbox.Visible = true;
                    _bottomHitbox.Visible = true;
                    _bRightHitbox.Visible = true;
                    _bLeftHitbox.Visible = true;
                    break;
                case WindowStyle.Dialog:
                    _child.X = 0;
                    _child.Y = _titleHeight;
                    Width = _child.Width;
                    Height = _child.Y + _child.Height;
                    _closeHitbox.Visible = true;
                    _minimizeHitbox.Visible = false;
                    _maximizeHitbox.Visible = false;
                    _titleHitbox.Visible = true;
                    _leftHitbox.Visible = false;
                    _rightHitbox.Visible = false;
                    _bottomHitbox.Visible = false;
                    _bRightHitbox.Visible = false;
                    _bLeftHitbox.Visible = false;
                    break;
            }

            _titleHitbox.X = 0;
            _titleHitbox.Y = 0;
            _titleHitbox.Width = Width;
            _titleHitbox.Height = _titleHeight;

            _bLeftHitbox.X = 0;
            _bLeftHitbox.Y = (Height - _borderWidth);
            _bLeftHitbox.Width = _borderWidth;
            _bLeftHitbox.Height = _borderWidth;

            _leftHitbox.X = 0;
            _leftHitbox.Y = _titleHeight;
            _leftHitbox.Width = _borderWidth;
            _leftHitbox.Height = _bLeftHitbox.Y - _titleHeight;

            _bRightHitbox.X = (Width - _borderWidth);
            _bRightHitbox.Y = _bLeftHitbox.Y;
            _bRightHitbox.Width = _borderWidth;
            _bRightHitbox.Height = _borderWidth;

            _bottomHitbox.X = _borderWidth;
            _bottomHitbox.Y = _bRightHitbox.Y;
            _bottomHitbox.Width = Width - (_borderWidth * 2);
            _bottomHitbox.Height = _borderWidth;

            _rightHitbox.X = _bRightHitbox.X;
            _rightHitbox.Y = _titleHeight;
            _rightHitbox.Width = _borderWidth;
            _rightHitbox.Height = _bRightHitbox.Y - _titleHeight;
            //That was all RSI-inducing. Wow.

            var csize = Theme.GetTitleButtonRectangle(Themes.TitleButton.Close, Width, _titleHeight);
            var mxsize = Theme.GetTitleButtonRectangle(Themes.TitleButton.Maximize, Width, _titleHeight);
            var mnsize = Theme.GetTitleButtonRectangle(Themes.TitleButton.Minimize, Width, _titleHeight);

            _closeHitbox.Width = csize.Width;
            _closeHitbox.Height = csize.Height;
            _closeHitbox.X = csize.X;
            _closeHitbox.Y = csize.Y;

            _minimizeHitbox.Width = mnsize.Width;
            _minimizeHitbox.Height = mnsize.Height;
            _minimizeHitbox.X = mnsize.X;
            _minimizeHitbox.Y = mnsize.Y;

            _maximizeHitbox.Width = mxsize.Width;
            _maximizeHitbox.Height = mxsize.Height;
            _maximizeHitbox.X = mxsize.X;
            _maximizeHitbox.Y = mxsize.Y;

            if(_closeHasMouse != _closeHitbox.ContainsMouse)
            {
                _closeHasMouse = _closeHitbox.ContainsMouse;
                Invalidate();
                _titleHitbox.Invalidate();
            }

            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx, RenderTarget2D currentTarget)
        {
            Theme.DrawWindowBorder(gfx, _title, _leftHitbox, _rightHitbox, _bottomHitbox, _bLeftHitbox, _bRightHitbox, _titleHitbox, _closeHitbox, _minimizeHitbox, _maximizeHitbox, true);
        }

        public WindowStyle WindowStyle
        {
            get
            {
                return _windowStyle;
            }
            set
            {
                if (_windowStyle == value)
                    return;
                _windowStyle = value;
                Invalidate();
            }
        }
    }
}
