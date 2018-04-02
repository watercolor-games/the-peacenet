using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.PeacegateThemes.PanelThemes
{
    public abstract class PanelTheme
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        
        public abstract Rectangle AppLauncherRectangle { get; }

        public virtual int DesktopPanelHeight
        {
            get
            {
                return AppLauncherRectangle.Height;
            }
        }

        public abstract SpriteFont StatusTextFont { get; }
        public abstract Color StatusTextColor { get; }
        
        public abstract Vector2 PanelButtonSize { get; }

        public abstract void DrawAppLauncher(GraphicsContext gfx, Rectangle rect, UIButtonState state);
        public abstract void DrawPanel(GraphicsContext gfx, Rectangle rect);
        public abstract void DrawPanelButton(GraphicsContext gfx, Rectangle rect, PanelButtonState state, UIButtonState mouseState, string text);
        
    }

    public enum PanelButtonState
    {
        Default,
        Active,
        Minimized
    }
}
