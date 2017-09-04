using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Frontend.GraphicsSubsystem
{
    public class GraphicsContext
    {
        public SpriteBatch Batch
        {
            get
            {
                return _spritebatch;
            }
        }

        public GraphicsDevice Device
        {
            get
            {
                return _graphicsDevice;
            }
        }

        private int _startx = 0;
        private int _starty = 0;

        private int _maxwidth = 1;
        private int _maxheight = 1;
        
        public int X
        {
            get
            {
                return _startx;
            }
            set
            {
                _startx = value;
            }
        }

        public int Y
        {
            get
            {
                return _starty;
            }
            set
            {
                _starty = value;
            }
        }

        public int Width
        {
            get
            {
                return _maxwidth;
            }
            set
            {
                _maxwidth = value;
            }
        }

        public int Height
        {
            get
            {
                return _maxheight;
            }
            set
            {
                _maxheight = value;
            }
        }


        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spritebatch;

        public GraphicsContext(GraphicsDevice device, SpriteBatch batch, int x, int y, int width, int height)
        {
            _graphicsDevice = device;
            _spritebatch = batch;
            _maxwidth = width;
            _maxheight = height;
            _startx = x;
            _starty = y;
        }

        public void Clear(Color c)
        {
            DrawRectangle(0, 0, _maxwidth, _maxheight, c);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2)
        {
            DrawLine(x, y, x1, y1, thickness, tex2, Color.White);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2, Color tint)
        {
            x += _startx;
            y += _starty;
            x1 += _startx;
            y1 += _starty;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(tex2, new Rectangle(x, y, distance, thickness), null, tint, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Color color)
        {
            x += _startx;
            y += _starty;
            x1 += _startx;
            y1 += _starty;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(UIManager.SkinTextures["PureWhite"], new Rectangle(x, y, distance, thickness), null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            x += _startx;
            y += _starty;
            _spritebatch.Draw(UIManager.SkinTextures["PureWhite"], new Rectangle(x, y, width, height), color);
        }

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, ImageLayout layout = ImageLayout.Stretch)
        {
            DrawRectangle(x, y, width, height, tex2, Color.White, layout);
        }

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, Color tint, ImageLayout layout = ImageLayout.Stretch)
        {
            if (tex2 == null)
                return;
            x += _startx;
            y += _starty;
            _spritebatch.End();
            var state = SamplerState.LinearClamp;
            if (layout == ImageLayout.Tile)
                state = SamplerState.LinearWrap;
            _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    state, DepthStencilState.Default,
                                    RasterizerState.CullNone);
            switch (layout)
            {
                case ImageLayout.Tile:
                    _spritebatch.Draw(tex2, new Vector2(x,y), new Rectangle(0, 0, width, height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    break;
                case ImageLayout.Stretch:
                    _spritebatch.Draw(tex2, new Rectangle(x, y, width, height), tint);
                    break;
                case ImageLayout.None:
                    _spritebatch.Draw(tex2, new Rectangle(x, y, tex2.Width, tex2.Height), tint);
                    break;
                case ImageLayout.Center:
                    _spritebatch.Draw(tex2, new Rectangle(x+((width - tex2.Width) / 2), y+((height - tex2.Height) / 2), tex2.Width, tex2.Height), tint);
                    break;
                case ImageLayout.Zoom:
                    float scale = Math.Min(width / tex2.Width, height / tex2.Height);
                    
                    var scaleWidth = (int)(tex2.Width * scale);
                    var scaleHeight = (int)(tex2.Height * scale);

                    _spritebatch.Draw(tex2, new Rectangle(x+(((int)width - scaleWidth) / 2), y+(((int)height - scaleHeight) / 2), scaleWidth, scaleHeight), tint);
                    break;
                    ;
            }
            _spritebatch.End();
            _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, DepthStencilState.Default,
                                    RasterizerState.CullNone);

        }

        public static Vector2 MeasureString(string text, System.Drawing.Font font, int wrapWidth = int.MaxValue)
        {
            return Plex.Engine.TextRenderer.MeasureText(text, font, wrapWidth);

        }

        [Obsolete("Don't be a broom. Use a TextControl.")]
        public static List<TextCache> StringCaches = new List<TextCache>();

        [Obsolete("Don't be a broom. Use a TextControl.")]
        public TextCache GetCache(string text, System.Drawing.Font font, int wrapWidth)
        {
            //Don't use LINQ, it could be a performance bottleneck.
            var caches = StringCaches.ToArray();
            for (int i = 0; i < caches.Length; i++)
            {
                var cache = caches[i];
                if (cache.Text == text && cache.FontFamily == font && cache.WrapWidth == wrapWidth)
                    return cache;
            }
            return null;
        }

        public void DrawString(string text, int x, int y, Color color, System.Drawing.Font font, int wrapWidth = int.MaxValue)
        {
            if (string.IsNullOrEmpty(text))
                return;
            x += _startx;
            y += _starty;

            Plex.Engine.TextRenderer.DrawText(this, x, y, text, font, color, wrapWidth);
        }

        private float getRotation(float x, float y, float x2, float y2)
        {
            float adj = x - x2;
            float opp = y - y2;
            float tan = opp / adj;
            float res = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            res = (res - 180) % 360;
            if (res < 0) { res += 360; }
            res = MathHelper.ToRadians(res);
            return res;
        }
    }

    public class TextCache
    {
        public string Text { get; set; }
        public System.Drawing.Font FontFamily { get; set; }
        public Texture2D Cache { get; set; }
        public int WrapWidth { get; set; }
    }
}
