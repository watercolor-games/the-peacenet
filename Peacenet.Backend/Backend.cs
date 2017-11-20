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
    public class Backend
    {
        private List<IBackendComponent> _components = null;
        private int _port = 0;
        private Thread _utilityThread = null;
        private Queue<Action> _utilityActions = new Queue<Action>();
        private bool _isRunning = false;
        private const int _waittimems = 1800000;
        private TcpListener _listener = null;
        private Thread _tcpthread = null;
        private bool _isMultiplayer = false;

        public bool IsMultiplayer
        {
            get
            {
                return _isMultiplayer;
            }
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
                    Logger.Log($"Found {type.Name}.");
                    var component = (IBackendComponent)Activator.CreateInstance(type, null);
                    _components.Add(component);
                }
            }
            Logger.Log("Initiating all backend components...");
            foreach (var component in _components)
            {
                component.Initiate();
            }
            Logger.Log("Utility thread creating!");
            _utilityThread = new Thread(this.UtilityThread);
            _utilityThread.IsBackground = true;

            _tcpthread = new Thread(ListenThread);
            _tcpthread.IsBackground = true;
        }


        private void ListenThread()
        {
            var delegator = GetBackendComponent<MessageDelegator>();
                        Logger.Log("Starting TCP thread.");
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (_isRunning)
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
                        var muid = reader.ReadString();
                        var mtype = reader.ReadInt32();
                        string session = reader.ReadString();
                        byte[] content = new byte[] { };
                        int len = reader.ReadInt32();
                        if (len > 0)
                            content = reader.ReadBytes(len);
                        byte[] returncontent = new byte[] { };
                        var result = delegator.HandleMessage(this, (ServerMessageType)mtype, session, content, out returncontent);

                        writer.Write(muid);
                        writer.Write((int)result);
                        writer.Write(session);
                        writer.Write(returncontent.Length);
                        if (returncontent.Length > 0)
                            writer.Write(returncontent);
                        writer.Flush();
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
        }

        public T GetBackendComponent<T>()
        {
            var component = _components.FirstOrDefault(x => x is T);
            if (component == null)
                throw new ArgumentException($"No backend component of type {nameof(T)} could be found. Are you sure it implements ${nameof(IBackendComponent)}?");
            return (T)component;            
        }

        private void UtilityThread()
        {
            Logger.Log("Utility thread started!");
            int _waittime = _waittimems;
            while (_isRunning)
            {
                while (_utilityActions.Count > 0)
                {
                    Logger.Log("Invoking utility action..");
                    _utilityActions.Dequeue()?.Invoke();
                }
                if (_waittime > 0)
                {
                    _waittime--;
                    Thread.Sleep(1);
                }
                else
                {
                    _waittime = _waittimems;
                    Logger.Log("Performing safety check...");
                    foreach (var component in _components)
                    {
                        component.SafetyCheck();
                    }
                    Logger.Log("Done.");
                }
            }
            Logger.Log("Utility thread is shutting down...");
            Logger.Log("ONE LAST SAFETY CHECK.");
            foreach (var component in _components)
            {
                component.SafetyCheck();
                component.Unload();
            }
        }

        public void Shutdown()
        {
            Logger.Log("Commencing shutdown...");
            Logger.Log("Stopping TCP listener.");
            _tcpthread.Abort();
            _listener.Stop();
            _listener = null;
            Logger.Log("Done.");
            Logger.Log("Stopping everything else...");
            _isRunning = false;
            Logger.Log("Waiting for utility thread shutdown...");
            while (_utilityThread.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(1);
            }
            Logger.Log("Everything's shut down. Cleaning up...");
            _utilityThread = null;
            _components = null;
            _utilityActions = null;
        }

        public void Listen()
        {
            if (_isRunning == true)
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
