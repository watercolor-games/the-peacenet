using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static Peacenet.Applications.Appearance;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Plex.Engine.Saves;
using Plex.Engine.Interfaces;
using Peacenet.PeacegateThemes;

namespace Peacenet.Tutorial
{
    /// <summary>
    /// The user-interface for the Tutorial Setup screen.
    /// </summary>
    public class PeacegateSetup : Window
    {
        private float _welcomeAnim = 0f;
        private float _cornerAnim = 0f;

        [Dependency]
        private OS _os = null;

        private Label _setupTitle = new Label();
        private Label _setupMode = new Label();
        private Button _next = new Button();
        private Button _back = new Button();
        private float _uiAnim = 0;

        private ScrollView _mainView = new ScrollView();

        private int _uiState = 0;
        private int _animState = 0;

        private double _animRide = 0;

        private TutorialBgmEntity _tutorial = null;
        private Label _introHeader = new Label();
        private Label _introText = new Label();

        private Panel _desktopWallpaperPanel = new Panel();
        private Label _desktopHeader = new Label();
        private Label _desktopText = new Label();
        private WallpaperGrid _wallpapers = new WallpaperGrid();

        private Panel _accentPanel = new Panel();
        private Label _accentHeader = new Label();
        private Label _accentText = new Label();
        private ListView _accentColors = new ListView();

        private Panel _setupComplete = new Panel();
        private Label _completeHead = new Label();
        private Label _completeText = new Label();

        private Texture2D _loadedWallpaper = null;

        private Panel _introPanel = new Panel();

        private void _resetUI()
        {
            _mainView.Clear();
            switch(_uiState)
            {
                case 0:
                    _setupMode.Text = "Introduction";
                    _back.Enabled = false;
                    _mainView.AddChild(_introPanel);
                    break;
                case 1:
                    _setupMode.Text = "Personalization";
                    _back.Enabled = true;
                    _mainView.AddChild(_desktopWallpaperPanel);
                    if(_wallpapers.SelectedTexture == null)
                    {
                        _loadedWallpaper = _plexgate.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage2");
                        Invalidate(true);
                    }
                    break;
                case 2:
                    _setupMode.Text = "Personalization";
                    _mainView.AddChild(_accentPanel);
                    _next.Text = "Next";
                    break;
                case 3:
                    _setupMode.Text = "Setup Complete";
                    _mainView.AddChild(_setupComplete);
                    _next.Text = "Finish";
                    break;
                case 4:
                    _setupTitle.Text = "Thanks for choosing Peacegate OS as your Peacenet gateway.";
                    _setupTitle.Alignment = TextAlignment.Center;
                    _setupTitle.MaxWidth = Width / 2;
                    _animState = 7;
                    break;
            }
        }

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        /// <inheritdoc/>
        public PeacegateSetup(WindowSystem _winsys, TutorialBgmEntity tutorial) : base(_winsys)
        {
            _tutorial = tutorial;
            SetWindowStyle(WindowStyle.NoBorder);
            Width = _winsys.Width;
            Height = _winsys.Height;
            AddChild(_setupTitle);
            AddChild(_setupMode);
            AddChild(_back);
            AddChild(_next);

            _back.Text = "Back";
            _next.Text = "Next";
            _setupMode.AutoSize = true;
            _setupMode.Text = "Introduction";

            _introPanel.AddChild(_introHeader);
            _introPanel.AddChild(_introText);
            _introPanel.AutoSize = true;

            _introHeader.AutoSize = true;
            _introText.AutoSize = true;
            _introHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _introHeader.Text = "Welcome to Peacegate OS.";
            _introText.Text = @"Peacegate OS is the gateway to the Peacenet. You will use it to run programs, interact with other members of the network, and perform other tasks. It is your primary user interface.

We will guide you through how to use Peacegate OS and the Peacegate Desktop, but first we must set some things up for first use. This installer program will guide you through the setup process and prepare your new environment for you.

Click 'Next' to get started.";

            _back.Click += (o, a) =>
            {
                if (_animState == 6)
                    return;
                _uiState--;
                _animState = 6;
            };
            _next.Click += (o, a) =>
            {
                if (_animState == 6)
                    return;
                _uiState++;
                _animState = 6;
            };

            var wall1 = new PictureBox();
            var wall2 = new PictureBox();
            wall1.Texture = _plexgate.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage");
            wall2.Texture = _plexgate.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage2");
            _wallpapers.AddChild(wall1);
            _wallpapers.AddChild(wall2);

            _desktopWallpaperPanel.AddChild(_desktopHeader);
            _desktopWallpaperPanel.AddChild(_desktopText);
            _desktopWallpaperPanel.AddChild(_wallpapers);

            _desktopHeader.AutoSize = true;
            _desktopText.AutoSize = true;
            _desktopWallpaperPanel.AutoSize = true;

            _desktopHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _desktopHeader.Text = "Choose a wallpaper";
            _desktopText.Text = "A wallpaper is displayed in the background of your desktop. You will see it when there are no windows open or anywhere where a window is not being displayed. Right now you only have a small selection of wallpapers, but you will unlock more as you explore the Peacenet.";

            _save.SetValue("desktop.wallpaper", "DesktopBackgroundImage2");

            _wallpapers.SelectedTextureChanged += (texture) =>
            {
                _loadedWallpaper = texture;
                string name = texture.Name.Remove(0, 8);
                _save.SetValue("desktop.wallpaper", name);
                Invalidate(true);
            };

            _accentHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _accentHeader.AutoSize = true;
            _accentHeader.Text = "Choose an accent color";
            _accentText.Text = "An accent color defines the general color of your Peacegate user interface. The accent color defines the color of buttons, window borders, the Desktop Panels, and other UI elements. You can change this later in your Terminal.";
            _accentText.AutoSize = true;
            _accentPanel.AutoSize = true;
            _accentColors.Layout = ListViewLayout.List;

            var currentAccent = _save.GetValue("theme.accent", PeacenetAccentColor.Blueberry);
            foreach (var accent in Enum.GetNames(typeof(PeacenetAccentColor)))
            {
                var lvitem = new ListViewItem();
                lvitem.Value = accent.ToString();
                lvitem.Tag = (PeacenetAccentColor)Enum.Parse(typeof(PeacenetAccentColor), accent);
                _accentColors.AddItem(lvitem);
            }
            _accentColors.SelectedIndex = Array.IndexOf(_accentColors.Items, _accentColors.Items.FirstOrDefault(x => (PeacenetAccentColor)x.Tag == currentAccent));
            _accentColors.SelectedIndexChanged += (o, a) =>
            {
                if (_accentColors.SelectedItem == null)
                    return;
                _save.SetValue<PeacenetAccentColor>("theme.accent", (PeacenetAccentColor)_accentColors.SelectedItem.Tag);
                _pn.AccentColor = (PeacenetAccentColor)_accentColors.SelectedItem.Tag;
            };
            _accentPanel.AddChild(_accentColors);
            _accentPanel.AddChild(_accentHeader);
            _accentPanel.AddChild(_accentText);

            _completeHead.Text = "You have successfully completed the Peacegate OS Setup.";
            _completeText.Text = @"That's all the information we need from you for now. Feel free to go back and change what you have set.

You will also be able to change your desktop wallpaper, accent color and other settings at any time in the Peacegate Settings program.

Press 'Finish' to exit Setup and continue system boot. When the system starts up again, you will have to complete the Peacegate GUI Crash Course. Good luck!";
            _completeHead.AutoSize = true;
            _completeHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _completeText.AutoSize = true;
            _setupComplete.AddChild(_completeHead);
            _setupComplete.AddChild(_completeText);

            _setupTitle.Text = "Peacegate OS Setup";

            AddChild(_mainView);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            switch(_animState)
            {
                case 0:
                    _welcomeAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if(_welcomeAnim>=1.0f)
                    {
                        _welcomeAnim = 1;
                        _animState++;
                    }
                    break;
                case 1:
                    _animRide += time.ElapsedGameTime.TotalSeconds;
                    if(_animRide >= 0.5)
                    {
                        _animRide = 0;
                        _animState++;
                    }
                    break;
                case 2:
                    _cornerAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_cornerAnim >= 1.0f)
                    {
                        _cornerAnim = 1;
                        _animState++;
                    }
                    break;
                case 3:
                    _animState++;
                    _resetUI();
                    break;
                case 4:
                    _uiAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    Invalidate(true);
                    if (_uiAnim >= 1.0f)
                    {
                        _uiAnim = 1;
                        _animState++;
                    }
                    break;
                case 6:
                    _uiAnim -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    Invalidate(true);
                    if (_uiAnim <= 0f)
                    {
                        _uiAnim = 0;
                        _animState = 3;
                    }
                    break;
                case 7:
                    _cornerAnim -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_cornerAnim <= 0.0f)
                    {
                        _animRide = 0;
                        _cornerAnim = 0;
                        _animState++;
                    }
                    break;
                case 8:
                    _tutorial.MoveToNextSection();
                    _animState++;
                    break;
                case 9:
                    _animState++;
                    break;
                case 10:
                    _welcomeAnim -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_welcomeAnim <= 0.0f)
                    {
                        _welcomeAnim = 0;
                        _animState++;
                    }
                    break;
                case 11:
                    _plexgate.GetLayer(LayerType.UserInterface).AddEntity((IEntity)_plexgate.Inject(new TutorialInstructionEntity(_tutorial)));
                    _os.PreventStartup = false;
                    Close();
                    break;
            }
            Width = (int)MathHelper.Lerp(WindowSystem.Width - 50, WindowSystem.Width, _welcomeAnim);
            Height = (int)MathHelper.Lerp(WindowSystem.Height - 50, WindowSystem.Height, _welcomeAnim);
            Parent.X = (int)MathHelper.Lerp(25, 0, _welcomeAnim);
            Parent.Y = (int)MathHelper.Lerp(25, 0, _welcomeAnim);

            _setupTitle.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _setupTitle.AutoSize = true;

            //first we calculate where the title should ACTUALLY BE
            var titleLocMax = new Vector2(15, 15);
            var titleLocMin = new Vector2((Width - _setupTitle.Width) / 2, (Height - _setupTitle.Height) / 2);
            var titleLoc = Vector2.Lerp(titleLocMin, titleLocMax, this._cornerAnim);

            //Next, we calculate the proper Y coordinate.
            int titleLocY = (int)MathHelper.Lerp(titleLoc.Y + (Width * 0.25F), titleLoc.Y, _welcomeAnim);
            _setupTitle.X = (int)titleLoc.X;
            _setupTitle.Y = titleLocY;
            _setupTitle.Opacity = _welcomeAnim;

            //Align the setup category title.
            _setupMode.X = _setupMode.X;
            int setupModeYMax = _setupTitle.Y + _setupTitle.Height + 5;
            _setupMode.X = _setupTitle.X;
            _setupMode.Y = (int)MathHelper.Lerp(setupModeYMax + (WindowSystem.Height * 0.25f), setupModeYMax, _uiAnim);
            _setupMode.Opacity = _uiAnim;

            int buttonY = (Height - _next.Height) - 15;
            _next.Y = (int)MathHelper.Lerp(buttonY + (WindowSystem.Height * 0.25F), buttonY, _uiAnim);
            _back.Y = _next.Y;
            _next.Opacity = _uiAnim;
            _back.Opacity = _uiAnim;

            _next.X = (Width - _next.Width) - 15;
            _back.X = (_next.X - _back.Width) - 5;

            _setupMode.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;

            _mainView.X = 0;
            _mainView.Y = _setupMode.Y + _setupMode.Height + 25;
            _mainView.Opacity = _uiAnim;
            _mainView.Height = (_next.Y-15) - _mainView.Y;
            _mainView.MinWidth = Width;
            _mainView.MaxWidth = Width;
            _mainView.Width = Width;

            Opacity = _welcomeAnim;

            switch(_uiState)
            {
                case 0:
                    _introHeader.X = 30;
                    _introHeader.Y = 30;
                    _introHeader.MaxWidth = (_introPanel.Width - 60);
                    _introText.X = 30;
                    _introText.Y = _introHeader.Y + _introHeader.Height + 10;
                    _introText.MaxWidth = _introHeader.MaxWidth;
                    _introPanel.Width = Width;
                    break;
                case 1:
                    _desktopWallpaperPanel.Width = Width;
                    _wallpapers.Width = Width - 30;
                    _desktopHeader.X = 30;
                    _desktopHeader.Y = 30;
                    _desktopText.X = 30;
                    _desktopText.Y = _desktopHeader.Y + _desktopHeader.Height + 10;
                    _desktopText.MaxWidth = Width - 60;
                    _desktopHeader.MaxWidth = _desktopText.MaxWidth;
                    _wallpapers.X = 30;
                    _wallpapers.Y = _desktopText.Y + _desktopText.Height + 30;
                    _wallpapers.MaxWidth = Width - 30;
                    break;
                case 2:
                    _accentText.X = 30;
                    _accentHeader.X = 30;
                    _accentHeader.Y = 30;
                    _accentText.Y = _accentHeader.Y + _accentHeader.Height + 10;
                    _accentHeader.MaxWidth = (Width - 60);
                    _accentText.MaxWidth = _accentHeader.MaxWidth;
                    _accentPanel.Width = Width;
                    _accentColors.Width = _accentHeader.MaxWidth;
                    _accentColors.Y = _accentText.Y + _accentText.Height + 30;
                    _accentColors.X = 30;
                    break;
                case 3:
                    _setupComplete.Width = Width;
                    _setupComplete.AutoSize = true;
                    _completeHead.MaxWidth = Width - 60;
                    _completeText.MaxWidth = _completeHead.MaxWidth;
                    _completeHead.X = 30;
                    _completeText.X = 30;
                    _completeHead.Y = 30;
                    _completeText.Y = _completeHead.Y + _completeHead.Height + 10;
                    break;
            }

            base.OnUpdate(time);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if(_loadedWallpaper != null)
            {
                gfx.Clear(Color.Black);
                gfx.DrawRectangle(0, 0, Width, Height, _loadedWallpaper, Color.White * (MathHelper.Lerp(1, 0.5F, _uiAnim)));
            }
            else
            {
                Theme.DrawControlBG(gfx, 0, 0, Width, Height);
            }
        }
    }
}
