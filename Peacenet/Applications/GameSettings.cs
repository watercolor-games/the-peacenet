using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Peacenet.Applications
{
    public class GameSettings : Window
    {
        public GameSettings(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            SetWindowStyle(WindowStyle.Dialog);
            Opacity = 0.15F;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx, RenderTarget2D currentTarget)
        {
            base.OnPaint(time, gfx, currentTarget);
        }
    }
}
