using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Plex.Engine;
using Plex.Frontend.GUI;
using System.Dynamic;

namespace Plex.Frontend.Apps
{
    [WinOpen("plextheme")]
    [DefaultTitle("PlexTheme")]
    [Launcher("PlexTheme", false, null, "Customization")]
    public class PlexTheme : Control, IPlexWindow
    {
        private Skin _skin
        {
            get
            {
                return SkinEngine.LoadedSkin;
            }
        }
        private List<Group> groups = new List<Group>();
        public string UIState = "";
        public SettingGroup CurrentSettingGroup = null;
        public TextControl Title = new TextControl();
        public Button BackButton = new Button();
        private List<PropertyUI> Properties = new List<PropertyUI>();
        private int _scrollValue = 0;
        private int _scrollMax = 0;

        public class SettingGroup
        {
            public string Title { get; set; }
            public List<Property> Properties { get; set; }

            public SettingGroup()
            {
                Properties = new List<Property>();
            }
        }

        protected override void OnMouseScroll(int value)
        {
            _scrollValue = MathHelper.Clamp(_scrollValue - value, 0, _scrollMax - Height);
        }

        public class PropertyUI
        {
            public TextControl Name { get; set; }
            public TextControl Description { get; set; }
            public Control Value { get; set; }
        }

        public class Property
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public System.Reflection.FieldInfo Field { get; set; }
        }

        public class Group
        {
            public TextControl Title = new TextControl();
            public ListView ListView = new ListView();
        }

        

        public PlexTheme()
        {
            UIState = GetHashCode().ToString();
            AddControl(BackButton);
            BackButton.Click += () =>
            {
                SetUIState(this.GetHashCode().ToString());
            };
            AddControl(Title);
            Title.AutoSize = true;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            try
            {
                Title.Font = SkinEngine.LoadedSkin.HeaderFont;
                Title.X = 15;
                Title.Y = 15 - _scrollValue;
                Title.AutoSize = true;

                int current_y = (Title.Y + Title.Height + 20);

                if (UIState == this.GetHashCode().ToString())
                {
                    Title.Text = "Settings";
                    BackButton.Visible = false;
                    foreach (var group in groups)
                    {
                        group.Title.Y = current_y - _scrollValue;
                        group.Title.X = 15;
                        group.Title.Font = SkinEngine.LoadedSkin.Header3Font;
                        current_y += group.Title.Height + 10;
                        group.ListView.Y = current_y - _scrollValue;
                        group.ListView.X = 15;
                        group.ListView.MaxWidth = Width - 30;
                        current_y += group.ListView.Height + 15;
                        _scrollMax = current_y;
                    }
                }
                else if(UIState == "ShowSettings")
                {
                    BackButton.Visible = true;
                    BackButton.X = 15;
                    BackButton.Y = Title.Y + ((Title.Height - BackButton.Height) / 2);
                    BackButton.Text = " << Back";
                    BackButton.AutoSize = true;
                    Title.X = BackButton.X + BackButton.Width + 15;
                    Title.Text = CurrentSettingGroup.Title;
                    foreach(var property in Properties)
                    {
                        property.Name.Y = current_y - _scrollValue;
                        property.Name.Font = SkinEngine.LoadedSkin.Header3Font;
                        property.Name.AutoSize = true;
                        property.Name.X = 15;
                        property.Value.Y = current_y - _scrollValue;
                        property.Value.X = (Width - property.Value.Width) - 30;
                        property.Name.MaxWidth = (property.Value.X) - 30;
                        current_y += Math.Max(property.Name.Height, property.Value.Height) + 10;
                        property.Description.X = 15;
                        property.Description.Y = current_y - _scrollValue;
                        property.Description.Font = SkinEngine.LoadedSkin.MainFont;
                        property.Description.AutoSize = true;
                        property.Description.MaxWidth = Width - 30;
                        current_y += property.Description.Height + 15;
                        _scrollMax = current_y;
                    }
                }
                _scrollValue = MathHelper.Clamp(_scrollValue, 0, _scrollMax - Height);
            }
            catch { }
        }

        public void SetUIState(string state)
        {
            this.ClearControls();
            AddControl(BackButton);
            AddControl(Title);
            UIState = state;
            if(UIState == this.GetHashCode().ToString())
            {
                ResetMetaListing();
            }
            else if(UIState == "ShowSettings")
            {
                Properties.Clear();
                foreach(var property in CurrentSettingGroup.Properties)
                {
                    var name = new TextControl();
                    name.AutoSize = true;
                    name.Text = property.Name;

                    var description = new TextControl();
                    description.AutoSize = true;
                    description.Text = property.Description;
                    Control value = null;
                    if(property.Field.FieldType == typeof(System.Drawing.Color))
                    {
                        var btn = new Button();
                        btn.Text = "Choose color...";
                        btn.AutoSize = true;
                        btn.Click += () =>
                        {
                            AppearanceManager.SetupDialog(new ColorDialog(property.Name, (System.Drawing.Color)property.Field.GetValue(_skin), (newcolor) =>
                            {
                                property.Field.SetValue(_skin, newcolor);
                                GraphicsSubsystem.UIManager.InvalidateAll();
                                SkinEngine.SaveSkin();
                                SkinEngine.LoadSkin();
                            }));
                        };
                        value = btn;
                    }
                    if(property.Field.FieldType == typeof(bool))
                    {
                        var checker = new CheckBox();
                        checker.Checked = (bool)property.Field.GetValue(_skin);
                        checker.CheckedChanged += () =>
                        {
                            property.Field.SetValue(_skin, checker.Checked);
                        };
                        checker.Width = 16;
                        checker.Height = 16;
                        value = checker;
                    }
                    if (property.Field.FieldType == typeof(System.Drawing.Font))
                    {
                        var btn = new Button();
                        btn.Text = "Select font...";
                        btn.AutoSize = true;
                        btn.Click += () =>
                        {
                            AppearanceManager.SetupDialog(new FontDialog(property.Name, (System.Drawing.Font)property.Field.GetValue(_skin), (newfont) =>
                            {
                                property.Field.SetValue(_skin, newfont);
                                GraphicsSubsystem.UIManager.InvalidateAll();
                                SkinEngine.SaveSkin();
                                SkinEngine.LoadSkin();
                            }));
                        };
                        value = btn;
                    }

                    if (property.Field.FieldType.IsEnum)
                    {
                        var names = Enum.GetNames(property.Field.FieldType);
                        var val = property.Field.GetValue(_skin).ToString();
                        var cbox = new ComboBox();
                        foreach(var ename in names)
                        {
                            cbox.AddItem(ename);
                            
                        }
                        cbox.SelectedIndex = names.ToList().IndexOf(val);
                        cbox.SelectedItemChanged += () =>
                        {
                            if(cbox.SelectedItem != null)
                            {
                                var eval = Enum.Parse(property.Field.FieldType, cbox.SelectedItem.ToString());
                                property.Field.SetValue(_skin, eval);

                            }
                        };
                        cbox.AutoSize = true;
                        value = cbox;
                    }
                    if(property.Field.FieldType == typeof(string) || property.Field.IsNumeric())
                    {
                        value = new TextInput();
                        value.Width = 150;
                        value.Height = 6 + _skin.MainFont.Height;
                        if(property.Field.FieldType == typeof(string))
                        {
                            (value as TextInput).TextFilter = TextFilter.None;
                        }
                        else
                        {
                            (value as TextInput).TextFilter = (property.Field.IsInteger()) ? TextFilter.Integer : TextFilter.Decimal;
                            value.Width = 75;
                        }
                        (value as TextInput).Text = property.Field.GetValue(_skin)?.ToString();
                        (value as TextInput).TextChanged += () =>
                        {
                            property.Field.SetValue(_skin, (value as TextInput).Value);
                        };
                    }
                    if (value == null)
                    {
                        value = new TextControl();
                        value.AutoSize = true;
                        var val = property.Field.GetValue(_skin);
                        string valstr = "null";
                        if (val != null)
                            valstr = val.ToString();
                        ((TextControl)value).Text = valstr;
                    }
                    AddControl(name);
                    AddControl(description);
                    AddControl(value);

                    Properties.Add(new PropertyUI
                    {
                        Name = name,
                        Description = description,
                        Value = value
                    });
                }
            }
        }

        public void ResetMetaListing()
        {
            var type = _skin.GetType();
            List<string> metanames = new List<string>();
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var meta = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterMetaAttribute) as ShifterMetaAttribute;
                if(meta != null)
                {
                    if (!metanames.Contains(meta.Meta))
                        metanames.Add(meta.Meta);
                }
            }
            while(groups.Count > 0)
            {
                RemoveControl(groups[0].Title);
                RemoveControl(groups[0].ListView);
                ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; //IT'S THE SEMICOLON PARTYFEST
                //that's actually valid C#
                //like, VS isn't fucking freaking out
                //wtf
                //Microsoft, you're drunk.
                groups.RemoveAt(0);

            }
            foreach (var meta in metanames)
            {
                var tc = new TextControl();
                tc.Font = SkinEngine.LoadedSkin.Header3Font;
                tc.AutoSize = true;
                tc.Text = Localization.Parse(meta);
                AddControl(tc);
                var lv = new ListView();
                foreach(var cat in GetCategoryListing(meta))
                {
                    lv.AddItem(new ListViewItem
                    {
                        Text = Localization.Parse(cat),
                        Tag = cat,
                        ImageKey = cat
                    });
                }
                lv.DoubleClick += () =>
                {
                    if(lv.SelectedItem != null)
                    {
                        CurrentSettingGroup = CreateSettingGroup(meta, lv.SelectedItem.Tag);
                        SetUIState("ShowSettings");
                    }
                };
                lv.AutoSize = true;
                AddControl(lv);
                groups.Add(new Group
                {
                    Title = tc,
                    ListView = lv
                });
            }
        }

        public SettingGroup CreateSettingGroup(string meta, string category)
        {
            SettingGroup group = new Apps.PlexTheme.SettingGroup();
            group.Title = Localization.Parse(meta) + ": " + Localization.Parse(category);
            var type = _skin.GetType();
            foreach(var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var ameta = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterMetaAttribute) as ShifterMetaAttribute;
                var acategory = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterCategoryAttribute) as ShifterCategoryAttribute;
                if(ameta != null && acategory != null)
                {
                    if(ameta.Meta == meta && acategory.Category == category)
                    {
                        var setting = new Property();
                        var name = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterNameAttribute) as ShifterNameAttribute;
                        var desc = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterDescriptionAttribute) as ShifterDescriptionAttribute;
                        setting.Name = (name == null) ? "Unnamed setting" : Localization.Parse(name.Name);
                        setting.Description = (desc == null) ? "This setting has no name or description applied to it." : Localization.Parse(desc.Description);
                        setting.Field = field;
                        group.Properties.Add(setting);
                    }
                }
            }
            return group;
        }

        public List<string> GetCategoryListing(string metaname)
        {
            List<string> catnames = new List<string>();
            var type = _skin.GetType();
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var meta = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterMetaAttribute) as ShifterMetaAttribute;
                if (meta != null)
                {
                    if(meta.Meta == metaname)
                    {
                        var cat = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterCategoryAttribute) as ShifterCategoryAttribute;
                        if(cat != null)
                        {
                            if (!catnames.Contains(cat.Category))
                                catnames.Add(cat.Category);
                        }
                    }
                }
            }

            return catnames;
        }

        public void OnLoad()
        {
            ResetMetaListing();
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

    public static class ReflectionExtensions
    {
        public static bool IsNumeric(this System.Reflection.FieldInfo info)
        {
            var valtype = info.FieldType;
            return (valtype == typeof(int) || valtype == typeof(short) || valtype == typeof(byte) || valtype == typeof(sbyte) || valtype == typeof(long) || valtype == typeof(float) || valtype == typeof(uint) || valtype == typeof(ulong) || valtype == typeof(decimal) || valtype == typeof(double));
        }

        public static bool IsInteger(this System.Reflection.FieldInfo info)
        {
            var valtype = info.FieldType;
            return (valtype == typeof(int) || valtype == typeof(short) || valtype == typeof(byte) || valtype == typeof(sbyte) || valtype == typeof(long) || valtype == typeof(uint) || valtype == typeof(ulong));
        }

    }
}
