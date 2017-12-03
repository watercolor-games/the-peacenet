using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;

namespace Peacenet.Applications
{
    public class GameSettings : Window
    {
        [Dependency]
        private Plexgate _plexgate = null;


        private Label _test = null;
        private PictureBox _picture = new PictureBox();

        public GameSettings(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            SetWindowStyle(WindowStyle.Dialog);
            Title = "Game settings";

            _test = new Label();
            _test.AutoSize = true;
            _test.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _test.Text = "This text is 50% translucent.";
            _test.Opacity = 0.5F;
            _picture.AutoSize = true;
            _picture.Texture = _plexgate.Content.Load<Texture2D>("Splash/Peacenet");

            for(int i = 0; i < 15; i++)
            {
                var lbl = new Label();
                lbl.Text = $"Label {i+1}";
                lbl.AutoSize = true;
                lbl.X = 15 * i;
                lbl.Y = 30 * i;
                AddChild(lbl);
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            _test.X = 15;
            _test.Y = 15;
            _picture.X = 15;
            _picture.Y = _test.Y + _test.Height + 5;
            base.OnUpdate(time);
        }
    }
}
