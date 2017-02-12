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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ShiftOS.Engine;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Discord", false, null, "Networking")]
    public partial class Discord : UserControl, IShiftOSWindow
    {
        public void OnUpgrade()
        {
        }

        public void OnSkinLoad()
        {
        }

        public void OnLoad()
        {
        }
        
        public bool OnUnload()
        {
            return true;
        }

        public Discord()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebRequest sendMessageRequest = WebRequest.Create("http://selfbot-areno.rhcloud.com/send/[" + SaveSystem.CurrentSave.Username + "@" + SaveSystem.CurrentSave.SystemName + "]: " + this.textBox1.Text);
            sendMessageRequest.GetResponse(); // It doesn't actually send the request until you use GetResponse()
        }
    }
}
