using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.Themes;
using Peacenet.PeacegateThemes.PanelThemes;
using Microsoft.Xna.Framework;

namespace Peacenet.Applications
{
    [AppLauncher("Desktop Panel Settings", "System", "Customize the look, feel and behaviour of the Peacegate Desktop panels.")]
    public class DesktopPanelSettings : Window
    {
        private Label _panelThemeListHead = new Label();
        private Label _panelThemeListDesc = new Label();

        private ScrollView _panelThemeListScroller = new ScrollView();
        private ListView _panelThemeList = new ListView();
        private ScrollView _mainView = new ScrollView();
        private Panel _mainPanel = new Panel();
        private Button _apply = new Button();

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        public DesktopPanelSettings(WindowSystem _winsys) : base(_winsys)
        {
            Title = "Desktop Panel Settings";
            SetWindowStyle(WindowStyle.Dialog);
            Height = 580;
            Width = 430;

            AddChild(_apply);
            AddChild(_mainView);
            _mainView.AddChild(_mainPanel);
            _mainPanel.AddChild(_panelThemeListHead);
            _mainPanel.AddChild(_panelThemeListDesc);
            _mainPanel.AddChild(_panelThemeListScroller);
            _panelThemeListScroller.AddChild(_panelThemeList);

            _apply.Text = "Apply";

            _mainPanel.AutoSize = true;

            _mainView.Width = 400;
            _mainPanel.Width = 400;
            _mainView.X = 15;
            _mainView.Y = 15;
            
            
            _panelThemeListHead.Text = "Panel theme";
            _panelThemeListHead.AutoSize = true;
            _panelThemeListHead.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
            _panelThemeListDesc.Text = "Panel Themes change the look and feel of the desktop panels, panel buttons, and Peacegate Menu button.";
            _panelThemeListDesc.AutoSize = true;

            _panelThemeListScroller.Height = 192;

            _panelThemeList.Layout = ListViewLayout.List;
            foreach (var theme in _pn.AvailablePanelThemes)
            {
                _panelThemeList.AddItem(new ListViewItem
                {
                    Value = theme.Name,
                    Tag = theme
                });
            }
            _panelThemeList.SelectedIndex = Array.IndexOf(_pn.AvailablePanelThemes, _pn.PanelTheme);

            _apply.Click += (o, a) =>
            {
                if(_panelThemeList.SelectedItem != null)
                {
                    _pn.PanelTheme = (PanelTheme)_panelThemeList.SelectedItem.Tag;
                }
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            _mainView.X = 15;
            _mainView.Y = 15;
            _mainView.Width = Width - 30;
            _mainPanel.Width = _mainView.Width;

            _apply.X = (Width - _apply.Width) - 15;
            _apply.Y = (Height - _apply.Height) - 15;

            _mainView.Height = _apply.Y - 20;

            _panelThemeListHead.X = 10;
            _panelThemeListHead.Y = 10;
            _panelThemeListHead.MaxWidth = _mainPanel.Width - 20;
            _panelThemeListDesc.X = 10;
            _panelThemeListDesc.Y = _panelThemeListHead.Y + _panelThemeListHead.Height + 5;
            _panelThemeListDesc.MaxWidth = _panelThemeListHead.MaxWidth;

            _panelThemeListScroller.X = 10;
            _panelThemeListScroller.Width = _panelThemeListHead.MaxWidth;
            _panelThemeListScroller.Y = _panelThemeListDesc.Y + _panelThemeListDesc.Height + 10;
            _panelThemeList.Width = _panelThemeListScroller.Width;

            base.OnUpdate(time);
        }
    }
}
