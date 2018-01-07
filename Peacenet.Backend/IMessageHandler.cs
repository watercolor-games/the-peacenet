using System;
using Plex.Objects;
using System.IO;

namespace Peacenet.Backend
{
    /// <summary>
    /// Defines a handler object for a <see cref="ServerMessageType"/>. 
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Retrieve the type of message this object is meant to handle.
        /// </summary>
        ServerMessageType HandledMessageType { get; }

        /// <summary>
        /// Handle the incoming request.
        /// </summary>
        /// <param name="backend">The <see cref="Backend"/> representing the server environment.</param>
        /// <param name="message">This is completely redundant...</param>
        /// <param name="session">The session ID of the user who sent the request. Null if the request is anonymous.</param>
        /// <param name="datareader">A <see cref="BinaryReader"/> for reading the body of the message.</param>
        /// <param name="datawriter">A <see cref="BinaryWriter"/> for writing to the response's body.</param>
        /// <returns>The response of the message.</returns>
        ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter);
    }

    /// <summary>
    /// Indicates that an <see cref="IMessageHandler"/> requires a valid authenticated user session. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiresSessionAttribute : Attribute { }
}

