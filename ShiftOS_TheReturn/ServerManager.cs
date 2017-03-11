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
using System.Windows.Forms;
using System.Threading;
using ShiftOS;
using static ShiftOS.Engine.SaveSystem;
using Newtonsoft.Json;

namespace ShiftOS.Engine
{
    public static class ServerManager
    {
        public static void PrintDiagnostics()
        {
            Console.WriteLine($@"{{CLIENT_DIAGNOSTICS}}

{{GUID}}: {thisGuid}
{{CLIENT_DATA}}:

{JsonConvert.SerializeObject(client, Formatting.Indented)}");
        }

        public static Guid thisGuid { get; private set; }
        private static NetObjectClient client { get; set; }

        public static void Disconnect()
        {
            if (client != null)
            {
                client.Disconnect();
            }
            Disconnected?.Invoke();

        }

        public static event EmptyEventHandler Disconnected;
        
        public static void InitiateMUDHack()
        {
            MessageReceived += ServerManager_MessageReceived;
            SendMessage("mudhack_init", "");
        }

        public static event Action<string> ServerPasswordGenerated;
        public static event EmptyEventHandler ServerAccessGranted;
        public static event EmptyEventHandler ServerAccessDenied;
        public static event Action<string> GUIDReceived;
        public static event Action<List<OnlineUser>> UsersReceived;

        private static void ServerManager_MessageReceived(ServerMessage msg)
        {
            switch(msg.Name)
            {
                case "mudhack_users":
                    UsersReceived?.Invoke(JsonConvert.DeserializeObject<List<OnlineUser>>(msg.Contents));
                    break;
                case "mudhack_init":
                    ServerPasswordGenerated?.Invoke(msg.Contents);
                    break;
                case "mudhack_denied":
                    ServerAccessDenied?.Invoke();
                    break;
                case "mudhack_granted":
                    ServerAccessGranted?.Invoke();
                    break;
                case "getguid_fromserver":
                    if(SaveSystem.CurrentSave.Username == msg.Contents)
                    {
                        client.Send(new NetObject("yes_i_am", new ServerMessage
                        {
                            Name = "getguid_reply",
                            GUID = msg.GUID,
                            Contents = thisGuid.ToString(),
                        }));
                    }
                    break;
                case "getguid_reply":
                    GUIDReceived?.Invoke(msg.Contents);
                    break;
            }
        }

        public static void Detach_ServerManager_MessageReceived()
        {
            MessageReceived -= new ServerMessageReceived(ServerManager_MessageReceived);
        }

        public static void Initiate(string mud_address, int port)
        {
            client = new NetObjectClient();

            client.OnReceived += (o, a) =>
            {
                var msg = a.Data.Object as ServerMessage;
                if (msg.Name == "Welcome")
                {
                    thisGuid = new Guid(msg.Contents);
                    GUIDReceived?.Invoke(msg.Contents);
                    TerminalBackend.PrefixEnabled = true;
                    TerminalBackend.PrintPrompt();
                }
                else if(msg.Name == "allusers")
                {
                    foreach(var acc in JsonConvert.DeserializeObject<string[]>(msg.Contents))
                    {
                        Console.WriteLine(acc);
                    }
                    TerminalBackend.PrintPrompt();
                }
                else if(msg.Name == "update_your_cp")
                {
                    var args = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                    if(args["username"] as string == SaveSystem.CurrentSave.Username)
                    {
                        SaveSystem.CurrentSave.Codepoints += (long)args["amount"];
                        Desktop.InvokeOnWorkerThread(new Action(() =>
                        {
                            Infobox.Show($"MUD Control Centre", $"Someone bought an item in your shop, and they have paid {args["amount"]}, and as such, you have been granted these Codepoints.");
                        }));
                        SaveSystem.SaveGame();
                    }
                }
                else if(msg.Name =="broadcast")
                {
                    Console.WriteLine(msg.Contents);
                }
                else if (msg.Name == "Error")
                {
                    var ex = JsonConvert.DeserializeObject<Exception>(msg.Contents);
                    TerminalBackend.PrefixEnabled = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.Red;
                    ConsoleEx.Bold = true;
                    Console.Write($@"{{MUD_ERROR}}: ");
                    ConsoleEx.Bold = false;
                    ConsoleEx.Italic = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(ex.Message);
                    TerminalBackend.PrefixEnabled = true;
                    TerminalBackend.PrintPrompt();
                }
                else
                {
                    MessageReceived?.Invoke(msg);
                }
            };

            client.Connect(mud_address, port);

        }

        public static void SendMessage(string name, string contents)
        {
            var sMsg = new ServerMessage
            {
                Name = name,
                Contents = contents,
                GUID = thisGuid.ToString(),
            };

            client.Send(new NetObject("msg", sMsg));

        }

        private static bool singleplayer = false;
        public static bool IsSingleplayer { get { return singleplayer; } }

        public static void StartLANServer()
        {
            singleplayer = true;
            ShiftOS.Server.Program.ServerStarted += (address) =>
            {
                Console.WriteLine($"Connecting to {address}...");
                Initiate(address, 13370);
            };
            Disconnected += () =>
            {
                ShiftOS.Server.Program.Stop();
            };
            ShiftOS.Server.Program.Main(new[] { "" });


        }


        public static event ServerMessageReceived MessageReceived;

    }

    public delegate void ServerMessageReceived(ServerMessage msg);

    public class MultiplayerOnlyAttribute : Attribute
    {
        /// <summary>
        /// Marks this application as a multiplayer-only application.
        /// </summary>
        public MultiplayerOnlyAttribute()
        {

        }
    }
}
