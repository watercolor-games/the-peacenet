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



        public ChatClient()
        {
            _messages.Add(new ChatMessage
            {
                Timestamp = DateTime.Now,
                Author = "michael",
                Message = "Welcome to ShiftOS IRC! Type in the box below to type a message."
            });
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
            int _bottomseparator = _send.Y - 10;
            gfx.DrawRectangle(0, _bottomseparator, Width, 1, UIManager.SkinTextures["ControlTextColor"]);
            int nnGap = 25;
            int messagebottom = _bottomseparator - 5;
            foreach (var msg in _messages.OrderByDescending(x=>x.Timestamp))
            {
                if (Height - messagebottom <= 0)
                    break;
                var tsProper = $"[{msg.Timestamp.Hour.ToString("##")}:{msg.Timestamp.Minute.ToString("##")}]";
                var nnProper = $"<{msg.Author}>";
                var tsMeasure = gfx.MeasureString(tsProper, LoadedSkin.TerminalFont);
                var nnMeasure = gfx.MeasureString(nnProper, LoadedSkin.TerminalFont);
                int old = vertSeparatorLeft;
                vertSeparatorLeft = (int)Math.Round(Math.Max(vertSeparatorLeft, tsMeasure.X + nnGap + nnMeasure.X+2));
                if (old != vertSeparatorLeft)
                    requiresRepaint = true;
                var msgMeasure = gfx.MeasureString(msg.Message, LoadedSkin.TerminalFont, Width - vertSeparatorLeft - 4);
                messagebottom -= (int)msgMeasure.Y;
                gfx.DrawString(tsProper, 0, messagebottom, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.TerminalFont);
                var nnColor = (msg.Author == SaveSystem.CurrentSave.Username) ? Color.Red : Color.LightGreen;
                gfx.DrawString(nnProper, (int)tsMeasure.X + nnGap, messagebottom, nnColor, LoadedSkin.TerminalFont);
                gfx.DrawString(msg.Message, vertSeparatorLeft + 4, messagebottom, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.TerminalFont, Width - vertSeparatorLeft - 4);
            }
            gfx.DrawRectangle(vertSeparatorLeft, 0, 1, _bottomseparator, UIManager.SkinTextures["ControlTextColor"]);
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
