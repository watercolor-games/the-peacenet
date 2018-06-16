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
using Peacenet.DesktopUI;
using Newtonsoft.Json;
using Plex.Objects;

namespace Peacenet.Applications
{
    /// <summary>
    /// Provides a GUI for managing Peace engine configuration values.
    /// </summary>
    public class GameSettings : Window
    {
        [Dependency]
        private GameLoop _plexgate = null;

        [Dependency]
        private ConfigManager _config = null;

        private ListBox _resolutions = null;

        private ScrollView _resolutionScroller = null;

        private Button _apply = new Button();
        private Button _cancel = new Button();

        private ScrollView _configView = new ScrollView();
        private Stacker _configPanel = new Stacker();

        private string getSettingCategory(string confKey)
        {
            var metadata = _settingsData.metadata.FirstOrDefault(x => x.key == confKey);
            if (metadata != null)
                return metadata.category;
            return "Hidden";
        }

        private CategoryMetadata getCategory(string catname)
        {
            return _settingsData.categories.FirstOrDefault(x => x.name == catname);
        }

        public string getCategoryDescription(string category)
        {
            var first = _settingsData.categories.FirstOrDefault(x => x.name == category);
            if (first != null)
                return first.desc;
            return "There is no description for this setting group.";
        }

        private SettingsData _settingsData = null;

        /// <inheritdoc/>
        public GameSettings(WindowSystem _winsys) : base(_winsys)
        {
            _settingsData = JsonConvert.DeserializeObject<SettingsData>(Properties.Resources.SettingsData);

            Width = 800;
            Height = 600;
            SetWindowStyle(WindowStyle.Dialog);
            Title = "System settings";
            _resolutions = new ListBox();
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
                foreach (var group in _groups)
                {
                    foreach (var setting in group.Settings)
                    {
                        var valueSetter = setting.SettingValue;
                        if (_config.Keys.Contains(valueSetter.ConfigKey))
                        {
                            _config.SetValue(valueSetter.ConfigKey, valueSetter.Value);
                        }
                    }
                }

                if(_resolutions.SelectedItem!=null)
                    _config.SetValue("screenResolution", _resolutions.SelectedItem.ToString());
                _config.Apply();
                Close();
            };

            AddChild(_configView);
            _configView.AddChild(_configPanel);

            if (_needsPopulate)
            {
                PopulateResolutions();
                _needsPopulate = false;
            }

            List<SettingGroup> autoGroups = new List<SettingGroup>();

            foreach (var key in _config.Keys)
            {
                string catName = getSettingCategory(key);
                var group = autoGroups.FirstOrDefault(x => x.Category == catName);
                if (group == null)
                {
                    var catdata = getCategory(catName);
                    if (catdata == null || catdata.show == false)
                        continue;
                    group = new SettingGroup
                    {
                        Category = catName,
                        Description = catdata.desc
                    };
                    _configPanel.AddChild(group);
                    autoGroups.Add(group);
                }

                var metadata = _settingsData.metadata.FirstOrDefault(x => x.key == key);
                if (metadata != null)
                    group.AddSetting(createSetting(metadata));
            }

            _groups.AddRange(autoGroups);

            var guiScaleSetting = new Setting
            {
                Name = "GUI Scale",
                Description = "Text too small? Are things not fitting on your screen? Set the GUI scale to change the size of user interface elements on-screen.",
                SettingValue = new ButtonSettingValue
                {
                    Text = "Change GUI Scale",
                }
            };
            var guiScaleButton = (guiScaleSetting.SettingValue as ButtonSettingValue);
            guiScaleButton.Click += (o, a) =>
            {
                var scaleSettings = new ScreenScaleSetter(WindowSystem);
                
                scaleSettings.Show();
            };

            autoGroups.First(x => x.Category == "Graphics").AddSetting(guiScaleSetting);



            var aboutGroup = new SettingGroup();
            aboutGroup.Category = "About";
            aboutGroup.Description = "Everything you need to know about this game and this game engine.";

            aboutGroup.AddSetting(new Setting
            {
                Name = "License info",
                Description = "The Peacenet and Peace Engine are licensed under the GNU GPL v3.",
                SettingValue = new ButtonSettingValue
                {
                    ConfigKey = null,
                    Value = null,
                    Text = "Read the license",
                }
            });
            (aboutGroup.Settings[0].SettingValue as Button).Click += (o, a) =>
            {
                var gpl = new GPLInfo(WindowSystem);
                gpl.Show();
            };

            _groups.Add(aboutGroup);
            _configPanel.AddChild(aboutGroup);
        }

        private Setting createSetting(SettingMetadata metadata)
        {
            var value = _config.GetValue(metadata.key);

            ISettingValue settingValue = null;

            if (value == null)
            {
                settingValue = new DummySettingValue
                {
                    Value = value,
                    ConfigKey = metadata.key
                };
            }
            else
            {
                var valueType = value.GetType();
                var valueSetter = ReflectMan.Types.FirstOrDefault(x => x.GetInterfaces().Contains(typeof(ISettingValue)) && x.Inherits(typeof(Control)) && x.GetCustomAttributes(false).Any(y => (y is SettingValueTypeAttribute) && (y as SettingValueTypeAttribute).Type == valueType));
                if (valueSetter == null)
                {
                    settingValue = new DummySettingValue
                    {
                        Value = value,
                        ConfigKey = metadata.key
                    };
                }
                else
                {
                    var obj = (ISettingValue)Activator.CreateInstance(valueSetter, null);
                    obj.Value = value;
                    obj.ConfigKey = metadata.key;
                    settingValue = obj;
                }
            }
            return new Setting
            {
                Name = metadata.name,
                Description = metadata.desc,
                SettingValue = settingValue
            };
        }

        private bool _needsPopulate = true;

        private List<SettingGroup> _groups = new List<SettingGroup>();

        /// <summary>
        /// Forces re-population of the list of available screen resolutions.
        /// </summary>
        public void PopulateResolutions()
        {
            _resolutions.Items.Clear();
            string[] resolutions = _plexgate.GetAvailableResolutions();
            string defres = _plexgate.GetSystemResolution();
            string setres = _config.GetValue("screenResolution", defres);
            foreach (var res in resolutions)
            {
                _resolutions.Items.Add(res);
            }
            _resolutions.SelectedIndex = Array.IndexOf(_resolutions.Items.ToArray(), _resolutions.Items.FirstOrDefault(x => x.ToString() == setres));
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if((this.Parent as WindowBorder).WindowStyle == WindowStyle.NoBorder)
            {
                Parent.X = 0;
                Parent.Y = 0;
                Width = Manager.ScreenWidth;
                Height = Manager.ScreenHeight;
            }
            _resolutionScroller.X = 15;
            _resolutionScroller.Y = 15;
            _resolutions.Width = (Width - 30) / 3;
            _resolutions.AutoSize = true;
            _resolutionScroller.Height = (Height - 30);
            
            _apply.X = (Width - _apply.Width) - 15;
            _apply.Y = (Height - _apply.Height) - 15;
            _cancel.X = (_apply.X - _cancel.Width) - 5;
            _cancel.Y = (_apply.Y);

            _configView.X = _resolutionScroller.X + _resolutionScroller.Width + 10;
            _configView.Y = 15;
            _configPanel.AutoSize = true;
            _configPanel.Width = (Width - _configView.X) - 15;
            _configView.Height = _cancel.Y - 30;

            if (_needsPopulate)
            {
                PopulateResolutions();
                _needsPopulate = false;
            }
        }

        private class SettingGroup : Panel
        {
            private Label _categoryText = null;
            private Label _description = null;

            public string Category { get => _categoryText.Text; set => _categoryText.Text = value; }
            public string Description { get => _description.Text; set => _description.Text = value; }

            public SettingGroup()
            {
                _categoryText = new Label();
                _description = new Label();

                AddChild(_categoryText);
                AddChild(_description);

                _categoryText.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;
                _categoryText.AutoSize = true;
                _description.AutoSize = true;
            }

            protected override void OnPaint(GameTime time, GraphicsContext gfx)
            {
            }

            public Setting[] Settings => _settings.ToArray();

            protected override void OnUpdate(GameTime time)
            {
                Width = Math.Max(400, Width);

                _categoryText.X = 15;
                _categoryText.Y = 15;
                _categoryText.MaxWidth = (Width - 30);
                _description.X = 15;
                _description.Y = _categoryText.Y + _categoryText.Height + 7;
                _description.MaxWidth = _categoryText.MaxWidth;

                int stackY = _description.Y + _description.Height + 10;

                foreach (var setting in _settings)
                {
                    setting.X = 15;
                    setting.Y = stackY;
                    setting.Width = Width - 30;
                    stackY += setting.Height + 7;
                }

                Height = stackY + 15;

                base.OnUpdate(time);
            }

            private List<Setting> _settings = new List<Setting>();

            public void AddSetting(Setting setting)
            {
                if (_settings.Contains(setting))
                    return;
                _settings.Add(setting);
                AddChild(setting);
            }

            public void RemoveSetting(Setting setting)
            {
                if (!_settings.Contains(setting))
                    return;
                _settings.Remove(setting);
                RemoveChild(setting);
            }


            public void ClearSettings()
            {
                foreach (var setting in _settings)
                    RemoveChild(setting);
                _settings.Clear();
            }
        }

        private class Setting : Control
        {
            private Label _name = null;
            private Label _description = null;

            private ISettingValue _settingValue;


            public object Value => _settingValue.Value;

            public string Name { get => _name.Text; set => _name.Text = value; }
            public string Description { get => _description.Text; set => _description.Text = value; }
            public GameSettings.ISettingValue SettingValue
            {
                get
                {
                    return _settingValue;
                }
                set
                {
                    if (_settingValue != value)
                    {
                        if (_settingValue != null)
                            RemoveChild(_settingValue as Control);

                        _settingValue = value;
                        AddChild(_settingValue as Control);
                    }

                }
            }

            public Setting()
            {
                _name = new Label();
                _description = new Label();

                _name.AutoSize = true;
                _description.AutoSize = true;
                AddChild(_name);
                AddChild(_description);

                _name.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            }

            protected override void OnPaint(GameTime time, GraphicsContext gfx)
            {
            }

            protected override void OnUpdate(GameTime time)
            {
                var _settingText = _settingValue as Control;

                Width = Math.Max(200, Width);

                _settingText.X = (Width - _settingText.Width) - 5;

                int textWidth = _settingText.X - 9;
                _name.X = 5;
                _description.X = 5;

                _name.MaxWidth = textWidth;
                _description.MaxWidth = textWidth;

                if (Math.Max(_name.Height + 4 + _description.Height, _settingText.Height) == _settingText.Height)
                {
                    _settingText.Y = 5;
                    Height = _settingText.Height + 10;
                    _name.Y = _settingText.Y + ((_settingText.Height - (_name.Height + 4 + _description.Height)) / 2);
                    _description.Y = _name.Y + _name.Height + 4;
                }
                else
                {
                    _name.Y = 5;
                    _description.Y = _name.Y + _name.Height + 4;
                    Height = _description.Y + _description.Height + 5;
                    _settingText.Y = (Height - _settingText.Height) / 2;
                }

            }
        }

        public interface ISettingValue
        {
            string ConfigKey { get; set; }
            object Value { get; set; }
        }

        public class DummySettingValue : Label, ISettingValue
        {
            public string ConfigKey { get; set; }

            private object _val;

            public DummySettingValue()
            {
                AutoSize = true;
                FontStyle = Plex.Engine.Themes.TextFontStyle.Muted;
            }

            public object Value
            {
                get
                {
                    return _val;
                }
                set
                {
                    _val = value;
                    Text = (_val == null) ? "<null>" : _val.ToString();
                }
            }
        }

        public class ButtonSettingValue : Button, ISettingValue
        {
            public string ConfigKey { get; set; }
            public object Value { get; set; }

        }

        [SettingValueType(typeof(float))]
        [SettingValueType(typeof(double))]
        public class SliderSettingValue : SliderBar, ISettingValue
        {
            public SliderSettingValue()
            {
                Width = 175;
            }

            public string ConfigKey { get; set; }
            public object Value { get => base.Value; set => base.Value = Convert.ToSingle(value); }
        }

        [SettingValueType(typeof(bool))]
        public class BooleanSettingValue : CheckLabel, ISettingValue
        {
            public BooleanSettingValue()
            {
                Text = "Enable";
            }

            public string ConfigKey { get; set; }
            public object Value { get => Checked; set => Checked = (bool)value; }
        }
    }

    public class SettingsData
    {
        public CategoryMetadata[] categories { get; set; }
        public SettingMetadata[] metadata { get; set; }
    }

    public class SettingMetadata
    {
        public string key { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string category { get; set; }
    }

    public class CategoryMetadata
    {
        public CategoryMetadata() { show = true; }

        public string name { get; set; }
        public string desc { get; set; }

        public bool show { get; set; }
    }



    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SettingValueTypeAttribute : Attribute
    {
        public Type Type { get; private set; }

        public SettingValueTypeAttribute(Type type)
        {
            this.Type = type;
        }
    }
}