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
            _panelButtonGroup.Gap = 2;

            foreach(var pbtn in AppearanceManager.OpenForms)
            {
                var image = new PictureBox();
                //Draw panel button background...
                image.Width = 100;
                image.Height = Height - 4;
                var text = new TextControl();
                text.FontStyle = TextControlFontStyle.Custom;
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
            SendToBack();
            X = 0;
            Width = GetSize().Width;
            Height = 24;
            Y = 0;
            var now = DateTime.Now.TimeOfDay;
            string ampm = "AM";
            if (now.Hours > 12)
                ampm = "PM";
            var newDateTimeString = string.Format("{0}:{1}:{2} {3}", (now.Hours > 12) ? now.Hours - 12 : now.Hours, (now.Minutes < 10) ? "0" + now.Minutes.ToString() : now.Minutes.ToString(), (now.Seconds < 10) ? "0" + now.Seconds.ToString() : now.Seconds.ToString(), ampm);


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

            _panelButtonGroup.Y = 0;
            _panelButtonGroup.Height = Height;
            _panelButtonGroup.Width = _panelClock.X - _panelButtonGroup.X;

        }

        private List<PanelButtonData> PanelButtons = new List<PanelButtonData>();
        
        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
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
