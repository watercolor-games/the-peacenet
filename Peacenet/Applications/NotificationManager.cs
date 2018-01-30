using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Peacenet.Server;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a GUI for managing Peacegate OS notifications as well as displaying available and active <see cref="Mission"/>s. 
    /// </summary>
    public class NotificationManager : Window
    {
        [Dependency]
        private Storyboard _story = null;

        [Dependency]
        private OS _os = null;

        private TabPanel _tabs = new TabPanel();
        private TabPage _missions = new TabPage();
        private TabPage _notifications = new TabPage();

        private Label _missionsHead = new Label();
        private Label _missionsDesc = new Label();

        private Panel _missionsPanel = new Panel();

        private Mission _selectedMission = null;
        private int _missionUIState = 0;

        /// <inheritdoc/>
        public NotificationManager(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            Title = "Notifications";
            AddChild(_tabs);
            _tabs.AddChild(_notifications);
            _tabs.AddChild(_missions);
            _notifications.Name = "Notifications";
            _missions.Name = "Missions";

            _missions.AddChild(_missionsHead);
            _missions.AddChild(_missionsDesc);
            _missionsHead.AutoSize = true;
            _missionsDesc.AutoSize = true;
            _missionsHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _missionsDesc.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _missionsHead.Text = "Missions";
            _missionsDesc.Text = "Select a mission below to view more information and embark on it.";

            _missions.AddChild(_missionsPanel);
            _missionsPanel.AutoSize = true;

            _missionsObjectives.Layout = ListViewLayout.List;
            _missions.AddChild(_missionsObjectives);
            _missions.AddChild(_missionsBack);
            _missions.AddChild(_missionsStart);
            _missions.AddChild(_missionsObjectivesHead);
            _missionsObjectivesHead.Text = "Objectives";
            _missionsObjectivesHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _missionsObjectivesHead.AutoSize = true;
            _missionsBack.Text = "Back";
            _missionsStart.Text = "Start mission";
            _missionsBack.Click += (o, a) =>
            {
                _missionUIState = 0;
                _selectedMission = null;
                _needsReset = true;
            };

            _missionsStart.Click += (o, a) =>
            {
                if(_selectedMission != null)
                {
                    _story.StartMission(_selectedMission);
                    Close();
                }
            };
        }

        [Dependency]
        private AsyncServerManager _server = null;

        private bool _needsReset = true;

        private void ResetUI()
        {
            _missionsPanel.Clear();
            switch (_missionUIState)
            {
                case 0:
                    _missions.Name = "Missions";
                    _missionsPanel.Visible = true;
                    _missionsObjectives.Visible = false;
                    _missionsBack.Visible = false;
                    _missionsStart.Visible = false;
                    _missionsObjectivesHead.Visible = false;
                    if (_server.IsMultiplayer == false)
                    {
                        _missionsHead.Text = "Missions";
                        _missionsDesc.Text = "Select a mission from the list below to view more information and start it.";
                        foreach (var mission in _story.AvailableMissions)
                        {
                            var pnl = new NotificationItem();
                            pnl.Title = mission.Name;
                            pnl.Message = mission.Description;
                            _missionsPanel.AddChild(pnl);
                            pnl.Activated += () =>
                            {
                                _selectedMission = mission;
                                _missionUIState = 1;
                                _needsReset = true;
                            };
                        }
                    }
                    else
                    {
                        _missionsHead.Text = "Missions are not available in Multiplayer.";
                        _missionsDesc.Text = "Want to play through the Peacenet story? Try the 'Singleplayer' mode in the main menu.";
                    }
                    break;
                case 1:
                    _missions.Name = "Mission briefing";
                    _missionsBack.Visible = true;
                    _missionsStart.Visible = true;
                    _missionsPanel.Visible = false;
                    _missionsObjectives.Visible = true;
                    _missionsObjectivesHead.Visible = true;
                    bool noMissions = true;
                    if (_selectedMission != null)
                    {
                        _missions.Name = "Mission briefing";
                        _missionsHead.Text = "Mission briefing: " + _selectedMission.Name;
                        _missionsDesc.Text = _selectedMission.Description;
                        _missionsObjectives.Clear();
                        if(_selectedMission.Objectives != null)
                        {
                            foreach(var objective in _selectedMission.Objectives)
                            {
                                noMissions = false;
                                var objectiveItem = new ListViewItem(_missionsObjectives);
                                objectiveItem.Value = objective.Name;
                                objectiveItem.Tag = objective;
                            }
                        }
                        if (noMissions)
                        {
                            var nmitem = new ListViewItem(_missionsObjectives);
                            nmitem.Value = "No objectives - this mission will complete instantly and shouldn't be in the game because it's broken.";
                        }
                    }
                    break;
            }
        }

        private Label _missionsObjectivesHead = new Label();
        private Button _missionsBack = new Button();
        private Button _missionsStart = new Button();
        private ListView _missionsObjectives = new ListView();

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_needsReset)
            {
                ResetUI();
                _needsReset = false;
            }
            _tabs.X = 15;
            _tabs.Y = 15;
            _tabs.Width = Width - 30;
            _tabs.Height = Height - 30;

            _missionsHead.X = 15;
            _missionsDesc.X = 15;
            _missionsHead.Y = 15;
            _missionsHead.MaxWidth = _tabs.Width - 30;
            _missionsDesc.MaxWidth = _tabs.Width - 30;
            _missionsDesc.Y = _missionsHead.Y + _missionsHead.Height + 5;

            _missionsObjectivesHead.X = 15;
            _missionsObjectivesHead.Y = _missionsDesc.Y + _missionsDesc.Height + 15;
            _missionsObjectivesHead.MaxWidth = _missionsObjectives.Parent.Width - 30;

            _missionsObjectives.X = 15;
            _missionsObjectives.Y = _missionsObjectivesHead.Y + _missionsObjectivesHead.Height + 10;
            _missionsObjectives.Width = _missionsObjectivesHead.MaxWidth;

            _missionsPanel.X = 15;
            _missionsPanel.Y = _missionsDesc.Y + _missionsDesc.Height + 15;
            _missionsPanel.Width = _tabs.Width - 30;
            int y = 3;
            foreach (var ctrl in _missionsPanel.Children)
            {
                ctrl.X = 0;
                ctrl.Y = y;
                ctrl.Width = ctrl.Parent.Width;
                y += ctrl.Height + 4;
            }

            _missionsStart.Y = _missionsObjectives.Y + _missionsObjectives.Height + 10;
            _missionsBack.Y = _missionsStart.Y;
            _missionsStart.X = 15;
            _missionsBack.X = (_missionsBack.Parent.Width - _missionsBack.Width) - 15;
        }
    }

    /// <summary>
    /// A <see cref="Panel"/> which displays notification/mission summaries with a heading and description label. Also contains <see cref="Button"/>-like mouse behavior.  
    /// </summary>
    public class NotificationItem : Panel
    {
        private Label _header = new Label();
        private Label _desc = new Label();
        
        /// <summary>
        /// Creates a new instance of the <see cref="NotificationItem"/> control. 
        /// </summary>
        public NotificationItem()
        {
            AutoSize = false;
            AddChild(_header);
            AddChild(_desc);
            //forward clicks to us
            _header.Click += _header_Click;
            _desc.Click += _header_Click;
            Click += _header_Click;
        }

        /// <summary>
        /// Occurs when the notification panel is clicked. This event exists as a workaround of the fact that <see cref="Control.Click"/> cannot be invoked outside the context of <see cref="Control"/> and we must propagate <see cref="Control.Click"/> events from the child <see cref="Label"/>s in the panel to prevent you having to pull your hair out.    
        /// </summary>
        public event Action Activated;

        private void _header_Click(object sender, EventArgs e)
        {
            Activated?.Invoke();
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if(LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                base.OnPaint(time, gfx);
            }
            else if(ContainsMouse)
            {
                gfx.DrawRectangle(0, 0, Width, Height, Theme.GetAccentColor());
            }
            else
            {
                Theme.DrawControlBG(gfx, 0, 0, Width, Height);
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _desc.FontStyle = Plex.Engine.Themes.TextFontStyle.System;
            _desc.AutoSize = true;
            _header.AutoSize = true;
            _header.MaxWidth = Width - 6;
            _desc.MaxWidth = Width - 6;
            _header.X = 3;
            _header.Y = 3;
            _desc.X = 3;
            _desc.Y = _header.Y + _header.Height + 4;
            Height = _desc.Y + _desc.Height + 3;
        }

        /// <summary>
        /// Gets or sets the title text of this notification panel.
        /// </summary>
        public string Title
        {
            get
            {
                return _header.Text;
            }
            set
            {
                _header.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the description text of this notification label.
        /// </summary>
        public string Message
        {
            get
            {
                return _desc.Text;
            }
            set
            {
                _desc.Text = value;
            }
        }
    }
}
