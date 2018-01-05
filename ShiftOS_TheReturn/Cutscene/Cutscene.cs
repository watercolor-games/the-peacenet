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
    public abstract class Cutscene : EntityContainer, ILoadable
    {
        [Dependency]
        private CutsceneManager _cutscene = null;

        private bool _hasFinished = false;

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

        public void NotifyFinished()
        {
            _hasFinished = true;
            _cutscene.Stop();
        }
        
        public virtual void Load(ContentManager content) { }
        public virtual void OnFinish() { }
        public virtual void OnPlay() { }
        
    }
}
