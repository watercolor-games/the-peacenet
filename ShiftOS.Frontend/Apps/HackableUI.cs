using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;
using Plex.Objects;

namespace Plex.Frontend.Apps
{
    public class HackableUI : Control, IPlexWindow
    {
        private HackableSystem _hackable = null;
        private PictureBox _logo = new PictureBox();
        private TextControl _systemName = new TextControl();

        private TextControl _osInfoHeader = new TextControl();
        private TextControl _osInfoText = new TextControl();

        private TextControl _mainHeader = new TextControl();
        private TextControl _mainText = new TextControl();

        private TerminalControl _terminal = new TerminalControl();

        private Button _action = new Button();

        private int _state = 0;

        public HackableUI(HackableSystem system)
        {
            AddControl(_logo);
            AddControl(_systemName);
            AddControl(_osInfoHeader);
            AddControl(_mainHeader);
            AddControl(_mainText);
            AddControl(_terminal);
            AddControl(_osInfoText);
            AddControl(_action);
            _action.Click += () =>
            {
                switch (_state)
                {
                    case 0:
                        UIManagerTools.EnterProtectedGUI(this);
                        _state = 1;
                        StartHackerConsole();
                        break;
                    case 1:
                        Engine.Infobox.PromptYesNo("End hack session", "Do you really want to end the hack session now?", (answer) =>
                        {
                            if (answer == true)
                            {
                                _state = 0;
                                UIManagerTools.LeaveProtectedGUI();
                                ServerManager.SendMessage("hack_abort", $"{_hackable.NetName}.{_hackable.SystemDescriptor}");
                                Hacking.BeginHack(null);
                                TerminalBackend.SetShellOverride("");
                            }
                        });
                        break;
                }
            };
            _terminal.Visible = false;
            Width = 800;
            Height = 600;
            _hackable = system;
        }

        private static bool _sessionStarted = false;

        [ClientMessageHandler("hack_started")]
        public static void HackStarted(string content, string ip)
        {
            _sessionStarted = true;
        }

        public void StartHackerConsole()
        {
            AppearanceManager.ConsoleOut = _terminal;
            AppearanceManager.StartConsoleOut();
            new System.Threading.Thread(() =>
            {
                _sessionStarted = false;
                TerminalBackend.InStory = true;
                TerminalBackend.PrefixEnabled = false;
                Console.WriteLine("Plexgate Hacker Utility - v4.7");
                Console.WriteLine("=============================");
                Console.WriteLine();
                Console.WriteLine("Starting Plexnet handshake with {0}.{1}...", _hackable.NetName, _hackable.SystemDescriptor.SystemName);
                ServerManager.SendMessage("hack_start", $"{_hackable.NetName}.{_hackable.SystemDescriptor.SystemName}");
                while (_sessionStarted == false)
                    Thread.Sleep(10);
                Thread.Sleep(245);
                Console.WriteLine("Data received.");
                Console.WriteLine("Rank: {0}", _hackable.SystemDescriptor.Rank);
                Console.WriteLine("");
                Console.WriteLine("Starting command shell...");
                Thread.Sleep(750);
                Console.WriteLine("Type 'help' for a list of commands.");
                TerminalBackend.SetShellOverride("> ");
                TerminalBackend.InStory = false;
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
            }).Start();
        }

        [ClientMessageHandler("hackable_data")]
        public static void HackableData(string content, string ip)
        {
            AppearanceManager.SetupWindow(new HackableUI(JsonConvert.DeserializeObject<HackableSystem>(content)));
        }

        public string GetFriendlySystemType(SystemType type)
        {
            switch (type)
            {
                case SystemType.Computer:
                    return "Personal computer";
                case SystemType.Database:
                    return "Database";
                case SystemType.MailServer:
                    return "Email storage & relay server";
                case SystemType.Mobile:
                    return "Cellular device";
                case SystemType.NAS:
                    return "Network-attached storage";
                case SystemType.Router:
                    return "Router";
                case SystemType.WebServer:
                    return "Plexnet web server";
            }
            return "Unknown system (That's a bug.)";
        }

        public void OnLoad()
        {
            _logo.Image = Properties.Resources.justthes.ToTexture2D(UIManager.GraphicsDevice);
            _logo.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            AppearanceManager.SetWindowTitle(this, $"{_hackable.SystemDescriptor.SystemName} - System details");
            _osInfoText.Text = $@"{GetFriendlySystemType(_hackable.SystemType)}, running Plexgate {_hackable.SystemType}.

Hack this system to uncover more data.";
        }

        public string GetStateActionText()
        {
            switch (_state)
            {
                case 0:
                    return "Start hack";
                case 1:
                    return "Abort hack";
                default:
                    return "Unknown";
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _logo.X = 30;
            _logo.Y = 30;
            _logo.Width = 64;
            _logo.Height = 64;

            _systemName.Font = SkinEngine.LoadedSkin.HeaderFont;
            _systemName.AutoSize = true;
            _systemName.Text = _hackable.SystemDescriptor.SystemName;
            _systemName.X = _logo.X + _logo.Width + 15;
            _systemName.Y = _logo.Y + ((_logo.Height - _systemName.Height) / 2);

            _action.Text = GetStateActionText();
            _action.AutoSize = true;
            _action.Font = SkinEngine.LoadedSkin.Header3Font;
            _action.X = (Width - _action.Width) - 30;
            _action.Y = _logo.Y + ((_logo.Height - _action.Height) / 2);

            _osInfoHeader.Text = "OS information";
            _osInfoHeader.Font = SkinEngine.LoadedSkin.Header2Font;
            _osInfoHeader.AutoSize = true;
            _osInfoHeader.MaxWidth = Width / 4;
            _osInfoHeader.X = Width - (Width / 4) - 60;
            _osInfoHeader.Y = _logo.Y + _logo.Height + 60;

            _osInfoText.Font = SkinEngine.LoadedSkin.MainFont;
            _osInfoText.AutoSize = true;
            _osInfoText.MaxWidth = _osInfoHeader.MaxWidth;
            _osInfoText.X = _osInfoHeader.X;
            _osInfoText.Y = _osInfoHeader.Y + _osInfoHeader.Height + 15;

            if(_state == 0)
            {
                _mainHeader.Visible = true;
                _mainHeader.AutoSize = true;
                _mainHeader.Text = "This system is hackable";
                _mainHeader.Font = SkinEngine.LoadedSkin.Header2Font;
                _mainHeader.X = 30;
                _mainHeader.Y = _osInfoHeader.Y;
                _mainHeader.MaxWidth = Width - _osInfoHeader.X - 90;
                _mainText.Text = "You can hack this system to gain cash, experience points, and other resources... if you're skilled enough to get in.";
                _mainText.X = 30;
                _mainText.Y = _mainHeader.Y + _mainHeader.Height + 15;
                _mainText.AutoSize = true;
                _mainText.Font = SkinEngine.LoadedSkin.MainFont;
                _mainText.MaxWidth = _mainHeader.MaxWidth;
                _terminal.Visible = false;
            }
            else if(_state == 1)
            {
                _terminal.Visible = true;
                _mainHeader.Visible = false;
                _mainText.Visible = false;
                _terminal.X = 0;
                _terminal.Y = _logo.Y + _logo.Height + 21;
                _terminal.Width = _osInfoHeader.X - 31;
                _terminal.Height = Height - _terminal.Y;
            }

            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            base.OnPaint(gfx, target);
            gfx.DrawRectangle(0, _logo.Y + _logo.Height + 20, Width, 1, SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
            gfx.DrawRectangle(_osInfoHeader.X - 30, _logo.Y + _logo.Height + 20, 1, Height - (_logo.Y + _logo.Height + 20), SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor());
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
