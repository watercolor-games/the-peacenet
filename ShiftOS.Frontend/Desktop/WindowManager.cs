using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.Desktop
{
    public class WindowManager : Engine.WindowManager
    {
        public override void Close(IShiftOSWindow win)
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

        public override void SetTitle(IShiftOSWindow win, string title)
        {
            var brdr = RunningBorders.FirstOrDefault(x => x.ParentWindow == win);
            if (brdr != null)
                brdr.Text = title;
        }

        public override void SetupDialog(IShiftOSWindow win)
        {
            var wb = new WindowBorder();
            wb.Width = (win as GUI.Control).Width + LoadedSkin.LeftBorderWidth + LoadedSkin.RightBorderWidth;
            wb.Height = (win as GUI.Control).Height + LoadedSkin.TitlebarHeight + LoadedSkin.BottomBorderWidth;
            wb.ParentWindow = win;
            wb.IsDialog = true;
            UIManager.AddTopLevel(wb);
            RunningBorders.Add(wb);
            win.OnLoad();
            win.OnUpgrade();
            win.OnSkinLoad();
        }

        private int MaxCount
        {
            get
            {
                if (Shiftorium.UpgradeInstalled("wm_unlimited_windows"))
                    return int.MaxValue;
                if (Shiftorium.UpgradeInstalled("wm_4_windows"))
                    return 4;
                if (Shiftorium.UpgradeInstalled("wm_2_windows"))
                    return 2;
                return 1;
            }
        }

        public override void SetupWindow(IShiftOSWindow win)
        {
            if (!Shiftorium.UpgradeAttributesUnlocked(win.GetType()))
            {
                Console.WriteLine("Application not found on system.");
                return;
            }
            while(AppearanceManager.OpenForms.Count >= MaxCount)
            {
                AppearanceManager.OpenForms[0].Close();
                AppearanceManager.OpenForms.RemoveAt(0);
            }
            var wb = new WindowBorder();
            wb.Width = (win as GUI.Control).Width + LoadedSkin.LeftBorderWidth + LoadedSkin.RightBorderWidth;
            wb.Height = (win as GUI.Control).Height + LoadedSkin.TitlebarHeight + LoadedSkin.BottomBorderWidth;
            wb.ParentWindow = win;
            wb.IsDialog = true;
            UIManager.AddTopLevel(wb);
            AppearanceManager.OpenForms.Add(wb);
            RunningBorders.Add(wb);
            win.OnLoad();
            win.OnUpgrade();
            win.OnSkinLoad();
            if (!Shiftorium.UpgradeInstalled("wm_free_placement"))
            {
                TileWindows();
            }
        }

        public void TileWindows()
        {
            if (AppearanceManager.OpenForms.Count == 0)
                return;
            else if(AppearanceManager.OpenForms.Count == 1)
            {
                var wb = (WindowBorder)AppearanceManager.OpenForms[0];
                wb.X = 0;
                wb.Y = 0;
                wb.ResizeWindow(UIManager.Viewport.Width, UIManager.Viewport.Height);
            }
        }
    }

    public class WindowBorder : GUI.Control, IWindowBorder
    {
        private string _text = "ShiftOS window";
        private GUI.Control _hostedwindow = null;

        public void ResizeWindow(int width, int height)
        {
            int titleheight = Shiftorium.UpgradeInstalled("wm_titlebar") ? LoadedSkin.TitlebarHeight : 0;
            int leftwidth = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.LeftBorderWidth : 0;
            int bottomheight = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.BottomBorderWidth : 0;
            int rightwidth = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.RightBorderWidth : 0;
            _hostedwindow.Width = width - leftwidth - rightwidth;
            _hostedwindow.Height = height - bottomheight - titleheight;
            Width = width;
            Height = height;

        }

        public WindowBorder()
        {
            X = 720;
            Y = 480;
            MouseDown += () =>
            {
                var scnloc = PointToScreen(MouseX, MouseY);
                mouseprevx = scnloc.X;
                mouseprevy = scnloc.Y;
                moving = true;
            };
            MouseMove += (loc) =>
            {
                if (moving == true)
                {
                    var scnloc = PointToScreen(MouseX, MouseY);
                    int differencex = scnloc.X - mouseprevx;
                    int differencey = scnloc.Y - mouseprevy;
                    X -= differencex;
                    Y -= differencey;
                    mouseprevx = scnloc.X;
                    mouseprevy = scnloc.Y;
                }
            };
            MouseUp += () =>
            {
                moving = false;
            };
        }

        private bool moving = false;
        private int mouseprevx = 0;
        private int mouseprevy = 0;
        

        public IShiftOSWindow ParentWindow
        {
            get
            {
                return (IShiftOSWindow)_hostedwindow;
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

        protected override void OnLayout()
        {
            if (moving)
            {
                var screenpoint = PointToScreen(MouseX, MouseY);
                this.X = lastmousex + screenpoint.X;
                this.Y = lastmousey + screenpoint.Y;
            }
            int titlebarheight = Shiftorium.UpgradeInstalled("wm_titlebar") ? LoadedSkin.TitlebarHeight : 0;
            int borderleft = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.LeftBorderWidth : 0;
            int borderright = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.RightBorderWidth : 0;
            int borderbottom = Shiftorium.UpgradeInstalled("window_borders") ? LoadedSkin.BottomBorderWidth : 0;
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

            if (Shiftorium.UpgradeInstalled("wm_titlebar"))
            {
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
                var textMeasure = gfx.MeasureString(Text, titlefont);
                PointF textloc;
                if (titletextcentered)
                    textloc = new PointF((titlebarwidth - textMeasure.X) / 2,
                        titletextleft.Y);
                else
                    textloc = new PointF(titlebarleft + titletextleft.X, titletextleft.Y);

                gfx.DrawString(Text, (int)textloc.X, (int)textloc.Y, titletextcolor.ToMonoColor(), titlefont);

                var tbuttonpos = LoadedSkin.TitleButtonPosition;

                //Draw close button
                if(Shiftorium.UpgradeInstalled("close_button"))
                {
                    var closebuttonsize = LoadedSkin.CloseButtonSize;
                    var closebuttonright = LoadedSkin.CloseButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
                    if (!UIManager.SkinTextures.ContainsKey("closebutton"))
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["CloseButtonColor"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["closebutton"]);
                    }
                }
                //Draw maximize button
                if (Shiftorium.UpgradeInstalled("maximize_button"))
                {
                    var closebuttonsize = LoadedSkin.MaximizeButtonSize;
                    var closebuttonright = LoadedSkin.MaximizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);

                    if (!UIManager.SkinTextures.ContainsKey("maximizebutton"))
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["MaximizeButtonColor"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["maximizebutton"]);
                    }
                }
                //Draw minimize button
                if (Shiftorium.UpgradeInstalled("minimize_button"))
                {
                    var closebuttonsize = LoadedSkin.MinimizeButtonSize;
                    var closebuttonright = LoadedSkin.MinimizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
                    if (!UIManager.SkinTextures.ContainsKey("minimizebutton"))
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["MinimizeButtonColor"]);
                    }
                    else
                    {
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["minimizebutton"]);
                    }

                }
            }
            else
            {
                //Set the titleheight to 0.
                titleheight = 0;

            }

            if (Shiftorium.UpgradeInstalled("window_borders"))
            {
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
            var lck = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
}
