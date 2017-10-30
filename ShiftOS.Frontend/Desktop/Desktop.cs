using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;


namespace Plex.Frontend.Desktop
{
    public class Desktop : GUI.TextControl, IDesktop
    {
        private ItemGroup _panelButtonGroup = new ItemGroup();
        private Button _userMenu = new Button();
        private TextControl _panelClock = new TextControl();


        public PlexSkin LoadedSkin
        {
            get
            {
                return (PlexSkin)SkinEngine.LoadedSkin;
            }
        }

        public Menu _appLauncher = new Menu();

        public Desktop()
        {
            AddControl(_panelButtonGroup);
            UIManager.ScreenRightclicked += (x, y) =>
            {
                _appLauncher.Layout(new GameTime());
                if(x+_appLauncher.Width >= UIManager.Viewport.Width)
                {
                    x -= (x + _appLauncher.Width) - UIManager.Viewport.Width;
                }
                if (y + _appLauncher.Height >= UIManager.Viewport.Height)
                {
                    y -= (y + _appLauncher.Height) - UIManager.Viewport.Height;
                }
                OpenAppLauncher(new System.Drawing.Point(x, y));
            };
            SaveSystem.GameReady += () =>
            {
                AudioPlayerSubsystem.Startup();
                Show();
                SetupDesktop();
                if (!Upgrades.UpgradeInstalled("tutorial1"))
                {
                    Story.Start("tutorial1");
                }
            };
            Click += () =>
            {
                if (UIManagerTools.InProtectedGUI == true)
                    return;
                if (_appLauncher.Visible == false)
                {
                    if (MouseX >= LoadedSkin.AppLauncherFromLeft.X && MouseX <= LoadedSkin.AppLauncherFromLeft.X + LoadedSkin.AppLauncherHolderSize.Width)
                    {
                        int dp_pos = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : GetSize().Height - LoadedSkin.DesktopPanelHeight;
                        int dp_height = LoadedSkin.DesktopPanelHeight;
                        if (MouseY >= 0 && MouseY <= dp_height)
                        {
                            int al_x = LoadedSkin.AppLauncherFromLeft.X;
                            _appLauncher.Layout(new GameTime());
                            int al_y = (dp_pos == 0) ? dp_height : (dp_pos - _appLauncher.Height);
                            OpenAppLauncher(new System.Drawing.Point(al_x, al_y));
                        }
                    }
                }
                else
                {
                    HideAppLauncher();
                }
            };

            AddControl(_userMenu);
            AddControl(_panelClock);

        }

        public string DesktopName
        {
            get
            {
                return "Plexgate";
            }
        }

        public void Close()
        {
            UIManager.StopHandling(this);
        }

        public Size GetSize()
        {
            return UIManager.Viewport;
        }

        public void HideAppLauncher()
        {
            _appLauncher.Hide();
        }

        public void InvokeOnWorkerThread(Action act)
        {
            UIManager.CrossThreadOperations.Enqueue(act);
        }

        public void KillWindow(IWindowBorder border)
        {
        }

        public void MaximizeWindow(IWindowBorder brdr)
        {
        }

        public void MinimizeWindow(IWindowBorder brdr)
        {
        }

        public void OpenAppLauncher(System.Drawing.Point loc)
        {
            _appLauncher.X = loc.X;
            _appLauncher.Y = loc.Y;
            _appLauncher.Show();
        }

        private string _pguiAppName = "";

        protected override void RenderText(GraphicsContext gfx)
        {
            int dp_height = LoadedSkin.DesktopPanelHeight;
            int dp_position = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - dp_height;
            int dp_width = Width;

            if (UIManagerTools.InProtectedGUI)
            {
                var appMeasure = GraphicsContext.MeasureString(_pguiAppName, LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopLeft);
                gfx.DrawString(_pguiAppName, 5, (Height - (int)appMeasure.Y) / 2, LoadedSkin.DesktopPanelClockColor.ToMonoColor(), LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopLeft);
            }
            else
            {
                int al_holder_width = LoadedSkin.AppLauncherHolderSize.Width;

                var almeasure = GraphicsContext.MeasureString(LoadedSkin.AppLauncherText, LoadedSkin.AppLauncherFont, Engine.GUI.TextAlignment.TopLeft);
                gfx.DrawString(LoadedSkin.AppLauncherText, (al_holder_width - (int)almeasure.X) / 2, (LoadedSkin.AppLauncherHolderSize.Height - (int)almeasure.Y) / 2, LoadedSkin.AppLauncherTextColor.ToMonoColor(), LoadedSkin.AppLauncherFont, Engine.GUI.TextAlignment.TopLeft);

                int initialGap = LoadedSkin.PanelButtonHolderFromLeft;
                int offset = initialGap;
            }
        }

        private bool _inpgui = false;

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            _appLauncher.ClearItems();
            List<string> cats = new List<string>();
            foreach(var item in items.OrderBy(x => Localization.Parse(x.DisplayData.Category)))
            {
                if (!cats.Contains(Localization.Parse(item.DisplayData.Category)))
                    cats.Add(Localization.Parse(item.DisplayData.Category));
            }
            foreach(var cat in cats)
            {
                var catitem = new MenuItem();
                catitem.Text = cat;

                foreach(var item in items.Where(x => Localization.Parse(x.DisplayData.Category) == cat).OrderBy(x=>Localization.Parse(x.DisplayData.Name)))
                {
                    var alitem = new MenuItem();
                    alitem.Text = Localization.Parse(item.DisplayData.Name);
                    alitem.ItemActivated += () =>
                    {
                        AppearanceManager.SetupWindow((IPlexWindow)Activator.CreateInstance(item.LaunchType, null));
                        HideAppLauncher();
                    };
                    catitem.AddItem(alitem);
                }

                _appLauncher.AddItem(catitem);
            }

            var shutdown = new MenuItem
            {
                Text = "Shut down",
            };
            shutdown.ItemActivated += () => { PlexCommands.Shutdown(); };
            _appLauncher.AddItem(shutdown);
        }

        private Control _currentwin = null;

        public void PopulatePanelButtons()
        {
            _panelButtonGroup.ClearControls();
            _panelButtonGroup.ItemGroupLayout = ItemGroupLayout.Custom;
            _panelButtonGroup.InitialGap = (LoadedSkin.DesktopPanelHeight - LoadedSkin.PanelButtonSize.Height) / 2;
            _panelButtonGroup.Gap = 2;

            foreach(var pbtn in AppearanceManager.OpenForms)
            {
                var image = new PictureBox();
                //Draw panel button background...
                if (UIManager.SkinTextures.ContainsKey("panelbutton"))
                {
                    image.Image = UIManager.SkinTextures["panelbutton"];
                    image.ImageLayout = SkinEngine.GetImageLayout("panelbutton");
                }
                else
                {
                    image.Image = UIManager.SkinTextures["PanelButtonColor"];
                    image.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                }
                image.Width = LoadedSkin.PanelButtonSize.Width;
                image.Height = LoadedSkin.PanelButtonSize.Height;
                var text = new TextControl();
                text.FontStyle = TextControlFontStyle.Custom;
                text.Font = LoadedSkin.PanelButtonFont;
                text.TextColor = LoadedSkin.PanelButtonTextColor.ToMonoColor();
                text.Text = pbtn.Text;
                text.AutoSize = true;
                text.Layout(new GameTime());
                text.X = 4;
                text.Y = (image.Height - text.Height) / 2;
                image.AddControl(text);
                _panelButtonGroup.AddControl(image);

                Action _click = () =>
                {
                    var wb = pbtn as WindowBorder;
                    if(wb != _currentwin)
                    {
                        _currentwin = wb;
                        UIManager.FocusedControl = wb;
                        UIManager.BringToFront(wb);
                    }
                    else
                    {
                        wb.ToggleMinimized();
                    }
                    this.MouseHandled ();
                };
                text.Click += _click;
                image.Click += _click;

            }
        }

        public void PushNotification(string app, string title, string message)
        {
        }

        public void RestoreWindow(IWindowBorder brdr)
        {
        }

        public void SetupDesktop()
        {
            _userMenu.Text = SaveSystem.GetUsername();
            _userMenu.Image = FontAwesome.user.ToTexture2D(UIManager.GraphicsDevice);
            Invalidate();
        }

        public void Show()
        {
            UIManager.AddTopLevel(this);
            Visible = true;
            Invalidate();
        }

        public void ShowWindow(IWindowBorder border)
        {
        }

        private string dateTimeString = "";
        
        protected override void OnLayout(GameTime gameTime)
        {
            FontStyle = TextControlFontStyle.Custom;
            TextColor = Microsoft.Xna.Framework.Color.White;
            if (_inpgui != UIManagerTools.InProtectedGUI)
            {
                _inpgui = UIManagerTools.InProtectedGUI;
                RequireTextRerender();
                Invalidate();
            }
            if (_inpgui)
            {
                _pguiAppName = DesktopName + " - Protected GUI";
            }

            SendToBack();
            X = 0;
            Width = GetSize().Width;
            Height = LoadedSkin.DesktopPanelHeight;
            Y = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : GetSize().Height - Height;
            var now = DateTime.Now.TimeOfDay;
            string ampm = "AM";
            if (now.Hours > 12)
                ampm = "PM";
            var newDateTimeString = string.Format("{0}:{1}:{2} {3}", (now.Hours > 12) ? now.Hours - 12 : now.Hours, (now.Minutes < 10) ? "0" + now.Minutes.ToString() : now.Minutes.ToString(), (now.Seconds < 10) ? "0" + now.Seconds.ToString() : now.Seconds.ToString(), ampm);


            _panelClock.FontStyle = TextControlFontStyle.Custom;
            _panelClock.TextColor = LoadedSkin.DesktopPanelClockColor.ToMonoColor();
            _panelClock.Font = LoadedSkin.DesktopPanelClockFont;
            _panelClock.AutoSize = true;

            if (newDateTimeString != dateTimeString)
            {
                dateTimeString = newDateTimeString;
                _panelClock.Text = dateTimeString;
                _panelClock.Layout(gameTime);
            }

            _userMenu.Y = (Height - _userMenu.Height) / 2;
            _userMenu.X = (Width - _userMenu.Width) - 5;

            _panelClock.Y = (Height - _panelClock.Height) / 2;
            _panelClock.X = (_userMenu.X - _panelClock.Width) - 5;

            _panelButtonGroup.Visible = !_inpgui;
            _panelButtonGroup.X = LoadedSkin.PanelButtonHolderFromLeft;
            _panelButtonGroup.Y = 0;
            _panelButtonGroup.Height = Height;
            _panelButtonGroup.Width = _panelClock.X - _panelButtonGroup.X;

        }

        private List<PanelButtonData> PanelButtons = new List<PanelButtonData>();
        
        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //Let's get data for the desktop panel.

            //We need the width and the height and the position.

            int dp_height = LoadedSkin.DesktopPanelHeight;
            int dp_position = (LoadedSkin.DesktopPanelPosition == 0) ? 0 : Height - dp_height;
            int dp_width = Width;

            //Alright, now we need to know if we should draw using a texture or a color
            if (UIManager.SkinTextures.ContainsKey("desktoppanel"))
            {
                //Draw with the texture
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, UIManager.SkinTextures["desktoppanel"], SkinEngine.GetImageLayout("desktoppanel"));
            }
            else
            {
                //draw with a color
                var color = UIManager.SkinTextures["DesktopPanelColor"];
                gfx.DrawRectangle(0, dp_position, dp_width, dp_height, color);
            }

            if (!_inpgui)
            {
                //Alright, now App Launcher.
                var al_left = LoadedSkin.AppLauncherFromLeft;
                var holderSize = LoadedSkin.AppLauncherHolderSize;
                if (UIManager.SkinTextures.ContainsKey("applauncher"))
                {
                    gfx.DrawRectangle(al_left.X, dp_position + al_left.Y, holderSize.Width, holderSize.Height, UIManager.SkinTextures["applauncher"], SkinEngine.GetImageLayout("applauncher"));
                }
            }

            var pcMeasure = GraphicsContext.MeasureString(dateTimeString, LoadedSkin.DesktopPanelClockFont, Engine.GUI.TextAlignment.TopRight);
            int panelclockleft = Width - (int)pcMeasure.X;
            int panelclockwidth = Width - panelclockleft;

            if (UIManager.SkinTextures.ContainsKey("panelclockbg"))
            {
                //draw the background using panelclock texture
                gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, UIManager.SkinTextures["panelclockbg"], SkinEngine.GetImageLayout("panelclockbg"));
            }
            else
            {
                if (!UIManager.SkinTextures.ContainsKey("desktoppanel"))
                {
                    //draw using the bg color
                    var pcBGColor = UIManager.SkinTextures["DesktopPanelClockBackgroundColor"];
                    gfx.DrawRectangle(panelclockleft, dp_position, panelclockwidth, dp_height, pcBGColor);
                }
            }

            base.OnPaint(gfx, target);
        }
    }

    public class PanelButtonData
    {
        public string Title { get; set; }
        public WindowBorder Window { get; set; }

        public bool IsActive
        {
            get
            {
                return Window.IsFocusedControl || Window.ContainsFocusedControl;
            }
        }
    }
    

    public class AppLauncherItem
    {
        public Engine.LauncherItem Data { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
