#pragma warning disable CS4014

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class Chat
    {
        private static List<string> ChatSessions = new List<string>();
        private static List<ChatMessage> _messages = new List<ChatMessage>();

        [ServerCommand("setpermission", "Set the permission of a user.", true)]
        [UsageString("-u <acct> -p <user|mod|admin>")]
        public static void SetUserPermission(Dictionary<string, object> args)
        {
            string acct = args["<acct>"].ToString();
            
            ACLPermission perm = ACLPermission.User;
            switch(args["<user | mod | admin>"].ToString())
            {
                case "mod":
                    perm = ACLPermission.Moderator;
                    break;
                case "admin":
                    perm = ACLPermission.Admin;
                    break;
            }

            var session = SessionManager.GetSessions().FirstOrDefault(x => x.Username + "@" + x.SaveID == acct);
            if(session == null)
            {
                Console.WriteLine("No user found.");
                return;
            }

            session.Permission = perm;

            Console.WriteLine("Permissions updated.");

            SessionManager.SetSessionInfo(session.SessionID, session);
        }



        private static async Task TalkToDiscord(string messagebody)
        {
            if (string.IsNullOrWhiteSpace(messagebody))
                return;
            string payload = Program.ServerConfig?.DiscordPayloadURL;
            if (string.IsNullOrWhiteSpace(payload))
                return;
            var wr = System.Net.WebRequest.Create(payload);
            wr.Method = "POST";
            wr.ContentType = "application/json";
            string json = JsonConvert.SerializeObject(new
            {
                username = Program.ServerConfig.ServerName,
                content = messagebody
            });
            wr.ContentLength = json.Length;
            using (var reqstr = await wr.GetRequestStreamAsync())
            {
                using(var writer = new StreamWriter(reqstr))
                {
                    writer.Write(json);
                }
            }
            var res = wr.GetResponse();
            res.Close();
            
        }

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
            var sessiondata = SessionManager.GrabAccount(session);
            TalkToDiscord($"**{sessiondata.Username}**@**{sessiondata.SaveID}** has left the chat. ");

            using (var bstr = new BroadcastStream(BroadcastType.CHAT_USERLEFT))
            {
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
            var sessiondata = SessionManager.GrabAccount(session);
            TalkToDiscord($"**{sessiondata.Username}**@**{sessiondata.SaveID}** has joined the chat. ");
            using (var bstr = new BroadcastStream(BroadcastType.CHAT_USERJOINED))
            {

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
            var acct = SessionManager.GrabAccount(session);

            if (chattext.StartsWith("/"))
            {
                string cmdstr = chattext.Remove(0, 1);
                string[] split = cmdstr.Split(' ');
                string cmd = split[0];
                var cmddata = chatCommands.FirstOrDefault(x => x.Name == cmd);
                if(cmddata == null)
                {
                    writer.Write("Command not found - type '/help' for a list of commands.");
                    return (byte)ServerResponseType.REQ_ERROR;
                }

                if (cmddata.SessionRequired)
                {
                    if (!ChatSessions.Contains(session))
                    {
                        writer.Write("You can't run this command - you are not in a chat.");
                        return (byte)ServerResponseType.REQ_ERROR;
                    }
                }

                int minAclLevel = (int)cmddata.MinimumPermission;
                int currentAclLevel = (int)acct.Permission;
                if(currentAclLevel < minAclLevel)
                {
                    writer.Write("You do not have permission to run this command.");
                    return (byte)ServerResponseType.REQ_ERROR;
                }

                string[] args = new string[] { };
                if(cmdstr.Length > cmd.Length)
                {
                    args = cmdstr.Remove(0, cmd.Length + 1).Split(' ');
                }
                try
                {
                    string result = cmddata.Method.Invoke(null, new[] { args }).ToString();
                    writer.Write(result);
                }
                catch(Exception ex)
                {
                    writer.Write("Command exception:\r\n" + ex.Message);
                }
                return (byte)ServerResponseType.REQ_ERROR; //If we return an error result, the client will look for an error message - or our command's output.
            }

            if (ChatSessions.Contains(session))
            {
                TalkToDiscord($"[**{acct.Username}**@**{acct.SaveID}**] {chattext}");

                BroadcastMessage(new ChatMessage
                {
                    Author = acct.Username + "@" + acct.SaveID,
                    ID = Guid.NewGuid().ToString(),
                    Text = chattext
                });

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
                TalkToDiscord($"_[**{acct.Username}**@**{acct.SaveID}** {chattext}]_");

                BroadcastAction(new ChatMessage
                {
                    Author = acct.Username + "@" + acct.SaveID,
                    ID = Guid.NewGuid().ToString(),
                    Text = chattext
                });

                return 0x00;
            }
            writer.Write("You are not currently in a chat! You can't send messages.");
            return (byte)ServerResponseType.REQ_ERROR;
        }

        [ChatCommand("kick", "Kick a user from the current chat.", ACLPermission.Moderator)]
        [SessionRequired]
        public static string Kick(string[] args)
        {
            if(args.Length >= 1)
            {
                string user = args[0];
                string usrstr = user;
                if (user.Contains("@"))
                {
                    usrstr = user.Substring(0, user.IndexOf("@"));
                }
                string sessionToRemove = null;
                foreach(var session in ChatSessions)
                {
                    var sacct = SessionManager.GrabAccount(session);
                    if(sacct.Username == usrstr)
                    {
                        sessionToRemove = session;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(sessionToRemove))
                    return "User not in chat.";
                ChatSessions.Remove(sessionToRemove);
                var acct = SessionManager.GrabAccount(sessionToRemove);

                TalkToDiscord($"**{acct.Username}**@**{acct.SaveID}** has been kicked.");

                var chmsg = new ChatMessage
                {
                    Author = "peacenet",
                    ID = Guid.NewGuid().ToString(),
                    Text = $"{acct.Username}@{acct.SaveID} has been kicked."
                };

                BroadcastMessage(chmsg);

            }
            return "User not provided.";

        }

        public static void BroadcastAction(ChatMessage chmsg)
        {
            _messages.Add(chmsg);
            using (var broadcast = new BroadcastStream(Objects.BroadcastType.CHAT_ACTIONSENT))
            {
                broadcast.Write(chmsg.ID);
                broadcast.Write(chmsg.Author);
                broadcast.Write(chmsg.Text);
                broadcast.Send();
            }
        }


        public static void BroadcastMessage(ChatMessage chmsg)
        {
            _messages.Add(chmsg);
            using (var broadcast = new BroadcastStream(Objects.BroadcastType.CHAT_MESSAGESENT))
            {
                broadcast.Write(chmsg.ID);
                broadcast.Write(chmsg.Author);
                broadcast.Write(chmsg.Text);
                broadcast.Send();
            }
        }

        [ChatCommand("help", "Shows a list of available chat commands.")]
        public static string ChatHelp(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Chat commands");
            sb.AppendLine("====================");
            sb.AppendLine("");
            foreach(var cmd in chatCommands.OrderBy(x=>x.Name))
            {
                sb.AppendLine($" - /{cmd.Name}: {cmd.Description}");
            }
            return sb.ToString();
        }

        private static List<ChatCommandAttribute> chatCommands = null;

        static Chat()
        {
            if(chatCommands == null)
            {
                chatCommands = new List<ChatCommandAttribute>();
                foreach(var type in ReflectMan.Types)
                {
                    foreach(var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        var attr = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ChatCommandAttribute) as ChatCommandAttribute;
                        if(attr != null)
                        {
                            if(chatCommands.FirstOrDefault(x=>x.Name == attr.Name) == null)
                            {
                                attr.Method = mth;
                                attr.SetSessionRequired(mth.GetCustomAttributes(false).FirstOrDefault(x => x is SessionRequired) != null);
                                chatCommands.Add(attr);
                            }
                            else
                            {
                                throw new InvalidOperationException("Can't have multiple chat commands with the same name...");
                            }
                        }
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChatCommandAttribute : Attribute
    {
        public ChatCommandAttribute(string name, string desc, ACLPermission minPerm = ACLPermission.User)
        {
            Name = name;
            Description = desc;
            MinimumPermission = minPerm;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ACLPermission MinimumPermission { get; private set; }

        public bool SessionRequired { get; private set; }

        public void SetSessionRequired(bool value)
        {
            SessionRequired = value;
        }

        public MethodInfo Method { get; internal set; }
    }

    public class ChatMessage
    {
        public string ID { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }
}
