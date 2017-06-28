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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [FileHandler("ShiftOS Skin", ".skn", "fileiconskin")]
    [Launcher("{TITLE_SKINLOADER}", true, "al_skin_loader", "{AL_CUSTOMIZATION}")]
    [RequiresUpgrade("skinning")]
    [WinOpen("skin_loader")]
    [DefaultTitle("{TITLE_SKINLOADER}")]
    [DefaultIcon("iconSkinLoader")]
    public partial class Skin_Loader : UserControl, IShiftOSWindow, IFileHandler
    {
        public Skin_Loader()
        {
            InitializeComponent();
            SetupControls(pnlborder);
            SetupControls(pnldesktop);
            LoadedSkin = JsonConvert.DeserializeObject<Skin>(JsonConvert.SerializeObject(SkinEngine.LoadedSkin));
            this.Load += (o, a) => { SetupUI(); };
            
        }

        public void OpenFile(string file)
        {
            AppearanceManager.SetupWindow(this);
            LoadedSkin = JsonConvert.DeserializeObject<Skin>(Objects.ShiftFS.Utils.ReadAllText(file));
            SetupUI();
        }

        public void SetupControls(Control ctrl)
        {
            ctrl.Tag = "keepbg keepfg keepfont";
            foreach (Control c in ctrl.Controls)
                SetupControls(c);
        }

        public Skin LoadedSkin { get; set; }

        public void SetupUI()
        {
            if (LoadedSkin != null)
            {
                SetupDesktop();
                Setup();
            }
        }

        public void SetupDesktop()
        {
            menuStrip1.Renderer = new ShiftOSMenuRenderer();

            this.DoubleBuffered = true;
            desktoppanel.BackColor = Color.Green;

            //upgrades

            if (SaveSystem.CurrentSave != null && LoadedSkin != null)
            {
                desktoppanel.Visible = ShiftoriumFrontend.UpgradeInstalled("desktop");
                lbtime.Visible = ShiftoriumFrontend.UpgradeInstalled("desktop_clock_widget");

                //skinning
                lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;

                sysmenuholder.Visible = ShiftoriumFrontend.UpgradeInstalled("app_launcher");

                //The Color Picker can give us transparent colors - which Windows Forms fucking despises when dealing with form backgrounds.
                //To compensate, we must recreate the desktop color and make the alpha channel '255'.
                pnldesktop.BackColor = Color.FromArgb(LoadedSkin.DesktopColor.R, LoadedSkin.DesktopColor.G, LoadedSkin.DesktopColor.B);
                //Not doing this will cause an ArgumentException.

                pnldesktop.BackgroundImage = GetImage("desktopbackground");
                pnldesktop.BackgroundImageLayout = GetImageLayout("desktopbackground");
                desktoppanel.BackgroundImage = GetImage("desktoppanel");
                menuStrip1.BackgroundImage = GetImage("applauncher");
                lbtime.ForeColor = LoadedSkin.DesktopPanelClockColor;
                lbtime.Font = LoadedSkin.DesktopPanelClockFont;
                lbtime.Text = Applications.Terminal.GetTime();
                lbtime.Left = desktoppanel.Width - lbtime.Width - LoadedSkin.DesktopPanelClockFromRight.X;
                lbtime.Top = LoadedSkin.DesktopPanelClockFromRight.Y;

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
                menuStrip1.Renderer = new ShiftOSMenuRenderer(new AppLauncherColorTable());
                desktoppanel.BackColor = LoadedSkin.DesktopPanelColor;
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

        public Image GetImage(string img)
        {
            var type = typeof(Skin);

            foreach (var field in type.GetFields())
            {
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    if (attr is ImageAttribute)
                    {
                        var iattr = attr as ImageAttribute;
                        if (iattr.Name == img)
                        {
                            byte[] image = (byte[])field.GetValue(LoadedSkin);
                            return SkinEngine.ImageFromBinary(image);
                        }
                    }
                }
            }

            return null;
        }

        bool IsDialog = false;

        public void Setup()
        {
            pnlcontents.BackColor = LoadedSkin.ControlColor;

            this.lbtitletext.Text = Localization.Parse("{TEMPLATE}");
            this.Dock = DockStyle.Fill;

            if (SaveSystem.CurrentSave != null)
            {
                this.pnltitle.Visible = ShiftoriumFrontend.UpgradeInstalled("wm_titlebar");
                this.pnlclose.Visible = ShiftoriumFrontend.UpgradeInstalled("close_button");
                this.pnlminimize.Visible = (IsDialog == false) && ShiftoriumFrontend.UpgradeInstalled("minimize_button");
                this.pnlmaximize.Visible = (IsDialog == false) && ShiftoriumFrontend.UpgradeInstalled("maximize_button");
                SetupSkin();
            }
            else
            {
                this.pnltitle.Visible = false;
                this.pnlclose.Visible = false;
                this.pnlminimize.Visible = false;
                this.pnlmaximize.Visible = false;

            }
        }

        public void SetupSkin()
        {
            pnltitlemaster.Height = LoadedSkin.TitlebarHeight;
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


            lbtitletext.BackColor = LoadedSkin.TitleBackgroundColor;
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
                    lbtitletext.Location = LoadedSkin.TitleTextLeft;
                    break;
                default:
                    lbtitletext.Left = (pnltitle.Width - lbtitletext.Width) / 2;
                    lbtitletext.Top = LoadedSkin.TitleTextLeft.Y;
                    break;
            }
        }

        public Point FromRight(Point input)
        {
            return new Point(pnltitle.Width - input.X, input.Y);
        }

        private void btnapply_Click(object sender, EventArgs e)
        {
            ShiftOS.Objects.ShiftFS.Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(LoadedSkin));
            SkinEngine.LoadSkin();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnloaddefault_Click(object sender, EventArgs e)
        {
            this.LoadedSkin = new ShiftOS.Engine.Skin();
            SetupUI();
        }

        private void btnexport_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".skn" }, FileOpenerStyle.Save, new Action<string>((filename) =>
             {
                 ShiftOS.Objects.ShiftFS.Utils.WriteAllText(filename, JsonConvert.SerializeObject(LoadedSkin));
                 string fname = filename.Split('/')[filename.Split('/').Length - 1];
                 if(!System.IO.Directory.Exists(Paths.SharedFolder + "\\skins"))
                 {
                     System.IO.Directory.CreateDirectory(Paths.SharedFolder + "\\skins");
                 }

                 string path = Paths.SharedFolder + "\\skins\\" + SaveSystem.CurrentUser.Username + "-" + fname;
                 System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(LoadedSkin));
                  
             })));
        }

        private void btnimport_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".skn" }, FileOpenerStyle.Open, new Action<string>((filename) =>
            {
                try
                {
                    LoadedSkin = JsonConvert.DeserializeObject<Skin>(ShiftOS.Objects.ShiftFS.Utils.ReadAllText(filename));
                }
                catch
                {
                    Infobox.Show("Invalid Skin", "This skin is not compatible with this version of ShiftOS.");
                }
                
                SetupUI();
            })));
        }

        public void OnLoad()
        {

            SetupUI();
        }

        public void OnSkinLoad()
        {
            SetupUI();
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            SetupUI();
        }
    }
}
