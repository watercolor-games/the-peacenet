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

        public override void DrawButtonBackground(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state)
        {
            throw new NotImplementedException();
        }

        public override void DrawButtonImage(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, Texture2D image)
        {
            throw new NotImplementedException();
        }

        public override void DrawButtonText(GraphicsContext gfx, string text, int x, int y, int width, int height, UIButtonState state)
        {
            throw new NotImplementedException();
        }

        public override void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver)
        {
            throw new NotImplementedException();
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

        public override void DrawWindowBorder(GraphicsContext graphics, int x, int y, int width, int height, bool focused, bool maximized, UIButtonState close, UIButtonState maximize, UIButtonState minimize, bool dialog, string titleText)
        {
            throw new NotImplementedException();
        }

        public override Color GetAccentColor()
        {
            return _accent;
        }

        public override Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
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

            _bgRegular = new Color(32, 32, 32, 255);
            _bgDark = new Color(16, 16, 16, 255);
            _bgLight = new Color(127, 127, 127, 255);
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
    }
}
