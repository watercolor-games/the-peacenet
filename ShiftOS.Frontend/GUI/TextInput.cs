using System;
using System.Collections.Generic;

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
        private System.Drawing.Font _font = new System.Drawing.Font("Tahoma", 9f);

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index == value)
                    return;
                if(_text.Length == 0)
                {
                    _index = 0;
                    return;
                }
                _index = MathHelper.Clamp(value, 0, _text.Length);
                Invalidate();
            }
        }

        public override void MouseStateChanged()
        {
            if (MouseLeftDown == true)
            {
                UIManager.FocusedControl = this;
                Invalidate();
            }
            base.MouseStateChanged();
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;

                _text = value;
                if(_index >= _text.Length)
                {
                    _index = _text.Length;
                }
                Invalidate();
            }
        }

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
            using(var gfx = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1)))
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
        
        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                gfx.DrawString(Text, (int)(2 - _textDrawOffset), 2, LoadedSkin.ControlTextColor.ToMonoColor(), _font);
            }
            if (IsFocusedControl)
            {
                //draw caret
                gfx.DrawRectangle((int)(caretPos - _textDrawOffset), 2, 2, Height - 4, UIManager.SkinTextures["ControlTextColor"]);
            }
            else
            {
                if(string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(_label))
                {
                    gfx.DrawString(_label, 2, 2, Color.Gray, _font);
                }
            }

        }
    }
}
