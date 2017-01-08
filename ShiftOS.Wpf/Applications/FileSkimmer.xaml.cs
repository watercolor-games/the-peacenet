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
    /// Interaction logic for FileSkimmer.xaml
    /// </summary>
    [Launcher("File Skimmer", true, "al_file_skimmer")]
    [RequiresUpgrade("file_skimmer")]
    public partial class FileSkimmer : UserControl, IShiftOSWindow
    {
        public FileSkimmer()
        {
            InitializeComponent();
        }

        private string currentFolder = "0:";

        static FileSkimmer()
        {
            FileSkimmerBackend.Init(new WpfFSFrontend());
        }

        public void OnLoad()
        {
            Reset();
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
                foreach(var dir in Utils.Mounts)
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

                foreach(var dir in Utils.GetDirectories(currentFolder))
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

                foreach(var dir in Utils.GetFiles(currentFolder))
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
                            FileSkimmerBackend.OpenFile(dir);
                        }
                    };

                    lbfiles.Children.Add(sp);

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
                if(a.ClickCount == 2)
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
    }

    public class WpfFSFrontend : IFileSkimmer
    {
        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            AppearanceManager.SetupDialog(new FileDialog(filetypes, style, callback));
        }

        public void OpenDirectory(string path)
        {
            var fs = new FileSkimmer();
            fs.ChangeDirectory(path);
            AppearanceManager.SetupWindow(fs);
        }

        public void OpenFile(string path)
        {
            bool opened = true;

            string ext = path.Split('.')[path.Split('.').Length - 1];
            switch(ext)
            {
                case "txt":
                    if (Shiftorium.UpgradeInstalled("textpad_open"))
                    {
                        var txt = new TextPad();
                        txt.LoadFile(path);
                        AppearanceManager.SetupWindow(txt);
                    }
                    else
                    {
                        opened = false;
                    }
                    break;
                case "pic":
                case "png":
                case "jpg":
                case "bmp":

                    break;
                case "wav":
                case "mp3":

                    break;
                case "lua":

                    break;
                case "py":

                    break;
                case "skn":

                    break;
                case "mfs":
                    Utils.MountPersistent(path);
                    string mount = (Utils.Mounts.Count - 1).ToString() + ":";
                    OpenDirectory(mount);
                    break;
                default:
                    opened = false;
                    break;
            }
            if(opened == false)
            {
                Infobox.Show("File Skimmer - Can't open file", "File Skimmer can't find an application to open this file!");
            }
        }
    }
}
