using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects;
using static Plex.Engine.FSUtils;

namespace Plex.Frontend.Apps
{
    [WinOpen("fileskimmer")]
    [Launcher("File Skimmer", false, null, "System")]
    public class FileSkimmer : Control, IPlexWindow
    {
        private string _currentdirectory = "0:";
        private const string SD_SYSTEM = "__system";
        private ListView _fList = null;
        private TextControl _currentdirtext = null;
        private Button _openFile = null;
        private TextControl _fileFilter = null;
        private TextControl _filePrompt = null;
        private TextInput _fileBox = null;
        private ScrollView _scrollView = null;
        private MenuItem _newFolder = null;
        private MenuItem _delete = null;
        private MenuItem _importFile = null;
        private MenuItem _exportFile = null;
        private MenuBar _menuBar = null;

        public bool IsDialog = false;
        public Action<string> DialogCallback = null;
        public string[] FileFilters = new string[] { "" };
        public FileOpenerStyle DialogMode = FileOpenerStyle.Open;

        public int SelectedFilter = 0;

        public FileSkimmer()
        {
            _fileBox = new TextInput();
            _filePrompt = new TextControl();
            _fileFilter = new TextControl();
            _openFile = new Button();
            _scrollView = new ScrollView();
            _menuBar = new MenuBar();

            AddControl(_menuBar);
            AddControl(_openFile);
            AddControl(_fileBox);
            AddControl(_filePrompt);
            AddControl(_fileFilter);

            _openFile.AutoSize = true;
            _fileFilter.AutoSize = true;
            _filePrompt.AutoSize = true;

            Width = 720;
            Height = 480;
            _fList = new ListView();
            _fList.DoubleClick += () =>
            {
                try
                {
                    Navigate(_fList.SelectedItem.Tag);
                }
                catch { }
            };
            AddControl(_scrollView);
            _scrollView.AddControl(_fList);
            _currentdirtext = new TextControl();
            _currentdirtext.AutoSize = true;
            AddControl(_currentdirtext);


            _newFolder = new MenuItem
            {
                Text = "New Folder",
            };
            _delete = new MenuItem
            {
                Text = "Delete"
            };
            _importFile = new MenuItem
            {
                Text = "Import file"
            };
            _exportFile = new MenuItem
            {
                Text = "Export file"
            };
            _newFolder.ItemActivated += () =>
            {
                Engine.Infobox.PromptText("New folder", "Please name your new folder.", (dname) =>
                {
                    if(string.IsNullOrWhiteSpace(dname))
                    {
                        Engine.Infobox.Show("New folder", "You can't have a blank directory name!");
                        return;
                    }
                    if(dname.Contains("/") || dname.Contains(":") || dname == "." || dname.Contains(".."))
                    {
                        Engine.Infobox.Show("New folder", $"Illegal folder name {dname}. Folders can't have '/' or ':' in their names and can't be either '.' or '..'.");
                        return;
                    }
                    string fpath = _currentdirectory + "/" + dname;
                    if (FSUtils.DirectoryExists(fpath))
                    {
                        Engine.Infobox.Show("New folder", "That folder already exists. Try specifying a different name?");
                        return;
                    }
                    try
                    {
                        FSUtils.CreateDirectory(fpath);
                    }
                    catch (Exception ex)
                    {
                        Engine.Infobox.Show("IO error", ex.Message);
                        return;
                    }
                    ResetList();
                });
            };

            _delete.ItemActivated += () =>
            {
                if(_fList.SelectedItem != null)
                {
                    try
                    {
                        string path = _fList.SelectedItem.Tag;
                        if (FileExists(path) || DirectoryExists(path))
                            Engine.Infobox.PromptYesNo("Delete file", "Are you sure you want to delete the file/directory at " + path + "?", (answer) =>
                            {
                                if(answer == true)
                                {
                                    Delete(path);
                                    ResetList();
                                }
                            });
                    }
                    catch(Exception ex)
                    {
                        Engine.Infobox.Show("IO error", ex.Message);
                    }
                }
            };

            _exportFile.ItemActivated += () =>
            {
                if (_fList.SelectedItem != null)
                {
                    try
                    {
                        string path = _fList.SelectedItem.Tag;
                        if (FileExists(path))
                        {
                            string outputPath = "";
                            string ext = path.Substring(path.LastIndexOf("."));
                            if(UIManager.FourthWall.GetFilePath($"Save {path} from Project: Plex to disk", "Source file type|*"+ext, FileOpenerStyle.Save, out outputPath) == true)
                            {
                                byte[] contents = ReadAllBytes(path);
                                System.IO.File.WriteAllBytes(outputPath, contents);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Engine.Infobox.Show("IO error", ex.Message);
                    }
                }

            };

            _importFile.ItemActivated += () =>
            {
                string path = "";
                string filter = "";
                foreach (var type in ReflectMan.Types.Where(x=>x.GetInterfaces().Contains(typeof(IPlexWindow))))
                {
                    var fattribs = type.GetCustomAttributes(false).Where(x => x is FileHandlerAttribute);
                    foreach (var attrib in fattribs)
                    {
                        var fattrib = attrib as FileHandlerAttribute;
                        if (fattrib != null)
                        {
                            filter += fattrib.Name + "|*" + fattrib.Extension + "|";
                        }
                    }
                }
                if (filter.EndsWith("|"))
                    filter = filter.Remove(filter.LastIndexOf("|"), 1);
                if (string.IsNullOrWhiteSpace(filter))
                {
                    Engine.Infobox.Show("Import file", "You can't import files right now because there are no known file types to import.");
                    return;
                }

                if(UIManager.FourthWall.GetFilePath("Import file into Project: Plex at " + _currentdirectory, filter, FileOpenerStyle.Open, out path))
                {
                    var finf = new System.IO.FileInfo(path);
                    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_-.";
                    string filteredfname = finf.Name;
                    foreach(char c in filteredfname)
                    {
                        if (!alphabet.Contains(c.ToString()))
                            filteredfname = filteredfname.Replace(c.ToString(), "_");
                    }
                    string fpath = _currentdirectory + "/" + filteredfname;
                    if (FileExists(fpath))
                    {
                        Engine.Infobox.Show("File exists", "This file already exists.");
                        return;
                    }
                    byte[] data = System.IO.File.ReadAllBytes(finf.FullName);
                    WriteAllBytes(fpath, data);
                    ResetList();
                }
            };

            _menuBar.AddItem(_newFolder);
            _menuBar.AddItem(_delete);
            _menuBar.AddItem(_importFile);
            _menuBar.AddItem(_exportFile);
            _menuBar.Visible = true;
        }

        public void OnLoad()
        {
            Width = 720;
            Height = 480;
            _fList.Click += () =>
            {
                if(_fList.SelectedItem != null)
                {
                    if(_fList.SelectedItem.Tag != "__up")
                    {
                        if(FileExists(_fList.SelectedItem.Tag))
                        {
                            _fileBox.Text = _fList.SelectedItem.Text;
                        }
                    }
                }
            };
            _fileBox.KeyEvent += (key) =>
            {
                if (key.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    HandleFileSelect();
                }
            };
            _openFile.Click += () =>
            {
                HandleFileSelect();
            };
            ResetList();
        }

        public void HandleFileSelect(bool quiet = false)
        {
            string fname = _fileBox.Text;
            string filter = FileFilters[SelectedFilter];
            if (filter != "Directory")
                if (!fname.EndsWith(filter))
                    fname += filter;
            string path = _currentdirectory + "/" + fname;
            bool exists = false;
            if (filter == "Directory")
                exists = DirectoryExists(path);
            else
                exists = FileExists(path);
            if (exists)
            {
                if (DialogMode == FileOpenerStyle.Open || quiet)
                {
                    DialogCallback?.Invoke(path);
                    AppearanceManager.Close(this);
                }
                else
                {
                    Engine.Infobox.PromptYesNo("Overwrite file?", "The file you chose already exists on disk. Would you like to overwrite it?", (answer) =>
                    {
                        if (answer)
                        {
                            DialogCallback?.Invoke(path);
                            AppearanceManager.Close(this);
                        }
                    });
                }
            }
            else
            {
                if (DialogMode == FileOpenerStyle.Open)
                {
                    if (quiet == false)
                        Engine.Infobox.Show("File not found.", "The requested file path could not be found on disk.");
                }
                else
                {
                    DialogCallback?.Invoke(path);
                    AppearanceManager.Close(this);
                }
            }  
        }

        public void Navigate(string path)
        {
            if(path == "__up")
            {
                if (_currentdirectory == SD_SYSTEM)
                    throw new Exception("Someone tried to make it so you can go \"up one directory\" in the mounts list.");
                if (_currentdirectory.EndsWith(":"))
                {
                    _currentdirectory = SD_SYSTEM;
                    ResetList();
                    return;
                }
                else
                {
                    int slashlast = _currentdirectory.LastIndexOf("/");
                    int len = _currentdirectory.Length - slashlast;
                    _currentdirectory = _currentdirectory.Remove(slashlast, len);
                    ResetList();
                    return;
                }
            }

            if (DirectoryExists(path))
            {
                _currentdirectory = path;
                ResetList();
            }
            else if (FileExists(path))
            {
                if (IsDialog)
                {
                    bool quiet = DialogMode == FileOpenerStyle.Open;
                    this.HandleFileSelect(quiet);
                }
                else
                {
                    if (!FileSkimmerBackend.OpenFile(path))
                    {
                        Engine.Infobox.Show("File Skimmer can't open this file!", "File Skimmer couldn't find a program that can open a file of this type. Please install a program that can handle this file and try again.");
                    }
                }
            }
        }

        public Texture2D ToTexture2D(System.Drawing.Image img)
        {
            if (img == null)
                return null;
            using(var bmp = new System.Drawing.Bitmap(img))
            {
                var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var arr = new byte[Math.Abs(lck.Stride) * lck.Height];
                Marshal.Copy(lck.Scan0, arr, 0, arr.Length);
                for(int i = 0; i < arr.Length; i += 4)
                {
                    byte r = arr[i];
                    byte b = arr[i + 2];
                    arr[i] = b;
                    arr[i + 2] = r;
                }
                var tex2 = new Texture2D(UIManager.GraphicsDevice, bmp.Width, bmp.Height);
                tex2.SetData<byte>(arr);
                bmp.UnlockBits(lck);
                return tex2;
            }
        }

        public System.Drawing.Image GetImage(FileType type)
        {
            switch (type)
            {
                case FileType.UpOne:
                    return FontAwesome.arrow_circle_up;
                case FileType.Mount:
                case FileType.Directory:
                    return FontAwesome.folder;
                case FileType.Executable:
                case FileType.Lua:
                case FileType.Python:
                    return FontAwesome.file_code_o;
                case FileType.Image:
                    return FontAwesome.file_picture_o;
                case FileType.Skin:
                    return FontAwesome.cogs;
                case FileType.TextFile:
                    return FontAwesome.file_text_o;
                default:
                    return FontAwesome.file_o;
            }
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void ResetList()
        {
            if (_currentdirectory == SD_SYSTEM)
                _currentdirtext.Text = "My storage drives";
            else
                _currentdirtext.Text = _currentdirectory;

            _fList.ClearItems();
            if (_currentdirectory != SD_SYSTEM)
                _fList.AddItem(new ListViewItem
                {
                    Text = "Up one...",
                    Tag = "__up",
                    ImageKey = FileType.UpOne.ToString()
                });

            if(_currentdirectory == SD_SYSTEM)
            {
                foreach(var mount in GetMounts())
                {
                    string mountpath = $"{mount.DriveNumber}:";
                    string name = mount.VolumeLabel;
                    _fList.AddItem(new ListViewItem
                    {
                        Text = name,
                        Tag = mountpath,
                        ImageKey = FileType.Mount.ToString()
                    });
                }
            }
            else
            {
                foreach(var dir in GetDirectories(_currentdirectory))
                {
                    if (dir.EndsWith(".."))
                        continue;
                    var dinf = GetFileInfo(dir);
                    _fList.AddItem(new ListViewItem
                    {
                        Text = dinf.Name,
                        Tag = dir,
                        ImageKey = FileType.Directory.ToString()
                    });
                }
                foreach (var dir in GetFiles(_currentdirectory))
                {
                    if (this.IsDialog)
                        if (!dir.EndsWith(FileFilters[SelectedFilter]))
                            continue;

                    var dinf = GetFileInfo(dir);
                    var ext = FileSkimmerBackend.GetFileType(dir);


                    _fList.AddItem(new ListViewItem
                    {
                        Text = dinf.Name,
                        Tag = dir,
                        ImageKey = ext.ToString()
                    });

                }

            }
            InvalidateTopLevel();
        }


        private const int _fileselectboxheight = 50;

        protected override void OnLayout(GameTime gameTime)
        {
            int listbottom = Height - _currentdirtext.Height;
            if (IsDialog)
                listbottom -= _fileselectboxheight;


            //Menu items
            _newFolder.Enabled = (_currentdirectory != SD_SYSTEM);
            bool hasFileOrDir = _fList.SelectedItem != null;
            if(hasFileOrDir == true)
                hasFileOrDir = _fList.SelectedItem.ImageKey != "Mount" && _fList.SelectedItem.ImageKey != "UpOne";
            _delete.Enabled = hasFileOrDir;
            _exportFile.Enabled = hasFileOrDir;
            if (_exportFile.Enabled)
                _exportFile.Enabled = _exportFile.Enabled && _fList.SelectedItem.ImageKey != "Directory";
            _importFile.Enabled = _newFolder.Enabled;

            _menuBar.X = 0;
            _menuBar.Y = 0;
            _menuBar.Width = Width;
            _menuBar.AutoSize = true;
            _menuBar.Layout(gameTime);

            _fList.X = 0;
            _fList.Y = 0;
            _fList.MaxWidth = Width;
            _fList.MaxHeight = int.MaxValue;
            _fList.AutoSize = true;
            _fList.Layout(gameTime);

            _scrollView.Y = _menuBar.Height;
            _scrollView.Width = Width;
            _scrollView.Height = listbottom - _scrollView.Y;
            _scrollView.X = 0;

            _currentdirtext.X = (Width - _currentdirtext.Width) / 2;
            _currentdirtext.Y = _scrollView.Height;

            if (IsDialog)
            {
                int _fileselectstart = Height - _fileselectboxheight;
                _openFile.Text = DialogMode.ToString();
                _openFile.Visible = true;
                _openFile.X = (Width - _openFile.Width) - 10;
                _openFile.Y = _fileselectstart + ((_fileselectboxheight - _openFile.Height) / 2);

                _filePrompt.AutoSize = true;
                _filePrompt.Text = $"{DialogMode} file: ";
                _filePrompt.Layout(gameTime);

                _filePrompt.X = 10;
                _filePrompt.Y = _fileselectstart + ((_fileselectboxheight - _filePrompt.Height) / 2);
                _filePrompt.Visible = true;

                _fileFilter.AutoSize = true;
                _fileFilter.Visible = true;
                _fileFilter.Text = FileFilters[SelectedFilter];
                _fileFilter.Layout(gameTime);
                _fileFilter.X = _openFile.X - _fileFilter.Width - 10;
                _fileFilter.Y = _fileselectstart + ((_fileselectboxheight - _fileFilter.Height) / 2);

                _fileBox.X = _filePrompt.X + _filePrompt.Width + 10;
                _fileBox.Width = _fileFilter.X - _fileBox.X;
                _fileBox.Height = 24;
                _fileBox.Y = _fileselectstart + ((_fileselectboxheight - _fileBox.Height) / 2);
                _fileBox.Visible = true;
            }
            else
            {
                _fileBox.Visible = false;
                this._filePrompt.Visible = false;
                this._openFile.Visible = false;
                this._fileFilter.Visible = false;
            }
        }
    }
}
