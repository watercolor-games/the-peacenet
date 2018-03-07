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
    /// A GUI element which can only contain one child and allows the child to be scrolled up and down by the mouse.
    /// </summary>
    public class ScrollView : Control
    {
        private int _scrollOffset = 0;
        private Control _host = null;
        private int _scrollHeight = 0;

        /// <inheritdoc/>
        public override void AddChild(Control child)
        {
            if (Children.Length > 0)
                throw new InvalidOperationException("Scroll views can only host one child.");
            base.AddChild(child);
            if (child != null)
                _host = child;
            _scrollOffset = 0;
            child.WidthChanged += Child_WidthChanged;
            child.MouseScroll += OnMouseScroll;
            _needsLayout = true;
        }

        /// <inheritdoc/>
        public override void RemoveChild(Control child)
        {
            base.RemoveChild(child);
            if (_host == child)
            {
                child.WidthChanged -= Child_WidthChanged;
                child.MouseScroll -= OnMouseScroll;
                _host = null;
                _needsLayout = true;
                _scrollOffset = 0;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseScroll(int delta)
        {
            int offset = MathHelper.Clamp(_scrollOffset + delta, 0, _scrollHeight - Height);
            if(offset != _scrollOffset)
            {
                _scrollOffset = offset;
                _needsLayout = true;
            }
        }

        private void Child_WidthChanged(object sender, EventArgs e)
        {
            _needsLayout = true;
        }

        private bool _needsLayout = true;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_needsLayout && _host != null)
            {
                _host.X = 0;
                _host.Y = 0 - _scrollOffset;
                Width = _host.Width;
                _scrollHeight = _host.Height;
                base.OnUpdate(time);
                _needsLayout = false;
            }
        }

        protected override bool CanBeScrolled
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
        }
    }
}
