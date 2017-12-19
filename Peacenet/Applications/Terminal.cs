using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects.Pty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine.Server;

namespace Peacenet.Applications
{
    [AppLauncher("Terminal", "System")]
    public class Terminal : Window
    {
        [Dependency]
        private TerminalManager _manager = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private TerminalEmulator _emulator = null;
        private Task _shellJob = null;
        
        public Terminal(WindowSystem _winsys) : base(_winsys)
        {
            _emulator = new TerminalEmulator(_plexgate.Content.Load<SpriteFont>("Fonts/Monospace"));
            AddChild(_emulator);

            SetWindowStyle(WindowStyle.Default);
            Width = (80 * _emulator.CharacterWidth);
            Height = (25 * _emulator.CharacterHeight);
            Title = "Terminal";
            Click += (o, a) =>
            {
                Manager.SetFocus(_emulator);
            };
        }

        public override void Show(int x = -1, int y = -1)
        {
            if(_shellJob == null)
            {
                _shellJob = Task.Run(() =>
                {
                    var ctx = _manager.CreateContext(_emulator.StdOut, _emulator.StdIn);
                    _manager.RunCommand("cli", ctx);
                    if (this.Visible == true && !this.Disposed)
                        Close();
                });
            }
            base.Show(x, y);
        }

        public override void Close()
        {
            if(_shellJob != null)
            {
                _shellJob = null;
            }
            base.Close();
        }

        private int _lastEmulatorY = -1;

        protected override void OnUpdate(GameTime time)
        {
            _emulator.X = 0;
            
            if (_emulator.Height > Height)
            {
                int diff_y = _emulator.Height - Height;
                _emulator.Y = 0 - diff_y;
            }
            else
            {
                _emulator.Y = 0;
            }
            if (_lastEmulatorY != _emulator.Y)
            {
                _lastEmulatorY = _emulator.Y;
                Invalidate(true);
            }
            _emulator.Width = Width;
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black);
        }
    }

    public class ShellCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private WatercolorAPIManager _Api = null;

        public string Description
        {
            get
            {
                return "Provides a command interpreter for the Peacegate operating system.";
            }
        }

        public string Name
        {
            get
            {
                return "cli";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string user = "user";
            string workdir = "/home";
            console.WorkingDirectory = workdir;
            while (true)
            {
                if (_Api.LoggedIn)
                    user = _Api.User.username;
                else
                    user = "user";
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                console.Write($"{user}@127.0.0.1:{console.WorkingDirectory.Replace("/home","~")}$ ");
                try
                {
                    string cmdstr = console.ReadLine();
                    if (string.IsNullOrWhiteSpace(cmdstr))
                        continue;
                    if (cmdstr == "exit")
                        return;
                    if (!_terminal.RunCommand(cmdstr, console))
                    {
                        console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Red);
                        console.WriteLine("Command not found.");
                    }
                }
                catch (Exception ex)
                {
                    console.WriteLine(ex.Message);
                }
            }
        }
    }

    public class TerminalEmulator : Control
    {
        private PseudoTerminal _master = null;
        private PseudoTerminal _slave = null;
        private StreamWriter _stdout = null;
        private StreamReader _stdin = null;
        private static SpriteFont _font = null;
        private string _textBuffer = "";
        private int _charX = 0;
        private int _charY = 0;
        private int _charWidth = 0;
        private int _charHeight = 0;

        private double _cursorAnim = 0;
        private bool _cursorOn = true;

        public int CharacterWidth
        {
            get
            {
                return _charWidth;
            }
        }

        public int CharacterHeight
        {
            get
            {
                return _charHeight;
            }
        }

        public StreamWriter StdOut
        {
            get
            {
                return _stdout;
            }
        }

        public StreamReader StdIn
        {
            get
            {
                return _stdin;
            }
        }

        public TerminalEmulator(SpriteFont font)
        {
            if (font == null)
                throw new ArgumentNullException("Console font can't be null.");
            _font = font;
            
            var csize = _font.MeasureString("#");
            _charWidth = (int)csize.X;
            _charHeight = (int)csize.Y;

            var options = new TerminalOptions();

            options.LFlag = PtyConstants.ICANON | PtyConstants.ECHO;
            options.C_cc[PtyConstants.VERASE] = (byte)'\b';
            options.C_cc[PtyConstants.VEOL] = (byte)'\r';
            options.C_cc[PtyConstants.VEOL2] = (byte)'\n';

            PseudoTerminal.CreatePair(out _master, out _slave, options);

            _stdout = new StreamWriter(_master);
            _stdin = new StreamReader(_master);
            _stdout.AutoFlush = true;

                    var ch = _slave.ReadByte();
                    if (ch != -1)
                    {
                        _textBuffer += (char)ch;
                        Invalidate(true);
                    }

            HasFocusedChanged += (o, a) =>
            {
                if (IsFocused)
                {
                    _cursorOn = true;
                    _cursorAnim = 0;
                    Invalidate(true);
                }
            };
        }

        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            if (_cursorOn != true)
                Invalidate(true);
            _cursorOn = true;
            _cursorAnim = 0;

            if(e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Microsoft.Xna.Framework.Input.Keys.V)
            {
                if(System.Windows.Forms.Clipboard.ContainsText())
                {
                    foreach(char c in System.Windows.Forms.Clipboard.GetText())
                    {
                        _slave.WriteByte((byte)c);
                    }
                }
                return;
            }

            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                _slave.WriteByte((byte)'\r');
                _slave.WriteByte((byte)'\n');
                return;
            }

            if (e.Character != null)
            {
                _slave.WriteByte((byte)e.Character);
            }

            base.OnKeyEvent(e);
        }

        private void parseEscape(string seq, GraphicsContext gfx)
        {
            switch (seq)
            {
                case "c":
                    gfx.Clear(Color.Black);
                    _charX = 0;
                    _charY = 0;
                    break;
            }
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black);
            _charX = 0;
            _charY = 0;
            bool escaped = false;
            string escSeq = "";
            for (int i = 0; i < _textBuffer.Length; i++)
            {
                char c = _textBuffer[i];
                if (c == 0x1B)
                {
                    escaped = !escaped;
                    if (escaped == true)
                    {
                        escSeq = "";
                    }
                    else
                    {
                        parseEscape(escSeq, gfx);
                    }
                    continue;
                }
                if (escaped)
                {
                    escSeq += c;
                    continue;
                }
                switch (c)
                {
                    case '\r':
                    case '\t':
                    case '\0':
                    case '\v':
                        break;
                    case '\b':
                        if (_charX == 0 && _charY == 0)
                            continue;

                        if (_charX > 0)
                            _charX--;
                        else
                        {
                            _charY--;
                            _charX = (Width / _charWidth) - 1;
                        }

                        gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, Color.Black);

                        continue;
                    case '\n':
                        _charX = 0;
                        _charY += 1;
                        break;
                    default:
                        gfx.Batch.DrawString(_font, c.ToString(), new Vector2(_charX * _charWidth, (_charY) * _charHeight), Color.White);
                        if ((_charX + 1) * _charWidth >= Width)
                        {
                            _charX = 0;
                            _charY++;
                        }
                        else
                        {
                            _charX++;
                        }
                        break;
                }
            }
           if(IsFocused && _cursorOn)
                gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, Color.White);
        }

        protected override void OnUpdate(GameTime time)
        {
            var ch = _slave.ReadByte();
            if (ch != -1)
            {
                _textBuffer += (char)ch;
                Invalidate(true);
            }

            if (Height != (_charY+1)*_charHeight)
            {
                Height = (_charY+1)*_charHeight;
            }

            _cursorAnim += time.ElapsedGameTime.TotalMilliseconds;
            if(_cursorAnim >= 500)
            {
                _cursorAnim = 0;
                _cursorOn = !_cursorOn;
                if(IsFocused)
                    Invalidate(true);
            }
        }

    }
}
