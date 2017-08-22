using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using Microsoft.Xna.Framework;using Newtonsoft.Json;using Plex.Engine;using Plex.Frontend.GUI;namespace Plex.Frontend.Apps{    [WinOpen("plextheme")]    [DefaultTitle("PlexTheme")]    [Launcher("PlexTheme", false, null, "Customization")]    public class PlexTheme : Control, IPlexWindow    {        private Skin _skin = null;        private List<Group> groups = new List<Group>();        public string UIState = "";        public SettingGroup CurrentSettingGroup = null;        public TextControl Title = new TextControl();        public Button BackButton = new Button();        public class SettingGroup
        {
            public string Title { get; set; }
            public List<Property> Properties { get; set; }

            public SettingGroup()
            {
                Properties = new List<Property>();
            }
        }        public class Property
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public System.Reflection.FieldInfo Field { get; set; }
        }        public class Group
        {
            public TextControl Title = new TextControl();
            public ListView ListView = new ListView();
        }                public PlexTheme()
        {
            UIState = GetHashCode().ToString();
            _skin = JsonConvert.DeserializeObject<Skin>(JsonConvert.SerializeObject(SkinEngine.LoadedSkin));
            AddControl(BackButton);
            BackButton.Click += () =>
            {
                SetUIState(this.GetHashCode().ToString());
            };
            AddControl(Title);
            Title.AutoSize = true;
        }        protected override void OnLayout(GameTime gameTime)        {            try
            {
                Title.Font = SkinEngine.LoadedSkin.HeaderFont;
                Title.X = 15;
                Title.Y = 15;
                Title.AutoSize = true;

                int current_y = Title.Y + Title.Height + 20;
                if (UIState == this.GetHashCode().ToString())
                {
                    Title.Text = "Settings";
                    BackButton.Visible = false;
                    foreach (var group in groups)
                    {
                        group.Title.Y = current_y;
                        group.Title.X = 15;
                        group.Title.Font = SkinEngine.LoadedSkin.Header3Font;
                        current_y += group.Title.Height + 10;
                        group.ListView.Y = current_y;
                        group.ListView.X = 15;
                        group.ListView.MaxWidth = Width - 30;
                        current_y += group.ListView.Height + 15;
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
                }
            
            }            catch { }        }        public void SetUIState(string state)
        {
            this.ClearControls();
            AddControl(BackButton);
            AddControl(Title);
            UIState = state;
            if(UIState == this.GetHashCode().ToString())
            {
                ResetMetaListing();
            }            else if(UIState == "ShowSettings")
            {

            }        }        public void ResetMetaListing()        {            var type = _skin.GetType();            List<string> metanames = new List<string>();            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))            {                var meta = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterMetaAttribute) as ShifterMetaAttribute;                if(meta != null)                {                    if (!metanames.Contains(meta.Meta))                        metanames.Add(meta.Meta);                }            }            while(groups.Count > 0)            {                RemoveControl(groups[0].Title);                RemoveControl(groups[0].ListView);                ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; //IT'S THE SEMICOLON PARTYFEST                //that's actually valid C#                //like, VS isn't fucking freaking out                //wtf                //Microsoft, you're drunk.                groups.RemoveAt(0);            }            foreach (var meta in metanames)            {                var tc = new TextControl();                tc.Font = SkinEngine.LoadedSkin.Header3Font;                tc.AutoSize = true;                tc.Text = Localization.Parse(meta);                AddControl(tc);                var lv = new ListView();                foreach(var cat in GetCategoryListing(meta))
                {
                    lv.AddItem(new ListViewItem
                    {
                        Text = Localization.Parse(cat),
                        Tag = cat,
                        ImageKey = cat
                    });
                }                lv.DoubleClick += () =>
                {
                    if(lv.SelectedItem != null)
                    {
                        CurrentSettingGroup = CreateSettingGroup(meta, lv.SelectedItem.Tag);
                        SetUIState("ShowSettings");
                    }
                };                lv.AutoSize = true;                AddControl(lv);                groups.Add(new Group
                {
                    Title = tc,
                    ListView = lv
                });            }        }        public SettingGroup CreateSettingGroup(string meta, string category)
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
        }        public List<string> GetCategoryListing(string metaname)        {            List<string> catnames = new List<string>();            var type = _skin.GetType();            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))            {                var meta = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterMetaAttribute) as ShifterMetaAttribute;                if (meta != null)                {                    if(meta.Meta == metaname)                    {                        var cat = field.GetCustomAttributes(false).FirstOrDefault(x => x is ShifterCategoryAttribute) as ShifterCategoryAttribute;                        if(cat != null)                        {                            if (!catnames.Contains(cat.Category))                                catnames.Add(cat.Category);                        }                    }                }            }            return catnames;        }        public void OnLoad()        {            ResetMetaListing();        }        public void OnSkinLoad()        {        }        public bool OnUnload()        {            return true;        }        public void OnUpgrade()        {        }    }}