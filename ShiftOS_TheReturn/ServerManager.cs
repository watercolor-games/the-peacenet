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
            _tcpReader = new BinaryReader(_tcpStream, Encoding.UTF8, true);
            _tcpWriter = new DebugBinaryWriter(_tcpStream, Encoding.UTF8, true);
            _thread = new Thread(() =>
            {
                while (true)
                {
                    while (_actionQueue.Count > 0)
                        _actionQueue.Dequeue()?.Invoke();
                    Thread.Sleep(1);
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

        private static async Task<PlexServerHeader> _sendMessageInternal(ServerMessageType message, byte[] dgram)
        {
            string sessionKey = "";
            if (SessionInfo != null)
                sessionKey = SessionInfo.SessionID;
            if (sessionKey == null)
                sessionKey = "";
            _tcpWriter.Write((int)message);
            _tcpWriter.Write(sessionKey);
            await Task.Run(() =>
            {
                if (dgram == null)
                    dgram = new byte[] { };
                _tcpWriter.Write(dgram.Length);
                if (dgram.Length > 0)
                    _tcpWriter.Write(dgram);
            });
            
            byte returncode = (byte)_tcpReader.ReadInt32();
            
            string _returnsessionid = _tcpReader.ReadString();
            byte[] retdgram = new byte[] { };
            int len = _tcpReader.ReadInt32();
            if (len > 0)
                retdgram = _tcpReader.ReadBytes(len);
            return new PlexServerHeader
            {
                Message = returncode,
                SessionID = _returnsessionid,
                Content = retdgram
            };

        }

        public static PlexServerHeader SendMessage(ServerMessageType message, byte[] dgram)
        {
            PlexServerHeader header = null;
            _actionQueue.Enqueue(() =>
            {
                var task = _sendMessageInternal(message, dgram);
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
            
            return header;
        }

        public static MemoryStream GetResponseStream(PlexServerHeader header)
        {
            if (header.Content.Length == 0)
                throw new InvalidOperationException("No body in the message, can't read.");
            return new MemoryStream(header.Content);
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
            using (var sstr = new ServerStream(ServerMessageType.USR_VALIDATEKEY))
            {
                sstr.Write(SessionInfo.SessionID);
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(GetResponseStream(result)))
                    {
                        UIManager.ClearTopLevels();
                        ServerManager.SessionInfo.SessionID = reader.ReadString();
                        SaveSystem.Begin();
                    }

                }
                else
                {
                    ServerLoginHandler.LoginRequired();
                }

            }
        }

        internal static void StartSinglePlayer(string host, int port)
        {
            var session = new SessionInfo();
            session.ServerIP = host;
            session.ServerPort = port;
            SessionInfo = session;
            using (var sstr = new ServerStream(ServerMessageType.USR_LOGIN))
            {
                sstr.Write("Anal");
                sstr.Write("bacteria");
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        UIManager.ClearTopLevels();
                        ServerManager.SessionInfo.SessionID = reader.ReadString();
                        SaveSystem.Begin();
                    }

                }
                else
                {
                    ServerLoginHandler.AccessDenied();
                }
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

    public class ServerStream : BinaryWriter
    {
        private ServerMessageType _type;

        public ServerStream(ServerMessageType type) : base(new MemoryStream())
        {
            _type = type;
        }

        public PlexServerHeader Send()
        {
            return ServerManager.SendMessage(_type, (BaseStream as MemoryStream).ToArray());
        }
    }
}