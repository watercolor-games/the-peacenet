using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class ScrollBar : Control
    {
        private double _visibleRide = 0;
        private int _animState = 0;

        private int _scrollHeight = -1;
        private int _scrollOffset = 0;
        private int _preferredScrollHeight = 1;

        private Hitbox _scrollNub = new Hitbox();
        private Hitbox _upArrow = new Hitbox();
        private Hitbox _downArrow = new Hitbox();

        public ScrollBar()
        {
            AddChild(_scrollNub);
            AddChild(_upArrow);
            AddChild(_downArrow);
        }

        public int PreferredScrollHeight
        {
            get
            {
                return _preferredScrollHeight;
            }
            set
            {
                if (_preferredScrollHeight == value)
                    return;
                _preferredScrollHeight = value;
                _scrollOffset = 0;
                _animState = 0;
                Invalidate();
            }
        }

        public int ScrollOffset
        {
            get
            {
                return _scrollOffset;
            }
            set
            {
                if (_scrollOffset == value)
                    return;
                _scrollOffset = value;
                _animState = 0;
                Invalidate(true);
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            if(Parent == null)
            {
                Visible = false;
                return;
            }

            Height = Parent.Height;
            Width = Theme.ScrollbarSize;
            Y = 0;
            X = Parent.Width - Width;

            switch (_animState)
            {
                case 0:
                    Opacity = 1;
                    _visibleRide += time.ElapsedGameTime.TotalSeconds;
                    if (_visibleRide >= 2.5)
                    {
                        _visibleRide = 0;
                        _animState++;
                    }
                    break;
                case 1:
                    Opacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (Opacity <= 0)
                    {
                        _animState++;
                    }
                    break;
            }

            _scrollHeight = Math.Max(_preferredScrollHeight, Height);
            _scrollOffset = MathHelper.Clamp(_scrollOffset, 0, _scrollHeight - Height);

            //Position the up and down arrows.
            _upArrow.X = 0;
            _upArrow.Y = 0;
            _upArrow.Width = Width;
            _upArrow.Height = Width;

            _downArrow.X = 0;
            _downArrow.Width = Width;
            _downArrow.Height = Width;
            _downArrow.Y = Height - _downArrow.Height;

            //Grab the percentage of space taken up by the viewport.
            float viewportSpace = Height / _scrollHeight;
            //Grab the percentage at which the scroll offset is.
            float offsetPercentage = _scrollOffset / _scrollHeight;

            //This is where the scroll display area starts (Y axis)
            int startY = _upArrow.Height;
            //And this is the available height.
            int availableHeight = Height - (_upArrow.Height + _downArrow.Height);

            //Get the real height of the viewport area for rendering the display.
            int realHeight = (int)MathHelper.Lerp(0, availableHeight, viewportSpace);
            //Get the location at which the viewport is
            int viewportLocation = (int)MathHelper.Lerp(startY, startY + availableHeight, offsetPercentage);

            //Viewport location becomes location of nub hitbox
            _scrollNub.Y = viewportLocation;
            //Viewport height becomes nub height
            _scrollNub.Height = realHeight;
            //X and Width for scroll nub should be equal to 0 and our width respectively
            _scrollNub.X = 0;
            _scrollNub.Width = Width;

            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            //The theme will render the scroll bar.
            Theme.DrawScrollbar(gfx, _upArrow, _downArrow, _scrollNub);
        }
    }
}
