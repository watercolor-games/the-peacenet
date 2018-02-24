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

        /// <summary>
        /// Gets whether or not a mission is currently in progress.
        /// </summary>
        public bool IsPlayingMission
        {
            get
            {
                return _entity.IsPlayingMission;
            }
        }

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

        /// <summary>
        /// Retrieves the current mission if any.
        /// </summary>
        public Mission CurrentMission
        {
            get
            {
                return _entity.CurrentMission;
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

        /// <summary>
        /// Start a mission.
        /// </summary>
        /// <param name="mission">The mission to start</param>
        public void StartMission(Mission mission)
        {
            if (IsPlayingMission)
                throw new InvalidOperationException("The player is already playing a mission.");
            if (mission == null)
                throw new ArgumentNullException(nameof(mission));
            if (mission.Completed)
                throw new InvalidOperationException("You cannot play a completed mission.");
            if (mission.Available == false)
                throw new InvalidOperationException("The specified mission is not available yet.");
            _entity.StartMission(mission);
        }
    }

    internal class MissionEntity : IEntity
    {
        /// <inheritdoc/>
        public void OnGameExit()
        {
            if(_current != null)
            {
                Logger.Log("Restoring pre-mission snapshot...");
                if(!string.IsNullOrWhiteSpace(_preMissionSnapshotId))
                {
                    Logger.Log("Restoring pre-mission snapshot...");
                    _save.RestoreSnapshot(_preMissionSnapshotId);
                }
            }
        }


        private Mission _current = null;
        private int _state = -1;
        private Objective[] _objectives = null;
        private int _currentObjective = 0;
        private int _animState = -1;

        [Dependency]
        private DiscordRPCModule _discord = null;

        private float _shroudFade = 0.0f;

        private float _missionStartFade = 0f;
        private float _missionNameFade = 0f;

        private double _missionStartRide = 0;

        private Color _shroudColor = Color.Black;

        private float _objectiveNameFade = 0f;
        private float _objectiveDescFade = 0f;
        private double _objectiveRide = 0f;

        private string _preMissionSnapshotId = null;
        private string _lastCheckpointSnapshotId = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private ThemeManager _theme = null;

        public Mission CurrentMission
        {
            get
            {
                return _current;
            }
        }

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

        /// <summary>
        /// Start a mission
        /// </summary>
        /// <param name="mission">The mission to start.</param>
        public void StartMission(Mission mission)
        {
            _current = mission;
            _objectives = _current.ObjectiveList.ToArray();
            _animState = 0;
            _preMissionSnapshotId = _save.CreateSnapshot();
            _current.OnStart();
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
            if (_current == null)
                return;
            gfx.BeginDraw();
            //Draw the shroud.
            gfx.Clear(_shroudColor * (_shroudFade / 2));

            var headFont = _theme.Theme.GetFont(TextFontStyle.Header1);
            var nameFont = _theme.Theme.GetFont(TextFontStyle.Header3);

            var headColor = _theme.Theme.GetFontColor(TextFontStyle.Header1);
            var nameColor = _theme.Theme.GetFontColor(TextFontStyle.Header3);

            string headString = "Mission Start";
            string nameString = _current.Name;

            var headMeasure = TextRenderer.MeasureText(headString, headFont, _winsys.Width / 2, Plex.Engine.TextRenderers.WrapMode.Words);
            var nameMeasure = TextRenderer.MeasureText(nameString, nameFont, (_winsys.Width / 2), Plex.Engine.TextRenderers.WrapMode.Words);

            float totalHeight = headMeasure.Y + 5 + nameMeasure.Y;
            float totalCenterY = (_winsys.Height - totalHeight) / 2;
            float headYMin = (totalCenterY + (_winsys.Height * 0.10f));

            gfx.DrawString(headString, (_winsys.Width - (int)headMeasure.X) / 2, (int)MathHelper.Lerp(headYMin, totalCenterY, _missionStartFade), headColor * _missionStartFade, headFont, TextAlignment.Left);

            float nameYMax = totalCenterY + headMeasure.Y + 5;
            float nameYMin = nameYMax + (_winsys.Height * 0.10f);

            gfx.DrawString(nameString, (_winsys.Width - (int)nameMeasure.X) / 2, (int)MathHelper.Lerp(nameYMin, nameYMax, _missionNameFade), nameColor * _missionNameFade, nameFont, TextAlignment.Left);

            string objectiveName = _objectives[_currentObjective].Name;
            string objectiveDesc = _objectives[_currentObjective].Description;
            string pressF6 = "Press [F6] to view objective info";

            var objDescFont = _theme.Theme.GetFont(TextFontStyle.System);
            var objDescColor = _theme.Theme.GetFontColor(TextFontStyle.System);

            var f6measure = TextRenderer.MeasureText(pressF6, objDescFont, (_winsys.Width / 2), Plex.Engine.TextRenderers.WrapMode.Words);

            float f6max = (_winsys.Height - f6measure.Y) - 45;
            float f6min = f6max + (_winsys.Height * 0.1f);
            gfx.DrawString(pressF6, 45, (int)MathHelper.Lerp(f6min, f6max, _objectiveDescFade), (objDescColor * 0.45F) * _objectiveDescFade, objDescFont, TextAlignment.Left, (_winsys.Width) / 2, Plex.Engine.TextRenderers.WrapMode.Words);

            var odescmeasure = TextRenderer.MeasureText(objectiveDesc, objDescFont, (_winsys.Width / 2), Plex.Engine.TextRenderers.WrapMode.Words);

            float odescMax = (f6max - odescmeasure.Y) - 15;
            float odescMin = odescMax + (_winsys.Height * 0.1f);
            gfx.DrawString(objectiveDesc, 45, (int)MathHelper.Lerp(odescMin, odescMax, _objectiveDescFade), objDescColor * _objectiveDescFade, objDescFont, TextAlignment.Left, (_winsys.Width) / 2, Plex.Engine.TextRenderers.WrapMode.Words);

            var onamemeasure = TextRenderer.MeasureText(objectiveName, nameFont, (_winsys.Width / 2), Plex.Engine.TextRenderers.WrapMode.Words);

            float onameMax = (((((_winsys.Height - 45) - f6measure.Y) - 15) - odescmeasure.Y) - 5) - onamemeasure.Y;
            float onameMin = onameMax + (_winsys.Height * 0.1f);
            gfx.DrawString(objectiveName, 45, (int)MathHelper.Lerp(onameMin, onameMax, _objectiveNameFade), nameColor * _objectiveNameFade, nameFont, TextAlignment.Left, (_winsys.Width) / 2, Plex.Engine.TextRenderers.WrapMode.Words);


            gfx.EndDraw();
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Keys.F6)
            {
                if(_current!=null)
                {
                    if (_animState < 7)
                        return;
                    if (_animState != 9)
                        return;
                    var missionMenu = new MissionMenu(_winsys);
                    missionMenu.Show();
                }
            }
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void Update(GameTime time)
        {
            if(_current == null)
            {
                _discord.GameState = "In Singleplayer";
                _discord.GameDetails = "Roaming the Peacenet";
            }

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
                        _state=-1;
                    }
                    break;
                case 1:
                    _discord.GameDetails = $"{_current.Name}";
                    _discord.GameState = "In Mission";
                    var obj = _objectives[_currentObjective];
                    var objState = obj.Update(time);
                    if (objState == ObjectiveState.Failed)
                        _state = 2;
                    else if(objState == ObjectiveState.Complete)
                    {
                        _current.OnObjectiveComplete(_currentObjective, _objectives[_currentObjective]);
                        if (_animState == 9)
                        {
                            _animState = 10;
                        }
                        else if(_animState < 9)
                        {
                            _animState = 7;
                            _objectiveNameFade = 0;
                            _objectiveDescFade = 0;
                            if (_currentObjective < _objectives.Length - 1)
                            {
                                _lastCheckpointSnapshotId = _save.CreateSnapshot();
                                _currentObjective++;
                                _current.OnObjectiveStart(_currentObjective, _objectives[_currentObjective]);
                            }
                            else
                            {
                                _animState = 12;
                            }

                        }
                    }
                    else if(objState == ObjectiveState.Failed)
                    {
                        _current.OnObjectiveFail(_currentObjective, _objectives[_currentObjective]);
                        _save.RestoreSnapshot(_preMissionSnapshotId);
                        _currentObjective = 0;
                        _current = null;
                        _objectives = null;
                        _preMissionSnapshotId = null;
                        _lastCheckpointSnapshotId = null;
                        _state = 0;
                        _objectiveNameFade = 0;
                        _objectiveDescFade = 0;
                        _animState = -1;
                    }
                    break;
            }

            switch(_animState)
            {
                case 0:
                    _shroudFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if(_shroudFade>=1)
                    {
                        _shroudFade = 1;
                        _animState++;
                    }
                    break;
                case 1:
                    _missionStartFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_missionStartFade >= 1)
                    {
                        _missionStartFade = 1;
                        _animState++;
                    }
                    break;
                case 2:
                    _missionNameFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_missionNameFade >= 1)
                    {
                        _missionNameFade = 1;
                        _animState++;
                        _missionStartRide = 0;
                    }
                    break;
                case 3:
                    _missionStartRide += time.ElapsedGameTime.TotalSeconds;
                    if(_missionStartRide>=5)
                    {
                        _missionStartRide = 0;
                        _animState++;
                    }
                    break;
                case 4:
                    _missionStartFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_missionStartFade <= 0)
                    {
                        _missionStartFade = 0;
                        _animState++;
                    }
                    break;
                case 5:
                    _missionNameFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_missionNameFade <= 0)
                    {
                        _missionNameFade = 0;
                        _animState++;
                    }
                    break;
                case 6:
                    _shroudFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_shroudFade <= 0)
                    {
                        _shroudFade = 0;
                        _animState++;
                        _state = 1;
                        _current.OnObjectiveStart(_currentObjective, _objectives[_currentObjective]);
                    }
                    break;
                case 7:
                    _objectiveNameFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_objectiveNameFade >= 1)
                    {
                        _objectiveNameFade = 1;
                        _animState++;
                    }
                    break;
                case 8:
                    _objectiveDescFade += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_objectiveDescFade >= 1)
                    {
                        _objectiveDescFade = 1;
                        _animState++;
                        _objectiveRide = 0;
                    }
                    break;
                case 10:
                    _objectiveDescFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_objectiveDescFade <= 0)
                    {
                        _objectiveDescFade = 0;
                        _animState++;
                    }
                    break;
                case 11:
                    _objectiveNameFade -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_objectiveNameFade <= 0)
                    {
                        _objectiveNameFade = 0;
                        if (_currentObjective < _objectives.Length - 1)
                        {
                            _lastCheckpointSnapshotId = _save.CreateSnapshot();
                            _currentObjective++;
                            _current.OnObjectiveStart(_currentObjective, _objectives[_currentObjective]);
                            _animState = 7;
                        }
                        else
                        {
                            _save.SetValue("mission." + _current.Name.ToLower().Replace(" ", "_") + ".completed", true);
                            _current.OnComplete();
                            _current.OnEnd();
                            _objectives = null;
                            _currentObjective = 0;
                            _current = null;
                            _animState++;
                            NotifyOfNewMissions();
                        }
                    }
                    break;
                case 12:
                    _infobox.Show("Mission complete.", "You have completed the mission successfully. Check the 'Missions' program for new missions to play.");
                    _animState++;
                    break;

            }
        }

        [Dependency]
        private InfoboxManager _infobox = null;
    }

    

    /// <summary>
    /// Represents a Peacenet Campaign mission.
    /// </summary>
    public abstract class Mission
    {
        [Dependency]
        private SaveManager _save = null;

        public virtual void OnObjectiveStart(int index, Objective data)
        {

        }

        public virtual void OnObjectiveFail(int index, Objective data)
        {

        }

        public virtual void OnObjectiveComplete(int index, Objective data)
        {

        }


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
        /// A method to be run when the mission is started.
        /// </summary>
        public virtual void OnStart()
        {

        }

        /// <summary>
        /// A method to be run just after the mission has ended.
        /// </summary>
        public virtual void OnEnd()
        {

        }

        /// <summary>
        /// A method to be run when the mission is complete. You should only need to override this if you have special things to do on mission complete. The mission manager will mark your mission as complete in the save file automatically.
        /// </summary>
        public virtual void OnComplete()
        {

        }

        /// <summary>
        /// Retrieves the objectives assigned to this mission
        /// </summary>
        public abstract IEnumerable<Objective> ObjectiveList
        {
            get;
        }

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

    /// <summary>
    /// Represents an objective for a Peacenet mission.
    /// </summary>
    public class Objective
    {
        /// <summary>
        /// Retrieves the name of the objective.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Retrieves the description of the objective.
        /// </summary>
        public string Description { get; private set; }

        private Func<GameTime, ObjectiveState> _onUpdate = null;

        /// <summary>
        /// Creates a new instance of the <see cref="Objective"/> class. 
        /// </summary>
        /// <param name="name">The name of the objective.</param>
        /// <param name="desc">The description of the objective</param>
        /// <param name="onUpdate">A function to be run every frame to determine if the objective has been completed</param>
        public Objective(string name, string desc, Func<GameTime, ObjectiveState> onUpdate)
        {
            if (onUpdate == null)
                throw new ArgumentNullException(nameof(onUpdate));

            Name = name;
            Description = desc;
            _onUpdate = onUpdate;
        }

        public ObjectiveState Update(GameTime time)
        {
            return _onUpdate(time);
        }
    }

    /// <summary>
    /// Describes the state of an objective in a Peacenet mission.
    /// </summary>
    public enum ObjectiveState
    {
        /// <summary>
        /// The objective is currently in progress.
        /// </summary>
        Active,
        /// <summary>
        /// The objective has been complete.
        /// </summary>
        Complete,
        /// <summary>
        /// The objective has been failed.
        /// </summary>
        Failed
    }
}
