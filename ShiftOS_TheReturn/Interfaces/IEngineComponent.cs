using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Interfaces
{
    public interface IEngineComponent
    {
        void Initiate();
        void OnKeyboardEvent(KeyboardEventArgs e);
        void OnGameUpdate(GameTime time);
        void OnFrameDraw(GameTime time, GraphicsContext ctx);
        void Unload();
    }
}
