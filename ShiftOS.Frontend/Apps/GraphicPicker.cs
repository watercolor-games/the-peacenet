using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GUI;
using Plex.Engine;
using Microsoft.Xna.Framework;
using static Plex.Engine.FSUtils;


namespace Plex.Frontend.Apps
{
    public class GraphicPicker : Control, IPlexWindow
    {
        private TextControl _title = new TextControl();
        private Button _tile = new Button();
        private Button _stretch = new Button();
        private Button _zoom = new Button();
        private Button _center = new Button();

        private Button _browse = new Button();
        private PictureBox _preview = new PictureBox();

        private Button _apply = new Button();
        private Button _cancel = new Button();
        private Button _reset = new Button();

        private System.Drawing.Image _image = null;
        private System.Windows.Forms.ImageLayout _layout = System.Windows.Forms.ImageLayout.None;
        private Action<System.Drawing.Image, byte[], System.Windows.Forms.ImageLayout> _callback = null;
        private string _titleText = "";

        public GraphicPicker(string title, System.Drawing.Image img, System.Windows.Forms.ImageLayout layout, Action<System.Drawing.Image, byte[], System.Windows.Forms.ImageLayout> callback)
        {
            Height = 550;
            Width = 400;

            _titleText = title;
            _image = img;
            _layout = layout;
            _callback = callback;

            AddControl(_title);
            AddControl(_tile);
            AddControl(_stretch);
            AddControl(_zoom);
            AddControl(_center);
            AddControl(_preview);
            AddControl(_browse);
            AddControl(_apply);
            AddControl(_reset);
            AddControl(_cancel);

            _cancel.Click += () =>
            {
                AppearanceManager.Close(this);
            };
            _apply.Click += () =>
            {
                byte[] bytes = (_image == null) ? null : (SkinEngine.ImageToBinary(_image));
                _callback.Invoke(_image, bytes, _layout);
                AppearanceManager.Close(this);
            };
            _reset.Click += () =>
            {
                _image = null;
                _layout = System.Windows.Forms.ImageLayout.Stretch;
                LoadPreview();
            };
            _browse.Click += () =>
            {
                FileSkimmerBackend.GetFile(new[] { ".png", ".bmp", ".jpg", ".jpeg" }, FileOpenerStyle.Open, (file) =>
                 {
                     try
                     {
                         _image = SkinEngine.ImageFromBinary(ReadAllBytes(file));
                         LoadPreview();
                     }
                     catch
                     {
                         Engine.Infobox.Show("Invalid image format", "The file you specified is not a valid picture format.");
                     }
                 });
            };
            _tile.Click += () =>
            {
                _layout = System.Windows.Forms.ImageLayout.Tile;
                LoadPreview();
            };
            _stretch.Click += () =>
            {
                _layout = System.Windows.Forms.ImageLayout.Stretch;
                LoadPreview();
            };
            _zoom.Click += () =>
            {
                _layout = System.Windows.Forms.ImageLayout.Zoom;
                LoadPreview();
            };
            _center.Click += () =>
            {
                _layout = System.Windows.Forms.ImageLayout.Center;
                LoadPreview();
            };

        }

        public void OnLoad()
        {
            _title.Text = _titleText;
            AppearanceManager.SetWindowTitle(this, (string.IsNullOrWhiteSpace(_titleText)) ? "Choose image" : _titleText + " - Choose image");

            _browse.Text = "Browse";
            _tile.Text = "Tile";
            _stretch.Text = "Stretch";
            _zoom.Text = "Zoom";
            _center.Text = "Center";

            _apply.Text = "Apply";
            _reset.Text = "Reset";
            _cancel.Text = "Cancel";

            LoadPreview();
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

        public void LoadPreview()
        {
            if(_preview.Image != null)
            {
                _preview.Image.Dispose();
                _preview.Image = null;
            }
            _preview.ImageLayout = _layout;
            if (_image == null)
                return;
            _preview.Image = _image.ToTexture2D(GraphicsSubsystem.UIManager.GraphicsDevice);
            Invalidate();
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.AutoSize = true;
            _title.Font = SkinEngine.LoadedSkin.HeaderFont;
            _title.MaxWidth = Width - 30;
            _title.Y = 15;
            _title.X = (Width - _title.Width) / 2;

            int width = Width - 30;
            int buttonwidth = (width / 4) - 10;

            int layoutsy = Height / 2;
            _tile.Y = layoutsy;
            _tile.X = 20;
            _tile.Width = buttonwidth;
            _tile.Height = 75;

            _stretch.Y = layoutsy;
            _stretch.X = _tile.X + _tile.Width + 5;
            _stretch.Width = buttonwidth;
            _stretch.Height = _tile.Height;

            _zoom.Y = layoutsy;
            _zoom.X = _stretch.X + _stretch.Width + 5;
            _zoom.Width = buttonwidth;
            _zoom.Height = _stretch.Height;

            _center.Y = layoutsy;
            _center.X = _zoom.X + _zoom.Width + 5;
            _center.Width = buttonwidth;
            _center.Height = _zoom.Height;

            layoutsy += 85;

            _preview.X = 20;
            _preview.Y = layoutsy;
            _preview.Width = buttonwidth;
            _preview.Height = 75;

            _browse.Y = layoutsy;
            _browse.Width = buttonwidth;
            _browse.X = (Width - buttonwidth) - 20;
            _browse.Height = 75;

            _apply.AutoSize = true;
            _cancel.AutoSize = true;
            _reset.AutoSize = true;
            _apply.Y = (Height - _apply.Height) - 15;
            _apply.X = (Width - _apply.Width) - 15;
            _reset.Y = _apply.Y;
            _reset.X = _apply.X - _reset.Width - 5;
            _cancel.Y = _reset.Y;
            _cancel.X = _reset.X - _cancel.Width - 5;

            base.OnLayout(gameTime);
        }
    }
}
