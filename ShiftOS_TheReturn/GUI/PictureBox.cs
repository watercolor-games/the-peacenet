using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using System.Drawing.Imaging;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Plex.Engine.GUI
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
    }
}
