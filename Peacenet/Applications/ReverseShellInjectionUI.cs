using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("Payload Injection Test", "Working prototype and test GUI for the 'Payload Injection' minigame.")]
    public class ReverseShellInjectionUI : Window
    {
        private PayloadTunnel2D _tunnel = new PayloadTunnel2D();


        public ReverseShellInjectionUI(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
            Title = "Payload Injection";

            AddChild(_tunnel);
        }

        protected override void OnUpdate(GameTime time)
        {
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            Parent.X = 0;
            Parent.Y = 0;

            _tunnel.Width = Width;
            _tunnel.Height = (Height / 4)*3;
            _tunnel.X = 0;
            _tunnel.Y = (Height - _tunnel.Height) / 2;

            base.OnUpdate(time);
        }
    }

    public class PayloadTunnel2D : Control
    {
        private float _playerY = 0.5f;
        private float _burst = 0;
        private float _speedLineLoc = 0;
        private List<DataPacket> _packets = new List<DataPacket>();

        private class DataPacket
        {
            public DataPacketType Type { get; set; }
            public Vector2 Location { get; set; }
            public float PlayerY { get; set; }


            public float Radius
            {
                get
                {
                    switch(Type)
                    {
                        case DataPacketType.Antivirus:
                            return 24;
                        case DataPacketType.Security:
                            return 8;
                        case DataPacketType.Burst:
                            return 16;
                        default:
                            return 4;
                    }
                }
            }
        }

        private enum DataPacketType
        {
            Security,
            Antivirus,
            Burst
        }

#if DEBUG
        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.S)
            {
                var packet = new DataPacket
                {
                    Type = DataPacketType.Security,
                    Location = new Vector2(0.75F, 0.1F),
                    PlayerY=_playerY
                };
                _packets.Add(packet);
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.A)
            {
                var packet = new DataPacket
                {
                    Type = DataPacketType.Antivirus,
                    Location = new Vector2(0.75F, 0.1F),
                    PlayerY = _playerY
                };
                _packets.Add(packet);
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.B)
            {
                var packet = new DataPacket
                {
                    Type = DataPacketType.Burst,
                    Location = new Vector2(0.75F, 0.1F),
                    PlayerY = _playerY
                };
                _packets.Add(packet);
            }

            base.OnKeyEvent(e);
        }
#endif

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.Clear(Color.Black);

            float playerY = MathHelper.Lerp(0, gfx.Height, _playerY);
            float playerX = MathHelper.Lerp(gfx.Width / 4, gfx.Width / 2, _burst);

            gfx.DrawCircle(new Vector2(playerX, playerY), 32, Color.Yellow);

            //It's time for a speedline miracle masterpiece.
            int speedLineWidth = gfx.Width / 2;
            float speedlineX = MathHelper.Lerp(gfx.Width, -speedLineWidth, _speedLineLoc);

            gfx.DrawRectangle(new Vector2(speedlineX, gfx.Height - (gfx.Height / 8)), new Vector2(speedlineX, 1), Color.White);
            gfx.DrawRectangle(new Vector2(speedlineX, gfx.Height / 8), new Vector2(speedlineX, 1), Color.White);

            foreach (var packet in _packets)
            {
                var loc = new Vector2(MathHelper.Lerp(0, gfx.Width, packet.Location.X), MathHelper.Lerp(0, gfx.Height, packet.Location.Y));
                var radius = packet.Radius;
                var color = Color.Gray;
                switch (packet.Type)
                {
                    case DataPacketType.Security:
                        color = Color.Red;
                        break;
                    case DataPacketType.Burst:
                        color = Color.Blue;
                        break;
                    case DataPacketType.Antivirus:
                        color = Color.Orange;
                        break;
                }

                gfx.DrawCircle(loc, radius, color);
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            float velocity = (((float)time.ElapsedGameTime.TotalSeconds) * (1 + _burst));

            _playerY = MathHelper.Clamp((float)MouseY / Height, 0, 1);

            _speedLineLoc = MathHelper.Clamp(_speedLineLoc + velocity, 0, 1);
            if (_speedLineLoc >= 1)
                _speedLineLoc = 0;

            Vector2 player = new Vector2(MathHelper.Lerp(Width / 4, Width / 2, _burst), MathHelper.Lerp(0, Height, _playerY));
            Rectangle playerRect = new Rectangle((int)player.X - 32, (int)player.Y - 32, 64, 64);

            for(int i = 0; i < _packets.Count;i++)
            {
                var packet = _packets[i];
                switch (packet.Type)
                {
                    case DataPacketType.Security:
                        float vertDistance = (float)Math.Round(packet.PlayerY - packet.Location.Y, 2);
                        float v = 0;
                        if (vertDistance < 0)
                            v = -(float)time.ElapsedGameTime.TotalSeconds;
                        else if(vertDistance > 0)
                            v = (float)time.ElapsedGameTime.TotalSeconds;
                        packet.Location = new Vector2(packet.Location.X - (velocity / 4), packet.Location.Y + v);
                        break;
                }

                //Despawn a data packet if it goes offscreen
                if(packet.Location.X < 0 || packet.Location.X > 1)
                {
                    _packets.RemoveAt(i);
                    i--;
                    continue;
                }
            }



            base.OnUpdate(time);
        }
    }
}
