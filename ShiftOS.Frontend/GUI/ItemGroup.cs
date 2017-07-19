using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ShiftOS.Frontend.GUI
{
    public class ItemGroup : Control
    {
        private int _gap = 3;
        private FlowDirection _flowDir = FlowDirection.LeftToRight;
        private int _initialgap = 2;

        protected override void OnLayout(GameTime gameTime)
        {
            if (AutoSize)
            {
                int _highesty = _initialgap;
                int _xx = _initialgap;
                foreach(var ctrl in Children)
                {
                    _xx += ctrl.Width + _gap;
                    if (_highesty < ctrl.Height + _initialgap + _gap)
                        _highesty = ctrl.Height + _initialgap + _gap;
                }
                Width = _xx;
                Height = _highesty;
            }

            int _x = _initialgap;
            int _y = _initialgap;
            int _maxYForRow = 0;
            foreach (var ctrl in Children)
            {
                if (_x + ctrl.Width + _gap > Width)
                {
                    _x = _initialgap;
                    _y = _maxYForRow;
                    _maxYForRow = 0;
                    if (_maxYForRow < ctrl.Height + _gap)
                        _maxYForRow = ctrl.Height + _gap;
                }
                ctrl.X = _x;
                ctrl.Y = _y;
                ctrl.Dock = DockStyle.None;
                _x += ctrl.Width + _gap;

                if (_maxYForRow < ctrl.Height + _gap)
                    _maxYForRow = ctrl.Height + _gap;

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
