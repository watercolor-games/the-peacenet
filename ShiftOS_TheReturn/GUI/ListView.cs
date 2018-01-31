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
    /// <summary>
    /// A control capable of holding a list of items and displaying them in various different views.
    /// </summary>
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
        private bool _requireLayout = true;

        private int _visibleItems = -1;

        /// <summary>
        /// Retrieves the number of items which are visible based on the currently applied item filter.
        /// </summary>
        public int VisibleItems
        {
            get
            {
                return (_visibleItems == -1) ? GetItems().Length : _visibleItems;
            }
        }

        /// <summary>
        /// Occurs when the player selects a different item in the list.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Retrieves the currently selected item. Returns null if none is selected.
        /// </summary>
        public ListViewItem SelectedItem
        {
            get
            {
                if (!(_selectedIndex >= 0 && _selectedIndex < Children.Length))
                    return null;
                return Children[_selectedIndex] as ListViewItem;
            }
        }

        /// <summary>
        /// Retrieves the index of the selected item. Returns -1 if no item is selected.
        /// </summary>
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
                _requireLayout = true;
                if (SelectedItem.Selected == false)
                    SelectedItem.Selected = true;
            }
        }

        /// <summary>
        /// Retrieves an array of all items in the list.
        /// </summary>
        /// <returns>All items in the list.</returns>
        public ListViewItem[] GetItems()
        {
            List<ListViewItem> items = new List<ListViewItem>();
            foreach(var child in Children)
            {
                if (child is ListViewItem)
                    items.Add(child as ListViewItem);
            }
            return items.ToArray();
        }

        internal void RequireLayout()
        {
            _requireLayout = true;
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
                _requireLayout = true;
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
                _requireLayout = true;
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
                _requireLayout = true;
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
                _requireLayout = true;
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
                _requireLayout = true;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListView"/> control. 
        /// </summary>
        public ListView()
        {
            _imagecache = new Dictionary<string, Texture2D>();
        }

        /// <summary>
        /// Retrieve the icon with the specified key.
        /// </summary>
        /// <param name="key">The key of the requested icon.</param>
        /// <returns>The icon found by the search. Returns null if no icon was found.</returns>
        public Texture2D GetImage(string key)
        {
            if (_imagecache.ContainsKey(key))
                return _imagecache[key];
            else
                return null;
        }

        /// <summary>
        /// Set the icon of a specified key.
        /// </summary>
        /// <param name="key">The key of the icon to set.</param>
        /// <param name="texture">The new icon to set.</param>
        public void SetImage(string key, Texture2D texture)
        {
            if (_imagecache.ContainsKey(key))
                _imagecache[key] = texture;
            else
                _imagecache.Add(key, texture);
        }

        /// <summary>
        /// Remove an icon from the icon list.
        /// </summary>
        /// <param name="key">The key of the icon to remove.</param>
        public void RemoveImage(string key)
        {
            if (_imagecache.ContainsKey(key))
                _imagecache.Remove(key);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }

        /// <inheritdoc/>
        public override void AddChild(Control child)
        {
            if (child == null)
                return;
            if (child.GetType() != typeof(ListViewItem))
                throw new InvalidOperationException("List view controls can only host childs of type " + (typeof(ListViewItem).FullName) + ".");
            base.AddChild(child);
            (child as ListViewItem).ItemClicked += this.child_DoubleClicked;
            _requireLayout = true;
        }

        private void child_DoubleClicked(ListViewItem child)
        {
            ItemClicked?.Invoke(child);
        }

        /// <summary>
        /// Occurs when a list view item is clicked.
        /// </summary>
        public event Action<ListViewItem> ItemClicked;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (!_requireLayout)
                return;
            switch (_layout)
            {
                case ListViewLayout.LargeIcon:
                case ListViewLayout.SmallIcon:
                    int _x = 0;
                    int _y = 0;
                    switch (_flow)
                    {
                        case IconFlowDirection.LeftToRight:
                            _x = Margin;
                            _y = Margin;
                            break;
                        case IconFlowDirection.TopDown:
                            _x = Margin;
                            _y = Margin;
                            break;
                    }
                    int _h = 0;
                    foreach(var child in Children)
                    {
                        if (!child.Visible)
                            continue;
                        child.X = _x;
                        child.Y = _y;
                        switch (_flow)
                        {
                            case IconFlowDirection.LeftToRight:
                                if (_x + child.Width + _hGap > Width - (Margin * 2))
                                {
                                    _x = Margin;
                                    _y += child.Height + _vGap;
                                }
                                else
                                {
                                    _x += child.Width + _hGap;
                                }
                                _h = Math.Max(_h, child.Y + child.Height + Margin);
                                break;
                            case IconFlowDirection.TopDown:
                                if (_y + child.Height + _vGap > Height - (Margin * 2))
                                {
                                    _y = Margin;
                                    _x += child.Width + _hGap;
                                }
                                else
                                {
                                    _y += child.Height + _vGap;
                                }
                                _h = Math.Max(_h, child.X + child.Width + Margin);
                                break;
                        }
                    }
                    if (_flow == IconFlowDirection.LeftToRight)
                    {
                        Height = _h;
                    }
                    else
                    {
                        Width = _h;
                    }
                    break;
                case ListViewLayout.List:
                    //Layout as a list
                    int y = 0;
                    foreach(var child in Children)
                    {
                        if (!child.Visible)
                            continue;
                        child.X = 0;
                        child.Y = y;
                        y += child.Height;
                    }
                    Height = y;
                    break;
            }
            _requireLayout = false;
        }

        /// <summary>
        /// Set an item filter on this list view.
        /// </summary>
        /// <param name="name">The filter for the list view. Only items which contain this string will show. Set to null or an empty string to disable the filter.</param>
        public void SetFilter(string name)
        {
            int visible = -1;
            bool isEmpty = string.IsNullOrWhiteSpace(name);
            foreach(var item in GetItems())
            {
                if (isEmpty)
                    item.Visible = true;
                else
                {
                    if (visible == -1)
                        visible++;
                    item.Visible = (bool)item.Value?.ToString()?.Contains(name);
                    if (item.Visible)
                        visible++;
                }
            }
            _visibleItems = visible;
            _requireLayout = true;
        }

        /// <inheritdoc/>
        public override void RemoveChild(Control child)
        {
            if (!(child is ListViewItem))
                throw new InvalidOperationException("You should never EVER see this.");
            base.RemoveChild(child);
            _requireLayout = true;
            (child as ListViewItem).ItemClicked -= this.child_DoubleClicked;
        }
    }

    /// <summary>
    /// Represents a control displayable within a <see cref="ListView"/>. 
    /// </summary>
    public class ListViewItem : Control
    {
        private Label _label = null;
        private PictureBox _picture = null;
        private ListView _view = null;
        private bool _selected = false;
        private object _value = null;
        private string _ikey = null;

        /// <summary>
        /// Occurs when the item is double-clicked.
        /// </summary>
        public event Action<ListViewItem> ItemClicked;

        /// <summary>
        /// Gets or sets whether the item is selected.
        /// </summary>
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
                Invalidate(true);
                _requiresLayout = true;
            }
        }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
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
                _requiresLayout = true;
            }
        }

        /// <summary>
        /// Gets or sets the key of the icon for the item.
        /// </summary>
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
                _requiresLayout = true;
            }
        }

        /// <summary>
        /// Gets or sets an optional data tag to associate with the item. Use it for whatever you want.
        /// </summary>
        public object Tag { get; set; }

        private void _doubleClick (object s, EventArgs e)
        {
            ItemClicked?.Invoke(this);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListViewItem"/> control. 
        /// </summary>
        /// <param name="parent">The <see cref="ListView"/> to which this item will be added.</param>
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
            _label.DoubleClick += _doubleClick;
            _picture.DoubleClick += _doubleClick;
            this.DoubleClick += _doubleClick;

        }

        private void selectme()
        {
            _view.SelectedIndex = Array.IndexOf(_view.Children, this);
            Selected = true;
        }

        private bool _requiresLayout = true;
        private bool _lastContainedMouse = false;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_lastContainedMouse != ContainsMouse)
            {
                _lastContainedMouse = ContainsMouse;
                Invalidate(true);
            }
            if (!_requiresLayout)
                return;
            _view.RequireLayout();
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
                    _label.Alignment = TextAlignment.Left | TextAlignment.Top;
                    _label.Update(time);
                    _picture.X = 2;
                    if (_picture.Texture != null)
                        _label.X = _picture.X + _picture.Width + 3;
                    Height = Math.Max(_label.Height, _picture.Height) + 4;
                    _picture.Y = (Height - _picture.Height) / 2;
                    _label.Y = (Height - _label.Height) / 2;

                    break;
            }
            _requiresLayout = false;
        }

        /// <inheritdoc/>
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

    /// <summary>
    /// Represents a layout style for a <see cref="ListView"/>. 
    /// </summary>
    public enum ListViewLayout
    {
        /// <summary>
        /// Text should be below the icon and the icon should be large.
        /// </summary>
        LargeIcon,
        /// <summary>
        /// Text should be beside the icon and the icon should be small.
        /// </summary>
        SmallIcon,
        /// <summary>
        /// Text should be beside the icon, the icon should be small, and the width of the item should match the list view's width.
        /// </summary>
        List
    }

    /// <summary>
    /// Represents a direction in which list view icons should flow
    /// </summary>
    public enum IconFlowDirection
    {
        /// <summary>
        /// Icons should flow horizontally
        /// </summary>
        LeftToRight,
        /// <summary>
        /// Icons should flow vertically
        /// </summary>
        TopDown,
    }
}
