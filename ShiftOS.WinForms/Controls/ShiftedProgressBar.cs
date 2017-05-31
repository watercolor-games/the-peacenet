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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Controls
{
    public partial class ShiftedProgressBar : Control
    {
        public ShiftedProgressBar()
        {
            this.SizeChanged += (o, a) =>
            {
                this.Refresh();
            };
            var t = new Timer();
            t.Interval = 100;
            t.Tick += (o, a) =>
            {
                if(this.Style == ProgressBarStyle.Marquee)
                {
                    if(_marqueePos >= this.Width)
                    {
                        _marqueePos = 0 - (this.Width / 4);
                    }
                    else
                    {
                        _marqueePos++;
                    }
                    this.Refresh();
                }
            };
            t.Start();
        }

        private int _value = 0;
        private int _max = 100;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this.Refresh();
            }
        }
        public int Maximum
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                this.Refresh();
            }
        }

        public ProgressBarStyle Style
        {
            get
            {
                return SkinEngine.LoadedSkin.ProgressBarStyle;
            }
        }

        public int BlockSize
        {
            get
            {
                return SkinEngine.LoadedSkin.ProgressBarBlockSize;
            }
        }

        public Color RealBackColor
        {
            get
            {
                return SkinEngine.LoadedSkin.ProgressBarBackgroundColor;
            }
        }

        public Image RealBackgroundImage
        {
            get
            {
                return SkinEngine.GetImage("progressbarbg");
            }
        }

        public Image ProgressImage
        {
            get
            {
                return SkinEngine.GetImage("progress");
            }
        }

        public Color ProgressColor
        {
            get
            {
                return SkinEngine.LoadedSkin.ProgressColor;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                pe.Graphics.Clear(this.RealBackColor);
                if (RealBackgroundImage != null)
                {
                    pe.Graphics.FillRectangle(new TextureBrush(RealBackgroundImage), new Rectangle(0, 0, this.Width, this.Height));
                }
                switch (Style)
                {
                    case ProgressBarStyle.Continuous:
                        double width = linear(this.Value, 0, this.Maximum, 0, this.Width);
                        if (ProgressImage != null)
                        {
                            pe.Graphics.FillRectangle(new TextureBrush(ProgressImage), new RectangleF(0, 0, (float)width, this.Height));
                        }
                        else
                        {
                            pe.Graphics.FillRectangle(new SolidBrush(ProgressColor), new RectangleF(0, 0, (float)width, this.Height));
                        }
                        break;
                    case ProgressBarStyle.Blocks:
                        int block_count = this.Width / (this.BlockSize + 2);
                        int blocks = (int)linear(this.Value, 0, this.Maximum, 0, block_count);
                        for (int i = 0; i < blocks - 1; i++)
                        {
                            int position = i * (BlockSize + 2);
                            if (ProgressImage != null)
                            {
                                pe.Graphics.FillRectangle(new TextureBrush(ProgressImage), new Rectangle(position, 0, BlockSize, this.Height));

                            }
                            else
                            {
                                pe.Graphics.FillRectangle(new SolidBrush(ProgressColor), new Rectangle(position, 0, BlockSize, this.Height));
                            }
                        }
                        break;
                    case ProgressBarStyle.Marquee:
                        if (ProgressImage != null)
                        {
                            pe.Graphics.FillRectangle(new TextureBrush(ProgressImage), new Rectangle(_marqueePos, 0, this.Width / 4, this.Height));
                        }
                        else
                        {
                            pe.Graphics.FillRectangle(new SolidBrush(ProgressColor), new Rectangle(_marqueePos, 0, this.Width / 4, this.Height));
                        }
                        break;
                }
            }
            catch
            {
                pe.Graphics.Clear(Color.Black);
                string text = "Preview mode. This control can't be drawn without an initiated ShiftOS engine.";
                SizeF sz = pe.Graphics.MeasureString(text, this.Font);
                PointF loc = new PointF(
                        (this.Width - sz.Width) / 2,
                        (this.Height - sz.Height) / 2
                    );
                pe.Graphics.DrawString(text, Font, new SolidBrush(Color.White), loc);
            }
        }

        private int _marqueePos = 0;

        static private double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}
