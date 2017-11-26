using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Plex.Engine.GUI;

namespace Peacenet
{
    public class TestWindowSystemUser : IEngineComponent
    {
        [Dependency]
        private WindowSystem _windowSystem = null;

        public void Initiate()
        {
            var win = new TestWindow(_windowSystem);
            win.Show();
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(MonoGame.Extended.Input.InputListeners.KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }

    public class TestWindow : Window
    {
        public TestWindow(WindowSystem _winsys) : base(_winsys)
        {
            Width = 720;
            Height = 480;
        }
    }
}
