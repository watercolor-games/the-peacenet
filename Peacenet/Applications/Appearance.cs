using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.Saves;
using Peacenet.CoreUtils;
using Plex.Engine.GraphicsSubsystem;

namespace Peacenet.Applications
{
    /// <summary>
    /// A namespace containing classes pertaining to in-game programs.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc { }

    /// <summary>
    /// A program that allows the player to customize various parameters of the <see cref="Plex.Engine.Themes.Theme"/> and <see cref="DesktopWindow"/> allowing the player to customize the look of the game's user interface.  
    /// </summary>
    [AppLauncher("Appearance settings", "System")]
    public class Appearance : Window
    {
        [Dependency]
        private Plexgate _plexgate = null;

        private Texture2D[] _wallpapers = null;

        private Label _header = new Label();
        private Label _description = new Label();

        private Label _wallpaperHeader = new Label();
        private ScrollView _wallpaperViewer = new ScrollView();

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private OS _os = null;

        private string _activeWallpaper = null;

        private WallpaperGrid _wallpaperGrid = new WallpaperGrid();


        /// <summary>
        /// A <see cref="Control"/> for displaying a grid of <see cref="Texture2D"/> wallpaper thumbnails. Used within the <see cref="Appearance"/> application.  
        /// </summary>
        public class WallpaperGrid : Control
        {
            private int _selectedIndex = -1;

            /// <summary>
            /// Retrieves the currently selected <see cref="Texture2D"/> wallpaper. Returns null if none is selected.  
            /// </summary>
            public Texture2D SelectedTexture
            {
                get
                {
                    if(_selectedIndex != -1)
                    {
                        return ((PictureBox)Children[_selectedIndex]).Texture;
                    }
                    return null;
                }
            }

            /// <summary>
            /// Occurs when a new wallpaper is selected.
            /// </summary>
            public event Action<Texture2D> SelectedTextureChanged;

            /// <inheritdoc/>
            public override void AddChild(Control child)
            {
                if (!(child is PictureBox))
                    throw new Exception("Cannot add controls that are not PictureBoxes to a wallpaper grid.");
                child.Click += Child_Click;
                base.AddChild(child);
            }

            private void Child_Click(object sender, EventArgs e)
            {
                if (_selectedIndex != -1)
                {
                    var child = (PictureBox)Children[_selectedIndex];
                    child.Tint = Color.White;
                }
                _selectedIndex = Array.IndexOf(Children, sender);
                var nchild = (PictureBox)Children[_selectedIndex];
                nchild.Tint = Theme.GetAccentColor().Lighten(1);
                SelectedTextureChanged?.Invoke(nchild.Texture);
                
            }

            /// <inheritdoc/>
            protected override void OnUpdate(GameTime time)
            {
                int x = 5;
                int y = 5;
                int h = 0;
                foreach(var child in Children)
                {
                    ((PictureBox)child).AutoSize = false;
                    child.Width = 160;
                    child.Height = 90;
                    if(x + child.Width + 5 > Width)
                    {
                        x = 5;
                        y += child.Height + 5;
                    }
                    child.X = x;
                    child.Y = y;
                    x += child.Width + 5;
                    h = Math.Max(h, y + child.Height + 5);
                }
                Height = h;
            }

            /// <inheritdoc/>
            public override void RemoveChild(Control child)
            {
                child.Click -= Child_Click;
                if (_selectedIndex != -1)
                {
                    var nchild = (PictureBox)Children[_selectedIndex];
                    nchild.Tint = Color.White;
                }
                _selectedIndex = -1;
                base.RemoveChild(child);
            }

            /// <inheritdoc/>
            protected override void OnPaint(GameTime time, GraphicsContext gfx)
            {
            }
        }

        /// <inheritdoc/>
        public Appearance(WindowSystem _winsys) : base(_winsys)
        {
            _wallpapers = _plexgate.Content.LoadAllIn<Texture2D>("Desktop");
            _activeWallpaper = _save.GetValue("desktop.wallpaper", "DesktopBackgroundImage2");

            AddChild(_header);
            AddChild(_description);
            AddChild(_wallpaperHeader);
            AddChild(_wallpaperViewer);
            Width = 675;
            Height = 700;
            Title = "Appearance Settings";
            foreach (var texture in _wallpapers)
            {
                var pbox = new PictureBox();
                pbox.Texture = texture;
                _wallpaperGrid.AddChild(pbox);
            }
            _wallpaperViewer.AddChild(_wallpaperGrid);
            _wallpaperGrid.SelectedTextureChanged += (texture) =>
            {
                if(texture != null)
                {
                    string name = texture.Name.Remove(0, 8);
                    _save.SetValue("desktop.wallpaper", name);
                    _os.FireWallpaperChanged();
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _header.Text = "Appearance settings";
            _header.AutoSize = true;
            _header.MaxWidth = Width - 30;
            _header.X = 15;
            _header.Y = 15;

            _description.X = 15;
            _description.Y = _header.Y + _header.Height + 5;
            _description.AutoSize = true;
            _description.MaxWidth = Width - 30;
            _description.Text = "Change the appearance of your Peacegate Desktop environment.";

            _wallpaperHeader.X = 15;
            _wallpaperHeader.Y = _description.Y + _description.Height + 30;
            _wallpaperHeader.AutoSize = true;
            _wallpaperHeader.MaxWidth = Width - 30;
            _wallpaperHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _wallpaperHeader.Text = "Wallpaper";

            _wallpaperGrid.Width = Width - 30;
            _wallpaperViewer.X = 15;
            _wallpaperViewer.Y = _wallpaperHeader.Y + _wallpaperHeader.Height + 5;
            _wallpaperViewer.Height = (Height - _wallpaperViewer.Y) - 15;
        }
    }
}
