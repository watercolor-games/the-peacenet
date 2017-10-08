using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
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
        public static Objects.Plexnet plexnet = null;

        [ClientMessageHandler("world")]
        public static void World(string content, string ip)
        {
            plexnet = JsonConvert.DeserializeObject<Objects.Plexnet>(content);
            WorldUpdated?.Invoke();
        }

        public static event Action WorldUpdated;

        public TextControl _title = new TextControl();
        public List<Node> _nodes = new List<Node>();

        public TextControl _hoverTitle = new TextControl();
        public TextControl _hoverdesc = new TextControl();

        public const int WorldSize = 500; //NOTE: The world is this size, squared.

        public float WorldOffsetX = 0;
        public float WorldOffsetY = 0;

        public bool Anim_IsMovingOffset = false;
        public float Anim_OffsetValue = 0;
        public float Anim_NewOffsetX = 0;
        public float Anim_NewOffsetY = 0;
        public float Anim_OldOffsetX = 0;
        public float Anim_OldOffsetY = 0;


        public float ZoomLevel = 1;

        private string netName = "";


        public Network()
        {
            Width = 720;
            Height = 480;
            AddControl(_title);
            WorldUpdated += Network_WorldUpdated;
            ServerManager.SendMessage("get_world", "");
        }

        private void Network_WorldUpdated()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            if (plexnet == null)
                return;
            ClearControls();
            AddControl(_title);
            _nodes.Clear();
            AddControl(_hoverTitle);
            AddControl(_hoverdesc);

            if (string.IsNullOrWhiteSpace(netName))
            {
                _title.Text = "The Plexnet";
                foreach(var network in plexnet.Networks)
                {
                    _nodes.Add(new Apps.Node
                    {
                        Tag = network.Name,
                        Type = NodeType.Network,
                        X = network.X,
                        Y = network.Y
                    });
                }
            }
            else
            {
                _title.Text = plexnet.Networks.FirstOrDefault(x => x.Name == netName).FriendlyName;
                foreach (var network in plexnet.Networks.FirstOrDefault(x=>x.Name == netName).Devices)
                {
                    _nodes.Add(new Apps.Node
                    {
                        Tag = network.SystemName,
                        Type = NodeType.System,
                        X = network.X,
                        Y = network.Y
                    });
                }
            }

            foreach (var ctrl in _nodes)
            {
                ctrl.Icon = new PictureBox();
                ctrl.Icon.MouseLeave += () =>
                {
                    _hoverTitle.Visible = false;
                    _hoverdesc.Visible = false;
                };
                ctrl.Icon.Click += () =>
                {
                    if(WorldOffsetX != ctrl.X || WorldOffsetY != ctrl.Y)
                    {
                        Anim_NewOffsetX = ctrl.X;
                        Anim_NewOffsetY = ctrl.Y;
                        Anim_OffsetValue = 0;
                        Anim_IsMovingOffset = true;
                        Anim_OldOffsetX = WorldOffsetX;
                        Anim_OldOffsetY = WorldOffsetY;
                    }
                    else
                    {
                        if(ctrl.Type == NodeType.Network)
                        {
                            netName = ctrl.Tag;
                            ResetUI();
                            WorldOffsetX = 0;
                            WorldOffsetY = 0;
                        }
                        else
                        {
                            ServerManager.SendMessage("get_hackable", netName + "." + ctrl.Tag);
                        }
                    }
                };
                ctrl.Icon.MouseEnter += () =>
                {
                    _hoverTitle.Visible = true;
                    _hoverdesc.Visible = true;
                    switch (ctrl.Type)
                    {
                        case NodeType.Network:
                            var netinfo = plexnet.Networks.FirstOrDefault(x => x.Name == ctrl.Tag);
                            _hoverTitle.Text = netinfo.FriendlyName;
                            _hoverdesc.Text = $@"{ctrl.Type}
{netinfo.Devices.Count} devices

{netinfo.Description}";
                            break;
                        case NodeType.System:
                            var sys = plexnet.Networks.FirstOrDefault(x => x.Name == netName).Devices.FirstOrDefault(x => x.SystemName == ctrl.Tag);
                            _hoverTitle.Text = $"{netName}.{sys.SystemName}";
                            _hoverdesc.Text = $@"{sys.SystemType}";
                            break;
                    }
                    _hoverTitle.X = ctrl.Icon.X + (ctrl.Icon.Width) + 10;
                    _hoverTitle.Y = ctrl.Icon.Y + (ctrl.Icon.Height) + 10;
                };
                AddControl(ctrl.Icon);

            }
        }

        protected override void OnMouseScroll(int value)
        {
            if (value > 0)
                ZoomLevel += (ZoomLevel / 2);
            else if (value < 0)
                ZoomLevel -= (ZoomLevel / 2);
            ZoomLevel = MathHelper.Clamp(ZoomLevel, 0.01F, 2);
            Invalidate();
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if(Anim_OffsetValue == 1.0)
            {
                Anim_IsMovingOffset = false;
            }
            else
            {
                Anim_OffsetValue = MathHelper.Clamp(Anim_OffsetValue + ((float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f), 0, 1);
                WorldOffsetX = MathHelper.Lerp(Anim_OldOffsetX, Anim_NewOffsetX, Anim_OffsetValue);
                WorldOffsetY = MathHelper.Lerp(Anim_OldOffsetY, Anim_NewOffsetY, Anim_OffsetValue);

            }

            _title.X = 15;
            _title.Y = 15;
            _title.AutoSize = true;
            _title.Font = SkinEngine.LoadedSkin.HeaderFont;
            _title.Text = "Network";
            _hoverTitle.AutoSize = true;
            _hoverTitle.Font = SkinEngine.LoadedSkin.Header3Font;
            _hoverTitle.MaxWidth = 300;
            _hoverdesc.AutoSize = true;
            _hoverdesc.X = _hoverTitle.X;
            _hoverdesc.Y = _hoverTitle.Y + _hoverTitle.Height + 5;
            _hoverdesc.MaxWidth = 300;
            _hoverdesc.Font = SkinEngine.LoadedSkin.MainFont;
            foreach(var node in _nodes)
            {
                node.Icon.Width = (int)(32 * ZoomLevel);
                node.Icon.Height = (int)(32 * ZoomLevel);

                node.Icon.X = (int)((ProgressBar.linear((node.X-WorldOffsetX)*ZoomLevel, -WorldSize, WorldSize, 0, Width) - (node.Icon.Width / 2)));
                node.Icon.Y = (int)((ProgressBar.linear((node.Y-WorldOffsetY)*ZoomLevel, -WorldSize, WorldSize, 0, Height) - (node.Icon.Height / 2)));

            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            foreach(var node in _nodes)
            {
                gfx.DrawRectangle(node.Icon.X, node.Icon.Y, node.Icon.Width, node.Icon.Height, Color.Grey);
            }
        }

        public void OnLoad()
        {
            ResetUI();
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
    }

    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PictureBox Icon { get; set; }
        public string Tag { get; set; }
        public NodeType Type { get; set; }
    }

    public enum NodeType
    {
        System,
        Network
    }
}
