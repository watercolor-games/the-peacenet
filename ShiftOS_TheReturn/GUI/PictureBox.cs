using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using System.Drawing.Imaging;
using Plex.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Plex.Frontend.GUI
{
    public class PictureBox : Control
    {
        private Texture2D img = null;
        private System.Windows.Forms.ImageLayout _layout = System.Windows.Forms.ImageLayout.Zoom;

        public System.Windows.Forms.ImageLayout ImageLayout
        {
            get
            {
                return _layout;
            }
            set
            {
                if (_layout == value)
                    return;
                _layout = value;
                Invalidate();
            }
        }

        public Texture2D Image
        {
            get
            {
                return img;
            }
            set
            {
                if (img != null)
                    img.Dispose();
                img = value;
                Invalidate();
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (AutoSize)
            {
                MaxWidth = int.MaxValue;
                MaxHeight = MaxWidth;
                Width = (img != null) ? img.Width : 0;
                Height = (img != null) ? img.Height : 0;

            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
           gfx.DrawRectangle(0, 0, Width, Height, img, Microsoft.Xna.Framework.Color.White * (float)Opacity, _layout);
        }

        //Again, thanks StackOverflow
        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(SkinEngine.LoadedSkin.ControlColor);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }

    
}
