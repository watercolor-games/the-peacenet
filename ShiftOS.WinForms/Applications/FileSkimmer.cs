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
using Newtonsoft.Json;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("{TITLE_FILESKIMMER}", true, "al_file_skimmer", "{AL_UTILITIES}")]
    [RequiresUpgrade("file_skimmer")]
    [WinOpen("{WO_FILESKIMMER}")]
    [DefaultTitle("{TITLE_FILESKIMMER}")]
    [DefaultIcon("iconFileSkimmer")]
    public partial class FileSkimmer : UserControl, IShiftOSWindow
    {

        public static Objects.ClientSave CurrentRemoteUser = new Objects.ClientSave();
        public static ShiftOSEnvironment OpenConnection = new ShiftOSEnvironment();

        private static event Action OnDisconnect;

        public static void DisconnectRemote()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                OnDisconnect?.Invoke();
                CurrentRemoteUser = new Objects.ClientSave();
                if (!string.IsNullOrWhiteSpace(OpenConnection.SystemName))
                    Infobox.Show("Connections terminated.", "All outbound File Skimmer connections have been terminated.");
                OpenConnection = new ShiftOSEnvironment();
            });
        }

        public FileSkimmer()
        {
            InitializeComponent();
            this.Load += (o, a) =>
            {
                ChangeDirectory(Paths.GetPath("root"));
            };
            OnDisconnect += FileSkimmer_OnDisconnect;
        }

        private void FileSkimmer_OnDisconnect()
        {
            connectToRemoteServerToolStripMenuItem.Text = "Start Remote Session";
            disconnectToolStripMenuItem.Visible = false;
            currentdir = "__system";
            ResetList();
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

        public void pinDirectory(string path)
        {
            int amountsCalled = -1;
            amountsCalled = amountsCalled + 1;
            List<string> Pinned = new List<string>();
            if(FileExists(Paths.GetPath("data") + "/pinned_items.dat"))
            {
                Pinned = JsonConvert.DeserializeObject<List<string>>(ReadAllText(Paths.GetPath("data") + "/pinned_items.dat"));
            }
            Pinned.Add(path);
            WriteAllText(Paths.GetPath("data") + "/pinned_items.dat", JsonConvert.SerializeObject(Pinned));
            ResetList();
        }

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

        public void PopulatePinned(TreeNode node, string[] items)
        {
            foreach(var dir in items)
            {
                var treenode = new TreeNode();
                if (DirectoryExists(dir))
                {
                    var dinf = GetDirectoryInfo(dir);
                    treenode.Text = dinf.Name;
                }
                else if (FileExists(dir))
                {
                    var finf = GetFileInfo(dir);
                    treenode.Text = finf.Name;
                }
                treenode.Tag = dir;
                node.Nodes.Add(treenode);
            }
        }

        public void ResetList()
        {
            pinnedItems.Nodes.Clear();
            List<string> Pinned = new List<string>();
            if(FileExists(Paths.GetPath("data") + "/pinned_items.dat"))
            {
                Pinned = JsonConvert.DeserializeObject<List<string>>(ReadAllText(Paths.GetPath("data") + "/pinned_items.dat"));
            }
            var node = new TreeNode();
            node.Text = "Pinned";
            PopulatePinned(node, Pinned.ToArray());
            pinnedItems.Nodes.Add(node);
            node.ExpandAll();

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
                case FileType.CommandFormat:
                    return Properties.Resources.fileiconcf;
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
            OnDisconnect -= FileSkimmer_OnDisconnect;
            return true;
        }

        public void OnUpgrade()
        {
            moveToolStripMenuItem.Visible = false;
            copyToolStripMenuItem.Visible = false;
            deleteToolStripMenuItem.Visible = false;
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Infobox.PromptText("New Folder", "Please type a name for your folder.", (path) =>
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if(!Utils.DirectoryExists(this.currentdir + "/" + path))
                    {
                        Utils.CreateDirectory(currentdir + "/" + path);
                        this.ResetList();
                    }
                    else
                    {
                        Infobox.Show("New folder", "A folder with that name already exists.");
                    }
                }
                else
                {
                    Infobox.Show("New folder", "You can't create a folder with no name!");
                }
            });
        }

        private void lvitems_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (currentdir != "__system")
                {
                    var itm = lvitems.SelectedItems[0];
                    if (itm.Tag.ToString() != "__..")
                    {
                        if (DirectoryExists(currentdir + "/" + itm.Tag.ToString()))
                        {
                            deleteToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_recursive_delete");
                            moveToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_move_folder");
                            copyToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_copy_folder");
                        }
                        else if (FileExists(currentdir + "/" + itm.Tag.ToString()))
                        {
                            deleteToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_delete");
                            moveToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_move");
                            copyToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("fs_copy");
                        }
                    }
                }
            }
            catch
            {
                moveToolStripMenuItem.Visible = false;
                copyToolStripMenuItem.Visible = false;
            }
        }

        private void lvitems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(lvitems.SelectedItems.Count == 0)
            {
                moveToolStripMenuItem.Visible = false;
                copyToolStripMenuItem.Visible = false;
                deleteToolStripMenuItem.Visible = false;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = currentdir + "/" + lvitems.SelectedItems[0].Tag.ToString();
                if (DirectoryExists(path))
                {
                    FileSkimmerBackend.GetFile(new[] { "Directory" }, FileOpenerStyle.Save, (newPath) =>
                     {
                         Copy(path, newPath);
                         ResetList();
                     });
                }
                else if (FileExists(path))
                {
                    string[] psplit = path.Split('.');
                    string ext = "." + psplit[psplit.Length - 1];
                    FileSkimmerBackend.GetFile(new[] { ext }, FileOpenerStyle.Save, (newPath) =>
                    {
                        Copy(path, newPath);
                        ResetList();
                    });

                }
            }
            catch
            {

            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = currentdir + "/" + lvitems.SelectedItems[0].Tag.ToString();
                if (DirectoryExists(path))
                {
                    FileSkimmerBackend.GetFile(new[] { "Directory" }, FileOpenerStyle.Save, (newPath) =>
                    {
                        Utils.Move(path, newPath);
                        ResetList();
                    });
                }
                else if (FileExists(path))
                {
                    string[] psplit = path.Split('.');
                    string ext = psplit[psplit.Length - 1];
                    FileSkimmerBackend.GetFile(new[] { ext }, FileOpenerStyle.Save, (newPath) =>
                    {
                        Utils.Move(path, newPath);
                        ResetList();
                    });

                }
            }
            catch
            {

            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Infobox.PromptYesNo("Delete file", "Are you sure you want to delete " + lvitems.SelectedItems[0].Text + "?", (result) =>
                {
                    if (result == true)
                    {
                        Delete(currentdir + "/" + lvitems.SelectedItems[0].Tag.ToString());
                        ResetList();
                    }
                });
            }
            catch { }
        }

        private void pinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Infobox.PromptYesNo("Pin folder", "Are you sure you want to pin \"" + lvitems.SelectedItems[0].Text + "\"?", (result) =>
                {
                    if (result == true)
                    {
                        if (currentdir != "__system" && lvitems.SelectedItems[0].Text != "Up one")
                        {
                            pinDirectory(currentdir + "/" + lvitems.SelectedItems[0].Text);
                            ResetList();
                        }
                        else
                        {
                            Infobox.Show("Cannot Pin", "You can only pin files or folders.");
                        }
                            
                    }
                });
            }
            catch { }
        }

        private void pinnedItems_Click(object sender, EventArgs e)
        {
            try
            {
                if (pinnedItems.SelectedNode != null)
                {
                    string path = pinnedItems.SelectedNode.Tag.ToString();
                    if (DirectoryExists(path))
                    {
                        currentdir = path;
                        ResetList();
                    }
                    else if (FileExists(path))
                    {
                        FileSkimmerBackend.OpenFile(path);
                    }
                }
            }
            catch { }
        }

        private void connectToRemoteServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConnectionBox();
        }

        public void ShowConnectionBox()
        {
            pnlconnect.Show();
            pnlconnect.BringToFront();
            pnlconnect.CenterParent();

            //header
            lbctitle.CenterParent();
            lbctitle.Top = 5;

            //description
            lbcdesc.CenterParent();
            lbcdesc.Top = lbctitle.Top + lbctitle.Height + 5;

            //credentials
            pnlcreds.CenterParent();
            pnlcreds.Top = lbcdesc.Top + lbcdesc.Height + 5;
            txtcsys.Text = OpenConnection.SystemName;
            txtcuser.Text = CurrentRemoteUser.Username;
            txtcpass.Text = CurrentRemoteUser.Password;

            //controls
            flcbuttons.CenterParent();
            flcbuttons.Top = pnlcreds.Top + pnlcreds.Height + 5;
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            pnlconnect.Hide();
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            var sys = VirtualEnvironments.Get(txtcsys.Text);

            if(sys != null)
            {
                //user auth
                var user = sys.Users.FirstOrDefault(x => x.Username == txtcuser.Text && x.Password == txtcpass.Text);
                if(user != null)
                {
                    OpenConnection = sys;
                    CurrentRemoteUser = user;
                    if (Mounts.Count == 3)
                        Mounts.RemoveAt(2);
                    Mounts.Add(sys.Filesystem);
                    ChangeDirectory("2:");
                    pnlconnect.Hide();
                    connectToRemoteServerToolStripMenuItem.Text = "Reauthenticate";
                    disconnectToolStripMenuItem.Visible = true;
                    return;
                }
                Infobox.Show("Access denied.", "Authentication failed for the specified user. Connection aborted.");
                return;
            }
            var t = new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(5000);
                Infobox.Show("Connection timeout.", "Cannot connect to the specified system name...");
            });
            t.IsBackground = true;
            t.Start();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisconnectRemote();
        }
    }
}
