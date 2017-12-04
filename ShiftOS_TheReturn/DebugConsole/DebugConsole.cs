using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Plex.Objects;
using WatercolorGames.CommandLine;
using DocoptNet;

namespace Plex.Engine.DebugConsole
{
    public class DebugConsole : IEngineComponent
    {
        [Dependency]
        private UIManager _ui = null;

        [Dependency]
        private Plexgate _plexgate = null;
        private ConsoleControl _console = null;
        private SpriteFont _monospace = null;

        private List<IDebugCommand> _commands = null;

        public void Initiate()
        {
            _commands = new List<IDebugCommand>();
            _monospace = _plexgate.Content.Load<SpriteFont>("Fonts/Monospace");
            _console = new ConsoleControl(this);
            _console.Visible = false;

            _console.StartShell((stdout, stdin) =>
            {
                stdout.WriteLine("Peacenet - Debug console");
                while (!_console.Disposed)
                {
                    stdout.Write("debug> ");
                    string[] cmd = null;
                    try
                    {
                        cmd = Tokenizer.TokenizeString(stdin.ReadLine());

                    }
                    catch(Exception ex)
                    {
                        stdout.WriteLine($"Parse error: {ex.Message}");
                        continue;
                    }
                    if (cmd.Length == 0)
                        continue;
                    string cmdname = cmd[0];
                    var cmdobj = _commands.FirstOrDefault(x => x.Name == cmdname);
                    if (cmdobj == null)
                    {
                        stdout.WriteLine("Command not found.");
                        continue;
                    }
                    string[] argv = new string[cmd.Length - 1];
                    for (int i = 0; i < argv.Length; i++)
                        argv[i] = cmd[i + 1];
                    StringBuilder _usageBuilder = new StringBuilder();
                    _usageBuilder.AppendLine(cmdobj.Name + ": " + cmdobj.Description);
                    _usageBuilder.AppendLine();
                    _usageBuilder.AppendLine("Usage: ");
                    foreach (var ustr in cmdobj.UsageStrings)
                        _usageBuilder.AppendLine($"  {cmdobj.Name} {ustr}");
                    if (!_usageBuilder.ToString().Contains("<"))
                        _usageBuilder.AppendLine($"  {cmdobj.Name}");
                    string usagestring = _usageBuilder.ToString();
                    var docopt = new Docopt();
                    var argsfinal = new Dictionary<string, object>();
                    bool hasFinished = false;
                    _plexgate.Invoke(() =>
                    {
                        try
                        {
                            var argsv = docopt.Apply(usagestring, argv, version: "Debug console", exit: false);
                            foreach (var arg in argsv)
                            {
                                if (arg.Value != null)
                                    argsfinal.Add(arg.Key, arg.Value.Value);
                                else
                                    argsfinal.Add(arg.Key, null);
                            }
                            cmdobj.Run(stdout, stdin, argsfinal);
                        }
                        catch (Exception ex)
                        {
                            stdout.WriteLine(ex.ToString());
                        }
                        hasFinished = true;
                    });
                    while (hasFinished==false) { }
                }
            });

            Logger.Log("Debug shell started - looking for commands.");
            foreach(var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IDebugCommand)))
                {
                    try
                    {
                        var cmd = (IDebugCommand)Activator.CreateInstance(type, null);
                        Logger.Log($"Found: {type.Name} - {cmd.Name}");
                        Logger.Log("Injecting dependencies...");
                        _plexgate.Inject(cmd);
                        _commands.Add(cmd);
                    }
                    catch { }
                }
            }
        }

        public SpriteFont ConsoleFont
        {
            get
            {
                return _monospace;
            }
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
            if (_console.Visible)
            {
                _console.X = 0;
                _console.Y = 0;
                _console.Width = _ui.ScreenWidth;
                _console.Height = _ui.ScreenHeight / 3;
            }
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
            if (e.Modifiers == KeyboardModifiers.Control && e.Character == '`')
            {
                if (_console.Visible == false)
                {
                    _ui.Add(_console);
                    _console.Visible = true;
                }
                else
                {
                    _ui.Remove(_console, false);
                    _console.Visible = false;
                }
            }
        }

        public void Unload()
        {
            _monospace = null;
        }
    }

    public class FPSDebugCommand : IDebugCommand
    {
        [Dependency]
        private UIManager _ui = null;

        public string Description
        {
            get
            {
                return "Toggles the frames-per-second (FPS) and RAM usage display.";
            }
        }

        public string Name
        {
            get
            {
                return "fps";
            }
        }

        public IEnumerable<string> UsageStrings
        {
            get
            {
                return new List<string>();
            }
        }

        public void Run(StreamWriter stdout, StreamReader stdin, Dictionary<string, object> args)
        {
            _ui.ShowPerfCounters = !_ui.ShowPerfCounters;
        }
    }

    public interface IDebugCommand
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> UsageStrings { get; }
        void Run(StreamWriter stdout, StreamReader stdin, Dictionary<string, object> args);
    }
}
