using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    public class GPLInfo : Window
    {
        private Label _header = new Label();
        private ScrollView _scroller = new ScrollView();
        private Panel _panel = new Panel();
        private Label _license = new Label();
        private Button _close = new Button();

        public GPLInfo(WindowSystem _winsys) : base(_winsys)
        {
            Title = "GNU General Public License v3.0";
            Width = 800;
            Height = 550;

            AddChild(_header);
            AddChild(_scroller);
            AddChild(_close);
            _scroller.AddChild(_panel);
            _panel.AddChild(_license);

            _header.Text = "GNU GPL v3 License Information";
            _header.AutoSize = true;
            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;

            _close.Text = "Close";
            
            _license.AutoSize = true;
            _license.FontStyle = Plex.Engine.Themes.TextFontStyle.Mono;

            SetWindowStyle(WindowStyle.Dialog);

            _license.Text = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "License.txt"));
            _panel.AutoSize = true;

            _close.Click += (o, a) =>
            {
                Close();
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            _header.X = 15;
            _header.Y = 15;
            _header.MaxWidth = Width - 30;

            _close.X = (Width - _close.Width) - 15;
            _close.Y = (Height - _close.Height) - 15;

            _scroller.X = 0;
            _scroller.Y = _header.Y + _header.Height + 10;
            _scroller.Height = (_close.Y - _scroller.Y) - 10;
            _scroller.Width = Width;
            _panel.Width = Width;
            _license.MaxWidth = Width - 30;
            _license.X = (_panel.Width - _license.Width) / 2;
            _license.Y = 15;
            base.OnUpdate(time);
        }
    }
}
