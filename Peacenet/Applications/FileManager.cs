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
        private Label _pathLabel = new Label();
        private PictureBox _back = new PictureBox();
        private PictureBox _forward = new PictureBox();
        private PictureBox _newFolder = new PictureBox();
        private PictureBox _up = new PictureBox();
        private PictureBox _refresh = new PictureBox();

        private const int _toolbarIconSize = 20;

        private ScrollView _sidebarView = new ScrollView();
        private ScrollView _filesView = new ScrollView();

        private ListBox _sidebarFolderList = new ListBox();

        private GridListView _fileList = new GridListView();

        private string _currentPath = "/home";

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
                var mime = _futils.GetMimeType(name);
                var existingMimeIcon = _fileList.GetImage(mime);
                if (existingMimeIcon == null)
                    _fileList.SetImage(mime, _futils.GetMimeIcon(mime));

                _fileList.Items.Add(new ListViewItem(name, mime, file));
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

            ResetLists();
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

            _sidebarView.Height = (Height - _sidebarView.Y);
            _filesView.Height = _sidebarView.Height;

            _sidebarView.X = 0;
            _sidebarView.Width = 175;
            _filesView.X = _sidebarView.X + _sidebarView.Width + 2;
            _filesView.Width = (Width - _filesView.X);

            _sidebarFolderList.Width = _sidebarView.Width;
            _fileList.Width = _filesView.Width;


            base.OnUpdate(time);
        }
    }
}
