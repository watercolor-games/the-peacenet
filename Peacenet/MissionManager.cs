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
        private Plexgate _plexgate = null;

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
            var mission = _plexgate.GetLayer(LayerType.Foreground).Entities.FirstOrDefault(x => x is Mission);
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
                var mission = (Mission)_plexgate.New(type);
                missions.Add(mission);
                Logger.Log($"Found: {mission.Name} (ID {mission.ID})");
            }
            _missions = missions.ToArray();
            
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
    }

    /// <summary>
    /// Represents a Peacenet Campaign mission entity with a name, description, objective list, and a lot of useful things for scripting out the story.
    /// </summary>
    public abstract class Mission : IEntity
    {
        [Dependency]
        private Plexgate _plexgate = null;

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

        private List<ObjectiveMedalData> _medals = new List<ObjectiveMedalData>();

        private struct ObjectiveMedalData
        {
            private string _name;
            private Medal _medal;

            public string Name => _name;
            public Medal Medal => _medal;

            public ObjectiveMedalData(string name, Medal medal)
            {
                _medal = medal;
                _name = name;
            }

            public static ObjectiveMedalData Empty => new ObjectiveMedalData("", Medal.Bronze);
        }

        private string[] _deps = null;

        public string Name => _name;
        public string Description => _desc;
        public string ObjectiveName => _current.Name;
        public string ID => _id;
        public string[] DependencyIDs => _deps;

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

        protected void AddObjective(string name, double timeout = 0)
        {
            _objectives.Add(new Objective(name, timeout));
        }

        private void nextObjective()
        {
            _preObjectiveSaveState = _save.CreateSnapshot();
            _index++;
            _current = _objectives[_index];
            _hasTimeout = _current.Timeout > 0;
            _timeout = _current.Timeout;
        }

        public void Start()
        {
            if (Completed)
                throw new InvalidOperationException("The mission is already completed.");
            if (!Available)
                throw new InvalidOperationException("The mission isn't available yet");
            if (_plexgate.GetLayer(LayerType.Foreground).Entities.FirstOrDefault(x => x is Mission) != null)
                throw new InvalidOperationException("Another mission is currently in progress.");
            if (_objectives.Count == 0)
                throw new InvalidOperationException("No objectives have been defined for this mission.");

            _index = -1;

            _preMissionSaveState = _save.CreateSnapshot(); //If the user abandons the mission, revert to this state.
            
            _plexgate.GetLayer(LayerType.Foreground).AddEntity(this);

            nextObjective();

        }

        public void Abandon()
        {
            if (!_plexgate.GetLayer(LayerType.Foreground).HasEntity(this))
                throw new InvalidOperationException("The mission is not being played and cannot be abandoned.");
            Fail("You abandoned the mission.");
        }

        protected void CompleteObjective(Medal medal)
        {
            _medals.Add(new ObjectiveMedalData(_current.Name, medal));
            nextObjective();
        }

        protected void Fail(string message)
        {
            _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(this);
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

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        protected abstract void UpdateObjective(GameTime time, int objectiveIndex);

        public void Update(GameTime time)
        {
            if(_hasTimeout)
            {
                _timeout -= time.ElapsedGameTime.TotalSeconds;
                if(_timeout<= 0)
                {
                    Fail("You ran out of time.");
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

        public string Name => _name;
        public double Timeout => _timeout;

        public Objective(string name, double timeout)
        {
            _name = name;
            _timeout = timeout;
        }

        public static Objective Empty => new Objective("", 0);
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
