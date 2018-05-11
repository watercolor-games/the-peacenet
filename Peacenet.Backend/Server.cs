using Newtonsoft.Json;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Peacenet.Backend
{
    /// <summary>
    /// Represents an instance of a Peacenet server.
    /// </summary>
    public class Server : IDisposable
    {
        private int _port = -1;
        private bool _multiplayer = true;
        private TcpListener _listener = null;

        private int _maxPlayers = 5;

        private List<Client> _clients = new List<Client>();
        private List<int> _whitelist = new List<int>();
        private List<int> _banlist = new List<int>();

        private ItchUser _singleplayerFakeUser = null;

        public Server(int port, int maxPlayers = 5, bool multiplayer = true)
        {
            _maxPlayers = maxPlayers;
            _port = port;
            _multiplayer = multiplayer;
            _singleplayerFakeUser = new ItchUser
            {
                cover_url = null,
                developer = false,
                display_name = "User",
                gamer = true,
                id = -12345,
                press_user = false,
                url = null,
                username = "user"
            };
        }

        public ItchUser GetItchUser(string apikey)
        {
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
                        if (responseData.errors != null)
                        {
                            foreach (var error in responseData.errors)
                            {
                                Logger.Log($"itch.io api error: {error}");
                            }
                            if (_multiplayer)
                                return null;
                            else
                                return _singleplayerFakeUser;
                        }
                        return responseData.user;
                    }
                }
            }

        }

        public bool Banned(int uid)
        {
            return _banlist.Contains(uid);
        }

        public bool IsWhitelisted(int uid)
        {
            if (_whitelist.Count == 0)
                return true;
            return _whitelist.Contains(uid);
        }

        public bool LoggedIn(string itchoauth)
        {
            return _clients.FirstOrDefault(x => x.OAuthToken == itchoauth) != null;
        }

        public bool Multiplayer
        {
            get
            {
                return _multiplayer;
            }
        }

        public int GetMaxPlayers()
        {
            return (_multiplayer) ? _maxPlayers : 1;
        }

        public bool HasPlayerSpace()
        {
            return _clients.Count + 1 <= GetMaxPlayers();
        }

        public void Listen()
        {
            var ip = (_multiplayer) ? IPAddress.Any : IPAddress.Loopback;
            _listener = new TcpListener(ip, _port);
            Logger.Log($"Starting TCP listener on {ip}, port {_port}...", System.ConsoleColor.Green);
            _listener.Start();
            while (_listener != null)
            {
                var client = _listener.AcceptTcpClient();
                var clientListener = new Client(this, client);
                Task.Run(() =>
                {
                    var handshakeResult = clientListener.Handshake();
                    if(handshakeResult == HandshakeResult.OK)
                    {
                        Logger.Log($"{clientListener.User.username} has joined the server!");
                        var listenResult = clientListener.Listen();
                        if (listenResult == ListenResult.OK)
                        {
                            Logger.Log($"{clientListener.User.username} has left.");
                        }
                        else
                        {
                            Logger.Log($"{clientListener.User.username} has been disconnected: {listenResult}", System.ConsoleColor.DarkYellow);
                        }
                    }
                    else
                    {
                        Logger.Log($"Client rejected: {handshakeResult}", System.ConsoleColor.DarkYellow);
                    }
                });
            }
        }

        public void Dispose()
        {
            if(_listener != null)
            {
                while(_clients.Count>0)
                {
                    _clients[0].TcpClient.Close();
                    _clients.RemoveAt(0);
                }
                _listener.Stop();
                _listener = null;
            }
        }
    }

    public class Client
    {
        public ItchUser User { get; private set; }
        public TcpClient TcpClient { get; private set; }
        public string OAuthToken { get; private set; }

        private Server _server = null;

        private BinaryReader reader = null;
        private BinaryWriter writer = null;

        private volatile bool _listening = false;

        public Client(Server server, TcpClient client)
        {
            _server = server;
            TcpClient = client;
        }

        public HandshakeResult Handshake()
        {
            var stream = TcpClient.GetStream();
            reader = new BinaryReader(stream, Encoding.UTF8);
            writer = new BinaryWriter(stream, Encoding.UTF8); 

            string itchOAuth = reader.ReadString();
            if(!_server.HasPlayerSpace())
            {
                writer.Write(4);
                TcpClient.Close();
                return HandshakeResult.MaxPlayersReached;
            }
            if (_server.LoggedIn(itchOAuth))
            {
                writer.Write(5);
                TcpClient.Close();
                return HandshakeResult.AlreadyLoggedIn;
            }
            var user = _server.GetItchUser(itchOAuth);
            if (user == null)
            {
                writer.Write(1);
                TcpClient.Close();
                return HandshakeResult.BadLogin;
            }
            if (_server.Multiplayer)
            {
                if (_server.Banned(user.id))
                {
                    writer.Write(2);
                    TcpClient.Close();
                    return HandshakeResult.Banned;
                }
                if (!_server.IsWhitelisted(user.id))
                {
                    writer.Write(3);
                    TcpClient.Close();
                    return HandshakeResult.NotWhitelisted;
                }
            }

            writer.Write(0);
            writer.Write("255.255.255.255"); //todo: actual ip address
            writer.Write((decimal)0.0); //todo: actual gvt alert
            writer.Write(false); //gvt alert isn't decreasing
            writer.Write((decimal)0.0); //reputation value
            writer.Write("{}"); //todo: save file
            writer.Write(1); //fs dir count
            writer.Write("/"); //fs dir list
            writer.Write(1); //file count
            writer.Write("/welcome.txt");
            writer.Write(2556); //size in bytes of the file.
            writer.Write(1); //World node count, not including player
            writer.Write("Test entity");
            writer.Write("This is a test world node. If you're seeing this in your World Map, the Milestone 4 server protocol is working perfectly so far.");
            writer.Write(3); //Country code, 0-6.
            writer.Write(0.0f); //Center of country on x-coordinate
            writer.Write(0.0f); //Center of country on y-coordinate.
            writer.Write("8.8.8.8"); //Entity IP address
            writer.Write(1); //number of linked nodes.
            writer.Write("255.255.255.255"); //Linked to player node
            writer.Write(0); //status of entity - 0 = idle, 1 = under attack, 2 = hacked, 3 = gvt alert.
            writer.Write((decimal)0.75); //entity reputation value
            writer.Write(0); //player country code
            writer.Write(0.0f); //player x coordinate
            writer.Write(0.0f); //player y coordinate
            writer.Write(0); //player shouldn't link to any nodes, it's already linked to by the only other one in the world.

            User = user;
            OAuthToken = itchOAuth;

            return HandshakeResult.OK;
        }

        public ListenResult Listen()
        {
            if (_listening == true)
                throw new InvalidOperationException("The server is already listening for requests on this client.");

            if (reader == null || writer == null || User == null)
                throw new InvalidOperationException("You cannot call Listen() before calling Handshake() successfully.");

            TcpClient.ReceiveTimeout = 2000;

            _listening = true;

            var dataAvailable = new ManualResetEvent(false);
            var dataRead = new ManualResetEvent(true);

            while (TcpClient.Connected)
            {
                if (TcpClient.Client.Available > 0)
                {
                    short track = 0;
                    short command = 0;
                    byte[] data = null;
                    byte[] resData = null;
                    short result = 0;
                    try
                    {
                        track = reader.ReadInt16();
                        command = reader.ReadInt16();
                        int len = reader.ReadInt32();
                        if (len > 0)
                        {
                            data = reader.ReadBytes(len);
                        }
                    }
                    catch(IOException)
                    {
                        TcpClient.Close();
                        return ListenResult.ReadTimeout;
                    }
                    Logger.Log($"<{User.username}/{track}>: {command}");
                    try
                    {
                        resData = Encoding.UTF8.GetBytes($@"Tracking ID: {track}
Itch username: {User.username}
Command code: {command}
Body: {Encoding.UTF8.GetString(data)}");
                    }
                    catch (Exception ex)
                    {
                        resData = Encoding.UTF8.GetBytes(ex.Message);
                        result = 1;
                    }
                    finally
                    {
                        writer.Write(track);
                        writer.Write(result);
                        if (resData == null || resData.Length == 0)
                            writer.Write(0);
                        else
                        {
                            writer.Write(resData.Length);
                            writer.Write(resData);
                        }
                    }
                    dataRead.Set();
                    dataAvailable.Reset();
                    if (result != 0)
                    {
                        Logger.Log($"Server error code {result}: {Encoding.UTF8.GetString(resData)}", System.ConsoleColor.Red);
                        TcpClient.Close();
                        return ListenResult.ConnectionError;
                    }
                }
                else
                {
                    if(!TcpClient.Client.Poll(10000000, SelectMode.SelectRead))
                    {
                        TcpClient.Close();
                        return ListenResult.ReadTimeout;
                    }
                }
            }

            return ListenResult.OK;

        }
    }

    public enum ListenResult
    {
        OK,
        ReadTimeout,
        ConnectionError
    }

    public enum HandshakeResult
    {
        OK,
        Banned,
        NotWhitelisted,
        BadLogin,
        AlreadyLoggedIn,
        MaxPlayersReached
    }
}
