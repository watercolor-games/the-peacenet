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
using System.Threading;

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
        public MainMenu.MainMenu ParentMenu = null;

        [Command("pushnote")]
        [RequiresArgument("target")]
        [RequiresArgument("title")]
        [RequiresArgument("body")]
        public static bool PushNote(Dictionary<string, object> args)
        {
            string ta = args["target"].ToString();
            string ti = args["title"].ToString();
            string bo = args["body"].ToString();

            Desktop.PushNotification(ta, ti, bo);

            return true;
        }

        public List<IDesktopWidget> Widgets = new List<IDesktopWidget>();


        private int millisecondsUntilScreensaver = 300000;

        public void LoadIcons()
        {
            //That shot me back to 0.0.x with that name. Whatever.
            flnotifications.Controls.Clear(); //Clear the status tray
            
            foreach(var itype in NotificationDaemon.GetAllStatusIcons())
            {
                //We have the types. No need for shiftorium calls or anything.
                //First create the icon control...

                var ic = new PictureBox();
                //We can use the type name, in lowercase, for the icon tag.
                ic.Tag = itype.Name.ToLower();
                //Next get the icon data if any.

                    //We can use this attribute's ID in the skin engine to get an icon.
                    var img = GetIcon(itype.Name);
                    //Make it transparent.
                    (img as Bitmap).MakeTransparent(Color.White);
                    //Assign it to the control
                    ic.Image = img;
                    //Set the sizing mode
                    ic.SizeMode = PictureBoxSizeMode.StretchImage;
                ic.Size = new Size(16, 16); //TODO: make it skinnable
                //add to the notification tray
                flnotifications.Controls.Add(ic);
                ic.Show();

                ic.BringToFront();

                flnotifications.Show();
                

                ic.Click += (o, a) =>
                {
                    HideAppLauncher();

                    if(itype.BaseType == typeof(UserControl))
                    {
                        UserControl ctrl = (UserControl)Activator.CreateInstance(itype);
                        (ctrl as IStatusIcon).Setup();
                        currentSettingsPane = ctrl;
                        ControlManager.SetupControls(ctrl);
                        this.Controls.Add(ctrl);
                        ctrl.BringToFront();
                        int left = ic.Parent.PointToScreen(ic.Location).X;
                        int realleft = left - ctrl.Width;
                        realleft += ic.Width;
                        ctrl.Left = realleft;
                        if (LoadedSkin.DesktopPanelPosition == 0)
                        {
                            ctrl.Top = desktoppanel.Height;
                        }
                        else
                        {
                            ctrl.Top = this.Height - desktoppanel.Height - ctrl.Height;
                        }
                        ctrl.Show();
                    }


                };
            }
        }

        public void PushNotification(string app, string title, string msg)
        {
            lbnotemsg.Text = msg;
            lbnotetitle.Text = title;
            int height = pnlnotificationbox.Height;
            pnlnotificationbox.AutoSize = false;
            pnlnotificationbox.Height = height;
            pnlnotificationbox.Width = lbnotetitle.Width;

            var ctl = flnotifications.Controls.ToList().FirstOrDefault(x => x.Tag.ToString() == app);
            if (ctl == null)
                pnlnotificationbox.Left = desktoppanel.Width - pnlnotificationbox.Width;
            else
            {
                int left = ctl.Parent.PointToScreen(ctl.Location).X;
                int realleft = left - pnlnotificationbox.Width;
                realleft += ctl.Width;
                pnlnotificationbox.Left = realleft;
            }


            if (LoadedSkin.DesktopPanelPosition == 0)
                pnlnotificationbox.Top = desktoppanel.Height;
            else
                pnlnotificationbox.Top = this.Height - desktoppanel.Height - pnlnotificationbox.Height;
            ControlManager.SetupControls(pnlnotificationbox);
            var notekiller = new System.Windows.Forms.Timer();
            notekiller.Interval = 10000;
            notekiller.Tick += (o, a) =>
            {
                pnlnotificationbox.Hide();
                pnlnotificationbox.AutoSize = true;
            };
            Engine.AudioManager.PlayStream(Properties.Resources.infobox);


            pnlnotificationbox.Show();
            pnlnotificationbox.BringToFront();
            notekiller.Start();
        }

        private UserControl currentSettingsPane = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftOS.WinForms.WinformsDesktop"/> class.
        /// </summary>
        public WinformsDesktop()
        {
            InitializeComponent();
            pnlwidgetlayer.Click += (o, a) =>
            {
                HideAppLauncher();
            };
            ControlManager.MakeDoubleBuffered(pnlwidgetlayer);
            this.Click += (o, a) =>
            {
                HideAppLauncher();
            };
            SetupControl(desktoppanel);
            Shiftorium.Installed += () =>
            {
                foreach (var widget in Widgets)
                {
                    widget.OnUpgrade();
                }

                LoadIcons();


            };
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

            SaveSystem.GameReady += () =>
            {
                VirusManager.Init();
                this.Invoke(new Action(LoadIcons));
                if (this.Visible == true)
                    this.Invoke(new Action(() => SetupDesktop()));
            };
            Shiftorium.Installed += () =>
            {
                if (this.Visible == true)
                    this.Invoke(new Action(() => SetupDesktop()));
            };
            var time = new System.Windows.Forms.Timer();
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
                LoadIcons();
                foreach (var widget in Widgets)
                {
                    widget.OnSkinLoad();
                }


                SetupDesktop();
            };

            time.Tick += (o, a) =>
            {
                if (Shiftorium.IsInitiated == true)
                {
                    if (SaveSystem.CurrentSave != null)
                    {

                        lbtime.Text = Applications.Terminal.GetTime();
                        lbtime.Left = pnlnotifications.Width - lbtime.Width - LoadedSkin.DesktopPanelClockFromRight.X;
                        lbtime.Top = LoadedSkin.DesktopPanelClockFromRight.Y;

                        pnlnotifications.Width = flnotifications.Width + lbtime.Width + LoadedSkin.DesktopPanelClockFromRight.X;
                    }
                }

                try
                {
                    if (SaveSystem.CurrentSave != null)
                    {
                        if (SaveSystem.CurrentSave.LastMonthPaid != DateTime.Now.Month)
                        {
                            if (SaveSystem.CurrentSave.Codepoints >= DownloadManager.GetAllSubscriptions()[SaveSystem.CurrentSave.ShiftnetSubscription].CostPerMonth)
                            {
                                SaveSystem.CurrentSave.Codepoints -= DownloadManager.GetAllSubscriptions()[SaveSystem.CurrentSave.ShiftnetSubscription].CostPerMonth;
                                SaveSystem.CurrentSave.LastMonthPaid = DateTime.Now.Month;
                            }
                            else
                            {
                                SaveSystem.CurrentSave.ShiftnetSubscription = 0;
                                SaveSystem.CurrentSave.LastMonthPaid = DateTime.Now.Month;
                                Infobox.Show("Shiftnet", "You do not have enough Codepoints to pay for your Shiftnet subscription this month. You have been downgraded to the free plan.");
                            }
                        }
                    }
                }
                catch { }


            };
            time.Start();

            this.DoubleBuffered = true;
        }

        public void HideScreensaver()
        {
            if (ResetDesktop == true)
            {
                this.Invoke(new Action(() =>
                {
                    this.TopMost = false;
                    pnlscreensaver.Hide();
                    Cursor.Show();
                    SetupDesktop();
                    ResetDesktop = false;

                }));
            }
        }

        private bool ResetDesktop = false;

        private void ShowScreensaver()
        {
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
                                        HideAppLauncher();
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

                    ControlManager.SetupControls(pnlnotificationbox);

                    //skinning
                    lbtime.BackColor = Color.Transparent;
                    pnlnotifications.BackgroundImage = GetImage("panelclockbg");
                    pnlnotifications.BackgroundImageLayout = GetImageLayout("panelclockbg");
                    pnlnotifications.BackColor = LoadedSkin.DesktopPanelClockBackgroundColor;

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

                    this.BackgroundImage = SkinEngine.GetImage("desktopbackground");
                    this.BackgroundImageLayout = GetImageLayout("desktopbackground");
                    desktoppanel.BackColor = LoadedSkin.DesktopPanelColor;

                    var pnlimg = GetImage("desktoppanel");
                    if (pnlimg != null)
                    {
                        var bmp = new Bitmap(pnlimg);
                        bmp.MakeTransparent(Color.FromArgb(1, 0, 1));
                        pnlimg = bmp;
                    }

                    desktoppanel.BackgroundImage = pnlimg;
                    if (desktoppanel.BackgroundImage != null)
                    {
                        desktoppanel.BackColor = Color.Transparent;
                    }
                    var appimg = GetImage("applauncher");
                    if (appimg != null)
                    {
                        var bmp = new Bitmap(appimg);
                        bmp.MakeTransparent(Color.FromArgb(1, 0, 1));
                        appimg = bmp;
                    }
                    menuStrip1.BackgroundImage = appimg;
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

                pnlwidgetlayer.Show();
                pnlwidgetlayer.BringToFront();

                if (Shiftorium.UpgradeInstalled("desktop_widgets"))
                {
                    Widgets.Clear();
                    pnlwidgetlayer.Controls.Clear();
                    foreach(var widget in WidgetManager.GetAllWidgetTypes())
                    {
                        UserControl w = (UserControl)Activator.CreateInstance(widget.Value, null);

                        w.Location = WidgetManager.LoadDetails(w.GetType()).Location;
                        pnlwidgetlayer.Controls.Add(w);
                        MakeWidgetMovable(w);
                        Widgets.Add(w as IDesktopWidget);
                    }
                }

                int lastHeight = 5;
                foreach (var widget in Widgets)
                {
                    if (WidgetManager.LoadDetails(widget.GetType()).IsVisible && Shiftorium.UpgradeInstalled("desktop_widgets"))
                    {
                        widget.OnSkinLoad();
                        
                        widget.OnUpgrade();
                        widget.Setup();
                        widget.Show();
                        if (widget.Location.X == -1 && widget.Location.Y == -1)
                        {
                            widget.Location = new Point(5, lastHeight);
                            lastHeight += widget.Size.Height + 5;
                        }
                    }
                    else
                    {
                        widget.Hide();
                    }
                }
                pnlwidgetlayer.Show();
                pnlwidgetlayer.BringToFront();


            }
            else
            {
                desktoppanel.Hide();
            }

            LuaInterpreter.RaiseEvent("on_desktop_skin", this);

            PopulatePanelButtons();
        }

        public void MakeWidgetMovable(Control w, Control startCtrl = null)
        {
            if (startCtrl == null)
                startCtrl = w;

            bool moving = false;

            w.MouseDown += (o, a) =>
            {
                HideAppLauncher();
                moving = true;
            };

            w.MouseMove += (o, a) =>
            {
                if (moving == true)
                {
                    var mPos = Cursor.Position;
                    int mY = mPos.Y - desktoppanel.Height;
                    int mX = mPos.X;

                    int ctrlHeight = startCtrl.Height / 2;
                    int ctrlWidth = startCtrl.Width / 2;

                    startCtrl.Location = new Point(
                            mX - ctrlWidth,
                            mY - ctrlHeight
                        );

                }
            };

            w.MouseUp += (o, a) =>
            {
                moving = false;
                var details = WidgetManager.LoadDetails(startCtrl.GetType());
                details.Location = startCtrl.Location;
                WidgetManager.SaveDetails(startCtrl.GetType(), details);
            };

            foreach (Control c in w.Controls)
                MakeWidgetMovable(c, startCtrl);

        }

        public ToolStripMenuItem GetALCategoryWithName(string text)
        {
            foreach (ToolStripMenuItem menuitem in apps.DropDownItems)
            {
                if (menuitem.Text == text)
                    return menuitem;
            }

            var itm = new ToolStripMenuItem();
            itm.Text = text;
            apps.DropDownItems.Add(itm);
            return itm;
        }

        public Dictionary<string, List<LauncherItem>> LauncherItemList = new Dictionary<string, List<LauncherItem>>();

        /// <summary>
        /// Populates the app launcher.
        /// </summary>
        /// <returns>The app launcher.</returns>
        /// <param name="items">Items.</param>
        public void PopulateAppLauncher(LauncherItem[] items)
        {
            if (Shiftorium.UpgradeInstalled("advanced_app_launcher"))
            {
                pnladvancedal.Visible = false;
                flapps.BackColor = LoadedSkin.Menu_ToolStripDropDownBackground;
                flcategories.BackColor = LoadedSkin.Menu_ToolStripDropDownBackground;
                pnlalsystemactions.BackColor = LoadedSkin.SystemPanelBackground;
                lbalstatus.BackColor = LoadedSkin.ALStatusPanelBackColor;
                //Fonts
                lbalstatus.Font = LoadedSkin.ALStatusPanelFont;
                lbalstatus.ForeColor = LoadedSkin.ALStatusPanelTextColor;
                btnshutdown.Font = LoadedSkin.ShutdownFont;

                //Upgrades
                btnshutdown.Visible = Shiftorium.UpgradeInstalled("al_shutdown");

                //Alignments and positions.
                lbalstatus.TextAlign = LoadedSkin.ALStatusPanelAlignment;
                if (LoadedSkin.ShutdownButtonStyle == 2)
                    btnshutdown.Hide();
                else if (LoadedSkin.ShutdownButtonStyle == 1)
                {
                    btnshutdown.Parent = pnlstatus;
                    btnshutdown.BringToFront();
                }
                else
                    btnshutdown.Parent = pnlalsystemactions;
                if (LoadedSkin.ShutdownOnLeft)
                {
                    btnshutdown.Location = LoadedSkin.ShutdownButtonFromSide;
                }
                else
                {
                    btnshutdown.Left = (btnshutdown.Parent.Width - btnshutdown.Width) - LoadedSkin.ShutdownButtonFromSide.X;
                    btnshutdown.Top = LoadedSkin.ShutdownButtonFromSide.Y;
                }

                //Images
                 lbalstatus.BackgroundImage = GetImage("al_bg_status");
                lbalstatus.BackgroundImageLayout = GetImageLayout("al_bg_status");

                pnlalsystemactions.BackgroundImage = GetImage("al_bg_system");
                pnlalsystemactions.BackgroundImageLayout = GetImageLayout("al_bg_system");
                if (pnlalsystemactions.BackgroundImage != null)
                    btnshutdown.BackColor = Color.Transparent;

                btnshutdown.Font = LoadedSkin.ShutdownFont;
                btnshutdown.ForeColor = LoadedSkin.ShutdownForeColor;

                pnladvancedal.Size = LoadedSkin.AALSize;

                pnlalsystemactions.Height = LoadedSkin.ALSystemActionHeight;
                pnlstatus.Height = LoadedSkin.ALSystemStatusHeight;

                flcategories.Width = LoadedSkin.AALCategoryViewWidth;
                this.flapps.Width = LoadedSkin.AALItemViewWidth;
            }


            if (DesktopFunctions.ShowDefaultElements == true)
            {
                apps.DropDownItems.Clear();

                Dictionary<string, List<ToolStripMenuItem>> sortedItems = new Dictionary<string, List<ToolStripMenuItem>>();

                flcategories.Controls.Clear();

                LauncherItemList.Clear();


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
                        LauncherItemList[kv.DisplayData.Category].Add(kv);
                    }
                    else
                    {
                        sortedItems.Add(kv.DisplayData.Category, new List<ToolStripMenuItem>());
                        sortedItems[kv.DisplayData.Category].Add(item);
                        LauncherItemList.Add(kv.DisplayData.Category, new List<LauncherItem> { kv });
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
                            if (Shiftorium.UpgradeInstalled("advanced_app_launcher"))
                            {
                                var catbtn = new Button();
                                catbtn.Font = LoadedSkin.AdvALItemFont;
                                catbtn.FlatStyle = FlatStyle.Flat;
                                catbtn.FlatAppearance.BorderSize = 0;
                                catbtn.FlatAppearance.MouseOverBackColor = LoadedSkin.Menu_MenuItemSelected;
                                catbtn.FlatAppearance.MouseDownBackColor = LoadedSkin.Menu_MenuItemPressedGradientBegin;
                                catbtn.BackColor = LoadedSkin.Menu_ToolStripDropDownBackground;
                                catbtn.TextAlign = ContentAlignment.MiddleLeft;
                                catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                                catbtn.MouseEnter += (o, a) =>
                                {
                                    catbtn.ForeColor = LoadedSkin.Menu_SelectedTextColor;
                                };
                                catbtn.MouseLeave += (o, a) =>
                                {
                                    catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                                };
                                catbtn.Text = kv.Key;
                                catbtn.Width = flcategories.Width;
                                catbtn.Height = 24;
                                flcategories.Controls.Add(catbtn);
                                catbtn.Show();
                                catbtn.Click += (o, a) => SetupAdvancedCategory(catbtn.Text);
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
                            TerminalBackend.InvokeCommand("shutdown");
                        };
                        apps.DropDownItems.Add(item);
                        if (Shiftorium.UpgradeInstalled("advanced_app_launcher"))
                        {
                            if (LoadedSkin.ShutdownButtonStyle == 2) {
                                var catbtn = new Button();
                                catbtn.Font = LoadedSkin.AdvALItemFont;
                                catbtn.FlatStyle = FlatStyle.Flat;
                                catbtn.FlatAppearance.BorderSize = 0;
                                catbtn.FlatAppearance.MouseOverBackColor = LoadedSkin.Menu_MenuItemSelected;
                                catbtn.FlatAppearance.MouseDownBackColor = LoadedSkin.Menu_MenuItemPressedGradientBegin;
                                catbtn.BackColor = LoadedSkin.Menu_ToolStripDropDownBackground;
                                catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                                catbtn.MouseEnter += (o, a) =>
                                {
                                    catbtn.ForeColor = LoadedSkin.Menu_SelectedTextColor;
                                };
                                catbtn.MouseLeave += (o, a) =>
                                {
                                    catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                                };

                                catbtn.TextAlign = ContentAlignment.MiddleLeft;
                                catbtn.Text = "Shutdown";
                                catbtn.Width = flcategories.Width;
                                catbtn.Height = 24;
                                flcategories.Controls.Add(catbtn);
                                catbtn.Show();
                                catbtn.Click += (o, a) => TerminalBackend.InvokeCommand("shutdown");
                            }
                        }
                    }
                }
            }
            LuaInterpreter.RaiseEvent("on_al_populate", items);
        }

        public void SetupAdvancedCategory(string cat)
        {
            flapps.Controls.Clear();

            foreach(var app in LauncherItemList[cat])
            {
                var catbtn = new Button();
                catbtn.Font = LoadedSkin.AdvALItemFont;
                catbtn.FlatStyle = FlatStyle.Flat;
                catbtn.FlatAppearance.BorderSize = 0;
                catbtn.FlatAppearance.MouseOverBackColor = LoadedSkin.Menu_MenuItemSelected;
                catbtn.FlatAppearance.MouseDownBackColor = LoadedSkin.Menu_MenuItemPressedGradientBegin;
                catbtn.BackColor = LoadedSkin.Menu_ToolStripDropDownBackground;
                catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                catbtn.MouseEnter += (o, a) =>
                {
                    catbtn.ForeColor = LoadedSkin.Menu_SelectedTextColor;
                };
                catbtn.MouseLeave += (o, a) =>
                {
                    catbtn.ForeColor = LoadedSkin.Menu_TextColor;
                };
                catbtn.TextAlign = ContentAlignment.MiddleLeft;
                catbtn.Text = (app is LuaLauncherItem) ? app.DisplayData.Name : NameChangerBackend.GetNameRaw(app.LaunchType);
                catbtn.Width = flapps.Width;
                catbtn.ImageAlign = ContentAlignment.MiddleRight;
                catbtn.Height = 24;
                catbtn.Image = (app.LaunchType == null) ? null : SkinEngine.GetIcon(app.LaunchType.Name);

                flapps.Controls.Add(catbtn);
                catbtn.Show();
                catbtn.Click += (o, a) =>
                {
                    pnladvancedal.Hide();
                    if(app is LuaLauncherItem)
                    {
                        var interp = new LuaInterpreter();
                        interp.ExecuteFile((app as LuaLauncherItem).LaunchPath);
                    }
                    else
                    {
                        IShiftOSWindow win = Activator.CreateInstance(app.LaunchType) as IShiftOSWindow;
                        AppearanceManager.SetupWindow(win);
                    }



                };

            }
        }

        /// <summary>
        /// Desktops the load.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Desktop_Load(object sender, EventArgs e)
        {
            SaveSystem.IsSandbox = this.IsSandbox;
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
        internal bool IsSandbox = false;

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
            try
            {
                if (this.Visible == true)
                {
                    this.Invoke(new Action(() =>
                    {
                        act?.Invoke();
                    }));
                }
                else
                {
                    ParentMenu?.Invoke(act);
                }
            }
            catch
            {

            }
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

        private void desktoppanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }

        private void lbtime_Click(object sender, EventArgs e)
        {
        }

        public void SetupControl(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
                SetupControl(c);
            ctrl.Click += (o, a) => HideAppLauncher();
        }

        private void apps_Click(object sender, EventArgs e)
        {
            if (Shiftorium.UpgradeInstalled("advanced_app_launcher"))
            {
                lbalstatus.Text = $@"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}
{SaveSystem.CurrentSave.Codepoints} Codepoints
{Shiftorium.GetAvailable().Length} available, {SaveSystem.CurrentSave.CountUpgrades()} installed.";

                flapps.Controls.Clear();
                apps.DropDown.Hide();
                pnladvancedal.Location = new Point(0, (LoadedSkin.DesktopPanelPosition == 0) ? desktoppanel.Height : this.Height - pnladvancedal.Height - desktoppanel.Height);
                pnladvancedal.Visible = !pnladvancedal.Visible;
                pnladvancedal.BringToFront();
            }

        }

        private void btnshutdown_Click(object sender, EventArgs e)
        {
            TerminalBackend.InvokeCommand("shutdown");
        }

        public void HideAppLauncher()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    currentSettingsPane?.Hide();
                    currentSettingsPane = null;
                    pnladvancedal.Hide();
                }));
            }
            catch { }
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

    public static class ControlCollectionExtensions
    {
        public static IList<Control> ToList(this Control.ControlCollection ctrls)
        {
            var lst = new List<Control>();
            foreach (var ctl in ctrls)
                lst.Add(ctl as Control);
            return lst;
        }
    }
}