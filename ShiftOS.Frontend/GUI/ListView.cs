﻿using System;
        {
            int _itemx = _initialmargin;
        }
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
                    var textmeasure = GraphicsContext.MeasureString(item.Text, LoadedSkin.MainFont, textwidth);
                    yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);

                    int texty = _itemy + texheight;
                    int textx = _itemx + ((textwidth - (int)textmeasure.X) / 2);

                    if (MouseX >= _itemx && MouseX <= _itemx + textwidth)
                    {
                        if (MouseY >= _itemy && MouseY <= _itemy + texheight + (int)textmeasure.Y)
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
            RequireTextRerender();
            RequireTextRerender();
            base.OnPaint(gfx, target);