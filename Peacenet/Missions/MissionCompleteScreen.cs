using Microsoft.Xna.Framework;
using Peacenet.DesktopUI;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Missions
{
    public class MissionCompleteScreen : Window
    {
        private MissionData _data;
        private Label _missionComplete = new Label();
        private Label _missionName = new Label();
        private Stacker _objectiveMedals = new Stacker();
        private MedalDisplay _averageMedal = new MedalDisplay();
        private int _currentObjective = 0;
        private Button _next = new Button();
        private int _state = 0;

        public MissionCompleteScreen(MissionData data, WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
            Title = "Mission complete.";
            _data = data;
            AddChild(_missionComplete);
            AddChild(_missionName);
            AddChild(_objectiveMedals);
            AddChild(_averageMedal);
            AddChild(_next);
        }
    }

    public class ObjectiveMedalDisplay : Control
    {
        private Label _objectiveName = new Label();
        private MedalDisplay _medal = new MedalDisplay();
        private Label _desc = new Label();
        private bool _opaque = false;

        public bool Show
        {
            get
            {
                return _opaque;
            }
            set
            {
                _opaque = value;
                if(value)
                {
                    _desc.Opacity = 0;
                    _objectiveName.Opacity = 0;
                    _medal.Opacity = 0;
                }
            }
        }

        public bool FullyVisible
        {
            get
            {
                return _desc.Opacity == 1;
            }
        }

        private ObjectiveMedal _medalData;


        public ObjectiveMedal Medal
        {
            get
            {
                return _medalData;
            }
            set
            {
                _medalData = value;
            }
        }

        public ObjectiveMedalDisplay()
        {
            AddChild(_objectiveName);
            AddChild(_medal);
            AddChild(_desc);

            _desc.AutoSize = true;
            _objectiveName.AutoSize = true;

            _desc.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            _objectiveName.Opacity = 0;
            _medal.Opacity = 0;
            _desc.Opacity = 0;
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
                return;

            Width = Parent.Width;

            _medal.X = Width - _medal.Width - 10;
            _objectiveName.X = 10;
            _objectiveName.MaxWidth = (_medal.X - 20);
            _objectiveName.Text = _medalData.ObjectiveName;
            _medal.Medal = _medalData.Medal;
            _desc.Text = _medalData.MedalDescription;
            _desc.X = _objectiveName.X;
            _desc.MaxWidth = _objectiveName.MaxWidth;
            int textHeight = _desc.Height + 3 + _objectiveName.Height;
            if(Math.Max(textHeight, _medal.Height) == textHeight)
            {
                _objectiveName.Y = 7;
                _medal.Y = _objectiveName.Y + ((textHeight - _medal.Height) / 2);
                _desc.Y = _objectiveName.Y + 3 + _objectiveName.Height;
            }
            else
            {
                _medal.Y = 7;
                _objectiveName.Y = _medal.Y + ((_medal.Height - textHeight) / 2);
                _desc.Y = _objectiveName.Y + _objectiveName.Height + 3;
            }

            if(_objectiveName.Opacity<1)
            {
                _objectiveName.Opacity = MathHelper.Clamp(_objectiveName.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
            }
            else
            {
                if (_medal.Opacity < 1)
                {
                    _medal.Opacity = MathHelper.Clamp(_medal.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                }
                else
                {
                    if (_desc.Opacity < 1)
                    {
                        _desc.Opacity = MathHelper.Clamp(_desc.Opacity + (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    }
                }
            }

            base.OnUpdate(time);
        }
    }

    public class MedalDisplay : Control
    {
        private Medal _medal = Medal.Bronze;

        private Label _head = new Label();
        private Label _xp = new Label();

        private const int _medalCircleRadius = 16;
        private const int _padH = 7;
        private const int _padV = 5;
        private const int _medalSeparator = 4;
        private const int _textSeparator = 3;

        public Medal Medal
        {
            get
            {
                return _medal;
            }
            set
            {
                _medal = value;
            }
        }

        public MedalDisplay()
        {
            AddChild(_head);
            AddChild(_xp);
            _head.AutoSize = true;
            _xp.AutoSize = true;
            _xp.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
        }

        protected override void OnUpdate(GameTime time)
        {
            _head.Text = _medal.ToString();
            _xp.Text = $"{(int)_medal} XP";

            int h = Math.Max(_medalCircleRadius * 2, _head.Height + _textSeparator + _xp.Height);
            int w = (_medalCircleRadius * 2) + _medalSeparator + Math.Max(_head.Width, _xp.Width);
            Width = (_padH * 2) + w;
            Height = (_padV * 2) + h;
            _head.Y = _padV;
            _xp.X = _head.Y + _head.Height + _textSeparator;
            _head.X = _padH + (_medalCircleRadius * 2) + _medalSeparator;
            _xp.X = _head.X;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlLightBG(gfx, 0, 0, Width, Height);
            var medalColor = Color.Brown;
            switch(_medal)
            {
                case Medal.Silver:
                    medalColor = Color.Silver;
                    break;
                case Medal.Gold:
                    medalColor = Color.Gold;
                    break;
            }
            gfx.DrawCircle(new Vector2(_padH + _medalCircleRadius, _padV + _medalCircleRadius), _medalCircleRadius, medalColor);
        }


    }

    public class MissionData
    {
        public string Name { get; set; }
        public ObjectiveMedal[] ObjectiveMedals { get; set; }
        public ItemUnlock[] Unlocks { get; set; }
    }

    public struct ItemUnlock
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string IconTexturePath { get; private set; }
        public string SaveBoolID { get; set; }

        public ItemUnlock(string name, string desc, string icon, string saveid)
        {
            Name = name;
            Description = desc;
            IconTexturePath = icon;
            SaveBoolID = saveid;
        }
    }

    public struct ObjectiveMedal
    {
        public string ObjectiveName { get; private set; }
        public string MedalDescription { get; private set; }
        public Medal Medal { get; private set; }

        public ObjectiveMedal(string name, string desc, Medal medal)
        {
            ObjectiveName = name;
            MedalDescription = desc;
            Medal = medal;
        }


    }

    public enum Medal
    {
        Bronze = 10,
        Silver = 25,
        Gold = 50
    }
}
