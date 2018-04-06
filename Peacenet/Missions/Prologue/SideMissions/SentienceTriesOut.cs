using Microsoft.Xna.Framework.Audio;
using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Missions.Prologue.SideMissions
{
    public class SentienceTriesOut : Mission
    {
        public override string Name => "Sentience Tries Out";

        public override string Description => "Welcome to Peacegate OS. To help you get acustomed to your new environment, we have prepared 5 simple tasks for you to complete within the operating system. Can you complete them in a timely manner?";

        public override bool Available => true;

        private SoundEffectInstance _philTask1 = null;
        private bool _task1Started = false;

        private SoundEffectInstance _philTask2 = null;
        private bool _task2Started = false;

        private SoundEffectInstance _philTask3 = null;
        private bool _task3Started = false;


        private bool _clockOpened = true; //change to false when clock exists and can be detected as being open.
        private bool _textWritten = true; //change to false when text editor lets you see document text.
        private bool _textSaved = true; //change to false when text can be seen being saved
        private bool _textOpened = true; //change to false when text can be seen being opened
        private bool _textDeleted = true; //change to false when text can be seen being deleted
        private bool _calculationDone = true; //change to false when calculator is finished and answers can be seen
        private bool _desktopCustomized = true; //change to false when desktop customizations are being watched.
        private bool _appInstalled = true; //change to false when PPM is implemented and can be watched




        [Dependency]
        private Plexgate _plexgate = null;

        public override void OnStart()
        {
            _philTask1 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/2").CreateInstance();
            _philTask2 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/3").CreateInstance();
            _philTask3 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/4").CreateInstance();
        }

        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("What time is it?", "First, a very simple task. See if you can find a clock program.", (time) =>
                {
                    if(_task1Started==false)
                    {
                        _philTask1.Play();
                        _task1Started = true;
                    }
                    return (_philTask1.State == SoundState.Playing || _clockOpened == false) ? ObjectiveState.Active : ObjectiveState.Complete;
                });
                yield return new Objective("Write, save, open and delete a Text Document.", "You won't get far in Peacegate if you can't view or edit documents. See if you can find a GUI-based text editor to help you out.", (time) =>
                {
                    if (_task2Started == false)
                    {
                        _philTask2.Play();
                        _task2Started = true;
                    }
                    return (_philTask2.State == SoundState.Playing || _textWritten == false) ? ObjectiveState.Active : ObjectiveState.Complete;
                });
            }
        }
    }
}
