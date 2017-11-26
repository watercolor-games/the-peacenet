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

        private List<WindowInfo> _windows = new List<WindowInfo>();

        public void SetWindowStyle(int wid, WindowStyle style)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == wid);
            if (win != null)
                win.Border.WindowStyle = style;
        }

        public int CreateWindowInfo(Window window, WindowStyle style)
        {
            var info = new WindowInfo
            {
                Border = new GUI.WindowBorder(window, style),
                WindowID = _windows.Count
            };
            _windows.Add(info);
            return info.WindowID;
        }

        public void Show(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                _uiman.Add(win.Border);
            }
        }

        public void Hide(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                _uiman.Remove(win.Border);
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

        public Window(WindowSystem _winsys)
        {
            _winsystem = _winsys;
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
        }

        public void Hide()
        {
            _winsystem.Hide((int)_wid);
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
        private Control _child = null;

        private const int _borderWidth = 2;
        private const int _titleHeight = 30;

        private bool _moving = false;
        private Vector2 _lastPos;
        private int diff_x = 0;
        private int diff_y = 0;

        public WindowBorder(Control child, WindowStyle style)
        {
            _child = child;
            _windowStyle = style;

            AddChild(_child);

            //dragging test
            MouseLeftDown += (o, a) =>
            {
                if (MouseX >= 0 && MouseX <= Width && MouseY >= 0 && MouseY <= _titleHeight)
                {
                    _moving = true;
                    _lastPos = new Vector2(MouseX, MouseY);
                }

            };
            MouseMove += (o, pt) =>
            {
                if (_moving)
                {
                    diff_x = (int)(pt.X - _lastPos.X);
                    diff_y = (int)(pt.Y - _lastPos.Y);

                    X += diff_x;
                    Y += diff_y;

                    _lastPos = new Vector2(pt.X - diff_x, pt.Y - diff_y);
                }
            };
            MouseLeftUp += (o, a) => 
            {
                _moving = false;
                diff_x = 0;
                diff_y = 0;

            };
        }

        protected override void OnUpdate(GameTime time)
        {
            switch (_windowStyle)
            {
                case WindowStyle.NoBorder:
                    _child.X = 0;
                    _child.Y = 0;
                    Width = _child.Width;
                    Height = _child.Height;
                    break;
                default:
                    _child.X = _borderWidth;
                    _child.Y = _titleHeight;
                    Width = _child.X + _child.Width + _borderWidth;
                    Height = _child.Y + _child.Height + _borderWidth;
                    break;
            }
            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx, RenderTarget2D currentTarget)
        {
            var accent = Theme.GetAccentColor();
            gfx.Clear(accent);
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
