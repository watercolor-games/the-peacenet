using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    /// <summary>
    /// Represents a datagram sent between a Peacenet server and a Peacenet client.
    /// </summary>
    public class PlexServerHeader
    {
        /// <summary>
        /// The type of the message sent.
        /// </summary>
        public byte Message { get; set; }
        /// <summary>
        /// The session ID of the message, used for authentication.
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// The message content.
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// A unique identifier for this datagram so the client knows what response datagrams are associated with it.
        /// </summary>
        public string TransactionKey { get; set; }
    }
}
