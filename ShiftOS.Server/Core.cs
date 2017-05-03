/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
        [MudRequest("mud_forward", typeof(ServerMessage))]
        public static void ForwardMessage(string guid, ServerMessage message)
        {
            if (message.GUID == "all")
            {
                Server.Program.server.DispatchAll(new NetObject("forward", new ServerMessage
                {
                    Name = "forward",
                    GUID = "Server",
                    Contents = JsonConvert.SerializeObject(message)
                }));
            }
            else
            {
                try
                {
                    Server.Program.server.DispatchTo(new Guid(message.GUID), new NetObject("forward", new ServerMessage
                    {
                        Name = "forward",
                        GUID = "Server",
                        Contents = JsonConvert.SerializeObject(message)
                    }));
                }
                catch
                {

                }
            }
        }
    

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
                        Contents = JsonConvert.SerializeObject(new
                        {
                            script = File.ReadAllText($"scripts/{user}/{script}.lua"),
                            args = sArgs
                        })
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
                        Contents = JsonConvert.SerializeObject(new MudException("<script_runner> Script not found or script error detected."))
                    }));
                }
                catch
                {
                    //fuck.
                }
            }

        }

        [MudRequest("diag_log", typeof(string))]
        public static void Diagnostic(string guid, string line)
        {
            List<string> lines = new List<string>();
            if (File.Exists("diagnostics.log"))
                lines = new List<string>(File.ReadAllLines("diagnostics.log"));

            lines.Add(line);
            File.WriteAllLines("diagnostics.log", lines.ToArray());
        }

        [MudRequest("getusers", typeof(string))]
        public static void GetAllUsers(string guid, string contents)
        {
            List<string> accs = new List<string>();
            if(contents == "dead")
            {
                foreach(var sve in Directory.GetFiles("deadsaves"))
                {
                    if (sve.EndsWith(".save"))
                    {
                        var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(sve));
                        accs.Add($"{save.Username}@{save.SystemName}");
                    }

                }
            }
            server.DispatchTo(new Guid(guid), new NetObject("h4xx0r", new ServerMessage
            {
                Name = "allusers",
                GUID = "server",
                Contents = JsonConvert.SerializeObject(accs)
            }));
        }

        [MudRequest("mud_save_allow_dead", typeof(Save))]
        public static void SaveDead(string guid, Save sve)
        {
            if(File.Exists("saves/" + sve.Username + ".save"))
            {
                WriteEncFile("saves/" + sve.Username + ".save", JsonConvert.SerializeObject(sve));
            }
            else if(File.Exists("deadsaves/" + sve.Username + ".save"))
            {
                File.WriteAllText("deadsaves/" + sve.Username + ".save", JsonConvert.SerializeObject(sve));
            }
        }

        [MudRequest("get_user_data", typeof(Dictionary<string, string>))]
        public static void GetUserData(string guid, Dictionary<string, string> contents)
        {
            string usr = contents["user"];
            string sys = contents["sysname"];

            foreach(var sve in Directory.GetFiles("deadsaves"))
            {
                if (sve.EndsWith(".save"))
                {
                    var saveFile = JsonConvert.DeserializeObject<Save>(File.ReadAllText(sve));
                    if(saveFile.Username == usr && saveFile.SystemName == sys)
                    {
                        server.DispatchTo(new Guid(guid), new NetObject("1337", new ServerMessage
                        {
                            Name = "user_data",
                            GUID = "server",
                            Contents = JsonConvert.SerializeObject(saveFile)
                        }));
                        return;
                    }
                }
            }
            foreach (var sve in Directory.GetFiles("saves"))
            {
                if (sve.EndsWith(".save"))
                {
                    var saveFile = JsonConvert.DeserializeObject<Save>(ReadEncFile(sve));
                    if (saveFile.Username == usr && saveFile.SystemName == sys)
                    {
                        server.DispatchTo(new Guid(guid), new NetObject("1337", new ServerMessage
                        {
                            Name = "user_data",
                            GUID = "server",
                            Contents = JsonConvert.SerializeObject(saveFile)
                        }));
                        return;
                    }
                }
            }

            server.DispatchTo(new Guid(guid), new NetObject("n07_50_1337", new ServerMessage
            {
                Name = "user_data_not_found",
                GUID = "server"
            }));

        }
    }
}
