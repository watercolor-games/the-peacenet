using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Frontend.GUI
{
    public class Button : TextControl
    {
        public Button()
        {
            TextAlign = TextAlign.MiddleCenter;
            Text = "Click me!";
        }

        protected override void OnLayout()
        {
            if(AutoSize == true)
            {
                int borderwidth = SkinEngine.LoadedSkin.ButtonBorderWidth * 2;

                using (var gfx = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    var measure = gfx.MeasureString(this.Text, this.Font);
                    Width = borderwidth + (int)measure.Width + 4;
                    Height = borderwidth + (int)measure.Height + 8;
                }
            }
            base.OnLayout();
        }

        public override void Paint(Graphics gfx)
        {
            Color bgCol = SkinEngine.LoadedSkin.ButtonBackgroundColor;
            Color fgCol = SkinEngine.LoadedSkin.ControlTextColor;
            if (ContainsMouse)
                bgCol = SkinEngine.LoadedSkin.ButtonHoverColor;
            if (MouseLeftDown)
                bgCol = SkinEngine.LoadedSkin.ButtonPressedColor;

            gfx.Clear(bgCol);
            gfx.DrawRectangle(new Pen(new SolidBrush(fgCol), SkinEngine.LoadedSkin.ButtonBorderWidth), new Rectangle(0, 0, Width, Height));
            base.Paint(gfx);

        }
    }
}
