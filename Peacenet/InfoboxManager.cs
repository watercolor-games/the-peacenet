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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Peacenet
{
    /// <summary>
    /// Provides an engine component that allows the use of classic ShiftOS infoboxes in The Peacenet.
    /// </summary>
    public class InfoboxManager : IEngineComponent, ILoadable
    {
        private SoundEffect _oldSynthLead = null;

        [Dependency]
        private WindowSystem _winmgr = null;

        /// <inheritdoc cref="Infobox.Show(string, string, Action)"/>
        public void Show(string title, string message, Action callback = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.Show(title, message, callback);
            _oldSynthLead.Play();
        }

        /// <inheritdoc cref="Infobox.ShowYesNo(string, string, Action{Boolean})"/>
        public void ShowYesNo(string title, string message, Action<bool> callback = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.ShowYesNo(title, message, callback);
            _oldSynthLead.Play();
        }

        /// <inheritdoc cref="Infobox.PromptText(string, string, Action{string}, Func{string, bool})"/>
        public void PromptText(string title, string message, Action<string> callback = null, Func<string, bool> validator = null)
        {
            var ibox = new Infobox(_winmgr);
            ibox.PromptText(title, message, callback, validator);
            _oldSynthLead.Play();
        }

        /// <inheritdoc/>
        public void Initiate()
        {
        }

        public void Load(ContentManager content)
        {
            _oldSynthLead = content.Load<SoundEffect>("SFX/C_OldSynthLead_01");
        }
    }

    /// <summary>
    /// A dialog box allowing you to ask the player for input or just display information.
    /// </summary>
    public class Infobox : Window
    {
        private Label _messageLabel = null;
        private PictureBox _picture = null;
        private Button _ok = null;
        private Button _yes = null;
        private Button _no = null;
        private TextBox _inputBox = null;

        [Dependency]
        private GameLoop _plexgate = null;

        /// <inheritdoc/>
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

        /// <summary>
        /// Show the infobox.
        /// </summary>
        /// <param name="title">The title of the infobox</param>
        /// <param name="message">The message to display to the player</param>
        /// <param name="callback">A callback to run when the infobox closes</param>
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

        /// <summary>
        /// Show the infobox, asking the player a "yes or no" question.
        /// </summary>
        /// <param name="title">The title of the infobox</param>
        /// <param name="message">The question to ask the player.</param>
        /// <param name="callback">A callback to run when the player chooses their answer.</param>
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

        /// <inheritdoc/>
        public override void Close()
        {
            if (_doneCallbacks == false)
            {
                _okCallback?.Invoke();
                _yesNoCallback?.Invoke(false);
            }
            base.Close();
        }

        /// <summary>
        /// Show the infobox, prompting the player for text input.
        /// </summary>
        /// <param name="title">The title of the infobox</param>
        /// <param name="message">The message to display to the player</param>
        /// <param name="callback">A callback to run when the player has submitted their text and it has been validated</param>
        /// <param name="validator">A validator callback to run when the player submits their text. If the function returns false, the infobox will not close and the final callback won't run.</param>
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

        /// <inheritdoc/>
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