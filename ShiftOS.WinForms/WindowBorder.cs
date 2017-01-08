/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static ShiftOS.Engine.SkinEngine;
using System.Runtime.InteropServices;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    public partial class WindowBorder : Form, IWindowBorder
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            if ((ParentWindow as IShiftOSWindow).OnUnload())
            {
                if (!SaveSystem.ShuttingDown)
                {
                    if(Engine.AppearanceManager.OpenForms.Contains(this))
                        Engine.AppearanceManager.OpenForms.Remove(this);
                    Desktop.ResetPanelButtons();
                }
            }
            base.OnClosing(e);
        }
        private UserControl _parentWindow = null;

        public IShiftOSWindow ParentWindow
        {
            get
            {
                return (IShiftOSWindow)_parentWindow;
            }
            set
            {
                _parentWindow = (UserControl)value;
            }
        }

        public WindowBorder(UserControl win)
        {
            InitializeComponent();
            this._parentWindow = win;
            Shiftorium.Installed += () =>
            {
                try
                {
                    this.ParentForm.Invoke(new Action(() =>
                    {
                        Setup();
                    }));
                }
                catch { }
            };
            SkinEngine.SkinLoaded += () =>
            {
                try
                {
                    Setup();
                    (ParentWindow as IShiftOSWindow).OnSkinLoad();
                    ControlManager.SetupControl(this.pnlcontents);
                }
                catch
                {

                }
            };

            this.Width = LoadedSkin.LeftBorderWidth + _parentWindow.Width + LoadedSkin.RightBorderWidth;
            this.Height = LoadedSkin.TitlebarHeight + _parentWindow.Height + LoadedSkin.BottomBorderWidth;

            this.pnlcontents.Controls.Add(this._parentWindow);
            this._parentWindow.Dock = DockStyle.Fill;
            this._parentWindow.Show();
            ControlManager.SetupControls(this._parentWindow);


            Desktop.ShowWindow(this);

        }

        public static void Universal_KeyDown(object o, KeyEventArgs a)
        {
            if (a.Control && a.KeyCode == Keys.T)
            {
                a.SuppressKeyPress = true;


                if (SaveSystem.CurrentSave != null)
                {
                    if (Shiftorium.UpgradeInstalled("window_manager"))
                    {
                        Engine.AppearanceManager.SetupWindow(new Applications.Terminal());
                    }
                }
            }
        }

        public void WindowBorder_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            this._parentWindow.TextChanged += (o, a) =>
            {
                Setup();
                Desktop.ResetPanelButtons();

            };

            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;

            if (!this.IsDialog)
            {
                Engine.AppearanceManager.OpenForms.Add(this);
            }

            SaveSystem.GameReady += () =>
            {
                if (Shiftorium.UpgradeInstalled("wm_free_placement"))
                {
                    AppearanceManager.Invoke(new Action(() =>
                    {
                        this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                        this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;

                    }));
                }
                AppearanceManager.Invoke(new Action(() =>
                {
                    Setup();
                }));
            };

            ControlManager.SetupControls(this);

            Setup();

            var sWin = (IShiftOSWindow)ParentWindow;

            sWin.OnLoad();
        }

        public void Setup()
        {
            this.lbtitletext.Text = Localization.Parse(this._parentWindow.Text);
            
            if (SaveSystem.CurrentSave != null)
            {
                this.pnltitle.Visible = Shiftorium.UpgradeInstalled("wm_titlebar");
                this.pnlclose.Visible = Shiftorium.UpgradeInstalled("close_button");
                this.pnlminimize.Visible = (IsDialog == false) && Shiftorium.UpgradeInstalled("minimize_button");
                this.pnlmaximize.Visible = (IsDialog == false) && Shiftorium.UpgradeInstalled("maximize_button");
                SetupSkin();
            }
            else
            {
                this.pnltitle.Visible = false;
                this.pnlclose.Visible = false;
                this.pnlminimize.Visible = false;
                this.pnlmaximize.Visible = false;

            }
        }

        public override string Text
        {
            get
            {
                return lbtitletext?.Text;
            }

            set
            {
                lbtitletext.Text = value;
            }
        }

        public void SetupSkin()
        {
            this.DoubleBuffered = true;
            this.TransparencyKey = LoadedSkin.SystemKey;
            pnltitle.Height = LoadedSkin.TitlebarHeight;
            pnltitle.BackColor = LoadedSkin.TitleBackgroundColor;
            pnltitle.BackgroundImage = GetImage("titlebar");
            pnltitleleft.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleright.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleleft.BackColor = LoadedSkin.TitleLeftCornerBackground;
            pnltitleright.BackColor = LoadedSkin.TitleRightCornerBackground;
            pnltitleleft.Width = LoadedSkin.TitleLeftCornerWidth;
            pnltitleright.Width = LoadedSkin.TitleRightCornerWidth;
            pnltitleleft.BackgroundImage = GetImage("titleleft");
            pnltitleleft.BackgroundImageLayout = GetImageLayout("titleleft");
            pnltitleright.BackgroundImage = GetImage("titleright");
            pnltitleright.BackgroundImageLayout = GetImageLayout("titleright");


            lbtitletext.BackColor = LoadedSkin.TitleBackgroundColor;
            lbtitletext.ForeColor = LoadedSkin.TitleTextColor;
            lbtitletext.Font = LoadedSkin.TitleFont;

            pnlleft.BackColor = LoadedSkin.BorderLeftBackground;
            pnlleft.BackgroundImage = GetImage("leftborder");
            pnlleft.BackgroundImageLayout = GetImageLayout("leftborder");
            pnlleft.Width = LoadedSkin.LeftBorderWidth;
            pnlright.BackColor = LoadedSkin.BorderRightBackground;
            pnlright.BackgroundImage = GetImage("rightborder");
            pnlright.BackgroundImageLayout = GetImageLayout("rightborder");
            pnlright.Width = LoadedSkin.RightBorderWidth;

            pnlbottom.BackColor = LoadedSkin.BorderBottomBackground;
            pnlbottom.BackgroundImage = GetImage("bottomborder");
            pnlbottom.BackgroundImageLayout = GetImageLayout("bottomborder");
            pnlbottom.Height = LoadedSkin.BottomBorderWidth;

            pnlbottomr.BackColor = LoadedSkin.BorderBottomRightBackground;
            pnlbottomr.BackgroundImage = GetImage("bottomrborder");
            pnlbottomr.BackgroundImageLayout = GetImageLayout("bottomrborder");
            pnlbottoml.BackColor = LoadedSkin.BorderBottomLeftBackground;
            pnlbottoml.BackgroundImage = GetImage("bottomlborder");
            pnlbottoml.BackgroundImageLayout = GetImageLayout("bottomlborder");

            lbtitletext.ForeColor = LoadedSkin.TitleTextColor;
            lbtitletext.Font = LoadedSkin.TitleFont;
            pnlclose.BackColor = LoadedSkin.CloseButtonColor;
            pnlclose.BackgroundImage = GetImage("closebutton");
            pnlclose.BackgroundImageLayout = GetImageLayout("closebutton");
            pnlminimize.BackColor = LoadedSkin.MinimizeButtonColor;
            pnlminimize.BackgroundImage = GetImage("minimizebutton");
            pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebutton");
            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonColor;
            pnlmaximize.BackgroundImage = GetImage("maximizebutton");
            pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebutton");

            pnlclose.Size = LoadedSkin.CloseButtonSize;
            pnlminimize.Size = LoadedSkin.MinimizeButtonSize;
            pnlmaximize.Size = LoadedSkin.MaximizeButtonSize;
            pnlclose.Location = FromRight(LoadedSkin.CloseButtonFromSide);
            pnlminimize.Location = FromRight(LoadedSkin.MinimizeButtonFromSide);
            pnlmaximize.Location = FromRight(LoadedSkin.MaximizeButtonFromSide);
            pnlclose.Left -= pnlclose.Width;
            pnlmaximize.Left -= pnlmaximize.Width;
            pnlminimize.Left -= pnlminimize.Width;

            switch (LoadedSkin.TitleTextCentered)
            {
                case false:
                    lbtitletext.Location = LoadedSkin.TitleTextLeft;
                    break;
                default:
                    lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                    lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                    break;
            }
        }

        public Point FromRight(Point input)
        {
            return new Point(pnltitle.Width - input.X, input.Y);
        }

        private void lbtitletext_Click(object sender, EventArgs e)
        {

        }

        private void pnlclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pnlmaximize_Click(object sender, EventArgs e)
        {
            TerminalBackend.InvokeCommand($"win.max{{id:{this.ParentForm.GetHashCode()}}}");
        }

        private void pnlminimize_Click(object sender, EventArgs e)
        {
            TerminalBackend.InvokeCommand($"win.mini{{id:{this.ParentForm.GetHashCode()}}}");
        }

        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public bool IsDialog = false;


        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void pnltitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Shiftorium.UpgradeInstalled("draggable_windows"))
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pnltitle_Paint(object sender, PaintEventArgs e) {

        }

        private void lbtitletext_MouseMove(object sender, MouseEventArgs e) {
            pnltitle_MouseMove(sender, e);
        }
    }
}
