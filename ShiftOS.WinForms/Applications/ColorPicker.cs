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
using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;
using API = ShiftOS.WinForms.Tools.ColorPickerDataBackend;

namespace ShiftOS.WinForms.Applications
{
    [DefaultTitle("{TITLE_COLORPICKER}")] [DefaultIcon("iconColourPicker")]
    public partial class ColorPicker : UserControl, IShiftOSWindow
    {
        public ColorPicker(Color oldcol, string ctc,  Action<Color> callback)
        {
            InitializeComponent();
            oldcolour = oldcol;
            colourtochange = ctc;
            Callback = callback;
        }

        private Action<Color> Callback = null;

        public Color NewColor = Color.Black;

        public void Color_Picker_Load(object sender, EventArgs e)
        {
            getoldcolour();
            determinelevels();
            shrinktosizebasedoncoloursbought();
            setupboughtcolours();
            loadmemory();

            foreach (Control ctrl in pnlanycolours.Controls)
            {
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionany);
                ctrl.Tag = "keepbg";
            }
            foreach (Control ctrl in pnlgraycolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctiongray);
            }

            foreach (Control ctrl in pnlredcolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionred);
            }

            foreach (Control ctrl in pnlgreencolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctiongreen);
            }

            foreach (Control ctrl in pnlbluecolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionblue);
            }

            foreach (Control ctrl in pnlorangecolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionorange);
            }

            foreach (Control ctrl in pnlyellowcolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionyellow);
            }

            foreach (Control ctrl in pnlbrowncolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionbrown);
            }

            foreach (Control ctrl in pnlpurplecolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionpurple);
            }

            foreach (Control ctrl in pnlpinkcolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.MouseDown += new MouseEventHandler(this.colourselctionpink);
            }
            pnloldcolour.Tag = "keepbg";
            pnloldcolour.Click += new EventHandler(pnloldcolour_Click);
            pnlnewcolour.Click += new EventHandler(pnlnewcolour_Click);
            pnlnewcolour.Tag = "keepbg";
        }

        public int anylevel = 0;
        public int graylevel = 0;
        public int greenlevel = 0;
        public int redlevel = 0;
        public int bluelevel = 0;
        public int yellowlevel = 0;
        public int orangelevel = 0;
        public int brownlevel = 0;
        public int purplelevel = 0;
        public int pinklevel = 0;



        public void loadmemory()
        {
            ///NYI






        }

        public void saveanymemory()
        {
            API.anymemory[0] = pnlany1.BackColor;
            API.anymemory[1] = pnlany2.BackColor;
            API.anymemory[2] = pnlany3.BackColor;
            API.anymemory[3] = pnlany4.BackColor;
            API.anymemory[4] = pnlany5.BackColor;
            API.anymemory[5] = pnlany6.BackColor;
            API.anymemory[6] = pnlany7.BackColor;
            API.anymemory[7] = pnlany8.BackColor;
            API.anymemory[8] = pnlany9.BackColor;
            API.anymemory[9] = pnlany10.BackColor;
            API.anymemory[10] = pnlany11.BackColor;
            API.anymemory[11] = pnlany12.BackColor;
            API.anymemory[12] = pnlany13.BackColor;
            API.anymemory[13] = pnlany14.BackColor;
            API.anymemory[14] = pnlany15.BackColor;
            API.anymemory[15] = pnlany16.BackColor;
        }

        public void savegraymemory()
        {
            API.graymemory[0] = pnlgray1.BackColor;
            API.graymemory[1] = pnlgray2.BackColor;
            API.graymemory[2] = pnlgray3.BackColor;
            API.graymemory[3] = pnlgray4.BackColor;
            API.graymemory[4] = pnlgray5.BackColor;
            API.graymemory[5] = pnlgray6.BackColor;
            API.graymemory[6] = pnlgray7.BackColor;
            API.graymemory[7] = pnlgray8.BackColor;
            API.graymemory[8] = pnlgray9.BackColor;
            API.graymemory[9] = pnlgray10.BackColor;
            API.graymemory[10] = pnlgray11.BackColor;
            API.graymemory[11] = pnlgray12.BackColor;
            API.graymemory[12] = pnlgray13.BackColor;
            API.graymemory[13] = pnlgray14.BackColor;
            API.graymemory[14] = pnlgray15.BackColor;
            API.graymemory[15] = pnlgray16.BackColor;
        }

        public void savepurplememory()
        {
            API.purplememory[0] = pnlpurple1.BackColor;
            API.purplememory[1] = pnlpurple2.BackColor;
            API.purplememory[2] = pnlpurple3.BackColor;
            API.purplememory[3] = pnlpurple4.BackColor;
            API.purplememory[4] = pnlpurple5.BackColor;
            API.purplememory[5] = pnlpurple6.BackColor;
            API.purplememory[6] = pnlpurple7.BackColor;
            API.purplememory[7] = pnlpurple8.BackColor;
            API.purplememory[8] = pnlpurple9.BackColor;
            API.purplememory[9] = pnlpurple10.BackColor;
            API.purplememory[10] = pnlpurple11.BackColor;
            API.purplememory[11] = pnlpurple12.BackColor;
            API.purplememory[12] = pnlpurple13.BackColor;
            API.purplememory[13] = pnlpurple14.BackColor;
            API.purplememory[14] = pnlpurple15.BackColor;
            API.purplememory[15] = pnlpurple16.BackColor;
        }

        public void savebluememory()
        {
            API.bluememory[0] = pnlblue1.BackColor;
            API.bluememory[1] = pnlblue2.BackColor;
            API.bluememory[2] = pnlblue3.BackColor;
            API.bluememory[3] = pnlblue4.BackColor;
            API.bluememory[4] = pnlblue5.BackColor;
            API.bluememory[5] = pnlblue6.BackColor;
            API.bluememory[6] = pnlblue7.BackColor;
            API.bluememory[7] = pnlblue8.BackColor;
            API.bluememory[8] = pnlblue9.BackColor;
            API.bluememory[9] = pnlblue10.BackColor;
            API.bluememory[10] = pnlblue11.BackColor;
            API.bluememory[11] = pnlblue12.BackColor;
            API.bluememory[12] = pnlblue13.BackColor;
            API.bluememory[13] = pnlblue14.BackColor;
            API.bluememory[14] = pnlblue15.BackColor;
            API.bluememory[15] = pnlblue16.BackColor;
        }

        public void savegreenmemory()
        {
            API.greenmemory[0] = pnlgreen1.BackColor;
            API.greenmemory[1] = pnlgreen2.BackColor;
            API.greenmemory[2] = pnlgreen3.BackColor;
            API.greenmemory[3] = pnlgreen4.BackColor;
            API.greenmemory[4] = pnlgreen5.BackColor;
            API.greenmemory[5] = pnlgreen6.BackColor;
            API.greenmemory[6] = pnlgreen7.BackColor;
            API.greenmemory[7] = pnlgreen8.BackColor;
            API.greenmemory[8] = pnlgreen9.BackColor;
            API.greenmemory[9] = pnlgreen10.BackColor;
            API.greenmemory[10] = pnlgreen11.BackColor;
            API.greenmemory[11] = pnlgreen12.BackColor;
            API.greenmemory[12] = pnlgreen13.BackColor;
            API.greenmemory[13] = pnlgreen14.BackColor;
            API.greenmemory[14] = pnlgreen15.BackColor;
            API.greenmemory[15] = pnlgreen16.BackColor;
        }

        public void saveyellowmemory()
        {
            API.yellowmemory[0] = pnlyellow1.BackColor;
            API.yellowmemory[1] = pnlyellow2.BackColor;
            API.yellowmemory[2] = pnlyellow3.BackColor;
            API.yellowmemory[3] = pnlyellow4.BackColor;
            API.yellowmemory[4] = pnlyellow5.BackColor;
            API.yellowmemory[5] = pnlyellow6.BackColor;
            API.yellowmemory[6] = pnlyellow7.BackColor;
            API.yellowmemory[7] = pnlyellow8.BackColor;
            API.yellowmemory[8] = pnlyellow9.BackColor;
            API.yellowmemory[9] = pnlyellow10.BackColor;
            API.yellowmemory[10] = pnlyellow11.BackColor;
            API.yellowmemory[11] = pnlyellow12.BackColor;
            API.yellowmemory[12] = pnlyellow13.BackColor;
            API.yellowmemory[13] = pnlyellow14.BackColor;
            API.yellowmemory[14] = pnlyellow15.BackColor;
            API.yellowmemory[15] = pnlyellow16.BackColor;
        }

        public void saveorangememory()
        {
            API.orangememory[0] = pnlorange1.BackColor;
            API.orangememory[1] = pnlorange2.BackColor;
            API.orangememory[2] = pnlorange3.BackColor;
            API.orangememory[3] = pnlorange4.BackColor;
            API.orangememory[4] = pnlorange5.BackColor;
            API.orangememory[5] = pnlorange6.BackColor;
            API.orangememory[6] = pnlorange7.BackColor;
            API.orangememory[7] = pnlorange8.BackColor;
            API.orangememory[8] = pnlorange9.BackColor;
            API.orangememory[9] = pnlorange10.BackColor;
            API.orangememory[10] = pnlorange11.BackColor;
            API.orangememory[11] = pnlorange12.BackColor;
            API.orangememory[12] = pnlorange13.BackColor;
            API.orangememory[13] = pnlorange14.BackColor;
            API.orangememory[14] = pnlorange15.BackColor;
            API.orangememory[15] = pnlorange16.BackColor;
        }

        public void savebrownmemory()
        {
            API.brownmemory[0] = pnlbrown1.BackColor;
            API.brownmemory[1] = pnlbrown2.BackColor;
            API.brownmemory[2] = pnlbrown3.BackColor;
            API.brownmemory[3] = pnlbrown4.BackColor;
            API.brownmemory[4] = pnlbrown5.BackColor;
            API.brownmemory[5] = pnlbrown6.BackColor;
            API.brownmemory[6] = pnlbrown7.BackColor;
            API.brownmemory[7] = pnlbrown8.BackColor;
            API.brownmemory[8] = pnlbrown9.BackColor;
            API.brownmemory[9] = pnlbrown10.BackColor;
            API.brownmemory[10] = pnlbrown11.BackColor;
            API.brownmemory[11] = pnlbrown12.BackColor;
            API.brownmemory[12] = pnlbrown13.BackColor;
            API.brownmemory[13] = pnlbrown14.BackColor;
            API.brownmemory[14] = pnlbrown15.BackColor;
            API.brownmemory[15] = pnlbrown16.BackColor;
        }

        public void saveredmemory()
        {
            API.redmemory[0] = pnlred1.BackColor;
            API.redmemory[1] = pnlred2.BackColor;
            API.redmemory[2] = pnlred3.BackColor;
            API.redmemory[3] = pnlred4.BackColor;
            API.redmemory[4] = pnlred5.BackColor;
            API.redmemory[5] = pnlred6.BackColor;
            API.redmemory[6] = pnlred7.BackColor;
            API.redmemory[7] = pnlred8.BackColor;
            API.redmemory[8] = pnlred9.BackColor;
            API.redmemory[9] = pnlred10.BackColor;
            API.redmemory[10] = pnlred11.BackColor;
            API.redmemory[11] = pnlred12.BackColor;
            API.redmemory[12] = pnlred13.BackColor;
            API.redmemory[13] = pnlred14.BackColor;
            API.redmemory[14] = pnlred15.BackColor;
            API.redmemory[15] = pnlred16.BackColor;
        }

        public void savepinkmemory()
        {
            API.pinkmemory[0] = pnlpink1.BackColor;
            API.pinkmemory[1] = pnlpink2.BackColor;
            API.pinkmemory[2] = pnlpink3.BackColor;
            API.pinkmemory[3] = pnlpink4.BackColor;
            API.pinkmemory[4] = pnlpink5.BackColor;
            API.pinkmemory[5] = pnlpink6.BackColor;
            API.pinkmemory[6] = pnlpink7.BackColor;
            API.pinkmemory[7] = pnlpink8.BackColor;
            API.pinkmemory[8] = pnlpink9.BackColor;
            API.pinkmemory[9] = pnlpink10.BackColor;
            API.pinkmemory[10] = pnlpink11.BackColor;
            API.pinkmemory[11] = pnlpink12.BackColor;
            API.pinkmemory[12] = pnlpink13.BackColor;
            API.pinkmemory[13] = pnlpink14.BackColor;
            API.pinkmemory[14] = pnlpink15.BackColor;
            API.pinkmemory[15] = pnlpink16.BackColor;
        }

        public string colourtochange = "";
        public Color oldcolour = Color.White;

        private void getoldcolour()
        {
            lblobjecttocolour.Text = colourtochange;
            pnloldcolour.BackColor = oldcolour;
            if (pnloldcolour.BackColor.IsNamedColor)
            {
                lbloldcolourname.Text = pnloldcolour.BackColor.Name + " :Name";
            }
            else
            {
                lbloldcolourname.Text = "Custom :Name";
            }
            lbloldcolourrgb.Text = pnloldcolour.BackColor.R + ", " + pnloldcolour.BackColor.G + ", " + pnloldcolour.BackColor.B + " :RGB";

            pnlnewcolour.BackColor = API.lastcolourpick;
            if (pnlnewcolour.BackColor.IsNamedColor)
            {
                lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
            }
            else
            {
                lblnewcolourname.Text = "Name: Custom";
            }
            lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
        }

        private void determinelevels()
        {
            graylevel = 4;
            anylevel = 4;
            purplelevel = 4;
            bluelevel = 4;
            greenlevel = 4;
            yellowlevel = 4;
            orangelevel = 4;
            brownlevel = 4;
            redlevel = 4;
            pinklevel = 4;
        }

        private void shrinktosizebasedoncoloursbought()
        {
            pnlpinkcolours.Show();
            pnlredcolours.Show();
            pnlbrowncolours.Show();
            pnlorangecolours.Show();
            pnlyellowcolours.Show();
            pnlgreencolours.Show();
            pnlbluecolours.Show();
            pnlpurplecolours.Show();
            pnlgraycolours.Show();
            pnlanycolours.Show();
        }

        private void setupboughtcolours()
        {
            switch (graylevel)
            {
                case 1:
                    pnlgray1.BackColor = Color.Black;
                    pnlgray1.Show();
                    pnlgray2.BackColor = Color.Gray;
                    pnlgray2.Show();
                    pnlgray3.BackColor = Color.White;
                    pnlgray3.Show();
                    break;
                case 2:
                    pnlgray1.BackColor = Color.Black;
                    pnlgray1.Show();
                    pnlgray2.BackColor = Color.DimGray;
                    pnlgray2.Show();
                    pnlgray3.BackColor = Color.Gray;
                    pnlgray3.Show();
                    pnlgray4.BackColor = Color.LightGray;
                    pnlgray4.Show();
                    pnlgray5.BackColor = Color.White;
                    pnlgray5.Show();
                    break;
                case 3:
                    pnlgray1.BackColor = Color.Black;
                    pnlgray1.Show();
                    pnlgray2.BackColor = Color.DimGray;
                    pnlgray2.Show();
                    pnlgray3.BackColor = Color.Gray;
                    pnlgray3.Show();
                    pnlgray4.BackColor = Color.DarkGray;
                    pnlgray4.Show();
                    pnlgray5.BackColor = Color.Silver;
                    pnlgray5.Show();
                    pnlgray6.BackColor = Color.LightGray;
                    pnlgray6.Show();
                    pnlgray7.BackColor = Color.Gainsboro;
                    pnlgray7.Show();
                    pnlgray8.BackColor = Color.WhiteSmoke;
                    pnlgray8.Show();
                    pnlgray9.BackColor = Color.White;
                    pnlgray9.Show();
                    break;
                case 4:
                    pnlgray1.BackColor = Color.Black;
                    pnlgray1.Show();
                    pnlgray2.BackColor = Color.DimGray;
                    pnlgray2.Show();
                    pnlgray3.BackColor = Color.Gray;
                    pnlgray3.Show();
                    pnlgray4.BackColor = Color.DarkGray;
                    pnlgray4.Show();
                    pnlgray5.BackColor = Color.Silver;
                    pnlgray5.Show();
                    pnlgray6.BackColor = Color.LightGray;
                    pnlgray6.Show();
                    pnlgray7.BackColor = Color.Gainsboro;
                    pnlgray7.Show();
                    pnlgray8.BackColor = Color.WhiteSmoke;
                    pnlgray8.Show();
                    pnlgray9.BackColor = Color.White;
                    pnlgray9.Show();
                    pnlgray10.BackColor = Color.White;
                    pnlgray10.Show();
                    pnlgray11.BackColor = Color.White;
                    pnlgray11.Show();
                    pnlgray12.BackColor = Color.White;
                    pnlgray12.Show();
                    pnlgray13.BackColor = Color.White;
                    pnlgray13.Show();
                    pnlgray14.BackColor = Color.White;
                    pnlgray14.Show();
                    pnlgray15.BackColor = Color.White;
                    pnlgray15.Show();
                    pnlgray16.BackColor = Color.White;
                    pnlgray16.Show();
                    pnlgraycustomcolour.Show();
                    lblcustomshadetut.Show();
                    txtcustomgrayshade.Show();
                    break;
            }

            switch (purplelevel)
            {
                case 1:
                    pnlpurple1.BackColor = Color.Purple;
                    pnlpurple1.Show();
                    break;
                case 2:
                    pnlpurple1.BackColor = Color.Indigo;
                    pnlpurple1.Show();
                    pnlpurple2.BackColor = Color.Purple;
                    pnlpurple2.Show();
                    pnlpurple3.BackColor = Color.MediumPurple;
                    pnlpurple3.Show();
                    break;
                case 3:
                    pnlpurple1.BackColor = Color.Indigo;
                    pnlpurple1.Show();
                    pnlpurple2.BackColor = Color.DarkSlateBlue;
                    pnlpurple2.Show();
                    pnlpurple3.BackColor = Color.Purple;
                    pnlpurple3.Show();
                    pnlpurple4.BackColor = Color.DarkOrchid;
                    pnlpurple4.Show();
                    pnlpurple5.BackColor = Color.DarkViolet;
                    pnlpurple5.Show();
                    pnlpurple6.BackColor = Color.BlueViolet;
                    pnlpurple6.Show();
                    pnlpurple7.BackColor = Color.SlateBlue;
                    pnlpurple7.Show();
                    pnlpurple8.BackColor = Color.MediumSlateBlue;
                    pnlpurple8.Show();
                    pnlpurple9.BackColor = Color.MediumPurple;
                    pnlpurple9.Show();
                    pnlpurple10.BackColor = Color.MediumOrchid;
                    pnlpurple10.Show();
                    pnlpurple11.BackColor = Color.Magenta;
                    pnlpurple11.Show();
                    pnlpurple12.BackColor = Color.Orchid;
                    pnlpurple12.Show();
                    pnlpurple13.BackColor = Color.Violet;
                    pnlpurple13.Show();
                    pnlpurple14.BackColor = Color.Plum;
                    pnlpurple14.Show();
                    pnlpurple15.BackColor = Color.Thistle;
                    pnlpurple15.Show();
                    pnlpurple16.BackColor = Color.Lavender;
                    pnlpurple16.Show();
                    break;
                case 4:
                    pnlpurple1.BackColor = Color.Indigo;
                    pnlpurple1.Show();
                    pnlpurple2.BackColor = Color.DarkSlateBlue;
                    pnlpurple2.Show();
                    pnlpurple3.BackColor = Color.Purple;
                    pnlpurple3.Show();
                    pnlpurple4.BackColor = Color.DarkOrchid;
                    pnlpurple4.Show();
                    pnlpurple5.BackColor = Color.DarkViolet;
                    pnlpurple5.Show();
                    pnlpurple6.BackColor = Color.BlueViolet;
                    pnlpurple6.Show();
                    pnlpurple7.BackColor = Color.SlateBlue;
                    pnlpurple7.Show();
                    pnlpurple8.BackColor = Color.MediumSlateBlue;
                    pnlpurple8.Show();
                    pnlpurple9.BackColor = Color.MediumPurple;
                    pnlpurple9.Show();
                    pnlpurple10.BackColor = Color.MediumOrchid;
                    pnlpurple10.Show();
                    pnlpurple11.BackColor = Color.Magenta;
                    pnlpurple11.Show();
                    pnlpurple12.BackColor = Color.Orchid;
                    pnlpurple12.Show();
                    pnlpurple13.BackColor = Color.Violet;
                    pnlpurple13.Show();
                    pnlpurple14.BackColor = Color.Plum;
                    pnlpurple14.Show();
                    pnlpurple15.BackColor = Color.Thistle;
                    pnlpurple15.Show();
                    pnlpurple16.BackColor = Color.Lavender;
                    pnlpurple16.Show();
                    pnlpurplecustomcolour.Show();
                    pnlpurpleoptions.Show();
                    break;
            }

            switch (bluelevel)
            {
                case 1:
                    pnlblue1.BackColor = Color.Blue;
                    pnlblue1.Show();
                    break;
                case 2:
                    pnlblue1.BackColor = Color.Navy;
                    pnlblue1.Show();
                    pnlblue2.BackColor = Color.Blue;
                    pnlblue2.Show();
                    pnlblue3.BackColor = Color.LightBlue;
                    pnlblue3.Show();
                    break;
                case 3:
                    pnlblue1.BackColor = Color.MidnightBlue;
                    pnlblue1.Show();
                    pnlblue2.BackColor = Color.Navy;
                    pnlblue2.Show();
                    pnlblue3.BackColor = Color.Blue;
                    pnlblue3.Show();
                    pnlblue4.BackColor = Color.RoyalBlue;
                    pnlblue4.Show();
                    pnlblue5.BackColor = Color.CornflowerBlue;
                    pnlblue5.Show();
                    pnlblue6.BackColor = Color.DeepSkyBlue;
                    pnlblue6.Show();
                    pnlblue7.BackColor = Color.SkyBlue;
                    pnlblue7.Show();
                    pnlblue8.BackColor = Color.LightBlue;
                    pnlblue8.Show();
                    pnlblue9.BackColor = Color.LightSteelBlue;
                    pnlblue9.Show();
                    pnlblue10.BackColor = Color.Cyan;
                    pnlblue10.Show();
                    pnlblue11.BackColor = Color.Aquamarine;
                    pnlblue11.Show();
                    pnlblue12.BackColor = Color.DarkTurquoise;
                    pnlblue12.Show();
                    pnlblue13.BackColor = Color.LightSeaGreen;
                    pnlblue13.Show();
                    pnlblue14.BackColor = Color.MediumAquamarine;
                    pnlblue14.Show();
                    pnlblue15.BackColor = Color.CadetBlue;
                    pnlblue15.Show();
                    pnlblue16.BackColor = Color.Teal;
                    pnlblue16.Show();
                    break;
                case 4:
                    pnlblue1.BackColor = Color.MidnightBlue;
                    pnlblue1.Show();
                    pnlblue2.BackColor = Color.Navy;
                    pnlblue2.Show();
                    pnlblue3.BackColor = Color.Blue;
                    pnlblue3.Show();
                    pnlblue4.BackColor = Color.RoyalBlue;
                    pnlblue4.Show();
                    pnlblue5.BackColor = Color.CornflowerBlue;
                    pnlblue5.Show();
                    pnlblue6.BackColor = Color.DeepSkyBlue;
                    pnlblue6.Show();
                    pnlblue7.BackColor = Color.SkyBlue;
                    pnlblue7.Show();
                    pnlblue8.BackColor = Color.LightBlue;
                    pnlblue8.Show();
                    pnlblue9.BackColor = Color.LightSteelBlue;
                    pnlblue9.Show();
                    pnlblue10.BackColor = Color.Cyan;
                    pnlblue10.Show();
                    pnlblue11.BackColor = Color.Aquamarine;
                    pnlblue11.Show();
                    pnlblue12.BackColor = Color.DarkTurquoise;
                    pnlblue12.Show();
                    pnlblue13.BackColor = Color.LightSeaGreen;
                    pnlblue13.Show();
                    pnlblue14.BackColor = Color.MediumAquamarine;
                    pnlblue14.Show();
                    pnlblue15.BackColor = Color.CadetBlue;
                    pnlblue15.Show();
                    pnlblue16.BackColor = Color.Teal;
                    pnlblue16.Show();
                    pnlbluecustomcolour.Show();
                    pnlblueoptions.Show();
                    break;
            }

            switch (greenlevel)
            {
                case 1:
                    pnlgreen1.BackColor = Color.Green;
                    pnlgreen1.Show();
                    break;
                case 2:
                    pnlgreen1.BackColor = Color.DarkGreen;
                    pnlgreen1.Show();
                    pnlgreen2.BackColor = Color.Green;
                    pnlgreen2.Show();
                    pnlgreen3.BackColor = Color.LightGreen;
                    pnlgreen3.Show();
                    break;
                case 3:
                    pnlgreen1.BackColor = Color.DarkGreen;
                    pnlgreen1.Show();
                    pnlgreen2.BackColor = Color.Green;
                    pnlgreen2.Show();
                    pnlgreen3.BackColor = Color.SeaGreen;
                    pnlgreen3.Show();
                    pnlgreen4.BackColor = Color.MediumSeaGreen;
                    pnlgreen4.Show();
                    pnlgreen5.BackColor = Color.DarkSeaGreen;
                    pnlgreen5.Show();
                    pnlgreen6.BackColor = Color.LightGreen;
                    pnlgreen6.Show();
                    pnlgreen7.BackColor = Color.MediumSpringGreen;
                    pnlgreen7.Show();
                    pnlgreen8.BackColor = Color.SpringGreen;
                    pnlgreen8.Show();
                    pnlgreen9.BackColor = Color.GreenYellow;
                    pnlgreen9.Show();
                    pnlgreen10.BackColor = Color.LawnGreen;
                    pnlgreen10.Show();
                    pnlgreen11.BackColor = Color.Lime;
                    pnlgreen11.Show();
                    pnlgreen12.BackColor = Color.LimeGreen;
                    pnlgreen12.Show();
                    pnlgreen13.BackColor = Color.YellowGreen;
                    pnlgreen13.Show();
                    pnlgreen14.BackColor = Color.OliveDrab;
                    pnlgreen14.Show();
                    pnlgreen15.BackColor = Color.Olive;
                    pnlgreen15.Show();
                    pnlgreen16.BackColor = Color.DarkOliveGreen;
                    pnlgreen16.Show();
                    break;
                case 4:
                    pnlgreen1.BackColor = Color.DarkGreen;
                    pnlgreen1.Show();
                    pnlgreen2.BackColor = Color.Green;
                    pnlgreen2.Show();
                    pnlgreen3.BackColor = Color.SeaGreen;
                    pnlgreen3.Show();
                    pnlgreen4.BackColor = Color.MediumSeaGreen;
                    pnlgreen4.Show();
                    pnlgreen5.BackColor = Color.DarkSeaGreen;
                    pnlgreen5.Show();
                    pnlgreen6.BackColor = Color.LightGreen;
                    pnlgreen6.Show();
                    pnlgreen7.BackColor = Color.MediumSpringGreen;
                    pnlgreen7.Show();
                    pnlgreen8.BackColor = Color.SpringGreen;
                    pnlgreen8.Show();
                    pnlgreen9.BackColor = Color.GreenYellow;
                    pnlgreen9.Show();
                    pnlgreen10.BackColor = Color.LawnGreen;
                    pnlgreen10.Show();
                    pnlgreen11.BackColor = Color.Lime;
                    pnlgreen11.Show();
                    pnlgreen12.BackColor = Color.LimeGreen;
                    pnlgreen12.Show();
                    pnlgreen13.BackColor = Color.YellowGreen;
                    pnlgreen13.Show();
                    pnlgreen14.BackColor = Color.OliveDrab;
                    pnlgreen14.Show();
                    pnlgreen15.BackColor = Color.Olive;
                    pnlgreen15.Show();
                    pnlgreen16.BackColor = Color.DarkOliveGreen;
                    pnlgreen16.Show();
                    pnlgreencustomcolour.Show();
                    pnlgreenoptions.Show();
                    break;
            }

            switch (yellowlevel)
            {
                case 1:
                    pnlyellow1.BackColor = Color.Yellow;
                    pnlyellow1.Show();
                    break;
                case 2:
                    pnlyellow1.BackColor = Color.DarkKhaki;
                    pnlyellow1.Show();
                    pnlyellow2.BackColor = Color.Yellow;
                    pnlyellow2.Show();
                    pnlyellow3.BackColor = Color.PaleGoldenrod;
                    pnlyellow3.Show();
                    break;
                case 3:
                    pnlyellow1.BackColor = Color.DarkKhaki;
                    pnlyellow1.Show();
                    pnlyellow2.BackColor = Color.Yellow;
                    pnlyellow2.Show();
                    pnlyellow3.BackColor = Color.Khaki;
                    pnlyellow3.Show();
                    pnlyellow4.BackColor = Color.PaleGoldenrod;
                    pnlyellow4.Show();
                    pnlyellow5.BackColor = Color.PeachPuff;
                    pnlyellow5.Show();
                    pnlyellow6.BackColor = Color.Moccasin;
                    pnlyellow6.Show();
                    pnlyellow7.BackColor = Color.PapayaWhip;
                    pnlyellow7.Show();
                    pnlyellow8.BackColor = Color.LightGoldenrodYellow;
                    pnlyellow8.Show();
                    pnlyellow9.BackColor = Color.LemonChiffon;
                    pnlyellow9.Show();
                    pnlyellow10.BackColor = Color.LightYellow;
                    pnlyellow10.Show();
                    break;
                case 4:
                    pnlyellow1.BackColor = Color.DarkKhaki;
                    pnlyellow1.Show();
                    pnlyellow2.BackColor = Color.Yellow;
                    pnlyellow2.Show();
                    pnlyellow3.BackColor = Color.Khaki;
                    pnlyellow3.Show();
                    pnlyellow4.BackColor = Color.PaleGoldenrod;
                    pnlyellow4.Show();
                    pnlyellow5.BackColor = Color.PeachPuff;
                    pnlyellow5.Show();
                    pnlyellow6.BackColor = Color.Moccasin;
                    pnlyellow6.Show();
                    pnlyellow7.BackColor = Color.PapayaWhip;
                    pnlyellow7.Show();
                    pnlyellow8.BackColor = Color.LightGoldenrodYellow;
                    pnlyellow8.Show();
                    pnlyellow9.BackColor = Color.LemonChiffon;
                    pnlyellow9.Show();
                    pnlyellow10.BackColor = Color.LightYellow;
                    pnlyellow10.Show();
                    pnlyellow11.BackColor = Color.White;
                    pnlyellow11.Show();
                    pnlyellow12.BackColor = Color.White;
                    pnlyellow12.Show();
                    pnlyellow13.BackColor = Color.White;
                    pnlyellow13.Show();
                    pnlyellow14.BackColor = Color.White;
                    pnlyellow14.Show();
                    pnlyellow15.BackColor = Color.White;
                    pnlyellow15.Show();
                    pnlyellow16.BackColor = Color.White;
                    pnlyellow16.Show();
                    pnlyellowcustomcolour.Show();
                    pnlyellowoptions.Show();
                    break;
            }

            switch (orangelevel)
            {
                case 1:
                    pnlorange1.BackColor = Color.DarkOrange;
                    pnlorange1.Show();
                    break;
                case 2:
                    pnlorange1.BackColor = Color.OrangeRed;
                    pnlorange1.Show();
                    pnlorange2.BackColor = Color.DarkOrange;
                    pnlorange2.Show();
                    pnlorange3.BackColor = Color.Orange;
                    pnlorange3.Show();
                    break;
                case 3:
                    pnlorange1.BackColor = Color.OrangeRed;
                    pnlorange1.Show();
                    pnlorange2.BackColor = Color.Tomato;
                    pnlorange2.Show();
                    pnlorange3.BackColor = Color.Coral;
                    pnlorange3.Show();
                    pnlorange4.BackColor = Color.DarkOrange;
                    pnlorange4.Show();
                    pnlorange5.BackColor = Color.Orange;
                    pnlorange5.Show();
                    pnlorange6.BackColor = Color.Gold;
                    pnlorange6.Show();
                    break;
                case 4:
                    pnlorange1.BackColor = Color.OrangeRed;
                    pnlorange1.Show();
                    pnlorange2.BackColor = Color.Tomato;
                    pnlorange2.Show();
                    pnlorange3.BackColor = Color.Coral;
                    pnlorange3.Show();
                    pnlorange4.BackColor = Color.DarkOrange;
                    pnlorange4.Show();
                    pnlorange5.BackColor = Color.Orange;
                    pnlorange5.Show();
                    pnlorange6.BackColor = Color.Gold;
                    pnlorange6.Show();
                    pnlorange7.BackColor = Color.White;
                    pnlorange7.Show();
                    pnlorange8.BackColor = Color.White;
                    pnlorange8.Show();
                    pnlorange9.BackColor = Color.White;
                    pnlorange9.Show();
                    pnlorange10.BackColor = Color.White;
                    pnlorange10.Show();
                    pnlorange11.BackColor = Color.White;
                    pnlorange11.Show();
                    pnlorange12.BackColor = Color.White;
                    pnlorange12.Show();
                    pnlorange13.BackColor = Color.White;
                    pnlorange13.Show();
                    pnlorange14.BackColor = Color.White;
                    pnlorange14.Show();
                    pnlorange15.BackColor = Color.White;
                    pnlorange15.Show();
                    pnlorange16.BackColor = Color.White;
                    pnlorange16.Show();
                    pnlorangecustomcolour.Show();
                    pnlorangeoptions.Show();
                    break;
            }

            switch (brownlevel)
            {
                case 1:
                    pnlbrown1.BackColor = Color.Sienna;
                    pnlbrown1.Show();
                    break;
                case 2:
                    pnlbrown1.BackColor = Color.SaddleBrown;
                    pnlbrown1.Show();
                    pnlbrown2.BackColor = Color.Sienna;
                    pnlbrown2.Show();
                    pnlbrown3.BackColor = Color.BurlyWood;
                    pnlbrown3.Show();
                    break;
                case 3:
                    pnlbrown1.BackColor = Color.Maroon;
                    pnlbrown1.Show();
                    pnlbrown2.BackColor = Color.Brown;
                    pnlbrown2.Show();
                    pnlbrown3.BackColor = Color.Sienna;
                    pnlbrown3.Show();
                    pnlbrown4.BackColor = Color.SaddleBrown;
                    pnlbrown4.Show();
                    pnlbrown5.BackColor = Color.Chocolate;
                    pnlbrown5.Show();
                    pnlbrown6.BackColor = Color.Peru;
                    pnlbrown6.Show();
                    pnlbrown7.BackColor = Color.DarkGoldenrod;
                    pnlbrown7.Show();
                    pnlbrown8.BackColor = Color.Goldenrod;
                    pnlbrown8.Show();
                    pnlbrown9.BackColor = Color.SandyBrown;
                    pnlbrown9.Show();
                    pnlbrown10.BackColor = Color.RosyBrown;
                    pnlbrown10.Show();
                    pnlbrown11.BackColor = Color.Tan;
                    pnlbrown11.Show();
                    pnlbrown12.BackColor = Color.BurlyWood;
                    pnlbrown12.Show();
                    pnlbrown13.BackColor = Color.Wheat;
                    pnlbrown13.Show();
                    pnlbrown14.BackColor = Color.NavajoWhite;
                    pnlbrown14.Show();
                    pnlbrown15.BackColor = Color.Bisque;
                    pnlbrown15.Show();
                    pnlbrown16.BackColor = Color.BlanchedAlmond;
                    pnlbrown16.Show();
                    break;
                case 4:
                    pnlbrown1.BackColor = Color.Maroon;
                    pnlbrown1.Show();
                    pnlbrown2.BackColor = Color.Brown;
                    pnlbrown2.Show();
                    pnlbrown3.BackColor = Color.Sienna;
                    pnlbrown3.Show();
                    pnlbrown4.BackColor = Color.SaddleBrown;
                    pnlbrown4.Show();
                    pnlbrown5.BackColor = Color.Chocolate;
                    pnlbrown5.Show();
                    pnlbrown6.BackColor = Color.Peru;
                    pnlbrown6.Show();
                    pnlbrown7.BackColor = Color.DarkGoldenrod;
                    pnlbrown7.Show();
                    pnlbrown8.BackColor = Color.Goldenrod;
                    pnlbrown8.Show();
                    pnlbrown9.BackColor = Color.SandyBrown;
                    pnlbrown9.Show();
                    pnlbrown10.BackColor = Color.RosyBrown;
                    pnlbrown10.Show();
                    pnlbrown11.BackColor = Color.Tan;
                    pnlbrown11.Show();
                    pnlbrown12.BackColor = Color.BurlyWood;
                    pnlbrown12.Show();
                    pnlbrown13.BackColor = Color.Wheat;
                    pnlbrown13.Show();
                    pnlbrown14.BackColor = Color.NavajoWhite;
                    pnlbrown14.Show();
                    pnlbrown15.BackColor = Color.Bisque;
                    pnlbrown15.Show();
                    pnlbrown16.BackColor = Color.BlanchedAlmond;
                    pnlbrown16.Show();
                    pnlbrowncustomcolour.Show();
                    pnlbrownoptions.Show();
                    break;
            }

            switch (redlevel)
            {
                case 1:
                    pnlred1.BackColor = Color.Red;
                    pnlred1.Show();
                    break;
                case 2:
                    pnlred1.BackColor = Color.DarkRed;
                    pnlred1.Show();
                    pnlred2.BackColor = Color.Red;
                    pnlred2.Show();
                    pnlred3.BackColor = Color.Salmon;
                    pnlred3.Show();
                    break;
                case 3:
                    pnlred1.BackColor = Color.DarkRed;
                    pnlred1.Show();
                    pnlred2.BackColor = Color.Red;
                    pnlred2.Show();
                    pnlred3.BackColor = Color.Firebrick;
                    pnlred3.Show();
                    pnlred4.BackColor = Color.Crimson;
                    pnlred4.Show();
                    pnlred5.BackColor = Color.IndianRed;
                    pnlred5.Show();
                    pnlred6.BackColor = Color.LightCoral;
                    pnlred6.Show();
                    pnlred7.BackColor = Color.DarkSalmon;
                    pnlred7.Show();
                    pnlred8.BackColor = Color.Salmon;
                    pnlred8.Show();
                    pnlred9.BackColor = Color.LightSalmon;
                    pnlred9.Show();
                    break;
                case 4:
                    pnlred1.BackColor = Color.DarkRed;
                    pnlred1.Show();
                    pnlred2.BackColor = Color.Red;
                    pnlred2.Show();
                    pnlred3.BackColor = Color.Firebrick;
                    pnlred3.Show();
                    pnlred4.BackColor = Color.Crimson;
                    pnlred4.Show();
                    pnlred5.BackColor = Color.IndianRed;
                    pnlred5.Show();
                    pnlred6.BackColor = Color.LightCoral;
                    pnlred6.Show();
                    pnlred7.BackColor = Color.DarkSalmon;
                    pnlred7.Show();
                    pnlred8.BackColor = Color.Salmon;
                    pnlred8.Show();
                    pnlred9.BackColor = Color.LightSalmon;
                    pnlred9.Show();
                    pnlred10.BackColor = Color.White;
                    pnlred10.Show();
                    pnlred11.BackColor = Color.White;
                    pnlred11.Show();
                    pnlred12.BackColor = Color.White;
                    pnlred12.Show();
                    pnlred13.BackColor = Color.White;
                    pnlred13.Show();
                    pnlred14.BackColor = Color.White;
                    pnlred14.Show();
                    pnlred15.BackColor = Color.White;
                    pnlred15.Show();
                    pnlred16.BackColor = Color.White;
                    pnlred16.Show();
                    pnlredcustomcolour.Show();
                    pnlredoptions.Show();
                    break;
            }

            switch (pinklevel)
            {
                case 1:
                    pnlpink1.BackColor = Color.HotPink;
                    pnlpink1.Show();
                    pnlpink1.BackColor = Color.DeepPink;
                    pnlpink1.Show();
                    pnlpink2.BackColor = Color.HotPink;
                    pnlpink2.Show();
                    pnlpink3.BackColor = Color.LightPink;
                    pnlpink3.Show();
                    break;
                case 3:
                    pnlpink1.BackColor = Color.MediumVioletRed;
                    pnlpink1.Show();
                    pnlpink2.BackColor = Color.PaleVioletRed;
                    pnlpink2.Show();
                    pnlpink3.BackColor = Color.DeepPink;
                    pnlpink3.Show();
                    pnlpink4.BackColor = Color.HotPink;
                    pnlpink4.Show();
                    pnlpink5.BackColor = Color.LightPink;
                    pnlpink5.Show();
                    pnlpink6.BackColor = Color.Pink;
                    pnlpink6.Show();
                    break;
                case 4:
                    pnlpink1.BackColor = Color.MediumVioletRed;
                    pnlpink1.Show();
                    pnlpink2.BackColor = Color.PaleVioletRed;
                    pnlpink2.Show();
                    pnlpink3.BackColor = Color.DeepPink;
                    pnlpink3.Show();
                    pnlpink4.BackColor = Color.HotPink;
                    pnlpink4.Show();
                    pnlpink5.BackColor = Color.LightPink;
                    pnlpink5.Show();
                    pnlpink6.BackColor = Color.Pink;
                    pnlpink6.Show();
                    pnlpink7.BackColor = Color.White;
                    pnlpink7.Show();
                    pnlpink8.BackColor = Color.White;
                    pnlpink8.Show();
                    pnlpink9.BackColor = Color.White;
                    pnlpink9.Show();
                    pnlpink10.BackColor = Color.White;
                    pnlpink10.Show();
                    pnlpink11.BackColor = Color.White;
                    pnlpink11.Show();
                    pnlpink12.BackColor = Color.White;
                    pnlpink12.Show();
                    pnlpink13.BackColor = Color.White;
                    pnlpink13.Show();
                    pnlpink14.BackColor = Color.White;
                    pnlpink14.Show();
                    pnlpink15.BackColor = Color.White;
                    pnlpink15.Show();
                    pnlpink16.BackColor = Color.White;
                    pnlpink16.Show();
                    pnlpinkcustomcolour.Show();
                    pnlpinkoptions.Show();
                    break;
            }

            switch (anylevel)
            {
                case 1:
                    pnlany1.BackColor = Color.White;
                    pnlany1.Show();
                    pnlanycustomcolour.Show();
                    pnlanyoptions.Show();
                    break;
                case 2:
                    pnlany1.BackColor = Color.White;
                    pnlany1.Show();
                    pnlany2.BackColor = Color.White;
                    pnlany2.Show();
                    pnlany3.BackColor = Color.White;
                    pnlany3.Show();
                    pnlany4.BackColor = Color.White;
                    pnlany4.Show();
                    pnlanycustomcolour.Show();
                    pnlanyoptions.Show();
                    break;
                case 3:
                    pnlany1.BackColor = Color.White;
                    pnlany1.Show();
                    pnlany2.BackColor = Color.White;
                    pnlany2.Show();
                    pnlany3.BackColor = Color.White;
                    pnlany3.Show();
                    pnlany4.BackColor = Color.White;
                    pnlany4.Show();
                    pnlany5.BackColor = Color.White;
                    pnlany5.Show();
                    pnlany6.BackColor = Color.White;
                    pnlany6.Show();
                    pnlany7.BackColor = Color.White;
                    pnlany7.Show();
                    pnlany8.BackColor = Color.White;
                    pnlany8.Show();
                    pnlanycustomcolour.Show();
                    pnlanyoptions.Show();
                    break;
                case 4:
                    pnlany1.BackColor = Color.White;
                    pnlany1.Show();
                    pnlany2.BackColor = Color.White;
                    pnlany2.Show();
                    pnlany3.BackColor = Color.White;
                    pnlany3.Show();
                    pnlany4.BackColor = Color.White;
                    pnlany4.Show();
                    pnlany5.BackColor = Color.White;
                    pnlany5.Show();
                    pnlany6.BackColor = Color.White;
                    pnlany6.Show();
                    pnlany7.BackColor = Color.White;
                    pnlany7.Show();
                    pnlany8.BackColor = Color.White;
                    pnlany8.Show();
                    pnlany9.BackColor = Color.White;
                    pnlany9.Show();
                    pnlany10.BackColor = Color.White;
                    pnlany10.Show();
                    pnlany11.BackColor = Color.White;
                    pnlany11.Show();
                    pnlany12.BackColor = Color.White;
                    pnlany12.Show();
                    pnlany13.BackColor = Color.White;
                    pnlany13.Show();
                    pnlany14.BackColor = Color.White;
                    pnlany14.Show();
                    pnlany15.BackColor = Color.White;
                    pnlany15.Show();
                    pnlany16.BackColor = Color.White;
                    pnlany16.Show();
                    pnlanycustomcolour.Show();
                    pnlanyoptions.Show();
                    break;
            }
        }

        public string Result = "Nothing";

        // ERROR: Handles clauses are not supported in C#
        private void pnloldcolour_Click(object sender, EventArgs e)
        {
            Callback?.Invoke(pnloldcolour.BackColor);
            this.Close();
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlnewcolour_Click(object sender, EventArgs e)
        {
            NewColor = pnlnewcolour.BackColor;
            Callback?.Invoke(NewColor);
            this.Close();
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnloldcolour_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), pnloldcolour.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlanycolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlanycolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlgraycolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlgraycolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlpurplecolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlpurplecolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlbluecolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlbluecolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlgreencolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlgreencolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlyellowcolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlyellowcolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlorangecolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlorangecolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlbrowncolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlbrowncolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlredcolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlredcolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlpinkcolours_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), pnlpinkcolours.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlnewcolour_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), pnlnewcolour.ClientRectangle);
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctiongray(object sender, MouseEventArgs e)
        {
            try
            {
                Panel spanel = (Panel)sender;
                if (e.Button == MouseButtons.Left)
                {
                    pnlnewcolour.BackColor = spanel.BackColor;
                    if (pnlnewcolour.BackColor.IsNamedColor)
                    {
                        if (pnlnewcolour.BackColor.Name.Length > 12)
                        {
                            lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                        }
                        else
                        {
                            lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                        }
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: Custom";
                    }
                    lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
                }
                if (e.Button == MouseButtons.Right)
                {
                    spanel.BackColor = pnlgraycustomcolour.BackColor;
                    savegraymemory();
                }
            }
            catch { }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionpurple(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlpurplecustomcolour.BackColor;
                savepurplememory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionblue(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlbluecustomcolour.BackColor;
                savebluememory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctiongreen(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlgreencustomcolour.BackColor;
                savegreenmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionyellow(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlyellowcustomcolour.BackColor;
                saveyellowmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionorange(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlorangecustomcolour.BackColor;
                saveorangememory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionbrown(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlbrowncustomcolour.BackColor;
                savebrownmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionred(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlredcustomcolour.BackColor;
                saveredmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionpink(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlpinkcustomcolour.BackColor;
                savepinkmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void colourselctionany(object sender, MouseEventArgs e)
        {
            Panel spanel = (Panel)sender;
            if (e.Button == MouseButtons.Left)
            {
                pnlnewcolour.BackColor = spanel.BackColor;
                if (pnlnewcolour.BackColor.IsNamedColor)
                {
                    if (pnlnewcolour.BackColor.Name.Length > 12)
                    {
                        lblnewcolourname.Text = pnlnewcolour.BackColor.Name;
                    }
                    else
                    {
                        lblnewcolourname.Text = "Name: " + pnlnewcolour.BackColor.Name;
                    }
                }
                else
                {
                    lblnewcolourname.Text = "Name: Custom";
                }
                lblnewcolourrgb.Text = "RGB: " + pnlnewcolour.BackColor.R + ", " + pnlnewcolour.BackColor.G + ", " + pnlnewcolour.BackColor.B;
            }
            if (e.Button == MouseButtons.Right)
            {
                spanel.BackColor = pnlanycustomcolour.BackColor;
                saveanymemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtcustomgrayshade_TextChanged(object sender, EventArgs e)
        {
            if (txtcustomgrayshade.Text == "")
            {
                txtcustomgrayshade.Text = "0";
            }
            else
            {
                if (Convert.ToInt32(txtcustomgrayshade.Text) > 255)
                {
                    txtcustomgrayshade.Text = "255";
                }
                else
                {
                    pnlgraycustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtcustomgrayshade.Text), Convert.ToInt16(txtcustomgrayshade.Text), Convert.ToInt16(txtcustomgrayshade.Text));
                }
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void CheckForNumber(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    e.Handled = true;
                    break;
            }
        }

        private void customcolourfailsafe()
        {
            if (txtcustomgrayshade.Text == "")
                txtcustomgrayshade.Text = "0";

            if (txtanysblue.Text == "")
                txtanysblue.Text = "0";
            if (txtanysred.Text == "")
                txtanysred.Text = "0";
            if (txtanysgreen.Text == "")
                txtanysgreen.Text = "0";

            if (txtpurplesblue.Text == "")
                txtpurplesblue.Text = "255";
            if (txtpurplesred.Text == "")
                txtpurplesred.Text = "150";
            if (txtpurplesgreen.Text == "")
                txtpurplesgreen.Text = "0";

            if (txtbluesblue.Text == "")
                txtbluesblue.Text = "255";
            if (txtbluesred.Text == "")
                txtbluesred.Text = "0";
            if (txtbluesgreen.Text == "")
                txtbluesgreen.Text = "0";

            if (txtgreensblue.Text == "")
                txtgreensblue.Text = "0";
            if (txtgreensred.Text == "")
                txtgreensred.Text = "0";
            if (txtgreensgreen.Text == "")
                txtgreensgreen.Text = "255";

            if (txtyellowsblue.Text == "")
                txtyellowsblue.Text = "0";
            if (txtyellowsred.Text == "")
                txtyellowsred.Text = "255";
            if (txtyellowsgreen.Text == "")
                txtyellowsgreen.Text = "255";

            if (txtorangesblue.Text == "")
                txtorangesblue.Text = "0";
            if (txtorangesred.Text == "")
                txtorangesred.Text = "255";
            if (txtorangesgreen.Text == "")
                txtorangesgreen.Text = "60";

            if (txtbrownsblue.Text == "")
                txtbrownsblue.Text = "0";
            if (txtbrownsred.Text == "")
                txtbrownsred.Text = "140";
            if (txtbrownsgreen.Text == "")
                txtbrownsgreen.Text = "60";

            if (txtredsblue.Text == "")
                txtredsblue.Text = "0";
            if (txtredsred.Text == "")
                txtredsred.Text = "255";
            if (txtredsgreen.Text == "")
                txtredsgreen.Text = "0";

            if (txtpinksblue.Text == "")
                txtpinksblue.Text = "150";
            if (txtpinksred.Text == "")
                txtpinksred.Text = "250";
            if (txtpinksgreen.Text == "")
                txtpinksgreen.Text = "0";
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtpurplesgreen_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlpurplecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtpurplesred.Text), Convert.ToInt16(txtpurplesgreen.Text), Convert.ToInt16(txtpurplesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlpurpleoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtpurplesblue.Text) > 255)
            {
                txtpurplesblue.Text = "255";
            }
            if (Convert.ToInt32(txtpurplesred.Text) > Convert.ToInt32(txtpurplesblue.Text))
            {
                txtpurplesred.Text = txtpurplesblue.Text;
            }
            if (Convert.ToInt32(txtpurplesgreen.Text) > Convert.ToInt32(txtpurplesblue.Text))
            {
                txtpurplesgreen.Text = txtpurplesred.Text;
            }
            if (Convert.ToInt32(txtpurplesgreen.Text) > Convert.ToInt32(txtpurplesred.Text))
            {
                txtpurplesgreen.Text = txtpurplesred.Text;
            }
            pnlpurplecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtpurplesred.Text), Convert.ToInt16(txtpurplesgreen.Text), Convert.ToInt16(txtpurplesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtbluesgreen_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlbluecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtbluesred.Text), Convert.ToInt16(txtbluesgreen.Text), Convert.ToInt16(txtbluesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlblueoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtbluesblue.Text) > 255)
            {
                txtbluesblue.Text = "255";
            }
            if (Convert.ToInt32(txtbluesgreen.Text) > Convert.ToInt32(txtbluesblue.Text))
            {
                txtbluesgreen.Text = txtbluesblue.Text;
            }
            if (Convert.ToInt32(txtbluesred.Text) > Convert.ToInt32(txtbluesblue.Text))
            {
                txtbluesred.Text = txtbluesgreen.Text;
            }
            if (Convert.ToInt32(txtbluesred.Text) > Convert.ToInt32(txtbluesgreen.Text))
            {
                txtbluesred.Text = txtbluesgreen.Text;
            }
            pnlbluecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtbluesred.Text), Convert.ToInt16(txtbluesgreen.Text), Convert.ToInt16(txtbluesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtgreensgreen_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlgreencustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtgreensred.Text), Convert.ToInt16(txtgreensgreen.Text), Convert.ToInt16(txtgreensblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlgreenoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtgreensgreen.Text) > 255)
            {
                txtgreensgreen.Text = "255";
            }
            if (Convert.ToInt32(txtgreensblue.Text) > Convert.ToInt32(txtgreensgreen.Text))
            {
                txtgreensblue.Text = txtgreensgreen.Text;
            }
            if (Convert.ToInt32(txtgreensred.Text) > Convert.ToInt32(txtgreensgreen.Text))
            {
                txtgreensred.Text = txtgreensgreen.Text;
            }
            if (Convert.ToInt32(txtgreensblue.Text) > Convert.ToInt32(txtgreensred.Text + 150))
            {
                txtgreensblue.Text = (Convert.ToInt16(txtgreensred.Text) + 150).ToString();
            }
            if (Convert.ToInt32(txtgreensred.Text) > Convert.ToInt32(txtgreensblue.Text + 150))
            {
                txtgreensred.Text = (Convert.ToInt32(txtgreensblue.Text) + 150).ToString();
            }
            pnlgreencustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtgreensred.Text), Convert.ToInt16(txtgreensgreen.Text), Convert.ToInt16(txtgreensblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtyellowsred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlyellowcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtyellowsred.Text), Convert.ToInt16(txtyellowsgreen.Text), Convert.ToInt16(txtyellowsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlyellowoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtyellowsred.Text) > 255)
            {
                txtyellowsred.Text = "255";
            }
            if (Convert.ToInt32(txtyellowsred.Text) < 180)
            {
                txtyellowsred.Text = "180";
            }
            if (Convert.ToInt32(txtyellowsgreen.Text) > Convert.ToInt32(txtyellowsred.Text))
            {
                txtyellowsgreen.Text = txtyellowsred.Text;
            }
            if (Convert.ToInt32(txtyellowsgreen.Text) < (Convert.ToInt32(txtyellowsred.Text) - 30))
            {
                txtyellowsgreen.Text = (Convert.ToInt32(txtyellowsred.Text) - 30).ToString();
            }
            if (Convert.ToInt32(txtyellowsblue.Text) > Convert.ToInt32(txtyellowsgreen.Text))
            {
                txtyellowsblue.Text = txtyellowsgreen.Text;
            }
            pnlyellowcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtyellowsred.Text), Convert.ToInt16(txtyellowsgreen.Text), Convert.ToInt16(txtyellowsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtorangesred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlorangecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtorangesred.Text), Convert.ToInt16(txtorangesgreen.Text), Convert.ToInt16(txtorangesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlorangeoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtorangesred.Text) > 255)
            {
                txtorangesred.Text = "255";
            }
            if (Convert.ToInt32(txtorangesred.Text) < 255)
            {
                txtorangesred.Text = "255";
            }
            if (Convert.ToInt32(txtorangesgreen.Text) > Convert.ToInt32(txtorangesred.Text) - 100)
            {
                txtorangesgreen.Text = (Convert.ToInt32(txtorangesred.Text) - 100).ToString();
            }
            if (Convert.ToInt32(txtorangesgreen.Text) < 30)
            {
                txtorangesgreen.Text = "30";
            }
            if (Convert.ToInt32(txtorangesblue.Text) > (Convert.ToInt32(txtorangesgreen.Text) - 30))
            {
                txtorangesblue.Text = (Convert.ToInt32(txtorangesgreen.Text) - 30).ToString();
            }
            pnlorangecustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtorangesred.Text), Convert.ToInt16(txtorangesgreen.Text), Convert.ToInt16(txtorangesblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtbrownsred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlbrowncustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtbrownsred.Text), Convert.ToInt16(txtbrownsgreen.Text), Convert.ToInt16(txtbrownsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlbrownoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtbrownsred.Text) > 255)
            {
                txtbrownsred.Text = "255";
            }
            if (Convert.ToInt32(txtbrownsred.Text) < 90)
            {
                txtbrownsred.Text = "90";
            }
            if (Convert.ToInt32(txtbrownsgreen.Text) > Convert.ToInt32(txtbrownsred.Text) - 30)
            {
                txtbrownsgreen.Text = (Convert.ToInt32(txtbrownsred.Text) - 30).ToString();
            }
            if (Convert.ToInt32(txtbrownsgreen.Text) < Convert.ToInt32(txtbrownsred.Text) - 128)
            {
                txtbrownsgreen.Text = (Convert.ToInt32(txtbrownsred.Text) - 128).ToString();
            }
            if (Convert.ToInt32(txtbrownsgreen.Text) < 60)
            {
                txtbrownsgreen.Text = "60";
            }
            if (Convert.ToInt32(txtbrownsblue.Text) > Convert.ToInt32(txtbrownsgreen.Text) - 60)
            {
                txtbrownsblue.Text = (Convert.ToInt32(txtbrownsgreen.Text) - 60).ToString();
            }
            pnlbrowncustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtbrownsred.Text), Convert.ToInt16(txtbrownsgreen.Text), Convert.ToInt16(txtbrownsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtsred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement

            pnlredcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtredsred.Text), Convert.ToInt16(txtredsgreen.Text), Convert.ToInt16(txtredsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlredoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtredsred.Text) > 255)
            {
                txtredsred.Text = "255";
            }
            if (Convert.ToInt32(txtredsblue.Text) > Convert.ToInt32(txtredsred.Text) - 80)
            {
                txtredsblue.Text = (Convert.ToInt32(txtredsred.Text) - 80).ToString();
            }
            if (Convert.ToInt32(txtredsgreen.Text) > Convert.ToInt32(txtredsred.Text) - 80)
            {
                txtredsgreen.Text = (Convert.ToInt32(txtredsred.Text) - 80).ToString();
            }
            if (Convert.ToInt32(txtredsgreen.Text) > Convert.ToInt32(txtredsblue.Text + 50))
            {
                txtredsgreen.Text = (Convert.ToInt32(txtredsblue.Text) + 50).ToString();
            }
            if (Convert.ToInt32(txtredsblue.Text) > Convert.ToInt32(txtredsgreen.Text + 50))
            {
                txtredsblue.Text = (Convert.ToInt32(txtredsgreen.Text) + 50).ToString();
            }
            pnlredcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtredsred.Text), Convert.ToInt16(txtredsgreen.Text), Convert.ToInt16(txtredsblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtpinksred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement
            try
            {
                pnlpinkcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtpinksred.Text), Convert.ToInt16(txtpinksgreen.Text), Convert.ToInt16(txtpinksblue.Text));
            }
            catch { }
        }



        // ERROR: Handles clauses are not supported in C#
        private void pnlpinkoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            if (Convert.ToInt32(txtpinksred.Text) > 255)
            {
                txtpinksred.Text = "255";
            }
            if (Convert.ToInt32(txtpinksblue.Text) > Convert.ToInt32(txtpinksred.Text) - 50)
            {
                txtpinksblue.Text = (Convert.ToInt32(txtpinksred.Text) - 50).ToString();
            }
            if (Convert.ToInt32(txtpinksgreen.Text) > Convert.ToInt32(txtpinksblue.Text))
            {
                txtpinksgreen.Text = txtpinksblue.Text;
            }
            pnlpinkcustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtpinksred.Text), Convert.ToInt16(txtpinksgreen.Text), Convert.ToInt16(txtpinksblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtanysred_TextChanged(object sender, EventArgs e)
        {
            // ERROR: Not supported in C#: OnErrorStatement
            try
            {
                pnlanycustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtanysred.Text), Convert.ToInt16(txtanysgreen.Text), Convert.ToInt16(txtanysblue.Text));
            }
            catch
            {

            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlanyoptions_MouseLeave(object sender, EventArgs e)
        {
            customcolourfailsafe();
            switch (anylevel)
            {
                case 1:
                    if (Convert.ToInt32(txtanysred.Text) > 150)
                    {
                        txtanysred.Text = "150";
                    }
                    if (Convert.ToInt32(txtanysred.Text) < 100)
                    {
                        txtanysred.Text = "100";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) > 150)
                    {
                        txtanysblue.Text = "150";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) < 100)
                    {
                        txtanysblue.Text = "100";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) > 150)
                    {
                        txtanysgreen.Text = "150";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) < 100)
                    {
                        txtanysgreen.Text = "100";
                    }
                    break;
                case 2:
                    if (Convert.ToInt32(txtanysred.Text) > 200)
                    {
                        txtanysred.Text = "200";
                    }
                    if (Convert.ToInt32(txtanysred.Text) < 100)
                    {
                        txtanysred.Text = "100";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) > 200)
                    {
                        txtanysblue.Text = "200";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) < 100)
                    {
                        txtanysblue.Text = "100";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) > 200)
                    {
                        txtanysgreen.Text = "200";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) < 100)
                    {
                        txtanysgreen.Text = "100";
                    }
                    break;
                case 3:
                    if (Convert.ToInt32(txtanysred.Text) > 225)
                    {
                        txtanysred.Text = "225";
                    }
                    if (Convert.ToInt32(txtanysred.Text) < 75)
                    {
                        txtanysred.Text = "75";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) > 225)
                    {
                        txtanysblue.Text = "225";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) < 75)
                    {
                        txtanysblue.Text = "75";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) > 225)
                    {
                        txtanysgreen.Text = "225";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) < 75)
                    {
                        txtanysgreen.Text = "75";
                    }
                    break;
                case 4:
                    if (Convert.ToInt32(txtanysred.Text) > 255)
                    {
                        txtanysred.Text = "255";
                    }
                    if (Convert.ToInt32(txtanysred.Text) < 0)
                    {
                        txtanysred.Text = "0";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) > 255)
                    {
                        txtanysblue.Text = "255";
                    }
                    if (Convert.ToInt32(txtanysblue.Text) < 0)
                    {
                        txtanysblue.Text = "0";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) > 255)
                    {
                        txtanysgreen.Text = "255";
                    }
                    if (Convert.ToInt32(txtanysgreen.Text) < 0)
                    {
                        txtanysgreen.Text = "0";
                    }
                    break;
            }
            pnlanycustomcolour.BackColor = Color.FromArgb(Convert.ToInt16(txtanysred.Text), Convert.ToInt16(txtanysgreen.Text), Convert.ToInt16(txtanysblue.Text));
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlgraycustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Gray Rules";
                //infobox.textinfo = "You must input a value between 0 (black) and 255 (white) to form a shade of gray." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Gray Memory Wiped";
                //infobox.textinfo = "All your custom shades of Gray have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.graymemory, 0, API.graymemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlpurplecustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Purple Rules";
                //infobox.textinfo = "Blue must have the highest value followed by red. Green must then have the lowest value." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Purple Memory Wiped";
                //infobox.textinfo = "All your custom shades of Purple have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.purplememory, 0, API.purplememory.Length);
                setupboughtcolours();
                loadmemory();
            }

        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlbluecustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Blue Rules";
                //infobox.textinfo = "Blue must have the highest value followed by green. Red must then have the lowest value." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Blue Memory Wiped";
                //infobox.textinfo = "All your custom shades of Blue have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.bluememory, 0, API.bluememory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlgreencustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Green Rules";
                //infobox.textinfo = "Green must have the highest value. Red and Blue need to have values within 150 of eachother." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Green Memory Wiped";
                //infobox.textinfo = "All your custom shades of Green have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.greenmemory, 0, API.greenmemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlyellowcustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Yellow Rules";
                //infobox.textinfo = "Red must have the highest value and be over 180. Green must be within 30 values of red. Blue must be the lowest value." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Yellow Memory Wiped";
                //infobox.textinfo = "All your custom shades of Yellow have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.yellowmemory, 0, API.yellowmemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlorangecustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Orange Rules";
                //infobox.textinfo = "Red must have a value of 255. Green must be 100 or more values less than red. Blue must be 30 or more values less than green." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Orange Memory Wiped";
                //infobox.textinfo = "All your custom shades of Orange have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.orangememory, 0, API.orangememory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlbrowncustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Brown Rules";
                //infobox.textinfo = "Red must have the highest value. Green must be 30 - 128 values lower than red. Blue must be 60 or more values less than green." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Brown Memory Wiped";
                //infobox.textinfo = "All your custom shades of Brown have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.brownmemory, 0, API.brownmemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlredcustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Red Rules";
                //infobox.textinfo = "Red must have the highest value. Green and blue must be 80 or more values less than red but within 50 values of eachother." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Red Memory Wiped";
                //infobox.textinfo = "All your custom shades of Red have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.redmemory, 0, API.redmemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlpinkcustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //infobox.title = "Pink Rules";
                //infobox.textinfo = "Red must have the highest value. Blue must be 50 or more values less than red. Green must have the lowest value." + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                //infobox.Show();
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Pink Memory Wiped";
                //infobox.textinfo = "All your custom shades of Pink have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.pinkmemory, 0, API.pinkmemory.Length);
                setupboughtcolours();
                loadmemory();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlanycustomcolour_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (anylevel)
                {
                    case 1:
                        //infobox.title = "Custom Rules";
                        //infobox.textinfo = "Red, Green and Blue may be set to any value between 100 and 150" + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                        //infobox.Show();
                        break;
                    case 2:
                        //infobox.title = "Custom Rules";
                        //infobox.textinfo = "Red, Green and Blue may be set to any value between 100 and 200" + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                        //infobox.Show();
                        break;
                    case 3:
                        //infobox.title = "Custom Rules";
                        //infobox.textinfo = "Red, Green and Blue may be set to any value between 75 and 225" + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                        //infobox.Show();
                        break;
                    case 4:
                        //infobox.title = "Custom Rules";
                        //infobox.textinfo = "Red, Green and Blue may be set to any value between 0 and 255" + Environment.NewLine + Environment.NewLine + "Right click a box on the left to use this colour.";
                        //infobox.Show();
                        break;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                //infobox.title = "Custom Colour Memory Wiped";
                //infobox.textinfo = "All your custom colour shades of have been wiped from memory." + Environment.NewLine + Environment.NewLine + "You can set custom colours but right clicking any of the boxes on the left.";
                //infobox.Show();
                Array.Clear(API.anymemory, 0, API.anymemory.Length);
                setupboughtcolours();
                loadmemory();
            }

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

        private void Label14_Click(object sender, EventArgs e)
        {

        }

        private void Label17_Click(object sender, EventArgs e)
        {

        }
    }
}
