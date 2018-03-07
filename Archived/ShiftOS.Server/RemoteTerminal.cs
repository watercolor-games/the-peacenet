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
using Newtonsoft.Json;
using NetSockets;

namespace ShiftOS.Server
{
    public static class RemoteTerminal
    {
        [MudRequest("trm_handshake_accept", typeof(Dictionary<string, object>))]
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

        [MudRequest("trm_invcmd", typeof(Dictionary<string, object>))]
        public static void InvokeCommand(string guid, object contents)
        {
            Console.WriteLine("Before arg check");
            var args = contents as Dictionary<string, object>;
            if (args["guid"] != null && args["command"] != null)
            {
                Console.WriteLine("arg check finished");
                string cmd = args["command"] as string;
                string cGuid = args["guid"] as string;
                Console.WriteLine("Before dispatch");
                Program.ClientDispatcher.Server.DispatchTo(new Guid(cGuid), new NetObject("trminvoke", new ServerMessage
                {
                    Name = "trm_invokecommand",
                    GUID = "server",
                    Contents = cmd
                }));
                Console.WriteLine("After dispatch");
            }

        }

        [MudRequest("trm_handshake_request", typeof(Dictionary<string, object>))]
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

        [MudRequest("trm_handshake_stop", typeof(Dictionary<string, object>))]
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

        [MudRequest("write", typeof(Dictionary<string, object>))]
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
