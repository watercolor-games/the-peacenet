using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;

namespace ShiftOS.WinForms.Servers
{
    [Server("Remote Terminal Server", 21)]
    //[RequiresUpgrade("story_hacker101_breakingthebonds")] //Uncomment when story is implemented.
    public class RemoteTerminalServer : Server
    {
        public void MessageReceived(ServerMessage msg)
        {
            var rtsMessage = JsonConvert.DeserializeObject<RTSMessage>(msg.Contents);
            if (msg.Name == "disconnected")
            {
                if (Applications.Terminal.IsInRemoteSystem == true)
                {
                    if (Applications.Terminal.RemoteSystemName == rtsMessage.SenderSystemName)
                    {
                        if(Applications.Terminal.RemoteUser == rtsMessage.Username)
                            if(Applications.Terminal.RemotePass == rtsMessage.Password)
                            {
                                Applications.Terminal.IsInRemoteSystem = false;
                                Applications.Terminal.RemoteSystemName = "";
                                Applications.Terminal.RemoteUser = "";
                                Applications.Terminal.RemotePass = "";
                                TerminalBackend.PrefixEnabled = true;
                                TerminalBackend.PrintPrompt();
                            }
                    }
                }
                return;
            }

            string currentUserName = SaveSystem.CurrentUser.Username;

            var user = SaveSystem.CurrentSave.Users.FirstOrDefault(x => x.Username == rtsMessage.Username && x.Password == rtsMessage.Password);

            if(user == null)
            {
                ServerManager.SendMessageToIngameServer(rtsMessage.SenderSystemName, 0, "Access denied.", "The username and password you have provided was denied.");
                return;
            }
            else
            {
                SaveSystem.CurrentUser = user;

                string cmd = rtsMessage.Namespace + "." + rtsMessage.Command + JsonConvert.SerializeObject(rtsMessage.Arguments);
                TerminalBackend.InvokeCommand(cmd, true);
                ServerManager.SendMessageToIngameServer(rtsMessage.SenderSystemName, 1, "writeline", TerminalBackend.LastCommandBuffer);
                ServerManager.SendMessageToIngameServer(rtsMessage.SenderSystemName, 1, "write", $"{rtsMessage.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");

                SaveSystem.CurrentUser = SaveSystem.Users.FirstOrDefault(x => x.Username == currentUserName);
            }
        }

        [Command("connect")]
        [RequiresArgument("sysname")]
        [RequiresArgument("username")]
        [RequiresArgument("password")]
        public static bool Connect(Dictionary<string, object> args)
        {
            string sysname = args["sysname"].ToString();
            string username = args["username"].ToString();
            string password = args["password"].ToString();

            bool connectionFinished = false;


            new Thread(() =>
            {
                Thread.Sleep(10000);
                if (connectionFinished == false)
                {
                    Applications.Terminal.IsInRemoteSystem = false;
                    Applications.Terminal.RemoteSystemName = "";
                    Applications.Terminal.RemoteUser = "";
                    Applications.Terminal.RemotePass = "";
                    TerminalBackend.PrefixEnabled = true;
                    Console.WriteLine("[rts] Connection failed, target system did not respond.");
                    TerminalBackend.PrintPrompt();

                }
            }).Start();

            ServerMessageReceived smr = null;
            smr = (msg) =>
            {
                if (msg.Name == "msgtosys")
                {
                    var m = JsonConvert.DeserializeObject<ServerMessage>(msg.Contents);
                    if (m.GUID.Split('|')[2] != ServerManager.thisGuid.ToString())
                    {
                        connectionFinished = true;
                        ServerManager.MessageReceived -= smr;
                    }
                }
            };
            ServerManager.MessageReceived += smr;
            ServerManager.SendMessageToIngameServer(sysname, 21, "cmd", JsonConvert.SerializeObject(new RTSMessage
            {
                SenderSystemName = SaveSystem.CurrentSave.SystemName,
                Username = username,
                Password = password,
                Namespace = "trm",
                Command = "clear"
            }));
            Applications.Terminal.IsInRemoteSystem = true;
            Applications.Terminal.RemoteSystemName = sysname;
            Applications.Terminal.RemoteUser = username;
            Applications.Terminal.RemotePass = password;
            TerminalBackend.PrefixEnabled = false;
            return true;
        }
    }

    [Server("Generic port 0", 0)]
    public class InfoboxServer : Server
    {
        public void MessageReceived(ServerMessage msg)
        {
            Infobox.Show(msg.Name, msg.Contents);
        }
    }

    [Server("Generic port 1", 1)]
    public class ConsoleServer : Server
    {
        public void MessageReceived(ServerMessage msg)
        {
            switch (msg.Name)
            {
                case "write":
                    Console.Write(msg.Contents);
                    break;
                case "writeline":
                    Console.WriteLine(msg.Contents);
                    break;
            }
        }
    }

    public class RTSMessage
    {
        public string SenderSystemName { get; set; }

        public string Namespace { get; set; }
        public string Command { get; set; }
        public Dictionary<string, object> Arguments { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
