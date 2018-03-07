using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Cutscene
{
    /// <summary>
    /// Represents a Peace engine cutscene.
    /// </summary>
    public abstract class Cutscene : EntityContainer, ILoadable
    {
        [Dependency]
        private CutsceneManager _cutscene = null;

        private bool _hasFinished = false;

        /// <summary>
        /// Retrieves the name of the cutscene.
        /// </summary>
        public abstract string Name { get; }

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

        /// <summary>
        /// Notify the cutscene manager that the cutscene is finished, stopping the cutscene from being played.
        /// </summary>
        public void NotifyFinished()
        {
            _hasFinished = true;
            _cutscene.Stop();
        }

        /// <inheritdoc/>
        public virtual void Load(ContentManager content) { }
        /// <summary>
        /// Fire a cutscene finish event.
        /// </summary>
        public virtual void OnFinish() { }
        /// <summary>
        /// Fire a cutscene play event.
        /// </summary>
        public virtual void OnPlay() { }
        
    }
}
