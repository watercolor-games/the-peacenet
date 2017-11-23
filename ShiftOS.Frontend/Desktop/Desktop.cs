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
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.Theming;

namespace Plex.Frontend.Desktop
{
    public class Desktop : Control, IDesktop
    {
        internal class Panel : Control
        {
            protected override void OnLayout(GameTime gameTime)
            {
                if (Parent == null)
                    Width = UIManager.Viewport.Width;
                else
                    Width = Parent.Width;
                Height = 24;
                X = 0;
            }

            protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
            {
                gfx.Clear(ThemeManager.Theme.GetAccentColor());
            }
        }

        private Panel _topPanel = null;
        private Panel _bottomPanel = null;

        private int _animState = -1;
        private float _animSlide = 0.0f;

        //Top panel items
        private TextControl _time = null;
        private Button _appLauncherOpen = null;
        private Button _systemOpen = null;

        //Menus
        private Menu _systemMenu = null;
        private MenuItem _sysSettings = null;

        private Menu _appLauncherMenu = null;

        //Bottom bar
        private ItemGroup _panelButtons = null;

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
        }

        public Desktop()
        {
            _topPanel = new Panel();
            _bottomPanel = new Panel();

            AddControl(_topPanel);
            AddControl(_bottomPanel);
            _time = new TextControl();
            _appLauncherOpen = new Button();
            _systemOpen = new Button();
            _topPanel.AddControl(_time);
            _topPanel.AddControl(_appLauncherOpen);
            _topPanel.AddControl(_systemOpen);

            _systemMenu = new Menu();
            _sysSettings = new MenuItem();
            _sysSettings.Text = "System settings";
            _systemMenu.AddItem(_sysSettings);

            _sysSettings.ItemActivated += () =>
            {
                AppearanceManager.SetupDialog(new Apps.GameSettings());
            };

            _systemOpen.Click += () =>
            {
                if (_systemMenu.Visible)
                {
                    _systemMenu.Hide();
                }
                else
                {
                    _systemMenu.Y = _topPanel.Y + _topPanel.Height;
                    _systemMenu.X = _systemOpen.X;
                    _systemMenu.Show();
                }
            };

            _appLauncherMenu = new Menu();

            _appLauncherOpen.Click += () =>
            {
                int _alX = 0;
                int _alY = _topPanel.Y + _topPanel.Height;

                if (_appLauncherMenu.Visible && (_appLauncherMenu.X == _alX && _appLauncherMenu.Y == _alY))
                {
                    _appLauncherMenu.Hide();
                }
                else
                {
                    OpenAppLauncher(new System.Drawing.Point(0, _topPanel.Y + _topPanel.Height));
                }
            };

            _panelButtons = new ItemGroup();
            _bottomPanel.AddControl(_panelButtons);

        }


        protected override void OnLayout(GameTime gameTime)
        {
            Width = GetSize().Width;
            Height = GetSize().Height;
            X = 0;
            Y = 0;

            _animSlide = MathHelper.Clamp(_animSlide, 0, 1);

            int _topPos = (int)ProgressBar.linear(_animSlide, 0, 1, 0 - _topPanel.Height, 0);
            int _bottomPos = (int)ProgressBar.linear(_animSlide, 0, 1, Height, Height - _bottomPanel.Height);

            _topPanel.Y = _topPos;
            _bottomPanel.Y = _bottomPos;

            switch (_animState)
            {
                case 0:
                    _animSlide = 0;
                    _animState++;
                    break;
                case 1:
                    _animSlide += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_animSlide >= 1F)
                        _animState++;
                    break;
                case 2:
                    break;
                case 3:
                    _animSlide -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    if (_animSlide <= 0F)
                        _animState++;

                    break;
            }

            TimeSpan now = DateTime.Now.TimeOfDay;
            int hour = (now.Hours > 12) ? now.Hours - 12 : now.Hours;
            string ampm = now.Hours > 12 ? "PM" : "AM";
            string minute = now.Minutes > 9 ? now.Minutes.ToString() : "0" + now.Minutes.ToString();
            string second = now.Seconds > 9 ? now.Seconds.ToString() : "0" + now.Seconds.ToString();

            _time.Text = $"{hour}:{minute}:{second} {ampm}";
            _time.AutoSize = true;

            _time.X = (_topPanel.Width - _time.Width) - 10;
            _time.Y = (_topPanel.Height - _time.Height) / 2;

            _appLauncherOpen.Text = "Peacegate";
            _appLauncherOpen.X = 2;
            _appLauncherOpen.Y = (_topPanel.Height - _appLauncherOpen.Height) / 2;

            _systemOpen.Text = "System";
            _systemOpen.X = _appLauncherOpen.X + _appLauncherOpen.Width + 3;
            _systemOpen.Y = _appLauncherOpen.Y;

            _panelButtons.AutoSize = true;
            _panelButtons.X = 0;
            _panelButtons.Y = (_bottomPanel.Height - _panelButtons.Height) / 2;
        }

        public string DesktopName
        {
            get
            {
                return "Peacegate";
            }
        }

        public void Close()
        {
            _animState = 3;
        }

        public Size GetSize()
        {
            return UIManager.Viewport;
        }

        public void HideAppLauncher()
        {
            _appLauncherMenu.Hide();
        }

        public void InvokeOnWorkerThread(Action act)
        {
            if (act == null)
                return;
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
            if (_appLauncherMenu.Visible && (_appLauncherMenu.X == loc.X && _appLauncherMenu.Y == loc.Y))
            {
                _appLauncherMenu.Hide();
                return;
            }

            _appLauncherMenu.X = loc.X;
            _appLauncherMenu.Y = loc.Y;

            if (_appLauncherMenu.Visible == false)
            {
                _appLauncherMenu.Show();
            }
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            //Somewhere to store all categories.
            List<string> categories = new List<string>();

            //Clear all existing AL items.
            _appLauncherMenu.ClearItems();

            //first we get all unique categories.
            foreach (var item in items)
            {
                if (categories.Contains(item.DisplayData.Category))
                    continue;
                categories.Add(item.DisplayData.Category);
            }

            //Now, for each category we create a menu item...
            foreach (var cat in categories)
            {
                var mitem = new MenuItem();
                //Set its text to the category text...
                mitem.Text = cat;
                //And add it to the app launcher.
                _appLauncherMenu.AddItem(mitem);
                //Now, we want to get all app launcher entries in the category...
                foreach (var entry in items.Where(x => x.DisplayData.Category == cat))
                {
                    //and create a menu item for it...
                    var entryItem = new MenuItem();
                    //assign its text...
                    entryItem.Text = entry.DisplayData.Name;
                    //We'll assume this isn't a lua item because those are de-de-de-de-DEPRECATED.
                    entryItem.ItemActivated += () =>
                    {
                        //In here we create an instance of the entry's window object...
                        var window = (IPlexWindow)Activator.CreateInstance(entry.LaunchType, null);
                        //...and display it.
                        AppearanceManager.SetupWindow(window); //Bam.
                        //Just to be safe...
                        HideAppLauncher();
                    };
                    //Don't forget to add it to the right place!!
                    mitem.AddItem(entryItem);
                }
            }

        }

        public void PopulatePanelButtons()
        {
            //Clear panel buttons
            _panelButtons.ClearControls();

            //Iterate through all windows that are open
            foreach (var window in AppearanceManager.OpenForms)
            {
                //Create a button
                var button = new Button();
                //Button text should be the window title.
                button.Text = window.Text;
                //Add button to the panel button list.
                _panelButtons.AddControl(button);
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
        }

        public void Show()
        {
            _animState = 0;
            _animSlide = 0;
            Visible = true;
            UIManager.AddTopLevel(this);
        }

        public void ShowWindow(IWindowBorder border)
        {
        }
    }
}
