using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// Provides extremely basic multi-line text editing support. This user interface element is extremely buggy.
    /// </summary>
    public class TextEditor : Control
    {
        private string _textRaw = "";
        private string[] _lines = null;
        private int _currentIndex = 0;
        private int _currentLine = 0;
        private int _currentChar = 0;
        private SpriteFont _font = null;
        private int _lastWidth = -1;
        private bool _caretVisible = true;
        private double _caretTime = 0;
        
        /// <summary>
        /// Gets or sets the text in this control.
        /// </summary>
        public string Text
        {
            get
            {
                return _textRaw;
            }
            set
            {
                if (value == null)
                    value = "";
                value = value.Replace("\r", "");
                if (_textRaw == value)
                    return;
                _textRaw = value;
                _currentIndex = 0;
                recalculateLines();
                Invalidate(true);
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            _caretVisible = true;
            _caretTime = 0;
            if (e.Key == Keys.Back)
            {
                if (_currentIndex > 0)
                {
                    _textRaw = _textRaw.Remove(_currentIndex - 1, 1);
                    _currentIndex--;
                    recalculateLines();
                    Invalidate(true);
                }
                return;
            }
            else if (e.Key == Keys.Left)
            {
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    recalculateLines();
                    Invalidate(true);
                }
            }
            else if (e.Key == Keys.Down)
            {
                if (_currentLine < _lines.Length - 1)
                {
                    string curr = _lines[_currentLine];
                    string next = _lines[_currentLine + 1];
                    if (string.IsNullOrEmpty(curr))
                    {
                        //We're just a blank line, so we can move forward one character to go to the next line.
                        _currentIndex++;
                    }
                    else if (string.IsNullOrEmpty(next) || next.Length < _currentChar) //next line's length value is less than the cursor x, so we need to move to the end of next line.
                    {
                        int len = curr.Substring(_currentChar).Length;
                        _currentIndex += len;
                        if (_textRaw[_currentIndex] == '\n')
                            _currentIndex += 1;
                        _currentIndex += next.Length;
                    }
                    else
                    {
                        //first we need the length from the beginning of current line to current char
                        int len = curr.Substring(_currentChar).Length;
                        //and we need the length from current char index (previous line) to end of previous line
                        int plen = next.Substring(0, _currentChar).Length;
                        //move back to start of our line
                        _currentIndex += len;
                        //if we're sitting right after a newline, move back one char
                        if (_textRaw[_currentIndex] == '\n')
                            _currentIndex++;
                        //move back by plen
                        _currentIndex += plen;
                    }
                }
                else
                {
                    _currentIndex = _textRaw.Length;
                }
                recalculateLines();
                Invalidate(true);

            }
            else if (e.Key == Keys.Up)
            {
                if (_currentLine > 0)
                {
                    string curr = _lines[_currentLine];
                    string prev = _lines[_currentLine - 1];
                    if (string.IsNullOrEmpty(curr))
                    {
                        //We're just a blank line, so we can move back one character to go to the previous line.
                        _currentIndex--;
                    }
                    else if (string.IsNullOrEmpty(prev) || prev.Length < _currentChar) //Previous line is blank or has a length value smaller than the current char index, let's back up till we're on it
                    {
                        int len = curr.Substring(0, _currentChar).Length;
                        _currentIndex -= len;
                        if (_textRaw[_currentIndex - 1] == '\n')
                            _currentIndex -= 1;
                    }
                    else
                    {
                        //first we need the length from the beginning of current line to current char
                        int len = curr.Substring(0, _currentChar).Length;
                        //and we need the length from current char index (previous line) to end of previous line
                        int plen = prev.Substring(_currentChar).Length;
                        //move back to start of our line
                        _currentIndex -= len;
                        //if we're sitting right after a newline, move back one char
                        if (_textRaw[_currentIndex - 1] == '\n')
                            _currentIndex--;
                        //move back by plen
                        _currentIndex -= plen;
                    }
                }
                else
                {
                    _currentIndex = 0;
                }
                recalculateLines();
                Invalidate(true);
            }
            else if (e.Key == Keys.Right)
            {
                if (_currentIndex < _textRaw.Length)
                {
                    _currentIndex++;
                    recalculateLines();
                    Invalidate(true);
                }
            }
            else if (e.Key == Keys.Home)
            {
                if (_currentChar == 0)
                    return;
                string current = _lines[_currentLine];
                _currentIndex -= current.Substring(0, _currentChar).Length;
                recalculateLines();
                Invalidate(true);
            }
            else if (e.Key == Keys.End)
            {
                string current = _lines[_currentLine];
                if (_currentChar < current.Length)
                {
                    _currentIndex += current.Substring(_currentChar).Length;
                    recalculateLines();
                    Invalidate(true);
                }
            }
            else if (e.Key == Keys.Delete)
            {
                if(_currentIndex < _textRaw.Length-1)
                {
                    _textRaw = _textRaw.Remove(_currentIndex, 1);
                    recalculateLines();
                    Invalidate(true);
                }
            }
            else if (e.Key == Keys.Enter)
            {
                _textRaw = _textRaw.Insert(_currentIndex, "\n");
                _currentIndex++;
                recalculateLines();
                Invalidate(true);
                return;
            }
            else if (e.Character != null)
            {
                if (!_font.Characters.Contains((char)e.Character))
                    return;
                _textRaw = _textRaw.Insert(_currentIndex, e.Character.ToString());
                _currentIndex++;
                recalculateLines();
                Invalidate(true);
            }
        }

        private void recalculateLines()
        {
            int x = 0;
            int y = 0;
            int i = 0;
            string wrapped = TextRenderer.WrapText(_font, _textRaw, Width, TextRenderers.WrapMode.Letters);
            if (_currentIndex > 0)
            {
                foreach (char c in wrapped)
                {
                    if (c == '\n' && _textRaw[i] == '\n') //the user hit the ENTER key here
                    {
                        y++;
                        x = 0;
                        i++;
                    }
                    else if (c == '\n') //The text was wrapped.
                    {
                        y++;
                        x = 0;
                    }
                    else
                    {
                        x = x + 1;
                        i++;
                    }
                    if (i == _currentIndex)
                        break;
                }
            }
            _currentLine = y;
            _currentChar = x;
            _lines = wrapped.Split('\n');
        }
        
        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if(_font == null)
            {
                _font = Theme.GetFont(Themes.TextFontStyle.Mono);
            }
            if(_lastWidth != Width)
            {
                _lastWidth = Width;
                recalculateLines();
            }
            if(_caretTime>=250)
            {
                _caretTime = 0;
                _caretVisible = !_caretVisible;
                Invalidate(true);
            }
            else
            {
                _caretTime += time.ElapsedGameTime.TotalMilliseconds;
            }
            base.OnUpdate(time);
        }

        /// </inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            int lineHeight = (int)_font.MeasureString("#").Y;
            if(string.IsNullOrEmpty(_textRaw))
            {
                if(HasFocused && _caretVisible)
                    gfx.DrawRectangle(0, 0, 1, lineHeight, Theme.GetFontColor(Themes.TextFontStyle.System));
            }
            else
            {
                for(int i = 0; i < _lines.Length; i++)
                {
                    string line = _lines[i];
                    if(i == _currentLine && HasFocused)
                    {
                        gfx.DrawString(line, 0, lineHeight * i, Theme.GetFontColor(Themes.TextFontStyle.System), _font, TextAlignment.Left, 0, TextRenderers.WrapMode.None);
                        if (_caretVisible)
                        {
                            string toCaret = (_currentChar == 0) ? "" : line.Substring(0, _currentChar);
                            int caretX = (int)_font.MeasureString(toCaret).X;
                            gfx.DrawRectangle(caretX, lineHeight * i, 1, lineHeight, Theme.GetFontColor(Themes.TextFontStyle.System));
                        }
                    }
                    else
                    {
                        gfx.DrawString(line, 0, lineHeight * i, Theme.GetFontColor(Themes.TextFontStyle.System)*0.5f, _font, TextAlignment.Left, 0, TextRenderers.WrapMode.None);
                    }
                }
            }
        }
    }
}
