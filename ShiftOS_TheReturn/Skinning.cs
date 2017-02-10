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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using static ShiftOS.Engine.SaveSystem;
using ShiftOS.Objects.ShiftFS;
using System.Reflection;

namespace ShiftOS.Engine {
    public static class SkinEngine {
        public static ImageLayout GetImageLayout(string img) {
            if (LoadedSkin.SkinImageLayouts.ContainsKey(img)) {
                return LoadedSkin.SkinImageLayouts[img];
            } else {
                LoadedSkin.SkinImageLayouts.Add(img, ImageLayout.Tile);
                return ImageLayout.Tile;
            }
        }

        public static System.Drawing.Image GetImage(string img) {
            var type = typeof(Skin);

            foreach (var field in type.GetFields()) {
                foreach (var attr in field.GetCustomAttributes(false)) {
                    if (attr is ImageAttribute) {
                        var iattr = attr as ImageAttribute;
                        if (iattr.Name == img) {
                            byte[] image = (byte[])field.GetValue(LoadedSkin);
                            return ImageFromBinary(image);
                        }
                    }
                }
            }

            return null;
        }

        public static void SetIconProber(IIconProber prober)
        {
            _iconProber = prober;
        }

        public static Image ImageFromBinary(byte[] image) {
            if (image == null)
                return null;
            Image img = (Bitmap)((new ImageConverter()).ConvertFrom(image));
            return img;
        }

        private static Skin loadedSkin = new Skin();

        public static Skin LoadedSkin
        {
            get
            {
                return loadedSkin;
            }
            private set
            {
                loadedSkin = value;
            }
        }

        public static void Init() {
            Application.ApplicationExit += (o, a) => {
                SaveSkin();
            };

            if (!Utils.FileExists(Paths.GetPath("skin.json"))) {
                LoadedSkin = new ShiftOS.Engine.Skin();
                SaveSkin();
            } else {
                LoadSkin();
            }
            if (SaveSystem.CurrentSave != null) {
                SkinLoaded?.Invoke();
            }
        }

        public static event EmptyEventHandler SkinLoaded;

        public static void LoadSkin() {
            LoadedSkin = JsonConvert.DeserializeObject<Skin>(Utils.ReadAllText(Paths.GetPath("skin.json")));
            SkinLoaded?.Invoke();
            Desktop.ResetPanelButtons();
            Desktop.PopulateAppLauncher();
        }

        public static void SaveSkin() {
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(LoadedSkin, Formatting.Indented));
        }

        private static IIconProber _iconProber = null;

        public static Image GetDefaultIcon(string id)
        {
            if (_iconProber == null)
            {
                return new Bitmap(16, 16);
            }
            else
            {
                foreach(var f in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
                {
                    if(f.EndsWith(".exe") || f.EndsWith(".dll"))
                    {
                        try
                        {
                            var asm = Assembly.LoadFile(f);
                            foreach(var type in asm.GetTypes())
                            {
                                if(type.Name == id)
                                {
                                    foreach(var attr in type.GetCustomAttributes(true))
                                    {
                                        if(attr is DefaultIconAttribute)
                                        {
                                            return _iconProber.GetIcon(attr as DefaultIconAttribute);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                return new Bitmap(16, 16);
            }
        }

        public static Image GetIcon(string id)
        {
            if (!LoadedSkin.AppIcons.ContainsKey(id))
                LoadedSkin.AppIcons.Add(id, null);

            if (LoadedSkin.AppIcons[id] == null)
                return GetDefaultIcon(id);
            else
            {
                using (var sr = new MemoryStream(LoadedSkin.AppIcons[id]))
                {
                    return Image.FromStream(sr);
                }
            }
             
        }
    }

    public interface IIconProber
    {
        Image GetIcon(DefaultIconAttribute attr);
    }

    public class DefaultIconAttribute : Attribute
    {
        public DefaultIconAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }

    public class Skin {
        //borrowing from the discourse theme for the default skin
        private static readonly Color DefaultBackground = Color.FromArgb(0, 0x44, 0x00);
        private static readonly Color DefaultForeground = Color.FromArgb(0xDD, 0xDD, 0xDD);
        private static readonly Color Accent1 = Color.FromArgb(0x66, 0x66, 0x66);
        private static readonly Color Accent2 = Color.FromArgb(0x80, 0, 0);
        private static readonly Color DesktopBG = Color.FromArgb(0x22, 0x22, 0x22);
        private static readonly Font SysFont = new Font("Tahoma", 9F);
        private static readonly Font SysFont2 = new Font("Tahoma", 10F, FontStyle.Bold);
        private static readonly Font Header1 = new Font("Helvetica", 20F, FontStyle.Bold);
        private static readonly Font Header2 = new Font("Helvetica", 17.5F, FontStyle.Bold);
        private static readonly Font Header3 = new Font("Tahoma", 15F, FontStyle.Bold);

        private static readonly Color TitleBG = Color.FromArgb(0x11, 0x11, 0x11);
        private static readonly Color TitleFG = Color.FromArgb(0xaa, 0xaa, 0xaa);

        //Todo: When making Shifter GUI we need to label all these with proper Shifter attributes to get 'em displaying in the right places.

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        [ShifterHidden]
        public Dictionary<string, string> AppNames = new Dictionary<string, string>();

        [ShifterHidden]
        public Dictionary<string, byte[]> AppIcons = new Dictionary<string, byte[]>();


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_title_text")]
        [ShifterName("Title Font")]
        [ShifterDescription("The font style for the title text.")]
        public Font TitleFont = SysFont2;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Font")]
        [ShifterDescription("The font style used by the system.")]
        public Font MainFont = SysFont;
        
        [ShifterEnumMask(new[] { "Right", "Left"})]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Title button position")]
        [ShifterDescription("Where should the title buttons be located?")]
        public int TitleButtonPosition = 0;

        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("1st level header")]
        [ShifterDescription("The font used in level 1 (title) headers.")]
        public Font HeaderFont = Header1;


        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("2nd level header")]
        [ShifterDescription("The font used in level 2 (subtitle) headers.")]
        public Font Header2Font = Header2;


        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("3rd level header")]
        [ShifterDescription("The font used in level 3 (section) headers.")]
        public Font Header3Font = Header3;



        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Background")]
        [ShifterDescription("The background color of all system controls in the UI.")]
        public Color ControlColor = DefaultBackground;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Foreground")]
        [ShifterDescription("The foreground color of all system controls in the UI.")]
        public Color ControlTextColor = DefaultForeground;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Text Color")]
        [RequiresUpgrade("shift_title_text")]
        [ShifterDescription("The color of the title text.")]
        public Color TitleTextColor = TitleFG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Background Color")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterDescription("The color of the titlebar's background.")]
        public Color TitleBackgroundColor = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Left Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the left border.")]
        public Color BorderLeftBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Right Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the right border.")]
        public Color BorderRightBackground = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel buttons from top")]
        [ShifterDescription("How far from the top should the panel buttons be?")]
        public int PanelButtonFromTop = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom border.")]
        public Color BorderBottomBackground = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [ShifterName("Panel button holder from left")]
        [ShifterDescription("How far from the left should the panel button holder be?")]
        [RequiresUpgrade("shift_panel_buttons")]
        public int PanelButtonHolderFromLeft = 68;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Left Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom left border.")]
        public Color BorderBottomLeftBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Right Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom right border.")]
        public Color BorderBottomRightBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Color")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button color")]
        public Color CloseButtonColor = Accent2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Color")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button color")]
        public Color MaximizeButtonColor = Accent1;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Color")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button color")]
        public Color MinimizeButtonColor = Accent1;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Background")]
        [RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The background color used by the desktop panel")]
        public Color DesktopPanelColor = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Text Color")]
        [RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The text color used by the desktop panel's clock.")]
        public Color DesktopPanelClockColor = TitleFG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Background Color")]
        [RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The background color used by the desktop panel's clock.")]
        public Color DesktopPanelClockBackgroundColor = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Font")]
        [RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The font used by the desktop panel's clock.")]
        public Font DesktopPanelClockFont = Skin.SysFont2;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock From Right")]
        [RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The position in pixels relative to the width of the desktop panel that the clock will sit at.")]
        public Point DesktopPanelClockFromRight = new Point(2, 2);


        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Height")]
        [RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The height in pixels of the desktop panel.")]
        public int DesktopPanelHeight = 24;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Position")]
        [ShifterEnumMask(new[] { "Top", "Bottom" })]
        [RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The position of the desktop panel.")]
        public int DesktopPanelPosition = 0;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Titlebar Height")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterDescription("The height of the titlebar.")]
        public int TitlebarHeight = 30;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Size")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button size")]
        public Size CloseButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Size")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button size")]
        public Size MaximizeButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Size")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button size")]
        public Size MinimizeButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button From Right")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button location from the right of the titlebar.")]
        public Point CloseButtonFromSide = new Point(3, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button From Right")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button location from the right of the titlebar.")]
        public Point MaximizeButtonFromSide = new Point(24 + 6, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button From Right")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button location from the right of the titlebar.")]
        public Point MinimizeButtonFromSide = new Point(48 + 9, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title text centered?")]
        [RequiresUpgrade("shift_title_text")]
        [ShifterDescription("Is the title text centered?")]
        public bool TitleTextCentered = false;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Text From Left")]
        [ShifterFlag("TitleTextCentered", false)]
        [RequiresUpgrade("shift_title_text")]
        [ShifterDescription("The title text location from the left of the titlebar.")]
        public Point TitleTextLeft = new Point(4, 4);

        [ShifterMeta("Desktop")]
        [ShifterCategory("General")]
        [ShifterName("Desktop Background Color")]
        [ShifterDescription("The background color of the desktop.")]
        public Color DesktopColor = DesktopBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select highlight")]
        public Color Menu_ButtonSelectedHighlight = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select border")]
        public Color Menu_ButtonSelectedHighlightBorder = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed highlight")]
        public Color Menu_ButtonPressedHighlight = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed border")]
        public Color Menu_ButtonPressedHighlightBorder = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked highlight")]
        public Color Menu_ButtonCheckedHighlight = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked border")]
        public Color Menu_ButtonCheckedHighlightBorder = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient border")]
        public Color Menu_ButtonPressedBorder = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient border")]
        public Color Menu_ButtonSelectedBorder = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient start")]
        public Color Menu_ButtonCheckedGradientBegin = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient middle")]
        public Color Menu_ButtonCheckedGradientMiddle = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient end")]
        public Color Menu_ButtonCheckedGradientEnd = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient start")]
        public Color Menu_ButtonSelectedGradientBegin = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient middle")]
        public Color Menu_ButtonSelectedGradientMiddle = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient end")]
        public Color Menu_ButtonSelectedGradientEnd = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient start")]
        public Color Menu_ButtonPressedGradientBegin = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient middle")]
        public Color Menu_ButtonPressedGradientMiddle = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient end")]
        public Color Menu_ButtonPressedGradientEnd = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG")]
        public Color Menu_CheckBackground = Skin.TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Selected)")]
        public Color Menu_CheckSelectedBackground = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Pressed)")]
        public Color Menu_CheckPressedBackground = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient start")]
        public Color Menu_ImageMarginGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient middle")]
        public Color Menu_ImageMarginGradientMiddle = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient end")]
        public Color Menu_ImageMarginGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient start")]
        public Color Menu_MenuStripGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient end")]
        public Color Menu_MenuStripGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item selected")]
        public Color Menu_MenuItemSelected = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item border")]
        public Color Menu_MenuItemBorder = DefaultBackground;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu border")]
        public Color Menu_MenuBorder = DefaultBackground;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient start")]
        public Color Menu_MenuItemSelectedGradientBegin = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient end")]
        public Color Menu_MenuItemSelectedGradientEnd = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient start")]
        public Color Menu_MenuItemPressedGradientBegin = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient middle")]
        public Color Menu_MenuItemPressedGradientMiddle = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient end")]
        public Color Menu_MenuItemPressedGradientEnd = Accent1;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient start")]
        public Color Menu_RaftingContainerGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient end")]
        public Color Menu_RaftingContainerGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 1")]
        public Color Menu_SeparatorDark = DefaultForeground;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 2")]
        public Color Menu_SeparatorLight = TitleFG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient start")]
        public Color Menu_StatusStripGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient end")]
        public Color Menu_StatusStripGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar Border")]
        public Color Menu_ToolStripBorder = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Dropdown background")]
        public Color Menu_ToolStripDropDownBackground = DefaultBackground;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient start")]
        public Color Menu_ToolStripGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient middle")]
        public Color Menu_ToolStripGradientMiddle = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient end")]
        public Color Menu_ToolStripGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient start")]
        public Color Menu_ToolStripContentPanelGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient end")]
        public Color Menu_ToolStripContentPanelGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient start")]
        public Color Menu_ToolStripPanelGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient end")]
        public Color Menu_ToolStripPanelGradientEnd = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu text color")]
        public Color Menu_TextColor = DefaultForeground;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu selected text color")]
        public Color Menu_SelectedTextColor = TitleFG;



        //Images
        [Image("closebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Image")]
        [RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Close Button using this setting.")]
        public byte[] CloseButtonImage = null;

        [Image("minimizebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Image")]
        [RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Minimize Button using this setting.")]
        public byte[] MinimizeButtonImage = null;

        [Image("maximizebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Image")]
        [RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Maximize Button using this setting.")]
        public byte[] MaximizeButtonImage = null;

        [Image("desktopbackground")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("General")]
        [ShifterName("Desktop Background Image")]
        [RequiresUpgrade("skinning")]
        [ShifterDescription("Use an image as your desktop background.")]
        public byte[] DesktopBackgroundImage = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Text Color")]
        [ShifterDescription("Change the color of the App Launcher text.")]
        public Color AppLauncherTextColor = Skin.DefaultForeground;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Selected Text Color")]
        [ShifterDescription("Change the color of the app launcher's text while it is selected.")]
        public Color AppLauncherSelectedTextColor = Skin.TitleFG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Font")]
        [ShifterDescription("Change the font that the App Launcher text is displayed in.")]
        public Font AppLauncherFont = Skin.SysFont2;


        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher text")]
        [ShifterDescription("The text displayed on the app launcher.")]
        [RequiresUpgrade("shift_app_launcher")]
        public string AppLauncherText = "ShiftOS";

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher from left")]
        [ShifterDescription("The position of the app launcher from the left of the Desktop Panel.")]
        [RequiresUpgrade("shift_app_launcher")]
        public Point AppLauncherFromLeft = new Point(0, 0);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher size")]
        [ShifterDescription("The size of the app launcher.")]
        [RequiresUpgrade("shift_app_launcher")]
        public Size AppLauncherHolderSize = new Size(68, 24);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher image")]
        [ShifterDescription("The image that will appear on the app launcher.")]
        [Image("applauncher")]
        [RequiresUpgrade("skinning;shift_app_launcher")]
        public byte[] AppLauncherImage = null;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal font")]
        public Font TerminalFont = new Font("Lucida Console", 9F, FontStyle.Regular);

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal text color")]
        public Color TerminalForeColor = DefaultForeground;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal background color")]
        public Color TerminalBackColor = DesktopBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel background image")]
        [Image("desktoppanel")]
        [RequiresUpgrade("skinning;shift_desktop_panel")]
        public byte[] DesktopPanelBackground = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Titlebar background image")]
        [Image("titlebar")]
        [RequiresUpgrade("skinning;shift_titlebar")]
        public byte[] TitleBarBackground = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterName("Show title corners?")]
        [ShifterDescription("If checked, a left and a right section will appear on the titlebar which is useful for rounded corners, padding, or other useful properties.")]
        public bool ShowTitleCorners = false;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left background color")]
        [ShifterDescription("What color should be used for the left title corner?")]
        public Color TitleLeftCornerBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right background color")]
        [ShifterDescription("What color should be used for the right title corner?")]
        public Color TitleRightCornerBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner width")]
        [ShifterDescription("How wide should the left title corner be?")]
        public int TitleLeftCornerWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner width")]
        [ShifterDescription("How wide should the right title corner be?")]
        public int TitleRightCornerWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("skinning;shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner background image")]
        [ShifterDescription("Select an image to appear as the background texture for the left titlebar corner.")]
        [Image("titleleft")]
        public byte[] TitleLeftBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("skinning;shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner background image")]
        [ShifterDescription("Select an image to appear as the background texture for the right titlebar corner.")]
        [Image("titleright")]
        public byte[] TitleRightBG = null;


        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System color key-out")]
        [ShifterDescription("This is a color that will be represented as \"transparent\" in windows. This does not affect the desktop.")]
        public Color SystemKey = Color.FromArgb(1, 0, 1);

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomborder")]
        [ShifterName("Bottom Border Image")]
        [ShifterDescription("Select an image to appear on the bottom border.")]
        public byte[] BottomBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomrborder")]
        [ShifterName("Bottom Right Border Image")]
        [ShifterDescription("Select an image to appear on the bottom right border.")]
        public byte[] BottomRBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomlborder")]
        [ShifterName("Bottom Left Border Image")]
        [ShifterDescription("Select an image to appear on the bottom left border.")]
        public byte[] BottomLBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders")]
        [Image("leftborder")]
        [ShifterName("Left Border Image")]
        [ShifterDescription("Select an image to appear on the left border.")]
        public byte[] LeftBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders")]
        [Image("rightborder")]
        [ShifterName("Right Border Image")]
        [ShifterDescription("Select an image to appear on the right border.")]
        public byte[] RightBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterName("Left border width")]
        [ShifterDescription("How wide should the left border be?")]
        public int LeftBorderWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterName("Right border width")]
        [ShifterDescription("How wide should the right border be?")]
        public int RightBorderWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterName("Bottom border height")]
        [ShifterDescription("How tall should the bottom border be?")]
        public int BottomBorderWidth = 2;

        [Image("panelbutton")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("skinning;shift_panel_buttons")]
        [ShifterName("Panel button background image")]
        [ShifterDescription("Select a texture to display as the panel button background.")]
        public byte[] PanelButtonBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button size")]
        [ShifterDescription("How big should the panel button be?")]
        public Size PanelButtonSize = new Size(185, 20);

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button background color")]
        [ShifterDescription("Select a background color for the panel button.")]
        public Color PanelButtonColor = Skin.Accent2;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button text color")]
        [ShifterDescription("Select a text color for the panel button.")]
        public Color PanelButtonTextColor = Skin.TitleFG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button text from left")]
        [ShifterDescription("The position relative to the panel button left in pixels that the text is placed at.")]
        public Point PanelButtonFromLeft = new Point(2, 2);

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button font")]
        [ShifterDescription("Select a font for the panel button.")]
        public Font PanelButtonFont = Skin.SysFont2;


        //we DO NOT want this showing in the shifter.
        [ShifterHidden]
        public Dictionary<string, ImageLayout> SkinImageLayouts = new Dictionary<string, ImageLayout>();

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("App icon from side")]
        [ShifterDescription("How far from the side should the icon be?")]
        [RequiresUpgrade("shift_titlebar;app_icons")]
        public Point TitlebarIconFromSide = new Point(4,4);
    }

    public class ShifterHiddenAttribute : Attribute {

    }

    public class ShifterFlagAttribute : Attribute {
        public ShifterFlagAttribute(string flag, bool expected) {
            Expected = expected;
            Flag = flag;
        }

        public bool Expected { get; set; }
        public string Flag { get; set; }
        public bool IsTrue(Skin skn) {
            foreach (var f in skn.GetType().GetFields()) {
                if (f.Name == Flag) {
                    if (f.FieldType == typeof(bool)) {
                        return (bool)f.GetValue(skn) == Expected;
                    }
                }
            }
            throw new ArgumentException("The flag attribute was given an incorrect flag variable name.");
        }
    }

    public class ImageAttribute : Attribute {
        /// <summary>
        /// Attribute a byte array within the Skin object with this attribute and the engine and Shifter will see it as an image and you'll be able to grab the image by calling SkinEngine.GetImage() passing the name you input here.
        /// </summary>
        /// <param name="name">The name you want to reference this image as in the code.</param>
        public ImageAttribute(string name) {
            Name = name;
        }

        public string Name { get; set; }
    }


    public class ShifterEnumMaskAttribute : Attribute {
        public ShifterEnumMaskAttribute(string[] items) {
            Items = items;
        }

        public string[] Items { get; set; }
    }



    public class ShifterNameAttribute : Attribute {
        public ShifterNameAttribute(string name) {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class ShifterDescriptionAttribute : Attribute {
        public ShifterDescriptionAttribute(string description) {
            Description = description;
        }

        public string Description { get; set; }
    }

    public class ShifterCategoryAttribute : Attribute {

        public ShifterCategoryAttribute(string category) {
            Category = category;
        }

        public string Category { get; set; }
    }

    public class ShifterMetaAttribute : Attribute {

        public ShifterMetaAttribute(string meta) {
            Meta = meta;
        }

        public string Meta { get; set; }
    }
}
