using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Engine;
using static Plex.Engine.SkinEngine;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Frontend.GUI
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
            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);
            if (_checked)
            {
                gfx.DrawLine(4, 4, Width-4, Height-4, 1, UIManager.SkinTextures["ControlTextColor"]);
                gfx.DrawLine(Width-4, 4, 4, Height - 4, 1, UIManager.SkinTextures["ControlTextColor"]);

            }

        }

        public event Action CheckedChanged;
    }
}
