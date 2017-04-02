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
            this.HideSelection = true;
            this.Select(this.TextLength, 0);
            this.SelectionFont = ConstructFont();
            this.SelectionColor = ControlManager.ConvertColor(ConsoleEx.ForegroundColor);
            this.SelectionBackColor = ControlManager.ConvertColor(ConsoleEx.BackgroundColor);
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
            this.HideSelection = true;
            this.Select(this.TextLength, 0);
            this.SelectionFont = ConstructFont();
            this.SelectionColor = ControlManager.ConvertColor(ConsoleEx.ForegroundColor);
            this.SelectionBackColor = ControlManager.ConvertColor(ConsoleEx.BackgroundColor);
            this.AppendText(Localization.Parse(text) + Environment.NewLine);
            this.HideSelection = false;
        }

        bool quickCopying = false;

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
    }
}
