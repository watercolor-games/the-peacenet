using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
{
    public class TextControl : Control
    {
        private string _text = "";
        private Font _font = new Font("Tahoma", 9f);
        private RenderTarget2D _textBuffer = null;
        bool requiresTextRerender = true;
        private TextAlignment alignment = TextAlignment.TopLeft;
        private Microsoft.Xna.Framework.Color _foreground = Microsoft.Xna.Framework.Color.Black;
        private TextControlFontStyle _fs = TextControlFontStyle.System;

        public TextAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (alignment == value)
                    return;
                alignment = value;
                RequireTextRerender();
                Invalidate();
            }
        }

        public Microsoft.Xna.Framework.Color TextColor
        {
            get
            {
                return _foreground;
            }
            set
            {
                if (_foreground == value)
                    return;
                _foreground = value;
                Invalidate();
            }
        }

        public TextControlFontStyle FontStyle
        {
            get
            {
                return _fs;
            }
            set
            {
                _fs = value;
                ResetStyle();
                Invalidate();
            }
        }

        protected override void OnDisposing()
        {
            if (_textBuffer != null)
            {
                _textBuffer.Dispose();
                _textBuffer = null;
            }
            base.OnDisposing();
        }

        public void RequireTextRerender()
        {
            requiresTextRerender = true;
        }

        public void DontRequireTextRerender()
        {
            requiresTextRerender = false;
        }

        protected virtual void RenderText(GraphicsContext gfx)
        {
            var sMeasure = GraphicsContext.MeasureString(_text, _font, Alignment, Width);
            PointF loc = new PointF(2, 2);
            float centerH = (Width - sMeasure.X) / 2;
            float centerV = (Height - sMeasure.Y) / 2;
            switch (this.alignment)
            {
                case TextAlignment.Top:
                    loc.X = centerH;
                    break;
                case TextAlignment.TopRight:
                    loc.X = Width - sMeasure.X;
                    break;
                case TextAlignment.Left:
                    loc.Y = centerV;
                    break;
                case TextAlignment.Middle:
                    loc.Y = centerV;
                    loc.X = centerH;
                    break;
                case TextAlignment.Right:
                    loc.Y = centerV;
                    loc.X = (Width - sMeasure.Y);
                    break;
                case TextAlignment.BottomLeft:
                    loc.Y = (Height - sMeasure.Y);
                    break;
                case TextAlignment.Bottom:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = centerH;
                    break;
                case TextAlignment.BottomRight:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = (Width - sMeasure.X);
                    break;

            }

            gfx.DrawString(_text, 0, 0, Microsoft.Xna.Framework.Color.White, _font, Alignment, this.Width);

        }

        public bool TextRerenderRequired
        {
            get
            {
                return this.requiresTextRerender;
            }
        }

        public void ResetStyle()
        {
            switch (_fs)
            {
                case TextControlFontStyle.Header1:
                    Font = SkinEngine.LoadedSkin.HeaderFont;
                    TextColor = SkinEngine.LoadedSkin.FirstLevelHeaderColor.ToMonoColor();
                    break;
                case TextControlFontStyle.Header2:
                    Font = SkinEngine.LoadedSkin.Header2Font;
                    TextColor = SkinEngine.LoadedSkin.SecondLevelHeaderColor.ToMonoColor();
                    break;
                case TextControlFontStyle.Header3:
                    Font = SkinEngine.LoadedSkin.Header3Font;
                    TextColor = SkinEngine.LoadedSkin.ThirdLevelHeaderColor.ToMonoColor();
                    break;
                case TextControlFontStyle.System:
                    Font = SkinEngine.LoadedSkin.MainFont;
                    TextColor = SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor();
                    break;
                case TextControlFontStyle.Mono:
                    Font = SkinEngine.LoadedSkin.TerminalFont;
                    TextColor = SkinEngine.LoadedSkin.TerminalForeColor.ToMonoColor();
                    break;


            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            ResetStyle();
            if (AutoSize)
            {
                if (requiresTextRerender)
                {
                    var measure = GraphicsContext.MeasureString(_text, _font, Alignment, MaxWidth);
                    Width = (int)measure.X;
                    Height = (int)measure.Y;
                }
                else
                {
                    Width = _textBuffer.Width;
                    Height = _textBuffer.Height;
                }
            }
            if (requiresTextRerender)
            {
                Invalidate();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                requiresTextRerender = true;
                OnTextChanged();
                Invalidate();
            }
        }

        public event Action TextChanged;
        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke();
        }

        public Font Font
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
                requiresTextRerender = true;
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            if(_textBuffer != null)
            {
                if(_textBuffer.Width != Width || _textBuffer.Height != Height)
                {
                    _textBuffer.Dispose();
                    _textBuffer = null;
                    RequireTextRerender();
                }
            }
            if (requiresTextRerender)
            {
                requiresTextRerender = false;
                if(_textBuffer == null)
                    _textBuffer = new RenderTarget2D(gfx.Device, Math.Max(1,Width), Math.Max(1,Height), false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                gfx.Batch.End();
                int x = gfx.X;
                int y = gfx.Y;
                int w = gfx.Width;
                int h = gfx.Height;
                gfx.X = 0;
                gfx.Y = 0;
                gfx.Width = Width;
                gfx.Height = Height;
                gfx.Device.SetRenderTarget(_textBuffer);
                gfx.Device.Clear(Microsoft.Xna.Framework.Color.Transparent);
                gfx.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);
                RenderText(gfx);
                gfx.Batch.End();
                gfx.Device.SetRenderTarget(target);
                gfx.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                gfx.X = x;
                gfx.Y = y;
                gfx.Width = w;
                gfx.Height = h;
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);

            }
            gfx.DrawRectangle(0, 0, Width, Height, _textBuffer, _foreground * (float)Opacity);
        }
    }

    public enum TextControlFontStyle
    {
        System,
        Header1,
        Header2,
        Header3,
        Mono,
        Custom
    }
}
