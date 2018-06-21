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
using Peacenet.Filesystem;
using WatercolorGames.CommandLine;
using Peacenet.CoreUtils;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Plex.Engine.Themes;
using System.Dynamic;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a terminal emulator and command-line interpreter for running in-game programs in a Bash-like environment.
    /// </summary>
    [AppLauncher("Terminal", "System", "Interact with the Peacegate Bash shell")]
    public class Terminal : Window
    {
        [Dependency]
        private TerminalManager _manager = null;

        [Dependency]
        private GameLoop _plexgate = null;

        private int _scrollOffset = 0;

        private ScrollBar _scrollbar = new ScrollBar();

        private TerminalEmulator _emulator = null;
        private Task _shellJob = null;

        /// <summary>
        /// Gets a Terminal command string to be run when the Terminal window opens.
        /// </summary>
        protected virtual string Shell
        {
            get
            {
                return "cli";
            }
        }

        /// <inheritdoc/>
        public Terminal(WindowSystem _winsys) : base(_winsys)
        {
            _emulator = _plexgate.New<TerminalEmulator>();
            AddChild(_emulator);
            SetWindowStyle(WindowStyle.Default);
            Width = (80 * _emulator.CharacterWidth);
            Height = (20 * _emulator.CharacterHeight);
            Title = "Terminal";
            AddChild(_scrollbar);
        }

        /// <inheritdoc/>
        public override void Show(int x = -1, int y = -1)
        {
            if(_shellJob == null)
            {
                _shellJob = Task.Run(() =>
                {
                    var ctx = _manager.CreateContext(_emulator.StdOut, _emulator.StdIn);
                    _manager.RunCommand(Shell, ctx);
                    if (this.Visible == true && !this.Disposed)
                        Close();
                });
            }
            base.Show(x, y);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            _emulator.Terminate();
            if(_shellJob != null)
            {
                _shellJob = null;
            }
            base.Close();
        }

        private int _lastEmulatorY = -1;
        private int _lastEmulatorHeight = 0;


        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _emulator.X = 0;

            if(_lastEmulatorHeight!=_emulator.Height)
            {
                _lastEmulatorHeight = _emulator.Height;
                _scrollOffset = 0;
            }

            _scrollbar.PreferredScrollHeight = _emulator.Height;

            if (_emulator.Height > Height)
            {
                int diff_y = (_emulator.Height - Height) - _scrollOffset;
                _scrollbar.ScrollOffset = diff_y;
                _emulator.Y = 0 - diff_y;
            }
            else
            {
                _scrollbar.ScrollOffset = 0;
                _emulator.Y = 0;
            }
            if (_lastEmulatorY != _emulator.Y)
            {
                _lastEmulatorY = _emulator.Y;
            }
            _emulator.Width = Width;
            _scrollbar.Visible = (this.Parent as WindowBorder).WindowStyle != WindowStyle.NoBorder;

            if (HasFocused && !_emulator.IsFocused)
                Manager.SetFocus(_emulator);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black);
        }
    }

    /// <summary>
    /// Provides a Bash-like command-line shell for Peacenet.
    /// </summary>
    public class ShellCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private ItchOAuthClient _Api = null;

        public static event Action<CommandInstruction> OnCommandRun;

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Provides a command interpreter for the Peacegate operating system.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "cli";
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
        private FSManager _fs = null;

        private List<CommandInstruction> _history = new List<CommandInstruction>();

        public CommandInstruction[] History
        {
            get
            {
                return _history.ToArray();
            }
        }

        /// <summary>
        /// Processes and evaluates a command string with the specified <see cref="ConsoleContext"/>. 
        /// </summary>
        /// <param name="console">A console context containing an input and output stream for commands to use.</param>
        /// <param name="command">The command string to parse and evaluate.</param>
        public void ProcessCommand(ConsoleContext console, string command)
        {
            var instruction = Tokenizer.GetCommandList(command);
            var stdin = console.StandardInput;
            var stdout = console.StandardOutput;
            var nconsole = new ConsoleContext(stdout, stdin);
            nconsole.WorkingDirectory = console.WorkingDirectory;
            MemoryStream lastPipe = null;
            for (int i = 0; i < instruction.Commands.Length; i++)
            {
                bool isPiping = !(i == instruction.Commands.Length - 1);
                if (lastPipe != null)
                    lastPipe.Position = 0;
                if (isPiping)
                {
                    var memstr = new MemoryStream();
                    string worktemp = nconsole.WorkingDirectory;
                    nconsole = new ConsoleContext(new StreamWriter(memstr) { AutoFlush = true }, nconsole.StandardInput);
                    nconsole.WorkingDirectory = worktemp;
                    if (!_terminal.RunCommand(instruction.Commands[i], nconsole))
                    {
                        console.WriteLine("Command not found.");
                        return;
                    }
                    lastPipe = memstr;
                }
                else
                {
                    string worktemp = nconsole.WorkingDirectory;
                    StreamWriter cout = stdout;
                    string fpath = null;
                    if (!string.IsNullOrWhiteSpace(instruction.OutputFile))
                    {
                        fpath = instruction.OutputFile;
                        if (!fpath.StartsWith("/"))
                            fpath = worktemp + "/" + fpath;
                        fpath = _futils.Resolve(fpath);
                        if (_fs.DirectoryExists(fpath))
                            throw new IOException("Directory exists.");
                        if(instruction.OutputFileType == OutputFileType.Append)
                        {
                            if (!_fs.FileExists(fpath))
                                throw new IOException("File not found.");
                        }
                        var memstr = new MemoryStream();
                        if(instruction.OutputFileType == OutputFileType.Append)
                        {
                            var bytes = _fs.ReadAllBytes(fpath);
                            memstr.Write(bytes, 0, bytes.Length);
                            memstr.Position = bytes.Length;
                        }
                        cout = new StreamWriter(memstr) { AutoFlush = true };
                    }

                    if (lastPipe != null)
                    {
                        nconsole = new ConsoleContext(cout, new StreamReader(lastPipe));
                    }
                    else
                    {
                        nconsole = new ConsoleContext(cout, nconsole.StandardInput);
                    }
                    nconsole.WorkingDirectory = worktemp;
                    if (!_terminal.RunCommand(instruction.Commands[i], nconsole))
                    {
                        console.WriteLine("Command not found.");
                        return;
                    }
                    if (!string.IsNullOrWhiteSpace(fpath))
                    {
                        var memstr = (MemoryStream)nconsole.StandardOutput.BaseStream;
                        memstr.Position = 0;
                        var bytes = memstr.ToArray();
                        _fs.WriteAllBytes(fpath, bytes);
                    }
                }
            }
            console.WorkingDirectory = nconsole.WorkingDirectory;
        }

        public Func<CommandInstruction, bool> CommandRun = null;

        private bool _allowExit = true;

        public bool AllowExit
        {
            get
            {
                return _allowExit;
            }
            set
            {
                _allowExit = value;
            }
        }

        [Dependency]
        private OS _os = null;

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            _history = new List<CommandInstruction>();
            string user = _os.Username;
            string workdir = "/home";
            console.WorkingDirectory = workdir;
            while (true)
            {
                try
                {
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                    console.Write(user);
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                    console.Write("@");
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                    console.Write(_os.Hostname);
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                    console.Write(":");
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                    console.Write(console.WorkingDirectory.Replace("/home", "~"));
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                    console.Write("$ ");
                    console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                    string cmdstr = console.ReadLine();
                    if (string.IsNullOrWhiteSpace(cmdstr))
                        continue;
                    if (cmdstr == "exit" && _allowExit == true)
                        return;
                    ProcessCommand(console, cmdstr);
                    var instruction = Tokenizer.GetCommandList(cmdstr);
                    _history.Add(instruction);
                    OnCommandRun?.Invoke(instruction);
                    if (CommandRun?.Invoke(instruction) == true)
                        return;
                }
                catch(TerminationRequestException)
                {
                    if(_allowExit)
                        return;
                }
                catch(Exception ex)
                {
                    console.WriteLine("bash: error: " + ex.Message);
#if DEBUG
                    console.WriteLine(ex.StackTrace);
#endif
                }
            }
        }

        [Dependency]
        private FileUtils _futils = null;
    }

    /// <summary>
    /// Implements a basic terminal emulator as a Peacenet UI element.
    /// </summary>
    public class TerminalEmulator : Control, ILoadable
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
        private Color _background = Color.Black;
        private Color _foreground = Color.White;

        private double _cursorAnim = 0;
        private bool _cursorOn = true;

        /// <summary>
        /// Retrieves the width (in pixels) of a single character.
        /// </summary>
        public int CharacterWidth
        {
            get
            {
                return _charWidth;
            }
        }

        /// <summary>
        /// Retrieves the height (in pixels) of a single character.
        /// </summary>
        public int CharacterHeight
        {
            get
            {
                return _charHeight;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="StreamWriter"/> which can be used to write text to the terminal. 
        /// </summary>
        public StreamWriter StdOut
        {
            get
            {
                return _stdout;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="StreamReader"/> which can be used to read user input from the terminal. 
        /// </summary>
        public StreamReader StdIn
        {
            get
            {
                return _stdin;
            }
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TerminalEmulator"/> using the specified font to display text.
        /// </summary>
        public TerminalEmulator()
        {
 
            var options = new TerminalOptions();

            options.LFlag = PtyConstants.ICANON | PtyConstants.ECHO;
            options.C_cc[PtyConstants.VERASE] = (byte)'\b';
            options.C_cc[PtyConstants.VEOL] = (byte)'\r';
            options.C_cc[PtyConstants.VEOL2] = (byte)'\n';

            PseudoTerminal.CreatePair(out _master, out _slave, options);

            _stdout = new StreamWriter(_master);
            _stdin = new StreamReader(_master);
            _stdout.AutoFlush = true;

                    
            HasFocusedChanged += (o, a) =>
            {
                if (IsFocused)
                {
                    _cursorOn = true;
                    _cursorAnim = 0;
                }
            };
        }

        /// <summary>
        /// Requests termination of any reads on any console contexts linked to this Terminal. WILL THROW AN EXCEPTION SOMEWHERE. THAT'S HOW IT WORKS.
        /// </summary>
        public void Terminate()
        {
            _slave.WriteByte(0x02);
            _slave.WriteByte((byte)'\n');

        }

        /// <inheritdoc/>
        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
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
            if(e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Microsoft.Xna.Framework.Input.Keys.C)
            {
                Terminate();
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

        private Color GetColor(Plex.Objects.ConsoleColor color)
        {
            switch(color)
            {
                case Plex.Objects.ConsoleColor.Black:
                    return Color.Black;
                case Plex.Objects.ConsoleColor.Blue:
                    return Color.Blue;
                case Plex.Objects.ConsoleColor.Gray:
                    return Color.Gray;
                case Plex.Objects.ConsoleColor.Green:
                    return Color.Green;
                case Plex.Objects.ConsoleColor.Orange:
                    return Color.Orange;
                case Plex.Objects.ConsoleColor.Pink:
                    return Color.Pink;
                case Plex.Objects.ConsoleColor.Purple:
                    return Color.Purple;
                case Plex.Objects.ConsoleColor.Red:
                    return Color.Red;
                case Plex.Objects.ConsoleColor.White:
                    return Color.White;
                case Plex.Objects.ConsoleColor.Yellow:
                    return Color.Yellow;
            }
            return Color.Black;
        }

        private void parseEscape(string seq, GraphicsContext gfx)
        {
            int test = -1;
            if(seq.Length == 2 && int.TryParse(seq, out test))
            {
                int bg = Convert.ToInt32(seq[0].ToString());
                int fg = Convert.ToInt32(seq[1].ToString());
                this._background = GetColor((Plex.Objects.ConsoleColor)bg);
                this._foreground = GetColor((Plex.Objects.ConsoleColor)fg);

            }
            switch (seq)
            {
                case "c":
                    gfx.Clear(_background);
                    _charX = 0;
                    _charY = 0;
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            _font.DefaultCharacter = '?';
            gfx.Clear(Color.Black);
            _background = Color.Black;
            _foreground = Color.White;
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


                        gfx.FillRectangle(new Vector2(_charX * _charWidth, _charY * _charHeight), new Vector2(_charWidth, _charHeight), _background);

                        continue;
                    case '\n':
                        gfx.FillRectangle(new Vector2((_charX+1) * _charWidth, _charY * _charHeight), new Vector2(Width - ((_charX+1)*_charWidth), _charHeight), _background);
                        _charX = 0;
                        _charY += 1;
                        break;
                    case (char)0x02:
                        continue;
                    default:
                        gfx.FillRectangle(new Vector2(_charX * _charWidth, _charY * _charHeight), new Vector2(_charWidth, _charHeight), _background);
                        gfx.DrawString(_font, c.ToString(), new Vector2(_charX*_charWidth,_charY*_charHeight), _foreground);
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
                gfx.FillRectangle(new Vector2(_charX * _charWidth, _charY * _charHeight), new Vector2(_charWidth, _charHeight), _foreground);
        }


        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if(_font != Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Mono))
            {
                _font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Mono);
                var csize = _font.MeasureString("#");
                _charWidth = (int)csize.X;
                _charHeight = (int)csize.Y;
            }

            var ch = _slave.ReadByte();
            if (ch != -1)
            {
                while (ch != -1)
                {
                    if ((char)ch == '\b')
                    {
                        if (_textBuffer.Length > 0)
                        {
                            char last = _textBuffer[_textBuffer.Length - 1];
                            _textBuffer = _textBuffer.Remove(_textBuffer.Length - 1, 1);
                            if (last == (char)0x1B)
                                _textBuffer = _textBuffer.Remove(_textBuffer.LastIndexOf((char)0x1B), _textBuffer.Length - _textBuffer.LastIndexOf((char)0x1B));
                        }
                    }
                    else
                    {
                        _textBuffer += (char)ch;
                    }
                    _cursorOn = true;
                    _cursorAnim = 0;
                    ch = _slave.ReadByte();
                }
            }

            if (Height != (_charY+1)*_charHeight)
            {
                Height = (_charY+1)*_charHeight;
            }

            _cursorAnim += time.ElapsedGameTime.TotalMilliseconds;
            if (_cursorAnim >= 250)
            {
                _cursorAnim = 0;
                _cursorOn = !_cursorOn;
            }
        }

        [Dependency]
        private ThemeManager _theme = null;

        public void Load(ContentManager content)
        {
            _font = _theme.Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Mono);
            var csize = _font.MeasureString("#");
            _charWidth = (int)csize.X;
            _charHeight = (int)csize.Y;

        }
    }

    public class CustomCommandTerminal : Terminal
    {
        private string _command = null;

        public CustomCommandTerminal(string cmd, WindowSystem _winsys) : base(_winsys)
        {
            _command = cmd;
        }

        protected override string Shell => _command;
    }
}
