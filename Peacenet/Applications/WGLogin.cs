#pragma warning disable CS4014 //why

using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a user-interface for logging into or registering for a Watercolor Games community account. This is a fire-and-forget user interface. Once it is shown to the user, it will handle everything on its own.
    /// </summary>
    public class WGLogin : Window
    {
        private Button _cancel = null;
        private Button _login = null;
        private Button _createAccount = null;
        private Label _wgLabel = null;
        private Label _usernameLabel = null;
        private Label _passwordLabel = null;
        private Label _whyLabel = null;
        private TextBox _email = null;
        private TextBox _password = null;
        private TextBox _confirm = null;
        private TextBox _username = null;
        private Label _confirmLabel = null;
        private Label _emailLabel = null;

        private bool _isRegistering = false;

        [Dependency]
        private WatercolorAPIManager _api = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        public WGLogin(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Watercolor Games - Login";
            SetWindowStyle(WindowStyle.NoBorder);

            _wgLabel = new Label();
            _emailLabel = new Label();
            _passwordLabel = new Label();
            _whyLabel = new Label();
            _login = new Button();
            _cancel = new Button();
            _createAccount = new Button();
            _email = new TextBox();
            _password = new TextBox();
            _usernameLabel = new Label();
            _username = new TextBox();
            _confirmLabel = new Label();
            _confirm = new TextBox();

            _email.Label = "user@example.com";
            _username.Label = "Display name";
            _confirm.Label = "Confirm password";
            _confirm.HasPassword = true;
            _password.Label = "Password here";
            _password.HasPassword = true;

            AddChild(_wgLabel);
            AddChild(_emailLabel);
            AddChild(_email);
            AddChild(_passwordLabel);
            AddChild(_password);
            AddChild(_whyLabel);
            AddChild(_cancel);
            AddChild(_login);
            AddChild(_createAccount);
            AddChild(_usernameLabel);
            AddChild(_confirmLabel);
            AddChild(_confirm);
            AddChild(_username);

            _cancel.Click += (o, a) => {
                if (_isRegistering)
                {
                    _isRegistering = false;
                }
                else
                {
                    Close();
                }
            };
            _login.Click += (o, a) =>
            {
                if (_isRegistering)
                {
                    if(_password.Text != _confirm.Text)
                    {
                        _infobox.Show("Passwords do not match", "Your passwords do not match. Please try again.");
                        return;
                    }
                    _api.Register(_username.Text, _password.Text, _email.Text, () =>
                    {
                        _api.Login(_email.Text, _password.Text, Close, (error) =>
                        {
                            _infobox.Show("Login error", error);
                        });

                    }, (error) =>
                    {
                        _infobox.Show("API error", error);
                    });
                }
                else
                {
                    _api.Login(_email.Text, _password.Text, Close, (error) =>
                    {
                        _infobox.Show("Login error", error);
                    });
                }
            };
            _createAccount.Click += (o, a) =>
            {
                _isRegistering = !_isRegistering;
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            int baseWidth = 500;
            int margin = 15;
            if (_isRegistering)
            {
                _wgLabel.Text = "Create Watercolor Account";
            }
            else
            {
                _wgLabel.Text = "Watercolor Account Login";
            }
            _emailLabel.Text = "Email address:";
            _wgLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _emailLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _passwordLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _usernameLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _confirmLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _usernameLabel.Text = "Display name:";
            _confirmLabel.Text = "Confirm:";

            _createAccount.Visible = !_isRegistering;

            _username.Visible = _isRegistering;
            _usernameLabel.Visible = _isRegistering;
            _confirmLabel.Visible = _isRegistering;
            _confirm.Visible = _isRegistering;

            _usernameLabel.AutoSize = true;
            _confirmLabel.AutoSize = true;

            _passwordLabel.Text = "Password:";
            _email.Width = baseWidth - (margin * 2);
            _username.Width = _email.Width;
            _confirm.Width = _email.Width;
            _password.Width = baseWidth - (margin * 2);
            _passwordLabel.AutoSize = true;
            _wgLabel.AutoSize = true;
            _emailLabel.AutoSize = true;
            _whyLabel.AutoSize = true;
            _whyLabel.MaxWidth = (baseWidth) - (margin * 2);
            if (_isRegistering)
            {
                _whyLabel.Text = @"By signing up for a Watercolor account you agree to the community and you agree that you are at least 13 years of age.";
            }
            else
            {
                _whyLabel.Text = @"What is a Watercolor account?

A Watercolor account is a way of identifying yourself in The Peacenet and any of our other games while playing online multiplayer or interacting with the community.

Any multiplayer servers you join will use your Watercolor account to know who you are and know what upgrades, stats, etc you have. Signing up is completely free and optional, but if you're logged in...Campaign characters can identify you by your name, so that's cool.";
            }
            _login.Text = (_isRegistering) ? "Submit" :"Log in";
            _cancel.Text = (_isRegistering) ? "Back" : "Cancel";
            _createAccount.Text = "Create an account!";

            Width = baseWidth;
            _wgLabel.X = margin;
            _wgLabel.Y = margin;
            _usernameLabel.X = margin;
            _usernameLabel.Y = _wgLabel.Y + _wgLabel.Height + 10;
            _username.X = margin;
            _username.Y = _usernameLabel.Y + _usernameLabel.Height;
            _emailLabel.X = margin;
            if (_isRegistering)
            {
                _emailLabel.Y = _username.Y + _username.Height + 10;
            }
            else
            {
                _emailLabel.Y = _wgLabel.Y + _wgLabel.Height + 10;
            }
            _email.X = margin;
            _email.Y = _emailLabel.Y + _emailLabel.Height;
            _passwordLabel.X = margin;
            _passwordLabel.Y = _email.Y + _email.Height + 10;
            _password.X = margin;
            _password.Y = _passwordLabel.Y + _passwordLabel.Height;
            _confirmLabel.X = margin;
            _confirmLabel.Y = _password.Y + _password.Height + 10;
            _confirm.X = margin;
            _confirm.Y = _confirmLabel.Y + _confirmLabel.Height;
            _whyLabel.X = margin;
            if (_isRegistering)
            {
                _whyLabel.Y = _confirm.Y + _confirm.Height + 15;

            }
            else
            {
                _whyLabel.Y = _password.Y + _password.Height + 15;
            }
            _cancel.Y = _whyLabel.Y + _whyLabel.Height + 15;
            Height = _cancel.Y + _cancel.Height + margin;
            _cancel.X = (Width - margin) - _cancel.Width;
            _createAccount.Y = _cancel.Y;
            _createAccount.X = margin;
            _login.X = (_cancel.X - 10) - _login.Width;
            _login.Y = _cancel.Y;

            Parent.X = (Manager.ScreenWidth - Width) / 2;
            Parent.Y = (Manager.ScreenHeight - Height) / 2;

            base.OnUpdate(time);
        }
    }
}
