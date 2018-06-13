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
using Peacenet.Filesystem;
using System.Threading;
using Plex.Engine.Config;
using Plex.Objects;
using Peacenet.GameState;

namespace Peacenet
{
    /// <summary>
    /// Provides the Peacegate OS engine component.
    /// </summary>
    public class OS : IEngineComponent, IDisposable
    {
        [Dependency]
        private SplashScreenComponent _splash = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        private bool _preventStartup = false;
        private Layer _osLayer = new Layer();
        private OSEntity _osEntity = null;

        public string Hostname { get; private set; } = "localhost";
        
        public uint CombineToUint(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 4)
                throw new ArgumentException($"You cannot convert a {values.Length} byte array to an unsigned integer.");
            int result = 0;
            result = values[0] + (values[1] << 8) + (values[2] << 16) + (values[3] << 24);
            return (uint)result;
        }


        public uint GetIPFromString(string iPAddress)
        {
            if (string.IsNullOrWhiteSpace(iPAddress))
                throw new ArgumentException("IP string cannot be empty.");
            if (!iPAddress.Contains("."))
                throw new FormatException();
            string[] segments = iPAddress.Split('.');
            if (segments.Length != 4)
                throw new FormatException();
            byte seg1 = Convert.ToByte(segments[0]);
            byte seg2 = Convert.ToByte(segments[1]);
            byte seg3 = Convert.ToByte(segments[2]);
            byte seg4 = Convert.ToByte(segments[3]);

            return this.CombineToUint(new byte[] { seg1, seg2, seg3, seg4 });
        }


        string getHostname()
        {
            if (!_fs.FileExists("/etc/hostname"))
                return "localhost";
            string hostnameFile = _fs.ReadAllText("/etc/hostname");
            if (hostnameFile.Contains("\n"))
                return hostnameFile.Substring(0, hostnameFile.IndexOf("\n"));
            return hostnameFile;
        }

        void updHostname()
        {
            Hostname = getHostname();
        }

        void wcallback(string fname)
        {
            if (fname == "/etc/hostname")
                updHostname();
        }

        /// <inheritdoc cref="OSEntity.AllowTerminalHotkey"/>
        public bool AllowTerminalHotkey
        {
            get
            {
                return _osEntity.AllowTerminalHotkey;
            }
            set
            {
                _osEntity.AllowTerminalHotkey = value;
            }
        }

        /// <summary>
        /// Retrieves the current Peacegate OS Desktop window.
        /// </summary>
        public DesktopWindow Desktop
        {
            get
            {
                if (IsDesktopOpen == false)
                    return null;
                return _osEntity.Desktop;
            }
        }

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
            _fs.WriteOperation += wcallback;

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

        [Dependency]
        private AppDataManager _appdata = null;


        private Texture2D _peacegate = null;

        public string SinglePlayerSaveDirectory
        {
            get
            {
                return Path.Combine(_appdata.GamePath, "world");
            }
        }

        [Dependency]
        private InfoboxManager _infobox = null;

        private EventWaitHandle _clientReady = new ManualResetEvent(false);

        internal void OnReady()
        {


            if(_osEntity != null)
            {
                _osEntity.Dispose();
                _osEntity = null;
            }
            startBoot();
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

        [Dependency]
        private GameManager _game = null;

        [Dependency]
        private ConfigManager _config = null;

        public string Username
        {
            get
            {
                if (!_api.LoggedIn)
                    return "user";

                if(_game.State is SinglePlayerStateInfo)
                {
                    if (!_config.GetValue("itch.showUsername", true))
                        return "user";
                }
                return _api.User.username;
            }
        }

        /// <summary>
        /// Retrieves all shell folders.
        /// </summary>
        /// <returns>A list containing all shell folders.</returns>
        public IEnumerable<ShellDirectoryInformation> GetShellDirs()
        {
            string uname = (Username != "user") ? Username + "'s" : "Your";
            yield return new ShellDirectoryInformation($"{uname} Home", "/home", _GameLoop.Content.Load<Texture2D>("UIIcons/home"));
            yield return new ShellDirectoryInformation("Desktop", "/home/Desktop", null);
            yield return new ShellDirectoryInformation("Documents", "/home/Documents", null);
            yield return new ShellDirectoryInformation("Downloads", "/home/Downloads", null);
            yield return new ShellDirectoryInformation("Music", "/home/Music", null);
            yield return new ShellDirectoryInformation("Pictures", "/home/Pictures", null);
            yield return new ShellDirectoryInformation("512MB Hard Disk Drive", "/", null);
        }



        /// <summary>
        /// Occurs once the Peacegate desktop starts.
        /// </summary>
        public event Action SessionStart;
        
        /// <summary>
        /// Occurs once the Peacegate desktop session ends.
        /// </summary>
        public event Action SessionEnd;

        internal void EnsureProperEnvironment()
        {
            _fs.SetupSpecialFiles();

            foreach(var shellDir in GetShellDirs())
            {
                if (!_fs.DirectoryExists(shellDir.Path))
                    _fs.CreateDirectory(shellDir.Path);
            }
        }

        private void startBoot()
        {
            updHostname();
            _osEntity = _GameLoop.New<OSEntity>();
            _osEntity.SessionStarted += () => SessionStart?.Invoke();
            _GameLoop.GetLayer(LayerType.Main).AddEntity(_osEntity);
        }


        internal void Shutdown()
        {
            if(_osEntity != null)
            {
                SessionEnd?.Invoke();
                _osLayer.RemoveEntity(_osEntity);
                _osEntity.Dispose();
                _osEntity = null;
            }
            _splash.Reset();
        }

        public void Dispose()
        {
            _fs.WriteOperation -= wcallback;
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
    public struct ShellDirectoryInformation
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShellDirectoryInformation"/> class. 
        /// </summary>
        /// <param name="name">The name of the directory presented to the player.</param>
        /// <param name="path">The full, absolute path to the directory on disk.</param>
        /// <param name="texture">The icon for the directory.</param>
        public ShellDirectoryInformation(string name, string path, Texture2D texture)
        {
            if (name==null)
                throw new ArgumentNullException(nameof(name));
            if (path==null)
                throw new ArgumentNullException(nameof(path));
            FriendlyName = name;
            Path = path;
            Texture = texture;
        }

        public static bool operator ==(ShellDirectoryInformation a, ShellDirectoryInformation b)
        {
            return (a.FriendlyName == b.FriendlyName && a.Path == b.Path && a.Texture == b.Texture);
        }

        public static bool operator !=(ShellDirectoryInformation a, ShellDirectoryInformation b)
        {
            return !(a.FriendlyName == b.FriendlyName && a.Path == b.Path && a.Texture == b.Texture);
        }

        public static ShellDirectoryInformation Empty => new ShellDirectoryInformation("", "", null);

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

        public override string ToString()
        {
            return $"{FriendlyName}";
        }
    }
}
