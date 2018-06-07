using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("Clock", "Accessories", "Find out what time it is.")]
    public class Clock : Window
    {
        private Rectangle _clockRectangle = Rectangle.Empty;
        private float _secondsDegrees = 45;
        private float _minutesDegrees = 90;
        private float _hoursDegrees = 0;

        private Label _head = new Label();
        private Label _currentTime = new Label();


        public Clock(WindowSystem _winsys) : base(_winsys)
        {
            Width = 500;
            Height = 250;
            Title = "Clock";

            AddChild(_head);
            AddChild(_currentTime);
        }

        private Vector2 getPointOnCircle(Vector2 center, float radius, float theta)
        {
            float x = center.X + radius * (float)Math.Cos(theta * Math.PI / 180);
            float y = center.Y + radius * (float)Math.Sin(theta * Math.PI / 180);
            return new Vector2(x, y);
        }

        protected override void OnUpdate(GameTime time)
        {
            _clockRectangle = new Rectangle(60, 60, (Width / 2) - 120, Height - 120);

            _head.AutoSize = true;
            _currentTime.AutoSize = true;
            _head.MaxWidth = (Width - (_clockRectangle.X + _clockRectangle.Width)) - 120;
            _currentTime.MaxWidth = _head.MaxWidth;

            _head.FontStyle = Plex.Engine.Themes.TextFontStyle.System;
            _currentTime.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;

            _head.Text = "Current time:";
            _currentTime.Text = DateTime.Now.ToLongTimeString();

            _head.X = _clockRectangle.X + _clockRectangle.Width + 60;
            _currentTime.X = _head.X;

            _head.Y = (Height - (_head.Height + 5 + _currentTime.Height)) / 2;
            _currentTime.Y = _head.Y + _head.Height + 5;

            float secondsPercent = (float)DateTime.Now.TimeOfDay.Seconds / 60;
            float minutesPercent = (float)DateTime.Now.TimeOfDay.Minutes / 60;

            float hours12 = (DateTime.Now.TimeOfDay.Hours > 12) ? DateTime.Now.TimeOfDay.Hours - 12 : DateTime.Now.TimeOfDay.Hours;
            float hoursPercent = hours12 / 12;

            _secondsDegrees = 360 * secondsPercent;
            _minutesDegrees = 360 * minutesPercent;
            _hoursDegrees = 360 * hoursPercent;



            
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            base.OnPaint(time, gfx);

            var accent = Theme.GetAccentColor();

            gfx.FillCircle(_clockRectangle.Center.ToVector2(), (_clockRectangle.Width / 2), Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System));
            gfx.FillCircle(_clockRectangle.Center.ToVector2(), ((_clockRectangle.Width / 2) - 4), Theme.ControlBG);
            gfx.FillCircle(_clockRectangle.Center.ToVector2(), ((_clockRectangle.Width / 2)/10), accent);

            var secondEnd = getPointOnCircle(_clockRectangle.Center.ToVector2(), (_clockRectangle.Width / 2) - 16, _secondsDegrees-90);
            var hourEnd = getPointOnCircle(_clockRectangle.Center.ToVector2(), (_clockRectangle.Width / 2) / 4, _hoursDegrees-90);
            var minuteEnd = getPointOnCircle(_clockRectangle.Center.ToVector2(), (_clockRectangle.Width / 2) - 8, _minutesDegrees-90);

            gfx.DrawLine(_clockRectangle.Center.ToVector2(), secondEnd, 1, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System));
            gfx.DrawLine(_clockRectangle.Center.ToVector2(), hourEnd, 2, Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System));
            gfx.DrawLine(_clockRectangle.Center.ToVector2(), minuteEnd, 2, accent);

            gfx.FillCircle(_clockRectangle.Center.ToVector2(), ((_clockRectangle.Width / 2) / 10), accent);
        }
    }
}
