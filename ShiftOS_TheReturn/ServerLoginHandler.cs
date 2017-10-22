using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;
using Plex.Objects;

namespace Plex.Engine
{
    public static class ServerLoginHandler
    {
        private static LoginScreen _loginScreen = null;
        internal static int triesLeft = 4;

        public static void AccessDenied()
        {
            if (triesLeft > 0)
            {
                Engine.Infobox.Show("Login failed.", "The username or password you have entered is incorrect. (You have " + triesLeft + " attempt(s) left.)");
                triesLeft--;
            }
            else
            {
                ServerManager.Disconnect(DisconnectType.Error, "You have ran out of login attempts and have been kicked off of the server.");
                foreach (var screen in AppearanceManager.OpenForms.Where(x => x.ParentWindow is LoginScreen).ToArray())
                {
                    AppearanceManager.Close(screen.ParentWindow);
                }
                _loginScreen = null;
            }
        }

        [ClientMessageHandler("malformed_data")]
        public static void MalformedDataHandler(string content, string ip)
        {
            foreach (var screen in AppearanceManager.OpenForms.Where(x => x.ParentWindow is LoginScreen).ToArray())
            {
                AppearanceManager.Close(screen.ParentWindow);
            }
            _loginScreen = null;
            ServerManager.Disconnect(DisconnectType.Error, "The client has sent an incorrect or malformed request to the server and has been kicked out for security purposes.");
        }

        public static void LoginRequired()
        {
            triesLeft = 4;
            foreach(var screen in AppearanceManager.OpenForms.Where(x=>x.ParentWindow is LoginScreen).ToArray())
            {
                AppearanceManager.Close(screen.ParentWindow);
            }
            UIManager.ClearTopLevels();
            _loginScreen = new LoginScreen();
            AppearanceManager.SetupDialog(_loginScreen);
            _loginScreen.CredentialsEntered += (username, password) =>
            {
                BinaryReader _r = null;
                string combined = $"{username}\t{password}";
                if (ServerManager.SendMessage(ServerMessageType.USR_LOGIN, (w) =>
                 {
                     w.Write(username);
                     w.Write(password);
                 }, out _r).Message != (byte)ServerResponseType.REQ_SUCCESS)
                {
                    AccessDenied();
                }
                else
                {
                    UIManager.ClearTopLevels();
                    ServerManager.SessionInfo.SessionID = _r.ReadString();
                    SaveSystem.Begin();

                }
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
        private BBCodeLabel _description = new BBCodeLabel();

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
            AddControl(_description);
            _description.Text = @"[b]Why do I need to log in?[/b]

You must log in or create an account on this server so we can protect your save file from hackers/cheaters.

After logging in once, you will not have to log in again unless you have been inactive for a week.";

            _passwordField.PasswordChar = true;

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
            _createacct.Click += () =>
            {
                AppearanceManager.SetupDialog(new RegisterScreen());
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
            _title.Font = SkinEngine.LoadedSkin.Header3Font;
            _uname.X = 5;
            _uname.Y = _title.Y + _title.Height + 10;

            _usernameField.X = 5;
            _usernameField.Y = _uname.Y + _uname.Height + 5;
            _usernameField.MinWidth = Width - 10;
            _usernameField.MinHeight = _usernameField.Font.Height + 6;
            _usernameField.Width = 1;
            _usernameField.Height = 1;


            _password.X = 5;
            _password.Y = _usernameField.Y + _usernameField.Height + 10;

            _passwordField.X = 5;
            _passwordField.Y = _password.Y + _password.Height + 5;
            _passwordField.MinWidth = Width - 10;
            _passwordField.MinHeight = _passwordField.Font.Height + 6;
            _passwordField.Width = 1;
            _passwordField.Height = 1;

            _createacct.X = 5;
            _createacct.Y = (Height - _createacct.Height) - 5;

            _ok.X = Width - _ok.Width - 5;
            _ok.Y = Height - _ok.Height - 5;
            _cancel.X = _ok.X - _cancel.Width - 5;
            _cancel.Y = _ok.Y;

            _description.X = 5;
            _description.Width = Width - 10;
            _description.Y = _passwordField.Y + _passwordField.Height + 5;
            _description.Height = _ok.Y - _description.Y - 10;

        }

        public void OnUpgrade()
        {
        }
    }

    [DefaultTitle("Create account")]
    public class RegisterScreen : Control, IPlexWindow
    {
        private TextControl _title = null;
        private TextInput _usernameField = null;
        private TextInput _passwordField = null;
        private TextInput _passwordConfirmField = null;
        private TextControl _uname = null;
        private TextControl _password = null;
        private TextControl _passwordConfirm = null;
        private Button _ok = null;
        private Button _cancel = null;

        public RegisterScreen()
        {
            Width = 420;
            Height = 400;

            _title = new TextControl();
            _usernameField = new Frontend.GUI.TextInput();
            _passwordField = new TextInput();
            _passwordConfirmField = new Frontend.GUI.TextInput();
            _uname = new Frontend.GUI.TextControl();
            _password = new Frontend.GUI.TextControl();
            _passwordConfirm = new Frontend.GUI.TextControl();
            _ok = new Frontend.GUI.Button();
            _cancel = new Frontend.GUI.Button();

            _title.Text = "Login";
            _uname.Text = "Username:";
            _password.Text = "Password:";
            _passwordConfirm.Text = "Confirm:";
            _ok.Text = "Register";
            _cancel.Text = "Cancel";

            _title.AutoSize = true;
            _uname.AutoSize = true;
            _password.AutoSize = true;
            _passwordConfirm.AutoSize = true;
            _ok.AutoSize = true;
            _cancel.AutoSize = true;

            AddControl(_title);
            AddControl(_uname);
            AddControl(_usernameField);
            AddControl(_password);
            AddControl(_passwordField);
            AddControl(_passwordConfirm);
            AddControl(_passwordConfirmField);
            AddControl(_ok);
            AddControl(_cancel);

            _passwordField.PasswordChar = true;
            _passwordConfirmField.PasswordChar = true;

            _ok.Click += () =>
            {
                if (string.IsNullOrWhiteSpace(_usernameField.Text))
                {
                    Infobox.Show("Please enter a username.", "Please enter a username you'll use to login in the future.");
                    return;
                }
                if(string.IsNullOrWhiteSpace(_passwordField.Text))
                {
                    Infobox.Show("Weak password", "You can't specify a blank string as your password.");
                    return;
                }
                if(_passwordField.Text.Length < 8)
                {
                    Infobox.Show("Weak password", "Your password must contain at least 8 characters and at most infinity characters.");
                    return;
                }
                if(_passwordField.Text != _passwordConfirmField.Text)
                {
                    Infobox.Show("Passwords don't match.", "You must prove you'll remember your password by typing the EXACT SAME PASSWORD twice. You didn't do that.");
                    return;
                }
                BinaryReader _r = null;
                if (ServerManager.SendMessage(ServerMessageType.USR_REGISTER, (w) =>
                {
                    w.Write(_usernameField.Text);
                    w.Write(_passwordField.Text);
                }, out _r).Message != (byte)ServerResponseType.REQ_SUCCESS)
                {
                    Engine.Infobox.Show("Error", "An error occurred while servicing the request.");
                }
                else
                {
                    UIManager.ClearTopLevels();
                    ServerManager.SessionInfo.SessionID = _r.ReadString();
                    SaveSystem.Begin();

                }
            };
            _cancel.Click += () =>
            {
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
            return true;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.X = 5;
            _title.Y = 5;
            _title.Font = SkinEngine.LoadedSkin.Header3Font;
            _uname.X = 5;
            _uname.Y = _title.Y + _title.Height + 10;

            _usernameField.X = 5;
            _usernameField.Y = _uname.Y + _uname.Height + 5;
            _usernameField.MinWidth = Width - 10;
            _usernameField.MinHeight = _usernameField.Font.Height + 6;
            _usernameField.Width = 1;
            _usernameField.Height = 1;


            _password.X = 5;
            _password.Y = _usernameField.Y + _usernameField.Height + 10;

            _passwordField.X = 5;
            _passwordField.Y = _password.Y + _password.Height + 5;
            _passwordField.MinWidth = Width - 10;
            _passwordField.MinHeight = _passwordField.Font.Height + 6;
            _passwordField.Width = 1;
            _passwordField.Height = 1;

            _ok.X = Width - _ok.Width - 5;
            _ok.Y = Height - _ok.Height - 5;
            _cancel.X = _ok.X - _cancel.Width - 5;
            _cancel.Y = _ok.Y;

            _passwordConfirm.X = 5;
            _passwordConfirm.Y = _passwordField.Y + _passwordField.Height + 10;

            _passwordConfirmField.X = 5;
            _passwordConfirmField.Y = _passwordConfirm.Y + _passwordConfirm.Height + 5;
            _passwordConfirmField.MinWidth = Width - 10;
            _passwordConfirmField.MinHeight = _passwordConfirmField.Font.Height + 6;
            _passwordConfirmField.Width = 1;
            _passwordConfirmField.Height = 1;


        }

        public void OnUpgrade()
        {
        }
    }

}
