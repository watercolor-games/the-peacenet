using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// Gets or sets a UI element which can host other UI elements and resize based on its contents.
    /// </summary>
    public class Panel : Control
    {
        private bool _autosize = false;
        private bool _needsLayout = true;

        /// <summary>
        /// Gets or sets whether the panel should auto-size based on its contents.
        /// </summary>
        public bool AutoSize
        {
            get
            {
                return _autosize;
            }
            set
            {
                if (_autosize == value)
                    return;
                _autosize = value;
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_autosize)
            {
                if (_needsLayout == true)
                {
                    if (Children.Length > 0)
                    {
                        var last = Children.Where(x => x.Visible).OrderByDescending(x => x.Y).First();
                        Height = last.Y + last.Height;
                    }
                    else
                    {
                        Height = 0;
                    }
                    _needsLayout = false;
                }
            }
        }

        private void ControlLayoutChanged(object sender, EventArgs e)
        {
            _needsLayout = true;
        }

        /// <inheritdoc/>
        public override void AddChild(Control child)
        {
            base.AddChild(child);
            child.XChanged += ControlLayoutChanged;
            child.YChanged += ControlLayoutChanged;
            child.WidthChanged += ControlLayoutChanged;
            child.HeightChanged += ControlLayoutChanged;
            child.VisibleChanged += ControlLayoutChanged;
            _needsLayout = true;
        }

        /// <inheritdoc/>
        public override void RemoveChild(Control child)
        {
            base.RemoveChild(child);
            child.XChanged -= ControlLayoutChanged;
            child.YChanged -= ControlLayoutChanged;
            child.WidthChanged -= ControlLayoutChanged;
            child.HeightChanged -= ControlLayoutChanged;
            child.VisibleChanged -= ControlLayoutChanged;
            _needsLayout = true;
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }
    }
}
