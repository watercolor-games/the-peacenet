using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.Desktop;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.Apps
{
    [WinOpen("systemstatus")]
    [Launcher("System Status", false, null, "System")]
    [DefaultTitle("System Status")]
    [SidePanel]
    public class SystemStatus : GUI.Control, IPlexWindow
    {
        GUI.TextControl _header = null;
        GUI.TextControl _mainstatus = null;

        public SystemStatus()
        {
            Width = 720;
            Height = 480;
            _header = new GUI.TextControl();
            _mainstatus = new GUI.TextControl();
            AddControl(_header);
            AddControl(_mainstatus);
            _header.AutoSize = true;
            _header.Text = "System Status";
            
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

        protected override void OnLayout(GameTime gameTime)
        {
            _header.Font = SkinEngine.LoadedSkin.HeaderFont;
            _header.X = 20;
            _header.Y = 20;
            _mainstatus.X = 20;
            _mainstatus.Y = _header.Y + _header.Height + 10;
            _mainstatus.Width = Width - 40;
            _mainstatus.Height = Height - (_header.Y + _header.Height) - 40;
            _mainstatus.Text = $@"Experience: {SaveSystem.CurrentSave.Experience}
Upgrades: {SaveSystem.CurrentSave.CountUpgrades()} installed, {Upgrades.GetDefaults().Count} available

Username: {SaveSystem.CurrentSave.Username}
System name: {SaveSystem.CurrentSave.SystemName}
Open programs: {AppearanceManager.OpenForms.Count}";
        }
    }
}
