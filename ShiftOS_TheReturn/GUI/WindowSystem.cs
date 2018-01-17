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
using Plex.Engine.Config;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// Provides basic window management.
    /// </summary>
    /// <remarks>
    ///     <para>The methods in this class allow you to manage windows even if you don't have access to a <see cref="Window"/> object. Only use these if you know what you're doing. The <see cref="Window"/> class has methods which wrap this component and make the experience more-or-less easy on you.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <seealso cref="Window"/>
    /// <see cref="WindowInfo"/>
    /// <seealso cref="UIManager"/>
    /// <seealso cref="IEngineComponent"/> 
    public class WindowSystem : IEngineComponent, IConfigurable
    {

        [Dependency]
        private UIManager _uiman = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private Config.ConfigManager _config = null;

        /// <summary>
        /// Occurs when the window list is updated.
        /// </summary>
        public event EventHandler WindowListUpdated;

        private List<WindowInfo> _windows = new List<WindowInfo>();

        private bool _allowFadingWindowsWhileDragging = true;

        /// <summary>
        /// Retrieves whether windows should fade while dragging.
        /// </summary>
        public bool FadeWindowsWhileDragging { get { return _allowFadingWindowsWhileDragging; } }

        /// <summary>
        /// See <see cref="UIManager.ScreenWidth"/>. 
        /// </summary>
        public int Width
        {
            get
            {
                return _uiman.ScreenWidth;
            }
        }

        /// <summary>
        /// See <see cref="UIManager.ScreenHeight"/>. 
        /// </summary>
        public int Height
        {
            get
            {
                return _uiman.ScreenHeight;
            }
        }
        
        /// <summary>
        /// Set the title of a window.
        /// </summary>
        /// <param name="winid">The ID of the window to set</param>
        /// <param name="title">The new title of the window</param>
        public void SetWindowTitle(int winid, string title)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
                win.Border.Title = title;

        }

        /// <summary>
        /// Set the style of a window
        /// </summary>
        /// <param name="wid">The ID of the window to set</param>
        /// <param name="style">The new style for the window</param>
        public void SetWindowStyle(int wid, WindowStyle style)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == wid);
            if (win != null)
            {
                win.Border.WindowStyle = style;
                WindowListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void InjectDependencies(Window window)
        {
            _plexgate.Inject(window);
        }

        private int _totalWindowsOpened = 0;

        /// <summary>
        /// Create a new window border.
        /// </summary>
        /// <param name="window">The window for the border to host</param>
        /// <param name="style">The style of the window border</param>
        /// <returns>The ID of the new window border</returns>
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
            WindowListUpdated?.Invoke(this, EventArgs.Empty);
            return info.WindowID;
        }

        /// <summary>
        /// Show a window.
        /// </summary>
        /// <param name="winid">The ID of the window to show</param>
        public void Show(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                win.Border.Visible = true;
            }
        }

        /// <summary>
        /// Hide a window
        /// </summary>
        /// <param name="winid">The ID of the window to hide</param>
        public void Hide(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                win.Border.Visible = false;
            }
        }

        /// <summary>
        /// Close a window
        /// </summary>
        /// <param name="winid">The ID of the window to close</param>
        public void Close(int winid)
        {
            var win = _windows.FirstOrDefault(x => x.WindowID == winid);
            if (win != null)
            {
                _uiman.Remove(win.Border);
                _windows.Remove(win);
                WindowListUpdated?.Invoke(this, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Retrieves an array containing information about all open windows.
        /// </summary>
        public WindowInfo[] WindowList
        {
            get
            {
                return _windows.ToArray();
            }
        }

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("Starting window system.", LogType.Info, "pgwinsys");
        }

        /// <inheritdoc/>
        public void ApplyConfig()
        {
            _allowFadingWindowsWhileDragging = (bool)_config.GetValue("windowManagerTranslucentWindowsWhileDragging", true);

        }
    }

    /// <summary>
    /// A class containing information about an open window.
    /// </summary>
    public class WindowInfo
    {
        /// <summary>
        /// Gets or sets the window's border.
        /// </summary>
        public WindowBorder Border { get; set; }
        /// <summary>
        /// Gets or sets the ID of the window.
        /// </summary>
        public int WindowID { get; set; }
    }

    /// <summary>
    /// A control which is tied to a <see cref="WindowSystem"/> and acts as a program window's client area. 
    /// </summary>
    public abstract class Window : Control
    {
        private WindowSystem _winsystem = null;
        private int? _wid = null;
        private WindowStyle _preferredStyle = WindowStyle.Default;
        private string _title = "Peacenet Window";

        /// <summary>
        /// Gets or sets the title text of the window.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of the window.
        /// </summary>
        /// <param name="_winsys">The <see cref="WindowSystem"/> instance to associate the window with.</param>
        public Window(WindowSystem _winsys)
        {
            _winsystem = _winsys;
            _winsystem.InjectDependencies(this);
        }
        
        /// <summary>
        /// Retrieves the window system associated with the window.
        /// </summary>
        public WindowSystem WindowSystem { get { return _winsystem; } }

        /// <summary>
        /// Set the style of the window's border.
        /// </summary>
        /// <param name="style">The new style for the border.</param>
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

        /// <summary>
        /// Present the window to the player.
        /// </summary>
        /// <param name="x">The starting X coordinate of the window border</param>
        /// <param name="y">The starting Y coordinate of the window border</param>
        /// <remarks>
        ///     <para>By default, the <paramref name="x"/> and <paramref name="y"/> values are both set to -1. This causes the window manager to determine the location of the new window on its own, causing the window to be placed in the center of the screen.</para>
        /// </remarks>
        public virtual void Show(int x = -1, int y = -1)
        {
            if (_wid == null)
                _wid = _winsystem.CreateWindowInfo(this, _preferredStyle);
            if(x > -1)
            {
                Parent.X = x;
            }
            if (y > -1)
            {
                Parent.Y = y;
            }
            _winsystem.Show((int)_wid);
            _winsystem.SetWindowTitle((int)_wid, _title);
            Visible = true;
        }

        /// <summary>
        /// Hide the window from the screen.
        /// </summary>
        public virtual void Hide()
        {
            _winsystem.Hide((int)_wid);
            Visible = false;
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        public virtual void Close()
        {
            if (_wid == null)
                return;
            _winsystem.Close((int)_wid);
            _wid = null;
        }

        /// <summary>
        /// Retrieves the ID of this window.
        /// </summary>
        public int? WindowID
        {
            get
            {
                return _wid;
            }
        }
    }

    /// <summary>
    /// Represents the style of a <see cref="WindowBorder"/>. 
    /// </summary>
    public enum WindowStyle
    {
        /// <summary>
        /// The default style. The borders and title bar are shown, and the window can be closed, minimized, maximized and dragged around.
        /// </summary>
        Default,
        /// <summary>
        /// The window has a titlebar, and no borders. It can still be dragged around and closed, but can't be minimized or maximized.
        /// </summary>
        Dialog,
        /// <summary>
        /// The window has no border or title bar whatsoever.
        /// </summary>
        NoBorder,
        /// <summary>
        /// Same as <see cref="Dialog"/> but dragging is disabled. 
        /// </summary>
        DialogNoDrag
    }

    /// <summary>
    /// A control which acts as the non-client area of a <see cref="Window"/> and as its window border. 
    /// </summary>
    public class WindowBorder : Control
    {
        private WindowStyle _windowStyle = WindowStyle.Default;
        private Window _child = null;

        private int _borderWidth { get { return Theme.WindowBorderWidth; } }
        private int _titleHeight { get { return Theme.WindowTitleHeight; } }

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

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
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
                Invalidate(true);
                _needsLayout = true;
            }
        }

        private int _dragAnimState = 1;
        private bool _moving = false;
        private Vector2 _lastPos;
        private int diff_x = 0;
        private int diff_y = 0;

        private WindowSystem _windowManager = null;

        private bool _needsLayout = true;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowBorder"/> control. 
        /// </summary>
        /// <param name="winsys">The window system to associate with the window border</param>
        /// <param name="child">The window to associate with the window border</param>
        /// <param name="style">The style of the window border</param>
        public WindowBorder(WindowSystem winsys, Window child, WindowStyle style)
        {
            HasFocusedChanged += (o, a) =>
            {
                if(_windowStyle != WindowStyle.NoBorder && HasFocused == true)
                {
                    Manager.Remove(this, false);
                    Manager.Add(this);
                    _needsLayout = true;
                }
            };

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
                _child.Close();
            };

            _titleHitbox.MouseLeftDown += (o, a) =>
            {
                if (_windowStyle == WindowStyle.DialogNoDrag)
                    return;
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

            X = (winsys.Width - _child.Width) / 2;
            Y = (winsys.Height - _child.Height) / 2;
            _child.WidthChanged += (o, a) => 
            {
                _needsLayout = true;
            };
            _child.HeightChanged += (o, a) =>
            {
                _needsLayout = true;
            };
        }

        private bool _closeHasMouse = false;

        private bool _lastFocused = true;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_closeHasMouse != _closeHitbox.ContainsMouse)
            {
                _closeHasMouse = _closeHitbox.ContainsMouse;
                Invalidate(true);
                _titleHitbox.Invalidate();
            }
            if (_lastFocused != HasFocused)
            {
                _lastFocused = HasFocused;
                Invalidate(true);
            }
            if (_windowManager.FadeWindowsWhileDragging)
            {
                switch (_dragAnimState)
                {
                    case 0:
                        float opacity = this._child.Opacity;
                        opacity = MathHelper.Clamp(opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0.75f, 1);
                        this._child.Opacity = opacity;
                        if (opacity >= 1)
                        {
                            _dragAnimState++;
                        }
                        break;
                    case 2:
                        float opacity2 = this._child.Opacity;
                        opacity2 = MathHelper.Clamp(opacity2 - (float)time.ElapsedGameTime.TotalSeconds * 2, 0.75F, 1);
                        this._child.Opacity = opacity2;
                        if (opacity2 <= 0.75)
                        {
                            _dragAnimState++;
                        }

                        break;
                }
            }
            else
            {
                this._child.Opacity = 1;
            }
            if (_needsLayout == false)
                return;
            _needsLayout = false;

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
                case WindowStyle.DialogNoDrag:
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


            

            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawWindowBorder(gfx, _title, _leftHitbox, _rightHitbox, _bottomHitbox, _bLeftHitbox, _bRightHitbox, _titleHitbox, _closeHitbox, _minimizeHitbox, _maximizeHitbox, HasFocused);
        }

        /// <summary>
        /// Gets or sets the style of the window border.
        /// </summary>
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
                Invalidate(true);
                _needsLayout = true;
            }
        }
    }
}
