using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.GUI
{
    public class TextInput : Control
    {
        private string _label = "Type here!";
        private string _text = "";
        private int _index = 0;
        private Font _font = new Font("Tahoma", 9f);

        protected override void OnKeyEvent(KeyEvent e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Left)
            {
                if (_index > 0)
                    _index--;
                
            }
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Back)
            {
                if(_index > 0)
                {
                    _text = _text.Remove(_index - 1, 1);
                    _index--;
                }
            }
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Delete)
            {
                if(_index < _text.Length - 1)
                {
                    _text = _text.Remove(_index, 1);
                }
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Right)
                if (_index < _text.Length)
                    _index++;
            if (e.KeyChar != '\0') {
                _text = _text.Insert(_index, e.KeyChar.ToString());
                _index++;
            }
            CalculateVisibleText();
            Invalidate();
            base.OnKeyEvent(e);
        }
        
        float caretPos = 2f;

        protected void CalculateVisibleText()
        {
            using(var gfx = Graphics.FromImage(new Bitmap(1, 1)))
            {
                string toCaret = _text.Substring(0, _index);
                var measure = gfx.MeasureString(toCaret, _font);
                caretPos = 2 + measure.Width;
                while(caretPos - _textDrawOffset < 0)
                {
                    _textDrawOffset -= 0.01f;
                }
                while(caretPos - _textDrawOffset > Width)
                {
                    _textDrawOffset += 0.01f;
                }

            }
        }

        private float _textDrawOffset = 0;
        
        protected override void OnPaint(Graphics gfx)
        {
            gfx.Clear(LoadedSkin.ControlColor);
            gfx.DrawString(_text, _font, new SolidBrush(LoadedSkin.ControlTextColor), 2 - _textDrawOffset, 2);
            if (IsFocusedControl)
            {
                //Draw caret.
                gfx.FillRectangle(new SolidBrush(LoadedSkin.ControlTextColor), new RectangleF(caretPos - _textDrawOffset, 2, 2, Height - 4));
            }
            else
            {
                if (string.IsNullOrEmpty(_text))
                {
                    gfx.DrawString(_label, _font, Brushes.Gray, 2, 2);
                }
            }
            gfx.DrawRectangle(new Pen(new SolidBrush(LoadedSkin.ControlTextColor), 1), new System.Drawing.Rectangle(0, 0, Width - 1, Height - 1));


        }
    }
}
