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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
namespace ShiftOS.Wpf
{
    /// <summary>
    /// Interaction logic for Desktop.xaml
    /// </summary>
    public partial class Desktop : Window
    {
        public Desktop()
        {
            InitializeComponent();
            SetupUpgradeable();
            Shiftorium.Installed += () => SetupUpgradeable();
            SaveSystem.GameReady += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetupDesktop();
                });
            };
        }

        public void SetupUpgradeable()
        {
            desktoppanel.Upgrade("desktop");
            apps.Upgrade("app_launcher");
        }

        public void SetupDesktop()
        {
            windowlayer.Background = LoadedSkin.DesktopColor.CreateBrush();
            try
            {
                windowlayer.Background = new ImageBrush(LoadedSkin.DesktopBackgroundImage.ToBitmapImage());
            }
            catch
            {

            }


            desktoppanel.Background = LoadedSkin.DesktopPanelColor.CreateBrush();
            try
            {
                desktoppanel.Background = new ImageBrush(LoadedSkin.DesktopPanelBackground.ToBitmapImage());
            }
            catch { }

            apps.Background = LoadedSkin.Menu_MenuStripGradientBegin.CreateBrush();
            apps.Content = LoadedSkin.AppLauncherText;
            apps.Foreground = LoadedSkin.Menu_TextColor.CreateBrush();

            PopulateAppLauncher();

            SetupUpgradeable();
        }

        private static event Action<WpfWindowBorder> onWindowShow;
        private static event Action<WpfWindowBorder> onWindowClose;

        public static void ShowWindow(WpfWindowBorder brdr)
        {
            
            onWindowShow?.Invoke(brdr);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SaveSystem.GameReady += () => Dispatcher.Invoke(() => PopulateAppLauncher());
            Shiftorium.Installed += () => Dispatcher.Invoke(() => PopulateAppLauncher());
            SkinEngine.SkinLoaded += () => Dispatcher.Invoke(() => PopulateAppLauncher());


            onWindowShow += (brdr) =>
            {
                brdr.Measure(windowlayer.DesiredSize);
                brdr.SetValue(System.Windows.Controls.Canvas.LeftProperty, (windowlayer.ActualWidth - brdr.Width) / 2);
                brdr.SetValue(System.Windows.Controls.Canvas.TopProperty, (windowlayer.ActualHeight - brdr.ActualActualHeight) / 2);
                windowlayer.Children.Add(brdr);
                brdr.PreviewMouseDown += (o, a) =>
                {
                    brdr.BringToFront(windowlayer);
                };
            };
            onWindowClose += (brdr) =>
            {
                if (windowlayer.Children.Contains(brdr))
                {
                    windowlayer.Children.Remove(brdr);
                    brdr.ParentWindow.OnUnload();
                    brdr = null;
                }
            };
            AppearanceManager.SetupWindow(new Terminal());
        }


        public static void RemoveWindow(WpfWindowBorder brdr)
        {
            onWindowClose?.Invoke(brdr);
        }

        public void PopulateAppLauncher()
        {
            appsmenu.Children.Clear();

            double biggestWidth = 0;

            appsmenu.Background = LoadedSkin.Menu_ToolStripDropDownBackground.CreateBrush();

            foreach (var kv in AppLauncherDaemon.Available())
            {
                var item = new Button();
                if (kv.LaunchType.BaseType == typeof(System.Windows.Forms.UserControl))
                    item.Content = kv.DisplayData.Name + " (Legacy)";
                else
                    item.Content = kv.DisplayData.Name;
                item.Margin = new Thickness(2);
                double measure = item.Content.ToString().Measure(LoadedSkin.MainFont);
                if (measure > biggestWidth)
                    biggestWidth = measure;
                item.Click += (o, a) =>
                {
                    AppearanceManager.SetupWindow(Activator.CreateInstance(kv.LaunchType) as IShiftOSWindow);
                    appsmenu.Visibility = Visibility.Hidden;
                };
                item.HorizontalAlignment = HorizontalAlignment.Stretch;
                appsmenu.Children.Add(item);
            }

            if (Shiftorium.UpgradeInstalled("al_shutdown"))
            {
                var item = new Button();
                item.Content = ShiftOS.Engine.Localization.Parse("{SHUTDOWN}");
                item.Margin = new Thickness(2);
                double measure = item.Content.ToString().Measure(LoadedSkin.MainFont);
                if (measure > biggestWidth)
                    biggestWidth = measure;


                item.Click += (o, a) =>
                {
                    TerminalBackend.InvokeCommand("sos.shutdown");
                };
                appsmenu.Children.Add(item);

            }

            appsmenu.Width = biggestWidth + 50;

            SkinAppLauncher();
        }

        public void SkinAppLauncher()
        {
            apps.Background = LoadedSkin.Menu_MenuStripGradientBegin.CreateBrush();
            apps.Foreground = LoadedSkin.Menu_TextColor.CreateBrush();
            apps.BorderThickness = new Thickness(0.0);

            appsmenu.Height = 0;

            foreach (Control app in appsmenu.Children)
            {
                app.Background = LoadedSkin.Menu_ToolStripDropDownBackground.CreateBrush();
                app.Foreground = LoadedSkin.Menu_TextColor.CreateBrush();
                app.BorderThickness = new Thickness(0.0);
                app.HorizontalContentAlignment = HorizontalAlignment.Left;
                app.SetMouseOverStyle("menuitem");
                appsmenu.Height += app.Height;
            }
        }

        private void apps_Click(object sender, RoutedEventArgs e)
        {
            if (appsmenu.Visibility == Visibility.Hidden)
            {
                appsmenu.Visibility = Visibility.Visible;
            }
            else
            {
                appsmenu.Visibility = Visibility.Hidden;
            }

        }
    }
}
