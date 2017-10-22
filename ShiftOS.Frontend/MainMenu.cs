using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Objects;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Plex.Frontend
{
    public class MainMenu : GUI.Control
    {
        private PictureBox _watercolorgames = new PictureBox();
        private PictureBox _peacenet_welcome = new PictureBox();
        private PictureBox _peacenet_text = new PictureBox();
        private float _wgFade = 0.0f;
        private int _wgState = 0;
        private double _wgRide = 0.0;

        //Variables for "Welcome to" slide animation.
        private double _wtSlide = 0.0;
        private Vector2 _wtSlideFrom = new Vector2(-999,-999);
        private Vector2 _wtSlideTo = new Vector2(-999, -999);

        //Variables for "thepeacenet" slide animation:
        private double _pnSlide = 0.0;
        private Vector2 _pnSlideFrom = new Vector2(-999, -999);
        private Vector2 _pnSlideTo = new Vector2(-999, -999);

        private Button _btnSinglePlayer = new Button();
        private Button _btnMultiplayer = new Button();
        private Button _btnOptions = new Button();

        private TextControl _tcMain = new TextControl();

        //Watercolor Animator button
        private Button _animate = new Button();

        private PictureBox _faUser = new PictureBox();
        private PictureBox _faGroup = new PictureBox();
        private PictureBox _faSettings = new PictureBox();


        public MainMenu()
        {
            AddControl(_watercolorgames);
            AddControl(_peacenet_text);
            AddControl(_peacenet_welcome);

            AddControl(_btnSinglePlayer);
            AddControl(_btnMultiplayer);
            AddControl(_btnOptions);
            AddControl(_animate);

            AddControl(_tcMain);

            AddControl(_faUser);
            AddControl(_faGroup);
            AddControl(_faSettings);
            
            _watercolorgames.Image = Properties.Resources.Watercolor_Full.ToTexture2D(UIManager.GraphicsDevice);
            _watercolorgames.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _peacenet_text.Image = Properties.Resources.thepeacenet_text.ToTexture2D(UIManager.GraphicsDevice);
            _peacenet_text.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _peacenet_welcome.Image = Properties.Resources.peacenet_welcome.ToTexture2D(UIManager.GraphicsDevice);
            _peacenet_welcome.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _btnSinglePlayer.Click += UIManager.StartSPServer;
            _btnMultiplayer.Click += () =>
            {
                AppearanceManager.SetupDialog(new Apps.MultiplayerServerList());
            };
            _btnOptions.Click += () =>
            {
                Engine.Infobox.Show("Not yet implemented", "Sorry about that... [pats back] It'll come soon.");
            };

            _faUser.Image = FontAwesome.user.ToTexture2D(UIManager.GraphicsDevice);
            _faGroup.Image = FontAwesome.group.ToTexture2D(UIManager.GraphicsDevice);
            _faSettings.Image = FontAwesome.cog.ToTexture2D(UIManager.GraphicsDevice);
            _faUser.AutoSize = true;
            _faGroup.AutoSize = true;
            _faSettings.AutoSize = true;

        }

        protected override void OnLayout(GameTime gameTime)
        {
            Width = UIManager.Viewport.Width;
            Height = UIManager.Viewport.Height;

            _peacenet_welcome.AutoSize = true;
            _peacenet_text.AutoSize = true;

            _wtSlide = MathHelper.Clamp((float)_wtSlide, 0, 1);
            _pnSlide = MathHelper.Clamp((float)_pnSlide, 0, 1);

            

            _watercolorgames.X = 0;
            _watercolorgames.Y = 0;
            _watercolorgames.Width = Width;
            _watercolorgames.Height = Height;

            //set opacity of WG logo for animation
            _watercolorgames.Opacity = MathHelper.Clamp(_wgFade, 0, 1);

            //Animate "Welcome To" text:
            var _wtSlideVector = Vector2.Lerp(_wtSlideFrom, _wtSlideTo, (float)_pnSlide);
            _peacenet_welcome.X = (int)_wtSlideVector.X;
            _peacenet_welcome.Y = (int)_wtSlideVector.Y;

            if(_wgState < 6)
                //animate peacenet text opacity
                _peacenet_text.Opacity = (float)_pnSlide;

            //Animate "thepeacenet":
            var _pnSlideVector = Vector2.Lerp(_pnSlideFrom, _pnSlideTo, (float)_pnSlide);
            _peacenet_text.X = (int)_pnSlideVector.X;
            _peacenet_text.Y = (int)_pnSlideVector.Y;

            if (_wgState >= 7)
                _tcMain.Opacity = (float)_wgRide;
            else
                _tcMain.Opacity = 0;
            _tcMain.AutoSize = true;
            _tcMain.FontStyle = TextControlFontStyle.Custom;
            _tcMain.Font = new System.Drawing.Font("Monda", 20F);
            _tcMain.TextColor = new Color(191, 191, 191);
            _tcMain.X = 15;
            _tcMain.Y = 15;
            _tcMain.Text = "Main menu (UNDER CONSTRUCTION)";


            switch (_wgState)
            {
                case 0:
                    //Increase logo opacity.
                    _wgFade += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if(_wgFade >= 1.0F)
                    {
                        //move to next state.
                        _wgState++;
                    }
                    break;
                case 1:
                    _wgRide += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(_wgRide >= 3.5F)
                    {
                        //move to next state
                        _wgState++;
                    }
                    break;
                case 2:
                    //Increase logo opacity.
                    _wgFade -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_wgFade <= 0F)
                    {
                        //move to next state.
                        _wgState++;
                        //set the slide positions for the next slide
                        int _pny = ((Height - _peacenet_text.Image.Height) / 2);
                        int _pnx = ((Width - _peacenet_text.Image.Width) / 2);
                        int _y = _pny - _peacenet_welcome.Image.Height;
                        _wtSlideFrom = new Vector2(0 - _peacenet_welcome.Image.Width, _y);
                        _wtSlideTo = new Vector2(_pnx, _y);

                        _pnSlideFrom = new Vector2(_pnx, _pny + _peacenet_text.Image.Height + 50);
                        _pnSlideTo = new Vector2(_pnx, _pny);
                    }
                    break;
                case 3:
                    _wtSlide += gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_wtSlide >= 1.0)
                        _wgState++;
                    break;
                case 4:
                    _pnSlide += gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_pnSlide >= 1.0)
                    {
                        _wgState++;
                        _wgRide = 0.0;
                    }
                    break;
                case 5:
                    _wgRide += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_wgRide >= 3.5F)
                    {
                        //move to next state
                        _wgState++;
                        _wtSlideTo = _wtSlideFrom;
                        _wtSlide = 1.0;
                        _pnSlide = 1.0;
                        _pnSlideFrom = new Vector2(Width - (_peacenet_text.Image.Width) - 15, 15);
                    }

                    break;
                case 6:
                    _wtSlide -= gameTime.ElapsedGameTime.TotalSeconds * 2;
                    _pnSlide -= gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if(_pnSlide <= 0.0)
                    {
                        _wgState++;
                        _wgRide = 0.0;
                    }
                    break;
                case 7:
                    _wgRide += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_wgRide >= 1.0)
                        _wgState++;
                    Invalidate();
                    break;
            }

            bool buttonsVisible = (_wgState >= 8);
            _btnSinglePlayer.Visible = buttonsVisible;
            _btnMultiplayer.Visible = buttonsVisible;
            _btnOptions.Visible = buttonsVisible;
            _animate.Visible = buttonsVisible;
            _faUser.Visible = buttonsVisible;
            _faGroup.Visible = buttonsVisible;
            _faSettings.Visible = buttonsVisible;
            if (buttonsVisible)
            {
                //Set button text
                _btnMultiplayer.Text = "Multiplayer";
                _btnSinglePlayer.Text = "Single player";
                _btnOptions.Text = "Settings";


                //Set location of "group" icon
                _faGroup.X = (Width - _faGroup.Width) / 2;
                _faGroup.Y = (Height - _faGroup.Height) / 2;

                _faUser.X = (_faGroup.X - _faUser.Width) - 15;
                _faUser.Y = _faGroup.Y;

                _faSettings.X = (_faGroup.X + _faGroup.Width) + 15;
                _faSettings.Y = _faUser.Y;
                
                //Calculate position of multiplayer




                _btnMultiplayer.X = _faGroup.X + ((_faGroup.Width - _btnMultiplayer.Width) / 2);
                _btnMultiplayer.Y = _faGroup.Y + _faGroup.Height + 10;

                //single player goes to the left of MP
                _btnSinglePlayer.X = _faUser.X + ((_faUser.Width - _btnSinglePlayer.Width) / 2);
                _btnSinglePlayer.Y = _faUser.Y + _faUser.Height + 10;

                //options to the right
                _btnOptions.X = _faSettings.X + ((_faSettings.Width - _btnOptions.Width) / 2);
                _btnOptions.Y = _faSettings.Y + _faSettings.Height + 10;

                _animate.Text = "Watercolor Animator";
                _animate.X = 15;
                _animate.Y = (Height - _animate.Height) - 15;

            }

            base.OnLayout(gameTime);
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Clear(Color.Black);
            if(_wgState >= 7)
            {
                int _w = (int)MathHelper.Lerp(0, Width, (float)_wgRide);
                gfx.DrawRectangle(0, _peacenet_text.Y + _peacenet_text.Image.Height + 10, _w, 2, new Color(64, 128, 255));
            }
        }
    }
}
