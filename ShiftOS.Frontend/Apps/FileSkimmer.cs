using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;
using static Plex.Objects.ShiftFS.Utils;

namespace Plex.Frontend.Apps
{
    [WinOpen("fileskimmer")]
    [Launcher("File Skimmer", false, null, "System")]
    [DefaultTitle("File Skimmer")]
    public class FileSkimmer : GUI.Control, IPlexWindow
    {
        private string _currentdirectory = "0:";
        private const string SD_SYSTEM = "__system";
        private GUI.ListView _fList = null;
        private GUI.TextControl _currentdirtext = null;
        private GUI.Button _openFile = null;
        private GUI.TextControl _fileFilter = null;
        private GUI.TextControl _filePrompt = null;
        private GUI.TextInput _fileBox = null;


        public bool IsDialog = false;
        public Action<string> DialogCallback = null;
        public string[] FileFilters = new string[] { "" };
        public FileOpenerStyle DialogMode = FileOpenerStyle.Open;

        public int SelectedFilter = 0;

        public FileSkimmer()
        {
            _fileBox = new GUI.TextInput();
            _filePrompt = new TextControl();
            _fileFilter = new TextControl();
            _openFile = new Button();

            AddControl(_openFile);
            AddControl(_fileBox);
            AddControl(_filePrompt);
            AddControl(_fileFilter);

            _openFile.AutoSize = true;
            _fileFilter.AutoSize = true;
            _filePrompt.AutoSize = true;

            Width = 720;
            Height = 480;
            _fList = new GUI.ListView();
            //TODO: keyboard support in listviews
            /*
            _fList.KeyEvent += (e) =>
            {
                if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    Navigate(_fList.SelectedItem.ToString());
                }
            };*/
            _fList.DoubleClick += () =>
            {
                try
                {
                    Navigate(_fList.SelectedItem.Tag);
                }
                catch { }
            };
            AddControl(_fList);
            _currentdirtext = new GUI.TextControl();
            _currentdirtext.AutoSize = true;
            AddControl(_currentdirtext);
            ResetList();

        }

        public void OnLoad()
        {
            Width = 720;
            Height = 480;
            _fList = new GUI.ListView();
            //TODO: keyboard support in listviews
            /*
            _fList.KeyEvent += (e) =>
            {
                if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    Navigate(_fList.SelectedItem.ToString());
                }
            };*/
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
            _fList.DoubleClick += () =>
            {
                try
                {
                    Navigate(_fList.SelectedItem.Tag);
                }
                catch { }
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
            AddControl(_fList);
            _currentdirtext = new GUI.TextControl();
            _currentdirtext.AutoSize = true;
            AddControl(_currentdirtext);
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
                    throw new NaughtyDeveloperException("Someone tried to make it so you can go \"up one directory\" in the mounts list.");
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

        public void OnSkinLoad()
        {
            foreach(var name in Enum.GetNames(typeof(FileType)))
            {
                FileType ftype = (FileType)Enum.Parse(typeof(FileType), name);
                var img = ToTexture2D(GetImage(ftype));
                _fList.SetImage(name, img);
            }


            _currentdirtext.Font = SkinEngine.LoadedSkin.Header3Font;
        }

        public System.Drawing.Image GetImage(FileType type)
        {
            switch (type)
            {
                case FileType.UpOne:
                    return Properties.Resources.fileicon5;
                case FileType.Mount:
                case FileType.Directory:
                    return Properties.Resources.fileicon0;
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
                _fList.AddItem(new GUI.ListViewItem
                {
                    Text = "Up one...",
                    Tag = "__up",
                    ImageKey = FileType.UpOne.ToString()
                });

            if(_currentdirectory == SD_SYSTEM)
            {
                foreach(var mount in Mounts)
                {
                    string mountpath = $"{Mounts.IndexOf(mount)}:";
                    string name = $"{mount.Name} ({mountpath})";
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
                    var dinf = GetDirectoryInfo(dir);
                    _fList.AddItem(new ListViewItem
                    {
                        Text = dinf.Name,
                        Tag = dir,
                        ImageKey = FileType.Directory.ToString()
                    });
                }
                foreach (var dir in GetFiles(_currentdirectory))
                {
                    var dinf = GetFileInfo(dir);
                    if (dinf.Name.EndsWith(FileFilters[SelectedFilter]))
                    {
                        var ext = FileSkimmerBackend.GetFileType(dir);


                        _fList.AddItem(new ListViewItem
                        {
                            Text = dinf.Name,
                            Tag = dir,
                            ImageKey = ext.ToString()
                        });
                    }
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

            _fList.X = 0;
            _fList.Y = 0;
            _fList.Width = Width;
            _fList.Height = listbottom;
            _currentdirtext.X = (Width - _currentdirtext.Width) / 2;
            _currentdirtext.Y = _fList.Height;

            if (IsDialog)
            {
                int _fileselectstart = Height - _fileselectboxheight;
                _openFile.Text = DialogMode.ToString();
                _openFile.Visible = true;
                _openFile.X = (Width - _openFile.Width) - 10;
                _openFile.Y = _fileselectstart + ((_fileselectboxheight - _openFile.Height) / 2);

                _filePrompt.AutoSize = true;
                _filePrompt.Text = $"{DialogMode} file: ";
                _filePrompt.Font = SkinEngine.LoadedSkin.MainFont;
                _filePrompt.Layout(gameTime);

                _filePrompt.X = 10;
                _filePrompt.Y = _fileselectstart + ((_fileselectboxheight - _filePrompt.Height) / 2);
                _filePrompt.Visible = true;

                _fileFilter.AutoSize = true;
                _fileFilter.Visible = true;
                _fileFilter.Text = FileFilters[SelectedFilter];
                _fileFilter.Font = SkinEngine.LoadedSkin.MainFont;
                _fileFilter.Layout(gameTime);
                _fileFilter.X = _openFile.X - _fileFilter.Width - 10;
                _fileFilter.Y = _fileselectstart + ((_fileselectboxheight - _fileFilter.Height) / 2);

                _fileBox.X = _filePrompt.X + _filePrompt.Width + 10;
                _fileBox.Width = _fileFilter.X - _fileBox.X;
                _fileBox.Height = 24;
                _fileBox.Y = _fileselectstart + ((_fileselectboxheight - _fileBox.Height) / 2);
                _fileBox.Visible = true;
            }

        }
    }
}
