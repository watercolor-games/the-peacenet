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
using System.Net.Sockets;
using System.Diagnostics;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        /// <summary>
        /// Print connection diagnostic information.
        /// </summary>
        public static void PrintDiagnostics()
        {
            Console.WriteLine($@"{{CLIENT_DIAGNOSTICS}}

{{GUID}}: {thisGuid}
Ping: {ServerManager.DigitalSocietyPing} ms
{{CLIENT_DATA}}:

{JsonConvert.SerializeObject(client, Formatting.Indented)}");
        }

        /// <summary>
        /// Gets the unique identifier for this Digital Society connection. This can be used for peer-to-peer communication between two clients.
        /// </summary>
        public static Guid thisGuid { get; private set; }

        /// <summary>
        /// Gets the underlying NetSockets client for this connection.
        /// </summary>
        public static NetObjectClient client { get; private set; }


        private static bool UserDisconnect = false;

        /// <summary>
        /// Gets or sets the server response time for the last request made by this client.
        /// </summary>
        public static long DigitalSocietyPing
        {
            get;
            private set;
        }

        /// <summary>
        /// Disconnect from the digital society intentionally.
        /// </summary>
        public static void Disconnect()
        {
            UserDisconnect = true;
            if (client != null)
            {
                client.Disconnect();
            }
            Disconnected?.Invoke();

        }

        /// <summary>
        /// Occurs when you are disconnected from the Digital Society.
        /// </summary>
        public static event EmptyEventHandler Disconnected;

        /// <summary>
        /// Occurs when the unique ID for this client is sent by the server.
        /// </summary>
        public static event Action<string> GUIDReceived;

        private static void ServerManager_MessageReceived(ServerMessage msg)
        {
            switch(msg.Name)
            {
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

        /// <summary>
        /// Initiate a new Digital Society connection.
        /// </summary>
        /// <param name="mud_address">The IP address or hostname of the target server</param>
        /// <param name="port">The target port.</param>
        public static void Initiate(string mud_address, int port)
        {
            client = new NetObjectClient();
            client.OnDisconnected += (o, a) =>
            {
                if (!UserDisconnect)
                {
                    Desktop.PushNotification("digital_society_connection", "Disconnected from Digital Society.", "The ShiftOS kernel has been disconnected from the Digital Society. We are attempting to re-connect you.");
                    TerminalBackend.PrefixEnabled = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.Red;
                    ConsoleEx.Bold = true;
                    Console.Write($@"Disconnected from MUD: ");
                    ConsoleEx.Bold = false;
                    ConsoleEx.Italic = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("You have been disconnected from the multi-user domain for an unknown reason. Your save data is preserved within the kernel and you will be reconnected shortly.");
                    TerminalBackend.PrefixEnabled = true;
                    TerminalBackend.PrintPrompt();
                    Initiate(mud_address, port);
                }
            };
            client.OnReceived += (o, a) =>
            {
                if (PingTimer.IsRunning)
                {
                    DigitalSocietyPing = PingTimer.ElapsedMilliseconds;
                    PingTimer.Reset();
                }
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
                else if(msg.Name == "forward")
                {
                    MessageReceived?.Invoke(JsonConvert.DeserializeObject<ServerMessage>(msg.Contents));
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

            try
            {
                client.Connect(mud_address, port);
            }
            catch(SocketException ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                Initiate(mud_address, port);
            }
        }

        private static Stopwatch PingTimer = new Stopwatch();

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="name">The message name</param>
        /// <param name="contents">The message body</param>
        public static void SendMessage(string name, string contents)
        {
            var sMsg = new ServerMessage
            {
                Name = name,
                Contents = contents,
                GUID = thisGuid.ToString(),
            };
            PingTimer.Start();
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

        /// <summary>
        /// Occurs when the server sends a message to this client.
        /// </summary>
        public static event ServerMessageReceived MessageReceived;

        /// <summary>
        /// Send a message to another client.
        /// </summary>
        /// <param name="targetGUID">The target client GUID.</param>
        /// <param name="title">The message title</param>
        /// <param name="message">The message contents</param>
        public static void Forward(string targetGUID, string title, string message)
        {
            var smsg = new ServerMessage
            {
                GUID = targetGUID,
                Name = title,
                Contents = message
            };
            ServerManager.SendMessage("mud_forward", JsonConvert.SerializeObject(smsg));
        }
    }

    /// <summary>
    /// Delegate for handling server messages
    /// </summary>
    /// <param name="msg">A server message containing the protocol message name, GUID of the sender, and the contents of the message.</param>
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
