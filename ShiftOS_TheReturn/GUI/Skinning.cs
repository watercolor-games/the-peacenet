using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using static Plex.Engine.SaveSystem;
using Plex.Objects.ShiftFS;
using System.Reflection;
using Plex.Engine.Scripting;
namespace Plex.Engine
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
            Utils.WriteAllText(Paths.GetPath("themedata.plex"), JsonConvert.SerializeObject(skn));
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

    public interface ISkinProvider
    {
        Skin GetDefaultSkin();
        Skin ReadSkin(string pfsPath);
        Skin GetEasterEggSkin();
    }

    /// <summary>
    /// Skin engine management class.
    /// </summary>
    public static class SkinEngine
    {
        private static ISkinProvider SkinProvider = null;
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

        public static byte[] ImageToBinary(Image img)
        {
            using(var memstr = new MemoryStream())
            {
                img.Save(memstr, System.Drawing.Imaging.ImageFormat.Png);
                return memstr.ToArray();
            }
        }

        /// <summary>
        /// Retrieves an image from the skin after postprocessing it.
        /// </summary>
        /// <param name="img">The image ID to search.</param>
        /// <returns>The post-processed image, or null if none was found.</returns>
        public static System.Drawing.Image GetImage(string img)
        {
            var type = LoadedSkin.GetType();

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

        internal static void LoadEngineDefault()
        {
            loadedSkin = SkinProvider.GetEasterEggSkin();
            SkinLoaded?.Invoke();
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

        private static Skin loadedSkin = null;

        /// <summary>
        /// Gets the currently loaded skin.
        /// </summary>
        public static Skin LoadedSkin
        {
            get
            {
                if (loadedSkin == null)
                    Init();
                return loadedSkin;
            }
            private set
            {
                loadedSkin = value;
            }
        }

        public static void SetSkinProvider(ISkinProvider provider)
        {
            SkinProvider = provider;
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
            if (Utils.Mounts.Count > 0)
            {
                if (!Utils.FileExists(Paths.GetPath("themedata.plex")))
                {
                    LoadedSkin = SkinProvider.GetDefaultSkin();
                    SaveSkin();
                }
                else
                {
                    LoadSkin();
                }
            }
            else
            {
                LoadedSkin = SkinProvider.GetDefaultSkin();
            }
            SkinLoaded?.Invoke();

        }

        public static void LoadDefaultSkin()
        {
            loadedSkin = SkinProvider.GetDefaultSkin();
            SaveSkin();
            SkinLoaded?.Invoke();
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
            LoadedSkin = SkinProvider.ReadSkin(Paths.GetPath("themedata.plex"));
            SkinLoaded?.Invoke();
            Desktop.ResetPanelButtons();
            Desktop.PopulateAppLauncher();
        }

        /// <summary>
        /// Save the skin loaded in memory to the filesystem.
        /// </summary>
        public static void SaveSkin()
        {
            Utils.WriteAllText(Paths.GetPath("themedata.plex"), JsonConvert.SerializeObject(LoadedSkin, Formatting.Indented));
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

        public static Skin GetDefaultSkin()
        {
            return SkinProvider.GetDefaultSkin();
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
    /// Sets the default icon ID for a <see cref="IPlexWindow"/>. 
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
    public abstract class Skin
    {
        //Todo: When making Shifter GUI we need to label all these with proper Shifter attributes to get 'em displaying in the right places.

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        //we DO NOT want this showing in the shifter.
        [ShifterHidden]
        public Dictionary<string, ImageLayout> SkinImageLayouts = new Dictionary<string, ImageLayout>();

        [ShifterHidden]
        public CommandParser CurrentParser = CommandParser.GenerateSample();

        [ShifterHidden]
        public Dictionary<string, string> AppNames = new Dictionary<string, string>();

        [ShifterHidden]
        public Dictionary<string, byte[]> AppIcons = new Dictionary<string, byte[]>();


        #region Menus -> Toolbars
        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar Border")]
        public Color Menu_ToolStripBorder = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Dropdown background")]
        public Color Menu_ToolStripDropDownBackground = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient start")]
        public Color Menu_ToolStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient middle")]
        public Color Menu_ToolStripGradientMiddle = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient end")]
        public Color Menu_ToolStripGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select highlight")]
        public Color Menu_ButtonSelectedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select border")]
        public Color Menu_ButtonSelectedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed highlight")]
        public Color Menu_ButtonPressedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed border")]
        public Color Menu_ButtonPressedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked highlight")]
        public Color Menu_ButtonCheckedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked border")]
        public Color Menu_ButtonCheckedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient border")]
        public Color Menu_ButtonPressedBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient border")]
        public Color Menu_ButtonSelectedBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient start")]
        public Color Menu_ButtonCheckedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient middle")]
        public Color Menu_ButtonCheckedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient end")]
        public Color Menu_ButtonCheckedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient start")]
        public Color Menu_ButtonSelectedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient middle")]
        public Color Menu_ButtonSelectedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient end")]
        public Color Menu_ButtonSelectedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient start")]
        public Color Menu_ButtonPressedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient middle")]
        public Color Menu_ButtonPressedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient end")]
        public Color Menu_ButtonPressedGradientEnd = Color.Black;




        #endregion

        #region Menus -> General
        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu text color")]
        public Color Menu_TextColor = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu selected text color")]
        public Color Menu_SelectedTextColor = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient start")]
        public Color Menu_RaftingContainerGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient end")]
        public Color Menu_RaftingContainerGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 1")]
        public Color Menu_SeparatorDark = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 2")]
        public Color Menu_SeparatorLight = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG")]
        public Color Menu_CheckBackground = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Selected)")]
        public Color Menu_CheckSelectedBackground = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Pressed)")]
        public Color Menu_CheckPressedBackground = Color.Black;




        #endregion

        #region Menus -> Menu Bars
        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient start")]
        public Color Menu_MenuItemPressedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient middle")]
        public Color Menu_MenuItemPressedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient end")]
        public Color Menu_MenuItemPressedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient start")]
        public Color Menu_MenuItemSelectedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient end")]
        public Color Menu_MenuItemSelectedGradientEnd = Color.Black;


        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient start")]
        public Color Menu_ImageMarginGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient middle")]
        public Color Menu_ImageMarginGradientMiddle = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient end")]
        public Color Menu_ImageMarginGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient start")]
        public Color Menu_MenuStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient end")]
        public Color Menu_MenuStripGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item selected")]
        public Color Menu_MenuItemSelected = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item border")]
        public Color Menu_MenuItemBorder = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu border")]
        public Color Menu_MenuBorder = Color.White;




        #endregion

        #region Menus -> Status bars
        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient start")]
        public Color Menu_StatusStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient end")]
        public Color Menu_StatusStripGradientEnd = Color.Gray;




        #endregion

        #region Menus -> Menu holders
        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient start")]
        public Color Menu_ToolStripContentPanelGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient end")]
        public Color Menu_ToolStripContentPanelGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient start")]
        public Color Menu_ToolStripPanelGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient end")]
        public Color Menu_ToolStripPanelGradientEnd = Color.Gray;




        #endregion


        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button background color")]
        [ShifterDescription("Set the background color for each button's Idle state.")]
        public Color ButtonBackgroundColor = Color.White;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons;skinning")]
        [Image("buttonhover")]
        [ShifterName("Button hover image")]
        [ShifterDescription("Set the image that's displayed when the mouse hovers over a button.")]
        public byte[] ButtonHoverImage = null;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("skinning;shift_buttons")]
        [Image("buttonpressed")]
        [ShifterName("Button pressed image")]
        [ShifterDescription("Select an image to show when the user presses a button.")]
        public byte[] ButtonPressedImage = null;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button hover color")]
        [ShifterDescription("Choose the color that displays on a button when the mouse hovers over it.")]
        public Color ButtonHoverColor = Color.Gray;

        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [ShifterName("Progress bar foreground color")]
        [ShifterDescription("Set the color of the progress indicator.")]
        public Color ProgressColor = Color.Gray;


        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button pressed color")]
        [ShifterDescription("Select the background color for the button when the mouse clicks it.")]
        public Color ButtonPressedColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button foreground color")]
        [ShifterDescription("Select the text and border color for each button.")]
        public Color ButtonForegroundColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button font")]
        [ShifterDescription("Select the font for the button's text.")]
        public Font ButtonTextFont = new Font("Microsoft Sans Serif", 9F);

        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons;skinning")]
        [Image("buttonidle")]
        [ShifterName("Button background color")]
        [ShifterDescription("Select an image to show as the button's Idle state.")]
        public byte[] ButtonBG = null;



        [ShifterMeta("System")]
        [ShifterCategory("Buttons")]
        //[RequiresUpgrade("shift_buttons")]
        [ShifterName("Button border width")]
        [ShifterDescription("Set the width, in pixels, of the button's border.")]
        public int ButtonBorderWidth = 2;


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


        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("1st level header")]
        [ShifterDescription("The font used in level 1 (title) headers.")]
        public Font HeaderFont = new Font("Microsoft Sans Serif", 20F);


        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("2nd level header")]
        [ShifterDescription("The font used in level 2 (subtitle) headers.")]
        public Font Header2Font = new Font("Microsoft Sans Serif", 15F);


        [ShifterMeta("System")]
        [ShifterCategory("Header Fonts")]
        [ShifterName("3rd level header")]
        [ShifterDescription("The font used in level 3 (section) headers.")]
        public Font Header3Font = new Font("Microsoft Sans Serif", 12F);


        [ShifterMeta("Desktop")]
        [ShifterCategory("General")]
        [ShifterName("Desktop Background Color")]
        [ShifterDescription("The background color of the desktop.")]
        public Color DesktopColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        //[RequiresUpgrade("shift_progress_bar")]
        [ShifterName("Progress bar background color")]
        [ShifterDescription("The background color of the progress bar.")]
        public Color ProgressBarBackgroundColor = Color.Black;


        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Background")]
        [ShifterDescription("The background color of all system controls in the UI.")]
        public Color ControlColor = Color.White;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Foreground")]
        [ShifterDescription("The foreground color of all system controls in the UI.")]
        public Color ControlTextColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Font")]
        [ShifterDescription("The font style used by the system.")]
        public Font MainFont = new Font("Microsoft Sans Serif", 9F);

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
        /// Perform algorithmic operations at the bit level on a Plex skin image.
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
