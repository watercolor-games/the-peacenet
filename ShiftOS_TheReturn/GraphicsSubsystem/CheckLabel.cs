using System;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class CheckLabel : Control
    {
        private TextControl _text = null;
        private CheckBox _check = null;

        public CheckLabel()
        {
            _text = new TextControl();
            _check = new CheckBox();
            _check.Width = 16;
            _check.Height = 16;

            AddControl(_check);
            AddControl(_text);
        }

        public string Text
        {
            get
            {
                return _text.Text;
            }
            set
            {
                _text.Text = value;
            }
        }

        public bool Value
        {
            get
            {
                return _check.Checked;
            }
            set
            {
                _check.Checked = value;
            }
        }

        protected override void OnLayout(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //Make the text left-center aligned.
            _text.Alignment = TextAlignment.Left;

            if (AutoSize)
            {
                _text.AutoSize = true;

                _text.Layout(gameTime);

                this.Height = _text.Height + 4;
                this.Width = 40 + _check.Width + _text.Width;
            }
            else
            {
                //Make sure the width can at least FIT a checkbox.
                Width = Math.Max(Width, _check.Width + 30);
                //Do the same for height.
                Height = Math.Max(Height, _check.Height + 30);

                //Now, get the available text space..
                int textwidth = Width - (30 + _check.Width);

                //and apply to text.
                _text.Width = textwidth;

                //Text height matches our height.
                _text.Height = Height;
            }
            //Position the text on the Y axis
            _text.Y = (Height - _text.Height) / 2;

            //Position checkbox
            _check.X = 15;
            _check.Y = (Height - _check.Height) / 2;

            //Position text next to checkbox.
            _text.X = _check.X + _check.Width + 10;

        }

        protected override void OnPaint(GraphicsContext gfx, Microsoft.Xna.Framework.Graphics.RenderTarget2D target)
        {
        }
    }
}
