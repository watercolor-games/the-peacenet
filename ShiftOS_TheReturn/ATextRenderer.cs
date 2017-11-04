using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using Plex.Engine.TextRenderers;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine
{
    public abstract class ATextRenderer
    {
        public abstract Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
        public abstract void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Color color, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
    }

    public static class TextRenderer
    {
        private static ATextRenderer _renderer = null;

        internal static void Init(ATextRenderer renderer)
        {
            _renderer = renderer;
        }

        public static Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            return _renderer.MeasureText(text, font, maxwidth, alignment, wrapMode);
        }

        public static void DrawText(GraphicsContext gfx, int x, int y, string text, System.Drawing.Font font, Color color, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            _renderer.DrawText(gfx, x, y, text, font, color, maxwidth, alignment, wrapMode);
        }
    }

}
