using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine;
using Plex.Engine.DebugConsole;
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
        private DebugConsole _debugConsole = null;

        [Dependency]
        private TerminalManager _manager = null;

        private TerminalEmulator _emulator = null;
        private Task _shellJob = null;
        
        public Terminal(WindowSystem _winsys) : base(_winsys)
        {
            _emulator = new TerminalEmulator(_debugConsole.ConsoleFont);
            AddChild(_emulator);

            SetWindowStyle(WindowStyle.Default);
            Width = (80 * _emulator.CharacterWidth);
            Height = (25 * _emulator.CharacterHeight);
            Title = "Terminal";
        }

        public override void Show(int x = -1, int y = -1)
        {
            if(_shellJob == null)
            {
                _shellJob = Task.Run(() =>
                {
                    var ctx = _manager.CreateContext(_emulator.StdOut, _emulator.StdIn);
                    _manager.RunCommand("cli", ctx);
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

        protected override void OnUpdate(GameTime time)
        {
            _emulator.X = 0;
            _emulator.Y = 0;
            _emulator.Width = Width;
            _emulator.Height = Height;
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
            while (true)
            {
                if (_Api.LoggedIn)
                    user = _Api.User.username;
                else
                    user = "user";
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                console.Write($"{user}@127.0.0.1:~$ ");
                try
                {
                    if (!_terminal.RunCommand(console.ReadLine(), console))
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
        private int _scrollOffset = 0;
        private bool _recalcScroll = false;

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

            Task.Run(() =>
            {
                while (!this.Disposed)
                {
                    var ch = _slave.ReadByte();
                    if (ch != -1)
                    {
                        _textBuffer += (char)ch;
                        Invalidate();
                        _recalcScroll = true;
                    }
                }
            });

        }

        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
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
                    _recalcScroll = true;
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
            int _lastCharX = -1;
            int _lastCharY = -1;
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
                        int newCharY = _lastCharY;
                        int newCharX = _lastCharX;

                        _lastCharX = _charX;
                        _lastCharY = _charY;

                        _charX = newCharX;
                        _charY = newCharY;

                        gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, Color.Black);

                        continue;
                    case '\n':
                        _lastCharX = _charX;
                        _lastCharY = _charY;
                        _charX = 0;
                        _charY += 1;
                        break;
                    default:
                        gfx.Batch.DrawString(_font, c.ToString(), new Vector2(_charX * _charWidth, (_charY - _scrollOffset) * _charHeight), Color.White);
                        _lastCharX = _charX;
                        _lastCharY = _charY;
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
           if(IsFocused)
                gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, Color.White);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_recalcScroll)
            {
                int _tempx = 0;
                int _tempy = 0;
                for (int i = 0; i < _textBuffer.Length; i++)
                {
                    if (_textBuffer[i] == '\r')
                        continue;
                    if (_textBuffer[i] == '\n')
                    {
                        _tempx = 0;
                        _tempy += _charHeight;
                    }

                    int next = _tempx + _charWidth;
                    if (next >= Width)
                    {
                        _tempx = 0;
                        _tempy += _charHeight;
                        continue;
                    }

                }

                if (_tempy <= Height)
                {
                    _scrollOffset = 0;
                }
                else
                {
                    int diffy = _tempy - Height;
                    _scrollOffset = (diffy / _charHeight) + 1;
                }
                _recalcScroll = false;
                Invalidate();
            }
        }

    }
}
