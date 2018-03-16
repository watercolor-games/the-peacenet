using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.GUI
{
    public class ContextMenu : Window
    {
        private List<MenuItem> _items = new List<MenuItem>();

        private int _imageSize = 16;
        private int _imageMargin = 3;
        private string _empty = "<empty>";
        private int _itemGap = 2;
        private int _rightPad = 100;


        public ContextMenu(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
            Click += ContextMenu_Click;
        }

        private void ContextMenu_Click(object sender, EventArgs e)
        {
            var font = Theme.GetFont(Themes.TextFontStyle.System);
            int fontHeight = (int)font.MeasureString("#").Y;
            int textLoc = 0;
            if (_items.Count == 0)
            {
                Close();
                return;
            }
            else
            {
                foreach (var item in _items)
                {
                    if (ContainsMouse && MouseY >= textLoc && MouseY <= textLoc + fontHeight)
                    {
                        item.Click();
                        Close();
                        return;
                    }
                    textLoc += fontHeight + _itemGap;

                }
            }

        }

        public void AddItem(MenuItem item)
        {
            if (item == null)
                return;
            if (_items.Contains(item))
                return;
            _items.Add(item);
        }

        public void RemoveItem(MenuItem item)
        {
            if (item == null)
                return;
            if (!_items.Contains(item))
                return;
            _items.Remove(item);
        }

        public void ClearItems()
        {
            _items.Clear();
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlBG(gfx, 0, 0, Width, Height);
            Theme.DrawControlLightBG(gfx, 0, 0, (_imageMargin * 2) + _imageSize, Height);
            var font = Theme.GetFont(Themes.TextFontStyle.System);
            int fontHeight = (int)font.MeasureString("#").Y;
            int textLoc = 0;
            if(_items.Count==0)
            {
                if(ContainsMouse)
                {
                    gfx.Clear(Theme.GetAccentColor());
                }
                gfx.DrawString(_empty, (_imageMargin * 2) + _imageSize + 2, textLoc, Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Left);
            }
            else
            {
                foreach(var item in _items)
                {
                    if (ContainsMouse && MouseY >= textLoc && MouseY <= textLoc + fontHeight)
                    {
                        gfx.DrawRectangle(0, textLoc, Width, fontHeight, Theme.GetAccentColor());
                    }
                    if(item.Image!=null)
                    {
                        gfx.DrawRectangle(_imageMargin, textLoc + ((fontHeight - _imageSize) / 2), _imageSize, _imageSize, item.Image);
                    }
                    gfx.DrawString(item.Text, (_imageMargin * 2) + _imageSize + 2, textLoc, Theme.GetFontColor(Themes.TextFontStyle.System), font, TextAlignment.Left);
                    textLoc += fontHeight + _itemGap;

                }
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            if (!HasFocused)
                Close();

            var font = Theme.GetFont(Themes.TextFontStyle.System);

            if (_items.Count == 0)
            {
                var measure = font.MeasureString(_empty);
                Width = (_imageMargin * 2) + _imageSize + (int)measure.X + _rightPad;
                Height = (int)measure.Y;
            }
            else
            {
                var longest = _items.OrderBy(x => x.Text.Length).Last().Text;
                var longestMeasure = font.MeasureString(longest);
                Width = (_imageMargin * 2) + _imageSize + (int)longestMeasure.X + _rightPad;
                int height = 0;
                foreach (var item in _items)
                {
                    height += (int)font.MeasureString(item.Text).Y + _itemGap;
                }
                Height = height - _itemGap;
            }

            base.OnUpdate(time);
        }


    }

    public class MenuItem
    {
        public string Text { get; set; }
        public Texture2D Image { get; set; }

        public event Action Activated;

        internal void Click()
        {
            Activated?.Invoke();
        }
    }
}
