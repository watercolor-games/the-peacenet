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

        //What time is it?
        private SoundEffectInstance _philTask1 = null;
        private bool _task1Started = false;

        //Write, save, open and delete a text document (Part 1: Write)
        private SoundEffectInstance _philTask2 = null;
        private bool _task2Started = false;

        //Write, save, open and delete a text document (Part 2: Save)
        private SoundEffectInstance _philTask3 = null;
        private bool _task3Started = false;

        //Write, save, open and delete a text document (Part 4: Delete) - Phil keeps messing up the 5th voice clip so we'll fix that later
        private SoundEffectInstance _philTask5 = null;
        private bool _task5Started = false;
        
        //Calculate 8 + 5 ^ 2 / 3
        private SoundEffectInstance _philTask6 = null;
        private bool _task6Started = false;

        //Personalize the Desktop
        private SoundEffectInstance _philTask7 = null;
        private bool _task7Started = false;

        //Install a program.
        private SoundEffectInstance _philTask8 = null;
        private bool _task8Started = false;

        public override string AfterCompleteCutscene => "sto_osft_outro";

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
            _philTask5 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/6").CreateInstance();
            _philTask6 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/7").CreateInstance();
            _philTask7 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/9").CreateInstance(); //Why was 8 not invited? Windows 8, of course.
            _philTask8 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/10").CreateInstance(); 

        }

        public override string PrerollCutscene => "sto_osft_peacegateIntro";

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
                    else if (_textWritten && _philTask2.State == SoundState.Stopped && _task3Started == false)
                    {
                        _philTask3.Play();
                        _task3Started = true;
                    }
                    else if (_textSaved && _philTask3.State == SoundState.Stopped && _task5Started == false)
                    {
                        _philTask5.Play();
                        _task5Started = true;
                    }
                    return (_task5Started == true && _philTask5.State != SoundState.Playing && (_textWritten && _textSaved && _textOpened && _textDeleted)) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Calculate 8 + 5 ^ 2 / 3", "No, that's not a math question. This isn't school. We just want to see if you can calculate things in Peacegate OS.", (time) =>
                {
                    if(_task6Started == false)
                    {
                        _philTask6.Play();
                        _task6Started = true;
                    }
                    return (_philTask6.State != SoundState.Playing && _calculationDone) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Personalize the Desktop", "Most normal operating systems usually allow some form of customization/appearance settings. See if Peacegate has the same.", (time) =>
                {
                    if(_task7Started==false)
                    {
                        _philTask7.Play();
                        _task7Started = true;
                    }
                    return (_philTask7.State != SoundState.Playing && _desktopCustomized) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Install a new program", "You can't get far with the base Peacegate programs, although they're user-friendly. See if there's a way to install new programs to your computer.", (time) =>
                {
                    if(_task8Started==false)
                    {
                        _philTask8.Play();
                        _task8Started = true;
                    }
                    return (_philTask8.State != SoundState.Playing && _appInstalled) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
            }
        }
    }
}
