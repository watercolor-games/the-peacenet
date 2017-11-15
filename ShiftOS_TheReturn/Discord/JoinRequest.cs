using System;
namespace Plex.Engine.Discord
{
    [Serializable]
    public struct JoinRequest
    {
        public string userId;
        public string username;
        public string avatar;
    }
}
