using System;
using System.Collections.Generic;
using Plex.Objects;
using System.Reflection;
using System.Linq;
using System.IO;

namespace Peacenet.Backend
{
    /// <summary>
    /// Provides a backend component that ISN'T modular which handles receiving and handling of server requests.
    /// </summary>
    public class MessageDelegator : IBackendComponent
    {
        private List<IMessageHandler> _handlers = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("Message delegator is loading...");
            _handlers = new List<IMessageHandler>();
            Logger.Log("Finding all handler objects...");
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IMessageHandler)))
                {
                    var handler = (IMessageHandler)Activator.CreateInstance(type, null);
                    Logger.Log($"Found handler: {type.Name} (for protocol message type {handler.HandledMessageType})");
                    if (_handlers.FirstOrDefault(x => x.HandledMessageType == handler.HandledMessageType) != null)
                    {
                        Logger.Log($"WARNING: Another handler handles the same message type as {handler.GetType().Name}! Ignoring it.");
                        continue;
                    }
                    _handlers.Add(handler);
                }
            }
            Logger.Log($"Done loading handlers. {_handlers.Count} found.");
        }

        /// <summary>
        /// Handles an incoming server message.
        /// </summary>
        /// <param name="backend">The server backend environment</param>
        /// <param name="messagetype">The type of the message</param>
        /// <param name="session_id">The caller user ID</param>
        /// <param name="dgram">The message body</param>
        /// <param name="returndgram">The returned message body</param>
        /// <returns>The result of the handler.</returns>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType messagetype, string session_id, byte[] dgram, out byte[] returndgram)
        {
#if HANG_DEBUG
            Logger.Log("Attempting to handle a " + messagetype + "...");
#endif
            var handler = _handlers.FirstOrDefault(x => x.HandledMessageType == messagetype);
            if (handler == null)
            {
                Logger.Log("WARNING: No handler for this message. Returning error.");
                returndgram = new byte[] { };
                return ServerResponseType.REQ_ERROR;
            }
            else
            {
                bool sessionRequired = handler.GetType().GetCustomAttributes(false).FirstOrDefault(x => x is RequiresSessionAttribute) != null;
                if (sessionRequired)
                {
                    if(string.IsNullOrWhiteSpace(session_id))
                    {
                        returndgram = new byte[0];
                        return ServerResponseType.REQ_LOGINREQUIRED;
                    }
                }
                using (var memstr = new MemoryStream(dgram))
                {
                    using (var rms = new MemoryStream())
                    {
                        var result = handler.HandleMessage(backend, messagetype, session_id, new BinaryReader(memstr), new BinaryWriter(rms));
                        returndgram = rms.ToArray();
                        return result;
                    }
                }
            }

        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
            //are you nuts?
        }

        /// <inheritdoc/>
        public void Unload()
        {
            Logger.Log("Clearing handler list...");
            _handlers = null;
            Logger.Log("Done.");
        }
    }
}
