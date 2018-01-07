using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.TextRenderers;
using Plex.Engine.GraphicsSubsystem;
using Plex.Objects;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine
{
    /// <summary>
    /// A class implementing a custom text renderer.
    /// </summary>
    public abstract class ATextRenderer
    {
        /// <summary>
        /// Measures a specified string to get its size in pixels.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The font to measure the text with.</param>
        /// <param name="maxwidth">The maximum width (in pixels) that text can be before it is wrapped.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="wrapMode">The wrap mode of the text.</param>
        /// <returns>A <see cref="Vector2"/> representing the size of the text in pixels.</returns>
        public abstract Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
        /// <summary>
        /// Render text to a MonoGame texture.
        /// </summary>
        /// <param name="gfx">The graphics context to render the text to.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to render the text in.</param>
        /// <param name="maxwidth">The maximum width text can be before it is wrapped.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="wrapMode">The type of text wrapping to use.</param>
        /// <returns>The rendered texture.</returns>
        public abstract Texture2D DrawText(GraphicsContext gfx, string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
    }

    /// <summary>
    /// A class for rendering text.
    /// </summary>
    public static class TextRenderer
    {
        private static ATextRenderer _renderer = null;
        private static List<ATextRenderer> _renderers = null;

        private static void _getAllRenderers()
        {
            _renderers = new List<ATextRenderer>();
            foreach (var renderer in ReflectMan.Types.Where(x => x.BaseType == typeof(ATextRenderer)))
            {
                try
                {
                    _renderers.Add((ATextRenderer)Activator.CreateInstance(renderer, null));
                }
                catch { }
            }
        }

        /// <summary>
        /// Retrieve the default text renderer.
        /// </summary>
        /// <returns>The default renderer to use. Returns null if none were found.</returns>
        public static ATextRenderer GetDefaultRenderer()
        {
            if (_renderers == null)
                _getAllRenderers();
            return _renderers.FirstOrDefault(x=>x.GetType().GetCustomAttributes(false).FirstOrDefault(y=>y is DefaultRenderer) != null);
        }

        /// <summary>
        /// Retrieves the fallback renderer to use if a default renderer couldn't be found.
        /// </summary>
        /// <returns>The fallback renderer. Returns null if none were found.</returns>
        public static ATextRenderer GetFallbackRenderer()
        {
            if (_renderers == null)
                _getAllRenderers();
            return _renderers.FirstOrDefault(x => x.GetType().GetCustomAttributes(false).FirstOrDefault(y => y is FallbackRenderer) != null);
        }

        /// <summary>
        /// Retrieves an array containing all available text renderers.
        /// </summary>
        public static ATextRenderer[] GetAvailableRenderers
        {
            get
            {
                return _renderers.ToArray();
            }
        }

        internal static void Init(ATextRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <summary>
        /// Measure a string of text to get its width and height in pixels.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="font">The font to measure with</param>
        /// <param name="maxwidth">The maximum width text can be before it is wrapped</param>
        /// <param name="alignment">The alignment of the text</param>
        /// <param name="wrapMode">The wrap mode to use</param>
        /// <returns>The size in pixels of the text.</returns>
        public static Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            return _renderer.MeasureText(text, font, maxwidth, alignment, wrapMode);
        }

        /// <summary>
        /// Render a string of text.
        /// </summary>
        /// <param name="gfx">The graphics context to render the text to.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="font">The font to render the text in.</param>
        /// <param name="maxwidth">The maximum width text can be before it is wrapped.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="wrapMode">The type of text wrapping to use.</param>
        public static void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Color color, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            var texture = _renderer.DrawText(gfx, text, font, maxwidth, alignment, wrapMode);
            if (texture == null)
                return;
            gfx.DrawRectangle(x, y, texture.Width, texture.Height, texture, color, System.Windows.Forms.ImageLayout.Stretch, false, false);

        }
    }

    /// <summary>
    /// Describes how text should be aligned when rendered or measured.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// The text is aligned to the top-left of the render bounds.
        /// </summary>
        TopLeft,
        /// <summary>
        /// The text is aligned to the top-center of the render bounds.
        /// </summary>
        Top,
        /// <summary>
        /// The text is aligned to the top-right of the render bounds.
        /// </summary>
        TopRight,
        /// <summary>
        /// The text is aligned to the left of the render bounds.
        /// </summary>
        Left,
        /// <summary>
        /// The text is aligned to the middle of the render bounds.
        /// </summary>
        Middle,
        /// <summary>
        /// The text is aligned to the right of the render bounds.
        /// </summary>
        Right,
        /// <summary>
        /// The text is aligned to the bottom-left of the render bounds.
        /// </summary>
        BottomLeft,
        /// <summary>
        /// The text is aligned to the bottom-center of the render bounds.
        /// </summary>
        Bottom,
        /// <summary>
        /// The text is aligned to the bottom-right of the render bounds.
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Indicates that this <see cref="ATextRenderer"/> should be the default renderer for the game. 
    /// </summary>
    public class DefaultRenderer : Attribute
    {

    }

    /// <summary>
    /// Indicates that this <see cref="ATextRenderer"/> should be the fallback renderer for the game. 
    /// </summary>
    public class FallbackRenderer : Attribute
    {

    }
}
