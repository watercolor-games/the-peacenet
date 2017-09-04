using System;
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

namespace Plex.Engine.TextRenderers
{
    /// <summary>
    /// Provides software text rendering for the UI subsystem using Windows Forms' <see cref="System.Windows.Forms.TextRenderer"/> class. (Slow) 
    /// </summary>
    public class WindowsFormsTextRenderer : ATextRenderer
    {
        public override void DrawText(GraphicsContext gfx, int x, int y, string text, Font font, Microsoft.Xna.Framework.Color color, int maxwidth)
        {
            var measure = MeasureText(text, font, maxwidth);
            using (var bmp = new System.Drawing.Bitmap((int)measure.X, (int)measure.Y))
            {
                using (var cgfx = System.Drawing.Graphics.FromImage(bmp))
                {
                    cgfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    System.Windows.Forms.TextRenderer.DrawText(cgfx, text, font, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Color.White, TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);

                }
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                var bytes = new byte[Math.Abs(lck.Stride) * lck.Height];
                Marshal.Copy(lck.Scan0, bytes, 0, bytes.Length);
                bmp.UnlockBits(lck);
                using (var tex2 = new Texture2D(gfx.Device, bmp.Width, bmp.Height))
                {
                    tex2.SetData<byte>(bytes);

                    gfx.DrawRectangle(x, y, bmp.Width, bmp.Height, tex2, color, ImageLayout.Stretch);
                }

            }
        }

        public override Vector2 MeasureText(string text, Font font, int maxwidth)
        {
            var measure = System.Windows.Forms.TextRenderer.MeasureText(text, font, new System.Drawing.Size(maxwidth, int.MaxValue), TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            return new Vector2(measure.Width, measure.Height);

        }
    }
}
