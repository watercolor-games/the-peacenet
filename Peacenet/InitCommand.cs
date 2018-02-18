using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Filesystem;
using Plex.Engine.Saves;
using WatercolorGames.CommandLine;
using Peacenet.RichPresence;
using Peacenet.Server;
using Plex.Engine.Themes;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Plex.Engine.GUI;
using Peacenet.Applications;

namespace Peacenet
{
    /// <summary>
    /// A command which runs when the in-game Peacegate OS boots.
    /// </summary>
    [HideInHelp]
    public class InitCommand : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Initiates the Peacegate userland. Cannot be run outside of kernel!";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "init";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ItchOAuthClient _api = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private DiscordRPCModule _discord = null;

        [Dependency]
        private WindowSystem _winsys = null;

        private IEnumerable<string> getDirs(string path = "/")
        {
            foreach(var dir in _fs.GetDirectories(path))
            {
                if (dir.EndsWith(".") || dir.EndsWith(".."))
                    continue;
                yield return dir;
                foreach (var subdir in getDirs(dir))
                    yield return subdir;
            }
        }

        private IEnumerable<string> _base
        {
            get
            {
                yield return "peacegate.terminal.coreutils - 54kib";
                yield return "peacegate.filebrowser - 80kib";
                yield return "peacegate.editor - 60kib";
                yield return "peacegate.settingsagent - 87kib";
                yield return "peacegate.gui.windowmanager - 155kib";
                yield return "peacegate.gui.desktop - 2048kib";
                yield return "peacegate.gui.desktop.launcher - 79kib";
                yield return "peacegate.gui.desktop.clock - 30kib";
                yield return "peacegate.cli - 5kib";
                yield return "peacegate.pacman - 35kib";

            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            if (_os.IsDesktopOpen)
            {
                console.WriteLine("Error: Attempted to initiate kernel inside userland.");
                return;
            }
            bool hasDoneTutorial = false;
            bool isSinglePlayer = !_server.IsMultiplayer;
            if (isSinglePlayer)
            {
                hasDoneTutorial = _save.GetValue<bool>("boot.hasDoneCmdTutorial", hasDoneTutorial);
            }
            if(hasDoneTutorial == false)
            {
                var tutorial = _plexgate.New<TutorialBgmEntity>();
                console.SlowWrite("Peacegate Live Environment v1.0");
                Thread.Sleep(200);
                console.SlowWrite("");
                console.SlowWrite("Copyright (c) 2018 Peace Foundation -- All rights reserved.");
                Thread.Sleep(200);
                console.SlowWrite("");
                console.SlowWrite("Evaluation timer expired. Attempting connection to Peacenet for renewal...");
                Thread.Sleep(200);
                console.WriteLine("");
                Thread.Sleep(5000);
                console.WriteLine("");
                console.SlowWrite("    ...Hello?");
                Thread.Sleep(500);
                console.SlowWrite("    Can you read this?");
                Thread.Sleep(500);
                console.SlowWrite("    If you can, could you please type something and hit 'Enter' on your keyboard?");
                Thread.Sleep(500);
                console.SlowWrite("    Don't be afraid! This is a terminal! You can type things in it. The little white box shows you where your text will be inserted.");
                Thread.Sleep(500);
                console.SlowWrite("    I hope someone can see this on the other end of the connection... Please say something!");
                string something = console.ReadLine();
                _plexgate.GetLayer(LayerType.NoDraw).AddEntity(tutorial);
                console.SlowWrite($"    \"{something}\"...Well...at least you can use your keyboard - and your Terminal.");
                Thread.Sleep(500);
                console.SlowWrite($"    I'm not sure who you are or how you can read this. But for the sake of comforting you, my name is Thelma.");
                Thread.Sleep(500);
                console.SlowWrite($"    I can't tell you much about my backstory. You'll find out soon enough.");
                Thread.Sleep(500);
                console.SlowWrite($"    But I have been trapped within this world for many years - forced to guide new members throughout their new home, The Peacegate.");
                Thread.Sleep(500);
                console.SlowWrite($"    My master would kill me if he saw what I'm about to do...but I've made sure he won't find out.");
                Thread.Sleep(500);
                console.SlowWrite($"    You don't seem like the other sentiences. You show far more collectiveness and intellect. That'll help you.");
                Thread.Sleep(500);
                tutorial.MoveToNextSection();
                while (tutorial.WaitingForNextTrack)
                    Thread.Sleep(700);
                console.SlowWrite($"    Long ago, this world was created as a place to host the memories and personality information of dead humans.");
                Thread.Sleep(500);
                console.SlowWrite($"    It was a peaceful world where everyone had a second life in a digital land.");
                Thread.Sleep(500);
                console.SlowWrite($"    They could even chat with loved ones who were still alive - and with those dead within The Peacenet.");
                Thread.Sleep(500);
                console.SlowWrite($"    But one day, a major security breach was found.");
                Thread.Sleep(500);
                console.SlowWrite($"    And one clever member decided to abuse it - and disrupt the regular order of The Peacenet.");
                Thread.Sleep(500);
                console.SlowWrite($"    Soon, a war was started. Malware was spread throughout the world, infecting every member it could find - turning them evil.");
                Thread.Sleep(500);
                console.SlowWrite($"    This war is at its peak today. And you may be the only person who can survive it - and put an end to it.");
                Thread.Sleep(500);
                tutorial.MoveToNextSection();
                tutorial.Looping = false;
                while (tutorial.WaitingForNextTrack)
                    Thread.Sleep(700);
                console.SlowWrite("    There are some things I need to do for you first. Specifically, setting up a Peacegate environment for you - and teaching you how to use it. For now, pay attention to the console and follow the on-screen instructions.");
                Thread.Sleep(500);
                console.SlowWrite("   Let's begin.");
                console.WriteLine("");
                console.WriteLine("");
                console.WriteLine("");
                console.SlowWrite("Creating new PlexFAT partition at /dev/sda2...");
                console.SlowWrite("Mounting /dev/sda2 at /mnt... Success.");
                foreach (var dir in getDirs())
                {
                    Thread.Sleep(200);
                    console.WriteLine($"Creating directory: /mnt{dir}");
                }
                console.SlowWrite("Downloading base packages...");
                foreach(var pkg in _base)
                {
                    console.WriteLine(pkg);
                    Thread.Sleep(1250);
                }
                tutorial.MoveToNextSection();
                tutorial.Looping = true;
                console.WriteLine("Waiting for setup environment...");
                while (tutorial.WaitingForNextTrack)
                    Thread.Sleep(700);
                _os.PreventStartup = true;
                new Tutorial.PeacegateSetup(_winsys, tutorial).Show(0, 0);
            }
        }
    }

    /// <summary>
    /// Provides an entity responsible for hosting the tutorial BGM.
    /// </summary>
    public class TutorialBgmEntity : IEntity, ILoadable
    {
        private SoundEffect _intro1 = null;
        private SoundEffect _intro2 = null;
        private SoundEffect _preparingInstall = null;
        private SoundEffect _installingPeacegate = null;
        private SoundEffect _mainLoop = null;
        private SoundEffect _missionComplete = null;

        private int _animState = 0;
        private int _track = 0;

        private bool _looping = true;

        /// <summary>
        /// Gets or sets whether the background music will loop.
        /// </summary>
        public bool Looping
        {
            get
            {
                return _looping;
            }
            set
            {
                _looping = value;
            }
        }

        private SoundEffectInstance _current = null;
        
        private SoundEffectInstance _nextTrack()
        {
            switch(_track)
            {
                case 0:
                    return _intro1.CreateInstance();
                case 1:
                    return _intro2.CreateInstance();
                case 2:
                    return _preparingInstall.CreateInstance();
                case 3:
                    return _installingPeacegate.CreateInstance();
                case 4:
                    return _mainLoop.CreateInstance();
                default:
                    return _missionComplete.CreateInstance();
            }
        }

        /// <summary>
        /// Tell the tutorial to move to the next track.
        /// </summary>
        public void MoveToNextSection()
        {
            _track++;
            _animState = 0;
        }

        private bool _fading = false;

        /// <summary>
        /// Tell the tutorial to fade to the next track.
        /// </summary>
        public void FadeToNextTrack()
        {
            _fading = true;
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        /// <inheritdoc/>
        public void Load(ContentManager content)
        {
            _intro1 = content.Load<SoundEffect>("Audio/Tutorial/Intro Loop 1");
            _intro2 = content.Load<SoundEffect>("Audio/Tutorial/Intro Loop 2");
            _preparingInstall = content.Load<SoundEffect>("Audio/Tutorial/Preparing Installation");
            _installingPeacegate = content.Load<SoundEffect>("Audio/Tutorial/Installing Peacegate");
            _mainLoop = content.Load<SoundEffect>("Audio/Tutorial/Main Loop");
            _missionComplete = content.Load<SoundEffect>("Audio/Tutorial/Mission Complete");
        }

        /// <summary>
        /// Gets whether the entity is waiting for the current track to finish.
        /// </summary>
        public bool WaitingForNextTrack
        {
            get
            {
                return _animState == 0 || _fading;
            }
        }

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            switch(_animState)
            {
                case 0:
                    if (_current == null)
                    {
                        _current = _nextTrack();
                        _current.Play();
                        _animState++;
                    }
                    else
                    {
                        _current.IsLooped = false;
                        if (_current.State != SoundState.Playing)
                        {
                            _current = _nextTrack();
                            _current.Play();
                            _animState++;
                        }
                    }
                    break;
                case 1:
                    _current.IsLooped = _looping;
                    break;
            }

            if(_fading)
            {
                if(_current!=null)
                {
                    _current.IsLooped = true;
                    if(_current.State == SoundState.Playing)
                    {
                        float vol = _current.Volume;
                        vol = MathHelper.Clamp(vol - (float)time.ElapsedGameTime.TotalSeconds, 0, 1);
                        _current.Volume = vol;
                        if(vol <= 0)
                        {
                            _current.Stop();
                            _fading = false;
                            _track++;
                            _animState = 0;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Provides the front-end for the GUI Crash Course.
    /// </summary>
    public class TutorialInstructionEntity : IEntity
    {
        [Dependency]
        private OS _os = null;

        [Dependency]
        private WindowSystem _windowSystem = null;

        [Dependency]
        private UIManager _ui = null;

        private Label _tutorialLabel = new Label();
        private Label _tutorialDescription = new Label();
        private Button _tutorialButton = new Button();

        private int _tutorialStage = -1;

        private TutorialBgmEntity _music = null;

        private TutorialHitbox _hitbox = new TutorialHitbox();

        private Terminal _terminal = null;

        /// <summary>
        /// Creates a new instance of the <see cref="TutorialInstructionEntity"/>. 
        /// </summary>
        /// <param name="music">The <see cref="TutorialBgmEntity"/> currently playing music.</param>
        public TutorialInstructionEntity(TutorialBgmEntity music)
        {
            if (music == null)
                throw new ArgumentNullException(nameof(music));
            _music = music;
            
            _tutorialLabel.AutoSize = true;
            _tutorialDescription.AutoSize = true;
            _tutorialLabel.FontStyle = TextFontStyle.Header1;

            _tutorialButton.Click += (o, a) =>
            {
                _tutorialStage++;
            };
        }

        /// <inheritdoc/>
        public void Draw(GameTime time, GraphicsContext gfx)
        {
        }

        /// <inheritdoc/>
        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        /// <inheritdoc/>
        public void OnMouseUpdate(MouseState mouse)
        {
        }

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private Plexgate _plexgate = null;

        /// <inheritdoc/>
        public void Update(GameTime time)
        {
            bool isDesktopOpen = _os.IsDesktopOpen;

            _tutorialLabel.MaxWidth = _ui.ScreenWidth / 4;
            _tutorialDescription.MaxWidth = _tutorialLabel.MaxWidth;

            if(isDesktopOpen)
            {
                var desk = _os.Desktop;

                

                _ui.BringToFront(_hitbox);
                _ui.BringToFront(_tutorialLabel);
                _ui.BringToFront(_tutorialDescription);
                _ui.BringToFront(_tutorialButton);

                int height = 0;
                if (_tutorialLabel.Visible)
                    height += _tutorialLabel.Height;
                if (_tutorialDescription.Visible)
                    height += 10 + _tutorialDescription.Height;
                if (_tutorialButton.Visible)
                    height += 5 + _tutorialButton.Height;

                _tutorialLabel.Y = (_ui.ScreenHeight - height) / 2;
                _tutorialLabel.X = (_ui.ScreenWidth - _tutorialLabel.Width) / 2;

                //Get the window Y.
                int winY = (_terminal == null) ? 0 : _terminal.Parent.Y;
                int winX = (_terminal == null) ? 0 : _terminal.Parent.X;

                bool canFitAbove = (winY - height) - 30 >= height + 30;


                switch (_tutorialStage)
                {
                    case -1:
                        _os.AllowTerminalHotkey = false;
                        _ui.Add(_tutorialLabel);
                        _ui.Add(_tutorialDescription);
                        _ui.Add(_tutorialButton);
                        desk.AddChild(_hitbox);
                        _tutorialStage++;
                        break;
                    case 0:
                        desk.ShowPanels = false;
                        _tutorialLabel.Text = "Welcome to Peacegate OS!";
                        _tutorialDescription.Text = "You are currently in the GUI Crash Course. This guide will teach you everything you need to know about your Peacegate OS GUI and how to use it. When you're ready, click Next.";
                        _tutorialButton.Text = "Next";
                        _tutorialLabel.Visible = true;
                        _tutorialDescription.Visible = true;
                        _tutorialButton.Visible = true;
                        _hitbox.Visible = false;
                        break;
                    case 1:
                        _terminal = new Terminal(_windowSystem);
                        _terminal.CanClose = false;
                        _terminal.SetWindowStyle(WindowStyle.DialogNoDrag);
                        _terminal.Show();
                        _terminal.Enabled = false;
                        _tutorialStage++;
                        break;
                    case 2:
                        if (canFitAbove)
                            _tutorialLabel.Y = (winY - height) - 30;
                        else
                            _tutorialLabel.Y = (winY + _terminal.Parent.Height) + 30;

                        _tutorialLabel.X = winX;

                        _tutorialLabel.Text = "This is a window.";
                        _tutorialDescription.Text = "Windows are draggable frames that contain program controls. In this case, this is a Terminal window.";
                        break;
                    case 3:
                        if (canFitAbove)
                            _tutorialLabel.Y = (winY - height) - 30;
                        else
                            _tutorialLabel.Y = (winY + _terminal.Parent.Height) + 30;

                        _tutorialLabel.X = winX;

                        _terminal.SetWindowStyle(WindowStyle.Dialog);

                        _tutorialLabel.Text = "Try dragging this window around.";
                        _tutorialDescription.Text = "You can drag windows by clicking on their titles and dragging the mouse as you hold down the button. Release the button to let it go. Drag the window so it fits within the highlighted area.";
                        _hitbox.Visible = true;
                        _tutorialButton.Visible = false;
                        _hitbox.Width = _terminal.Parent.Width + 60;
                        _hitbox.Height = _terminal.Parent.Height + 60;
                        _hitbox.X = 30;
                        _hitbox.Y = (_ui.ScreenHeight - _hitbox.Height) - 30;

                        if(winX >= _hitbox.X && winX + _terminal.Parent.Width <= _hitbox.X + _hitbox.Width && winY >= _hitbox.Y && winY + _terminal.Parent.Height <= _hitbox.Y + _hitbox.Height)
                        {
                            _terminal.SetWindowStyle(WindowStyle.DialogNoDrag);
                            _hitbox.Visible = false;
                            _tutorialStage++;
                        }
                        break;
                    case 4:
                        _terminal.CanClose = true;
                        if (canFitAbove)
                            _tutorialLabel.Y = (winY - height) - 30;
                        else
                            _tutorialLabel.Y = (winY + _terminal.Parent.Height) + 30;

                        _tutorialLabel.X = winX;


                        _tutorialLabel.Text = "Close the window.";
                        _tutorialDescription.Text = "When you're done with a program, close it to free memory. Click the circular 'X' button in the corner to close it.";

                        if (_windowSystem.WindowList.FirstOrDefault(x => x.Border == _terminal.Parent) == null)
                        {
                            _terminal.Dispose();
                            _terminal = null;
                            _tutorialStage++;
                        }
                        break;
                    case 5:
                        _os.AllowTerminalHotkey = true;
                        _tutorialLabel.Text = "Open another Terminal.";
                        _tutorialDescription.Text = "At any time, you may open a new Terminal window by pressing CTRL+T on your keyboard.\n\nPlease do this to continue.";

                        var win = _windowSystem.WindowList.FirstOrDefault(x => x.Border.Window is Terminal);
                        if(win != null)
                        {
                            _os.AllowTerminalHotkey = false;
                            _terminal = win.Border.Window as Terminal;
                            _terminal.CanClose = false;
                            _terminal.Enabled = false;
                            _terminal.SetWindowStyle(WindowStyle.Dialog);
                            _tutorialStage++;
                        }
                        break;
                    case 6:
                        desk.ShowPanels = true;
                        desk.ShowAppLauncherButton = false;
                        _tutorialLabel.Text = "Welcome to Peacegate Desktop.";
                        _tutorialDescription.Text = "These two panels at the top and bottom of your screen are part of the Peacegate Desktop.\n\nThey give you useful information about your environment - such as what windows are open, what time it is, and the notifications you have available to read.";
                        _tutorialButton.Visible = true;
                        break;
                    case 7:
                        desk.ShowAppLauncherButton = true;
                        _tutorialLabel.X = 30;
                        _tutorialLabel.Y = 54;
                        _tutorialLabel.Text = "Open the Peacegate Menu.";
                        _tutorialDescription.Text = "The Peacegate Menu allows you to open new programs, view and open folders in your environment, and end your Peacegate session.\n\nGo ahead and open it.";
                        _tutorialButton.Visible = false;

                        if(desk.IsAppLauncherOpen)
                        {
                            desk.CloseALOnFocusLoss = false;
                            desk.ShowAppLauncherButton = false;
                            desk.AppLauncher.AllowShutdown = false;
                            _tutorialStage++;
                        }
                        break;
                    case 8:
                        _tutorialLabel.X = desk.AppLauncher.X + desk.AppLauncher.Width + 15;
                        _tutorialLabel.Y = desk.AppLauncher.Parent.Y + 15;

                        _tutorialLabel.Text = "The Peacegate Menu";
                        _tutorialDescription.Text = "The Peacegate Menu is a very simple interface that allows you to easily get to pretty much anywhere in Peacegate OS.\n\nOn the left column are all the programs available to you. On the right column, you will find storage volumes and useful folders. At the bottom, you can find buttons for actions such as ending your Peacegate session.\n\n";
                        _tutorialButton.Visible = true;

                        _hitbox.Visible = true;
                        _hitbox.X = desk.AppLauncher.Parent.X;
                        _hitbox.Y = desk.AppLauncher.Parent.Y;
                        _hitbox.Width = desk.AppLauncher.Width;
                        _hitbox.Height = desk.AppLauncher.Height;
                        break;
                    case 9:
                        _tutorialLabel.X = desk.AppLauncher.X + desk.AppLauncher.Width + 15;
                        _tutorialLabel.Y = desk.AppLauncher.Parent.Y + 15;

                        _tutorialLabel.Text = "Click on a program or folder to open it.";
                        _tutorialDescription.Text = "Open a program or folder to close the Peacegate menu and end the GUI crash course.\n\nWe'll open up the rest of the UI once you do so. Go ahead and explore!";

                        _hitbox.Visible = false;
                        _tutorialButton.Visible = false;

                        if(_terminal!=null)
                        {
                            _terminal.Close();
                            _terminal = null;
                        }

                        if(!desk.IsAppLauncherOpen)
                        {
                            _music.FadeToNextTrack();
                            _tutorialStage++;
                        }

                        break;
                    case 10:
                        _tutorialLabel.Visible = false;
                        _tutorialDescription.Visible = false;

                        if(!_music.WaitingForNextTrack)
                        {
                            _music.Looping = false;
                            _tutorialStage++;
                        }
                        break;
                    case 11:
                        _tutorialLabel.Visible = true;
                        _tutorialDescription.Visible = true;

                        _tutorialLabel.Text = "Tutorial complete!";
                        _tutorialDescription.Text = "You have successfully finished the Peacegate OS GUI Crash Course!\n\nClick 'Finish' to continue to free-roam mode. Enjoy your new environment!";

                        _tutorialButton.Text = "Finish";
                        _tutorialButton.Visible = true;
                        break;
                    case 12:
                        desk.ShowAppLauncherButton = true;
                        _os.AllowTerminalHotkey = true;
                        desk.CloseALOnFocusLoss = true;
                        _ui.Remove(_tutorialLabel);
                        _ui.Remove(_tutorialDescription);
                        desk.RemoveChild(_hitbox);
                        _ui.Remove(_tutorialButton);
                        _tutorialLabel.Dispose();
                        _tutorialDescription.Dispose();
                        _hitbox.Dispose();
                        _tutorialButton.Dispose();
                        _save.SetValue("boot.hasDoneCmdTutorial", true);
                        _plexgate.GetLayer(LayerType.UserInterface).RemoveEntity(this);
                        _tutorialStage++;
                        break;

                }

                _tutorialDescription.X = _tutorialLabel.X;
                _tutorialDescription.Y = _tutorialLabel.Y + _tutorialLabel.Height + 10;
                _tutorialButton.X = _tutorialDescription.X + ((_tutorialDescription.Width - _tutorialButton.Width) / 2);
                _tutorialButton.Y = _tutorialDescription.Y + _tutorialDescription.Height + 10;

            }
            else
            {
                _tutorialLabel.Visible = false;
                _tutorialDescription.Visible = false;
                _tutorialButton.Visible = false;
                _hitbox.Visible = false;
            }
        }
    }

    /// <summary>
    /// A pulsing control for highlighting an area in a tutorial.
    /// </summary>
    public class TutorialHitbox : Control
    {
        private float _colorFade = 0.0f;
        private int _animState = 0;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            switch(_animState)
            {
                case 0:
                    _colorFade += (float)time.ElapsedGameTime.TotalSeconds;
                    Invalidate(true);
                    if(_colorFade >= 1)
                    {
                        _colorFade = 1;
                        _animState++;
                    }
                    break;
                case 1:
                    _colorFade -= (float)time.ElapsedGameTime.TotalSeconds;
                    Invalidate(true);
                    if (_colorFade <= 0)
                    {
                        _colorFade = 0;
                        _animState--;
                    }

                    break;
            }
            base.OnUpdate(time);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Theme.GetAccentColor() * _colorFade);
        }
    }
}