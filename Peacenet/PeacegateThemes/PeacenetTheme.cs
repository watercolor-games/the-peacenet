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
using Plex.Engine.Config;

namespace Peacenet.PeacegateThemes
{
    /// <summary>
    /// A <see cref="Theme"/> object which renders the UI with a sleek dark user interface with a light blue accent color. 
    /// </summary>
    [PeacegateTheme("Default Dark", "The default dark theme for Peacegate OS.", "ThemePreviews/DefaultDark")]
    public class PeacenetTheme : Theme
    {
        public override Color ControlBG => _bgRegular;

        [Dependency]
        private ConfigManager _config = null;

        public override int ScrollbarSize
        {
            get
            {
                return 17;
            }
        }

        public override void DrawScrollbar(GraphicsContext gfx, Hitbox upArrow, Hitbox downArrow, Hitbox scrollNub)
        {
            DrawControlDarkBG(gfx, 0, 0, gfx.Width, gfx.Height);

            var accent = GetAccentColor();

            gfx.FillRectangle(upArrow.X, upArrow.Y, upArrow.Width, upArrow.Height, _bgLight);
            gfx.FillRectangle(downArrow.X, downArrow.Y, downArrow.Width, downArrow.Height, _bgLight);

            gfx.FillRectangle(scrollNub.X, scrollNub.Y, scrollNub.Width, scrollNub.Height, accent);

        }

        //Lime assets
        private Texture2D bleftlime;
        private Texture2D leftlime;
        private Texture2D _closelime;

        //Tangerine assets
        private Texture2D blefttangerine;
        private Texture2D lefttangerine;
        private Texture2D _closetangerine;

        //Raspberry assets
        private Texture2D bleftraspberry;
        private Texture2D leftraspberry;
        private Texture2D _closeraspberry;

        //Blueberry assets
        private Texture2D bleftblueberry;
        private Texture2D leftblueberry;
        private Texture2D _closeblueberry;

        //Grape assets
        private Texture2D bleftgrape;
        private Texture2D leftgrape;
        private Texture2D _closegrape;

        //Inactive assets
        private Texture2D bleftinactive;
        private Texture2D leftinactive;
        private Texture2D _closeinactive;



        //Textures
        private Texture2D _arrowup = null;
        private Texture2D _arrowdown = null;
        private Texture2D _arrowleft = null;
        private Texture2D _arrowright = null;

        private Texture2D _close = null;
        private Texture2D _minimize = null;
        private Texture2D _maximize = null;
        private Texture2D _restore = null;

        private SpriteFont _titleFont;

        //Button state colors
        private Color _bStateTextIdle;
        private Color _bStateTextHover;
        private Color _bStateTextPressed;


        //Control BGs
        private Color _bgRegular;
        private Color _bgDark;
        private Color _bgLight;

        private Color _peace;
        private Color _gray;

        private SpriteFont _head1;
        private SpriteFont _head2;
        private SpriteFont _head3;
        private SpriteFont _mono;
        private SpriteFont _system;
        private SpriteFont _highlight;
        private SpriteFont _muted;

        private Texture2D _check = null;
        private Texture2D _times = null;

        private Color _buttonIdleBG;

        /// <inheritdoc/>
        public override int WindowBorderWidth
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public override int WindowTitleHeight
        {
            get
            {
                return 24;
            }
        }

        /// <inheritdoc/>
        public override SpriteFont GetFont(TextFontStyle style)
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
                case TextFontStyle.Muted:
                    return _muted;
                case TextFontStyle.Highlight:
                    return _highlight;
                default:
                    return _system;
            }
        }

        /// <inheritdoc/>
        public override Color GetFontColor(TextFontStyle style)
        {
            switch (style)
            {
                case TextFontStyle.Header1:
                case TextFontStyle.Header2:
                case TextFontStyle.Header3:
                    return GetAccentColor().Lighten(0.75F).Lighten(0.75F);
                default:
                    return Color.White;
            }
        }

        /// <inheritdoc/>
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
            gfx.FillRectangle(x, y, width, height, arrow, cstate, ImageLayout.Zoom);
        }

        /// <inheritdoc/>
        public override void DrawButton(GraphicsContext gfx, string text, Texture2D image, UIButtonState state, bool showImage, Rectangle imageRect, Rectangle textRect)
        {
            var bg = GetAccentColor().Darken(0.35F);
            var fg = _bStateTextIdle;

            switch (state)
            {
                case UIButtonState.Hover:
                    bg = bg.Lighten(0.2F);
                    fg = _bStateTextHover;
                    break;
                case UIButtonState.Pressed:
                    bg = bg.Darken(0.2F);
                    fg = _bStateTextIdle;
                    break;
            }

            gfx.Clear(bg);

            if (showImage)
            {
                gfx.FillRectangle(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, image, fg);
            }
            gfx.DrawString(text, textRect.X, textRect.Y, fg, _highlight, TextAlignment.Left, textRect.Width, Plex.Engine.TextRenderers.WrapMode.Words);
        }


        /// <inheritdoc/>
        public override void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver)
        {
            var fg = (isMouseOver) ? Color.White : _bStateTextIdle;
            var bg = (isMouseOver) ? _bgLight : _bgDark;

            gfx.Clear(fg);
            gfx.FillRectangle(x + 2, y + 2, width - 4, height - 4, bg);

            if(isChecked)
            gfx.FillRectangle(x+2, y+2, width-4, height-4, _check, fg, ImageLayout.Zoom);
        }

        /// <inheritdoc/>
        public override void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, _bgDark);
        }

        /// <inheritdoc/>
        public override void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, _bgRegular);
        }

        /// <inheritdoc/>
        public override void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, _bgLight);
        }

        /// <inheritdoc/>
        public override void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style, UIButtonState state)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        [Dependency]
        private PeacenetThemeManager _pn = null;

        /// <inheritdoc/>
        public override Color GetAccentColor()
        {
            switch(_pn.AccentColor)
            {
                case PeacenetAccentColor.Blueberry:
                    return _accentBlueberry;
                case PeacenetAccentColor.Orange:
                    return _accentTangerine;
                case PeacenetAccentColor.Grape:
                    return _accentGrape;
                case PeacenetAccentColor.Raspberry:
                    return _accentRaspberry;
                case PeacenetAccentColor.Lime:
                    return _accentLime;
            }
            return _accentLime;
        }

        /// <inheritdoc/>
        public override Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight)
        {
            const int _buttonWidth = 16;
            const int _spacing = 2;

            int _closeX = (windowWidth - _spacing) - _buttonWidth;
            int _maximizeX = (_closeX - _spacing) - _buttonWidth;
            int _minimizeX = (_maximizeX - _spacing) - _buttonWidth;

            int _buttonY = (WindowTitleHeight - _buttonWidth) / 2;

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

        private Texture2D _getLeftAsset()
        {
            switch (_pn.AccentColor)
            {
                case PeacenetAccentColor.Blueberry:
                    return leftblueberry;
                case PeacenetAccentColor.Grape:
                    return leftgrape;
                case PeacenetAccentColor.Lime:
                    return leftlime;
                case PeacenetAccentColor.Orange:
                    return lefttangerine;
                case PeacenetAccentColor.Raspberry:
                    return leftraspberry;
            }
            return bar;
        }

        private Texture2D _getCloseAsset()
        {
            switch (_pn.AccentColor)
            {
                case PeacenetAccentColor.Blueberry:
                    return _closeblueberry;
                case PeacenetAccentColor.Grape:
                    return _closegrape;
                case PeacenetAccentColor.Lime:
                    return _closelime;
                case PeacenetAccentColor.Orange:
                    return _closetangerine;
                case PeacenetAccentColor.Raspberry:
                    return _closeraspberry;
            }
            return _close;
        }


        /// <inheritdoc/>
        public override void LoadThemeData(GraphicsDevice device, ContentManager content)
        {
            var fontSize = _config.GetValue<PeacenetFontSize>("theme.fontsize", PeacenetFontSize.Small);

            _arrowup = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-up");
            _arrowdown = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-down");
            _arrowleft = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-left");
            _arrowright = content.Load<Texture2D>("ThemeAssets/Arrows/chevron-right");
            _close = content.Load<Texture2D>("ThemeAssets/New/close");
            _closelime = content.Load<Texture2D>("ThemeAssets/New/closelime");
            _minimize = content.Load<Texture2D>("ThemeAssets/New/min");
            _maximize = content.Load<Texture2D>("ThemeAssets/New/max");
            _restore = content.Load<Texture2D>("ThemeAssets/WindowBorder/Restore");
            leftlime = content.Load<Texture2D>("ThemeAssets/New/leftlime");
            bar = content.Load<Texture2D>("ThemeAssets/New/bar");

            _closeraspberry = content.Load<Texture2D>("ThemeAssets/New/closered");
            bleftraspberry = content.Load<Texture2D>("ThemeAssets/New/bleftred");
            leftraspberry = content.Load<Texture2D>("ThemeAssets/New/leftred");

            _closeblueberry = content.Load<Texture2D>("ThemeAssets/New/closeblue");
            bleftblueberry = content.Load<Texture2D>("ThemeAssets/New/bleftblue");
            leftblueberry = content.Load<Texture2D>("ThemeAssets/New/leftblue");

            _closegrape = content.Load<Texture2D>("ThemeAssets/New/closepurple");
            bleftgrape = content.Load<Texture2D>("ThemeAssets/New/bleftpurple");
            leftgrape = content.Load<Texture2D>("ThemeAssets/New/leftpurple");

            _closetangerine = content.Load<Texture2D>("ThemeAssets/New/closeorange");
            blefttangerine = content.Load<Texture2D>("ThemeAssets/New/bleftorange");
            lefttangerine = content.Load<Texture2D>("ThemeAssets/New/leftorange");

            _closeinactive = content.Load<Texture2D>("ThemeAssets/New/closeinactive");
            bleftinactive = content.Load<Texture2D>("ThemeAssets/New/bleftinactive");
            leftinactive = content.Load<Texture2D>("ThemeAssets/New/leftinactive");


            _bStateTextIdle = new Color(191, 191, 191, 255);
            _bStateTextHover = Color.White;
            _bStateTextPressed = Color.Gray;

            _bgRegular = new Color(64, 64, 64, 255);
            _bgDark = new Color(32, 32, 32, 255);
            _bgLight = new Color(127, 127, 127, 255);

            _buttonIdleBG = new Color(90, 90, 90, 255);

            _titleFont = content.Load<SpriteFont>("ThemeAssets/Fonts/Titlebar");

            _head1 = content.Load<SpriteFont>($"ThemeAssets/Fonts/Header1/{fontSize}");
            _head2 = content.Load < SpriteFont>($"ThemeAssets/Fonts/Header2/{fontSize}"); ;
            _head3 = content.Load < SpriteFont>($"ThemeAssets/Fonts/Header3/{fontSize}");
            _mono = content.Load < SpriteFont>($"ThemeAssets/Fonts/Mono/{fontSize}");
            _system = content.Load < SpriteFont>($"ThemeAssets/Fonts/System/{fontSize}");
            _muted = content.Load<SpriteFont>($"ThemeAssets/Fonts/Muted/{fontSize}");
            _highlight = content.Load<SpriteFont>($"ThemeAssets/Fonts/Highlight/{fontSize}");


            _peace = new Color(64, 128, 255,255);
            _gray = new Color(191, 191, 191, 255);

            _check = content.Load<Texture2D>("ThemeAssets/CheckBox/check");
            _times = content.Load<Texture2D>("ThemeAssets/CheckBox/times");

            _accentTangerine = new Color(0xF7, 0x94, 0x1B);
            _accentBlueberry = new Color(0x1B, 0xAA, 0xF7);
            _accentLime = new Color(0x2C, 0xD3, 0x1D);
            _accentGrape = new Color(0x94, 0x44, 0xFF);
            _accentRaspberry = new Color(0xF7, 0x1B, 0x1B);
            _accentInactive = new Color(121, 121, 121);
        }

        /// <inheritdoc/>
        public override void UnloadThemeData()
        {
        }

        private Texture2D bar;

        //accent colors
        private Color _accentTangerine;
        private Color _accentRaspberry;
        private Color _accentGrape;
        private Color _accentBlueberry;
        private Color _accentLime;
        private Color _accentInactive;

        /// <inheritdoc/>
        public override void DrawWindowBorder(GraphicsContext graphics, string titletext, Hitbox leftBorder, Hitbox rightBorder, Hitbox bottomBorder, Hitbox leftCorner, Hitbox rightCorner, Hitbox title, Hitbox close, Hitbox minimize, Hitbox maximize, bool isFocused)
        {
            var accent = GetAccentColor();
            if (isFocused == false)
                accent = _accentInactive;
            //First, the titlebar.
            if (title.Visible)
            {
                //The background.
                graphics.FillRectangle(title.X, title.Y, leftlime.Width, title.Height, (isFocused) ? _getLeftAsset() : leftinactive, Color.White, ImageLayout.Stretch);
                graphics.FillRectangle(title.X + leftlime.Width, title.Y, title.Width - leftlime.Width, title.Height, bar, Color.White);
                //Now the text.
                var titleTextMeasure = TextRenderer.MeasureText(titletext, _titleFont, title.Width, Plex.Engine.TextRenderers.WrapMode.None);
                int _textX = 50;
                int _textY = (int)((title.Height - titleTextMeasure.Y) / 2);

                graphics.DrawString(titletext, _textX, _textY, Color.White, _titleFont, TextAlignment.Left, (int)titleTextMeasure.X, Plex.Engine.TextRenderers.WrapMode.None);

                if (close.Visible)
                {
                    if (close.ContainsMouse)
                        graphics.FillRectangle(close.X, close.Y, close.Width, close.Height, this._getCloseAsset(), Color.White);
                    else
                        graphics.FillRectangle(close.X, close.Y, close.Width, close.Height, this._close, Color.White);
                }
                if (minimize.Visible)
                {
                    graphics.FillRectangle(minimize.X, minimize.Y, minimize.Width, minimize.Height, _minimize, Color.White);

                }
                if (maximize.Visible)
                {
                    graphics.FillRectangle(maximize.X, maximize.Y, maximize.Width, maximize.Height, _maximize, Color.White);
                }
            }

            //We only need to draw the other borders if ONE of them is visible.
            if (leftBorder.Visible)
            {
                graphics.FillRectangle(leftBorder.X, leftBorder.Y, leftBorder.Width, leftBorder.Height, accent);
                graphics.FillRectangle(rightBorder.X, rightBorder.Y, rightBorder.Width, rightBorder.Height, bar, Color.White);
                graphics.FillRectangle(bottomBorder.X, bottomBorder.Y, _getLeftAsset().Width, bottomBorder.Height, (isFocused) ? _getLeftAsset() : leftinactive, Color.White);
                graphics.FillRectangle(bottomBorder.X+_getLeftAsset().Width, bottomBorder.Y, bottomBorder.Width-_getLeftAsset().Width, bottomBorder.Height, bar, Color.White);
                graphics.FillRectangle(rightCorner.X, rightCorner.Y, rightCorner.Width, rightCorner.Height, bar, Color.White);
                graphics.FillRectangle(leftCorner.X, leftCorner.Y, leftCorner.Width, leftCorner.Height, accent);

            }
        }
    }

    /// <summary>
    /// Represents an accent color within the Peacenet theme.
    /// </summary>
    public enum PeacenetAccentColor
    {
        /// <summary>
        /// Represents a lime accent color.
        /// </summary>
        Lime,
        /// <summary>
        /// represents a blue accent color.
        /// </summary>
        Blueberry,
        /// <summary>
        /// Represents an orange accent color.
        /// </summary>
        Orange,
        /// <summary>
        /// Represents a purple accent color.
        /// </summary>
        Grape,
        /// <summary>
        /// Represents a red accent color.
        /// </summary>
        Raspberry
    }

    public enum PeacenetFontSize
    {
        Small,
        Medium,
        Large
    }
}
