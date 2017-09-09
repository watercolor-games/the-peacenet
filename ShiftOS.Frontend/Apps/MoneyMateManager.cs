using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Extras;

namespace Plex.Frontend.Apps
{
    [Launcher("MoneyMate Manager", false, null, "MoneyMate")]
    [WinOpen("moneymatemgr")]
    [DefaultTitle("MoneyMate Manager")]
    [Installer("MoneyMate Manager", "Manage your MoneyMate account, transactions, and much more with a simple graphical user interface for Plexgate! Lifetime satisfaction guaranteed!", 1400 * 1024)]
    public class MoneyMateManager : Control, IPlexWindow
    {
        public MoneyMateManager()
        {
            Width = 600;
            Height = 400;
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
    }
}
