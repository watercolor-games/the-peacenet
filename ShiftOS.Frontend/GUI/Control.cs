using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using Plex.Frontend.GraphicsSubsystem;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace Plex.Frontend.GUI
{
    public abstract class Control
    {
        private int _x = 0;
        private int _y = 0;
        private int _w = 0;
        private int _h = 0;
        private Control _parent = null;
        private List<Control> _children = new List<Control>();
        private bool _wasMouseInControl = false;
        private bool _leftState = false;
        private bool _rightState = false;
        private bool _middleState = false;
        private bool _visible = true;
        private DockStyle _dock = DockStyle.None;
        private bool _focused = false;
        private bool _autoSize = false;
        private double _opacity = 1.0;
        private bool _invalidated = true;
        private Bitmap _texCache = null;
        private Anchor _anchor = null;
        private int _mouseX = 0;
        private int _mouseY = 0;
        private bool _captureMouse = false;
        
        public void BringToFront()
        {
            if(_parent != null)
            {
                _parent._children.Remove(this);
                _parent.AddControl(this);
            }
            else
            {
                UIManager.BringToFront(this);
            }
        }

        public bool RequiresPaint
        {
            get
            {
                bool requires_child_repaint = false;
                foreach (var child in _children)
                {
                    requires_child_repaint = child.RequiresPaint;
                    if (requires_child_repaint)
                        break;
                }
                return _invalidated || requires_child_repaint;
            }
        }

        public Image TextureCache
        {
            get
            {
                return _texCache;
            }
        }

        public byte[] PaintCache
        {
            get
            {
                var data = _texCache.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var rgb = new byte[Math.Abs(data.Stride) * data.Height];
                Marshal.Copy(data.Scan0, rgb, 0, rgb.Length);
                for(int i = 0; i < rgb.Length; i += 4)
                {
                    byte r = rgb[i];
                    byte b = rgb[i + 2];
                    rgb[i] = b;
                    rgb[i + 2] = r;
                }
                _texCache.UnlockBits(data);
                return rgb;
            }
        }

        public bool CaptureMouse
        {
            get
            {
                return _captureMouse;
            }
            set
            {
                _captureMouse = value;
            }
        }

        public int MouseX
        {
            get
            {
                return _mouseX;
            }
        }

        public int MouseY
        {
            get
            {
                return _mouseY;
            }
        }


        public Anchor Anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                if (_anchor == value)
                    return;

                _anchor = value;
                Invalidate();
            }
        }

        public void Invalidate()
        {
            _invalidated = true;
            foreach(var child in _children)
            {
                child.Invalidate();
            }
        }

        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                if (_opacity == value)
                    return;
                _opacity = value;
                Invalidate();
            }
        }

        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                _autoSize = value;
            }
        }

        //Thank you, StackOverflow.
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public DockStyle Dock
        {
            get
            {
                return _dock;
            }
            set
            {
                _dock = value;
            }
        }

        public bool ContainsMouse
        {
            get { return _wasMouseInControl; }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                Invalidate();
            }
        }

        public void AddControl(Control ctrl)
        {
            if (!_children.Contains(ctrl))
            {
                ctrl._parent = this;
                _children.Add(ctrl);
                Invalidate();
                UIManager.FocusedControl = ctrl;
            }
        }

        public bool MouseLeftDown
        {
            get
            {
                return _leftState;
            }
        }

        public bool MouseMiddleDown
        {
            get
            {
                return _middleState;
            }
        }

        public bool MouseRightDown
        {
            get
            {
                return _rightState;
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
                if (_x == value)
                    return;
                _x = value;
                Invalidate();
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
                if (_y == value)
                    return;
                _y = value;
                Invalidate();
            }
        }

        public int Width
        {
            get
            {
                return _w;
            }
            set
            {
                if (_w == value)
                    return;
                _w = value;
                Invalidate();
            }
        }

        public int Height
        {
            get
            {
                return _h;
            }
            set
            {
                if (_h == value)
                    return;
                _h = value;
                Invalidate();
            }
        }

        public Control Parent
        {
            get
            {
                return _parent;
            }
        }

        public Control[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }

        public Point PointToParent(int x, int y)
        {
            return new Point(x + _x, y + _y);
        }

        public Point PointToScreen(int x, int y)
        {
            var parentCoords = PointToParent(x, y);
            Control parent = this._parent;
            while(parent != null)
            {
                parentCoords = parent.PointToParent(parentCoords.X, parentCoords.Y);
                parent = parent.Parent;
            }
            return parentCoords;
        }

        public void ClearControls()
        {
            _children.Clear();
            Invalidate(); 
        }

        public void RemoveControl(Control ctrl)
        {
            if(_children.Contains(ctrl))
            {
                _children.Remove(ctrl);
                ctrl._parent = null;
                Invalidate();
            }
        }

        public Point PointToLocal(int x, int y)
        {
            return new GUI.Point(x - _x, y - _y);
        }

        public virtual void MouseStateChanged() { }

        protected virtual void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlColor"]);
        }

        public void SendToBack()
        {
            if(_parent != null)
            {
                _parent._children.Remove(this);
                _parent._children.Insert(0, this);
            }
            else
            {
                UIManager.SendToBack(this);
            }
        }

        public void InvalidateTopLevel()
        {
            var parent = this;
            while (parent.Parent != null)
                parent = parent.Parent;
            parent.Invalidate();
        }

        public void Paint(GraphicsContext gfx)
        {
            if (_visible == true)
            {
                OnPaint(gfx);
                int draw_x = gfx.X;
                int draw_y = gfx.Y;
                int draw_width = gfx.Width;
                int draw_height = gfx.Height;
                foreach (var ctrl in _children)
                {
                    if (ctrl.Visible == true)
                    {
                        gfx.X = draw_x + ctrl.X;
                        gfx.Y = draw_y + ctrl.Y;
                        gfx.Width = ctrl.Width;
                        gfx.Height = ctrl.Height;
                        ctrl.Paint(gfx);
                        gfx.X = draw_x;
                        gfx.Y = draw_y;
                    }
                    gfx.Width = draw_width;
                    gfx.Height = draw_height;
                }
                _invalidated = false;
            }
        }

        public void Layout(GameTime gameTime)
        {
            //Dock style
            if(_parent != null)
            {
                if(_anchor != null)
                {
                    
                }

                switch (_dock)
                {
                    case DockStyle.Top:
                        X = 0;
                        Y = 0;
                        Width = _parent.Width;
                        break;
                    case DockStyle.Left:
                        X = 0;
                        Y = 0;
                        Height = _parent.Height;
                        break;
                    case DockStyle.Right:
                        Y = 0;
                        X = _parent.Width - Width;
                        Height = _parent.Height;
                        break;
                    case DockStyle.Bottom:
                        X = 0;
                        Y = _parent.Height - Height;
                        Width = _parent.Width;
                        break;
                    case DockStyle.Fill:
                        X = 0;
                        Y = 0;
                        Width = _parent.Width;
                        Height = _parent.Height;
                        break;
                }
            }
            OnLayout(gameTime);
            foreach (var child in _children)
                child.Layout(gameTime);
        }

        protected virtual void OnLayout(GameTime gameTime)
        {
            //do nothing
        }

        public bool IsFocusedControl
        {
            get
            {
                return UIManager.FocusedControl == this;
            }
        }

        public bool ContainsFocusedControl
        {
            get
            {
                if (UIManager.FocusedControl == null)
                    return false;
                else
                {
                    bool contains = false;

                    var ctrl = UIManager.FocusedControl;
                    while(ctrl.Parent != null)
                    {
                        ctrl = ctrl.Parent;
                        if (ctrl == this)
                            contains = true;
                    }
                    return contains;
                }
            }
        }

        public virtual bool ProcessMouseState(MouseState state, double lastLeftClickMS)
        {
            //If we aren't rendering the control, we aren't accepting input.
            if (_visible == false)
                return false;


            //Firstly, we get the mouse coordinates in the local space
            var coords = PointToLocal(state.Position.X, state.Position.Y);
            _mouseX = coords.X;
            _mouseY = coords.Y;
            //Now we check if the mouse is within the bounds of the control
            if(coords.X >= 0 && coords.Y >= 0 && coords.X <= _w && coords.Y <= _h)
            {
                //We're in the local space. Let's fire the MouseMove event.
                MouseMove?.Invoke(coords);
                //Also, if the mouse hasn't been in the local space last time it moved, fire MouseEnter.
                if(_wasMouseInControl == false)
                {
                    _wasMouseInControl = true;
                    MouseEnter?.Invoke();
                    Invalidate();
                }

                //Things are going to get a bit complicated.
                //Firstly, we need to find out if we have any children.
                bool _requiresMoreWork = true;
                if(_children.Count > 0)
                {
                    //We do. We're going to iterate through them all and process the mouse state.
                    foreach(var control in _children)
                    {

                        //If the process method returns true, then we do not need to do anything else on our end.

                        //We need to first create a new mousestate object with the new coordinates

                        var nstate = new MouseState(coords.X, coords.Y, state.ScrollWheelValue, state.LeftButton, state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
                        //pass that state to the process method, and set the _requiresMoreWork value to the opposite of the return value
                        _requiresMoreWork = !control.ProcessMouseState(nstate, lastLeftClickMS);
                        //If it's false, break the loop.
                        if (_requiresMoreWork == false)
                            break;
                    }
                }

                //If we need to do more work...
                if(_requiresMoreWork == true)
                {
                    bool fire = false; //so we know to fire a MouseStateChanged method
                    //Let's get the state values of each button
                    bool ld = state.LeftButton == ButtonState.Pressed;
                    bool md = state.MiddleButton == ButtonState.Pressed;
                    bool rd = state.RightButton == ButtonState.Pressed;
                    if(ld != _leftState || md != _middleState || rd != _rightState)
                    {
                        fire = true;
                    }
                    if (_leftState == true && ld == false)
                    {
                        Click?.Invoke();
                        Invalidate();
                        MouseUp?.Invoke();
                    }
                    if (_leftState == false && ld == true)
                    {
                        if (lastLeftClickMS <= 500 & lastLeftClickMS > 0)
                            DoubleClick?.Invoke();
                        var focused = UIManager.FocusedControl;
                        UIManager.FocusedControl = this;
                        focused?.InvalidateTopLevel();
                        InvalidateTopLevel();
                        MouseDown?.Invoke();

                    }
                    _leftState = ld;
                    _middleState = md;
                    _rightState = rd;
                    if (fire)
                        MouseStateChanged();
                }
                return true;
            }
            else
            {
                _leftState = false;
                _rightState = false;
                _middleState = false;
                MouseStateChanged();
                //If the mouse was in local space before, fire MouseLeave
                if (_wasMouseInControl == true)
                {
                    if (CaptureMouse == true)
                    {
                        _wasMouseInControl = true;
                        int newX = MathHelper.Clamp(state.X, X, X + Width);
                        int newY = MathHelper.Clamp(state.Y, Y, Y + Height);
                        Mouse.SetPosition(newX, newY);

                    }
                    else
                    {
                        _wasMouseInControl = false;
                        MouseLeave?.Invoke();
                        Invalidate();
                    }
                }
            }
            if (CaptureMouse == true)
            {
                _mouseX = coords.X;
                _mouseY = coords.Y;
                _wasMouseInControl = true;
                int newX = MathHelper.Clamp(state.X, X, X + Width);
                int newY = MathHelper.Clamp(state.Y, Y, Y + Height);
                Mouse.SetPosition(newX, newY);
                return true;
            }

            //Mouse is not in the local space, don't do anything.
            return false;
        }

        protected virtual void OnKeyEvent(KeyEvent e)
        {

        }

        public void ProcessKeyEvent(KeyEvent e)
        {
            OnKeyEvent(e);
            KeyEvent?.Invoke(e);
        }

        public event Action DoubleClick;
        public event Action<Point> MouseMove;
        public event Action MouseEnter;
        public event Action MouseLeave;
        public event Action Click;
        public event Action<KeyEvent> KeyEvent;
        public event Action MouseDown;
        public event Action MouseUp;
    }

    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public enum DockStyle
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Fill
    }

    //Thanks, StackOverflow.
    public static class BitmapExtensions
    {
        public static Image SetOpacity(this Image image, float opacity)
        {
            var colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = opacity;
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
            var output = new Bitmap(image.Width, image.Height);
            using (var gfx = Graphics.FromImage(output))
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.DrawImage(
                    image,
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    0,
                    0,
                    image.Width,
                    image.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }
            return output;
        }
    }

    [Flags]
    public enum AnchorStyle
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public class Anchor
    {
        public AnchorStyle Style { get; set; }
        public int Distance { get; set; }
    }
}
