﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend.Apps;

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

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2)
        {
            DrawRectangle(x, y, width, height, tex2, Color.White);
        }

        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, Color tint)
        {
            if (tex2 == null)
                return;
            x += _startx;
            y += _starty;
            _spritebatch.Draw(tex2, new Rectangle(x, y, width, height), tint);
        }

        public static Vector2 MeasureString(string text, System.Drawing.Font font, int wrapWidth = int.MaxValue)
        {
            var measure = TextRenderer.MeasureText(text, font, new System.Drawing.Size(wrapWidth, int.MaxValue), TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            return new Vector2(measure.Width, measure.Height);
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
            TextCache cache = null;
            x += _startx;
            y += _starty;
            //cache = GetCache(text, font, wrapWidth);
            //if (cache == null)
            //{
            var measure = MeasureString(text, font, wrapWidth);
            using (var bmp = new System.Drawing.Bitmap((int)measure.X, (int)measure.Y))
            {
                using (var gfx = System.Drawing.Graphics.FromImage(bmp))
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        TextRenderer.DrawText(gfx, text, font, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Color.White, TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
                    }
                    else
                    {
                        gfx.DrawString(text, font, System.Drawing.Brushes.Black, new System.Drawing.Rectangle(0,0,bmp.Width,bmp.Height), System.Drawing.StringFormat.GenericTypographic);
                    }
                }
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                var bytes = new byte[Math.Abs(lck.Stride) * lck.Height];
                Marshal.Copy(lck.Scan0, bytes, 0, bytes.Length);
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    for (int i = 0; i < bytes.Length; i += 4)
                    {
                        bytes[i] = (byte)(255 - bytes[i]);
                        bytes[i + 1] = (byte)(255 - bytes[i + 1]);
                        bytes[i + 2] = (byte)(255 - bytes[i + 2]);

                    }
                }
                bmp.UnlockBits(lck);
                using (var tex2 = new Texture2D(_graphicsDevice, bmp.Width, bmp.Height))
                {
                    tex2.SetData<byte>(bytes);

                    _spritebatch.Draw(tex2, new Rectangle(x, y, tex2.Width, tex2.Height), color);
                }

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

    public class TextCache
    {
        public string Text { get; set; }
        public System.Drawing.Font FontFamily { get; set; }
        public Texture2D Cache { get; set; }
        public int WrapWidth { get; set; }
    }
}
