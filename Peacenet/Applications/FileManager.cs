using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peacenet.Filesystem;
using Peacenet.CoreUtils;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a graphical user interface allowing the player to browse and manage files and folders in their in-game hard drive.
    /// </summary>
    [AppLauncher("File browser", "Accessories", "Manage your computer's files")]
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
        private Button _newFolder = new Button();


        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private FSManager _fs = null;

        private Stack<string> _pastLocs = new Stack<string>();
        private Stack<string> _futureLocs = new Stack<string>();

        private Button _open = new Button();
        private TextBox _openField = new TextBox();

        [Dependency]
        private OS _os = null;

        [Dependency]
        private FileUtils _futils = null;

        private bool _isDialog = false;

        private bool _showHidden = false;

        private string _currentDirectory = "/";

        private Action<string> _openCallback = null;

        private bool _isSaving = false;

        /// <summary>
        /// Sets a callback <see cref="Action"/> to be run when the user selects a file path. Setting the callback to null transforms the GUI into "browse mode", as if the file manager was opened from the App Launcher or command-line. Setting it to anything else will transform the GUI into either "open mode" or "save mode" depending on the value of <paramref name="isSaving"/>. When the callback is run, the GUI is closed and the <see cref="string"/> argument of the callback is populated with an absolute path to the file the player selected. 
        /// </summary>
        /// <param name="callback">A callback to be run when the user selects a file. Set to null to disable dialog mode.</param>
        /// <param name="isSaving">If in dialog mode, whether or not the player is saving a file.</param>
        public void SetDialogCallback(Action<string> callback, bool isSaving)
        {
            _isSaving = isSaving;
            if (callback == null)
                _isDialog = false;
            else
            {
                _isDialog = true;
            }
            _openCallback = callback;
        }

        /// <summary>
        /// Changes the current directory of the GUI.
        /// </summary>
        /// <param name="dir">A path pointing to an existing Peacenet directory within the player's in-game FS.</param>
        public void SetCurrentDirectory(string dir)
        {
            if (_fs.DirectoryExists(dir))
            {
                _needsReset = true;
                _pastLocs.Clear();
                _futureLocs.Clear();
                _currentDirectory = dir;
            }
        }

        private void open(string filepath)
        {
            if (_isDialog)
            {
                if (_isSaving)
                {
                    if (_fs.FileExists(filepath))
                    {
                        Enabled = false;
                        _infobox.ShowYesNo("Overwrite file", $"The file {filepath} already exists. Do you really want to overwrite it?", (answer) =>
                        {
                            Enabled = true;
                            if (answer)
                            {
                                _openCallback?.Invoke(filepath);
                                Close();
                            }
                        });
                    }
                    else
                    {
                        _openCallback?.Invoke(filepath);
                        Close();
                    }
                }
                else
                {
                    _openCallback?.Invoke(filepath);
                    Close();
                }
            }
        }


        /// <inheritdoc/>
        public FileManager(WindowSystem _winsys) : base(_winsys)
        {
            AddChild(_places);
            AddChild(_files);
            AddChild(_back);
            AddChild(_forward);
            AddChild(_path);
            AddChild(_search);
            AddChild(_searchButton);
            AddChild(_newFolder);
            _places.AddChild(_placesView);
            _files.AddChild(_filesView);
            Width = 800;
            Height = 600;
            Title = "File browser";

            AddChild(_open);
            AddChild(_openField);

            _newFolder.Text = "New folder";
            _newFolder.Click += (o, a) =>
            {
                string folderPath = _currentDirectory;
                _infobox.PromptText("New folder", "Please enter a name for your new folder.", (name) =>
                {
                    string fullPath = (folderPath.EndsWith("/")) ? folderPath + name : folderPath + "/" + name;
                    _fs.CreateDirectory(fullPath);
                    _needsReset = true;
                }, (proposedName)=>
                {
                    if(string.IsNullOrWhiteSpace(proposedName))
                    {
                        _infobox.Show("New folder", "Your folder's name must not be blank.");
                        return false;
                    }

                    foreach(char c in proposedName)
                    {
                        if (char.IsLetterOrDigit(c))
                            continue;
                        if (c == '_' || c == ' ' || c == '-' || c == '.')
                            continue;
                        _infobox.Show("Invalid path character", "Your new folder's name contains an invalid character. Valid characters include any letter or number as well as '.', '_', '-' or a space.");
                        return false;
                    }

                    string fullPath = (folderPath.EndsWith("/")) ? folderPath + proposedName : folderPath + "/" + proposedName;
                    if(_fs.DirectoryExists(fullPath) || _fs.FileExists(fullPath))
                    {
                        _infobox.Show("New folder", "A folder or file already exists with that name.");
                        return false;
                    }

                    return true;
                });
            };

            _placesView.ItemClicked += (item) =>
            {
                if (_placesView.SelectedIndex == -1)
                    return;
                var path = item?.Tag?.ToString();
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
                var lvitem = new ListViewItem();
                lvitem.Value = shelldir.FriendlyName;
                lvitem.Tag = shelldir.Path;
                lvitem.ImageKey = lvitem.Tag.ToString();
                if (shelldir.Texture != null)
                    _placesView.SetImage(lvitem.ImageKey, shelldir.Texture);
                _placesView.AddItem(lvitem);
            }

            _filesView.SetImage("directory", _plexgate.Content.Load<Texture2D>("UIIcons/folder"));

            _filesView.SelectedIndexChanged += (o, a) =>
            {
                if(_filesView.SelectedItem != null)
                {
                    _openField.Text = _filesView.SelectedItem.Value.ToString();
                }
            };
            _filesView.ItemClicked += (item) =>
            {
                if (_fs.DirectoryExists(item.Tag.ToString()))
                {
                    _futureLocs.Clear();
                    _pastLocs.Push(_currentDirectory);
                    _currentDirectory = item.Tag.ToString();
                    _needsReset = true;
                }
                else
                {
                    if (_isDialog)
                    {
                        open(item.Tag.ToString());
                        return;
                    }
                    if(!_utils.OpenFile(item.Tag.ToString()))
                    {
                        _infobox.Show("Can't open file", "File Manager couldn't find a program that can open that file!");
                    }
                    return;
                }
            };
            _open.Click += (o, a) =>
            {
                string path = _openField.Text;
                if (path.StartsWith("/"))
                    path = _futils.Resolve(path);
                else
                    path = _futils.Resolve(_currentDirectory + "/" + path);
                if (_isSaving == false)
                {
                    if (_fs.FileExists(path))
                    {
                        open(path);
                        return;
                    }
                    Enabled = false;
                    _infobox.Show("File not found.", $"The system could not find the file specified: {path}", () =>
                    {
                        Enabled = true;
                    });
                }
                else
                {
                    open(path);
                }
            };
            _searchButton.Click += (o, a) =>
            {
                _filesView.Filter = _search.Text;
                if (_filesView.VisibleItems.Length == 0)
                {
                    _statusHead.Text = "No results found";
                    _statusDescription.Text = $"Your search \"{_search.Text}\" did not return any results.";
                    _statusDescription.Visible = true;
                    _statusHead.Visible = true;
                }
                else
                {
                    _statusHead.Visible = false;
                    _statusDescription.Visible = false;
                }
            };
            _search.KeyEvent += _search_KeyEvent;

            AddChild(_statusHead);
            AddChild(_statusDescription);
        }

        [Dependency]
        private FileUtilities _utils = null;

        private void _search_KeyEvent(object sender, MonoGame.Extended.Input.InputListeners.KeyboardEventArgs e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                _filesView.Filter = _search.Text;
                if (_filesView.VisibleItems.Length == 0)
                {
                    _statusHead.Text = "No results found";
                    _statusDescription.Text = $"Your search \"{_search.Text}\" did not return any results.";
                    _statusDescription.Visible = true;
                    _statusHead.Visible = true;
                }
                else
                {
                    _statusHead.Visible = false;
                    _statusDescription.Visible = false;
                }
            }

        }

        private bool _needsReset = true;

        private Label _statusHead = new Label();
        private Label _statusDescription = new Label();

        /// <summary>
        /// Force a reset of the GUI causing reloading of icons and re-population of the Places and Files lists.
        /// </summary>
        public void ResetUI()
        {
            _search.Text = "";
            _filesView.Filter = null;
            _back.Image = _plexgate.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-left");
            _forward.Image = _plexgate.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-right");
            _searchButton.Image = _plexgate.Content.Load<Texture2D>("UIIcons/search");
            _forward.ShowImage = true;
            _back.ShowImage = true;
            _searchButton.ShowImage = true;

            var placesItems = _placesView.Items;
            var currentWork = placesItems.OrderByDescending(x => x.Tag.ToString().Length).FirstOrDefault(x => _currentDirectory.StartsWith(x.Tag.ToString()));
            if (currentWork != null)
                _placesView.SelectedIndex = Array.IndexOf(placesItems, currentWork);
            else
                _placesView.SelectedIndex = -1;

            _filesView.ClearItems();
            if (!_fs.DirectoryExists(_currentDirectory))
            {
                Enabled = false;
                _infobox.Show("File manager", $"The directory {_currentDirectory} was not found on your system.", () =>
                {
                    _currentDirectory = "/";
                    _needsReset = true;
                    Enabled = true;
                });
                return;
            }
            bool noFiles = true;

            foreach (var dir in _fs.GetDirectories(_currentDirectory))
            {
                string shorthand = _futils.GetNameFromPath(dir);
                if (shorthand == "." || shorthand == "..")
                    continue;
                if (shorthand.StartsWith("."))
                    if (!_showHidden)
                        continue;
                var lvitem = new ListViewItem();
                lvitem.Value = shorthand;
                lvitem.Tag = dir;
                lvitem.ImageKey = "directory";
                _filesView.AddItem(lvitem);
                noFiles = false;
            }
            foreach(var dir in _fs.GetFiles(_currentDirectory))
            {
                string shorthand = _futils.GetNameFromPath(dir);
                if (shorthand == "." || shorthand == "..")
                    continue;
                if (shorthand.StartsWith("."))
                    if (!_showHidden)
                        continue;
                var lvitem = new ListViewItem();
                lvitem.Value = shorthand;
                lvitem.Tag = dir;
                lvitem.ImageKey = _futils.GetMimeType(shorthand);
                if (_filesView.GetImage(lvitem.ImageKey) == null)
                {
                    _filesView.SetImage(lvitem.ImageKey, _futils.GetMimeIcon(lvitem.ImageKey));
                }
                _filesView.AddItem(lvitem);

                noFiles = false;
            }
            if(noFiles == true)
            {
                _statusHead.Text = "This folder is empty";
                _statusDescription.Text = "There are no files or folders to show in this folder.";
                _statusHead.Visible = true;
                _statusDescription.Visible = true;
                _searchButton.Enabled = false;
                _search.Enabled = false;
            }
            else
            {
                _statusHead.Visible = false;
                _statusDescription.Visible = false;
                _searchButton.Enabled = true;
                _search.Enabled = true;

            }
        }
        
        [Dependency]
        private InfoboxManager _infobox = null;

        /// <inheritdoc/>
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
            _newFolder.X = _forward.X + _forward.Width + 2;

            
            _searchButton.X = (Width - _searchButton.Width) - 5;
            _search.X = _searchButton.X - _search.Width - 2;
            _search.Y = 5;

            _path.X = _newFolder.X + _newFolder.Width + 5;
            _path.AutoSize = true;
            _path.MaxWidth = (_search.X) - (_path.X) - 5;
            _path.Text = "This is your current path...";
            _path.Y = (_search.Y) + ((_search.Height - _path.Height) / 2);

            _searchButton.Y = (_search.Y) + ((_search.Height - _searchButton.Height) / 2);
            _back.Y = (_search.Y) + ((_search.Height - _back.Height) / 2);
            _forward.Y = (_search.Y) + ((_search.Height - _forward.Height) / 2);
            _newFolder.Y = _forward.Y;

            _open.Visible = _isDialog;
            _openField.Visible = _isDialog;

            
            _places.X = 0;
            _places.Y = _search.Y + _search.Height + 5;
            _placesView.Width = Width / 3;
            _files.X = _places.Width + 2;
            _files.Y = _places.Y;
            _filesView.Width = Width - _files.X;
            _placesView.Layout = ListViewLayout.List;

            _path.Text = _currentDirectory;

            _back.Enabled = (_pastLocs.Count > 0);
            _forward.Enabled = (_futureLocs.Count > 0);

            _statusHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _statusDescription.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _statusHead.AutoSize = true;
            _statusDescription.AutoSize = true;
            _statusHead.MaxWidth = _filesView.Width - 60;
            _statusDescription.MaxWidth = _statusHead.MaxWidth;
            _statusHead.X = (_files.X) + ((_filesView.Width - _statusHead.Width) / 2);
            _statusDescription.X = (_files.X) + ((_filesView.Width - _statusDescription.Width) / 2);

            _statusHead.Y = _files.Y + 25;
            _statusDescription.Y = _statusHead.Y + _statusHead.Height + 5;

            if (_open.Visible)
            {
                _open.Text = (_isSaving) ? "Save" : "Open";
                _openField.Label = "File name or file path";

                float _larger = Math.Max(_open.Height, _openField.Height);
                float _starty = (Height - _larger) - 6;
                _open.Y = _starty + ((_larger - _open.Height) / 2)+3;
                _openField.Y = _starty + ((_larger - _openField.Height) / 2)+3;

                _open.X = (Width - _open.Width) - 3;
                _openField.X = 3;
                _openField.Width = (_open.X - _openField.X) - 6;
                _places.Height = (Height - _places.Y) - (_larger+3);
                _open.Enabled = !string.IsNullOrWhiteSpace(_openField.Text);
            }
            else
            {
                _places.Height = Height - _places.Y;

            }
            _files.Height = _places.Height;
        }
    }
}
