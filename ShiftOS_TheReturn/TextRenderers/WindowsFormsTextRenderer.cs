﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Frontend.GraphicsSubsystem;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GUI;

namespace Plex.Engine.TextRenderers
{
    /// <summary>
    /// Provides software text rendering for the UI subsystem using Windows Forms' <see cref="System.Windows.Forms.TextRenderer"/> class. (Slow) 
    /// </summary>
    public class WindowsFormsTextRenderer : ATextRenderer
    {
        public override void DrawText(GraphicsContext gfx, int x, int y, string text, Font font, Microsoft.Xna.Framework.Color color, int maxwidth, TextAlignment alignment)
        {
            var measure = MeasureText(text, font, maxwidth, alignment);
            if ((int)measure.X == 0 || (int)measure.Y == 0)
                return;
            using (var bmp = new System.Drawing.Bitmap((int)measure.X, (int)measure.Y))
            {
                using (var cgfx = System.Drawing.Graphics.FromImage(bmp))
                {
                    cgfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    cgfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    cgfx.Clear(System.Drawing.Color.Transparent);
                    System.Windows.Forms.TextRenderer.DrawText(cgfx, text, font, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Color.White, System.Drawing.Color.Black, GetFlags(alignment));
                }
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                var bytes = new byte[Math.Abs(lck.Stride) * lck.Height];
                Marshal.Copy(lck.Scan0, bytes, 0, bytes.Length);
                for(int i = 0; i < bytes.Length; i += 4)
                {
                    var avg = (byte)(((int)bytes[i] + (int)bytes[i + 1] + (int)bytes[i + 2]) / 3);
                    if (avg < 127)
                        bytes[i + 3] = avg;
                }
                bmp.UnlockBits(lck);
                using (var tex2 = new Texture2D(gfx.Device, bmp.Width, bmp.Height))
                {
                    tex2.SetData<byte>(bytes);
                    gfx.DrawRectangle(x, y, bmp.Width, bmp.Height, tex2, color, ImageLayout.Stretch);
                }

            }
        }

        public readonly TextFormatFlags baseFlags = TextFormatFlags.NoPadding | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

        public TextFormatFlags GetFlags(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.TopLeft:
                default:
                    return baseFlags | TextFormatFlags.Top | TextFormatFlags.Left;
                case TextAlignment.Top:
                    return baseFlags | TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                case TextAlignment.TopRight:
                    return baseFlags | TextFormatFlags.Top | TextFormatFlags.Right;
                case TextAlignment.Left:
                    return baseFlags | TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                case TextAlignment.Middle:
                    return baseFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                case TextAlignment.Right:
                    return baseFlags | TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                case TextAlignment.BottomLeft:
                    return baseFlags | TextFormatFlags.Bottom | TextFormatFlags.Left;
                case TextAlignment.Bottom:
                    return baseFlags | TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                case TextAlignment.BottomRight:
                    return baseFlags | TextFormatFlags.Bottom | TextFormatFlags.Right;
            }
        }

        public override Vector2 MeasureText(string text, Font font, int maxwidth, TextAlignment alignment)
        {
            using (var gfx = Graphics.FromHwnd(IntPtr.Zero))
            {
                var measure = System.Windows.Forms.TextRenderer.MeasureText(gfx, text, font, new System.Drawing.Size(maxwidth, int.MaxValue), GetFlags(alignment));
                return new Vector2(measure.Width, measure.Height);
            }

        }
    }
}
