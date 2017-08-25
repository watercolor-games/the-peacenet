using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.Stories
{
    public static class TheSyndicate
    {
        [Mission("m00_thesyndicate", "The Syndicate", "Where the hell are you? And who do you trust? That's a hard question, but it's one worth figuring out.", 650, "alkaline")]
        public static void MTheSyndicate()
        {
            Story.Context.AutoComplete = false;

            var irc = new ChatClient();
            AppearanceManager.SetupWindow(irc);

            irc.FakeConnection(new Objects.IRCNetwork
            {
                FriendlyName = "The Syndicate Network",
                MOTD = "",
                SystemName = "thesyndicatenetwork",
                Channel = new Objects.IRCChannel
                {
                    Tag = "#general",
                    Topic = "A place for general discussion.",
                    OnlineUsers = new List<Objects.IRCUser>()
                           {
                                new Objects.IRCUser
                                {
                                     Nickname = "alkaline",
                                      Permission = Objects.IRCPermission.NetOp
                                }
                           }
                }
            });
            while(irc.ChannelConnected == false)
            {
                Thread.Sleep(100);
            }

        }

        private static ChatClient _client = null;

        public static void WriteMessage(string who, string message)
        {
            if (_client == null)
                return;

            foreach(char c in message)
            {
                Thread.Sleep(45);
            }

            _client.SendClientMessage(who, message);
        }


    }
}
