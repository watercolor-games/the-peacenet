using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.TextRenderers;
using Plex.Engine.Config;

namespace Peacenet.Applications
{
    public class ScreenScaleSetter : Window
    {
        private Label _title = new Label();
        private Label _description = new Label();
        private SliderBar _scaleSlider = new SliderBar();

        private Button _continue = new Button();
        private Button _cancel = new Button();

        private const float _maxScale = 2;
        private const float _minScale = 0.5F;

        private float _scale = 0;

        [Dependency]
        private GameLoop _plexgate = null;

        [Dependency]
        private ConfigManager _config = null;

        private RenderTarget2D _scalePreview = null;

        private Action<float> _scaleSet = null;

        public ScreenScaleSetter(WindowSystem _winsys, Action<float> scaleSet = null) : base(_winsys)
        {
            _scale = _plexgate.RenderScale;
            AddChild(_title);
            AddChild(_description);
            AddChild(_scaleSlider);
            AddChild(_continue);
            AddChild(_cancel);

            SetWindowStyle(WindowStyle.NoBorder);
            Title = "Scale Settings";

            _title.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            _title.AutoSize = true;
            _title.Text = "GUI Scale Settings";

            _description.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _description.Text = "Adjust the slider below until the rectangle is as small as possible and the text is still readable.";
            _description.AutoSize = true;

            _continue.Text = "Done";
            _cancel.Text = "Cancel";
            _cancel.Click += (o, a) =>
            {
                Close();
            };
            _continue.Click += (o, a) =>
            {
                if(scaleSet!=null)
                {
                    scaleSet(_scale);
                    Close();
                }
                else
                {
                    _config.SetValue("renderScale", _scale);
                    _plexgate.RenderScale = _scale;
                    Close();
                }
            };
            if (scaleSet != null)
                _cancel.Visible = false;

            float scalePercentageStart = _scale - _minScale;
            _scaleSlider.Value = scalePercentageStart / (_maxScale - _minScale);
        }

        public RenderTarget2D GetControlBuffer
        {
            get
            {
                Control ctrl = this;
                while(ctrl != null)
                {
                    if (ctrl.BackBuffer != null)
                        return ctrl.BackBuffer;
                    ctrl = ctrl.Parent;
                }
                return _plexgate.GameRenderTarget;
            }
        }

        public Vector2 GetScaleSize()
        {
            float scaleHeight = GameLoop.BaseRenderHeight / _scale;

            return new Vector2(_plexgate.AspectRatio * scaleHeight, scaleHeight);
        }

        private double _updateTime = 0;

        protected override void OnUpdate(GameTime time)
        {
            _updateTime += time.ElapsedGameTime.TotalSeconds;
            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            Parent.X = 0;
            Parent.Y = 0;

            if (_updateTime >= 0.5)
            {
                _scale = MathHelper.Lerp(_minScale, _maxScale, _scaleSlider.Value);
                _updateTime = 0;
            }

            var scaleSize = GetScaleSize();
            if(_scalePreview == null || _scalePreview.Width != (int)scaleSize.X || _scalePreview.Height != (int)scaleSize.Y)
            {
                if (_scalePreview != null)
                    _scalePreview.Dispose();
                _scalePreview = new RenderTarget2D(_plexgate.GraphicsDevice, (int)scaleSize.X, (int)scaleSize.Y, false, _plexgate.GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.Depth24, 8, RenderTargetUsage.PreserveContents);
            }

            _scaleSlider.Text = $"GUI Scale: {Math.Round((float)GameLoop.BaseRenderHeight/_scalePreview.Height, 2)}x";

            _title.MaxWidth = Width / 2;
            _description.MaxWidth = (int)(Width * 0.75);

            _title.Y = 30;
            _title.X = (Width - _title.Width) / 2;

            _continue.X = (Width - _continue.Width) - 30;
            _continue.Y = (Height - _continue.Height) - 30;

            _cancel.Y = _continue.Y;
            _cancel.X = _continue.X - _cancel.Width - 10;

            _scaleSlider.Width = _description.MaxWidth;
            _scaleSlider.X = (Width - _scaleSlider.Width) / 2;
            _scaleSlider.Y = _continue.Y - _scaleSlider.Height - 25;

            _description.Y = _scaleSlider.Y - _description.Height - 25;
            _description.X = (Width - _description.Width) / 2;


            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            int x = gfx.X;
            int y = gfx.Y;
            int w = gfx.Width;
            int h = gfx.Height;

            
            gfx.SetRenderTarget(_scalePreview);

            int rectWidth = 800;
            int rectHeight = 600;

            gfx.FillRectangle(new Vector2((gfx.Width - rectWidth) / 2, (gfx.Height - rectHeight) / 2), new Vector2(rectWidth, rectHeight), Theme.GetAccentColor().Darken(0.5F));

            var font = Theme.GetFont(Plex.Engine.Themes.TextFontStyle.System);
            var fontColor = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);

            int fontWidth = rectWidth - 60;
            string text = "Welcome to Peacegate OS." +
                "\n" +
                "\n" +
                "Peacegate OS is a Unix-based operating system written for use in The Peacenet. Interactive access is given only to administrative users of The Peacenet, such as those working for the Peace Foundation." +
                "\n" +
                "\n" +
                "Peacegate OS's interactive mode features the Peacegate Desktop Environment, a fork of the famous GNOME 2 desktop environment for Linux.";

            var measure = TextRenderer.MeasureText(text, font, fontWidth, Plex.Engine.TextRenderers.WrapMode.Words);

            float textY = ((gfx.Height - rectHeight) / 2) + ((rectHeight - measure.Y) / 2);
            float textX = ((gfx.Width - rectWidth) / 2) + 30;

            gfx.DrawString(text, new Vector2(textX, textY), fontColor, font, TextAlignment.Center, fontWidth, WrapMode.Words);

            gfx.SetRenderTarget(GetControlBuffer);

            gfx.ScissorRectangle = new Rectangle(x, y, w, h);

            base.OnPaint(time, gfx);

            Theme.DrawControlDarkBG(gfx, 0, 0, Width, _title.Y + _title.Height + 30);
            Theme.DrawControlDarkBG(gfx, 0, _description.Y - 30, Width, Height - (_description.Y - 30));

            gfx.FillRectangle(Vector2.Zero, new Vector2(Width, Height), _scalePreview, Color.White);
        }
    }
}
