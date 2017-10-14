using System;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Engine.TextRenderers
{
	/// <summary>
	/// Provides text rendering for the UI subsystem from PlexNative.
	/// </summary>
	public class NativeTextRenderer : ATextRenderer
	{
		private static class Implementation
		{
			[System.Runtime.InteropServices.DllImport("PlexNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
			public static extern long MeasureString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth);
			
			[System.Runtime.InteropServices.DllImport("PlexNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
			public static extern void DrawString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth, double r, double g, double b, double a, int w, int h, byte[] buffer);
			
			public enum WrapMode
			{
				None = 0,
				Letters = 1,
				Words = 2
			}
		}
		
        public static class ImplementationForWindowsBecauseGodForbidWeOmitTheDllExtensionInTheFuckingFilenameFuckYouMicrosoft
        {
            [System.Runtime.InteropServices.DllImport("PlexNative.dll")]
            public static extern long MeasureString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth);

            [System.Runtime.InteropServices.DllImport("PlexNative.dll")]
            public static extern void DrawString(string text, int textlen, string typeface, int typefacelen, double pointsize, int styles, int alignment, int wrapmode, int wrapwidth, double r, double g, double b, double a, int w, int h, byte[] buffer);

        }

        public NativeTextRenderer()
        {
            // Throw an exception if there was a DllImport failure so
            // that the engine can choose a different renderer.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(ImplementationForWindowsBecauseGodForbidWeOmitTheDllExtensionInTheFuckingFilenameFuckYouMicrosoft));
            }
            else
            {
                System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(Implementation));
            }
        }
		
		public override Microsoft.Xna.Framework.Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, Plex.Engine.GUI.TextAlignment alignment)
		{
            long result = -1;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                result  = ImplementationForWindowsBecauseGodForbidWeOmitTheDllExtensionInTheFuckingFilenameFuckYouMicrosoft.MeasureString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth);
            }
            else
            {
                result = Implementation.MeasureString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth);
            }
            return new Microsoft.Xna.Framework.Vector2((int)(result & uint.MaxValue), (int)(result >> 32));
		}
		
		public override void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Microsoft.Xna.Framework.Color color, int maxwidth, Plex.Engine.GUI.TextAlignment alignment)
		{
			var measure = MeasureText(text, font, maxwidth, alignment);
			var data = new byte[(int)measure.X * (int)measure.Y * 4];
			if (data.Length == 0)
				return;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ImplementationForWindowsBecauseGodForbidWeOmitTheDllExtensionInTheFuckingFilenameFuckYouMicrosoft.DrawString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth, color.B / 255.0, color.G / 255.0, color.R / 255.0, color.A / 255.0, (int)measure.X, (int)measure.Y, data);
            }
            else
            {
                Implementation.DrawString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth, color.B / 255.0, color.G / 255.0, color.R / 255.0, color.A / 255.0, (int)measure.X, (int)measure.Y, data);
            }
            var tex2 = new Microsoft.Xna.Framework.Graphics.Texture2D(gfx.Device, (int)measure.X, (int)measure.Y);
			tex2.SetData<byte>(data);
			gfx.DrawRectangle(x, y, (int)measure.X, (int)measure.Y, tex2);
		}
	}
}

