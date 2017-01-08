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

#define FLOYDSTEINBERG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using ShiftOS.Engine;
using System.Runtime.InteropServices;

namespace ShiftOS.WinForms.Tools
{
    public static class DitheringEngine
    {
        public static Color GetColor(Color source)
        {
            if (Shiftorium.UpgradeInstalled("color_depth_24_bits"))
            {
                return Color.FromArgb(source.R, source.G, source.B); //get rid of the alpha channel.
            }
            else
            {
                if (Shiftorium.UpgradeInstalled("color_depth_16_bits"))
                {
                    byte r = (byte)linear(source.R, 0, 0xFF, 0, 0x1F);
                    byte g = (byte)linear(source.G, 0, 0xFF, 0, 0x3F);
                    byte b = (byte)linear(source.B, 0, 0xFF, 0, 0x1F);

                    return Color.FromArgb(r, g, b);
                }
                else
                {
                    int gray = (source.R + source.G + source.B) / 3;

                    if (Shiftorium.UpgradeInstalled("color_depth_8_bits"))
                        return Color.FromArgb(gray, gray, gray);
                    else
                    {
                        if (Shiftorium.UpgradeInstalled("color_depth_6_bits"))
                        {
                            int gray6 = (int)linear(gray, 0, 0xFF, 0, 0x3F) * 3;

                            

                            return Color.FromArgb(gray6, gray6, gray6);
                        }
                        else
                        {
                            if (Shiftorium.UpgradeInstalled("color_depth_4_bits"))
                            {
                                int gray4 = (int)linear(linear(gray, 0, 0xFF, 0, 0xF), 0, 0xF, 0, 0xFF) * 0xF;
                                return Color.FromArgb(gray4, gray4, gray4);
                            }
                            else
                            {
                                if (Shiftorium.UpgradeInstalled("color_depth_2_bits"))
                                {
                                    int gray2 = (int)linear(linear(gray, 0, 0xFF, 0, 0x3), 0, 0x3, 0, 0xFF) * 63;
                                    return Color.FromArgb(gray2, gray2, gray2);
                                }
                                else {
                                    if (gray >= 127)
                                    {
                                        return Color.Black;
                                    }
                                    else
                                    {
                                        return Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DitherColor(Color source, int width, int height, Action<Image> result)
        {
            var bmp = new Bitmap(width + 1, height + 1);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte[] rgb = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, rgb, 0, rgb.Length);
            for (int i = 0; i < rgb.Length - 3; i += 3)
            {
                    rgb[i] = source.R;
                    rgb[i + 1] = source.G;
                    rgb[i + 2] = source.B;
            }
            Marshal.Copy(rgb, 0, data.Scan0, rgb.Length);
            bmp.UnlockBits(data);
            DitherImage(bmp, result);

        }

        static private double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

#if NODITHER
        public static void DitherImage(Image source, Action<Image> result)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                result?.Invoke(source);
            }));
        }
#endif

#if BINARIZE
        public static void DitherImage(Image source, Action<Image> result)
        {
            if (source == null)
            {
                result?.Invoke(source);
                return;
            }


            var t = new Thread(new ThreadStart(() =>
            {
                var bmp = new Bitmap(source.Width, source.Height);
                var sourceBmp = (Bitmap)source;
                int error = 0;
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {

                        Color c = sourceBmp.GetPixel(x, y);
                        int gray = ((c.R + c.G + c.B) / 3);
                        if (gray >= 127)
                        {
                            error = gray - 255;
                            bmp.SetPixel(x, y, Color.White);
                        }
                        else
                        {
                            error = gray;
                            bmp.SetPixel(x, y, Color.Black);
                        }


                    }
                }
                Desktop.InvokeOnWorkerThread(new Action(() => { result?.Invoke(bmp); }));
            }));
            t.IsBackground = true;
            t.Start();

        }
#endif

#if DITHER1D
        public static void DitherImage(Image source, Action<Image> result)
        {
            if (source == null)
            {
                result?.Invoke(source);
                return;
            }


            var t = new Thread(new ThreadStart(() =>
            {
                var bmp = new Bitmap(source.Width, source.Height);
                var sourceBmp = (Bitmap)source;
                int error = 0;
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {

                        Color c = sourceBmp.GetPixel(x, y);
                        int gray = ((c.R + c.G + c.B) / 3) + error;
                        if (gray >= 127)
                        {
                            error = gray - 255;
                            bmp.SetPixel(x, y, Color.White);
                        }
                        else
                        {
                            error = gray;
                            bmp.SetPixel(x, y, Color.Black);
                        }


                    }
                }
                Desktop.InvokeOnWorkerThread(new Action(() => { result?.Invoke(bmp); }));
            }));
            t.IsBackground = true;
            t.Start();
        }
#endif

#if FLOYDSTEINBERG
        public static void DitherImage(Image source, Action<Image> result)
        {
            if (source == null)
            {
                result?.Invoke(source);
                return;
            }


            var bmp = new Bitmap(source.Width, source.Height);
            var sourceBmp = (Bitmap)source;
            var sourceLck = sourceBmp.LockBits(new Rectangle(0, 0, sourceBmp.Width, sourceBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var destLck = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int error_r = 0;
            

            int sourceBytes = Math.Abs(sourceLck.Stride) * sourceLck.Height;
            int destBytes = Math.Abs(destLck.Stride) * destLck.Height;

            IntPtr sourcePtr = sourceLck.Scan0;
            IntPtr destPtr = destLck.Scan0;

            byte[] destArr = new byte[destBytes];
            byte[] sourceArr = new byte[sourceBytes];

            byte[] grays = new byte[destBytes];

            Marshal.Copy(sourcePtr, sourceArr, 0, sourceBytes);
            Marshal.Copy(destPtr, destArr, 0, destBytes);

            int width = Math.Abs(destLck.Stride);
            int height = destLck.Height;

            bool sixteenBits = Shiftorium.UpgradeInstalled("color_depth_16_bits");
            bool twoBits = Shiftorium.UpgradeInstalled("color_depth_2_bits");
            bool sixBits = Shiftorium.UpgradeInstalled("color_depth_6_bits");
            bool fourBits = Shiftorium.UpgradeInstalled("color_depth_4_bits");
            bool eightBits = Shiftorium.UpgradeInstalled("color_depth_8_bits");
            bool color_depth_floydsteinberg = Shiftorium.UpgradeInstalled("color_depth_floyd-steinberg_dithering");
            bool dithering = Shiftorium.UpgradeInstalled("color_depth_dithering");

            for (int y = 0; y < (destLck.Height); y++)
            {
                for (int x = 0; x < (Math.Abs(destLck.Stride)); x += 3)
                {
                    int i = getIndexFromXY(x, y, width);
                    byte red = sourceArr[i];
                    byte green = sourceArr[i + 1];
                    byte blue = sourceArr[i + 2];

                    Color oldc = Color.FromArgb(red, green, blue);
                    Color newc;

                    if (sixteenBits)
                    {
                        newc = GetColor(oldc);
                    }
                    else
                    {
                        int gray = ((oldc.R + oldc.G + oldc.B) / 3);

                        byte newgray = 0;

                        if (dithering && !color_depth_floydsteinberg)
                            gray += error_r;



                        if (eightBits)
                        {
                            newgray = (byte)gray;
                            error_r = 0;
                        }
                        else if(sixBits)
                        {
                            newgray = (byte)(linear(gray, 0, 0xFF, 0, 0x3F) * 3);
                            error_r = newgray - gray;
                        }
                        else if (fourBits)
                        {
                            newgray = (byte)(linear(gray, 0, 0xFF, 0, 0xF) * 0xF);
                            error_r = newgray - gray;
                        }
                        else if (twoBits)
                        {
                            if (gray >= 191)
                                newgray = 0xFF;
                            else if (gray >= 127)
                                newgray = Color.LightGray.R;
                            else if (gray >= 64)
                                newgray = Color.DarkGray.R;
                            else
                                newgray = 0x00;
                            error_r = newgray - gray;
                        }
                        else
                        {
                            if (gray >= 127)
                                newgray = 0xFF;
                            else
                                newgray = 0x00;
                            error_r = newgray - gray;
                        }
                        newc = Color.FromArgb(newgray, newgray, newgray);
                    }

                    int nextIndex = getIndexFromXY(x + 3, y, width);
                    int nextRow = getIndexFromXY(x, y + 1, width);
                    int nextIndexOnNextRow = getIndexFromXY(x + 3, y + 1, width);
                    int prevIndexOnNextRow = getIndexFromXY(x - 3, y + 1, width);

                    grays[i] = newc.R;
                    grays[i + 1] = newc.G;
                    grays[i + 2] = newc.B;

                    if (dithering)
                    {
                        if (color_depth_floydsteinberg)
                        {
                            if (x + 3 < width)
                            {
                                sourceArr[nextIndex] += (byte)((error_r * 7) / 16);
                                sourceArr[nextIndex + 1] += (byte)((error_r * 7) / 16);
                                sourceArr[nextIndex + 2] += (byte)((error_r * 7) / 16);
                            }
                            if (y + 1 < height)
                            {
                                sourceArr[nextRow] += (byte)((error_r) / 16);
                                sourceArr[nextRow + 1] += (byte)((error_r) / 16);
                                sourceArr[nextRow + 2] += (byte)((error_r) / 16);
                            }
                            if (x + 3 < width && y + 1 < height)
                            {
                                sourceArr[nextIndexOnNextRow] += (byte)((error_r * 3) / 16);
                                sourceArr[nextIndexOnNextRow + 1] += (byte)((error_r * 3) / 16);
                                sourceArr[nextIndexOnNextRow + 2] += (byte)((error_r * 3) / 16);

                            }
                            if (x - 3 > 0 && y + 1 < height)
                            {
                                sourceArr[prevIndexOnNextRow] += (byte)((error_r * 5) / 16);
                                sourceArr[prevIndexOnNextRow + 1] += (byte)((error_r * 5) / 16);
                                sourceArr[prevIndexOnNextRow + 2] += (byte)((error_r * 5) / 16);

                            }
                        }
                    }
                }
            }

            for (int i = 0; i < destArr.Length - 3; i++)
            {
                destArr[i] = grays[i];

            }

            Marshal.Copy(destArr, 0, destPtr, destBytes);


            bmp.UnlockBits(destLck);



            Desktop.InvokeOnWorkerThread(new Action(() => { result?.Invoke(bmp); }));

        }
#endif

        private static int getIndexFromXY(int x, int y, int width)
        {
            return (width * y) + x;
        }
    }
}
