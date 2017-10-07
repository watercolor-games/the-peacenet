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
    public class ItemGroup : Control
    {
        private ItemGroupLayout _igLayout = ItemGroupLayout.SkinDefined;
        private int _igInitialGap = 10;
        private int _igGap = 5;

        public ItemGroupLayout ItemGroupLayout
        {
            get
            {
                return _igLayout;
            }
            set
            {
                if (_igLayout == value)
                    return;
                _igLayout = value;
                Invalidate();
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(Microsoft.Xna.Framework.Color.Transparent);
        }

        public int InitialGap
        {
            get
            {
                switch (_igLayout)
                {
                    case ItemGroupLayout.SkinDefined:
                        return SkinEngine.LoadedSkin.ItemGroupInitialGap;
                    default:
                        return _igInitialGap;
                }
            }
            set
            {
                if (_igLayout == ItemGroupLayout.SkinDefined)
                    return;
                if (_igInitialGap == value)
                    return;
                _igInitialGap = value;
                Invalidate();
            }
        }

        public int Gap
        {
            get
            {
                switch (_igLayout)
                {
                    case ItemGroupLayout.SkinDefined:
                        return SkinEngine.LoadedSkin.ItemGroupGap;
                    default:
                        return _igGap;
                }
            }
            set
            {
                if (_igLayout == ItemGroupLayout.SkinDefined)
                    return;
                if (_igGap == value)
                    return;
                _igGap = value;
                Invalidate();
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (AutoSize)
            {
                int _highesty = InitialGap;
                int _xx = InitialGap;
                foreach(var ctrl in Children)
                {
                    _xx += ctrl.Width + Gap;
                    if (_highesty < ctrl.Height + InitialGap + Gap)
                        _highesty = ctrl.Height + InitialGap + Gap;
                }
                Width = _xx;
                Height = _highesty;
            }

            int _x = InitialGap;
            int _y = InitialGap;
            int _maxYForRow = 0;
            foreach (var ctrl in Children)
            {
                if (_x + ctrl.Width + Gap > Width)
                {
                    _x = InitialGap;
                    _y = _maxYForRow;
                    _maxYForRow = 0;
                    if (_maxYForRow < ctrl.Height + Gap)
                        _maxYForRow = ctrl.Height + Gap;
                }
                ctrl.X = _x;
                ctrl.Y = _y;
                ctrl.Dock = DockStyle.None;
                _x += ctrl.Width + Gap;

                if (_maxYForRow < ctrl.Height + Gap)
                    _maxYForRow = ctrl.Height + Gap;

            }
        }
    }

    public enum FlowDirection
    {
        LeftToRight,
        TopDown,
        RightToLeft,
        BottomUp
    }
}
