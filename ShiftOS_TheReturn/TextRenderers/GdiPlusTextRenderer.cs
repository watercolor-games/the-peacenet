using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.TextRenderers
{
    /// <summary>
    /// Provides GDI+ software text rendering for the UI subsystem. (Slow)
    /// </summary>
    [FallbackRenderer]
    public class GdiPlusTextRenderer : ATextRenderer
    {
        public override Texture2D DrawText(GraphicsContext gfx, string text, Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            var measure = MeasureText(text, font, maxwidth, alignment, wrapMode);
            using(var bmp = new Bitmap((int)measure.X, (int)measure.Y))
            {
                using(var cgfx = Graphics.FromImage(bmp))
                {
                    cgfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    cgfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    cgfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    var format = StringFormat.GenericDefault;
                    format.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip;
                    if (wrapMode == WrapMode.None)
                        format.FormatFlags |= StringFormatFlags.NoWrap;

                    cgfx.DrawString(text, font, Brushes.Black, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), format);

                }
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                var bytes = new byte[Math.Abs(lck.Stride) * lck.Height];
                Marshal.Copy(lck.Scan0, bytes, 0, bytes.Length);
                for(int i = 0; i < bytes.Length; i += 4)
                {
                    bytes[i] = (byte)(255 - bytes[i]);
                    bytes[i+1] = (byte)(255 - bytes[i+1]);
                    bytes[i+2] = (byte)(255 - bytes[i+2]);

                }
                bmp.UnlockBits(lck);
                var tex2 = new Texture2D(gfx.Device, bmp.Width, bmp.Height);
                tex2.SetData<byte>(bytes);
                return tex2;
            }
        }

        public override Vector2 MeasureText(string text, Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            using(var gfx = Graphics.FromHwnd(IntPtr.Zero))
            {
                var format = StringFormat.GenericDefault;
                format.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip;
                if (wrapMode == WrapMode.None)
                    format.FormatFlags |= StringFormatFlags.NoWrap;
                var measure = gfx.MeasureString(text, font, maxwidth, format);
                return new Vector2(measure.Width, measure.Height);
            }
        }
    }
}
