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

        private MissionEntity _entity = null;

        private Mission[] _missions = null;

        /// <summary>
        /// Retrieves a list of all available missions in the game.
        /// </summary>
        public Mission[] Missions
        {
            get
            {
                return _missions.Where(x => (x.Completed == false) && x.Available).ToArray();
            }
        }

        /// <inheritdoc/>
        public void Initiate()
        {
            _entity = _plexgate.New<MissionEntity>();

            _os.SessionStart += () =>
            {
                _plexgate.GetLayer(LayerType.Foreground).AddEntity(_entity);
                _entity.NotifyOfNewMissions();
            };

            _os.SessionEnd += () =>
            {
                _plexgate.GetLayer(LayerType.Foreground).RemoveEntity(_entity);
            };

            List<Mission> missions = new List<Mission>();
            Logger.Log("Looking for mission objects...");
            foreach(var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(Mission)))
            {
                var mission = (Mission)_plexgate.Inject(Activator.CreateInstance(type, null));
                Logger.Log($"Found: {mission.Name} ({type.FullName})");
                missions.Add(mission);
            }
            _missions = missions.ToArray();
        }


    }

    internal class MissionEntity : IEntity
    {
        private Mission _current = null;
        private int _state = -1;

        public bool IsPlayingMission
        {
            get
            {
                return _current != null;
            }
        }

        public void NotifyOfNewMissions()
        {
            _state = 0;
        }

        [Dependency]
        private WindowSystem _winsys = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private MissionManager _manager = null;

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
            switch(_state)
            {
                case 0:
                    if (_os.IsDesktopOpen)
                    {
                        var desk = _os.Desktop;
                        if (!desk.Visible)
                            return;
                        if (_plexgate.GetLayer(LayerType.UserInterface).Entities.FirstOrDefault(x => x is TutorialInstructionEntity) != null)
                            return;
                        int count = _manager.Missions.Length;
                        if(count>0)
                        {
                            if (count == 1)
                                desk.ShowNotification("New mission available", "A new mission has been added to your Missions list for you to play.");
                            else
                                desk.ShowNotification("New missions available", $"There are {count} new missions in your Missions list available for you to play.");
                        }
                        _state++;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Represents a Peacenet Campaign mission.
    /// </summary>
    public abstract class Mission
    {
        [Dependency]
        private SaveManager _save = null;

        /// <summary>
        /// Retrieves the name of this mission.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Retrieves the description of the mission.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Retrieves whether the mission is currently available.
        /// </summary>
        public abstract bool Available { get; }

        /// <summary>
        /// Retrieves whether the mission has been completed.
        /// </summary>
        public bool Completed
        {
            get
            {
                return _save.GetValue<bool>("mission." + Name.ToLower().Replace(" ", "_") + ".completed", false);
            }
        }
    }
}
