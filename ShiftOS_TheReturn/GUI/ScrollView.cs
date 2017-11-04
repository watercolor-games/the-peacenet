using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
{
    public class ScrollView : Control
    {
        private int _realHeight = 0;
        private RenderTarget2D _realContents = null;
        private int _scrollY = 0;
        private int _upState = 0;
        private int _downState = 0;
        private int _nubState = 0;

        public int Maximum
        {
            get
            {
                return _realHeight - Height;
            }
        }

        public int Value
        {
            get
            {
                return _scrollY;
            }
            set
            {
                value = MathHelper.Clamp(value, 0, Maximum);
                if (_scrollY == value)
                    return;
                _scrollY = value;
                Invalidate();
            }
        }


        public ScrollView()
        {
            Click += () =>
            {
                //We're in the scroll bar.
                bool inUpArrow = MouseY - _scrollY <= 24;
                bool inDownArrow = MouseY - _scrollY >= Height - 24;
                int _scrollBarX = Width - 24;
                int _arrowSize = 24;
                int nubAreaHeight = Height - (_arrowSize * 2);

                int nubmargin = ((24 - 20) / 2);
                int lerp = (int)ProgressBar.linear(_scrollY, 0, _realHeight, _arrowSize + nubmargin, nubAreaHeight - (nubmargin * 2));
                //...to get the location of the nub.
                //Now we do it again to get the HEIGHT of the nub.
                int nubheight = (int)ProgressBar.linear(Height, 0, _realHeight, _arrowSize + nubmargin, nubAreaHeight - (nubmargin * 2));
                //And as for the X and width, these values are calculated using the skin.
                int nubX = _scrollBarX + nubmargin;
                int nubW = 20;
                bool inNub = (MouseX >= nubX && MouseX <= nubX + nubW && MouseY - _scrollY >= lerp && MouseY - _scrollY <= lerp + nubheight);
                if (inUpArrow)
                {
                    ScrollBy(-16);
                    return;
                }
                if (inDownArrow)
                {
                    ScrollBy(16);
                    return;
                }

            };
        }

        public void RecalculateScrollHeight()
        {
            int h = 0;
            foreach (var ctrl in Children)
            {
                h = Math.Max(h, ctrl.Y + ctrl.Height);
            }
            if (h > 0)
                h += 15;
            _realHeight = Math.Max(Height, h);
            _scrollY = MathHelper.Clamp(_scrollY, 0, _realHeight - Height);
        }

        public void RecalculateRenderTargetHeight(GraphicsContext gfx)
        {
            RecalculateScrollHeight();
            AllocRenderTarget(gfx);
        }

        private void AllocRenderTarget(GraphicsContext gfx)
        {
            if(_realContents != null)
            {
                if(_realContents.Height != _realHeight || _realContents.Width != Width)
                {
                    _realContents.Dispose();
                }
                else
                {
                    return;
                }
            }
            _realContents = new RenderTarget2D(gfx.Device, Math.Max(1, Width), Math.Max(1, _realHeight), false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
        }

        protected override void BeforePaint(GraphicsContext gfx, RenderTarget2D target)
        {
            _oldx = gfx.X;
            _oldy = gfx.Y;
            //recalc render target
            RecalculateRenderTargetHeight(gfx);
            //End the draw call.
            gfx.Batch.End();
            //Switch to scrolled rendertarget
            gfx.Device.SetRenderTarget(_realContents);
            //Begin the draw call
            gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);
            //Clear the ui
            gfx.Clear(Color.Transparent);
            //Update width/heights
            gfx.X = 0;
            gfx.Y = 0;
            gfx.Width = _realContents.Width;
            gfx.Height = _realContents.Height;
        }

        int _oldx = 0;
        int _oldy = 0;

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(Color.DarkGray);
        }

        protected override void AfterPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //End draw call
            gfx.Batch.End();
            //Set rendertarget to the UI one
            gfx.Device.SetRenderTarget(target);
            //Begin draw call
            gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);
            //Update width/heights
            gfx.X = _oldx;
            gfx.Y = _oldy;
            gfx.Width = Width;
            gfx.Height = Height;
            //Render the scrolled target
            gfx.DrawRectangle(0, 0 - _scrollY, Width, _realHeight, _realContents);

            //Don't do anything beyond this point if the real height matches render height
            if(Height == _realHeight)
                return;
            
            //we offset the draw location by X and Y because if we don't the UI is drawn in an incorrect location. This is a quirk with how the UI draws elements.

            //Now, let's draw the scroll bar itself.
            int _scrollBarX = Width - 24;
            int _scrollBarY = 0;
            int _arrowSize = 24;

            //Draw the scrollbar background.
            gfx.DrawRectangle(_scrollBarX, _scrollBarY, _arrowSize, Height, Color.Black);

            //Get the height of the nub area
            int nubAreaHeight = Height - (_arrowSize * 2);
            //In this area we can draw the nub - the button the user drags to scroll.
            //We'll use this value, the scroll Y, and the height of the scrollable rendertarget in a lerp...
            int nubmargin = ((24 - 20) / 2);
            int lerp = (int)ProgressBar.linear(_scrollY, 0, _realHeight, _arrowSize + nubmargin, nubAreaHeight - (nubmargin * 2));
            //...to get the location of the nub.
            //Now we do it again to get the HEIGHT of the nub.
            int nubheight = (int)ProgressBar.linear(Height, 0, _realHeight, _arrowSize+nubmargin, nubAreaHeight-(nubmargin*2));
            //And as for the X and width, these values are calculated using the skin.
            int nubX = _scrollBarX + nubmargin;
            int nubW = 20;
            //Let's draw the nub.
            gfx.DrawRectangle(nubX, lerp, nubW, nubheight, GetNubColor());


            //The arrows will be a lot easier...
            //First the backgrounds.

            var tArrowBG = GetArrowBGColor(_upState);
            var tArrowFG = GetArrowFGColor(_upState);
            var bArrowBG = GetArrowBGColor(_downState);
            var bArrowFG = GetArrowFGColor(_downState);


            //Top:
            gfx.DrawRectangle(_scrollBarX, 0, _arrowSize, _arrowSize, tArrowBG);
            //Bottom
            gfx.DrawRectangle(_scrollBarX, Height - _arrowSize, _arrowSize, _arrowSize, bArrowBG);
        }

        private void SetBarState(int up, int down, int nub)
        {
            if (_upState == up && _downState == down && _nubState == nub)
                return;
            _upState = up;
            _downState = down;
            _nubState = nub;
            Invalidate();

        }

        private Color GetNubColor()
        {
            switch (_nubState)
            {
                case 1:
                    return Color.LightBlue;
                case 2:
                    return Color.DarkBlue;
                default:
                    return Color.DarkGray;

            }
        }

        private Color GetArrowFGColor(int state)
        {
            switch (state)
            {
                case 1:
                    return Color.White;
                case 2:
                    return Color.White;
                default:
                    return Color.LightGray;

            }
        }

        private Color GetArrowBGColor(int state)
        {
            switch (state)
            {
                case 1:
                    return Color.LightBlue;
                case 2:
                    return Color.DarkBlue;
                default:
                    return Color.DarkGray;

            }
        }


        protected override void OnLayout(GameTime gameTime)
        {
            if(MouseX >= Width - 24)
            {
                //We're in the scroll bar.
                bool inUpArrow = MouseY-_scrollY <= 24;
                bool inDownArrow = MouseY-_scrollY >= Height - 24;
                int _scrollBarX = Width - 24;
                int _arrowSize = 24;
                int nubAreaHeight = Height - (_arrowSize * 2);

                int nubmargin = ((24 - 20) / 2);
                int lerp = (int)ProgressBar.linear(_scrollY, 0, _realHeight, _arrowSize + nubmargin, nubAreaHeight - (nubmargin * 2));
                //...to get the location of the nub.
                //Now we do it again to get the HEIGHT of the nub.
                int nubheight = (int)ProgressBar.linear(Height, 0, _realHeight, _arrowSize + nubmargin, nubAreaHeight - (nubmargin * 2));
                //And as for the X and width, these values are calculated using the skin.
                int nubX = _scrollBarX + nubmargin;
                int nubW = 20;
                bool inNub = (MouseX >= nubX && MouseX <= nubX + nubW && MouseY-_scrollY >= lerp && MouseY-_scrollY <= lerp + nubheight);
                if (inUpArrow)
                {
                    SetBarState(MouseLeftDown ? 2 : 1, 0, 0);
                    return;
                }
                if (inDownArrow)
                {
                    SetBarState(0, MouseLeftDown ? 2 : 1, 0);
                    return;
                }
                if (inNub)
                {
                    SetBarState(0, 0, MouseLeftDown ? 2 : 1);
                    return;
                }

            }
            SetBarState(0, 0, 0);
        }

        private void ScrollBy(int amount)
        {
            _scrollY = MathHelper.Clamp(_scrollY + amount, 0, _realHeight - Height);
            Invalidate();

        }

        int scrollLast = 0;
        
        public override bool ProcessMouseState(MouseState state, double lastLeftClickMS, int width = 0, int height = 0)
        {
            //Check if the mouse is in the control
            var coords = PointToLocal(state.Position.X, state.Position.Y);
            if(coords.X >= 0 && coords.Y >= 0 && coords.X <= Width && coords.Y <= Height)
            {
                int scroll = state.ScrollWheelValue;
                int delta = scrollLast - scroll;
                if (delta != 0)
                {
                    ScrollBy(delta);
                    scrollLast = scroll;
                    return true;
                }
                //Account for the scrollable UI
                state = new MouseState(state.X, state.Y + _scrollY, state.ScrollWheelValue, state.LeftButton, state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
                return base.ProcessMouseState(state, lastLeftClickMS, Width, _realHeight);
            }
            return false;
        }
    }

    public static class stuff
    {
        public static System.Drawing.Color ToGdiColor(this Microsoft.Xna.Framework.Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
