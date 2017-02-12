using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using NetSockets;
using Newtonsoft.Json;
using System.IO;
using static ShiftOS.Server.Program;
using Discord;

namespace ShiftOS.Server
{
    public static class ChatBackend
    {
        public static void StartDiscordBots()
        {
            Reinitialized?.Invoke();
            foreach (var chat in JsonConvert.DeserializeObject<List<ShiftOS.Objects.Channel>>(File.ReadAllText("chats.json")))
            {

                bool chatKilled = false;
                if (chat.IsDiscordProxy == true)
                {
                    DiscordConfigBuilder builder = new DiscordConfigBuilder();
                    builder.AppName = "ShiftOS";
                    builder.AppVersion = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                    builder.AppUrl = "http://getshiftos.ml/";
                    var client = new DiscordClient(builder);
                    client.Connect(chat.DiscordBotToken, TokenType.Bot);
                    client.SetGame("ShiftOS");
                    client.SetStatus(UserStatus.Online);
                    client.MessageReceived += (s, e) =>
                    {
                        if (chatKilled == false)
                        {
                            if (e.Channel.Id.ToString() == chat.DiscordChannelID)
                            {
                                server.DispatchAll(new NetObject("chat_msgreceived", new ServerMessage
                                {
                                    Name = "chat_msgreceived",
                                    GUID = "server",
                                    Contents = JsonConvert.SerializeObject(new ChatMessage(e.User.Name, "discord_" + e.Channel.Name, e.Message.Text, chat.ID))
                                }));
                            }
                        }
                    };
                    MessageReceived += (g, msg) =>
                    {
                        if (chatKilled == false)
                        {
                            //Determine if the message was sent to this channel.
                            if (msg.Channel == chat.ID)
                            {
                                //Get the Discord channel for this chat.
                                var dChan = client.GetChannel(Convert.ToUInt64(chat.DiscordChannelID));
                                //Relay the message to Discord.
                                dChan.SendMessage($"**[{msg.Username}@{msg.SystemName} ({msg.Channel})]: {msg.Message}");

                            }
                            //Relay it back to all MUD clients.
                            RelayMessage(g, msg);
                        }
                    };
                    Reinitialized += () =>
                    {
                        client.Disconnect();
                        client.Dispose();
                        chatKilled = true;
                    };
                }
                else
                {
                    MessageReceived += (g, msg) =>
                    {
                        if (chatKilled == false)
                        {
                            //Just relay it.
                            RelayMessage(g, msg);
                        }
                    };
                    Reinitialized += () => { chatKilled = true; };
                }
            }
        }

        internal static void RelayMessage(string guid, ChatMessage msg)
        {
            server.DispatchAllExcept(new Guid(guid), new NetObject("chat_msgreceived", new ServerMessage
            {
                Name = "chat_msgreceived",
                GUID = "server",
                Contents = JsonConvert.SerializeObject(msg)
            }));

        }

        public static event Action<string, ChatMessage> MessageReceived;
        public static event empty Reinitialized;


        public delegate void empty();

        [MudRequest("chat_send")]
        public static void ReceiveMessage(string guid, object contents)
        {
            var msg = JsonConvert.DeserializeObject<ChatMessage>(JsonConvert.SerializeObject(contents));
            MessageReceived?.Invoke(guid, msg);

        }
    }
}
