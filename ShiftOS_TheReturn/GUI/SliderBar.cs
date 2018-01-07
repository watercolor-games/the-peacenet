using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// A <see cref="ProgressBar"/> whose progress value can be modified using the mouse. This is useful for allowing the player to adjust percentage values. 
    /// </summary>
    public class SliderBar : ProgressBar
    {
        private bool _isMouseDown = false;

        /// <summary>
        /// Creates a new instance of the <see cref="SliderBar"/> class. 
        /// </summary>
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

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (!ContainsMouse)
                _isMouseDown = false;
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
