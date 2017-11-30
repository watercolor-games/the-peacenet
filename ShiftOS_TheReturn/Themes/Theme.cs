using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Themes
{
    public abstract class Theme
    {
        public abstract void LoadThemeData(GraphicsDevice device, ContentManager content);
        public abstract void UnloadThemeData();

        public abstract Color GetAccentColor();

        //Rendering
        public abstract void DrawButtonBackground(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state);
        public abstract void DrawButtonImage(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, Texture2D image);
        public abstract void DrawButtonText(GraphicsContext gfx, string text, int x, int y, int width, int height, UIButtonState state);
        public abstract void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, ArrowDirection direction);
        public abstract void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style);
        public abstract void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style, UIButtonState state);
        public abstract void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style);
        public abstract void DrawWindowBorder(GraphicsContext graphics, string titletext, Hitbox leftBorder, Hitbox rightBorder, Hitbox bottomBorder, Hitbox leftCorner, Hitbox rightCorner, Hitbox title, Hitbox close, Hitbox minimize, Hitbox maximize, bool isFocused);
        public abstract Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight);
        public abstract void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver);
        public abstract System.Drawing.Font GetFont(TextFontStyle style);
        public abstract Color GetFontColor(TextFontStyle style);

        //Measurement
        public abstract Vector2 MeasureString(TextFontStyle style, string text, TextAlignment alignment = TextAlignment.TopLeft, int maxwidth = int.MaxValue);
    }

    public class DummyThemeAttribute : Attribute
    { }

    public enum UIButtonState
    {
        Idle,
        Hover,
        Pressed
    }

    public enum TextFontStyle
    {
        System,
        Mono,
        Header1,
        Header2,
        Header3,
        Custom
    }

    public enum TitleButton
    {
        Close,
        Minimize,
        Maximize
    }

    public enum ArrowDirection
    {
        Up,
        Left,
        Down,
        Right
    }
}
