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
        private LiteCollection<ChatMessage> _chatlog = null;

        [Dependency]
        private DatabaseHolder _db = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("Chat system is starting...");
            _chatlog = _db.Database.GetCollection<ChatMessage>("chatlog");
            _chatlog.EnsureIndex(x => x.Id);
            Logger.Log("Done.");
        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }

        [Dependency]
        private Backend _backend = null;

        /// <summary>
        /// Add a message to the log.
        /// </summary>
        /// <param name="author">The author's username</param>
        /// <param name="uid">The Watercolor user ID of the author</param>
        /// <param name="message">The message text</param>
        /// <param name="type">The type of message</param>
        public void AddMessage(string author, string uid, string message, ChatMessageType type = ChatMessageType.Regular)
        {
            _chatlog.Insert(new ChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageContents = message,
                TimeUtc = DateTime.UtcNow,
                Username = author,
                WatercolorUid = uid,
                Type = type
            });

            byte[] data = null;
            using(var ms = new MemoryStream())
            {
                using(var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    switch (type)
                    {
                        case ChatMessageType.Join:
                        case ChatMessageType.Leave:
                            //only write author
                            writer.Write(author);
                            break;
                        case ChatMessageType.Action:
                        case ChatMessageType.Regular:
                            //write author and message
                            writer.Write(author);
                            writer.Write(message);
                            break;
                    }
                    data = ms.ToArray();
                }
            }
            switch (type)
            {
                case ChatMessageType.Regular:
                    Logger.Log($"[chat] <{author}> {message}");
                    _backend.Broadcast(ServerBroadcastType.Chat_Message, data);
                    break;
                case ChatMessageType.Action:
                    Logger.Log($"[chat] *{author} {message}*");
                    _backend.Broadcast(ServerBroadcastType.Chat_Action, data);
                    break;
                case ChatMessageType.Join:
                    Logger.Log($"[chat] {author} has joined");
                    _backend.Broadcast(ServerBroadcastType.Chat_UserJoin, data);
                    break;
                case ChatMessageType.Leave:
                    Logger.Log($"[chat] {author} has left.");
                    _backend.Broadcast(ServerBroadcastType.Chat_UserLeave, data);
                    break;

            }
        }

        /// <summary>
        /// Retrieve the last <paramref name="count"/> messages in the log.
        /// </summary>
        /// <param name="count">The amount of messages to retrieve.</param>
        /// <returns>An <see cref="IEnumerable{ChatMessage}"/> containing the latest messages found.</returns>
        public IEnumerable<ChatMessage> RetrieveLast(int count)
        {
            int sent = 0;
            foreach(var msg in _chatlog.FindAll().OrderByDescending(x => x.TimeUtc))
            {
                if (sent >= count)
                    break;
                yield return msg;
                sent++;
            }
        }


    }

    /// <summary>
    /// Handler for chat join requests.
    /// </summary>
    [RequiresSession]
    public class ChatJoinHandler : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.CHAT_JOIN;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var usr = backend.GetUserInfo(session);
            if (usr == null)
                return ServerResponseType.REQ_ERROR;
            var chat = backend.GetBackendComponent<ChatBackend>();
            chat.AddMessage(usr.username, session, "", ChatMessageType.Join);

            return ServerResponseType.REQ_SUCCESS;
        }
    }

    /// <summary>
    /// Handler for chat leave requests.
    /// </summary>
    [RequiresSession]
    public class ChatLeaveHandler : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.CHAT_LEAVE;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var usr = backend.GetUserInfo(session);
            if (usr == null)
                return ServerResponseType.REQ_ERROR;
            var chat = backend.GetBackendComponent<ChatBackend>();
            chat.AddMessage(usr.username, session, "", ChatMessageType.Leave);

            return ServerResponseType.REQ_SUCCESS;
        }
    }

    /// <summary>
    /// Handler for incoming chat messages.
    /// </summary>
    [RequiresSession]
    public class ChatMessageHandler : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.CHAT_SENDTEXT;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var usr = backend.GetUserInfo(session);
            if (usr == null)
                return ServerResponseType.REQ_ERROR;
            string messagetext = datareader.ReadString();
            var chat = backend.GetBackendComponent<ChatBackend>();
            chat.AddMessage(usr.username, session, messagetext, ChatMessageType.Regular);

            return ServerResponseType.REQ_SUCCESS;
        }
    }

    /// <summary>
    /// A handler for chat actions.
    /// </summary>
    [RequiresSession]
    public class ChatActionHandler : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.CHAT_SENDACTION;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var usr = backend.GetUserInfo(session);
            if (usr == null)
                return ServerResponseType.REQ_ERROR;
            string messagetext = datareader.ReadString();
            var chat = backend.GetBackendComponent<ChatBackend>();
            chat.AddMessage(usr.username, session, messagetext, ChatMessageType.Action);

            return ServerResponseType.REQ_SUCCESS;
        }
    }

}
