using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GUI;
using ShiftOS.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;

namespace ShiftOS.Frontend
{
    public class MainMenu : GUI.Control
    {
        public MainMenu()
        {
            X = 0;
            Y = 0;
            Width = UIManager.Viewport.Width;
            Height = UIManager.Viewport.Height;
        }

        private Color _redbg = new Color(127, 0, 0, 255);
        private Color _bluebg = new Color(0, 0, 127, 255);
        private float _bglerp = 0.0f;
        private int _lerpdir = 1;

        protected override void OnLayout(GameTime gameTime)
        {
            if (_lerpdir == 1)
                _bglerp += 0.001f;
            else
                _bglerp -= 0.001f;
            if (_bglerp <= 0.0)
                _lerpdir = 1;
            else if (_bglerp >= 1)
                _lerpdir = -1;
            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Color.Lerp(_redbg, _bluebg, _bglerp));
            gfx.DrawString("ShiftOS", 30, 30, Color.White, new System.Drawing.Font("Consolas", 48f));

        }
    }
}
