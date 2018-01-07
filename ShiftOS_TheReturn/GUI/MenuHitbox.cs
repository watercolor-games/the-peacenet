using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// A hitbox that paints the theme's accent color in its area if the mouse is hovering over it.
    /// </summary>
    public class MenuHitbox : Hitbox
    {
        private float _opacityAnim = 0;
        private bool? _lastFocus = null;
        private int _animState = -1;

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            float _lastOpacity = _opacityAnim;
            if (Visible == false)
            {
                _opacityAnim = 0;
            }
            else
            {
                bool focused = ContainsMouse;
                if (focused != _lastFocus)
                {
                    _animState = 0;
                    _lastFocus = focused;
                }
                switch (_animState)
                {
                    case 0:
                        if (_lastFocus == true)
                        {
                            _opacityAnim += (float)time.ElapsedGameTime.TotalSeconds * 8;
                            if (_opacityAnim >= 1)
                            {
                                _animState++;
                            }
                        }
                        else
                        {
                            _opacityAnim -= (float)time.ElapsedGameTime.TotalSeconds * 8;
                            if (_opacityAnim <= 0)
                            {
                                _animState++;
                            }

                        }
                        break;
                }
            }
            if (_opacityAnim != _lastOpacity)
                Invalidate(true);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Theme.GetAccentColor() * _opacityAnim);
        }
    }
}
