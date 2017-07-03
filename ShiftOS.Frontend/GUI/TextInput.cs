using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.GUI
{
    public class TextInput : Control
    {
        private string _label = "Type here!";
        private string _text = "";
        private int _index = 0;

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
            if (e.KeyChar != '\0')
                _text.Insert(_index, e.KeyChar.ToString());
            base.OnKeyEvent(e);
        }

        protected override void OnLayout()
        {
            base.OnLayout();
        }

        private int _textOffset = 0;

        protected override void OnPaint(Graphics gfx)
        {
            gfx.Clear(LoadedSkin.ControlColor);

        }
    }
}
