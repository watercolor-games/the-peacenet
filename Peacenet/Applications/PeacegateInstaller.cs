using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peacenet.Filesystem;
using Peacenet.PeacegateThemes;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Saves;

namespace Peacenet.Applications
{
    public class PeacegateInstaller : Window
    {
        private DesktopWindow _desktop = null;

        private Label _header = new Label();
        private Panel _bodyPanel = new Panel();
        private Button _next = new Button();
        private Button _prev = new Button();
        private Label _welcome = new Label();

        private int _state = 0;

        private Label _hostnameHeader = new Label();
        private TextBox _hostname = new TextBox();
        private Label _hostnameDesc = new Label();

        private Label _personalizationDesc = new Label();

        private PictureBox _wall1 = new PictureBox();
        private PictureBox _wall2 = new PictureBox();
        private ListBox _accents = new ListBox();

        [Dependency]
        private GameLoop _gameloop = null;

        [Dependency]
        private InfoboxManager _infobox = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private OS _os = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private MissionManager _mission = null;

        private ProgressBar _installProgress = new ProgressBar();
        private Label _installStatus = new Label();

        private readonly string[] _installs = new string[]
        {
            "pde",
            "pde-paneltheme-peacegate",
            "pde-panel-email",
            "pde-panel-peacenet",
            "pde-panel-skill",
            "pde-applaunchertheme-kde",
            "peacegate-filemanager",
            "peacegate-terminal",
            "peacegate-texteditor",
            "peacegate-peacenet-worldmap",
            "peacegate-imageviewer",
            "peacegate-settings-daemon",
            "peacegate-clock-app",
            "ftp",
            "openssh-client",
            "cowsay",
            "fortune",
            "peacenet-upgrade-system",
            "peacenet-rep-system",
            "peacenet-medal-daemon",
            "nmap",
            "gnu-coreutils"
        };

        public PeacegateInstaller(DesktopWindow desktop, WindowSystem _winsys) : base(_winsys)
        {
            Width = 650;
            Height = 400;
            Title = "Peacegate OS";
            CanClose = false;
            SetWindowStyle(WindowStyle.Dialog);
            _desktop = desktop;

            AddChild(_header);
            AddChild(_bodyPanel);
            AddChild(_next);
            AddChild(_prev);

            _header.AutoSize = true;
            _bodyPanel.DrawBackground = true;

            _header.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;

            _prev.Click += (o, a) =>
            {
                _state--;
                UpdateUI();
            };

            _next.Click += (o, a) =>
            {
                if(_state == 1)
                {
                    if(!HostnameValid())
                    {
                        _infobox.Show("Invalid Hostname", "Your hostname cannot be blank, and it can only contain numbers (0-9), letters (a-z), capitals (A-Z), underscores '_' and hyphens '-'.");
                        return;
                    }
                    _fs.WriteAllText("/etc/hostname", _hostname.Text);
                }
                _state++;
                UpdateUI();
            };

            _hostnameHeader.AutoSize = true;
            _hostnameDesc.AutoSize = true;
            _hostnameHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;

            _hostnameHeader.Text = "Hostname:";
            _hostname.Label = "Hostname";
            _hostnameDesc.Text = @"All Peacegate OS systems require a hostname. Your hostname is displayed publically within The Peacenet in place of your public IP address.

Your hostname can only contain letters (a-z, A-Z), numbers (0-9), hyphens ('-') and underscores ('_').";

            _wall1.Texture = _gameloop.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage");
            _wall2.Texture = _gameloop.Content.Load<Texture2D>("Desktop/DesktopBackgroundImage2");
            _accents.Items.AddRange(Enum.GetNames(typeof(PeacenetAccentColor)));
            _accents.AutoSize = true;

            _personalizationDesc.AutoSize = true;

            _accents.SelectedIndexChanged += (o, a) =>
            {
                if(_accents.SelectedItem != null)
                {
                    var accent = (PeacenetAccentColor)Enum.Parse(typeof(PeacenetAccentColor), _accents.SelectedItem.ToString());
                    this._save.SetValue<PeacenetAccentColor>("theme.accent", accent);
                    _pn.AccentColor = accent;
                }
            };

            _wall1.Click += (o, a) =>
            {
                _save.SetValue("desktop.wallpaper", "DesktopBackgroundImage");
                _os.FireWallpaperChanged();
            };

            _wall2.Click += (o, a) =>
            {
                _save.SetValue("desktop.wallpaper", "DesktopBackgroundImage2");
                _os.FireWallpaperChanged();
            };

            UpdateUI();
        }

        private bool HostnameValid()
        {
            if (string.IsNullOrWhiteSpace(_hostname.Text))
                return false;

            string valid = "0123456789-_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (char c in _hostname.Text)
                if (!valid.Contains(c))
                    return false;

            return true;
        }

        private void Install()
        {
            SetProgress("Beginning installation...", 0);
            Thread.Sleep(5000);
            var packages = _installs;
            for(int i = 0; i < packages.Length; i++)
            {
                string pack = packages[i];
                for(int j = 0; j <= pack.Length*4; j++)
                {
                    SetProgress($"Installing package: {pack} [{i + 1}/{packages.Length}]", (float)j / (pack.Length * 4));
                    Thread.Sleep(25);
                }
            }
            SetProgress("Done.", 1f);
            _state = 5;
            _gameloop.Invoke(() => UpdateUI());
        }

        private void UpdateUI()
        {
            _bodyPanel.Clear();
            _next.Text = "Next";
            _prev.Text = "Back";
            switch(_state)
            {
                case 0:
                    _prev.Enabled = false;
                    _header.Text = "Welcome to Peacegate!";
                    _bodyPanel.AddChild(_welcome);
                    _welcome.X = 15;
                    _welcome.Y = 7;
                    _welcome.MaxWidth = Width - 30;
                    _welcome.AutoSize = true;
                    _welcome.Text = @"This wizard will guide you through the installation and configuration of Peacegate OS.

Peacegate OS is a Linux-based operating system built for use in The Peacenet. It has many useful programs and features for communicating with people within Peacenet. It also contains many traditional programs you can use to accomplish every-day tasks.

This version of Peacegate is intended for use as an interactive system, meaning you can directly interact with the operating system and The Peacenet.

To continue using Peacegate, you'll need to set a hostname and create the base configuration. This wizard will guide you through it. Simply click 'Next' and follow the instructions.";
                    break;
                case 1:
                    _header.Text = "Network config";
                    _prev.Enabled = true;
                    _bodyPanel.AddChild(_hostnameHeader);
                    _bodyPanel.AddChild(_hostname);
                    _bodyPanel.AddChild(_hostnameDesc);
                    break;
                case 2:
                    _header.Text = "Wallpaper and Accent Color";
                    _bodyPanel.AddChild(_personalizationDesc);
                    _bodyPanel.AddChild(_wall1);
                    _bodyPanel.AddChild(_wall2);
                    _bodyPanel.AddChild(_accents);
                    _personalizationDesc.Text = "Select a wallpaper and an accent color to use on your desktop. You can change these at any time.";
                    _next.Text = "Next";
                    break;
                case 3:
                    _header.Text = "Ready to install?";
                    _bodyPanel.AddChild(_welcome);
                    _next.Text = "Install";
                    _welcome.X = 15;
                    _welcome.Y = 7;
                    _welcome.MaxWidth = Width - 30;
                    _welcome.AutoSize = true;
                    _welcome.Text = "You have supplied all the necessary details for Peacegate OS to install onto your system. Before you can use it, Peacegate must install its core packages and programs to your system.\n\nAt any time you may go back and reconfigure Peacegate OS by clicking 'Back'. If you're ready to go, click 'Install.'";
                    _next.Text = "Finish";

                    break;
                case 4:
                    _prev.Enabled = false;
                    _next.Enabled = false;
                    _bodyPanel.AddChild(_installStatus);
                    _bodyPanel.AddChild(_installProgress);
                    _header.Text = "Please wait.";
                    _bodyPanel.AddChild(_welcome);
                    _welcome.X = 15;
                    _welcome.Y = 7;
                    _welcome.MaxWidth = Width - 30;
                    _welcome.AutoSize = true;
                    _welcome.Text = @"Peacegate OS is now installing base packages to your system. This shouldn't take too long, The Peacenet's download speeds are extremely fast compared to the Internet. Maybe this is your first glimpse of just what kind of system you're dealing with...";

                    Task.Run(() => Install());

                    break;
                case 5:
                    _desktop.ShowPanels = true;
                    Close();
                    var m = _mission.Available.FirstOrDefault(x => x.ID == "welcome_to_the_peacenet");
                    if (m != null)
                        m.Start();
                    break;
            }
        }

        protected void SetProgress(string status, float progress)
        {
            _gameloop.Invoke(() =>
            {
                _installStatus.Text = status;
                _installProgress.Value = progress;
            });
        }

        protected override void OnUpdate(GameTime time)
        {
            _header.X = 15;
            _header.Y = 15;
            _header.MaxWidth = Width - 30;

            _next.X = (Width - _next.Width) - 15;
            _prev.X = (_next.X - _prev.Width) - 5;
            _next.Y = (Height - _next.Height) - 15;
            _prev.Y = _next.Y;

            int bottom = _prev.Y - 15;
            _bodyPanel.Y = _header.Y + _header.Height + 7;
            _bodyPanel.Height = bottom - _bodyPanel.Y;
            _bodyPanel.Width = Width;
            _bodyPanel.X = 0;

            _hostnameHeader.X = 15;
            _hostnameHeader.Y = 7;
            _hostnameHeader.MaxWidth = Width - 30;

            _hostname.X = 15;
            _hostname.Y = _hostnameHeader.Y + _hostnameHeader.Height + 3;
            _hostname.Width = _hostnameHeader.MaxWidth;

            _hostnameDesc.X = 15;
            _hostnameDesc.Y = _hostname.Y + _hostname.Height + 15;
            _hostnameDesc.MaxWidth = _hostnameHeader.MaxWidth;

            _accents.X = 15;
            _accents.Width = Width - 30;
            _accents.Y = (_bodyPanel.Height - _accents.Height) - 7;

            _personalizationDesc.MaxWidth = _accents.Width;
            _personalizationDesc.X = 15;
            _personalizationDesc.Y = 7;

            float aspect = (1920f / 1080f);
            float wallHeight = (_accents.Y - 7) - (_personalizationDesc.Y + _personalizationDesc.Height + 3);


            _wall1.Width = (int)(wallHeight * aspect);
            _wall1.Height = (int)wallHeight;
            _wall2.Width = _wall1.Width;
            _wall2.Height = _wall1.Height;
            _wall1.X = 15;
            _wall2.X = (Width - _wall2.Width) - 15;
            _wall1.Y = _personalizationDesc.Y + _personalizationDesc.Height + 3;
            _wall2.Y = _wall1.Y;

            _installProgress.X = 15;
            _installProgress.Y = _bodyPanel.Height - _installProgress.Height;
            _installProgress.Width = _bodyPanel.Width - 30;
            _installStatus.X = _installProgress.X;
            _installStatus.Y = _installProgress.Y - _installStatus.Height - 4;
            _installStatus.MaxWidth = _installProgress.Width;
            _installStatus.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            _installStatus.AutoSize = true;
            
            base.OnUpdate(time);
        }
    }
}
