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

namespace Plex.Engine.Server
{
    public class AsyncServerManager : IEngineComponent
    {
        [Dependency]
        private WatercolorAPIManager _api = null;

        private List<ServerInformation> _serverInfo = new List<ServerInformation>();
        private TcpClient _tcpClient = null;
        private BinaryReader _reader = null;
        private BinaryWriter _writer = null;
        private string _session = "";
        private Action<string> _onConnectionError;
        private bool _isMultiplayer = false;

        public bool Connected
        {
            get
            {
                return (_tcpClient == null) ? false : _tcpClient.Connected;
            }
        }

        public void Disconnect()
        {
            _tcpClient?.Close();
            _tcpClient = null;
        }

        public ServerInformation[] GetServers
        {
            get
            {
                return _serverInfo.ToArray();
            }
        }

        public void Initiate()
        {
            Logger.Log("Loading server list from configuration...");
            if (File.Exists("servers.json"))
                _serverInfo = JsonConvert.DeserializeObject<List<ServerInformation>>(File.ReadAllText("servers.json"));
            Logger.Log($"{_serverInfo.Count} servers loaded.");
        }

        public async Task SendMessage(ServerMessageType type, byte[] body, Action<ServerResponseType, BinaryReader> onResponse)
        {
            await Task.Run(() =>
            {
                string muid = Guid.NewGuid().ToString();
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
                lock (_reader)
                {
                    readMUID:
                    string rMuid = _reader.ReadString();
                    if (rMuid != muid)
                        goto readMUID;
                    int remoteResponse = _reader.ReadInt32();
                    _session = _reader.ReadString();
                    int remoteLen = _reader.ReadInt32();
                    byte[] remoteBody = new byte[remoteLen];
                    if(remoteLen>0)
                    {
                        remoteBody = _reader.ReadBytes(remoteLen);
                    }

                    ServerResponseType response = (ServerResponseType)remoteResponse;
                    if (response == ServerResponseType.REQ_LOGINREQUIRED)
                    {
                        _onConnectionError?.Invoke("A Watercolor API error occurred on the server and you have lost connection as a result.");
                        Disconnect();
                    }
                    else
                    {
                        if (remoteLen > 0)
                        {
                            using (var memstr = new MemoryStream(remoteBody))
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
                }
            });
        }

        public void Connect(ServerInformation information, Action onConnected, Action<string> onError)
        {
            Connect(information.Address, onConnected, onError);
        }

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
                    if (_api.LoggedIn == false)
                        throw new InvalidOperationException("Cannot connect to a multiplayer server without a Watercolor account.");
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
                    SendMessage(ServerMessageType.U_CONF, null, (res, reader) =>
                    {
                        _isMultiplayer = reader.ReadBoolean();
                        onConnected?.Invoke();
                    });
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex.Message);
                    Disconnect();
                }
            });

        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void SetServer(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
                return;
            if (_serverInfo.FirstOrDefault(x => x.Name == name) != null)
                _serverInfo.FirstOrDefault(x => x.Name == name).Address = address;
            else
                _serverInfo.Add(new ServerInformation
                {
                    Name = name,
                    Address = address
                });
        }

        public void Unload()
        {
            File.WriteAllText("servers.json", JsonConvert.SerializeObject(_serverInfo, Formatting.Indented));
        }
    }

    public class ServerInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
