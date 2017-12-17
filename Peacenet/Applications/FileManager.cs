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

            _placesView.Clear();
            _filesView.Clear();

            foreach(var dir in _fs.GetDirectories(_currentDirectory))
            {
                var lvitem = new ListViewItem(_filesView);
                lvitem.ImageKey = "dir";
                lvitem.Value = _fs.GetFileRecord(dir).Name;
                lvitem.Tag = dir;
            }

            _filesView.SetImage("dir", _plexgate.Content.Load<Texture2D>("UIIcons/folder"));

            _placesView.SetImage("home", _plexgate.Content.Load<Texture2D>("UIIcons/home"));
            _placesView.SelectedIndex = -1;

            var homedir = new ListViewItem(_placesView);
            homedir.ImageKey = "home";
            homedir.Tag = "/home";
            homedir.Value = "Your home";
        }

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
