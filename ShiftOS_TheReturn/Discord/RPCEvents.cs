using System;
using Plex.Engine.Discord.CallbackDelegates;
namespace Plex.Engine.Discord
{
    public struct EventHandlers
    {
        public ReadyCallback readyCallback;
        public DisconnectedCallback disconnectedCallback;
        public ErrorCallback errorCallback;
        public JoinCallback joinCallback;
        public SpectateCallback spectateCallback;
        public RequestCallback requestCallback;
    }
}
