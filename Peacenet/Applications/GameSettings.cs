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

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a GUI for managing Peace engine configuration values.
    /// </summary>
    [AppLauncher("System settings", "System")]
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

        private Label _audio = new Label();

        private CheckLabel _ignoreOpacity = new CheckLabel();
        private CheckLabel _fadingWindows = new CheckLabel();

        private Label _audioVolume = new Label();
        private SliderBar _audioVolumeSlider = new SliderBar();

        private Label _ux = new Label();

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
                _config.Apply();
                Close();
            };

            AddChild(_configView);
            _configView.AddChild(_configPanel);

            _configPanel.AddChild(_graphics);
            _configPanel.AddChild(_audio);

            _configPanel.AddChild(_audioVolumeSlider);
            _configPanel.AddChild(_audioVolume);

            _configPanel.AddChild(_ux);
            _configPanel.AddChild(_ignoreOpacity);
            _configPanel.AddChild(_fadingWindows);

            _configPanel.AddChild(_gfxFullscreen);
            if (_needsPopulate)
            {
                PopulateResolutions();
                _needsPopulate = false;
            }

        }

        private bool _needsPopulate = true;

        /// <summary>
        /// Forces re-population of the list of available screen resolutions.
        /// </summary>
        public void PopulateResolutions()
        {
            _resolutions.Clear();
            string[] resolutions = _plexgate.GetAvailableResolutions();
            string defres = _plexgate.GetSystemResolution();
            string setres = _config.GetValue("screenResolution", defres);
            foreach (var res in resolutions)
            {
                var lvitem = new ListViewItem(_resolutions);
                lvitem.Value = res;
                if (setres == res)
                {
                    _resolutions.SelectedIndex = Array.IndexOf(_resolutions.Children, lvitem);
                    lvitem.Selected = true;
                }
            }

            _gfxFullscreen.Checked = _config.GetValue("uiFullscreen", true);
            _audioVolumeSlider.Value = _config.GetValue("audioSfxVolume", 1f);
            _ignoreOpacity.Checked = _config.GetValue("uiIgnoreControlOpacity", false);
            _fadingWindows.Checked = _config.GetValue("windowManagerTranslucentWindowsWhileDragging", true);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
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

            _audio.X = 15;
            _audio.Y = _gfxFullscreen.Y + _gfxFullscreen.Height + 10;
            _audio.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _audio.AutoSize = true;
            _audio.Text = "Audio";

            _audioVolumeSlider.Height = 5;

            _audioVolume.X = 20;
            _audioVolume.Y = _audio.Y + _audio.Height + 10;
            _audioVolume.Text = $"Audio volume: {_audioVolumeSlider.Value * 100}%";
            _audioVolume.AutoSize = true;

            _audioVolumeSlider.X = 350;
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
            _ignoreOpacity.Text = @"Disable UI transparency effects?
                
Disabling transparency effects can help gain performance on low-end GPUs but can degrade the user-experience since a lot of UX-related features rely on this setting being off.";

            _fadingWindows.X = 25;
            _fadingWindows.MaxWidth = _ignoreOpacity.MaxWidth;
            _fadingWindows.Y = _ignoreOpacity.Y + _ignoreOpacity.Height + 10;
            _fadingWindows.Text = @"Fade windows while dragging?

If enabled, windows will fade out as you are dragging them, much like they do in the KWin Window Manager. Requires transparent UI elements.";
        }
    }
}
