using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using Newtonsoft.Json;
using NetSockets;

namespace ShiftOS.Server
{
    public static class RemoteTerminal
    {
        [MudRequest("trm_handshake_accept")]
        public static void AcceptHandshake(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["guid"] != null && args["target"] != null)
            {
                
                Program.ClientDispatcher.Server.DispatchTo(new Guid(args["target"] as string), new NetObject("hold_it", new ServerMessage
                {
                    Name = "trm_handshake_guid",
                    GUID = args["guid"] as string
                }));
            }
        }

        [MudRequest("trm_handshake_request")]
        public static void RequestHandshake(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["username"] != null && args["password"] != null && args["sysname"] != null)
            {
                Program.ClientDispatcher.Server.DispatchAll(new NetObject("hold_my_hand", new ServerMessage
                {
                    Name = "handshake_from",
                    GUID = guid,
                    Contents = JsonConvert.SerializeObject(args)
                }));
            }
        }

        [MudRequest("trm_handshake_stop")]
        public static void StopSession(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["guid"] != null)
            {
                Program.ClientDispatcher.Server.DispatchTo(new Guid(args["guid"] as string), new NetObject("trm_handshake_stop", new ServerMessage
                {
                    Name = "trm_handshake_stop",
                    GUID = guid
                }));
            }

        }

        [MudRequest("write")]
        public static void WriteText(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            if (args["guid"] != null && args["text"] != null)
            {
                Program.ClientDispatcher.DispatchTo("pleasewrite", args["guid"] as string, args["text"]);
            }

        }
    }
}
