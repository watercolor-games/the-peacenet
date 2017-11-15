using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects;
using static Plex.Engine.FSUtils;
using Plex.Engine.Theming;

namespace Plex.Frontend
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FileSkimmerBackend.Init(new MGFSLayer());
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
                    Server.Program.LoadWorld();
                    Server.Terminal.Populate();
                    Plex.Server.Program.StartFromClient(null, false);
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
            using (var game = new Plexgate("378307289502973963")) //This is the peacenet discord app ID. It's used for RPC.
            {
                game.Initializing += () =>
                {
                    //Create a main menu
                    var mm = new MainMenu();
                    UIManager.AddTopLevel(mm);
                };
                game.LoadingContent += () =>
                {
                    ThemeManager.LoadTheme(new PeacenetTheme());
                };
                game.Run();
            }
            if(ServerThread != null)
                if(ServerThread.ThreadState != ThreadState.Aborted)
                    ServerThread.Abort();
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
