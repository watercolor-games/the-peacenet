using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a user interface for starting missions.
    /// </summary>
    [AppLauncher("Missions", "Single-player")]
    public class MissionMenu : Window
    {
        private Label _missionsHead = new Label();
        private Label _missionsDesc = new Label();
        private ScrollView _missionsView = new ScrollView();

        private ListView _availableView = new ListView();

        private Panel _currentView = new Panel();

        private Label _currentHead = new Label();
        private Label _currentDesc = new Label();
        private Button _startMission = new Button();

        private int _state = 0;

        private Mission _selected = null;

        [Dependency]
        private MissionManager _missionManager = null;

        /// <summary>
        /// Creates a new instance of the <see cref="MissionMenu"/> window.
        /// </summary>
        /// <param name="_winsys">The window manager associated with this <see cref="Window"/>.</param>
        public MissionMenu(WindowSystem _winsys) : base(_winsys)
        {
            Width = 720;
            Height = 550;
            SetWindowStyle(WindowStyle.Dialog);
            Title = "Missions";
            if(_missionManager.IsPlayingMission)
            {
                _state = 2;
                _selected = _missionManager.CurrentMission;
            }
            else
            {
                if(_missionManager.Missions.Length == 1)
                {
                    _state = 1;
                    _selected = _missionManager.Missions[0];
                }
                else
                {
                    _state = 0;
                }
            }

            AddChild(_missionsHead);
            AddChild(_missionsDesc);
            AddChild(_missionsView);

            _currentView.AddChild(_currentHead);
            _currentView.AddChild(_currentDesc);
            _currentView.AddChild(_startMission);

            foreach(var mission in _missionManager.Missions)
            {
                var lvitem = new ListViewItem(_availableView);
                lvitem.Value = mission.Name;
                lvitem.Tag = mission;
            }

            _availableView.SelectedIndexChanged += (o, a) =>
            {
                if(_availableView.SelectedItem != null)
                {
                    _state = 1;
                    _selected = _availableView.SelectedItem.Tag as Mission;
                }
            };

            _missionsHead.AutoSize = true;
            _missionsHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _missionsDesc.AutoSize = true;
            _currentView.AutoSize = true;
            _currentHead.AutoSize = true;
            _currentHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _currentDesc.AutoSize = true;

            _startMission.Click += (o, a) =>
            {
                _missionManager.StartMission(_selected);
                Close();
            };

            ResetUI();
        }

        /// <summary>
        /// Reset the user interface for this app.
        /// </summary>
        public void ResetUI()
        {
            _missionsView.Clear();
            switch(_state)
            {
                case 0:
                    _missionsView.AddChild(_availableView);
                    break;
                case 1:
                    _missionsView.AddChild(_currentView);
                    _currentHead.Text = _selected.Name;
                    _currentDesc.Text = _selected.Description;
                    _startMission.Text = "Start mission";
                    _startMission.Enabled = true;
                    break;
                case 2:
                    _missionsView.AddChild(_currentView);
                    _currentHead.Text = _selected.Name;
                    _currentDesc.Text = _selected.Description;
                    _startMission.Text = "Mission in progress";
                    _startMission.Enabled = false;
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _missionsHead.X = 15;
            _missionsHead.Y = 15;
            _missionsHead.Text = "Missions";
            _missionsHead.MaxWidth = Width - 30;
            _missionsDesc.Text = "Missions are groups of objectives that you can complete to unravel the story of The Peacenet and gain more resources.";
            _missionsDesc.MaxWidth = _missionsHead.MaxWidth;
            _missionsDesc.X = 15;
            _missionsDesc.Y = _missionsHead.Y + _missionsHead.Height + 5;

            _missionsView.X = 0;
            _missionsView.Y = _missionsDesc.Y + _missionsDesc.Height + 15;
            _missionsView.Width = Width;
            _missionsView.Height = Height - _missionsView.Y;

            switch (_state)
            {
                case 0:
                    _availableView.X = 15;
                    _availableView.Y = 15;
                    _availableView.Width = Width - 30;
                    break;
                case 1:
                case 2:
                    _currentView.Width = Width;
                    _currentHead.X = 30;
                    _currentHead.Y = 30;
                    _currentHead.MaxWidth = Width - 60;
                    _currentDesc.X = 30;
                    _currentDesc.Y = _currentHead.Y + _currentHead.Height + 5;
                    _currentDesc.MaxWidth = _currentHead.MaxWidth;

                    _startMission.X = (Width - 30) - _startMission.Width;
                    _startMission.Y = _currentDesc.Y + _currentDesc.Height + 15; 
                    break;
            }
            base.OnUpdate(time);
        }
    }
}
