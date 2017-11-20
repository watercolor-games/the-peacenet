using System;
using System.Collections.Generic;
using Plex.Objects;
using System.Reflection;
using System.Linq;
using System.IO;

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
                    Logger.Log($"Found handler: {type.Name}");
                    var handler = (IMessageHandler)Activator.CreateInstance(type, null);
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
