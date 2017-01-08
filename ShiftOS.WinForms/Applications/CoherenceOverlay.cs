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
using ShiftOS.Engine;
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    public partial class CoherenceOverlay : UserControl, IShiftOSWindow
    {
        public CoherenceOverlay(IntPtr handle, CoherenceCommands.RECT rect)
        {
            InitializeComponent();
            this.Load += (o, a) =>
            {
                try
                {
                    int left = this.ParentForm.Left;
                    int top = this.ParentForm.Top;
                    int oldwidth = this.ParentForm.Width;
                    int oldheight = this.ParentForm.Height;

                    var t = new Thread(new ThreadStart(() =>
                    {
                        while (CoherenceCommands.GetWindowRect(handle, ref rect))
                        {

                            if (left != rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Left = rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth;
                                    left = rect.Left - SkinEngine.LoadedSkin.LeftBorderWidth;
                                }));
                            }
                            if (top != rect.Top - SkinEngine.LoadedSkin.TitlebarHeight)
                            {
                                this.Invoke(new Action(() =>
                                {

                                    this.ParentForm.Top = rect.Top - SkinEngine.LoadedSkin.TitlebarHeight;
                                    top = rect.Top - SkinEngine.LoadedSkin.TitlebarHeight;
                                }));
                            }
                            int width = (rect.Right - rect.Left) + 1;
                            int height = (rect.Bottom - rect.Top) + 1;

                            if (oldheight != SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Height = SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth;
                                    oldheight = SkinEngine.LoadedSkin.TitlebarHeight + height + SkinEngine.LoadedSkin.BottomBorderWidth;
                                }));
                            }
                            if (oldwidth != SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    this.ParentForm.Width = SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth;
                                    oldwidth = SkinEngine.LoadedSkin.LeftBorderWidth + width + SkinEngine.LoadedSkin.RightBorderWidth;
                                }));
                            }
                        }
                    }));
                    t.IsBackground = true;
                    t.Start();
                }
                catch
                {

                }
            };
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
