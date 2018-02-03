using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Engine.Filesystem;
using Peacenet.Applications;

namespace Peacenet.DesktopUI
{
    /// <summary>
    /// The window used for the App Launcher.
    /// </summary>
    public class AppLauncherMenu : Window
    {
        //App Launcher is a two-column display.
        private ListView _applications = new ListView();
        private ListView _places = new ListView();

        //...These two columns are scrollable.
        private ScrollView _appsView = new ScrollView();
        private ScrollView _placesView = new ScrollView();

        //There's also a label at the top that shows the username.
        private Label _username = new Label();
        //The top bar also shows the hostname.
        private Label _hostname = new Label();

        //We also have a shutdown button at the bottom.
        private Button _shutdown = new Button();

        //Here are some engine dependencies.
        [Dependency]
        private ItchOAuthClient _itch = null;
        [Dependency]
        private FSManager _fs = null;
        [Dependency]
        private OS _os = null;
        [Dependency]
        private AppLauncherManager _al = null;
        [Dependency]
        private InfoboxManager _infobox = null;

        private DesktopWindow _desktop = null;

        /// <inheritdoc/>
        public AppLauncherMenu(WindowSystem _winsys, DesktopWindow desktop) : base(_winsys)
        {
            _desktop = desktop;

            //Set our width - height can be dealt with later.
            Width = 475;

            //Also, we want to be border-less.
            SetWindowStyle(WindowStyle.NoBorder);
        }

        /// <inheritdoc/>
        public override void Show(int x = -1, int y = -1)
        {
            //Instantiate all ui elements
            _appsView = new ScrollView();
            _placesView = new ScrollView();
            _applications = new ListView();
            _places = new ListView();
            _hostname = new Label();
            _username = new Label();
            _shutdown = new Button();

            //Set text
            _shutdown.Text = "Exit Peacegate";
            _username.Text = (_itch.LoggedIn) ? _itch.User.display_name : "User";
            string hn = (_fs.FileExists("/etc/hostname")) ? _fs.ReadAllText("/etc/hostname") : "127.0.0.1";
            string un = (_itch.LoggedIn) ? _itch.User.username : "user";
            _hostname.Text = $"{un}@{hn}";

            //Add all of our ui elements.
            AddChild(_appsView);
            AddChild(_placesView);
            AddChild(_username);
            AddChild(_hostname);
            AddChild(_shutdown);

            //Add list views to their scrollviews.
            _appsView.AddChild(_applications);
            _placesView.AddChild(_places);

            //Now, make the list views show List mode
            _applications.Layout = ListViewLayout.List;
            _places.Layout = ListViewLayout.List;

            //Add shell directories
            foreach(var shelldir in _os.GetShellDirs())
            {
                _places.SetImage(shelldir.FriendlyName, shelldir.Texture);
                var lvitem = new ListViewItem(_places);
                lvitem.Value = shelldir.FriendlyName;
                lvitem.ImageKey = shelldir.FriendlyName;
                lvitem.Tag = shelldir.Path;
            }

            //Populate the main event.
            List<AppLauncherItem> items = new List<AppLauncherItem>();
            foreach (var cat in _al.GetAllCategories())
                foreach (var item in _al.GetAllInCategory(cat))
                    items.Add(item);
            foreach(var item in items.OrderBy(z=>z.Attribute.Name))
            {
                var lvitem = new ListViewItem(_applications);
                lvitem.ImageKey = item.Attribute.Name;
                lvitem.Value = item.Attribute.Name;
                lvitem.Tag = item.WindowType;
            }

            //When a place is selected, open it.
            _places.SelectedIndexChanged += (o, a) =>
            {
                var item = _places.SelectedItem;
                if (item != null)
                {
                    Task.Run(() =>
                    {
                        var fb = new FileManager(WindowSystem);
                        fb.SetCurrentDirectory(item.Tag.ToString());
                        fb.Show();
                    });
                    Close();
                }
            };
            //When an app is selected, open it.
            _applications.SelectedIndexChanged += (o, a) =>
            {
                var item = _applications.SelectedItem;
                if (item != null)
                {
                    Task.Run(() =>
                    {
                        var wintype = (item.Tag as Type);
                        var window = Activator.CreateInstance(wintype, new[] { WindowSystem });
                        (window as Window).Show();
                    });
                    Close();
                }
            };

            _shutdown.Click += (o, a) =>
            {
                foreach(var win in WindowSystem.WindowList)
                {
                    win.Border.Enabled = false;
                }
                _infobox.ShowYesNo("Exit Peacegate OS", "Are you sure you want to exit Peacegate OS and shut down your computer?", (answer) =>
                {
                    if (answer)
                    {
                        foreach(var window in WindowSystem.WindowList)
                        {
                            if (window.Border != _desktop.Parent)
                                WindowSystem.Close(window.WindowID);
                        }
                        _desktop.Shutdown();
                    }
                    else
                    {
                        foreach (var win in WindowSystem.WindowList)
                        {
                            win.Border.Enabled = true;
                        }
                    }
                });

            };
            base.Show(x, y);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (!HasFocused)
                Close();

            _username.X = 3;
            _username.Y = 3;
            _username.AutoSize = true;
            _username.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;
            _hostname.X = 3;
            _hostname.Y = _username.Y + _username.Height + 3;
            _hostname.AutoSize = true;
            _hostname.FontStyle = Plex.Engine.Themes.TextFontStyle.System;

            _appsView.X = 0;
            _appsView.Y = _hostname.Y + _hostname.Height + 3;
            _applications.Width = (Width / 2);
            _appsView.Height = 450;
            _placesView.X = _applications.Width;
            _placesView.Y = _appsView.Y;
            _places.Width = _applications.Width;
            _placesView.Height = _appsView.Height;

            _shutdown.Y = _placesView.Y + _placesView.Height + 3;
            _shutdown.X = (Width - _shutdown.Width) - 3;
            Height = _shutdown.Y + _shutdown.Height + 3;

            base.OnUpdate(time);
        }
    }
}
