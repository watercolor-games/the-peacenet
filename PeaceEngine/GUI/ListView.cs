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
        private List<ListViewItem> _items = new List<ListViewItem>();
        private Dictionary<string, Texture2D> _images = new Dictionary<string, Texture2D>();
        private IconFlowDirection _flow = IconFlowDirection.LeftToRight;
        private ListViewLayout _layout = ListViewLayout.LargeIcon;
        private int _selectedIndex = -1;
        private string _filter = null;

        private int _margin = 10;
        private int _horizontalIconPad = 5;
        private int _verticalIconPad = 7;
        private int _smallIconSize = 24;
        private int _largeIconSize = 48;
        private int _listIconSize = 20;
        private int _largeIconTextWidth = 150;


        public event EventHandler SelectedIndexChanged;
        public event Action<ListViewItem> ItemClicked;

        public ListView()
        {
            Click += (o, a) =>
            {
                var itemAtCursor = GetItemAtPoint(MouseX, MouseY);
                if(itemAtCursor==null)
                {
                    SelectedIndex = -1;
                }
                else
                {
                    SelectedIndex = _items.IndexOf(itemAtCursor);
                }
            };
            DoubleClick += (o, a) =>
            {
                var itemSelected = SelectedItem;
                var itemAtCursor = GetItemAtPoint(MouseX, MouseY);
                if(itemSelected == itemAtCursor)
                {
                    if(itemAtCursor != null)
                    {
                        ItemClicked?.Invoke(itemAtCursor);
                    }
                }
                else
                {
                    if(itemAtCursor != null)
                    {
                        SelectedIndex = _items.IndexOf(itemAtCursor);
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }
            };
        }

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        public ListViewLayout Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                _layout = value;
            }
        }

        public IconFlowDirection IconFlow
        {
            get
            {
                return _flow;
            }
            set
            {
                _flow = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                value = MathHelper.Clamp(value, -1, _items.Count-1);
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ListViewItem[] Items
        {
            get
            {
                return _items.ToArray();
            }
        }

        public ListViewItem[] VisibleItems
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_filter))
                    return Items;
                return _items.Where(x => x.Value.Contains(_filter)).ToArray();
            }
        }

        public ListViewItem SelectedItem
        {
            get
            {
                if (_selectedIndex == -1)
                    return null;
                return _items[_selectedIndex];
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            int height = 0;
            int width = 0;

            var font = Theme.GetFont(Themes.TextFontStyle.System);

            switch(_layout)
            {
                case ListViewLayout.List:
                    width = Width;
                    foreach(var item in VisibleItems)
                    {
                        var image = GetImage(item.ImageKey);
                        int maxwidth = Width - (_margin * 2);
                        if (image != null)
                            MaxWidth -= (_listIconSize + _horizontalIconPad);
                        var textMeasure = TextRenderer.MeasureText(item.Value, font, MaxWidth, TextRenderers.WrapMode.Words);
                        height += Math.Max((int)textMeasure.Y, _listIconSize);
                    }
                    break;
                case ListViewLayout.LargeIcon:
                    switch(_flow)
                    {
                        case IconFlowDirection.LeftToRight:
                            width = Width;
                            int x1 = 0;
                            int line1 = 0;
                            foreach(var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);
                                x1 += (_horizontalIconPad * 2) + _largeIconTextWidth;
                                line1 = Math.Max(line1, _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize);
                                if(x1 > Width - (_margin*2))
                                {
                                    x1 = 0;
                                    height += line1;
                                    line1 = 0;
                                }
                            }
                            if(line1>0)
                            {
                                height += line1;
                                line1 = 0;
                            }
                            break;
                        case IconFlowDirection.TopDown:
                            height = Height - (_margin * 2);
                            int y1 = 0;
                            int col1 = 0;
                            foreach (var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);
                                y1 += (_verticalIconPad) + _largeIconSize + 5 + (int)textMeasure.Y;
                                col1 = Math.Max(col1, _largeIconTextWidth + (_horizontalIconPad * 2));
                                if (y1 > height)
                                {
                                    y1 = 0;
                                    width += col1;
                                    col1 = 0;
                                }
                            }
                            if(col1>0)
                            {
                                width += col1;
                                col1 = 0;
                            }
                            break;
                    }
                    break;
            }

            Width = width;
            Height = height + (_margin * 2);
            base.OnUpdate(time);
        }

        private ListViewItem GetItemAtPoint(int x, int y)
        {
            int h = _margin;
            int v = _margin;

            var font = Theme.GetFont(Themes.TextFontStyle.System);

            switch (_layout)
            {
                case ListViewLayout.List:
                    if (x < _margin)
                        return null;
                    if (x > Width - _margin)
                        return null;
                    foreach (var item in VisibleItems)
                    {
                        var image = GetImage(item.ImageKey);
                        int maxwidth = Width - (_margin * 2);
                        if (image != null)
                            MaxWidth -= (_listIconSize + _horizontalIconPad);
                        var textMeasure = TextRenderer.MeasureText(item.Value, font, MaxWidth, TextRenderers.WrapMode.Words);
                        int height = Math.Max((int)textMeasure.Y, _listIconSize);
                        if (y >= v && y <= v + height)
                            return item;
                        v += height;
                    }
                    break;
                case ListViewLayout.LargeIcon:
                    switch (_flow)
                    {
                        case IconFlowDirection.LeftToRight:
                            int line1 = 0;
                            foreach (var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);

                                if (h + (_horizontalIconPad * 2) + _largeIconTextWidth > Width - (_margin * 2))
                                {
                                    h = 0;
                                    v += line1;
                                    line1 = 0;
                                }

                                if (x >= h && x <= h + (_horizontalIconPad*2) + _largeIconTextWidth)
                                {
                                    if (y >= v && y <= v + _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize)
                                        return item;
                                }
                                h += (_horizontalIconPad * 2) + _largeIconTextWidth;
                                line1 = Math.Max(line1, _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize);
                                
                            }
                            break;
                        case IconFlowDirection.TopDown:
                            int col1 = 0;
                            foreach (var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);

                                if (v + (_verticalIconPad) + _largeIconSize + 5 + (int)textMeasure.Y > Height - (_margin * 2))
                                {
                                    v = 0;
                                    h += col1;
                                    col1 = 0;
                                }

                                if (x >= h && x <= h + (_horizontalIconPad * 2) + _largeIconTextWidth)
                                {
                                    if (y >= v && y <= v + _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize)
                                        return item;
                                }

                                v += (_verticalIconPad) + _largeIconSize + 5 + (int)textMeasure.Y;
                                col1 = Math.Max(col1, _largeIconTextWidth + (_horizontalIconPad * 2));
                                
                            }
                            break;
                    }
                    break;
            }
            return null;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            int x = _margin;
            int y = _margin;

            var font = Theme.GetFont(Themes.TextFontStyle.System);

            switch (_layout)
            {
                case ListViewLayout.List:
                    foreach (var item in VisibleItems)
                    {
                        var image = GetImage(item.ImageKey);
                        int maxwidth = Width - (_margin * 2);
                        if (image != null)
                            MaxWidth -= (_listIconSize + _horizontalIconPad);
                        var textMeasure = TextRenderer.MeasureText(item.Value, font, MaxWidth, TextRenderers.WrapMode.Words);

                        int height = (image == null) ? (int)textMeasure.Y : Math.Max((int)textMeasure.Y, _listIconSize);

                        if (item == SelectedItem)
                        {
                            gfx.DrawRectangle(x, y, Width - (_margin*2), height, Theme.GetAccentColor());
                        }
                        if (image == null)
                        {
                            gfx.DrawString(item.Value, x, y + ((height - (int)textMeasure.Y)/2), Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Left, Width - (_margin * 2), TextRenderers.WrapMode.Words);
                        }
                        else
                        {
                            gfx.DrawRectangle(x, y + ((height - _listIconSize) / 2), _listIconSize, _listIconSize, image);
                            gfx.DrawString(item.Value, x + _listIconSize + _horizontalIconPad, y + ((height - (int)textMeasure.Y) / 2), Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Left, maxwidth, TextRenderers.WrapMode.Words);
                        }
                        y += height;
                    }
                    break;
                case ListViewLayout.LargeIcon:
                    switch (_flow)
                    {
                        case IconFlowDirection.LeftToRight:
                            int ln = 0;
                            foreach (var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);
                                var image = GetImage(item.ImageKey);
                                int width = Math.Max((int)textMeasure.X, _largeIconSize);

                                if (x + (_horizontalIconPad * 2) + _largeIconTextWidth > Width - (_margin * 2))
                                {
                                    x = _margin;
                                    y += ln;
                                    ln = 0;
                                }

                                if (image != null)
                                {
                                    gfx.DrawRectangle(x + _horizontalIconPad + ((_largeIconTextWidth - _largeIconSize) / 2), y + _verticalIconPad, _largeIconSize, _largeIconSize, image, (item == SelectedItem) ? Theme.GetAccentColor() : Color.White);
                                }

                                if (item == SelectedItem)
                                {
                                    gfx.DrawRectangle(x + (_horizontalIconPad / 2), y + (_verticalIconPad + _largeIconSize + 5), _largeIconTextWidth + _horizontalIconPad, (int)textMeasure.Y, Theme.GetAccentColor());
                                }

                                gfx.DrawString(item.Value, x + _horizontalIconPad, y + _verticalIconPad + _largeIconSize + 5, Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Center, _largeIconTextWidth, TextRenderers.WrapMode.Words);

                                x += (_horizontalIconPad * 2) + _largeIconTextWidth;
                                ln = Math.Max(ln, _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize);
                            }
                            break;
                        case IconFlowDirection.TopDown:
                            int col = 0;
                            foreach(var item in VisibleItems)
                            {
                                var textMeasure = TextRenderer.MeasureText(item.Value, font, _largeIconTextWidth, TextRenderers.WrapMode.Words);
                                var image = GetImage(item.ImageKey);

                                if (y + _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize > Height - (_margin * 2))
                                {
                                    y = _margin;
                                    x += col;
                                    col = 0;
                                }

                                if (image != null)
                                {
                                    gfx.DrawRectangle(x + _horizontalIconPad + ((_largeIconTextWidth - _largeIconSize) / 2), y + _verticalIconPad, _largeIconSize, _largeIconSize, image, (item == SelectedItem) ? Theme.GetAccentColor() : Color.White);
                                }

                                if (item == SelectedItem)
                                {
                                    gfx.DrawRectangle(x + (_horizontalIconPad / 2), y + (_verticalIconPad + _largeIconSize + 5), _largeIconTextWidth + _horizontalIconPad, (int)textMeasure.Y, Theme.GetAccentColor());
                                }

                                gfx.DrawString(item.Value, x + _horizontalIconPad, y + _verticalIconPad + _largeIconSize + 5, Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Center, _largeIconTextWidth, TextRenderers.WrapMode.Words);

                                y += _verticalIconPad + (int)textMeasure.Y + 5 + _largeIconSize;
                                col = Math.Max(col, (_horizontalIconPad * 2) + _largeIconTextWidth);
                                
                            }
                            break;
                    }
                    break;

            }


        }

        public void SetImage(string key, Texture2D value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (_images.ContainsKey(key))
                if (value == null)
                    _images.Remove(key);
                else
                    _images[key] = value;
            else
            {
                if (value == null)
                    return;
                _images.Add(key, value);
            }
        }

        public Texture2D GetImage(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (!_images.ContainsKey(key))
                return null;
            return _images[key];
        }

        public void AddItem(ListViewItem item)
        {
            if (item == null)
                return;
            if (_items.Contains(item))
                return;
            _items.Add(item);
            SelectedIndex = -1;
        }

        public void RemoveItem(ListViewItem item)
        {
            if (item == null)
                return;
            if (!_items.Contains(item))
                return;
            _items.Remove(item);
            SelectedIndex = -1;
        }

        public void ClearItems()
        {
            _items.Clear();
            SelectedIndex = -1;
        }

        [Obsolete("Please use ListView.Items instead.")]
        public ListViewItem[] GetItems()
        {
            return Items;
        }

        [Obsolete("Please use ListView.Filter instead.")]
        public void SetFilter(string filter)
        {
            Filter = filter;
        }
    }

    public class ListViewItem
    {
        public string Value { get; set; }
        public string ImageKey { get; set; }
        public object Tag { get; set; }
    }

    /// <summary>
    /// A control capable of holding a list of items and displaying them in various different views.
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
