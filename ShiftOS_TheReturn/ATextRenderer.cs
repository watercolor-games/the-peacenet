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
    public abstract class ATextRenderer
    {
        public abstract Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
        public abstract Texture2D DrawText(GraphicsContext gfx, string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode);
    }

    internal class TextCacheInformation
    {
        public string Renderer { get; set; }
        public string Text { get; set; }
        public TextAlignment Alignment { get; set; }
        public WrapMode Wrapping { get; set; }
        public int MaxWidth { get; set; }
        public Texture2D Texture { get; set; }
        public System.Drawing.Font Font { get; set; }
    }

    public static class TextRenderer
    {
        private static ATextRenderer _renderer = null;
        private static List<ATextRenderer> _renderers = null;
        private static List<TextCacheInformation> _cache = new List<TextCacheInformation>();

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

        public static ATextRenderer GetDefaultRenderer()
        {
            if (_renderers == null)
                _getAllRenderers();
            return _renderers.FirstOrDefault(x=>x.GetType().GetCustomAttributes(false).FirstOrDefault(y=>y is DefaultRenderer) != null);
        }

        public static ATextRenderer GetFallbackRenderer()
        {
            if (_renderers == null)
                _getAllRenderers();
            return _renderers.FirstOrDefault(x => x.GetType().GetCustomAttributes(false).FirstOrDefault(y => y is FallbackRenderer) != null);
        }

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

        public static Vector2 MeasureText(string text, System.Drawing.Font font, int maxwidth, TextAlignment alignment, WrapMode wrapMode)
        {
            return _renderer.MeasureText(text, font, maxwidth, alignment, wrapMode);
        }

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

    public enum TextAlignment
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Middle,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public class DefaultRenderer : Attribute
    {

    }

    public class FallbackRenderer : Attribute
    {

    }
}
