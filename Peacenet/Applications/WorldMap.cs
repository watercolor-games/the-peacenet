using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("World Map", "System", "View a map of your current Peacenet country and interact with other members in The Peacenet.")]
    public class WorldMap : Window
    {
        [Dependency]
        private GameManager _game = null;

        public WorldMap(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black * 0.5F);

            gfx.DrawRectangle(0, 0, Width, Height, _game.State.CountryTexture, Theme.GetAccentColor(), System.Windows.Forms.ImageLayout.Zoom);
        }

        protected override void OnUpdate(GameTime time)
        {
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            Parent.X = 0;
            Parent.Y = 0;

            base.OnUpdate(time);
        }
    }
}
