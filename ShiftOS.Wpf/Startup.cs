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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Wpf
{
    public static class ShiftOSSkin
    {
        public static Brush GetAppButtonHoverBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(AppButtonHoverBrushProperty);
        }

        public static void SetAppButtonHoverBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(AppButtonHoverBrushProperty, value);
        }

        public static readonly DependencyProperty AppButtonHoverBrushProperty =
            DependencyProperty.RegisterAttached(
                "AppButtonHoverBrush",
                typeof(Brush),
                typeof(ShiftOSSkin),
                new FrameworkPropertyMetadata(LoadedSkin.Menu_MenuItemSelected.CreateBrush()));
    }

    public static class Startup
    {
        private static Visibility getVisibility(string upg)
        {
            if (Shiftorium.UpgradeInstalled(upg))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;

            }
        }


        public static void Upgrade(this Control ctrl, string upg)
        {
            ctrl.Visibility = getVisibility(upg);

        }

        public static void Upgrade(this UIElement ctrl, string upg)
        {
            ctrl.Visibility = getVisibility(upg);

        }

        static public void BringToFront(this UserControl pToMove, Canvas pParent)
        {
                int currentIndex = Canvas.GetZIndex(pToMove);
                int zIndex = 0;
                int maxZ = 0;
                UserControl child;
                for (int i = 0; i < pParent.Children.Count; i++)
                {
                    if (pParent.Children[i] is UserControl &&
                        pParent.Children[i] != pToMove)
                    {
                        child = pParent.Children[i] as UserControl;
                        zIndex = Canvas.GetZIndex(child);
                        maxZ = Math.Max(maxZ, zIndex);
                        if (zIndex > currentIndex)
                        {
                            Canvas.SetZIndex(child, zIndex - 1);
                        }
                    }
                }
                Canvas.SetZIndex(pToMove, maxZ);
            
        }

        public static void SetTitle(this IShiftOSWindow win, string title)
        {
            foreach(var frm in AppearanceManager.OpenForms)
            {
                if(frm.ParentWindow == win)
                {
                    frm.Text = title;
                }
            }
        }

        public static void InitiateEngine(System.IO.TextWriter writer)
        {
            OutOfBoxExperience.Init(new OOBE());
            AppearanceManager.Initiate(new WpfWindowManager());
            Infobox.Init(new WpfInfoboxFrontend());
            FileSkimmerBackend.Init(new Applications.WpfFSFrontend());
            if (writer != null)
            {
                Console.SetOut(writer);
            }
            SaveSystem.Begin(false);
            AppearanceManager.OnExit += () =>
            {
                Environment.Exit(0);
            };
        }

        public static void SetMouseOverStyle(this Control button, string style = "")
        {
            var bg = button.Background;
            var fg = button.Foreground;
            var border = button.BorderThickness;
            var borderfg = button.BorderBrush;

            button.MouseEnter += (o, a) =>
            {
                try
                {
                    switch (style)
                    {
                        case "menuitem":
                            button.Background = LoadedSkin.Menu_MenuItemSelected.CreateBrush();
                            button.Foreground = LoadedSkin.Menu_SelectedTextColor.CreateBrush();
                            button.BorderBrush = LoadedSkin.Menu_MenuItemBorder.CreateBrush();
                            break;
                    }
                }
                catch { }
            };
            button.MouseLeave += (o, a) =>
            {
                try
                {
                    button.Background = bg;
                    button.Foreground = fg;
                    button.BorderThickness = border;
                    button.BorderBrush = borderfg;
                }
                catch
                {

                }
            };


        }

        public static void DestroyShiftOSEngine()
        {
            ServerManager.Disconnect();
        }

        public static TextBox ConsoleOut { get; set; }

        public static SolidColorBrush CreateBrush(this System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public struct Font
        {
            public FontFamily FontFamily { get; set; }
            public double FontSize { get; set; }
            public FontStyle FontStyle { get; set; }
            public FontWeight FontWeight { get; set; }
        }

        private static Font createFont(System.Drawing.Font f)
        {
            var font = new Font();
            font.FontFamily = new FontFamily(f.Name);
            font.FontSize = f.Size;
            switch (f.Style)
            {
                case System.Drawing.FontStyle.Bold:
                    font.FontWeight = FontWeights.Bold;
                    break;
                case System.Drawing.FontStyle.Italic:
                    font.FontStyle = FontStyles.Oblique;
                    break;
                default:
                case System.Drawing.FontStyle.Regular:
                    font.FontStyle = FontStyles.Normal;
                    break;


            }
            return font;
        }

        public static void SetFont(this Control ctrl, System.Drawing.Font f)
        {
            var font = createFont(f);
            ctrl.FontFamily = font.FontFamily;
            ctrl.FontSize = PointsToPixels(font.FontSize);
            ctrl.FontStyle = font.FontStyle;
            ctrl.FontWeight = font.FontWeight;
        }

        private static double PointsToPixels(double points)
        {
            return points * (96.0 / 72.0);
        }

        public static void SetFont(this TextBlock ui, System.Drawing.Font f)
        {
            var font = createFont(f);
            ui.FontFamily = font.FontFamily;
            ui.FontSize = PointsToPixels(font.FontSize);
            ui.FontStyle = font.FontStyle;
            ui.FontWeight = font.FontWeight;
        }

        public static BitmapImage ToBitmapImage(this System.Drawing.Image img)
        {
            using(var str = new MemoryStream())
            {
                img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                return ToBitmapImage(str.ToArray());
            }
        }

        public static BitmapImage ToBitmapImage(this byte[] imgSource)
        {
            using(MemoryStream ms = new MemoryStream(imgSource))
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                return img;
            }
        }
    }

    public class WpfTerminalTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        public TextBox UnderlyingControl
        {
            get
            {
                return Startup.ConsoleOut;
            }
        }

        public void select()
        {
            try
            {
                UnderlyingControl.Select(UnderlyingControl.Text.Length, 0);
                UnderlyingControl.ScrollToEnd();
                UnderlyingControl.Focus();
                AppearanceManager.CurrentPosition = 1;
                AppearanceManager.LastLength = UnderlyingControl.Text.Length - 1;
            }
            catch { }
        }

        public override void Write(char value)
        {
            try
            {
                UnderlyingControl.Dispatcher.Invoke(new Action(() =>
                {
                    UnderlyingControl.AppendText(value.ToString());
                    select();
                }));
            }
            catch { }
        }

        public override void WriteLine(string value)
        {
            try
            {
                UnderlyingControl.Dispatcher.Invoke(new Action(() =>
                {
                    UnderlyingControl.AppendText(ShiftOS.Engine.Localization.Parse(value) + Environment.NewLine);
                    select();
                }));
            }
            catch { }
        }

        public void SetLastText()
        {
            if (SaveSystem.CurrentSave != null)
            {
                if (!Shiftorium.UpgradeInstalled("window_manager"))
                    AppearanceManager.LastTerminalText = UnderlyingControl.Text;
            }
        }

        public override void Write(string value)
        {
            try
            {
                UnderlyingControl.Dispatcher.Invoke(new Action(() =>
                {
                    UnderlyingControl.AppendText(ShiftOS.Engine.Localization.Parse(value));
                    select();
                }));
            }
            catch { }
        }


    }
}
