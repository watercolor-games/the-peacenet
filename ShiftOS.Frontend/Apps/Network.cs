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
        public TextControl _title = new TextControl();
        public List<Node> _nodes = new List<Node>();

        public TextControl _hoverTitle = new TextControl();
        public TextControl _hoverdesc = new TextControl();

        public const int WorldSize = 1000; //NOTE: The world is this size, squared.

        public float WorldOffsetX = 0;
        public float WorldOffsetY = 0;

        public bool Anim_IsMovingOffset = false;
        public float Anim_OffsetValue = 0;
        public float Anim_NewOffsetX = 0;
        public float Anim_NewOffsetY = 0;
        public float Anim_OldOffsetX = 0;
        public float Anim_OldOffsetY = 0;


        public float ZoomLevel = 1;


        public Network()
        {
            Width = 720;
            Height = 480;
            AddControl(_title);
            _nodes.Add(new Apps.Node
            {
                X = 0,
                Y = 0,
                Hackable = new Hackable
                {
                    Dependencies = "",
                    Description = "This is your own system.",
                    FriendlyName = SaveSystem.CurrentSave.SystemName,
                    OnHackCompleteStoryEvent = "",
                    OnHackFailedStoryEvent = "",
                    SystemName = SaveSystem.CurrentSave.SystemName,
                    SystemType = SystemType.Computer,
                    WelcomeMessage = ""
                },
                 Icon = new PictureBox(),
            });
        }

        public void ResetUI()
        {
            ClearControls();
            AddControl(_title);

            foreach(var hackable in Hacking.Hackables)
            {
                var node = _nodes.FirstOrDefault(x => x.Hackable == hackable);
                if(node == null)
                {
                    _nodes.Add(new Node
                    {
                        X = hackable.X,
                        Y = hackable.Y,
                        Connections = new List<int>(),
                        Hackable = hackable,
                    });
                }
            }
            AddControl(_hoverTitle);
            AddControl(_hoverdesc);

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
                };
                ctrl.Icon.MouseEnter += () =>
                {
                    _hoverTitle.Visible = true;
                    _hoverdesc.Visible = true;
                    _hoverTitle.Text = ctrl.Hackable.SystemName;
                    _hoverdesc.Text = $@"{ctrl.Hackable.SystemType}

{ctrl.Hackable.Description}";

                    _hoverTitle.X = ctrl.Icon.X + (ctrl.Icon.Width / 2) + 10;
                    _hoverTitle.Y = ctrl.Icon.Y + (ctrl.Icon.Height / 2) + 10;

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
                gfx.DrawRectangle(node.Icon.X, node.Icon.Y, node.Icon.Width, node.Icon.Height, Color.White);
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
        public Hackable Hackable { get; set; }
        public PictureBox Icon { get; set; }
        public List<int> Connections { get; set; } 
    }
}
