using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public static class TutorialManager
    {
        private static ITutorial _tut = null;

        public static void RegisterTutorial(ITutorial tut)
        {
            _tut = tut;
            _tut.OnComplete += (o, a) =>
            {
                SaveSystem.CurrentSave.StoryPosition = 2;
            };
        }

        public static void StartTutorial()
        {
            _tut.Start();
        }
    }

    public interface ITutorial
    {
        void Start();
        event EventHandler OnComplete;
    }
}
