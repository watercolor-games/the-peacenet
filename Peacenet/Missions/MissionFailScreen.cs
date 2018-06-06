using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Missions
{
    public class MissionFailScreen : Window
    {
        [Dependency]
        private SaveManager _save = null;

        private Mission _mission = null;
        private string _prevSnap = null;
        private string _abandonSnap = null;

        private float _fade = 0f;
        private float _uiFade = 0f;

        private Label _header = new Label();
        private Label _description = new Label();

        private Button _abandon = new Button();
        private Button _retryMission = new Button();
        private Button _retryObjective = new Button();

        private bool _restartingMission = false;
        private bool _closing = false;

        private Rectangle _uiRect = Rectangle.Empty;

        public MissionFailScreen(Mission mission, string failDescription, string abandonSnapshot, string previousObjectiveSnapshot, WindowSystem _winsys) : base(_winsys)
        {
            AddChild(_header);
            AddChild(_description);
            AddChild(_abandon);
            AddChild(_retryMission);
            AddChild(_retryObjective);

            _prevSnap = previousObjectiveSnapshot;
            _abandonSnap = abandonSnapshot;
            _mission = mission;

            _header.Text = "Mission failed...";
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _header.AutoSize = true;

            _description.Text = failDescription;
            _description.AutoSize = true;

            _abandon.Text = "Abandon mission";
            _retryMission.Text = "Retry mission";
            _retryObjective.Text = "Retry objective";

            _header.Alignment = Plex.Engine.TextAlignment.Center;
            _description.Alignment = Plex.Engine.TextAlignment.Center;

            SetWindowStyle(WindowStyle.NoBorder);
            Title = "Mission failed.";
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black * _fade);
            Theme.DrawControlBG(gfx, _uiRect.X, _uiRect.Y, _uiRect.Width, _uiRect.Height);
        }

        protected override void OnUpdate(GameTime time)
        {
            Parent.X = 0;
            Parent.Y = 0;
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            int halfWidth = Width / 2;

            _header.MaxWidth = halfWidth;
            _description.MaxWidth = halfWidth;

            _header.X = (Width - _header.Width) / 2;
            _description.X = (Width - _description.Width) / 2;
            _retryMission.X = (Width - _retryMission.Width) / 2;
            _retryObjective.X = (_retryMission.X - _retryObjective.Width) - 3;
            _abandon.X = _retryMission.X + _retryMission.Width + 3;

            float uiFade = (_uiFade <= 0.5F) ? MathHelper.Clamp(_uiFade * 2, 0, 1) : MathHelper.Clamp(1-((_uiFade * 2) - 1), 0, 1);
            int descriptionYBase = (Height - _description.Height) / 2;
            int uiTenth = Height / 10;
            _description.Y = (int)MathHelper.Lerp(descriptionYBase + uiTenth, descriptionYBase + uiTenth, _uiFade);
            _description.Opacity = uiFade;
            _header.Y = (_description.Y - _header.Height) - 7;
            _header.Opacity = uiFade;
            _retryMission.Y = _description.Y + _description.Height + 10;
            _retryMission.Opacity = uiFade;
            _abandon.Y = _retryMission.Y;
            _abandon.Opacity = uiFade;
            _retryObjective.Y = _retryMission.Y;
            _retryObjective.Opacity = uiFade;

            if (_closing == false)
            {
                if (_fade < 1)
                {
                    _fade = MathHelper.Clamp(_fade + ((float)time.ElapsedGameTime.TotalSeconds / 1), 0, 1);
                }
                else
                {
                    if(_uiFade < 0.5F)
                    {
                        _uiFade = MathHelper.Clamp(_uiFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 1);
                    }
                }
            }
            else
            {
                if (_uiFade < 1F)
                {
                    _uiFade = MathHelper.Clamp(_uiFade + ((float)time.ElapsedGameTime.TotalSeconds), 0, 1);
                }
                else
                {
                    if (_fade >0)
                    {
                        _fade = MathHelper.Clamp(_fade - ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
                    }
                    else
                    {
                        if(_restartingMission)
                        {
                            _mission.Start();
                        }
                        Close();
                    }

                }

            }
        }
    }
}
