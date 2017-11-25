using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class WindowSystem : IEngineComponent
    {
        [Dependency]
        private UIManager _uiman = null;

        public void Initiate()
        {
            Logger.Log("Starting window system.", LogType.Info, "pgwinsys");
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            Logger.Log("Stopping window system.", LogType.Info, "pgwinsys");
        }
    }
}
