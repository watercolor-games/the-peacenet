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
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultTitle("{TITLE_FILEDIALOG}")]
    [DefaultIcon("iconFileSkimmer")]
    public partial class FileDialog : UserControl, IShiftOSWindow
    {
        public FileDialog(string[] filetypes, FileOpenerStyle style, Action<string> _callback)
        {
            callback = _callback;
            InitializeComponent();
            foreach(var itm in filetypes)
            {
                cbfiletypes.Items.Add(itm);
            }
            cbfiletypes.SelectedIndex = 0;
            cbfiletypes.SelectedIndexChanged += (o, a) => { ResetList(); };
            this.lvitems.SelectedIndexChanged += (o, a) =>
            {
                try
                {
                    var itm = lvitems.SelectedItems[0];
                    if (cbfiletypes.Text != "Directory")
                    {
                        if (FileExists(currentdir + "/" + itm.Text))
                        {
                            txtfilename.Text = itm.Text;
                        }
                    }
                    else
                    {
                        if (DirectoryExists(currentdir + "/" + itm.Text))
                        {
                            txtfilename.Text = itm.Text;
                        }

                    }
                }
                catch { }

            };
            btnok.Click += (o, a) =>
            {
                string fname = "";
                fname = (!string.IsNullOrWhiteSpace(txtfilename.Text)) ? txtfilename.Text : "";
                if (cbfiletypes.Text != "Directory")
                {
                    fname = (!fname.EndsWith(cbfiletypes.SelectedItem.ToString())) ? fname + cbfiletypes.SelectedItem.ToString() : fname;
                    fname = (fname == cbfiletypes.SelectedItem.ToString()) ? "" : fname;
                }

                switch (style)
                {

                    case FileOpenerStyle.Open:

                        if (cbfiletypes.Text == "Directory")
                        {
                            if (DirectoryExists(currentdir + "/" + fname))
                            {
                                callback?.Invoke(currentdir + "/" + fname);
                                this.Close();
                            }
                            else
                            {
                                Infobox.Show("{TITLE_FILENOTFOUND}", "{PROMPT_FILENOTFOUND}");
                            }

                        }
                        else
                        {
                            if (FileExists(currentdir + "/" + fname))
                            {
                                callback?.Invoke(currentdir + "/" + fname);
                                this.Close();
                            }
                            else
                            {
                                Infobox.Show("{FILE_NOT_FOUND}", "{FILE_NOT_FOUND_EXP}");
                            }
                        }
                        break;
                    case FileOpenerStyle.Save:
                        if (!string.IsNullOrWhiteSpace(fname))
                        {
                            callback?.Invoke(currentdir + "/" + fname);
                            this.Close();
                        }
                        else
                        {
                            Infobox.Show("{ENTER_FILENAME}", "{ENTER_FILENAME_EXP}");
                        }
                        break;
                }
            };
            btnok.Text = style.ToString();
            this.Text = style.ToString() + " File";
            this.lvitems.DoubleClick += new EventHandler(this.lvitems_DoubleClick);
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
            if (currentdir == "__system")
            {
                ChangeDirectory(path);
            }
            else if (DirectoryExists(currentdir + "/" + path))
            {
                ChangeDirectory(currentdir + "/" + path);
            }
            else if (FileExists(currentdir + "/" + path))
            {
                callback?.Invoke(currentdir + "/" + txtfilename.Text);
                this.Close();
            }
            else if (path == "__..")
            {
                ChangeToParent();
            }
        }

        Action<string> callback;

        string currentdrive = "0:";

        public void ChangeToParent()
        {
            if (currentdir == currentdrive)
            {
                ChangeDirectory("__system");
            }

            ChangeDirectory(GetParent(currentdir));
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

        private string currentdir = "";

        public void ChangeDirectory(string path)
        {
            currentdir = path;
            lbcurrentfolder.Text = currentdir;
            ResetList();
        }

        public void ResetList()
        {
            if (lvitems.LargeImageList == null)
            {
                lvitems.LargeImageList = new ImageList();
                lvitems.LargeImageList.TransparentColor = SkinEngine.LoadedSkin.ControlColor;
                lvitems.LargeImageList.ImageSize = new Size(42, 42);
                FileSkimmer.GetAllTypes(lvitems.LargeImageList);
            }


            lvitems.Items.Clear();

            if (currentdir == "__system")
            {
                //List all drives
                foreach (var dir in Mounts)
                {
                    var item = FileSkimmer.ConstructItemAsMount(dir);
                    item.ImageKey = "Mount";
                    lvitems.Items.Add(item);
                }
            }
            else if (DirectoryExists(currentdir))
            {
                var up = new ListViewItem();
                up.ImageKey = "UpOne";
                up.Text = "Up one";
                up.Tag = "__..";
                lvitems.Items.Add(up);


                foreach (var dir in GetDirectories(currentdir))
                {
                    var item = FileSkimmer.ConstructItem(GetDirectoryInfo(dir));
                    item.ImageKey = "Directory";
                    lvitems.Items.Add(item);
                }

                foreach (var dir in GetFiles(currentdir))
                {
                    if (dir.EndsWith(cbfiletypes.SelectedItem as string))
                    {
                        var item = FileSkimmer.ConstructItem(GetFileInfo(dir));
                        item.ImageKey = FileSkimmerBackend.GetFileType(dir).ToString();
                        lvitems.Items.Add(item);
                    }
                }

            }
        }

        [Obsolete("Use the relevant static method within File Skimmer instead.")]
        public static ListViewItem ConstructItemAsMount(Directory dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name + "(" + Mounts.IndexOf(dir).ToString() + ":/)";
            item.Tag = Mounts.IndexOf(dir).ToString() + ":";
            return item;
        }


        [Obsolete("Use the relevant static method within File Skimmer instead.")]
        public static ListViewItem ConstructItem(Directory dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name;
            item.Tag = item.Text;
            return item;
        }

        [Obsolete("Use the relevant static method within File Skimmer instead.")]
        public static ListViewItem ConstructItem(File dir)
        {
            var item = new ListViewItem();
            item.Text = dir.Name;
            item.Tag = item.Text;
            return item;
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
