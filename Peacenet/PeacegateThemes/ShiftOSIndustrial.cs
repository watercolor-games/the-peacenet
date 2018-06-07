using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.TextRenderers;
using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.PeacegateThemes
{
    [PeacegateTheme("ShiftOS - Industrial", "The light ShiftOS UI color scheme with the Industrial window borders.", "ThemePreviews/Industrial")]
    public class ShiftOSIndustrial : PeacenetTheme
    {
        private Texture2D _check = null;

        private Texture2D _titlebar = null;
        private Texture2D _titlebarleft = null;
        private Texture2D _titlebarright = null;

        private Texture2D _borderleft = null;
        private Texture2D _borderright = null;
        private Texture2D _borderbottom = null;

        private Texture2D _cornerleft = null;
        private Texture2D _cornerright = null;

        private Texture2D _close = null;
        private Texture2D _minimize = null;
        private Texture2D _maximize = null;

        public override int ScrollbarSize => 20;

        public override int WindowTitleHeight => 23;

        public override int WindowBorderWidth => 5;

        public override void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, ArrowDirection direction)
        {

        }

        public override void DrawButton(GraphicsContext gfx, string text, Texture2D image, UIButtonState state, bool showImage, Rectangle imageRect, Rectangle textRect)
        {
            Color bg = Color.White;
            Color fg = Color.Black;
            switch (state)
            {
                case UIButtonState.Hover:
                    bg = Color.Gray;
                    fg = Color.White;
                    break;
                case UIButtonState.Pressed:
                    bg = Color.Black;
                    fg = Color.White;
                    break;
            }

            gfx.FillRectangle(0, 0, gfx.Width, gfx.Height, Color.Black);
            gfx.FillRectangle(2, 2, gfx.Width - 4, gfx.Height - 4, bg);

            if(showImage)
            {
                gfx.FillRectangle(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, image, fg);
            }

            gfx.DrawString(text, new Vector2(textRect.X, textRect.Y), fg, GetFont(TextFontStyle.Highlight), TextAlignment.Left, textRect.Width, WrapMode.Words);

        }

        public override void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver)
        {
            Color bg = Color.White;
            Color fg = Color.Black;
            if (isMouseOver)
            {
                bg = Color.Black;
                fg = Color.White;
            }

            gfx.FillRectangle(x, y, width, height, Color.Black);
            gfx.FillRectangle(x+2, y+2, width - 4, height - 4, bg);

            if (isChecked)
                gfx.FillRectangle(x + 2, y + 2, width - 4, height - 4, _check, fg);

        }

        public override void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, Color.White);
        }

        public override void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, Color.Black);
            graphics.FillRectangle(x+2, y+2, width-4, height-4, Color.White);

        }

        public override void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, Color.Black);
            graphics.FillRectangle(x + 1, y + 1, width - 2, height - 2, Color.White);
        }

        public override void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
            throw new NotImplementedException();
        }

        public override void DrawScrollbar(GraphicsContext gfx, Hitbox upArrow, Hitbox downArrow, Hitbox scrollNub)
        {
        }

        public override void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style, UIButtonState state)
        {
        }

        public override void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style)
        {
        }

        public override void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.FillRectangle(x, y, width, height, Color.Black);
        }

        public override void DrawWindowBorder(GraphicsContext graphics, string titletext, Hitbox leftBorder, Hitbox rightBorder, Hitbox bottomBorder, Hitbox leftCorner, Hitbox rightCorner, Hitbox title, Hitbox close, Hitbox minimize, Hitbox maximize, bool isFocused)
        {
            graphics.FillRectangle(title.X, title.Y, _titlebarleft.Width, title.Height, _titlebarleft, Color.White);
            graphics.FillRectangle(title.X + (title.Width - _titlebarright.Width), title.Y, _titlebarright.Width, title.Height, _titlebarright, Color.White);
            graphics.FillRectangle(title.X + _titlebarleft.Width, title.Y, (title.Width - _titlebarleft.Width) - _titlebarright.Width, title.Height, _titlebar, Color.White);

            graphics.FillRectangle(leftBorder.X, leftBorder.Y, 5, leftBorder.Height, _borderleft, Color.White);
            graphics.FillRectangle(rightBorder.X, rightBorder.Y, 5, rightBorder.Height, _borderright, Color.White);
            graphics.FillRectangle(bottomBorder.X, bottomBorder.Y, bottomBorder.Width, 5, _borderbottom, Color.White);

            graphics.FillRectangle(leftCorner.X, leftCorner.Y, leftCorner.Width, leftCorner.Height, _cornerleft, Color.White);
            graphics.FillRectangle(rightCorner.X, rightCorner.Y, rightCorner.Width, rightCorner.Height, _cornerright, Color.White);

            graphics.DrawString(titletext, new Vector2(26, 3), Color.White, GetFont(TextFontStyle.Highlight), TextAlignment.Left);

            if(close.Visible)
                graphics.FillRectangle(close.X, close.Y, close.Width, close.Height, _close, Color.White);
            if(minimize.Visible)
                graphics.FillRectangle(maximize.X, maximize.Y, maximize.Width, maximize.Height, _maximize, Color.White);
            if (maximize.Visible)
                graphics.FillRectangle(minimize.X, minimize.Y, minimize.Width, minimize.Height, _minimize, Color.White);


        }

        public override Color GetAccentColor()
        {
            return Color.Gray;
        }

        public override Color GetFontColor(TextFontStyle style)
        {
            return Color.Black;
        }

        public override Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight)
        {
            switch(button)
            {
                case TitleButton.Close:
                    return new Rectangle(windowWidth - 21, 3, 17, 17);
                case TitleButton.Maximize:
                    return new Rectangle(windowWidth - 40, 3, 17, 17);
                case TitleButton.Minimize:
                    return new Rectangle(windowWidth - 59, 3, 17, 17);
            }
            return Rectangle.Empty;
        }

        public override void LoadThemeData(GraphicsDevice device, ContentManager content)
        {
            _titlebar = content.Load<Texture2D>("ThemeAssets/Industrial/TitleBar");
            _titlebarleft = content.Load<Texture2D>("ThemeAssets/Industrial/titlebarleft");
            _titlebarright = content.Load<Texture2D>("ThemeAssets/Industrial/titlebarright");

            _borderbottom = content.Load<Texture2D>("ThemeAssets/Industrial/bottomborder");
            _borderleft = content.Load<Texture2D>("ThemeAssets/Industrial/leftborder");
            _borderright = content.Load<Texture2D>("ThemeAssets/Industrial/rightborder");

            _cornerleft = content.Load<Texture2D>("ThemeAssets/Industrial/bottomleftcorner");
            _cornerright = content.Load<Texture2D>("ThemeAssets/Industrial/bottomrightcorner");

            _close = content.Load<Texture2D>("ThemeAssets/Industrial/close button");
            _minimize = content.Load<Texture2D>("ThemeAssets/Industrial/minimizebutton");
            _maximize = content.Load<Texture2D>("ThemeAssets/Industrial/maximize button");


            _check = content.Load<Texture2D>("ThemeAssets/CheckBox/check");
            base.LoadThemeData(device, content);
        }

        public override void UnloadThemeData()
        {
            base.UnloadThemeData();
        }
    }
}
