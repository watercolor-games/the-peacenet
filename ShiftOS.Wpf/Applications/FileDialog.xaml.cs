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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Engine;

namespace ShiftOS.Wpf.Applications
{
    /// <summary>
    /// Interaction logic for FileDialog.xaml
    /// </summary>
    public partial class FileDialog : UserControl, IShiftOSWindow
    {
        public FileDialog(string[] filters, FileOpenerStyle style, Action<string> callback)
        {
            InitializeComponent();
            Filters = filters;
            OpenerStyle = style;
            Callback = callback;
        }

        public string[] Filters { get; set; }
        public FileOpenerStyle OpenerStyle { get; set; }
        public Action<string> Callback { get; set; }

        private string currentFolder = "0:";

        public void OnLoad()
        {
            Reset();
            foreach(var f in Filters)
            {
                filter.Items.Add(f);
            }
            filter.SelectedIndex = 0;
        }

        public string GetParent(string path)
        {
            string[] pathlist = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (pathlist.Length > 1)
            {
                if (path.EndsWith("/"))
                {
                    path = path.Remove(path.Length - 1, 1);
                }
                path = path.Remove(path.LastIndexOf('/'), path.Length - path.LastIndexOf('/'));
                return path;
            }
            else
            {
                return "__system";
            }
        }

        public void Reset()
        {
            if (currentFolder != "__system")
                this.SetTitle("File Skimmer - " + currentFolder);
            else
                this.SetTitle("File Skimmer");

            lbfiles.Children.Clear();
            if (currentFolder == "__system")
            {
                foreach (var dir in Utils.Mounts)
                {
                    var sp = new StackPanel();
                    sp.Width = 50;
                    sp.Margin = new Thickness(5);
                    var label = new TextBlock();
                    label.Text = dir.Name;
                    label.TextWrapping = TextWrapping.Wrap;
                    label.TextAlignment = TextAlignment.Center;

                    var img = new Image();
                    img.Width = 42;
                    img.Height = 42;
                    img.Source = FileSkimmerBackend.GetImage(Utils.Mounts.IndexOf(dir) + ":").ToBitmapImage();

                    sp.Children.Add(img);
                    sp.Children.Add(label);

                    sp.PreviewMouseLeftButtonDown += (o, a) =>
                    {
                        if (a.ClickCount == 2)
                        {
                            ChangeDirectory(Utils.Mounts.IndexOf(dir) + ":");
                        }
                    };

                    lbfiles.Children.Add(sp);

                }
            }
            else
            {
                var __up = CreateUpOneDirectory();
                lbfiles.Children.Add(__up);

                foreach (var dir in Utils.GetDirectories(currentFolder))
                {
                    var sp = new StackPanel();
                    sp.Margin = new Thickness(5);
                    sp.Width = 50;
                    var label = new TextBlock();
                    label.Text = Utils.GetDirectoryInfo(dir).Name;
                    label.TextWrapping = TextWrapping.Wrap;
                    label.TextAlignment = TextAlignment.Center;

                    var img = new Image();
                    img.Width = 42;
                    img.Height = 42;
                    img.Source = FileSkimmerBackend.GetImage(dir).ToBitmapImage();

                    sp.Children.Add(img);
                    sp.Children.Add(label);

                    sp.PreviewMouseLeftButtonDown += (o, a) =>
                    {
                        if (a.ClickCount == 2)
                        {
                            ChangeDirectory(dir);
                        }
                    };

                    lbfiles.Children.Add(sp);

                }

                foreach (var dir in Utils.GetFiles(currentFolder))
                {
                    if (dir.EndsWith(filter.SelectedItem.ToString()))
                    {
                        var sp = new StackPanel();
                        sp.Margin = new Thickness(5);
                        sp.Width = 50;
                        var label = new TextBlock();
                        label.Text = Utils.GetFileInfo(dir).Name;
                        label.TextWrapping = TextWrapping.Wrap;
                        label.TextAlignment = TextAlignment.Center;

                        var img = new Image();
                        img.Width = 42;
                        img.Height = 42;
                        img.Source = FileSkimmerBackend.GetImage(dir).ToBitmapImage();

                        sp.Children.Add(img);
                        sp.Children.Add(label);

                        sp.PreviewMouseLeftButtonDown += (o, a) =>
                        {
                            if (a.ClickCount == 2)
                            {
                                if(OpenerStyle == FileOpenerStyle.Open)
                                {
                                    Callback?.Invoke(dir);
                                    AppearanceManager.Close(this);
                                }
                            }
                            else
                            {
                                txtfilename.Text = Utils.GetFileInfo(dir).Name;
                            }
                        };

                        lbfiles.Children.Add(sp);
                    }
                }
            }
        }

        public void ChangeDirectory(string path)
        {
            currentFolder = path;
            Reset();
        }

        private StackPanel CreateUpOneDirectory()
        {
            var sp = new StackPanel();
            sp.Margin = new Thickness(5);
            sp.Width = 50;
            var label = new TextBlock();
            label.Text = "Up one directory";
            label.TextWrapping = TextWrapping.Wrap;
            label.TextAlignment = TextAlignment.Center;

            var img = new Image();
            img.Width = 42;
            img.Height = 42;
            img.Source = FileSkimmerBackend.GetImage("__upone").ToBitmapImage();

            sp.Children.Add(img);
            sp.Children.Add(label);

            sp.PreviewMouseLeftButtonDown += (o, a) =>
            {
                if (a.ClickCount == 2)
                {
                    GoUp();
                }
            };

            return sp;
        }

        public void GoUp()
        {
            if (currentFolder.EndsWith(":"))
            {
                currentFolder = "__system";
                Reset();
            }
            else
            {
                currentFolder = GetParent(currentFolder);
                Reset();
            }
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


        private void btnconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtfilename.Text))
            {
                if (!txtfilename.Text.EndsWith(filter.SelectedItem.ToString()))
                    txtfilename.Text += filter.SelectedItem.ToString();

                switch (OpenerStyle)
                {
                    case FileOpenerStyle.Open:
                        if(Utils.FileExists(currentFolder + "/" + txtfilename.Text))
                        {
                            Callback?.Invoke(currentFolder + "/" + txtfilename.Text);
                            AppearanceManager.Close(this);
                        }
                        else
                        {
                            Infobox.Show("File not found", $"The file {currentFolder}/{txtfilename.Text} was not found.");
                        }
                        break;
                    case FileOpenerStyle.Save:
                        Callback?.Invoke(currentFolder + "/" + txtfilename.Text);
                        AppearanceManager.Close(this);
                        break;
                }
            }
            else
            {
                Infobox.Show("File Skimmer", "Please provide a filename.");
            }
        }

        private void filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reset();
        }
    }
}
