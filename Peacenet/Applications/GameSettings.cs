using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Plex.Engine.Config;
using System.Reflection;
using Peacenet.PeacegateThemes;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a GUI for managing Peace engine configuration values.
    /// </summary>
    public class GameSettings : Window
    {
        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private ConfigManager _config = null;

        private ListView _resolutions = null;

        private ScrollView _resolutionScroller = null;

        private Button _apply = new Button();
        private Button _cancel = new Button();

        private ScrollView _configView = new ScrollView();
        private Panel _configPanel = new Panel();

        private Label _graphics = new Label();

        private CheckLabel _gfxFullscreen = new CheckLabel();
        private Label _gfxGuiScale = new Label();
        private Button _gfxSetGuiScale = new Button();


        private Label _audio = new Label();

        private CheckLabel _ignoreOpacity = new CheckLabel();
        private CheckLabel _fadingWindows = new CheckLabel();

        private Label _audioVolume = new Label();
        private SliderBar _audioVolumeSlider = new SliderBar();

        private Label _aboutHeader = new Label();

        private Label _ux = new Label();

        private Label _aboutText = new Label();

        private Button _license = new Button();

        private Label _fontSizeLabel = new Label();
        private ListView _fontSizeList = new ListView();

        /// <inheritdoc/>
        public GameSettings(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            SetWindowStyle(WindowStyle.Dialog);
            Title = "System settings";
            _resolutions = new ListView();
            _resolutionScroller = new ScrollView();
            _resolutionScroller.AddChild(_resolutions);
            AddChild(_resolutionScroller);
            AddChild(_apply);
            AddChild(_cancel);
            _apply.Text = "Apply";
            _cancel.Text = "Cancel";
            _cancel.Click += (o, a) =>
            {
                Close();
            };
            _apply.Click += (o, a) =>
            {
                _config.SetValue("screenResolution", _resolutions.SelectedItem.Value.ToString());
                _config.SetValue("uiFullscreen", _gfxFullscreen.Checked);
                _config.SetValue("audioSfxVolume", _audioVolumeSlider.Value);
                _config.SetValue("uiIgnoreControlOpacity", _ignoreOpacity.Checked);
                _config.SetValue("windowManagerTranslucentWindowsWhileDragging", _fadingWindows.Checked);
                _config.SetValue("theme.fontsize", (PeacenetFontSize)_fontSizeList.SelectedItem.Tag);
                _config.Apply();
                Close();
            };

            AddChild(_configView);
            _configView.AddChild(_configPanel);

            _configPanel.AddChild(_graphics);
            _configPanel.AddChild(_audio);
            _configPanel.AddChild(_aboutHeader);

            _configPanel.AddChild(_audioVolumeSlider);
            _configPanel.AddChild(_audioVolume);

            _configPanel.AddChild(_ux);
            _configPanel.AddChild(_ignoreOpacity);
            _configPanel.AddChild(_fadingWindows);

            _configPanel.AddChild(_gfxFullscreen);
            _configPanel.AddChild(_gfxSetGuiScale);
            _configPanel.AddChild(_gfxGuiScale);

            _gfxGuiScale.AutoSize = true;
            _gfxSetGuiScale.Text = "Change GUI Scale";
            _gfxSetGuiScale.Click += (o, a) =>
            {
                var scalesettings = new ScreenScaleSetter(WindowSystem);
                scalesettings.Show();
            };
            
            if (_needsPopulate)
            {
                PopulateResolutions();
                _needsPopulate = false;
            }
            _configPanel.AddChild(_aboutText);

            var peacenetVersion = this.GetType().Assembly.GetName().Version;
            var engineVersion = typeof(Plexgate).Assembly.GetName().Version;

            if(peacenetVersion != null)
            {
                _aboutText.Text += $"Peacenet - Version {peacenetVersion.Major}.{peacenetVersion.Minor} - Milestone {peacenetVersion.Build} build {peacenetVersion.Revision}.";
            }
            if (engineVersion != null)
            {
                _aboutText.Text += $"\nPeace Engine - Version {engineVersion.Major}.{engineVersion.Minor} - Milestone {engineVersion.Build} build {engineVersion.Revision}.";
            }
            _license.Text = "License";

            _configPanel.AddChild(_license);

            _license.Click += (o, a) =>
            {
                var gpl = new GPLInfo(WindowSystem);
                gpl.Show();
            };

            _fontSizeLabel.Text = "Font size";
            _fontSizeLabel.AutoSize = true;

            _configPanel.AddChild(_fontSizeLabel);
            _configPanel.AddChild(_fontSizeList);
        }

        private bool _needsPopulate = true;

        /// <summary>
        /// Forces re-population of the list of available screen resolutions.
        /// </summary>
        public void PopulateResolutions()
        {
            _resolutions.ClearItems();
            string[] resolutions = _plexgate.GetAvailableResolutions();
            string defres = _plexgate.GetSystemResolution();
            string setres = _config.GetValue("screenResolution", defres);
            foreach (var res in resolutions)
            {
                var lvitem = new ListViewItem();
                lvitem.Value = res;
                _resolutions.AddItem(lvitem);
            }
            _resolutions.SelectedIndex = Array.IndexOf(_resolutions.Items, _resolutions.Items.FirstOrDefault(x => x.Value == setres));

            _gfxFullscreen.Checked = _config.GetValue("uiFullscreen", true);
            _audioVolumeSlider.Value = _config.GetValue("audioSfxVolume", 1f);
            _ignoreOpacity.Checked = _config.GetValue("uiIgnoreControlOpacity", false);
            _fadingWindows.Checked = _config.GetValue("windowManagerTranslucentWindowsWhileDragging", true);

            _fontSizeList.ClearItems();
            foreach (var name in Enum.GetNames(typeof(PeacenetFontSize)))
            {
                var item = new ListViewItem
                {
                    Value = name,
                    Tag = (PeacenetFontSize)Enum.Parse(typeof(PeacenetFontSize), name)
                };
                _fontSizeList.AddItem(item);
            }
            var fontsize = _config.GetValue<PeacenetFontSize>("theme.fontsize", PeacenetFontSize.Small);
            _fontSizeList.SelectedIndex = Array.IndexOf(_fontSizeList.Items, _fontSizeList.Items.FirstOrDefault(x => (PeacenetFontSize)x.Tag == fontsize));
            _fontSizeList.Layout = ListViewLayout.List;
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            _gfxGuiScale.Text = $"GUI Scale: {Math.Round((float)_plexgate.BaseRenderHeight / _plexgate.GetRenderScreenSize().Y, 2)}x";

            _resolutionScroller.X = 15;
            _resolutionScroller.Y = 15;
            _resolutions.Width = (Width - 30) / 3;
            _resolutionScroller.Height = (Height - 30);
            _resolutions.Layout = ListViewLayout.List;

            _apply.X = (Width - _apply.Width) - 15;
            _apply.Y = (Height - _apply.Height) - 15;
            _cancel.X = (_apply.X - _cancel.Width) - 5;
            _cancel.Y = (_apply.Y);

            _configView.X = _resolutionScroller.X + _resolutionScroller.Width + 10;
            _configView.Y = 15;
            _configPanel.AutoSize = true;
            _configPanel.Width = (Width - _configView.X) - 15;
            _configView.Height = _cancel.Y - 30;

            _graphics.X = 15;
            _graphics.Y = 15;
            _graphics.AutoSize = true;
            _graphics.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _graphics.Text = "Graphics";

            _gfxFullscreen.X = 20;
            _gfxFullscreen.Y = _graphics.Y + _graphics.Height + 10;
            _gfxFullscreen.MaxWidth = (_configPanel.Width - 35);
            _gfxFullscreen.Text = "Fullscreen";

            _gfxSetGuiScale.X = (_configPanel.Width - _gfxSetGuiScale.Width) - 15;

            _gfxGuiScale.MaxWidth = _gfxSetGuiScale.X - 30;
            _gfxGuiScale.X = 15;

            int guiScaleMax = Math.Max(_gfxGuiScale.Height, _gfxSetGuiScale.Height);
            if (guiScaleMax == _gfxGuiScale.Height)
            {
                _gfxGuiScale.Y = _gfxFullscreen.Y + _gfxFullscreen.Height + 7;
                _gfxSetGuiScale.Y = _gfxGuiScale.Y + ((_gfxGuiScale.Height - _gfxSetGuiScale.Height) / 2);
            }
            else
            {
                _gfxSetGuiScale.Y = _gfxFullscreen.Y + _gfxFullscreen.Height + 7;
                _gfxGuiScale.Y = _gfxSetGuiScale.Y + ((_gfxSetGuiScale.Height - _gfxGuiScale.Height) / 2);
            }

            _audio.X = 15;
            _audio.Y = _gfxFullscreen.Y + _gfxFullscreen.Height + guiScaleMax + 17;
            _audio.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _audio.AutoSize = true;
            _audio.Text = "Audio";

            _audioVolume.X = 20;
            _audioVolume.Y = _audio.Y + _audio.Height + 10;
            _audioVolume.Text = $"Audio volume:";
            _audioVolume.AutoSize = true;

            _audioVolumeSlider.X = _audioVolume.X + _audioVolume.Width + 15;
            _audioVolumeSlider.Width = (_configPanel.Width - _audioVolumeSlider.X) - 15;
            _audioVolumeSlider.Y = _audioVolume.Y + ((_audioVolume.Height - _audioVolumeSlider.Height) / 2);


            _ux.Text = "User experience";
            _ux.X = 15;
            _ux.Y = _audioVolume.Y + _audioVolume.Height + 10;
            _ux.AutoSize = true;
            _ux.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;

            _ignoreOpacity.X = 25;
            _ignoreOpacity.Y = _ux.Y + _ux.Height + 10;
            _ignoreOpacity.MaxWidth = (_configPanel.Width - 70);
            _ignoreOpacity.Text = @"Disable Compositor

To allow certain user interface effects (such as translucency), Peace Engine uses a Compositor, allowing certain UI elements to render to a separate buffer and have that buffer rendered to the screen with effects applied.

Since these operations can be taxing on lower-end hardware, it is advised to disable the Compositor if you experience frequent frame drops or disappearing UI elements.

(Note: Disabling the Compositor may affect the game's look and feel.)";

            _fadingWindows.X = 25;
            _fadingWindows.MaxWidth = _ignoreOpacity.MaxWidth;
            _fadingWindows.Enabled = !_ignoreOpacity.Checked;
            if (_ignoreOpacity.Checked)
                _fadingWindows.Checked = false;
            _fadingWindows.Y = _ignoreOpacity.Y + _ignoreOpacity.Height + 10;
            _fadingWindows.Text = @"Fade windows while dragging?

If enabled, windows will fade out as you are dragging them, much like they do in the KWin Window Manager. Requires transparent UI elements.";

            if (_ignoreOpacity.Checked)
                _fadingWindows.Text += "\n\n(Compositor must be enabled to turn this setting on.)";

            _fontSizeLabel.MaxWidth = (_configPanel.Width - 30);
            _fontSizeLabel.Y = _fadingWindows.Y + _fadingWindows.Height + 10;
            _fontSizeLabel.X = 15;

            _fontSizeList.X = 15;
            _fontSizeList.Width = _fontSizeLabel.MaxWidth;
            _fontSizeList.Y = _fontSizeLabel.Y + _fontSizeLabel.Height + 5;



            _aboutHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _aboutHeader.Text = "About";
            _aboutHeader.X = 15;
            _aboutHeader.Y = _fontSizeList.Y + _fontSizeList.Height + 10;
            _aboutHeader.AutoSize = true;

            _aboutText.X = 15;
            _aboutText.MaxWidth = _ignoreOpacity.MaxWidth;
            _aboutText.AutoSize = true;
            _aboutText.Y = _aboutHeader.Y + _aboutHeader.Height + 10;

            _license.X = (_configPanel.Width - _license.Width) - 15;
            _license.Y = _aboutText.Y + _aboutText.Height + 15;
        }
        
    }
}
