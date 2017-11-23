using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects.Pty;

namespace Plex.Frontend.Apps
{
    [FileHandler("Shell script", ".trm", "fileicontrm")]
    [Launcher("{TITLE_TERMINAL}", false, null, "{AL_UTILITIES}")]
    [WinOpen("{WO_TERMINAL}")]
    public class Terminal : ScrollView, IPlexWindow
    {
        private bool _isOpen = true;
        private TerminalEmulator _terminal = null;
        
        public TerminalEmulator TerminalControl
        {
            get
            {
                return _terminal;
            }
        }

        public Terminal()
        {
            var measurement = TerminalEmulator.GetCharMeasurement();
            Width = (int)measurement.X * 80;
            Height = (int)measurement.Y * 25;
        }

        public void OnLoad()
        {
            _terminal = new Apps.TerminalEmulator();
            AddControl(_terminal);

            _terminal.StartShell(Shell);
        }

        public void Shell(StreamWriter stdout, StreamReader stdin)
        {
            while (_isOpen)
            {
                stdout.Write((char)0x1B);
                stdout.Write("!b");
                stdout.Write((char)0x1B);
                stdout.Write((char)0x1B);
                stdout.Write("!i");
                stdout.Write((char)0x1B);
                stdout.Write((char)0x1B);
                stdout.Write(((int)Plex.Objects.ConsoleColor.Black));
                stdout.Write(((int)Plex.Objects.ConsoleColor.Green));
                stdout.Write((char)0x1B);
                stdout.Write(TerminalBackend.ShellOverride);
                stdout.Write((char)0x1B);
                stdout.Write(((int)Plex.Objects.ConsoleColor.Black));
                stdout.Write(((int)Plex.Objects.ConsoleColor.White));
                stdout.Write((char)0x1B);
                string cmd = stdin.ReadLine();
                Task.Delay(10);
                TerminalBackend.InvokeCommand(cmd, stdout, stdin, false);
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _terminal.X = 0;
            _terminal.Y = 0;
            if (this.IsFocusedControl)
            {
                UIManager.FocusedControl = _terminal;
            }
        }

        public void OnSkinLoad()
        {
            
        }

        public bool OnUnload()
        {
            _isOpen = false;
            return true;
        }

        public void OnUpgrade()
        {
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(Microsoft.Xna.Framework.Color.Black);
        }
    }

    public class TerminalEmulator : TextControl
    {
        private static SpriteFont f_regular = null;
        private static SpriteFont f_italic = null;
        private static SpriteFont f_bold = null;
        private static SpriteFont f_bolditalic= null;

        //Hacky stuff to prevent backspacing stdout
        private int charsWrittenSinceLastKeyPress = 0; // total characters that have been written to stdout since something
                                                      // was last read from stdout
        private int totalCharsReadSinceLastEcho = 0; // total characters that have been read from stdin since something 



        internal static Vector2 GetCharMeasurement()
        {
            return f_regular.MeasureString("#");
        }


        /// <summary>
        /// Called by MainMenu - loads all our spritefonts.
        /// </summary>
        internal static void LoadFonts()
        {
            f_regular = UIManager.ContentLoader.Load<SpriteFont>("UbuntuMono-R");
            f_bold = UIManager.ContentLoader.Load<SpriteFont>("UbuntuMono-B");
            f_italic = UIManager.ContentLoader.Load<SpriteFont>("UbuntuMono-RI");
            f_bolditalic = UIManager.ContentLoader.Load<SpriteFont>("UbuntuMono-BI");

        }


        private PseudoTerminal _master = null;
        private PseudoTerminal _slave = null;
        private StreamWriter _stdout = null;
        private StreamReader _stdin = null;

        //Text buffer.
        private string _tbuffer = "";
        private int _tbufferpos = 0;

        //Information for tracking char position.
        private int _charX = 0;
        private int _charY = 0;
        private int _columnHeight = 0;
        private bool _resized = false;
        private int _linewidth = 0;

        //Info for tracking char style.
        private bool _bold = false;
        private bool _italic = false;
        private Microsoft.Xna.Framework.Color _bgColor = Microsoft.Xna.Framework.Color.Black;
        private Microsoft.Xna.Framework.Color _fgColor = Microsoft.Xna.Framework.Color.White;

        //Character size.
        private int _charWidth = 0;
        private int _charHeight = 0;

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

            Task.Run(() =>
            {
                SlaveThread(_slave);
            });
        }

        public void StartShell(Action<StreamWriter, StreamReader> shell)
        {
            Task.Run(() =>
            {
                shell?.Invoke(_stdout, _stdin);
            });
        }


        private void SlaveThread(PseudoTerminal slave)
        {

            while(this.Visible)
            {
                var ch = slave.ReadByte();

                if (ch != -1)
                {
                    if (ch == '\b' && totalCharsReadSinceLastEcho <= -1)
                    {
                        continue;
                    }


                    _tbuffer += (char)ch;

                    if (charsWrittenSinceLastKeyPress > 1)
                    {
                        totalCharsReadSinceLastEcho = 1;
                    }

                    charsWrittenSinceLastKeyPress++;

                    RequireTextRerender();
                    Invalidate();
                }
            }
        }

        private SpriteFont GetFont()
        {
            if (_bold && _italic)
                return f_bolditalic;
            if (_bold)
                return f_bold;
            if (_italic)
                return f_italic;
            return f_regular;
        }

        private bool _isEscaping = false;
        private string _escapeBuffer = "";
        
        private void InterpretEscapeString()
        {
            switch (_escapeBuffer)
            {
                case "c":
                    Clear();
                    break;
                case "b":
                    _bold = true;
                    break;
                case "!b":
                    _bold = false;
                    break;
                case "i":
                    _italic = true;
                    break;
                case "!i":
                    _italic = false;
                    break;
                default:
                    if(_escapeBuffer.Length == 2)
                    {
                        int bg = 0;
                        int fg = 0;

                        if (int.TryParse(_escapeBuffer[0].ToString(), out bg) == false || int.TryParse(_escapeBuffer[1].ToString(), out fg) == false)
                            return;
                        _bgColor = GetColor(bg);
                        _fgColor = GetColor(fg);
                    }
                    break;
            }
        }

        private Microsoft.Xna.Framework.Color GetColor(int colorCode)
        {
            switch (colorCode)
            {
                default:
                case 0:
                    return Microsoft.Xna.Framework.Color.Black;
                case 1:
                    return Microsoft.Xna.Framework.Color.White;
                case 2:
                    return Microsoft.Xna.Framework.Color.Gray;
                case 3:
                    return Microsoft.Xna.Framework.Color.OrangeRed;
                case 4:
                    return Microsoft.Xna.Framework.Color.LightGreen;
                case 5:
                    return Microsoft.Xna.Framework.Color.LightBlue;
                case 6:
                    return Microsoft.Xna.Framework.Color.Yellow;
                case 7:
                    return Microsoft.Xna.Framework.Color.Orange;
                case 8:
                    return Microsoft.Xna.Framework.Color.Purple;
                case 9:
                    return Microsoft.Xna.Framework.Color.LightPink;

            }
        }


        protected override void RenderText(GraphicsContext gfx)
        {
            gfx.Batch.End();
            gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                    RasterizerState);

            if (_resized == true)
            {
                _charX = 0;
                _charY = 0;
                _bold = false;
                _italic = false;
                _bgColor = Microsoft.Xna.Framework.Color.Black;
                _fgColor = Microsoft.Xna.Framework.Color.White;
                _tbufferpos = 0;
				gfx.Clear(_bgColor);
				_resized = false;
            }
            while (_tbufferpos != _tbuffer.Length)
            {
                char c = _tbuffer[_tbufferpos];
                byte b = (byte)c;
                _tbufferpos++;
                if (b == 0x1B)
                {
                    if (_isEscaping == true)
                    {
                        _isEscaping = false;
                        InterpretEscapeString();
                        if (_escapeBuffer == "c")
                        {
                            gfx.Clear(_bgColor);
                        }
                        continue;
                    }
                    else
                    {
                        _isEscaping = true;
                        _escapeBuffer = "";
                    }
                    continue;
                }
                if (_isEscaping)
                {
                    _escapeBuffer += c.ToString();
                }
                else
                {
                    switch (c)
                    {
                        case '\b':
                            if (_charX > 0)
                            {
                                _charX--;
                            }
                            else
                            {
                                if (_charY > 0)
                                {
                                    _charY--;
                                    _charX = _linewidth;
                                }
                            }
                            gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, _bgColor);
                            break;
                        case '\n':
                            int _rx = (_charX * _charWidth);
                            int _ry = (_charY * _charHeight);
                            int _rwidth = (Width - _rx);
                            int _rheight = (_charHeight);
                            gfx.DrawRectangle(_rx, _ry, _rwidth, _rheight, _bgColor);
                            _charX = 0;
                            _charY++;
                            int _nx = (_charX * _charWidth);
                            int _ny = (_charY * _charHeight);
                            int _nwidth = (Width - _nx);
                            int _nheight = (_charHeight);
                            gfx.DrawRectangle(_nx, _ny, _nwidth, _nheight, _bgColor);
                            break;
                        case '\t':
                            for (int i = 0; i < 4; i++)
                            {
                                if (_charX + 1 >= _linewidth)
                                {
                                    _charY++;
                                    _charX = 0;

                                }
                                else
                                    _charX++;
                            }
                            break;
                        case '\v':
                            continue;
                        default:
                            gfx.DrawRectangle(_charX * _charWidth, _charY * _charHeight, _charWidth, _charHeight, _bgColor);
                            gfx.Batch.DrawString(GetFont(), c.ToString(), new Vector2(_charX * _charWidth, _charY * _charHeight), _fgColor);
                            if (_charX + 1 > _linewidth)
                            {
                                _charY++;
                                _charX = 0;
                            }
                            else
                            {
                                _charX++;
                            }
                            break;
                    }
                }
            }
            if (_showDebug)
            {
                string text = $@"Cursor X: {this._charX}
Cursor Y: {_charY}
Cursor Width: {_charWidth}
CursorHeight: {_charHeight}

Line width (in chars): {_linewidth}
Lines: {_columnHeight}

BG: {_bgColor}
FG: {_fgColor}
Bold: {_bold}
Italic: {_italic}

Buffer position: {_tbufferpos}
Buffer requires complete redraw: {_resized}";

                gfx.DrawString(text, 0, 0, Microsoft.Xna.Framework.Color.White, new System.Drawing.Font("Monda", 9F), TextAlignment.TopLeft, Width, Engine.TextRenderers.WrapMode.Words);
            }
            gfx.Batch.End();
            gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                    RasterizerState);

        }

        private bool _showDebug = false;


        protected override void OnKeyEvent(KeyEvent e)
        {
            charsWrittenSinceLastKeyPress = 0;


            if (e.Key == Keys.I && e.ControlDown)
            {
                _showDebug = !_showDebug;
                Invalidate();
            }

            if (e.Key == Keys.Enter)
            {
                _slave.WriteByte((byte)'\r');
                _slave.WriteByte((byte)'\n');
                return;

            }

            if (e.KeyChar != '\0')
            {
                if (e.KeyChar != '\b')
                {
                    totalCharsReadSinceLastEcho++;
                }
                else if (totalCharsReadSinceLastEcho >= 0)
                {
                    totalCharsReadSinceLastEcho--;
                }
                _slave.WriteByte((byte)e.KeyChar);

            }
        }


        protected override void OnLayout(GameTime gameTime)
        {
            int w = Width;
            int h = Height;

            base.OnLayout(gameTime);

            ClearTextBufferEveryRerender = false;
            

            Width = (Parent == null) ? 1280 : Parent.Width;
            _columnHeight = Math.Max(_columnHeight, _charY+1);
            Height = _columnHeight * _charHeight;

            if (Width != w || Height != h)
            {
                _resized = true;
                RequireTextRerender();
                Invalidate();

            }

            var measure = f_regular.MeasureString("#");
            _charWidth = (int)measure.X;
            _charHeight = (int)measure.Y;

            _linewidth = (Width / _charWidth);

            if (_showDebug == true)
            {
                RequireTextRerender();
                Invalidate();
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(Microsoft.Xna.Framework.Color.Black);

            base.OnPaint(gfx, target);

        }

        protected override void AfterPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //Draw the caret.
            int x = (_charX * _charWidth);
            int y = (_charY * _charHeight);
            gfx.DrawRectangle(x, y, _charWidth, _charHeight, _fgColor);
        }

        public void Write(string text)
        {
            _stdout.Write(text);
        }

        public void WriteLine(string text)
        {
            _stdout.WriteLine(text);
        }

        public void Clear()
        {
            _tbuffer = "";
            _tbufferpos = 0;
            _resized = true;
            RequireTextRerender();
            Invalidate();
        }
    }
}
