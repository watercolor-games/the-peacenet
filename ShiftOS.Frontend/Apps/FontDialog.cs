using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Engine.GUI;
using System.Drawing.Text;
using System.Drawing;

namespace Plex.Frontend.Apps
{
    public class FontDialog : Control, IPlexWindow
    {
        private System.Drawing.Font _font = null;
        private string _title = "";
        private Action<System.Drawing.Font> _callback = null;

        private TextControl _preview = new TextControl();
        private TextInput _families = new TextInput();
        private TextInput _size = new TextInput();

        private Button _ok = new Button();
        private Button _cancel = new Button();

        public FontDialog(string title, System.Drawing.Font font, Action<System.Drawing.Font> callback = null)
        {
            Width = 420;
            Height = 300;

            _font = font;
            _title = title;
            _callback = callback;

            AddControl(_families);
            AddControl(_size);
            AddControl(_preview);
            AddControl(_ok);
            AddControl(_cancel);

            _ok.Click += () =>
            {
                _callback?.Invoke(_font);
                AppearanceManager.Close(this);
            };
            _cancel.Click += () =>
            {
                _callback?.Invoke(font);
                AppearanceManager.Close(this);
            };

            _ok.Text = "OK";
            _cancel.Text = "Cancel";

            _ok.AutoSize = true;
            _cancel.AutoSize = true;

            _preview.AutoSize = true;
            _preview.Text = "The quick brown fox jumped over the lazy dog.";

            _families.TextChanged += () =>
            {
                if (_font.Name == _families.Text)
                    return;
                try
                {
                    UpdateFont();
                }
                catch
                {
                    //_size.Text = ((int)_font.Size).ToString();
                }
            };
            _families.AutoSize = true;

            _size.TextFilter = TextFilter.Integer;
            _size.TextChanged += () =>
            {
                try
                {
                    UpdateFont();
                }
                catch
                {
                    _size.Text = ((int)_font.Size).ToString();
                }
            };
            _size.AutoSize = true;
            _size.MinWidth = 150;
            _size.MinHeight = 6 + _size.Font.Height;
        }

        public void UpdateFont()
        {
            _font = new System.Drawing.Font(_families.Text, _size.Value);
            _preview.Font = _font;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _size.Text = ((int)_font.Size).ToString();

            _families.X = 15;
            _families.Y = 15;
            _families.MinWidth = 175;
            _families.MinHeight = _families.Font.Height + 4;
            _size.X = _families.X + _families.Width + 10;
            _size.Y = 15;

            _ok.X = Width - _ok.Width - 15;
            _ok.Y = Height - _ok.Height - 15;
            _cancel.X = _ok.X - _cancel.Width - 10;
            _cancel.Y = _ok.Y;

            int _bottom = _cancel.Y - 15;
            int _top = _size.Y + _size.Height + 15;
            _preview.MaxWidth = Width - 30;
            _preview.MaxHeight = _bottom - _top;
            _preview.X = 15;
            _preview.Y = _top;

        }

        public void OnLoad()
        {
            AppearanceManager.SetWindowTitle(this, (string.IsNullOrWhiteSpace(_title)) ? "Select Font" : $"Select font for {_title}");
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
