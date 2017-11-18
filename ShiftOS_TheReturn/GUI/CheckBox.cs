using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.GUI
{
    public class CheckBox : GUI.Control
    {
        public CheckBox()
        {
            Width = 16;
            Height = 16;
            Click += () =>
            {
                Checked = !_checked;
            };
        }

        private bool _checked = false;

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (value == _checked)
                    return;
                _checked = value;
                CheckedChanged?.Invoke();
                Invalidate();
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            Theming.ThemeManager.Theme.DrawCheckbox(gfx, 0, 0, Width, Height, Checked, ContainsMouse);
        }

        public event Action CheckedChanged;
    }
}
