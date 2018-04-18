using Microsoft.Xna.Framework;
using Peacenet.Email;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Peacenet.DesktopUI;

namespace Peacenet.Applications
{
    [AppLauncher("Email Viewer", "Networking", "View your inbox, send emails and reply to incoming emails.")]
    public class EmailViewer : Window
    {
        [Dependency]
        private EmailClient _email = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        private ScrollView _sidebar = new ScrollView();
        private ListView _sidebarView = new ListView();

        private Label _header = new Label();

        private ScrollView _body = new ScrollView();
        private Panel _bodyView = new Panel();

        private Button _compose = new Button();

        private ListView _inbox = new ListView();
        private ListView _outbox = new ListView();
        private Stacker _thread = new Stacker();

        private int _uiState = 0;

        private string _subject = null;

        private Panel _composePanel = new Panel();
        private Panel _replyPanel = new Panel();

        private TextBox _composeSubject = new TextBox();
        private TextBox _composeTo = new TextBox();
        private TextEditor _composeMessage = new TextEditor();
        private TextEditor _replyMessage = new TextEditor();

        private string[] _addresses = null;

        private void SetupMainUI()
        {
            _compose.Text = "Compose";
            _bodyView.Clear();
            List<string> _subjects = new List<string>();
            switch (_uiState)
            {
                case 0: //inbox
                    _header.Text = "Inbox";
                    _inbox.ClearItems();
                    foreach(var message in _email.Inbox.OrderByDescending(x=>x.Timestamp))
                    {
                        if (_subjects.Contains(message.Subject))
                            continue;
                        var item = new ListViewItem
                        {
                            Tag = message.Subject,
                            Value = $"{message.Subject} - {message.From}"
                        };
                        _inbox.AddItem(item);
                        _subjects.Add(message.Subject);
                    }
                    _bodyView.AddChild(_inbox);
                    break;
                case 1: //outbox
                    _header.Text = "Outbox";
                    _outbox.ClearItems();
                    foreach (var message in _email.Outbox.OrderByDescending(x => x.Timestamp))
                    {
                        if (_subjects.Contains(message.Subject))
                            continue;
                        var item = new ListViewItem
                        {
                            Tag = message.Subject,
                            Value = $"{message.Subject} - {message.To}"
                        };
                        _outbox.AddItem(item);
                        _subjects.Add(message.Subject);
                    }
                    _bodyView.AddChild(_outbox);
                    break;
                case 2: //view message
                    _compose.Text = "Reply";
                    _header.Text = _subject;
                    _thread.Clear();
                    foreach (var message in _email.GetThread(_subject))
                    {
                        var panel = new EmailMessagePanel();
                        panel.Subject = message.Subject;
                        panel.From = message.From;
                        panel.Message = message.Message;
                        _thread.AddChild(panel);
                    }
                    _bodyView.AddChild(_thread);
                    break;
                case 3: //compose message
                    _header.Text = "Compose message";
                    _compose.Text = "Send";
                    _composeMessage.Text = "";
                    _composeSubject.Text = "";
                    _composeTo.Text = "";
                    _bodyView.AddChild(_composePanel);
                    break;
                case 4: //compose reply
                    _header.Text = _subject;
                    _compose.Text = "Send";
                    _replyMessage.Text = "";
                    _bodyView.AddChild(_replyPanel);
                    var addresses = new List<string>();
                    foreach(var message in _email.GetThread(_subject))
                    {
                        if (message.From == _email.MyEmailAddress || addresses.Contains(message.From))
                            continue;
                        addresses.Add(message.From);
                    }
                    _addresses = addresses.ToArray();
                    break;
            }
        }

        public EmailViewer(WindowSystem _winsys) : base(_winsys)
        {
            Width = 750;
            Height = 500;

            AddChild(_sidebar);
            AddChild(_body);
            AddChild(_header);
            AddChild(_compose);

            _compose.Click += (o, a) =>
            {
                switch(_uiState)
                {
                    case 0:
                    case 1:
                        _uiState = 3;
                        SetupMainUI();
                        break;
                    case 2:
                        _uiState = 4;
                        SetupMainUI();
                        break;
                    case 3:
                        if (string.IsNullOrWhiteSpace(_composeSubject.Text))
                        {
                            _composeSubject.Text = "(No subject)";
                        }
                        if (string.IsNullOrWhiteSpace(_composeTo.Text))
                        {
                            _infobox.Show("No recipient specified.", "Who should we send this message to? You can't specify a blank recipient!");
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(_composeMessage.Text))
                        {
                            _infobox.Show("Blank reply", "You can't send a blank message. Please try again.");
                            return;
                        }
                        _email.SendMessage(_composeTo.Text, _composeSubject.Text, _composeMessage.Text);
                        _uiState = 1;
                        SetupMainUI();
                        break;
                    case 4:
                        if(string.IsNullOrWhiteSpace(_replyMessage.Text))
                        {
                            _infobox.Show("Blank reply", "You can't send a blank message. Please try again.");
                            return;
                        }
                        foreach(var address in _addresses)
                        {
                            _email.SendMessage(address, _subject, _replyMessage.Text);
                        }
                        _uiState = 1;
                        SetupMainUI();
                        break;
                }
            };

            _body.AddChild(_bodyView);
            _sidebar.AddChild(_sidebarView);
            _sidebarView.Layout = ListViewLayout.List;
            _inbox.Layout = ListViewLayout.List;
            _outbox.Layout = ListViewLayout.List;
            _sidebarView.AddItem(new ListViewItem
            {
                Value = "Inbox",
                Tag = 0
            });
            _sidebarView.AddItem(new ListViewItem
            {
                Value = "Sent messages",
                Tag = 1
            });

            _sidebarView.ItemClicked += (item) =>
            {
                _uiState = (int)item.Tag;
                SetupMainUI();
            };
            _inbox.ItemClicked += (item) =>
            {
                _subject = item.Tag.ToString();
                _uiState = 2;
                SetupMainUI();
            };
            _outbox.ItemClicked += (item) =>
            {
                _subject = item.Tag.ToString();
                _uiState = 2;
                SetupMainUI();
            };
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            SetupMainUI();

            _composePanel.AutoSize = true;
            _composePanel.AddChild(_composeTo);
            _composePanel.AddChild(_composeSubject);
            _composePanel.AddChild(_composeMessage);
            _replyPanel.AutoSize = true;
            _replyPanel.AddChild(_replyMessage);

            _thread.AutoSize = true;
        }


        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
        }

        protected override void OnUpdate(GameTime time)
        {
            Title = $"{_email.MyEmailAddress} - Email Viewer";

            _sidebar.X = 0;
            _sidebar.Y = 0;
            _sidebar.Width = 200;
            _sidebar.Height = Height;

            _sidebarView.Width = _sidebar.Width;

            _header.AutoSize = true;
            _header.X = _sidebar.Width + 15;
            _header.Y = 15;
            _header.MaxWidth = (((Width - _sidebar.Width) - _compose.Width) - 15) - 30;

            _compose.X = (Width - _compose.Width) - 15;
            _compose.Y = _header.Y + ((_header.Height - _compose.Height) / 2);

            _body.X = _sidebar.Width;
            _body.Y = _header.Y + _header.Height + 15;
            _body.Width = Width - _body.X;
            _body.Height = Height - _body.Y;

            _bodyView.AutoSize = true;
            _bodyView.Width = _body.Width;

            _inbox.Width = _body.Width;
            _outbox.Width = _body.Width;
            _thread.Width = _body.Width;

            _composePanel.Width = _body.Width;
            _replyPanel.Width = _body.Width;

            _replyMessage.Width = _replyPanel.Width - 30;
            _replyMessage.AutoHeight = true;
            _replyMessage.X = 15;
            _replyMessage.Y = 15;

            _composeTo.Label = "To...";
            _composeTo.Width = _composePanel.Width - 30;
            _composeTo.X = 15;
            _composeTo.Y = 15;
            _composeSubject.Label = "Subject...";
            _composeSubject.X = 15;
            _composeSubject.Y = _composeTo.Y + _composeTo.Height + 5;
            _composeSubject.Width = _composeTo.Width;
            _composeMessage.X = 15;
            _composeMessage.Y = _composeSubject.Y + _composeSubject.Height + 10;
            _composeMessage.Width = _composeTo.Width;
            _composeMessage.AutoHeight = true;
        }
    }

    public class EmailMessagePanel : Control
    {
        private string _subject = "(No subject)";
        private string _message = "";
        private string _from = "mail@exo.no"; //not an easter egg

        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Parent == null)
                return;

            Width = Parent.Width - 30;
            X = 15;


            var header = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Header3);
            var highlight = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Highlight);
            var body = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);

            var headMeasure = TextRenderer.MeasureText(_subject, header, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            var fromMeasure = TextRenderer.MeasureText(_from, highlight, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            var bodyMeasure = TextRenderer.MeasureText(_message, body, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);

            Height = 15 + (int)headMeasure.Y + 5 + (int)fromMeasure.Y + 10 + (int)bodyMeasure.Y + 30;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            Theme.DrawControlBG(gfx, 0, 0, Width, Height - 15);

            var header = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Header3);
            var highlight = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Highlight);
            var body = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);

            var headMeasure = TextRenderer.MeasureText(_subject, header, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            var fromMeasure = TextRenderer.MeasureText(_from, highlight, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            var bodyMeasure = TextRenderer.MeasureText(_message, body, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);

            gfx.DrawString(_subject, 15, 15, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.Header3), header, TextAlignment.Left, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            gfx.DrawString(_from, 15, 15 + (int)headMeasure.Y + 5, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.Highlight), highlight, TextAlignment.Left, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);
            gfx.DrawString(_message, 15, 15 + (int)headMeasure.Y + 5 + (int)fromMeasure.Y + 10, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), body, TextAlignment.Left, Width - 30, Plex.Engine.TextRenderers.WrapMode.Words);

        }
    }
}
