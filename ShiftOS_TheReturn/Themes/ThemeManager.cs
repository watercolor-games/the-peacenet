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

namespace Plex.Engine.Themes
{
    public class ThemeManager : IEngineComponent
    {

        [Dependency]
        private Plexgate _plexgate = null;

        private Theme _theme = null;
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
                    _theme = (Theme)Activator.CreateInstance(type, null);
                }
            }
            if (_theme == null)
            {
                Logger.Log("Couldn't find a non-dummy theme.", LogType.Warning, "themer");
                try
                {
                    _theme = (Theme)Activator.CreateInstance(dummy, null);
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

        public Theme Theme
        {
            get
            {
                return _theme;
            }
        }

        public int DrawIndex
        {
            get
            {
                return -1;
            }
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
            _theme?.UnloadThemeData();
        }
    }
}
