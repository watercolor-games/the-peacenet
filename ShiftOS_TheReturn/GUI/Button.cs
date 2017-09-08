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
            if (AutoSize == true)
            {
                int borderwidth = SkinEngine.LoadedSkin.ButtonBorderWidth * 2;

                base.OnLayout(gameTime);
                if (TextRerenderRequired == true)
                {
                    Width += borderwidth*4;
                    Height += borderwidth*4;
                }

            }
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            var fgCol = SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor();
            var measure = GraphicsContext.MeasureString(Text, Font, Engine.GUI.TextAlignment.Middle);

            var loc = new Vector2((Width - measure.X) / 2, (Height - measure.Y) / 2);

            gfx.DrawString(Text, (int)loc.X, (int)loc.Y, fgCol, Font, Engine.GUI.TextAlignment.Middle);

        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            var bgCol = UIManager.SkinTextures["ButtonBackgroundColor"];
            var fgCol = SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor();
            if (ContainsMouse)
                bgCol = UIManager.SkinTextures["ButtonHoverColor"];
            if (MouseLeftDown)
                bgCol = UIManager.SkinTextures["ButtonPressedColor"];

            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            gfx.DrawRectangle(SkinEngine.LoadedSkin.ButtonBorderWidth, SkinEngine.LoadedSkin.ButtonBorderWidth, Width - (SkinEngine.LoadedSkin.ButtonBorderWidth * 2), Height - (SkinEngine.LoadedSkin.ButtonBorderWidth * 2), bgCol);

            base.OnPaint(gfx, target);
        }
    }
}
