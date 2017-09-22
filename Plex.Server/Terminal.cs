using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class Terminal
    {
        private static string _shelloverride = "";

        public static void SetShellOverride(string value)
        {
            _shelloverride = value;
        }

        public static bool RunClient(string cmd, Dictionary<string, object> args)
        {
            return false;
        }

        [ServerMessageHandler("trm_invoke")]
        [SessionRequired]
        public static void InvokeCMD(string session_id, string content, string ip)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var memwriter = new MemoryTextWriter();
            Console.SetOut(memwriter);
            SetShellOverride(data["shell"].ToString());
            bool result = RunClient(data["cmd"].ToString(), data["args"] as Dictionary<string, object>);
            SetShellOverride("");
            var str = Console.OpenStandardOutput();
            var writer = new System.IO.StreamWriter(str);
            Console.SetOut(writer);

            Program.SendMessage(new Objects.PlexServerHeader
            {
                Message = "trm_result",
                Content = JsonConvert.SerializeObject(new
                {
                    result = result,
                    message = memwriter.ToString()
                }),
                IPForwardedBy = ip,
                SessionID = session_id
            });
        }
    }
}
