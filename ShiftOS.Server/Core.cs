using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using NetSockets;
using Newtonsoft.Json;
using System.IO;
using static ShiftOS.Server.Program;


namespace ShiftOS.Server
{
    public static class Core
    {
        [MudRequest("getguid_reply", typeof(string))]
        public static void GuidBounce(string guid, object contents)
        {
            //The message's GUID was manipulated by the client to send to another client.
            //So we can just bounce back the message to the other client.
            server.DispatchTo(new Guid(guid), new NetObject("bounce", new ServerMessage
            {
                Name = "getguid_reply",
                GUID = guid,
                Contents = contents as string
            }));

        }

        [MudRequest("getguid_send", typeof(string))]
        public static void GuidReceiver(string guid, object contents)
        {
            string usrname = contents as string;
            server.DispatchAll(new NetObject("are_you_this_guy", new ServerMessage
            {
                Name = "getguid_fromserver",
                GUID = guid,
                Contents = usrname,
            }));

        }

        [MudRequest("script", typeof(Dictionary<string, object>))]
        public static void RunScript(string guid, object contents)
        {
            try
            {
                var args = contents as Dictionary<string, object>;

                string user = "";
                string script = "";
                string sArgs = "";

                if (!args.ContainsKey("user"))
                    throw new MudException("No 'user' arg specified in message to server");

                if (!args.ContainsKey("script"))
                    throw new MudException("No 'script' arg specified in message to server");

                if (!args.ContainsKey("args"))
                    throw new MudException("No 'args' arg specified in message to server");

                user = args["user"] as string;
                script = args["script"] as string;
                sArgs = args["args"] as string;

                if (File.Exists($"scripts/{user}/{script}.lua"))
                {
                    var script_arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(sArgs);
                    server.DispatchTo(new Guid(guid), new NetObject("runme", new ServerMessage
                    {
                        Name = "run",
                        GUID = "Server",
                        Contents = $@"{{
    script:""{File.ReadAllText($"scripts/{user}/{script}.lua").Replace("\"", "\\\"")}"",
							args:""{sArgs}""
							}}"
                    }));
                }
                else
                {
                    throw new MudException($"{user}.{script}: Script not found.");
                }
            }
            catch
            {
                try
                {
                    Program.server.DispatchTo(new Guid(guid), new NetObject("error", new ServerMessage
                    {
                        Name = "Error",
                        GUID = "Server",
                        Contents = JsonConvert.SerializeObject(new MudException("Command parse error"))
                    }));
                }
                catch
                {
                    //fuck.
                }
            }

        }
    }
}
