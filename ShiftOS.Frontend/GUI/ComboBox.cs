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
    public class ComboBox : TextControl
    {
        private List<object> items = null;
        private int _selectedIndex = -1;

        public event Action SelectedItemChanged;

        public object SelectedItem
        {
            get
            {
                if (_selectedIndex == -1)
                    return null;
                return items[_selectedIndex];
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
                if (value == _selectedIndex)
                    return;
                _selectedIndex = MathHelper.Clamp(value, -1, items.Count);
                SelectedItemChanged?.Invoke();
            }
        }

        public void AddItem(object item)
        {
            items.Add(item);
            Invalidate();
        }

        public void ClearItems()
        {
            items.Clear();
            SelectedIndex = -1;
            Invalidate();
        }

        public void RemoveItem(object item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                SelectedIndex = MathHelper.Clamp(_selectedIndex, 0, items.Count);
                Invalidate();
            }
        }

        public ComboBox() : base()
        {
            items = new List<object>();
            MinWidth = 175;
            MinHeight = 24;

        }

        protected override void RenderText(GraphicsContext gfx)
        {
            var measure = GraphicsContext.MeasureString(Text, Font);
            gfx.DrawString(Text, 2, (Height - (int)measure.Y) / 2, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), Font);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (items.Count == 0 || _selectedIndex == -1)
                Text = "";
            else
            {
                SelectedIndex = MathHelper.Clamp(_selectedIndex, 0, items.Count);
                Text = items[_selectedIndex].ToString();
            }

            if (AutoSize)
            {
                var textmeasure = GraphicsContext.MeasureString(Text, Font);
                int padding_w = 4;
                int padding_h = 4;
                int sidebar_width = 25;
                Width = padding_w + (int)textmeasure.X + sidebar_width;
                Height = padding_h + (int)textmeasure.Y;
            }
        }



        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, SkinEngine.LoadedSkin.ControlColor.ToMonoColor());
            base.OnPaint(gfx, target);

            //draw separator
            int sepstart = Width - 26;
            gfx.DrawRectangle(sepstart, 0, 1, Height, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());

            //draw button
            var buttonbg = SkinEngine.LoadedSkin.ButtonBackgroundColor.ToMonoColor();
            if (ContainsMouse)
                buttonbg = SkinEngine.LoadedSkin.ButtonHoverColor.ToMonoColor();
            if (MouseLeftDown)
                buttonbg = SkinEngine.LoadedSkin.ButtonPressedColor.ToMonoColor();
            gfx.DrawRectangle(sepstart + 1, 1, (Width - sepstart) - 2, Height - 2, buttonbg);

        }
    }
}
