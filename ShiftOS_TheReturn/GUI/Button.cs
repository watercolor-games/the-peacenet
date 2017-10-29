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
        private Texture2D _image = null;
        private int _imageWidth = 24;
        private int _imageHeight = 24;

        private int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
            set
            {
                if (_imageHeight == value)
                    return;
                _imageHeight = value;
                RequireTextRerender();
                Invalidate();
            }
        }


        private int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                if (_imageWidth == value)
                    return;
                _imageWidth = value;
                RequireTextRerender();
                Invalidate();
            }
        }

        public Texture2D Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image == value)
                    return;
                _image = value;
                RequireTextRerender();
                Invalidate();
            }
        }

        public Button()
        {
            Alignment = Engine.GUI.TextAlignment.Middle;
            Text = "Click me!";
        }

        protected override void OnLayout(GameTime gameTime)
        {
            FontStyle = TextControlFontStyle.Custom;
            Font = SkinEngine.LoadedSkin.ButtonFont;
            var fgCol = SkinEngine.LoadedSkin.ButtonIdleTextColor.ToMonoColor();
            if (ContainsMouse)
            {
                fgCol = SkinEngine.LoadedSkin.ButtonHoverTextColor.ToMonoColor();

            }
            if (MouseLeftDown)
            {
                fgCol = SkinEngine.LoadedSkin.ButtonPressedTextColor.ToMonoColor();
            }
            TextColor = fgCol;
            AutoSize = true;
            Font = SkinEngine.LoadedSkin.ButtonFont;
            if (AutoSize == true)
            {
                base.OnLayout(gameTime);
                if (TextRerenderRequired == true)
                {
                    if (_image != null)
                    {
                        ImageHeight = Height;
                        ImageWidth = ImageHeight;
                        Height = Math.Max(Height, _imageHeight);
                        Width += _imageWidth + 3;
                    }
                    Width += (SkinEngine.LoadedSkin.ButtonMargins.Width*2);
                    Height += (SkinEngine.LoadedSkin.ButtonMargins.Height * 2);
                }

            }
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int w = (_image == null) ? Width : Width - _imageWidth;
            var measure = GraphicsContext.MeasureString(Text, Font, Engine.GUI.TextAlignment.Middle);

            var loc = new Vector2((w - measure.X) / 2, (Height - measure.Y) / 2);

            if(_image != null)
            {
                loc.X += _imageWidth + 3;
            }

            gfx.DrawString(Text, (int)loc.X, (int)loc.Y, Microsoft.Xna.Framework.Color.White, Font, Engine.GUI.TextAlignment.Middle);

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

            if(_image != null)
                gfx.DrawRectangle(SkinEngine.LoadedSkin.ButtonMargins.Width, (Height - _imageHeight) / 2, _imageWidth, _imageHeight, _image, fgCol, System.Windows.Forms.ImageLayout.Stretch);

            base.OnPaint(gfx, target);
        }
    }
}
