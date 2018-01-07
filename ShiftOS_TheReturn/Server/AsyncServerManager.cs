#pragma warning disable CS4014 //FUCK OFF I WANT ASYNC CALLS

using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Config;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using Plex.Objects;
using System.Threading;
using Microsoft.Xna.Framework.Input;

namespace Plex.Engine.Server
{
    /// <summary>
    /// Provides an engine component for connecting to and talking with a Peacenet server.
    /// </summary>
    public class AsyncServerManager : IEngineComponent
    {


        [Dependency]
        private WatercolorAPIManager _api = null;

        [Dependency]
        private AppDataManager _appdata = null;

        private TcpClient _tcpClient = null;
        private BinaryReader _reader = null;
        private BinaryWriter _writer = null;
        private string _session = "";
        private Action<string> _onConnectionError;
        private bool _isMultiplayer = false;
        private PlexServerHeader _receivedHeader = null;
        private Queue<PlexBroadcast> _broadcasts = new Queue<PlexBroadcast>();
        private EventWaitHandle _messageReceived = new ManualResetEvent(false);


        private void _listen()
        {
            if (_tcpClient == null)
                return;
            while (_tcpClient.Connected)
            {
                try
                {
                    string muid = _reader.ReadString();
                    bool isBroadcast = (muid == "broadcast");
                    if (isBroadcast)
                    {
                        int restype = _reader.ReadInt32();
                        int btype = _reader.ReadInt32();
                        int len = _reader.ReadInt32();
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            data = _reader.ReadBytes(len);
                        }
                        _broadcasts.Enqueue(new PlexBroadcast((ServerBroadcastType)btype, data));
                    }
                    else
                    {
                        if (muid != _wantedMuid)
                            continue;
                        int remoteResponse = _reader.ReadInt32();
                        _session = _reader.ReadString();
                        int remoteLen = _reader.ReadInt32();
                        byte[] remoteBody = new byte[remoteLen];
                        if (remoteLen > 0)
                        {
                            remoteBody = _reader.ReadBytes(remoteLen);
                        }
                        _receivedHeader = new PlexServerHeader
                        {
                            Content = remoteBody,
                            Message = (byte)remoteResponse,
                            SessionID = null,
                            TransactionKey = null,
                        };
                        _messageReceived.Set();
                    }
                }
                catch
                {
                }
                if (_tcpClient == null)
                    return;
            }
        }

        /// <summary>
        /// Retrieves whether the engine is connected to a server.
        /// </summary>
        public bool Connected
        {
            get
            {
                return (_tcpClient == null) ? false : _tcpClient.Connected;
            }
        }

        /// <summary>
        /// Disconnects from a server.
        /// </summary>
        public void Disconnect()
        {
            _tcpClient?.Close();
            _reader.Close();
            _writer.Close();
            _broadcasts.Clear();
            _tcpClient = null;
        }

        [Dependency]
        private Plexgate _plexgate = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            var layer = new Layer();
            var entity = _plexgate.New<serverEntity>();
            layer.AddEntity(entity);
            _plexgate.AddLayer(layer);
        }

        private string _wantedMuid = null;

        /// <summary>
        /// Send a message to a server.
        /// </summary>
        /// <param name="type">The type of message to send.</param>
        /// <param name="body">The body of the message.</param>
        /// <param name="onResponse">A callback to run when the server responds.</param>
        /// <returns>A <see cref="Task"/> which may be awaited with <see langword="await"/> keyword or with <see cref="Task.Wait()"/>.</returns>
        public async Task SendMessage(ServerMessageType type, byte[] body, Action<ServerResponseType, BinaryReader> onResponse)
        {
            await Task.Run(() =>
            {
                string muid = Guid.NewGuid().ToString();
                _messageReceived.Reset();
                _wantedMuid = muid;
                lock (_writer)
                {
                    _writer.Write(muid);
                    _writer.Write((int)type);
                    _writer.Write(_session);
                    int len = 0;
                    if (body == null)
                        len = 0;
                    else
                        len = body.Length;
                    _writer.Write(len);
                    if (len > 0)
                        _writer.Write(body);
                    _writer.Flush();
                }
                _messageReceived.WaitOne();
                ServerResponseType response = (ServerResponseType)_receivedHeader.Message;
                if (response == ServerResponseType.REQ_LOGINREQUIRED)
                {
                    _onConnectionError?.Invoke("A Watercolor API error occurred on the server and you have lost connection as a result.");
                    Disconnect();
                }
                else
                {
                    if (_receivedHeader.Content.Length > 0)
                    {
                        using (var memstr = new MemoryStream(_receivedHeader.Content))
                        {
                            using (var memreader = new BinaryReader(memstr, Encoding.UTF8))
                            {
                                onResponse?.Invoke(response, memreader);
                            }
                        }
                    }
                    else
                    {
                        onResponse?.Invoke(response, null);
                    }
                }

            });
        }

        /// <summary>
        /// Retrieves whether the current server is a Multiplayer server.
        /// </summary>
        public bool IsMultiplayer
        {
            get
            {
                return _isMultiplayer;
            }
        }

        /// <summary>
        /// Connect to a Peacenet server.
        /// </summary>
        /// <param name="address">The address (including the port!) of the server to connect to.</param>
        /// <param name="onConnected">A callback action to be called when connection has succeeded.</param>
        /// <param name="onError">A callback action to be called any time there is a fatal connection error and the engine has disconnected from the server.</param>
        public void Connect(string address, Action onConnected, Action<string> onError)
        {
            Task.Run(() =>
            {
                try
                {
                    _onConnectionError = onError;
                    if (_tcpClient != null)
                        if (_tcpClient.Connected)
                            throw new InvalidOperationException("Cannot connect to server while an active connection is open!");
                    _session = _api.Token;
                    string[] sp = address.Split(':');
                    if (sp.Length != 2) throw new FormatException("The address string was not in the correct format (host:port)");
                    var lookup = Dns.GetHostEntry(sp[0]);
                    var first = lookup.AddressList.Last(); //irony
                    int port = -1;
                    if (!int.TryParse(sp[1], out port))
                        throw new FormatException("The port was not a valid integer.");
                    if (port < 0 || port > 65535)
                        throw new FormatException("Invalid port - must be between 0 and 65535.");
                    var endpoint = new IPEndPoint(first.MapToIPv4(), port);
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(endpoint);
                    var stream = _tcpClient.GetStream();
                    _reader = new BinaryReader(stream, Encoding.UTF8, true);
                    _writer = new BinaryWriter(stream, Encoding.UTF8, true);
                    var listener = new Thread(_listen);
                    listener.IsBackground = true;
                    listener.Start();
                    SendMessage(ServerMessageType.U_CONF, null, (res, reader) =>
                    {
                        _isMultiplayer = reader.ReadBoolean();
                        if (_api.LoggedIn == false)
                            if (_isMultiplayer)
                            {
                                Disconnect();
                                onError?.Invoke("Cannot connect to a multiplayer server without a Watercolor account.");
                                return;
                            }
                        onConnected?.Invoke();
                    });
                }
                catch (Exception ex)
                {
                    Disconnect();
                    onError?.Invoke(ex.Message);
                }
            });

        }

        private class serverEntity : IEntity
        {
            [Dependency]
            private AsyncServerManager _server = null;
            public void Draw(GameTime time, GraphicsContext gfx)
            {
            }

            public void OnKeyEvent(KeyboardEventArgs e)
            {
            }

            public void OnMouseUpdate(MouseState mouse)
            {
            }

            public void Update(GameTime time)
            {
                if (_server.Connected)
                {
                    while (_server._broadcasts.Count > 0)
                    {
                        var broadcast = _server._broadcasts.Dequeue();
                        if (broadcast.Type == ServerBroadcastType.Shutdown)
                        {
                            using (var reader = broadcast.OpenStream())
                            {
                                string msg = reader.ReadString();
                                _server.Disconnect();
                                _server._onConnectionError?.Invoke($@"The server has been shut down by an administrator.

{msg}");
                                _server._broadcasts.Clear();
                            }
                        }
                        else
                        {
                            _server.BroadcastReceived?.Invoke(broadcast.Type, broadcast.OpenStream());
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Occurs when a broadcasted message is received from the server.
        /// </summary>
        public event Action<ServerBroadcastType, BinaryReader> BroadcastReceived;
    }
}
