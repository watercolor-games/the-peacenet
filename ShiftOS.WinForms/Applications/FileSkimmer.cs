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

using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static ShiftOS.Objects.ShiftFS.Utils;
using Newtonsoft.Json;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("File Skimmer", true, "al_file_skimmer")]
    [RequiresUpgrade("file_skimmer")]
    [WinOpen("file_skimmer")]
    public partial class FileSkimmer : UserControl, IShiftOSWindow
    {
        public FileSkimmer()
        {
            InitializeComponent();
            this.Load += (o, a) =>
            {
                ChangeDirectory(Paths.GetPath("root"));
            };
        }

        private void lvitems_DoubleClick(object sender, EventArgs e)
        {
            if (lvitems.SelectedItems.Count <= 0)
                return;

            var item = lvitems.SelectedItems[0];
            var path = item.Tag as string;

            if(currentdir == "__system")
            {
                ChangeDirectory(path);
            }
            else if(DirectoryExists(currentdir + "/" + path))
            {
                ChangeDirectory(currentdir + "/" + path);
            }
            else if(FileExists(currentdir + "/" + path))
            {
                FileSkimmerBackend.OpenFile(currentdir + "/" + path);
            }
            else if(path == "__..")
            {
                ChangeToParent();
            }
        }

        [Obsolete("This just forwards over to FileSkimmerBackend.OpenFile().")]
        public void Open(string path)
        {
            FileSkimmerBackend.OpenFile(path);
        }

        [Obsolete("Forwarded to FileSkimmerBackend.GetFileType().")]
        public static FileType GetFileType(string path)
        {
            return FileSkimmerBackend.GetFileType(path);
        }
        
        string currentdrive = "0:";

        public void ChangeToParent()
        {
            if(currentdir == currentdrive)
            {
                ChangeDirectory("__system");
            }

            ChangeDirectory(GetParent(currentdir));
        }

        public string GetParent(string path)
        {
            string[] pathlist = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if(pathlist.Length > 1)
            {
                if(path.EndsWith("/"))
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

        private string currentdir = "";

        public void ChangeDirectory(string path)
        {
            currentdir = path;
            lbcurrentfolder.Text = currentdir;
            ResetList();
        }

        
        public static void GetAllTypes(ImageList list)
        {
            list.Images.Add("Directory", Properties.Resources.fileicon0);
            list.Images.Add("UpOne", Properties.Resources.fileicon5);
            list.Images.Add("Mount", Properties.Resources.FloppyDriveIcon);

            foreach (FileType value in Enum.GetValues(typeof(FileType)))
            {
                list.Images.Add(value.ToString(), GetImage(value));
            }
        }

        public void ResetList()
        {
            if(lvitems.LargeImageList == null)
            {
                lvitems.LargeImageList = new ImageList();
                lvitems.LargeImageList.TransparentColor = SkinEngine.LoadedSkin.ControlColor;
                lvitems.LargeImageList.ImageSize = new Size(42, 42);
                GetAllTypes(lvitems.LargeImageList);
            }



            lvitems.Items.Clear();
            if (currentdir == "__system")
            {
                //List all drives
                foreach (var dir in Mounts)
                {
                    var item = ConstructItemAsMount(dir);
                    item.ImageKey = "Mount";
                    lvitems.Items.Add(item);
                }
            }
            else if (DirectoryExists(currentdir))
            {
                var up = new ListViewItem();
                up.Text = "Up one";
                up.ImageKey = "UpOne";
                up.Tag = "__..";
                lvitems.Items.Add(up);


                foreach(var dir in GetDirectories(currentdir))
                {
                    var item = ConstructItem(GetDirectoryInfo(dir));
                    item.ImageKey = "Directory";
                    lvitems.Items.Add(item);
                }

                foreach (var dir in GetFiles(currentdir))
                {
                    var item = ConstructItem(GetFileInfo(dir));
                    item.ImageKey = FileSkimmerBackend.GetFileType(dir).ToString();
                    lvitems.Items.Add(item);
                }

            }
            
        }

        public static ListViewItem ConstructItemAsMount(Directory dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name + "(" + Mounts.IndexOf(dir).ToString() + ":/)";
            item.Tag = Mounts.IndexOf(dir).ToString() + ":";
            return item;
        }

        public static ListViewItem ConstructItem(Directory dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name;
            item.Tag = item.Text;
            return item;
        }
        public static ListViewItem ConstructItem(File dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name;
            item.ImageKey = "Directory";
            item.Tag = item.Text;
            return item;
        }

        public static Image GetImage(FileType type)
        {
            switch(type)
            {
                case FileType.Executable:
                case FileType.Lua:
                case FileType.Python:
                    return Properties.Resources.fileiconsaa;
                case FileType.Image:
                    return Properties.Resources.fileicon3;
                case FileType.Skin:
                    return Properties.Resources.fileicon10;
                case FileType.TextFile:
                    return Properties.Resources.fileicon2;
                default:
                    return Properties.Resources.fileicon1;
            }
        }

        private void FileSkimmer_Load(object sender, EventArgs e) {
                
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
