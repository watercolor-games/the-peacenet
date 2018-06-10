using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peacenet.CoreUtils;
using Microsoft.Xna.Framework;
using Peacenet.Filesystem;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a simple GUI-based text editor for the Peacenet.
    /// </summary>
    [AppLauncher("Text Editor", "Accessories", "Write, save, and open a text document")]
    public class TextEditorApp : Window, IFileViewer
    {
        [Dependency]
        private GUIUtils _guiutils = null;

        [Dependency]
        private FileUtils _futils = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private GameLoop _gameloop = null;

        private string _path = null;

        private PictureBox _open = new PictureBox();
        private PictureBox _save = new PictureBox();
        private PictureBox _new = new PictureBox();
        private TextEditor _editor = new TextEditor();

        public string Text
        {
            get
            {
                return _editor.Text;
            }
            set
            {
                _editor.Text = value;
            }
        }

        public string FilePath => _path;

        /// <inheritdoc/>
        public TextEditorApp(WindowSystem _winsys) : base(_winsys)
        {
            Title = "New document - Text Editor";
            Width = 450;
            Height = 300;
            AddChild(_open);
            AddChild(_save);
            AddChild(_new);
            AddChild(_editor);

            _new.Texture = _gameloop.Content.Load<Texture2D>("UIIcons/FileIcons/unknown");
            _save.Texture = _gameloop.Content.Load<Texture2D>("UIIcons/save");
            _open.Texture = _gameloop.Content.Load<Texture2D>("UIIcons/folder-open-o");


            _new.Click += (o, a) =>
            {
                _editor.Text = "";
                Title = "New document - Text Editor";
                _path = null;
            };
            _open.Click += (o, a) =>
            {
                _guiutils.AskForFile(false, new[] { "text/plain" }, (file) =>
                {
                    try
                    {
                        _editor.Text = _fs.ReadAllText(file);
                        Title = $"{_futils.GetNameFromPath(file)} - Text Editor";
                        _path = file;
                    }
                    catch (InvalidDataException)
                    {
                        _infobox.Show("Text Editor", $"'{file}' contains invalid characters and could not be opened.  Are you sure it's a text document?");
                    }
                });
            };
            _save.Click += (o, a) =>
            {
                _guiutils.AskForFile(true, new[] { "text/plain" }, (file) =>
                {
                    _fs.WriteAllText(file, _editor.Text);
                    Title = $"{_futils.GetNameFromPath(file)} - Text Editor";
                    _path = file;
                });
            };
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _new.Width = 20;
            _open.Width = 20;
            _save.Width = 20;
            _new.Height = 20;
            _open.Height = 20;
            _save.Height = 20;

            _new.X = 3;
            _new.Y = 3;
            _open.Y = 3;
            _save.Y = 3;
            _open.X = _new.X + _new.Width + 3;
            _save.X = _open.X + _open.Width + 3;
            _editor.X = 0;
            _editor.Y = _save.Y + _save.Height + 3;
            _editor.Width = Width;
            _editor.Height = Height - _editor.Y;

            _new.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_new.LeftButtonPressed)
                _new.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_new.ContainsMouse)
                _new.Tint = Theme.GetAccentColor();

            _save.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_save.LeftButtonPressed)
                _save.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_save.ContainsMouse)
                _save.Tint = Theme.GetAccentColor();

            _open.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (_open.LeftButtonPressed)
                _open.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (_open.ContainsMouse)
                _open.Tint = Theme.GetAccentColor();

        }

        public void View(string path)
        {
            _editor.Text = _fs.ReadAllText(path);
            _path = path;
        }
    }

    public class TextHandler : IFileHandler
    {
        public string Name => "Text Editor";

        public IEnumerable<string> MimeTypes
        {
            get
            {
                yield return "text/plain";
            }
        }

        [Dependency]
        private WindowSystem _winsys = null;

        [Dependency]
        private FSManager _fs = null;

        public void OpenFile(string path)
        {
            var editor = new TextEditorApp(_winsys);
            editor.View(path);
            editor.Show();
        }
    }
}