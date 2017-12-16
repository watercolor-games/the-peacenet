using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class Panel : Control
    {
        private bool _autosize = false;

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

        protected override void OnUpdate(GameTime time)
        {
            if (_autosize)
            {
                var last = Children.Where(x=>x.Visible).OrderByDescending(x => x.Y).First();
                Height = last.Y + last.Height;
            }
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }
    }
}
