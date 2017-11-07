using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.Theming
{
    public static class ThemeManager
    {
        private static Theme _theme = null;

        public static Theme Theme
        {
            get
            {
                return _theme;
            }
        }

        public static void LoadTheme(Theme theme)
        {
            if (theme == null)
                throw new ArgumentNullException();
            _theme = theme;
            _theme.LoadThemeData(UIManager.GraphicsDevice);

        }
    }
}
