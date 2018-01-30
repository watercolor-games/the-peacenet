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
        private static string WrapText(SpriteFont font, string text, float maxLineWidth, WrapMode mode)
        {
            if (mode == WrapMode.Words)
            {
                text = text.Trim();
                var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                StringBuilder sb = new StringBuilder();
                float lineWidth = 0;
                float spaceWidth = font.MeasureString(" ").X;

                for(int i = 0; i < words.Count; i++)
                {
                    string word = words[i].Replace("\r", "");
                    bool insertLineEnding = false;

                    if (word.Contains("\n"))
                    {
                        int indexOfNewline = word.IndexOf('\n');
                        string nextWord = word.Substring(indexOfNewline+1);
                        word = word.Substring(0, indexOfNewline);
                        if(nextWord.Length>0)
                            words.Insert(i + 1, nextWord);
                        if (word.Length < 1)
                        {
                            sb.Append("\n");
                            lineWidth = 0;
                            continue;
                        }
                        insertLineEnding = true;
                    }


                    Vector2 size = font.MeasureString(word);

                    if (lineWidth + size.X <= maxLineWidth) 
                    {
                        sb.Append(word + " ");
                        lineWidth += size.X + spaceWidth;
                    }
                    else
                    {
                        if (size.X >= maxLineWidth)
                        {
                            if (sb.ToString() == " ")
                            {
                                sb.Append(WrapText(font, word.Insert(word.Length / 2, " "), maxLineWidth, mode));
                            }
                            else
                            {
                                sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " "), maxLineWidth, mode));
                            }
                            
                        }
                        else
                        {
                            sb.Append("\n" + word + " ");
                            lineWidth = 0;
                        }
                    }
                    if(insertLineEnding)
                    {
                        sb.Append("\n");
                        lineWidth = 0;
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
            switch(wrapMode)
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
            string measured = (wrapMode == WrapMode.None) ? text : WrapText(font, text, maxwidth, wrapMode);
            Vector2 size = font.MeasureString(measured);
            var bounds = new Rectangle(x, y, maxwidth, (int)(size.Y));
            Vector2 pos = bounds.Center.ToVector2();
            Vector2 origin = size * 0.5f;

            if (alignment.HasFlag(TextAlignment.Left))
                origin.X += bounds.Width / 2 - size.X / 2;

            if (alignment.HasFlag(TextAlignment.Right))
                origin.X -= bounds.Width / 2 - size.X / 2;

            if (alignment.HasFlag(TextAlignment.Top))
                origin.Y += bounds.Height / 2 - size.Y / 2;

            if (alignment.HasFlag(TextAlignment.Bottom))
                origin.Y -= bounds.Height / 2 - size.Y / 2;

            gfx.Batch.DrawString(font, measured, pos, color, 0, origin, 1, SpriteEffects.None, 0);


        }


    }

    /// <summary>
    /// Describes how text should be aligned when rendered.
    /// </summary>
    [Flags]
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
        /// <summary>
        /// Text should be rendered to the top.
        /// </summary>
        Top = 4,
        /// <summary>
        /// Text should be rendered to the bottom.
        /// </summary>
        Bottom = 8
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
