using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.DesktopUI
{
    public class NotificationBanner : Control
    {
        private float _anim = 0;
        private bool _animReverse = false;
        private double _time = 0;
        private BannerState _bannerState = BannerState.Initial;

        private Label _header = new Label();
        private Label _desc = new Label();

        public NotificationBanner()
        {
            AddChild(_header);
            AddChild(_desc);

            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.System;
            _desc.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;

            _header.AutoSize = true;
            _desc.AutoSize = true;

            Width = 325;
            _header.MaxWidth = Width - 30;
            _desc.MaxWidth = _header.MaxWidth;
        }

        public string Header { get => _header.Text; set => _header.Text = value; }
        public string Description { get => _desc.Text; set => _desc.Text = value; }
        public BannerState BannerState => _bannerState;

        protected override void OnUpdate(GameTime time)
        {
            _header.X = 15;
            _header.Y = 15;
            _desc.X = 15;
            _desc.Y = _header.Y + _header.Height + 4;

            int maxHeight = _desc.Y + _desc.Height + 15;
            Height = (int)MathHelper.Lerp(0, maxHeight, _anim);
            Opacity = _anim;

            if (_anim < 1 && _time < 5)
            {
                _bannerState = BannerState.FadeIn;
                _anim = MathHelper.Clamp(_anim + ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
            }
            else if(_time < 5)
            {
                _bannerState = BannerState.Visible;
                _time = MathHelper.Clamp((float)_time + (float)time.ElapsedGameTime.TotalSeconds, 0, 5);
            }
            else
            {
                if (_anim > 0)
                {
                    _bannerState = BannerState.FadeOut;
                    _anim = MathHelper.Clamp(_anim - ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                }
                else
                {
                    _bannerState = BannerState.Finished;
                }
            }

            base.OnUpdate(time);
        }

    }

    public enum BannerState
    {
        Initial,
        FadeIn,
        Visible,
        FadeOut,
        Finished
    }
}
