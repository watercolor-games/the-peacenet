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
using Microsoft.Xna.Framework.Input;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;


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
        public PlexSkin LoadedSkin
        {
            get
            {
                return (PlexSkin)SkinEngine.LoadedSkin;
            }
        }

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
                if (brdr.Close())
                {
                    RunningBorders.Remove(brdr);
                    if (AppearanceManager.OpenForms.Contains(brdr))
                    {
                        AppearanceManager.OpenForms.Remove(brdr);
                        Engine.Desktop.ResetPanelButtons();
                    }
                    win = null;
                }
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
            if (UIManagerTools.InTextMode)
            {
                Console.WriteLine("You can't run this program in textmode.");
                return;
            }

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
                return 0;
            }
        }

        public override void SetupWindow(IPlexWindow win)
        {
            if (UIManagerTools.InTextMode)
            {
                Console.WriteLine("You can't run this program in textmode.");
                return;
            }


            if (UIManagerTools.InProtectedGUI)
            {
                Engine.Infobox.Show("Protected GUI", "You can't open this program right now - you are in protected GUI mode.");
                return;
            }
            if (!Upgrades.UpgradeAttributesUnlocked(win.GetType()))
            {
                Console.WriteLine("Application not found on system.");
                return;
            }
            var wb = new WindowBorder();
            wb.Text = GetTitle(win);
            wb.Width = (win as GUI.Control).Width + LoadedSkin.LeftBorderWidth + LoadedSkin.RightBorderWidth;
            wb.Height = (win as GUI.Control).Height + LoadedSkin.TitlebarHeight + LoadedSkin.BottomBorderWidth;
            wb.ParentWindow = win;
            wb.IsDialog = false;
            wb.X = (UIManager.Viewport.Width - wb.Width) / 2;
            wb.Y = (UIManager.Viewport.Height - wb.Height) / 2;

            UIManager.AddTopLevel(wb);
            AppearanceManager.OpenForms.Add(wb);
            RunningBorders.Add(wb);
            win.OnLoad();
            win.OnUpgrade();
            win.OnSkinLoad();
        }

    }

    public class WindowBorder : GUI.TextControl, IWindowBorder
    {
        public PlexSkin LoadedSkin
        {
            get
            {
                return (PlexSkin)SkinEngine.LoadedSkin;
            }
        }


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

        private void Upgrades_Installed()
        {
            ParentWindow.OnUpgrade();
        }

        public WindowBorder()
        {
            Upgrades.Installed += Upgrades_Installed;
            //Enforce the 800x600 window rule.
            MaxWidth = 800;
            MaxHeight = 600;
            MinWidth = 100;
            MinHeight = 100;
            this.MouseDown += () =>
            {
                var mstate = Mouse.GetState();
                moving = (mstate.LeftButton == ButtonState.Pressed && mstate.Y >= Y && mstate.Y <= Y + LoadedSkin.TitlebarHeight && mstate.X >= X && mstate.X <= X + Width);
                CaptureMouse = true;
                dist_x = Mouse.GetState().X - X;
                dist_y = Mouse.GetState().Y - Y;
            };
            MouseUp += () =>
            {
                moving = false;
                CaptureMouse = false;
            };
            Click += () =>
            {
                var cbtnloc = LoadedSkin.CloseButtonFromSide;
                var cbtnsize = LoadedSkin.CloseButtonSize;
                var realcloc = new Vector2(
                        (LoadedSkin.TitleButtonPosition == 1) ? cbtnloc.X : (Width - LoadedSkin.TitleRightCornerWidth) - cbtnsize.Width,
                        cbtnloc.Y
                    );
                if(MouseX >= realcloc.X && MouseY >= realcloc.Y && MouseX <= realcloc.X + cbtnsize.Width && MouseY <= realcloc.Y + cbtnsize.Height)
                {
                    AppearanceManager.Close(ParentWindow);
                }
            };
            X = 720;
            Y = 480;
        }

        private bool moving = false;

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

        protected override void RenderText(GraphicsContext gfx)
        {
            var titlefont = LoadedSkin.TitleFont;
            var titletextcolor = LoadedSkin.TitleTextColor;
            var titletextleft = LoadedSkin.TitleTextLeft;
            bool titletextcentered = LoadedSkin.TitleTextCentered;
            int titlebarleft = 0;
            int titlebarwidth = Width;

            //Now we draw the title text.
            var textMeasure = GraphicsContext.MeasureString(Text, titlefont, Engine.GUI.TextAlignment.TopLeft);
            PointF textloc;
            if (titletextcentered)
                textloc = new PointF((titlebarwidth - textMeasure.X) / 2,
                    titletextleft.Y);
            else
                textloc = new PointF(titlebarleft + titletextleft.X, titletextleft.Y);

            gfx.DrawString(Text, (int)textloc.X, (int)textloc.Y, titletextcolor.ToMonoColor(), titlefont, Engine.GUI.TextAlignment.TopLeft);

        }

        public bool Close()
        {
            if (!ParentWindow.OnUnload())
                return false;
            Upgrades.Installed -= Upgrades_Installed;
            Visible = false;
            UIManager.StopHandling(this);
            return true;
        }


        protected override void OnLayout(GameTime gameTime)
        {
            if (UIManagerTools.InProtectedGUI && !IsDialog)
            {
                Visible = false;
                return;
            }
            else
            {
                Visible = true;
            }
            if (IsFocusedControl || ContainsFocusedControl)
            {
                UIManager.BringToFront(this);
            }
            var mstate = Mouse.GetState();
            if (moving)
            {
                X = mstate.X - dist_x;
                Y = mstate.Y - dist_y;
            }

            int titlebarheight = LoadedSkin.TitlebarHeight;
            int borderleft = LoadedSkin.LeftBorderWidth;
            int borderright = LoadedSkin.RightBorderWidth;
            int borderbottom = LoadedSkin.BottomBorderWidth;
            int maxwidth = (MaxWidth - LoadedSkin.LeftBorderWidth) - LoadedSkin.RightBorderWidth;
            int maxheight = (MaxHeight - LoadedSkin.TitlebarHeight) - LoadedSkin.BottomBorderWidth;
            _hostedwindow.MaxWidth = maxwidth;
            _hostedwindow.MaxHeight = maxheight;
            _hostedwindow.X = borderleft;
            _hostedwindow.Y = titlebarheight;
            Width = borderleft + _hostedwindow.Width + LoadedSkin.RightBorderWidth;
            Height = titlebarheight + _hostedwindow.Height + LoadedSkin.BottomBorderWidth;

        }



        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            try
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
                    if (UIManager.SkinTextures.ContainsKey("titleleft"))
                    {
                        gfx.DrawRectangle(0, 0, leftwidth, titleheight, UIManager.SkinTextures["titleleft"], SkinEngine.GetImageLayout("titleleft"));
                    }
                    else
                    {
                        gfx.DrawRectangle(0, 0, leftwidth, titleheight, leftcolor);
                    }

                    //draw right corner
                    if (UIManager.SkinTextures.ContainsKey("titleright"))
                    {
                        gfx.DrawRectangle(titlebarleft + titlebarwidth, 0, rightwidth, titleheight, UIManager.SkinTextures["titleright"], SkinEngine.GetImageLayout("titleright"));
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
                    gfx.DrawRectangle(titlebarleft, 0, titlebarwidth, titleheight, UIManager.SkinTextures["titlebar"], SkinEngine.GetImageLayout("titlebar"));
                }

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
                    gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["closebutton"], SkinEngine.GetImageLayout("closebutton"));
                }
                if (!IsDialog)
                {
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
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["maximizebutton"], SkinEngine.GetImageLayout("maximizebutton"));
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
                        gfx.DrawRectangle(closebuttonright.X, closebuttonright.Y, closebuttonsize.Width, closebuttonsize.Height, UIManager.SkinTextures["minimizebutton"], SkinEngine.GetImageLayout("minimizebutton"));
                    }
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
                    gfx.DrawRectangle(0, bottomlocy, leftborderwidth, bottomborderwidth, UIManager.SkinTextures["bottomlborder"], SkinEngine.GetImageLayout("bottomlborder"));
                }

                //BOTTOM RIGHT
                if (!UIManager.SkinTextures.ContainsKey("bottomrborder"))
                {
                    gfx.DrawRectangle(brightlocx, bottomlocy, rightborderwidth, bottomborderwidth, borderbrightcolor);
                }
                else
                {
                    gfx.DrawRectangle(brightlocx, bottomlocy, rightborderwidth, bottomborderwidth, UIManager.SkinTextures["bottomrborder"], SkinEngine.GetImageLayout("bottomrborder"));
                }

                //BOTTOM
                if (!UIManager.SkinTextures.ContainsKey("bottomborder"))
                {
                    gfx.DrawRectangle(leftborderwidth, bottomlocy, bottomwidth, bottomborderwidth, borderbottomcolor);
                }
                else
                {
                    gfx.DrawRectangle(leftborderwidth, bottomlocy, bottomwidth, bottomborderwidth, UIManager.SkinTextures["bottomborder"], SkinEngine.GetImageLayout("bottomborder"));
                }

                //LEFT
                if (!UIManager.SkinTextures.ContainsKey("leftborder"))
                {
                    gfx.DrawRectangle(0, titleheight, leftborderwidth, Height - titleheight - bottomborderwidth, borderleftcolor);
                }
                else
                {
                    gfx.DrawRectangle(0, titleheight, leftborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["leftborder"], SkinEngine.GetImageLayout("leftborder"));
                }

                //RIGHT
                if (!UIManager.SkinTextures.ContainsKey("rightborder"))
                {
                    gfx.DrawRectangle(brightlocx, titleheight, rightborderwidth, Height - titleheight - bottomborderwidth, borderrightcolor);
                }
                else
                {
                    gfx.DrawRectangle(brightlocx, titleheight, rightborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["rightborder"], SkinEngine.GetImageLayout("rightborder"));
                }



                gfx.DrawRectangle(leftborderwidth, titleheight, Width - leftborderwidth - rightborderwidth, Height - titleheight - bottomborderwidth, UIManager.SkinTextures["ControlColor"]);
                //So here's what we're gonna do now.
                //Now that we have a titlebar and window borders...
                //We're going to composite the hosted window
                //and draw it to the remaining area.
            }
            catch (KeyNotFoundException)
            {
                UIManager.ResetSkinTextures(gfx.Device);
                OnPaint(gfx, target);
            }
            //Painting of the canvas is done by the Paint() method.
            base.OnPaint(gfx, target);
        }

        int dist_x = 0;
        int dist_y = 0;

        public override void MouseStateChanged()
        {
            base.MouseStateChanged();
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SidePanel : Attribute
    {
        
    }
}
