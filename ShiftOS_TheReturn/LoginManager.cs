using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;

namespace ShiftOS.Engine
{
    public static class LoginManager
    {
        private static ILoginFrontend _login = null;

        public static void Init(ILoginFrontend login)
        {
            _login = login;
        }

        public static void PromptForLogin()
        {
            _login.LoginComplete += (user) =>
            {
                LoginComplete?.Invoke(user);
            };
            _login.Login();
        }

        public static bool ShouldUseGUILogin
        {
            get
            {
                if (_login == null)
                    return false;
                return _login.UseGUILogin;
            }
        }

        public static event Action<ClientSave> LoginComplete;
    }

    /// <summary>
    /// Interface for GUI-based logins.
    /// </summary>
    public interface ILoginFrontend
    {
        /// <summary>
        /// When implemented, shows the login UI.
        /// </summary>
        void Login();
        
        /// <summary>
        /// Gets whether the ShiftOS engine should use a GUI-based login system or the default one.
        /// </summary>
        bool UseGUILogin { get; }


        /// <summary>
        /// Occurs when the login is complete.
        /// </summary>
        event Action<ClientSave> LoginComplete;

        

    }
}
