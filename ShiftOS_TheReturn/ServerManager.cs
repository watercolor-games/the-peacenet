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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AsyncExecutionAttribute : Attribute
    {

    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        public static SessionInfo SessionInfo { get; internal set; }
        private static TcpClient _tcpClient = new TcpClient();
        private static BinaryWriter _tcpWriter = null;
        private static BinaryReader _tcpReader = null;
        private static Stream _tcpStream = null;
        private static Queue<Action> _actionQueue = new Queue<Action>();
        private static Thread _thread = null;

        public static void ConnectToServer(string hostname, int port)
        {
            _tcpClient = new TcpClient(hostname, port);
            _tcpStream = _tcpClient.GetStream();
            _tcpReader = new BinaryReader(_tcpStream);
            _tcpWriter = new BinaryWriter(_tcpStream);
            _thread = new Thread(() =>
            {
                while (true)
                {
                    while (_actionQueue.Count == 0)
                        Thread.Sleep(1);
                    _actionQueue.Dequeue()?.Invoke();
                }
            });
            _thread.Start();
        }

        [ClientMessageHandler("server_error")]
        public static void ErrorHandler(string content, string ip)
        {
            Disconnect(DisconnectType.Error, "The server has experienced an unexpected error with that request. You have been disconnected to prevent further errors.");
        }


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

        private static async Task<PlexServerHeader> _sendMessageInternal(ServerMessageType message, Action<BinaryWriter> contentWriter)
        {
            string sessionKey = "";
            if (SessionInfo != null)
                sessionKey = SessionInfo.SessionID;
            if (sessionKey == null)
                sessionKey = "";
            _tcpWriter.Write((int)message);
            _tcpWriter.Write(sessionKey);
            await Task.Run(() => contentWriter?.Invoke(_tcpWriter));
            Thread.Sleep(6);
            byte returncode = (byte)_tcpReader.ReadInt32();
            Thread.Sleep(6);
            string _returnsessionid = _tcpReader.ReadString();
            return new PlexServerHeader
            {
                Message = returncode,
                SessionID = _returnsessionid
            };

        }

        public static PlexServerHeader SendMessage(ServerMessageType message, Action<BinaryWriter> contentWriter, out BinaryReader reader)
        {
            PlexServerHeader header = null;
            _actionQueue.Enqueue(() =>
            {
                var task = _sendMessageInternal(message, contentWriter);
                task.Wait();
                header = task.Result;
            });
            while (header == null)
            {
                Thread.Sleep(1);
            }
            if(header.Message == (byte)ServerResponseType.REQ_LOGINREQUIRED)
            {
                ServerLoginHandler.LoginRequired();
                header.Message = 0x00;
            }
            reader = _tcpReader;
            return header;
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
            BinaryReader _r = null;
            if(SendMessage(ServerMessageType.USR_VALIDATEKEY, (w) =>
            {
                w.Write(session.SessionID);
            }, out _r).Message == (byte)ServerResponseType.REQ_SUCCESS)
            {
                UIManager.ClearTopLevels();
                ServerManager.SessionInfo.SessionID = _r.ReadString();
                SaveSystem.Begin();

            }
            else
            {
                ServerLoginHandler.LoginRequired();
            }
        }

        internal static void StartSinglePlayer(string host, int port)
        {
            var session = new SessionInfo();
            session.ServerIP = host;
            session.ServerPort = port;
            SessionInfo = session;
            BinaryReader _r = null;
            if (ServerManager.SendMessage(ServerMessageType.USR_LOGIN, (w) =>
            {
                w.Write("You");
                w.Write("suck");
            }, out _r).Message != (byte)ServerResponseType.REQ_SUCCESS)
            {
                ServerLoginHandler.AccessDenied();
            }
            else
            {
                UIManager.ClearTopLevels();
                ServerManager.SessionInfo.SessionID = _r.ReadString();
                SaveSystem.Begin();

            }
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