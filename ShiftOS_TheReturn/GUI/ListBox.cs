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
    public class ListBox : GUI.TextControl
    {
        private int fontheight = 0;
        private List<object> items = new List<object>();
        private int selectedIndex = -1;
        private int _itemOver = -1;

        protected override void RenderText(GraphicsContext gfx)
        {
            for (int i = 0; i < items.Count; i++)
            {
                int x = 1;
                int y = fontheight * i;
                if (i >= 0 && i < items.Count)
                {
                    var state = Theming.ButtonState.Idle;
                    if (i == _itemOver)
                    {
                        state = Theming.ButtonState.MouseHover;

                    }
                    else if (i == selectedIndex)
                    {
                        state = Theming.ButtonState.MouseDown;
                    }
                    Theming.ThemeManager.Theme.DrawStatedString(gfx, items[i].ToString(), x, y + 2, int.MaxValue, int.MaxValue, TextControlFontStyle.System, state);
                }
            }

        }

        public ListBox()
        {
            fontheight = (int)Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, "Measure test.").Y;
            MouseMove += (loc) =>
            {
                if (fontheight <= 0)
                    return;
                int screeni = (loc.Y / fontheight);
                int i = screeni;
                if (_itemOver == i)
                    return;
                _itemOver = i;
                Invalidate();
            };
            Click += () =>
            {
                //loop through the list of items on the screen
                for(int i = 0; i < items.Count; i++)
                {
                    int screeni = i;
                    int loc = 1+screeni * fontheight;
                    int height = 1+(screeni + 1) * fontheight;
                    if(MouseY >= loc && MouseY <= height)
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
                selectedIndex = -1;
                Invalidate();
                RequireTextRerender();
            };
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
            RequireTextRerender();
            Invalidate();
        }

        public void RemoveItem(object item)
        {
            items.Remove(item);
            selectedIndex = -1;
            SelectedIndexChanged?.Invoke();
            RequireTextRerender();
            Invalidate();
        }

        protected override void OnKeyEvent(KeyEvent e)
        {
            if(e.Key== Microsoft.Xna.Framework.Input.Keys.Down)
            {
                if(selectedIndex < items.Count - 1)
                {
                    selectedIndex++;
                    SelectedIndexChanged?.Invoke();
                    Invalidate();
                }
            }
            else if(e.Key == Microsoft.Xna.Framework.Input.Keys.Up)
            {
                if(selectedIndex > 0)
                {
                    selectedIndex--;
                    SelectedIndexChanged?.Invoke();
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            Theming.ThemeManager.Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            var accent = Theming.ThemeManager.Theme.GetAccentColor();
            for (int i = 0; i < items.Count; i++)
            {
                int x = 1;
                int y = fontheight * i;
                int width = Width - 2;
                int height = fontheight;
                
                if (i == selectedIndex)
                {
                    //draw the string as selected
                    gfx.DrawRectangle(x, y + 2, width, height, accent);
                }
                else if (i == _itemOver)
                {
                    gfx.DrawRectangle(x, y + 2, width, height, accent*0.75F);
                }
            }
            base.OnPaint(gfx, target);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            TextColor = Color.White;
            if (AutoSize)
            {
                Height = items.Count * fontheight;
            }
            
            base.OnLayout(gameTime);
        }

        public event Action SelectedIndexChanged;
    }
}
