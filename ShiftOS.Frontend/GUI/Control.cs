using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using ShiftOS.Frontend.GraphicsSubsystem;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;

namespace ShiftOS.Frontend.GUI
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

        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = (double)MathHelper.Clamp((float)value, 0, 1);
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
                _visible = value;
            }
        }

        public void AddControl(Control ctrl)
        {
            if (!_children.Contains(ctrl))
            {
                ctrl._parent = this;
                _children.Add(ctrl);
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

        public int Width
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
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
                _h = value;
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
        }

        public Point PointToLocal(int x, int y)
        {
            return new GUI.Point(x - _x, y - _y);
        }

        public virtual void MouseStateChanged() { }

        public virtual void Paint(System.Drawing.Graphics gfx)
        {
            if (_visible == true)
            {
                if (_children.Count > 0)
                {
                    foreach (var child in _children)
                    {
                        using (var cBmp = new System.Drawing.Bitmap(child.Width, child.Height))
                        {
                            child.Paint(System.Drawing.Graphics.FromImage(cBmp));
                            cBmp.SetOpacity((float)child.Opacity);
                            gfx.DrawImage(cBmp, new System.Drawing.Point(child.X, child.Y));

                        }
                    }
                }
            }
        }

        public void Layout()
        {
            //Dock style
            if(_parent != null)
            {
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
            OnLayout();
            foreach (var child in _children)
                child.Layout();
        }

        protected virtual void OnLayout()
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

        public virtual bool ProcessMouseState(MouseState state)
        {
            //If we aren't rendering the control, we aren't accepting input.
            if (_visible == false)
                return false;


            //Firstly, we get the mouse coordinates in the local space
            var coords = PointToLocal(state.Position.X, state.Position.Y);
            //Now we check if the mouse is within the bounds of the control
            if(coords.X > 0 && coords.Y > 0 && coords.X <= _w && coords.Y <= _h)
            {
                //We're in the local space. Let's fire the MouseMove event.
                MouseMove?.Invoke(coords);
                //Also, if the mouse hasn't been in the local space last time it moved, fire MouseEnter.
                if(_wasMouseInControl == false)
                {
                    _wasMouseInControl = true;
                    MouseEnter?.Invoke();
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
                        _requiresMoreWork = !control.ProcessMouseState(nstate);
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
                        Click?.Invoke();
                    if (_leftState == false && ld == true)
                        UIManager.FocusedControl = this;
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
                if(_wasMouseInControl == true)
                {
                    _wasMouseInControl = false;
                    MouseLeave?.Invoke();
                }
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

        public event Action<Point> MouseMove;
        public event Action MouseEnter;
        public event Action MouseLeave;
        public event Action Click;
        public event Action<KeyEvent> KeyEvent;
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
}
