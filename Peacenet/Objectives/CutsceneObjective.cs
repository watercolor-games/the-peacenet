using Plex.Engine;
using Plex.Engine.Cutscene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Objectives
{
    public class CutsceneObjective : Objective
    {
        [Dependency]
        private CutsceneManager _cutscene = null;

        private string _ccId = null;
        private bool _complete = false;

        public CutsceneObjective(string name, string description, string id) : base(name, description)
        {
            _ccId = id;
        }

        public override void OnLoad()
        {
            _cutscene.Play(_ccId, () =>
            {
                _complete = true;
            });
        }

        public override bool Update(GameTime time)
        {
            return _complete;
        }
    }
}
