using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects.Pty;
using System.IO;
using System.Threading;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.DebugConsole
{
    public class ConsoleControl : Control
    {
        private PseudoTerminal _master = null;
        private PseudoTerminal _slave = null;
        private StreamWriter _stdout = null;
        private StreamReader _stdin = null;
        private DebugConsole _host = null;
        private string _textBuffer = "";
        private int _charX = 0;
        private int _charY = 0;
        private int _charWidth = 0;
        private int _charHeight = 0;
        private Thread _shellThread = null;
        private int _scrollOffset = 0;
        private bool _recalcScroll = false;

        public ConsoleControl(DebugConsole host)
        {
            _host = host;

            var csize = _host.ConsoleFont.MeasureString("#");
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
                    if(ch != -1)
                    {
                        _textBuffer += (char)ch;
                        Invalidate();
                        _recalcScroll = true;
                    }
                }
            });
        }

        public void StartShell(Action<StreamWriter, StreamReader> shellAction)
        {
            if(_shellThread != null)
            {
                if(_shellThread.ThreadState == ThreadState.Running)
                {
                    _shellThread.Abort();
                    _shellThread = null;
                }
            }
            _shellThread = new Thread(() =>
            {
                shellAction?.Invoke(_stdout, _stdin);
            });
            _shellThread.IsBackground = true;
            _shellThread.Start();
        }

        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                _slave.WriteByte((byte)'\r');
                _slave.WriteByte((byte)'\n');
                return;
            }

            if(e.Character != null)
            {
                _slave.WriteByte((byte)e.Character);
            }

            base.OnKeyEvent(e);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            base.OnPaint(time, gfx);
            _charX = 0;
            _charY = 0;
            for (int i = 0; i < _textBuffer.Length; i++)
            {
                char c = _textBuffer[i];
                switch (c)
                {
                    case '\r':
                    case '\t':
                    case '\0':
                    case '\v':
                    case '\b':
                        continue;
                    case '\n':
                        _charX = 0;
                        _charY += 1;
                        break;
                    default:
                        gfx.Batch.DrawString(_host.ConsoleFont, c.ToString(), new Vector2(_charX * _charWidth, (_charY - _scrollOffset) * _charHeight), Color.White);
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
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_recalcScroll)
            {
                int _tempx = 0;
                int _tempy = 0;
                for(int i = 0; i < _textBuffer.Length; i++)
                {
                    if (_textBuffer[i] == '\r')
                        continue;
                    if (_textBuffer[i] == '\n')
                    {
                        _tempx = 0;
                        _tempy += _charHeight;
                    }

                    int next = _tempx + _charWidth;
                    if(next >= Width)
                    {
                        _tempx = 0;
                        _tempy += _charHeight;
                        continue;
                    }

                }

                if(_tempy <= Height)
                {
                    _scrollOffset = 0;
                }
                else
                {
                    int diffy = _tempy - Height;
                    _scrollOffset = (diffy / _charHeight)+1;
                }
                _recalcScroll = false;
                Invalidate();
            }
        }
    }
}
