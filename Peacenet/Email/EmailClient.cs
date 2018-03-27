using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peacenet.Server;
using Plex.Engine;
using Plex.Engine.Interfaces;

namespace Peacenet.Email
{
    public class EmailClient : IEngineComponent
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private ItchOAuthClient _itch = null;

        private string _myAddress = null;

        private List<EmailMessage> _messages = new List<EmailMessage>();

        private void PopulateMessages()
        {
            if (!_server.Connected)
                return;
            _server.SendMessage(Plex.Objects.ServerMessageType.MAIL_GETALL, null, (res, reader) =>
            {
                if (res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    return;
                _messages.Clear();
                _myAddress = reader.ReadString();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string from = reader.ReadString();
                    string to = reader.ReadString();
                    bool isUnread = reader.ReadBoolean();
                    string timestamp = reader.ReadString();
                    string subject = reader.ReadString();
                    string message = reader.ReadString();

                    _messages.Add(new EmailMessage
                    {
                        IsUnread = isUnread,
                        From = from,
                        To = to,
                        Subject = subject,
                        Message = message,
                        Timestamp = DateTime.Parse(timestamp)
                    });
                }
            }).Wait();
        }

        private int _unreadCount = 0;

        public int UnreadMessages
        {
            get
            {
                return _unreadCount;
            }
        }

        public EmailMessage[] Inbox
        {
            get
            {
                return _messages.Where(x => x.To == _myAddress).ToArray();
            }
        }

        public EmailMessage[] Outbox
        {
            get
            {
                return _messages.Where(x => x.From == _myAddress).ToArray();
            }
        }

        public EmailMessage[] GetThread(string subject)
        {
            return _messages.Where(x => x.Subject == subject).OrderBy(x => x.Timestamp).ToArray();
        }

        public void SendMessage(string to, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient address can't be blank.");
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject can't be blank.");
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message can't be blank.");

            Task.Run(() =>
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                    {
                        writer.Write(to);
                        writer.Write(subject);
                        writer.Write(message);
                        writer.Flush();
                        _server.SendMessage(Plex.Objects.ServerMessageType.MAIL_SEND, ms.ToArray(), null).Wait();
                    }
                }
            });
        }

        public string MyEmailAddress
        {
            get
            {
                return _myAddress;
            }
        }

        public void Initiate()
        {
            _server.BroadcastReceived += (type, reader) =>
            {
                if (type == Plex.Objects.ServerBroadcastType.EmailReceived)
                {
                    string id = reader.ReadString();
                    if (_server.IsMultiplayer)
                        if (_itch.User.id.ToString() != id)
                            return;
                    string from = reader.ReadString();
                    string subject = reader.ReadString();
                    if (_os.IsDesktopOpen)
                        _os.Desktop.ShowNotification($"New message from {from}", subject);
                    Task.Run(() => PopulateMessages());
                }
                else if (type == Plex.Objects.ServerBroadcastType.EmailsUnread)
                {
                    string id = reader.ReadString();
                    if (_server.IsMultiplayer)
                        if (_itch.User.id.ToString() != id)
                            return;
                    _unreadCount = reader.ReadInt32();
                    Task.Run(() => PopulateMessages());
                }
            };
        }
    }

    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsUnread { get; set; }
    }
}
