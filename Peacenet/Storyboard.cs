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
using Plex.Objects;
using Microsoft.Xna.Framework.Input;

namespace Peacenet
{
    public class Storyboard : IEngineComponent, IDisposable
    {
        private class StoryboardEntity : IEntity
        {
            [Dependency]
            private Storyboard _story = null;

            public void Draw(GameTime time, GraphicsContext gfx)
            {
            }

            public void OnKeyEvent(KeyboardEventArgs e)
            {
            }

            public void OnMouseUpdate(MouseState mouse)
            {
            }

            public void Update(GameTime time)
            {
                if (_story._currentObjective != null)
                {
                    if (_story._completed == false)
                    {
                        if (_story._currentObjective.Update(time) == true)
                        {
                            _story._completed = true;
                            _story._objectiveComplete.Set();
                        }
                    }
                }
            }
        }

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

        private List<Mission> _foundMissions = new List<Mission>();

        private Mission _currentMission = null;

        private int _state = 0;

        private Objective _currentObjective = null;

        private bool _running = true;

        private Mission[] _available = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public void StartMission(Mission mission)
        {
            if (_currentMission != null)
                throw new InvalidOperationException("Cannot start a mission while another mission is in progress.");
            _currentMission = mission;
            _missionStarted.Set();
        }
        
        [Dependency]
        private InfoboxManager _infobox = null;

        public void Initiate()
        {
            Logger.Log("Storyboard is looking for missions...");

            foreach(var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(Mission)))
            {
                var mission = (Mission)Activator.CreateInstance(type, null);
                Logger.Log($"Found mission: {mission.Name.ToUpper()} from {type.FullName}.");
                _plexgate.Inject(mission);
                _foundMissions.Add(mission);
            }

            Logger.Log("Done.");

            Task.Run(() =>
            {
                while (_running)
                {
                    try
                    {
                        _missionStarted.WaitOne();
                        if (_currentMission != null)
                        {
                            _currentMission.OnStart();
                            if (_currentMission.Objectives != null)
                            {
                                var objectives = _currentMission.Objectives;
                                if (objectives != null)
                                {
                                    while (objectives.Count > 0)
                                    {
                                        _currentObjective = objectives.Dequeue();
                                        if (_currentObjective == null)
                                            continue;
                                        _completed = false;
                                        _plexgate.Inject(_currentObjective);
                                        _currentObjective.OnLoad();
                                        _objectiveComplete.WaitOne();
                                        _currentObjective.OnUnload();
                                        _objectiveComplete.Reset();
                                    }
                                }
                                
                            }
                            _currentMission.OnComplete();
                            _currentMission = null;
                        }
                        _missionStarted.Reset();
                    }
                    catch(Exception ex)
                    {
                        _infobox.Show("Mission aborted.", $"Sorry to break the immersion, but an error occurred while running a Peacenet mission that required the mission to be aborted.\r\n\r\n{ex.Message}");
                        _currentMission = null;
                        _currentObjective = null;
                        _missionStarted.Reset();
                        _objectiveComplete.Reset();
                    }
                }
            });
            var layer = new Layer();
            _entity = _plexgate.New<StoryboardEntity>();
            layer.AddEntity(_entity);
            _plexgate.AddLayer(layer);
        }

        private StoryboardEntity _entity = null;

        private Thread _bgThread = null;

        public Mission[] AvailableMissions
        {
            get
            {
                if (_server.IsMultiplayer)
                    return new Mission[0];
                return _foundMissions.Where(x => x.IsAvailable == true && x.IsComplete == false).ToArray();
            }
        }

        private bool _completed = false;

        public void Dispose()
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

        public abstract Queue<Objective> Objectives { get; }

        public abstract bool IsAvailable { get; }
        public abstract bool IsComplete { get; }
        public abstract void OnComplete();
        public abstract void OnStart();
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
