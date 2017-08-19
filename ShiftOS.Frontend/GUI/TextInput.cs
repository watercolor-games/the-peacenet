using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.GUI
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
                CalculateVisibleText();
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
                CalculateVisibleText();
                Invalidate();
            }
        }

        protected override void OnKeyEvent(KeyEvent e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                return;
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Left)
            {
                if (_index > 0)
                    _index--;
                
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
                if (e.KeyChar == '\b')
                {
                    if (_index > 0)
                    {
                        _text = _text.Remove(_index - 1, 1);
                        _index--;
                    }
                }
                else
                {
                    _text = _text.Insert(_index, e.KeyChar.ToString());
                    _index++;
                }
            }
            caretMS = 0;
            CalculateVisibleText();
            Invalidate();
            base.OnKeyEvent(e);
        }
        
        float caretPos = 2f;

        protected void CalculateVisibleText()
        {
            using(var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                string toCaret = _text.Substring(0, _index);
                var measure = gfx.SmartMeasureString(toCaret, _font);
                caretPos = 2 + measure.Width;
                if(caretPos - _textDrawOffset < 0)
                {
                    _textDrawOffset += (caretPos - _textDrawOffset);
                }
                if(caretPos - _textDrawOffset > Width)
                {
                    _textDrawOffset -= caretPos - _textDrawOffset;
                }

            }
        }

        private float _textDrawOffset = 0;
        
        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);

            int textY = (Height - _font.Height) / 2;
            int caretHeight = _font.Height;
            

            if (!string.IsNullOrWhiteSpace(Text))
            {
                gfx.DrawString(Text, (int)(2 - _textDrawOffset), textY, LoadedSkin.ControlTextColor.ToMonoColor(), _font);
            }
            if (IsFocusedControl)
            {
                if (caretMS <= 250)
                {
                    //draw caret
                    gfx.DrawRectangle((int)(caretPos - _textDrawOffset), textY, 2, caretHeight, UIManager.SkinTextures["ControlTextColor"]);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(_label))
                {
                    gfx.DrawString(_label, 2, textY, Color.Gray, _font);
                }
            }

        }

        private double caretMS = 0;

        protected override void OnLayout(GameTime gameTime)
        {
            caretMS += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (caretMS >= 500)
                caretMS = 0;
            Invalidate();
        }
    }
}
