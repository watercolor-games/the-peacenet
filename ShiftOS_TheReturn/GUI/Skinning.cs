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
using Plex.Objects;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Engine
{
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
            if (ServerManager.SessionInfo != null)
            {
                if (!FSUtils.FileExists(Paths.GetPath("themedata.plex")))
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
            FSUtils.WriteAllText(Paths.GetPath("themedata.plex"), JsonConvert.SerializeObject(LoadedSkin, Formatting.Indented));
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
                foreach (var type in ReflectMan.Types.Where(t => t.Name == id))
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

    public enum ArrowDirection
    {
        Top,
        Left,
        Bottom,
        Right
    }

    /// <summary>
    /// The data stored in any .skn file.
    /// </summary>
    public abstract class Skin
    {
        #region Draw calls
        public abstract void DrawArrow(ArrowDirection dir, int x, int y, int w, int h, GraphicsContext gfx, Color c);

        #endregion

        //Todo: When making Shifter GUI we need to label all these with proper Shifter attributes to get 'em displaying in the right places.

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Accent key")]
        [ShifterDescription("This is a color the system will replace with the user-defined Accent Color when rendering skin assets to the screen. Other color settings in PlexTheme that are set to this color will also be rendered as the user-defined accent color.")]
        public Color AccentKey = Color.FromArgb(1, 0, 1);


        //we DO NOT want this showing in the shifter.
        [ShifterHidden]
        public Dictionary<string, ImageLayout> SkinImageLayouts = new Dictionary<string, ImageLayout>();

        [ShifterHidden]
        public CommandParser CurrentParser = CommandParser.GenerateSample();

        [ShifterHidden]
        public Dictionary<string, string> AppNames = new Dictionary<string, string>();

        [ShifterHidden]
        public Dictionary<string, byte[]> AppIcons = new Dictionary<string, byte[]>();

        #region System -> Terminal
        [ShifterMeta("System")]
        [ShifterCategory("Terminal")]
        [ShifterName("Terminal font")]
        [ShifterDescription("The font used by the Terminal for text display.")]
        public Font TerminalFont = new Font("Lucida Console", 9F, FontStyle.Regular);

        [ShifterMeta("System")]
        [ShifterCategory("Terminal")]
        [ShifterName("Terminal text color")]
        [ShifterDescription("The color used by the Terminal for text and the caret.")]
        public Color TerminalForeColor = Color.White;

        [ShifterMeta("System")]
        [ShifterCategory("Terminal")]
        [ShifterName("Terminal background color")]
        [ShifterDescription("The color used by the Terminal for the background.")]
        public Color TerminalBackColor = Color.Black;


        #endregion

        #region UI typography
        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("1st level header")]
        [ShifterDescription("The font used in level 1 (title) headers.")]
        public Font HeaderFont = new Font("Microsoft Sans Serif", 20F);

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("Inset/outset background color")]
        [ShifterDescription("A color used by elements such as list boxes, list views, and other content elements that makes them stand out from the rest of the UI, giving the UI some more depth.")]
        public Color InsetBackgroundColor = Color.LightGray;

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("1st level header color")]
        [ShifterDescription("The color used for first-level headers.")]
        public Color FirstLevelHeaderColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("2nd level header")]
        [ShifterDescription("The font used in level 2 (subtitle) headers.")]
        public Font Header2Font = new Font("Microsoft Sans Serif", 15F);

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("2nd level header color")]
        [ShifterDescription("The color used for second-level headers.")]
        public Color SecondLevelHeaderColor = Color.Black;


        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("3rd level header")]
        [ShifterDescription("The font used in level 3 (section) headers.")]
        public Font Header3Font = new Font("Microsoft Sans Serif", 12F);

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("3rd level header color")]
        [ShifterDescription("The color used for third-level headers.")]
        public Color ThirdLevelHeaderColor = Color.Black;


        [ShifterMeta("Desktop")]
        [ShifterCategory("General")]
        [ShifterName("Desktop Background Color")]
        [ShifterDescription("The background color of the desktop.")]
        public Color DesktopColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System Background")]
        [ShifterDescription("The background color of any UI elements that don't have their own background style.")]
        public Color ControlColor = Color.White;

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("General text color")]
        [ShifterDescription("The foreground color of any text element that doesn't define its own typographic style.")]
        public Color ControlTextColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("UI typography")]
        [ShifterName("System Font")]
        [ShifterDescription("The font style used by the system.")]
        public Font MainFont = new Font("Microsoft Sans Serif", 9F);
        #endregion


        #region Buttons
        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button color (idle)")]
        [ShifterDescription("The background color of buttons in their 'idle' state.")]
        public Color ButtonIdleColor = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button text color (idle)")]
        [ShifterDescription("The text color of buttons in their 'idle' state")]
        public Color ButtonIdleTextColor = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button color (hover)")]
        [ShifterDescription("The color of buttons when the mouse hovers over them")]
        public Color ButtonHoverColor = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterDescription("The text color of buttons when you hover over them")]
        [ShifterName("Button text color (hover)")]
        public Color ButtonHoverTextColor = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button color (pressed)")]
        [ShifterDescription("The color of buttons when they're pressed down.")]
        public Color ButtonPressedColor = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button text color (pressed)")]
        [ShifterDescription("The text color of buttons when they are pressed down.")]
        public Color ButtonPressedTextColor = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button font")]
        [ShifterDescription("The font for the text on buttons.")]
        public Font ButtonFont = new Font("Microsoft Sans Serif", 9F);

        [ShifterMeta("UI elements")]
        [ShifterCategory("Buttons")]
        [ShifterName("Button margins")]
        [ShifterDescription("These values specify the amount of space in pixels between the edges of a button and its text. The first value is the horizontal margin and the second is the vertical margin. Play around with these values and see how they affect the button's look!")]
        public Size ButtonMargins = new Size(10, 5);

        #endregion

        #region Progress bars

        [ShifterMeta("UI elements")]
        [ShifterCategory("Progress bars")]
        [ShifterName("Progress color")]
        [ShifterDescription("The foreground color of progress bars. This color is used to indicate progress.")]
        public Color ProgressColor = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Progress bars")]
        [ShifterName("Progress bar background color")]
        [ShifterDescription("The background color of progress bars.")]
        public Color ProgressBarBackgroundColor = Color.Black;

        #endregion

        #region Menu dropdowns

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown background")]
        [ShifterDescription("The background color of the itemlist area of dropdown menus.")]
        public Color DropdownBackground = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown - Item selected color")]
        [ShifterDescription("The background color used when a dropdown menu item is selected.")]
        public Color DropdownItemSelected = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown item text color")]
        [ShifterDescription("The text color of unselected dropdown menu items.")]
        public Color DropdownItemTextColor = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown item text color (selected)")]
        [ShifterDescription("The text color of selected dropdown menu items.")]
        public Color DropdownItemSelectedTextColor = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown font")]
        [ShifterDescription("The font style used for menu items in a dropdown.")]
        public Font DropdownFont = new Font("Microsoft Sans Serif", 9F);

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown margin color")]
        [ShifterDescription("The background color of the Image Margin area of dropdowns.")]
        public Color DropdownMarginColor = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Dropdowns")]
        [ShifterName("Dropdown item text color (disabled)")]
        [ShifterDescription("The text color of disabled dropdown menu items.")]
        public Color DropdownItemDisabledText = Color.Gray;

        #endregion

        #region Menu bars

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item text color")]
        [ShifterDescription("The text color used for menu items.")]
        public Color MenuItemTextColor = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item color (selected)")]
        [ShifterDescription("The background color of selected menu items.")]
        public Color MenuItemSelectedColor = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item text color (selected)")]
        [ShifterDescription("The text color of selected menu items.")]
        public Color MenuItemSelectedTextColor = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu bar background")]
        [ShifterDescription("The background used for menu bars.")]
        public Color MenuBarBackgroundColor = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu font")]
        [ShifterDescription("The font style used on the text of menu items.")]
        public Font MenuFont = new Font("Microsoft Sans Serif", 9F);

        #endregion

        #region Listbox

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item color (selected)")]
        [ShifterDescription("The background color of selected list items.")]
        public Color ListBoxSelectedItemColor = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item color (hover)")]
        [ShifterDescription("The background color of list items when the mouse is hovered over them - Note, this only affects Listboxes and not Listviews.")]
        public Color ListBoxHoverItemColor = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item text color")]
        [ShifterDescription("The text color of list items")]
        public Color ListBoxTextColor = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item text color (selected)")]
        [ShifterDescription("The text color of selected list items")]
        public Color ListBoxSelectedItemTextColor = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item text color (hover)")]
        [ShifterDescription("The text color of list items when the mouse is hovered over them - Note, this only affects Listboxes and not Listviews.")]
        public Color ListBoxHoverTextColor = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Listboxes and Listviews")]
        [ShifterName("List item font")]
        [ShifterDescription("The font style used for text in list items.")]
        public Font ListBoxFont = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);

        #endregion

        #region Textbox

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box text color")]
        [ShifterDescription("The color used for rendering text within a text box.")]
        public Color TextBoxTextColor = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box font")]
        [ShifterDescription("The font used by text in a text box.")]
        public Font TextBoxFont = new Font("Microsoft Sans-Serif", 9F);

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box border width")]
        [ShifterDescription("The width of the border in a text box.")]
        public int TextBoxBorderWidth = 2;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box border style")]
        [ShifterDescription("The style of the text box border.\r\n\r\nAvailable options:\r\n - MaterialDesign: The default look of the text box border.\r\n - Classic: The look of text boxes in beta versions of the Peacegate.")]
        public TextBoxBorderStyle TextBoxBorderStyle = TextBoxBorderStyle.MaterialDesign;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterDescription("The border color used for text boxes when they are not in focus or hovered over by the mouse.")]
        [ShifterName("Text box border color (Idle)")]
        public Color TextBoxBorderColorIdle = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box border color (Hover)")]
        [ShifterDescription("The color of a text box's border when the mouse is hovering over the box.")]
        public Color TextBoxBorderColorHover = Color.LightGray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box border color (Focused)")]
        [ShifterDescription("The color of a text box's border when it is in focus.")]
        public Color TextBoxBorderColorFocused = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Text Boxes")]
        [ShifterName("Text box background color")]
        [ShifterDescription("The background color of all text boxes.")]
        public Color TextBoxBackColor = Color.White;

        #endregion

        #region Item group

        public int ItemGroupInitialGap = 10;
        public int ItemGroupGap = 5;

        #endregion

        #region Scroll view

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scroll view scrollbar width")]
        [ShifterDescription("The width of the scroll bar situated at the right of a scroll view.")]
        public int ScrollViewScrollbarWidth = 20;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar nub width")]
        [ShifterDescription("The width of the inner portion, otherwise known as the nub, of the scrollbar which indicates where the scroll view is looking.")]
        public int ScrollViewScrollNubWidth = 16;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scroll view scrollbar background")]
        [ShifterDescription("The background color of the scroll view's scrollbar.")]
        public Color ScrollViewScrollbarBackground = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar nub color (Idle)")]
        [ShifterDescription("The color of the scroll nub while it is idle.")]
        public Color ScrollViewScrollNubColorIdle = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar nub color (Hover)")]
        [ShifterDescription("The color of the nub while the mouse is hovering over it.")]
        public Color ScrollViewScrollNubColorHover = Color.LightGray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar nub color (Pressed)")]
        [ShifterDescription("The color of the nub when it is pressed down.")]
        public Color ScrollViewScrollNubColorPressed = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow background (Idle)")]
        [ShifterDescription("The background color of the scrollbar's arrows when idle.")]
        public Color ScrollViewArrowBackgroundIdle = Color.Gray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow background (Hover)")]
        [ShifterDescription("The background color of the scrollbar's arrows when hovered over by the mouse.")]
        public Color ScrollViewArrowBackgroundHover = Color.LightGray;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow background (Pressed)")]
        [ShifterDescription("The background color of the scrollbar's arrows when pressed.")]
        public Color ScrollViewArrowBackgroundPressed = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow color (Idle)")]
        [ShifterDescription("The foreground color of the scrollbar's arrows when idle.")]
        public Color ScrollViewArrowColorIdle = Color.White;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow color (Hover)")]
        [ShifterDescription("The foreground color of the scrollbar's arrows when hovered over by the mouse.")]
        public Color ScrollViewArrowColorHover = Color.Black;

        [ShifterMeta("UI elements")]
        [ShifterCategory("Scroll views")]
        [ShifterName("Scrollbar arrow color (Pressed)")]
        [ShifterDescription("The foreground color of the scrollbar's arrows when pressed down.")]
        public Color ScrollViewArrowColorPressed = Color.White;


        #endregion
    }

    public enum ItemGroupLayout
    {
        SkinDefined,
        Custom
    }

    public enum TextBoxBorderStyle
    {
        Classic, //like ShiftOS
        MaterialDesign //android-style text borders
    }

    public enum ButtonRenderStyle
    {
        Colored,
        Textured
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
