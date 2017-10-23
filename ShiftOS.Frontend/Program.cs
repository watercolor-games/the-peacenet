using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;
using Plex.Objects;
using static Plex.Engine.FSUtils;


namespace Plex.Frontend
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static event Action WorldLoadCompleted;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SkinEngine.SetSkinProvider(new PlexSkinProvider());
            AudioPlayerSubsystem.Init(new AudioPlayer());
            //Let's get localization going.
            Localization.RegisterProvider(new MonoGameLanguageProvider());
            FileSkimmerBackend.Init(new MGFSLayer());
            OutOfBoxExperience.Init(new MonoGameOOBE());
            //Now we can initiate the Infobox subsystem
            Engine.Infobox.Init(new Infobox());
            //First things first, let's initiate the window manager.
            AppearanceManager.Initiate(new Desktop.WindowManager());
            //Cool. Now the engine's window management system talks to us.
            //Let's initiate the engine just for a ha.
            //Also initiate the desktop
            Engine.Desktop.Init(new Desktop.Desktop());

            TextControl _status = new TextControl();
            _status.Font = new System.Drawing.Font("Monda", 25f, System.Drawing.FontStyle.Regular);
            _status.Text = "Project: Plex is generating the world...\r\nPlease be patient.";
            _status.AutoSize = false;
            _status.X = 0;
            _status.Y = 0;
            _status.Alignment = Engine.GUI.TextAlignment.Middle;

            Thread ServerThread = null;

            UIManager.SinglePlayerStarted += () =>
            {
                ServerThread = new Thread(() =>
                {
                    System.Diagnostics.Debug.Print("Starting local server...");
                    Server.Program.SetServerPort(3252);
                    _status.Width = UIManager.Viewport.Width;
                    _status.Height = UIManager.Viewport.Height;
                    UIManager.AddTopLevel(_status);
                    Server.Program.LoadRanks();
                    Server.Program.LoadWorld();
                    Server.Terminal.Populate();
                    Plex.Server.Program.Main(null, false);
                });
                ServerThread.IsBackground = true;
                ServerThread.Start();
            };


            Server.Program.ServerStarted += () =>
            {
                UIManager.StopHandling(_status);
                Engine.Desktop.InvokeOnWorkerThread(() =>
                {
                    Thread.Sleep(500);
                    UIManager.ConnectToServer("localhost", 3252);
                });
            };

            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            };
            

            Story.MissionComplete += (mission) =>
            {
                var mc = new Apps.MissionComplete(mission);
                AppearanceManager.SetupDialog(mc);
            };
            using (var game = new Plexgate())
            {
                game.Initializing += () =>
                {
                    //Create a main menu
                    var mm = new MainMenu();
                    UIManager.AddTopLevel(mm);
                };
                game.Run();
            }
            if(ServerThread != null)
                if(ServerThread.ThreadState != ThreadState.Aborted)
                    ServerThread.Abort();
        }
    }

    public class AudioPlayer : IAudioPlayer
    {
        SoundPlayer _player = new SoundPlayer();

        public void Infobox()
        {
            _player.Stream = Properties.Resources.maximize;
            _player.Load();
            _player.Play();
        }

        public void Notification()
        {
            _player.Stream = Properties.Resources.openwindow;
            _player.Load();
            _player.Play();
        }

        public void Shutdown()
        {
            _player.Stream = Properties.Resources.shutdown1;
            _player.Load();
            _player.Play();
        }

        public void Startup()
        {
            _player.Stream = Properties.Resources.startup;
            _player.Load();
            _player.Play();
        }
    }

    public class PlexSkinProvider : ISkinProvider
    {
        public Skin GetEasterEggSkin()
        {
            return new PlexSkin();
        }

        public Skin GetDefaultSkin()
        {
            //todo: material design skin
            return JsonConvert.DeserializeObject<PlexSkin>(Encoding.UTF8.GetString(Properties.Resources.arnix));
        }

        public Skin ReadSkin(string pfsPath)
        {
            try
            {
                return JsonConvert.DeserializeObject<PlexSkin>(ReadAllText(pfsPath));
            }
            catch
            {
                Engine.Infobox.Show("Peacegate Initializer", "An error occurred trying to load the system UI skin. The skin has been reset.");
                return GetDefaultSkin();
            }
        }
    }

    [ShiftoriumProvider]
    public class MonoGameShiftoriumProvider : IShiftoriumProvider
    {
        public List<ShiftoriumUpgrade> GetDefaults()
        {
            return JsonConvert.DeserializeObject<List<ShiftoriumUpgrade>>(Properties.Resources.Shiftorium);
        }
    }

    public class MGFSLayer : IFileSkimmer
    {
        public string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.CommandFormat:
                    return ".cf";
                case FileType.Executable:
                    return ".saa";
                case FileType.Filesystem:
                    return ".mfs";
                case FileType.Image:
                    return ".png";
                case FileType.JSON:
                    return ".json";
                case FileType.Lua:
                    return ".lua";
                case FileType.Python:
                    return ".py";
                case FileType.Skin:
                    return ".skn";
                case FileType.TextFile:
                    return ".txt";
                default:
                    return ".scrtm";
            }
        }

        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            var fs = new Apps.FileSkimmer();
            fs.IsDialog = true;
            fs.DialogMode = style;
            fs.FileFilters = filetypes;
            fs.DialogCallback = callback;
            AppearanceManager.SetupDialog(fs);
        }

        public void OpenDirectory(string path)
        {
            if (!DirectoryExists(path))
                return;
            var fs = new Apps.FileSkimmer();
            fs.Navigate(path);
            AppearanceManager.SetupWindow(fs);
        }
    }

}
