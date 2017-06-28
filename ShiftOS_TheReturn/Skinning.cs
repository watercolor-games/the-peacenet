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
using ShiftOS.Engine.Scripting;
namespace ShiftOS.Engine
{
    /// <summary>
    /// Skinning API for Lua.
    /// </summary>
    [Exposed("skinning")]
    public class SkinFunctions
    {
        /// <summary>
        /// Reload the current skin.
        /// </summary>
        public void loadSkin()
        {
            SkinEngine.LoadSkin();
        }

        /// <summary>
        /// Get the current skin info.
        /// </summary>
        /// <returns>A proxy object containing all skin variables.</returns>
        public dynamic getSkin()
        {
            return SkinEngine.LoadedSkin;
        }

        /// <summary>
        /// Set the current skin to the specified <see cref="Skin"/> class. 
        /// </summary>
        /// <param name="skn">The <see cref="Skin"/> class to load.</param>
        public void setSkin(Skin skn)
        {
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(skn));
            SkinEngine.LoadSkin();
        }

        /// <summary>
        /// Retrieves an image from the skin file.
        /// </summary>
        /// <param name="id">The skin image ID</param>
        /// <returns>The loaded image, null (nil in Lua) if none is found.</returns>
        public dynamic getImage(string id)
        {
            return SkinEngine.GetImage(id);
        }
    }

    /// <summary>
    /// Skin engine management class.
    /// </summary>
    public static class SkinEngine
    {
        private static ISkinPostProcessor processor = null;

        /// <summary>
        /// Load a new skin postprocessor into the engine.
        /// </summary>
        /// <param name="_processor">The postprocessor to load.</param>
        public static void SetPostProcessor(ISkinPostProcessor _processor)
        {
            processor = _processor;
        }

        /// <summary>
        /// Retrieve the user-specified image layout of a skin image.
        /// </summary>
        /// <param name="img">The skin image ID.</param>
        /// <returns>The <see cref="ImageLayout"/> for the image.</returns>
        public static ImageLayout GetImageLayout(string img)
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
        /// Retrieves an image from the skin after postprocessing it.
        /// </summary>
        /// <param name="img">The image ID to search.</param>
        /// <returns>The post-processed image, or null if none was found.</returns>
        public static System.Drawing.Image GetImage(string img)
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
                            if (processor != null)
                                image = processor.ProcessImage(image);
                            return ImageFromBinary(image);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Set the engine's current icon prober.
        /// </summary>
        /// <param name="prober">The icon prober to use.</param>
        public static void SetIconProber(IIconProber prober)
        {
            _iconProber = prober;
        }

        /// <summary>
        /// Load a <see cref="Image"/> from a <see cref="byte"/> array.  
        /// </summary>
        /// <param name="image">The array to convert</param>
        /// <returns>The resulting image.</returns>
        public static Image ImageFromBinary(byte[] image)
        {
            if (image == null)
                return null;
            Image img = (Bitmap)((new ImageConverter()).ConvertFrom(image));
            return img;
        }

        private static Skin loadedSkin = new Skin();

        /// <summary>
        /// Gets the currently loaded skin.
        /// </summary>
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

        /// <summary>
        /// Initiates the skin engine.
        /// </summary>
        public static void Init()
        {
            Application.ApplicationExit += (o, a) =>
            {
                SaveSkin();
            };

            if (!Utils.FileExists(Paths.GetPath("skin.json")))
            {
                LoadedSkin = new ShiftOS.Engine.Skin();
                SaveSkin();
            }
            else
            {
                LoadSkin();
            }
            if (SaveSystem.CurrentSave != null)
            {
                SkinLoaded?.Invoke();
            }
        }

        /// <summary>
        /// Occurs when the skin is loaded.
        /// </summary>
        public static event EmptyEventHandler SkinLoaded;

        /// <summary>
        /// Reload the current skin.
        /// </summary>
        public static void LoadSkin()
        {
            LoadedSkin = JsonConvert.DeserializeObject<Skin>(Utils.ReadAllText(Paths.GetPath("skin.json")));
            SkinLoaded?.Invoke();
            Desktop.ResetPanelButtons();
            Desktop.PopulateAppLauncher();
        }

        /// <summary>
        /// Save the skin loaded in memory to the filesystem.
        /// </summary>
        public static void SaveSkin()
        {
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(LoadedSkin, Formatting.Indented));
        }

        private static IIconProber _iconProber = null;

        /// <summary>
        /// Retrieves the default icon for a given icon ID.
        /// </summary>
        /// <param name="id">The icon ID to search.</param>
        /// <returns>The resulting icon image.</returns>
        public static Image GetDefaultIcon(string id)
        {
            if (_iconProber != null)
            {
                foreach (var type in Array.FindAll(ReflectMan.Types, t => t.Name == id))
                {
                    var attr = Array.Find(type.GetCustomAttributes(true), a => a is DefaultIconAttribute);
                    if (attr != null)
                        return _iconProber.GetIcon(attr as DefaultIconAttribute);
                }
            }
            return new Bitmap(16, 16);
        }

        /// <summary>
        /// Retrieves the user-defined icon for a specified icon ID.
        /// </summary>
        /// <param name="id">The icon ID to search.</param>
        /// <returns>The resulting icon image.</returns>
        public static Image GetIcon(string id)
        {
            if (!LoadedSkin.AppIcons.ContainsKey(id))
                LoadedSkin.AppIcons.Add(id, null);

            if (LoadedSkin.AppIcons[id] == null)
            {
                var img = GetDefaultIcon(id);
                using (var mstr = new MemoryStream())
                {
                    img.Save(mstr, System.Drawing.Imaging.ImageFormat.Png);
                    LoadedSkin.AppIcons[id] = mstr.ToArray();
                }
                return img;
            }
            else
            {
                using (var sr = new MemoryStream(LoadedSkin.AppIcons[id]))
                {
                    return Image.FromStream(sr);
                }
            }

        }
    }

    /// <summary>
    /// Interface for probing app icons.
    /// </summary>
    public interface IIconProber
    {
        /// <summary>
        /// Retrieve the icon image from a <see cref="DefaultIconAttribute"/>. 
        /// </summary>
        /// <param name="attr">The attribute data</param>
        /// <returns>The resulting image.</returns>
        Image GetIcon(DefaultIconAttribute attr);
    }

    /// <summary>
    /// Sets the default icon ID for a <see cref="IShiftOSWindow"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class DefaultIconAttribute : Attribute
    {
        public DefaultIconAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }

    /// <summary>
    /// The data stored in any .skn file.
    /// </summary>
    public class Skin
    {
        //borrowing from the discourse theme for the default skin
        private static readonly Color DefaultBackground = Color.FromArgb(0x11, 0x11, 0x11);
        private static readonly Color DefaultForeground = Color.FromArgb(0xDD, 0xDD, 0xDD);
        private static readonly Color Accent1 = Color.FromArgb(0x66, 0x66, 0x66);
        private static readonly Color Accent2 = Color.FromArgb(0x0, 0x80, 0);
        private static readonly Color DesktopBG = Color.FromArgb(0x00, 0x00, 0x00);
        private static readonly Font SysFont = new Font("Tahoma", 9F);
        private static readonly Font SysFont2 = new Font("Tahoma", 10F, FontStyle.Bold);
        private static readonly Font Header1 = new Font("Courier New", 20F, FontStyle.Bold);
        private static readonly Font Header2 = new Font("Courier New", 17.5F, FontStyle.Bold);
        private static readonly Font Header3 = new Font("Courier New", 15F, FontStyle.Bold);

        private static readonly Color TitleBG = Color.FromArgb(0x11, 0x55, 0x11);
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

        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar;skinning")]
        [Image("progressbarbg")]
        [ShifterName("Progress Bar Background Image")]
        [ShifterDescription("Set an image for the background of a progress bar.")]
        public byte[] ProgressBarBG = null;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar;skinning")]
        [Image("progress")]
        [ShifterName("Progress Image")]
        [ShifterDescription("Set the image for the progress inside a progress bar.")]
        public byte[] Progress = null;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar")]
        [ShifterName("Progress bar foreground color")]
        [ShifterDescription("Set the color of the progress indicator.")]
        public Color ProgressColor = Accent1;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar")]
        [ShifterName("Progress bar background color")]
        [ShifterDescription("The background color of the progress bar.")]
        public Color ProgressBarBackgroundColor = Color.Black;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar")]
        [ShifterName("Progress bar block size")]
        [ShifterDescription("If the progress bar style is set to Blocks, this determines how wide each block should be.")]
        public int ProgressBarBlockSize = 15;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [RequiresUpgrade("shift_progress_bar")]
        [ShifterDescription("Set the style of a progress bar.\r\nMarquee: The progress bar will render a box that moves from the left to the right in a loop.\r\nContinuous: Progress is shown by a single, continuous box.\r\nBlocks: Just like Continuous, but the box is split into even smaller boxes of a set width.")]
        [ShifterName("Progress bar style")]
        public ProgressBarStyle ProgressBarStyle = ProgressBarStyle.Continuous;






        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button background color")]
        [ShifterDescription("Set the background color for each button's Idle state.")]
        public Color ButtonBackgroundColor = Skin.DefaultBackground;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons;skinning")]
        [Image("buttonhover")]
        [ShifterName("Button hover image")]
        [ShifterDescription("Set the image that's displayed when the mouse hovers over a button.")]
        public byte[] ButtonHoverImage = null;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("skinning;shift_buttons")]
        [Image("buttonpressed")]
        [ShifterName("Button pressed image")]
        [ShifterDescription("Select an image to show when the user presses a button.")]
        public byte[] ButtonPressedImage = null;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button hover color")]
        [ShifterDescription("Choose the color that displays on a button when the mouse hovers over it.")]
        public Color ButtonHoverColor = Skin.Accent1;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button pressed color")]
        [ShifterDescription("Select the background color for the button when the mouse clicks it.")]
        public Color ButtonPressedColor = Skin.Accent2;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button foreground color")]
        [ShifterDescription("Select the text and border color for each button.")]
        public Color ButtonForegroundColor = Skin.DefaultForeground;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button border width")]
        [ShifterDescription("Set the width, in pixels, of the button's border.")]
        public int ButtonBorderWidth = 2;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons")]
        [ShifterName("Button font")]
        [ShifterDescription("Select the font for the button's text.")]
        public Font ButtonTextFont = Skin.SysFont;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        [RequiresUpgrade("shift_buttons;skinning")]
        [Image("buttonidle")]
        [ShifterName("Button background color")]
        [ShifterDescription("Select an image to show as the button's Idle state.")]
        public byte[] ButtonBG = null;


        [Image("panelclockbg")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel Clock")]
        [ShifterName("Panel Clock Background Image")]
        [ShifterDescription("Set the background image of the panel clock.")]
        [RequiresUpgrade("skinning;shift_panel_clock")]
        public byte[] PanelClockBG = null;

        [ShifterMeta("System")]
        [ShifterCategory("Login Screen")]
        [RequiresUpgrade("gui_based_login_screen")]
        [ShifterName("Login Screen Background Color")]
        [ShifterDescription("Change the background color of the login screen.")]
        public Color LoginScreenColor = Skin.DesktopBG;

        [ShifterMeta("System")]
        [ShifterCategory("Login Screen")]
        [RequiresUpgrade("skinning;gui_based_login_screen")]
        [ShifterName("Login Screen Background Image")]
        [ShifterDescription("Set an image as your login screen!")]
        [Image("login")]
        public byte[] LoginScreenBG = null;


        [RequiresUpgrade("shift_screensaver")]
        [ShifterMeta("System")]
        [ShifterCategory("Screen saver")]
        [ShifterName("Screen saver wait (milliseconds)")]
        [ShifterDescription("How long do you have to stay idle before the screensaver activates?")]
        public int ScreensaverWait = 300000;

        [RequiresUpgrade("skinning;shift_screensaver")]
        [ShifterMeta("System")]
        [ShifterCategory("Screen saver")]
        [ShifterName("Screen saver image")]
        [ShifterDescription("What image should appear on the screen saver?")]
        public byte[] ScreensaverImage = null;



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

        [ShifterEnumMask(new[] { "Right", "Left" })]
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
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Inactive Background Color")]
        [RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterDescription("The color of the titlebar's background when the window isn't active.")]
        public Color TitleInactiveBackgroundColor = Skin.DefaultBackground;



        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Left Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the left border.")]
        public Color BorderLeftBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Left Border Inactive Background")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the left border when the window is inactive.")]
        public Color BorderInactiveLeftBackground = DefaultBackground;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Right Border Background")]
        [RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the right border.")]
        public Color BorderRightBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Right Border Inactive Background")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the right border when the window is inactive.")]
        public Color BorderInactiveRightBackground = DefaultBackground;


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

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Border Inactive Background")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom border when the window is inactive.")]
        public Color BorderInactiveBottomBackground = DefaultBackground;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Use Inactive Border Assets?")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("Do you want to use separate colors and images for inactive Window Borders?")]
        public bool RenderInactiveBorders = false;


        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [ShifterName("Panel button holder from left")]
        [ShifterDescription("How far from the left should the panel button holder be?")]
        [RequiresUpgrade("shift_panel_buttons")]
        public int PanelButtonHolderFromLeft = 100;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Left Border Inactive Background")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom left border when the window is inactive.")]
        public Color BorderInactiveBottomLeftBackground = DefaultBackground;


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
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Right Border Inactive Background")]
        [RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom right border when the window is inactive.")]
        public Color BorderInactiveBottomRightBackground = DefaultBackground;

        #region Windows -> Title Buttons -> Idle -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Color")]
        [RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button color")]
        public Color CloseButtonColor = Color.FromArgb(0x80,0,0);

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
        #endregion

        #region Windows -> Title Buttons -> Over -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Over Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The close button color when the mouse hovers over it.")]
        public Color CloseButtonOverColor = Color.FromArgb(0x80, 0, 0);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Over Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The maximize button color when the mouse hovers over it.")]
        public Color MaximizeButtonOverColor = Accent1;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Over Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The minimize button color when the mouse hovers over it")]
        public Color MinimizeButtonOverColor = Accent1;
        #endregion

        #region Windows -> Title Buttons -> Down -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Down Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The close button color when the mouse clicks it.")]
        public Color CloseButtonDownColor = Color.FromArgb(0x80, 0, 0);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Down Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The maximize button color when the mouse clicks it.")]
        public Color MaximizeButtonDownColor = Accent1;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Down Color")]
        [RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The minimize button color when the mouse clicks it")]
        public Color MinimizeButtonDownColor = Accent1;
        #endregion


        [ShifterHidden]
        public CommandParser CurrentParser = CommandParser.GenerateSample();


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

        #region Menus -> Toolbars
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
        #endregion

        #region Menus -> General
        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu text color")]
        public Color Menu_TextColor = DefaultForeground;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu selected text color")]
        public Color Menu_SelectedTextColor = TitleFG;

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
        #endregion

        #region Menus -> Menu Bars
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
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient start")]
        public Color Menu_MenuItemSelectedGradientBegin = Accent2;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient end")]
        public Color Menu_MenuItemSelectedGradientEnd = Accent2;


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
        #endregion

        #region Menus -> Status bars
        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient start")]
        public Color Menu_StatusStripGradientBegin = TitleBG;

        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient end")]
        public Color Menu_StatusStripGradientEnd = TitleBG;
        #endregion

        #region Menus -> Menu holders
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
        #endregion

        #region Windows -> Title Buttons -> Idle -> Images
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
        #endregion

        #region Windows -> Title Buttons -> Mouse Over -> Images
        //Images
        [Image("closebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Over Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Close Button when the mouse hovers over it using this setting.")]
        public byte[] CloseButtonOverImage = null;

        [Image("minimizebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Over Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Minimize Button when the mouse hovers over it using this setting.")]
        public byte[] MinimizeButtonOverImage = null;

        [Image("maximizebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Over Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Maximize Button when the mouse hovers over it using this setting.")]
        public byte[] MaximizeButtonOverImage = null;
        #endregion

        #region Windows -> Title Buttons -> Mouse Down -> Images
        //Images
        [Image("closebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Down Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Close Button when the mouse clicks it using this setting.")]
        public byte[] CloseButtonDownImage = null;

        [Image("minimizebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Down Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Minimize Button when the mouse clicks it using this setting.")]
        public byte[] MinimizeButtonDownImage = null;

        [Image("maximizebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Down Image")]
        [RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Maximize Button when the mouse clicks it using this setting.")]
        public byte[] MaximizeButtonDownImage = null;
        #endregion


        #region Desktop -> Images
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
        public Size AppLauncherHolderSize = new Size(100, 24);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher image")]
        [ShifterDescription("The image that will appear on the app launcher.")]
        [Image("applauncher")]
        [RequiresUpgrade("skinning;shift_app_launcher")]
        public byte[] AppLauncherImage = null;
        #endregion


        #region System -> General
        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal font")]
        public Font TerminalFont = new Font("Lucida Console", 9F, FontStyle.Regular);

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal text color")]
        public ConsoleColor TerminalForeColorCC = ConsoleColor.White;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Terminal background color")]
        public ConsoleColor TerminalBackColorCC = ConsoleColor.Black;
        #endregion

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
        [ShifterName("Titlebar inactive background image")]
        [Image("titlebarinactive")]
        [RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        public byte[] TitleBarInactiveBackground = null;


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
        [RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left inactive background color")]
        [ShifterDescription("What color should be used for the left title corner when the window is inactive?")]
        public Color TitleInactiveLeftCornerBackground = DefaultBackground;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right background color")]
        [ShifterDescription("What color should be used for the right title corner?")]
        public Color TitleRightCornerBackground = TitleBG;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right inactive background color")]
        [ShifterDescription("What color should be used for the right title corner when the window is inactive?")]
        public Color TitleInactiveRightCornerBackground = DefaultBackground;


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
        [RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner inactive background image")]
        [ShifterDescription("Select an image to appear as the background texture for the left titlebar corner when the window is inactive.")]
        [Image("titleleftinactive")]
        public byte[] TitleLeftInactiveBG = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("skinning;shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner background image")]
        [ShifterDescription("Select an image to appear as the background texture for the right titlebar corner.")]
        [Image("titleright")]
        public byte[] TitleRightBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner inactive background image")]
        [ShifterDescription("Select an image to appear as the background texture for the right titlebar corner when the window is inactive.")]
        [Image("titlerightinactive")]
        public byte[] TitleRightInactiveBG = null;


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
        [RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomborderinactive")]
        [ShifterName("Bottom Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom border when the window is inactive. ")]
        public byte[] BottomBorderInactiveBG = null;


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
        [RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomrborderinactive")]
        [ShifterName("Bottom Right Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom right border when the window is inactive.")]
        public byte[] BottomRBorderInactiveBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomlborderinactive")]
        [ShifterName("Bottom Left Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom left border when the window is inactive.")]
        public byte[] BottomLBorderInactiveBG = null;



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
        [RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("leftborderinactive")]
        [ShifterName("Left Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the left border when the window is inactive.")]
        public byte[] LeftBorderInactiveBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("rightborderinactive")]
        [ShifterName("Right Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the right border when the window is inactive.")]
        public byte[] RightBorderInactiveBG = null;



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
        public Point TitlebarIconFromSide = new Point(4, 4);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Font")]
        [ShifterDescription("The font used by the status panel in the Advanced App Launcher.")]
        public Font ALStatusPanelFont = SysFont;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Text Color")]
        [ShifterDescription("The text color for the AL status panel.")]
        public Color ALStatusPanelTextColor = Skin.DefaultForeground;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Background")]
        [ShifterDescription("The status panel's background color.")]
        public Color ALStatusPanelBackColor = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Text Alignment")]
        [ShifterDescription("What part of the panel should the status text stick to?")]
        public ContentAlignment ALStatusPanelAlignment = ContentAlignment.MiddleCenter;


        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL System Status Height")]
        [ShifterDescription("Set the height of the top system status bar in the App Launcher.")]
        public int ALSystemStatusHeight = 50;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Size")]
        [ShifterDescription("Set the size of the App Launcher's container")]
        public Size AALSize = new Size(425, 500);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Category View Width")]
        [ShifterDescription("Set the width of the left Category Listing on the app launcher.")]
        public int AALCategoryViewWidth = 237;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Item List Width")]
        [ShifterDescription("Set the width of the item list in the app launcher.")]
        public int AALItemViewWidth = 237;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL System Actions Height")]
        [ShifterDescription("Set the height of the bottom system actions bar in the App Launcher.")]
        public int ALSystemActionHeight = 30;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("skinning;shift_advanced_app_launcher")]
        [ShifterName("Status Panel Background Image")]
        [ShifterDescription("Use an image for the App Launcher Status Panel")]
        [Image("al_bg_status")]
        public byte[] ALStatusPanelBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterEnumMask(new[] { "Button, bottom panel", "Button, top panel", "Category Item" })]
        [ShifterName("Shutdown Button position")]
        [ShifterDescription("Change the position and layout of the App Launcher Shutdown button.")]
        public int ShutdownButtonStyle = 0;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Button from side")]
        [ShifterDescription("The location relative to the side of the system panel that the shutdown button should reside from.")]
        public Point ShutdownButtonFromSide = new Point(2, 2);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Align shutdown button to left?")]
        [ShifterDescription("Should the location of the shutdown button be calculated relative to the left of the system panel?")]
        public bool ShutdownOnLeft = false;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Button Font")]
        [ShifterDescription("The font of the Shutdown Button")]
        public Font ShutdownFont = SysFont;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Text Color")]
        [ShifterDescription("The foreground color of the Shutdown button")]
        public Color ShutdownForeColor = DefaultForeground;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("System Panel background color")]
        [ShifterDescription("The background color of the App Launcher System Panel.")]
        public Color SystemPanelBackground = TitleBG;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("skinning;shift_advanced_app_launcher")]
        [ShifterName("System Panel Background Image")]
        [ShifterDescription("The background image of the System Panel.")]
        [Image("al_bg_system")]
        public byte[] SystemPanelBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("App Launcher Item Font")]
        [ShifterDescription("Select the font to use for the items in the App Launcher.")]
        public Font AdvALItemFont = SysFont2;
    }

    /// <summary>
    /// Marks a skin spec field as hidden from the Shifter.
    /// </summary>
    public class ShifterHiddenAttribute : Attribute
    {

    }

    public class ShifterFlagAttribute : Attribute
    {
        public ShifterFlagAttribute(string flag, bool expected)
        {
            Expected = expected;
            Flag = flag;
        }

        public bool Expected { get; set; }
        public string Flag { get; set; }
        public bool IsTrue(Skin skn)
        {
            foreach (var f in skn.GetType().GetFields())
            {
                if (f.Name == Flag)
                {
                    if (f.FieldType == typeof(bool))
                    {
                        return (bool)f.GetValue(skn) == Expected;
                    }
                }
            }
            throw new ArgumentException("The flag attribute was given an incorrect flag variable name.");
        }
    }

    public class ImageAttribute : Attribute
    {
        /// <summary>
        /// Attribute a byte array within the Skin object with this attribute and the engine and Shifter will see it as an image and you'll be able to grab the image by calling SkinEngine.GetImage() passing the name you input here.
        /// </summary>
        /// <param name="name">The name you want to reference this image as in the code.</param>
        public ImageAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }


    public class ShifterEnumMaskAttribute : Attribute
    {
        public ShifterEnumMaskAttribute(string[] items)
        {
            Items = items;
        }

        public string[] Items { get; set; }
    }



    public class ShifterNameAttribute : Attribute
    {
        public ShifterNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class ShifterDescriptionAttribute : Attribute
    {
        public ShifterDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    public class ShifterCategoryAttribute : Attribute
    {

        public ShifterCategoryAttribute(string category)
        {
            Category = category;
        }

        public string Category { get; set; }
    }

    public interface ISkinPostProcessor
    {
        /// <summary>
        /// Perform algorithmic operations at the bit level on a ShiftOS skin image.
        /// </summary>
        /// <param name="original">The image, as loaded by the engine, as a 32-bit ARGB byte array.</param>
        /// <returns>The processed image.</returns>
        byte[] ProcessImage(byte[] original);
    }

    public class ShifterMetaAttribute : Attribute
    {

        public ShifterMetaAttribute(string meta)
        {
            Meta = meta;
        }

        public string Meta { get; set; }
    }
}