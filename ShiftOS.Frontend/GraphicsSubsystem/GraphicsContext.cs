using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShiftOS.Frontend.Apps;

namespace ShiftOS.Frontend.GraphicsSubsystem
{
    public class GraphicsContext
    {
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
            x += _startx;
            y += _starty;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(tex2, new Rectangle(x, y, distance, thickness), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawLine(int x, int y, int x1, int y1, int thickness, Color color)
        {
            x += _startx;
            y += _starty;
            var tex2 = new Texture2D(_graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            byte[] colordata = new byte[] { color.B, color.G, color.R, color.A };
            tex2.SetData<byte>(colordata);
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(tex2, new Rectangle(x, y, distance, thickness), null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            x += _startx;
            y += _starty;
            var tex2 = new Texture2D(_graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            byte[] colordata = new byte[] { color.B, color.G, color.R, color.A };
            tex2.SetData<byte>(colordata);
            _spritebatch.Draw(tex2, new Rectangle(x, y, width, height), color);
        }

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2)
        {
            x += _startx;
            y += _starty;
            _spritebatch.Draw(tex2, new Rectangle(x, y, width, height), Color.White);
        }

        public Vector2 MeasureString(string text, System.Drawing.Font font, int wrapWidth = int.MaxValue)
        {
            using(var gfx = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1)))
            {
                var s = gfx.SmartMeasureString(text, font, wrapWidth);
                return new Vector2((float)Math.Ceiling(s.Width), (float)Math.Ceiling(s.Height));
            }
        }

        public void DrawString(string text, int x, int y, Color color, System.Drawing.Font font, int wrapWidth = 0)
        {
            x += _startx;
            y += _starty;
            Vector2 measure;
            if (wrapWidth == 0)
                measure = MeasureString(text, font);
            else
                measure = MeasureString(text, font, wrapWidth);
            using(var bmp = new System.Drawing.Bitmap((int)measure.X, (int)measure.Y))
            {
                using(var gfx = System.Drawing.Graphics.FromImage(bmp))
                {
                    var textformat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericTypographic);
                    textformat.FormatFlags = System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
                    textformat.Trimming = System.Drawing.StringTrimming.None;
                    textformat.FormatFlags |= System.Drawing.StringFormatFlags.NoClip;

                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    gfx.DrawString(text, font, new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)), 0, 0, textformat);
                }
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var data = new byte[Math.Abs(lck.Stride) * lck.Height];
                System.Runtime.InteropServices.Marshal.Copy(lck.Scan0, data, 0, data.Length);
                bmp.UnlockBits(lck);
                var tex2 = new Texture2D(_graphicsDevice, bmp.Width, bmp.Height);
                for(int i = 0; i < data.Length; i += 4)
                {
                    byte r = data[i];
                    byte b = data[i + 2];
                    data[i] = b;
                    data[i + 2] = r;
                }

                tex2.SetData<byte>(data);
                _spritebatch.Draw(tex2, new Rectangle(x, y, bmp.Width, bmp.Height), Color.White);
            }
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
}
