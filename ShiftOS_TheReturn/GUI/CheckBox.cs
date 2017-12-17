using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class CheckBox : Control
    {
        private bool _checked = false;

        public CheckBox()
        {
            Click += (o, a) =>
            {
                Checked = !Checked;
            };
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked == value)
                    return;
                _checked = value;
                Invalidate(true);
            }
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawCheckbox(gfx, 0, 0, Width, Height, _checked, ContainsMouse);
        }
    }
}
