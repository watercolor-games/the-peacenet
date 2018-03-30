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
using Plex.Engine;
using System.Collections.Concurrent;

namespace Peacenet.Server
{
    /// <summary>
    /// Provides an engine component for connecting to and talking with a Peacenet server.
    /// </summary>
    public class AsyncServerManager : IEngineComponent
    {
        private List<SavedServer> _savedServers = new List<SavedServer>();

        [Dependency]
        private ItchOAuthClient _api = null;

        [Dependency]
        private AppDataManager _appdata = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        private TcpClient _tcpClient = null;
        private BinaryReader _reader = null;
        private BinaryWriter _writer = null;
        private string _session = "";
        private Action<string> _onConnectionError;
        private bool _isMultiplayer = false;
        private Queue<PlexBroadcast> _broadcasts = new Queue<PlexBroadcast>();
        private EventWaitHandle _messageReceived = new AutoResetEvent(false);

        ConcurrentDictionary<string, PlexServerHeader> _responses = null;


        private void _listen()
        {
            if (_tcpClient == null)
                return;
            while (_tcpClient.Connected)
            {
                try
                {
                    Logger.Log("Receiving message from server...");
                    string muid = _reader.ReadString();
                    bool isBroadcast = (muid == "broadcast");
                    Logger.Log("Got message ID");
                    if (isBroadcast)
                    {
                        int restype = _reader.ReadInt32();
                        Logger.Log("Got message result type");
                        int btype = _reader.ReadInt32();
                        Logger.Log("Got broadcast code");
                        int len = _reader.ReadInt32();
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            data = _reader.ReadBytes(len);
                        }
                        Logger.Log("Broadcast body read.");
                        _broadcasts.Enqueue(new PlexBroadcast((ServerBroadcastType)btype, data));
                    }
                    else
                    {
                        if (!_responses.ContainsKey(muid))
                        {
                            Logger.Log("Skipping read of direct reply. It's not for us.");
                            continue;
                        }
                        int remoteResponse = _reader.ReadInt32();
                        _session = _reader.ReadString();
                        int remoteLen = _reader.ReadInt32();
                        byte[] remoteBody = new byte[remoteLen];
                        if (remoteLen > 0)
                        {
                            remoteBody = _reader.ReadBytes(remoteLen);
                        }
                        _responses[muid] = new PlexServerHeader
                        {
                            Content = remoteBody,
                            Message = (byte)remoteResponse,
                            SessionID = null,
                            TransactionKey = null,
                        };
                        _messageReceived.Set();
                    }
                    if (_tcpClient == null)
                        return;
                }
                catch(Exception ex)
                {
                    Logger.Log("Breaking out of the listener loop - we can't read anymore.", LogType.Warning, "peacenetserver");
                    Logger.Log(ex.ToString(), LogType.Error, "peacenetserver");
                    if(_tcpClient!=null)
                    {
                        if(Connected)
                        {
                            Disconnect(ex.GetType().FullName + ": " + ex.Message);
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Retrieves whether the engine is connected to a server.
        /// </summary>
        public bool Connected
        {
            get
            {
                try
                {
                    return (_tcpClient == null) ? false : _tcpClient.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Disconnects from a server.
        /// </summary>
        public void Disconnect(string error = null)
        {
            _tcpClient?.Close();
            _reader.Close();
            _writer.Close();
            _broadcasts.Clear();
            _tcpClient = null;
            _responses = null;
            if(!string.IsNullOrWhiteSpace(error))
            {
                Logger.Log("Disconnected from server: " + error, LogType.Warning, "peacenetserver");
                _infobox.Show("Disconnected from server.", error);
            }
        }

        [Dependency]
        private Plexgate _plexgate = null;

        

        /// <inheritdoc/>
        public void Initiate()
        {
            _responses = new ConcurrentDictionary<string, PlexServerHeader>();
            var entity = _plexgate.New<serverEntity>();
            _plexgate.GetLayer(LayerType.NoDraw).AddEntity(entity);
            _savedServers = new List<SavedServer>();
            if(File.Exists(Path.Combine(_appdata.GamePath, "servers.json")))
            {
                _savedServers = JsonConvert.DeserializeObject<List<SavedServer>>(File.ReadAllText(Path.Combine(_appdata.GamePath, "servers.json")));
            }
        }

        public SavedServer[] SavedServers
        {
            get
            {
                return _savedServers.ToArray();
            }
        }

        public void AddServer(SavedServer server)
        {
            if (server == null)
                return;
            if (_savedServers.Contains(server))
                return;
            _savedServers.Add(server);
            _writeServers();
        }

        public void RemoveServer(SavedServer server)
        {
            if (server == null)
                return;
            if (!_savedServers.Contains(server))
                return;
            _savedServers.Remove(server);
            _writeServers();
        }

        private void _writeServers()
        {
            File.WriteAllText(Path.Combine(_appdata.GamePath, "servers.json"), JsonConvert.SerializeObject(_savedServers, Formatting.Indented));
        }

        public void ClearServers()
        {
            _savedServers.Clear();
            _writeServers();
        }

        private EventWaitHandle _messageHandled = new ManualResetEvent(true);

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
                _messageHandled.WaitOne();
                _messageHandled.Reset();
                Logger.Log($"Sending message to server: {type} (body is {body?.Length} bytes long)");
                string muid = Guid.NewGuid().ToString();
                _responses[muid] = null;
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
                _messageHandled.Set();
                PlexServerHeader hdr;
                do
                {
                    _messageReceived.WaitOne();
                    hdr = _responses[muid];
                }
                while (hdr == null);
                var response = (ServerResponseType)hdr.Message;
                if (response == ServerResponseType.REQ_LOGINREQUIRED)
                {
                    Disconnect("You must be signed into an itch.io account to play Multiplayer.");
                }
                else
                {
                    if (hdr.Content.Length > 0)
                    {
                        using (var memstr = new MemoryStream(hdr.Content))
                        {
                            using (var memreader = new BinaryReader(memstr, Encoding.UTF8))
                            {
                                onResponse?.Invoke(response, memreader);
                            }
                        }
                    }
                    else
                    {
                        onResponse?.Invoke(response, null); // what?
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

        public async Task<PeacenetConnectionResult> Connect(string address)
        {
            try
            {
                if (_tcpClient != null)
                    if (_tcpClient.Connected)
                        return new PeacenetConnectionResult(ConnectionResultType.AlreadyConnected);

                _session = _api.Token;

                Logger.Log("Attempting connection to " + address + "...", LogType.Info, "peacenetserver");

                IPEndPoint endpoint = null;
                var endpointResult = GetEndPoint(address, 3251, out endpoint);

                if (endpointResult != EndPointResult.Success)
                {
                    switch (endpointResult)
                    {
                        case EndPointResult.BadPort:
                            Logger.Log("Invalid port.", LogType.Error, "peacenetserver");
                            return new PeacenetConnectionResult(ConnectionResultType.Other, new ArgumentOutOfRangeException($"The requested port is outside the range of valid ports ({IPEndPoint.MinPort} - {IPEndPoint.MaxPort})."));
                        case EndPointResult.DNSLookupError:
                            Logger.Log("DNS lookup error.", LogType.Error, "peacenetserver");
                            return new PeacenetConnectionResult(ConnectionResultType.Other, new Exception("A connection couldn't be established because the specified hostname does not point to a valid IP address."));
                        case EndPointResult.InvalidIPAddress:
                            Logger.Log("Invalid IP address.", LogType.Error, "peacenetserver");
                            return new PeacenetConnectionResult(ConnectionResultType.Other, new Exception("A connection couldn't be established because the specified IP address is not valid."));

                    }
                }

                _tcpClient = new TcpClient();

                var result = _tcpClient.BeginConnect(endpoint.Address, endpoint.Port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(10000);

                if (!success)
                {
                    Logger.Log("Connection timed out.", LogType.Error, "peacenetserver");
                    return new PeacenetConnectionResult(ConnectionResultType.ConnectionTimeout);
                }
                _tcpClient.EndConnect(result);

                Logger.Log("Sending handshake message to retrieve server type...", LogType.Info, "peacenetserver");

                var stream = _tcpClient.GetStream();
                _reader = new BinaryReader(stream, Encoding.UTF8, true);
                _writer = new BinaryWriter(stream, Encoding.UTF8, true);
                var listener = new Thread(_listen);
                listener.IsBackground = true;
                listener.Start();
                await SendMessage(ServerMessageType.U_CONF, null, (res, reader) =>
                {
                    Logger.Log("Handshake received.", LogType.Info, "peacenetserver");
                    _isMultiplayer = reader.ReadBoolean();
                });

                Logger.Log("Is Multiplayer: " + _isMultiplayer, LogType.Info, "peacenetserver");

                if (_isMultiplayer)
                {
                    if (!_api.LoggedIn)
                    {
                        Logger.Log("Not signed into itch.io - disconnecting from server...", LogType.Error, "peacenetserver");
                        Disconnect();
                        return new PeacenetConnectionResult(ConnectionResultType.BadItchAuth);
                    }
                }

                return new PeacenetConnectionResult(ConnectionResultType.Success);
            }
            catch (Exception ex)
            {
                return new PeacenetConnectionResult(ConnectionResultType.Other, ex);
            }
        }

        /// <summary>
        /// Connect to a Peacenet server.
        /// </summary>
        /// <param name="address">The address (including the port!) of the server to connect to.</param>
        /// <param name="onConnected">A callback action to be called when connection has succeeded.</param>
        /// <param name="onError">A callback action to be called any time there is a fatal connection error and the engine has disconnected from the server.</param>
        [Obsolete("This method's kinda fucked and hacked together. Please use the async Connect() method instead.")]
        public void ConnectOld(string address, Action onConnected, Action<string> onError)
        {
            Task.Run(() =>
            {
                try
                {
                    Logger.Log($"Attempting connection to {address}...", LogType.Info, "peacenetserver");
                    _onConnectionError = onError;
                    if (_tcpClient != null)
                        if (_tcpClient.Connected)
                            throw new InvalidOperationException("Cannot connect to server while an active connection is open!");
                    Logger.Log("Retrieving itch.io API key...", LogType.Info, "peacenetserver");
                    _session = _api.Token;
                    Logger.Log("Parsing server address: " + address, LogType.Info, "peacenetserver");
                    string[] sp = address.Split(':');
                    if (sp.Length != 2) throw new FormatException("The address string was not in the correct format (host:port)");
                    Logger.Log("Performing DNS resolution for " + sp[0] + "...", LogType.Info, "peacenetserver");
                    var lookup = Dns.GetHostEntry(sp[0]);
                    var first = lookup.AddressList.Last(); //irony
                    Logger.Log($"DNS lookup complete. Connecting to {first.MapToIPv4().ToString()}...");
                    int port = -1;
                    if (!int.TryParse(sp[1], out port))
                        throw new FormatException("The port was not a valid integer.");
                    if (port < 0 || port > 65535)
                        throw new FormatException("Invalid port - must be between 0 and 65535.");
                    var endpoint = new IPEndPoint(first.MapToIPv4(), port);
                    _tcpClient = new TcpClient();
                    _tcpClient.ReceiveTimeout = 20000;
                    _tcpClient.SendTimeout = 20000;
                    var result = _tcpClient.BeginConnect(endpoint.Address, endpoint.Port, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10));

                    if (!success)
                    {
                        throw new TimeoutException();
                    }

                    // we have connected
                    _tcpClient.EndConnect(result);
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
                                Logger.Log("Attempted to connect to remote server without itch.io authentication, not possible.", LogType.Error, "peacenetserver");
                                Disconnect();
                                onError?.Invoke("Cannot connect to a multiplayer server without an itch.io account.");
                                return;
                            }
                        Logger.Log("Connection successful", LogType.Info, "peacenetserver");
                        onConnected?.Invoke();
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log($"Connection error: {ex}", LogType.Error, "peacenetserver");
                    Disconnect();
                    onError?.Invoke(ex.Message);
                }
            });

        }

        private class serverEntity : IEntity
        {
            /// <inheritdoc/>
            public void OnGameExit()
            {

            }

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
                                _server.Disconnect("The server has been shut down by an administrator.\n\n" + msg);
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

        private EndPointResult GetEndPoint(string address, int defaultPort, out IPEndPoint ep)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                ep = null;
                return EndPointResult.InvalidIPAddress;
            }

            IPAddress ip = null;
            int port = -1;

            string[] addressSegments = address.Split(':');

            if(addressSegments.Length <= 2)
            {
                if (!IPAddress.TryParse(addressSegments[0], out ip))
                {
                    var dns = DNSLookup(addressSegments[0]);
                    if (dns == null)
                    {
                        ep = null;
                        return EndPointResult.DNSLookupError;
                    }
                    ip = dns;
                }

                if(addressSegments.Length == 1)
                {
                    port = defaultPort;
                }
                else
                {
                    port = GetPort(addressSegments[1]);
                }
            }
            else if(addressSegments.Length > 2)
            {
                if(addressSegments[0].StartsWith("[") && addressSegments[addressSegments.Length-2].EndsWith("]"))
                {
                    string addressString = string.Join(":", addressSegments.Take(addressSegments.Length - 1).ToArray());
                    IPAddress.TryParse(addressString, out ip); //we use TryParse because it'll set ip to null if it's a bad ip and I hate code duplication
                    port = GetPort(addressSegments[addressSegments.Length - 1]);
                }
                else
                {
                    string addressString = string.Join(":", addressSegments);
                    IPAddress.TryParse(addressString, out ip); //we use TryParse because it'll set ip to null if it's a bad ip and I hate code duplication
                    port = defaultPort;
                }
            }

            if (ip == null)
            {
                ep = null;
                return EndPointResult.InvalidIPAddress;
            }
            if (port == -1 || (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort))
            {
                ep = null;
                return EndPointResult.BadPort;
            }

            ep = new IPEndPoint(ip.MapToIPv4(), port);
            return EndPointResult.Success;
        }

        private IPAddress DNSLookup(string hostname)
        {
            Logger.Log("Performing DNS lookup for " + hostname + "...", LogType.Info, "peacenetserver");

            var hosts = Dns.GetHostAddresses(hostname);

            if(hosts == null || hosts.Length==0)
            {
                Logger.Log("DNS lookup failed.", LogType.Error, "peacenetserver");
                return null;
            }

            Logger.Log($"Connecting to {hosts.Last().ToString()}...", LogType.Info, "peacenetserver");
            return hosts.Last();
        }

        private int GetPort(string portString)
        {
            int port = -1;
            if(!int.TryParse(portString, out port) || (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort))
            {
                Logger.Log("Invalid port: " + portString, LogType.Error, "peacenetserver");
                return -1;
            }
            return port;
        }


        /// <summary>
        /// Occurs when a broadcasted message is received from the server.
        /// </summary>
        public event Action<ServerBroadcastType, BinaryReader> BroadcastReceived;
    }

    public class SavedServer
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public enum EndPointResult
    {
        Success,
        BadPort,
        InvalidIPAddress,
        DNSLookupError
    }

    public enum ConnectionResultType
    {
        Success,
        BadItchAuth,
        ConnectionTimeout,
        AlreadyConnected,
        Other
    }

    public class PeacenetConnectionResult
    {
        public ConnectionResultType Result { get; private set; }
        public Exception Exception { get; private set; }

        public PeacenetConnectionResult(ConnectionResultType type, Exception ex = null)
        {
            Result = type;
            Exception = ex;
        }
    }
}
