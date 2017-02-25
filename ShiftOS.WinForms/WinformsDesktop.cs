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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
using ShiftOS.WinForms.Tools;
using ShiftOS.WinForms.Applications;
using Newtonsoft.Json;
using ShiftOS.Engine.Scripting;

/// <summary>
/// Winforms desktop.
/// </summary>
namespace ShiftOS.WinForms
{
	/// <summary>
	/// Winforms desktop.
	/// </summary>
    public partial class WinformsDesktop : Form, IDesktop
    {
		/// <summary>
		/// Occurs when window added.
		/// </summary>
        private static event Action<WindowBorder> windowAdded;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShiftOS.WinForms.WinformsDesktop"/> class.
		/// </summary>
        public WinformsDesktop()
        {
            InitializeComponent();
            this.TopMost = false;

            NotificationDaemon.NotificationMade += (note) =>
            {
                //Soon this will pop a balloon note.
                btnnotifications.Text = "Notifications (" + NotificationDaemon.GetUnreadCount().ToString() + ")";

            };

            NotificationDaemon.NotificationRead += (note) =>
            {
                btnnotifications.Text = "Notifications (" + NotificationDaemon.GetUnreadCount().ToString() + ")";

            };

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

            SaveSystem.GameReady += () =>
            {
                if(this.Visible == true)
                    this.Invoke(new Action(() => SetupDesktop()));
                this.Invoke(new Action(() =>
                {
                    btnnotifications.Text = "Notifications (" + NotificationDaemon.GetUnreadCount().ToString() + ")";
                }));
            };
            Shiftorium.Installed += () =>
            {
                if(this.Visible == true)
                    this.Invoke(new Action(() => SetupDesktop()));
            };
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

                ShiftOS.Engine.Scripting.LuaInterpreter.RaiseEvent("on_key_down", a);
            };
            SkinEngine.SkinLoaded += () =>
            {
                SetupDesktop();
            };
            time.Tick += (o, a) =>
            {
                if (Shiftorium.IsInitiated == true)
                {
                    if (SaveSystem.CurrentSave != null && TutorialManager.IsInTutorial == false)
                    {
                        lbtime.Text = Applications.Terminal.GetTime();
                        lbtime.Left = desktoppanel.Width - lbtime.Width - LoadedSkin.DesktopPanelClockFromRight.X;
                        lbtime.Top = LoadedSkin.DesktopPanelClockFromRight.Y;
                    }
                }

                btnnotifications.Left = lbtime.Left - btnnotifications.Width - 2;
                btnnotifications.Top = (desktoppanel.Height - btnnotifications.Height) / 2;
            };
            time.Start();

            this.DoubleBuffered = true;
        }

		/// <summary>
		/// Populates the panel buttons.
		/// </summary>
		/// <returns>The panel buttons.</returns>
        public void PopulatePanelButtons()
        {
            if (DesktopFunctions.ShowDefaultElements == true)
            {
                panelbuttonholder.Controls.Clear();
                if (Shiftorium.IsInitiated == true)
                {
                    if (Shiftorium.UpgradeInstalled("wm_panel_buttons"))
                    {
                        foreach (WindowBorder form in Engine.AppearanceManager.OpenForms)
                        {
                            if (form != null)
                            {
                                if (form.Visible == true)
                                {
                                    EventHandler onClick = (o, a) =>
                                    {
                                        if (form == focused)
                                        {
                                            if (form.IsMinimized)
                                            {
                                                RestoreWindow(form);
                                            }
                                            else
                                            {
                                                MinimizeWindow(form);
                                            }
                                        }
                                        else
                                        {
                                            form.BringToFront();
                                            focused = form;
                                        }
                                    };

                                    var pnlbtn = new Panel();
                                    pnlbtn.Margin = new Padding(2, LoadedSkin.PanelButtonFromTop, 0, 0);
                                    pnlbtn.BackColor = LoadedSkin.PanelButtonColor;
                                    pnlbtn.BackgroundImage = GetImage("panelbutton");
                                    pnlbtn.BackgroundImageLayout = GetImageLayout("panelbutton");

                                    var pnlbtntext = new Label();
                                    pnlbtntext.Text = NameChangerBackend.GetName(form.ParentWindow);
                                    pnlbtntext.AutoSize = true;
                                    pnlbtntext.Location = LoadedSkin.PanelButtonFromLeft;
                                    pnlbtntext.ForeColor = LoadedSkin.PanelButtonTextColor;
                                    pnlbtntext.BackColor = Color.Transparent;

                                    pnlbtn.BackColor = LoadedSkin.PanelButtonColor;
                                    if (pnlbtn.BackgroundImage != null)
                                    {
                                        pnlbtntext.BackColor = Color.Transparent;
                                    }
                                    pnlbtn.Size = LoadedSkin.PanelButtonSize;
                                    pnlbtn.Tag = "keepbg";
                                    pnlbtntext.Tag = "keepbg";
                                    pnlbtn.Controls.Add(pnlbtntext);
                                    this.panelbuttonholder.Controls.Add(pnlbtn);
                                    pnlbtn.Show();
                                    pnlbtntext.Show();

                                    if (Shiftorium.UpgradeInstalled("useful_panel_buttons"))
                                    {
                                        pnlbtn.Click += onClick;
                                        pnlbtntext.Click += onClick;
                                    }
                                    pnlbtntext.Font = LoadedSkin.PanelButtonFont;

                                }
                            }
                        }
                    }
                }
            }

            LuaInterpreter.RaiseEvent("on_panelbutton_populate", this);
        }

		/// <summary>
		/// Setups the desktop.
		/// </summary>
		/// <returns>The desktop.</returns>
        public void SetupDesktop()
        {
            if (DesktopFunctions.ShowDefaultElements == true)
            {
                ToolStripManager.Renderer = new ShiftOSMenuRenderer();

                this.DoubleBuffered = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                desktoppanel.BackColor = Color.Green;

                //upgrades

                if (Shiftorium.IsInitiated == true)
                {
                    desktoppanel.Visible = Shiftorium.UpgradeInstalled("desktop");
                    lbtime.Visible = Shiftorium.UpgradeInstalled("desktop_clock_widget");

                    btnnotifications.Visible = Shiftorium.UpgradeInstalled("panel_notifications");

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
                    desktoppanel.BackgroundImage = ((Bitmap)GetImage("desktoppanel"));
                    if (desktoppanel.BackgroundImage != null) ((Bitmap)desktoppanel.BackgroundImage).MakeTransparent(Color.FromArgb(1, 0, 1));
                    menuStrip1.BackgroundImage = GetImage("applauncher");
                    if (menuStrip1.BackgroundImage != null) ((Bitmap)menuStrip1.BackgroundImage).MakeTransparent(Color.FromArgb(1, 0, 1));
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
            }
            else
            {
                desktoppanel.Hide();
            }

            LuaInterpreter.RaiseEvent("on_desktop_skin", this);

            PopulatePanelButtons();
        }

        public ToolStripMenuItem GetALCategoryWithName(string text)
        {
            foreach(ToolStripMenuItem menuitem in apps.DropDownItems)
            {
                if (menuitem.Text == text)
                    return menuitem;
            }

            var itm = new ToolStripMenuItem();
            itm.Text = text;
            apps.DropDownItems.Add(itm);
            return itm;
        }

		/// <summary>
		/// Populates the app launcher.
		/// </summary>
		/// <returns>The app launcher.</returns>
		/// <param name="items">Items.</param>
        public void PopulateAppLauncher(LauncherItem[] items)
        {
            if (DesktopFunctions.ShowDefaultElements == true)
            {
                apps.DropDownItems.Clear();

                Dictionary<string, List<ToolStripMenuItem>> sortedItems = new Dictionary<string, List<ToolStripMenuItem>>();



                foreach (var kv in items)
                {
                    var item = new ToolStripMenuItem();
                    item.Text = (kv.LaunchType == null) ? kv.DisplayData.Name : Applications.NameChangerBackend.GetNameRaw(kv.LaunchType);
                    item.Image = (kv.LaunchType == null) ? null : SkinEngine.GetIcon(kv.LaunchType.Name);
                    item.Click += (o, a) =>
                    {
                        if (kv is LuaLauncherItem)
                        {
                            var interpreter = new Engine.Scripting.LuaInterpreter();
                            interpreter.ExecuteFile((kv as LuaLauncherItem).LaunchPath);
                        }
                        else
                        {
                            Engine.AppearanceManager.SetupWindow(Activator.CreateInstance(kv.LaunchType) as IShiftOSWindow);
                        }

                    };
                    if (sortedItems.ContainsKey(kv.DisplayData.Category))
                    {
                        sortedItems[kv.DisplayData.Category].Add(item);
                    }
                    else
                    {
                        sortedItems.Add(kv.DisplayData.Category, new List<ToolStripMenuItem>());
                        sortedItems[kv.DisplayData.Category].Add(item);
                    }
                }

                foreach (var kv in sortedItems)
                {
                    if (Shiftorium.IsInitiated == true)
                    {
                        if (Shiftorium.UpgradeInstalled("app_launcher_categories"))
                        {
                            var cat = GetALCategoryWithName(kv.Key);
                            foreach (var subItem in kv.Value)
                            {
                                cat.DropDownItems.Add(subItem);
                            }
                        }

                        else
                        {
                            foreach (var subItem in kv.Value)
                            {
                                apps.DropDownItems.Add(subItem);
                            }
                        }
                    }
                }

                if (Shiftorium.IsInitiated == true)
                {
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
            }
            LuaInterpreter.RaiseEvent("on_al_populate", items);
        }


		/// <summary>
		/// Desktops the load.
		/// </summary>
		/// <returns>The load.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        private void Desktop_Load(object sender, EventArgs e)
        {

            SaveSystem.Begin();

            SetupDesktop();

            SaveSystem.GameReady += () =>
            {
                this.Invoke(new Action(() =>
                {
                    LuaInterpreter.RaiseEvent("on_desktop_load", this);
                }));
            };
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="border">Border.</param>
        public void ShowWindow(IWindowBorder border)
        {
            var brdr = border as Form;
            focused = border;
            brdr.GotFocus += (o, a) =>
            {
                focused = border;
            };
            brdr.FormBorderStyle = FormBorderStyle.None;
            brdr.Show();
            brdr.TopMost = true;
        }

		/// <summary>
		/// Kills the window.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="border">Border.</param>
        public void KillWindow(IWindowBorder border)
        {
            border.Close();
        }

        private IWindowBorder focused = null;

        public string DesktopName
        {
            get
            {
                return "ShiftOS Desktop";
            }
        }


        /// <summary>
        /// Minimizes the window.
        /// </summary>
        /// <param name="brdr">Brdr.</param>
        public void MinimizeWindow(IWindowBorder brdr)
        {
            var loc = (brdr as WindowBorder).Location;
            var sz = (brdr as WindowBorder).Size;
            (brdr as WindowBorder).Tag = JsonConvert.SerializeObject(new
            {
                Size = sz,
                Location = loc
            });
            (brdr as WindowBorder).Location = new Point(this.GetSize().Width * 2, this.GetSize().Height * 2);
        }

		/// <summary>
		/// Maximizes the window.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="brdr">Brdr.</param>
        public void MaximizeWindow(IWindowBorder brdr)
        {
            int startY = (LoadedSkin.DesktopPanelPosition == 1) ? 0 : LoadedSkin.DesktopPanelHeight;
            int h = this.GetSize().Height - LoadedSkin.DesktopPanelHeight;
            var loc = (brdr as WindowBorder).Location;
            var sz = (brdr as WindowBorder).Size;
            (brdr as WindowBorder).Tag = JsonConvert.SerializeObject(new
            {
                Size = sz,
                Location = loc
            });
            (brdr as WindowBorder).Location = new Point(0, startY);
            (brdr as WindowBorder).Size = new Size(this.GetSize().Width, h);

        }

        /// <summary>
        /// Restores the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="brdr">Brdr.</param>
        public void RestoreWindow(IWindowBorder brdr)
        {
            dynamic tag = JsonConvert.DeserializeObject<dynamic>((brdr as WindowBorder).Tag.ToString());
            (brdr as WindowBorder).Location = tag.Location;
            (brdr as WindowBorder).Size = tag.Size;

        }

        /// <summary>
        /// Invokes the on worker thread.
        /// </summary>
        /// <returns>The on worker thread.</returns>
        /// <param name="act">Act.</param>
        public void InvokeOnWorkerThread(Action act)
        {
            this.Invoke(act);
        }

        public void OpenAppLauncher(Point loc)
        {
            apps.DropDown.Left = loc.X;
            apps.DropDown.Top = loc.Y;
            apps.ShowDropDown();
        }

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <returns>The size.</returns>
        public Size GetSize()
        {
            return this.Size;
        }

        private void btnnotifications_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupWindow(new Applications.Notifications());
        }
    }

    [ShiftOS.Engine.Scripting.Exposed("desktop")]
    public class DesktopFunctions
    {
        public static bool ShowDefaultElements = true;

        public dynamic getWindow()
        {
            return Desktop.CurrentDesktop;
        }

        public void showDefaultElements(bool val)
        {
            ShowDefaultElements = val;
            SkinEngine.LoadSkin();
        }

        public dynamic getOpenWindows()
        {
            return AppearanceManager.OpenForms;
        }

        public string getALItemName(LauncherItem kv)
        {
            return (kv.LaunchType == null) ? kv.DisplayData.Name : Applications.NameChangerBackend.GetNameRaw(kv.LaunchType);
        }

        public void openAppLauncher(Point loc)
        {
            Desktop.OpenAppLauncher(loc);
        }

        public string getWindowTitle(IWindowBorder form)
        {
            return NameChangerBackend.GetName(form.ParentWindow);
        }

        public void openApp(LauncherItem kv)
        {
            if (kv is LuaLauncherItem)
            {
                var interpreter = new Engine.Scripting.LuaInterpreter();
                interpreter.ExecuteFile((kv as LuaLauncherItem).LaunchPath);
            }
            else
            {
                Engine.AppearanceManager.SetupWindow(Activator.CreateInstance(kv.LaunchType) as IShiftOSWindow);
            }
        }
    }
}
