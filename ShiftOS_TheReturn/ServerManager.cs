using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using NetSockets;
using System.Windows.Forms;
using System.Threading;
using Plex;
using static Plex.Engine.SaveSystem;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Net.NetworkInformation;
using Plex.Frontend.GraphicsSubsystem;
using System.Net;

namespace Plex.Engine
{
    public enum DisconnectType
    {
        UserRequested,
        Error,
        EngineShutdown
    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        public static SessionInfo SessionInfo { get; internal set; }

        [ClientMessageHandler("server_banned")]
        public static void BannedHandler(string content, string ip)
        {
            Disconnect(DisconnectType.Error, "You have been banned from this server and can't connect to it.");
        }

        [ClientMessageHandler("server_shutdown")]
        public static void MaintenanceHandler(string content, string ip)
        {
            Disconnect(DisconnectType.Error, "This server has been shut down by its administrator for maintenance.");
        }



        public static void Disconnect(DisconnectType type, string userMessage = "You have been disconnected from the server.")
        {
            UIManager.NetworkClient.Close();
            UIManager.ClearTopLevels();
            UIManager.Game.IPAddress = null;
            if (type == DisconnectType.UserRequested || type == DisconnectType.Error)
            {
                UIManager.Game.FireInitialized();
                if (type == DisconnectType.Error)
                {
                    Infobox.Show("Disconnected from server.", userMessage);
                }
            }
        }

        public static void SendMessage(string message, string contents)
        {
            string sessionKey = "";
            if (SessionInfo != null)
                sessionKey = SessionInfo.SessionID;
            var header = new PlexServerHeader
            {
                Content = contents,
                IPForwardedBy = "",
                Message = message,
                SessionID = sessionKey
            };
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            UIManager.NetworkClient.Send(data, data.Length);
        }

        internal static void HandleMessage(PlexServerHeader header)
        {
            if (!string.IsNullOrWhiteSpace(header.SessionID))
            {
                if (SessionInfo == null)
                    return;
                if (SessionInfo != null)
                    if (SessionInfo.SessionID != header.SessionID)
                        return;
            }

            foreach (var type in ReflectMan.Types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is ClientMessageHandlerAttribute) != null))
                {
                    var attribute = method.GetCustomAttributes(false).FirstOrDefault(x => x is ClientMessageHandlerAttribute) as ClientMessageHandlerAttribute;
                    if (attribute.ID == header.Message)
                    {
                        method.Invoke(null, new[] { header.Content, header.IPForwardedBy });
                    }
                }
            }
        }

        internal static void StartSessionManager(string host, int port)
        {
            var uconf = UserConfig.Get();
            var session = new SessionInfo();
            session.ServerIP = host;
            session.ServerPort = port;
            if (uconf.SessionCache.ContainsKey(host + ":" + port))
            {
                var key = uconf.SessionCache[host + ":" + port];
                session.SessionID = key;
            }
            else
            {
                session.SessionID = "";
            }
            SessionInfo = session;
            SendMessage("session_verify", session.SessionID);
        }

        internal static void StartSinglePlayer(string host, int port)
        {
            var session = new SessionInfo();
            session.ServerIP = host;
            session.ServerPort = port;
            SessionInfo = session;
            SendMessage("acct_get_key", session.SessionID);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ClientMessageHandlerAttribute : Attribute
    {
        public ClientMessageHandlerAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }
}