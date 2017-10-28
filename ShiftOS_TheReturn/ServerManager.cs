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
        private static Thread _bthread = null;
        private static PlexServerHeader _private = null;
        private static bool _connected = false;

        public static bool ConnectToServer(string hostname, int port)
        {
            if (_connected)
                return false;
            _connected = true;
            _tcpClient = new TcpClient();
            UIManager.ShowCloudUpload();
            UIManager.ShowCloudDownload();
            var task = _tcpClient.ConnectAsync(hostname, port);
            Stopwatch _sw = new Stopwatch();
            double _secondsWastedWaitingForShittyConnections = 0;
            while (!task.IsCompleted)
            {
                _sw.Start();
                UIManager.Game.RunOneFrame();
                _sw.Stop();
                _secondsWastedWaitingForShittyConnections += (double)_sw.Elapsed.TotalSeconds;
                _sw.Reset();
                if(_secondsWastedWaitingForShittyConnections>=4.0)
                {
                    _tcpClient = null;
                    _connected = false;
                    throw new Exception($"The connection to {hostname} on port {port} has timed out (Are you sure this is a Peacenet server?)");
                }
            }
            UIManager.HideCloudUpload();
            UIManager.HideCloudDownload();



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
            _bthread = new Thread(() =>
            {
                while (true)
                {
                    lock (_tcpReader)
                    {
                        bool readSessionKey = true;
                        string sessionKey = "";
                        string bstr = _tcpReader.ReadString();
                        if (bstr == "broadcast")
                            readSessionKey = false;
                        int _btype = _tcpReader.ReadInt32();
                        if (readSessionKey)
                            sessionKey = _tcpReader.ReadString();
                        int clength = _tcpReader.ReadInt32();
                        byte[] c = new byte[] { };
                        if (clength > 0)
                            c = _tcpReader.ReadBytes(clength);
                        var header = new PlexServerHeader
                        {
                            Content = c,
                            SessionID = sessionKey,
                            Message = (byte)_btype,
                            TransactionKey = bstr
                        };
                        if (readSessionKey)
                        {
                            if(header.Message == (byte)ServerResponseType.REQ_BANNED)
                            {
                                //Handle bans right now and here.
                                using(var reader = new BinaryReader(GetResponseStream(header)))
                                {
                                    DateTime banlift = DateTime.Parse(reader.ReadString());
                                    Disconnect(DisconnectType.Error, "You have been banned from this server until " + banlift.ToString() + ".");
                                }
                            }
                            _private = header;
                        }
                        else
                        {
                            _private = null;
                            HandleBroadcast(header);
                        }
                    }
                }
            });
            _bthread.Start();
            return true;
        }

        [BroadcastHandler(BroadcastType.SRV_ANNOUNCEMENT)]
        public static void HandleAnnouncement(PlexServerHeader header)
        {
            using(var reader = new BinaryReader(GetResponseStream(header)))
            {
                Infobox.Show(reader.ReadString(), reader.ReadString());
            }
        }

        internal static void HandleBroadcast(PlexServerHeader header)
        {
            BroadcastType btype = (BroadcastType)header.Message;
            if (_bdb.ContainsKey(btype))
                _bdb[btype].Invoke(null, new[] { header });
        }

        private static Dictionary<BroadcastType, MethodInfo> _bdb = new Dictionary<BroadcastType, MethodInfo>();

        internal static void BuildBroadcastHandlerDB()
        {
            foreach(var type in ReflectMan.Types)
            {
                foreach(var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var attr = mth.GetCustomAttributes(false).FirstOrDefault(x => x is BroadcastHandler) as BroadcastHandler;
                    if(attr != null)
                    {
                        var btype = attr.Type;
                        if (!_bdb.ContainsKey(btype))
                            _bdb.Add(btype, mth);
                    }
                }
            }
        }

        public static void Disconnect(DisconnectType type, string userMessage = "You have been disconnected from the server.")
        {
            _connected = false;
            _bthread.Abort();
            _tcpClient.Close();
            _tcpWriter.Close();
            _tcpReader.Close();
            _tcpClient = null;
            _tcpWriter = null;
            _tcpReader = null;
            _bthread = null;
            _actionQueue.Clear();
            SessionInfo = null;
            _thread.Abort();
            _thread = null;
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
        static uint msgtoken = 0;
        private static async Task<PlexServerHeader> _sendMessageInternal(ServerMessageType message, byte[] dgram)
        {
            _private = null;
            msgtoken++;
            string sessionKey = "";
            if (SessionInfo != null)
                sessionKey = SessionInfo.SessionID;
            if (sessionKey == null)
                sessionKey = "";
            await Task.Run(() =>
            {
                lock (_tcpWriter)
                {
                    _tcpWriter.Write(msgtoken.ToString());
                    _tcpWriter.Write((int)message);
                    _tcpWriter.Write(sessionKey);
                    if (dgram == null)
                        dgram = new byte[] { };
                    _tcpWriter.Write(dgram.Length);
                    if (dgram.Length > 0)
                        _tcpWriter.Write(dgram);
                }
            });

            while (_private == null)
                Thread.Sleep(1);
            while (_private.TransactionKey != msgtoken.ToString())
                Thread.Sleep(1);
            return _private;
        }

        public static PlexServerHeader SendMessage(ServerMessageType message, byte[] dgram)
        {
            bool runGame = Thread.CurrentThread.ManagedThreadId.ToString() == UIManager.Game.ThreadID;
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
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BroadcastHandler : Attribute
    {
        public BroadcastHandler(BroadcastType type)
        {
            Type = type;
        }

        public BroadcastType Type { get; private set; }
    }
}