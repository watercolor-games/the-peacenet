using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.GUI;

namespace Plex.Frontend.Apps
{
    [DefaultTitle("Community")]
    public class Community : Control, IPlexWindow
    {
        private TextControl Developer = null;
        private TextControl Dummy = null;
        public Community()
        {
            Width = 640;
            Height = 480;
            Developer = new GUI.TextControl();
            Dummy = new GUI.TextControl();

            AddControl(Developer);
            AddControl(Dummy);
            Dummy.Text = "dummy";
            Developer.Text = "HEY LOOK I FOUND DEVELOPER TEXT";
            Dummy.AutoSize = true;
            Developer.AutoSize = false;
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
            Dummy.Font = SkinEngine.LoadedSkin.Header3Font;
            Developer.Font = SkinEngine.LoadedSkin.MainFont;
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
            Dummy.X = 15;
            Dummy.Y = 15;

            Developer.X = 15;
            Developer.Y = Dummy.Y + Dummy.Height + 10;
            Developer.Width = Width - 30;
            Developer.Height = (Height - Developer.Y) - 15;
        }
    }
}
