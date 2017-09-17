using System;
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
            int rx = (Border / 2);
            int ry = (Border / 2);
            for(int i = 0; i < MenuItems.Length; i++)
            {
                var loc = GraphicsContext.MeasureString(MenuItems[i].Text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft);
                int w = (int)loc.X;
                int h = (int)loc.Y;
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
            this.Width = Parent.Width;
            this.Height = 24;
            this.X = 0;
            this.Y = 0;
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int text_x = (Border / 2);
            int text_y = (Border / 2);
            if (MenuItems.Length > 0)
            {
                for (int i = 0; i < MenuItems.Length; i++)
                {
                    bool selected = i == SelectedIndex;
                    Color _text = SkinEngine.LoadedSkin.Menu_TextColor.ToMonoColor();
                    if (selected)
                        _text = SkinEngine.LoadedSkin.Menu_SelectedTextColor.ToMonoColor();
                    gfx.DrawString(MenuItems[i].Text, text_x, text_y, _text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft);
                    text_x += (int)GraphicsContext.MeasureString(MenuItems[i].Text, SkinEngine.LoadedSkin.MainFont, Engine.GUI.TextAlignment.TopLeft).X + 10;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(SkinEngine.LoadedSkin.Menu_MenuStripGradientBegin.ToMonoColor());
            gfx.DrawRectangle(SelectedX, SelectedY, SelectedW, SelectedH, SkinEngine.LoadedSkin.Menu_MenuItemBorder.ToMonoColor());
            gfx.DrawRectangle(SelectedX+2, SelectedY+2, SelectedW-4, SelectedH-4, SkinEngine.LoadedSkin.Menu_MenuItemSelected.ToMonoColor());

            base.OnPaint(gfx, target);
        }

    }
}
