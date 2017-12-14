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
    public class WGLogin : Window
    {
        private Button _cancel = null;
        private Button _login = null;
        private Button _createAccount = null;
        private Label _wgLabel = null;
        private Label _usernameLabel = null;
        private Label _passwordLabel = null;
        private Label _whyLabel = null;
        private TextBox _username = null;
        private TextBox _password = null;

        [Dependency]
        private WatercolorAPIManager _api = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        public WGLogin(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Watercolor Games - Login";
            SetWindowStyle(WindowStyle.NoBorder);

            _wgLabel = new Label();
            _usernameLabel = new Label();
            _passwordLabel = new Label();
            _whyLabel = new Label();
            _login = new Button();
            _cancel = new Button();
            _createAccount = new Button();
            _username = new TextBox();
            _password = new TextBox();

            AddChild(_wgLabel);
            AddChild(_usernameLabel);
            AddChild(_username);
            AddChild(_passwordLabel);
            AddChild(_password);
            AddChild(_whyLabel);
            AddChild(_cancel);
            AddChild(_login);
            AddChild(_createAccount);

            _cancel.Click += (o, a) => { Close(); };
            _login.Click += (o, a) =>
            {
                _api.Login(_username.Text, _password.Text, Close, (error) =>
                {
                    _infobox.Show("Login error", error);
                });
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            int baseWidth = 500;
            int margin = 15;
            _wgLabel.Text = "Watercolor Account Login";
            _usernameLabel.Text = "Email address:";
            _wgLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _usernameLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _passwordLabel.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _passwordLabel.Text = "Password:";
            _username.Width = baseWidth - (margin * 2);
            _password.Width = baseWidth - (margin * 2);
            _passwordLabel.AutoSize = true;
            _wgLabel.AutoSize = true;
            _usernameLabel.AutoSize = true;
            _whyLabel.AutoSize = true;
            _whyLabel.MaxWidth = (baseWidth) - (margin * 2);
            _whyLabel.Text = @"What is a Watercolor account?

A Watercolor account is a way of identifying yourself in The Peacenet and any of our other games while playing online multiplayer or interacting with the community.

Any multiplayer servers you join will use your Watercolor account to know who you are and know what upgrades, stats, etc you have. Signing up is completely free and optional, but if you're logged in...Campaign characters can identify you by your name, so that's cool.";
            _login.Text = "Log in";
            _cancel.Text = "Cancel";
            _createAccount.Text = "Create an account!";

            Width = baseWidth;
            _wgLabel.X = margin;
            _wgLabel.Y = margin;
            _usernameLabel.X = margin;
            _usernameLabel.Y = _wgLabel.Y + _wgLabel.Height + 10;
            _username.X = margin;
            _username.Y = _usernameLabel.Y + _usernameLabel.Height;
            _passwordLabel.X = margin;
            _passwordLabel.Y = _username.Y + _username.Height + 10;
            _password.X = margin;
            _password.Y = _passwordLabel.Y + _passwordLabel.Height;
            _whyLabel.X = margin;
            _whyLabel.Y = _password.Y + _password.Height + 15;
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
