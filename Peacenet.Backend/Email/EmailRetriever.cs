using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.Backend.Email
{
    public class EmailRetriever : IMessageHandler
    {
        [Dependency]
        private EmailProvider _email = null;

        [Dependency]
        private SystemEntityBackend _entity = null;

        public ServerMessageType HandledMessageType => ServerMessageType.MAIL_GETALL;

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var entity = _entity.GetPlayerEntityId(session);
            var emails = _email.GetEmails(entity);
            datawriter.Write(_email.GetAddress(entity));
            datawriter.Write(emails.Length);
            foreach(var email in emails)
            {
                datawriter.Write(_email.GetAddress(email.FromEntity));
                datawriter.Write(_email.GetAddress(email.ToEntity));
                datawriter.Write(_email.GetUnread(email.Id, entity));
                datawriter.Write(email.Timestamp.ToString());
                datawriter.Write(email.Subject);
                datawriter.Write(email.Message);
            }
            return ServerResponseType.REQ_SUCCESS;
        }
    }
}
