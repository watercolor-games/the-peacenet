using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            while(AppearanceManager.OpenForms.Count > MaxCount)
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
            _hostedwindow.Height = width - bottomheight - titleheight;
            Width = width;
            Height = height;

        }

        public WindowBorder()
        {
            X = 720;
            Y = 480;
        }

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

        private bool moving = false;

        public override void MouseStateChanged()
        {
            if (Shiftorium.UpgradeInstalled("wm_titlebar"))
            {
                if (Shiftorium.UpgradeInstalled("close_button"))
                {
                    var closebuttonsize = LoadedSkin.CloseButtonSize;
                    var closebuttonloc = LoadedSkin.CloseButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonloc = new Point(Width - closebuttonsize.Width - closebuttonloc.X, closebuttonloc.Y);
                    if(MouseX > closebuttonloc.X && MouseY > closebuttonloc.Y && MouseX < closebuttonloc.X + closebuttonsize.Width && MouseY < closebuttonloc.Y + closebuttonsize.Height)
                    {
                        Close();
                    }
                }
                if (Shiftorium.UpgradeInstalled("minimize_button"))
                {
                    var closebuttonsize = LoadedSkin.MinimizeButtonSize;
                    var closebuttonloc = LoadedSkin.MinimizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonloc = new Point(Width - closebuttonsize.Width - closebuttonloc.X, closebuttonloc.Y);
                    if (MouseX > closebuttonloc.X && MouseY > closebuttonloc.Y && MouseX < closebuttonloc.X + closebuttonsize.Width && MouseY < closebuttonloc.Y + closebuttonsize.Height)
                    {
                        if (IsFocusedControl || ContainsFocusedControl)
                            UIManager.FocusedControl = null;
                        Visible = false;
                    }
                }
                if (Shiftorium.UpgradeInstalled("maximize_button"))
                {
                    var closebuttonsize = LoadedSkin.MaximizeButtonSize;
                    var closebuttonloc = LoadedSkin.MaximizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonloc = new Point(Width - closebuttonsize.Width - closebuttonloc.X, closebuttonloc.Y);
                    if (MouseX > closebuttonloc.X && MouseY > closebuttonloc.Y && MouseX < closebuttonloc.X + closebuttonsize.Width && MouseY < closebuttonloc.Y + closebuttonsize.Height)
                    {
                        AppearanceManager.Maximize(this);
                    }
                }
                if (MouseY < LoadedSkin.TitlebarHeight)
                {
                    var screenpoint = PointToScreen(MouseX, MouseY);
                    lastmousex = this.X - screenpoint.X;
                    lastmousey = this.Y - screenpoint.Y;

                    moving = MouseLeftDown;
                    CaptureMouse = moving;
                }
            }
        }

        protected override void OnPaint(Graphics gfx)
        {
            int titleheight = LoadedSkin.TitlebarHeight;
            int leftborderwidth = LoadedSkin.LeftBorderWidth;
            int rightborderwidth = LoadedSkin.RightBorderWidth;
            int bottomborderwidth = LoadedSkin.BottomBorderWidth;

            if (Shiftorium.UpgradeInstalled("wm_titlebar"))
            {
                var titlebarcolor = LoadedSkin.TitleBackgroundColor;
                var titlefont = LoadedSkin.TitleFont;
                var titletextcolor = LoadedSkin.TitleTextColor;
                var titletextleft = LoadedSkin.TitleTextLeft;
                bool titletextcentered = LoadedSkin.TitleTextCentered;

                 var titlebarbg = GetImage("titlebar");
                var titlebarlayout = GetImageLayout("titlebar");

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
                    var leftimage = GetImage("titleleft");
                    var rightimage = GetImage("titleright");
                    //and the colors
                    var leftcolor = LoadedSkin.TitleLeftCornerBackground;
                    var rightcolor = LoadedSkin.TitleRightCornerBackground;
                    //and the layouts...
                    var leftlayout = GetImageLayout("titlebarleft");
                    var rightlayout = GetImageLayout("titlebarright");
                    //and the widths
                    var leftwidth = LoadedSkin.TitleLeftCornerWidth;
                    var rightwidth = LoadedSkin.TitleRightCornerWidth;

                    //draw left corner
                    if(leftimage != null)
                    {
                        var resized = ResizeImage(leftimage, leftwidth, titleheight);
                        gfx.DrawImage(resized, 0, 0);
                    }
                    else
                    {
                        gfx.FillRectangle(new SolidBrush(leftcolor), new Rectangle(0, 0, leftwidth, titleheight));
                    }

                    //draw right corner
                    if (rightimage != null)
                    {
                        var resized = ResizeImage(rightimage, rightwidth, titleheight);
                        gfx.DrawImage(resized, titlebarleft+titlebarwidth, 0);
                    }
                    else
                    {
                        gfx.FillRectangle(new SolidBrush(rightcolor), new Rectangle(titlebarleft+titlebarwidth, 0, rightwidth, titleheight));
                    }
                }

                if (titlebarbg == null)
                {
                    //draw the title bg
                    gfx.FillRectangle(new SolidBrush(titlebarcolor), new Rectangle(titlebarleft, 0, titlebarwidth, titleheight));


                }
                else
                {
                    var resized = ResizeImage(titlebarbg, titlebarwidth, titleheight);
                    gfx.DrawImage(resized, titlebarleft, 0);
                }
                //Now we draw the title text.
                var textMeasure = gfx.MeasureString(_text, titlefont);
                PointF textloc;
                if (titletextcentered)
                    textloc = new PointF((titlebarwidth - textMeasure.Width) / 2,
                        titletextleft.Y);
                else
                    textloc = new PointF(titlebarleft + titletextleft.X, titletextleft.Y);

                gfx.DrawString(_text, titlefont, new SolidBrush(titletextcolor), textloc);

                var tbuttonpos = LoadedSkin.TitleButtonPosition;

                //Draw close button
                if(Shiftorium.UpgradeInstalled("close_button"))
                {
                    var closebuttoncolor = LoadedSkin.CloseButtonColor;
                    var closebuttonsize = LoadedSkin.CloseButtonSize;
                    var closebuttonright = LoadedSkin.CloseButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
                    var img = GetImage("closebutton");
                    if (img == null)
                    {
                        gfx.FillRectangle(new SolidBrush(closebuttoncolor), new Rectangle(closebuttonright, closebuttonsize));
                    }
                    else
                    {
                        var resized = ResizeImage(img, closebuttonsize.Width, closebuttonsize.Height);
                        gfx.DrawImage(resized, closebuttonright);
                    }
                }
                //Draw maximize button
                if (Shiftorium.UpgradeInstalled("maximize_button"))
                {
                    var closebuttoncolor = LoadedSkin.MaximizeButtonColor;
                    var closebuttonsize = LoadedSkin.MaximizeButtonSize;
                    var closebuttonright = LoadedSkin.MaximizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);

                    gfx.FillRectangle(new SolidBrush(closebuttoncolor), new Rectangle(closebuttonright, closebuttonsize));
                    var img = GetImage("maximizebutton");
                    if (img == null)
                    {
                        gfx.FillRectangle(new SolidBrush(closebuttoncolor), new Rectangle(closebuttonright, closebuttonsize));
                    }
                    else
                    {
                        var resized = ResizeImage(img, closebuttonsize.Width, closebuttonsize.Height);
                        gfx.DrawImage(resized, closebuttonright);
                    }

                }
                //Draw minimize button
                if (Shiftorium.UpgradeInstalled("minimize_button"))
                {
                    var closebuttoncolor = LoadedSkin.MinimizeButtonColor;
                    var closebuttonsize = LoadedSkin.MinimizeButtonSize;
                    var closebuttonright = LoadedSkin.MinimizeButtonFromSide;
                    if (LoadedSkin.TitleButtonPosition == 0)
                        closebuttonright = new Point(Width - closebuttonsize.Width - closebuttonright.X, closebuttonright.Y);
                    var img = GetImage("minimizebutton");
                    if (img == null)
                    {
                        gfx.FillRectangle(new SolidBrush(closebuttoncolor), new Rectangle(closebuttonright, closebuttonsize));
                    }
                    else
                    {
                        var resized = ResizeImage(img, closebuttonsize.Width, closebuttonsize.Height);
                        gfx.DrawImage(resized, closebuttonright);
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

                var borderleftcolor = (ContainsFocusedControl || IsFocusedControl) ? LoadedSkin.BorderLeftBackground : LoadedSkin.BorderInactiveLeftBackground;
                var borderrightcolor = (ContainsFocusedControl || IsFocusedControl) ? LoadedSkin.BorderRightBackground : LoadedSkin.BorderInactiveRightBackground;
                var borderbottomcolor = (ContainsFocusedControl || IsFocusedControl) ? LoadedSkin.BorderBottomBackground : LoadedSkin.BorderInactiveBottomBackground;
                var borderbleftcolor = (ContainsFocusedControl || IsFocusedControl) ? LoadedSkin.BorderBottomLeftBackground : LoadedSkin.BorderInactiveBottomLeftBackground;
                var borderbrightcolor = (ContainsFocusedControl || IsFocusedControl) ? LoadedSkin.BorderBottomRightBackground : LoadedSkin.BorderInactiveBottomRightBackground;


                //draw border corners
                //BOTTOM LEFT
                var bottomlimg = GetImage("bottomlborder");
                if (bottomlimg == null)
                {
                    gfx.FillRectangle(new SolidBrush(borderbleftcolor), new Rectangle(0, bottomlocy, leftborderwidth, bottomborderwidth));
                }
                else
                {
                    bottomlimg = ResizeImage(bottomlimg, leftborderwidth, bottomborderwidth);
                    gfx.DrawImage(bottomlimg, 0, bottomlocy);
                }

                //BOTTOM RIGHT
                var bottomrimg = GetImage("bottomrborder");
                if (bottomrimg == null)
                {
                    gfx.FillRectangle(new SolidBrush(borderbrightcolor), new Rectangle(brightlocx, bottomlocy, rightborderwidth, bottomborderwidth));
                }
                else
                {
                    bottomrimg = ResizeImage(bottomrimg, rightborderwidth, bottomborderwidth);
                    gfx.DrawImage(bottomrimg, brightlocx, bottomlocy);
                }

                //BOTTOM
                var bottomimg = GetImage("bottomborder");
                if (bottomimg == null)
                    gfx.FillRectangle(new SolidBrush(borderbottomcolor), new Rectangle(bottomlocx, bottomlocy, bottomwidth, bottomborderwidth));
                else
                {
                    bottomimg = ResizeImage(bottomimg, bottomwidth, bottomborderwidth);
                    gfx.DrawImage(bottomimg, bottomlocx, bottomlocy);
                }

                //LEFT
                var leftimg = GetImage("leftborder");
                if (leftimg == null)
                    gfx.FillRectangle(new SolidBrush(borderleftcolor), new Rectangle(0, titleheight, leftborderwidth, Height - titleheight - bottomborderwidth));
                else
                {
                    leftimg = ResizeImage(leftimg, leftborderwidth, Height - bottomborderwidth - titleheight);
                    gfx.DrawImage(leftimg, 0, titleheight);
                }

                //RIGHT
                var rightimg = GetImage("rightborder");
                if (rightimg == null)
                {
                    gfx.FillRectangle(new SolidBrush(borderrightcolor), new Rectangle(brightlocx, titleheight, rightborderwidth, Height - titleheight - bottomborderwidth));
                }
                else
                {
                    rightimg = ResizeImage(rightimg, rightborderwidth, Height - titleheight - bottomborderwidth);
                    gfx.DrawImage(rightimg, brightlocx, titleheight);
                }
            }

            //So here's what we're gonna do now.
            //Now that we have a titlebar and window borders...
            //We're going to composite the hosted window
            //and draw it to the remaining area.

            //Painting of the canvas is done by the Paint() method.
        }

    }
}
