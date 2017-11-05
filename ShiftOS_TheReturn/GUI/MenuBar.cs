using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class MenuBar : Menu
    {
        private int _fontHeight = 0;

        public MenuBar() : base()
        {
            _fontHeight = (int)Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, "I am the bread of the sweetness.").Y + 6;
        }

        protected override void OnClick()
        {
            if (SelectedIndex != -1)
            {
                var item = MenuItems[SelectedIndex];
                if (item.Enabled == false)
                    return;
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
                if (MenuItems[i].Enabled == false)
                    continue;
                var loc = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, MenuItems[i].Text);
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
            FontStyle = TextControlFontStyle.Custom;
            TextColor = Color.White;
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
                    if (MenuItems[i].Enabled == false)
                        continue;
                    var measure = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, MenuItems[i].Text);
                    bool selected = i == SelectedIndex;
                    var _text = Theming.ButtonState.Idle;
                    if (selected)
                        _text = Theming.ButtonState.MouseHover;

                    int t_y = text_y + ((_fontHeight - (int)measure.Y) / 2);
                    int t_x = text_x + 3;
                    Theming.ThemeManager.Theme.DrawStatedString(gfx, MenuItems[i].Text, t_x, t_y, (int)measure.X, (int)measure.Y, TextControlFontStyle.System, _text);
                    text_x += (int)measure.X + 16;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            Theming.ThemeManager.Theme.DrawControlLightBG(gfx, 0, 0, Width, Height);
            var accent = Theming.ThemeManager.Theme.GetAccentColor();
            gfx.DrawRectangle(SelectedX, SelectedY, SelectedW, SelectedH, accent);
            
            base.PaintBG = false;
            base.OnPaint(gfx, target);
        }

    }
}
