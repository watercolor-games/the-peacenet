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
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.Theming;

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
                return 0;
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

        public bool DisplayObsolescenceIfAny(IPlexWindow win)
        {
            if (!(win is InfoboxMessage))
            {
                var attrib = win.GetType().GetCustomAttributes(true).FirstOrDefault(x => x is ObsoleteAttribute) as ObsoleteAttribute;

                if(attrib != null)
                {
                    Engine.Infobox.Show($"{win.GetType().Name} is obsolete.", attrib.Message);
                    return true;
                }
            }
            return false;
        }

        public override void SetupDialog(IPlexWindow win)
        {
            if (DisplayObsolescenceIfAny(win))
                return;
            var wb = new WindowBorder();
            wb.Text = GetTitle(win);
            var ctl = win as Control;
            if (ctl.Width < 30)
                ctl.Width = 30;
            if (ctl.Height < 30)
                ctl.Height = 30;
            wb.Width = (win as Control).Width + 4;
            wb.Height = (win as Control).Height + 32;
            wb.ParentWindow = win;
            wb.IsDialog = true;
            UIManager.AddTopLevel(wb);
            RunningBorders.Add(wb);
            wb.X = (UIManager.Viewport.Width - wb.Width) / 2;
            wb.Y = (UIManager.Viewport.Height - wb.Height) / 2;
            win.OnLoad();
            win.OnUpgrade();
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
            bool isSingleInstance = win.GetType().GetCustomAttributes(false).FirstOrDefault(x => x is SingleInstanceAttribute) != null;

            if (!Upgrades.UpgradeAttributesUnlocked(win.GetType()))
            {
                Console.WriteLine("Application not found on system.");
                return;
            }
            if (DisplayObsolescenceIfAny(win))
                return;
            if (isSingleInstance)
            {
                var alreadyOpen = AppearanceManager.OpenForms.FirstOrDefault(x => x.ParentWindow.GetType() == win.GetType());
                if (alreadyOpen != null)
                {
                    var aoWB = ((WindowBorder)alreadyOpen);
                    if (!aoWB.Visible)
                        aoWB.ToggleMinimized();
                    aoWB.BringToFront();
                    UIManager.FocusedControl = aoWB;
                    return;
                }
            }
            var wb = new WindowBorder();
            wb.Text = GetTitle(win);
            wb.Width = (win as Control).Width + 4;
            wb.Height = (win as Control).Height + 32;
            wb.ParentWindow = win;
            wb.IsDialog = false;
            wb.X = (UIManager.Viewport.Width - wb.Width) / 2;
            wb.Y = (UIManager.Viewport.Height - wb.Height) / 2;

            UIManager.AddTopLevel(wb);
            AppearanceManager.OpenForms.Add(wb);
            RunningBorders.Add(wb);
            win.OnLoad();
            win.OnUpgrade();
        }

    }

    public class WindowBorder : TextControl, IWindowBorder
    {
        private Engine.Theming.ButtonState _closeState = Engine.Theming.ButtonState.Idle;
        private Engine.Theming.ButtonState _minState = Engine.Theming.ButtonState.Idle;
        private Engine.Theming.ButtonState _maxState = Engine.Theming.ButtonState.Idle;

        private bool showMaximize = false;

        private bool _maximized = false;
        private int _normalx = 0;
        private int _normaly = 0;
        private int _normalw = 0;
        private int _normalh = 0;

        public void ToggleMaximized()
        {
            if(_maximized == false)
            {
                _normalx = X;
                _normaly = Y;
                _normalw = Width;
                _normalh = Height;
                MaxWidth = int.MaxValue;
                MaxHeight = int.MaxValue;
                X = 0;
                Y = 24;
                Width = UIManager.Viewport.Width;
                Height = UIManager.Viewport.Height - 24;
                _maximized = true;
            }
            else
            {
                X = _normalx;
                Y = _normaly;
                Width = _normalw;
                Height = _normalh;
                MaxWidth = 800;
                MaxHeight = 600;
                _maximized = false;
            }
            ResetChildWindowSize();
        }

        private bool _minimized = false;

        public void ToggleMinimized()
        {
            _minimized = !_minimized;
        }

        public void ResetChildWindowSize()
        {
            int titlebar = 30;
            int bleft = 2;
            int bright = 2;
            int bbottom = 2;

            _hostedwindow.X = bleft;
            _hostedwindow.Y = titlebar;
            _hostedwindow.MaxWidth = Width - bright - bleft;
            _hostedwindow.MaxHeight = Height - bbottom - titlebar;
            _hostedwindow.Width = _hostedwindow.MaxWidth;
            _hostedwindow.Height = _hostedwindow.MaxHeight;

        }

        private Control _hostedwindow = null;

        public void ResizeWindow(int width, int height)
        {
            int titleheight = 30;
            int leftwidth = 2;
            int bottomheight = 2;
            int rightwidth = 2;
            _hostedwindow.Width = width - leftwidth - rightwidth;
            _hostedwindow.Height = height - bottomheight - titleheight;
            Width = width;
            Height = height;
        }

        private void Upgrades_Installed()
        {
            bool max = Upgrades.IsLoaded("maximizable_windows");
            if(max != showMaximize)
            {
                showMaximize = max;
                Invalidate();
            }
            ParentWindow?.OnUpgrade();
        }

        public WindowBorder()
        {
            Upgrades.Installed += Upgrades_Installed;
            Upgrades_Installed();
            //Enforce the 800x600 window rule.
            MaxWidth = 800;
            MaxHeight = 600;
            MinWidth = 100;
            MinHeight = 100;
            this.MouseDown += () =>
            {
                var mstate = Mouse.GetState();
                moving = (mstate.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && mstate.Y >= Y && mstate.Y <= Y + 30 && mstate.X >= X && mstate.X <= X + Width);
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
                var _crect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Close, Width, Height);
                if (MouseX >= _crect.X && MouseX <= _crect.X + _crect.Width && MouseY >= _crect.Y && MouseY <= _crect.Y + _crect.Height)
                {
                    if (MouseLeftDown)
                    {
                        Close();
                    }
                }
                if (!IsDialog)
                {
                    var _mxrect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Maximize, Width, Height);
                    if (MouseX >= _mxrect.X && MouseX <= _mxrect.X + _mxrect.Width && MouseY >= _mxrect.Y && MouseY <= _mxrect.Y + _mxrect.Height)
                    {
                        if (MouseLeftDown && showMaximize)
                        {
                            ToggleMaximized();
                            return;
                        }
                    }
                    var _mnrect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Minimize, Width, Height);
                    if (MouseX >= _mnrect.X && MouseX <= _mnrect.X + _mnrect.Width && MouseY >= _mnrect.Y && MouseY <= _mnrect.Y + _mxrect.Height)
                    {
                        if (MouseLeftDown)
                        {
                            ToggleMinimized();
                        }
                    }

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
                _hostedwindow = (Control)value;
                ClearControls();
                AddControl(_hostedwindow);
                Width = 2 + _hostedwindow.Width + 2;
                Height = 2 + _hostedwindow.Height + 30;

            }
        }

        public bool IsDialog { get; set; }

        protected override void RenderText(GraphicsContext gfx)
        {
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
            var _cstate = Engine.Theming.ButtonState.Idle;
            var _mxstate = Engine.Theming.ButtonState.Idle;
            var _mnstate = Engine.Theming.ButtonState.Idle;

            var _crect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Close, Width, Height);
            if(MouseX >= _crect.X && MouseX <= _crect.X + _crect.Width && MouseY >= _crect.Y && MouseY <= _crect.Y + _crect.Height)
            {
                _cstate = Engine.Theming.ButtonState.MouseHover;
                if (MouseLeftDown)
                    _cstate = Engine.Theming.ButtonState.MouseDown;
            }
            if (!IsDialog)
            {
                var _mxrect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Maximize, Width, Height);
                if (MouseX >= _mxrect.X && MouseX <= _mxrect.X + _mxrect.Width && MouseY >= _mxrect.Y && MouseY <= _mxrect.Y + _mxrect.Height && showMaximize)
                {
                    _mxstate = Engine.Theming.ButtonState.MouseHover;
                    if (MouseLeftDown)
                        _mxstate = Engine.Theming.ButtonState.MouseDown;
                }
                var _mnrect = ThemeManager.Theme.GetTitleButtonRectangle(TitleButton.Minimize, Width, Height);
                if (MouseX >= _mnrect.X && MouseX <= _mnrect.X + _mnrect.Width && MouseY >= _mnrect.Y && MouseY <= _mnrect.Y + _mxrect.Height)
                {
                    _mnstate = Engine.Theming.ButtonState.MouseHover;
                    if (MouseLeftDown)
                        _mnstate = Engine.Theming.ButtonState.MouseDown;
                }

            }

            if(_closeState != _cstate || _minState != _mnstate || _maxState != _mxstate)
            {
                Invalidate();
            }

            _closeState = _cstate;
            _minState = _mnstate;
            _maxState = _mxstate;

            FontStyle = TextControlFontStyle.Custom;
            TextColor = Microsoft.Xna.Framework.Color.White;
            if (_minimized == true)
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            
                if (IsFocusedControl || ContainsFocusedControl)
                {
                    UIManager.BringToFront(this);
                }
                var mstate = Mouse.GetState();
                if (moving && _maximized == false)
                {
                    X = mstate.X - dist_x;
                    Y = mstate.Y - dist_y;
                }

                int titlebarheight = 30;
                int borderleft = 2;
                int borderright = 2;
                int borderbottom = 2;
                int maxwidth = (MaxWidth - borderleft) - borderright;
                int maxheight = (MaxHeight - titlebarheight) - borderbottom;
                _hostedwindow.MaxWidth = maxwidth;
                _hostedwindow.MaxHeight = maxheight;
                _hostedwindow.X = borderleft;
                _hostedwindow.Y = titlebarheight;
                Width = borderleft + _hostedwindow.Width + borderright;
                Height = titlebarheight + _hostedwindow.Height + borderbottom;

            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            ThemeManager.Theme.DrawWindowBorder(gfx, 0,0,Width, Height, ContainsFocusedControl||IsFocusedControl, _maximized, _closeState, _maxState, _minState, IsDialog);
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
