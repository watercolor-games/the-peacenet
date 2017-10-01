using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.GUI;
using static Plex.Engine.FSUtils;

namespace Plex.Frontend.Apps
{
    [FileHandler("Plex Editor", ".txt", "")]
    [DefaultTitle("Plex Editor")]
    [WinOpen("edit")]
    [Launcher("Plex Editor", false, null, "Accessories")]
    public class TextPad : GUI.Control, IPlexWindow, IFileHandler
    {
        private TerminalControl contentsLabel = null;
        private MenuBar _menuBar = new MenuBar();

        public TextPad()
        {
            Width = 700;
            Height = 600;
            contentsLabel = new TerminalControl();
            contentsLabel.PerformTerminalBehaviours = false;
            AddControl(contentsLabel);

            AddControl(_menuBar);
            _menuBar.Visible = true;
            var n = new MenuItem
            {
                Text = "New"
            };
            n.ItemActivated += () =>
            {
                contentsLabel.Text = "";
            };
            _menuBar.AddItem(n);

            var o = new MenuItem
            {
                Text = "Open"
            };
            o.ItemActivated += () =>
            {
                FileSkimmerBackend.GetFile(new[] { ".txt" }, FileOpenerStyle.Open, (path) =>
                 {
                     contentsLabel.Text = ReadAllText(path);
                 });
            };
            _menuBar.AddItem(o);

            var s = new MenuItem
            {
                Text = "Save"
            };
            s.ItemActivated += () =>
            {
                FileSkimmerBackend.GetFile(new[] { ".txt" }, FileOpenerStyle.Save, (path) =>
                 {
                     WriteAllText(path, contentsLabel.Text);
                 });
            };
            _menuBar.AddItem(s);
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
            contentsLabel.Text = ReadAllText(file);
            AppearanceManager.SetupWindow(this);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _menuBar.Width = Width;
            _menuBar.Height = 24;
            contentsLabel.X = 0;
            contentsLabel.Y = _menuBar.Height;
            contentsLabel.Width = Width;
            contentsLabel.Height = Height - _menuBar.Height;
        }

    }
}
