using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Frontend.GUI;

namespace Plex.Engine
{
    public static class ServerLoginHandler
    {
        private static LoginScreen _loginScreen = null;

        [ClientMessageHandler("login_required")]
        public static void LoginRequired(string content, string ip)
        {
            foreach(var screen in AppearanceManager.OpenForms.Where(x=>x.ParentWindow is LoginScreen).ToArray())
            {
                AppearanceManager.Close(screen.ParentWindow);
            }
            _loginScreen = new LoginScreen();
            AppearanceManager.SetupDialog(_loginScreen);
            _loginScreen.CredentialsEntered += (username, password) =>
            {
                string combined = $"{username}\t{password}";
                ServerManager.SendMessage("acct_get_key", combined);
            };
            _loginScreen.Disconnected += () =>
            {
                ServerManager.Disconnect(DisconnectType.UserRequested);
            };
        }
    }

    public class LoginScreen : Control, IPlexWindow
    {
        private TextControl _title = null;
        private TextInput _usernameField = null;
        private TextInput _passwordField = null;
        private TextControl _uname = null;
        private TextControl _password = null;
        private Button _ok = null;
        private Button _cancel = null;
        private Button _createacct = null;

        public bool DisconnectOnClose = true;

        

        public event Action<string, string> CredentialsEntered;
        public event Action Disconnected;

        public LoginScreen()
        {
            Width = 420;
            Height = 300;

            _title = new TextControl();
            _usernameField = new Frontend.GUI.TextInput();
            _passwordField = new TextInput();
            _uname = new Frontend.GUI.TextControl();
            _password = new Frontend.GUI.TextControl();
            _ok = new Frontend.GUI.Button();
            _cancel = new Frontend.GUI.Button();
            _createacct = new Frontend.GUI.Button();

            _title.Text = "Login";
            _uname.Text = "Username:";
            _password.Text = "Password:";
            _ok.Text = "Login";
            _cancel.Text = "Disconnect";
            _createacct.Text = "Create account";

            _title.AutoSize = true;
            _uname.AutoSize = true;
            _password.AutoSize = true;
            _ok.AutoSize = true;
            _cancel.AutoSize = true;
            _createacct.AutoSize = true;

            AddControl(_title);
            AddControl(_uname);
            AddControl(_usernameField);
            AddControl(_password);
            AddControl(_passwordField);
            AddControl(_ok);
            AddControl(_cancel);
            AddControl(_createacct);

            _ok.Click += () =>
            {
                DisconnectOnClose = false;
                CredentialsEntered?.Invoke(_usernameField.Text, _passwordField.Text);
            };
            _cancel.Click += () =>
            {
                DisconnectOnClose = true;
                AppearanceManager.Close(this);
            };
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            if (DisconnectOnClose)
                Disconnected?.Invoke();
            return true;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.X = 5;
            _title.Y = 5;

            _uname.X = 5;
            _uname.Y = _title.Y + _title.Height + 10;

            _usernameField.X = 5;
            _usernameField.Y = _uname.Y + _uname.Height + 5;
            _usernameField.MinWidth = Width - 10;
            _usernameField.MinHeight = _usernameField.Font.Height;
            _usernameField.AutoSize = true;

            _password.X = 5;
            _password.Y = _usernameField.Y + _usernameField.Height + 10;

            _passwordField.X = 5;
            _passwordField.Y = _password.Y + _password.Height + 5;
            _passwordField.MinWidth = Width - 10;
            _passwordField.MinHeight = _passwordField.Font.Height;
            _passwordField.AutoSize = true;

            _createacct.X = 5;
            _createacct.Y = (Height - _createacct.Height) - 5;

            _ok.X = Width - _ok.Width - 5;
            _ok.Y = Height - _ok.Height - 5;
            _cancel.X = _ok.X - _cancel.Width - 5;
            _cancel.Y = _ok.Y; 

        }

        public void OnUpgrade()
        {
        }
    }
}
