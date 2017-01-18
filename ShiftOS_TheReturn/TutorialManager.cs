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
            IsInTutorial = false;
            _tut = tut;
            _tut.OnComplete += (o, a) =>
            {
                SaveSystem.CurrentSave.StoryPosition = 2;
                IsInTutorial = false;
            };
        }

        public static bool IsInTutorial
        {
            get; private set;
        }

        public static int Progress
        {
            get
            {
                return _tut.TutorialProgress;
            }
        }

        public static void StartTutorial()
        {
            IsInTutorial = true;
            _tut.Start();
        }
    }

    public interface ITutorial
    {
        int TutorialProgress { get; set; }
        void Start();
        event EventHandler OnComplete;
    }

    public class TutorialLockAttribute : Attribute
    {
        public TutorialLockAttribute(int progress)
        {
            Progress = progress;
        }
        
        public TutorialLockAttribute() : this(0)
        {
            
        }

        public int Progress { get; private set; }
    }
}
