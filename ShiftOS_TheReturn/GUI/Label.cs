using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class Label : Control
    {
        private bool _autoSize = false;
        private System.Drawing.Font _font;
        private Color _color;
        private Themes.TextFontStyle _style = Themes.TextFontStyle.System;
        private string _text = "";
        private TextAlignment _alignment = TextAlignment.TopLeft;

        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                if (_autoSize == value)
                    return;
                _autoSize = value;
            }
        }

        public Color CustomColor
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color == value)
                    return;
                _color = value;
                Invalidate();
            }
        }

        public System.Drawing.Font CustomFont
        {
            get
            {
                return _font;
            }
            set
            {
                if (_font == value)
                    return;
                _font = value;
                Invalidate();
            }
        }

        public Themes.TextFontStyle FontStyle
        {
            get
            {
                return _style;
            }
            set
            {
                if (_style == value)
                    return;
                _style = value;
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

        public TextAlignment Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                if (_alignment == value)
                    return;
                _alignment = value;
                Invalidate();
            }
        }

        private System.Drawing.Font getFont()
        {
            if (_style == Themes.TextFontStyle.Custom)
                return _font;
            return Theme.GetFont(_style);
        }

        private Color getColor()
        {
            if (_style == Themes.TextFontStyle.Custom)
                return _color;
            return Theme.GetFontColor(_style);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_autoSize)
            {
                var font = getFont();
                var measure = TextRenderer.MeasureText(_text, font, int.MaxValue, _alignment, TextRenderers.WrapMode.Words);
                Width = (int)measure.X;
                Height = (int)measure.Y;
            }
            
            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx, RenderTarget2D currentTarget)
        {
            var font = getFont();
            var color = getColor();
            var measure = TextRenderer.MeasureText(_text, font, Width, _alignment, TextRenderers.WrapMode.Words);

            int x = 0;
            int y = 0;
            switch (_alignment)
            {
                case TextAlignment.Top:
                    x = (Width - (int)measure.X) / 2;
                    break;
                case TextAlignment.TopRight:
                    x = (Width - (int)measure.X);
                    break;
                case TextAlignment.Left:
                    x = 0;
                    y = (Height - (int)measure.Y) / 2;
                    break;
                case TextAlignment.Middle:
                    x = (Width - (int)measure.X)/2;
                    y = (Height - (int)measure.Y) / 2;
                    break;
                case TextAlignment.Right:
                    x = (Width - (int)measure.X);
                    y = (Height - (int)measure.Y)/2;

                    break;
                case TextAlignment.BottomLeft:
                    x = 0;
                    y = (Height - (int)measure.Y);

                    break;
                case TextAlignment.Bottom:
                    x = (Width - (int)measure.X)/2;
                    y = (Height - (int)measure.Y);

                    break;
                case TextAlignment.BottomRight:
                    x = (Width - (int)measure.X);
                    y = (Height - (int)measure.Y);

                    break;
            }

            gfx.DrawString(_text, x, y, color, font, _alignment, (int)measure.X, TextRenderers.WrapMode.Words);
        }
    }
}
