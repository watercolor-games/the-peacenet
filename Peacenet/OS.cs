using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Cutscene;
using Plex.Engine;
using Plex.Engine.Saves;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Plex.Engine.Themes;
using Plex.Engine.Filesystem;
using Plex.Engine.Server;
using System.Threading;
using Plex.Engine.Config;

namespace Peacenet
{
    public class OS : IEngineComponent
    {
        [Dependency]
        private UIManager _ui = null;
        [Dependency]
        private SaveManager _save = null;
        [Dependency]
        private WindowSystem _winmgr = null;
        [Dependency]
        private Plexgate _plexgate = null;
        [Dependency]
        private ThemeManager _theme = null;

        //Variables for bootup.
        private float _peacegateIconOpacity = 0.0f;
        private double _peacegateRide = 0.0;


        private int _osIntroState = -1;

        private Random _rnd = new Random();
        
        public void Initiate()
        {
            _osIntroState = -1;
            _peacegate = _plexgate.Content.Load<Texture2D>("Desktop/UIIcons/Peacegate");
            _jingle = _plexgate.Content.Load<SoundEffect>("Audio/PeacegateStartup/Jingle");
            _jingleInstance = _jingle.CreateInstance();
        }

        [Dependency]
        private FSManager _fs = null;

        private readonly string[] requiredPaths = new string[]
        {
            "/home",
            "/home/Desktop",
            "/home/Documents",
            "/home/Pictures",
            "/home/Music",
            "/home/Downloads",
            "/bin",
            "/etc",
            "/etc/peacegate",
            "/root"
        };

        private Backend.Backend _localBackend = null;

        [Dependency]
        private AppDataManager _appdata = null;


        [Dependency]
        private AsyncServerManager _server = null;

        private Texture2D _peacegate = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        private EventWaitHandle _clientReady = new ManualResetEvent(false);

        public void OnReady()
        {
            if (!_server.Connected)
            {
                _localBackend = new Backend.Backend(3252, false, Path.Combine(_appdata.GamePath, "world"));
                _localBackend.Listen();
                _localBackend.ServerReady.WaitOne();
                Logger.Log("Starting internal single-player server.", LogType.Info, "peacegate");
                _clientReady.Reset();
                Exception err = null;
                _server.Connect("localhost:3252", ()=>
                {
                    _clientReady.Set();
                }, (error) =>
                {
                    err = new Exception(error);
                    _clientReady.Set();
                });
                _clientReady.WaitOne();
                if(err != null)
                {
                    _infobox.Show("Can't start campaign", "Failed to connect to local server. This is usually a sign of a major bug.\r\n\r\n" + err.Message);
                    Shutdown();
                    return;
                }
                startBoot();
            }
            else
            {
                startBoot();
            }
        }
        
        [Dependency]
        private WatercolorAPIManager _api = null;

        public IEnumerable<ShellDirectoryInformation> GetShellDirs()
        {
            string uname = "Your";
            if (_api.LoggedIn)
                uname = (string.IsNullOrWhiteSpace(_api.User.fullname)) ? _api.User.username + "'s" : _api.User.fullname + "'s";
            yield return new ShellDirectoryInformation($"{uname} Home", "/home", _plexgate.Content.Load<Texture2D>("UIIcons/home"));
            yield return new ShellDirectoryInformation("Desktop", "/home/Desktop", null);
            yield return new ShellDirectoryInformation("Documents", "/home/Documents", null);
            yield return new ShellDirectoryInformation("Downloads", "/home/Downloads", null);
            yield return new ShellDirectoryInformation("Music", "/home/Music", null);
            yield return new ShellDirectoryInformation("Pictures", "/home/Pictures", null);
            yield return new ShellDirectoryInformation("512MB Hard Disk Drive", "/", null);
        }

        private void startBoot()
        {
            try
            {
                _fs.SetBackend(new AsyncServerFSBackend());
            }
            catch(Exception ex)
            {
                _infobox.Show("Startup error", $"Could not start game session due to an error initializing the filesystem backend.\r\n\r\n{ex.Message}");
                Shutdown();
                return;
            }

            _osIntroState = 0;
            foreach (var dir in requiredPaths)
            {
                try
                {
                    if (!_fs.DirectoryExists(dir))
                        _fs.CreateDirectory(dir);
                }
                catch
                {

                }
            }
        }

        private System.Drawing.Font _bootFont = new System.Drawing.Font("Monda", 60F);

        private bool _wgDeskOpen = false;

        public DesktopWindow Desktop
        {
            get;
            private set;
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            int peacegateX = (_ui.ScreenWidth - _peacegate.Width) / 2;
            int peacegateYMax = (_ui.ScreenHeight - _peacegate.Height) / 2;
            int peacegateYMin = peacegateYMax + (int)(_ui.ScreenHeight * 0.15);
            int peacegateY = (int)MathHelper.Lerp(peacegateYMin, peacegateYMax, _peacegateIconOpacity);
            ctx.BeginDraw();
            ctx.DrawRectangle(peacegateX, peacegateY, _peacegate.Width, _peacegate.Height, _peacegate, Color.White * _peacegateIconOpacity);

            int _textY = peacegateY + _peacegate.Height + 25;
            string text = "Welcome to Peacegate.";
            var measure = TextRenderer.MeasureText(text, _bootFont, int.MaxValue, TextAlignment.TopLeft, Plex.Engine.TextRenderers.WrapMode.None);
            int _textX = ((_ui.ScreenWidth - (int)measure.X) / 2);
            ctx.DrawString(text, _textX, _textY, Color.White * _peacegateIconOpacity, _bootFont, TextAlignment.TopLeft, int.MaxValue, Plex.Engine.TextRenderers.WrapMode.None);

            ctx.EndDraw();
        }

        private Applications.SystemInitTerminal _init = null;

        private SoundEffect _jingle = null;
        private SoundEffectInstance _jingleInstance = null;

        public void OnGameUpdate(GameTime time)
        {
            switch (_osIntroState)
            {
                case 0:
                    _init = new Applications.SystemInitTerminal(_winmgr);
                    _init.Show();
                    _osIntroState++;
                    break;

                case 1:
                    if (_init.Visible == false || _init.Disposed == true)
                        _osIntroState++;
                    break;
                case 2:
                    if (_jingleInstance.State != SoundState.Playing)
                        _jingleInstance.Play();
                    _peacegateIconOpacity += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacegateIconOpacity >= 1F)
                    {
                        _peacegateRide = 0;
                        _osIntroState++;
                    }
                    break;
                case 3:
                    if (_jingleInstance.State == SoundState.Stopped)
                        _osIntroState++;
                    break;
                case 4:
                    _peacegateIconOpacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if(_peacegateIconOpacity<=0)
                    {
                        _osIntroState++;
                    }
                    break;
                case 5:
                    _wgDeskOpen = true;
                    var desk = new DesktopWindow(_winmgr);
                    desk.Show();
                    Desktop = desk;
                    _osIntroState = -1;
                    break;
            }
        }

        public bool IsDesktopOpen
        {
            get
            {
                return _wgDeskOpen;
            }
        }

        public void Shutdown()
        {
            _wgDeskOpen = false;
            if (_localBackend != null)
                _localBackend.Shutdown();
            _localBackend = null;
            if(_server.Connected)
                _server?.Disconnect();
            Desktop = null;
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            if (_wgDeskOpen)
            {
                if(e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Microsoft.Xna.Framework.Input.Keys.T)
                {
                    var term = new Applications.Terminal(_winmgr);
                    term.Show();
                }
            }
        }

        public void Unload()
        {
            _peacegate.Dispose();
        }
    }

    public static class ContentHelper
    {
        public static T[] LoadAllIn<T>(this ContentManager content, string contentdir)
        {
            List<T> _contentList = new List<T>();
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + contentdir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                try
                {
                    _contentList.Add(content.Load<T>(contentdir + "/" + key));
                }
                catch { }
            }
            return _contentList.ToArray();
        }
    }

    public class ShellDirectoryInformation
    {
        public ShellDirectoryInformation(string name, string path, Texture2D texture)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            FriendlyName = name;
            Path = path;
            Texture = texture;
        }

        public Texture2D Texture { get; private set; }
        public string FriendlyName { get; private set; }
        public string Path { get; private set; }
    }
}
