using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class ScrollView : Control
    {
        private int _scrollOffset = 0;
        private Control _host = null;
        private int _scrollHeight = 0;

        public override void AddChild(Control child)
        {
            if (Children.Length > 0)
                throw new InvalidOperationException("Scroll views can only host one child.");
            base.AddChild(child);
            if (child != null)
                _host = child;
            _scrollOffset = 0;
        }

        protected override void OnUpdate(GameTime time)
        {
            _host.X = 0;
            _host.Y = 0 - _scrollOffset;
            Width = _host.Width;
            _scrollHeight = _host.Height;
            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
        }
    }
}
