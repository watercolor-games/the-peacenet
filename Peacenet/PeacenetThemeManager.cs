using Microsoft.Xna.Framework.Graphics;
using Peacenet.PeacegateThemes;
using Peacenet.PeacegateThemes.PanelThemes;
using Plex.Engine;
using Plex.Engine.Interfaces;
using Plex.Engine.Themes;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet
{
    public class PeacenetThemeManager : IEngineComponent, IDisposable
    {
        private PeacenetAccentColor _accent = PeacenetAccentColor.Blueberry;

        [Dependency]
        private GameLoop _GameLoop = null;

        private PanelTheme _panelTheme = null;
        private List<PanelTheme> _panelThemes = null;

        public PeacenetAccentColor AccentColor
        {
            get { return _accent; }
            set { _accent = value; }
        }

        public PanelTheme[] AvailablePanelThemes
        {
            get
            {
                return _panelThemes.ToArray();
            }
        }

        public PanelTheme PanelTheme
        {
            get
            {
                return _panelTheme;
            }
            set
            {
                if (value == _panelTheme)
                    return;
                if (value == null)
                    return;
                _panelTheme = value;
                PanelThemeChanged?.Invoke();
            }
        }

        public event Action PanelThemeChanged;

        private List<ThemeInfo> _themes = new List<ThemeInfo>();

        public ThemeInfo[] Themes
        {
            get
            {
                return _themes.ToArray();
            }
        }

        public void Dispose()
        {
            while(_themes.Count>0)
            {
                _themes[0].Preview?.Dispose();
                _themes.RemoveAt(0);
            }
        }

        public void Initiate()
        {
            _panelThemes = new List<PanelTheme>();
            foreach(var type in ReflectMan.Types.Where(x=>x.Inherits(typeof(Theme))))
            {
                var attribute = type.GetCustomAttributes(false).FirstOrDefault(x => x is PeacegateThemeAttribute) as PeacegateThemeAttribute;
                if(attribute!= null)
                {
                    Texture2D preview = null;

                    if(!string.IsNullOrWhiteSpace(attribute.PreviewTexture))
                    {
                        Logger.Log($"Loading preview texture for theme {attribute.Name} ({attribute.PreviewTexture})");
                        preview = _GameLoop.Content.Load<Texture2D>(attribute.PreviewTexture);
                    }

                    _themes.Add(new ThemeInfo
                    {
                        Name = attribute.Name,
                        Description = attribute.Description,
                        Preview = preview,
                        ThemeType = type
                    });
                }
            }

            foreach (var type in ReflectMan.Types.Where(x => x.Inherits(typeof(PanelTheme))))
            {
                _panelThemes.Add((PanelTheme)_GameLoop.New(type));
            }
            _panelTheme = _GameLoop.New<WindowTheme>();

        }
    }

    public class ThemeInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Texture2D Preview { get; set; }
        public Type ThemeType { get; set; }
    }

    public static class ReflectionExtensions
    {
        public static bool Inherits(this Type input, Type baseType)
        {
            var b = input.BaseType;
            while(b != null)
            {
                if (b == baseType)
                    return true;
                b = b.BaseType;
            }

            return false;
        }
    }
}
