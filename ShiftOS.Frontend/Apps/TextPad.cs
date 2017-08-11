using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;

namespace Plex.Frontend.Apps
{
    [FileHandler("Plex Editor", ".txt", "")]
    [DefaultTitle("Plex Editor")]
    [WinOpen("edit")]
    [Launcher("Plex Editor", false, null, "Accessories")]
    public class TextPad : GUI.Control, IPlexWindow, IFileHandler
    {
        private TerminalControl contentsLabel = null;

        public TextPad()
        {
            contentsLabel = new TerminalControl();
            contentsLabel.Dock = GUI.DockStyle.Fill;
            contentsLabel.PerformTerminalBehaviours = false;
            AddControl(contentsLabel);
        }

        public void OnLoad()
        {
            
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void OpenFile(string file)
        {
            contentsLabel.Text = Objects.ShiftFS.Utils.ReadAllText(file);
            AppearanceManager.SetupWindow(this);
        }
    }
}
