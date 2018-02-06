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
using System.Threading;
using Plex.Engine.Config;
using Peacenet.Server;

namespace Peacenet
{
    /// <summary>
    /// Provides the Peacegate OS engine component.
    /// </summary>
    public class OS : IEngineComponent
    {
        [Dependency]
        private SplashScreenComponent _splash = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private bool _preventStartup = false;
        private Layer _osLayer = new Layer();
        private OSEntity _osEntity = null;

        /// <summary>
        /// Gets or sets whether the OS module should prevent the desktop from starting after the kernel messages end. Useful for the tutorial.
        /// </summary>
        public bool PreventStartup
        {
            get
            {
                return _preventStartup;
            }
            set
            {
                _preventStartup = value;
            }
        }

        /// <inheritdoc/>
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

        internal void OnReady()
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
        private ItchOAuthClient _api = null;

        internal event Action WallpaperChanged;

        /// <summary>
        /// Fires the "Wallpaper Changed" event.
        /// </summary>
        public void FireWallpaperChanged()
        {
            WallpaperChanged?.Invoke();
        }

        /// <summary>
        /// Retrieves all shell folders.
        /// </summary>
        /// <returns>A list containing all shell folders.</returns>
        public IEnumerable<ShellDirectoryInformation> GetShellDirs()
        {
            string uname = "Your";
            if (_api.LoggedIn)
                uname = (_api.User.display_name.ToLower().EndsWith("s")) ? _api.User.display_name + "'" : _api.User.display_name + "'s";
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


        internal void Shutdown()
        {
            if(_osEntity != null)
            {
                _osLayer.RemoveEntity(_osEntity);
                _osEntity.Dispose();
                _osEntity = null;
            }
            _splash.Reset();
        }

        /// <summary>
        /// Gets a value indicating whether Peacegate OS's desktop is open
        /// </summary>
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

    /// <summary>
    /// Extension methods for the MonoGame content pipeline.
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Loads all resources at the given path.
        /// </summary>
        /// <typeparam name="T">The type of resource to load.</typeparam>
        /// <param name="content">The <see cref="ContentManager"/> object responsible for loading the content.</param>
        /// <param name="contentdir">The path to the content directory where the resources should be loaded from.</param>
        /// <returns>All resources of the given type loaded from the specified directory</returns>
        /// <exception cref="DirectoryNotFoundException">The specified content directory doesn't exist.</exception>
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

    /// <summary>
    /// Contains information about a Peacegate shell directory.
    /// </summary>
    public class ShellDirectoryInformation
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShellDirectoryInformation"/> class. 
        /// </summary>
        /// <param name="name">The name of the directory presented to the player.</param>
        /// <param name="path">The full, absolute path to the directory on disk.</param>
        /// <param name="texture">The icon for the directory.</param>
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

        /// <summary>
        /// Retrieves the icon for this directory.
        /// </summary>
        public Texture2D Texture { get; private set; }
        /// <summary>
        /// Retrieves the user-presentable name of this directory.
        /// </summary>
        public string FriendlyName { get; private set; }
        /// <summary>
        /// Retrieves the full path of the directory.
        /// </summary>
        public string Path { get; private set; }
    }
}
