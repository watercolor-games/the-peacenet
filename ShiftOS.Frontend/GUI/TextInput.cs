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

        private int textInputOffset = 0;
        private int maxCanFit = 5;
        string visibleText = "";
        float caretPos = 2f;

        protected void CalculateVisibleText()
        {
            visibleText = "";
            caretPos = -1f;
            using (var gfx = Graphics.FromImage(new Bitmap(1, 1)))
            {
                for (int i = textInputOffset; i < _text.Length; i++)
                {
                    visibleText += _text[i];
                    var measure = gfx.MeasureString(visibleText, _font);
                    if (measure.Width > Width)
                    {
                        maxCanFit = visibleText.Length;
                        if(_index < textInputOffset)
                        {
                            textInputOffset = MathHelper.Clamp(_index - (maxCanFit / 2), 0, _text.Length - 1);

                        }
                        if(_index > textInputOffset + maxCanFit)
                        {
                            textInputOffset = MathHelper.Clamp(_index + (maxCanFit / 2), 0, _text.Length - 1) - maxCanFit;
                        }
                        break;
                    }
                    Height = (int)measure.Height + 4;
                }
            }
        }

        
        protected override void OnPaint(Graphics gfx)
        {
            gfx.Clear(LoadedSkin.ControlColor);
            gfx.DrawString(visibleText, _font, new SolidBrush(LoadedSkin.ControlTextColor), 2, 2);
            if (IsFocusedControl)
            {
                //Draw caret.
                gfx.FillRectangle(new SolidBrush(LoadedSkin.ControlTextColor), new RectangleF(caretPos, 2, 2, Height - 4));
            }
            gfx.DrawRectangle(new Pen(new SolidBrush(LoadedSkin.ControlTextColor), 1), new Rectangle(0, 0, Width - 1, Height - 1));


        }
    }
}
