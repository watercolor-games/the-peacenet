using System;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Peacenet.Filesystem;
using Peacenet.CoreUtils;
using System.IO;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework;

namespace Peacenet
{
    [AppLauncher("Image Viewer", "Accessories", "Look at your pictures")]
    public class ImageViewer : Window
    {
        [Dependency]
        FSManager fs;

        [Dependency]
        GameLoop plex;

        [Dependency]
        GUIUtils gui;

        Texture2D img;
        PictureBox open = new PictureBox();

        public ImageViewer(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Image Viewer";
            Width = 640;
            Height = 480;
            AddChild(open);
            open.Click += (sender, e) => gui.AskForFile(false, new[] { "image/bmp", "image/jpeg", "image/png" }, (path) => load(path));
            img = null;
            open.Texture = plex.Content.Load<Texture2D>("UIIcons/folder-open-o");
        }
        void load(string path)
        {
            using (var fobj = new MemoryStream(fs.ReadAllBytes(path)))
                img = Texture2D.FromStream(plex.GraphicsDevice, fobj);
            Title = $"{path} - Image Viewer";
        }
        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime time)
        {
            open.X = 3;
            open.Y = 3;
            open.Width = 20;
            open.Height = 20;

            open.Tint = Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.System);
            if (open.LeftButtonPressed)
                open.Tint = Theme.GetAccentColor().Darken(0.5F);
            else if (open.ContainsMouse)
                open.Tint = Theme.GetAccentColor();

        }
        protected override void OnPaint(Microsoft.Xna.Framework.GameTime time, Plex.Engine.GraphicsSubsystem.GraphicsContext gfx)
        {
            base.OnPaint(time, gfx);
            var drawy = open.Y + open.Height + 3;
            Theme.DrawControlDarkBG(gfx, 0, drawy, Width, Height - drawy);
            if (img != null)
                gfx.FillRectangle(0, drawy, Width, Height - drawy, img, Color.White, ImageLayout.Zoom);
        }
    }
}