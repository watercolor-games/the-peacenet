using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.GUI
{
    public class ListBox : GUI.TextControl
    {
        private int fontheight = 0;
        private List<object> items = new List<object>();
        private int selectedIndex = -1;
        private int itemOffset = 0;
        private int itemsPerPage = 1;

        private int _itemOver = -1;

        protected override void RenderText(GraphicsContext gfx)
        {
            for (int i = itemOffset; i < items.Count && i < itemsPerPage; i++)
            {
                int x = 1;
                int y = fontheight * (i - itemOffset);
                int width = Width - 2;
                int height = fontheight;
                if(i >= 0 && i < items.Count)
                {
                    if (i == _itemOver)
                    {
                        gfx.DrawString(items[i].ToString(), x, y + 2, LoadedSkin.ListBoxHoverTextColor.ToMonoColor(), LoadedSkin.ListBoxFont, Alignment);

                    }
                    else if (i == selectedIndex)
                    {
                        gfx.DrawString(items[i].ToString(), x, y + 2, LoadedSkin.ListBoxSelectedItemTextColor.ToMonoColor(), LoadedSkin.ListBoxFont, Alignment);

                    }
                    else
                    {
                        gfx.DrawString(items[i].ToString(), x, y + 2, LoadedSkin.ListBoxTextColor.ToMonoColor(), LoadedSkin.ListBoxFont, Alignment);
                    }
                }
            }

        }

        public ListBox()
        {
            MouseMove += (loc) =>
            {
                if (fontheight <= 0)
                    return;
                int screeni = (loc.Y / fontheight);
                int i = screeni + itemOffset;
                if (_itemOver == i)
                    return;
                _itemOver = i;
                Invalidate();
            };
            Click += () =>
            {
                //loop through the list of items on the screen
                for(int i = itemOffset; i < itemOffset + itemsPerPage && i < items.Count; i++)
                {
                    int screeni = i - itemOffset;
                    int loc = 1+screeni * fontheight;
                    int height = 1+(screeni + 1) * fontheight;
                    if(MouseY >= loc && MouseY <= height)
                    {
                        SelectedIndex = i;
                        RecalculateItemsPerPage();
                        return;
                    }
                }
                selectedIndex = -1;
                Invalidate();
                RequireTextRerender();
            };
        }


        protected override void OnMouseScroll(int value)
        {
            if (this.itemOffset - (value / fontheight) < 0)
                return;
            if (this.itemOffset - (value / fontheight) > items.Count - itemsPerPage)
                return;
            if (this.itemOffset - (value / fontheight) == itemOffset)
                return;
            itemOffset -= value / fontheight;
            RequireTextRerender();
            Invalidate();
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = MathHelper.Clamp(value, -1, items.Count - 1);
                RecalculateItemsPerPage();
                SelectedIndexChanged?.Invoke();
            }
        }

        public object SelectedItem
        {
            get
            {
                try
                {
                    return items[SelectedIndex];
                }
                catch
                {
                    return null;
                }
            }
        }

        public void ClearItems()
        {
            selectedIndex = -1;
            items.Clear();
            SelectedIndexChanged?.Invoke();
            RequireTextRerender();
            Invalidate();
        }

        public void AddItem(object item)
        {
            items.Add(item);
            RecalculateItemsPerPage();
            RequireTextRerender();
            Invalidate();
        }

        public void RemoveItem(object item)
        {
            items.Remove(item);
            selectedIndex = -1;
            RecalculateItemsPerPage();
            SelectedIndexChanged?.Invoke();
            RequireTextRerender();
            Invalidate();
        }

        public void RecalculateItemsPerPage()
        {
            itemsPerPage = 0;
           while(itemsPerPage * fontheight < Height && itemsPerPage < items.Count)
            {
                itemsPerPage++;
                RequireTextRerender();
            }
           //We have the amount of items we can fit on screen.
           //Now let's calculate the offset based on this, as well
           //as the currently selected item.
           //of course, if there IS one.
           if(selectedIndex > -1)
            {
                if(selectedIndex >= items.Count)
                {
                    selectedIndex = items.Count - 1;
                    RequireTextRerender();
                }
                while (this.itemOffset > selectedIndex)
                {
                    itemOffset--;
                    RequireTextRerender();
                }
                while (this.itemOffset + itemsPerPage < selectedIndex )
                {
                    itemOffset++;
                    RequireTextRerender();
                }
            }
        }

        protected override void OnKeyEvent(KeyEvent e)
        {
            if(e.Key== Microsoft.Xna.Framework.Input.Keys.Down)
            {
                if(selectedIndex < items.Count - 1)
                {
                    selectedIndex++;
                    RecalculateItemsPerPage();
                    SelectedIndexChanged?.Invoke();
                    Invalidate();
                }
            }
            else if(e.Key == Microsoft.Xna.Framework.Input.Keys.Up)
            {
                if(selectedIndex > 0)
                {
                    selectedIndex--;
                    RecalculateItemsPerPage();
                    SelectedIndexChanged?.Invoke();
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(LoadedSkin.InsetBackgroundColor.ToMonoColor());
            for (int i = itemOffset; i < items.Count && i < itemsPerPage; i++)
            {
                int x = 1;
                int y = fontheight * (i - itemOffset);
                int width = Width - 2;
                int height = fontheight;
                if (i == selectedIndex)
                {
                    //draw the string as selected
                    gfx.DrawRectangle(x, y + 2, width, height, LoadedSkin.ListBoxSelectedItemColor.ToMonoColor());
                }
                else if (i == _itemOver)
                {
                    gfx.DrawRectangle(x, y + 2, width, height, LoadedSkin.ListBoxHoverItemColor.ToMonoColor());
                }
            }
            base.OnPaint(gfx, target);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            FontStyle = TextControlFontStyle.Custom;
            Font = SkinEngine.LoadedSkin.ListBoxFont;
            TextColor = Color.White;

            if(fontheight != LoadedSkin.ListBoxFont.Height)
            {
                fontheight = LoadedSkin.ListBoxFont.Height;
                Invalidate();
            }
            base.OnLayout(gameTime);
        }

        public event Action SelectedIndexChanged;
    }
}
