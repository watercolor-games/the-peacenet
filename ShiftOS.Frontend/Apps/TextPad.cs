using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Frontend.Apps
{
    [FileHandler("TextPad", ".txt", "")]
    [DefaultTitle("TextPad")]
    [WinOpen("textpad")]
    [Launcher("TextPad", false, null, "Accessories")]
    public class TextPad : GUI.Control, IShiftOSWindow, IFileHandler
    {
        private TerminalControl contentsLabel = null;

        public TextPad()
        {
            contentsLabel = new TerminalControl();
            contentsLabel.Dock = GUI.DockStyle.Fill;
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
            //contentsLabel.Text = Objects.ShiftFS.Utils.ReadAllText(file);
            AppearanceManager.SetupWindow(this);
        }
    }
}
