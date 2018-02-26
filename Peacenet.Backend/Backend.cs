using System;
using Plex.Objects;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Peacenet.Backend
{
    /// <summary>
    /// Encapsulates a Peacenet server.
    /// </summary>
    public class Backend : IDisposable
    {
        private List<IMessageHandler> _handlers = null;
        private List<ComponentInfo> _components = null;
        private int _port = 0;
        private Thread _utilityThread = null;
        private Queue<Action> _utilityActions = new Queue<Action>();
        private volatile bool _isRunning = false;
        private const int _waittimems = 1800000;
        private TcpListener _listener = null;
        private Thread _tcpthread = null;
        private bool _isMultiplayer = false;
        private Dictionary<string, ItchUser> _users = new Dictionary<string, ItchUser>();
        private Dictionary<string, string> _keys = new Dictionary<string, string>();

        private List<TcpClient> _connected = new List<TcpClient>();

        private Dictionary<string, TcpClient> _playerIds = new Dictionary<string, TcpClient>();

        /// <summary>
        /// Broadcast a message to all clients.
        /// </summary>
        /// <param name="type">The type of message</param>
        /// <param name="body">The body of the message.</param>
        public void Broadcast(ServerBroadcastType type, byte[] body)
        {
            if (body == null)
                body = new byte[0];
            lock (_connected)
            {
                foreach(var client in _connected)
                {
                    var stream = client.GetStream();

                    using(var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    {
                        writer.Write("broadcast");
                        writer.Write((int)ServerResponseType.REQ_SUCCESS);
                        writer.Write((int)type);
                        writer.Write(body.Length);
                        if (body.Length > 0)
                            writer.Write(body);
                        writer.Flush();
                    }
                }
            }
        }

        internal void BroadcastToPlayer(ServerBroadcastType message, byte[] body, string playerId)
        {
            if (playerId == null)
                return;
            lock (_connected)
            {
                if (body == null)
                    body = new byte[0];
                if (!_playerIds.ContainsKey(playerId))
                    return;
                var client = _playerIds[playerId];
                var stream = client.GetStream();

                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.Write("broadcast");
                    writer.Write((int)ServerResponseType.REQ_SUCCESS);
                    writer.Write((int)message);
                    writer.Write(body.Length);
                    if (body.Length > 0)
                        writer.Write(body);
                    writer.Flush();
                }

            }
        }

        private string _rootDirectory = null;

        /// <summary>
        /// Retrieves the root directory of the server where server data may be stored.
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return _rootDirectory;
            }
        }

        private EventWaitHandle _serverReady = new ManualResetEvent(false);

        /// <summary>
        /// Retrieves an <see cref="EventWaitHandle"/> which can be used to determine when the server is ready to be connected to. 
        /// </summary>
        public EventWaitHandle ServerReady
        {
            get
            {
                return _serverReady;
            }
        }

        // Fired on any of these events:
        // 1) A new utility action has been enqueued
        // 2) The timer has expired - it's time to call SafetyCheck()
        // 3) It's time to shut down
        private EventWaitHandle _workForUtility = new ManualResetEvent(false);

        private Timer _safetyWatch = null;

        private Timer _wgTimer = null;

        // Set to true before the timer fires.
        private volatile bool _safety = false;

        // Fired by the utility thread to let the main thread know that
        // it's done shutting down.
        private EventWaitHandle _shutdownComplete = new AutoResetEvent(false);


        /// <summary>
        /// Retrieves whether this is a Multiplayer server.
        /// </summary>
        public bool IsMultiplayer
        {
            get
            {
                return _isMultiplayer;
            }
        }

        private void EnqueueUtilityAction(Action action)
        {
            lock (_utilityActions)
            {
                _utilityActions.Enqueue(action);
            }
            _workForUtility.Set();
        }

        /// <summary>
        /// Retrieves the profile data of an itch.io user.
        /// </summary>
        /// <param name="id">The ID of the itch.io user</param>
        /// <returns>A <see cref="ItchUser"/> representing the cached profile data for this user. For security purposes, this method returns null if no user with the specified id is connected to the server</returns>
        public ItchUser GetUserInfo(string id)
        {
            if (_users.ContainsKey(id))
                return _users[id];
            return null;
        }

        public event Action<string, ItchUser> PlayerJoined;

        /// <summary>
        /// Creates a new instance of the Peacenet server.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="isMultiplayer">Whether this is a multiplayer server</param>
        /// <param name="rootDir">The root directory for server data.</param>
        public Backend(int port, bool isMultiplayer = true, string rootDir = null)
        {
            _rootDirectory = rootDir;
            if (string.IsNullOrWhiteSpace(_rootDirectory))
                _rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(_rootDirectory))
                Directory.CreateDirectory(_rootDirectory);
            _isMultiplayer = isMultiplayer;
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));
            _port = port;
            Logger.Log("Initiating Peacenet backend...");
            _components = new List<ComponentInfo>();
            Logger.Log("Probing for backend components...");
             foreach (var type in ReflectMan.Types)
             {
                 if (type.GetInterfaces().Contains(typeof(IBackendComponent)))
                 {
                    if (type.GetConstructor(Type.EmptyTypes) == null)
                    {
                        Logger.Log($"Found {type.Name}, but it doesn't have a parameterless constructor, so it's ignored.  Probably a mistake.");
                        continue;
                    }
                    Logger.Log($"Found {type.Name}.");
                    var component = (IBackendComponent)Activator.CreateInstance(type, null);
                    _components.Add(new Peacenet.Backend.ComponentInfo
                    {
                        Initialized = false,
                        Component = component
                    });
                 }
             }
            Logger.Log("Initiating all backend components...");
            foreach (var component in _components.ToArray())
            {
                Inject(component.Component);
            }

            foreach (var component in _components.ToArray())
            {
                RecursiveInit(component.Component);
            }


            Logger.Log("Utility thread creating!");
            _utilityThread = new Thread(this.UtilityThread);
            _utilityThread.IsBackground = true;

            _tcpthread = new Thread(ListenThread);
            _tcpthread.IsBackground = true;

            if(!_isMultiplayer)
            {
                _keys.Add("__SINGLEPLAYER", "-1");
                _users.Add("-1", new ItchUser
                {
                    cover_url = null,
                    developer = false,
                    display_name = "Player",
                    gamer = true,
                    id = -1,
                    press_user = false,
                    url = null,
                    username = "player"
                });
            }

            Logger.Log("Message delegator is loading...");
            _handlers = new List<IMessageHandler>();
            Logger.Log("Finding all handler objects...");
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IMessageHandler)))
                {
                    var handler = (IMessageHandler)Activator.CreateInstance(type, null);
                    Logger.Log($"Found handler: {type.Name} (for protocol message type {handler.HandledMessageType})");
                    if (_handlers.FirstOrDefault(x => x.HandledMessageType == handler.HandledMessageType) != null)
                    {
                        Logger.Log($"WARNING: Another handler handles the same message type as {handler.GetType().Name}! Ignoring it.");
                        continue;
                    }
                    _handlers.Add(handler);
                    Inject(handler);
                }
            }
            Logger.Log($"Done loading handlers. {_handlers.Count} found.");

        }

        /// <summary>
        /// Handles an incoming server message.
        /// </summary>
        /// <param name="messagetype">The type of the message</param>
        /// <param name="session_id">The caller user ID</param>
        /// <param name="dgram">The message body</param>
        /// <param name="returndgram">The returned message body</param>
        /// <returns>The result of the handler.</returns>
        public ServerResponseType HandleMessage(ServerMessageType messagetype, string session_id, byte[] dgram, out byte[] returndgram)
        {
#if HANG_DEBUG
            Logger.Log("Attempting to handle a " + messagetype + "...");
#endif
            var handler = _handlers.FirstOrDefault(x => x.HandledMessageType == messagetype);
            if (handler == null)
            {
                Logger.Log("WARNING: No handler for this message. Returning error.");
                returndgram = new byte[] { };
                return ServerResponseType.REQ_ERROR;
            }
            else
            {
                bool sessionRequired = handler.GetType().GetCustomAttributes(false).FirstOrDefault(x => x is RequiresSessionAttribute) != null;
                if (sessionRequired)
                {
                    if (string.IsNullOrWhiteSpace(session_id))
                    {
                        returndgram = new byte[0];
                        return ServerResponseType.REQ_LOGINREQUIRED;
                    }
                }
                using (var memstr = new MemoryStream(dgram))
                {
                    using (var rms = new MemoryStream())
                    {
                        var result = handler.HandleMessage(this, messagetype, session_id, new BinaryReader(memstr), new BinaryWriter(rms));
                        returndgram = rms.ToArray();
                        return result;
                    }
                }
            }

        }


        private void RecursiveInit(IBackendComponent component)
        {
            foreach (var field in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttributes(false).Any(t => t is DependencyAttribute)))
            {
                if (field.FieldType == this.GetType())
                    continue;
                else
                {
                    var c = this.GetBackendComponent(field.FieldType);
                    RecursiveInit(c);
                }
            }
            if (_components.FirstOrDefault(x => x.Component == component).Initialized == false)
            {
                component.Initiate();
                _components.FirstOrDefault(x => x.Component == component).Initialized = true;
            }
        }

        private ItchUser getItchUser(string apikey)
        {
            if (_isMultiplayer == false)
                apikey = "__SINGLEPLAYER";
            if (_keys.ContainsKey(apikey))
                return GetUserInfo(_keys[apikey]);
            var wr = WebRequest.Create("https://itch.io/api/1/key/me");
            wr.Headers.Add("Authorization: " + apikey);
            using (var response = wr.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        var responseData = JsonConvert.DeserializeObject<ItchResponse>(json);
                        if(responseData.errors!=null)
                        {
                            foreach(var error in responseData.errors)
                            {
                                Logger.Log($"itch.io api error: {error}");
                            }
                            return null;
                        }
                        _keys.Add(apikey, responseData.user.id.ToString());
                        _users.Add(_keys[apikey], responseData.user);
                        return responseData.user;
                    }
                }
            }
        }

        private void ListenThread()
        {
            Logger.Log("Starting TCP thread.");
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _serverReady.Set();
            while (_isRunning)
            {
                try
                {
                    var connection = _listener.AcceptTcpClient();
                    _connected.Add(connection);
                    Logger.Log($"New client connection.");
                    var t = new Thread(() =>
                    {
                        string csession = "";
                        var stream = connection.GetStream();
                        var reader = new BinaryReader(stream);
                        var writer = new BinaryWriter(stream);
                        bool playerJoined = false;

                        while (connection.Connected)
                        {
                            try
                            {
                                var muid = reader.ReadString();
                                Logger.Log("New message incoming.");
                                var mtype = reader.ReadInt32();
                                Logger.Log("Received message type.");
                                string session = reader.ReadString();
                                Logger.Log("Received itch.io OAuth2 token.");
                                if (_isMultiplayer == false)
                                    session = "__SINGLEPLAYER";
                                byte[] content = new byte[] { };
                                Logger.Log("Reading body...");
                                int len = reader.ReadInt32();
                                if (len > 0)
                                    content = reader.ReadBytes(len);
                                Logger.Log("Body received.");
                                byte[] returncontent = new byte[] { };
                                if (_isMultiplayer)
                                {
                                    if (!_keys.ContainsKey(session))
                                    {
                                        Logger.Log("Downloading itch.io user profile data to cache...");
                                        var user = getItchUser(session);
                                        Logger.Log($"{user.display_name} ({user.username}) has connected to the server.");
                                        PlayerJoined?.Invoke(_keys[session], user);
                                    }
                                }
                                else
                                {
                                    if(playerJoined==false)
                                    {
                                        playerJoined = true;
                                        var key = _keys[session];
                                        var user = _users[key];
                                        PlayerJoined?.Invoke(key, user);
                                    }
                                }
                                if (string.IsNullOrWhiteSpace(csession))
                                    csession = _keys[session];
                                if (!_playerIds.ContainsKey(csession))
                                    _playerIds.Add(csession, connection);
                                var result = HandleMessage((ServerMessageType)mtype, _keys[session], content, out returncontent);
                                Logger.Log("Replying to message...");
                                writer.Write(muid);
                                Logger.Log("Writing message result");
                                writer.Write((int)result);
                                Logger.Log("Writing OAuth2 token");
                                writer.Write(session);
                                Logger.Log("Writing body length");
                                writer.Write(returncontent.Length);
                                Logger.Log("Writing body");
                                if (returncontent.Length > 0)
                                    writer.Write(returncontent);
                                writer.Flush();
                            }
                            catch(EndOfStreamException)
                            {
                                break;
                            }
                            catch(IOException)
                            {
                                break;
                            }

                        }
                        reader.Close();
                        writer.Close();
                        stream.Close();
                        reader.Dispose();
                        writer.Dispose();
                        stream.Dispose();
                        _connected.Remove(connection);
                    });
                    t.IsBackground = true;
                    t.Start();
                }
                catch (SocketException sex)
                {
                    Logger.Log("Socket exception: " + sex.ToString());
                } 
            }
        }

        /// <summary>
        /// Retrieves an instance of a given backend component.
        /// </summary>
        /// <typeparam name="T">The type of backend component to look for.</typeparam>
        /// <returns>The instance of the backend component.</returns>
        /// <exception cref="InvalidOperationException">The requested backend component type was never found or loaded.</exception> 
        public T GetBackendComponent<T>() where T : IBackendComponent, new()
        {
            try
            {
                return (T)_components.First(x => x.Component is T).Component;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading backend component \"{typeof(T).FullName}\". See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Retrieves an <see cref="IBackendComponent"/> of the specified type. 
        /// </summary>
        /// <param name="t">The type to search for.</param>
        /// <returns>The backend component instance.</returns>
        /// <exception cref="ArgumentException">The requested backend component type couldn't be found.</exception> 
        public IBackendComponent GetBackendComponent(Type t)
        {
            if (!typeof(IBackendComponent).IsAssignableFrom(t) || t.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"{t.Name} is not an IBackendComponent, or does not provide a parameterless constructor.");
            return _components.First(x => t.IsAssignableFrom(x.Component.GetType())).Component;
        }

        public object Inject(object client)
        {
            foreach (var field in client.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttributes(false).Any(t => t is DependencyAttribute)))
            {
                if (field.FieldType == typeof(Backend))
                    field.SetValue(client, this);
                else
                    field.SetValue(client, GetBackendComponent(field.FieldType));
            }
            return client;
        }

        /// <summary>
        /// Creates a new instance of the specified object, injecting any dependencies into it. Ahhh, don't stop. Keep injecting me those dependencies.
        /// </summary>
        /// <typeparam name="T">The type of the object to create</typeparam>
        /// <returns>The newly-created object with all dependencies injected.</returns>
        public T New<T>() where T : new()
        {
            return (T)Inject(new T());
        }
        /// <summary>
        /// Creates a new instance of the specified type and injects all dependencies.
        /// </summary>
        /// <param name="t">The type of the object to create.</param>
        /// <returns>The newly created object with all dependencies injected.</returns>
        /// <exception cref="ArgumentException">The specified type doesn't define a public parameterless constructor.</exception> 
        public object New(Type t)
        {
            if (t.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"{t.Name} does not provide a parameterless constructor.");
            return Inject(Activator.CreateInstance(t, null));
        }



        private void InvokeSafetyCheck(object o)
        {
            _safety = true;
            _workForUtility.Set();
        }

        private void UtilityThread()
        {
            Logger.Log("Utility thread started!");
            _workForUtility.Set();
            if (_safetyWatch == null)
                _safetyWatch = new Timer(InvokeSafetyCheck, null, _waittimems, _waittimems);
            while (_isRunning)
            {
                _workForUtility.WaitOne();
                lock (_utilityActions)
                {
                    while (_utilityActions.Count > 0)
                    {
                        Logger.Log("Invoking utility action..");
                        _utilityActions.Dequeue()?.Invoke();
                    }
                }
                if (_safety)
                {
                    Logger.Log("Performing safety check...");
                    foreach (var component in _components)
                    {
                        component.Component.SafetyCheck();
                    }
                    Logger.Log("Done.");
                    _safety = false;
                }
                _workForUtility.Reset();
            }
            Logger.Log("Utility thread is shutting down...");
            Logger.Log("ONE LAST SAFETY CHECK.");
            foreach (var component in _components)
            {
                component.Component.SafetyCheck();
            }
            //we do this stuff separately so that each component can safety-check before any of their dependencies unload. Thus, fixing a FATAL CRASH.
            foreach (var component in _components)
            {
                component.Component.Unload();
            }
            Logger.Log("Goodnight Australia.");
            _shutdownComplete.Set();
        }

        /// <summary>
        /// Shuts down the server, disconnecting all clients and unloading resources.
        /// </summary>
        /// <param name="message">A message to broadcast to all clients as the server shuts down.</param>
        public void Shutdown(string message = "Server going down for maintenance.")
        {
            if (!string.IsNullOrEmpty(message))
            {
                using (var memstr = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                    {
                        writer.Write(message);
                        Broadcast(ServerBroadcastType.Shutdown, memstr.ToArray());
                    }
                }
            }
            Logger.Log("Commencing shutdown...");
            Logger.Log("Stopping TCP listener.");
            _listener?.Stop();
            _tcpthread?.Abort();
            _listener = null;
            Logger.Log("Done.");
            Logger.Log("Stopping everything else...");
            _isRunning = false;
            _workForUtility?.Set();
            Logger.Log("Waiting for utility thread shutdown...");
            if(_shutdownComplete!=null)
                _shutdownComplete.WaitOne();
            Logger.Log("Everything's shut down. Cleaning up...");
            _utilityThread = null;
            _components = null;
            _utilityActions = null;

            _shutdownComplete?.Dispose();
            _shutdownComplete = null;
            _workForUtility?.Dispose();
            _workForUtility = null;
            _safetyWatch?.Dispose();
            _safetyWatch = null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Shutdown();
        }

        /// <summary>
        /// Starts the server and allows connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">The server is already running.</exception> 
        public void Listen()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("The listener is already running.");
            }
            Logger.Log("Starting the backend!!");
            _isRunning = true;
            _utilityThread.Start();
            _tcpthread.Start();
        }
    }

    /// <summary>
    /// Handler for retrieving server config.
    /// </summary>
    public class ServerConfigHandler : IMessageHandler
    {
        /// <inheritdoc/>
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.U_CONF;
            }
        }

        /// <inheritdoc/>
        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            datawriter.Write(backend.IsMultiplayer);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    /// <summary>
    /// A class encapsulating an <see cref="IBackendComponent"/> that indicates whether it has been initialized properly or not. 
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// Gets or sets whether the component has been initialized.
        /// </summary>
        public bool Initialized { get; set; }
        /// <summary>
        /// Gets or sets the underlying <see cref="IBackendComponent"/>. 
        /// </summary>
        public IBackendComponent Component { get; set; }
    }
}
