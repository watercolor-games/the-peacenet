using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Objects;
using Microsoft.Xna.Framework.Content;
using Plex.Engine.Config;

namespace Plex.Engine.Themes
{
    /// <summary>
    /// Provides simple user interface theming.
    /// </summary>
    public class ThemeManager : IEngineComponent, IDisposable, IConfigurable
    {

        [Dependency]
        private Plexgate _plexgate = null;

        private Theme _theme = null;
        /// <inheritdoc/>
        public void Initiate()
        {

        }

        /// <summary>
        /// Retrieves the currently loaded theme.
        /// </summary>
        public Theme Theme
        {
            get
            {
                if (_theme == null) //We're not Windows. Let's throw a valuable error when the theme is null.
                    throw new InvalidOperationException("No UI theme has been initiated. Please be sure to set ThemeManager.Theme to a valid Theme object before utilizing theme features!");

                return _theme;
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("You cannot operate Peace Engine without a theme object.");
                if (_theme != null)
                    _theme.UnloadThemeData();
                _theme = value;
                _theme.LoadThemeData(_plexgate.GraphicsDevice, _plexgate.Content);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _theme?.UnloadThemeData();
        }

        public void ApplyConfig()
        {
            if (_theme == null)
                return;
            _theme.LoadThemeData(_plexgate.GraphicsDevice, _plexgate.Content);
        }
    }
}
