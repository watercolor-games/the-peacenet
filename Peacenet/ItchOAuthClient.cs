using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Engine.Config;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using Plex.Engine.TextRenderers;
using Plex.Engine.Themes;
using Plex.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Peacenet
{
    /// <summary>
    /// Provides an OAuth2 client for itch.io.
    /// </summary>
    public class ItchOAuthClient : IEngineComponent
    {
        private string _token = "";
        private HttpServer _callbackSrv = null;

        [Dependency]
        private GameLoop _plebgate = null;

        [Dependency]
        private ConfigManager _config = null;

        [Dependency]
        private UIManager _ui = null;

        private ItchEntity _entity = null;

        private ItchUser _user = null;

        /// <summary>
        /// Gets the currently logged-in user.
        /// </summary>
        public ItchUser User
        {
            get
            {
                return _user;
            }
        }

        /// <summary>
        /// Gets the API token of this user.
        /// </summary>
        public string Token
        {
            get
            {
                return _token;
            }
        }

        /// <inheritdoc/>
        public void Initiate()
        {
            _config.GetValue("itch.showUsername", false); //set default value to false. User can choose if they want to display their itch username in Peacegate OS.

            _entity = _plebgate.New<ItchEntity>();
            _token = _config.GetValue<string>("itch.apikey", _token);
            _callbackSrv = _plebgate.New<ItchOAuthCallbackServer>();
            try
            {
                fetchUserData();
            }
            catch
            {
                Logger.Log("Connection to itch.io failed.", System.ConsoleColor.Yellow);
            }
        }

        /// <summary>
        /// Direct the user to the itch.io OAuth login page. This function runs asynchronously and cannot be awaited.
        /// </summary>
        /// <param name="clientId">The itch.io Client ID to use</param>
        /// <param name="onWaitBegin">A function that is called just before the client waits for itch.io to respond. Use this to show any UI that you need to.</param>
        /// <param name="onComplete">A function to be called when the login is complete.</param>
        public void Login(string clientId, Action onWaitBegin, Action onComplete)
        {
            if (LoggedIn)
                return;
            Logger.Log("Starting internal server for callback...");
            Logger.Log("Starting oauth2 request...");
            _entity.Spawn();
            Task.Run(() =>
            {
                string uri = Uri.EscapeDataString("http://localhost:3254/itch_callback");
                Process.Start("https://itch.io/user/oauth?client_id=17a4b2de3caf06c14a524936d88402c1&scope=profile%3Ame&response_type=token&redirect_uri=http%3A%2F%2Flocalhost%3A3254%2Fitch_callback");
                string token = (_callbackSrv as ItchOAuthCallbackServer).WaitForToken();
                if (_token != null)
                {
                    _token = token;
                    _config.SetValue("itch.apikey", _token);
                    _config.SaveToDisk(); fetchUserData();
                    fetchUserData();
                }
                _plebgate.Invoke(_entity.Despawn);
            });
        }

        public void KillServer()
        {
            (_callbackSrv as ItchOAuthCallbackServer).CancelTokenWait();
        }

        private void fetchUserData()
        {
            if (string.IsNullOrWhiteSpace(_token))
                return;
            var wr = WebRequest.Create("https://itch.io/api/1/key/me");
            wr.Headers.Add("Authorization: Bearer " + _token);
            using (var response = wr.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        var responseObject = JsonConvert.DeserializeObject<ItchResponse>(json);
                        if(responseObject.errors != null)
                        {
                            foreach(var error in responseObject.errors)
                            {
                                Logger.Log(error, System.ConsoleColor.DarkYellow);
                            }
                            _token = null;
                            _user = null;
                            _config.SetValue("itch.apikey", null);
                            _config.SaveToDisk();
                            return;
                        }
                        _user = responseObject.user;
                        if (_user.display_name == null)
                            _user.display_name = _user.username;
                    }
                }
            }
            
        }

        /// <summary>
        /// Retrieves whether the user is logged in via itch.io
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_token) && (_user != null);
            }
        }


        /// <summary>
        /// Log out of itch.io.
        /// </summary>
        public void Logout()
        {
            _token = null;
        }

    }


    /// <summary>
    /// Provides an extremely barebones non-SSL HTTP server written entirely in C#.
    /// </summary>
    public abstract class HttpServer
    {

        private int port;
        private TcpListener listener;
        private bool is_active = true;

        /// <summary>
        /// Creates a new instance of the <see cref="HttpServer"/> class. 
        /// </summary>
        /// <param name="port">The port on which the server will listen.</param>
        public HttpServer(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// Start listening for requests.
        /// </summary>
        public void listen()
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                Logger.Log($"{s.Client.RemoteEndPoint} just connected.");
                HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
            }
            listener.Stop();
        }

        public void stop()
        {
            is_active = false;
        }


        /// <summary>
        /// Handle an incoming HTTP GET request.
        /// </summary>
        /// <param name="p">The request processor containing details about the request.</param>
        public abstract void handleGETRequest(HttpProcessor p);
        /// <summary>
        /// Handle an incoming POST request.
        /// </summary>
        /// <param name="p">The details about the request.</param>
        /// <param name="inputData">The request's input stream.</param>
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    /// <summary>
    /// Processes an HTTP request.
    /// </summary>
    public class HttpProcessor
    {
        /// <summary>
        /// The socket that the request was made on.
        /// </summary>
        public TcpClient socket;
        /// <summary>
        /// The server that the request was made on.
        /// </summary>
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        /// <summary>
        /// The method of the request (GET, POST, etc)
        /// </summary>
        public String http_method;
        /// <summary>
        /// The URL of the request (ex: /foo/bar.html)
        /// </summary>
        public String http_url;
        /// <summary>
        /// The HTTP protocol version.
        /// </summary>
        public String http_protocol_versionstring;
        /// <summary>
        /// A hash-table containing key-value pairs representing the HTTP headers of the request.
        /// </summary>
        public Hashtable httpHeaders = new Hashtable();

        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// Creates a new instance of the <see cref="HttpProcessor"/> class. 
        /// </summary>
        /// <param name="s">The socket the request is made on.</param>
        /// <param name="srv">The server that the request was made on.</param>
        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            try
            {
                outputStream.Flush();
            }
            catch { }// bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        /// <summary>
        /// Parse the request.
        /// </summary>
        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        /// <summary>
        /// Read the request's HTTP headers.
        /// </summary>
        public void readHeaders()
        {
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                httpHeaders[name] = value;
            }
        }

        /// <summary>
        /// Handle a GET request.
        /// </summary>
        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        /// <summary>
        /// Handle a POST request.
        /// </summary>
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        internal void writeForbidden()
        {
            outputStream.WriteLine("HTTP/1.0 403 Forbidden");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("X-Content-Type-Options: nosniff");
            outputStream.WriteLine("X-Frame-Options: DENY");
            outputStream.WriteLine("X-XSS-Options: 1");
            outputStream.WriteLine("Content-Type: text/plain");
            outputStream.WriteLine("");
        }

        /// <summary>
        /// Write raw binary to the response stream.
        /// </summary>
        /// <param name="contentType">The MIME type of the data being written</param>
        /// <param name="data">The binary data to write</param>
        public void writeBinary(string contentType, byte[] data)
        {
            outputStream.Write("HTTP/1.0 200 OK\r\n");
            outputStream.Write("Content-Type: " + contentType + "\r\n");
            outputStream.Write("Content-Length: " + data.Length + "\r\n");
            outputStream.WriteLine("X-XSS-Options: 1");
            outputStream.WriteLine("X-Frame-Options: DENY");
            outputStream.WriteLine("X-Content-Type-Options: nosniff");
            outputStream.Write("Connection: close\r\n");
            outputStream.Write("\r\n");
            outputStream.Flush();
            for (int i = 0; i < data.Length; i++)
                outputStream.BaseStream.WriteByte(data[i]);
        }

        /// <summary>
        /// Return an HTTP 200 response.
        /// </summary>
        /// <param name="content_type">The MIME type of the request body</param>
        public void writeSuccess(string content_type = "text/html")
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("X-XSS-Options: 1");
            outputStream.WriteLine("X-Frame-Options: DENY");
            outputStream.WriteLine("X-Content-Type-Options: nosniff");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        /// <summary>
        /// Write an HTTP 404 response.
        /// </summary>
        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("X-XSS-Options: 1");
            outputStream.WriteLine("Content-Type: text/html");
            outputStream.WriteLine("X-Content-Type-Options: nosniff");
            outputStream.WriteLine("X-Frame-Options: DENY");
            outputStream.WriteLine("");
        }
    }

    internal class ItchOAuthCallbackServer : HttpServer
    {
        private EventWaitHandle _receivedToken = new ManualResetEvent(false);
        private string _token = null;

        private string _jsCallbackPage = @"<html>
    <head>
        <title>Processing request...</title>
        <script src=""https://code.jquery.com/jquery-3.2.1.min.js""
            integrity=""sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=""
            crossorigin=""anonymous""></script>

    </head>

    <body>
        <script>
            var hash = window.location.hash;
            $.ajax(""http://localhost:3254/itch_callback"", {
                    type: ""POST"",
                    contentType: ""text/plain"",
                    data: hash,
                    success: function (a,b,c){
                        close();
                    }
                });
        </script>
        <h1>Sign-in complete</h1>

        <p>This browser window should close automatically once Peacenet has successfully logged you in to itch.io.</p>

        <p>If the window doesn't close and the game IS logged in, you can safely close the window on your own.</p>

        <p><strong>DO NOT refresh this page.</strong> By the time the game has received your itch.io API key, the webserver responsible for showing this page has been shut down.</p>
    </body>
</html>";


        public string WaitForToken()
        {
            Task.Run(() =>
            {
                listen();
            });
            _receivedToken.Reset();
            _receivedToken.WaitOne();

            return _token;
        }

        public void CancelTokenWait()
        {
            base.stop();
            _token = null;
            _receivedToken.Set();
        }

        public ItchOAuthCallbackServer() : base(3254)
        {
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            if(p.http_url == "/itch_callback")
            {
                p.writeSuccess();
                p.outputStream.Write(_jsCallbackPage);
                return;
            }
            p.writeFailure();
            p.outputStream.WriteLine("<h1>404 Not Found</h1><br/><p>An attempt was made to talk to The Peacenet through the OAuth2 callback webserver and the request wasn't an OAuth2 callback.</p>");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            if(p.http_url == "/itch_callback")
            {
                string hash = inputData.ReadToEnd();
                _token = hash.Replace("#access_token=", "");
                p.writeSuccess();
                _receivedToken.Set();
                stop();
                return;
            }
            p.writeFailure();
            p.outputStream.WriteLine("<h1>404 Not Found</h1><br/><p>An attempt was made to talk to The Peacenet through the OAuth2 callback webserver and the request wasn't an OAuth2 callback.</p>");
        }
    }

    public class ItchEntity : IEntity
    {
        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private ItchOAuthClient _oauth = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        private string _head = "Waiting for itch.io login";
        private string _desc = @"You should see a browser window asking you to log in to itch.io or to authorize The Peacenet to access your account. Log in and authorize The Peacenet to sign in.

If you don't see a browser window, you may need to press F11 to exit Fullscreen mode. Click the button below to cancel the login process and return to the game.";
        private string _paused = "Note: Game updating is currently paused.";
        private string _buttonText = "Cancel";

        private float _textHeadAnim = 0f;
        private float _textDescAnim = 0f;
        private float _buttonAnim = 0f;
        private bool _animReversed = false;

        private const int _headDescPadV = 10;
        private const int _descMutePadV = 5;
        private const int _muteButtonPadV = 7;
        private const int _buttonMarginH = 28;
        private const int _buttonMarginV = 7;

        private int _headHeight = 0;
        private int _descHeight = 0;
        private int _pausedHeight = 0;

        private int _uiHeight = 0;

        private Rectangle _button = Rectangle.Empty;
        private bool _buttonHovered = false;
        private ButtonState _buttonState = ButtonState.Released;

        private int _textStartY = 0;

        [Dependency]
        private ThemeManager _theme = null;

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black * MathHelper.Lerp(0, 0.75F, _textHeadAnim));

            int _half = (gfx.Width - (gfx.Width / 2)) / 2;

            int headYBase = (!_animReversed) ? _textStartY : _textStartY - (gfx.Height / 10);
            int headYUnder = (_animReversed) ? _textStartY : _textStartY + (gfx.Height / 10);
            int headYAnim = (int)MathHelper.Lerp(headYUnder, headYBase, _textHeadAnim);
            gfx.DrawString(_head, new Vector2(_half, headYAnim), _theme.Theme.GetFontColor(TextFontStyle.Header1) * _textHeadAnim, _theme.Theme.GetFont(TextFontStyle.Header1), TextAlignment.Center, gfx.Width / 2, WrapMode.Words);

            int descYBase = _textStartY + _headHeight + _headDescPadV;
            int descYUpper = (!_animReversed) ? descYBase : descYBase - (gfx.Height / 10);
            int descYLower = (_animReversed) ? descYBase + (gfx.Height / 10) : descYBase;
            int descYAnim = (int)MathHelper.Lerp(descYLower, descYUpper, _textDescAnim);
            gfx.DrawString(_desc, new Vector2(_half, descYAnim), _theme.Theme.GetFontColor(TextFontStyle.System) * _textDescAnim, _theme.Theme.GetFont(TextFontStyle.System), TextAlignment.Center, gfx.Width / 2, WrapMode.Words);

            int mutedY = descYAnim + _descMutePadV + _descHeight;
            gfx.DrawString(_paused, new Vector2(_half, mutedY), _theme.Theme.GetFontColor(TextFontStyle.Muted) * _textDescAnim, _theme.Theme.GetFont(TextFontStyle.Muted), TextAlignment.Center, gfx.Width / 2, WrapMode.Words);

            int x = gfx.X;
            int y = gfx.Y;
            int w = gfx.Width;
            int h = gfx.Height;
            gfx.ScissorRectangle = new Rectangle(_button.X, _button.Y, _button.Width, _button.Height);
            _theme.Theme.DrawButton(gfx, _buttonText, null, GetButtonState(), false, Rectangle.Empty, new Rectangle(_buttonMarginH, _buttonMarginV, _button.Width - (_buttonMarginH * 2), _button.Height - (_buttonMarginV * 2)));
            gfx.ScissorRectangle = new Rectangle(x, y, w, h);

        }

        private UIButtonState GetButtonState()
        {
            if (!_buttonHovered)
                return UIButtonState.Idle;
            return _buttonState == ButtonState.Pressed ? UIButtonState.Pressed : UIButtonState.Hover;
        }

        public void Spawn()
        {
            _animReversed = false;
            _buttonState = ButtonState.Released;
            _buttonHovered = false;
            _textHeadAnim = 0;
            _textDescAnim = 0;
            _buttonAnim = 0;
            _GameLoop.GetLayer(LayerType.Foreground).AddEntity(this);
        }

        public void OnGameExit()
        {

        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        private void CancelLogin()
        {
            _oauth.KillServer();
        }

        public void Despawn()
        {
            _animReversed = true;
        }

        public void Update(GameTime time)
        {
            var headFont = _theme.Theme.GetFont(TextFontStyle.Header1);
            var descFont = _theme.Theme.GetFont(TextFontStyle.System);
            var mutedFont = _theme.Theme.GetFont(TextFontStyle.Muted);
            var buttonFont = _theme.Theme.GetFont(TextFontStyle.Highlight);

            int halfWidth = _ui.ScreenWidth / 2;
            var headMeasure = TextRenderer.MeasureText(_head, headFont, halfWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var descMeasure = TextRenderer.MeasureText(_desc, descFont, halfWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var muteMeasure = TextRenderer.MeasureText(_paused, mutedFont, halfWidth, Plex.Engine.TextRenderers.WrapMode.Words);
            var buttonMeasure = TextRenderer.MeasureText(_buttonText, buttonFont, halfWidth, Plex.Engine.TextRenderers.WrapMode.None);

            _headHeight = (int)headMeasure.Y;
            _descHeight = (int)descMeasure.Y;
            _pausedHeight = (int)muteMeasure.Y;


            float uiHeight = headMeasure.Y + _headDescPadV + descMeasure.Y + _descMutePadV + muteMeasure.Y + _muteButtonPadV + (_buttonMarginV * 2) + buttonMeasure.Y;
            _uiHeight = (int)uiHeight;
            _textStartY = (_ui.ScreenHeight - (int)uiHeight) / 2;

            _ui.DoInput = false;

            if(_animReversed)
            {
                if (_textHeadAnim > 0F)
                {
                    _textHeadAnim = MathHelper.Clamp(_textHeadAnim - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                }
                else
                {
                    if (_textDescAnim >0)
                    {
                        _textDescAnim = MathHelper.Clamp(_textDescAnim - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    }
                    else
                    {
                        if (_buttonAnim >0)
                        {
                            _buttonAnim = MathHelper.Clamp(_buttonAnim - (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        }
                        else
                        {
                            _ui.DoInput = true;
                            _GameLoop.GetLayer(LayerType.Foreground).RemoveEntity(this);
                        }
                    }
                }

            }
            else
            {
                if(_textHeadAnim<1F)
                {
                    _textHeadAnim = MathHelper.Clamp(_textHeadAnim + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                }
                else
                {
                    if (_textDescAnim < 1)
                    {
                        _textDescAnim = MathHelper.Clamp(_textDescAnim + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                    }
                    else
                    {
                        if (_buttonAnim < 1)
                        {
                            _buttonAnim = MathHelper.Clamp(_buttonAnim + (float)time.ElapsedGameTime.TotalSeconds * 3, 0, 1);
                        }
                    }
                }
            }

            int buttonYBase = ((_textStartY + _uiHeight) - ((_buttonMarginV * 2) + (int)buttonMeasure.Y)) - ((_animReversed) ? (_ui.ScreenHeight / 10) : 0);
            int buttonYUnder = buttonYBase + ((!_animReversed) ? (_ui.ScreenHeight / 10) : 0);
            int buttonYAnim = (int)MathHelper.Lerp(buttonYUnder, buttonYBase, _buttonAnim);
            int buttonWidth = (_buttonMarginH * 2) + (int)buttonMeasure.X;
            int buttonHeight = (_buttonMarginV * 2) + (int)buttonMeasure.Y;
            _button = new Rectangle((_ui.ScreenWidth - buttonWidth) / 2, buttonYAnim, buttonWidth, buttonHeight);
        }
    }
}
