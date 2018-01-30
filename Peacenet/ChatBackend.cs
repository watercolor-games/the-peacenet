using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using System.IO;
using Peacenet.Server;

namespace Peacenet
{
    /// <summary>
    /// Provides support for chat using the Peacenet server's chat system.
    /// </summary>
    public class ChatBackend : IEngineComponent
    {

        [Dependency]
        private AsyncServerManager _server = null;

        private IChatFrontend frontend = null;

        /// <inheritdoc/>
        public void Initiate()
        {
            _server.BroadcastReceived += (type, reader) =>
            {
                if (frontend == null)
                    return;
                switch (type)
                {
                    case Plex.Objects.ServerBroadcastType.Chat_UserJoin:
                        string user = reader.ReadString();
                        frontend.UserJoined(user);
                        break;
                    case Plex.Objects.ServerBroadcastType.Chat_UserLeave:
                        string luser = reader.ReadString();
                        frontend.UserLeft(luser);
                        break;
                    case Plex.Objects.ServerBroadcastType.Chat_Message:
                        string author = reader.ReadString();
                        string message = reader.ReadString();
                        frontend.MessageReceived(author, message);
                        break;
                    case Plex.Objects.ServerBroadcastType.Chat_Action:
                        string subject = reader.ReadString();
                        string action = reader.ReadString();
                        frontend.ActionReceived(subject, action);
                        break;
                }
            };
        }

        /// <summary>
        /// Log out of chat.
        /// </summary>
        public void Logout()
        {
            if(frontend != null)
            {
                _server.SendMessage(Plex.Objects.ServerMessageType.CHAT_LEAVE, null, (res, reader) =>
                {

                }).Wait();
                frontend = null;
            }
        }


        /// <summary>
        /// Log into chat.
        /// </summary>
        /// <param name="fend">A frontend to pipe server chat events to.</param>
        public void Login(IChatFrontend fend)
        {
            if (!_server.IsMultiplayer)
            {
                throw new InvalidOperationException("Cannot connect to server. Connection refused.");
            }
            if (fend == null)
                return;
            if (this.frontend != fend)
                if (frontend != null)
                    Logout();
            frontend = fend;
            _server.SendMessage(Plex.Objects.ServerMessageType.CHAT_JOIN, null, (res, reader) =>
            {

            }).Wait();
        }

        /// <summary>
        /// Send a message to chat.
        /// </summary>
        /// <param name="message">The message text to send.</param>
        public void SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            if (frontend == null)
                throw new InvalidOperationException("You are not logged in to chat.");
            using(var memstr = new MemoryStream())
            {
                using(var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(message);
                    _server.SendMessage(Plex.Objects.ServerMessageType.CHAT_SENDTEXT, memstr.ToArray(), (res, reader) =>
                   {

                   }).Wait();
                }
            }
        }

        /// <summary>
        /// Send an action to chat.
        /// </summary>
        /// <param name="message">The action body to send.</param>
        public void SendAction(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            if (frontend == null)
                throw new InvalidOperationException("You are not logged in to chat.");
            using (var memstr = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(message);
                    _server.SendMessage(Plex.Objects.ServerMessageType.CHAT_SENDACTION, memstr.ToArray(), (res, reader) =>
                    {

                    }).Wait();
                }
            }
        }

    }

    /// <summary>
    /// Provides a simple API for handling server-side chat events.
    /// </summary>
    public interface IChatFrontend
    {
        /// <summary>
        /// REport that a user has joined the chat.
        /// </summary>
        /// <param name="user">The username of the user.</param>
        void UserJoined(string user);

        /// <summary>
        /// Report that a user has left the chat.
        /// </summary>
        /// <param name="user">The username of the user.</param>
        void UserLeft(string user);

        /// <summary>
        /// Report that a message has been received.
        /// </summary>
        /// <param name="user">The author's username.</param>
        /// <param name="message">The message text.</param>
        void MessageReceived(string user, string message);

        /// <summary>
        /// Report that an action has been received.
        /// </summary>
        /// <param name="user">The author's username.</param>
        /// <param name="message">The action body.</param>
        void ActionReceived(string user, string message);
    }


}
