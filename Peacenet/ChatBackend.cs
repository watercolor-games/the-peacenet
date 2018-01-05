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
using Plex.Engine.Server;
using System.IO;

namespace Peacenet
{
    public class ChatBackend : IEngineComponent
    {

        [Dependency]
        private AsyncServerManager _server = null;

        private IChatFrontend frontend = null;

        public int DrawIndex
        {
            get
            {
                return -1;
            }
        }

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

    public interface IChatFrontend
    {
        void UserJoined(string user);
        void UserLeft(string user);
        void MessageReceived(string user, string message);
        void ActionReceived(string user, string message);
    }


}
