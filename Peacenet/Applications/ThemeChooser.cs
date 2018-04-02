using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.Themes;
using Microsoft.Xna.Framework;

namespace Peacenet.Applications
{
    [AppLauncher("Theme Chooser", "System", "Choose the UI theme and window border style for Peacegate OS.")]
    public class ThemeChooser : Window
    {
        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private PictureBox _preview = new PictureBox();
        private ListView _themeList = new ListView();
        private ScrollView _themeView = new ScrollView();

        private Label _themeName = new Label();
        private Label _themeDesc = new Label();

        private Button _apply = new Button();

        public ThemeChooser(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Theme chooser";
            _preview.Width = 448;
            _preview.Height = 252;

            _themeView.Width = 300;
            Width = (_preview.Width + _themeView.Width) + 40;

            AddChild(_preview);
            AddChild(_themeView);
            _themeView.AddChild(_themeList);

            _themeList.Layout = ListViewLayout.List;

            foreach (var item in _pn.Themes)
                _themeList.AddItem(new ListViewItem
                {
                    Value = item.Name,
                    Tag = item
                });

            int index = Array.IndexOf(_pn.Themes, _pn.Themes.FirstOrDefault(x => x.ThemeType == _theme.Theme.GetType()));
            _themeList.SelectedIndex = index;

            _apply.Text = "Apply theme";

            AddChild(_apply);
            AddChild(_themeName);
            AddChild(_themeDesc);

            _themeName.AutoSize = true;
            _themeDesc.AutoSize = true;
            _themeName.FontStyle = TextFontStyle.Header3;

            _apply.Click += (o,a) =>
            {
                if (_themeList.SelectedItem == null)
                    return;
                var themeinfo = (ThemeInfo)_themeList.SelectedItem.Tag;
                var instance = (Theme)_plexgate.New(themeinfo.ThemeType);
                _theme.Theme = instance;
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            _preview.X = 15;
            _preview.Y = 15;

            _themeView.Y = _preview.Y;
            _themeView.X = _preview.X + _preview.Width + 5;

            _themeList.X = 0;
            _themeList.Y = 0;
            _themeList.Width = _themeView.Width;
            
            if(_themeList.SelectedItem != null)
            {
                var info = (_themeList.SelectedItem.Tag as ThemeInfo);
                _preview.Texture = info.Preview;
                _themeName.Text = info.Name;
                _themeDesc.Text = info.Description;
            }
            _preview.Layout = System.Windows.Forms.ImageLayout.Zoom;

            _themeName.MaxWidth = _preview.Width;
            _themeDesc.MaxWidth = _preview.Width;

            _themeName.X = _preview.X;
            _themeName.Y = _preview.Y + _preview.Height + 10;
            _themeDesc.X = _themeName.X;
            _themeDesc.Y = _themeName.Y + _themeName.Height + 5;

            Height = _themeDesc.Y + _themeDesc.Height + 15;

            _apply.Y = (Height - _apply.Height) - 15;
            _apply.X = (Width - _apply.Width) - 15;

            _themeView.Height = _apply.Y - 25;

            base.OnUpdate(time);
        }
    }
}
