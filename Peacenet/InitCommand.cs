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
                var layer = new Layer();
                var tutorial = _plexgate.New<TutorialBgmEntity>();
                layer.AddEntity(tutorial);
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
                _plexgate.AddLayer(layer);
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
                return _animState == 0;
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
        }
    }
}