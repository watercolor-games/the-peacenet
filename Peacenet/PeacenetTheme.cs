using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;

namespace Peacenet
{
    public class PeacenetTheme : Theme
    {
        //Textures
        private Texture2D _arrowup = null;
        private Texture2D _arrowdown = null;
        private Texture2D _arrowleft = null;
        private Texture2D _arrowright = null;

        private Texture2D _close = null;
        private Texture2D _minimize = null;
        private Texture2D _maximize = null;
        private Texture2D _restore = null;

        private System.Drawing.Font _titleFont;

        //Button state colors
        private Color _bStateTextIdle;
        private Color _bStateTextHover;
        private Color _bStateTextPressed;


        //Accent
        private Color _accent;


        //Control BGs
        private Color _bgRegular;
        private Color _bgDark;
        private Color _bgLight;

        private Color _peace;
        private Color _gray;

        private System.Drawing.Font _head1;
        private System.Drawing.Font _head2;
        private System.Drawing.Font _head3;
        private System.Drawing.Font _mono;
        private System.Drawing.Font _system;

        private Texture2D _check = null;
        private Texture2D _times = null;

        private Color _buttonIdleBG;

        public override System.Drawing.Font GetFont(TextFontStyle style)
        {
            switch (style)
            {
                case TextFontStyle.Header1:
                    return _head1;
                case TextFontStyle.Header2:
                    return _head2;
                case TextFontStyle.Header3:
                    return _head3;
                case TextFontStyle.Mono:
                    return _mono;
                default:
                    return _system;
            }
        }

        public override Color GetFontColor(TextFontStyle style)
        {
            switch (style)
            {
                case TextFontStyle.Header1:
                    return _peace;
                case TextFontStyle.Header2:
                case TextFontStyle.Header3:
                case TextFontStyle.Mono:
                    return Color.White;
                default:
                    return _gray;
            }
        }

        public override void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, ArrowDirection direction)
        {
            var arrow = _arrowup;
            var cstate = _bStateTextIdle;
            switch (direction)
            {
                case ArrowDirection.Down:
                    arrow = _arrowdown;
                    break;
                case ArrowDirection.Up:
                    arrow = _arrowup;
                    break;
                case ArrowDirection.Right:
                    arrow = _arrowright;
                    break;
                case ArrowDirection.Left:
                    arrow = _arrowleft;
                    break;
            }
            switch (state)
            {
                case UIButtonState.Hover:
                    cstate = _bStateTextHover;
                    break;
                case UIButtonState.Pressed:
                    cstate = _bStateTextPressed;
                    break;
            }
            gfx.DrawRectangle(x, y, width, height, arrow, cstate, System.Windows.Forms.ImageLayout.Zoom);

        }

        public override void DrawButton(GraphicsContext gfx, string text, Texture2D image, UIButtonState state, bool showImage, Rectangle imageRect, Rectangle textRect)
        {
            var bg = _buttonIdleBG;
            var fg = _bStateTextIdle;
            switch (state)
            {
                case UIButtonState.Hover:
                    bg = GetAccentColor();
                    fg = _bStateTextHover;
                    break;
                case UIButtonState.Pressed:
                    bg = _bgDark;
                    fg = _bStateTextPressed;
                    break;
            }

            gfx.Clear(bg);

            if (showImage)
            {
                gfx.DrawRectangle(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, image, fg);
            }

            gfx.DrawString(text, textRect.X, textRect.Y, fg, _system, (showImage) ? TextAlignment.Left : TextAlignment.Middle, textRect.Width, Plex.Engine.TextRenderers.WrapMode.Words);
        }


        public override void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver)
        {
            var fg = (isMouseOver) ? Color.White : _bStateTextIdle;
            var bg = (isMouseOver) ? _bgLight : _bgDark;

            gfx.Clear(fg);
            gfx.DrawRectangle(x + 2, y + 2, width - 4, height - 4, bg);

            if(isChecked)
            gfx.DrawRectangle(x+2, y+2, width-4, height-4, _check, fg, System.Windows.Forms.ImageLayout.Zoom);
        }

        public override void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _bgRegular);
        }

        public override void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _bgDark);
        }

        public override void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _bgLight);
        }

        public override void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
            throw new NotImplementedException();
        }

        public override void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style, UIButtonState state)
        {
            throw new NotImplementedException();
        }

        public override void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
            throw new NotImplementedException();
        }

        public override void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Color GetAccentColor()
        {
            return _accent;
        }

        public override Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight)
        {
            const int _buttonWidth = 24;
            const int _spacing = 2;

            int _closeX = (windowWidth - _spacing) - _buttonWidth;
            int _maximizeX = (_closeX - _spacing) - _buttonWidth;
            int _minimizeX = (_maximizeX - _spacing) - _buttonWidth;

            int _buttonY = (30 - _buttonWidth) / 2;

            switch(button)
            {
                case TitleButton.Close:
                    return new Rectangle(_closeX, _buttonY, _buttonWidth, _buttonWidth);
                case TitleButton.Minimize:
                    return new Rectangle(_minimizeX, _buttonY, _buttonWidth, _buttonWidth);

                case TitleButton.Maximize:
                    return new Rectangle(_maximizeX, _buttonY, _buttonWidth, _buttonWidth);
            }
            return Rectangle.Empty;
        }

        public override void LoadThemeData(GraphicsDevice device, ContentManager content)
        {
            _arrowup = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-up");
            _arrowdown = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-down");
            _arrowleft = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-left");
            _arrowright = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-right");
            _close = content.Load<Texture2D>("ThemeAssets/WindowBorder/Close");
            _minimize = content.Load<Texture2D>("ThemeAssets/WindowBorder/Minimize");
            _maximize = content.Load<Texture2D>("ThemeAssets/WindowBorder/Maximize");
            _restore = content.Load<Texture2D>("ThemeAssets/WindowBorder/Restore");

            _accent = new Color(64, 128, 255, 255);

            _bStateTextIdle = new Color(191, 191, 191, 255);
            _bStateTextHover = Color.White;
            _bStateTextPressed = Color.Gray;

            _bgRegular = new Color(64, 64, 64, 255);
            _bgDark = new Color(32, 32, 32, 255);
            _bgLight = new Color(127, 127, 127, 255);

            _buttonIdleBG = new Color(90, 90, 90, 255);

            _titleFont = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif.Name, 12F, System.Drawing.FontStyle.Bold);

            _head1 = new System.Drawing.Font("Monda", 24F, System.Drawing.FontStyle.Bold);
            _head2 = new System.Drawing.Font("Monda", 16F, System.Drawing.FontStyle.Bold);
            _head3 = new System.Drawing.Font("Monda", 12F);
            _mono = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 10F);
            _system = new System.Drawing.Font("Monda", 10F);

            _peace = new Color(64, 128, 255,255);
            _gray = new Color(191, 191, 191, 255);

            _check = content.Load<Texture2D>("ThemeAssets/CheckBox/check");
            _times = content.Load<Texture2D>("ThemeAssets/CheckBox/times");

        }

        public override Vector2 MeasureString(TextFontStyle style, string text, TextAlignment alignment = TextAlignment.TopLeft, int maxwidth = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public override void UnloadThemeData()
        {
            _arrowup.Dispose();
            _arrowdown.Dispose();
            _arrowleft.Dispose();
            _arrowright.Dispose();
            _close.Dispose();
            _minimize.Dispose();
            _maximize.Dispose();
            _restore.Dispose();
        }

        public override void DrawWindowBorder(GraphicsContext graphics, string titletext, Hitbox leftBorder, Hitbox rightBorder, Hitbox bottomBorder, Hitbox leftCorner, Hitbox rightCorner, Hitbox title, Hitbox close, Hitbox minimize, Hitbox maximize, bool isFocused)
        {
            var accent = GetAccentColor();
            if (isFocused == false)
                accent = accent * 0.5F;
            graphics.Clear(accent);
            //First, the titlebar.
            if (title.Visible)
            {
                //The background.
                graphics.DrawRectangle(title.X, title.Y, title.Width, title.Height, accent);
                //Now the text.
                var titleTextMeasure = TextRenderer.MeasureText(titletext, _titleFont, title.Width, TextAlignment.Middle, Plex.Engine.TextRenderers.WrapMode.None);
                int _textX = (int)((title.Width - titleTextMeasure.X) / 2);
                int _textY = (int)((title.Height - titleTextMeasure.Y) / 2);

                graphics.DrawString(titletext, _textX, _textY, Color.White, _titleFont, TextAlignment.Middle, (int)titleTextMeasure.X, Plex.Engine.TextRenderers.WrapMode.None);

                if (close.Visible)
                {
                    if(close.ContainsMouse)
                        graphics.DrawRectangle(close.X, close.Y, close.Width, close.Height, this._close, this._bStateTextHover);
                    else
                        graphics.DrawRectangle(close.X, close.Y, close.Width, close.Height, this._close, this._bStateTextIdle);
                }
                if (minimize.Visible)
                {
                    if (minimize.ContainsMouse)
                        graphics.DrawRectangle(minimize.X, minimize.Y, minimize.Width, minimize.Height, this._minimize, this._bStateTextHover);
                    else
                        graphics.DrawRectangle(minimize.X, minimize.Y, minimize.Width, minimize.Height, this._minimize, this._bStateTextIdle);

                }
                if (maximize.Visible)
                {
                    if (maximize.ContainsMouse)
                        graphics.DrawRectangle(maximize.X, maximize.Y, maximize.Width, maximize.Height, this._maximize, this._bStateTextHover);
                    else
                        graphics.DrawRectangle(maximize.X, maximize.Y, maximize.Width, maximize.Height, this._maximize, this._bStateTextIdle);
                }
            }

            //We only need to draw the other borders if ONE of them is visible.
            if (leftBorder.Visible)
            {
                graphics.DrawRectangle(leftBorder.X, leftBorder.Y, leftBorder.Width, leftBorder.Height, accent);
                graphics.DrawRectangle(rightBorder.X, rightBorder.Y, rightBorder.Width, rightBorder.Height, accent);
                graphics.DrawRectangle(bottomBorder.X, bottomBorder.Y, bottomBorder.Width, bottomBorder.Height, accent);
                graphics.DrawRectangle(rightCorner.X, rightCorner.Y, rightCorner.Width, rightCorner.Height, accent);
                graphics.DrawRectangle(leftCorner.X, leftCorner.Y, leftCorner.Width, leftCorner.Height, accent);

            }
        }
    }
}
