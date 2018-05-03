using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.Backend.Email
{
    public class SendEmail : IMessageHandler
    {
        [Dependency]
        private EmailProvider _email = null;

        [Dependency]
        private SystemEntityBackend _entity = null;

        public ServerMessageType HandledMessageType => ServerMessageType.MAIL_SEND;

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string entity = _entity.GetPlayerEntityId(session);
            string to = datareader.ReadString();
            string subject = datareader.ReadString();
            string text = datareader.ReadString();
            _email.SendMessage(entity, to, subject, text);
            return ServerResponseType.REQ_SUCCESS;
        }
    }
}
