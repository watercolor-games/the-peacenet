using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plex.Server
{
    public static class Hacking
    {
        [SessionRequired]
        [ServerMessageHandler("get_hackable")]
        public static void GetHackable(string session_id, string content, string ip)
        {
            var system = Program.GetSaveFromPrl(content);
            if(system != null)
            {
                Program.SendMessage(new Objects.PlexServerHeader
                {
                    Content = JsonConvert.SerializeObject(system),
                    Message = "hackable_data",
                    IPForwardedBy = ip,
                    SessionID = session_id
                });
            }
        }
    }
}
