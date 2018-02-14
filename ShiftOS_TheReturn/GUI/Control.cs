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
    /// <summary>
    /// A class representing a Peacenet GUI element.
    /// </summary>
    public class Control : IDisposable
    {
        private int _x = 0;
        private int _y = 0;
        private int _width = 1;
        private int _height = 1;
        private List<Control> _children = null;
        private bool _invalidated = true;
        internal RenderTarget2D _rendertarget = null;
        internal RenderTarget2D _userfacingtarget = null;
        private bool _resized = false;
        private Control _parent = null;
        private int _mousex = -1;
        private int _mousey = -1;
        private ButtonState _left;
        private ButtonState _middle;
        private ButtonState _right;
        private bool _isVisible = true;
        private bool _disposed = false;
        private float _opacity = 1;

        private bool _enabled = true;

        /// <summary>
        /// Gets or sets whether this control is enabled. If not, the control will not receive mouse or keyboard events, and will be visually grayed out.
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                Invalidate();
            }
        }

        private bool _doDoubleClick = false;
        private double _doubleClickCooldown = 0;

        private int _minWidth = 1;
        private int _minHeight = 1;
        private int _maxWidth = 0;
        private int _maxHeight = 0;

        /// <summary>
        /// Retrieves whether this control is focused.
        /// </summary>
        public bool IsFocused
        {
            get
            {
                return Manager.IsFocused(this);
            }
        }

        /// <summary>
        /// Retrieves whether this control is focused or contains a child/descendent UI element which is in focus.
        /// </summary>
        public bool HasFocused
        {
            get
            {
                if (IsFocused)
                    return true;
                foreach(var child in Children)
                {
                    if (child.HasFocused)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        /// <summary>
        /// Retrieves the back buffer for the control.
        /// </summary>
        public RenderTarget2D BackBuffer
        {
            get
            {
                return _rendertarget;
            }
        }

        /// <summary>
        /// Occurs when the player clicks on the control.
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Occurs when the player right-clicks the control.
        /// </summary>
        public event EventHandler RightClick;
        /// <summary>
        /// Occurs when the player middle-clicks the control.
        /// </summary>
        public event EventHandler MiddleClick;
        /// <summary>
        /// Occurs when the player's mouse moves. Coordinates are relative to the control's <see cref="X"/> and <see cref="Y"/> values.  
        /// </summary>
        public event Action<object, Vector2> MouseMove;
        /// <summary>
        /// Occurs when the left mouse button is pressed down.
        /// </summary>
        public event EventHandler MouseLeftDown;
        /// <summary>
        /// Occurs when the right mouse button is pressed down.
        /// </summary>
        public event EventHandler MouseRightDown;
        /// <summary>
        /// Occurs when the middle mouse button is pressed down.
        /// </summary>
        public event EventHandler MouseMiddleDown;
        /// <summary>
        /// Occurs when the left mouse button is released.
        /// </summary>
        public event EventHandler MouseLeftUp;
        /// <summary>
        /// Occurs when the right mouse button is released.
        /// </summary>
        public event EventHandler MouseRightUp;
        /// <summary>
        /// Occurs when the middle mouse button is released.
        /// </summary>
        public event EventHandler MouseMiddleUp;
        /// <summary>
        /// Occurs when a keyboard event is fired by the engine to this control.
        /// </summary>
        public event EventHandler<KeyboardEventArgs> KeyEvent;
        /// <summary>
        /// Occurs when the player double-clicks the control.
        /// </summary>
        public event EventHandler DoubleClick;


        /// <summary>
        /// Occurs when the control's width is changed.
        /// </summary>
        public event EventHandler WidthChanged;
        /// <summary>
        /// Occurs when the control's height is changed.
        /// </summary>
        public event EventHandler HeightChanged;
        /// <summary>
        /// Occurs when the control's X coordinate is changed.
        /// </summary>
        public event EventHandler XChanged;
        /// <summary>
        /// Occurs when the control's Y coordinate is changed.
        /// </summary>
        public event EventHandler YChanged;
        /// <summary>
        /// Occurs when the control's "Visible" property is changed.
        /// </summary>
        public event EventHandler VisibleChanged;

        /// <summary>
        /// Gets or sets the minimum width of the control.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minimum height of the control.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum width of the control.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum height of the control.
        /// </summary>
        public int MaxHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                value = Math.Max(1, value);
                if (value == _maxHeight)
                    return;
                _maxHeight = value;
                Height = Height;
            }
        }

        /// <summary>
        /// Clears all child elements from the control.
        /// </summary>
        public void Clear()
        {
            while(_children.Count > 0)
            {
                RemoveChild(_children[0]);
            }
        }

        /// <summary>
        /// Gets or sets the opacity of the control.
        /// </summary>
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
                Invalidate();
            }
        }

        /// <summary>
        /// Gets whether the control has been disposed.
        /// </summary>
        public bool Disposed
        {
            get
            {
                return _disposed;
            }
        }

        /// <summary>
        /// Gets or sets whether the control should be rendered on-screen.
        /// </summary>
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
                VisibleChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets whether the mouse is within the control's render bounds.
        /// </summary>
        public bool ContainsMouse
        {
            get
            {
                if (_enabled == false)
                    return false;
                return (_mousex >= 0 && _mousex <= Width && _mousey >= 0 && _mousey <= Height);
            }
        }

        private UIManager _manager = null;

        /// <summary>
        /// Retrieves the control's associated <see cref="UIManager"/> instance.
        /// </summary>
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

        /// <summary>
        /// When overriden in a derived class, this method handles any keyboard events from the engine.
        /// </summary>
        /// <param name="e">A <see cref="KeyboardEventArgs"/> object containing information about the event.</param>
        protected virtual void OnKeyEvent(KeyboardEventArgs e)
        {

        }

        private Themes.Theme _theme = null;

        /// <summary>
        /// Retrieves the control's associated <see cref="Themes.Theme"/>. 
        /// </summary>
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

        /// <summary>
        /// Adds a child to this control.
        /// </summary>
        /// <param name="child">The child control to add.</param>
        public virtual void AddChild(Control child)
        {
            if (_children == null)
                return;
            if (_children.Contains(child))
                return;
            _children.Add(child);
            child._parent = this;
            Invalidate();
        }

        /// <summary>
        /// Retrieves the mouse X coordinate relative to the control's X coordinate.
        /// </summary>
        public int MouseX
        {
            get
            {
                return _mousex;
            }
        }

        /// <summary>
        /// Retrieves the mouse's Y coordinate relative to the control's Y coordinate.
        /// </summary>
        public int MouseY
        {
            get
            {
                return _mousey;
            }
        }

        /// <summary>
        /// Retrieves the parent of this control. Returns null if the control is a top-level.
        /// </summary>
        public Control Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
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
                Invalidate(true);
                WidthChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
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
                Invalidate(true);
                HeightChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the control.
        /// </summary>
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x == value)
                    return;
                _x = value;
                XChanged?.Invoke(this, EventArgs.Empty);
                if (_parent != null)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the control.
        /// </summary>
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y == value)
                    return;
                _y = value;
                YChanged?.Invoke(this, EventArgs.Empty);
                if(_parent!=null)
                    Invalidate();
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Control"/> class. 
        /// </summary>
        public Control()
        {
            _children = new List<Control>();
        }

        /// <summary>
        /// Updates the control's layout.
        /// </summary>
        /// <param name="time">The time since the last frame update.</param>
        protected virtual void OnUpdate(GameTime time)
        {

        }

        private ButtonState _lastLeft;
        private ButtonState _lastRight;
        private ButtonState _lastMiddle;

        /// <summary>
        /// Removes a child from this control.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        public virtual void RemoveChild(Control child)
        {
            if (!_children.Contains(child))
                return;
            _children.Remove(child);
            Invalidate();
        }

        private int _lastScrollValue = 0;

        /// <summary>
        /// Processes a mouse scroll event.
        /// </summary>
        /// <param name="delta">The delta scroll value.</param>
        protected virtual void OnMouseScroll(int delta)
        {

        }

        /// <summary>
        /// Occurs when the scrollwheel is moved on this control.
        /// </summary>
        public event Action<int> MouseScroll;

        internal void ResetMouseState()
        {
            _mousex = -1;
            _mousey = -1;
            _left = ButtonState.Released;
            _right = ButtonState.Released;
            _middle = ButtonState.Released;
        }

        /// <summary>
        /// Recursively invalidate ALL child UI elements.
        /// </summary>
        public void InvalidateAll()
        {
            Invalidate(true);
            foreach (var child in Children)
                child.InvalidateAll();
        }

        /// <summary>
        /// Handle a mouse state update.
        /// </summary>
        /// <param name="state">The <see cref="MouseState"/> object containing mouse information.</param>
        /// <returns>Whether the mouse was inside the control and button events were fired.</returns>
        public virtual bool PropagateMouseState(MouseState state)
        {
            //Let's see if this beats Matt Trobbiani's mouse handler...

            //Skip mouse events if the control is disabled.
            if(_enabled == false)
            {
                return false;
            }
            //Skip them if it's not visible.
            if(_isVisible==false)
            {
                return false;
            }

            //Grab the X and Y coordinates of the mouse, relative to this control.
            int x, y = 0;
            if(_parent == null)
            {
                x = state.X - X;
                y = state.Y - Y;
            }
            else
            {
                x = _parent._mousex - X;
                y = _parent._mousey - Y;
            }

            //Has the mouse moved?
            bool moved = (x != _mousex) || (y != _mousey);
            //Was the mouse previously in the control?
            bool previouslyInControl = (_mousex >= 0 && _mousex <= Width) && (_mousey >= 0 && _mousey <= Height);

            //Update the mouse coordinates.
            _mousex = x;
            _mousey = y;

            //Get the state of the three mouse buttons we care about.
            var leftState = state.LeftButton;
            var rightState = state.RightButton;
            var middleState = state.MiddleButton;

            //If the mouse is inside the control...
            if (x >= 0 && x <= Width && y >= 0 && y <= Height)
            {
                bool ret = false;
                //Propagate the mouse events to all children - returning true if a child does.
                foreach(var child in Children.OrderByDescending(z=>Array.IndexOf(Children, z)))
                {
                    if(ret == false)
                    {
                        bool res = child.PropagateMouseState(state);
                        if (res)

                            ret = true;
                    }
                    else
                    {
                        child.ResetMouseState();
                    }
                }
                if (ret)
                    return true;
                //If we moved, fire the MouseMove event.
                if(moved)
                {
                    MouseMove?.Invoke(this, new Vector2(x, y));
                }
                //If the mouse has entered the control...
                if(previouslyInControl == false)
                {
                    Invalidate(true);
                }

                //Let's handle left clicking!
                if(_left == ButtonState.Pressed && leftState == ButtonState.Released)
                {
                    if (!Manager.IsFocused(this))
                    {
                        Manager.SetFocus(this);
                    }
                    if (_doubleClickCooldown <= 0)
                    {
                        //We've clicked.
                        Click?.Invoke(this, EventArgs.Empty);
                        //Give 250 milliseconds of double-clickiness.
                        _doubleClickCooldown = 250;
                    }
                    else
                    {
                        DoubleClick?.Invoke(this, EventArgs.Empty);
                        _doubleClickCooldown = 0;
                    }
                    //And the mouse has also been released.
                    MouseLeftUp?.Invoke(this, EventArgs.Empty);
                    //Also, we gain focus.
                    
                    Invalidate(true);
                }
                //Now for left mouse-down...
                else if(_left == ButtonState.Released && leftState == ButtonState.Pressed)
                {
                    //The mouse button has been pressed down.
                    MouseLeftDown?.Invoke(this, EventArgs.Empty);
                    Invalidate(true);
                }
                //update the left state
                _left = leftState;
            }
            else
            {
                //If the mouse left the control...
                if(previouslyInControl)
                {
                    Invalidate(true);
                }
                //If we moved, fire the MouseMove event.
                if (moved)
                {
                    MouseMove?.Invoke(this, new Vector2(x, y));
                }

                //Reset the mouse states
                _left = ButtonState.Released;
                _right = ButtonState.Released;
                _middle = ButtonState.Released;
                return false;
            }
            return true;

            /* Old code.
            if (_enabled == false || _isVisible == false)
                return false;
            int x = 0;
            int y = 0;

            if (Parent == null)
            {
                x = state.X - X;
                y = state.Y - Y;
            }
            //For controls with parents, poll mouse information from the parent.
            else
            {
                x = Parent._mousex - X;
                y = Parent._mousey - Y;
            }
            if(_mousex != x || _mousey != y)
            {
                bool hasMouse = ContainsMouse;
                _mousex = x;
                _mousey = y;
                MouseMove?.Invoke(this, new Vector2(x, y));
                if(ContainsMouse != hasMouse)
                {
                    Invalidate(true);
                }
            }

            bool doEvents = !skipEvents;
            foreach(var child in Children.OrderByDescending(z=>Array.IndexOf(Children, z)))
            {
                bool res = child.PropagateMouseState(state, skipEvents);
                if (doEvents == true && res == true)
                    return true;
            }
            if (doEvents)
            {
                if (ContainsMouse)
                {
                    if(state.ScrollWheelValue != _lastScrollValue)
                    {
                        _lastScrollValue = state.ScrollWheelValue;
                        OnMouseScroll(state.ScrollWheelValue);
                        MouseScroll?.Invoke(state.ScrollWheelValue);
                    }
                    bool left = LeftMouseState == ButtonState.Pressed;
                    bool right = RightMouseState == ButtonState.Pressed;
                    bool middle = MiddleMouseState == ButtonState.Pressed;

                    if(LeftMouseState != state.LeftButton)
                    {
                        if(left)
                        {
                            if (_doubleClickCooldown == 0)
                            {
                                Click?.Invoke(this, EventArgs.Empty);
                                _doubleClickCooldown = 250;
                                if(!Manager.IsFocused(this))
                                {
                                    Manager.SetFocus(this);
                                    Invalidate(true);
                                }
                            }
                            else
                            {
                                DoubleClick?.Invoke(this, EventArgs.Empty);
                            }
                            MouseLeftUp?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            MouseLeftDown?.Invoke(this, EventArgs.Empty);
                        }
                        _lastLeft = state.LeftButton;
                        Invalidate(true);
                    }
                    if (RightMouseState != state.RightButton)
                    {
                        if (right)
                        {
                            MouseRightUp?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            MouseRightDown?.Invoke(this, EventArgs.Empty);
                        }
                        _lastRight = state.RightButton;
                        Invalidate(true);
                    }

                    return true;
                }
                else
                {
                    if (_lastLeft != ButtonState.Released || _lastRight != ButtonState.Released || _lastMiddle != ButtonState.Released)
                    {
                        _lastLeft = ButtonState.Released;
                        _lastRight = ButtonState.Released;
                        _lastMiddle = ButtonState.Released;
                        Invalidate(true);
                    }

                }
            }
            return false;*/
        }

        private bool? _lastFocus = null;

        /// <summary>
        /// Occurs when the control gains or loses focus.
        /// </summary>
        public event EventHandler HasFocusedChanged;

        /// <summary>
        /// Retrieves an array containing all child controls.
        /// </summary>
        public Control[] Children
        {
            get
            {
                if (_children == null)
                    return new Control[0];
                return _children.ToArray();
            }
        }

        private MouseState _lastState;

        /// <summary>
        /// Fire an update event.
        /// </summary>
        /// <param name="time">The time since the last frame. Used for animation.</param>
        public void Update(GameTime time)
        {
            if (_lastFocus != HasFocused)
            {
                _lastFocus = HasFocused;
                HasFocusedChanged?.Invoke(this, EventArgs.Empty);
            }
            if (_rendertarget == null)
            {
                Invalidate(true);
                _resized = true;
            }
            if (_userfacingtarget == null)
            {
                Invalidate(true);
                _resized = true;
            }

            if (_isVisible == false)
                return;
            OnUpdate(time);
            if (_children == null)
                return;
            if (_disposed)
                return;
            try
            {
                foreach (var child in _children)
                {
                    child.Update(time);
                }
            }
            catch { }
            if (_doubleClickCooldown >= 0)
            {
                _doubleClickCooldown = MathHelper.Clamp((int)(_doubleClickCooldown - time.ElapsedGameTime.TotalMilliseconds), 0, 250);
            }
        }



        /// <summary>
        /// Paint the control onto its front surface. 
        /// </summary>
        /// <param name="time">The time since the last frame.</param>
        /// <param name="gfx">The graphics context used to render to the back buffer.</param>
        protected virtual void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlBG(gfx, 0, 0, Width, Height);
        }

        private bool _needsRerender = true;

        /// <summary>
        /// Invalidate the control's render surfaces.
        /// </summary>
        /// <param name="needsRepaint">Whether the control's front surface needs repainting.</param>
        public void Invalidate(bool needsRepaint = false)
        {
            if (needsRepaint)
            {
                _invalidated = true;
            }
            _needsRerender = true;
            Parent?.Invalidate();
        }

        /// <summary>
        /// Fire a render event.
        /// </summary>
        /// <param name="time">The time since the last frame.</param>
        /// <param name="gfx">The graphics context to render the control to.</param>
        public void Draw(GameTime time, GraphicsContext gfx)
        {
            bool makeBack = false; //normally I'd let this be false but I thought I'd try making the backbuffer reset if the control's invalidated. This seemed to help, but right after restarting the game and doing the same thing, the bug was back. So this only works intermitently.
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
                _rendertarget = new RenderTarget2D(gfx.Device, Width, Height, false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 8, RenderTargetUsage.PreserveContents);
            }

            if (_needsRerender)
            {
                if (_invalidated)
                {
                    if (_resized)
                    {
                        _userfacingtarget?.Dispose();
                        _userfacingtarget = new RenderTarget2D(gfx.Device, Width, Height, false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 8, RenderTargetUsage.PreserveContents);
                        _resized = false;
                    }
                    gfx.Device.SetRenderTarget(_userfacingtarget);
                    gfx.Device.Clear(Color.Transparent);
                    gfx.BeginDraw();
                    OnPaint(time, gfx);
                    gfx.EndDraw();
                    _invalidated = false;
                }
                foreach (var child in Children)
                {
                    if (!child.Visible)
                        continue;
                    if (child.Opacity > 0)
                        child.Draw(time, gfx);
                }
                gfx.Device.SetRenderTarget(_rendertarget);
                gfx.Device.Clear(Color.Transparent);
                if (_userfacingtarget != null)
                {
                    gfx.BeginDraw();
                    gfx.Batch.Draw(_userfacingtarget, new Rectangle(0, 0, Width, Height), Color.White);
                    foreach (var child in Children)
                    {
                        if (!child.Visible)
                            continue;
                        if (child.Opacity > 0)
                        {
                            var tint = (child.Enabled) ? Color.White : Color.Gray;
                            if (Manager.IgnoreControlOpacity)
                            {
                                gfx.Batch.Draw(child.BackBuffer, new Rectangle(child.X, child.Y, child.Width, child.Height), tint);
                            }
                            else
                            {
                                gfx.Batch.Draw(child.BackBuffer, new Rectangle(child.X, child.Y, child.Width, child.Height), tint * child.Opacity);
                            }
                        }
                    }
                    gfx.EndDraw();
                }
                else
                {
                    Invalidate(true);
                    _resized = true;

                }
                _needsRerender = false;

            }
        }

        /// <summary>
        /// Retrieve the left mouse state.
        /// </summary>
        public ButtonState LeftMouseState
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Retrieve the right mouse state.
        /// </summary>
        public ButtonState RightMouseState
        {
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// Retrieve the middle mouse state.
        /// </summary>
        public ButtonState MiddleMouseState
        {
            get
            {
                return _middle;
            }
        }

        /// <inheritdoc/>
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
            if (_children != null)
            {
                while (_children.Count > 0)
                {
                    _children[0].Dispose();
                    _children.RemoveAt(0);
                }
                _children = null;
            }
            _disposed = true;
        }
    }
}
