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
            Logger.Log("Searching for a Plexgate theme object...", LogType.Info, "themer");
            Type dummy = null;
            foreach (var type in ReflectMan.Types.Where(x=>x.BaseType == typeof(Theme)))
            {
                if(type.GetCustomAttributes(false).Any(x=>x is DummyThemeAttribute))
                {
                    dummy = type;
                }
                else
                {
                    _theme = (Theme)_plexgate.New(type);
                }
            }
            if (_theme == null)
            {
                Logger.Log("Couldn't find a non-dummy theme.", LogType.Warning, "themer");
                try
                {
                    _theme = (Theme)_plexgate.New(dummy);
                }
                catch
                {
                    Logger.Log("AHH! Couldn't load the dummy theme. The UI system won't work.", LogType.Fatal, "themer");
                }
            }
            if (_theme != null)
            {
                Logger.Log("Theme loaded: " + _theme?.GetType().Name, LogType.Info, "themer");
                _theme.LoadThemeData(_plexgate.GraphicsDevice, _plexgate.Content);
            }
        }

        /// <summary>
        /// Retrieves the currently loaded theme.
        /// </summary>
        public Theme Theme
        {
            get
            {
                return _theme;
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
