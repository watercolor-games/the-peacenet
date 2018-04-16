using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicLua;

namespace Peacenet.Applications
{
    [AppLauncher("Calculator", "Accessories", "Perform simple mathematical operations just by telling your computer to do it.")]
    public class Calculator : Window
    {
        private dynamic lua = new DynamicLua.DynamicLua();

        private TextBox _display = new TextBox();
        private Button[] _buttonList = null;

        private const int _buttonHSpacing = 3;

        private readonly string[] _buttons = new string[] { "1", "2", "3", "(", "/", "4", "5", "6", ")", "*", "7", "8", "9", "^", "+", "0", ".", "=", "-" };

        public Calculator(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Calculator";
            _buttonList = new Button[_buttons.Length];
            foreach(var text in _buttons)
            {
                var button = new Button();
                button.Text = text;
                button.Click += button_Click;
                AddChild(button);
                _buttonList[Array.IndexOf(_buttons, text)] = button;
            }
            AddChild(_display);
            Height = 475;
            Width = 300;
        }

        protected override void OnUpdate(GameTime time)
        {
            _display.X = 15;
            _display.Y = 15;
            _display.Width = Width - 30;

            float buttonY = _display.Y + _display.Height + 10;
            float buttonWidth = _buttonList[0].Width;
            float buttonHeight = _buttonList[0].Height;
            float maxWidth = (buttonWidth * 5) + (_buttonHSpacing * 4);
            float buttonX = (Width - maxWidth) / 2;
            float w = 0;
            foreach(var button in _buttonList)
            {
                button.X = buttonX + w;
                button.Y = buttonY;
                if(w + buttonWidth + _buttonHSpacing >= maxWidth)
                {
                    w = 0;
                    buttonY += buttonHeight + 2;
                }
                else
                {
                    w += buttonWidth + _buttonHSpacing;
                }
            }

            base.OnUpdate(time);
        }

        private double _value = 0;

        public double Value
        {
            get
            {
                return _value;
            }
        }

        public void Calculate()
        {
            //First, grab the display's current text.
            string text = _display.Text;

            //Easter egg: This calculator runs on Lua.
            try
            {
                lua("result = " + text);
                double result = lua.result;
                _value = result;
                _display.Text = result.ToString();
            }
            catch (Exception ex)
            {
                _display.Text = ex.Message;
            }


        }

        public void button_Click(object s, EventArgs a)
        {
            var button = (Button)s;
            if(button.Text == "=")
            {
                Calculate();
            }
            else
            {
                _display.Text += button.Text;
            }
        }

    }

}
