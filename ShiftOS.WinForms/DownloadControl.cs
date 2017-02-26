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
using System.Windows.Forms;
using ShiftOS.WinForms.Applications;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    public partial class DownloadControl : UserControl
    {
        public DownloadControl(int index)
        {
            InitializeComponent();
            var d = DownloadManager.Downloads[index];
            lbshiftneturl.Text = d.ShiftnetUrl;
            pcicon.Image = FileSkimmerBackend.GetImage(d.Destination);
            int bytesTransferred = 0;
            pgprogress.Maximum = DownloadManager.Downloads[index].Bytes.Length;
            pgprogress.Value = DownloadManager.Downloads[index].Progress;
            DownloadManager.ProgressUpdate += (i, p) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        if (i == index)
                        {
                            bytesTransferred += 256;
                            pgprogress.Value = bytesTransferred;
                            lbshiftneturl.Text = $@"{d.ShiftnetUrl}
{bytesTransferred} B out of {d.Bytes.Length} B transferred at {DownloadManager.GetDownloadSpeed()} B per second.
To {d.Destination}";
                            pgprogress.Maximum = d.Bytes.Length;
                        }
                    }));
                }
                catch
                {

                }
            };
        }
    }
}
