using Plex.Engine;
using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Saves;
using Plex.Objects;
using Plex.Engine.GUI;
using Plex.Engine.Themes;
using Peacenet.Applications;
using Peacenet.RichPresence;
using Plex.Engine.Cutscene;
using Peacenet.Missions;

namespace Peacenet
{
    /// <summary>
    /// Provides a backend utility for managing Campaign missions.
    /// </summary>
    public class MissionManager : IEngineComponent
    {
        [Dependency]
        private OS _os = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private GameManager _game = null;

        public void AbandonMission()
        {
            var mission = getCurrent();
            if (mission == null)
                throw new InvalidOperationException("There are currently no missions active.");
            mission.Abandon();
        }

        private Mission getCurrent()
        {
            var mission = _GameLoop.GetLayer(LayerType.Foreground).Entities.FirstOrDefault(x => x is Mission);
            if (mission != null)
                return (Mission)mission;
            return null;
        }

        /// <summary>
        /// Gets whether or not a mission is currently in progress.
        /// </summary>
        public bool IsPlayingMission
        {
            get
            {
                return getCurrent() != null;
            }
        }

        public Mission[] Missions => _missions;

        public Mission CurrentMission => getCurrent();

        private Mission[] _missions = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("Looking for missions...");
            List<Mission> missions = new List<Mission>();
            foreach(var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(Mission)))
            {
                var mission = (Mission)_GameLoop.New(type);
                missions.Add(mission);
                Logger.Log($"Found: {mission.Name} (ID {mission.ID})");
            }
            _missions = missions.ToArray();
            _game.MissionCompleted += (id) =>
            {
                Logger.Log($"Good job, player! Mission {id} completed.", System.ConsoleColor.Green);
                int mCount = _missions.Where(x => x.Available && !x.Completed).Count();
                if (mCount > 0)
                {
                    if(mCount == 1)
                    {
                        _os.Desktop.ShowNotification("New mission available", "Check your World Map or Missions menu to see the new mission.");
                    }
                    else
                    {
                        _os.Desktop.ShowNotification("New missions available", $"There are {mCount} Missions available. Check your World Map or Missions menu for more info.");
                    }
                }
            };
            _os.SessionStart += () =>
            {
                int mCount = _missions.Where(x => x.Available && !x.Completed).Count();
                if (mCount > 0)
                {
                    if (mCount == 1)
                    {
                        _os.Desktop.ShowNotification("New mission available", "Check your World Map or Missions menu to see the new mission.");
                    }
                    else
                    {
                        _os.Desktop.ShowNotification("New missions available", $"There are {mCount} Missions available. Check your World Map or Missions menu for more info.");
                    }
                }
            };
        }

        /// <summary>
        /// Start a mission.
        /// </summary>
        /// <param name="mission">The mission to start</param>
        [Obsolete("Please use Mission.Start().")]
        public void StartMission(Mission mission)
        {
            mission.Start();
        }

        public Mission[] Available
        {
            get
            {
                return _missions.Where(x => x.Available).ToArray();
            }
        }
    }

    /// <summary>
    /// Represents a Peacenet Campaign mission entity with a name, description, objective list, and a lot of useful things for scripting out the story.
    /// </summary>
    public abstract class Mission : IEntity
    {
        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private WindowSystem _win = null;

        [Dependency]
        private GameManager _game = null;

        private string _preMissionSaveState = null;
        private string _preObjectiveSaveState = null;

        private string _name = "Mission name";
        private string _desc = "This is a Peacenet mission.";
        private string _id = "mission_id";
        private const string _idValid = "abcdefghijklmnopqrstuvwxyz0123456789-_";

        private List<Objective> _objectives = new List<Objective>();

        private int _index = -1;
        private Objective _current = Objective.Empty;
        private double _timeout = 0;
        private bool _hasTimeout = false;

        private List<ObjectiveMedal> _medals = new List<ObjectiveMedal>();

        private string[] _deps = null;

        public string Name => _name;
        public string Description => _desc;
        public string ObjectiveName => _current.Name;
        public string ID => _id;
        public string[] DependencyIDs => _deps;
        private TimeoutType _timeoutType = TimeoutType.Complete;

        private double _timeoutDuration = 0;

        public TimeoutType TimeoutType => _timeoutType;
        public bool HasTimeout => _hasTimeout;
        public double TimeoutDuration => _timeoutDuration;

        private string makeID(string str)
        {
            
            string text = "";
            foreach(var c in str.ToLower())
            {
                if (_idValid.Contains(c))
                    text += c;
                else
                    text += "_";
            }
            return text;
        }

        public int ObjectiveIndex => _index;
        public double Timeout => _timeout;

        public Mission(string name, string desc)
        {
            _id = makeID(name);
            _name = name;
            _desc = desc;

            var type = GetType();
            var attributes = type.GetCustomAttributes(true).Where(x => x is RequiresMissionAttribute).Select(x => (x as RequiresMissionAttribute).ID);
            _deps = attributes.ToArray();
        }

        public bool Available
        {
            get
            {
                if (Completed)
                    return false;
                return _deps.Where(x => !_game.State.IsMissionComplete(x)).Count() == 0;
            }
        }

        public bool Completed
        {
            get
            {
                return _game.State.IsMissionComplete(_id);
            }
        }

        protected void AddObjective(string name, double timeout = 0, TimeoutType timeoutType = TimeoutType.Fail)
        {
            _objectives.Add(new Objective(name, timeout, timeoutType));
        }

        private void nextObjective()
        {
            _preObjectiveSaveState = _save.CreateSnapshot();
            _index++;
            _current = _objectives[_index];
            _hasTimeout = _current.Timeout > 0;
            _timeout = _current.Timeout;
            _timeoutDuration = _current.Timeout;
            _timeoutType = _current.TimeoutType;
        }

        public void Start()
        {
            if (Completed)
                throw new InvalidOperationException("The mission is already completed.");
            if (!Available)
                throw new InvalidOperationException("The mission isn't available yet");
            if (_GameLoop.GetLayer(LayerType.Foreground).Entities.FirstOrDefault(x => x is Mission) != null)
                throw new InvalidOperationException("Another mission is currently in progress.");
            if (_objectives.Count == 0)
                throw new InvalidOperationException("No objectives have been defined for this mission.");

            _index = -1;

            _preMissionSaveState = _save.CreateSnapshot(); //If the user abandons the mission, revert to this state.
            
            _GameLoop.GetLayer(LayerType.Foreground).AddEntity(this);

            OnStart();

            nextObjective();

        }

        public void Abandon()
        {
            if (!_GameLoop.GetLayer(LayerType.Foreground).HasEntity(this))
                throw new InvalidOperationException("The mission is not being played and cannot be abandoned.");
            Fail("You abandoned the mission.");
        }

        private void Complete()
        {
            var completeDialog = new MissionCompleteScreen(new MissionData
            {
                ID = _id,
                Name = _name,
                ObjectiveMedals = _medals.ToArray(),
                Unlocks = null
            }, _win);
            OnEnd();
            completeDialog.Show();
            _GameLoop.GetLayer(LayerType.Foreground).RemoveEntity(this);
        }

        protected void CompleteObjective(Medal medal = Medal.Gold, string description = "For successfully completing the objective")
        {
            _medals.Add(new ObjectiveMedal(_current.Name, description, medal));
            if (_index == _objectives.Count - 1)
            {
                Complete();
            }
            else
            {
                nextObjective();
            }
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }


        protected void Fail(string message)
        {
            OnEnd();
            _GameLoop.GetLayer(LayerType.Foreground).RemoveEntity(this);
            var fail = new MissionFailScreen(this, message, _preMissionSaveState, _preObjectiveSaveState, _win);
            fail.Show();
        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            
        }

        public void OnGameExit()
        {
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        protected abstract void UpdateObjective(GameTime time, int objectiveIndex);

        public void Update(GameTime time)
        {
            if(_hasTimeout)
            {
                _timeout -= time.ElapsedGameTime.TotalSeconds;
                if (_timeout <= 0)
                {
                    switch (_timeoutType)
                    {
                        case TimeoutType.Fail:
                            Fail("You ran out of time.");
                            break;
                        case TimeoutType.Complete:
                            CompleteObjective();
                            break;
                    }
                    return;
                }
            }
            UpdateObjective(time, _index);
        }


    }

    public struct Objective
    {
        private double _timeout;
        private string _name;
        private TimeoutType _timeoutType;

        public string Name => _name;
        public double Timeout => _timeout;
        public TimeoutType TimeoutType => _timeoutType;

        public Objective(string name, double timeout, TimeoutType timeoutType)
        {
            _name = name;
            _timeout = timeout;
            _timeoutType = timeoutType;
        }

        public static Objective Empty => new Objective("", 0, TimeoutType.Complete);
    }

    /// <summary>
    /// Represents a value indicating how the mission system should handle an objective timeout.
    /// </summary>
    public enum TimeoutType
    {
        /// <summary>
        /// Specifies that a mission should fail when an objective times out.
        /// </summary>
        Fail,
        /// <summary>
        /// Specifies that an objective should complete if it times out.
        /// </summary>
        Complete
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresMissionAttribute : Attribute
    {
        private string _id = null;

        public string ID => _id;

        public RequiresMissionAttribute(string id)
        {
            _id = id;
        }
    }
}
