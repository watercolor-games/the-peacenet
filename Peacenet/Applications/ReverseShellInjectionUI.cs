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
            _tunnel.Height = (Height / 4) * 3;
            _tunnel.X = 0;
            _tunnel.Y = (Height - _tunnel.Height) / 2;

            base.OnUpdate(time);
        }
    }

    public class PayloadTunnel2D : Control
    {
        private float _tunnelWidth = 10000;
        private float _tunnelPosition = 0;
        private Vector2 _playerPosition = Vector2.Zero;
        private int _playerRadius = 32;
        private const float _playerSpeed = 250;
        private const float _playerBurstSpeed = 500;

        private float _burstTransition = 0;

        private bool _burst = false;
        private double _burstCooldown = 0;

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            float scroll = -MathHelper.Clamp(_tunnelPosition, 0, _tunnelWidth - gfx.Width);

            gfx.Clear(Color.Black);

            gfx.DrawRectangle(new Vector2(scroll, 0), new Vector2(250, Height), Color.Red);
            gfx.DrawRectangle(new Vector2((_tunnelWidth + scroll)-250, 0), new Vector2(250, Height), Color.Green);

            //Debug.
            //When the player hits these positions, the game will slide.
            float playerWall = (Width / 4);
            float playerWallBurst = (Width / 2);

            float playerWallLerped = MathHelper.Lerp(playerWall, playerWallBurst, _burstTransition);
            gfx.DrawRectangle(new Vector2(playerWallLerped, 0), new Vector2(150, Height), Color.Gray);


            string status = $"Completion: {Math.Round(_tunnelPosition / _tunnelWidth, 2) * 100}% | Bursting: {_burst} | Burst cooldown: {Math.Round(_burstCooldown, 2)}s";

            gfx.Batch.DrawString(Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Mono), status, new Vector2(X,Y), Color.White);

            gfx.DrawCircle(_playerPosition.OffsetX(scroll), _playerRadius, Color.White);
        }

        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.B)
            {
                if(!_burst)
                {
                    if(_burstCooldown==0)
                    {
                        _burst = true;
                        _burstCooldown = 5;
                    }
                }
            }

            base.OnKeyEvent(e);
        }

        protected override void OnUpdate(GameTime time)
        {
            if (!HasFocused)
                Manager.SetFocus(this);

            //When the player hits these positions, the game will slide.
            float playerWall = _tunnelPosition + (Width / 4);
            float playerWallBurst = _tunnelPosition + (Width / 2);

            float playerWallLerped = MathHelper.Lerp(playerWall, playerWallBurst, _burstTransition);

            _playerPosition.Y = MathHelper.Clamp(MouseY, 0, Height);

            float playerSpeed = MathHelper.Lerp(_playerSpeed, _playerBurstSpeed, _burstTransition);
            if (!_burst && _burstTransition > 0)
                playerSpeed = -playerSpeed * 2f;

            var playerVelocity = _playerPosition.GetVelocity(new Vector2(_tunnelWidth, _playerPosition.Y), playerSpeed);
            _playerPosition += playerVelocity * (float)time.ElapsedGameTime.TotalSeconds;


            if(_burst)
            {
                _burstTransition = MathHelper.Clamp(_burstTransition + ((float)time.ElapsedGameTime.TotalSeconds * 2), 0, 1);
                _burstCooldown = MathHelper.Clamp((float)_burstCooldown - (float)time.ElapsedGameTime.TotalSeconds, 0, 5);
                if(_burstCooldown==0)
                {
                    _burst = false;
                    _burstCooldown = 30;
                }
            }
            else
            {
                _burstTransition = MathHelper.Clamp(_burstTransition - ((float)time.ElapsedGameTime.TotalSeconds), 0, 1);
                _burstCooldown = MathHelper.Clamp((float)_burstCooldown - (float)time.ElapsedGameTime.TotalSeconds, 0, 30);
            }

            if(_playerPosition.X >= playerWallLerped)
            {
                var sVector = new Vector2(playerWallLerped, 0);
                var dVector = new Vector2(_tunnelWidth, 0);
                float speed = playerSpeed;
                var vel = sVector.GetVelocity(dVector, speed);
                _tunnelPosition += vel.X * (float)time.ElapsedGameTime.TotalSeconds;
            }

            base.OnUpdate(time);
        }
    }

    public static class VectorMathHelpers
    {
        /// <summary>
        /// Computes the velocity of an object toward a target.
        /// </summary>
        /// <param name="source">The source vector, i.e where a missile was launched.</param>
        /// <param name="destination">The destination vector, i.e the missile's target.</param>
        /// <param name="speed">The speed of the theoretical missile.</param>
        /// <returns>A normalized <see cref="Vector2"></see> containing the velocity of the source object towards its destination, multiplied by the source's speed.</returns>
        public static Vector2 GetVelocity(this Vector2 source, Vector2 destination, float speed)
        {
            //Calculate the distance between the source and destination.
            var distance = destination - source;

            //Now we have the direction in which the source object is travelling.

            //Now we want to normalize it. Thankfully MonoGame can do that for us.
            var normalized = Vector2.Normalize(distance);

            //Now we multiply it by the speed.
            return normalized * speed;
        }

        public static Vector2 OffsetX(this Vector2 vector, float value)
        {
            return new Vector2(vector.X + value, vector.Y);
        }
    }
}
