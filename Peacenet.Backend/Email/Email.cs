using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend.Email
{
    public class Email
    {
        public string Id { get; set; }
        public string FromEntity { get; set; }
        public string ToEntity { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } //used for ordering messages in threads.
    }

    public class ReadEmail
    {
        public string Id { get; set; }
        public string Entity { get; set; }
        public string EmailId { get; set; }
    }
}
