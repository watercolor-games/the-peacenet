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

                _config.Apply();
                Close();
            };
        }

        private bool _needsPopulate = true;

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
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_needsPopulate)
            {
                PopulateResolutions();
                _needsPopulate = false;
            }
            _resolutionScroller.X = 15;
            _resolutionScroller.Y = 15;
            _resolutions.Width = (Width - 30) / 3;
            _resolutionScroller.Height = (Height - 30);
            _resolutions.Layout = ListViewLayout.List;

            _apply.X = (Width - _apply.Width) - 15;
            _apply.Y = (Height - _apply.Height) - 15;
            _cancel.X = (_apply.X - _cancel.Width) - 5;
            _cancel.Y = (_apply.Y);
        }
    }
}
