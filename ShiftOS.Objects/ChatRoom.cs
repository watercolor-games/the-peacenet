using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class ChatRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}
