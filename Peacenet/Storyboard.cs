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
    /// <summary>
    /// The Peacenet Storyboard engine component, which contains methods useful for missions and other storyline elements.
    /// </summary>
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

        /// <summary>
        /// Starts a mission.
        /// </summary>
        /// <param name="mission">The mission to start.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mission"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the player is already in a mission.</exception> 
        public void StartMission(Mission mission)
        {
            if (mission == null)
                throw new ArgumentNullException(nameof(mission));
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

        /// <summary>
        /// SLOW: Retrieves a list of available missions.
        /// </summary>
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

        /// <summary>
        /// Force-quits the current mission as the engine unloads.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _currentMission = null;
            _missionStarted.Set();
        }
    }

    /// <summary>
    /// A class representing a Peacenet story mission.
    /// </summary>
    public abstract class Mission
    {
        /// <summary>
        /// Retrieves the name of the mission.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Retrieves the description of the mission.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Retrieves an ordered <see cref="Queue{Objective}"/> containing a list of all tasks the player must perform to complete this mission. 
        /// </summary>
        public abstract Queue<Objective> Objectives { get; }

        /// <summary>
        /// Retrieves whether the mission is available to be played.
        /// </summary>
        public abstract bool IsAvailable { get; }
        /// <summary>
        /// Retrieves whether the mission has been completed yet. If true, the mission cannot be played regardless of <see cref="IsAvailable"/>'s value. 
        /// </summary>
        public abstract bool IsComplete { get; }
        /// <summary>
        /// Occurs when all objectives have been completed. This is a good time to mark the mission itself as complete.
        /// </summary>
        public abstract void OnComplete();
        /// <summary>
        /// Occurs when the mission is about to start.
        /// </summary>
        public abstract void OnStart();
    }

    /// <summary>
    /// A class representing a task for the player to complete in a <see cref="Mission"/>. 
    /// </summary>
    public abstract class Objective
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Objective"/> class. 
        /// </summary>
        /// <param name="name">The name of the objective.</param>
        /// <param name="description">The description of the objective.</param>
        public Objective(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Retrieves the name of this objective.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Retrieves the description of the objective.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// A method which is run when the objective is started.
        /// </summary>
        public virtual void OnLoad()
        {

        }

        /// <summary>
        /// Occurs every frame while the objective is active and determines whether the objective is completed.
        /// </summary>
        /// <param name="time">The time that has passed since the last frame update. See the MonoGame <see cref="GameTime"/> class for details.</param>
        /// <returns>Whether the objective is now complete.</returns>
        public abstract bool Update(GameTime time);

        /// <summary>
        /// Occurs once the objective is completed and unloaded.
        /// </summary>
        public virtual void OnUnload()
        {

        }

    }
}
