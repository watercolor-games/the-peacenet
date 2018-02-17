using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine.GUI;
using Plex.Engine;

namespace Peacenet.TerminalCommands
{
    /// <summary>
    /// Provides a debug command for testing notifications on the desktop.
    /// </summary>
    public class NotificationTestCommand : ITerminalCommand
    {
        [Dependency]
        private WindowSystem _winsys = null;

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Show a notification on the desktop.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "shownote";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                yield return "<text>";
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string text = arguments["<text>"].ToString();
            var desktopborder = _winsys.WindowList.FirstOrDefault(x => x.Border.Window is DesktopWindow);
            if(desktopborder==null)
            {
                console.WriteLine("Error: Couldn't find Peacegate Desktop instance (is this a valid game session?)");
                return;
            }
            var desktop = desktopborder.Border.Window as DesktopWindow;
            desktop.ShowNotification("Test notification from terminal", text);
        }
    }
}
