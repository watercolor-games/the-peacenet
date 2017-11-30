using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;

namespace Peacenet.Applications
{
    public class GameSettings : Window
    {
        private Label _test = null;

        public GameSettings(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            SetWindowStyle(WindowStyle.Dialog);
            Title = "System settings";
            _test = new Label();
            _test.AutoSize = true;
            _test.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _test.Text = "Settings";
            AddChild(_test);
        }

        protected override void OnUpdate(GameTime time)
        {
            _test.X = 15;
            _test.Y = 15;
            base.OnUpdate(time);
        }
    }
}
