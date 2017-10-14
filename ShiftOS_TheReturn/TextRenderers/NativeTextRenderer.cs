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
        public NativeTextRenderer()
        {
            // Throw an exception if there was a DllImport failure so
            // that the engine can choose a different renderer.
            System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(Implementation));
        }
		
		public override Microsoft.Xna.Framework.Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, Plex.Engine.GUI.TextAlignment alignment)
		{
            long result = -1;
            result = Implementation.MeasureString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth);
            return new Microsoft.Xna.Framework.Vector2((int)(result & uint.MaxValue), (int)(result >> 32));
		}
		
		public override void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Microsoft.Xna.Framework.Color color, int maxwidth, Plex.Engine.GUI.TextAlignment alignment)
		{
			var measure = MeasureText(text, font, maxwidth, alignment);
			var data = new byte[(int)measure.X * (int)measure.Y * 4];
			if (data.Length == 0)
				return;
            Implementation.DrawString(text, text.Length, font.FontFamily.Name, font.FontFamily.Name.Length, font.SizeInPoints, (int)font.Style, (int)alignment, (int)Implementation.WrapMode.Words, maxwidth, 0, 0, 0, 1, (int)measure.X, (int)measure.Y, data);
            for(int i = 0; i < data.Length; i += 4)
            {
                data[i] = (byte)(255 - data[i]);
                data[i+1] = (byte)(255 - data[i+1]);
                data[i+2] = (byte)(255 - data[i+2]);

            }
            var tex2 = new Microsoft.Xna.Framework.Graphics.Texture2D(gfx.Device, (int)measure.X, (int)measure.Y);
			tex2.SetData<byte>(data);
            gfx.DrawRectangle(x, y, (int)measure.X, (int)measure.Y, tex2, color, System.Windows.Forms.ImageLayout.Stretch, true);
		}
	}
}

