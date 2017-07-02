using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShiftOS.Engine;
using ShiftOS.Frontend.Desktop;

namespace ShiftOS.Frontend.GraphicsSubsystem
{
    public static class UIManager
    {
        private static List<GUI.Control> topLevels = new List<GUI.Control>();

        public static void DrawControls(GraphicsDevice graphics, SpriteBatch batch)
        {
            foreach (var ctrl in topLevels)
            {
                using(var bmp = new System.Drawing.Bitmap(ctrl.Width, ctrl.Height))
                {
                    var gfx = System.Drawing.Graphics.FromImage(bmp);
                    ctrl.Paint(gfx);
                    //get the bits of the bitmap
                    var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    byte[] rgb = new byte[Math.Abs(data.Stride) * data.Height];
                    Marshal.Copy(data.Scan0, rgb, 0, rgb.Length);
                    bmp.UnlockBits(data);
                    var tex2 = new Texture2D(graphics, bmp.Width, bmp.Height);
                    tex2.SetData<byte>(rgb);
                    batch.Draw(tex2, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), Color.White);
                }
            }
        }

        public static void AddTopLevel(GUI.Control ctrl)
        {
            if (!topLevels.Contains(ctrl))
                topLevels.Add(ctrl);
        }

        public static void ProcessMouseState(MouseState state)
        {
            foreach(var ctrl in topLevels)
            {
                if (ctrl.ProcessMouseState(state) == true)
                    break;
            }
        }



        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            if (SkinEngine.LoadedSkin == null)
                SkinEngine.Init();
            graphics.Clear(SkinEngine.LoadedSkin.DesktopColor.ToMonoColor());
            var desktopbg = SkinEngine.GetImage("desktopbackground");
            if(desktopbg != null)
            {
                var tex2 = new Texture2D(graphics, desktopbg.Width, desktopbg.Height);
                tex2.SetData<byte>(SkinEngine.LoadedSkin.DesktopBackgroundImage);
                batch.Draw(tex2, new Rectangle(0, 0, width, height), Color.White);
            }
        }

        public static Color ToMonoColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        internal static void StopHandling(GUI.Control ctrl)
        {
            if (topLevels.Contains(ctrl))
                topLevels.Remove(ctrl);
            ctrl = null;
        }
    }
}
