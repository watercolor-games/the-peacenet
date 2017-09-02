using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
{
    public class ScrollBar : Control
    {
        private int _value = 0;
        private int _max = 0;
        private ScrollbarPosition _pos = ScrollbarPosition.VerticalRight;

        public event Action ScrollValueChanged;

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;
                int height = GetThumbSize();
                _value = MathHelper.Clamp(value, 0, _max - height);
                ScrollValueChanged?.Invoke();
                Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return _max;
            }
            set
            {
                if (_max == value)
                    return;
                _max = Math.Max(0, value);
                int height = GetThumbSize();
                Value = MathHelper.Clamp(Value, 0, _max - height);
                Invalidate();
            }
        }

        public ScrollbarPosition Position
        {
            get
            {
                return _pos;
            }
            set
            {
                if (_pos == value)
                    return;
                _pos = value;
                Invalidate();
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            switch (_pos)
            {
                case ScrollbarPosition.HorizontalBottom:
                    Width = Parent.Width;
                    Height = 24;
                    X = 0;
                    Y = Parent.Height - Height;
                    break;
                case ScrollbarPosition.HorizontalTop:
                    X = 0;
                    Y = 0;
                    Width = Parent.Width;
                    Height = 24;
                    break;
                case ScrollbarPosition.VerticalLeft:
                    Width = 24;
                    Height = Parent.Height;
                    X = 0;
                    Y = 0;
                    break;
                case ScrollbarPosition.VerticalRight:
                    Width = 24;
                    Height = Parent.Height;
                    Y = 0;
                    X = Parent.Width - Width;
                    break;
            }
        }

        public int GetThumbSize()
        {
            switch (_pos)
            {
                case ScrollbarPosition.HorizontalBottom:
                case ScrollbarPosition.HorizontalTop:
                    return (Parent == null) ? Width : Parent.Width;
                default:
                    return (Parent == null) ? Height : Parent.Height;
            }
        }

        public int GetScrollDimension()
        {
            switch (_pos)
            {
                case ScrollbarPosition.HorizontalBottom:
                case ScrollbarPosition.HorizontalTop:
                    return Width;
                default:
                    return Height;
            }
        }


        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.DrawRectangle(0, 0, Width, Height, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, SkinEngine.LoadedSkin.ControlColor.ToMonoColor());
            int scrollheight = (int)ProgressBar.linear(GetThumbSize(), 0, _max, 0, GetScrollDimension());
            int scroll_pos = (int)ProgressBar.linear(_value, 0, _max - GetThumbSize(), 0, GetScrollDimension());
            if(GetScrollDimension() == Width)
            {
                gfx.DrawRectangle(scroll_pos, 0, scroll_pos + scrollheight, Height, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
            }
            else
            {
                gfx.DrawRectangle(0, scroll_pos, Width, scroll_pos + scrollheight, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
            }
        }

    }

    public enum ScrollbarPosition
    {
        VerticalRight,
        VerticalLeft,
        HorizontalTop,
        HorizontalBottom
    }
}
