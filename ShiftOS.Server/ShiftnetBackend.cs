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
    public static class ShiftnetBackend
    {
        [MudRequest("download_start", typeof(string))]
        public static void StartDownload(string guid, object contents)
        {
            string url = contents as string;
            if (!url.StartsWith("shiftnet/"))
            {
                try
                {

                    server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                    {
                        Name = "shiftnet_file",
                        GUID = "server",
                        Contents = (File.Exists("badrequest.md") == true) ? File.ReadAllText("badrequest.md") : @"# Bad request.

You have sent a bad request to the multi-user domain. Please try again."
                    }));
                }
                catch { }
                return;
            }

            if (File.Exists(url))
            {
                try
                {
                    server.DispatchTo(new Guid(guid), new NetObject("download", new ServerMessage
                    {
                        Name = "download_meta",
                        GUID = "server",
                        Contents = JsonConvert.SerializeObject(File.ReadAllBytes(url))
                    }));
                }
                catch { }
            }
            else
            {
                try
                {
                    server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                    {
                        Name = "shiftnet_file",
                        GUID = "server",
                        Contents = (File.Exists("notfound.md") == true) ? File.ReadAllText("notfound.md") : @"# Not found.

The page you requested at was not found on this multi-user domain."
                    }));
                }
                catch { }
            }

        }

        [MudRequest("shiftnet_get", typeof(Dictionary<string, object>))]
        public static void GetPage(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            string surl = args["url"] as string;
            while (surl.EndsWith("/"))
            {
                surl = surl.Remove(surl.Length - 1, 1);
            }
            if (!surl.StartsWith("shiftnet/"))
            {
                server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                {
                    Name = "shiftnet_file",
                    GUID = "server",
                    Contents = (File.Exists("badrequest.md") == true) ? File.ReadAllText("badrequest.md") : @"# Bad request.

You have sent a bad request to the multi-user domain. Please try again."
                }));

                return;
            }

            if (File.Exists(surl))
            {
                server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                {
                    Name = "shiftnet_file",
                    GUID = "server",
                    Contents = File.ReadAllText(surl)
                }));
            }
            else if (File.Exists(surl + "/home.rnp"))
            {
                server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                {
                    Name = "shiftnet_file",
                    GUID = "server",
                    Contents = File.ReadAllText(surl + "/home.rnp")
                }));

            }
            else
            {
                server.DispatchTo(new Guid(guid), new NetObject("shiftnet_got", new ServerMessage
                {
                    Name = "shiftnet_file",
                    GUID = "server",
                    Contents = (File.Exists("notfound.md") == true) ? File.ReadAllText("notfound.md") : @"# Not found.

The page you requested at was not found on this multi-user domain."
                }));

            }

        }
    }
}
