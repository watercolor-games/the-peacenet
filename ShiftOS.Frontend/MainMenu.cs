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

        public void Close()
        {
            UIManager.StopHandling(this);
        }

        private Color _redbg = new Color(127, 0, 0, 255);
        private Color _bluebg = new Color(0, 0, 127, 255);
        private float _bglerp = 0.0f;
        private int _lerpdir = 1;

        protected override void OnLayout(GameTime gameTime)
        {
            if (_lerpdir == 1)
                _bglerp += 0.0001f;
            else
                _bglerp -= 0.001f;
            if (_bglerp <= 0.0)
                _lerpdir = 1;
            else if (_bglerp >= 1)
                _lerpdir = -1;
            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, Color.Lerp(_redbg, _bluebg, _bglerp));
            
        }
    }
}
