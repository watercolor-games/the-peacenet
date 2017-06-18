/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Controls
{
    public class TerminalBox : RichTextBox, ITerminalWidget
    {
        public void SelectBottom()
        {
            try
            {
                this.Select(this.Text.Length, 0);
                this.ScrollToCaret();
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
                if(AppearanceManager.ConsoleOut == this)
                AppearanceManager.ConsoleOut = null;
            base.Dispose(disposing);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Select(this.TextLength, 0);
        }

        public void Write(string text)
        {
            Thread.Sleep(5);
            this.HideSelection = true;
            this.SelectionColor = ControlManager.ConvertColor(ConsoleEx.ForegroundColor);
            this.SelectionBackColor = ControlManager.ConvertColor(ConsoleEx.BackgroundColor);
            this.SelectionFont = ConstructFont();
            this.AppendText(Localization.Parse(text));
            this.HideSelection = false;
        }

        private Font ConstructFont()
        {
            FontStyle fs = FontStyle.Regular;
            if (ConsoleEx.Bold)
                fs = fs | FontStyle.Bold;
            if (ConsoleEx.Italic)
                fs = fs | FontStyle.Italic;
            if (ConsoleEx.Underline)
                fs = fs | FontStyle.Underline;

            return new Font(this.Font, fs);
        }

        public void WriteLine(string text)
        {
            Thread.Sleep(5);
            Engine.AudioManager.PlayStream(Properties.Resources.writesound);
            this.HideSelection = true;
            this.SelectionColor = ControlManager.ConvertColor(ConsoleEx.ForegroundColor);
            this.SelectionBackColor = ControlManager.ConvertColor(ConsoleEx.BackgroundColor);
            this.SelectionFont = ConstructFont();
            this.Select(this.TextLength, 0);
            this.AppendText(Localization.Parse(text) + Environment.NewLine);
            this.HideSelection = false;
        }

        bool quickCopying = false;

        bool busy = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //if right-clicking, then we initiate a quick-copy.
            if (e.Button == MouseButtons.Right)
                quickCopying = true;
            
            //Override the mouse event so that it's a left-click at all times.
            base.OnMouseDown(new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta));
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if(quickCopying == true)
            {
                if (!string.IsNullOrWhiteSpace(this.SelectedText))
                {
                    this.Copy();
                }
            }
            base.OnMouseUp(mevent);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!TerminalBackend.InStory)
            {
                switch (e.KeyCode) {
                    case Keys.Add:
                    case Keys.Alt:
                    case Keys.Apps:
                    case Keys.Attn:
                    case Keys.BrowserBack:
                    case Keys.BrowserFavorites:
                    case Keys.BrowserForward:
                    case Keys.BrowserHome:
                    case Keys.BrowserRefresh:
                    case Keys.BrowserSearch:
                    case Keys.BrowserStop:
                    case Keys.Cancel:
                    case Keys.Capital:
                    case Keys.Clear:
                    case Keys.Control:
                    case Keys.ControlKey:
                    case Keys.Crsel:
                    case Keys.Decimal:
                    case Keys.Divide:
                    case Keys.Down:
                    case Keys.End:
                    case Keys.Enter:
                    case Keys.EraseEof:
                    case Keys.Escape:
                    case Keys.Execute:
                    case Keys.Exsel:
                    case Keys.F1:
                    case Keys.F10:
                    case Keys.F11:
                    case Keys.F12:
                    case Keys.F13:
                    case Keys.F14:
                    case Keys.F15:
                    case Keys.F16:
                    case Keys.F17:
                    case Keys.F18:
                    case Keys.F19:
                    case Keys.F2:
                    case Keys.F20:
                    case Keys.F21:
                    case Keys.F22:
                    case Keys.F23:
                    case Keys.F24:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6:
                    case Keys.F7:
                    case Keys.F8:
                    case Keys.F9:
                    case Keys.FinalMode:
                    case Keys.HanguelMode:
                    case Keys.HanjaMode:
                    case Keys.Help:
                    case Keys.Home:
                    case Keys.IMEAccept:
                    case Keys.IMEConvert:
                    case Keys.IMEModeChange:
                    case Keys.IMENonconvert:
                    case Keys.Insert:
                    case Keys.JunjaMode:
                    case Keys.KeyCode:
                    case Keys.LaunchApplication1:
                    case Keys.LaunchApplication2:
                    case Keys.LaunchMail:
                    case Keys.LButton:
                    case Keys.LControlKey:
                    case Keys.Left:
                    case Keys.LineFeed:
                    case Keys.LMenu:
                    case Keys.LShiftKey:
                    case Keys.LWin:
                    case Keys.MButton:
                    case Keys.MediaNextTrack:
                    case Keys.MediaPlayPause:
                    case Keys.MediaPreviousTrack:
                    case Keys.MediaStop:
                    case Keys.Menu:
                    case Keys.Modifiers:
                    case Keys.Multiply:
                    case Keys.Next:
                    case Keys.NoName:
                    case Keys.None:
                    case Keys.NumLock:
                    case Keys.Pa1:
                    case Keys.Packet:
                    case Keys.PageUp:
                    case Keys.Pause:
                    case Keys.Play:
                    case Keys.Print:
                    case Keys.PrintScreen:
                    case Keys.ProcessKey:
                    case Keys.RButton:
                    case Keys.RControlKey:
                    case Keys.Right:
                    case Keys.RMenu:
                    case Keys.RShiftKey:
                    case Keys.RWin:
                    case Keys.Scroll:
                    case Keys.Select:
                    case Keys.SelectMedia:
                    case Keys.Separator:
                    case Keys.Shift:
                    case Keys.ShiftKey:
                    case Keys.Sleep:
                    case Keys.Subtract:
                    case Keys.Tab:
                    case Keys.Up:
                    case Keys.VolumeDown:
                    case Keys.VolumeMute:
                    case Keys.VolumeUp:
                    case Keys.XButton1:
                    case Keys.XButton2:
                    case Keys.Zoom:

                        break;
                    default:
                        //Engine.AudioManager.PlayStream(Properties.Resources.typesound); // infernal beeping noise only enable for the trailers
                        break;
            }
        }
        }

        public string ThreadId = "";

        public TerminalBox() : base()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            this.Tag = "keepbg keepfg keepfont";
        }
    }
}
