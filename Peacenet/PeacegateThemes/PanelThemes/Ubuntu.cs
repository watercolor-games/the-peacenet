using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using Plex.Engine;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Plex.Engine.TextRenderers;

namespace Peacenet.PeacegateThemes.PanelThemes
{
    public class Ubuntu : PanelTheme, ILoadable
    {
        private Texture2D _panelBG = null;
        private Texture2D _appBG = null;
        private Texture2D _buttonBG = null;

        [Dependency]
        private ThemeManager _theme = null;

        public override string Name => "Ubuntu Linux";

        public override string Description => "A simple, elegant Panel Theme based off the Ubuntu 10.04 desktop theme from years past.";

        public override Rectangle AppLauncherRectangle => new Rectangle(0, 0, 135, 24);

        public override SpriteFont StatusTextFont => _theme.Theme.GetFont(TextFontStyle.System);

        public override Color StatusTextColor => Color.White;

        public override Color StatusTextHoverColor => StatusTextColor.Darken(0.15F);
            
        public override Vector2 PanelButtonSize => new Vector2(239, 24);

        public override void DrawAppLauncher(GraphicsContext gfx, Rectangle rect, UIButtonState state)
        {
            gfx.FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, _appBG, Color.White);
        }

        public override void DrawPanel(GraphicsContext gfx, Rectangle rect)
        {
            gfx.FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, _panelBG, Color.White);
        }

        public override void DrawPanelButton(GraphicsContext gfx, Rectangle rect, PanelButtonState state, UIButtonState mouseState, string text)
        {
            gfx.FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, _buttonBG, Color.White);

            var measure = _theme.Theme.GetFont(TextFontStyle.System).MeasureString(text);
            gfx.DrawString(text, new Vector2(rect.X + 22, rect.Y + ((rect.Height - measure.Y) / 2)), Color.White, _theme.Theme.GetFont(TextFontStyle.System), TextAlignment.Left, rect.Width, WrapMode.None);
        }

        public void Load(ContentManager content)
        {
            _panelBG = content.Load<Texture2D>("ThemeAssets/Industrial/taskbar");
            _appBG = content.Load<Texture2D>("ThemeAssets/Industrial/startbutton");
            _buttonBG = content.Load<Texture2D>("ThemeAssets/Industrial/taskbarbutton");

        }
    }
}
