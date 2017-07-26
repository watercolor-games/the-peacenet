using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GUI;
using ShiftOS.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;

namespace ShiftOS.Frontend
{
    public class MainMenu : GUI.Control
    {
        private TextControl _mainTitle = new TextControl();
        private TextControl _menuTitle = new TextControl();
        private Button _campaign = new Button();
        private Button _sandbox = new Button();

        public MainMenu()
        {
            X = 0;
            Y = 0;
            Width = UIManager.Viewport.Width;
            Height = UIManager.Viewport.Height;

            AddControl(_mainTitle);
            AddControl(_menuTitle);
            AddControl(_campaign);
            AddControl(_sandbox);

            _campaign.Text = "Campaign";
            _campaign.Font = new System.Drawing.Font("Lucida Console", 10F);
            _campaign.Width = 200;
            _campaign.Height = 6 + _campaign.Font.Height;
            _campaign.X = 30;

            _sandbox.Text = "Sandbox";
            _sandbox.Width = 200;
            _sandbox.Height = 6 + _campaign.Font.Height;
            _sandbox.Font = _campaign.Font;
            _sandbox.X = 30;

            _mainTitle.X = 30;
            _mainTitle.Y = 30;
            _mainTitle.Font = new System.Drawing.Font("Lucida Console", 48F);
            _mainTitle.AutoSize = true;
            _mainTitle.Text = "ShiftOS";

            _menuTitle.Text = "Main menu";
            _menuTitle.Font = new System.Drawing.Font(_menuTitle.Font.Name, 16F);
            _menuTitle.X = 30;
            _menuTitle.Y = _mainTitle.Y + _mainTitle.Font.Height + 10;

            _campaign.Y = _menuTitle.Y + _menuTitle.Font.Height + 15;
            _sandbox.Y = _campaign.Y + _campaign.Font.Height + 15;
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

        }

        private string _tipText = "This is some tip text.";
        private float _tipFade = 0.0f;
        private int _tipTime = 0;



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
                    return "Want to hear about the latest ShiftOS news and events? Check out the Community menu!";
                case 2:
                    return "Go follow DevX on Twitter! http://twitter.com/MichaelTheShift";
                case 3:
                    return "Ran out of things to do in-game? Visit the modding forums for some extra user-created content or how to make your own ShiftOS-y things!";
                case 4:
                    return "Tip of advice: Never use a GUI toolkit made in the 90s for utility design to develop a game that simulates an operating system with tools that look like they should be made in a GUI toolkit from the 90s for utility design.";
                case 5:
                    return "Skins are a very extensive and neat way to customize your ShiftOS experience. Why not give them a try?";
                case 6:
                    return "We thought of putting some pong stuff here but ShiftOS is already mostly just playing pong if you don't play this update. Ping pung pong pang.";
                case 7:
                    return "Welcome to the Digital Society. Do you wish to continue?";
                case 8:
                    return "Open-source projects are pretty cool, you can use, modify, copy and redistribute the code without worrying too much about what lawyer you're gonna hire to act on your behalf. That's why ShiftOS is one of them. http://github.com/shiftos-game/ShiftOS";
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

            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Color.Lerp(_redbg, _bluebg, _bglerp));
            gfx.DrawRectangle(0, 0, Width / 4, Height, Color.White * 0.35F);

            var measure = gfx.MeasureString(_tipText, _campaign.Font, (Width / 4) - 30);
            int _height = (Height - (int)measure.Y) - 30;
            gfx.DrawString(_tipText, 30, _height, Color.White * _tipFade, _campaign.Font, (Width / 4) - 30);
        }
    }
}
