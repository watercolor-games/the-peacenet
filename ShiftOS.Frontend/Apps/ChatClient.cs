using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Objects.ShiftFS;
using Plex.Objects;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Plex.Frontend.Apps
{
    [WinOpen("chat")]
    [Launcher("Chat", false, null, "Networking")]
    [SingleInstance]
    public class ChatClient : Control, IPlexWindow
    {
        private ScrollView _messageList = new ScrollView();
        private List<ClientChatMessage> _realMessages = new List<ClientChatMessage>();
        private Button _sendMessage = new Button();
        private TextInput _msgText = new TextInput();

        public ChatClient()
        {
            Width = 720;
            Height = 600;
            _msgText.KeyEvent += (key) =>
            {
                if (key.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                    SendToServer();
            };
            _sendMessage.Click += SendToServer;
        }


        /// <summary>
        /// Adds a new message to the user interface. Will edit an existing message if you provide an already-existing ID.
        /// </summary>
        /// <param name="id">The ID of the message for tracking purposes.</param>
        /// <param name="author">The author of the message.</param>
        /// <param name="text">The text of the message.</param>
        /// <param name="dt">The timestamp of the message, used for sorting.</param>
        public void AddClientMessage(string id, string author, string text, DateTime dt)
        {
            var existing = _realMessages.FirstOrDefault(x => x.MessageID == id);
            if(existing == null)
            {
                existing = new ClientChatMessage
                {
                    MessageID = id,
                    AuthorText = new TextControl
                    {
                        FontStyle = TextControlFontStyle.Header3,
                        Text = author,
                        AutoSize = true
                    },
                    MessageText = new TextControl
                    {
                        FontStyle = TextControlFontStyle.System,
                        AutoSize = true,
                        Text = text
                    },
                    Timestamp = new TextControl
                    {
                        FontStyle = TextControlFontStyle.System,
                        AutoSize = true,
                        Text = dt.ToString()
                    },
                    Avatar = new PictureBox { }
                };
                Engine.Desktop.InvokeOnWorkerThread(() =>
                {
                    existing.Avatar.Image = FontAwesome.user.ToTexture2D(UIManager.GraphicsDevice);
                    if (!existing.MessageID.StartsWith("client_"))
                    {
                        DownloadAvatar(author, existing.Avatar);
                    }
                    _realMessages.Add(existing);
                    AddToView(existing);
                });
            }
            else
            {
                existing.MessageText.Text = text; //We don't want to change EVERYTHING about the message. That's just stupid.
            }
        }

        private Dictionary<string, Texture2D> avatarcache = new Dictionary<string, Texture2D>();

        public void DownloadAvatar(string author, PictureBox destination)
        {
            if (avatarcache.ContainsKey(author))
                destination.Image = avatarcache[author];
            else
            {
                string uname = author;
                if(uname.Contains("@"))
                {
                    uname = uname.Substring(0, uname.IndexOf("@"));
                }
                new Thread(() =>
                {
                    var wc = new System.Net.WebClient();
                    try
                    {
                        var bytes = wc.DownloadData("http://" + ServerManager.SessionInfo.ServerIP + ":3253/avatar/" + uname);
                        using(var memstr = new MemoryStream(bytes))
                        {
                            var img = System.Drawing.Image.FromStream(memstr);
                            Engine.Desktop.InvokeOnWorkerThread(() =>
                            {
                                var tex2 = img.ToTexture2D(UIManager.GraphicsDevice);
                                avatarcache.Add(author, tex2);
                                destination.Image = tex2;
                            });
                        }
                    }
                    catch { }
                }).Start();
            }
        }

        public void AddToView(ClientChatMessage msg)
        {
            bool doScroll = _messageList.Value == _messageList.Maximum;

            _messageList.AddControl(msg.Avatar);
            _messageList.AddControl(msg.Timestamp);
            _messageList.AddControl(msg.AuthorText);
            _messageList.AddControl(msg.MessageText);
            if (doScroll)
            {
                OnLayout(new GameTime());
                _messageList.RecalculateScrollHeight();
                _messageList.Value = _messageList.Maximum;
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _sendMessage.Text = "Send";
            _sendMessage.Y = (Height - _sendMessage.Height) - 10;
            _sendMessage.X = (Width - _sendMessage.Width) - 10;

            _msgText.X = 10;
            
            _msgText.AutoSize = false;
            _msgText.MinWidth = (Width - 25) - _sendMessage.Width;
            
            _msgText.MinHeight = _sendMessage.Height;

            _msgText.MaxWidth = _msgText.MinWidth;
            _msgText.Width = _msgText.MaxWidth;

            _msgText.Y = _sendMessage.Y + ((_sendMessage.Height - _msgText.Height) / 2);
            _msgText.Layout(gameTime);

            _messageList.X = 0;
            _messageList.Y = 0;
            _messageList.MinWidth = Width;
            _messageList.MaxWidth = Width;
            _messageList.Width = Width;
            _messageList.AutoSize = true;
            _messageList.MinHeight = _sendMessage.Y - 10;
            _messageList.MaxHeight = _sendMessage.Y - 10;
            int msgCurrentY = 0;
            foreach(var msg in _realMessages)
            {
                msg.Layout(ref msgCurrentY, _messageList.MaxWidth);
            }
        }

        [BroadcastHandler(BroadcastType.CHAT_USERLEFT)]
        public static void HandleChatUserLeft(PlexServerHeader header)
        {
            var chatclient = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is ChatClient);
            if (chatclient != null)
            {
                var real = (ChatClient)chatclient.ParentWindow;
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(header)))
                {
                    real.AddClientMessage("client_" + Guid.NewGuid().ToString(), "peacenet", $"{reader.ReadString()} has left the chat.", DateTime.Now);
                }
            }
        }


        [BroadcastHandler(BroadcastType.CHAT_USERJOINED)]
        public static void HandleChatUserJoined(PlexServerHeader header)
        {
            var chatclient = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is ChatClient);
            if (chatclient != null)
            {
                var real = (ChatClient)chatclient.ParentWindow;
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(header)))
                {
                    real.AddClientMessage("client_" + Guid.NewGuid().ToString(), "peacenet", $"{reader.ReadString()} has joined the chat!", DateTime.Now);
                }
            }
        }


        public void SendToServer()
        {
            if (string.IsNullOrWhiteSpace(_msgText.Text))
                return;
            string txt = _msgText.Text;
            var req = ServerMessageType.CHAT_SENDTEXT;
            if(txt.StartsWith("/me "))
            {
                req = ServerMessageType.CHAT_SENDACTION;
                txt = txt.Remove(0, 4);
            }
            _msgText.Text = "";
            using(var netstr = new ServerStream(req))
            {
                netstr.Write(txt);
                var result = netstr.Send();
                if(result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        AddClientMessage("client_" + Guid.NewGuid().ToString(), "peacenet", reader.ReadString(), DateTime.Now);
                    }
                }
            }

            
        }


        [BroadcastHandler(BroadcastType.CHAT_MESSAGESENT)]
        public static void HandleChatMessageSent(PlexServerHeader header)
        {
            var chatclient = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is ChatClient);
            if (chatclient != null)
            {
                var real = (ChatClient)chatclient.ParentWindow;
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(header)))
                {
                    real.AddClientMessage(reader.ReadString(), reader.ReadString(), reader.ReadString(), DateTime.Now);
                }
            }
        }

        [BroadcastHandler(BroadcastType.CHAT_ACTIONSENT)]
        public static void HandleChatActionSent(PlexServerHeader header)
        {
            var chatclient = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow is ChatClient);
            if (chatclient != null)
            {
                var real = (ChatClient)chatclient.ParentWindow;
                using (var reader = new BinaryReader(ServerManager.GetResponseStream(header)))
                {
                    real.AddClientMessage(reader.ReadString(), reader.ReadString(), "[" + reader.ReadString() + "]", DateTime.Now);

                }
            }
        }

        public void OnLoad()
        {
            AddControl(_messageList);
            AddControl(_sendMessage);
            AddControl(_msgText);
            using(var netstr = new ServerStream(ServerMessageType.CHAT_JOIN))
            {
                var result = netstr.Send();
                if(result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        AddClientMessage("client_" + Guid.NewGuid().ToString(), "peacenet", reader.ReadString(), DateTime.Now);
                    }
                }
            }
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            using (var netstr = new ServerStream(ServerMessageType.CHAT_LEAVE))
            {
                var result = netstr.Send();
                if (result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        AddClientMessage("client_" + Guid.NewGuid().ToString(), "peacenet", reader.ReadString(), DateTime.Now);
                    }
                }
            }
            return true;
        }

        public void OnUpgrade()
        {
        }
    }

    public class ClientChatMessage
    {
        public TextControl AuthorText { get; set; }
        public TextControl MessageText { get; set; }
        public TextControl Timestamp { get; set; }
        public string MessageID { get; set; }
        public PictureBox Avatar { get; set; }

        private int lastWidth = 0;

        public void Layout(ref int layout_y, int layout_max_width)
        {
            Avatar.Width = 64;
            Avatar.Height = 64;

            int real_width = (layout_max_width - 40) - Avatar.Width;
            if (real_width != lastWidth)
            {
                AuthorText.RequireTextRerender();
                MessageText.RequireTextRerender();
                lastWidth = real_width;
            }
            AuthorText.AutoSize = true;
            AuthorText.MaxWidth = real_width;
            AuthorText.Layout(new GameTime());
            MessageText.AutoSize = true;
            MessageText.MaxWidth = real_width;
            MessageText.Layout(new GameTime());
            Timestamp.Layout(new GameTime());

            layout_y += 15;
            Avatar.Y = layout_y;
            Avatar.X = 15;


            AuthorText.X = Avatar.X + Avatar.Width + 10;
            AuthorText.MaxWidth = real_width;
            AuthorText.Y = layout_y;

            Timestamp.X = AuthorText.X + AuthorText.Width + 5;
            Timestamp.Y = AuthorText.Y + ((AuthorText.Height - Timestamp.Height) / 2);

            layout_y += AuthorText.Height + 5;
            MessageText.X = AuthorText.X;
            MessageText.MaxWidth = real_width;
            MessageText.AutoSize = true;
            MessageText.Y = layout_y;
            layout_y += MessageText.Height + 5;
        }
    }
}
