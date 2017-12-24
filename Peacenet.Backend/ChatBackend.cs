using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend
{
    public class ChatBackend : IBackendComponent
    {
        private LiteCollection<ChatMessage> _chatlog = null;

        [Dependency]
        private DatabaseHolder _db = null;

        public void Initiate()
        {
            Logger.Log("Chat system is starting...");
            _chatlog = _db.Database.GetCollection<ChatMessage>("chatlog");
            _chatlog.EnsureIndex(x => x.Id);
            Logger.Log("Done.");
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
        }
        
        public IEnumerable<ChatMessage> RetrieveLast(int count)
        {
            int sent = 0;
            foreach(var msg in _chatlog.FindAll().OrderByDescending(x => x.TimeUtc))
            {
                if (sent >= count)
                    break;
                yield return msg;
                sent++;
            }
        }


    }

    public class ChatMessage
    {
        public string Id { get; set; }
        public string WatercolorUid { get; set; }
        public string Username { get; set; }
        public string MessageContents { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}
