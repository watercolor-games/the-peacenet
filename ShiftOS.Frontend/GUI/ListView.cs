﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.GUI
{
    public class ListView : Control
    {
        private List<ListViewItem> _items = null;
        private Dictionary<string, Texture2D> _images = null;
        private const int _itemimagemargin = 15;
        private const int _initialmargin = 20;
        private const int _itemgap = 5;
        private int scroll = 0;
        private const int defaulttexturesize = 42;
        private int _selected = -1;

        public ListView()
        {
            _items = new List<ListViewItem>();
            _images = new Dictionary<string, Texture2D>();
            Click += () =>
            {
                using (var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                {
                    int _itemx = _initialmargin;
                    int _itemy = _initialmargin - scroll;
                    int yhelper = 0;
                    foreach (var item in _items)
                    {
                        Texture2D image = null;
                        int texwidth = defaulttexturesize;
                        int texheight = defaulttexturesize;
                        if (_images.ContainsKey(item.ImageKey))
                        {
                            texwidth = _images[item.ImageKey].Width;
                            texheight = _images[item.ImageKey].Height;
                            image = _images[item.ImageKey];
                        }
                        int textwidth = texwidth + (_itemimagemargin * 2);
                        var textmeasure = gfx.MeasureString(item.Text, LoadedSkin.MainFont, textwidth);
                        yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Height);

                        int texty = _itemy + texheight;
                        int textx = _itemx + ((textwidth - (int)textmeasure.Width) / 2);

                        if(MouseX >= _itemx && MouseX <= _itemx + textwidth)
                        {
                            if(MouseY >= _itemy && MouseY <= _itemy + texheight + (int)textmeasure.Height)
                            {
                                _selected = _items.IndexOf(item);
                                Invalidate();
                                return;
                            }
                        }

                        _itemx += textwidth + _itemgap;
                        if (_itemx >= (Width - (_initialmargin * 2)))
                        {
                            _itemx = _initialmargin;
                            _itemy += yhelper;
                        }
                    }
                }
                _selected = -1;
                Invalidate();
            };
        }

        public int SelectedIndex
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value == _selected)
                    return;
                _selected = MathHelper.Clamp(value, -1, _items.Count - 1);
                Invalidate();
            }
        }

        public ListViewItem SelectedItem
        {
            get
            {
                if (_selected == -1)
                    return null;
                return _items[_selected];
            }
        }

        public void ClearItems()
        {
            _items.Clear();
            scroll = 0;
            _selected = -1;
            Invalidate();
        }

        public void RemoveItem(ListViewItem item)
        {
            if (!_items.Contains(item))
                throw new ArgumentException("This list view doesn't contain that item.");
            if (_selected == _items.IndexOf(item))
                _selected = -1;
            _items.Remove(item);
            Invalidate();
        }

        public void AddItem(ListViewItem item)
        {
            if (_items.Contains(item))
                throw new ArgumentException("Item already exists in this listview.");
            _items.Add(item);
            Invalidate();
        }

        public void SetImage(string key, Texture2D value)
        {
            if (_images.ContainsKey(key))
                _images[key] = value;
            else
                _images.Add(key, value);
            Invalidate();
        }

        public Texture2D GetImage(string key)
        {
            if (_images.ContainsKey(key))
                return _images[key];
            return null;
        }

        public void ClearImages()
        {
            _images.Clear();
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.Clear(LoadedSkin.ControlColor.ToMonoColor());
            int _itemx = _initialmargin;
            int _itemy = _initialmargin - scroll;
            int yhelper = 0;
            foreach (var item in _items)
            {
                Texture2D image = null;
                int texwidth = defaulttexturesize;
                int texheight = defaulttexturesize;
                if (_images.ContainsKey(item.ImageKey))
                {
                    texwidth = _images[item.ImageKey].Width;
                    texheight = _images[item.ImageKey].Height;
                    image = _images[item.ImageKey];
                }
                int textwidth = texwidth + (_itemimagemargin * 2);
                var textmeasure = gfx.MeasureString(item.Text, LoadedSkin.MainFont, textwidth);
                yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);

                if(image != null)
                {
                    int imageDrawX = _itemx + ((textwidth - texwidth) / 2);
                    gfx.DrawRectangle(imageDrawX, _itemy, texwidth, texheight, image);
                }

                int texty = _itemy + texheight;
                int textx = _itemx + ((textwidth - (int)textmeasure.X) / 2);
                if(_items.IndexOf(item) == _selected)
                {
                    gfx.DrawRectangle(textx, texty, (int)textmeasure.X, (int)textmeasure.Y, LoadedSkin.ButtonPressedColor.ToMonoColor());
                }
                gfx.DrawString(item.Text, textx, texty, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.MainFont, textwidth);
                _itemx += textwidth + _itemgap;
                if(_itemx >= (Width - (_initialmargin * 2)))
                {
                    _itemx = _initialmargin;
                    _itemy += yhelper;
                }
            }
        }
    }

    public class ListViewItem
    {
        public string Text { get; set; }
        public string Tag { get; set; }
        public string ImageKey { get; set; }

    }
}
