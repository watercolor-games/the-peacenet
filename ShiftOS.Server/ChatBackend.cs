/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
using Discord.Net.WebSockets;

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
                string chatID = chat.ID;
                bool chatKilled = false;
                if (chat.IsDiscordProxy == true)
                {
                    DiscordSocketConfig builder = new DiscordSocketConfig();
                    builder.AudioMode = Discord.Audio.AudioMode.Disabled;
                    builder.WebSocketProvider = () => Discord.Net.Providers.WS4Net.WS4NetProvider.Instance();
                    var client = new DiscordSocketClient(builder);
                    await client.LoginAsync(TokenType.Bot, chat.DiscordBotToken);

                    await client.ConnectAsync();
                    await client.SetGameAsync("ShiftOS");
                    await client.SetStatusAsync(UserStatus.Online);
                    //Get the Discord channel for this chat.
                    var Chan = client.GetChannel(Convert.ToUInt64(chat.DiscordChannelID)) as ISocketMessageChannel;
                    //Relay the message to Discord.
                    await Chan.SendMessageAsync("**Hello! Multi-user domain is online.**");

                    client.MessageReceived += async (s) =>
                    {
                        if (chatKilled == false)
                        {
                            if (s.Channel.Id == Convert.ToUInt64(chat.DiscordChannelID))
                            {
                                if (s.Author.Id != client.CurrentUser.Id)
                                {
                                    var msg = new ChatMessage(s.Author.Username, "discord_" + s.Channel.Name, (s as SocketUserMessage).Resolve(0), chatID);
                                    server.DispatchAll(new NetObject("chat_msgreceived", new ServerMessage
                                    {
                                        Name = "chat_msgreceived",
                                        GUID = "server",
                                        Contents = JsonConvert.SerializeObject(msg)
                                    }));
                                    Log(chatID, $"[{msg.Username}@{msg.SystemName}] {msg.Message}");
                                }
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
                                var dChan = client.GetChannel(Convert.ToUInt64(chat.DiscordChannelID)) as ISocketMessageChannel;
                                //Relay the message to Discord.
                                dChan.SendMessageAsync($"**[{msg.Username}@{msg.SystemName}]** `<mud/{msg.Channel}>` {msg.Message}");
                                //Relay it back to all MUD clients.
                                RelayMessage(g, msg);
                                Log(chatID, $"[{msg.Username}@{msg.SystemName}] {msg.Message}");

                            }
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
                            //...Then log it.
                            Log(chatID, $"[{msg.Username}@{msg.SystemName}] {msg.Message}");
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

        [MudRequest("chat_getallchannels", null)]
        public static void GetAllChannels(string guid, object contents)
        {
            server.DispatchTo(new Guid(guid), new NetObject("chat_all", new ServerMessage
            {
                Name = "chat_all",
                GUID = "Server",
                Contents = (File.Exists("chats.json") == true) ? File.ReadAllText("chats.json") : "[]"
            }));
        }

        [MudRequest("chat_send", typeof(Dictionary<string, string>))]
        public static void ReceiveMessage(string guid, object contents)
        {
            var msg = contents as Dictionary<string, string>;
            MessageReceived?.Invoke(guid, new ChatMessage(msg["Username"], msg["SystemName"], msg["Message"], msg["Channel"]));

        }

        [MudRequest("chat_getlog", typeof(ChatLogRequest))]
        public static void GetChatlog(string guid, ChatLogRequest req)
        {
            if (!Directory.Exists("chatlogs"))
                Directory.CreateDirectory("chatlogs");

            if(File.Exists("chatlogs/" + req.Channel + ".log"))
            {
                string[] log = File.ReadAllLines("chatlogs/" + req.Channel + ".log");
                string seg = "";

                if(req.Backtrack == 0 || log.Length < req.Backtrack)
                {
                    //send all of it.
                    foreach(var ln in log)
                    {
                        seg += ln + Environment.NewLine;
                    }
                }
                else
                {
                    //send only a specific chunk.
                    for(int i = log.Length - 1; i >= log.Length - req.Backtrack; i--)
                    {
                        seg += log[i] + Environment.NewLine;
                    }
                }

                try
                {
                    server.DispatchTo(new Guid(guid), new NetObject("always watching, always listening, my eyes are everywhere, you cannot escape me", new ServerMessage
                    {
                        Name = "chatlog",
                        Contents = seg,
                        GUID = "server"
                    }));
                }
                catch { }
            }
        }


        public static void Log(string channel, string line)
        {
            if (!Directory.Exists("chatlogs"))
                Directory.CreateDirectory("chatlogs");

            List<string> lines = new List<string>();
            if (File.Exists("chatlogs/" + channel + ".log"))
                lines = new List<string>(File.ReadAllLines("chatlogs/" + channel + ".log"));

            lines.Add(line);
            File.WriteAllLines("chatlogs/" + channel + ".log", lines.ToArray());

        }
    }

}
