using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peacenet.CoreUtils;
using Peacenet.Filesystem;
using Plex.Engine;
using Plex.Engine.GUI;

namespace Peacenet.Applications
{
    [AppLauncher("File Manager", "System", "View and manage the files and folders on your system.")]
    public class FileManager : Window
    {
        private Stack<string> _prevStack = new Stack<string>();
        private Stack<string> _nextStack = new Stack<string>();


        private Label _pathLabel = new Label();
        private PictureBox _back = new PictureBox();
        private PictureBox _forward = new PictureBox();
        private PictureBox _newFolder = new PictureBox();
        private PictureBox _up = new PictureBox();
        private PictureBox _refresh = new PictureBox();

        private PictureBox _action = new PictureBox();
        private Texture2D _save = null;
        private Texture2D _open = null;
        private TextBox _filename = new TextBox();

        private const int _toolbarIconSize = 20;

        private ScrollView _sidebarView = new ScrollView();
        private ScrollView _filesView = new ScrollView();

        private ListBox _sidebarFolderList = new ListBox();

        private GridListView _fileList = new GridListView();

        private string _currentPath = "/home";

        private string[] _filter = null;

        private FileManagerMode _mode = FileManagerMode.Browse;

        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private GameLoop _gameloop = null;

        [Dependency]
        private OS _os = null;

        public string[] FileFilter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                ResetLists();
            }
        }
        public FileManagerMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                ResetLists();
            }
        }


        private Texture2D _directoryTexture = null;

        public string CurrentPath
        {
            get
            {
                return _currentPath;
            }
            set
            {
                if (_currentPath == value)
                    return;

                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!_fs.DirectoryExists(value))
                    throw new DirectoryNotFoundException($"The directory \"{value}\" does not exist.");

                _currentPath = value;
                ResetLists();
            }
        }

        private void ResetLists()
        {
            var shellPaths = _os.GetShellDirs().ToArray();
            var foundPath = shellPaths.OrderByDescending(x => x.Path.Length).FirstOrDefault(x => _currentPath.StartsWith(x.Path));
            int sidebarSelected = -1;
            if (foundPath != ShellDirectoryInformation.Empty)
                sidebarSelected = Array.IndexOf(shellPaths, foundPath);

            _fileList.Items.Clear();
            _sidebarFolderList.Items.Clear();

            foreach(var item in shellPaths)
            {
                _sidebarFolderList.Items.Add(item);
            }
            _sidebarFolderList.SelectedIndex = sidebarSelected;

            foreach(var dir in _fs.GetDirectories(_currentPath))
            {
                var name = _futils.GetNameFromPath(dir);
                if (name.StartsWith("."))
                    continue;
                _fileList.Items.Add(new ListViewItem(name, "dir", dir));
            }

            foreach (var file in _fs.GetFiles(_currentPath))
            {
                var name = _futils.GetNameFromPath(file);
                if (name.StartsWith("."))
                    continue;
                if (_mode != FileManagerMode.Browse && _filter != null && _filter.Length > 0)
                    if (!_filter.Contains(_futils.GetMimeType(name)))
                        continue;

                var mime = _futils.GetMimeType(name);
                var existingMimeIcon = _fileList.GetImage(mime);
                if (existingMimeIcon == null)
                    _fileList.SetImage(mime, _futils.GetMimeIcon(mime));

                _fileList.Items.Add(new ListViewItem(name, mime, file));
            }

            if (_mode == FileManagerMode.Browse)
            {
                _action.Visible = false;
                _filename.Visible = false;
            }
            else
            {
                _action.Visible = true;
                _filename.Visible = true;
             
                _action.Texture = (_mode == FileManagerMode.SaveFile) ? _save : _open;
            }
        }

        public FileManager(WindowSystem _winsys) : base(_winsys)
        {
            _directoryTexture = _gameloop.Content.Load<Texture2D>("UIIcons/folder");

            AddChild(_pathLabel);
            _pathLabel.AutoSize = true;

            AddChild(_back);
            AddChild(_forward);
            AddChild(_up);
            AddChild(_refresh);
            AddChild(_newFolder);

            AddChild(_sidebarView);
            AddChild(_filesView);

            _sidebarView.AddChild(_sidebarFolderList);
            _filesView.AddChild(_fileList);

            _sidebarFolderList.AutoSize = true;
            _fileList.AutoSize = true;

            Width = 550;
            Height = 300;

            Title = "File Manager";

            _fileList.SetImage("dir", _directoryTexture);

            _save = _gameloop.Content.Load<Texture2D>("UIIcons/save");
            _open = _gameloop.Content.Load<Texture2D>("UIIcons/folder-open-o");


            ResetLists();

            _fileList.SelectedIndexChanged += (o, a) =>
            {
                if (_mode == FileManagerMode.Browse)
                    return;

                var selected = _fileList.SelectedItem;
                if (selected == null)
                    _filename.Text = "";
                else
                    _filename.Text = selected.Text;
            };

            _sidebarFolderList.Click += (o, a) =>
            {
                var selectedItem = _sidebarFolderList.SelectedItem;
                if(selectedItem != null)
                {
                    _prevStack.Push(CurrentPath);
                    _nextStack.Clear();
                    CurrentPath = ((ShellDirectoryInformation)selectedItem).Path;
                }
            };

            _fileList.DoubleClick += (o, a) =>
            {
                var selected = _fileList.SelectedItem;
                if(selected != null)
                {
                    if (_fs.DirectoryExists(selected.Tag.ToString()))
                    {
                        _prevStack.Push(CurrentPath);
                        _nextStack.Clear();
                        CurrentPath = selected.Tag.ToString();
                    }
                    else if (_fs.FileExists(selected.Tag.ToString()))
                    {
                        if (_mode == FileManagerMode.Browse)
                        {
                            _infobox.Show("Not yet implemented", "You cannot yet open files from File Manager.");
                            return;
                        }

                        HandleFileSelect(selected.Tag.ToString());
                    }
                        
                }
            };

            _back.Texture = _gameloop.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-left");
            _forward.Texture = _gameloop.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-right");
            _refresh.Texture = _gameloop.Content.Load<Texture2D>("UIIcons/refresh");
            _newFolder.Texture = _gameloop.Content.Load<Texture2D>("UIIcons/folder");
            _up.Texture = _gameloop.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-up");

            _up.Click += (o, a) =>
            {
                string path = CurrentPath;
                if (path.EndsWith("/"))
                    path = path.Remove(path.LastIndexOf("/"), 1);
                if(string.IsNullOrWhiteSpace(path))
                {
                    _infobox.Show("Invalid Operation", "You cannot move up beyond the root folder '/'.");
                    return;
                }
                int lastSlash = path.LastIndexOf("/");
                string parent = path.Substring(0, lastSlash);
                if (string.IsNullOrWhiteSpace(parent))
                    parent = "/";
                _prevStack.Push(CurrentPath);
                _nextStack.Clear();
                CurrentPath = parent;
            };

            _newFolder.Click += (o, a) =>
            {
                string path = CurrentPath;
                _infobox.PromptText("New folder", "Please enter a name for your new folder.", (name) =>
                {
                    if (path.EndsWith("/"))
                        path += name;
                    else
                        path += "/" + name;
                    if(_fs.DirectoryExists(path))
                    {
                        _infobox.Show("Folder exists", "There is already an existing folder with that name.");
                        return;
                    }

                    _fs.CreateDirectory(path);
                    ResetLists();
                }, (name) =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        _infobox.Show("Invalid folder name", "Folder names cannot be blank.");
                        return false;
                    }
                    string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_ ";
                    if (name.Any(x => !validChars.Contains(x)))
                    {
                        _infobox.Show("Invalid folder name", "The name you entered contains characters not valid in a folder name.");
                        return false;
                    }
                    return true;

                });
            };

            _refresh.Click += (o, a) =>
            {
                ResetLists();
            };

            _back.Click += (o, a) =>
            {
                _nextStack.Push(CurrentPath);
                CurrentPath = _prevStack.Pop();
            };
            _forward.Click += (o, a) =>
            {
                _prevStack.Push(CurrentPath);
                CurrentPath = _nextStack.Pop();
            };

            _action.Click += (o, a) =>
            {
                HandleActionClick();
            };

            _filename.KeyEvent += (o, a) =>
            {
                if(a.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    HandleActionClick();
                }
            };

            AddChild(_filename);
            AddChild(_action);
        }

        private void HandleActionClick()
        {
            string path = CurrentPath;
            if (path.EndsWith("/"))
                path += _filename.Text;
            else
                path += "/" + _filename.Text;

            if(_fs.DirectoryExists(path))
            {
                _prevStack.Push(CurrentPath);
                _nextStack.Clear();
                CurrentPath = path;
                _filename.Text = "";
            }
            else if(_fs.FileExists(path))
            {
                string name = _futils.GetNameFromPath(path);
                if(_filter == null || _filter.Length == 0)
                    HandleFileSelect(path);
                else if(!_filter.Contains(_futils.GetMimeType(name)))
                {
                    _infobox.Show("File not found", "No file with that name exists in the current folder.");
                    return;
                }
                HandleFileSelect(path);
                    

            }
            else
            {
                if (_mode == FileManagerMode.SaveFile)
                    HandleFileSelect(path);
                else
                    _infobox.Show("File not found", "No file with that name exists in the current folder.");
            }
        }

        public event Action<string> FileSelected;

        private void HandleFileSelect(string path)
        {
            if (_mode == FileManagerMode.OpenFile)
            {
                FileSelected?.Invoke(path);
            }
            else if (_mode == FileManagerMode.SaveFile)
            {
                if (_fs.FileExists(path))
                {
                    _infobox.ShowYesNo("Overwrite file?", "Are you sure you want to overwrite " + path + "?", (answer) =>
                    {
                        if (answer)
                            FileSelected?.Invoke(path);
                    });
                }
                else
                {
                    FileSelected?.Invoke(path);
                }
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            _back.Width = _toolbarIconSize;
            _back.Height = _toolbarIconSize;
            _forward.Width = _toolbarIconSize;
            _forward.Height = _toolbarIconSize;
            _up.Width = _toolbarIconSize;
            _up.Height = _toolbarIconSize;
            _newFolder.Width = _toolbarIconSize;
            _newFolder.Height = _toolbarIconSize;
            _refresh.Width = _toolbarIconSize;
            _refresh.Height = _toolbarIconSize;


            _pathLabel.Text = _currentPath;

            int toolbarPaddingV = 4;
            int toolbarBaseHeight = Math.Max(_pathLabel.Height, _toolbarIconSize);
            if(toolbarBaseHeight == _pathLabel.Height)
            {
                _pathLabel.Y = toolbarPaddingV;
                _back.Y = _pathLabel.Y + ((_pathLabel.Height - _toolbarIconSize) / 2);
            }
            else
            {
                _back.Y = toolbarPaddingV;
                _pathLabel.Y = _back.Y + ((_back.Height - _pathLabel.Height) / 2);
            }

            int bottomStart = Height;

            if (_action.Visible)
            {
                _action.Width = 20;
                _action.Height = 20;

                int bottomHeight = Math.Max(_action.Height, _filename.Height);

                if(bottomHeight == _action.Height)
                {
                    _action.Y = (Height - 3) - _action.Height;
                    _filename.Y = _action.Y + ((_action.Height - _filename.Height) / 2);
                    bottomStart = _action.Y - 3;
                }
                else
                {
                    _filename.Y = (Height - 3) - _filename.Height;
                    _action.Y = _filename.Y + ((_filename.Height - _action.Height) / 2);
                    bottomStart = _filename.Y - 3;
                }

                _action.X = (Width - 3) - _action.Width;

                _filename.X = 3;
                _filename.Width = _action.X - 6;

                _filename.Label = "Filename";

                _action.Enabled = !string.IsNullOrEmpty(_filename.Text);
            }


            _back.X = toolbarPaddingV;
            _forward.X = _back.X + _back.Width + toolbarPaddingV;
            _up.X = _forward.X + _forward.Width + toolbarPaddingV;
            _newFolder.X = _up.X + _up.Width + toolbarPaddingV;
            _refresh.X = _newFolder.X + _newFolder.Width + toolbarPaddingV;

            _forward.Y = _back.Y;
            _up.Y = _back.Y;
            _newFolder.Y = _back.Y;
            _refresh.Y = _back.Y;

            _pathLabel.X = _refresh.X + _refresh.Width + (toolbarPaddingV * 2);

            _sidebarView.Y = toolbarBaseHeight + (toolbarPaddingV * 2);
            _filesView.Y = _sidebarView.Y;

            _sidebarView.Height = (bottomStart - _sidebarView.Y);
            _filesView.Height = _sidebarView.Height;

            _sidebarView.X = 0;
            _sidebarView.Width = 175;
            _filesView.X = _sidebarView.X + _sidebarView.Width + 2;
            _filesView.Width = (Width - _filesView.X);

            _sidebarFolderList.Width = _sidebarView.Width;
            _fileList.Width = _filesView.Width;

            _back.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_back.LeftButtonPressed)
                _back.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_back.ContainsMouse)
                _back.Tint = Theme.GetAccentColor();

            _refresh.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_refresh.LeftButtonPressed)
                _refresh.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_refresh.ContainsMouse)
                _refresh.Tint = Theme.GetAccentColor();

            _forward.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_forward.LeftButtonPressed)
                _forward.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_forward.ContainsMouse)
                _forward.Tint = Theme.GetAccentColor();

            _newFolder.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_newFolder.LeftButtonPressed)
                _newFolder.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_newFolder.ContainsMouse)
                _newFolder.Tint = Theme.GetAccentColor();

            _up.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_up.LeftButtonPressed)
                _up.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_up.ContainsMouse)
                _up.Tint = Theme.GetAccentColor();

            _action.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_action.LeftButtonPressed)
                _action.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_action.ContainsMouse)
                _action.Tint = Theme.GetAccentColor();


            _up.Enabled = (CurrentPath != "/");
            _back.Enabled = _prevStack.Count > 0;
            _forward.Enabled = _nextStack.Count > 0;

            base.OnUpdate(time);
        }
    }

    public enum FileManagerMode
    {
        Browse,
        OpenFile,
        SaveFile
    }
}
