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
    public class TextInput : TextControl
    {
        private string _label = "Type here!";
        private int _index = 0;

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


            if (!string.IsNullOrWhiteSpace(Text))
            {
                gfx.DrawString(Text, (int)(2 - TextDrawOffset), textY, LoadedSkin.ControlTextColor.ToMonoColor(), Font);
            }
            if(!IsFocusedControl)
            {
                if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(_label))
                {
                    gfx.DrawString(_label, 2, textY, Color.Gray, Font);
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
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Back)
            {
                if (_index > 0)
                {
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
                if (e.KeyChar != '\b')
                {
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

        protected void CalculateVisibleText()
        {
            if (_index < 0)
                _index = 0;
            string toCaret = Text.Substring(0, _index);
            var measure = GraphicsContext.MeasureString(toCaret, Font);
            caretPos = 2 + measure.X;
            if (caretPos - TextDrawOffset < 0)
            {
                TextDrawOffset += (caretPos - TextDrawOffset);
            }
            if (caretPos - TextDrawOffset > Width)
            {
                TextDrawOffset -= caretPos - TextDrawOffset;
            }

        }
        

        private float TextDrawOffset = 0;

        public bool PaintBackground = true;

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            if (PaintBackground == true)
            {
                gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
                gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);
            }
            int textY = (Height - Font.Height) / 2;
            int caretHeight = Font.Height;

            if (IsFocusedControl)
            {
                if (caretMS <= 250)
                {
                    //draw caret
                    gfx.DrawRectangle((int)(caretPos - TextDrawOffset), textY, 2, caretHeight, UIManager.SkinTextures["ControlTextColor"]);
                }
            }

            base.OnPaint(gfx, target);
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
