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
using Discord.WebSocket;

namespace ShiftOS.Server
{
    public static class ChatBackend
    {
        public static async Task StartDiscordBots()
        {
            Reinitialized?.Invoke();
            if (!File.Exists("chats.json"))
                File.WriteAllText("chats.json", "[]");
            foreach (var chat in JsonConvert.DeserializeObject<List<ShiftOS.Objects.Channel>>(File.ReadAllText("chats.json")))
            {

                bool chatKilled = false;
                if (chat.IsDiscordProxy == true)
                {
                    DiscordSocketConfig builder = new DiscordSocketConfig();
                    builder.AudioMode = Discord.Audio.AudioMode.Disabled;
                    var client = new DiscordSocketClient(builder);
                    await client.LoginAsync(TokenType.Bot, chat.DiscordBotToken);

                    await client.ConnectAsync();
                    await client.SetGameAsync("ShiftOS");
                    await client.SetStatusAsync(UserStatus.Online);
                    client.MessageReceived += async (s) =>
                    {
                        if (chatKilled == false)
                        {
                            if (s.Channel.Id == Convert.ToUInt64(chat.DiscordChannelID))
                            {
                                server.DispatchAll(new NetObject("chat_msgreceived", new ServerMessage
                                {
                                    Name = "chat_msgreceived",
                                    GUID = "server",
                                    Contents = JsonConvert.SerializeObject(new ChatMessage(s.Author.Mention, "discord_" + s.Channel.Name, s.Content, chat.ID))
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
                                var dChan = client.GetChannel(Convert.ToUInt64(chat.ID)) as ISocketMessageChannel;
                                //Relay the message to Discord.
                                dChan.SendMessageAsync($"**[{msg.Username}@{msg.SystemName}] `<mud/{msg.Channel}> {msg.Message}");

                            }
                            //Relay it back to all MUD clients.
                            RelayMessage(g, msg);
                        }
                    };
                    Reinitialized += () =>
                    {
                        client.DisconnectAsync();
                        
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

        [MudRequest("chat_getallchannels")]
        public static void GetAllChannels(string guid, object contents)
        {
            server.DispatchTo(new Guid(guid), new NetObject("chat_all", new ServerMessage
            {
                Name = "chat_all",
                GUID = "Server",
                Contents = (File.Exists("chats.json") == true) ? File.ReadAllText("chats.json") : "[]"
            }));
        }

        [MudRequest("chat_send")]
        public static void ReceiveMessage(string guid, object contents)
        {
            var msg = JsonConvert.DeserializeObject<ChatMessage>(JsonConvert.SerializeObject(contents));
            MessageReceived?.Invoke(guid, msg);

        }
    }
}
