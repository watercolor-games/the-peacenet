using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

        protected override void OnLayout(GameTime gameTime)
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
            var bgCol = UIManager.SkinTextures["ButtonBackgroundColor"];
            var fgCol = SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor();
            if (ContainsMouse)
                bgCol = UIManager.SkinTextures["ButtonHoverColor"];
            if (MouseLeftDown)
                bgCol = UIManager.SkinTextures["ButtonPressedColor"];

            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, bgCol);

            var measure = gfx.MeasureString(Text, Font);

            var loc = new Vector2((Width - measure.X) / 2, (Height - measure.Y) / 2);

            gfx.DrawString(Text, (int)loc.X, (int)loc.Y, fgCol, Font);

        }
    }
}
