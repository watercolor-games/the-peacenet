using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.Desktop
{
    public static class PlexWindowExtensions
    {
        public static bool IsSidePanel(this IWindowBorder border)
        {
            var win = border.ParentWindow.GetType();
            var attr = win.GetCustomAttributes(false).FirstOrDefault(x => x is SidePanel);
            return attr != null;
        }
    }

    public class WindowManager : Engine.WindowManager
    {
        public int DesktopStart
        {
            get
            {
                return (LoadedSkin.DesktopPanelPosition == 1) ? 0 : LoadedSkin.DesktopPanelHeight;
            }
        }

        public override void Close(IPlexWindow win)
        {
            var brdr = RunningBorders.FirstOrDefault(x => x.ParentWindow == win);
            if (brdr != null)
            {
                brdr.Close();
                RunningBorders.Remove(brdr);
                if (AppearanceManager.OpenForms.Contains(brdr))
                {
                    AppearanceManager.OpenForms.Remove(brdr);
                    TileWindows();
                    Engine.Desktop.ResetPanelButtons();
                }
                win = null;
            }
        }

        private List<WindowBorder> RunningBorders = new List<WindowBorder>();

        public override void InvokeAction(Action act)
        {
            UIManager.CrossThreadOperations.Enqueue(act);
        }

        public override void Maximize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void Minimize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void SetTitle(IPlexWindow win, string title)
        {
            var brdr = RunningBorders.FirstOrDefault(x => x.ParentWindow == win);
            if (brdr != null)
                brdr.Text = title;
        }

        public string GetTitle(IPlexWindow win)
        {
            var type = win.GetType();
            var attr = type.GetCustomAttributes(false).FirstOrDefault(x => x is DefaultTitleAttribute) as DefaultTitleAttribute;
            if (attr != null)
                return Localization.Parse(attr.Title);
            return "Plex Window";
        }

        public override void SetupDialog(IPlexWindow win)
        {
            var wb = new WindowBorder();
            wb.Text = GetTitle(win);
            var ctl = win as GUI.Control;
            if (ctl.Width < 30)
                ctl.Width = 30;
            if (ctl.Height < 30)
                ctl.Height = 30;
            wb.Width = (win as GUI.Control).Width + LoadedSkin.LeftBorderWidth + LoadedSkin.RightBorderWidth;
            wb.Height = (win as GUI.Control).Height + LoadedSkin.TitlebarHeight + LoadedSkin.BottomBorderWidth;
            wb.ParentWindow = win;
            wb.IsDialog = true;
            UIManager.AddTopLevel(wb);
            RunningBorders.Add(wb);
            wb.X = (UIManager.Viewport.Width - wb.Width) / 2;
            wb.Y = (UIManager.Viewport.Height - wb.Height) / 2;
            win.OnLoad();
            win.OnUpgrade();
            win.OnSkinLoad();
        }

        private int MaxCount
        {
            get
            {
                if (Upgrades.UpgradeInstalled("wm_unlimited_windows"))
                    return int.MaxValue;
                if (Upgrades.UpgradeInstalled("wm_4_windows"))
                    return 4;
                if (Upgrades.UpgradeInstalled("wm_2_windows"))
                    return 2;
                return 1;
            }
        }

        public override void SetupWindow(IPlexWindow win)
        {
            if (!Upgrades.UpgradeAttributesUnlocked(win.GetType()))
            {
                Console.WriteLine("Application not found on system.");
                return;
            }
            while (AppearanceManager.OpenForms.Where(x => !x.IsSidePanel()).Count() >= MaxCount)
            {
                var brdr = AppearanceManager.OpenForms.FirstOrDefault(x => !x.IsSidePanel());
                if (brdr != null)
                {
                    brdr.Close();
                    AppearanceManager.OpenForms.Remove(brdr);
                }

            }

            var wb = new WindowBorder();
            wb.Text = GetTitle(win);
            wb.Width = (win as GUI.Control).Width + LoadedSkin.LeftBorderWidth + LoadedSkin.RightBorderWidth;
            wb.Height = (win as GUI.Control).Height + LoadedSkin.TitlebarHeight + LoadedSkin.BottomBorderWidth;
            wb.ParentWindow = win;
            wb.IsDialog = true;
            UIManager.AddTopLevel(wb);
            AppearanceManager.OpenForms.Add(wb);
            RunningBorders.Add(wb);
            TileWindows();
            win.OnLoad();
            win.OnUpgrade();
            win.OnSkinLoad();
        }

        public void TileWindows()
        {
            try
            {
                if (AppearanceManager.OpenForms.Count == 0)
                    return;
                int start = 0;
                var brdr = AppearanceManager.OpenForms.FirstOrDefault(x => x.IsSidePanel()) as WindowBorder;
                if (brdr != null)
                {
                    brdr.X = 0;
                    brdr.Y = DesktopStart;
                    brdr.ResizeWindow(UIManager.Viewport.Width / 4, UIManager.Viewport.Height - LoadedSkin.DesktopPanelHeight);
                    start = UIManager.Viewport.Width / 4;
                }


                var wb = (WindowBorder)AppearanceManager.OpenForms.FirstOrDefault(x => !x.IsSidePanel());
                if (wb != null)
                {
                    wb.X = start;
                    wb.Y = DesktopStart;
                    wb.ResizeWindow(UIManager.Viewport.Width - start, UIManager.Viewport.Height - LoadedSkin.DesktopPanelHeight);
                }
            }
            catch(Exception ex)
            {
#if DEBUG
                Debug.WriteLine("<engine> [WARN] Window management fucked up.");
                Debug.WriteLine("<engine> [WARN] " + ex.ToString());
#endif
            }
        }
    }

    public class WindowBorder : GUI.Control, IWindowBorder
    {
        private string _text = "Plex window";
        private GUI.Control _hostedwindow = null;

        public void ResizeWindow(int width, int height)
        {
            int titleheight = LoadedSkin.TitlebarHeight;
            int leftwidth = LoadedSkin.LeftBorderWidth;
            int bottomheight = LoadedSkin.BottomBorderWidth;
            int rightwidth = LoadedSkin.RightBorderWidth;
            _hostedwindow.Width = width - leftwidth - rightwidth;
            _hostedwindow.Height = height - bottomheight - titleheight;
            Width = width;
            Height = height;

        }

        public WindowBorder()
        {
            Click += () =>
            {
                var cbtnloc = LoadedSkin.CloseButtonFromSide;
                var realcloc = new Vector2(
                        (LoadedSkin.TitleButtonPosition == 1) ? cbtnloc.X : (Width - LoadedSkin.TitleLeftCornerWidth - LoadedSkin.TitleRightCornerWidth) - cbtnloc.X,
                        cbtnloc.Y
                    );
                var cbtnsize = LoadedSkin.CloseButtonSize;
                if(MouseX >= realcloc.X && MouseY >= realcloc.Y && MouseX <= realcloc.X + cbtnsize.Width && MouseY <= realcloc.Y + cbtnsize.Height)
                {
                    AppearanceManager.Close(ParentWindow);
                }
            };
            X = 720;
            Y = 480;
        }

        public IPlexWindow ParentWindow
        {
            get
            {
                return (IPlexWindow)_hostedwindow;
            }

            set
            {
                _hostedwindow = (GUI.Control)value;
                ClearControls();
                AddControl(_hostedwindow);
                Width = LoadedSkin.LeftBorderWidth + _hostedwindow.Width + LoadedSkin.RightBorderWidth;
                Height = LoadedSkin.BottomBorderWidth + _hostedwindow.Height + LoadedSkin.TitlebarHeight;

            }
        }

        public bool IsDialog { get; set; }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public void Close()
        {
            Visible = false;
            UIManager.StopHandling(this);
        }

        private int lastmousex, lastmousey = 0;

        protected override void OnLayout(GameTime gameTime)
        {
            if (IsFocusedControl || ContainsFocusedControl)
            {
                UIManager.BringToFront(this);
            }
            int titlebarheight = LoadedSkin.TitlebarHeight;
            int borderleft = LoadedSkin.LeftBorderWidth;
            int borderright = LoadedSkin.RightBorderWidth;
            int borderbottom = LoadedSkin.BottomBorderWidth;
            _hostedwindow.X = borderleft;
            _hostedwindow.Y = titlebarheight;
            Width = _hostedwindow.X + _hostedwindow.Width + LoadedSkin.RightBorderWidth;
            Height = _hostedwindow.Y + _hostedwindow.Height + LoadedSkin.BottomBorderWidth;
        }



        protected override void OnPaint(GraphicsContext gfx)
        {
            int titleheight = LoadedSkin.TitlebarHeight;
            int leftborderwidth = LoadedSkin.LeftBorderWidth;
            int rightborderwidth = LoadedSkin.RightBorderWidth;
            int bottomborderwidth = LoadedSkin.BottomBorderWidth;

                var titlebarcolor = UIManager.SkinTextures["TitleBackgroundColor"];
                var titlefont = LoadedSkin.TitleFont;
                var titletextcolor = LoadedSkin.TitleTextColor;
                var titletextleft = LoadedSkin.TitleTextLeft;
                bool titletextcentered = LoadedSkin.TitleTextCentered;

                var drawcorners = LoadedSkin.ShowTitleCorners;
                int titlebarleft = 0;
                int titlebarwidth = Width;
                if (drawcorners)
                {
                    //set titleleft to the first corner width
                    titlebarleft = LoadedSkin.TitleLeftCornerWidth;
                    titlebarwidth -= titlebarleft;
                    titlebarwidth -= LoadedSkin.TitleRightCornerWidth;

                    
                    //Let's get the left and right images.
                    //and the colors
                    var leftcolor = UIManager.SkinTextures["TitleLeftCornerBackground"];
                    var rightcolor = UIManager.SkinTextures["TitleRightCornerBackground"];
                    //and the widths
                    var leftwidth = LoadedSkin.TitleLeftCornerWidth;
                    var rightwidth = LoadedSkin.TitleRightCornerWidth;

                    //draw left corner
                    if(UIManager.SkinTextures.ContainsKey("titleleft"))
                    {
                        gfx.DrawRectangle(0, 0, leftwidth, titleheight, UIManager.SkinTextures["titleleft"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(0, 0, leftwidth, titleheight, leftcolor);
                    }

                    //draw right corner
                    if (UIManager.SkinTextures.ContainsKey("titleright"))
                    {
                        gfx.DrawRectangle(titlebarleft + titlebarwidth, 0, rightwidth, titleheight, UIManager.SkinTextures["titleright"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(titlebarleft + titlebarwidth, 0, rightwidth, titleheight, rightcolor);
                    }
                }

                if (!UIManager.SkinTextures.ContainsKey("titlebar"))
                {
                    //draw the title bg
                    gfx.DrawRectangle(titlebarleft, 0, titlebarwidth, titleheight, titlebarcolor);

                }
                else
                {
                    gfx.DrawRectangle(titlebarleft, 0, titlebarwidth, titleheight, UIManager.SkinTextures["titlebar"]);
                }
                //Now we draw the title text.
                var textMeasure = GraphicsContext.MeasureString(Text, titlefont);
                PointF textloc;
                if (titletextcentered)
                    textloc = new PointF((titlebarwidth - textMeasure.X) / 2,
                        titletextleft.Y);
                else
                    textloc = new PointF(titlebarleft + titletextleft.X, titletextleft.Y);

                gfx.DrawString(Text, (int)textloc.X, (int)textloc.Y, titletextcolor.ToMonoColor(), titlefont);

                var tbuttonpos = LoadedSkin.TitleButtonPosition;

                //Draw close button
                    var closebuttonsize = LoadedSkin.CloseButtonSize;
                    var closebuttonright = LoadedSkin.CloseButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new System.Drawing.Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
                    if (!UIManager.SkinTextures.ContainsKey("closebutton"))
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["CloseButtonColor"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["closebutton"]);
                    }
                
                //Draw maximize button
                closebuttonsize = LoadedSkin.MaximizeButtonSize;
                closebuttonright = LoadedSkin.MaximizeButtonFromSide;
                if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new System.Drawing.Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);

                    if (!UIManager.SkinTextures.ContainsKey("maximizebutton"))
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["MaximizeButtonColor"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["maximizebutton"]);
                    }
                
                //Draw minimize button
                closebuttonsize = LoadedSkin.MinimizeButtonSize;
                closebuttonright = LoadedSkin.MinimizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new System.Drawing.Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
            if (!UIManager.SkinTextures.ContainsKey("minimizebutton"))
            {
                gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["MinimizeButtonColor"]);
            }
            else
            {
                gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["minimizebutton"]);
            }

                
            
                //Some variables we'll need...
                int bottomlocy = Height - LoadedSkin.BottomBorderWidth;
                int bottomlocx = leftborderwidth;
                int bottomwidth = Width - bottomlocx - rightborderwidth;
                int brightlocx = Width - rightborderwidth;

                var borderleftcolor = (ContainsFocusedControl || IsFocusedControl) ? UIManager.SkinTextures["BorderLeftBackground"] : UIManager.SkinTextures["BorderInactiveLeftBackground"];
                var borderrightcolor = (ContainsFocusedControl || IsFocusedControl) ? UIManager.SkinTextures["BorderRightBackground"] : UIManager.SkinTextures["BorderInactiveRightBackground"];
                var borderbottomcolor = (ContainsFocusedControl || IsFocusedControl) ? UIManager.SkinTextures["BorderBottomBackground"] : UIManager.SkinTextures["BorderInactiveBottomBackground"];
                var borderbleftcolor = (ContainsFocusedControl || IsFocusedControl) ? UIManager.SkinTextures["BorderBottomLeftBackground"] : UIManager.SkinTextures["BorderInactiveBottomLeftBackground"];
                var borderbrightcolor = (ContainsFocusedControl || IsFocusedControl) ? UIManager.SkinTextures["BorderBottomRightBackground"] : UIManager.SkinTextures["BorderInactiveBottomRightBackground"];


                //draw border corners
                //BOTTOM LEFT
                if (!UIManager.SkinTextures.ContainsKey("bottomlborder"))
                {
                    gfx.DrawRectangle(0, bottomlocy, leftborderwidth, bottomborderwidth, borderbleftcolor);
                }
                else
                {
                    gfx.DrawRectangle(0, bottomlocy, leftborderwidth, bottomborderwidth, UIManager.SkinTextures["bottomlborder"]);
                }

                //BOTTOM RIGHT
                if (!UIManager.SkinTextures.ContainsKey("bottomrborder"))
                {
                    gfx.DrawRectangle(brightlocx, bottomlocy, rightborderwidth, bottomborderwidth, borderbrightcolor);
                }
                else
                {
                    gfx.DrawRectangle(brightlocx, bottomlocy, rightborderwidth, bottomborderwidth, UIManager.SkinTextures["bottomrborder"]);
                }

                //BOTTOM
                if (!UIManager.SkinTextures.ContainsKey("bottomborder"))
                {
                    gfx.DrawRectangle(leftborderwidth, bottomlocy, bottomwidth, bottomborderwidth, borderbottomcolor);
                }
                else
                {
                    gfx.DrawRectangle(leftborderwidth, bottomlocy, bottomwidth, bottomborderwidth, UIManager.SkinTextures["bottomborder"]);
                }

                //LEFT
                if (!UIManager.SkinTextures.ContainsKey("leftborder"))
                {
                    gfx.DrawRectangle(0, titleheight, leftborderwidth, Height - titleheight - bottomborderwidth, borderleftcolor);
                }
                else
                {
                    gfx.DrawRectangle(0, titleheight, leftborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["leftborder"]);
                }

            //RIGHT
            if (!UIManager.SkinTextures.ContainsKey("rightborder"))
            {
                gfx.DrawRectangle(brightlocx, titleheight, rightborderwidth, Height - titleheight - bottomborderwidth, borderrightcolor);
            }
            else
            {
                gfx.DrawRectangle(brightlocx, titleheight, rightborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["rightborder"]);
            }

            

            gfx.DrawRectangle(leftborderwidth, titleheight, Width - leftborderwidth - rightborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["ControlColor"]);
            //So here's what we're gonna do now.
            //Now that we have a titlebar and window borders...
            //We're going to composite the hosted window
            //and draw it to the remaining area.

            //Painting of the canvas is done by the Paint() method.
        }

    }

    public static class ImageExtensioons
    {
        public static Texture2D ToTexture2D(this Image image, GraphicsDevice device)
        {
            var bmp = (Bitmap)image;
            var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = new byte[Math.Abs(lck.Stride) * lck.Height];
            Marshal.Copy(lck.Scan0, data, 0, data.Length);
            bmp.UnlockBits(lck);
            for(int i = 0; i < data.Length; i += 4)
            {
                byte r = data[i];
                byte b = data[i + 2];
                data[i] = b;
                data[i + 2] = r;
            }
            var tex2 = new Texture2D(device, bmp.Width, bmp.Height);
            tex2.SetData<byte>(data);
            return tex2;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SidePanel : Attribute
    {
        
    }
}
