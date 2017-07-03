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
            act?.Invoke();
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

        public override void SetupWindow(IShiftOSWindow win)
        {
            if (!Shiftorium.UpgradeAttributesUnlocked(win.GetType()))
            {
                Console.WriteLine("Application not found on system.");
                return;
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

        }
    }

    public class WindowBorder : GUI.Control, IWindowBorder
    {
        private string _text = "ShiftOS window";
        private GUI.Control _hostedwindow = null;

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

        public override void Paint(Graphics gfx)
        {
            int titleheight = LoadedSkin.TitlebarHeight;
            int leftborderwidth = LoadedSkin.LeftBorderWidth;
            int rightborderwidth = LoadedSkin.RightBorderWidth;
            int bottomborderwidth = LoadedSkin.BottomBorderWidth;

            if (Shiftorium.UpgradeInstalled("wm_titlebar") || true)
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
                    var leftimage = GetImage("titlebarleft");
                    var rightimage = GetImage("titlebarright");
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
                if(Shiftorium.UpgradeInstalled("close_button") || true)
                {
                    var closebuttoncolor = LoadedSkin.CloseButtonColor;
                    var closebuttonsize = LoadedSkin.CloseButtonSize;
                    var closebuttonright = LoadedSkin.CloseButtonFromSide;

                    gfx.FillRectangle(new SolidBrush(closebuttoncolor), new Rectangle(closebuttonright, closebuttonsize)); 

                }
            }
            else
            {
                //Set the titleheight to 0.
                titleheight = 0;

            }

            //So here's what we're gonna do now.
            //Now that we have a titlebar and window borders...
            //We're going to composite the hosted window
            //and draw it to the remaining area.

            //First let's GET the window.
            if(_hostedwindow != null)
            {
                var win = _hostedwindow;
                //Now let's create a new bitmap to draw onto, the same size as the client area.
                using(var bmp = new Bitmap(Width, Height - titleheight))
                {
                    //Now, let's create a graphics object.
                    using(var cgfx = Graphics.FromImage(bmp))
                    {
                        //And composite...
                        win.Paint(cgfx);

                    }
                    //Now draw the bitmap to our client area
                    gfx.DrawImage(bmp, 0, titleheight);
                    //We now have a full window.
                }
            }
        }

    }
}
