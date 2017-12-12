using System;
using System.Collections.Generic;
using Plex.Objects;
using System.Reflection;
using System.Linq;
using System.IO;
using Peacenet.Backend.Sessions;

namespace Peacenet.Backend
{
    public class MessageDelegator : IBackendComponent
    {
        private List<IMessageHandler> _handlers = null;

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

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType messagetype, string session_id, byte[] dgram, out byte[] returndgram)
        {
            Logger.Log("Attempting to handle a " + messagetype + "...");
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

        public void SafetyCheck()
        {
            //are you nuts?
        }

        public void Unload()
        {
            Logger.Log("Clearing handler list...");
            _handlers = null;
            Logger.Log("Done.");
        }
    }
}
