using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine;
using Plex.Engine.Themes;
using Plex.Engine.Saves;
using Peacenet.PeacegateThemes;

namespace Peacenet
{
    /// <summary>
    /// Provides a Terminal Command which can configure the <see cref="PeacenetTheme"/>.
    /// </summary>
    public class ThemeConfigurationCommand : ITerminalCommand
    {
        public string Description
        {
            get
            {
                return "Customize your Peacenet OS theme.";
            }
        }

        public string Name
        {
            get
            {
                return "themeconf";
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                yield return "setaccent <accent>";
                yield return "listaccents";

            }
        }

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private Plex.Engine.GraphicsSubsystem.UIManager _ui = null;

        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            if((bool)arguments["listaccents"])
            {
                foreach(var name in Enum.GetNames(typeof(PeacenetAccentColor)))
                {
                    console.WriteLine($" - {name}");
                }
            }
            else
            {
                string name = arguments["<accent>"].ToString();
                var accent = (PeacenetAccentColor)Enum.Parse(typeof(PeacenetAccentColor), name);
                this._save.SetValue<PeacenetAccentColor>("theme.accent", accent);
                _pn.AccentColor = accent;
            }
        }
    }
}
