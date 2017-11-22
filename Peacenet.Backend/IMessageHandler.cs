using System;
using Plex.Objects;
using System.IO;

namespace Peacenet.Backend
{
    public interface IMessageHandler
    {
        ServerMessageType HandledMessageType { get; }

        ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiresSessionAttribute : Attribute { }
}

