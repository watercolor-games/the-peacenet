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
    /// <summary>
    /// Provides various types of <see cref="Objective"/> objects usable in a <see cref="Mission"/>.   
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc { }

    /// <summary>
    /// A simple objective which plays a cutscene and completes when the cutscene is finished.
    /// </summary>
    public class CutsceneObjective : Objective
    {
        [Dependency]
        private CutsceneManager _cutscene = null;

        private string _ccId = null;
        private bool _complete = false;

        /// <summary>
        /// Creates a new instance of the <see cref="CutsceneObjective"/>. 
        /// </summary>
        /// <param name="name">The name of the objective.</param>
        /// <param name="description">The description of the objective.</param>
        /// <param name="id">The ID of the <see cref="Plex.Engine.Cutscene.Cutscene"/> to play.</param>
        public CutsceneObjective(string name, string description, string id) : base(name, description)
        {
            _ccId = id;
        }

        /// <inheritdoc/>
        public override void OnLoad()
        {
            _cutscene.Play(_ccId, () =>
            {
                _complete = true;
            });
        }

        /// <inheritdoc/>
        public override bool Update(GameTime time)
        {
            return _complete;
        }
    }
}
