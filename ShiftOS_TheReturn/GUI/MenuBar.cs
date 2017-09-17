﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
{
    public class MenuBar : Menu
    {
        private int _fontHeight = 0;

        public MenuBar() : base()
        {

        }

        protected override void OnClick()
        {
            if (SelectedIndex != -1)
            {
                var item = MenuItems[SelectedIndex];
                if (item.HasDropdown)
                {
                    int x = SelectedX;
                    int y = Height;
                    var pt = PointToScreen(x, y);
                    SetDropdown(pt.X, pt.Y, item);
                }
                else
                {
                    item.Activate();
                }
            }
        }

        protected override void CalculateSelectedItem(int x, int y)
        {
            if (x >= SelectedX && x <= SelectedX + SelectedW && y >= SelectedY && y <= SelectedY + SelectedH)
                return;
            int rx = (Border / 2)+10;
            int ry = (Height-_fontHeight)/2;
            for(int i = 0; i < MenuItems.Length; i++)
            {
                var loc = GraphicsContext.MeasureString(MenuItems[i].Text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft);
                int w = (int)loc.X+6;
                int h = _fontHeight;
                if(x >= rx && x <= rx+w && y >= ry && y <= ry + h)
                {
                    Select(i, rx, ry, w, h);
                    return;
                }
                rx += w + 10;
            }
            Select(-1, 0, 0, 0, 0);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _fontHeight = SkinEngine.LoadedSkin.MainFont.Height + 6;
            this.Width = Parent.Width;
            this.Height = _fontHeight + 6;
            this.X = 0;
            this.Y = 0;
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int text_x = (Border / 2)+10;
            int text_y = (Height-_fontHeight)/2;
            if (MenuItems.Length > 0)
            {
                for (int i = 0; i < MenuItems.Length; i++)
                {
                    var measure = GraphicsContext.MeasureString(MenuItems[i].Text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft);
                    bool selected = i == SelectedIndex;
                    Color _text = SkinEngine.LoadedSkin.Menu_TextColor.ToMonoColor();
                    if (selected)
                        _text = SkinEngine.LoadedSkin.Menu_SelectedTextColor.ToMonoColor();

                    int t_y = text_y + ((_fontHeight - (int)measure.Y) / 2);
                    int t_x = text_x + 3;
                    gfx.DrawString(MenuItems[i].Text, t_x, t_y, _text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft);
                    text_x += (int)measure.X + 16;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(SkinEngine.LoadedSkin.Menu_MenuStripGradientBegin.ToMonoColor());
            gfx.DrawRectangle(SelectedX, SelectedY, SelectedW, SelectedH, SkinEngine.LoadedSkin.Menu_MenuItemBorder.ToMonoColor());
            gfx.DrawRectangle(SelectedX+2, SelectedY+2, SelectedW-4, SelectedH-4, SkinEngine.LoadedSkin.Menu_MenuItemSelected.ToMonoColor());

            base.PaintBG = false;
            base.OnPaint(gfx, target);
        }

    }
}
