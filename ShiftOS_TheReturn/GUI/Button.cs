using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Theming;

namespace Plex.Engine.GUI
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
            AutoSize = true;
            if (AutoSize == true)
            {
                int w = 0;
                int h = 0;
                var measure = ThemeManager.Theme.MeasureString(TextControlFontStyle.System, Text);
                w = (int)measure.X;
                h = (int)measure.Y;
                if (TextRerenderRequired == true)
                {
                    if (_image != null)
                    {
                        ImageHeight = h;
                        ImageWidth = ImageHeight;
                        h = Math.Max((int)measure.Y, _imageHeight);
                        w += _imageWidth + 3;
                    }
                    w += (4*2);
                    h += (3 * 2);
                    MinWidth = w;
                    MinHeight = h;
                    MaxWidth = w;
                    MaxHeight = h;
                    Width = w;
                    Height = h;

                }

            }
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int w = (_image == null) ? Width : Width - _imageWidth;
            var measure = ThemeManager.Theme.MeasureString(TextControlFontStyle.System, Text);

            var loc = new Vector2((w - measure.X) / 2, (Height - measure.Y) / 2);

            if(_image != null)
            {
                loc.X += _imageWidth + 3;
            }

            var state = ButtonState.Idle;
            if (ContainsMouse)
                state = ButtonState.MouseHover;
            if (MouseLeftDown)
                state = ButtonState.MouseDown;
            ThemeManager.Theme.DrawButtonText(gfx, Text, (int)loc.X, (int)loc.Y, (int)measure.X, (int)measure.Y, state);
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            var state = ButtonState.Idle;
            if (ContainsMouse)
                state = ButtonState.MouseHover;
            if (MouseLeftDown)
                state = ButtonState.MouseDown;
            ThemeManager.Theme.DrawButtonBackground(gfx, 0, 0, Width, Height, state);
            if(_image != null)
            {
                int _ix = 4;
                int _iy = (Height - ImageHeight) / 2;
                ThemeManager.Theme.DrawButtonImage(gfx, _ix, _iy, ImageWidth, ImageHeight, state, _image);
            }
            base.OnPaint(gfx, target);
        }
    }
}
