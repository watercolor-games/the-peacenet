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
using System.IO;

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

        public static Image DitherColor(Color source, int width, int height)
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
            return DitherImage(bmp);

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

        public static int GetClosestColor(int gray, bool eightBits, bool sixBits, bool fourBits, bool twoBits)
        {
            int newgray = gray;
            if (!eightBits)
            {
                if (sixBits)
                {
                    newgray = gray >> 2;
                }
                else
                {
                    if (fourBits)
                    {
                        newgray = (int)linear(gray, 0, 255, 0, 15) * 4;
                    }
                    else
                    {
                        if (twoBits)
                        {
                            if (gray > 127 + 63)
                            {
                                newgray = 255;
                            }
                            else if (gray > 127)
                                newgray = 127 + 63;
                            else if (gray > 63)
                            {
                                newgray = 127;
                            }
                            else if (gray > 0)
                                newgray = 63;
                            else
                                newgray = 0;
                        }
                        else
                        {
                            if (gray > 127)
                                newgray = 255;
                            else
                                newgray = 0;
                        }
                    }
                }
            }
            return newgray;
        }

#if FLOYDSTEINBERG
        public static Image DitherImage(Image source)
        {
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
            bool twentyfourbits = Shiftorium.UpgradeInstalled("color_depth_24_bits");
            if (twentyfourbits)
            {
                sourceArr.CopyTo(destArr, 0);
            }
            else
            {

                if (!sixteenBits)
                {
                    if (dithering == true)
                    {
                        if (false == true)
                        {

                        }
                        else
                        {
                            int error = 0;
                            for (int i = 0; i < destArr.Length; i += 3)
                            {
                                byte r = sourceArr[i];
                                byte g = sourceArr[i + 1];
                                byte b = sourceArr[i + 2];

                                if (SkinEngine.LoadedSkin.SystemKey == Color.FromArgb(r, g, b))
                                {
                                    destArr[i] = r;
                                    destArr[i + 1] = g;
                                    destArr[i + 2] = b;
                                    continue;
                                }


                                int gray = (((r + g + b) / 3) + error);
                                int newgray = gray;
                                newgray = GetClosestColor(gray, eightBits, sixBits, fourBits, twoBits);
                                if (newgray > 255)
                                    newgray = 255;
                                if (newgray < 0)
                                    newgray = 0;
                                error = gray - newgray;
                                destArr[i] = (byte)newgray;
                                destArr[i + 1] = (byte)newgray;
                                destArr[i + 2] = (byte)newgray;

                            }
                        }
                    }

                    else
                    {
                        for (int i = 0; i < sourceArr.Length; i += 3)
                        {
                            byte r = sourceArr[i];
                            byte g = sourceArr[i + 1];
                            byte b = sourceArr[i + 2];
                            if (SkinEngine.LoadedSkin.SystemKey == Color.FromArgb(r, g, b))
                            {
                                destArr[i] = r;
                                destArr[i + 1] = g;
                                destArr[i + 2] = b;
                                continue;
                            }

                            int gray = (r + g + b) / 3;
                            int newgray = GetClosestColor(gray, eightBits, sixBits, fourBits, twoBits);
                            destArr[i] = (byte)newgray;
                            destArr[i + 1] = (byte)newgray;
                            destArr[i + 2] = (byte)newgray;



                        }
                    }
                }
            }

            Marshal.Copy(destArr, 0, destPtr, destBytes);


            bmp.UnlockBits(destLck);



            return bmp;
        }
#endif

        private static int getIndexFromXY(int x, int y, int width)
        {
            return (width * y) + x;
        }
    }

    public class DitheringSkinPostProcessor : ISkinPostProcessor
    {
        public byte[] ProcessImage(byte[] original)
        {
            try
            {
                var img = SkinEngine.ImageFromBinary(original);
                var dithered = DitheringEngine.DitherImage(img);
                using (var mstr = new MemoryStream())
                {
                    dithered.Save(mstr, System.Drawing.Imaging.ImageFormat.Bmp);
                    return mstr.ToArray();
                }
            }
            catch
            {
                return original;
            }
        }
    }
}
