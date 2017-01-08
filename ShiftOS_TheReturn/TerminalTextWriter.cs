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
    public class TerminalTextWriter : TextWriter
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
        

        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        public ITerminalWidget UnderlyingControl
        {
            get
            {
                return AppearanceManager.ConsoleOut;
            }
        }

        public void select()
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.SelectBottom();
                
            }));
        }

        public override void Write(char value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.Write(value.ToString());
                select();
            }));
        }

        public override void WriteLine(string value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.WriteLine(value);
                select();
            }));
        }

        public void SetLastText()
        {
        }

        public override void Write(string value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.Write(value.ToString());
                select();
            }));
        }


    }
}
