using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Server
{
    public static class Chat
    {
        private static List<string> ChatSessions = new List<string>();

        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.CHAT_LEAVE)]
        public static byte HandleLeave(string session, BinaryReader reader, BinaryWriter writer)
        {
            if (!ChatSessions.Contains(session))
            {
                writer.Write("You are not in the chat.");
                return (byte)ServerResponseType.REQ_ERROR;
            }
            ChatSessions.Remove(session);
            using (var bstr = new BroadcastStream(BroadcastType.CHAT_USERLEFT))
            {
                var sessiondata = SessionManager.GrabAccount(session);
                bstr.Write($"{sessiondata.Username}@{sessiondata.SaveID}");
                bstr.Send();
            }
            return 0x00;
        }


        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.CHAT_JOIN)]
        public static byte HandleJoin(string session, BinaryReader reader, BinaryWriter writer)
        {
            if (ChatSessions.Contains(session))
            {
                writer.Write("You are already in the chat.");
                return (byte)ServerResponseType.REQ_ERROR;
            }
            ChatSessions.Add(session);
            using(var bstr = new BroadcastStream(BroadcastType.CHAT_USERJOINED))
            {
                var sessiondata = SessionManager.GrabAccount(session);
                bstr.Write($"{sessiondata.Username}@{sessiondata.SaveID}");
                bstr.Send();
            }
            return 0x00;
        }

        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.CHAT_SENDTEXT)]
        public static byte ChatTextSend(string session, BinaryReader reader, BinaryWriter writer)
        {
            string chattext = reader.ReadString();

            if (ChatSessions.Contains(session))
            {
                var acct = SessionManager.GrabAccount(session);
                using(var broadcast = new BroadcastStream(Objects.BroadcastType.CHAT_MESSAGESENT))
                {
                    broadcast.Write(acct.Username + "@" + acct.SaveID);
                    broadcast.Write(chattext);
                    broadcast.Send();
                }
                return 0x00;
            }
            writer.Write("You are not currently in a chat! You can't send messages.");
            return (byte)ServerResponseType.REQ_ERROR;
        }

        [SessionRequired]
        [ServerMessageHandler(ServerMessageType.CHAT_SENDACTION)]
        public static byte ChatActionSend(string session, BinaryReader reader, BinaryWriter writer)
        {
            string chattext = reader.ReadString();

            if (ChatSessions.Contains(session))
            {
                var acct = SessionManager.GrabAccount(session);
                using (var broadcast = new BroadcastStream(Objects.BroadcastType.CHAT_ACTIONSENT))
                {
                    broadcast.Write(acct.Username + "@" + acct.SaveID);
                    broadcast.Write(chattext);
                    broadcast.Send();
                }
                return 0x00;
            }
            writer.Write("You are not currently in a chat! You can't send messages.");
            return (byte)ServerResponseType.REQ_ERROR;
        }

    }
}
