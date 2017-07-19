using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.GUI
{
    public class ListBox : GUI.Control
    {
        private int fontheight = 0;
        private List<object> items = new List<object>();
        private int selectedIndex = -1;
        private int itemOffset = 0;
        private int itemsPerPage = 1;

        public ListBox()
        {
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
            };
        }


        public int SelectedIndex
        {
            get
            {
                return MathHelper.Clamp(selectedIndex, 0, items.Count - 1);
            }
            set
            {
                selectedIndex = MathHelper.Clamp(value, 0, items.Count - 1);
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
                    return "";
                }
            }
        }

        public void ClearItems()
        {
            selectedIndex = -1;
            items.Clear();
            SelectedIndexChanged?.Invoke();
            Invalidate();
        }

        public void AddItem(object item)
        {
            items.Add(item);
            RecalculateItemsPerPage();
            Invalidate();
        }

        public void RemoveItem(object item)
        {
            items.Remove(item);
            selectedIndex = -1;
            RecalculateItemsPerPage();
            SelectedIndexChanged?.Invoke();
            Invalidate();
        }

        public void RecalculateItemsPerPage()
        {
            itemsPerPage = 0;
           while(itemsPerPage * fontheight < Height && itemsPerPage < items.Count)
            {
                itemsPerPage++;
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
                }
                while(this.itemOffset > selectedIndex)
                {
                    itemOffset--;
                }
                while(this.itemOffset + itemsPerPage < selectedIndex)
                {
                    itemOffset++;
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

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.Clear(LoadedSkin.ControlTextColor.ToMonoColor());
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);
            for(int i = itemOffset; i < items.Count && i < itemsPerPage; i++)
            {
                int x = 1;
                int y = fontheight * (i - itemOffset);
                int width = Width - 2;
                int height = fontheight;
                if(i == selectedIndex)
                {
                    //draw the string as selected
                    gfx.DrawRectangle(x, y+2, width, height, UIManager.SkinTextures["ControlTextColor"]);
                    gfx.DrawString(items[i].ToString(), x, y+2, LoadedSkin.ControlColor.ToMonoColor(), LoadedSkin.MainFont);
                }
                else
                {
                    gfx.DrawRectangle(x, y+2, width, height, UIManager.SkinTextures["ControlColor"]);
                    gfx.DrawString(items[i].ToString(), x, y+2, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.MainFont);

                }
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if(fontheight != LoadedSkin.MainFont.Height)
            {
                fontheight = LoadedSkin.MainFont.Height;
                Invalidate();
            }
            base.OnLayout(gameTime);
        }

        public event Action SelectedIndexChanged;
    }
}
