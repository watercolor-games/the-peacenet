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
    /// Represents a boolean value as a GUI element.
    /// </summary>
    public class CheckBox : Control
    {
        private bool _checked = false;

        /// <summary>
        /// Creates a new instance of the <see cref="CheckBox"/> control. 
        /// </summary>
        public CheckBox()
        {
            Click += (o, a) =>
            {
                Checked = !Checked;
            };
        }

        /// <summary>
        /// Gets or sets the value of the check box.
        /// </summary>
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

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawCheckbox(gfx, 0, 0, Width, Height, _checked, ContainsMouse);
        }
    }
}
