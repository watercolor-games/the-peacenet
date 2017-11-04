using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.GUI;

namespace Plex.Frontend.Apps
{
    [DefaultTitle("Mission complete.")]
    public class MissionComplete : Control, IPlexWindow
    {
        private MissionAttribute _mission = null;
        private TextControl _cptnobvious = null;
        private TextControl _mtitle = null;
        private TextControl _description = null;
        private TextControl _xpEarned = null;
        private Button _continue = null;


        public MissionComplete(MissionAttribute mission)
        {
            _mission = mission;
            Width = 800;
            Height = 600;
            _cptnobvious = new TextControl();
            _cptnobvious.AutoSize = true;
            _cptnobvious.Text = " - Mission complete - ";
            AddControl(_cptnobvious);

            _mtitle = new GUI.TextControl();
            _mtitle.AutoSize = true;
            AddControl(_mtitle);

            _description = new GUI.TextControl();
            _description.AutoSize = true;
            AddControl(_description);

            _xpEarned = new TextControl();
            _xpEarned.AutoSize = true;
            AddControl(_xpEarned);

            _continue = new Button();
            _continue.Text = "Continue";
            _continue.AutoSize = true;
            AddControl(_continue);
            _continue.Click += () =>
            {
                AppearanceManager.Close(this);
            };
        }

        public void OnLoad()
        {
            _mtitle.Text = _mission.Name;
            _description.Text = @"You have successfully completed this mission!

You may now continue to free roam and further explore the Plexgate.";
            _xpEarned.Text = $"{_mission.CodepointAward} XP earned.";
        }

        public void OnSkinLoad()
        {
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _cptnobvious.Y = 10;
            _cptnobvious.X = (Width - _cptnobvious.Width) / 2;

            _mtitle.Y = _cptnobvious.Y + _cptnobvious.Height + 10;
            _mtitle.X = (Width - _mtitle.Width) / 2;

            _description.Y = _mtitle.Y + _mtitle.Height + 20;
            _description.X = (Width - _description.Width) / 2;

            _continue.Y = Height - _continue.Height - 10;
            _continue.X = (Width - _continue.Width) / 2;

            _xpEarned.Y = _continue.Y - _xpEarned.Height - 15;
            _xpEarned.X = (Width - _xpEarned.Width) / 2;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
