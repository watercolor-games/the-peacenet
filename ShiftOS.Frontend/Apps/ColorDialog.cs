using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GUI;
using Plex.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.Apps
{
    public class ColorDialog : Control, IPlexWindow
    {
        private Button _ok = new Button();
        private Button _cancel = new Button();
        private TextControl _alpha = new TextControl();
        private TextInput _alphaBox = new TextInput();
        private TextControl _red = new TextControl();
        private TextInput _redBox = new TextInput();
        private TextControl _green = new TextControl();
        private TextInput _greenBox = new TextInput();
        private TextControl _blue = new TextControl();
        private TextInput _blueBox = new TextInput();

        private System.Drawing.Color _selectedColor = System.Drawing.Color.White;
        private Action<System.Drawing.Color> callback = null;
        private string title = "";

        public ColorDialog(string title, System.Drawing.Color color, Action<System.Drawing.Color> call = null)
        {
            Width = 420;
            Height = 300;

            AddControl(_ok);
            AddControl(_cancel);
            _ok.AutoSize = true;
            _cancel.AutoSize = true;
            _ok.Text = "OK";
            _cancel.Text = "Cancel";
            _selectedColor = color;
            callback = call;
            this.title = title;

            AddControl(_alpha);
            AddControl(_alphaBox);
            AddControl(_red);
            AddControl(_redBox);
            AddControl(_green);
            AddControl(_greenBox);
            AddControl(_blue);
            AddControl(_blueBox);
            _alpha.Text = "Alpha";
            _red.Text = "Red";
            _green.Text = "Green";
            _blue.Text = "Blue";
            _alpha.AutoSize = true;
            _red.AutoSize = true;
            _green.AutoSize = true;
            _blue.AutoSize = true;

            _ok.Click += () =>
            {
                callback?.Invoke(_selectedColor);
                AppearanceManager.Close(this);
            };
            _cancel.Click += () =>
            {
                callback?.Invoke(color);
                AppearanceManager.Close(this);
            };

            _redBox.TextFilter = TextFilter.Integer;
            _greenBox.TextFilter = TextFilter.Integer;
            _blueBox.TextFilter = TextFilter.Integer;
            _alphaBox.TextFilter = TextFilter.Integer;

            _alphaBox.Text = color.A.ToString();
            _redBox.Text = color.R.ToString();
            _greenBox.Text = color.G.ToString();
            _blueBox.Text = color.B.ToString();


            Action _textchanged = () =>
            {
                _alphaBox.Text = MathHelper.Clamp((int)_alphaBox.Value, 0, 255).ToString();
                _redBox.Text = MathHelper.Clamp((int)_redBox.Value, 0, 255).ToString();
                _greenBox.Text = MathHelper.Clamp((int)_greenBox.Value, 0, 255).ToString();
                _blueBox.Text = MathHelper.Clamp((int)_blueBox.Value, 0, 255).ToString();

                try
                {
                    _selectedColor = System.Drawing.Color.FromArgb(_alphaBox.Value, _redBox.Value, _greenBox.Value, _blueBox.Value);
                    Invalidate();
                }
                catch
                {

                }
            };
            _alphaBox.TextChanged += _textchanged;
            _redBox.TextChanged += _textchanged;
            _greenBox.TextChanged += _textchanged;
            _blueBox.TextChanged += _textchanged;

        }

        public void OnLoad()
        {
            AppearanceManager.SetWindowTitle(this, (string.IsNullOrWhiteSpace(title)) ? "Choose Color" : $"{title} - Choose Color");
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _ok.X = (Width - 15) - _ok.Width;
            _ok.Y = Height - _ok.Height - 15;
            _cancel.X = _ok.X - _cancel.Width - 10;
            _cancel.Y = _ok.Y;

            _alpha.X = 15;
            _alpha.Y = 15;
            _alpha.MaxWidth = ((Width - 30) / 2);
            _alphaBox.X = 15;
            _alphaBox.Y = _alpha.Y + _alpha.Height + 5;
            _alphaBox.Height = _alphaBox.Font.Height + 6;
            _alphaBox.Width = _alpha.MaxWidth;

            _red.X = 15;
            _red.Y = _alphaBox.Y + _alphaBox.Height + 15;
            _red.MaxWidth = _alpha.MaxWidth;
            _redBox.X = 15;
            _redBox.Y = _red.Y + _red.Height + 5;
            _redBox.Width = _red.MaxWidth;
            _redBox.Height = _alphaBox.Height;

            _green.X = 15;
            _green.Y = _redBox.Y + _redBox.Height + 15;
            _green.MaxWidth = _alpha.MaxWidth;
            _greenBox.X = 15;
            _greenBox.Y = _green.Y + _green.Height + 5;
            _greenBox.Width = _green.MaxWidth;
            _greenBox.Height = _redBox.Height;

            _blue.X = 15;
            _blue.Y = _greenBox.Y + _greenBox.Height + 15;
            _blue.MaxWidth = _green.MaxWidth;
            _blueBox.X = 15;
            _blueBox.Y = _blue.Y + _blue.Height + 5;
            _blueBox.Width = _greenBox.Width;
            _blueBox.Height = _greenBox.Height;

        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //Calculate available space for color preview

            //first we need the x coord
            int x = _redBox.X + _redBox.Width + 15;
            //now we can use this to get the distance from right
            int rightdist = (Width - x) - 15;
            //we have the width
            gfx.DrawRectangle(x, 15, rightdist, rightdist, _selectedColor.ToMonoColor());
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
