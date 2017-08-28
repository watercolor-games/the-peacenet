using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Plex.Objects;
using Newtonsoft.Json;
using System.Reflection;
using Plex.Engine;

namespace Plex.Server
{
    public class Program
    {
        private static UdpClient _server = null;

        public static void SendMessage(PlexServerHeader header)
        {
            var ip = IPAddress.Parse(header.IPForwardedBy);
            int port = 62252;
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            _server.Send(bytes, bytes.Length, new IPEndPoint(ip, port));
        }

        public static void Broadcast(PlexServerHeader header)
        {
            var ip = IPAddress.Broadcast;
            int port = 62252;
            var data = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(data);
            _server.Send(bytes, bytes.Length, new IPEndPoint(ip, port));

        }

        public static void Main(string[] args)
        {
            _server = new UdpClient();
            var _ipEP = new IPEndPoint(IPAddress.Any, 62252);
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, 62252));
            _server.Client = sock;
            while (true)
            {
                _ipEP = new IPEndPoint(IPAddress.Any, 62252);
                var receive = _server.Receive(ref _ipEP);
                string data = Encoding.UTF8.GetString(receive);
                if(data == "heart")
                {
                    var beat = Encoding.UTF8.GetBytes("beat");
                    _server.Send(beat, beat.Length, new IPEndPoint(_ipEP.Address, _ipEP.Port));
                }
                else
                {
                    var header = JsonConvert.DeserializeObject<PlexServerHeader>(data);
                    ServerManager.HandleMessage(header);
                }
            }
        }
    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        internal static void HandleMessage(PlexServerHeader header)
        {
            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is ServerMessageHandlerAttribute) != null))
                {
                    var attribute = method.GetCustomAttributes(false).FirstOrDefault(x => x is ServerMessageHandlerAttribute) as ServerMessageHandlerAttribute;
                    if (attribute.ID == header.Message)
                    {
                        var sessionRequired = method.GetCustomAttributes(false).FirstOrDefault(x => x is SessionRequired) as SessionRequired;
                        if(sessionRequired != null)
                        {
                            bool nosession = string.IsNullOrWhiteSpace(header.SessionID);
                            if(nosession == false)
                            {
                                nosession = SessionManager.IsExpired(header.SessionID);
                            }

                            if (nosession)
                            {
                                Program.SendMessage(new PlexServerHeader
                                {
                                    IPForwardedBy = header.IPForwardedBy,
                                    Message = "login_required",
                                    Content = ""
                                });
                                return;
                            }
                        }
                        method.Invoke(null, new[] { header.SessionID, header.Content, header.IPForwardedBy });
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SessionRequired : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerMessageHandlerAttribute : Attribute
    {
        public ServerMessageHandlerAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }

}
