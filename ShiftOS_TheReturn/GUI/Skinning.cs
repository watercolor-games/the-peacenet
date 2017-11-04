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
    //this'll be used even after skinning is redone. I may move it however. -Alkaline
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
    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
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

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public enum TextBoxBorderStyle
    {
        Classic, //like ShiftOS
        MaterialDesign //android-style text borders
    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public enum ButtonRenderStyle
    {
        Colored,
        Textured
    }

    /// <summary>
    /// Marks a skin spec field as hidden from the Shifter.
    /// </summary>
    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterHiddenAttribute : Attribute
    {

    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
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

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
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


    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterEnumMaskAttribute : Attribute
    {
        public ShifterEnumMaskAttribute(string[] items)
        {
            Items = items;
        }

        public string[] Items { get; set; }
    }



    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterNameAttribute : Attribute
    {
        public ShifterNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterDescriptionAttribute : Attribute
    {
        public ShifterDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterCategoryAttribute : Attribute
    {

        public ShifterCategoryAttribute(string category)
        {
            Category = category;
        }

        public string Category { get; set; }
    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public interface ISkinPostProcessor
    {
        /// <summary>
        /// Perform algorithmic operations at the bit level on a Plex skin image.
        /// </summary>
        /// <param name="original">The image, as loaded by the engine, as a 32-bit ARGB byte array.</param>
        /// <returns>The processed image.</returns>
        byte[] ProcessImage(byte[] original);
    }

    [Obsolete("ShiftOS-style skinning will soon not be directly supported by the engine.")]
    public class ShifterMetaAttribute : Attribute
    {

        public ShifterMetaAttribute(string meta)
        {
            Meta = meta;
        }

        public string Meta { get; set; }
    }
}
