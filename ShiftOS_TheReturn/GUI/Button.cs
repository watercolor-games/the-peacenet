using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
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
            AutoSize = true;
            Font = SkinEngine.LoadedSkin.ButtonFont;
            if (AutoSize == true)
            {
                base.OnLayout(gameTime);
                if (TextRerenderRequired == true)
                {
                    Width += (SkinEngine.LoadedSkin.ButtonMargins.Width*2);
                    Height += (SkinEngine.LoadedSkin.ButtonMargins.Height * 2);
                }

            }
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            var bgCol = SkinEngine.LoadedSkin.ButtonIdleColor.ToMonoColor();
            var fgCol = SkinEngine.LoadedSkin.ButtonIdleTextColor.ToMonoColor();
            if (ContainsMouse)
            {
                bgCol = SkinEngine.LoadedSkin.ButtonHoverColor.ToMonoColor();
                fgCol = SkinEngine.LoadedSkin.ButtonHoverTextColor.ToMonoColor();

            }
            if (MouseLeftDown)
            {
                bgCol = SkinEngine.LoadedSkin.ButtonPressedColor.ToMonoColor();
                fgCol = SkinEngine.LoadedSkin.ButtonPressedTextColor.ToMonoColor();
            }
            var measure = GraphicsContext.MeasureString(Text, Font, Engine.GUI.TextAlignment.Middle);

            var loc = new Vector2((Width - measure.X) / 2, (Height - measure.Y) / 2);

            gfx.DrawString(Text, (int)loc.X, (int)loc.Y, fgCol, Font, Engine.GUI.TextAlignment.Middle);

        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            var bgCol = SkinEngine.LoadedSkin.ButtonIdleColor.ToMonoColor();
            var fgCol = SkinEngine.LoadedSkin.ButtonIdleTextColor.ToMonoColor();
            if (ContainsMouse)
            {
                bgCol = SkinEngine.LoadedSkin.ButtonHoverColor.ToMonoColor();
                fgCol = SkinEngine.LoadedSkin.ButtonHoverTextColor.ToMonoColor();

            }
            if (MouseLeftDown)
            {
                bgCol = SkinEngine.LoadedSkin.ButtonPressedColor.ToMonoColor();
                fgCol = SkinEngine.LoadedSkin.ButtonPressedTextColor.ToMonoColor();
            }
            gfx.DrawRectangle(0, 0, Width, Height, bgCol);

            base.OnPaint(gfx, target);
        }
    }
}
