using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Cutscene
{
    public abstract class Cutscene
    {
        private bool _hasFinished = false;

        public abstract string Name { get; }

        /// <summary>
        /// Gets the cutscene's content manager. Note that the content manager is null until the cutscene manager requests you to load your assets.
        /// </summary>
        public ContentManager Content
        {
            get; internal set;
        }

        internal bool IsFinished
        {
            get
            {
                return _hasFinished;
            }
            set
            {
                _hasFinished = value;
            }
        }

        public void NotifyFinished()
        {
            _hasFinished = true;
        }

        public abstract void LoadResources();
        public abstract void OnPlay();
        public abstract void OnFinish();

        public abstract void UnloadResources();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime time, GraphicsContext gfx);

    }
}
