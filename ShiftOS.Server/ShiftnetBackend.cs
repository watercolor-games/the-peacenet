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
        [MudRequest("download_start")]
        public static void StartDownload(string guid, object contents)
        {
            string url = contents as string;
            if (!url.StartsWith("shiftnet/"))
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

            if (File.Exists(url))
            {
                server.DispatchTo(new Guid(guid), new NetObject("download", new ServerMessage
                {
                    Name = "download_meta",
                    GUID = "server",
                    Contents = JsonConvert.SerializeObject(File.ReadAllBytes(url))
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

        [MudRequest("shiftnet_get")]
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
