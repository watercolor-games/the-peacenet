using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// Provides extremely basic multi-line text editing support. This user interface element is extremely buggy.
    /// </summary>
    public class TextEditor : Control
    {
        private SpriteFont _font = null;

        private string _text = "";
        private bool _acceptsReturn = true;
        private bool _autoSize = false;

        private int _charX = 0;
        private int _charY = 0;
        
        private int _index = 0;

        private bool _needsHeightMeasure = false;
        private int _lineHeight = 0;

        /// <summary>
        /// Gets or sets whether the height of the control is determined by the height of the text.
        /// </summary>
        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                if (_autoSize != value)
                    _autoSize = value;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Enter:
                    Text = Text.Insert(_index, "\n");
                    _index++;
                    break;
                case Keys.Back:
                    if(_index > 0)
                    {
                        _text = _text.Remove(_index-1, 1);
                        _index--;
                        Invalidate(true);
                    }
                    break;
                default:
                    if(e.Character != null)
                    {
                        char real = (char)e.Character;
                        switch (real)
                        {
                            case '\r':
                            case '\n':
                            case '\b':
                            case '\0':
                            case '\t':
                            case '\v':

                                break;
                            default:
                                _text = _text.Insert(_index, real.ToString());
                                _index++;
                                Invalidate(true);
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the text of the text box.
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
                _index = MathHelper.Clamp(_index, 0, _text.Length);
                Invalidate(true);
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_font == null)
            {
                _font = Theme.GetFont(Themes.TextFontStyle.System);
                _needsHeightMeasure = true;
            }
            if (_needsHeightMeasure)
            {
                _lineHeight = (int)TextRenderer.MeasureText("#", _font, int.MaxValue, TextRenderers.WrapMode.None).Y;
                _needsHeightMeasure = false;
            }
            if(_autoSize)
                Height = _charY + _lineHeight;
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            _charX = 0;
            _charY = 0;
            for (int i = 0; i <= _text.Length; i++)
            {
                if(i < _text.Length)
                {
                    char c = _text[i];
                    switch (c)
                    {
                        case '\b':
                        case '\t':
                        case '\v':
                        case '\r':
                        case '\0':
                            continue;
                        case '\n':
                            _charX = 0;
                            _charY += _lineHeight;
                            break;
                        default:
                            var measure = (int)TextRenderer.MeasureText(c.ToString(), _font, int.MaxValue, TextRenderers.WrapMode.None).X;
                            gfx.DrawString(c.ToString(), _charX, _charY, Color.White, _font, TextAlignment.Top | TextAlignment.Left);
                            if(_charX + measure >= Width)
                            {
                                _charX = 0;
                                _charY += _lineHeight;
                            }
                            else
                            {
                                _charX += measure;
                            }
                            break;
                    }
                }
                if (i == _index)
                {
                    if (HasFocused)
                    {
                        gfx.DrawRectangle(_charX, _charY, 2, _lineHeight, Color.White);
                    }
                }
            }
        }

    }
}
