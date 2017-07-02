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
using ShiftOS.WinForms.Applications;

/// <summary>
/// Window border.
/// </summary>
namespace ShiftOS.WinForms
{
	/// <summary>
	/// Window border.
	/// </summary>
    public partial class WindowBorder : Form, IWindowBorder
    {
        private bool IsFocused { get; set; }

		/// <summary>
		/// Raises the closing event.
		/// </summary>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// The parent window.
		/// </summary>
        private UserControl _parentWindow = null;

        /// <summary>
        /// Gets or sets the parent window.
        /// </summary>
        /// <value>The parent window.</value>
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

        internal void SetTitle(string title)
        {
            lbtitletext.Text = title;
        }

        public void SetupControls(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
                SetupControls(c);
            ctrl.Click += (o, a) =>
            {
                Desktop.HideAppLauncher();
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftOS.WinForms.WindowBorder"/> class.
        /// </summary>
        /// <param name="win">Window.</param>
        public WindowBorder(UserControl win)
        {
            InitializeComponent();
            IsFocused = true;
            this.Activated += (o, a) =>
            {
                IsFocused = true;
                SetupSkin();
            };
            this.Deactivate += (o, a) =>
            {
                IsFocused = false;
                SetupSkin();
            };
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
                    ControlManager.SetupControls(this.pnlcontents);
                }
                catch
                {

                }
            };

            this.Width = (LoadedSkin.LeftBorderWidth*2) + _parentWindow.Width + LoadedSkin.RightBorderWidth;
            this.Height = (LoadedSkin.TitlebarHeight*2) + _parentWindow.Height + LoadedSkin.BottomBorderWidth;

            this.pnlcontents.Controls.Add(this._parentWindow);
            this._parentWindow.Dock = DockStyle.Fill;
            this._parentWindow.Show();
            SetupControls(this);
            ControlManager.SetupControls(this._parentWindow);

            Shiftorium.Installed += () =>
            {
                Setup();
                ParentWindow.OnUpgrade();
            };
            Setup();
            this._parentWindow.TextChanged += (o, a) =>
            {
                Setup();
                Desktop.ResetPanelButtons();

            };


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

        }

        /// <summary>
        /// Universals the key down.
        /// </summary>
        /// <returns>The key down.</returns>
        /// <param name="o">O.</param>
        /// <param name="a">The alpha component.</param>
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

            ShiftOS.Engine.Scripting.LuaInterpreter.RaiseEvent("on_key_down", a);
        }

        /// <summary>
        /// Windows the border load.
        /// </summary>
        /// <returns>The border load.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public void WindowBorder_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            
        }

        /// <summary>
        /// Setup this instance.
        /// </summary>
        public void Setup()
        {
            this.lbtitletext.Text = NameChangerBackend.GetName(ParentWindow);
            
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

        public void SetDefaultBorders()
        {
            pnltitle.BackColor = LoadedSkin.TitleBackgroundColor;
            pnltitle.BackgroundImage = GetImage("titlebar");
            pnltitleleft.BackColor = LoadedSkin.TitleLeftCornerBackground;
            pnltitleright.BackColor = LoadedSkin.TitleRightCornerBackground;
            pnltitleleft.BackgroundImage = GetImage("titleleft");
            pnltitleleft.BackgroundImageLayout = GetImageLayout("titleleft");
            pnltitleright.BackgroundImage = GetImage("titleright");
            pnltitleright.BackgroundImageLayout = GetImageLayout("titleright");
            pnltitle.BackgroundImageLayout = GetImageLayout("titlebar"); //RETARD ALERT. WHY WASN'T THIS THERE WHEN IMAGELAYOUTS WERE FIRST IMPLEMENTED?

            pnlleft.BackColor = LoadedSkin.BorderLeftBackground;
            pnlleft.BackgroundImage = GetImage("leftborder");
            pnlleft.BackgroundImageLayout = GetImageLayout("leftborder");

            pnlright.BackColor = LoadedSkin.BorderRightBackground;
            pnlright.BackgroundImage = GetImage("rightborder");
            pnlright.BackgroundImageLayout = GetImageLayout("rightborder");

            pnlbottom.BackColor = LoadedSkin.BorderBottomBackground;
            pnlbottom.BackgroundImage = GetImage("bottomborder");
            pnlbottom.BackgroundImageLayout = GetImageLayout("bottomborder");

            pnlbottomr.BackColor = LoadedSkin.BorderBottomRightBackground;
            pnlbottomr.BackgroundImage = GetImage("bottomrborder");
            pnlbottomr.BackgroundImageLayout = GetImageLayout("bottomrborder");

            pnlbottoml.BackColor = LoadedSkin.BorderBottomLeftBackground;
            pnlbottoml.BackgroundImage = GetImage("bottomlborder");
            pnlbottoml.BackgroundImageLayout = GetImageLayout("bottomlborder");

        }

        public void SetInactiveBorders()
        {
            pnltitle.BackColor = LoadedSkin.TitleInactiveBackgroundColor;
            pnltitle.BackgroundImage = GetImage("titlebarinactive");
            pnltitleleft.BackColor = LoadedSkin.TitleInactiveLeftCornerBackground;
            pnltitleright.BackColor = LoadedSkin.TitleInactiveRightCornerBackground;
            pnltitleleft.BackgroundImage = GetImage("titleleftinactive");
            pnltitleleft.BackgroundImageLayout = GetImageLayout("titleleftinactive");
            pnltitleright.BackgroundImage = GetImage("titlerightinactive");
            pnltitleright.BackgroundImageLayout = GetImageLayout("titlerightinactive");
            pnltitle.BackgroundImageLayout = GetImageLayout("titlebarinactive"); //RETARD ALERT. WHY WASN'T THIS THERE WHEN IMAGELAYOUTS WERE FIRST IMPLEMENTED?

            pnlleft.BackColor = LoadedSkin.BorderInactiveLeftBackground;
            pnlleft.BackgroundImage = GetImage("leftborderinactive");
            pnlleft.BackgroundImageLayout = GetImageLayout("leftborderinactive");

            pnlright.BackColor = LoadedSkin.BorderInactiveRightBackground;
            pnlright.BackgroundImage = GetImage("rightborderinactive");
            pnlright.BackgroundImageLayout = GetImageLayout("rightborderinactive");

            pnlbottom.BackColor = LoadedSkin.BorderInactiveBottomBackground;
            pnlbottom.BackgroundImage = GetImage("bottomborderinactive");
            pnlbottom.BackgroundImageLayout = GetImageLayout("bottomborderinactive");

            pnlbottomr.BackColor = LoadedSkin.BorderInactiveBottomRightBackground;
            pnlbottomr.BackgroundImage = GetImage("bottomrborderinactive");
            pnlbottomr.BackgroundImageLayout = GetImageLayout("bottomrborderinactive");

            pnlbottoml.BackColor = LoadedSkin.BorderInactiveBottomLeftBackground;
            pnlbottoml.BackgroundImage = GetImage("bottomlborderinactive");
            pnlbottoml.BackgroundImageLayout = GetImageLayout("bottomlborderinactive");

        }

        /// <summary>
        /// Setups the skin.
        /// </summary>
        /// <returns>The skin.</returns>
        public void SetupSkin()
        {
            //Border colors and images...
            if (IsFocused)
            {
                SetDefaultBorders();
            }
            else
            {
                if (LoadedSkin.RenderInactiveBorders)
                    SetInactiveBorders();
                else
                    SetDefaultBorders();
            }


            this.DoubleBuffered = true;
            this.TransparencyKey = LoadedSkin.SystemKey;
            pnlcontents.BackColor = this.TransparencyKey;
            pnltitle.Height = LoadedSkin.TitlebarHeight;
            pnltitleleft.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleright.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleleft.Width = LoadedSkin.TitleLeftCornerWidth;
            pnltitleright.Width = LoadedSkin.TitleRightCornerWidth;
            
            lbtitletext.BackColor = (pnltitle.BackgroundImage != null) ? Color.Transparent : LoadedSkin.TitleBackgroundColor;
            lbtitletext.ForeColor = LoadedSkin.TitleTextColor;
            lbtitletext.Font = LoadedSkin.TitleFont;

            pnlleft.Width = LoadedSkin.LeftBorderWidth;
            pnlright.Width = LoadedSkin.RightBorderWidth;

            pnlbottom.Height = LoadedSkin.BottomBorderWidth;

            pnlbottomr.Width = pnlright.Width;
            pnlbottoml.Width = pnlleft.Width;

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
                    lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                            LoadedSkin.TitleTextLeft.Y);
                    break;
                default:
                    lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                    lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                    break;
            }

            if (Shiftorium.UpgradeInstalled("app_icons"))
            {
                pnlicon.Show();
                pnlicon.Size = new Size(16, 16);
                pnlicon.BackColor = Color.Transparent;
                pnlicon.BackgroundImage = GetIcon(this.ParentWindow.GetType().Name);
                pnlicon.BackgroundImageLayout = ImageLayout.Stretch;
                pnlicon.Location = LoadedSkin.TitlebarIconFromSide;
            }
            else
            {
                pnlicon.Hide();
            }
        }

        /// <summary>
        /// Froms the right.
        /// </summary>
        /// <returns>The right.</returns>
        /// <param name="input">Input.</param>
        public Point FromRight(Point input)
        {
            return new Point(pnltitle.Width - input.X, input.Y);
        }

		/// <summary>
		/// Lbtitletexts the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void lbtitletext_Click(object sender, EventArgs e)
        {

        }

		/// <summary>
		/// Pnlcloses the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void pnlclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		/// <summary>
		/// Pnlmaximizes the click.
		/// </summary>
		/// <returns>The click.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void pnlmaximize_Click(object sender, EventArgs e)
        {
            if (maximized == false)
                Desktop.MaximizeWindow(this);
            else
                Desktop.RestoreWindow(this);
            maximized = !maximized;
            SetupSkin();
        }

        bool minimized = false;
        bool maximized = false;

        public bool IsMinimized
        {
            get
            {
                return minimized;
            }
        }

        public bool IsMaximized
        {
            get
            {
                return maximized;
            }
        }


        /// <summary>
        /// Pnlminimizes the click.
        /// </summary>
        /// <returns>The click.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void pnlminimize_Click(object sender, EventArgs e)
        {
            if (minimized == false)
                Desktop.MinimizeWindow(this);
            else
                Desktop.RestoreWindow(this);
            minimized = !minimized;
        }


        /// <summary>
        /// The W m NCLBUTTONDOW.
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        /// <summary>
        /// The H t CAPTIO.
        /// </summary>
        public const int HT_CAPTION = 0x2;

        /// <summary>
        /// The is dialog.
        /// </summary>
        public bool IsDialog = false;


        [DllImportAttribute("user32.dll")]
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="hWnd">H window.</param>
        /// <param name="Msg">Message.</param>
        /// <param name="wParam">W parameter.</param>
        /// <param name="lParam">L parameter.</param>
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        /// <summary>
        /// Releases the capture.
        /// </summary>
        /// <returns>The capture.</returns>
        public static extern bool ReleaseCapture();

		/// <summary>
		/// Pnltitles the mouse move.
		/// </summary>
		/// <returns>The mouse move.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void pnltitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Shiftorium.UpgradeInstalled("draggable_windows"))
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

		/// <summary>
		/// Pnltitles the paint.
		/// </summary>
		/// <returns>The paint.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void pnltitle_Paint(object sender, PaintEventArgs e) {

        }

		/// <summary>
		/// Lbtitletexts the mouse move.
		/// </summary>
		/// <returns>The mouse move.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void lbtitletext_MouseMove(object sender, MouseEventArgs e) {
            pnltitle_MouseMove(sender, e);
        }

        bool resizing = false;

        private void pnlright_MouseDown(object sender, MouseEventArgs e)
        {
            if (Shiftorium.UpgradeInstalled("resizable_windows"))
            {
                resizing = true;
            }
        }

        private void pnlright_MouseMove(object sender, MouseEventArgs e)
        {
            if(resizing == true)
            {
                this.Width += e.X;
                switch (LoadedSkin.TitleTextCentered)
                {
                    case false:
                        lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                                LoadedSkin.TitleTextLeft.Y);
                        break;
                    default:
                        lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                        lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                        break;
                }
            }
        }

        private void pnlright_MouseUp(object sender, MouseEventArgs e)
        {
            resizing = false;
            pnlcontents.Show();
            switch (LoadedSkin.TitleTextCentered)
            {
                case false:
                    lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                            LoadedSkin.TitleTextLeft.Y);
                    break;
                default:
                    lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                    lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                    break;
            }
        }

        private void pnlleft_MouseMove(object sender, MouseEventArgs e)
        {
            if(resizing == true)
            {
                this.Left += e.X;
                this.Width -= e.X;
                switch (LoadedSkin.TitleTextCentered)
                {
                    case false:
                        lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                                LoadedSkin.TitleTextLeft.Y);
                        break;
                    default:
                        lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                        lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                        break;
                }
            }
        }

        private void pnlbottom_MouseMove(object sender, MouseEventArgs e)
        {
            if(resizing == true)
            {
                this.Height += e.Y;
            }
        }

        private void pnlbottomr_MouseMove(object sender, MouseEventArgs e)
        {
            if(resizing == true)
            {
                this.Width += e.X;
                this.Height += e.Y;
                switch (LoadedSkin.TitleTextCentered)
                {
                    case false:
                        lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                                LoadedSkin.TitleTextLeft.Y);
                        break;
                    default:
                        lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                        lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                        break;
                }
            }
        }

        private void pnlbottoml_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing == true)
            {
                this.Width -= e.X;
                this.Height += e.Y;
                this.Left += e.X;
                switch (LoadedSkin.TitleTextCentered)
                {
                    case false:
                        lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                                LoadedSkin.TitleTextLeft.Y);
                        break;
                    default:
                        lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                        lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                        break;
                }
            }

        }

        private void pnlclose_MouseDown(object sender, MouseEventArgs e)
        {
            pnlclose.BackColor = LoadedSkin.CloseButtonDownColor;
            if(LoadedSkin.CloseButtonDownImage != null)
            {
                pnlclose.BackgroundImage = GetImage("closebuttondown");
                pnlclose.BackgroundImageLayout = GetImageLayout("closebuttondown");
            }
        }

        private void pnlclose_MouseEnter(object sender, EventArgs e)
        {
            pnlclose.BackColor = LoadedSkin.CloseButtonOverColor;
            if (LoadedSkin.CloseButtonOverImage != null)
            {
                pnlclose.BackgroundImage = GetImage("closebuttonover");
                pnlclose.BackgroundImageLayout = GetImageLayout("closebuttonover");
            }

        }

        private void pnlclose_MouseLeave(object sender, EventArgs e)
        {
            pnlclose.BackColor = LoadedSkin.CloseButtonColor;
            pnlclose.BackgroundImage = GetImage("closebutton");
            pnlclose.BackgroundImageLayout = GetImageLayout("closebutton");
        }

        private void pnlclose_MouseUp(object sender, MouseEventArgs e)
        {
            pnlclose.BackColor = LoadedSkin.CloseButtonColor;
            pnlclose.BackgroundImage = GetImage("closebutton");
            pnlclose.BackgroundImageLayout = GetImageLayout("closebutton");

        }

        private void pnlmaximize_MouseDown(object sender, MouseEventArgs e)
        {

            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonDownColor;
            if (LoadedSkin.MaximizeButtonDownImage != null)
            {
                pnlmaximize.BackgroundImage = GetImage("maximizebuttondown");
                pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebuttondown");
            }
        }

        private void pnlmaximize_MouseEnter(object sender, EventArgs e)
        {
            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonOverColor;
            if (LoadedSkin.MaximizeButtonOverImage != null)
            {
                pnlmaximize.BackgroundImage = GetImage("maximizebuttonover");
                pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebuttonover");
            }

        }

        private void pnlmaximize_MouseLeave(object sender, EventArgs e)
        {
            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonColor;
            pnlmaximize.BackgroundImage = GetImage("maximizebutton");
            pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebutton");
        }

        private void pnlmaximize_MouseUp(object sender, MouseEventArgs e)
        {
            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonColor;
            pnlmaximize.BackgroundImage = GetImage("maximizebutton");
            pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebutton");
        }

        private void pnlminimize_MouseDown(object sender, MouseEventArgs e)
        {

            pnlminimize.BackColor = LoadedSkin.MinimizeButtonDownColor;
            if (LoadedSkin.MinimizeButtonDownImage != null)
            {
                pnlminimize.BackgroundImage = GetImage("minimizebuttondown");
                pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebuttondown");
            }
        }

        private void pnlminimize_MouseEnter(object sender, EventArgs e)
        {
            pnlminimize.BackColor = LoadedSkin.MinimizeButtonOverColor;
            if (LoadedSkin.MinimizeButtonOverImage != null)
            {
                pnlminimize.BackgroundImage = GetImage("minimizebuttonover");
                pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebuttonover");
            }

        }

        private void pnlminimize_MouseLeave(object sender, EventArgs e)
        {
            pnlminimize.BackColor = LoadedSkin.MinimizeButtonColor;
            pnlminimize.BackgroundImage = GetImage("minimizebutton");
            pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebutton");
        }

        private void pnlminimize_MouseUp(object sender, MouseEventArgs e)
        {
            pnlminimize.BackColor = LoadedSkin.MinimizeButtonColor;
            pnlminimize.BackgroundImage = GetImage("minimizebutton");
            pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebutton");
        }
    }
}
