using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Frontend.GUI
{
    public class TextControl : Control
    {
        private string _text = "Text Control";
        private TextAlign _textAlign = TextAlign.TopLeft;
        private Font _font = new Font("Tahoma", 9f);

        protected override void OnLayout()
        {
            if (AutoSize)
            {
                using (var bmp = new Bitmap(1, 1))
                {
                    using(var gfx = Graphics.FromImage(bmp))
                    {
                        var measure = gfx.MeasureString(_text, _font);
                        Width = (int)measure.Width;
                        Height = (int)measure.Height;
                    }
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        public TextAlign TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }

        protected override void OnPaint(Graphics gfx)
        {
            var sMeasure = gfx.MeasureString(_text, _font, Width);
            PointF loc = new PointF(2, 2);
            float centerH = (Width - sMeasure.Width) / 2;
            float centerV = (Height - sMeasure.Height) / 2;
            switch (_textAlign)
            {
                case TextAlign.TopCenter:
                    loc.X = centerH;
                    break;
                case TextAlign.TopRight:
                    loc.X = Width - sMeasure.Width;
                    break;
                case TextAlign.MiddleLeft:
                    loc.Y = centerV;
                    break;
                case TextAlign.MiddleCenter:
                    loc.Y = centerV;
                    loc.X = centerH;
                    break;
                case TextAlign.MiddleRight:
                    loc.Y = centerV;
                    loc.X = (Width - sMeasure.Width);
                    break;
                case TextAlign.BottomLeft:
                    loc.Y = (Height - sMeasure.Height);
                    break;
                case TextAlign.BottomCenter:
                    loc.Y = (Height - sMeasure.Height);
                    loc.X = centerH;
                    break;
                case TextAlign.BottomRight:
                    loc.Y = (Height - sMeasure.Height);
                    loc.X = (Width - sMeasure.Width);
                    break;

            }

            gfx.DrawString(_text, _font, new SolidBrush(Engine.SkinEngine.LoadedSkin.ControlTextColor), new RectangleF(loc.X, loc.Y, sMeasure.Width, sMeasure.Height));
        }
    }

    public enum TextAlign
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
