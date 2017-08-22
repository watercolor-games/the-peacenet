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

namespace Plex.Frontend
{
    public class MainMenu : GUI.Control
    {
        private TextControl _mainTitle = new TextControl();
        private TextControl _menuTitle = new TextControl();
        private Button _campaign = new Button();
        private Button _sandbox = new Button();
        private Button _options = new Button();
        private Button _resDown = new Button();
        private Button _resUp = new Button();
        private TextControl _resDisplay = new TextControl();

        private TextControl _bodyTitle = new TextControl();
        private TextControl _bodySubtitle = new TextControl();
        private BBCodeLabel _bodyText = new BBCodeLabel();

        private Button _bloomDown = new Button();
        private Button _bloomUp = new Button();
        private TextControl _bloomDisplay = new TextControl();
        private int _bloomindex = 0;
        private BloomPresets[] blooms = null;

        private Button _optionsSave = new Button();
        private CheckBox _fullscreen = new CheckBox();
        private Button _close = new Button();
        private TextControl _tips = new TextControl();

        public MainMenu()
        {
            X = 0;
            Y = 0;
            Width = UIManager.Viewport.Width;
            Height = UIManager.Viewport.Height;

            int _bodystart = Width / 4;
            AddControl(_bodyTitle);
            AddControl(_bodySubtitle);
            AddControl(_bodyText);
            AddControl(_mainTitle);
            AddControl(_menuTitle);
            AddControl(_campaign);
            AddControl(_sandbox);
            AddControl(_options);
            AddControl(_resUp);
            AddControl(_resDown);
            AddControl(_resDisplay);
            AddControl(_optionsSave);
            AddControl(_fullscreen);
            AddControl(_tips);
            AddControl(_bloomUp);
            AddControl(_bloomDown);
            AddControl(_bloomDisplay);

            _bodyTitle.X = _bodystart + 45;
            _bodyTitle.Y = 45;
            _bodyTitle.Font = SkinEngine.LoadedSkin.HeaderFont;
            _bodyTitle.Height = SkinEngine.LoadedSkin.HeaderFont.Height;
            _bodyTitle.AutoSize = true;
            _bodyTitle.Text = "Loading latest announcement...";

            _bodySubtitle.X = _bodyTitle.X;
            _bodySubtitle.Y = _bodyTitle.Y + _bodyTitle.Height + 15;
            _bodySubtitle.Font = SkinEngine.LoadedSkin.Header2Font;
            _bodySubtitle.Height = _bodySubtitle.Font.Height;
            _bodySubtitle.AutoSize = true;
            _bodySubtitle.Text = "Plex is connecting to servers to load the latest announcement.";

            _bodyText.X = _bodyTitle.X;
            _bodyText.Y = _bodySubtitle.Y + _bodySubtitle.Height + 15;
            _bodyText.Width = ((Width - _bodystart) - 90);
            _bodyText.Height = ((Height - (_bodySubtitle.Y + _bodySubtitle.Height + 15)) - 45);
            _bodyText.Text = @"[b]Please wait while Plex connects to servers.[/b]

If you see this screen for more than a few seconds, you either don't have a working internet connection, the servers are offline, or we haven't set them up yet.

In the case of no internet, connect to the internet and restart the game...or just play offline and not see announcements, it's up to you.

In the case of server downtime or no servers at all, we're working on it. Please be patient.


[u]This is a [b]BBCode[/b] parser[/u]

You can use BBCode with this GUI element. This element can be declared like this ([b]note[/b]: in this case, we're declaring a HUD element):

[code]
var bbcode = new Plex.Frontend.GUI.BBCodeLabel();
Plex.Frontend.UIManager.AddHUD(bbcode);

bbcode.Text = ""[b]Bold[/b] text!"";
[/code]";

            _optionsSave.Text = "Save sentience settings";
            _optionsSave.Width = (Width / 4) - 60;
            _optionsSave.X = 30;
            _optionsSave.Y = 300;
           
            _optionsSave.Height = 6 + _optionsSave.Font.Height;

            _campaign.Text = "Campaign";
            _campaign.Font = new System.Drawing.Font("Lucida Console", 10F);
            _campaign.Width = 200;
            _campaign.Height = 6 + _campaign.Font.Height;
            _campaign.X = 30;

            _optionsSave.Font = _campaign.Font;
            _optionsSave.Height = 6 + _optionsSave.Font.Height;


            _sandbox.Text = "Sandbox";
            _sandbox.Width = 200;
            _sandbox.Height = 6 + _campaign.Font.Height;
            _sandbox.Font = _campaign.Font;
            _sandbox.X = 30;

            _fullscreen.X = 30;
            _fullscreen.Y = _campaign.Y;


            _options.Text = "Options";
            _options.Width = 200;
            _options.Height = 6 + _campaign.Font.Height;
            _options.Font = _campaign.Font;
            _options.X = 30;

            _mainTitle.X = 30;
            _mainTitle.Y = 30;
            _mainTitle.Font = new System.Drawing.Font("Lucida Console", 48F);
            _mainTitle.AutoSize = true;
            _mainTitle.Text = "Plex";

            _menuTitle.Text = "Main menu";
            _menuTitle.Font = new System.Drawing.Font(_menuTitle.Font.Name, 16F);
            _menuTitle.X = 30;
            _menuTitle.Y = _mainTitle.Y + _mainTitle.Font.Height + 10;
            _menuTitle.AutoSize = true;

            _campaign.Y = _menuTitle.Y + _menuTitle.Font.Height + 15;
            _sandbox.Y = _campaign.Y + _campaign.Font.Height + 15;
            _options.Y = _sandbox.Y + _sandbox.Font.Height + 15;
            _campaign.Click += () =>
            {
                SaveSystem.IsSandbox = false;
                SaveSystem.Begin(false);
                Close();
            };
            _sandbox.Click += () =>
            {
                SaveSystem.IsSandbox = true;
                SaveSystem.Begin(false);
                Close();
            };
            _options.Click += () =>
            {
                _menuTitle.Text = "Options";
                ShowOptions();
            };

            _resDown.Text = "<<";
            _resUp.Text = ">>";


            _resDown.Font = _campaign.Font;
            _resDown.Height = _campaign.Height;
            _resDown.Width = 40;
            _resUp.Font = _resDown.Font;
            _resUp.Height = _resDown.Height;
            _resUp.Width = _resDown.Width;
            _resDown.Y = _campaign.Y;
            _resUp.Y = _campaign.Y;
            _resDown.X = 30;
            _resUp.X = ((Width / 4) - _resUp.Width) - 30;
            _resDisplay.X = _resDown.X + _resDown.Width;
            _resDisplay.Width = (_resUp.X - _resDown.X);
            _resDisplay.Y = _resDown.Y;
            _resDisplay.Height = _resDown.Height;
            _resDisplay.TextAlign = TextAlign.MiddleCenter;
            _resDown.Click += () =>
            {
                if (_resIndex > 0)
                {
                    _resIndex--;
                    var res = _screenResolutions[_resIndex];
                    _resDisplay.Text = $"{res.Width}x{res.Height}";
                }
            };
            _resUp.Click += () =>
            {
                if(_resIndex < _screenResolutions.Count - 1)
                {
                    _resIndex++;
                    var res = _screenResolutions[_resIndex];
                    _resDisplay.Text = $"{res.Width}x{res.Height}";

                }
            };
            _bloomDown.Text = "<<";
            _bloomUp.Text = ">>";

            _bloomDown.Font = _campaign.Font;
            _bloomDown.Height = _campaign.Height;
            _bloomDown.Width = 40;
            _bloomUp.Font = _campaign.Font;
            _bloomUp.Height = _campaign.Height;
            _bloomUp.Width = 40;

            _bloomDown.Y = _resDown.Height + _resDown.Y + 5;
            _bloomUp.Y = _bloomDown.Y;
            _bloomDown.X = 30;
            _bloomUp.X = ((Width / 4) - _bloomUp.Width) - 30;
            _bloomDisplay.X = _resDown.X + _resDown.Width;
            _bloomDisplay.Width = (_resUp.X - _resDown.X);
            _bloomDisplay.Y = _bloomDown.Y;
            _bloomDisplay.Height = _resDown.Height;
            _bloomDisplay.TextAlign = TextAlign.MiddleCenter;

            _bloomDown.Click += () =>
            {
                if (_bloomindex > 0)
                {
                    _bloomindex--;
                    var res = blooms[_bloomindex];
                    _bloomDisplay.Text = res.ToString();
                }
            };
            _bloomUp.Click += () =>
            {
                if (_bloomindex < blooms.Length - 1)
                {
                    _bloomindex++;
                    var res = blooms[_bloomindex];
                    _bloomDisplay.Text = res.ToString();
                }
            };

            _fullscreen.CheckedChanged += () =>
            {
                UIManager.Fullscreen = _fullscreen.Checked;
            };

            _optionsSave.Click += () =>
            {

                if (UIManager.ScreenSize != _screenResolutions[_resIndex] || UIManager.BloomPreset != blooms[_bloomindex])
                {

                    Engine.Infobox.PromptYesNo("Confirm sentience edit", "Performing this operation requires your sentience to be re-established which may cause you to go unconscious. Do you wish to continue?", (sleep) =>
                    {
                        if (sleep == true)
                        {
                            SaveOptions();
                            System.Diagnostics.Process.Start("Plex.exe");
                            Environment.Exit(-1);
                        }
                    });
                }
                else
                {
                    SaveOptions();
                    _menuTitle.Text = "Main Menu";
                    _campaign.Visible = true;
                    _close.Visible = true;
                    _sandbox.Visible = true;
                    _options.Visible = true;
                }
            };

            foreach(var mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.OrderBy(x=>x.Width * x.Height))
            {
                    _screenResolutions.Add(new System.Drawing.Size(mode.Width, mode.Height));
                    if (UIManager.ScreenSize == _screenResolutions.Last())
                        _resIndex = _screenResolutions.Count - 1;
            }
            _fullscreen.Y = _bloomDown.Y + _bloomDown.Height + 5;
            List<BloomPresets> presets = new List<BloomPresets>();
            foreach(var e in Enum.GetValues(typeof(BloomPresets)))
            {
                presets.Add((BloomPresets)e);
            }
            presets = presets.OrderBy(x => (int)x).ToList();
            blooms = presets.ToArray();
            _bloomindex = presets.IndexOf(UIManager.BloomPreset);

            _close.X = 30;
            _close.Font = _campaign.Font;
            _close.Y = _options.Y + _options.Font.Height + 15;
            _close.Text = "Close";
            _close.Height = _campaign.Height;
            _close.Width = _campaign.Width;
            AddControl(_close);
            _close.Click += () =>
            {
                SaveSystem.IsSandbox = true;
                PlexCommands.Shutdown();
            };
        }

        public void SaveOptions()
        {
            var uconf = Objects.UserConfig.Get();
            var res = _screenResolutions[_resIndex];
            uconf.ScreenWidth = res.Width;
            uconf.BloomPreset = blooms[_bloomindex];
            uconf.ScreenHeight = res.Height;
            System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(uconf, Newtonsoft.Json.Formatting.Indented));

        }

        public void ShowOptions()
        {
            _campaign.Visible = false;
            _sandbox.Visible = false;
            _options.Visible = false;
            _close.Visible = false;
            var res = _screenResolutions[_resIndex];
            _resDisplay.Text = $"{res.Width}x{res.Height}";

        }

        private string _tipText = "This is some tip text.";
        private float _tipFade = 0.0f;
        private int _tipTime = 0;
        private List<System.Drawing.Size> _screenResolutions = new List<System.Drawing.Size>();
        private int _resIndex = 0;

        public void Close()
        {
            UIManager.StopHandling(this);
        }

        private Color _redbg = new Color(180, 0, 0, 255);
        private Color _bluebg = new Color(0, 0, 180, 255);
        private float _bglerp = 0.0f;
        private int _lerpdir = 1;
        private bool _tipVisible = false;
        int rnd = 0;
        private Random _rnd = new Random();
        private int _tipWidth = 0;
        private int _tipHeight = 0;
        


        public string GetRandomString()
        {
            var r = _rnd.Next(0, 10);
            while(r == rnd)
            {
                r = _rnd.Next(0, 10);
            }
            rnd = r;
            switch (r)
            {
                case 0:
                    return "Sentience choppy? Try adjusting sentience quality settings in Options.";
                case 1:
                    return "Want to hear about the latest Plex news and events? Check out the Community menu!";
                case 2:
                    return "Go follow DevX on Twitter! http://twitter.com/MichaelTheShift";
                case 3:
                    return "Ran out of things to do in-game? Visit the modding forums for some extra user-created content or how to make your own Plex-y things!";
                case 4:
                    return "Tip of advice: Never use a GUI toolkit made in the 90s for utility design to develop a game that simulates an operating system with tools that look like they should be made in a GUI toolkit from the 90s for utility design.";
                case 5:
                    return "Skins are a very extensive and neat way to customize your Plex experience. Why not give them a try?";
                case 6:
                    return "We thought of putting some pong stuff here but Plex is already mostly just playing pong if you don't play this update. Ping pung pong pang.";
                case 7:
                    return "Welcome to the Digital Society. Do you wish to continue?";
                case 8:
                    return "Open-source projects are pretty cool, you can use, modify, copy and redistribute the code without worrying too much about what lawyer you're gonna hire to act on your behalf. That's why Plex is one of them. http://github.com/Plex-game/Plex";
                case 9:
                    return "Sure, you can toggle fullscreen in Options, but you can also use your F11 key to toggle it on and off in-game!";
                default:
                    return "We ran out of things to say.";
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (_lerpdir == 1)
                _bglerp += 0.00075f;
            else
                _bglerp -= 0.00075f;
            if (_bglerp <= 0.0)
                _lerpdir = 1;
            else if (_bglerp >= 1)
                _lerpdir = -1;

            if (_tipVisible == false)
            {


                _tipTime = 0;
                if (_tipFade == 0.0f)
                {
                    _tipText = GetRandomString();
                    var measure = GraphicsContext.MeasureString(_tipText, _campaign.Font, (Width / 4) - 30);
                    _tipWidth = (int)measure.X;
                    _tipHeight = (int)measure.Y;

                }
                _tipFade += 0.01f;
                if(_tipFade >= 1.0f)
                {
                    _tipVisible = true;
                }
            }
            else
            {
                _tipTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if(_tipTime >= 10000)
                {
                    _tipFade -= 0.01f;
                    if(_tipFade <= 0.0f)
                    {
                        _tipVisible = false;
                        _tipText = GetRandomString();
                    }
                }
            }

            _tips.Visible = _tipVisible;
            _tips.Opacity = _tipFade;
            _tips.Text = _tipText;
            _tips.Width = _tipWidth;
            _tips.Height = _tipHeight;
            _tips.X = 30;
            _tips.Y = (Height - _tips.Height) - 30;
            _tips.Font = _campaign.Font;

            _resDown.Visible = (_menuTitle.Text == "Options" && _resIndex > 0);
            _bloomUp.Visible = _resDown.Visible;
            _bloomDown.Visible = _resDown.Visible;
            _bloomDisplay.Visible = _resDown.Visible;

            _resUp.Visible = (_menuTitle.Text == "Options" && _resIndex < _screenResolutions.Count - 1);
            _resDisplay.Visible = _menuTitle.Text == "Options";
            _optionsSave.Visible = _resDisplay.Visible;
            _fullscreen.Visible = _optionsSave.Visible;
            _bloomDisplay.Text = blooms[_bloomindex].ToString();

            if (_fullscreen.Visible)
            {
                _fullscreen.Checked = UIManager.Fullscreen;
            }

            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Color.Lerp(_redbg, _bluebg, _bglerp));
            gfx.DrawRectangle(0, 0, Width / 4, Height, Color.White * 0.35F);

            //var measure = GraphicsContext.MeasureString(_tipText, _campaign.Font, (Width / 4) - 30);
            //int _height = (Height - (int)measure.Y) - 30;
            //gfx.DrawString(_tipText, 30, _height, Color.White * _tipFade, _campaign.Font, (Width / 4) - 30);
        }
    }
}
