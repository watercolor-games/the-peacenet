using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;
using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.TextRenderers;
using Microsoft.Xna.Framework.Graphics;

namespace Peacenet.PeacegateThemes.PanelThemes
{
    public class WindowTheme : PanelTheme
    {
        private const string _appMenuTitle = "Peacegate";

        [Dependency]
        private ThemeManager _theme = null;

        public override string Name => "Default";

        public override Color StatusTextHoverColor => _theme.Theme.GetAccentColor();

        public override string Description => "Desktop panels are rendered using the system's window/UI theme.";

        public override Rectangle AppLauncherRectangle
        {
            get
            {
                var font = _theme.Theme.GetFont(TextFontStyle.Highlight);
                var measure = font.MeasureString(_appMenuTitle);
                return new Rectangle(0, 0, (int)measure.X + 10, (int)measure.Y + 7);
            }
        }

        public override SpriteFont StatusTextFont => _theme.Theme.GetFont(TextFontStyle.Highlight);
        public override Color StatusTextColor => _theme.Theme.GetFontColor(TextFontStyle.Highlight);

        public override Vector2 PanelButtonSize
        {
            get
            {
                var appLauncherSize = AppLauncherRectangle.Height;
                return new Vector2(175, appLauncherSize - 4);
            }
        }

        public override void DrawAppLauncher(GraphicsContext gfx, Rectangle rect, UIButtonState state)
        {
            var font = _theme.Theme.GetFont(TextFontStyle.Highlight);
            var measure = font.MeasureString(_appMenuTitle);

            gfx.DrawString(_appMenuTitle, rect.X + ((rect.Width - (int)measure.X) / 2), rect.Y + ((rect.Height - (int)measure.Y) / 2), (state != UIButtonState.Idle) ? _theme.Theme.GetAccentColor() : _theme.Theme.GetFontColor(TextFontStyle.Highlight), font, TextAlignment.Left, rect.Width, Plex.Engine.TextRenderers.WrapMode.None);
        }

        public override void DrawPanel(GraphicsContext gfx, Rectangle rect)
        {
            _theme.Theme.DrawControlBG(gfx, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public override void DrawPanelButton(GraphicsContext gfx, Rectangle rect, PanelButtonState state, UIButtonState mouseState, string text)
        {
            var font = _theme.Theme.GetFont(TextFontStyle.Highlight);
            switch(state)
            {
                case PanelButtonState.Default:
                    _theme.Theme.DrawControlLightBG(gfx, rect.X, rect.Y, rect.Width, rect.Height);
                    break;
                case PanelButtonState.Active:
                    _theme.Theme.DrawControlDarkBG(gfx, rect.X, rect.Y, rect.Width, rect.Height);
                    break;
                case PanelButtonState.Minimized:
                    _theme.Theme.DrawControlBG(gfx, rect.X, rect.Y, rect.Width, rect.Height);
                    break;
            }

            var measure = font.MeasureString(text);
            gfx.DrawString(text, new Vector2(rect.X + 6, rect.Y + ((rect.Height - (int)measure.Y) / 2)), (mouseState == UIButtonState.Idle) ? _theme.Theme.GetFontColor(TextFontStyle.Highlight) : _theme.Theme.GetAccentColor(), font, TextAlignment.Left, rect.Width, WrapMode.None);

        }
    }
}
