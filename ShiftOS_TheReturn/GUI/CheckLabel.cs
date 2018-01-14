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
    /// Represents a labeled check box.
    /// </summary>
    public class CheckLabel : Control
    {
        private CheckBox _check = null;
        private Label _label = null;

        /// <summary>
        /// Gets or sets the value of the check box.
        /// </summary>
        public bool Checked
        {
            get
            {
                return _check.Checked;
            }
            set
            {
                _check.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the checkbox's label.
        /// </summary>
        public string Text
        {
            get
            {
                return _label.Text;
            }
            set
            {
                _label.Text = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CheckLabel"/> control. 
        /// </summary>
        public CheckLabel()
        {
            _check = new CheckBox();
            _label = new Label();
            AddChild(_check);
            AddChild(_label);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _check.X = 2;
            _check.Width = 16;
            _check.Height = 16;
            _label.AutoSize = true;
            _label.Alignment = TextAlignment.Left;
            _label.MaxWidth = (MaxWidth - _check.X + _check.Width + 3);
            _label.X = _check.X + _check.Width + 3;
            Width = _label.X + _label.Width + 2;
            Height = Math.Max(_label.Height, _check.Height) + 2;
            _check.Y = 2;
            _label.Y = (Height - _label.Height) / 2;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }
    }
}
