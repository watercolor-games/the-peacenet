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
    /// <summary>
    /// A control which simply displays a picture.
    /// </summary>
    public class PictureBox : Control
    {
        private Texture2D _texture = null;
        private Color _tint = Color.White;
        private bool _autoSize = false;
        private ImageLayout _layout = ImageLayout.Stretch;
        private bool _premultiplied = false;
        private float _scale = 1;

        /// <summary>
        /// Gets or sets the tint of the picture.
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets whether the control should auto-size to match the size of the picture
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets the scale of the picture if the contol is auto-sizing. A value of 1.0 means 1:1 scale.
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets whether the picture's texture data is pre-multiplied.
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets the picture's texture.
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets the layout of the texture
        /// </summary>
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
                Invalidate(true);
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, _texture, _tint, _layout, false, _premultiplied);
        }

        /// <inheritdoc/>
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
