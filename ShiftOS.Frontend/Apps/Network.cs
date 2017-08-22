using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;
using Plex.Objects;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.Apps
{
    [WinOpen("network")]
    [Launcher("Network", false, null, "Networking")]
    [DefaultTitle("Network")]
    public class Network : Control, IPlexWindow
    {
        private NetworkGUIState _state = NetworkGUIState.Listing;
        private TextControl _title = new TextControl();
        private TextControl _subtitle = new TextControl();
        private TextControl _description = new TextControl();
        private ListBox _sysList = new ListBox();
        private Button _connect = new Button();
        private Button _disconnect = new Button();
        private HackableSystem _current = null;
        
        public Network()
        {
            Width = 720;
            Height = 480;
            AddControl(_title);
            AddControl(_subtitle);
            AddControl(_description);
            AddControl(_sysList);
            AddControl(_connect);
            AddControl(_disconnect);
            
            _connect.Text = "Connect";
            _connect.AutoSize = true;

            _title.Font = LoadedSkin.HeaderFont;
            _title.AutoSize = true;
            _title.Text = "Network";

            _subtitle.AutoSize = true;
            _subtitle.Text = "Select a Digital Society system below to view information about it.";
            _subtitle.Font = LoadedSkin.Header3Font;

            _sysList.DoubleClick += () =>
            {
                if(_sysList.SelectedItem != null)
                {
                    ProcedurallyGenerateWhatWillBringPlexBack(_sysList.SelectedItem as Hackable);
                }
            };
            _sysList.KeyEvent += (k) =>
            {
                if (k.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    if (_sysList.SelectedItem != null)
                    {
                        ProcedurallyGenerateWhatWillBringPlexBack(_sysList.SelectedItem as Hackable);
                    }
                }
            };
            _connect.Click += () =>
            {
                ListPorts();
            };
        }

        public void ListPorts()
        {
            _state = NetworkGUIState.PortsList;
            int portListStart = _sysList.Y;
            int portHeight = 100;
            for(int i = 0; i < _current.PortsToUnlock.Count; i++)
            {
                var portview = new PortView(_current.PortsToUnlock[i]);
                portview.Width = _sysList.Width;
                portview.X = _sysList.X;
                portview.Y = portListStart + (portHeight * i);
                portview.Height = portHeight;
                AddControl(portview);
            }
        }

        public void OnLoad()
        {
            ListHackables();
        }

        bool doCountdown = false;

        public void ProcedurallyGenerateWhatWillBringPlexBack(Hackable hackable)
        {
            _state = NetworkGUIState.Information;
            Hacking.InitHack(hackable);
            doCountdown = Hacking.CurrentHackable.DoConnectionTimeout;
            Hacking.CurrentHackable.DoConnectionTimeout = false;
            _current = Hacking.CurrentHackable;
        }

        public void ListHackables()
        {
            _state = NetworkGUIState.Listing;
            _sysList.ClearItems();
            foreach(var hackable in Hacking.AvailableToHack.OrderBy(x=>x.FriendlyName))
            {
                _sysList.AddItem(hackable);
            }
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        protected override void OnLayout(GameTime gameTime)
        {
            /*_title.X = 15;
            _title.Y = 15;

            _subtitle.X = 15;
            _subtitle.Y = _title.Y + _title.Height + 7;

            _description.Visible = (_current != null) && _state != NetworkGUIState.Listing;
            _connect.Visible = (_current != null) && _state == NetworkGUIState.Information;
            _disconnect.Visible = (_current != null) && _state == NetworkGUIState.Information;
            _sysList.Visible = _state == NetworkGUIState.Listing;

            if (_sysList.Visible)
            {
                _sysList.X = 15;
                _sysList.Y = _subtitle.Y + _subtitle.Height + 10;
                _sysList.Width = this.Width - 30;
                _sysList.Height = (Height - _sysList.Y - 15);

                _title.Text = "Network";
                _subtitle.Text = "Select a Digital Society system below to view information about it.";

            }
            if (_current != null)
            {
                _title.Text = _current.Data.SystemName;
                _subtitle.Text = _current.Data.FriendlyName;
                if(_state == NetworkGUIState.Information)
                {
                    _description.Visible = true;
                    _description.AutoSize = false;
                    _description.X = _sysList.X;
                    _description.Y = _sysList.Y;
                    _description.Width = _sysList.Width;
                    _description.Height = _sysList.Height - 50;
                    _description.Text = $@"System type: {_current.Data.SystemType}
Loot rarity: {_current.Data.LootRarity}

{_current.Data.Description}";

                    _connect.Visible = true;
                    _connect.X = Width - _connect.Width - 15;
                    _connect.Y = _description.Y + _description.Height + 15;


                }
            }
            base.OnLayout(gameTime);*/
        }

        public class PortView : Control
        {
            private Port _port = null;
            private Button _start = new Button();

            public PortView(Port port)
            {
                _port = port;
                _start.AutoSize = true;
                _start.Text = "Connect";
                _start.Click += () =>
                {
                    ConnectionStarted?.Invoke(port);
                };
                AddControl(_start);
            }

            private bool unlocked = false;

            protected override void OnLayout(GameTime gameTime)
            {
                /*_start.Y = (Height - _start.Height) / 2;
                _start.X = (Width - _start.Width) - 15;
                unlocked = _port.Locks.Count > 0;
                base.OnLayout(gameTime);*/
            }

            protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
            {
                /*gfx.Clear(LoadedSkin.ControlTextColor.ToMonoColor());
                gfx.DrawRectangle(1, 1, Width - 2, Height - 2, LoadedSkin.ControlColor.ToMonoColor());
                gfx.DrawString(_port.Name + " (" + _port.Value + ")", 15, 15, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.Header3Font);
                string _status = "Tier: " + _port.Tier.ToString();
                string lockstatus = (unlocked == true) ? "Unlocked" : $"{_port.Locks.Count} locks";
                _status += " - " + lockstatus;
                gfx.DrawString(_status, 15, 15 + LoadedSkin.Header3Font.Height + 5, LoadedSkin.ControlTextColor.ToMonoColor(), LoadedSkin.MainFont);*/
            }

            public event Action<Port> ConnectionStarted;
        }

        public enum NetworkGUIState
        {
            Listing,
            Information,
            PortsList,
        }
    }
}
