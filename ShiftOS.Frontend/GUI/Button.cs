using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

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
                    Width = borderwidth + (int)measure.Width + 16;
                    Height = borderwidth + (int)measure.Height + 12;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            var bgCol = SkinEngine.LoadedSkin.ButtonBackgroundColor.ToMonoColor();
            var fgCol = SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor();
            if (ContainsMouse)
                bgCol = SkinEngine.LoadedSkin.ButtonHoverColor.ToMonoColor();
            if (MouseLeftDown)
                bgCol = SkinEngine.LoadedSkin.ButtonPressedColor.ToMonoColor();

            base.OnPaint(gfx);
            base.OnPaint(gfx);
        }
    }
}
