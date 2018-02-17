using Plex.Engine;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Missions.Prologue
{
    /// <summary>
    /// The very first Peacenet mission.
    /// </summary>
    public class TerminalTutorial : Mission
    {
        [Dependency]
        private SaveManager _save = null;

        /// <inheritdoc/>
        public override bool Available
        {
            get
            {
                return _save.GetValue("boot.hasDoneCmdTutorial", false);
            }
        }

        /// <inheritdoc/>
        public override string Description
        {
            get
            {
                return "Welcome to your new Peacegate environment. Now that you know the basics of the Peacegate OS GUI, it is time for you to learn how to use the Terminal.";
            }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "The Terminal";
            }
        }
    }
}
