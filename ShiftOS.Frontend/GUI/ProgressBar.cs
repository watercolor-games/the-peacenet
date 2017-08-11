using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.GUI
{
    public class ProgressBar : Control
    {
        private int _maximum = 100;
        private int _value = 0;

        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.Clear(LoadedSkin.ProgressBarBackgroundColor.ToMonoColor());
            int w = (int)linear(_value, 0, _maximum, 0, Width);
            gfx.DrawRectangle(0, 0, w, Height, LoadedSkin.ProgressColor.ToMonoColor());
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

    }
}
