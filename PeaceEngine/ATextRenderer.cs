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
    /// A class for rendering text.
    /// </summary>
    public static class TextRenderer
    {
        private static string WrapLine(SpriteFont font, string text, float maxLineWidth)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (font.MeasureString(text).X <= maxLineWidth)
                return text;
            text = text.Trim();
            var sb = new StringBuilder();
            
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Perform text wrapping on a string of text using the specified <see cref="SpriteFont"/>.
        /// </summary>
        /// <param name="font">The <see cref="SpriteFont"/> representing the font to measure the text in.</param>
        /// <param name="text">The text to wrap.</param>
        /// <param name="maxLineWidth">The maximum width (in pixels) that a line of text can be.</param>
        /// <param name="mode">The type of text wrapping to apply.</param>
        /// <returns>The resulting wrapped text.</returns>
        public static string WrapText(SpriteFont font, string text, float maxLineWidth, WrapMode mode)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (font.MeasureString(text).X <= maxLineWidth)
                return text;
            if (mode == WrapMode.Words)
            {
                float spacewidth = font.MeasureString(" ").X;
                if (maxLineWidth < spacewidth)
                    return text;
                text = text.Trim().Replace("\r", "");
                string[] lines = text.Split('\n');
                float lineWidth = 0;
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    lineWidth = 0;
                    if (sb.Length>0)
                        sb.Append("\n");
                    var words = line.Split(' ').ToList();
                    for (int i = 0; i < words.Count; i++)
                    {
                        string word = words[i];

                        Vector2 size = font.MeasureString(word);

                        if (lineWidth + size.X <= maxLineWidth)
                        {

                            sb.Append(word + " ");
                            lineWidth += size.X + spacewidth;
                        }
                        else
                        {
                            if (size.X >= maxLineWidth)
                            {
                                if (sb.Length>0)
                                    sb.Append("\n");
                                int half = word.Length / 2;
                                string first = word.Substring(0, half);
                                string second = word.Substring(half);
                                words[i] = first;
                                words.Insert(i + 1, second);
                                i--;
                                continue;
                            }
                            else
                            {
                                sb.Append("\n" + word + " ");
                                lineWidth = size.X + spacewidth;
                            }
                        }
                    }

                }
                return sb.ToString().TrimEnd();
            }
            else
            {
                float lineWidth = 0;
                string newstr = "";
                foreach (char c in text)
                {
                    var measure = font.MeasureString(c.ToString());
                    if(lineWidth + measure.X > maxLineWidth)
                    {
                        newstr += "\n";
                        lineWidth = 0;
                    }
                    else
                    {
                        if (c == '\n')
                        {
                            lineWidth = 0;
                        }
                        else
                            lineWidth += measure.X;
                    }
                    newstr += c;
                }
                return newstr;
            }

        }

        /// <summary>
        /// Measure a string of text to get its width and height in pixels.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="font">The font to measure with</param>
        /// <param name="maxwidth">The maximum width text can be before it is wrapped</param>
        /// <param name="wrapMode">The wrap mode to use</param>
        /// <returns>The size in pixels of the text.</returns>
        public static Vector2 MeasureText(string text, SpriteFont font, int maxwidth, WrapMode wrapMode)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;
            switch (wrapMode)
            {
                case WrapMode.None:
                    return font.MeasureString(text);
                default:
                    return font.MeasureString(WrapText(font, text, maxwidth, wrapMode));
            }
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
        /// <param name="color">The color of the text to render</param>
        public static void DrawText(GraphicsContext gfx, int x, int y, string text, SpriteFont font, Color color, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            if (string.IsNullOrEmpty(text))
                return;
            string measured = (wrapMode == WrapMode.None) ? text : WrapText(font, text, maxwidth, wrapMode);
            string[] lines = measured.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var measure = font.MeasureString(line);
                switch (alignment)
                {
                    case TextAlignment.Left:
                        gfx.Batch.DrawString(font, line, new Vector2(x, y + (measure.Y * i)), color);
                        break;
                    case TextAlignment.Center:
                        gfx.Batch.DrawString(font, line, new Vector2(x + ((maxwidth-measure.X)/2), y + (measure.Y * i)), color);
                        break;
                    case TextAlignment.Right:
                        gfx.Batch.DrawString(font, line, new Vector2(x + (maxwidth - measure.X), y + (measure.Y * i)), color);
                        break;
                }
            }

        }


    }

    /// <summary>
    /// Describes how text should be aligned when rendered.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// Text should be aligned to the centre of the render bounds.
        /// </summary>
        Center = 0,
        /// <summary>
        /// Text should be aligned to the left.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Text should be rendered to the right.
        /// </summary>
        Right = 2,
    }

    /// <summary>
    /// Indicates that this <see cref="ATextRenderer"/> should be the default renderer for the game. 
    /// </summary>
    [Obsolete("GDI fonts no longer supported.")]
    public class DefaultRenderer : Attribute
    {

    }

    /// <summary>
    /// Indicates that this <see cref="ATextRenderer"/> should be the fallback renderer for the game. 
    /// </summary>
    [Obsolete("GDI fonts no longer supported.")]
    public class FallbackRenderer : Attribute
    {

    }
}
