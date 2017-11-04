using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class TextInput : TextControl
    {
        private string _label = "Type here!";
        private int _index = 0;
        private bool passwordChar = false;

        public bool PasswordChar
        {
            get
            {
                return passwordChar;
            }
            set
            {
                if (passwordChar == value)
                    return;
                passwordChar = value;
                this.CalculateVisibleText();
                this.Invalidate();
                this.RequireTextRerender();
            }
        }

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
                if(Text.Length == 0)
                {
                    _index = 0;
                    return;
                }
                _index = MathHelper.Clamp(value, 0, Text.Length);
                CalculateVisibleText();
                Invalidate();
            }
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int textY = (Height - Font.Height) / 2;
            int caretHeight = Font.Height;

            string text = Text;
            if (PasswordChar)
                text = "*".Repeat(Text.Length);
            if (!string.IsNullOrWhiteSpace(text))
            {
                gfx.DrawString(text, (int)(2 - TextDrawOffset), textY, Microsoft.Xna.Framework.Color.White, Font, Engine.GUI.TextAlignment.TopLeft);
            }
            if(!IsFocusedControl)
            {
                if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(_label))
                {
                    gfx.DrawString(_label, 2, textY, Color.Gray, Font, Engine.GUI.TextAlignment.TopLeft);
                }
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

        public TextFilter TextFilter { get; set; }

        protected override void OnTextChanged()
        {
            if (_index > Text.Length)
            {
                _index = Text.Length;
            }
            CalculateVisibleText();

            base.OnTextChanged();
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
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Tab)
            {
                Text = Text.Insert(Index, "    ");
                Index += 4;
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Back)
            {
                if (_index > 0)
                {
                    if (TextFilter == TextFilter.Decimal | TextFilter == TextFilter.Integer)
                    {
                        if(string.IsNullOrWhiteSpace(Text.Remove(_index - 1, 1)))
                        {
                            Text = "0";
                            return;
                        }
                        if(Text.Remove(_index - 1, 1).StartsWith("."))
                        {
                            Text = "0" + Text.Remove(_index - 1, 1);
                            return;
                        }
                    }

                    _index--;
                    Text = Text.Remove(_index, 1);
                    
                }

            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Delete)
            {
                if(_index < Text.Length - 1)
                {
                    Text = Text.Remove(_index, 1);
                }
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Right)
                if (_index < Text.Length)
                    _index++;
            if (e.KeyChar != '\0') {
                switch (TextFilter)
                {
                    case TextFilter.Alphabetical:
                        if (!char.IsLetter(e.KeyChar))
                            return;
                        break;
                    case TextFilter.Alphanumeric:
                        if (!char.IsLetterOrDigit(e.KeyChar))
                            return;
                        break;
                    case TextFilter.Integer:
                        if (!char.IsDigit(e.KeyChar))
                            return;
                        break;
                    case TextFilter.Decimal:
                        if ((!char.IsDigit(e.KeyChar) && e.KeyChar != '.') || (e.KeyChar == '.' && Text.Contains(".")))
                            return;
                        break;
                }

                if (e.KeyChar != '\b')
                {
                    if (TextFilter == TextFilter.Integer || TextFilter == TextFilter.Decimal)
                    {
                        if (Text == "0" && e.KeyChar != '.')
                        {
                            Text = e.KeyChar.ToString();
                            return;
                        }
                    }
                    Text = Text.Insert(_index, e.KeyChar.ToString());
                    _index++;
                }
            }
            caretMS = 0;
            CalculateVisibleText();
            Invalidate();
            base.OnKeyEvent(e);
        }
        
        float caretPos = 2f;

        public dynamic Value
        {
            get
            {
                switch (TextFilter)
                {
                    case TextFilter.Decimal:
                        return Convert.ToDouble(Text);
                    case TextFilter.Integer:
                        long value = Convert.ToInt64(Text);
                        if (value <= int.MaxValue)
                            return (int)value;
                        else return value;
                    default:
                        return Text;
                }
            }
        }

        protected void CalculateVisibleText()
        {
            string text = Text;
            if (PasswordChar)
                text = "*".Repeat(Text.Length);
            if (_index < 0)
                _index = 0;
            if (_index > text.Length)
                _index = text.Length;
            string toCaret = text.Substring(0, _index);
            var measure = GraphicsContext.MeasureString(toCaret, Font, Engine.GUI.TextAlignment.TopLeft);
            caretPos = 2 + measure.X;
            if (caretPos - TextDrawOffset < 0)
            {
                TextDrawOffset += (caretPos - TextDrawOffset);
                RequireTextRerender();
            }
            if (caretPos - TextDrawOffset > Width)
            {
                TextDrawOffset -= caretPos - TextDrawOffset;
                RequireTextRerender();
            }

        }
        

        private float TextDrawOffset = 0;

        public bool PaintBackground = true;

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            int borderWidth = 1;
            bool useClassicBorders = true;
            var borderColor = Color.White;
            var bgColor = Color.DarkGray;
            if (PaintBackground == true)
            {
                if (useClassicBorders == true)
                {
                    gfx.DrawRectangle(0, 0, Width, Height, borderColor);
                    gfx.DrawRectangle(borderWidth, borderWidth, Width - (borderWidth * 2), Height - (borderWidth * 2), bgColor);
                }
                else
                {
                    //Material design-esque borders.


                    //First we get the border start y...
                    int bStartY = Height - (Height / 4);
                    //just a quarter of the height above the bottom
                    //draw a rectangle on the left
                    gfx.DrawRectangle(0, bStartY, borderWidth, Height - bStartY, borderColor);
                    //now on the right
                    gfx.DrawRectangle(Width - borderWidth, bStartY, borderWidth, Height - bStartY, borderColor);
                    //now along the bottom
                    gfx.DrawRectangle(borderWidth, Height - borderWidth, Width - (borderWidth*2), borderWidth, borderColor);
                    //done with the border
                    //now we must draw the actual background.
                    //This'll handle the inner BG
                    gfx.DrawRectangle(borderWidth, 0, Width - (borderWidth * 2),Height -  borderWidth, bgColor);
                    //now for the outer edges
                    //draw a rectangle on the left
                    gfx.DrawRectangle(0, 0, borderWidth, bStartY, bgColor);
                    //now on the right
                    gfx.DrawRectangle(Width - borderWidth, 0, borderWidth, bStartY, bgColor);
                    //done.

                }
            }

            OnPaintCaret(gfx, target);

            base.OnPaint(gfx, target);
        }

        protected virtual void OnPaintCaret(GraphicsContext gfx, RenderTarget2D target)
        {
            int textY = (Height - Font.Height) / 2;
            int caretHeight = Font.Height;

            if (IsFocusedControl)
            {
                if (caretMS <= 250)
                {
                    //draw caret
                    gfx.DrawRectangle((int)(caretPos - TextDrawOffset), textY, 2, caretHeight, Color.White);
                }
            }

        }

        private double caretMS = 0;

        private bool _overrideDefaultStyle = false;

        public bool OverrideDefaultStyle
        {
            get
            {
                return _overrideDefaultStyle;
            }
            set
            {
                if (_overrideDefaultStyle == value)
                    return;
                _overrideDefaultStyle = value;

            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            caretMS += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (caretMS >= 500)
                caretMS = 0;
            Invalidate();
            base.OnLayout(gameTime);
        }
    }
    
    public enum TextFilter
    {
        None,
        Alphabetical,
        Alphanumeric,
        Integer,
        Decimal
    }

    public static class StringExtensions
    {
        public static string Repeat(this string str, int len)
        {
            string n = "";
            for (int i = 0; i < len; i++)
                n += str;
            return n;
        }
    }
}
