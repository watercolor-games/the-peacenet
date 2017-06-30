/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Newtonsoft.Json;
using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;
using System.Linq;
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("{TITLE_SHIFTER}", true, "al_shifter", "{AL_CUSTOMIZATION}")]
    [RequiresUpgrade("shifter")]
    [WinOpen("{WO_SHIFTER}")]
    [DefaultTitle("{TITLE_SHIFTER}")]
    [DefaultIcon("iconShifter")]
    public partial class Shifter : UserControl, IShiftOSWindow
    {
        public Shifter()
        {
            InitializeComponent();
            PopulateShifter();
        }

        [ShifterMeta("Desktop")]
        public void ResetDesktop()
        {
            pnldesktoppreview.BringToFront();
            pnldesktoppreview.Tag = "keepbg";
            SetupDesktop();
        }

        /// <summary>
        /// Populates the panel buttons.
        /// </summary>
        /// <returns>The panel buttons.</returns>
        public void PopulatePanelButtons()
        {
            if (DesktopFunctions.ShowDefaultElements == true)
            {
                panelbuttonholder.Controls.Clear();
                if (Shiftorium.IsInitiated == true)
                {
                    if (Shiftorium.UpgradeInstalled("wm_panel_buttons"))
                    {

                        var pnlbtn = new Panel();
                        pnlbtn.Margin = new Padding(2, LoadedSkin.PanelButtonFromTop, 0, 0);
                        pnlbtn.BackColor = LoadedSkin.PanelButtonColor;
                        pnlbtn.BackgroundImage = GetImage("panelbutton");
                        pnlbtn.BackgroundImageLayout = GetImageLayout("panelbutton");

                        var pnlbtntext = new Label();
                        pnlbtntext.Text = "Panel Button Text";
                        pnlbtntext.AutoSize = true;
                        pnlbtntext.Location = LoadedSkin.PanelButtonFromLeft;
                        pnlbtntext.ForeColor = LoadedSkin.PanelButtonTextColor;
                        pnlbtntext.BackColor = Color.Transparent;

                        pnlbtn.BackColor = LoadedSkin.PanelButtonColor;
                        if (pnlbtn.BackgroundImage != null)
                        {
                            pnlbtntext.BackColor = Color.Transparent;
                        }
                        pnlbtn.Size = LoadedSkin.PanelButtonSize;
                        pnlbtn.Tag = "keepbg";
                        pnlbtntext.Tag = "keepbg";
                        pnlbtn.Controls.Add(pnlbtntext);
                        this.panelbuttonholder.Controls.Add(pnlbtn);
                        pnlbtn.Show();
                        pnlbtntext.Show();

                        pnlbtntext.Font = LoadedSkin.PanelButtonFont;




                    }
                }
            }

        }

        /// <summary>
        /// Setups the desktop.
        /// </summary>
        /// <returns>The desktop.</returns>
        public void SetupDesktop()
        {
            if (DesktopFunctions.ShowDefaultElements == true)
            {
                desktoppanel.BackColor = Color.Green;

                //upgrades

                if (Shiftorium.IsInitiated == true)
                {
                    desktoppanel.Visible = Shiftorium.UpgradeInstalled("desktop");
                    lbtime.Visible = Shiftorium.UpgradeInstalled("desktop_clock_widget");

                    //skinning
                    lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;

                    panelbuttonholder.Top = 0;
                    panelbuttonholder.Left = LoadedSkin.PanelButtonHolderFromLeft;
                    panelbuttonholder.Height = desktoppanel.Height;
                    panelbuttonholder.BackColor = Color.Transparent;
                    panelbuttonholder.Margin = new Padding(0, 0, 0, 0);

                    sysmenuholder.Visible = Shiftorium.UpgradeInstalled("app_launcher");

                    //The Color Picker can give us transparent colors - which Windows Forms fucking despises when dealing with form backgrounds.
                    //To compensate, we must recreate the desktop color and make the alpha channel '255'.
                    pnldesktoppreview.BackColor = Color.FromArgb(LoadedSkin.DesktopColor.R, LoadedSkin.DesktopColor.G, LoadedSkin.DesktopColor.B);
                    //Not doing this will cause an ArgumentException.

                    pnldesktoppreview.BackgroundImage = SkinEngine.GetImage("desktopbackground");
                    pnldesktoppreview.BackgroundImageLayout = GetImageLayout("desktopbackground");
                    desktoppanel.BackColor = LoadedSkin.DesktopPanelColor;

                    var pnlimg = GetImage("desktoppanel");
                    if (pnlimg != null)
                    {
                        var bmp = new Bitmap(pnlimg);
                        bmp.MakeTransparent(Color.FromArgb(1, 0, 1));
                        pnlimg = bmp;
                    }

                    desktoppanel.BackgroundImage = pnlimg;
                    if (desktoppanel.BackgroundImage != null)
                    {
                        desktoppanel.BackColor = Color.Transparent;
                    }
                    var appimg = GetImage("applauncher");
                    if (appimg != null)
                    {
                        var bmp = new Bitmap(appimg);
                        bmp.MakeTransparent(Color.FromArgb(1, 0, 1));
                        appimg = bmp;
                    }
                    menuStrip1.BackgroundImage = appimg;
                    lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;
                    lbtime.Font = LoadedSkin.DesktopPanelClockFont;
                    if (desktoppanel.BackgroundImage == null)
                    {
                        lbtime.BackColor = LoadedSkin.DesktopPanelClockBackgroundColor;
                    }
                    else
                    {
                        lbtime.BackColor = Color.Transparent;
                    }
                    apps.Text = LoadedSkin.AppLauncherText;
                    sysmenuholder.Location = LoadedSkin.AppLauncherFromLeft;
                    sysmenuholder.Size = LoadedSkin.AppLauncherHolderSize;
                    apps.Size = sysmenuholder.Size;
                    menuStrip1.Renderer = new ShiftOSMenuRenderer(new AppLauncherColorTable(LoadedSkin));
                    desktoppanel.BackgroundImageLayout = GetImageLayout("desktoppanel");
                    desktoppanel.Height = LoadedSkin.DesktopPanelHeight;
                    if (LoadedSkin.DesktopPanelPosition == 1)
                    {
                        desktoppanel.Dock = DockStyle.Bottom;
                    }
                    else
                    {
                        desktoppanel.Dock = DockStyle.Top;
                    }
                }
            }
            else
            {
                desktoppanel.Hide();
            }

            PopulatePanelButtons();
        }




        [ShifterMeta("Windows")]
        public void SetupWindowPreview()
        {
            pnlwindow.BringToFront();
            WBSetup();
        }

        [ShifterMeta("Menus")]
        public void SetupMenusPreview()
        {
            SetupWindowPreview();
        }

        [ShifterMeta("System")]
        public void SetupSystemPreview()
        {
            SetupWindowPreview();
        }

        /// <summary>
        /// Setup this instance.
        /// </summary>
        public void WBSetup()
        {
            this.lbtitletext.Text = "Window Preview";

            if (SaveSystem.CurrentSave != null)
            {
                this.pnltitle.Visible = Shiftorium.UpgradeInstalled("wm_titlebar");
                this.pnlclose.Visible = Shiftorium.UpgradeInstalled("close_button");
                this.pnlminimize.Visible = Shiftorium.UpgradeInstalled("minimize_button");
                this.pnlmaximize.Visible =Shiftorium.UpgradeInstalled("maximize_button");
                WBSetupSkin();
            }
            else
            {
                this.pnltitle.Visible = false;
                this.pnlclose.Visible = false;
                this.pnlminimize.Visible = false;
                this.pnlmaximize.Visible = false;

            }
        }

        public Image GetImage(string id)
        {
            var type = typeof(ShiftOS.Engine.Skin);
            foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach(var attrib in field.GetCustomAttributes(false))
                {
                    if(attrib is ImageAttribute)
                    {
                        var img = attrib as ImageAttribute;
                        if(img.Name == id)
                        {
                            return SkinEngine.ImageFromBinary((byte[])field.GetValue(LoadedSkin));
                        }
                    }
                }
            }
            return null;
        }

        public ImageLayout GetImageLayout(string img)
        {
            if (LoadedSkin.SkinImageLayouts.ContainsKey(img))
            {
                return LoadedSkin.SkinImageLayouts[img];
            }
            else
            {
                LoadedSkin.SkinImageLayouts.Add(img, ImageLayout.Tile);
                return ImageLayout.Tile;
            }

        }

        /// <summary>
        /// Setups the skin.
        /// </summary>
        /// <returns>The skin.</returns>
        public void WBSetupSkin()
        {
            this.DoubleBuffered = true;
            var renderer = new ShiftOSMenuRenderer(LoadedSkin);
            mspreview.Renderer = renderer;
            tspreview.Renderer = renderer;
            pnltitle.Height = LoadedSkin.TitlebarHeight;
            pnltitle.BackColor = LoadedSkin.TitleBackgroundColor;
            pnltitle.BackgroundImage = GetImage("titlebar");
            pnltitleleft.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleright.Visible = LoadedSkin.ShowTitleCorners;
            pnltitleleft.BackColor = LoadedSkin.TitleLeftCornerBackground;
            pnltitleright.BackColor = LoadedSkin.TitleRightCornerBackground;
            pnltitleleft.Width = LoadedSkin.TitleLeftCornerWidth;
            pnltitleright.Width = LoadedSkin.TitleRightCornerWidth;
            pnltitleleft.BackgroundImage = GetImage("titleleft");
            pnltitleleft.BackgroundImageLayout = GetImageLayout("titleleft");
            pnltitleright.BackgroundImage = GetImage("titleright");
            pnltitleright.BackgroundImageLayout = GetImageLayout("titleright");
            pnltitle.BackgroundImageLayout = GetImageLayout("titlebar"); //RETARD ALERT. WHY WASN'T THIS THERE WHEN IMAGELAYOUTS WERE FIRST IMPLEMENTED?

            lbtitletext.BackColor = (pnltitle.BackgroundImage != null) ? Color.Transparent : LoadedSkin.TitleBackgroundColor;
            lbtitletext.ForeColor = LoadedSkin.TitleTextColor;
            lbtitletext.Font = LoadedSkin.TitleFont;

            pnlleft.BackColor = LoadedSkin.BorderLeftBackground;
            pnlleft.BackgroundImage = GetImage("leftborder");
            pnlleft.BackgroundImageLayout = GetImageLayout("leftborder");
            pnlleft.Width = LoadedSkin.LeftBorderWidth;
            pnlright.BackColor = LoadedSkin.BorderRightBackground;
            pnlright.BackgroundImage = GetImage("rightborder");
            pnlright.BackgroundImageLayout = GetImageLayout("rightborder");
            pnlright.Width = LoadedSkin.RightBorderWidth;

            pnlbottom.BackColor = LoadedSkin.BorderBottomBackground;
            pnlbottom.BackgroundImage = GetImage("bottomborder");
            pnlbottom.BackgroundImageLayout = GetImageLayout("bottomborder");
            pnlbottom.Height = LoadedSkin.BottomBorderWidth;

            pnlbottomr.BackColor = LoadedSkin.BorderBottomRightBackground;
            pnlbottomr.BackgroundImage = GetImage("bottomrborder");
            pnlbottomr.BackgroundImageLayout = GetImageLayout("bottomrborder");
            pnlbottoml.BackColor = LoadedSkin.BorderBottomLeftBackground;
            pnlbottoml.BackgroundImage = GetImage("bottomlborder");
            pnlbottoml.BackgroundImageLayout = GetImageLayout("bottomlborder");

            lbtitletext.ForeColor = LoadedSkin.TitleTextColor;
            lbtitletext.Font = LoadedSkin.TitleFont;
            pnlclose.BackColor = LoadedSkin.CloseButtonColor;
            pnlclose.BackgroundImage = GetImage("closebutton");
            pnlclose.BackgroundImageLayout = GetImageLayout("closebutton");
            pnlminimize.BackColor = LoadedSkin.MinimizeButtonColor;
            pnlminimize.BackgroundImage = GetImage("minimizebutton");
            pnlminimize.BackgroundImageLayout = GetImageLayout("minimizebutton");
            pnlmaximize.BackColor = LoadedSkin.MaximizeButtonColor;
            pnlmaximize.BackgroundImage = GetImage("maximizebutton");
            pnlmaximize.BackgroundImageLayout = GetImageLayout("maximizebutton");

            pnlclose.Size = LoadedSkin.CloseButtonSize;
            pnlminimize.Size = LoadedSkin.MinimizeButtonSize;
            pnlmaximize.Size = LoadedSkin.MaximizeButtonSize;
            pnlclose.Location = FromRight(LoadedSkin.CloseButtonFromSide);
            pnlminimize.Location = FromRight(LoadedSkin.MinimizeButtonFromSide);
            pnlmaximize.Location = FromRight(LoadedSkin.MaximizeButtonFromSide);
            pnlclose.Left -= pnlclose.Width;
            pnlmaximize.Left -= pnlmaximize.Width;
            pnlminimize.Left -= pnlminimize.Width;

            switch (LoadedSkin.TitleTextCentered)
            {
                case false:
                    lbtitletext.Location = new Point(16 + LoadedSkin.TitlebarIconFromSide.X + LoadedSkin.TitleTextLeft.X,
                            LoadedSkin.TitleTextLeft.Y);
                    break;
                default:
                    lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                    lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                    break;
            }

            if (Shiftorium.UpgradeInstalled("app_icons"))
            {
                pnlicon.Show();
                pnlicon.Size = new Size(16, 16);
                pnlicon.BackColor = Color.Transparent;
                pnlicon.BackgroundImageLayout = ImageLayout.Stretch;
                pnlicon.Location = LoadedSkin.TitlebarIconFromSide;
            }
            else
            {
                pnlicon.Hide();
            }
        }

        /// <summary>
        /// Froms the right.
        /// </summary>
        /// <returns>The right.</returns>
        /// <param name="input">Input.</param>
        public Point FromRight(Point input)
        {
            return new Point(pnltitle.Width - input.X, input.Y);
        }


        public uint CodepointValue = 0;
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
                    foreach(var mth in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    {
                        foreach(var attrib in mth.GetCustomAttributes(false))
                        {
                            if(attrib is ShifterMetaAttribute)
                            {
                                var meta = attrib as ShifterMetaAttribute;
                                if(meta.Meta == btn.Text)
                                {
                                    mth?.Invoke(this, null);
                                }
                            }
                        }
                    }

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

        public void InvokeSetup(string cat)
        {
            foreach(var mth in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach(var attr in mth.GetCustomAttributes(false))
                {
                    if(attr is ShifterMetaAttribute)
                    {
                        if ((attr as ShifterMetaAttribute).Meta == cat)
                            mth.Invoke(this, null);
                    }
                }
            }
        }

        public void PopulateBody(string cat, string subcat)
        {
            flbody.Controls.Clear();

            IEnumerable<ShifterSetting> cats = this.settings.Where(x => x.SubCategory == subcat && x.Category == cat && x.Field.FlagFullfilled(LoadedSkin)).OrderBy(x=>x.Name);

            new Thread(() =>
            {
                foreach (var c in cats)
                {
                    Label lbl = null;
                    int labelHeight = 0;
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        lbl = new Label();
                        lbl.AutoSize = true;
                        lbl.Text = c.Name + ":";
                        flbody.Controls.Add(lbl);
                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                        lbl.Show();
                    });
                    //Cool - label's in.
                    if (c.Field.FieldType == typeof(Point))
                    {
                        TextBox width = null;
                        TextBox height = null;
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            width = new TextBox();
                            height = new TextBox();
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

                                    if (x != oldx || y != oldy)
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
                                InvokeSetup(cat);
                            };

                            width.TextChanged += tc;
                            height.TextChanged += tc;
                        });
                    }
                    else if (c.Field.FieldType == typeof(string))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            var str = new TextBox();
                            str.Width = 120;
                            ControlManager.SetupControl(str);
                            labelHeight = str.Height;
                            str.Text = c.Field.GetValue(LoadedSkin).ToString();
                            flbody.SetFlowBreak(str, true);
                            str.TextChanged += (o, a) =>
                            {
                                c.Field.SetValue(LoadedSkin, str.Text); CodepointValue += 100;

                                InvokeSetup(cat);
                            };
                            flbody.Controls.Add(str);
                            str.Show();
                        });
                    }
                    else if (c.Field.FieldType == typeof(byte[]))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
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
                                    InvokeSetup(cat);

                                })));
                            };
                            flbody.Controls.Add(color);
                            color.Show();
                        });
                    }
                    else if (c.Field.FieldType == typeof(Size))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
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
                                InvokeSetup(cat);

                            };

                            width.TextChanged += tc;
                            height.TextChanged += tc;
                        });
                    }
                    else if (c.Field.FieldType == typeof(bool))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            var check = new CheckBox();
                            check.Checked = ((bool)c.Field.GetValue(LoadedSkin));
                            labelHeight = check.Height;
                            check.CheckedChanged += (o, a) =>
                            {
                                c.Field.SetValue(LoadedSkin, check.Checked);
                                CodepointValue += 50;
                                InvokeSetup(cat);

                            };
                            flbody.SetFlowBreak(check, true);

                            flbody.Controls.Add(check);
                            check.Show();
                        });
                    }
                    else if (c.Field.FieldType == typeof(Font))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
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
                            foreach (var font in FontFamily.Families)
                            {
                                name.Items.Add(font.Name);
                            }
                            name.Text = ((Font)c.Field.GetValue(LoadedSkin)).Name;

                            size.Text = ((Font)c.Field.GetValue(LoadedSkin)).Size.ToString();

                            //populate the style box
                            foreach (var s in (FontStyle[])Enum.GetValues(typeof(FontStyle)))
                            {
                                style.Items.Add(s.ToString());
                            }
                            style.Text = ((Font)c.Field.GetValue(LoadedSkin)).Style.ToString();

                            name.SelectedIndexChanged += (o, a) =>
                            {
                                var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                                var f = en[style.SelectedIndex];

                                c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToDouble(size.Text), f));
                                CodepointValue += 100;
                                InvokeSetup(cat);

                            };

                            style.SelectedIndexChanged += (o, a) =>
                            {
                                var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                                var f = en[style.SelectedIndex];

                                c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToDouble(size.Text), f));
                                CodepointValue += 50;
                                InvokeSetup(cat);

                            };

                            size.TextChanged += (o, a) =>
                            {
                                try
                                {
                                    var en = (FontStyle[])Enum.GetValues(typeof(FontStyle));

                                    var f = en[style.SelectedIndex];

                                    c.Field.SetValue(LoadedSkin, new Font(name.Text, (float)Convert.ToDouble(size.Text), f));
                                }
                                catch
                                {
                                    size.Text = ((Font)c.Field.GetValue(LoadedSkin)).Size.ToString();
                                }
                                CodepointValue += 50;
                                InvokeSetup(cat);

                            };

                            flbody.Controls.Add(name);
                            flbody.Controls.Add(size);
                            flbody.Controls.Add(style);

                            name.Show();
                            size.Show();
                            style.Show();
                        });
                    }
                    else if (c.Field.FieldType == typeof(Color))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            var color = new Button();
                            color.Width = 40;
                            labelHeight = color.Height;
                            //just so it's flat like the system.
                            ControlManager.SetupControl(color);

                            color.BackColor = ((Color)c.Field.GetValue(LoadedSkin));
                            color.Click += (o, a) =>
                            {
                                AppearanceManager.SetupDialog(new ColorPicker((Color)c.Field.GetValue(LoadedSkin), c.Name, new Action<Color>((col) =>
                                {
                                    color.BackColor = col;
                                    c.Field.SetValue(LoadedSkin, col);
                                    CodepointValue += 300;
                                    InvokeSetup(cat);

                                })));
                            };
                            flbody.SetFlowBreak(color, true);
                            color.Tag = "keepbg";
                            flbody.Controls.Add(color);
                            color.Show();
                        });
                    }
                    else if (c.Field.FieldType.IsEnum == true)
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            var cBox = new ComboBox();
                            cBox.Width = 150;
                            ControlManager.SetupControl(cBox);

                            foreach (var itm in Enum.GetNames(c.Field.FieldType))
                            {
                                cBox.Items.Add(itm);
                            }

                            cBox.Text = c.Field.GetValue(LoadedSkin).ToString();

                            cBox.SelectedIndexChanged += (o, a) =>
                            {
                                c.Field.SetValue(LoadedSkin, Enum.Parse(c.Field.FieldType, cBox.Text));
                                InvokeSetup(cat);

                            };

                            labelHeight = cBox.Height;

                            flbody.Controls.Add(cBox);
                            cBox.Show();
                            flbody.SetFlowBreak(cBox, true);
                        });
                    }
                    else if (c.Field.FieldType == typeof(int))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            if (c.Field.HasShifterEnumMask())
                            {
                                var name = new ComboBox();
                                name.Width = 120;
                                ControlManager.SetupControl(name);
                                string[] items = c.Field.GetShifterEnumMask();
                                foreach (var item in items)
                                {
                                    name.Items.Add(item);
                                }
                                name.SelectedIndex = (int)c.Field.GetValue(LoadedSkin);
                                name.SelectedIndexChanged += (o, a) =>
                                {
                                    c.Field.SetValue(LoadedSkin, name.SelectedIndex);
                                    CodepointValue += 75;
                                    InvokeSetup(cat);

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
                                    InvokeSetup(cat);

                                };

                                width.TextChanged += tc;
                                flbody.SetFlowBreak(width, true);

                            }
                        });
                    }
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        lbl.AutoSize = false;
                        lbl.Width = (int)this.CreateGraphics().MeasureString(lbl.Text, SkinEngine.LoadedSkin.MainFont).Width + 15;
                        lbl.Height = labelHeight;
                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                    });

                    if (!string.IsNullOrWhiteSpace(c.Description))
                    {
                        Desktop.InvokeOnWorkerThread(() =>
                        {
                            var desc = new Label();
                            flbody.SetFlowBreak(desc, true);
                            desc.Text = c.Description;
                            desc.AutoSize = true;
                            flbody.Controls.Add(desc);
                            desc.Show();
                        });
                    }
                }
            }).Start();
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
            Infobox.Show("{TITLE_SKINAPPLIED}", Localization.Parse("{PROMPT_SKINAPPLIED}", new Dictionary<string, string>
            {
                ["%cp"] = CodepointValue.ToString()
            }));
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
            pnlintro.BringToFront();
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
