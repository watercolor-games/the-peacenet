using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Plex.Engine.GUI
{
    public class SliderBar : ProgressBar
    {
        private bool _isMouseDown = false;

        public SliderBar()
        {
            MouseLeftDown += (o, a) =>
            {
                _isMouseDown = true;
            };
            MouseLeftUp += (o, a) =>
            {
                _isMouseDown = false;
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_isMouseDown)
            {
                float mousex = MouseX;
                float width = Width;
                Value = (mousex / width);
            }
            base.OnUpdate(time);
        }
    }
}
