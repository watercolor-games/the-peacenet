using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Filesystem;
using Peacenet.CoreUtils;

namespace Peacenet.Applications
{
    [AppLauncher("File browser", "Accessories")]
    public class FileManager : Window
    {
        private ScrollView _places = new ScrollView();
        private ListView _placesView = new ListView();
        private ScrollView _files = new ScrollView();
        private ListView _filesView = new ListView();
        private Button _back = new Button();
        private Button _forward = new Button();
        private Label _path = new Label();
        private TextBox _search = new TextBox();
        private Button _searchButton = new Button();

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private FSManager _fs = null;

        private Stack<string> _pastLocs = new Stack<string>();
        private Stack<string> _futureLocs = new Stack<string>();

        [Dependency]
        private OS _os = null;

        [Dependency]
        private FileUtils _futils = null;

        private bool _showHidden = false;

        private string _currentDirectory = "/";

        public FileManager(WindowSystem _winsys) : base(_winsys)
        {
            AddChild(_places);
            AddChild(_files);
            AddChild(_back);
            AddChild(_forward);
            AddChild(_path);
            AddChild(_search);
            AddChild(_searchButton);
            _places.AddChild(_placesView);
            _files.AddChild(_filesView);
            Width = 800;
            Height = 600;
            Title = "File browser";

            _placesView.SelectedIndexChanged += (o, a) =>
            {
                if (_placesView.SelectedIndex == -1)
                    return;
                var path = _placesView.SelectedItem?.Tag?.ToString();
                if(path != _currentDirectory)
                {
                    _pastLocs.Push(_currentDirectory);
                    _futureLocs.Clear();
                    _currentDirectory = path;
                    _needsReset = true;
                }
            };

            _back.Click += (o, a) =>
            {
                if (_pastLocs.Count > 0)
                {
                    _futureLocs.Push(_currentDirectory);
                    _currentDirectory = _pastLocs.Pop();
                    _needsReset = true;
                }
            };
            _forward.Click += (o, a) =>
            {
                if (_futureLocs.Count > 0)
                {
                    _pastLocs.Push(_currentDirectory);
                    _currentDirectory = _futureLocs.Pop();
                    _needsReset = true;
                }
            };

            foreach(var shelldir in _os.GetShellDirs())
            {
                var lvitem = new ListViewItem(_placesView);
                lvitem.Value = shelldir.FriendlyName;
                lvitem.Tag = shelldir.Path;
                lvitem.ImageKey = lvitem.Tag.ToString();
                if (shelldir.Texture != null)
                    _placesView.SetImage(lvitem.ImageKey, shelldir.Texture);
            }
        }

        private bool _needsReset = true;

        public void ResetUI()
        {
            _back.Image = _plexgate.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-left");
            _forward.Image = _plexgate.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-right");
            _searchButton.Image = _plexgate.Content.Load<Texture2D>("UIIcons/search");
            _forward.ShowImage = true;
            _back.ShowImage = true;
            _searchButton.ShowImage = true;

            var placesItems = _placesView.GetItems();
            var currentWork = placesItems.OrderByDescending(x => x.Tag.ToString().Length).FirstOrDefault(x => _currentDirectory.StartsWith(x.Tag.ToString()));
            if (currentWork != null)
                _placesView.SelectedIndex = Array.IndexOf(placesItems, currentWork);
            else
                _placesView.SelectedIndex = -1;

            _filesView.Clear();
            if (!_fs.DirectoryExists(_currentDirectory))
            {
                _infobox.Show("File manager", $"The directory {_currentDirectory} was not found on your system.", () =>
                {
                    _currentDirectory = "/";
                    _needsReset = true;
                });
                return;
            }

            foreach(var dir in _fs.GetDirectories(_currentDirectory))
            {
                string shorthand = _futils.GetNameFromPath(dir);
                if (shorthand == "." || shorthand == "..")
                    continue;
                if (shorthand.StartsWith("."))
                    if (!_showHidden)
                        continue;
                var lvitem = new ListViewItem(_filesView);
                lvitem.Value = shorthand;
                lvitem.Tag = dir;
                lvitem.ImageKey = "directory";
            }
            foreach(var dir in _fs.GetFiles(_currentDirectory))
            {
                string shorthand = _futils.GetNameFromPath(dir);
                if (shorthand == "." || shorthand == "..")
                    continue;
                if (shorthand.StartsWith("."))
                    if (!_showHidden)
                        continue;
                var lvitem = new ListViewItem(_filesView);
                lvitem.Value = shorthand;
                lvitem.Tag = dir;
                lvitem.ImageKey = _futils.GetMimeType(shorthand);

            }
        }
        
        [Dependency]
        private InfoboxManager _infobox = null;

        protected override void OnUpdate(GameTime time)
        {
            if(_needsReset)
            {
                ResetUI();
                _needsReset = false;
            }
            _search.Width = 350;
            _search.Label = "Search...";
            base.OnUpdate(time);
            _back.X = 5;
            _forward.X = _back.X + _back.Width + 2;
            
            _searchButton.X = (Width - _searchButton.Width) - 5;
            _search.X = _searchButton.X - _search.Width - 2;
            _search.Y = 5;

            _path.X = _forward.X + _forward.Width + 5;
            _path.AutoSize = true;
            _path.MaxWidth = (_search.X) - (_path.X) - 5;
            _path.Text = "This is your current path...";
            _path.Y = (_search.Y) + ((_search.Height - _path.Height) / 2);

            _searchButton.Y = (_search.Y) + ((_search.Height - _searchButton.Height) / 2);
            _back.Y = (_search.Y) + ((_search.Height - _back.Height) / 2);
            _forward.Y = (_search.Y) + ((_search.Height - _forward.Height) / 2);

            _places.X = 0;
            _places.Y = _search.Y + _search.Height + 5;
            _placesView.Width = Width / 3;
            _places.Height = Height - _places.Y;
            _files.X = _places.Width + 2;
            _files.Y = _places.Y;
            _filesView.Width = Width - _files.X;
            _files.Height = _places.Height;
            _placesView.Layout = ListViewLayout.List;

            _path.Text = _currentDirectory;
        }
    }
}
