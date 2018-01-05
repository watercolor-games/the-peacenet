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
using Plex.Engine.Server;
using Peacenet.MainMenu;
using Microsoft.Xna.Framework.Content;

namespace Peacenet
{
    public class SplashScreenComponent : IEngineComponent, ILoadable
    {
        [Dependency]
        private Plexgate _plexgate = null;
        private Layer layer = null;

        public void Initiate()
        {
            layer = new Layer();
            _plexgate.AddLayer(layer);

        }

        private SplashEntity splash = null;

        public void Load(ContentManager content)
        {
            splash = (_plexgate.New<SplashEntity>());
            layer.AddEntity(splash);
        }
        
        public void Reset()
        {
            layer.ClearEntities();
            splash = _plexgate.New<SplashEntity>();
            layer.AddEntity(splash);
            MakeVisible();
        }

        public void MakeVisible()
        {
            _plexgate.AddLayer(layer);
        }

        public void MakeHidden()
        {
            _plexgate.RemoveLayer(layer);
        }

    }
}
