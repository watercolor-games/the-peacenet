using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GUI;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Objects;
using ShiftOS.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using static ShiftOS.Engine.SkinEngine;
using System.Text.RegularExpressions;
using System.Threading;

namespace ShiftOS.Frontend.Apps
{
    [WinOpen("irc")]
    [DefaultTitle("IRC Client")]
    [Launcher("IRC Client", false, null, "Networking")]
    public class ChatClient : Control, IShiftOSWindow
    {
        private TextControl _sendprompt = null;
        private TextInput _input = null;
        private Button _send = null;
        private List<ChatMessage> _messages = new List<ChatMessage>();
        const int usersListWidth = 100;
        const int topicBarHeight = 24;
        public IRCNetwork NetInfo = null;

        public ChatClient()
        {
            _send = new GUI.Button();
            _input = new GUI.TextInput();
            _sendprompt = new GUI.TextControl();
            _sendprompt.Text = "Send message:";
            _sendprompt.AutoSize = true;
            _send.Text = "Send";
            _send.AutoSize = true;
            AddControl(_send);
            AddControl(_sendprompt);
            AddControl(_input);

            _input.KeyEvent += (key) =>
            {
                if(key.Key == Microsoft.Xna.Framework.Input.Keys.Enter && !string.IsNullOrWhiteSpace(_input.Text))
                {
                    SendMessage();
                }
            };
            _send.Click += () =>
            {
                if (!string.IsNullOrWhiteSpace(_input.Text))
                {
                    SendMessage();
                }

            };
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _send.X = Width - _send.Width - 10;
            _send.Y = Height - _send.Height - 10;
            _sendprompt.X = 10;
            _sendprompt.Y = _send.Y + ((_send.Height - _sendprompt.Height) / 2);
            _input.Height = 24;
            _input.Y = _send.Y + ((_send.Height - _input.Height) / 2);
            _input.X = _sendprompt.X + _sendprompt.Width + 10;
            int inRight = (Width - _send.Width - 20);
            _input.AutoSize = false;
            _input.Width = inRight - _input.X;
            if (requiresRepaint)
            {
                Invalidate();
                requiresRepaint = false;
            }
        }

        public bool ChannelConnected
        {
            get; private set;
        }

        public bool NetworkConnected
        {
            get; private set;
        }

        public void SendMessage()
        {
            _messages.Add(new Apps.ChatMessage
            {
                Timestamp = DateTime.Now,
                Author = SaveSystem.CurrentSave.Username,
                Message = _input.Text
            });
            _input.Text = "";

            //Let's try the AI stuff... :P
            var rmsg = _messages[rnd.Next(_messages.Count)].Message;
            if (!messagecache.Contains(_messages.Last().Message))
            {
                messagecache.Add(_messages.Last().Message);
#if RIP_USERS_SSD
                SaveCache();
#endif
			}
            var split = new List<string>(rmsg.Split(' '));
            List<string> nmsg = new List<string>();
            if (split.Count > 2)
            {
                int amount = rnd.Next(2, 50);
                for (int i = 0; i < amount; i++)
                {
                    nmsg.Add(split[rnd.Next(split.Count)]);
                }
            }
            else if (split.Count < 6)
            {
                for (int i = 0; i < rnd.Next(2); i++)
                {
                    split.RemoveAt(i);
                }
                split.AddRange(Regex.Split(Regex.Replace(messagecache[rnd.Next(messagecache.Count)], "debugbot", outcomes[rnd.Next(outcomes.Length)], RegexOptions.IgnoreCase), " "));
            }
            split.RemoveAt(rnd.Next(split.Count));
            split.Add(Regex.Replace(messagecache[rnd.Next(messagecache.Count)], "debugbot", outcomes[rnd.Next(outcomes.Length)], RegexOptions.IgnoreCase));
            string combinedResult = string.Join(" ", split);
            _messages.Add(new ChatMessage
            {
                Timestamp = DateTime.Now,
                Author = "debugbot",
                Message = combinedResult
            });

        }

        readonly string[] outcomes = new string[] { "ok", "sure", "yeah", "yes", "no", "nope", "alright" };
        Random rnd = new Random();
        private List<string> messagecache = new List<string>();

        public void SendClientMessage(string nick, string message)
        {
            _messages.Add(new Apps.ChatMessage
            {
                Timestamp = DateTime.Now,
                Author = nick,
                Message = message
            });
            Invalidate();
        }

        int vertSeparatorLeft = 20;
        bool requiresRepaint = false;

        protected override void OnPaint(GraphicsContext gfx)
        {
            int messagesTop = NetworkConnected ? topicBarHeight : 0;
            int messagesFromRight = ChannelConnected ? usersListWidth : 0;

            int _bottomseparator = _send.Y - 10;
            gfx.DrawRectangle(0, _bottomseparator, Width, 1, UIManager.SkinTextures["ControlTextColor"]);
            int nnGap = 25;
            int messagebottom = _bottomseparator - 5;
            try
            {
                foreach (var msg in _messages.OrderByDescending(x => x.Timestamp))
                {
                    if (Height - messagebottom <= messagesTop)
                        break;
                    var tsProper = $"[{msg.Timestamp.Hour.ToString("##")}:{msg.Timestamp.Minute.ToString("##")}]";
                    var nnProper = $"<{msg.Author}>";
                    var tsMeasure = gfx.MeasureString(tsProper, LoadedSkin.TerminalFont);
                    var nnMeasure = gfx.MeasureString(nnProper, LoadedSkin.TerminalFont);
                    int old = vertSeparatorLeft;
                    vertSeparatorLeft = (int)Math.Round(Math.Max(vertSeparatorLeft, tsMeasure.X + nnGap + nnMeasure.X + 2));
                    if (old != vertSeparatorLeft)
                        requiresRepaint = true;
                    var msgMeasure = gfx.MeasureString(msg.Message, LoadedSkin.TerminalFont, (Width - vertSeparatorLeft - 4) - messagesFromRight);
                    messagebottom -= (int)msgMeasure.Y;
                    gfx.DrawString(tsProper, 0, messagebottom, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.TerminalFont);
                    var nnColor = Color.LightGreen;

                    if (msg.Author == SaveSystem.CurrentSave.Username)
                        nnColor = Color.Red;
                    else
                    {
                        if (NetInfo != null)
                        {
                            if (NetInfo.Channel != null)
                            {
                                if (NetInfo.Channel.OnlineUsers != null)
                                {
                                    var user = NetInfo.Channel.OnlineUsers.FirstOrDefault(x => x.Nickname == msg.Author);
                                    if (user != null)
                                    {
                                        switch (user.Permission)
                                        {
                                            case IRCPermission.ChanOp:
                                                nnColor = Color.Orange;
                                                break;
                                            case IRCPermission.NetOp:
                                                nnColor = Color.Yellow;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    gfx.DrawString(nnProper, (int)tsMeasure.X + nnGap, messagebottom, nnColor, LoadedSkin.TerminalFont);
                    var mcolor = LoadedSkin.ControlTextColor.ToMonoColor();
                    if (msg.Message.Contains(SaveSystem.CurrentSave.Username))
                        mcolor = Color.Orange;
                    gfx.DrawString(msg.Message, vertSeparatorLeft + 4, messagebottom, mcolor, LoadedSkin.TerminalFont, (Width - vertSeparatorLeft - 4) - messagesFromRight);
                }
            }
            catch { }

            string topic = "";
            if (NetworkConnected)
            {
                topic = $"{NetInfo.FriendlyName}: {NetInfo.MOTD}";
                if (ChannelConnected)
                {
                    topic = $"#{NetInfo.Channel.Tag} | {NetInfo.Channel.Topic}";
                    int usersStartY = messagesTop;
                    foreach(var user in NetInfo.Channel.OnlineUsers.OrderBy(x=>x.Nickname))
                    {
                        var measure = gfx.MeasureString(user.Nickname, LoadedSkin.TerminalFont);

                        var nnColor = Color.LightGreen;
                        if (user.Nickname == SaveSystem.CurrentSave.Username)
                            nnColor = Color.Red;
                        else
                        {
                            switch (user.Permission)
                            {
                                case IRCPermission.ChanOp:
                                    nnColor = Color.Orange;
                                    break;
                                case IRCPermission.NetOp:
                                    nnColor = Color.Yellow;
                                    break;
                            }
                        }

                        gfx.DrawString(user.Nickname, Width - messagesFromRight + 2, usersStartY, nnColor, LoadedSkin.TerminalFont);

                        usersStartY += (int)measure.Y;
                    }
                    gfx.DrawRectangle(Width - messagesFromRight, messagesTop, 1, _bottomseparator - messagesTop, LoadedSkin.ControlTextColor.ToMonoColor());
                }
                gfx.DrawString(topic, 0, 0, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.TerminalFont);
                gfx.DrawRectangle(0, messagesTop, Width, 1, LoadedSkin.ControlTextColor.ToMonoColor());
            }

            gfx.DrawRectangle(vertSeparatorLeft, messagesTop, 1, _bottomseparator - messagesTop, UIManager.SkinTextures["ControlTextColor"]);
        }
		
        public void FakeConnection(IRCNetwork net)
        {
            NetInfo = net;
            var cs = net.Channel.OnlineUsers.FirstOrDefault(x => x.Nickname == "ChanServ");
            if (cs == null)
                net.Channel.OnlineUsers.Add(new IRCUser
                {
                    Nickname = "ChanServ",
                    Permission = IRCPermission.ChanOp
                });
            var t = new Thread(() =>
            {
                SendClientMessage("shiftos", $"Looking up {net.SystemName}");
                Thread.Sleep(250);
                SendClientMessage("*", $"Connecting to {net.SystemName} ({net.SystemName}:6667)");
                Thread.Sleep(1500);
                SendClientMessage("*", "Connected. Now logging in.");
                Thread.Sleep(25);
                SendClientMessage("*", "*** Looking up your hostname... ");
                Thread.Sleep(2000);
                SendClientMessage("*", "***Checking Ident");
                Thread.Sleep(10);
                SendClientMessage("*", "*** Couldn't look up your hostname");
                Thread.Sleep(10);
                SendClientMessage("*", "***No Ident response");
                Thread.Sleep(750);
                SendClientMessage("*", "Capabilities supported: account-notify extended-join identify-msg multi-prefix sasl");
                Thread.Sleep(250);
                SendClientMessage("*", "Capabilities requested: account-notify extended-join identify-msg multi-prefix");
                Thread.Sleep(250);
                SendClientMessage("*", "Capabilities acknowledged: account-notify extended-join identify-msg multi-prefix");
                Thread.Sleep(500);
                SendClientMessage("*", $"Welcome to the {net.FriendlyName} {SaveSystem.CurrentSave.Username}");
                NetworkConnected = true;
                Thread.Sleep(250);
                SendClientMessage("*", $"{SaveSystem.CurrentSave.Username} sets mode +i on {SaveSystem.CurrentSave.Username}");
                Thread.Sleep(300);
                SendClientMessage("shiftos", "Joining #" + net.Channel.Tag);
                Thread.Sleep(100);
                ChannelConnected = true;
                SendClientMessage("shiftos", $"{net.Channel.Topic}: {net.Channel.OnlineUsers.Count} users online");
                Thread.Sleep(10);
                SendClientMessage("ChanServ", "ChanServ sets mode -v on " + SaveSystem.CurrentSave.Username);
            });
            t.Start();
        }
        
        public void OnLoad()
        {
			if (System.IO.File.Exists("aicache.dat"))
				messagecache = System.IO.File.ReadAllLines("aicache.dat").ToList();
        }

        public void OnSkinLoad()
        {
        }
        
        public bool OnUnload()
        {
			// this doesn't get called... dammit
			SaveCache();
            return true;
        }

		private void SaveCache()
		{
			// It's watching you...
			System.IO.File.WriteAllLines("aicache.dat", messagecache);
		}

        public void OnUpgrade()
        {
        }
    }
    
    public class ChatMessage
    {
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
    }
}
