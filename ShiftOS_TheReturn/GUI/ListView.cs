using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class ListView : Control
    {
        //Fields
        private Dictionary<string, Texture2D> _imagecache = null;
        private ListViewLayout _layout = ListViewLayout.LargeIcon;
        private IconFlowDirection _flow = IconFlowDirection.LeftToRight;
        private int _margin = 15;
        private int _hGap = 5;
        private int _vGap = 3;
        private int _selectedIndex = -1;

        public event EventHandler SelectedIndexChanged;

        public ListViewItem SelectedItem
        {
            get
            {
                if (!(_selectedIndex >= 0 && _selectedIndex < Children.Length))
                    return null;
                return Children[_selectedIndex] as ListViewItem;
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                value = MathHelper.Clamp(value, -1, Children.Length);
                if (_selectedIndex == value)
                    return;
                if (SelectedItem != null)
                    SelectedItem.Selected = false;
                _selectedIndex = value;
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Specifies the gap between items on the horizontal axis. Only affects LargeIcon and SmallIcon views, default is 5 pixels.
        /// </summary>
        public int HGap
        {
            get
            {
                return _hGap;
            }
            set
            {
                if (_hGap == value)
                    return;
                _hGap = value;
            }
        }

        /// <summary>
        /// Specifies the gap between items on the vertical axis. Only affects LargeIcon and SmallIcon views, default is 3 pixels.
        /// </summary>
        public int VGap
        {
            get
            {
                return _vGap;
            }
            set
            {
                if (_vGap == value)
                    return;
                _vGap = value;
            }
        }

        /// <summary>
        /// Specifies the gap between the edge of the list view and its items. Only affects the LargeIcon and SmallIcon views, default is 15 pixels.
        /// </summary>
        public int Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                if (_margin == value)
                    return;
                _margin = value;
            }
        }

        /// <summary>
        /// Determines the direction in which list view items are laid out. Only affects SmallIcon and LargeIcon view modes. Default is LeftToRight.
        /// </summary>
        public IconFlowDirection IconFlow
        {
            get
            {
                return _flow;
            }
            set
            {
                if (_flow == value)
                    return;
                _flow = value;
            }
        }

        /// <summary>
        /// Determines the layout of the list view and its items. Default is LargeIcon.
        /// </summary>
        public ListViewLayout Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                if (_layout == value)
                    return;
                _layout = value;
            }
        }

        public ListView()
        {
            _imagecache = new Dictionary<string, Texture2D>();
        }

        public Texture2D GetImage(string key)
        {
            if (_imagecache.ContainsKey(key))
                return _imagecache[key];
            else
                return null;
        }

        public void SetImage(string key, Texture2D texture)
        {
            if (_imagecache.ContainsKey(key))
                _imagecache[key] = texture;
            else
                _imagecache.Add(key, texture);
        }

        public void RemoveImage(string key)
        {
            if (_imagecache.ContainsKey(key))
                _imagecache.Remove(key);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }

        //Method overrides for list view
        public override void AddChild(Control child)
        {
            if (child == null)
                return;
            if (child.GetType() != typeof(ListViewItem))
                throw new InvalidOperationException("List view controls can only host childs of type " + (typeof(ListViewItem).FullName) + ".");
            base.AddChild(child);
        }

        protected override void OnUpdate(GameTime time)
        {
            switch (_layout)
            {
                case ListViewLayout.LargeIcon:
                case ListViewLayout.SmallIcon:
                    int _x = Margin;
                    int _y = Margin;
                    int _h = 0;
                    foreach(var child in Children)
                    {
                        child.X = _x;
                        child.Y = _y;
                        if(_x + child.Width + _hGap > Width - (Margin * 2))
                        {
                            _x = Margin;
                            _y += child.Height + _vGap;
                        }
                        else
                        {
                            _x += child.Width + _hGap;
                        }
                        _h = Math.Max(_h, child.Y + child.Height + Margin);
                    }
                    Height = _h;
                    break;
                case ListViewLayout.List:
                    //Layout as a list
                    int y = 0;
                    foreach(var child in Children)
                    {
                        child.X = 0;
                        child.Y = y;
                        y += child.Height;
                    }
                    Height = y;
                    break;
            }
        }
    }

    public class ListViewItem : Control
    {
        private Label _label = null;
        private PictureBox _picture = null;
        private ListView _view = null;
        private bool _selected = false;
        private object _value = null;
        private string _ikey = null;

        

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected == value)
                    return;
                _selected = value;
                Invalidate();
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                if (_value == null)
                    _label.Text = "";
                else
                    _label.Text = _value.ToString();
            }
        }

        public string ImageKey
        {
            get
            {
                return _ikey;
            }
            set
            {
                if (_ikey == value)
                    return;
                _ikey = value;
            }
        }

        public object Tag { get; set; }

        public ListViewItem(ListView parent) : base()
        {
            _label = new GUI.Label();
            _picture = new PictureBox();
            AddChild(_label);
            AddChild(_picture);

            _view = parent;
            _view.AddChild(this);
            Click += (o, a) =>
            {
                selectme();
            };
            _picture.Click += (o, a) =>
            {
                selectme();
            };
            _label.Click += (o, a) =>
            {
                selectme();
            };
        }

        private void selectme()
        {
            _view.SelectedIndex = Array.IndexOf(_view.Children, this);
            Selected = true;
        }

        protected override void OnUpdate(GameTime time)
        {
            if(!string.IsNullOrWhiteSpace(_ikey))
            {
                _picture.Texture = _view.GetImage(_ikey);
            }
            else
            {
                _picture.Texture = null;
            }

            _picture.AutoSize = false;
            _picture.Layout = System.Windows.Forms.ImageLayout.Zoom;

            switch (_view.Layout)
            {
                case ListViewLayout.LargeIcon:
                    _picture.Width = 48;
                    _picture.Height = 48;
                    _label.AutoSize = true;
                    _label.MaxWidth = _picture.Width + 20;
                    _label.Alignment = TextAlignment.Top;
                    _label.Update(time);
                    Width = Math.Max(_label.Width, _picture.Width) + 20;
                    _picture.X = (Width - _picture.Width) / 2;
                    _label.X = (Width - _label.Width) / 2;
                    _picture.Y = 10;
                    _label.Y = _picture.Y + _picture.Height + 5;
                    Height = _label.Y + _label.Height + 10;
                    break;
                case ListViewLayout.SmallIcon:
                    Width = _view.Width;
                    _picture.Width = 24;
                    _picture.Height = 24;
                    _label.AutoSize = true;
                    _label.MaxWidth = 200;
                    _label.Alignment = TextAlignment.Left;
                    _label.Update(time);
                    Height = (Math.Max(_picture.Height, _label.Height) + 10);
                    _picture.X = 5;
                    _label.X = _picture.X + _picture.Width + 3;
                    Width = _label.X + _label.Width + 5;
                    _picture.Y = (Height - _picture.Height) / 2;
                    _label.Y = (Height - _label.Height) / 2;
                    break;
                case ListViewLayout.List:
                    Width = _view.Width;
                    _picture.Width = 20;
                    _picture.Height = 20;
                    int maxWidth = (Width - 4);
                    if (_picture.Texture != null)
                        maxWidth -= _picture.Width + 3;
                    _label.AutoSize = true;
                    _label.MaxWidth = maxWidth;
                    _label.Alignment = TextAlignment.Left;
                    _label.Update(time);
                    _picture.X = 2;
                    if (_picture.Texture != null)
                        _label.X = _picture.X + _picture.Width + 3;
                    Height = Math.Max(_label.Height, _picture.Height) + 4;
                    _picture.Y = (Height - _picture.Height) / 2;
                    _label.Y = (Height - _label.Height) / 2;

                    break;
            }
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Transparent);
            var accent = Theme.GetAccentColor();
            if(_view.Layout != ListViewLayout.List)
                _picture.Tint = (_selected) ? accent : Color.White;

            var highlight = Color.Transparent;
            if (ContainsMouse)
                highlight = accent * 0.5F;
            if (_selected)
                highlight = accent;
            switch (_view.Layout)
            {
                case ListViewLayout.LargeIcon:
                case ListViewLayout.SmallIcon:
                    //only highlight text
                    gfx.DrawRectangle(_label.X, _label.Y, _label.Width, _label.Height, highlight);
                    break;
                case ListViewLayout.List:
                    //highlight entire item
                    gfx.DrawRectangle(0, 0, Width, Height, highlight);
                    break;
            }
        }
    }

    public enum ListViewLayout
    {
        LargeIcon,
        SmallIcon,
        List
    }

    public enum IconFlowDirection
    {
        LeftToRight,
        RightToLeft,
        TopDown,
        BottomUp
    }
}
