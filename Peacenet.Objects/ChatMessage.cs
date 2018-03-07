using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// A unique ID for the message.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The Watercolor User ID of the message's author.
        /// </summary>
        public string WatercolorUid { get; set; }
        /// <summary>
        /// The username of the author.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The content of the message.
        /// </summary>
        public string MessageContents { get; set; }
        /// <summary>
        /// The time at which the message was sent, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime TimeUtc { get; set; }

        /// <summary>
        /// The type of message received.
        /// </summary>
        public ChatMessageType Type { get; set; }
    }

    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public enum ChatMessageType
    {
        /// <summary>
        /// A regular text message ("I love cake", "Did you see these new API docs?", etc)
        /// </summary>
        Regular,
        /// <summary>
        /// A third-person action ("Alkaline loves cake", "She went to the store", etc)
        /// </summary>
        Action,
        /// <summary>
        /// A user join event ("Alkaline has joined the chat.")
        /// </summary>
        Join,
        /// <summary>
        /// A user leave event ("Alkaline has left the chat.")
        /// </summary>
        Leave
    }
}
