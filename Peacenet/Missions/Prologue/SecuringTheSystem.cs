using Plex.Engine;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Peacenet.Missions.Prologue
{
    public class SecuringTheSystem : Mission
    {
        private SoundEffect _missionTheme = null;
        private SoundEffectInstance _themeInstance = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private SaveManager _save = null;

        public override bool Available
        {
            get
            {
                return _save.GetValue<bool>("mission.the_terminal.completed", false);
            }
        }

        public override string Description
        {
            get
            {
                return "You now know the basics of the Peacenet command line. Now it's time to get your system connected to The Peacenet, and secured from the digital warfare.";
            }
        }

        public override string Name
        {
            get
            {
                return "Securing the System";
            }
        }

        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("Not yet implemented", "This objective is not yet implemented.", (time) =>
                {
                    return ObjectiveState.Active;
                });
            }
        }

        public override void OnStart()
        {
            _missionTheme = _plexgate.Content.Load<SoundEffect>("Audio/SecuringTheSystem");
            _themeInstance = _missionTheme.CreateInstance();
            _themeInstance.IsLooped = true;
            _themeInstance.Play();
            base.OnStart();
        }

        public override void OnEnd()
        {
            _themeInstance.Stop();
            base.OnEnd();
        }
    }
}
