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
    /// <summary>
    /// A class containing methods to support rendering of themed user interface elements.
    /// </summary>
    public abstract class Theme
    {
        /// <summary>
        /// Load theme resources into memory.
        /// </summary>
        /// <param name="device">A <see cref="GraphicsDevice"/> representing the game's primary graphics device - useful for creating textures at runtime for your theme.</param>
        /// <param name="content">A <see cref="ContentManager"/> for loading baked assets into your theme.</param>
        public abstract void LoadThemeData(GraphicsDevice device, ContentManager content);

        /// <summary>
        /// Unload theme resources from memory.
        /// </summary>
        public abstract void UnloadThemeData();

        /// <summary>
        /// Retrieves an accent color for the theme.
        /// </summary>
        /// <returns>A <see cref="Color"/> representing the theme's accent color.</returns>
        public abstract Color GetAccentColor();

        /// <summary>
        /// Draw a themed button.
        /// </summary>
        /// <param name="gfx">The graphics context to render the button on.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="image">The icon for the button.</param>
        /// <param name="state">The button's mouse state.</param>
        /// <param name="showImage">Whether an icon should even be rendered.</param>
        /// <param name="imageRect">The region to render the icon in.</param>
        /// <param name="textRect">The region to render text in.</param>
        public abstract void DrawButton(GraphicsContext gfx, string text, Texture2D image, UIButtonState state, bool showImage, Rectangle imageRect, Rectangle textRect);
        /// <summary>
        /// Draw an arrow poonting in a given direction.
        /// </summary>
        /// <param name="gfx">The graphics context to render the arrow on.</param>
        /// <param name="x">The X coordinate of the arrow.</param>
        /// <param name="y">The Y coordinate of the arrow.</param>
        /// <param name="width">The width of the arrow.</param>
        /// <param name="height">The height of the arrow.</param>
        /// <param name="state">The arrow's mouse state.</param>
        /// <param name="direction">The direction of the arrow.</param>
        public abstract void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, UIButtonState state, ArrowDirection direction);
        /// <summary>
        /// Draw a themed text caret rectangle.
        /// </summary>
        /// <param name="graphics">The graphics device to render the caret on.</param>
        /// <param name="x">The X coordinate of the caret</param>
        /// <param name="y">The Y coordinate of the caret</param>
        /// <param name="width">The width of the caret</param>
        /// <param name="height">The height of the caret</param>
        public abstract void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height);
        /// <summary>
        /// Draw a themed control's background.
        /// </summary>
        /// <param name="graphics">The graphics context to render the background on.</param>
        /// <param name="x">The X coordinate of the background.</param>
        /// <param name="y">The Y coordinate of the background.</param>
        /// <param name="width">The width of the background.</param>
        /// <param name="height">The height of the background.</param>
        public abstract void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height);
        /// <summary>
        /// Draw a darker-themed control's background.
        /// </summary>
        /// <param name="graphics">The graphics context to render the background on.</param>
        /// <param name="x">The X coordinate of the background.</param>
        /// <param name="y">The Y coordinate of the background.</param>
        /// <param name="width">The width of the background.</param>
        /// <param name="height">The height of the background.</param>
        public abstract void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height);
        /// <summary>
        /// Draw a lighter-themed control's background.
        /// </summary>
        /// <param name="graphics">The graphics context to render the background on.</param>
        /// <param name="x">The X coordinate of the background.</param>
        /// <param name="y">The Y coordinate of the background.</param>
        /// <param name="width">The width of the background.</param>
        /// <param name="height">The height of the background.</param>
        public abstract void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height);
        /// <summary>
        /// Draw a themed text string.
        /// </summary>
        /// <param name="graphics">The graphics context to render the string on.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="style">The text style.</param>
        public abstract void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style);
        /// <summary>
        /// Draw a themed text string that has a mouse state.
        /// </summary>
        /// <param name="graphics">The graphics context to render the string on.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="style">The text style.</param>
        /// <param name="state">The text's mouse state</param>
        public abstract void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style, UIButtonState state);
        /// <summary>
        /// Draw a themed text string that's disabled.
        /// </summary>
        /// <param name="graphics">The graphics context to render the string on.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="style">The text style.</param>
        public abstract void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextFontStyle style);
        /// <summary>
        /// Draw a themed window border.
        /// </summary>
        /// <param name="graphics">The graphics context to render the window border on.</param>
        /// <param name="titletext">The window's title text.</param>
        /// <param name="leftBorder">A hitbox for the left border.</param>
        /// <param name="rightBorder">A hitbox for the right border.</param>
        /// <param name="bottomBorder">A hitbox for the bottom border.</param>
        /// <param name="leftCorner">A hitbox for the bottom left corner.</param>
        /// <param name="rightCorner">A hitbox for the bottom right corner.</param>
        /// <param name="title">A hitbox for the top ("title") border.</param>
        /// <param name="close">A hitbox for the close button.</param>
        /// <param name="minimize">A hitbox for the minimize button.</param>
        /// <param name="maximize">A hitbox for the maximize button.</param>
        /// <param name="isFocused">Whether the window is focused.</param>
        public abstract void DrawWindowBorder(GraphicsContext graphics, string titletext, Hitbox leftBorder, Hitbox rightBorder, Hitbox bottomBorder, Hitbox leftCorner, Hitbox rightCorner, Hitbox title, Hitbox close, Hitbox minimize, Hitbox maximize, bool isFocused);
        /// <summary>
        /// Get a rectangle representing where a given title button should be displayed in a window.
        /// </summary>
        /// <param name="button">The type of title button to measure.</param>
        /// <param name="windowWidth">The window's width</param>
        /// <param name="windowHeight">The window's height</param>
        /// <returns>The coordinates and size of the title button, represented by a <see cref="Rectangle"/>.</returns>
        public abstract Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight);
        /// <summary>
        /// Render a themed check box.
        /// </summary>
        /// <param name="gfx">The graphics context to render the checkbox on.</param>
        /// <param name="x">The X coordinate of the check box.</param>
        /// <param name="y">The Y coordinate of the check box.</param>
        /// <param name="width">The width of the checkbox.</param>
        /// <param name="height">The height of the checkbox.</param>
        /// <param name="isChecked">Whether the checkbox is checked.</param>
        /// <param name="isMouseOver">Whether the checkbox is hovered over by the mouse.</param>
        public abstract void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver);
        /// <summary>
        /// Get a font from a font style.
        /// </summary>
        /// <param name="style">The font style to use</param>
        /// <returns>A <see cref="System.Drawing.Font"/> representing the name, size and other properties of the font.</returns>
        public abstract SpriteFont GetFont(TextFontStyle style);
        /// <summary>
        /// Get the color of a font based on the font style.
        /// </summary>
        /// <param name="style">The font style to use</param>
        /// <returns>The <see cref="Color"/> of any text rendered with this font style</returns>
        public abstract Color GetFontColor(TextFontStyle style);

        /// <summary>
        /// Retrieves the height in pixels of the window title bar.
        /// </summary>
        public abstract int WindowTitleHeight { get; }
        

        /// <summary>
        /// Retrieves the width in pixels of each window border.
        /// </summary>
        public abstract int WindowBorderWidth { get; }
    }

    /// <summary>
    /// Indicates that a <see cref="Theme"/> object should only be used if the game doesn't have its own theme. 
    /// </summary>
    public class DummyThemeAttribute : Attribute
    { }

    /// <summary>
    /// Represents a mouse state for a <see cref="Button"/> or other UI element. 
    /// </summary>
    public enum UIButtonState
    {
        /// <summary>
        /// The mouse is not doing anything with the UI element.
        /// </summary>
        Idle,
        /// <summary>
        /// The mouse is hovering over the UI element.
        /// </summary>
        Hover,
        /// <summary>
        /// The mouse is clicking and holding onto the UI element.
        /// </summary>
        Pressed
    }

    /// <summary>
    /// Represents a font and color to use for rendering a string of text.
    /// </summary>
    public enum TextFontStyle
    {
        /// <summary>
        /// The text is regular user interface text and shouldn't be too fancy.
        /// </summary>
        System,
        /// <summary>
        /// The text is monospace (such as code) and should be rendered with a fixed-width font and should stand out.
        /// </summary>
        Mono,
        /// <summary>
        /// The text is a first-level heading (title) and should be rendered with a larger font and accent color to stand out to the eye.
        /// </summary>
        Header1,
        /// <summary>
        /// The text is a second-level (subtitle) heading and should be rendered with a moderately larger font to stand out to the eye.
        /// </summary>
        Header2,
        /// <summary>
        /// The text is a third-level (emphasis) heading and should stand out to the eye with an ever-so-slightly larger font.
        /// </summary>
        Header3,
        /// <summary>
        /// No themed or preset font style is present for this text.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Represents a window title button.
    /// </summary>
    public enum TitleButton
    {
        /// <summary>
        /// Represents the window's Close button.
        /// </summary>
        Close,
        /// <summary>
        /// Represents a window's Minimize button.
        /// </summary>
        Minimize,
        /// <summary>
        /// Represents a window's Maximize/Restore button.
        /// </summary>
        Maximize
    }

    /// <summary>
    /// Represents a direction which an arrow should point.
    /// </summary>
    public enum ArrowDirection
    {
        /// <summary>
        /// The arrow points upwards.
        /// </summary>
        Up,
        /// <summary>
        /// The arrow points to the left.
        /// </summary>
        Left,
        /// <summary>
        /// The arrow points downwards.
        /// </summary>
        Down,
        /// <summary>
        /// The arrow points to the right.
        /// </summary>
        Right
    }
}
