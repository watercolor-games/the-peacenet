using Microsoft.Xna.Framework;
using Peacenet.DesktopUI;
using Peacenet.Email;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("Email Viewer", "Networking", "View, manage, send and reply to Peacenet emails.")]
    public class EmailViewer : Window
    {
        private Stacker _inboxStacker = new Stacker();
        private ScrollView _view = new ScrollView();
        private int _state = 0;

        private Label _header = new Label();
        private EmailThread _currentThread = null;

        [Dependency]
        private MissionManager _mission = null;

        [Dependency]
        private GameManager _game = null;

        public EmailViewer(WindowSystem _winsys) : base(_winsys)
        {
            Width = 615;
            Height = 375;
            Title = "Email Viewer";

            AddChild(_header);
            AddChild(_view);

            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _header.AutoSize = true;

            _inboxStacker.AutoSize = true;

            SetupUI();
        }

        private void SetupUI()
        {
            _view.Clear();
            switch (_state)
            {
                case 0:
                    _header.Text = "Inbox";
                    _view.AddChild(_inboxStacker);
                    _inboxStacker.Clear();
                    var threads = _game.State.Emails.Where(x => _game.State.GetMessages(x.Id).FirstOrDefault(y => y.To == "{you}") != null).OrderBy(x => x.Sent);
                    var head = new EmailSubjectDisplay();
                    head.Subject = "Subject";
                    head.Time = "Last message time";
                    head.FromTo = "Author(s)";
                    head.IsUnread = false;
                    head.Enabled = false;
                    _inboxStacker.AddChild(head);
                    if(threads.Count()==0)
                    {
                        var noMessages = new EmailSubjectDisplay();
                        noMessages.Subject = "No messages to display.";
                        noMessages.Enabled = false;
                        noMessages.IsUnread = false;
                        _inboxStacker.AddChild(noMessages);

                    }
                    foreach(var thread in threads)
                    {
                        var display = new EmailSubjectDisplay();
                        string from = "";
                        foreach(var message in _game.State.GetMessages(thread.Id))
                        {
                            string toName = (message.To == "{you}") ? "me" : message.To.Substring(0, message.To.LastIndexOf("@"));
                            string fromName = (message.From == "{you}") ? "me" : message.From.Substring(0, message.From.LastIndexOf("@"));

                            if(!from.Contains(fromName))
                            {
                                if (from.Length > 0)
                                    from += ", ";
                                from += fromName;
                            }
                            if (!from.Contains(toName))
                            {
                                if (from.Length > 0)
                                    from += ", ";
                                from += toName;
                            }

                        }
                        display.FromTo = from;
                        display.Subject = thread.Subject;
                        var time = _game.State.GetMessages(thread.Id).OrderByDescending(x => x.Sent).First().Sent;
                        display.Time = time.ToShortDateString() + " " + time.ToShortTimeString();
                        display.IsUnread = _game.State.GetMessages(thread.Id).Where(x => x.IsUnread).Count() > 0;
                        display.Click += (o, a) =>
                        {
                            _currentThread = thread;
                            _state = 1;
                            SetupUI();
                        };
                        _inboxStacker.AddChild(display);
                    }
                    break;
                case 1:
                    _header.Text = _currentThread.Subject;
                    _view.AddChild(_inboxStacker);
                    _inboxStacker.Clear();
                    foreach (var message in _game.State.GetMessages(_currentThread.Id).OrderBy(x=>x.Sent))
                    {
                        if (message.IsUnread)
                            _game.State.MarkRead(message.Id);
                        var display = new EmailMessageDisplay();
                        display.Header = $"From {message.From} to {message.To} at {message.Sent.ToShortDateString()} {message.Sent.ToShortTimeString()}".Replace("{you}", "me");
                        display.Message = message.Message;
                        display.MissionID = message.MissionID;
                        display.MissionStart += () =>
                          {
                              var m = _mission.Available.FirstOrDefault(x => x.ID == display.MissionID);
                              if(m!=null)
                              {
                                  Close();
                                  m.Start();
                              }
                          };
                        _inboxStacker.AddChild(display);
                    }
                    break;
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            _header.X = 15;
            _header.Y = 15;
            _header.MaxWidth = Width - 30;

            _view.X = 0;
            _view.Y = _header.Y + _header.Height + 7;
            _view.Width = Width;
            _view.Height = Height - _view.Y;

            _inboxStacker.Width = Width;
            base.OnUpdate(time);
        }
    }

    public class EmailSubjectDisplay : Control
    {
        public string Subject { get; set; }
        public string FromTo { get; set; }
        public string Time { get; set; }
        public bool IsUnread { get; set; }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if (Parent == null)
                return;
            var font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);

            int textPad = 7;

            int thirdWidth = (Width / 3) - (textPad * 2);

            var subjectMeasure = TextRenderer.MeasureText(Subject, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var timeMeasure = TextRenderer.MeasureText(Time, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var fromMeasure = TextRenderer.MeasureText(FromTo, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);

            if(ContainsMouse)
            {
                gfx.FillRectangle(0, 0, Width, Height, Theme.GetAccentColor());
            }
            else if(IsUnread)
            {
                Theme.DrawControlLightBG(gfx, 0, 0, Width, Height);
            }

            gfx.DrawString(FromTo, new Vector2(textPad, (Height - fromMeasure.Y) / 2), Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), font, TextAlignment.Left, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            gfx.DrawString(Subject, new Vector2((Width - thirdWidth) / 2, (Height - subjectMeasure.Y) / 2), Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), font, TextAlignment.Left, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            gfx.DrawString(Time, new Vector2((Width - textPad)-thirdWidth, (Height - timeMeasure.Y) / 2), Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), font, TextAlignment.Left, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
                return;
            var font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);

            int textPad = 7;

            int thirdWidth = (Width / 3) - (textPad * 2);

            var subjectMeasure = TextRenderer.MeasureText(Subject, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var timeMeasure = TextRenderer.MeasureText(Time, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var fromMeasure = TextRenderer.MeasureText(FromTo, font, thirdWidth, Plex.Engine.TextRenderers.WrapMode.Words);

            Height = (textPad * 2) + (int)Math.Max(Math.Max(subjectMeasure.Y, timeMeasure.Y), fromMeasure.Y);
            Width = Parent.Width;

            base.OnUpdate(time);
        }

    }

    public class EmailMessageDisplay : Control
    {
        private Label _head = new Label();
        private Label _message = new Label();
        private Button _missionButton = new Button();
        private string _missionID = null;

        [Dependency]
        private MissionManager _mission = null;

        public EmailMessageDisplay()
        {
            AddChild(_head);
            AddChild(_message);
            AddChild(_missionButton);
            _missionButton.Click += (o, a) =>
            {
                MissionStart?.Invoke();
            };
        }

        public event Action MissionStart;

        public string MissionID
        {
            get
            {
                return _missionID;
            }
            set
            {
                _missionID = value;
            }
        }

        public string Header
        {
            get
            {
                return _head.Text;
            }
            set
            {
                _head.Text = value;
            }
        }

        public string Message
        {
            get
            {
                return _message.Text;
            }
            set
            {
                _message.Text = value;
            }
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if (Parent == null)
                return;
            Theme.DrawControlLightBG(gfx, 15, 15, Width - 30, Height - 30);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
                return;

            bool hasMission = !string.IsNullOrWhiteSpace(_missionID) && !_mission.IsPlayingMission;
            if(hasMission)
            {
                var m = _mission.Available.FirstOrDefault(x => x.ID == _missionID);
                if (m == null)
                    hasMission = false;
                else
                {
                    _missionButton.Text = $"Start \"{m.Name}\"";
                }
            }

            _missionButton.Visible = hasMission;

            _missionButton.X = (Width - 45) - _missionButton.Width;
            

            Width = Parent.Width;

            int labelWidth = Width - 60;

            _head.MaxWidth = labelWidth;
            _message.MaxWidth = labelWidth;

            _head.AutoSize = true;
            _message.AutoSize = true;
            _head.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _head.X = 30;
            _head.Y = 30;
            _message.X = 30;
            _message.Y = _head.Y + _head.Height + 7;

            _missionButton.Y = _message.Y + _message.Height + 10;

            if (_missionButton.Visible)
            {
                Height = _missionButton.Y + _missionButton.Height + 45;
            }
            else
            { 
                Height = _message.Y + _message.Height + 30;
            }
            base.OnUpdate(time);
        }
    }
}
