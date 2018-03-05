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
    public class ChatManager : IEngineComponent
    {
        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public event Action<string, string, string> MessageReceived;

        

        public void Initiate()
        {
            _server.BroadcastReceived += (msg, reader) =>
            {
                if (msg != Plex.Objects.ServerBroadcastType.Chat_MessageReceived)
                    return;
                string author = reader.ReadString();
                string recipient = reader.ReadString();
                string message = reader.ReadString();
                _plexgate.Invoke(() =>
                {
                    MessageReceived?.Invoke(author, recipient, message);
                });
            };
        }

        public void SendMessage(string text, string recipient)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    writer.Write(recipient);
                    writer.Write(text);
                    writer.Flush();
                    _server.SendMessage(Plex.Objects.ServerMessageType.CHAT_SENDMESSAGE, ms.ToArray(), (res, reader) =>
                    {

                    }).Wait();
                }
            }
        }
    }
}