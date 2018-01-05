using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using System.IO;
using Plex.Objects;

namespace Plex.Engine.Cutscene
{
    public class CutsceneManager : IEngineComponent, IDisposable
    {


        [Dependency]
        private Plexgate _plexgate = null;

        private Layer _cutsceneLayer = new Layer();

        private List<Cutscene> _cutscenes = null;
        private Cutscene _current = null;
        
        public int DrawIndex
        {
            get
            {
                return 5;
            }
        }

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

        public void Stop(bool runCallback = true)
        {
            if(_current != null)
            {
                _current.IsFinished = true;
                _current.OnFinish();
                _cutsceneLayer.RemoveEntity(_current);
                if (runCallback)
                    _callback?.Invoke();
                _callback = null;
                _current = null;
            }
        }

        public bool Play(string name, Action callback = null)
        {
            var cs = _cutscenes.FirstOrDefault(x => x.Name == name);
            if (cs == null)
                return false;
            Stop(false);
            _callback = callback;
            cs.IsFinished = false;
            _current = cs;
            _current.OnPlay();
            _cutsceneLayer.AddEntity(_current);
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
                cs.Load(_plexgate.Content);
                _cutscenes.Add(cs);
            }
            Logger.Log($"{_cutscenes.Count} cutscenes loaded.");
            _cutsceneLayer = new Engine.Layer();
            _plexgate.AddLayer(_cutsceneLayer);
        }

        public void Dispose()
        {
            while(_cutscenes.Count == 0)
            {
                var cs = _cutscenes[0];
                cs.Dispose();
                _cutscenes.RemoveAt(0);
                cs = null;
            }
            _cutscenes = null;
        }
    }
}
