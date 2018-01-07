using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Peacenet.CoreUtils
{
    /// <summary>
    /// A command-line frontend for the game's chat system.
    /// </summary>
    public class irc : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Peacegate implementation of the Internet Relay Chat protocol.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "irc";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        [Dependency]
        private ChatBackend _chat = null;

        private class CmdChatFrontend : IChatFrontend
        {
            private ConsoleContext console = null;

            public CmdChatFrontend(ConsoleContext ctx)
            {
                if (ctx == null)
                    throw new ArgumentNullException(nameof(ctx));
                console = ctx;
            }

            public void ActionReceived(string user, string message)
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Green);
                console.SetItalic(true);
                console.SetBold(false);
                console.WriteLine($"*{user} {message}*");
            }

            public void MessageReceived(string user, string message)
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.White);
                console.SetItalic(true);
                console.SetBold(true);
                console.WriteLine($"<{user}> {message}");

            }

            public void UserJoined(string user)
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Green);
                console.SetItalic(false);
                console.SetBold(true);
                console.WriteLine($"*{user} has joined the chat!*");

            }

            public void UserLeft(string user)
            {
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Orange);
                console.SetItalic(false);
                console.SetBold(true);
                console.WriteLine($"*{user} has left the chat!*");
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.WriteLine("Starting chat... type /exit to exit.");
            var chat = new CmdChatFrontend(console);
            _chat.Login(chat);
            try
            {
                bool running = true;
                while (running)
                {
                    string message = console.ReadLine();
                    if (message == "/exit")
                        running = false;
                    else
                    {
                        if(message.StartsWith("/me "))
                        {
                            string action = message.Remove(0, 4);
                            _chat.SendAction(action);
                        }
                        else
                        {
                            _chat.SendMessage(message);
                        }
                    }
                }
                _chat.Logout();
            }
            catch(Exception ex)
            {
                console.WriteLine("irc: " + ex.Message);
            }
        }
    }
}
