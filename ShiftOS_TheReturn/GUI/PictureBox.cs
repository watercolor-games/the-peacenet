using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class PictureBox : Control
    {
        private Texture2D _texture = null;
        private Color _tint = Color.White;
        private bool _autoSize = false;
        private ImageLayout _layout = ImageLayout.Stretch;
        private bool _premultiplied = false;
        private float _scale = 1;

        public Color Tint
        {
            get
            {
                return _tint;
            }
            set
            {
                if (_tint == value)
                    return;
                _tint = value;
                Invalidate();
            }
        }

        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                if (_autoSize == value)
                    return;
                _autoSize = value;
                Invalidate();
            }
        }

        public float AutoSizeScale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (_scale == value)
                    return;
                _scale = value;
                Invalidate();
            }
        }

        public bool Premultipied
        {
            get
            {
                return _premultiplied;
            }
            set
            {
                if (_premultiplied == value)
                    return;
                _premultiplied = value;
                Invalidate();
            }
        }

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
            set
            {
                if (_texture == value)
                    return;
                _texture = value;
                Invalidate();
            }
        }

        public ImageLayout Layout
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

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, _texture, _tint, _layout, false, _premultiplied);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (AutoSize)
            {
                if (_texture != null)
                {
                    Width = (int)(_texture.Width*_scale);
                    Height = (int)(_texture.Height*_scale);
                }
                else
                {
                    Width = 1;
                    Height = 1;
                }
            }
            base.OnUpdate(time);
        }
    }
}
