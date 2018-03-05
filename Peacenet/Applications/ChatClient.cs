using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;

namespace Peacenet.Applications
{
    [AppLauncher("Peacenet Communicator", "Networking")]
    public class ChatClient : Window
    {
        private ScrollView _sideView = new ScrollView();
        private Panel _sidePanel = new Panel();
        private Label _contactsHeader = new Label();
        private ListView _contactsList = new ListView();
        private Label _groupsHeader = new Label();
        private ListView _groupsList = new ListView();

        private TextBox _messageInput = new TextBox();
        private Button _sendMessage = new Button();

        private Label _mainHeader = new Label();

        private ChatSurface _surface = new ChatSurface();

        private bool _inGroup = true;

        [Dependency]
        private ChatManager _chat = null;

        public ChatClient(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Peacenet Communicator";
            Width = 800;
            Height = 600;

            AddChild(_sideView);
            AddChild(_messageInput);
            AddChild(_sendMessage);
            AddChild(_surface);

            _sideView.AddChild(_sidePanel);
            _sidePanel.AddChild(_contactsHeader);
            _sidePanel.AddChild(_contactsList);
            _sidePanel.AddChild(_groupsHeader);
            _sidePanel.AddChild(_groupsList);

            AddChild(_mainHeader);

            _mainHeader.Text = "Peacenet Communicator Test Environment";
            _mainHeader.AutoSize = true;

            _messageInput.Label = "Send message...";

            _sidePanel.AutoSize = true;
            _groupsHeader.AutoSize = true;
            _contactsHeader.AutoSize = true;
            _groupsHeader.Text = "Groups";
            _contactsHeader.Text = "Contacts";
            _groupsHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _contactsHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _contactsList.Layout = ListViewLayout.List;
            _groupsList.Layout = ListViewLayout.List;
            _sendMessage.Text = "Send";

            _sendMessage.Click += (o, a) =>
            {
                SendMessage(_messageInput.Text);
                _messageInput.Text = "";
            };
            _messageInput.KeyEvent += (o, a) =>
            {
                if(a.Key== Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    SendMessage(_messageInput.Text);
                    _messageInput.Text = "";
                }
            };
            _chat.MessageReceived += _chat_MessageReceived;
        }

        private void _chat_MessageReceived(string author, string recipient, string message)
        {
            bool showMessage = false;

            if (_inGroup == true)
                showMessage = recipient == "group";

            if(showMessage)
            {
                _surface.AddMessage(author, message);
            }
        }

        public override void Close()
        {
            _chat.MessageReceived -= _chat_MessageReceived;
            base.Close();
        }

        private void SendMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            if (_inGroup)
                _chat.SendMessage(_messageInput.Text, "group"); //sending 'group' as recipient will tell the server this isn't a DM.
        }

        protected override void OnUpdate(GameTime time)
        {
            _mainHeader.X = 10;
            _mainHeader.Y = 10;
            _mainHeader.MaxWidth = Width - 20;

            _sideView.X = 0;
            _sideView.Y = _mainHeader.Y + _mainHeader.Height + 10;
            _sidePanel.Width = 250;
            _sideView.Height = Height - _sideView.Y;

            _sendMessage.X = (Width - _sendMessage.Width) - 10;

            _messageInput.X = _sidePanel.Width + 10;
            _messageInput.Width = ((Width - _messageInput.X) - _sendMessage.Width) - 15;

            int maxInputHeight = Math.Max(_messageInput.Height, _sendMessage.Height);
            if(maxInputHeight==_sendMessage.Height)
            {
                _sendMessage.Y = (Height - _sendMessage.Height) - 10;
                _messageInput.Y = _sendMessage.Y + ((_sendMessage.Height - _messageInput.Height) / 2);
            }
            else
            {
                _messageInput.Y = (Height - _messageInput.Height) - 10;
                _sendMessage.Y = _messageInput.Y + ((_messageInput.Height - _sendMessage.Height) / 2);
            }

            _surface.X = _sidePanel.Width;
            _surface.Y = _sideView.Y;
            _surface.Width = Width - _surface.X;

            int bottomHeight = Height - (Math.Min(_sendMessage.Y, _messageInput.Y));
            _surface.Height = ((Height - _surface.Y) - bottomHeight) - 5;

            _contactsHeader.X = 5;
            _groupsHeader.X = 5;
            _contactsHeader.MaxWidth = _sidePanel.Width - 10;
            _groupsHeader.MaxWidth = _sidePanel.Width - 10;
            _contactsHeader.Y = 5;
            _contactsList.X = 5;
            _contactsList.Y = _contactsHeader.Y + _contactsHeader.Height + 5;
            _groupsHeader.Y = _contactsList.Y + _contactsList.Height + 15;
            _groupsList.X = 5;
            _groupsList.Y = _groupsHeader.Y + _groupsHeader.Height + 5;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
        }

        public class ChatSurface : Control
        {
            private List<ChatMessage> _messages = new List<ChatMessage>();
            private int _scrollBack = 0;

            public void AddMessage(string author, string text)
            {
                _messages.Add(new ChatMessage
                {
                    Author = author,
                    Message = text,
                    Timestamp = DateTime.Now
                });
                Invalidate();
            }

            public void AddMessage(ChatMessage message)
            {
                if (message == null)
                    return;
                _messages.Add(message);
                Invalidate();
            }

            public void ClearMessages()
            {
                _messages.Clear();
                Invalidate();
            }

            protected override void OnPaint(GameTime time, GraphicsContext gfx)
            {
                Theme.DrawControlBG(gfx, 0, 0, Width, Height);
                if (_messages.Count == 0)
                    return;
                var font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);
                var messageWithLongestName = _messages.OrderByDescending(x => font.MeasureString(x.Author).X).First();

                var messageNameSpace = font.MeasureString(messageWithLongestName.Author).X + 30;

                gfx.DrawRectangle((int)messageNameSpace, 0, 1, Height, Theme.GetAccentColor());

                var orderedByDate = _messages.OrderByDescending(x => x.Timestamp).ToArray();

                int textY = Height;
                for(int i = _scrollBack; i < orderedByDate.Length; i++)
                {
                    if (textY <= 0)
                        break;
                    var nameMeasure = font.MeasureString(orderedByDate[i].Author);
                    var messageMeasure = TextRenderer.MeasureText(orderedByDate[i].Message, font, (Width - (int)messageNameSpace) - 10, Plex.Engine.TextRenderers.WrapMode.Words);
                    gfx.DrawString(orderedByDate[i].Message, (int)messageNameSpace + 5, textY - (int)messageMeasure.Y, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System), font, TextAlignment.Left, (Width - (int)messageNameSpace) - 10, Plex.Engine.TextRenderers.WrapMode.Words);
                    gfx.Batch.DrawString(font, orderedByDate[i].Author, new Vector2((messageNameSpace - 15) - nameMeasure.X, textY - messageMeasure.Y), Theme.GetAccentColor());
                    textY -= (int)messageMeasure.Y;
                }
            }
        }

        public class ChatMessage
        {
            public string Author { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
