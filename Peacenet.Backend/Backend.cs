using System;
using Plex.Objects;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace Peacenet.Backend
{
    public class Backend : IDisposable
    {
        private List<IBackendComponent> _components = null;
        private int _port = 0;
        private Thread _utilityThread = null;
        private Queue<Action> _utilityActions = new Queue<Action>();
        private volatile bool _isRunning = false;
        private const int _waittimems = 1800000;
        private TcpListener _listener = null;
        private Thread _tcpthread = null;
        private bool _isMultiplayer = false;

        // Fired on any of these events:
        // 1) A new utility action has been enqueued
        // 2) The timer has expired - it's time to call SafetyCheck()
        // 3) It's time to shut down
        private EventWaitHandle _workForUtility = new ManualResetEvent(false);

        private Timer _safetyWatch = null;

        // Set to true before the timer fires.
        private volatile bool _safety = false;

        // Fired by the utility thread to let the main thread know that
        // it's done shutting down.
        private EventWaitHandle _shutdownComplete = new AutoResetEvent(false);

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

        public Backend(int port, bool isMultiplayer = true)
        {
            _isMultiplayer = isMultiplayer;
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));
            _port = port;
            Logger.Log("Initiating Peacenet backend...");
            _components = new List<IBackendComponent>();
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
                    _components.Add(component);
                 }
             }
            Logger.Log("Initiating all backend components...");
            foreach (var component in _components.ToArray())
            {
                Inject(component);
                component.Initiate();
            }

            Logger.Log("Utility thread creating!");
            _utilityThread = new Thread(this.UtilityThread);
            _utilityThread.IsBackground = true;

            _tcpthread = new Thread(ListenThread);
            _tcpthread.IsBackground = true;
        }

        private string getWatercolorId(string token)
        {
            if (_isMultiplayer == false)
                return "entity.singleplayeruser";
            string uid = null;
            var wr = WebRequest.Create("https://getshiftos.net/users/getid");
            wr.ContentType = "text/plain";
            wr.Method = "GET";
            wr.Headers.Add("Authentication: " + token);
            try
            {
                using (var res = wr.GetResponse())
                {
                    using (var str = res.GetResponseStream())
                    {
                        using (var reader = new StreamReader(str))
                        {
                            return uid;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }


        private void ListenThread()
        {
            var delegator = GetBackendComponent<MessageDelegator>();
                        Logger.Log("Starting TCP thread.");
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (_isRunning)
            {
                try
                {
                    var connection = _listener.AcceptTcpClient();
                    Logger.Log($"New client connection.");
                    var t = new Thread(() =>
                    {
                        var stream = connection.GetStream();
                        var reader = new BinaryReader(stream);
                        var writer = new BinaryWriter(stream);

                        while (connection.Connected)
                        {
                            try
                            {
                                var muid = reader.ReadString();
                                var mtype = reader.ReadInt32();
                                string session = reader.ReadString();
                                byte[] content = new byte[] { };
                                int len = reader.ReadInt32();
                                if (len > 0)
                                    content = reader.ReadBytes(len);
                                byte[] returncontent = new byte[] { };
                                var result = delegator.HandleMessage(this, (ServerMessageType)mtype, getWatercolorId(session), content, out returncontent);

                                writer.Write(muid);
                                writer.Write((int)result);
                                writer.Write(session);
                                writer.Write(returncontent.Length);
                                if (returncontent.Length > 0)
                                    writer.Write(returncontent);
                                writer.Flush();
                            }
                            catch (EndOfStreamException) { }
                        }
                        reader.Close();
                        writer.Close();
                        stream.Close();
                        reader.Dispose();
                        writer.Dispose();
                        stream.Dispose();
                    });
                    t.IsBackground = true;
                    t.Start();
                }
                catch (SocketException) { }
            }
        }

        public T GetBackendComponent<T>() where T : IBackendComponent, new()
        {
            return (T)_components.First(x => x is T);
        }

        public IBackendComponent GetBackendComponent(Type t)
        {
            if (!typeof(IBackendComponent).IsAssignableFrom(t) || t.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"{t.Name} is not an IBackendComponent, or does not provide a parameterless constructor.");
            return _components.First(x => t.IsAssignableFrom(x.GetType()));
        }

        private object Inject(object client)
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

        public T New<T>() where T : new()
        {
            return (T)Inject(new T());
        }

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
                        component.SafetyCheck();
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
                component.SafetyCheck();
                component.Unload();
            }
            Logger.Log("Goodnight Australia.");
            _shutdownComplete.Set();
        }

        public void Shutdown()
        {
            Logger.Log("Commencing shutdown...");
            Logger.Log("Stopping TCP listener.");
            _listener.Stop();
            _tcpthread.Abort();
            _listener = null;
            Logger.Log("Done.");
            Logger.Log("Stopping everything else...");
            _isRunning = false;
            _workForUtility.Set();
            Logger.Log("Waiting for utility thread shutdown...");
            _shutdownComplete.WaitOne();
            Logger.Log("Everything's shut down. Cleaning up...");
            _utilityThread = null;
            _components = null;
            _utilityActions = null;

            _shutdownComplete.Dispose();
            _shutdownComplete = null;
            _workForUtility.Dispose();
            _workForUtility = null;
            _safetyWatch.Dispose();
            _safetyWatch = null;
        }

        public void Dispose()
        {
            Shutdown();
        }

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

    public class ServerConfigHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.U_CONF;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            datawriter.Write(backend.IsMultiplayer);
            return ServerResponseType.REQ_SUCCESS;
        }
    }
}
