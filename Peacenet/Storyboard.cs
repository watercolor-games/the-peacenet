using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Plex.Engine.Saves;
using Plex.Engine.Cutscene;
using Plex.Engine.Server;
using System.Threading;

namespace Peacenet
{
    public class Storyboard : IEngineComponent
    {
        [Dependency]
        private OS _os = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private CutsceneManager _cutscene = null;

        [Dependency]
        private AsyncServerManager _server = null;

        private EventWaitHandle _missionStarted = new ManualResetEvent(false);
        private EventWaitHandle _objectiveComplete = new ManualResetEvent(false);


        private Mission _currentMission = null;

        private int _state = 0;

        private Objective _currentObjective = null;

        private bool _running = true;

        [Dependency]
        private Plexgate _plexgate = null;

        public void Initiate()
        {
            Task.Run(() =>
            {
                while (_running)
                {
                    _missionStarted.WaitOne();
                    if (_currentMission != null)
                    {
                        if (_currentMission.Objectives != null)
                        {
                            while(_currentMission.Objectives.Count > 0)
                            {
                                _currentObjective = _currentMission.Objectives.Dequeue();
                                if (_currentObjective == null)
                                    continue;
                                _plexgate.Inject(_currentObjective);
                                _currentObjective.OnLoad();
                                _objectiveComplete.WaitOne();
                                _currentObjective.OnUnload();
                                _objectiveComplete.Reset();
                            }

                        }
                        _save.SetValue(_currentMission.SaveValue, true);
                        _currentMission = null;
                    }
                    _missionStarted.Reset();
                }
            });
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            
        }

        public void Unload()
        {
            _running = false;
            _currentMission = null;
            _missionStarted.Set();
        }
    }

    public abstract class Mission
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string SaveValue { get; }

        public abstract Queue<Objective> Objectives { get; }
    }

    public class Objective
    {
        public Objective(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }

        public virtual void OnLoad()
        {

        }

        public virtual bool Update(GameTime time)
        {
            return false;
        }

        public virtual void OnUnload()
        {

        }

    }
}
