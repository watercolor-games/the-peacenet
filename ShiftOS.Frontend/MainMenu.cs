using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Objects;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Plex.Frontend.Apps;
using System.Threading;

namespace Plex.Frontend
{
    public class MainMenu : Control
    {
        private TerminalEmulator _terminal = new TerminalEmulator();
        private PictureBox _watercolorgames = new PictureBox();
        private PictureBox _peacenet_welcome = new PictureBox();
        private PictureBox _peacenet_text = new PictureBox();
        private TextControl _pressStart = new TextControl();
        private TextControl _legalBullshit = new TextControl();
        private float _wgFade = 0.0f;
        private int _wgState = -1;
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

        private float _psFade = 0;

        private ConsoleContext _console = null;

        private static Action<ConsoleContext> _gameStarted = null;

        private void SlowWrite(string text)
        {
            for(int i = 0; i < text.Length; i++)
            {
                _console.Write(text[i].ToString());
                Thread.Sleep(75);
            }
        }

        private static readonly string[] kernelbootmsgs = new string[]
        {
            "Finding hardware devices...",
            "done",
            "Primary input devices: standard computer keyboard, mouse",
            "Checking for updates even though there aren't any",
            "Dumping useless pointers to disk...",
            "Error 0x000000EF at 0xFFFFFFFF",
            "Does anybody actually read these?",
            "Making no progress...",
            "VESA video driver loaded into memory.",
            "Initializing splash screen...",
        };

        public MainMenu()
        {
            Plex.Frontend.Apps.TerminalEmulator.LoadFonts();
            AddControl(_terminal);
            _terminal.StartShell((stdout, stdin) =>
            {
                _console = new ConsoleContext(stdout, stdin);
                Thread.Sleep(500);
                _console.SetBold(true);
                _console.SetColors(Objects.ConsoleColor.White, Objects.ConsoleColor.Black);
                _console.WriteLine("Peacegate v9.4.7");
                _console.SetBold(false);
                _console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.White);
                _console.WriteLine("");
                SlowWrite("RAM: 256M OK\r\n");
                SlowWrite("Starting kernel bootstrap...");
                Thread.Sleep(250);
                _console.WriteLine("");
                Thread.Sleep(250);
                _console.WriteLine("");
                foreach(var line in kernelbootmsgs)
                {
                    _console.WriteLine(line);
                    Thread.Sleep(150);
                }
                _wgState = 0; //start splashscreen
                if(_gameStarted == null)
                {
                    _gameStarted = (ctx) =>
                    {
                        if(SaveSystem.IsSandbox == false)
                        {
                            //only display these in single player
                            ctx.WriteLine("Please authenticate with relay to gain a system context.");
                            Thread.Sleep(250);
                            ctx.SetBold(true);
                            ctx.Write("username: ");
                            Thread.Sleep(125);
                            ctx.SetBold(false);
                            ctx.WriteLine("user");
                            Thread.Sleep(250);
                            ctx.SetBold(true);
                            ctx.Write("password: ");
                            ctx.SetBold(false);
                            Thread.Sleep(125);
                            ctx.WriteLine("*********"); //What, you'd think I'd put an actual pwd on Campaign?
                            Thread.Sleep(500);
                            ctx.SetBold(true);
                            ctx.WriteLine("Access Granted: ");
                            ctx.SetBold(false);
                            ctx.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.Red);
                            ctx.WriteLine($"{SaveSystem.GetUsername()}@{SaveSystem.GetSystemName()}");
                            ctx.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.White);
                            Thread.Sleep(2000);
                        }
                        ctx.WriteLine("SYSTEM CONTEXT RECEIVED.");
                        ctx.WriteLine("");
                        ctx.WriteLine("");
                        Thread.Sleep(2000);
                        ctx.WriteLine("Starting Peacegate Desktop.");
                        Thread.Sleep(1000);
                        Engine.Desktop.InvokeOnWorkerThread(() =>
                        {
                            UIManager.ClearTopLevels();
                            Engine.Desktop.CurrentDesktop.Show();
                            Engine.Desktop.CurrentDesktop.SetupDesktop();
                        });
                    };
                    SaveSystem.GameReady += () =>
                    {
                        _gameStarted?.Invoke(_console);
                    };
                }
            });
            AddControl(_watercolorgames);
            AddControl(_peacenet_text);
            AddControl(_peacenet_welcome);
            AddControl(_legalBullshit);
            AddControl(_btnSinglePlayer);
            AddControl(_btnMultiplayer);
            AddControl(_btnOptions);
            AddControl(_animate);

            //AddControl(_tcMain);

            AddControl(_faUser);
            AddControl(_faGroup);
            AddControl(_faSettings);
            AddControl(_pressStart);

            _watercolorgames.Image = UIManager.ContentLoader.Load<Texture2D>("Artwork/Watercolor_Full");
            _watercolorgames.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _peacenet_text.Image = UIManager.ContentLoader.Load<Texture2D>("Artwork/thepeacenet_text");
            _peacenet_text.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _peacenet_welcome.Image = UIManager.ContentLoader.Load<Texture2D>("Artwork/peacenet_welcome");
            _peacenet_welcome.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            _btnSinglePlayer.Click += ()=>
            {
                _console.WriteLine("127.0.0.1:3252");
                _console.WriteLine("Connecting to relay...");
                _wgState++;
                UIManager.StartSPServer();
            };
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


            UIManager.Game._uploadImg.Image = FontAwesome.cloud_upload.ToTexture2D(UIManager.GraphicsDevice);
            UIManager.Game._downloadImg.Image = FontAwesome.cloud_download.ToTexture2D(UIManager.GraphicsDevice);


        }

        protected override void OnLayout(GameTime gameTime)
        {
            _terminal.X = 0;
            _terminal.Y = 0;
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

            if (_wgState >= 8)
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


            _pressStart.Text = "Press ENTER to continue";
            _pressStart.FontStyle = TextControlFontStyle.Header2;
            _pressStart.AutoSize = true;
            _pressStart.X = (Width - _pressStart.Width) / 2;
            _pressStart.Y = _peacenet_text.Y + _peacenet_text.Image.Height + 75;

            _pressStart.Opacity = _psFade;
            _legalBullshit.Opacity = _psFade;

            _legalBullshit.MaxWidth = Width / 3;
            _legalBullshit.AutoSize = true;
            _legalBullshit.Text = "Copyright 2017 Watercolor Games ";
            _legalBullshit.X = (Width - _legalBullshit.Width) / 2;
            _legalBullshit.Y = (Height - _legalBullshit.Height) - 20;
            _legalBullshit.Alignment = Engine.GUI.TextAlignment.Middle;

            switch (_wgState)
            {
                case 0:
                    //Increase logo opacity.
                    _wgFade += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if(_wgFade >= 1.0F)
                    {
                        _console.WriteLine("Kernel boot succeeded.");
                        _console.Write("Peacegate is copyright (c) 2017 ");
                        _console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.Blue);
                        _console.Write("watercolor");
                        _console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.White);
                        _console.WriteLine("games.");
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
                        _console.WriteLine("Connecting to the Peacenet...");
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
                    {
                        _console.WriteLine("Server says...");
                        _console.Write("Welcome to");
                        _wgState++;
                    }
                    break;
                case 4:
                    _pnSlide += gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_pnSlide >= 1.0)
                    {
                        _console.Write(" The ");
                        _console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.Green);
                        _console.SetBold(true);
                        _console.WriteLine("Peacenet.");
                        _console.SetColors(Objects.ConsoleColor.Black, Objects.ConsoleColor.White);
                        _console.SetBold(false);
                        _wgState++;
                        _wgRide = 0.0;
                    }
                    break;
                case 5:
                    _wgRide += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_wgRide >= 0.5F)
                    {
                        _console.WriteLine("Continuing initialization... Press ENTER to continue.");
                        //move to next state
                        _wgState++;
                        _wtSlide = 1.0;
                        _pnSlide = 1.0;
                        _pnSlideFrom = new Vector2(Width - (_peacenet_text.Image.Width) - 15, 15);
                        _pressStart.RequireTextRerender();
                    }

                    break;
                case 6:
                    _psFade = MathHelper.Clamp(_psFade + ((float)gameTime.ElapsedGameTime.TotalSeconds * 2), 0, 1);
                    if (_psFade >= 1)
                        _wgState++;
                    break;
                case 7:
                    UIManager.FocusedControl = this;
                    break;
                case 8:
                    _wtSlide -= gameTime.ElapsedGameTime.TotalSeconds * 2;
                    _pnSlide -= gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if(_pnSlide <= 0.0)
                    {
                        _console.WriteLine("Please input a Peacegate relay server to connect to.");
                        _console.Write("> ");
                        _wgState++;
                        _wgRide = 0.0;
                    }
                    break;
                case 9:
                    _wgRide += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_wgRide >= 1.0)
                        _wgState++;
                    Invalidate();
                    break;
            }

            bool buttonsVisible = (_wgState == 10);
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
            if(_wgState >= 8)
            {
                int _w = (int)MathHelper.Lerp(0, Width, (float)_wgRide);
                gfx.DrawRectangle(0, _peacenet_text.Y + _peacenet_text.Image.Height + 10, _w, 2, new Color(64, 128, 255));
            }
        }

        private bool _enterPressed = false;

        protected override void OnKeyEvent(KeyEvent e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                if (_wgState == 7)
                {
                    if (_enterPressed == false)
                    {
                        _enterPressed = true;
                        _console.WriteLine("This operating system uses cloud save features. Do not switch off your system while the cloud icon is displayed.");
                        UIManager.ShowCloudUpload();
                        Engine.Infobox.Show("Information", "This game uses cloud save features. Do not switch off your system while the cloud icon is displayed.", () =>
                        {
                            _wgState++;
                            UIManager.HideCloudUpload();
                        });
                    }
                }
            base.OnKeyEvent(e);
        }
    }
}
