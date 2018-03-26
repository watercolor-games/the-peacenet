using System;
using System.Collections.Generic;
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

        private int _unreadCount = 0;

        public int UnreadMessages
        {
            get
            {
                return _unreadCount;
            }
        }

        public void Initiate()
        {
            _server.BroadcastReceived += (type, reader) =>
            {
                if(type == Plex.Objects.ServerBroadcastType.EmailReceived)
                {
                    string id = reader.ReadString();
                    if (_server.IsMultiplayer)
                        if (_itch.User.id.ToString() != id)
                            return;
                    string from = reader.ReadString();
                    string subject = reader.ReadString();
                    if (_os.IsDesktopOpen)
                        _os.Desktop.ShowNotification($"New message from {from}", subject);
                }
                else if(type == Plex.Objects.ServerBroadcastType.EmailsUnread)
                {
                    string id = reader.ReadString();
                    if (_server.IsMultiplayer)
                        if (_itch.User.id.ToString() != id)
                            return;
                    _unreadCount = reader.ReadInt32();
                }
            };
        }
    }
}
