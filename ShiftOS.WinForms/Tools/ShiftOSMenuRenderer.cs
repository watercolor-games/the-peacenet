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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ShiftOS.Engine;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Tools
{
    public class ShiftOSMenuRenderer : ToolStripProfessionalRenderer
    {
        public ShiftOSMenuRenderer() : base(new ShiftOSColorTable(ShiftOS.Engine.SkinEngine.LoadedSkin))
        {
            
        }

        public ShiftOSMenuRenderer(ProfessionalColorTable table) : base(table)
        {

        }

        public ShiftOSMenuRenderer(Skin skn) : base(new ShiftOSColorTable(skn))
        {

        }

        public Skin LoadedSkin
        {
            get
            {
                if(ColorTable is ShiftOSColorTable)
                {
                    return (ColorTable as ShiftOSColorTable).LoadedSkin;
                }
                else
                {
                    return SkinEngine.LoadedSkin;
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.Tag?.ToString() == "applauncherbutton")
            {
                e.TextFont = LoadedSkin.AppLauncherFont;
                if(e.Item.Selected == true)
                {
                    e.TextColor = LoadedSkin.AppLauncherSelectedTextColor;
                }
                else
                {
                    e.TextColor = LoadedSkin.AppLauncherTextColor;
                }
            }
            else
            {
                e.TextFont = LoadedSkin.MainFont;
                if (e.Item.Selected == true)
                {
                    e.TextColor = LoadedSkin.Menu_SelectedTextColor;
                }
                else
                {
                    e.TextColor = LoadedSkin.Menu_TextColor;
                }
            }
            e.TextRectangle = GenRect(e.Text, e.TextFont, e.Item.Size, e.Graphics, e.TextRectangle);
            base.OnRenderItemText(e);
        }

        private Rectangle GenRect(string t, Font f, Size s, Graphics g, Rectangle rekt)
        {

            Rectangle rect = new Rectangle();
            var fSize = g.MeasureString(t, f);
            int width = rekt.Left + (int)fSize.Width;
            int height = rekt.Top + (int)fSize.Height;

            int rwidth = rekt.Left + rekt.Width;
            int rheight = rekt.Top + rekt.Height;

            int wDiff = (width - rwidth);
            int hDiff = (height - rheight);

            rect = new Rectangle(rekt.Left, rekt.Top, rekt.Width + wDiff, rekt.Height + hDiff);



            return rect;
        }
    }

    public class ShiftOSColorTable : ProfessionalColorTable
    {
        public ShiftOSColorTable(ShiftOS.Engine.Skin skn)
        {
            LoadedSkin = skn;
        }

        public Skin LoadedSkin { get; private set; }

        public Image GetImage(string id)
        {
            return SkinEngine.GetImage(id);
        }

        public override Color ButtonSelectedHighlight
        {
            get { return LoadedSkin.Menu_ButtonSelectedHighlight; }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonSelectedHighlight; }
        }
        public override Color ButtonPressedHighlight
        {
            get { return LoadedSkin.Menu_ButtonPressedHighlight; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonPressedHighlight; }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return LoadedSkin.Menu_ButtonCheckedHighlight; }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonCheckedHighlightBorder; }
        }
        public override Color ButtonPressedBorder
        {
            get { return LoadedSkin.Menu_ButtonPressedBorder; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return LoadedSkin.Menu_ButtonSelectedBorder; }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientBegin; }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientMiddle; }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientEnd; }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientBegin; }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientMiddle; }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientEnd; }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientBegin; }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientMiddle; }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientEnd; }
        }
        public override Color CheckBackground
        {
            get { return LoadedSkin.Menu_CheckBackground; }
        }
        public override Color CheckSelectedBackground
        {
            get { return LoadedSkin.Menu_CheckSelectedBackground; }
        }
        public override Color CheckPressedBackground
        {
            get { return LoadedSkin.Menu_CheckPressedBackground; }
        }
        public override Color GripDark
        {
            get { return Color.Transparent; }
        }
        public override Color GripLight
        {
            get { return Color.Transparent; }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return LoadedSkin.Menu_ImageMarginGradientBegin; }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return LoadedSkin.Menu_ImageMarginGradientMiddle; }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return LoadedSkin.Menu_ImageMarginGradientEnd; }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get { return LoadedSkin.Menu_ImageMarginGradientBegin; }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get { return LoadedSkin.Menu_ImageMarginGradientMiddle; }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get { return LoadedSkin.Menu_ImageMarginGradientEnd; }
        }
        public override Color MenuStripGradientBegin
        {
            get { return LoadedSkin.Menu_MenuStripGradientBegin; }
        }
        public override Color MenuStripGradientEnd
        {
            get { return LoadedSkin.Menu_MenuStripGradientEnd; }
        }
        public override Color MenuItemSelected
        {
            get { return LoadedSkin.Menu_MenuItemSelected; }
        }
        public override Color MenuItemBorder
        {
            get { return LoadedSkin.Menu_MenuItemSelected; }
        }
        public override Color MenuBorder
        {
            get { return LoadedSkin.Menu_MenuBorder; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return LoadedSkin.Menu_MenuItemSelectedGradientBegin; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return LoadedSkin.Menu_MenuItemSelectedGradientEnd; }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return LoadedSkin.Menu_MenuItemPressedGradientBegin; }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return LoadedSkin.Menu_MenuItemPressedGradientMiddle; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return LoadedSkin.Menu_MenuItemPressedGradientEnd; }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return LoadedSkin.Menu_RaftingContainerGradientBegin; }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return LoadedSkin.Menu_RaftingContainerGradientEnd; }
        }
        public override Color SeparatorDark
        {
            get { return LoadedSkin.Menu_SeparatorDark; }
        }
        public override Color SeparatorLight
        {
            get { return LoadedSkin.Menu_SeparatorLight; }
        }
        public override Color StatusStripGradientBegin
        {
            get { return LoadedSkin.Menu_StatusStripGradientBegin; }
        }
        public override Color StatusStripGradientEnd
        {
            get { return LoadedSkin.Menu_StatusStripGradientEnd; }
        }
        public override Color ToolStripBorder
        {
            get { return LoadedSkin.Menu_ToolStripBorder; }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return LoadedSkin.Menu_ToolStripDropDownBackground; }
        }
        public override Color ToolStripGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripGradientBegin; }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return LoadedSkin.Menu_ToolStripGradientMiddle; }
        }
        public override Color ToolStripGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripGradientEnd; }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripContentPanelGradientBegin; }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripContentPanelGradientEnd; }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripPanelGradientBegin; }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripPanelGradientEnd; }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return Color.Transparent; }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return Color.Transparent; }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return Color.Transparent; }
        }
    }

    public class AppLauncherColorTable : ProfessionalColorTable
    {
        public Image GetImage(string id)
        {
            return SkinEngine.GetImage(id);
        }

        public AppLauncherColorTable()
        {
            LoadedSkin = SkinEngine.LoadedSkin;
        }

        public AppLauncherColorTable(Skin skn)
        {
            LoadedSkin = skn;
        }

        public Skin LoadedSkin { get; private set; }

        public override Color ButtonSelectedHighlight
        {
            get { return LoadedSkin.Menu_ButtonSelectedHighlight; }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonSelectedHighlight; }
        }
        public override Color ButtonPressedHighlight
        {
            get { return LoadedSkin.Menu_ButtonPressedHighlight; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonPressedHighlight; }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return LoadedSkin.Menu_ButtonCheckedHighlight; }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return LoadedSkin.Menu_ButtonCheckedHighlightBorder; }
        }
        public override Color ButtonPressedBorder
        {
            get { return LoadedSkin.Menu_ButtonPressedBorder; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return LoadedSkin.Menu_ButtonSelectedBorder; }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientBegin; }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientMiddle; }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonCheckedGradientEnd; }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientBegin; }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientMiddle; }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonSelectedGradientEnd; }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientBegin; }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientMiddle; }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return LoadedSkin.Menu_ButtonPressedGradientEnd; }
        }
        public override Color CheckBackground
        {
            get { return LoadedSkin.Menu_CheckBackground; }
        }
        public override Color CheckSelectedBackground
        {
            get { return LoadedSkin.Menu_CheckSelectedBackground; }
        }
        public override Color CheckPressedBackground
        {
            get { return LoadedSkin.Menu_CheckPressedBackground; }
        }
        public override Color GripDark
        {
            get { return Color.Transparent; }
        }
        public override Color GripLight
        {
            get { return Color.Transparent; }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return LoadedSkin.Menu_ImageMarginGradientBegin; }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return LoadedSkin.Menu_ImageMarginGradientMiddle; }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return LoadedSkin.Menu_ImageMarginGradientEnd; }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get { return LoadedSkin.Menu_ImageMarginGradientBegin; }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get { return LoadedSkin.Menu_ImageMarginGradientMiddle; }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get { return LoadedSkin.Menu_ImageMarginGradientEnd; }
        }
        public override Color MenuStripGradientBegin
        {
            get
            {
                if (LoadedSkin.AppLauncherImage != null)
                {
                    return Color.Transparent;
                }
                else
                {
                    return LoadedSkin.Menu_MenuStripGradientBegin;
                }
            }
        }
        public override Color MenuStripGradientEnd
        {
            get
            {
                if (LoadedSkin.AppLauncherImage != null)
                {
                    return Color.Transparent;
                }
                else
                {
                    return LoadedSkin.Menu_MenuStripGradientEnd;
                }
            }
        }
        public override Color MenuItemSelected
        {
            get { return LoadedSkin.Menu_MenuItemSelected; }
        }
        public override Color MenuItemBorder
        {
            get { return LoadedSkin.Menu_MenuItemSelected; }
        }
        public override Color MenuBorder
        {
            get { return LoadedSkin.Menu_MenuBorder; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return (GetImage("applauncher") != null) ? Color.Transparent : LoadedSkin.Menu_MenuItemSelectedGradientBegin; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return (GetImage("applauncher") != null) ? Color.Transparent : LoadedSkin.Menu_MenuItemSelectedGradientEnd; }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return (GetImage("applauncher") != null) ? Color.Transparent : LoadedSkin.Menu_MenuItemPressedGradientBegin; }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return (GetImage("applauncher") != null) ? Color.Transparent : LoadedSkin.Menu_MenuItemPressedGradientMiddle; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return (GetImage("applauncher") != null) ? Color.Transparent : LoadedSkin.Menu_MenuItemPressedGradientEnd; }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return LoadedSkin.Menu_RaftingContainerGradientBegin; }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return LoadedSkin.Menu_RaftingContainerGradientEnd; }
        }
        public override Color SeparatorDark
        {
            get { return LoadedSkin.Menu_SeparatorDark; }
        }
        public override Color SeparatorLight
        {
            get { return LoadedSkin.Menu_SeparatorLight; }
        }
        public override Color StatusStripGradientBegin
        {
            get { return LoadedSkin.Menu_StatusStripGradientBegin; }
        }
        public override Color StatusStripGradientEnd
        {
            get { return LoadedSkin.Menu_StatusStripGradientEnd; }
        }
        public override Color ToolStripBorder
        {
            get { return LoadedSkin.Menu_ToolStripBorder; }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return LoadedSkin.Menu_ToolStripDropDownBackground; }
        }
        public override Color ToolStripGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripGradientBegin; }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return LoadedSkin.Menu_ToolStripGradientMiddle; }
        }
        public override Color ToolStripGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripGradientEnd; }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripContentPanelGradientBegin; }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripContentPanelGradientEnd; }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get { return LoadedSkin.Menu_ToolStripPanelGradientBegin; }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get { return LoadedSkin.Menu_ToolStripPanelGradientEnd; }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return Color.Transparent; }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return Color.Transparent; }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return Color.Transparent; }
        }
    }
}
