using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GUI;
using Plex.Engine.TextRenderers;

namespace Plex.Engine.GraphicsSubsystem
{
    public class GraphicsContext
    {
        private static Texture2D white = null;


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

        public int X
        {
            get
            {
                return Device.ScissorRectangle.X;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(value, Y, Width, Height);
            }
        }

        public int Y
        {
            get
            {
                return Device.ScissorRectangle.Y;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, value, Width, Height);
            }
        }

        public int Width
        {
            get
            {
                return Device.ScissorRectangle.Width;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, Y, value, Height);
            }
        }

        public int Height
        {
            get
            {
                return Device.ScissorRectangle.Height;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, Y, Width, value);
            }
        }
        
        public void DrawPolygon(Color c, params int[] locs)
        {
            if ((locs.Length % 2) != 0)
                throw new Exception("The locs argument count must be a multiple of 2.");
            for(int i = 0; i < locs.Length; i+= 2)
            {
                int x = locs[i];
                int y = locs[i + 1];
                int x1 = locs[0];
                int y1 = locs[1];

                if (i < locs.Length - 2)
                {
                    x1 = locs[i + 2];
                    y1 = locs[i + 3];
                }
                DrawLine(x, y, x1, y1, 1, c);
            }
        }

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spritebatch;

        public GraphicsContext(GraphicsDevice device, SpriteBatch batch, int x, int y, int width, int height)
        {
            if (device == null || batch == null)
                throw new ArgumentNullException();
            
            _graphicsDevice = device;
            _spritebatch = batch;
            if(white == null)
            {
                white = new Texture2D(_graphicsDevice, 1, 1);
                white.SetData<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            }
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        public void Clear(Color c)
        {
            DrawRectangle(0, 0, Width, Height, c);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2)
        {
            DrawLine(x, y, x1, y1, thickness, tex2, Color.White);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2, Color tint)
        {
            x += X;
            y += Y;
            x1 += X;
            y1 += Y;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(tex2, new Rectangle(x, y, distance, thickness), null, tint, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Color color)
        {
            x += X;
            y += Y;
            x1 += X;
            y1 += Y;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(white, new Rectangle(x, y, distance, thickness), null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            x += X;
            y += Y;
            _spritebatch.Draw(white, new Rectangle(x, y, width, height), color);
        }

        public void DrawCircle(int x, int y, int radius, Color color)
        {
            float step = (float) Math.PI / (radius * 4);
            var rect = new Rectangle(x, y, radius, 1);
            for (float theta = 0; theta < 2 * Math.PI; theta += step)
                _spritebatch.Draw(white, rect, null, color, theta, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, ImageLayout layout = ImageLayout.Stretch)
        {
            DrawRectangle(x, y, width, height, tex2, Color.White, layout);
        }

        public readonly RasterizerState RasterizerState = new RasterizerState { ScissorTestEnable = true };

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, Color tint, ImageLayout layout = ImageLayout.Stretch, bool premultiplied = false)
        {
            if (tex2 == null)
                return;
            x += X;
            y += Y;
            _spritebatch.End();
            var state = SamplerState.LinearClamp;
            if (layout == ImageLayout.Tile)
                state = SamplerState.LinearWrap;
            if (premultiplied)
            {
                _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                                        state, Device.DepthStencilState,
                                        RasterizerState);
            }
            else
            {
                _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                        state, Device.DepthStencilState,
                                        RasterizerState);
            }
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
                    float scale = Math.Min(width / (float)tex2.Width, height / (float)tex2.Height);
                    
                    var scaleWidth = (int)(tex2.Width * scale);
                    var scaleHeight = (int)(tex2.Height * scale);

                    _spritebatch.Draw(tex2, new Rectangle(x+(((int)width - scaleWidth) / 2), y+(((int)height - scaleHeight) / 2), scaleWidth, scaleHeight), tint);
                    break;
                    ;
            }
            _spritebatch.End();
            _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, Device.DepthStencilState,
                                    RasterizerState.CullNone);

        }

        public static Vector2 MeasureString(string text, System.Drawing.Font font, TextAlignment alignment, int wrapWidth = int.MaxValue, WrapMode wrapMode = WrapMode.Words)
        {
            return Plex.Engine.TextRenderer.MeasureText(text, font, wrapWidth, alignment, wrapMode);

        }

        public void DrawString(string text, int x, int y, Color color, System.Drawing.Font font, TextAlignment alignment, int wrapWidth = int.MaxValue, WrapMode wrapMode = WrapMode.Words)
        {
            if (string.IsNullOrEmpty(text))
                return;
            Plex.Engine.TextRenderer.DrawText(this, x, y, text, font, color, wrapWidth, alignment, wrapMode);
        }

        private float getRotation(float x, float y, float x2, float y2)
        {
            float adj = x - x2;
            float opp = y - y2;
            return (float) Math.Atan2(opp, adj) - (float) Math.PI;
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
