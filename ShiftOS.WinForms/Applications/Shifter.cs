using Newtonsoft.Json;
using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Shifter", true, "al_shifter")]
    [RequiresUpgrade("shifter")]
    [WinOpen("shifter")]
    public partial class Shifter : UserControl, IShiftOSWindow
    {
        public Shifter()
        {
            InitializeComponent();
            PopulateShifter();
        }

        public int CodepointValue = 0;
        public List<ShifterSetting> settings = new List<ShifterSetting>();
        public Skin LoadedSkin = null;

        public void PopulateShifter()
        {
            if (LoadedSkin == null)
                LoadedSkin = JsonConvert.DeserializeObject<Skin>(JsonConvert.SerializeObject(SkinEngine.LoadedSkin));

            settings.Clear();

            foreach(var field in LoadedSkin.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShiftoriumFrontend.UpgradeAttributesUnlocked(field))
                {
                    bool addToShifter = true;
                    ShifterSetting setting = new Applications.ShifterSetting();
                    foreach (var attr in field.GetCustomAttributes(false))
                    {
                        if (attr is ShifterHiddenAttribute)
                        {
                            addToShifter = false;
                            continue;
                        }

                        if (attr is ShifterMetaAttribute)
                        {
                            setting.Category = (attr as ShifterMetaAttribute).Meta;
                        }
                        if (attr is ShifterCategoryAttribute)
                        {
                            setting.SubCategory = (attr as ShifterCategoryAttribute).Category;
                        }
                        if (attr is ShifterNameAttribute)
                        {
                            setting.Name = (attr as ShifterNameAttribute).Name;
                        }
                        if (attr is ShifterDescriptionAttribute)
                        {
                            setting.Description = (attr as ShifterDescriptionAttribute).Description;
                        }

                    }
                    if (addToShifter == true)
                    {
                        setting.Field = field;
                        settings.Add(setting);
                    }
                }
            }

            PopulateCategories();
        }

        public void PopulateCategories()
        {
            flmeta.Controls.Clear();

            List<string> cats = new List<string>();

            foreach(var c in this.settings)
            {
                if (!cats.Contains(c.Category))
                {
                    cats.Add(c.Category);
                }
            }

            foreach(var c in cats)
            {
                var btn = new Button();
                btn.Text = c;
                btn.Width = flmeta.Width - (flmeta.Margin.Left * 2);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Click += (o, a) =>
                {
                    PopulateSubcategories(c);
                };

                flmeta.Controls.Add(btn);
                btn.Show();
            }
        }

        public void PopulateSubcategories(string cat)
        {
            flcategory.Controls.Clear();

            List<string> cats = new List<string>();

            foreach (var c in this.settings)
            {
                if (c.Category == cat)
                {
                    if (!cats.Contains(c.SubCategory))
                    {
                        cats.Add(c.SubCategory);
                    }
                }
            }

            foreach (var c in cats)
            {
                var btn = new Button();
                btn.Text = c;
                btn.Width = flcategory.Width - (flcategory.Margin.Left * 2);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Click += (o, a) =>
                {
                    PopulateBody(cat, c);
                };

                flcategory.Controls.Add(btn);
                btn.Show();
            }
        }

        public void PopulateBody(string cat, string subcat)
        {
            flbody.Controls.Clear();

            List<ShifterSetting> cats = new List<ShifterSetting>();

            foreach (var c in this.settings)
            {
                if (c.SubCategory == subcat && c.Category == cat)
                {
                    if (c.Field.FlagFullfilled(LoadedSkin))
                    {
                        if (!cats.Contains(c))
                        {
                            cats.Add(c);
                        }
                    }
                }
            }

            foreach(var c in cats)
            {
                var lbl = new Label();
                int labelHeight = 0;
                lbl.AutoSize = true;
                lbl.Text = c.Name + ":";
                flbody.Controls.Add(lbl);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Show();
                //Cool - label's in.
                if(c.Field.FieldType == typeof(Point))
                {
                    var width = new TextBox();
                    var height = new TextBox();
                    labelHeight = width.Height; //irony?
                    width.Width = 30;
                    height.Width = width.Width;
                    width.Text = ((Point)c.Field.GetValue(this.LoadedSkin)).X.ToString();
                    height.Text = ((Point)c.Field.GetValue(this.LoadedSkin)).Y.ToString();
                    flbody.SetFlowBreak(height, true);
                    ControlManager.SetupControl(width);
                    ControlManager.SetupControl(height);

                    flbody.Controls.Add(width);
                    width.Show();
                    flbody.Controls.Add(height);
                    height.Show();

                    EventHandler tc = (o, a) =>
                    {
                        try
                        {
                            int x = Convert.ToInt32(width.Text);
                            int y = Convert.ToInt32(height.Text);

                            int oldx = ((Point)c.Field.GetValue(this.LoadedSkin)).X;
                            int oldy = ((Point)c.Field.GetValue(this.LoadedSkin)).Y;

                            if(x != oldx || y != oldy)
                            {
                                c.Field.SetValue(LoadedSkin, new Point(x, y));
                                CodepointValue += 200;
                            }
                        }
                        catch
                        {
                            width.Text = ((Point)c.Field.GetValue(this.LoadedSkin)).X.ToString();
                            height.Text = ((Point)c.Field.GetValue(this.LoadedSkin)).Y.ToString();
                        }
                    };

                    width.TextChanged += tc;
                    height.TextChanged += tc;

                }
                else if(c.Field.FieldType == typeof(string))
                {
                    var str = new TextBox();
                    str.Width = 120;
                    ControlManager.SetupControl(str);
                    labelHeight = str.Height;
                    str.Text = c.Field.GetValue(LoadedSkin).ToString();
                    flbody.SetFlowBreak(str, true);
                    str.TextChanged += (o, a) => { c.Field.SetValue(LoadedSkin, str.Text); CodepointValue += 100; };
                    flbody.Controls.Add(str);
                    str.Show();
                }
                else if(c.Field.FieldType == typeof(byte[]))
                {
                    //We'll assume that this is an image file.
                    var color = new Button();
                    color.Width = 40;
                    labelHeight = color.Height; 
                                                //just so it's flat like the system.
                    ControlManager.SetupControl(color);
                    flbody.SetFlowBreak(color, true);

                    color.BackgroundImage = SkinEngine.ImageFromBinary((byte[])c.Field.GetValue(this.LoadedSkin));
                    color.Click += (o, a) =>
                    {
                        AppearanceManager.SetupDialog(new GraphicPicker(color.BackgroundImage, c.Name, GetLayout(c.Field.GetImageName()), new Action<byte[], Image, ImageLayout>((col, gdiImg, layout) =>
                        {
                            c.Field.SetValue(LoadedSkin, col);
                            color.BackgroundImage = SkinEngine.ImageFromBinary(col);
                            color.BackgroundImageLayout = layout;
                            LoadedSkin.SkinImageLayouts[c.Field.GetImageName()] = layout;
                            CodepointValue += 700;
                        })));
                    };
                    flbody.Controls.Add(color);
                    color.Show();
                }
                else if (c.Field.FieldType == typeof(Size))
                {
                    var width = new TextBox();
                    var height = new TextBox();
                    width.Width = 30;
                    height.Width = width.Width;
                    labelHeight = width.Height;
                    flbody.SetFlowBreak(height, true);

                    width.Text = ((Size)c.Field.GetValue(this.LoadedSkin)).Width.ToString();
                    height.Text = ((Size)c.Field.GetValue(this.LoadedSkin)).Height.ToString();
                    ControlManager.SetupControl(width);
                    ControlManager.SetupControl(height);

                    flbody.Controls.Add(width);
                    width.Show();
                    flbody.Controls.Add(height);
                    height.Show();

                    EventHandler tc = (o, a) =>
                    {
                        try
                        {
                            int x = Convert.ToInt32(width.Text);
                            int y = Convert.ToInt32(height.Text);

                            int oldx = ((Size)c.Field.GetValue(this.LoadedSkin)).Width;
                            int oldy = ((Size)c.Field.GetValue(this.LoadedSkin)).Height;

                            if (x != oldx || y != oldy)
                            {
                                c.Field.SetValue(LoadedSkin, new Size(x, y));
                                CodepointValue += 200;
                            }
                        }
                        catch
                        {
                            width.Text = ((Size)c.Field.GetValue(this.LoadedSkin)).Width.ToString();
                            height.Text = ((Size)c.Field.GetValue(this.LoadedSkin)).Height.ToString();
                        }
                    };

                    width.TextChanged += tc;
                    height.TextChanged += tc;

                }
                else if(c.Field.FieldType == typeof(bool))
                {
                    var check = new CheckBox();
                    check.Checked = ((bool)c.Field.GetValue(LoadedSkin));
                    labelHeight = check.Height;
                    check.CheckedChanged += (o, a) =>
                    {
                        c.Field.SetValue(LoadedSkin, check.Checked);
                        CodepointValue += 50;
                    };
                    flbody.SetFlowBreak(check, true);

                    flbody.Controls.Add(check);
                    check.Show();
                }
                else if(c.Field.FieldType == typeof(Font))
                {
                    var name = new ComboBox();
                    var size = new TextBox();
                    var style = new ComboBox();

                    name.Width = 120;
                    labelHeight = name.Height;
                    size.Width = 40;
                    style.Width = 80;
                    flbody.SetFlowBreak(style, true);

                    ControlManager.SetupControl(name);
                    ControlManager.SetupControl(size);
                    ControlManager.SetupControl(style);

                    //populate the font name box
                    foreach(var font in FontFamily.Families)
                    {
                        name.Items.Add(font.Name);
                    }
                    name.Text = ((Font)c.Field.GetValue(LoadedSkin)).Name;

                    size.Text = ((Font)c.Field.GetValue(LoadedSkin)).Size.ToString();

                    //populate the style box
                    foreach(var s in (FontStyle[])Enum.GetValues(typeof(FontStyle)))
                    {
                        style.Items.Add(s.ToString());
                    }
                    style.Text = ((Font)c.Field.GetValue(LoadedSkin)).Style.ToString();

                    name.SelectedIndexChanged += (o, a) =>
                    {
                        var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                        var f = en[style.SelectedIndex];

                        c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToInt32(size.Text), f));
                        CodepointValue += 100;
                    };

                    style.SelectedIndexChanged += (o, a) =>
                    {
                        var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                        var f = en[style.SelectedIndex];

                        c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToInt32(size.Text), f));
                        CodepointValue += 50;
                    };

                    size.TextChanged += (o, a) =>
                    {
                        try
                        {
                            var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                            var f = en[style.SelectedIndex];

                            c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToInt32(size.Text), f));
                        }
                        catch
                        {
                            size.Text = ((Font)c.Field.GetValue(LoadedSkin)).Size.ToString();
                        }
                        CodepointValue += 50;
                    };

                    flbody.Controls.Add(name);
                    flbody.Controls.Add(size);
                    flbody.Controls.Add(style);

                    name.Show();
                    size.Show();
                    style.Show();

                }
                else if(c.Field.FieldType == typeof(Color))
                {
                    var color = new Button();
                    color.Width = 40;
                    labelHeight = color.Height;
                    //just so it's flat like the system.
                    ControlManager.SetupControl(color);

                    color.BackColor = ((Color)c.Field.GetValue(LoadedSkin));
                    color.BackColorChanged += (o, a) =>
                    {
                        c.Field.SetValue(LoadedSkin, color.BackColor);
                    };
                    color.Click += (o, a) =>
                    {
                        AppearanceManager.SetupDialog(new ColorPicker(color.BackColor, c.Name, new Action<Color>((col) =>
                        {
                            color.BackColor = col;
                            CodepointValue += 300;
                        })));
                    };
                    flbody.SetFlowBreak(color, true);

                    flbody.Controls.Add(color);
                    color.Show();
                }
                else if(c.Field.FieldType == typeof(int))
                {
                    if (c.Field.HasShifterEnumMask())
                    {
                        var name = new ComboBox();
                        name.Width = 120;
                        ControlManager.SetupControl(name);
                        string[] items = c.Field.GetShifterEnumMask();
                        foreach(var item in items)
                        {
                            name.Items.Add(item);
                        }
                        name.SelectedIndex = (int)c.Field.GetValue(LoadedSkin);
                        name.SelectedIndexChanged += (o, a) =>
                        {
                            c.Field.SetValue(LoadedSkin, name.SelectedIndex);
                            CodepointValue += 75;
                        };
                        labelHeight = name.Height;
                        flbody.Controls.Add(name);
                        name.Show();
                        flbody.SetFlowBreak(name, true);

                    }
                    else
                    {
                        var width = new TextBox();
                        width.Width = 30;
                        width.Text = ((int)c.Field.GetValue(this.LoadedSkin)).ToString();
                        ControlManager.SetupControl(width);
                        labelHeight = width.Height;
                        flbody.Controls.Add(width);
                        width.Show();

                        EventHandler tc = (o, a) =>
                        {
                            try
                            {
                                int x = Convert.ToInt32(width.Text);

                                int oldx = ((int)c.Field.GetValue(this.LoadedSkin));

                                if (x != oldx)
                                {
                                    c.Field.SetValue(LoadedSkin, x);
                                    CodepointValue += 75;
                                }
                            }
                            catch
                            {
                                width.Text = ((int)c.Field.GetValue(this.LoadedSkin)).ToString();
                            }
                        };

                        width.TextChanged += tc;
                        flbody.SetFlowBreak(width, true);

                    }
                }
                lbl.AutoSize = false;
                lbl.Width = (int)this.CreateGraphics().MeasureString(lbl.Text, SkinEngine.LoadedSkin.MainFont).Width + 15;
                lbl.Height = labelHeight;
                lbl.TextAlign = ContentAlignment.MiddleLeft;

                if (!string.IsNullOrWhiteSpace(c.Description))
                {
                    var desc = new Label();
                    flbody.SetFlowBreak(desc, true);
                    desc.Text = c.Description;
                    desc.AutoSize = true;
                    flbody.Controls.Add(desc);
                    desc.Show();
                }
            }
        }

        public ImageLayout GetLayout(string name)
        {
            if (!LoadedSkin.SkinImageLayouts.ContainsKey(name))
            {
                LoadedSkin.SkinImageLayouts.Add(name, ImageLayout.Tile);
                return ImageLayout.Tile;
            }
            else
            {
                return LoadedSkin.SkinImageLayouts[name];
            }
        }

        private void btnapply_Click(object sender, EventArgs e)
        {
            //Apply the skin.
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(LoadedSkin));
            SkinEngine.LoadSkin();
            CodepointValue = CodepointValue / 4;
            Infobox.Show("{SHIFTER_SKIN_APPLIED}", "{YOU_HAVE_EARNED} " + CodepointValue.ToString() + " {CODEPOINTS}.");
            ShiftOS.Engine.Shiftorium.Silent = true;
            SaveSystem.CurrentSave.Codepoints += CodepointValue;
            SaveSystem.SaveGame();
            ShiftOS.Engine.Shiftorium.Silent = false;
            CodepointValue = 0;
        }

        private void Shifter_Load(object sender, EventArgs e) {

        }

        public void OnLoad()
        {
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

    public class ShifterSetting
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public FieldInfo Field { get; set; }
    }

    public static class ShifterReflectionUtilities
    {
        public static bool HasShifterEnumMask(this FieldInfo field)
        {
            foreach(var attr in field.GetCustomAttributes(false))
            {
                if (attr is ShifterEnumMaskAttribute)
                    return true;
            }
            return false;
        }

        public static bool FlagFullfilled(this FieldInfo field, Skin skn)
        {
            foreach(var attr in field.GetCustomAttributes(false))
            {
                if(attr is ShifterFlagAttribute)
                {
                    return (attr as ShifterFlagAttribute).IsTrue(skn);
                }
            }
            return true;
        }

        public static string GetImageName(this FieldInfo field)
        {
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    if (attr is ImageAttribute)
                    {
                        var eattr = attr as ImageAttribute;
                        return eattr.Name;
                    }
                }
                return null;

        }

        public static string[] GetShifterEnumMask(this FieldInfo field)
        {
            if(field.HasShifterEnumMask())
            {
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    if (attr is ShifterEnumMaskAttribute)
                    {
                        var eattr = attr as ShifterEnumMaskAttribute;
                        return eattr.Items;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
