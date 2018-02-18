using Microsoft.Xna.Framework.Audio;
using Peacenet.Applications;
using Plex.Engine;
using Plex.Engine.GUI;
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

        [Dependency]
        private WindowSystem _winsys = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private SoundEffect _bgm = null;
        private SoundEffectInstance _bgmInstance = null;

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

        /// <inheritdoc/>
        public override void OnStart()
        {
            timeWasted = 0;
            _bgm = _plexgate.Content.Load<SoundEffect>("Audio/Mission1");
            _bgmInstance = _bgm.CreateInstance();
            _bgmInstance.IsLooped = true;
            _bgmInstance.Play();
            
            base.OnStart();
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            _bgmInstance.Stop();
            base.OnEnd();
        }

        private double timeWasted = 0;

        /// <inheritdoc/>
        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("Open a Terminal", "Open a Terminal to start this mission.", (time) =>
                {
                    var window = _winsys.WindowList.FirstOrDefault(x => x.Border.Window is Terminal);
                    if (window != null)
                        return ObjectiveState.Complete;
                    return ObjectiveState.Active;
                });
                yield return new Objective("This is a test.", "Please wait 10 seconds for the test to end.", (time) =>
                {
                    timeWasted += time.ElapsedGameTime.TotalSeconds;
                    return (timeWasted > 10) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
            }
        }
    }
}
