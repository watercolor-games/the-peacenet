using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Extras;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Objects;

namespace Plex.Frontend.Stories
{
    public static class TheFundamentals
    {
        [RequiresUpgrade("gcc")]
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
                                    Nickname = SaveSystem.GetUsername(),
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
            _client.SendClientMessage("alkaline", $"Welcome to The Syndicate Network, {SaveSystem.GetUsername()}. My name's...alkaline.");
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
            _client.SendClientMessage("jonnythesweetness4", $"alkaline: Who's {SaveSystem.GetUsername()}? What are they doing here?");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", "Relax. They're fine. In fact they may be able to help with Project: Greenlight.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"{SaveSystem.GetUsername()}: Don't worry about Jonathan. He gets a bit...uncomfortable...when I invite new people to the server.");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"He's...gone through quite a few things...he's just looking out for us and our safety. Once you get to know Jonny you'll find out he's a fuckin' amazing friend to have.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "alkaline: Don't you dare tell them! Not until they've gotten through our initiation test!");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Right. Let's get to it. {SaveSystem.GetUsername()} and I were just talking about MoneyMate, and system upgrades, before you joined.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "MoneyMate?");
            Thread.Sleep(4000);
            _client.SendClientMessage("alkaline", $"Yea. What about it? MoneyMate's perfect for {SaveSystem.GetUsername()}.");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", "Hm, I've had bad experiences with them...but whatever you say, alkaline...");
            Thread.Sleep(4000);
            _client.SendClientMessage("jonnythesweetness4", $"Alright, {SaveSystem.GetUsername()}. Open the Plexnet Browser and head to 'main.moneymate/home.rnp'");

            Story.PushObjective("Get MoneyMate", "Follow Jonny's request, and visit 'main.moneymate' in the Plexnet Browser.", () =>
            {
                var win = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is Apps.Plexnet);
                if(win != null)
                {
                    return ((Apps.Plexnet)win.ParentWindow).CurrentUrl == "main.moneymate/home.rnp";
                }
                return false;
            }, () =>
            {
                Thread.Sleep(2000);
                _client.SendClientMessage("jonnythesweetness4", $"{SaveSystem.GetUsername()}, you there yet? If you are, click the big 'download' link towards the bottom.");
                Thread.Sleep(4000);
                _client.SendClientMessage("jonnythesweetness4", "It'll give you a .pst file. Open it in Installer and it'll install MoneyMate manager.");
                Story.PushObjective("Download & install MoneyMate.", "You've got a big download link right there! Go ahead and click it and install MoneyMate Manager.", () =>
                {
                    var type = typeof(MoneyMateManager);
                    return Upgrades.UpgradeAttributesUnlocked(type);
                }, () =>
                {
                    Thread.Sleep(2000);
                    _client.SendClientMessage("jonnythesweetness4", $"{SaveSystem.GetUsername()}: Installed yet?");
                    Thread.Sleep(4000);
                    _client.SendClientMessage("alkaline", "jonnythesweetness4: In this amount of time? They must've.");
                    Thread.Sleep(4000);
                    _client.SendClientMessage("alkaline", $"Anyway {SaveSystem.GetUsername()}, now that you've installed MoneyMate Manager, you must load its upgrades in order for you to use it.");
                    Thread.Sleep(4000);
                    _client.SendClientMessage("alkaline", $"To do this, open your 'Upgrades' application...");
                    Story.PushObjective("Open the Upgrade Manager", "Open the Upgrade Manager (from App Launcher or Terminal) to load MoneyMate's upgrades.", () =>
                    {
                        return AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is CodeShop) != null;
                    },
                    ()=>
                    {
                        Thread.Sleep(1000);
                        _client.SendClientMessage("alkaline", $"Now select MoneyMate Manager (unloaded), and load it.");
                        Story.PushObjective("Load MoneyMate Manager", "Load the MoneyMate Manager upgrade.", () =>
                        {
                            var upg = typeof(MoneyMateManager).GetCustomAttributes(false).FirstOrDefault(x => x is InstallerAttribute) as InstallerAttribute;
                            return Upgrades.IsLoaded(upg.Upgrade);
                        }, () =>
                        {
                            Thread.Sleep(1000);
                            _client.SendClientMessage("alkaline", "Loaded? Hmm... your system node says it is. You've got MoneyMate installed. Good job!");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness4", "Wait, alkaline, how do you know the user successfully did what we said? For sure?");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "That's...not something you need to know, Jonny.");
                            //Coder's note: I keep getting SendMessage and SendClientMessage mixed up. Why?
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", $"Anyway, {SaveSystem.GetUsername()}, a word on upgrades... Right now you can only have 5 loaded at once.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness4", "OOH! Ranks! These are fun to explain!");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "ugh, take it away Jonny... -_-");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness", $"{SaveSystem.GetUsername()}: In the Plexnet, tasks you perform (such as the ones we're getting you to) earn you Experience Points (XP, as we like to shorten it to). The more XP you earn, the higher your System Rank goes. As you earn more ranks, you can gain more upgrade slots and even earn special upgrades you can't get anywhere else!");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "Right now, you can only load 5 upgrades at once, this includes MoneyMate Manager. Ranking up will allow you to load more upgrades at once.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "Since certain tasks may require certain upgrades to be loaded, you will have to decide which ones to unload, or you will have to perform a less intensive task, until you earn enough upgrade slots to satisfy the requirements of the initial task.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness4", "The reason we got you to install MoneyMate, is so that you have a program which allows you to send and receive money in the Plexnet, allowing you to buy upgrades.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness4", "You can also use money to pay for services on the Plexnet.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "Money and experience are very valuable resources within the Plexnet. If you want to get far, you'll want to earn lots of money and lots of XP.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("alkaline", "We'll let ya get off and go explore the net some more on your own. Come see me again and I'll teach you how to earn your first money.");
                            Thread.Sleep(4000);
                            _client.SendClientMessage("jonnythesweetness4", $"Oh, and, {SaveSystem.GetUsername()}, I uhh... have something I wanna tell you in private.... so when you get the chance please come find me.");
                            Story.Context.MarkComplete();
                        });

                    });
                });
            });
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
