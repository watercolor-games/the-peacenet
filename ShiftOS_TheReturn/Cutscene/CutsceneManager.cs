using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.DebugConsole;
using System.IO;
using Plex.Objects;

namespace Plex.Engine.Cutscene
{
    public class CutsceneManager : IEngineComponent
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private UIManager _ui = null;

        private List<Cutscene> _cutscenes = null;
        private Cutscene _current = null;
        
        public bool IsPlaying
        {
            get
            {
                if (_current == null)
                    return false;
                return !_current.IsFinished;
            }
        }

        private Action _callback = null;

        public bool Play(string name, Action callback = null)
        {
            var cs = _cutscenes.FirstOrDefault(x => x.Name == name);
            if (cs == null)
                return false;
            if (_current != null)
            {
                _current.IsFinished = true;
                _current = null;
            }
            _callback = callback;
            cs.IsFinished = false;
            _current = cs;
            _current.OnPlay();
            _ui.HideUI();
            return true;
        }

        public void Initiate()
        {
            _cutscenes = new List<Cutscene>();
            Logger.Log("Looking for coded cutscenes...");
            foreach (var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(Cutscene)))
            {
                var cs = (Cutscene)Activator.CreateInstance(type, null);
                Logger.Log($"Found: {cs.Name}");
                if (_cutscenes.FirstOrDefault(x => x.Name == cs.Name)!=null)
                {
                    Logger.Log("...but it's a duplicate.");
                    continue;
                }
                _plexgate.Inject(cs);
                cs.Content = _plexgate.Content;
                cs.LoadResources();
                _cutscenes.Add(cs);
            }
            Logger.Log($"{_cutscenes.Count} cutscenes loaded.");
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            if(_current != null)
            {
                _current.Draw(time, ctx);
            }
        }

        public void OnGameUpdate(GameTime time)
        {
            if (_current != null)
            {
                _current.Update(time);
                if (_current.IsFinished)
                {
                    _current.OnFinish();
                    _callback?.Invoke();
                    _current = null;
                    _ui.ShowUI();
                }
            }
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            while(_cutscenes.Count == 0)
            {
                var cs = _cutscenes[0];
                cs.UnloadResources();
                _cutscenes.RemoveAt(0);
                cs = null;
            }
            _cutscenes = null;
        }
    }

    public class HideUICommand : IDebugCommand
    {
        public string Description
        {
            get
            {
                return "Tests smooth UI-to-cutscene transition. WARNING - THIS WILL PUT THE GAME IN AN UNRECOVERABLE STATE!";
            }
        }

        public string Name
        {
            get
            {
                return "smoothcs";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                return new List<string>();
            }
        }

        [Dependency]
        private UIManager _ui = null;

        public void Run(StreamWriter stdout, StreamReader stdin, Dictionary<string, object> args)
        {
            _ui.HideUI();
        }
    }

    public class CutsceneDebugCmd : IDebugCommand
    {
        public string Description
        {
            get
            {
                return "Play a coded cutscene.";
            }
        }

        public string Name
        {
            get
            {
                return "playccs";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                yield return "<name>";
            }
        }

        [Dependency]
        private CutsceneManager _cs = null;

        public void Run(StreamWriter stdout, StreamReader stdin, Dictionary<string, object> args)
        {
            string _name = args["<name>"].ToString();
            if (_cs.Play(_name) == false)
                stdout.WriteLine("Cutscene not found.");
        }
    }
}
