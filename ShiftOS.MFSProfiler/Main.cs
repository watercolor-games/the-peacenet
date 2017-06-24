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
using static ShiftOS.Objects.ShiftFS.Utils;

using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;
using System.Threading;

namespace ShiftOS.MFSProfiler
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            SetupTree();
        }

        public void SetupTree()
        {
            tvfiles.Nodes.Clear();

            foreach(var dir in Mounts)
            {
                var mountNode = new TreeNode();
                mountNode.Text = dir.Name;
                mountNode.Tag = Mounts.IndexOf(dir).ToString() + ":";

                RecursiveDirectoryAdd(mountNode);

                tvfiles.Nodes.Add(mountNode);
            }
        }

        public void RecursiveDirectoryAdd(TreeNode node)
        {
            node.ContextMenuStrip = ctxfileoptions;
            foreach (var dir in GetDirectories(node.Tag.ToString()))
            {
                var dirInf = GetDirectoryInfo(dir);
                var child = new TreeNode();
                child.Text = dirInf.Name;
                child.Tag = dir;
                RecursiveDirectoryAdd(child);
                node.Nodes.Add(child);
                node.Expand();
            }
            foreach (var dir in GetFiles(node.Tag.ToString()))
            {
                var dirInf = GetFileInfo(dir);
                var child = new TreeNode();
                child.Text = dirInf.Name;
                child.Tag = dir;
                node.Nodes.Add(child);
                node.Expand();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var opener = new OpenFileDialog();
            opener.Filter = "Mini Filesystem|*.mfs";
            opener.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opener.Title = "Mount filesystem";
            if(opener.ShowDialog() == DialogResult.OK)
            {
                Mount(System.IO.File.ReadAllText(opener.FileName));
                SetupTree();
            }
        }

        private void tvfiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (FileExists(tvfiles.SelectedNode.Tag.ToString()))
                {
                    pnlfileinfo.BringToFront();

                    txtascii.Text = ReadAllText(tvfiles.SelectedNode.Tag.ToString());
                    txtbinary.Text = "";
                    var finf = GetFileInfo(tvfiles.SelectedNode.Tag.ToString());
                    var t = new Thread(new ThreadStart(() =>
                    {
                        foreach (var b in finf.Data)
                        {
                            txtbinary.Invoke(new Action(() =>
                            {
                                txtbinary.Text += b.ToString() + " ";
                            }));
                        }
                    }));
                    t.IsBackground = true;
                    t.Start();


                    lbfileinfo.Text = $@"Name: {finf.Name}
Permissions: {finf.permissions}
Size: {finf.Data.Length}
System path: {tvfiles.SelectedNode.Tag.ToString()}";
                }
            } catch { }
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opener = new OpenFileDialog();
            opener.Title = "Import file into ShiftFS";
            opener.Filter = "All files|*.*";
            opener.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bytes = System.IO.File.ReadAllBytes(opener.FileName);
                WriteAllBytes(tvfiles.SelectedNode.Tag.ToString() + "/" + opener.SafeFileName, bytes);
                SetupTree();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Infobox.PromptText("Create filesystem", "Please name your file system.", (result) =>
            {
                if(string.IsNullOrWhiteSpace(result))
                    MessageBox.Show(text: "ShiftFS does not allow blank volume names.", caption: "Volume name can't be blank", icon: MessageBoxIcon.Error, buttons:MessageBoxButtons.OK);
                else
                {
                    var dir = new Directory();
                    dir.Name = result;
                    dir.permissions = Objects.UserPermissions.Guest;
                    Mounts.Add(dir);
                    SetupTree();
                }
            });
        }

        private void newDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DirectoryExists(tvfiles.SelectedNode.Tag.ToString()))
            {
                Infobox.PromptText("New directory", "Please name your directory.", (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                        MessageBox.Show(text: "ShiftFS does not allow blank directory names.", caption: "Directory name can't be blank", icon: MessageBoxIcon.Error, buttons: MessageBoxButtons.OK);
                    else
                    {
                        var dinf = GetDirectoryInfo(tvfiles.SelectedNode.Tag.ToString());
                        dinf.AddDirectory(new Directory
                        {
                            Name = result,
                            permissions = Objects.UserPermissions.Guest
                        });
                        SetupTree();
                    }
                });
            }
        }

        public void Import(string win, string sfs)
        {
            foreach(var dir in System.IO.Directory.GetDirectories(win))
            {
                var dinf = new System.IO.DirectoryInfo(dir);
                CreateDirectory(sfs + dinf.Name);
                Import(dir, sfs + dinf.Name + "/");
            }
            foreach(var file in System.IO.Directory.GetFiles(win))
            {
                var finf = new System.IO.FileInfo(file);
                WriteAllBytes(sfs + finf.Name, System.IO.File.ReadAllBytes(file));
            }
        }

        private void exportToMFSFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = tvfiles.SelectedNode.Tag.ToString();
            string[] split = path.Split('/');
            int number = Convert.ToInt32(split[0].Replace(":", ""));
            string json = ExportMount(number);
            var saver = new SaveFileDialog();
            saver.Filter = "MFS/ShiftFS file (Why the fuck do we also call it \".mfs\"?)|*.mfs";
            saver.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saver.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllText(saver.FileName, json);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var picker = new FolderBrowserDialog();
            if(picker.ShowDialog() == DialogResult.OK)
            {
                var inf = new System.IO.DirectoryInfo(picker.SelectedPath);
                var dir = new Directory
                {
                    Name = inf.Name,
                    permissions = Objects.UserPermissions.Guest,
                };
                Mounts.Add(dir);
                string mpath = (Mounts.Count - 1) + ":/";
                Import(inf.FullName, mpath);
                SetupTree();
            }
        }
    }
}
