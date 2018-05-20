using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Email
{
    public class EmailMessage
    {
        public string Id { get; set; }
        public string EmailId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public bool IsUnread { get; set; }
        public DateTime Sent { get; set; }

        public string MissionID { get; set; }
    }
}
