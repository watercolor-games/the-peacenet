using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;

namespace Peacenet.Backend
{
    /// <summary>
    /// Provides a backend for basic server chat.
    /// </summary>
    public class ChatBackend : IBackendComponent
    {
        private LiteCollection<ChatMessage> _messages = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        [Dependency]
        private DatabaseHolder _database = null;

        [Dependency]
        private Backend _backend = null;

        public void Initiate()
        {
            _messages = _database.Database.GetCollection<ChatMessage>("chat_messages");
            _messages.EnsureIndex(x => x.Id);
            int deletedNoAuthor = _messages.Delete(x => _entityBackend.GetEntity(x.AuthorEntityId) == null);
            int deletedNoValidRecipient = _messages.Delete(x => x.RecipientEntityId != null && (_entityBackend.GetEntity(x.RecipientEntityId) == null));
            Logger.Log($"{deletedNoAuthor} chat messages deleted from database because of no valid author entity. {deletedNoValidRecipient} deleted because of no valid recipient entity. {_messages.Count()} still remain.");
        }

        public void SafetyCheck()
        {
            int deletedNoAuthor = _messages.Delete(x => _entityBackend.GetEntity(x.AuthorEntityId) == null);
            int deletedNoValidRecipient = _messages.Delete(x => x.RecipientEntityId != null && (_entityBackend.GetEntity(x.RecipientEntityId) == null));
            Logger.Log($"{deletedNoAuthor} chat messages deleted from database because of no valid author entity. {deletedNoValidRecipient} deleted because of no valid recipient entity. {_messages.Count()} still remain.");
        }

        public void AddMessage(string authorEntity, string recipientEntity, string message)
        {
            if (authorEntity == null)
                return;
            if (_entityBackend.GetEntity(authorEntity) == null)
                return;
            if (recipientEntity != null)
                if (_entityBackend.GetEntity(recipientEntity) == null)
                    return;
            _messages.Insert(new Peacenet.Backend.ChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                AuthorEntityId = authorEntity,
                RecipientEntityId = recipientEntity,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            string authorName = _entityBackend.GetEntity(authorEntity).DisplayName;
            string recipientName = (recipientEntity == null) ? "group" : _entityBackend.GetEntity(recipientEntity).DisplayName;
            Logger.Log($"[chat] <{authorName} -> {recipientName}> {message}");
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    writer.Write(authorName);
                    writer.Write(recipientName);
                    writer.Write(message);
                    writer.Flush();
                    if(recipientEntity==null)
                        _backend.Broadcast(ServerBroadcastType.Chat_MessageReceived, ms.ToArray());
                    else
                    {
                        if(_entityBackend.GetPlayerId(recipientEntity) != null)
                        {
                            _backend.BroadcastToPlayer(ServerBroadcastType.Chat_MessageReceived, ms.ToArray(), _entityBackend.GetPlayerId(recipientEntity));
                        }
                        if (_entityBackend.GetPlayerId(authorEntity) != null)
                        {
                            _backend.BroadcastToPlayer(ServerBroadcastType.Chat_MessageReceived, ms.ToArray(), _entityBackend.GetPlayerId(authorEntity));
                        }
                    }
                }
            }
        }

        public ChatMessage[] GetAllTowardsEntity(string recipient)
        {
            if (_entityBackend.GetEntity(recipient) == null)
                return new ChatMessage[0];
            return _messages.Find(x => x.RecipientEntityId == recipient).OrderByDescending(x => x.Timestamp).ToArray();
        }

        public string[] GetContacts(string entityId)
        {
            if (_entityBackend.GetEntity(entityId) == null)
                return new string[0];
            List<string> entities = new List<string>();
            foreach(var message in _messages.Find(x=>x.RecipientEntityId==entityId))
            {
                if (!entities.Contains(message.AuthorEntityId))
                    entities.Add(message.AuthorEntityId);
            }
            return entities.ToArray();
        }

        public ChatMessage[] GetLatestGroupLog()
        {
            return _messages.Find(x => x.RecipientEntityId == null).OrderByDescending(x => x.Timestamp).Take(100).ToArray();
        }

        public void Unload()
        {
        }
    }

    public class ChatMessage
    {
        public string Id { get; set; }
        public string AuthorEntityId { get; set; }
        public string RecipientEntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class ChatMessageReceiver : IMessageHandler
    {
        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        [Dependency]
        private ChatBackend _chat = null;

        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.CHAT_SENDMESSAGE;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            string recipient = datareader.ReadString();
            string messagetext = datareader.ReadString();
            string sender = _entityBackend.GetPlayerEntityId(session);
            _chat.AddMessage(sender, (recipient == "group") ? null : recipient, messagetext);
            return ServerResponseType.REQ_SUCCESS;
        }
    }
}
