using Microsoft.Xna.Framework;
using Peacenet.DesktopUI;
using Plex.Engine;
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
        private Panel _view = new Panel();
        private ScrollView _objectiveListView = new ScrollView();
        private Label _objectivesHead = new Label();
        private Label _avgMedalHead = new Label();
        private MedalDisplay _averageMedal = new MedalDisplay();
        private Button _next = new Button();
        private int _state = 0;
        private float _bgFade = 0;
        private float _headerFade = 0f;
        private float _viewWidth = 0f;
        private float _viewHeight = 0f;
        private Label _totalXP = new Label();
        private int _total = 0;
        private float _totalXPPercent = 0;
        private XPDisplay _xpDisplay = new XPDisplay();

        private Label _newLevel = new Label();

        private bool _hasNewLevel = false;

        [Dependency]
        private GameManager _game = null;

        public MissionCompleteScreen(MissionData data, WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
            Title = "Mission complete.";
            _data = data;
            AddChild(_missionComplete);
            AddChild(_missionName);
            AddChild(_view);
            _view.AutoSize = false;
            _view.AddChild(_objectivesHead);
            _view.AddChild(_objectiveListView);
            _objectiveListView.AddChild(_objectiveMedals);
            _view.AddChild(_avgMedalHead);
            _view.AddChild(_averageMedal);
            _view.AddChild(_xpDisplay);
            _view.AddChild(_newLevel);

            foreach(var m in _data.ObjectiveMedals)
            {
                var d = new ObjectiveMedalDisplay();
                d.Medal = m;
                d.Show = false;
                _objectiveMedals.AddChild(d);
            }

            _newLevel.AutoSize = true;
            _newLevel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _newLevel.Text = "New skill level!";

            _averageMedal.Medal = GetAverageMedal();
            _avgMedalHead.Text = "Mission medal:";
            _avgMedalHead.AutoSize = true;
            _avgMedalHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _objectivesHead.Text = "Objectives";
            _objectivesHead.AutoSize = true;
            _objectivesHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _view.AddChild(_averageMedal);
            AddChild(_next);

            _missionComplete.Text = "Mission complete";
            _missionComplete.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _missionComplete.AutoSize = true;
            _missionName.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _missionName.AutoSize = true;
            _totalXP.AutoSize = true;
            _totalXP.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _view.AddChild(_totalXP);
            _view.DrawBackground = true;

            int level = _game.State.SkillLevel;
            _total = GetTotal();
            _game.State.AddXP(_total);
            _hasNewLevel = _game.State.SkillLevel != level;
            _next.Text = "Continue";
            
            _avgMedalHead.Opacity = 0;
            _totalXP.Opacity = 0;
            _xpDisplay.Opacity = 0;
            _newLevel.Opacity = 0;
        }

        private int GetTotal()
        {
            int xp = 0;
            foreach (var medal in _data.ObjectiveMedals)
                xp += (int)medal.Medal;
            xp += (int)GetAverageMedal();
            return xp;
        }

        private Medal GetAverageMedal()
        {
            int medal = 0;
            int count = _data.ObjectiveMedals.Length;
            foreach (var m in _data.ObjectiveMedals)
                medal += (int)m.Medal;
            return (Medal)(medal/count);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black * (_bgFade * 0.75F));
            
        }

        protected override void OnUpdate(GameTime time)
        {
            Parent.X = 0;
            Parent.Y = 0;
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;

            _missionName.Text = _data.Name;
            _missionName.MaxWidth = Width / 2;
            _missionComplete.MaxWidth = Width / 2;

            switch(_state)
            {
                case 0:
                    _next.Enabled = false;
                    _bgFade = MathHelper.Clamp(_bgFade + ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                    if(_bgFade==1)
                    {
                        _state++;
                    }
                    break;
                case 1:
                    _headerFade = MathHelper.Clamp(_headerFade + (float)time.ElapsedGameTime.TotalSeconds, 0, 0.5f);
                    if(_headerFade>=0.5F)
                    {
                        _state++;
                    }
                    break;
                case 2:
                    _viewWidth = MathHelper.Clamp(_viewWidth + ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_viewWidth >= 1)
                        _state=4;
                    break;
                case 4:
                    _viewHeight = MathHelper.Clamp(_viewHeight + ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_viewHeight >= 1)
                        _state++;
                    break;
                case 5:
                    _avgMedalHead.Opacity = MathHelper.Clamp(_avgMedalHead.Opacity + ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_avgMedalHead.Opacity >= 1)
                    {
                        foreach(var m in _objectiveMedals.Children)
                        {
                            if (m is ObjectiveMedalDisplay)
                                (m as ObjectiveMedalDisplay).Show = true;
                        }
                        _state++;
                    }
                    break;
                case 6:
                    _totalXP.Opacity = MathHelper.Clamp(_totalXP.Opacity + ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_totalXP.Opacity >= 1)
                    {
                        _state++;
                    }
                    break;
                case 7:
                    _totalXPPercent = MathHelper.Clamp((float)_totalXPPercent + ((float)time.ElapsedGameTime.TotalSeconds / 2.5f), 0, 1);
                    if(_totalXPPercent==1)
                    {
                        _state = 14;
                        _next.Enabled = true;
                    }
                    break;
                case 9:
                    _viewHeight = MathHelper.Clamp(_viewHeight - ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_viewHeight <= 0)
                        _state++;
                    break;
                case 10:
                    _viewWidth = MathHelper.Clamp(_viewWidth - ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                    if (_viewWidth <= 0)
                        _state++;
                    break;
                case 11:
                    _headerFade = MathHelper.Clamp(_headerFade + (float)time.ElapsedGameTime.TotalSeconds, 0, 1f);
                    if (_headerFade >= 1F)
                    {
                        _state++;
                    }
                    break;
                case 12:
                    _bgFade = MathHelper.Clamp(_bgFade - ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                    if (_bgFade >0)
                    {
                        _state++;
                        _game.State.CompleteMission(_data.ID);
                        Close();
                    }
                    break;
                case 14:
                    _xpDisplay.Opacity = MathHelper.Clamp(_xpDisplay.Opacity + ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                    if(_xpDisplay.Opacity>=1)
                    {
                        _state++;
                    }
                    break;
                case 15:
                    if(_hasNewLevel)
                    {
                        _newLevel.Opacity = MathHelper.Clamp(_newLevel.Opacity + ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                        if (_newLevel.Opacity >= 1)
                        {
                            _state++;
                        }
                    }
                    else
                    {
                        _state++;
                    }
                    break;
                case 16:
                    _xpDisplay.TotalXP = _game.State.TotalXP;
                    _xpDisplay.SkillLevel = _game.State.SkillLevel;
                    _xpDisplay.SkillLevelPercentage = _game.State.SkillLevelPercentage;
                    _xpDisplay.ApplyAnim();
                    _state = 8;
                    break;
                    
            }

            _view.Opacity = _viewWidth;
            _view.Width = (int)MathHelper.Lerp(0, Width, _viewWidth);
            _view.Height = (int)MathHelper.Lerp(2, Height / 2, _viewHeight);
            _view.X = (Width - _view.Width) / 2;
            _view.Y = (Height - _view.Height) / 2;
            _next.Opacity = _viewHeight;
            _next.Y = _view.Y + _view.Height + 15;
            _next.X = (Width - _next.Width) / 2;

            _objectivesHead.Opacity = _viewHeight;
            _objectiveListView.Opacity = _viewHeight;
            _averageMedal.Opacity = _avgMedalHead.Opacity;
            float headOpacity = (_headerFade <= 0.5F) ? (_headerFade * 2) : 2 - (_headerFade * 2);

            int missionSeparator = 4;
            int missionHeight = _missionComplete.Height + missionSeparator + _missionName.Height;
            int missionBase = _view.Y - 7 - (missionHeight);
            int screenTenth = Height / 10;
            int missionY = (int)MathHelper.Lerp(missionBase + screenTenth, missionBase - screenTenth, _headerFade);

            float mOpacity = (_headerFade <= 0.5F) ? _headerFade * 2 : 2 - (_headerFade * 2);

            _missionComplete.Opacity = mOpacity;
            _missionName.Opacity = mOpacity;

            _missionComplete.Y = missionY;
            _missionName.Y = _missionComplete.Y + _missionComplete.Height + missionSeparator;
            _missionComplete.X = (Width - _missionComplete.Width) / 2;
            _missionName.X = (Width - _missionName.Width) / 2;

            int quarterWidth = Width / 4;
            _objectiveListView.Width = quarterWidth;
            _objectiveMedals.Width = _objectiveListView.Width;
            _objectiveMedals.AutoSize = true;
            int halfSeparator = 45;
            int total = (quarterWidth * 2) + halfSeparator;

            _objectiveListView.X = (Width - total) / 2;
            _objectivesHead.X = _objectiveListView.X;
            _objectivesHead.Y = 30;
            _objectivesHead.MaxWidth = quarterWidth;
            _objectiveListView.Y = _objectivesHead.Y + _objectivesHead.Height + 15;
            _objectiveListView.Height = (_view.Height - _objectiveListView.Y) - 30;

            _avgMedalHead.X = _objectiveListView.X + quarterWidth + halfSeparator;
            _avgMedalHead.Y = _objectivesHead.Y;
            _avgMedalHead.MaxWidth = quarterWidth;
            _averageMedal.X = _avgMedalHead.X;
            _averageMedal.Y = _avgMedalHead.Y + _avgMedalHead.Height + 15;

            _totalXP.MaxWidth = quarterWidth;
            _totalXP.X = _avgMedalHead.X;
            
            int xp = (int)Math.Round((double)_total * _totalXPPercent);
            _totalXP.Text = $"Total XP Earned: {xp}";

            _newLevel.Y = (_objectiveListView.Y + _objectiveListView.Height) - _newLevel.Height;

            int skillYLow = (_newLevel.Y + _newLevel.Height) - _xpDisplay.Height;
            int skillYHigh = (_newLevel.Y - _xpDisplay.Height) - 7;
            _xpDisplay.Y = (int)MathHelper.Lerp(skillYLow, skillYHigh, _newLevel.Opacity);

            int xpYLow = (_xpDisplay.Y + _xpDisplay.Height) - _totalXP.Height;
            int xpYHigh = (_xpDisplay.Y - _totalXP.Height) - 14;
            _totalXP.Y = (int)MathHelper.Lerp(xpYLow, xpYHigh, _xpDisplay.Opacity);

            _xpDisplay.Width = _totalXP.MaxWidth;
            _newLevel.MaxWidth = _totalXP.MaxWidth;

            _newLevel.X = _totalXP.X;
            _xpDisplay.X = _totalXP.X;
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
                Height = textHeight + 14;
            }
            else
            {
                _medal.Y = 7;
                _objectiveName.Y = _medal.Y + ((_medal.Height - textHeight) / 2);
                _desc.Y = _objectiveName.Y + _objectiveName.Height + 3;
                Height = _medal.Height + 14;
            }

            if (Show)
            {
                if (_objectiveName.Opacity < 1)
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
            }
            else
            {
                if (_objectiveName.Opacity > 0)
                {
                    _objectiveName.Opacity = MathHelper.Clamp(_objectiveName.Opacity - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                }
                else
                {
                    if (_medal.Opacity > 0)
                    {
                        _medal.Opacity = MathHelper.Clamp(_medal.Opacity - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                    }
                    else
                    {
                        if (_desc.Opacity > 0)
                        {
                            _desc.Opacity = MathHelper.Clamp(_desc.Opacity - (float)time.ElapsedGameTime.TotalSeconds * 2, 0, 1);
                        }
                    }
                }

            }

            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
        }
    }

    public class MedalDisplay : Control
    {
        private Medal _medal = Medal.Bronze;

        private Label _head = new Label();
        private Label _xp = new Label();

        private const int _medalCircleRadius = 8;
        private const int _padH = 4;
        private const int _padV = 2;
        private const int _medalSeparator = 4;
        private const int _textSeparator = 0;

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
            _xp.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
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
            _xp.Y = _head.Y + _head.Height + _textSeparator;
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
            gfx.FillCircle(new Vector2(_padH + _medalCircleRadius, (Height/2)), _medalCircleRadius, medalColor);
        }


    }

    public class MissionData
    {
        public string ID { get; set; }
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
