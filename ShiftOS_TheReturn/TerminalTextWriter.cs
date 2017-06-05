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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Backend class for forwarding <see cref="System.Console"/> to the ShiftOS terminal. 
    /// </summary>
    public class TerminalTextWriter : TextWriter
    {
        
        public TerminalTextWriter()
        {
            ConsoleEx.OnFlush = () =>
            {
                System.Diagnostics.Debug.WriteLine("[terminal] " + buffer);
                Desktop.InvokeOnWorkerThread(() =>
                {
                    UnderlyingControl?.Write(buffer);
                    buffer = "";
                });
            };
        }

        /// <summary>
        /// Gets the encoding format for this <see cref="TextWriter"/>. God bless the Unicode Consortiem. 
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="ITerminalWidget"/> that this <see cref="TerminalTextWriter"/> is forwarding to.  
        /// </summary>
        public ITerminalWidget UnderlyingControl
        {
            get
            {
                return AppearanceManager.ConsoleOut;
            }
        }

        /// <summary>
        /// Moves the caret to the last character in the textbox.
        /// </summary>
        public void select()
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl?.SelectBottom();
                
            }));
        }

        /// <summary>
        /// Write a character to the Terminal.
        /// </summary>
        /// <param name="value">The character to write.</param>
        public override void Write(char value)
        {
            if (TerminalBackend.IsForwardingConsoleWrites)
            {
                ServerManager.SendMessage("write", $@"{{
    guid: ""{TerminalBackend.ForwardGUID}"",
    text: ""{value}""
}}");
            }
            else
            {
                buffer += value;
            }
        }

        private string buffer = "";

        public string Buffer
        {
            get
            {
                return buffer;
            }
        }

        /// <summary>
        /// Write text to the Terminal, followed by a newline.
        /// </summary>
        /// <param name="value">The text to write.</param>
        public override void WriteLine(string value)
        {
            if (TerminalBackend.IsForwardingConsoleWrites)
            {
                ServerManager.SendMessage("write", $@"{{
    guid: ""{TerminalBackend.ForwardGUID}"",
    text: ""{value + Environment.NewLine}""
}}");
            }
            else
            {

                buffer += value + Environment.NewLine;
                ConsoleEx.Flush();
            }
        }

        [Obsolete("Stub.")]
        public void SetLastText()
        {
        }

        /// <summary>
        /// Write text to the Terminal.
        /// </summary>
        /// <param name="value">The text to write.</param>
        public override void Write(string value)
        {
            if (TerminalBackend.IsForwardingConsoleWrites)
            {
                ServerManager.SendMessage("write", $@"{{
    guid: ""{TerminalBackend.ForwardGUID}"",
    text: ""{value}""
}}");
            }
            else
            {

                Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                buffer += value;
            }));
            }
        }


    }
}
