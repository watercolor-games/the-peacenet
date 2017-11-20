using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects;

namespace Plex.Frontend.Apps
{
    [DefaultTitle("Select server")]
    public class MultiplayerServerList : Control, IPlexWindow
    {
        private List<ServerDetails> _servers = null;
        private TextControl _title = new TextControl();
        private Button _close = new Button();
        private Button _delete = new Button();
        private Button _rename = new Button();
        private Button _add = new Button();
        private Button _details = new Button();
        private Button _connect = new Button();
        private ListView _list = new ListView();


        public MultiplayerServerList(Action _cb = null)
        {
            _callback = _cb;
            Width = 900;
            Height = 600;
            AddControl(_close);
            AddControl(_delete);
            AddControl(_rename);
            AddControl(_details);
            AddControl(_add);
            AddControl(_connect);
            AddControl(_list);
            AddControl(_title);

            _close.Text = "Close";
            _close.AutoSize = true;
            _add.Text = "Add server";
            _add.AutoSize = true;
            _rename.Text = "Rename";
            _rename.Visible = true;
            _delete.Text = "Delete";
            _delete.AutoSize = true;
            _details.AutoSize = true;
            _details.Text = "Details";
            _connect.Text = "Connect";
            _connect.AutoSize = true;

            _close.Click += () =>
            {
                AppearanceManager.Close(this);
            };

            _list.DoubleClick += () =>
            {
                if(_list.SelectedItem != null)
                {
                    ConnectToServer();
                }
            };

            _add.Click += () =>
            {
                Engine.Infobox.PromptText("Name the server", "Please enter a name for this server so we can show it in the list.", (name) =>
                {
                    if(string.IsNullOrWhiteSpace(name))
                    {
                        Engine.Infobox.Show("Invalid name", "You can't put a blank name. Server not added.");
                        return;
                    }
                    var server = new ServerDetails();
                    server.FriendlyName = name;
                    Engine.Infobox.PromptText("Please enter the server's hostname", "Please enter the hostname we should connect to.\r\n\r\nExamples:\r\ntheplexnet.com\r\nlocalhost\r\n127.0.0.1\r\ntheplexnet.com:420\r\nlocalhost:1337", (hn) =>
                    {
                        int port = 3251;
                        string host = hn;
                        bool parsePort = false;
                        if (string.IsNullOrWhiteSpace(hn))
                        {
                            Engine.Infobox.Show("Empty hostname.", "You cannot supply an empty hostname! Server not added.");
                            return;
                        }
                        if (hn.Contains(":"))
                        {
                            parsePort = true;
                        }

                        if (parsePort)
                        {
                            string[] split = hn.Split(':');
                            host = split[0];
                            if(int.TryParse(split[1], out port) == false)
                            {
                                Engine.Infobox.Show("Invalid port.", "The port you entered (" + split[1] + ") is not a valid number.");
                                return;
                            }
                            if(port < 0 || port > 65535)
                            {
                                Engine.Infobox.Show("Invalid port.", "The port you entered (" + split[1] + ") is either too large or too small. Ports must be greater than or equal to 0, and less than or equal to 65535.");
                                return;
                            }
                            
                        }
                        server.Hostname = host;
                        server.Port = port;
                        _servers.Add(server);
                        RefreshList();
                    });
                });
            };
        }

        

        public void ConnectToServer()
        {
            var server = _servers[_list.SelectedIndex];
            try
            {
                UIManager.ConnectToServer(server.Hostname, server.Port);
            }
            catch (Exception ex)
            {
                Engine.Infobox.Show("Connection error.", ex.Message);
                return;
            }
            _callback?.Invoke();
            AppearanceManager.Close(this);
        }

        public Action _callback = null;

        public void OnLoad()
        {
            if (_servers == null)
            {
                _servers = new List<ServerDetails>();
            }
            RefreshList();
        }

        public void RefreshList()
        {
            _list.ClearItems();
            foreach(var server in _servers)
            {
                _list.AddItem(new ListViewItem
                {
                    Text = server.FriendlyName,
                     Tag = _servers.IndexOf(server).ToString(),
                     ImageKey = _servers.IndexOf(server).ToString()
                });
            }

            _list.ClearImages();
            new Thread(() =>
            {
                WebClient wc = new WebClient();

                foreach (var server in _servers)
                {
                    string key = _servers.IndexOf(server).ToString();
                    try
                    {
                        UIManager.ShowCloudDownload();
                        byte[] serverIcon = wc.DownloadData("http://" + server.Hostname + ":3253/servericon");

                        using(var memstr = new MemoryStream(serverIcon))
                        {
                            var texture = System.Drawing.Image.FromStream(memstr).ToTexture2D(UIManager.GraphicsDevice);
                            _list.SetImage(key, texture);
                        }

                    }
                    catch
                    {
                        _list.SetImage(key, FontAwesome.times_circle.ToTexture2D(UIManager.GraphicsDevice));
                    }
                    finally
                    {
                        UIManager.HideCloudDownload();
                    }
                }
            }).Start();
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.X = 15;
            _title.Y = 15;
            _title.AutoSize = true;
            _title.Text = "Select a server";

            _close.X = Width - _close.Width - 15;
            _add.X = 15;
            _close.Y = Height - _close.Height - 15;
            _add.Y = _close.Y;

            bool _showmore = _list.SelectedItem != null;
            _rename.Visible = _showmore;
            _details.Visible = _showmore;
            _delete.Visible = _showmore;
            _connect.Visible = _showmore;

            _delete.Y = _close.Y;
            _delete.X = _add.X + _add.Width + 5;
            _rename.X = _delete.X + _delete.Width + 5;
            _details.X = _rename.X + _rename.Width + 5;
            _connect.X = _details.X + _details.Width + 5;
            _rename.Y = _delete.Y;
            _details.Y = _rename.Y;
            _connect.Y = _details.Y;

            _list.X = 15;
            _list.Y = _title.Y + _title.Height + 15;
            _list.Width = Width - 30;
            _list.Height = Height - _list.Y - (_close.Height + 30);
        }

        public void OnSkinLoad()
        {
            _title.FontStyle = TextControlFontStyle.Header1;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
