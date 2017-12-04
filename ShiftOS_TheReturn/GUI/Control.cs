using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.GUI
{
    public class Control : IDisposable
    {
        private int _x = 0;
        private int _y = 0;
        private int _width = 1;
        private int _height = 1;
        private List<Control> _children = null;
        private bool _invalidated = true;
        private RenderTarget2D _rendertarget = null;
        private RenderTarget2D _userfacingtarget = null;
        private bool _resized = false;
        private Control _parent = null;
        private int _mousex = 0;
        private int _mousey = 0;
        private ButtonState _left;
        private ButtonState _middle;
        private ButtonState _right;
        private bool _isVisible = true;
        private bool _disposed = false;
        private float _opacity = 1;

        private int _minWidth = 1;
        private int _minHeight = 1;
        private int _maxWidth = 0;
        private int _maxHeight = 0;

        public bool IsFocused
        {
            get
            {
                return Manager.IsFocused(this);
            }
        }

        public bool HasFocused
        {
            get
            {
                if (IsFocused)
                    return true;
                foreach(var child in _children)
                {
                    if (child.HasFocused)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        public RenderTarget2D BackBuffer
        {
            get
            {
                return _rendertarget;
            }
        }

        public event EventHandler Click;
        public event EventHandler RightClick;
        public event EventHandler MiddleClick;
        public event Action<object, Vector2> MouseMove;
        public event EventHandler MouseLeftDown;
        public event EventHandler MouseRightDown;
        public event EventHandler MouseMiddleDown;
        public event EventHandler MouseLeftUp;
        public event EventHandler MouseRightUp;
        public event EventHandler MouseMiddleUp;
        public event EventHandler<KeyboardEventArgs> KeyEvent;

        public int MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                value = Math.Max(1, value);
                if (value == _minWidth)
                    return;
                _minWidth = value;
                Width = Width;
            }
        }

        public int MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                value = Math.Max(1, value);
                if (value == _minHeight)
                    return;
                _minHeight = value;
                Height = Height;
            }
        }

        public int MaxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                value = Math.Max(_minWidth, value);
                if (value == _maxWidth)
                    return;
                _maxWidth = value;
                Width = Width;
            }
        }

        public int MaxHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                value = Math.Max(1, value);
                if (value == _minHeight)
                    return;
                _minHeight = value;
                Height = Height;
            }
        }


        public float Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                value = MathHelper.Clamp(value, 0, 1);
                if (value == _opacity)
                    return;
                _opacity = value;
            }
        }

        public bool Disposed
        {
            get
            {
                return _disposed;
            }
        }

        public bool Visible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
            }
        }

        public bool ContainsMouse
        {
            get
            {
                return (_mousex >= 0 && _mousex <= Width && _mousey >= 0 && _mousey <= Height);
            }
        }

        private UIManager _manager = null;

        public UIManager Manager
        {
            get
            {
                if (Parent != null)
                    return Parent.Manager;
                return _manager;
            }
        }

        internal void SetManager(UIManager manager)
        {
            _manager = manager;
        }

        internal void ProcessKeyboardEvent(KeyboardEventArgs e)
        {
            OnKeyEvent(e);
            KeyEvent?.Invoke(this, e);
        }

        protected virtual void OnKeyEvent(KeyboardEventArgs e)
        {

        }

        private Themes.Theme _theme = null;

        public Theme Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme; //this will walk up the ui tree to the toplevel and grab the theme.
                return _theme;
            }
        }

        internal void SetTheme(Theme theme)
        {
            _theme = theme;
        }

        public void AddChild(Control child)
        {
            if (_children.Contains(child))
                return;
            _children.Add(child);
            child._parent = this;
        }

        public int MouseX
        {
            get
            {
                return _mousex;
            }
        }

        public int MouseY
        {
            get
            {
                return _mousey;
            }
        }

        public Control Parent
        {
            get
            {
                return _parent;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (_width == value)
                    return;
                _width = value;
                _resized = true;
                _invalidated = true;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (_height == value)
                    return;
                _height = value;
                _resized = true;
                _invalidated = true;
            }
        }

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }


        public Control()
        {
            _children = new List<Control>();
        }

        protected virtual void OnUpdate(GameTime time)
        {

        }

        private ButtonState _lastLeft;
        private ButtonState _lastRight;
        private ButtonState _lastMiddle;

        public void RemoveChild(Control child)
        {
            if (!_children.Contains(child))
                return;
            _children.Remove(child);
        }

        internal bool PropagateMouseState(ButtonState left, ButtonState middle, ButtonState right)
        {
            foreach(var child in _children)
            {
                if (child.PropagateMouseState(left, middle, right))
                    return true;
            }
            if (_isVisible == false)
                return false;
            bool isInCtrl = (_mousex >= 0 && _mousex <= Width && _mousey >= 0 && _mousey <= Height);
            if (_lastLeft == left && _lastRight == right && _lastMiddle == middle)
                return isInCtrl;
            _lastLeft = left;
            _lastRight = right;
            _lastMiddle = middle;

            if (isInCtrl)
            {
                bool fireLeft = (_left == ButtonState.Pressed && left == ButtonState.Released);
                bool fireRight = (_right == ButtonState.Pressed && right == ButtonState.Released);
                bool fireMiddle = (_middle == ButtonState.Pressed && middle == ButtonState.Released);
                if (_left != left || _right != right || _middle != middle)
                    _invalidated = true;
                if (_left != left && left == ButtonState.Pressed)
                    MouseLeftDown?.Invoke(this, EventArgs.Empty);
                if (_right != right && right == ButtonState.Pressed)
                    MouseRightDown?.Invoke(this, EventArgs.Empty);
                if (_middle != middle && middle == ButtonState.Pressed)
                    MouseMiddleDown?.Invoke(this, EventArgs.Empty);
                if (_left != left && left == ButtonState.Released)
                    MouseLeftUp?.Invoke(this, EventArgs.Empty);
                if (_right != right && right == ButtonState.Released)
                    MouseRightUp?.Invoke(this, EventArgs.Empty);
                if (_middle != middle && middle == ButtonState.Released)
                    MouseMiddleUp?.Invoke(this, EventArgs.Empty);

                _left = left;
                _middle = middle;
                _right = right;
                if (fireLeft)
                {
                    Click?.Invoke(this, EventArgs.Empty);
                    if (!IsFocused)
                    {
                        Manager.SetFocus(this);
                        Invalidate();
                    }
                }
                if (fireRight)
                {
                    RightClick?.Invoke(this, EventArgs.Empty);

                }
                if (fireMiddle)
                {
                    MiddleClick?.Invoke(this, EventArgs.Empty);
                    
                }
            }
            else
            {
                _left = ButtonState.Released;
                _middle = _left;
                _right = _left;
            }
            return isInCtrl;
        }

        public void Update(GameTime time)
        {
            if (_rendertarget == null)
            {
                _invalidated = true;
                _resized = true;
            }
            if (_userfacingtarget == null)
            {
                _invalidated = true;
                _resized = true;
            }

            if (_isVisible == false)
                return;
            //Pull the mouse state.
            var mouse = Mouse.GetState();
            //For toplevels, set mouse input loc directly.
            int _newmousex = 0;
            int _newmousey = 0;
            if (Parent == null)
            {
                _newmousex = mouse.X - X;
                _newmousey = mouse.Y - Y;
            }
            //For controls with parents, poll mouse information from the parent.
            else
            {
                _newmousex = Parent._mousex - X;
                _newmousey = Parent._mousey - Y;
            }
            if(_newmousex != _mousex || _newmousey != _mousey)
            {
                bool hasMouse = ContainsMouse;
                _mousex = _newmousex;
                _mousey = _newmousey;
                MouseMove?.Invoke(this, new Vector2(_newmousex, _newmousey));
                if(hasMouse != ContainsMouse)
                    _invalidated = true;
            }
            OnUpdate(time);
            foreach (var child in _children)
            {
                child.Update(time);
            }
        }



        protected virtual void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlBG(gfx, 0, 0, Width, Height);
        }

        public void Invalidate()
        {
            _invalidated = true;
        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            bool makeBack = _invalidated; //normally I'd let this be false but I thought I'd try making the backbuffer reset if the control's invalidated. This seemed to help, but right after restarting the game and doing the same thing, the bug was back. So this only works intermitently.
            if (_rendertarget == null)
                makeBack = true;
            else
            {
                if (_rendertarget.Width != Width || _rendertarget.Height != Height)
                {
                    _rendertarget.Dispose();
                    _rendertarget = null;
                    makeBack = true;
                }
            }
            if (makeBack)
            {
                _rendertarget = new RenderTarget2D(gfx.Device, Width, Height, false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            }

            if (_invalidated)
            {
                if (_resized)
                {
                    _userfacingtarget?.Dispose();
                    _userfacingtarget = new RenderTarget2D(gfx.Device, Width, Height, false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                    _resized = false;
                }
                gfx.Device.SetRenderTarget(_userfacingtarget);
                gfx.Device.Clear(Color.Transparent);
                gfx.BeginDraw();
                OnPaint(time, gfx);
                gfx.EndDraw();
                _invalidated = false;
            }

            gfx.Device.SetRenderTarget(_rendertarget);
            gfx.Device.Clear(Color.Transparent);
            gfx.BeginDraw();
            gfx.DrawRectangle(0, 0, Width, Height, _userfacingtarget);
            gfx.EndDraw();
            foreach (var child in _children)
            {
                if (!child.Visible)
                    continue;
                child.Draw(time, gfx);
                gfx.Device.SetRenderTarget(_rendertarget);
                gfx.BeginDraw();
                if (Manager.IgnoreControlOpacity)
                {
                    gfx.DrawRectangle(child.X, child.Y, child.Width, child.Height, child.BackBuffer);
                }
                else
                {
                    gfx.DrawRectangle(child.X, child.Y, child.Width, child.Height, child.BackBuffer, Color.White * child.Opacity);
                }
                gfx.EndDraw();
            }
        }

        public ButtonState LeftMouseState
        {
            get
            {
                return _lastLeft;
            }
        }

        public ButtonState RightMouseState
        {
            get
            {
                return _lastRight;
            }
        }
        public ButtonState MiddleMouseState
        {
            get
            {
                return _lastMiddle;
            }
        }

        public void Dispose()
        {
            if (_rendertarget != null)
            {
                _rendertarget.Dispose();
                _rendertarget = null;
            }
            if (_userfacingtarget != null)
            {
                _userfacingtarget.Dispose();
                _userfacingtarget = null;
            }
            while (_children.Count > 0)
            {
                _children[0].Dispose();
                _children.RemoveAt(0);
            }
            _children = null;
            _disposed = true;
        }
    }

    public class TestChild : Control
    {
        
        protected override void OnUpdate(GameTime time)
        {
            var measure = TextRenderer.MeasureText(((int)time.TotalGameTime.TotalSeconds).ToString(), new System.Drawing.Font("Lucida Console", 12F), Parent.Width, TextAlignment.Middle, TextRenderers.WrapMode.Words);
            Width = (int)measure.X;
            Height = (int)measure.Y;
            X = (Parent.Width - Width) / 2;
            Y = (Parent.Height - Height) / 2;
            Invalidate();
            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Gray);
            gfx.DrawString(((int)time.TotalGameTime.TotalSeconds).ToString(), 0, 0, Color.White, new System.Drawing.Font("Lucida Console", 12F), TextAlignment.Middle, Width, TextRenderers.WrapMode.Words);
        }
    }
}
