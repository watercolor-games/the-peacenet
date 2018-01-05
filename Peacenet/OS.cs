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
        private SplashScreenComponent _splash = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private Layer _osLayer = new Layer();
        private OSEntity _osEntity = null;
        
        public void Initiate()
        {
            _osLayer = new Layer();
            _plexgate.AddLayer(_osLayer);
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
            if(_osEntity != null)
            {
                _osLayer.RemoveEntity(_osEntity);
                _osEntity.Dispose();
                _osEntity = null;
            }
            Task.Run(() =>
            {
                if (!_server.Connected)
                {
                    _localBackend = new Backend.Backend(3252, false, Path.Combine(_appdata.GamePath, "world"));
                    _localBackend.Listen();
                    _localBackend.ServerReady.WaitOne();
                    Logger.Log("Starting internal single-player server.", LogType.Info, "peacegate");
                    _clientReady.Reset();
                    Exception err = null;
                    _server.Connect("localhost:3252", () =>
                    {
                        _clientReady.Set();
                    }, (error) =>
                    {
                        err = new Exception(error);
                        _clientReady.Set();
                    });
                    _clientReady.WaitOne();
                    if (err != null)
                    {
                        _infobox.Show("Can't start campaign", "Failed to connect to local server. This is usually a sign of a major bug.\r\n\r\n" + err.Message);
                        Shutdown();
                        return;
                    }
                    _plexgate.Invoke(startBoot);
                }
                else
                {
                    _plexgate.Invoke(startBoot);
                }
            });
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
            _osEntity = _plexgate.New<OSEntity>();
            _osLayer.AddEntity(_osEntity);
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


        public void Shutdown()
        {
            if(_osEntity != null)
            {
                _osLayer.RemoveEntity(_osEntity);
                _osEntity.Dispose();
                _osEntity = null;
            }
            _splash.Reset();
        }

        public bool IsDesktopOpen
        {
            get
            {
                if (_osEntity == null)
                    return false;
                return _osEntity.Desktop != null;
            }
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
