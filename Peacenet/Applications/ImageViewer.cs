using System;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Peacenet.Filesystem;
using Peacenet.CoreUtils;
using System.IO;

namespace Peacenet
{
    [AppLauncher("Image Viewer", "Accessories", "Look at your pictures")]
    public class ImageViewer : Window
    {
        [Dependency]
        FSManager fs;

        [Dependency]
        Plexgate plex;

        [Dependency]
        GUIUtils gui;

        Texture2D img;
        Button open = new Button();

        public ImageViewer(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Image Viewer";
            Width = 640;
            Height = 480;
            AddChild(open);
            open.Click += (sender, e) => gui.AskForFile(false, (path) => load(path));
            img = null;
        }
        void load(string path)
        {
            using (var fobj = fs.OpenRead(path))
                img = Texture2D.FromStream(plex.GraphicsDevice, fobj);
            Title = $"{path} - Image Viewer";
        }
        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime time)
        {
            open.X = 3;
            open.Y = 3;
            open.Text = "Open";
        }
        protected override void OnPaint(Microsoft.Xna.Framework.GameTime time, Plex.Engine.GraphicsSubsystem.GraphicsContext gfx)
        {
            base.OnPaint(time, gfx);
            var drawy = open.Y + open.Height + 3;
            if (img != null)   
                gfx.DrawRectangle(0, drawy, Width, Height - drawy, img, System.Windows.Forms.ImageLayout.Zoom);
        }
    }
}
