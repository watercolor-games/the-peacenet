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
    /// <summary>
    /// A class representing a control that contains text.
    /// </summary>
    public class Label : Control
    {
        private bool _autoSize = false;
        private SpriteFont _font;
        private Color _color;
        private Themes.TextFontStyle _style = Themes.TextFontStyle.System;
        private string _text = "";
        private TextAlignment _alignment = TextAlignment.Top | TextAlignment.Left;
        private bool _remeasure = true;

        /// <summary>
        /// Gets or sets whether the control should be auto-sized based on the size of the text
        /// </summary>
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
                _remeasure = true;
            }
        }

        /// <summary>
        /// If the <see cref="FontStyle"/> is <see cref="Themes.TextFontStyle.Custom"/>, this is the color of the text.  
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// If the <see cref="FontStyle"/> is <see cref="Themes.TextFontStyle.Custom"/>, this is the font of the text.  
        /// </summary>
        public SpriteFont CustomFont
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
                Invalidate(true);
                _remeasure = true;
            }
        }

        /// <summary>
        /// The font style of the label
        /// </summary>
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
                Invalidate(true);
                _remeasure = true;
            }
        }

        /// <summary>
        /// Gets or sets the label's text.
        /// </summary>
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
                Invalidate(true);
                _remeasure = true;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the label's text.
        /// </summary>
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
                Invalidate(true);
                _remeasure = true;
            }
        }

        private SpriteFont getFont()
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

        private int _lastmaxwidth = 0;
        private int _lastmaxheight = 0;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_lastmaxheight != MaxHeight)
                _remeasure = true;
            if (_lastmaxwidth != MaxWidth)
                _remeasure = true;
            _lastmaxheight = MaxHeight;
            _lastmaxwidth = MaxWidth;
            if (_autoSize)
            {
                if (_remeasure)
                {
                    var font = getFont();
                    var measure = TextRenderer.MeasureText(_text, font, (MaxWidth == 0) ? int.MaxValue : MaxWidth, TextRenderers.WrapMode.Words);
                    Width = (int)measure.X;
                    Height = (int)measure.Y;
                    _remeasure = false;
                }
            }
            
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            var font = getFont();
            var color = getColor();
            var measure = TextRenderer.MeasureText(_text, font, Width, TextRenderers.WrapMode.Words);

            int x = 0;
            int y = 0;

            if(_alignment.HasFlag( TextAlignment.Center))
            {
                x = (Width - (int)measure.X) / 2;
                y = (Height - (int)measure.Y) / 2;
            }

            if (_alignment.HasFlag(TextAlignment.Left))
            {
                x = 0;
            }
            if(_alignment.HasFlag(TextAlignment.Right))
            {
                x = Width - (int)measure.X;
            }
            if(_alignment.HasFlag(TextAlignment.Top))
            {
                y = 0;
            }
            if(_alignment.HasFlag(TextAlignment.Bottom))
            {
                y = Height - (int)measure.Y;
            }

            gfx.DrawString(_text, x, y, color, font, _alignment, (int)measure.X, TextRenderers.WrapMode.Words);
        }
    }
}
