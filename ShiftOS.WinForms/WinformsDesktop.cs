using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    public partial class WinformsDesktop : Form, IDesktop
    {
        private static event Action<WindowBorder> windowAdded;

        public WinformsDesktop()
        {
            InitializeComponent();
            this.TopMost = false;
            this.LocationChanged += (o, a) =>
            {
                if (this.Left != 0)
                    this.Left = 0;
                if (this.Top != 0)
                    this.Top = 0;
            };

            this.SizeChanged += (o, a) =>
            {
                if (this.ClientRectangle != Screen.PrimaryScreen.Bounds)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            };

            SaveSystem.GameReady += () => this.Invoke(new Action(() => SetupDesktop()));
            Shiftorium.Installed += () => this.Invoke(new Action(() => SetupDesktop()));
            var time = new Timer();
            time.Interval = 100;
            this.KeyDown += (o, a) =>
            {
                if (a.Control && a.KeyCode == Keys.T)
                {
                    Engine.AppearanceManager.SetupWindow(new Applications.Terminal());
                }
                /*if (a.Control && a.KeyCode == Keys.Tab)
                {
                    // CtrlTabMenu 
                    CtrlTabMenu.Show();
                    if (a.Shift) CtrlTabMenu.CycleBack();
                    else CtrlTabMenu.CycleForwards();
                }*/ //nyi
            };
            SkinEngine.SkinLoaded += () =>
            {
                SetupDesktop();
            };
            time.Tick += (o, a) =>
            {
                if (SaveSystem.CurrentSave != null)
                {
                    lbtime.Text = Applications.Terminal.GetTime();
                    lbtime.Left = desktoppanel.Width - lbtime.Width - LoadedSkin.DesktopPanelClockFromRight.X;
                    lbtime.Top = LoadedSkin.DesktopPanelClockFromRight.Y;
                }
            };
            time.Start();

            this.DoubleBuffered = true;
        }

        public void PopulatePanelButtons()
        {
            panelbuttonholder.Controls.Clear();
            if (Shiftorium.UpgradeInstalled("wm_panel_buttons"))
            {
                foreach (WindowBorder form in Engine.AppearanceManager.OpenForms)
                {
                    if (form != null)
                    {
                        if (form.Visible == true)
                        {
                            var pnlbtn = new Panel();
                            pnlbtn.Margin = new Padding(2, LoadedSkin.PanelButtonFromTop, 0, 0);
                            pnlbtn.BackgroundImage = GetImage("panelbutton");
                            pnlbtn.BackgroundImageLayout = GetImageLayout("panelbutton");

                            var pnlbtntext = new Label();
                            pnlbtntext.Text = form.Text;
                            pnlbtntext.AutoSize = true;
                            pnlbtntext.Location = LoadedSkin.PanelButtonFromLeft;
                            pnlbtntext.ForeColor = LoadedSkin.PanelButtonTextColor;
                            pnlbtntext.Font = LoadedSkin.PanelButtonFont;

                            pnlbtn.BackColor = LoadedSkin.PanelButtonColor;
                            if (pnlbtn.BackgroundImage != null)
                            {
                                pnlbtntext.BackColor = Color.Transparent;
                            }
                            pnlbtn.Size = LoadedSkin.PanelButtonSize;

                            pnlbtn.Controls.Add(pnlbtntext);
                            this.panelbuttonholder.Controls.Add(pnlbtn);
                            ControlManager.SetupControls(pnlbtn);
                            pnlbtn.Show();
                            pnlbtntext.Show();
                        }
                    }
                }
            }
        }

        public void SetupDesktop()
        {
            ToolStripManager.Renderer = new ShiftOSMenuRenderer();

            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            desktoppanel.BackColor = Color.Green;

            //upgrades

            if (SaveSystem.CurrentSave != null)
            {
                desktoppanel.Visible = Shiftorium.UpgradeInstalled("desktop");
                lbtime.Visible = Shiftorium.UpgradeInstalled("desktop_clock_widget");

                //skinning
                lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;

                panelbuttonholder.Top = 0;
                panelbuttonholder.Left = LoadedSkin.PanelButtonHolderFromLeft;
                panelbuttonholder.Height = desktoppanel.Height;
                panelbuttonholder.BackColor = Color.Transparent;
                panelbuttonholder.Margin = new Padding(0, 0, 0, 0);

                sysmenuholder.Visible = Shiftorium.UpgradeInstalled("app_launcher");

                //The Color Picker can give us transparent colors - which Windows Forms fucking despises when dealing with form backgrounds.
                //To compensate, we must recreate the desktop color and make the alpha channel '255'.
                this.BackColor = Color.FromArgb(LoadedSkin.DesktopColor.R, LoadedSkin.DesktopColor.G, LoadedSkin.DesktopColor.B);
                //Not doing this will cause an ArgumentException.

                DitheringEngine.DitherImage(SkinEngine.GetImage("desktopbackground"), new Action<Image>((img) =>
                {
                    this.BackgroundImage = img;
                }));
                this.BackgroundImageLayout = GetImageLayout("desktopbackground");
                desktoppanel.BackgroundImage = GetImage("desktoppanel");
                menuStrip1.BackgroundImage = GetImage("applauncher");
                lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;
                lbtime.Font = LoadedSkin.DesktopPanelClockFont;
                if (desktoppanel.BackgroundImage == null)
                {
                    lbtime.BackColor = LoadedSkin.DesktopPanelClockBackgroundColor;
                }
                else
                {
                    lbtime.BackColor = Color.Transparent;
                }
                apps.Text = LoadedSkin.AppLauncherText;
                sysmenuholder.Location = LoadedSkin.AppLauncherFromLeft;
                sysmenuholder.Size = LoadedSkin.AppLauncherHolderSize;
                apps.Size = sysmenuholder.Size;
                menuStrip1.Renderer = new ShiftOSMenuRenderer(new AppLauncherColorTable());
                desktoppanel.BackColor = LoadedSkin.DesktopPanelColor;
                desktoppanel.BackgroundImageLayout = GetImageLayout("desktoppanel");
                desktoppanel.Height = LoadedSkin.DesktopPanelHeight;
                if (LoadedSkin.DesktopPanelPosition == 1)
                {
                    desktoppanel.Dock = DockStyle.Bottom;
                }
                else
                {
                    desktoppanel.Dock = DockStyle.Top;
                }
            }

            PopulatePanelButtons();
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            apps.DropDownItems.Clear();

            foreach (var kv in items)
            {
                var item = new ToolStripMenuItem();
                item.Text = kv.DisplayData.Name;
                item.Click += (o, a) =>
                {
                    Engine.AppearanceManager.SetupWindow(Activator.CreateInstance(kv.LaunchType) as IShiftOSWindow);
                };
                apps.DropDownItems.Add(item);
            }

            if (Shiftorium.UpgradeInstalled("al_shutdown"))
            {
                apps.DropDownItems.Add(new ToolStripSeparator());
                var item = new ToolStripMenuItem();
                item.Text = Localization.Parse("{SHUTDOWN}");
                item.Click += (o, a) =>
                {
                    TerminalBackend.InvokeCommand("sos.shutdown");
                };
                apps.DropDownItems.Add(item);

            }
        }



        private void Desktop_Load(object sender, EventArgs e)
        {

            SaveSystem.Begin();

            SetupDesktop();
        }

        public void ShowWindow(IWindowBorder border)
        {
            var brdr = border as Form;
            brdr.FormBorderStyle = FormBorderStyle.None;
            brdr.Show();
            brdr.TopMost = true;
        }

        public void KillWindow(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public void MinimizeWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void MaximizeWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void RestoreWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void InvokeOnWorkerThread(Action act)
        {
            this.Invoke(act);
        }

        public Size GetSize()
        {
            return this.Size;
        }
    }
}
