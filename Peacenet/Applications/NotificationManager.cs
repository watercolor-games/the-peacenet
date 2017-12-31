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
            for(int i = 0; i < 10; i++)
            {
                var head = new Label();
                head.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
                head.Text = $"Dummy text {i + 1}";
                head.AutoSize = true;
                var desc = new Label();
                desc.FontStyle = Plex.Engine.Themes.TextFontStyle.System;
                desc.AutoSize = true;
                desc.Text = $"This is a dummy description for a dummy mission. This is dummy description {i + 1}.";
                _missionsPanel.AddChild(head);
                _missionsPanel.AddChild(desc);
            }
        }

        protected override void OnUpdate(GameTime time)
        {
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

            _missionsPanel.X = 15;
            _missionsPanel.Y = _missionsDesc.Y + _missionsDesc.Height + 15;
            _missionsPanel.Width = _tabs.Width - 30;
            int y = 3;
            foreach (var ctrl in _missionsPanel.Children)
            {
                ctrl.X = 3;
                ctrl.Y = y;
                ctrl.MaxWidth = ctrl.Parent.Width - 6;
                y += ctrl.Height + 4;
            }
        }
    }
}
