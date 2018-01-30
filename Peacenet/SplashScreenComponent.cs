using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Themes;
using Plex.Engine.GUI;
using Peacenet.Applications;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.Cutscene;
using Plex.Engine.Saves;
using Peacenet.MainMenu;
using Microsoft.Xna.Framework.Content;

namespace Peacenet
{
    /// <summary>
    /// Provides a simple engine component for displaying the Peacenet splash screen and main menu.
    /// </summary>
    public class SplashScreenComponent : IEngineComponent, ILoadable
    {
        [Dependency]
        private Plexgate _plexgate = null;
        private Layer layer = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            layer = new Layer();
            _plexgate.AddLayer(layer);

        }

        private SplashEntity splash = null;

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            splash = (_plexgate.New<SplashEntity>());
            layer.AddEntity(splash);
        }
        
        /// <summary>
        /// Re-spawns the <see cref="SplashEntity"/>, causing the main menu to show once again. 
        /// </summary>
        public void Reset()
        {
            layer.ClearEntities();
            splash = _plexgate.New<SplashEntity>();
            layer.AddEntity(splash);
            MakeVisible();
        }

        /// <summary>
        /// Makes the splash screen entity layer visible to the renderer.
        /// </summary>
        public void MakeVisible()
        {
            _plexgate.AddLayer(layer);
        }

        /// <summary>
        /// Makes the splash screen entity layer hidden from the renderer.
        /// </summary>
        public void MakeHidden()
        {
            _plexgate.RemoveLayer(layer);
        }

    }
}
