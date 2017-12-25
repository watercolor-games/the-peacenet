using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string WatercolorUid { get; set; }
        public string Username { get; set; }
        public string MessageContents { get; set; }
        public DateTime TimeUtc { get; set; }
        public ChatMessageType Type { get; set; }
    }

    public enum ChatMessageType
    {
        Regular,
        Action,
        Join,
        Leave
    }
}
