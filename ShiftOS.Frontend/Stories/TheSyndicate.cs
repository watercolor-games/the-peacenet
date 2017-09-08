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
    public static class TheFundamentals
    {
        [Mission("m00_fundamentals_01", "The Fundamentals", "Welcome to Plexnet. Let's get your system ready.", 650, "alkaline")]
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
                                },
                                new Objects.IRCUser
                                {
                                    Nickname = SaveSystem.CurrentSave.Username,
                                    Permission = Objects.IRCPermission.User
                                }
                           }
                }
            });
            while(irc.ChannelConnected == false)
            {
                Thread.Sleep(100);
            }
            _client = irc;
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Welcome to The Syndicate Network, {SaveSystem.CurrentSave.Username}. My name's...alkaline.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"I'm the creator of Plexgate, and I've got some things you need to know.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Your system's probably quite barebones, not many things to do, and you're probably not sure where to start...");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Well, that's what we're here for.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Yknow, you're not going to be able to do much without money.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"There's something called MoneyMate, it's like a virtual bank account you can use in the Plexnet to buy things, such as system upgrades.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", "Lord knows, you're going to need a lot of those.");
            Thread.Sleep(4000);
            _client.SendClientMessage("ChanServ", $"ChanServ sets mode +v on jonnythesweetness4");
            _client.AddUser("jonnythesweetness4", Objects.IRCPermission.ChanOp);
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "Hello people!");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", "Hello, Jonathan.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", $"alkaline: Who's {SaveSystem.CurrentSave.Username}? What are they doing here?");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", "Relax. They're fine. In fact they may be able to help with Project: Greenlight.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"{SaveSystem.CurrentSave.Username}: Don't worry about Jonathan. He gets a bit...uncomfortable...when I invite new people to the server.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"He's...gone through quite a few things...he's just looking out for us and our safety. Once you get to know Jonny you'll find out he's a fuckin' amazing friend to have.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "alkaline: Don't you dare tell them! Not until they've gotten through our initiation test!");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Right. Let's get to it. {SaveSystem.CurrentSave.Username} and I were just talking about MoneyMate, and system upgrades, before you joined.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "MoneyMate?");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Yea. What about it? MoneyMate's perfect for {SaveSystem.CurrentSave.Username}.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "Hm, I've had bad experiences with them...but whatever you say, alkaline...");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", $"Alright, {SaveSystem.CurrentSave.Username}. Open the Plexnet Browser and head to 'net.moneymate/home.rnp', and download the moneymate .pst file. Let us know when you've got it.");


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
