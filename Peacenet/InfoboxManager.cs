using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Peacenet
{
    public class InfoboxManager : IEngineComponent
    {
        [Dependency]
        private WindowSystem _winmgr = null;

        public void Show(string title, string message, Action callback = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.Show(title, message, callback);
        }

        public void ShowYesNo(string title, string message, Action<bool> callback = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.ShowYesNo(title, message, callback);
        }

        public void PromptText(string title, string message, Action<string> callback = null, Func<string, bool> validator = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.PromptText(title, message, callback, validator);
        }

        public void Initiate()
        {
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }

    public class Infobox : Window
    {
        private Label _messageLabel = null;
        private PictureBox _picture = null;
        private Button _ok = null;
        private Button _yes = null;
        private Button _no = null;
        private TextBox _inputBox = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public Infobox(WindowSystem _winsys) : base(_winsys)
        {
            _messageLabel = new Label();
            _picture = new PictureBox();
            _yes = new Button();
            _no = new Button();
            _ok = new Button();
            AddChild(_messageLabel);
            AddChild(_picture);
            AddChild(_yes);
            AddChild(_no);
            AddChild(_ok);
            _ok.Text = "OK";
            _yes.Text = "Yes";
            _no.Text = "No";
            _messageLabel.AutoSize = true;
            _picture.Width = 64;
            _picture.Height = 64;
            SetWindowStyle(WindowStyle.DialogNoDrag);
            _picture.Texture = _plexgate.Content.Load<Texture2D>("Infobox/warning");
            _inputBox = new TextBox();
            AddChild(_inputBox);
        }

        private Action _okCallback = null;
        private Action<bool> _yesNoCallback = null;

        public void Show(string title, string message, Action callback = null)
        {
            _ok.Click += (o, a) =>
            {
                _doneCallbacks = true;
                callback?.Invoke();
                Close();
            };
            _okCallback = callback;
            _yes.Visible = false;
            _no.Visible = false;
            _inputBox.Visible = false;
            _ok.Visible = true;
            Title = title;
            _messageLabel.Text = message;
            Show();
        }

        public void ShowYesNo(string title, string message, Action<bool> callback = null)
        {
            _yes.Click += (o, a) =>
            {
                _doneCallbacks = true;
                callback?.Invoke(true);
                Close();
            };
            _no.Click += (o, a) =>
            {
                _doneCallbacks = true;
                callback?.Invoke(false);
                Close();
            };
            _yesNoCallback = callback;
            _yes.Visible = true;
            _no.Visible = true;
            _ok.Visible = false;
            _inputBox.Visible = false;
            Title = title;
            _messageLabel.Text = message;
            Show();
        }

        private bool _doneCallbacks = false;

        public override void Close()
        {
            if (_doneCallbacks == false)
            {
                _okCallback?.Invoke();
                _yesNoCallback?.Invoke(false);
            }
            base.Close();
        }

        public void PromptText(string title, string message, Action<string> callback = null, Func<string, bool> validator = null)
        {
            if (validator == null)
                validator = (value) => { return true; };
            _ok.Click += (o, a) =>
            {
                if (validator.Invoke(_inputBox.Text) == true)
                {
                    callback?.Invoke(_inputBox.Text);
                    Close();
                }
            };
            _yes.Visible = false;
            _no.Visible = false;
            _inputBox.Visible = true;
            _ok.Visible = true;
            Title = title;
            _messageLabel.Text = message;
            Show();

        }

        protected override void OnUpdate(GameTime time)
        {
            int contentHeight = Math.Max(_picture.Height, _messageLabel.Height);

            _picture.X = 15;
            _picture.Y = 15;

            _messageLabel.Y = _picture.Y;
            _messageLabel.X = _picture.X + _picture.Width + 15;

            int width = _messageLabel.X + _messageLabel.Width + 15;

            Width = width;
            if (_inputBox.Visible)
            {
                _inputBox.Y = _messageLabel.Y + _messageLabel.Height + 10;
                _inputBox.Width = _messageLabel.Width;
                _inputBox.X = _messageLabel.X;
                contentHeight = Math.Max(_picture.Height, _messageLabel.Height + 15 + _inputBox.Height);
            }

            Height = 15 + contentHeight + 15 + _ok.Height + 15;

            _ok.Y = this.Height - Math.Max(_ok.Height, _yes.Height) - 10;
            _ok.X = (Width - _ok.Width) / 2;

            int yesNoWidth = _yes.Width + _no.Width + 5;
            int yesX = (Width - yesNoWidth) / 2;

            _yes.X = yesX;
            _yes.Y = _ok.Y;
            _no.X = _yes.X + _yes.Width + 5;
            _no.Y = _yes.Y;

            Parent.X = (Manager.ScreenWidth - Width) / 2;
            Parent.Y = (Manager.ScreenHeight - Height) / 2;

        }
    }
}
