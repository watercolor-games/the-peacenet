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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Controls
{
    public partial class ColorControl : UserControl
    {
        public ColorControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public Color SelectedForeground = Color.Black;

        public Color GetColorFromCoords(Point pt)
        {
            for (int r = 0; r <= 255; r++)
            {
                //As we move to the right of the panel things should get more blue...
                //and as we go down, more red
                //and as we go down AND to the right, green.
                for (int b = 0; b <= 255; b++)
                {
                    int xy = LinearInterpolate(0, 255 * 255, pt.X * pt.Y, 0, 255);
                    int g = LinearInterpolate(0, 255 * 255, r * b, 0, 255);
                    if (pt.X == b && pt.Y == r && g == xy)
                        return Color.FromArgb(r, g, b);
                }
            }

            return Color.Empty;
        }

        public Color SelectedColor
        {
            get
            {
                return SelectedForeground;
            }
            set
            {
                SelectedForeground = value;
            }
        }

        int LastX, LastY = 0;

        private void ColorPicker_Load(object sender, EventArgs e)
        {
            pnlcolorbox.MouseMove += (o, a) =>
            {
                if (a.Button == MouseButtons.Left)
                {
                    SelectedColor = GetColorFromCoords(a.Location);
                    LastX = a.Location.X;
                    LastY = a.Location.Y;
                    DrawEverything();
                }

            };
            pnlcolorbox.Paint += (o, a) =>
            {

                float selectedX = 0;
                float selectedY = 0;
                float width = 5;
                float height = 5;

                for (int r = 0; r <= 255; r++)
                {
                    //As we move to the right of the panel things should get more blue...
                    //and as we go down, more red
                    //and as we go down AND to the right, green.
                    for (int b = 0; b <= 255; b++)
                    {
                        int g = LinearInterpolate(0, 255 * 255, r * b, 0, 255);
                        var c = Color.FromArgb(r, g, b);
                        a.Graphics.FillRectangle(new SolidBrush(c), b, r, 1, 1);
                        if (SelectedColor == c)
                        {
                            selectedX = b - 2;
                            selectedY = r - 2;
                        }
                    }
                }

                int selectedg = LinearInterpolate(0, 255 * 255, (int)(selectedX + 2 * selectedY + 2), 0, 255);
                var inverted = InvertColor((int)selectedX + 2, selectedg, (int)selectedY + 2);
                a.Graphics.DrawEllipse(new Pen(new SolidBrush(inverted), 1), selectedX, selectedY, width, height);

            };

            red.Width = 255;
            red.Left = pnlcolorbox.Left;
            red.Top = pnlcolorbox.Top + pnlcolorbox.Height + 5;
            red.Height = 20;
            red.TextChanged += (o, a) =>
            {
                if(red.Text != SelectedColor.R.ToString())
                {
                    try
                    {
                        SelectedColor = Color.FromArgb(SelectedColor.A, Convert.ToInt32(red.Text), SelectedColor.G, SelectedColor.B);
                        DrawEverything();
                    }
                    catch
                    {
                        red.Text = SelectedColor.R.ToString();
                    }
                }
            };
            this.Controls.Add(red);
            red.Show();

            green.Width = 255;
            green.Left = pnlcolorbox.Left;
            green.Top = red.Top + red.Height + 5;
            green.Height = 20;
            green.TextChanged += (o, a) =>
            {
                if (green.Text != SelectedColor.G.ToString())
                {
                    try
                    {
                        SelectedColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, Convert.ToInt32(green.Text), SelectedColor.B);
                        DrawEverything();
                    }
                    catch
                    {
                        green.Text = SelectedColor.G.ToString();
                    }
                }
            };
            this.Controls.Add(green);
            green.Show();

            blue.Width = 255;
            blue.Left = pnlcolorbox.Left;
            blue.Top = green.Top + red.Height + 5;
            blue.Height = 20;
            blue.TextChanged += (o, a) =>
            {
                if (blue.Text != SelectedColor.B.ToString())
                {
                    try
                    {
                        SelectedColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, Convert.ToInt32(blue.Text));
                        DrawEverything();
                    }
                    catch
                    {
                        blue.Text = SelectedColor.B.ToString();
                    }
                }
            };
            this.Controls.Add(blue);
            blue.Show();

            alpha.Width = 255;
            alpha.Left = pnlcolorbox.Left;
            alpha.Top = blue.Top + red.Height + 5;
            alpha.Height = 20;
            alpha.TextChanged += (o, a) =>
            {
                if (alpha.Text != SelectedColor.A.ToString())
                {
                    try
                    {
                        SelectedColor = Color.FromArgb(Convert.ToInt32(alpha.Text), SelectedColor.R, SelectedColor.G, SelectedColor.B);
                        DrawEverything();
                    }
                    catch
                    {
                        alpha.Text = SelectedColor.A.ToString();
                    }
                }
            }; this.Controls.Add(alpha);
            alpha.Show();

            preview.Width = 255;
            preview.Left = pnlcolorbox.Left;
            preview.Top = alpha.Top + red.Height + 5;
            preview.Height = 20;
            preview.Paint += (o, a) =>
            {
                int width = preview.Width / 2;
                int height = preview.Height;
                a.Graphics.FillRectangle(new SolidBrush(SelectedForeground), 0, 0, width * 2, height);
            };
            this.Controls.Add(preview);
            preview.Show();

            ControlManager.SetupControl(red);
            ControlManager.SetupControl(green);
            ControlManager.SetupControl(blue);
            ControlManager.SetupControl(alpha);


            DrawEverything();
        }


        TextBox red = new TextBox();
        TextBox green = new TextBox();
        TextBox blue = new TextBox();
        TextBox alpha = new TextBox();
        Panel preview = new Panel();


        public Color InvertColor(int r, int g, int b)
        {
            return Color.FromArgb(255 - r, 255 - g, 255 - b);
        }

        public void DrawEverything()
        {
            pnlcolorbox.Refresh();
            red.Text = SelectedColor.R.ToString();
            green.Text = SelectedColor.G.ToString();
            blue.Text = SelectedColor.B.ToString();
            alpha.Text = SelectedColor.A.ToString();
            preview.Refresh();
        }

        public int LinearInterpolate(int input_start, int input_end, int input, int output_start, int output_end)
        {
            int input_range = input_end - input_start;
            int output_range = output_end - output_start;

            return (input - input_start) * output_range / input_range + output_start;
        }

    }
}
