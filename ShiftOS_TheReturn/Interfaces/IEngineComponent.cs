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
    /// <summary>
    /// Provides an extremely simple API for creating modular Peacenet engine components.
    /// </summary>
    public interface IEngineComponent
    {
        /// <summary>
        /// This method is run when your component is successfully loaded into the engine.
        /// </summary>
        void Initiate();
    }
}
