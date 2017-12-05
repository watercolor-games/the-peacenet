using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class Button : Control
    {
        private string _text = "";
        private Texture2D _texture = null;
        private bool _showImage = false;

        private int _textureX = 0;
        private int _textureY = 0;
        private int _textureW = 0;
        private int _textureH = 0;

        private int _lX = 0;
        private int _lY = 0;
        private int _lW = 0;
        private int _lH = 0;

        public Rectangle ImageRect
        {
            get
            {
                return new Rectangle(_textureX, _textureY, _textureW, _textureH);
            }
        }

        public Rectangle TextRect
        {
            get
            {
                return new Rectangle(_lX, _lY, _lW, _lH);
            }
        }

        public bool ShowImage
        {
            get
            {
                return _showImage;
            }
            set
            {
                if (_showImage == value)
                    return;
                _showImage = value;
                Invalidate();
            }
        }

        public Texture2D Image
        {
            get
            {
                return _texture;
            }
            set
            {
                if (_texture == value)
                    return;
                _texture = value;
                Invalidate();
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                Invalidate();
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_showImage)
            {
                _textureW = 16;
                _textureH = 16;
            }
            else
            {
                _textureW = 0;
                _textureH = 0;
            }
            int _minimumHorizontal = 8;
            TextAlignment _alignment = TextAlignment.Middle;
            if (_showImage)
            {
                _minimumHorizontal += _textureW+3;
                _alignment = TextAlignment.Left;
            }
            int realMax = MaxWidth == 0 ? int.MaxValue : MaxWidth;

            var font = Theme.GetFont(Themes.TextFontStyle.System);
            var measure = TextRenderer.MeasureText(_text, font, realMax - _minimumHorizontal, _alignment, TextRenderers.WrapMode.Words);

            _lW = (int)measure.X;
            _lH = (int)measure.Y;

            int realHeight = 6 + (Math.Max(_textureH, _lH));
            Height = realHeight;
            int realWidth = (int)measure.X;
            if (_showImage)
            {
                if (realWidth >= 0)
                    realWidth += 3;
                realWidth += _textureW;
            }
            realWidth += 8;
            Width = realWidth;
            _lY = (Height - _lH) / 2;
            if (_showImage)
            {
                _textureX = 4;
                _textureY = (Height - _textureH) / 2;
                _lX = _textureX + _textureW + 3;
            }
            else
            {
                _lX = (Width - _lW) / 2;
            }


            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var state = Themes.UIButtonState.Idle;
            if (ContainsMouse)
                state = Themes.UIButtonState.Hover;
            if (LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                state = Themes.UIButtonState.Pressed;
            Theme.DrawButton(gfx, _text, _texture, state, _showImage, new Rectangle(_textureX, _textureY, _textureW, _textureH), new Rectangle(_lX, _lY, _lW, _lH));

        }
    }
}
