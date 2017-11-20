using System;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.TextRenderers
{

    public enum WrapMode
    {
        /// <summary>
        /// Text is drawn as-is, with no maximum wrapwidth whatsoever.
        /// </summary>
        None = 0,
        /// <summary>
        /// Text is drawn using a wrapwidth to drop text to a new line when its width exceeds that of the wrapwidth. Ignored on GDI.
        /// </summary>
        Letters = 1,

        /// <summary>
        /// Text is drawn using a wrapwidth to drop text to a new line when its width exceeds that of the wrapwidth, preserving whole words.
        /// </summary>
        Words = 2
    }

    /// <summary>
    /// Provides text rendering for the UI subsystem from PlexNative.
    /// </summary>
    [DefaultRenderer]
    public class NativeTextRenderer : ATextRenderer
	{
		private static class Implementation
		{
			[System.Runtime.InteropServices.DllImport("PlexNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
			public static extern long MeasureString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth);
			
			[System.Runtime.InteropServices.DllImport("PlexNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
			public static extern void DrawString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth, int w, int h, byte[] buffer);
			
			
		}
        public NativeTextRenderer()
        {
            // Throw an exception if there was a DllImport failure so
            // that the engine can choose a different renderer.
            System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(Implementation));
        }
		
		public override Microsoft.Xna.Framework.Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, Plex.Engine.GUI.TextAlignment alignment, WrapMode wrapMode)
		{
            long result = -1;
            result = Implementation.MeasureString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)wrapMode, maxwidth);
            return new Microsoft.Xna.Framework.Vector2((int)(result & uint.MaxValue), (int)(result >> 32));
		}
		
		public override void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Microsoft.Xna.Framework.Color color, int maxwidth, Plex.Engine.GUI.TextAlignment alignment, WrapMode wrapMode)
		{
			var measure = MeasureText(text, font, maxwidth, alignment, wrapMode);
			var data = new byte[(int)measure.X * (int)measure.Y * 4];
			if (data.Length == 0)
				return;
            Implementation.DrawString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)wrapMode, maxwidth, (int)measure.X, (int)measure.Y, data);
            var tex2 = new Microsoft.Xna.Framework.Graphics.Texture2D(gfx.Device, (int)measure.X, (int)measure.Y);
			tex2.SetData<byte>(data);
            gfx.DrawRectangle(x, y, (int)measure.X, (int)measure.Y, tex2, color, System.Windows.Forms.ImageLayout.Stretch, true);
		}
	}
}

