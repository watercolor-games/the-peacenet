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

using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.WinForms.Tools;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Artpad", true, "al_artpad", "Graphics")]
    [RequiresUpgrade("artpad")]
    [WinOpen("artpad")]
    [DefaultIcon("iconArtpad")]
    public partial class Artpad : UserControl, IShiftOSWindow
    {
        /// <summary>
        /// artpad_'s GUI.
        /// </summary>
        public Artpad()
        {
            try
            {
                InitializeComponent();
                
            }
            catch (Exception ex)
            {
                CrashHandler.Start(ex);
                this.Close();
            }
        }

        public void LoadPicture(string pic)
        {
            savelocation = pic;
            this.openpic();
        }
        
        #region Variables

        public int rolldownsize;
        public int oldbordersize;
        public int oldtitlebarheight;
        public bool justopened = false;
        public bool needtorollback = false;
        public int minimumsizewidth = 502;

        public int minimumsizeheight = 398;
        public int codepointsearned;
        public bool codepointscooldown = false;

        public bool needtosave = false;
        int canvaswidth = 150;
        int canvasheight = 100;
        public Bitmap canvasbitmap;

        Color canvascolor = Color.White;

        Bitmap previewcanvasbitmap;
        int magnificationlevel = 1;
        Rectangle magnifyRect;
        Graphics graphicsbitmap;
        public Color drawingcolour = Color.Black;
        string selectedtool = "Pixel Setter";
        bool pixalplacermovable = false;
        public string savelocation;
        System.Drawing.Drawing2D.GraphicsPath mousePath = new System.Drawing.Drawing2D.GraphicsPath();
        int pencilwidth = 1;
        undo undo = new undo();
        Point thisPoint;
        int eracerwidth = 15;

        string eracertype = "square";
        int paintbrushwidth = 15;

        string paintbrushtype = "circle";
        float rectanglestartpointx;
        float rectanglestartpointy;
        bool currentlydrawingsquare;
        int squarewidth = 1;
        bool squarefillon = false;

        Color fillsquarecolor = Color.Black;
        float ovalstartpointx;
        float ovalstartpointy;
        bool currentlydrawingoval;
        int ovalwidth = 2;
        bool ovalfillon = false;

        Color fillovalcolor = Color.Black;
        float linestartpointx;
        float linestartpointy;
        bool currentlydrawingline;

        int linewidth = 2;
        bool currentlydrawingtext;
        System.Drawing.Font drawtextfont = new System.Drawing.Font("Microsoft Sans Serif", 16);
        int drawtextsize;
        string drawtextfontname;

        FontStyle drawtextfontstyle;

        #endregion

        #region Setup

        private void Template_Load(object sender, EventArgs e)
        {
            justopened = true;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;

            setuppreview();
            settoolcolours();
            loadcolors();
            AddFonts();
            setuptoolbox();
            determinevisiblepalettes();
            tmrsetupui.Start();
        }



        public void setupcanvas()
        {
            canvasbitmap = new Bitmap(canvaswidth, canvasheight);
            previewcanvasbitmap = new Bitmap(canvaswidth, canvasheight);
            picdrawingdisplay.Size = new Size(canvaswidth, canvasheight);
            picdrawingdisplay.Location = new Point((pnldrawingbackground.Width - canvaswidth) / 2, (pnldrawingbackground.Height - canvasheight) / 2);
            graphicsbitmap = Graphics.FromImage(canvasbitmap);
            SolidBrush canvasbrush = new SolidBrush(canvascolor);
            graphicsbitmap.FillRectangle(canvasbrush, 0, 0, canvaswidth, canvasheight);
            magnificationlevel = 1;
            lblzoomlevel.Text = magnificationlevel + "X";
            setuppreview();
            needtosave = false;
        }

        public void setuptoolbox()
        {
            btnpixelplacer.Hide();
            btnpencil.Hide();
            btnfloodfill.Hide();
            btnoval.Hide();
            btnsquare.Hide();
            btnlinetool.Hide();
            btnpaintbrush.Hide();
            btntexttool.Hide();
            btneracer.Hide();
            btnnew.Hide();
            btnopen.Hide();
            btnsave.Hide();
            btnundo.Hide();
            btnredo.Hide();
            btnpixelplacermovementmode.Hide();

            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_placer") == true)
                btnpixelplacer.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pencil") == true)
                btnpencil.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_fill_tool") == true)
                btnfloodfill.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_oval_tool") == true)
                btnoval.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_rectangle_tool") == true)
                btnsquare.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_line_tool") == true)
                btnlinetool.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_paintbrush") == true)
                btnpaintbrush.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_text_tool") == true)
                btntexttool.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_eraser") == true)
                btneracer.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_new") == true)
                btnnew.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_load") == true)
                btnopen.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_save") == true)
                btnsave.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_undo") == true)
                btnundo.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_redo") == true)
                btnredo.Show();
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pp_movement_mode") == true)
                btnpixelplacermovementmode.Show();

        }

        private void AddFonts()
        {
            // Get the installed fonts collection.
            InstalledFontCollection allFonts = new InstalledFontCollection();

            // Get an array of the system's font familiies.
            FontFamily[] fontFamilies = allFonts.Families;

            // Display the font families.
            foreach (FontFamily myFont in fontFamilies)
            {
                combodrawtextfont.Items.Add(myFont.Name);
            }
            //font_family

            combodrawtextfont.SelectedItem = combodrawtextfont.Items[1];
            combofontstyle.Text = "Regular";
            txtdrawtextsize.Text = "16";
        }

        #endregion

        #region General

        // ERROR: Handles clauses are not supported in C#
        private void btnpixelsetter_Click(object sender, EventArgs e)
        {
            selectedtool = "Pixel Setter";
            gettoolsettings(pnlpixelsettersettings);
        }

        private void gettoolsettings(Panel toolpanel)
        {
            //hide all properties panels
            pnlpixelsettersettings.Hide();
            pnlmagnifiersettings.Hide();

            //show chosen panel
            toolpanel.Dock = DockStyle.Fill;
            toolpanel.BringToFront();
            toolpanel.Show();

            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnpixelsettersetpixel_Click(object sender, EventArgs e)
        {
            try
            {
                undo.undoStack.Push((Image)canvasbitmap.Clone());
                undo.redoStack.Clear();
                canvasbitmap.SetPixel(Convert.ToInt32(txtpixelsetterxcoordinate.Text), Convert.ToInt32(txtpixelsetterycoordinate.Text), drawingcolour);
                picdrawingdisplay.Invalidate();
            }
            catch
            {
                Infobox.Show("{ARTPAD_PLACEMENT_ERROR}", "{ARTPAD_PLACEMENT_ERROR_EXP}");
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnmagnify_Click(object sender, EventArgs e)
        {
            selectedtool = "Magnifier";
            gettoolsettings(pnlmagnifiersettings);
        }

        #endregion

        #region Drawing Display

        // ERROR: Handles clauses are not supported in C#
        private void picdrawingdisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, canvaswidth * magnificationlevel, canvasheight * magnificationlevel);
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.ScaleTransform(magnificationlevel, magnificationlevel);

            if (currentlydrawingsquare == true)
            {
                GC.Collect();
                Graphics g = Graphics.FromImage(previewcanvasbitmap);
                previewcanvasbitmap = new Bitmap(canvasbitmap.Width, canvasbitmap.Height);
                g = Graphics.FromImage(previewcanvasbitmap);
                var CurrentPen = new Pen(Color.FromArgb(255, drawingcolour), squarewidth);
                var CurrentBrush = new SolidBrush(Color.FromArgb(255, fillsquarecolor));
                RectangleF rectdraw = new RectangleF(rectanglestartpointx, rectanglestartpointy, thisPoint.X - rectanglestartpointx, thisPoint.Y - rectanglestartpointy);
                int heightamount;
                int widthamount;
                if (rectdraw.Height < 0)
                    heightamount = (int)Math.Abs(rectdraw.Height);
                else
                    heightamount = 0;
                if (rectdraw.Width < 0)
                    widthamount = (int)Math.Abs(rectdraw.Width);
                else
                    widthamount = 0;
                if (squarewidth > 0)
                {
                    g.DrawRectangle(CurrentPen, rectdraw.X - widthamount, rectdraw.Y - heightamount, Math.Abs(rectdraw.Width), Math.Abs(rectdraw.Height));
                }
                if (squarefillon == true)
                {
                    float correctionamount = squarewidth / 2;
                    int addfillamount;
                    if (squarewidth > 0)
                        addfillamount = squarewidth;
                    else
                        addfillamount = -1;
                    g.FillRectangle(CurrentBrush, (rectdraw.X - widthamount) + correctionamount, (rectdraw.Y - heightamount) + correctionamount, Math.Abs(rectdraw.Width) - addfillamount, Math.Abs(rectdraw.Height) - addfillamount);
                }
            }

            if (currentlydrawingoval == true)
            {
                GC.Collect();
                Graphics g = Graphics.FromImage(previewcanvasbitmap);
                previewcanvasbitmap = new Bitmap(canvasbitmap.Width, canvasbitmap.Height);
                g = Graphics.FromImage(previewcanvasbitmap);
                var CurrentPen = new Pen(Color.FromArgb(255, drawingcolour), ovalwidth);
                var CurrentBrush = new SolidBrush(Color.FromArgb(255, fillovalcolor));
                RectangleF ovaldraw = new RectangleF(ovalstartpointx, ovalstartpointy, thisPoint.X - ovalstartpointx, thisPoint.Y - ovalstartpointy);
                int heightamount;
                int widthamount;
                if (ovaldraw.Height < 0)
                    heightamount = (int)Math.Abs(ovaldraw.Height);
                else
                    heightamount = 0;
                if (ovaldraw.Width < 0)
                    widthamount = (int)Math.Abs(ovaldraw.Width);
                else
                    widthamount = 0;
                if (ovalwidth > 0)
                {
                    g.DrawEllipse(CurrentPen, ovaldraw.X - widthamount, ovaldraw.Y - heightamount, Math.Abs(ovaldraw.Width), Math.Abs(ovaldraw.Height));
                }
                if (ovalfillon == true)
                {
                    g.FillEllipse(CurrentBrush, (ovaldraw.X - widthamount), (ovaldraw.Y - heightamount), Math.Abs(ovaldraw.Width), Math.Abs(ovaldraw.Height));
                }
            }

            if (currentlydrawingline == true)
            {
                GC.Collect();
                Graphics g = Graphics.FromImage(previewcanvasbitmap);
                previewcanvasbitmap = new Bitmap(canvasbitmap.Width, canvasbitmap.Height);
                g = Graphics.FromImage(previewcanvasbitmap);
                var CurrentPen = new Pen(Color.FromArgb(255, drawingcolour), linewidth);
                g.DrawLine(CurrentPen, linestartpointx, linestartpointy, thisPoint.X, thisPoint.Y);
            }

            if (currentlydrawingtext == true)
            {
                GC.Collect();
                Graphics g = Graphics.FromImage(previewcanvasbitmap);
                previewcanvasbitmap = new Bitmap(canvasbitmap.Width, canvasbitmap.Height);
                g = Graphics.FromImage(previewcanvasbitmap);
                var CurrentBrush = new SolidBrush(Color.FromArgb(255, drawingcolour));
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                drawtextfont = new System.Drawing.Font(drawtextfontname, drawtextsize, drawtextfontstyle);
                g.DrawString(txtdrawstringtext.Text, drawtextfont, CurrentBrush, thisPoint.X, thisPoint.Y);
            }
            try
            {
                e.Graphics.DrawImage(canvasbitmap, 0, 0);
                e.Graphics.DrawImage(previewcanvasbitmap, 0, 0);
            }
            catch
            {

            }
        }



        // ERROR: Handles clauses are not supported in C#
        private void picdrawingdisplay_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            undo.undoStack.Push((Image)canvasbitmap.Clone());
            undo.redoStack.Clear();


            thisPoint.X = (int)(e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (int)(e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;

            if (selectedtool == "Pixel Placer")
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (thisPoint.X < canvasbitmap.Width && thisPoint.X > -1)
                    {
                        if (thisPoint.Y < canvasbitmap.Height && thisPoint.Y > -1)
                        {
                            canvasbitmap.SetPixel(thisPoint.X, thisPoint.Y, drawingcolour);
                            //set the pixel on the canvas
                            picdrawingdisplay.Invalidate();
                            //refresh the picture from the canvas
                        }
                    }
                }
            }

            if (selectedtool == "Pencil")
            {
                if (e.Button == MouseButtons.Left)
                {
                    mousePath.StartFigure();
                    picdrawingdisplay.Invalidate();
                }
            }

            if (selectedtool == "Flood Fill")
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (thisPoint.X < canvasbitmap.Width && thisPoint.X > -1)
                    {
                        if (thisPoint.Y < canvasbitmap.Height && thisPoint.Y > -1)
                        {
                            SafeFloodFill(canvasbitmap, thisPoint.X, thisPoint.Y, drawingcolour);
                            graphicsbitmap = Graphics.FromImage(canvasbitmap);
                            picdrawingdisplay.Invalidate();
                        }
                    }
                }
            }

            if (selectedtool == "Square _tool")
            {
                if (e.Button == MouseButtons.Left)
                {
                    rectanglestartpointx = thisPoint.X;
                    rectanglestartpointy = thisPoint.Y;
                    currentlydrawingsquare = true;
                    picdrawingdisplay.Invalidate();
                }
            }

            if (selectedtool == "Oval _tool")
            {
                if (e.Button == MouseButtons.Left)
                {
                    ovalstartpointx = thisPoint.X;
                    ovalstartpointy = thisPoint.Y;
                    currentlydrawingoval = true;
                    picdrawingdisplay.Invalidate();
                }
            }

            if (selectedtool == "Line _tool")
            {
                if (e.Button == MouseButtons.Left)
                {
                    linestartpointx = thisPoint.X;
                    linestartpointy = thisPoint.Y;
                    currentlydrawingline = true;
                    picdrawingdisplay.Invalidate();
                }
            }

            if (selectedtool == "Text _tool")
            {
                if (e.Button == MouseButtons.Left)
                {
                    currentlydrawingtext = true;
                    picdrawingdisplay.Invalidate();
                }
            }

            if (selectedtool == "Eracer")
            {
                var CurrentPen = new Pen(Color.FromArgb(255, canvascolor), eracerwidth);
                float halfsize = eracerwidth / 2;
                if (eracertype == "circle")
                {
                    graphicsbitmap.DrawEllipse(CurrentPen, thisPoint.X - halfsize, thisPoint.Y - halfsize, eracerwidth, eracerwidth);
                }
                else
                {
                    graphicsbitmap.DrawRectangle(CurrentPen, thisPoint.X - halfsize, thisPoint.Y - halfsize, eracerwidth, eracerwidth);
                }
                picdrawingdisplay.Invalidate();
            }

            if (selectedtool == "Paint Brush")
            {
                var CurrentBrush = new SolidBrush(Color.FromArgb(255, drawingcolour));
                float halfsize = paintbrushwidth / 2;
                if (paintbrushtype == "circle")
                {
                    graphicsbitmap.FillEllipse(CurrentBrush, thisPoint.X - halfsize, thisPoint.Y - halfsize, paintbrushwidth, paintbrushwidth);
                }
                else
                {
                    graphicsbitmap.FillRectangle(CurrentBrush, thisPoint.X - halfsize, thisPoint.Y - halfsize, paintbrushwidth, paintbrushwidth);
                }
                picdrawingdisplay.Invalidate();
                CurrentBrush.Dispose();
            }
            preparecooldown();
        }

        // ERROR: Handles clauses are not supported in C#
        private void picdrawingdisplay_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point lastpoint;

            thisPoint.X = (int)(e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (int)(e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;


            if (e.Button == MouseButtons.Left)
            {
                undo.redoStack.Clear();
                lastpoint = thisPoint;
                preparecooldown();

                if (selectedtool == "Pixel Placer" && pixalplacermovable == true)
                {
                    if (thisPoint.X < canvasbitmap.Width && thisPoint.X > -1)
                    {
                        if (thisPoint.Y < canvasbitmap.Height && thisPoint.Y > -1)
                        {
                            canvasbitmap.SetPixel(thisPoint.X, thisPoint.Y, drawingcolour);
                            //set the pixel on the canvas
                            picdrawingdisplay.Invalidate();
                            //refresh the picture from the canvas
                        }
                    }
                }

                if (selectedtool == "Pencil")
                {
                    mousePath.AddLine(thisPoint.X, thisPoint.Y, thisPoint.X, thisPoint.Y);
                    var CurrentPen = new Pen(Color.FromArgb(255, drawingcolour), pencilwidth);
                    graphicsbitmap.DrawPath(CurrentPen, mousePath);
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Square _tool")
                {
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Oval _tool")
                {
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Line _tool")
                {
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Text _tool")
                {
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Eracer")
                {
                    var CurrentPen = new Pen(Color.FromArgb(255, canvascolor), eracerwidth);
                    float halfsize = eracerwidth / 2;
                    if (eracertype == "circle")
                    {
                        graphicsbitmap.DrawEllipse(CurrentPen, thisPoint.X - halfsize, thisPoint.Y - halfsize, eracerwidth, eracerwidth);
                    }
                    else
                    {
                        graphicsbitmap.DrawRectangle(CurrentPen, thisPoint.X - halfsize, thisPoint.Y - halfsize, eracerwidth, eracerwidth);
                    }
                    picdrawingdisplay.Invalidate();
                }

                if (selectedtool == "Paint Brush")
                {
                    var CurrentBrush = new SolidBrush(Color.FromArgb(255, drawingcolour));
                    float halfsize = paintbrushwidth / 2;
                    if (paintbrushtype == "circle")
                    {
                        graphicsbitmap.FillEllipse(CurrentBrush, thisPoint.X - halfsize, thisPoint.Y - halfsize, paintbrushwidth, paintbrushwidth);
                    }
                    else
                    {
                        graphicsbitmap.FillRectangle(CurrentBrush, thisPoint.X - halfsize, thisPoint.Y - halfsize, paintbrushwidth, paintbrushwidth);
                    }
                    picdrawingdisplay.Invalidate();
                }
            }

        }


        // ERROR: Handles clauses are not supported in C#
        private void picdrawingdisplay_MouseUp(object sender, MouseEventArgs e)
        {
            thisPoint.X = (int)(e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (int)(e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;

            if (selectedtool == "Pencil")
            {
                if (e.Button == MouseButtons.Left)
                {
                    mousePath.Reset();
                }
            }

            if (selectedtool == "Square _tool")
            {
                picdrawingdisplay.Invalidate();
                currentlydrawingsquare = false;
            }

            if (selectedtool == "Oval _tool")
            {
                picdrawingdisplay.Invalidate();
                currentlydrawingoval = false;
            }


            if (selectedtool == "Line _tool")
            {
                picdrawingdisplay.Invalidate();
                currentlydrawingline = false;
            }

            if (selectedtool == "Text _tool")
            {
                picdrawingdisplay.Invalidate();
                currentlydrawingtext = false;
            }

            using (Graphics g = Graphics.FromImage(canvasbitmap))
            {
                g.DrawImage(previewcanvasbitmap, 0, 0);
            }
            previewcanvasbitmap = new Bitmap(canvasbitmap.Width, canvasbitmap.Height);
            picdrawingdisplay.Invalidate();
            preparecooldown();

        }

        #endregion

        #region Color Palettes
        // ERROR: Handles clauses are not supported in C#
        private void colourpalette1_MouseClick(object sender, MouseEventArgs e)
        {
            var s = (Control)sender;
            if (e.Button == MouseButtons.Left)
            {
                drawingcolour = s.BackColor;
                setuppreview();
                settoolcolours();
            }
            else
            {
                AppearanceManager.SetupDialog(new ColorPicker(s.BackColor, "artpad_ Color", new Action<Color>((color) => {
                    s.BackColor = color;
                })));
            }
        }

        //<unused>
        public void loadcolors()
        {
            /*bool allwhite = true;
            for (int i = 0; i <= 127; i++)
            {
                if (ShiftOSDesktop.artpad_colour_palettes(i) == null)
                {
                }
                else {
                    allwhite = false;
                }
            }
            if (allwhite == true)
            {
                for (i = 0; i <= 127; i++)
                {
                    ShiftOSDesktop.artpad_colour_palettes(i) = Color.Black;
                }
            }
            colourpalette1.BackColor = ShiftOSDesktop.artpad_colour_palettes(0);
            colourpalette2.BackColor = ShiftOSDesktop.artpad_colour_palettes(1);
            colourpalette3.BackColor = ShiftOSDesktop.artpad_colour_palettes(2);
            colourpalette4.BackColor = ShiftOSDesktop.artpad_colour_palettes(3);
            colourpalette5.BackColor = ShiftOSDesktop.artpad_colour_palettes(4);
            colourpalette6.BackColor = ShiftOSDesktop.artpad_colour_palettes(5);
            colourpalette7.BackColor = ShiftOSDesktop.artpad_colour_palettes(6);
            colourpalette8.BackColor = ShiftOSDesktop.artpad_colour_palettes(7);
            colourpalette9.BackColor = ShiftOSDesktop.artpad_colour_palettes(8);
            colourpalette10.BackColor = ShiftOSDesktop.artpad_colour_palettes(9);
            colourpalette11.BackColor = ShiftOSDesktop.artpad_colour_palettes(10);
            colourpalette12.BackColor = ShiftOSDesktop.artpad_colour_palettes(11);
            colourpalette13.BackColor = ShiftOSDesktop.artpad_colour_palettes(12);
            colourpalette14.BackColor = ShiftOSDesktop.artpad_colour_palettes(13);
            colourpalette15.BackColor = ShiftOSDesktop.artpad_colour_palettes(14);
            colourpalette16.BackColor = ShiftOSDesktop.artpad_colour_palettes(15);
            colourpalette17.BackColor = ShiftOSDesktop.artpad_colour_palettes(16);
            colourpalette18.BackColor = ShiftOSDesktop.artpad_colour_palettes(17);
            colourpalette19.BackColor = ShiftOSDesktop.artpad_colour_palettes(18);
            colourpalette20.BackColor = ShiftOSDesktop.artpad_colour_palettes(19);
            colourpalette21.BackColor = ShiftOSDesktop.artpad_colour_palettes(20);
            colourpalette22.BackColor = ShiftOSDesktop.artpad_colour_palettes(21);
            colourpalette23.BackColor = ShiftOSDesktop.artpad_colour_palettes(22);
            colourpalette24.BackColor = ShiftOSDesktop.artpad_colour_palettes(23);
            colourpalette25.BackColor = ShiftOSDesktop.artpad_colour_palettes(24);
            colourpalette26.BackColor = ShiftOSDesktop.artpad_colour_palettes(25);
            colourpalette27.BackColor = ShiftOSDesktop.artpad_colour_palettes(26);
            colourpalette28.BackColor = ShiftOSDesktop.artpad_colour_palettes(27);
            colourpalette29.BackColor = ShiftOSDesktop.artpad_colour_palettes(28);
            colourpalette30.BackColor = ShiftOSDesktop.artpad_colour_palettes(29);
            colourpalette31.BackColor = ShiftOSDesktop.artpad_colour_palettes(30);
            colourpalette32.BackColor = ShiftOSDesktop.artpad_colour_palettes(31);
            colourpalette33.BackColor = ShiftOSDesktop.artpad_colour_palettes(32);
            colourpalette34.BackColor = ShiftOSDesktop.artpad_colour_palettes(33);
            colourpalette35.BackColor = ShiftOSDesktop.artpad_colour_palettes(34);
            colourpalette36.BackColor = ShiftOSDesktop.artpad_colour_palettes(35);
            colourpalette37.BackColor = ShiftOSDesktop.artpad_colour_palettes(36);
            colourpalette38.BackColor = ShiftOSDesktop.artpad_colour_palettes(37);
            colourpalette39.BackColor = ShiftOSDesktop.artpad_colour_palettes(38);
            colourpalette40.BackColor = ShiftOSDesktop.artpad_colour_palettes(39);
            colourpalette41.BackColor = ShiftOSDesktop.artpad_colour_palettes(40);
            colourpalette42.BackColor = ShiftOSDesktop.artpad_colour_palettes(41);
            colourpalette43.BackColor = ShiftOSDesktop.artpad_colour_palettes(42);
            colourpalette44.BackColor = ShiftOSDesktop.artpad_colour_palettes(43);
            colourpalette45.BackColor = ShiftOSDesktop.artpad_colour_palettes(44);
            colourpalette46.BackColor = ShiftOSDesktop.artpad_colour_palettes(45);
            colourpalette47.BackColor = ShiftOSDesktop.artpad_colour_palettes(46);
            colourpalette48.BackColor = ShiftOSDesktop.artpad_colour_palettes(47);
            colourpalette49.BackColor = ShiftOSDesktop.artpad_colour_palettes(48);
            colourpalette50.BackColor = ShiftOSDesktop.artpad_colour_palettes(49);
            colourpalette51.BackColor = ShiftOSDesktop.artpad_colour_palettes(50);
            colourpalette52.BackColor = ShiftOSDesktop.artpad_colour_palettes(51);
            colourpalette53.BackColor = ShiftOSDesktop.artpad_colour_palettes(52);
            colourpalette54.BackColor = ShiftOSDesktop.artpad_colour_palettes(53);
            colourpalette55.BackColor = ShiftOSDesktop.artpad_colour_palettes(54);
            colourpalette56.BackColor = ShiftOSDesktop.artpad_colour_palettes(55);
            colourpalette57.BackColor = ShiftOSDesktop.artpad_colour_palettes(56);
            colourpalette58.BackColor = ShiftOSDesktop.artpad_colour_palettes(57);
            colourpalette59.BackColor = ShiftOSDesktop.artpad_colour_palettes(58);
            colourpalette60.BackColor = ShiftOSDesktop.artpad_colour_palettes(59);
            colourpalette61.BackColor = ShiftOSDesktop.artpad_colour_palettes(60);
            colourpalette62.BackColor = ShiftOSDesktop.artpad_colour_palettes(61);
            colourpalette63.BackColor = ShiftOSDesktop.artpad_colour_palettes(62);
            colourpalette64.BackColor = ShiftOSDesktop.artpad_colour_palettes(63);
            colourpalette65.BackColor = ShiftOSDesktop.artpad_colour_palettes(64);
            colourpalette66.BackColor = ShiftOSDesktop.artpad_colour_palettes(65);
            colourpalette67.BackColor = ShiftOSDesktop.artpad_colour_palettes(66);
            colourpalette68.BackColor = ShiftOSDesktop.artpad_colour_palettes(67);
            colourpalette69.BackColor = ShiftOSDesktop.artpad_colour_palettes(68);
            colourpalette70.BackColor = ShiftOSDesktop.artpad_colour_palettes(69);
            colourpalette71.BackColor = ShiftOSDesktop.artpad_colour_palettes(70);
            colourpalette72.BackColor = ShiftOSDesktop.artpad_colour_palettes(71);
            colourpalette73.BackColor = ShiftOSDesktop.artpad_colour_palettes(72);
            colourpalette74.BackColor = ShiftOSDesktop.artpad_colour_palettes(73);
            colourpalette75.BackColor = ShiftOSDesktop.artpad_colour_palettes(74);
            colourpalette76.BackColor = ShiftOSDesktop.artpad_colour_palettes(75);
            colourpalette77.BackColor = ShiftOSDesktop.artpad_colour_palettes(76);
            colourpalette78.BackColor = ShiftOSDesktop.artpad_colour_palettes(77);
            colourpalette79.BackColor = ShiftOSDesktop.artpad_colour_palettes(78);
            colourpalette80.BackColor = ShiftOSDesktop.artpad_colour_palettes(79);
            colourpalette81.BackColor = ShiftOSDesktop.artpad_colour_palettes(80);
            colourpalette82.BackColor = ShiftOSDesktop.artpad_colour_palettes(81);
            colourpalette83.BackColor = ShiftOSDesktop.artpad_colour_palettes(82);
            colourpalette84.BackColor = ShiftOSDesktop.artpad_colour_palettes(83);
            colourpalette85.BackColor = ShiftOSDesktop.artpad_colour_palettes(84);
            colourpalette86.BackColor = ShiftOSDesktop.artpad_colour_palettes(85);
            colourpalette87.BackColor = ShiftOSDesktop.artpad_colour_palettes(86);
            colourpalette88.BackColor = ShiftOSDesktop.artpad_colour_palettes(87);
            colourpalette89.BackColor = ShiftOSDesktop.artpad_colour_palettes(88);
            colourpalette90.BackColor = ShiftOSDesktop.artpad_colour_palettes(89);
            colourpalette91.BackColor = ShiftOSDesktop.artpad_colour_palettes(90);
            colourpalette92.BackColor = ShiftOSDesktop.artpad_colour_palettes(91);
            colourpalette93.BackColor = ShiftOSDesktop.artpad_colour_palettes(92);
            colourpalette94.BackColor = ShiftOSDesktop.artpad_colour_palettes(93);
            colourpalette95.BackColor = ShiftOSDesktop.artpad_colour_palettes(94);
            colourpalette96.BackColor = ShiftOSDesktop.artpad_colour_palettes(95);
            colourpalette97.BackColor = ShiftOSDesktop.artpad_colour_palettes(96);
            colourpalette98.BackColor = ShiftOSDesktop.artpad_colour_palettes(97);
            colourpalette99.BackColor = ShiftOSDesktop.artpad_colour_palettes(98);
            colourpalette100.BackColor = ShiftOSDesktop.artpad_colour_palettes(99);
            colourpalette101.BackColor = ShiftOSDesktop.artpad_colour_palettes(100);
            colourpalette102.BackColor = ShiftOSDesktop.artpad_colour_palettes(101);
            colourpalette103.BackColor = ShiftOSDesktop.artpad_colour_palettes(102);
            colourpalette104.BackColor = ShiftOSDesktop.artpad_colour_palettes(103);
            colourpalette105.BackColor = ShiftOSDesktop.artpad_colour_palettes(104);
            colourpalette106.BackColor = ShiftOSDesktop.artpad_colour_palettes(105);
            colourpalette107.BackColor = ShiftOSDesktop.artpad_colour_palettes(106);
            colourpalette108.BackColor = ShiftOSDesktop.artpad_colour_palettes(107);
            colourpalette109.BackColor = ShiftOSDesktop.artpad_colour_palettes(108);
            colourpalette110.BackColor = ShiftOSDesktop.artpad_colour_palettes(109);
            colourpalette111.BackColor = ShiftOSDesktop.artpad_colour_palettes(110);
            colourpalette112.BackColor = ShiftOSDesktop.artpad_colour_palettes(111);
            colourpalette113.BackColor = ShiftOSDesktop.artpad_colour_palettes(112);
            colourpalette114.BackColor = ShiftOSDesktop.artpad_colour_palettes(113);
            colourpalette115.BackColor = ShiftOSDesktop.artpad_colour_palettes(114);
            colourpalette116.BackColor = ShiftOSDesktop.artpad_colour_palettes(115);
            colourpalette117.BackColor = ShiftOSDesktop.artpad_colour_palettes(116);
            colourpalette118.BackColor = ShiftOSDesktop.artpad_colour_palettes(117);
            colourpalette119.BackColor = ShiftOSDesktop.artpad_colour_palettes(118);
            colourpalette120.BackColor = ShiftOSDesktop.artpad_colour_palettes(119);
            colourpalette121.BackColor = ShiftOSDesktop.artpad_colour_palettes(120);
            colourpalette122.BackColor = ShiftOSDesktop.artpad_colour_palettes(121);
            colourpalette123.BackColor = ShiftOSDesktop.artpad_colour_palettes(122);
            colourpalette124.BackColor = ShiftOSDesktop.artpad_colour_palettes(123);
            colourpalette125.BackColor = ShiftOSDesktop.artpad_colour_palettes(124);
            colourpalette126.BackColor = ShiftOSDesktop.artpad_colour_palettes(125);
            colourpalette127.BackColor = ShiftOSDesktop.artpad_colour_palettes(126);
            colourpalette128.BackColor = ShiftOSDesktop.artpad_colour_palettes(127);
        */
        }

        public void savecolors()
        {/*
            ShiftOSDesktop.artpad_colour_palettes(0) = colourpalette1.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(1) = colourpalette2.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(2) = colourpalette3.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(3) = colourpalette4.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(4) = colourpalette5.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(5) = colourpalette6.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(6) = colourpalette7.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(7) = colourpalette8.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(8) = colourpalette9.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(9) = colourpalette10.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(10) = colourpalette11.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(11) = colourpalette12.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(12) = colourpalette13.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(13) = colourpalette14.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(14) = colourpalette15.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(15) = colourpalette16.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(16) = colourpalette17.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(17) = colourpalette18.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(18) = colourpalette19.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(19) = colourpalette20.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(20) = colourpalette21.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(21) = colourpalette22.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(22) = colourpalette23.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(23) = colourpalette24.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(24) = colourpalette25.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(25) = colourpalette26.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(26) = colourpalette27.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(27) = colourpalette28.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(28) = colourpalette29.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(29) = colourpalette30.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(30) = colourpalette31.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(31) = colourpalette32.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(32) = colourpalette33.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(33) = colourpalette34.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(34) = colourpalette35.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(35) = colourpalette36.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(36) = colourpalette37.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(37) = colourpalette38.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(38) = colourpalette39.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(39) = colourpalette40.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(40) = colourpalette41.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(41) = colourpalette42.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(42) = colourpalette43.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(43) = colourpalette44.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(44) = colourpalette45.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(45) = colourpalette46.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(46) = colourpalette47.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(47) = colourpalette48.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(48) = colourpalette49.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(49) = colourpalette50.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(50) = colourpalette51.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(51) = colourpalette52.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(52) = colourpalette53.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(53) = colourpalette54.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(54) = colourpalette55.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(55) = colourpalette56.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(56) = colourpalette57.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(57) = colourpalette58.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(58) = colourpalette59.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(59) = colourpalette60.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(60) = colourpalette61.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(61) = colourpalette62.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(62) = colourpalette63.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(63) = colourpalette64.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(64) = colourpalette65.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(65) = colourpalette66.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(66) = colourpalette67.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(67) = colourpalette68.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(68) = colourpalette69.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(69) = colourpalette70.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(70) = colourpalette71.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(71) = colourpalette72.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(72) = colourpalette73.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(73) = colourpalette74.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(74) = colourpalette75.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(75) = colourpalette76.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(76) = colourpalette77.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(77) = colourpalette78.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(78) = colourpalette79.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(79) = colourpalette80.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(80) = colourpalette81.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(81) = colourpalette82.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(82) = colourpalette83.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(83) = colourpalette84.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(84) = colourpalette85.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(85) = colourpalette86.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(86) = colourpalette87.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(87) = colourpalette88.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(88) = colourpalette89.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(89) = colourpalette90.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(90) = colourpalette91.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(91) = colourpalette92.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(92) = colourpalette93.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(93) = colourpalette94.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(94) = colourpalette95.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(95) = colourpalette96.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(96) = colourpalette97.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(97) = colourpalette98.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(98) = colourpalette99.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(99) = colourpalette100.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(100) = colourpalette101.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(101) = colourpalette102.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(102) = colourpalette103.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(103) = colourpalette104.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(104) = colourpalette105.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(105) = colourpalette106.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(106) = colourpalette107.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(107) = colourpalette108.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(108) = colourpalette109.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(109) = colourpalette110.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(110) = colourpalette111.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(111) = colourpalette112.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(112) = colourpalette113.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(113) = colourpalette114.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(114) = colourpalette115.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(115) = colourpalette116.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(116) = colourpalette117.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(117) = colourpalette118.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(118) = colourpalette119.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(119) = colourpalette120.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(120) = colourpalette121.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(121) = colourpalette122.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(122) = colourpalette123.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(123) = colourpalette124.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(124) = colourpalette125.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(125) = colourpalette126.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(126) = colourpalette127.BackColor;
            ShiftOSDesktop.artpad_colour_palettes(127) = colourpalette128.BackColor;
        */
        }
        //</unused>

        public void settoolcolours()
        {
            btnpixelsetter.BackColor = drawingcolour;
            btnpixelplacer.BackColor = drawingcolour;
            btnpencil.BackColor = drawingcolour;
            btnfloodfill.BackColor = drawingcolour;
            btnsquare.BackColor = drawingcolour;
            btnoval.BackColor = drawingcolour;
            btnlinetool.BackColor = drawingcolour;
            btnpaintbrush.BackColor = drawingcolour;
            btntexttool.BackColor = drawingcolour;
        }
        #endregion

        #region Zooming

        // ERROR: Handles clauses are not supported in C#
        private void btnzoomin_Click(object sender, EventArgs e)
        {
            if (magnificationlevel < 256)
            {
                magnificationlevel *= 2;
            }
            else
            {
                Infobox.Show("{ARTPAD_MAGNIFIER_ERROR}"
                , "{ARTPAD_MAGNIFICATION_ERROR_EXP}");
            }
            setmagnification();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnzoomout_Click(object sender, EventArgs e)
        {
            if (magnificationlevel > 1)
            {
                magnificationlevel /= 2;
                pnldrawingbackground.AutoScrollPosition = new Point(0, 0);
            }
            else
            {
                Infobox.Show("{ARTPAD_MAGNIFIER_ERROR}"
                , "{ARTPAD_MAGNIFICATION_ERROR_EXP_2}");
            }
            setmagnification();
        }

        private void setmagnification()
        {
            magnifyRect.Width = (int)canvaswidth / magnificationlevel;
            magnifyRect.Height = (int)canvasheight / magnificationlevel;
            picdrawingdisplay.Size = new Size(canvaswidth * magnificationlevel, canvasheight * magnificationlevel);
            if (picdrawingdisplay.Height > 468 && picdrawingdisplay.Width > 676)
            {
                picdrawingdisplay.Location = new Point(0, 0);
            }
            else
            {
                picdrawingdisplay.Location = new Point((pnldrawingbackground.Width - canvaswidth * magnificationlevel) / 2, (pnldrawingbackground.Height - canvasheight * magnificationlevel) / 2);
            }
            picdrawingdisplay.Invalidate();
            lblzoomlevel.Text = magnificationlevel + "X";
        }

        #endregion

        #region Pixel Placer

        // ERROR: Handles clauses are not supported in C#
        private void pnlpixelplacer_Click(object sender, EventArgs e)
        {
            selectedtool = "Pixel Placer";
            gettoolsettings(pnlpixelplacersettings);
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnpixelplacermovementmode_Click(object sender, EventArgs e)
        {
            if (pixalplacermovable == false)
            {
                pixalplacermovable = true;
                btnpixelplacermovementmode.ForeColor = Color.White;
                btnpixelplacermovementmode.BackColor = Color.Black;
                btnpixelplacermovementmode.Text = "Deactivate Movement Mode";
                lblpixelplacerhelp.Text = "Movement mode is enabled. Click and drag on the canvas to place pixels as you move the mouse. Please use 4x magnification or greater and move the mouse very slowly.";
            }
            else
            {
                pixalplacermovable = false;
                btnpixelplacermovementmode.ForeColor = Color.Black;
                btnpixelplacermovementmode.BackColor = Color.White;
                btnpixelplacermovementmode.Text = "Activate Movement Mode";
                lblpixelplacerhelp.Text = "This tool does not contain any alterable settings. Simply click on the canvas and a pixel will be placed in the spot you click.";
            }
        }

        #endregion

        #region Saving

        // ERROR: Handles clauses are not supported in C#
        private void btnsave_Click(object sender, EventArgs e)
        {
            showsavedialog();
        }

        public void showsavedialog()
        {
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".pic" }, FileOpenerStyle.Save, new Action<string>((file) =>
            {
                var res = file;
                savelocation = res;
                saveimage();
            })));
        }

        public void saveimage()
        {
            var ms = new MemoryStream();
            canvasbitmap.Save(ms, ImageFormat.Bmp);
            Utils.WriteAllBytes(this.savelocation, ms.ToArray());
            ms.Close();
        }
        #endregion

        #region New Canvas

        // ERROR: Handles clauses are not supported in C#
        private void txtnewcanvaswidth_TextChanged(object sender, EventArgs e)
        {
            if (txtnewcanvaswidth.Text == "" | txtnewcanvasheight.Text == "")
            {
                if (txtnewcanvasheight.Text == "")
                {
                    txtnewcanvasheight.Text = "0";
                }
                if (txtnewcanvaswidth.Text == "")
                {
                    txtnewcanvaswidth.Text = "0";
                }
            }
            else
            {
                try
                {
                    lbltotalpixels.Text = (Convert.ToInt32(txtnewcanvaswidth.Text) * Convert.ToInt32(txtnewcanvasheight.Text)).ToString();
                    if (ShiftoriumFrontend.UpgradeInstalled("artpad_limitless_pixels") == true)
                    {
                        lbltotalpixels.ForeColor = Color.Black;
                    }
                    else
                    {
                        if ((Convert.ToInt32(txtnewcanvaswidth.Text) * Convert.ToInt32(txtnewcanvasheight.Text)) > GetPixelLimit())
                        {
                            lbltotalpixels.ForeColor = Color.Red;
                        }
                        else
                        {
                            lbltotalpixels.ForeColor = Color.Black;
                        }
                    }
                }
                catch
                {

                }
            }

        }

        public int GetPixelLimit()
        {
            int value = 2;


            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_4"))
            {
                value = 4;
                if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_8"))
                {
                    value = 8;
                    if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_16"))
                    {
                        value = 16;
                        if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_64"))
                        {
                            value = 64;
                            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_256"))
                            {
                                value = 256;
                                if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_1024"))
                                {
                                    value = 1024;
                                    if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_4096"))
                                    {
                                        value = 4096;
                                        if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_16384"))
                                        {
                                            value = 16384;
                                            if (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_limit_65536"))
                                            {
                                                value = 65536;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        // ERROR: Handles clauses are not supported in C#
        private void btncreate_Click(object sender, EventArgs e)
        {
            if (lbltotalpixels.ForeColor == Color.Red)
            {
                Infobox.Show("{ARTPAD_ERROR}", "{ARTPAD_IMAGE_TOO_BIG}");
            }
            else
            {
                if (lbltotalpixels.Text == "0")
                {
                }
                else
                {
                    canvaswidth = Convert.ToInt32(txtnewcanvaswidth.Text);
                    canvasheight = Convert.ToInt32(txtnewcanvasheight.Text);
                    picdrawingdisplay.Show();
                    setupcanvas();
                    pnlinitialcanvassettings.Hide();
                }
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btncancel_Click(object sender, EventArgs e)
        {
            pnlinitialcanvassettings.Hide();
            picdrawingdisplay.Hide();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnnew_Click(object sender, EventArgs e)
        {
            pnlinitialcanvassettings.Show();
            picdrawingdisplay.Hide();
        }

        #endregion

        #region Preview

        public void setuppreview()
        {
            lbltoolselected.Text = selectedtool;
            picpreview.CreateGraphics().FillRectangle(Brushes.White, 0, 0, 70, 50);
            switch (selectedtool)
            {
                case "Square _tool":
                    var CurrentPen = new Pen(Color.FromArgb(255, drawingcolour), squarewidth);
                    var CurrentBrush = new SolidBrush(Color.FromArgb(255, fillsquarecolor));
                    RectangleF rectdraw = new RectangleF(0, 0, picpreview.Width, picpreview.Height);
                    float correctionamount = squarewidth / 2;
                    if (squarewidth > 0)
                    {
                        picpreview.CreateGraphics().DrawRectangle(CurrentPen, rectdraw.X + correctionamount, rectdraw.Y + correctionamount, rectdraw.Width - squarewidth, rectdraw.Height - squarewidth);
                    }
                    if (squarefillon == true)
                    {
                        picpreview.CreateGraphics().FillRectangle(CurrentBrush, rectdraw.X + squarewidth, rectdraw.Y + squarewidth, rectdraw.Width - squarewidth - squarewidth, rectdraw.Height - squarewidth - squarewidth);
                    }
                    break;
                case "Oval _tool":
                    var ovalCurrentPen = new Pen(Color.FromArgb(255, drawingcolour), ovalwidth);
                    var ovalCurrentBrush = new SolidBrush(Color.FromArgb(255, fillovalcolor));
                    RectangleF ovalrectdraw = new RectangleF(0, 0, picpreview.Width, picpreview.Height);
                    float ovalcorrectionamount = ovalwidth / 2;
                    if (ovalwidth > 0)
                    {
                        picpreview.CreateGraphics().DrawEllipse(ovalCurrentPen, ovalrectdraw.X + ovalcorrectionamount, ovalrectdraw.Y + ovalcorrectionamount, ovalrectdraw.Width - ovalwidth, ovalrectdraw.Height - ovalwidth);
                    }
                    if (ovalfillon == true)
                    {
                        float fixer = ovalwidth / 2;
                        picpreview.CreateGraphics().FillEllipse(ovalCurrentBrush, (ovalrectdraw.X + fixer), (ovalrectdraw.Y + fixer), ovalrectdraw.Width - fixer - fixer, ovalrectdraw.Height - fixer - fixer);
                    }
                    break;
                case "Text _tool":
                    var textCurrentBrush = new SolidBrush(Color.FromArgb(255, drawingcolour));
                    drawtextfont = new System.Drawing.Font(drawtextfontname, 20, drawtextfontstyle);
                    picpreview.CreateGraphics().DrawString("A", drawtextfont, textCurrentBrush, 20, 0);
                    break;
                case "Line _tool":
                    var lineCurrentPen = new Pen(Color.FromArgb(255, drawingcolour), linewidth);
                    picpreview.CreateGraphics().DrawLine(lineCurrentPen, 0, 0, picpreview.Width, picpreview.Height);
                    break;
                case "Pencil":
                    var pencilCurrentPen = new Pen(Color.FromArgb(255, drawingcolour), pencilwidth);
                    picpreview.CreateGraphics().DrawLine(pencilCurrentPen, 0, 25, picpreview.Width, 25);
                    break;
                case "Paint Brush":
                    var pbCurrentBrush = new SolidBrush(Color.FromArgb(255, drawingcolour));
                    float halfsize = paintbrushwidth / 2;
                    float halfwidth = picdrawingdisplay.Width / 2;
                    float halfheight = picdrawingdisplay.Height / 2;
                    if (paintbrushtype == "circle")
                    {
                        picpreview.CreateGraphics().FillEllipse(pbCurrentBrush, halfwidth - 15 - halfsize, halfheight - 1 - halfsize, paintbrushwidth, paintbrushwidth);
                    }
                    else
                    {
                        picpreview.CreateGraphics().FillRectangle(pbCurrentBrush, halfwidth - 15 - halfsize, halfheight - 1 - halfsize, paintbrushwidth, paintbrushwidth);
                    }
                    break;
                case "Eracer":
                    System.Drawing.SolidBrush drawbrush = new System.Drawing.SolidBrush(drawingcolour);
                    picpreview.CreateGraphics().FillRectangle(drawbrush, 0, 0, picpreview.Width, picpreview.Height);
                    var eCurrentBrush = new SolidBrush(Color.FromArgb(255, Color.White));
                    float ehalfsize = eracerwidth / 2;
                    float ehalfwidth = picdrawingdisplay.Width / 2;
                    float ehalfheight = picdrawingdisplay.Height / 2;
                    if (eracertype == "circle")
                    {
                        picpreview.CreateGraphics().FillEllipse(eCurrentBrush, ehalfwidth - 15 - ehalfsize, ehalfheight - ehalfsize, eracerwidth, eracerwidth);
                    }
                    else
                    {
                        picpreview.CreateGraphics().FillRectangle(eCurrentBrush, ehalfwidth - 15 - ehalfsize, ehalfheight - ehalfsize, eracerwidth, eracerwidth);
                    }
                    break;
                default:
                    System.Drawing.SolidBrush ddrawbrush = new System.Drawing.SolidBrush(drawingcolour);
                    picpreview.CreateGraphics().FillRectangle(ddrawbrush, 0, 0, picpreview.Width, picpreview.Height);
                    ddrawbrush.Dispose();
                    break;
            }
        }

        #endregion

        #region Pencil

        // ERROR: Handles clauses are not supported in C#
        private void btnpencil_Click(object sender, EventArgs e)
        {
            selectedtool = "Pencil";
            gettoolsettings(pnlpencilsettings);
        }

        // ERROR: Handles clauses are not supported in C#
        private void ChangePencilSize(object sender, EventArgs e)
        {
            var s = (Control)sender;
            switch (s.Name.ToString())
            {
                case "btnpencilsize1":
                    pencilwidth = 1;
                    break;
                case "btnpencilsize2":
                    pencilwidth = 2;
                    break;
                case "btnpencilsize3":
                    pencilwidth = 3;
                    break;
            }
            setuppreview();
        }

        #endregion

        #region Undo/Redo

        // ERROR: Handles clauses are not supported in C#
        private void btnundo_Click(object sender, EventArgs e)
        {
            try
            {
                undo.redoStack.Push((Image)canvasbitmap.Clone());
                canvasbitmap = (Bitmap)undo.undoStack.Pop();
                graphicsbitmap = Graphics.FromImage(canvasbitmap);
                picdrawingdisplay.Invalidate();
            }
            catch
            {
                Infobox.Show("{ARTPAD_UNDO_ERROR}"
                , "{ARTPAD_NEXT_STEP_WILL_KILL_CANVAS_JUST_FLIPPING_CLICK_NEW}");
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnredo_Click(object sender, EventArgs e)
        {
            try
            {
                undo.undoStack.Push((Image)canvasbitmap.Clone());
                canvasbitmap = (Bitmap)undo.redoStack.Pop();
                graphicsbitmap = Graphics.FromImage(canvasbitmap);
                picdrawingdisplay.Invalidate();
            }
            catch
            {
                 Infobox.Show("{ARTPAD_REDO_ERROR}"
                , "{ARTPAD_NOTHING_TO_REDO}");
            }
        }

        #endregion

        #region File Loading

        // ERROR: Handles clauses are not supported in C#
        private void btnopen_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new FileDialog(new[] { ".pic" }, FileOpenerStyle.Open, new Action<string>((file) =>
            {
                string res = file;
                savelocation = res;
                openpic();
            })));
        }

        public void openpic()
        {
            pnlinitialcanvassettings.Hide();
            picdrawingdisplay.Show();
            magnificationlevel = 1;
            setmagnification();
            canvasbitmap = (Bitmap)SkinEngine.ImageFromBinary(Utils.ReadAllBytes(savelocation));
            canvasheight = canvasbitmap.Height;
            canvaswidth = canvasbitmap.Width;
            picdrawingdisplay.Size = new Size(canvaswidth, canvasheight);
            picdrawingdisplay.Location = new Point((pnldrawingbackground.Width - canvaswidth) / 2, (pnldrawingbackground.Height - canvasheight) / 2);
            graphicsbitmap = Graphics.FromImage(canvasbitmap);
            picdrawingdisplay.Invalidate();
        }

        #endregion

        #region Flood Fill

        // Flood fill the point.
        public void SafeFloodFill(Bitmap bm, int x, int y, Color new_color)
        {
            // Get the old and new colors.
            Color old_color = bm.GetPixel(x, y);

            // The following "If Then" test was added by Reuben
            // Jollif
            // to protect the code in case the start pixel
            // has the same color as the fill color.
            if (old_color.ToArgb() != new_color.ToArgb())
            {
                // Start with the original point in the stack.
                Stack<Point> pts = new Stack<Point>(1000);
                pts.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);

                // While the stack is not empty, process a point.
                while (pts.Count > 0)
                {
                    Point pt = (Point)pts.Pop();
                    if (pt.X > 0)
                        SafeCheckPoint(bm, pts, pt.X - 1, pt.Y, old_color, new_color);

                    if (pt.Y > 0)
                        SafeCheckPoint(bm, pts, pt.X, pt.Y - 1, old_color, new_color);
                    if (pt.X < bm.Width - 1)
                        SafeCheckPoint(bm, pts, pt.X + 1, pt.Y, old_color, new_color);
                    if (pt.Y < bm.Height - 1)
                        SafeCheckPoint(bm, pts, pt.X, pt.Y + 1, old_color, new_color);
                }
            }
        }

        // See if this point should be added to the stack.
        private void SafeCheckPoint(Bitmap bm, Stack<Point> pts, int x, int y, Color old_color, Color new_color)
        {
            Color clr = bm.GetPixel(x, y);
            if (clr.Equals(old_color))
            {
                pts.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnfill_Click(object sender, EventArgs e)
        {
            selectedtool = "Flood Fill";
            gettoolsettings(pnlfloodfillsettings);
        }

        #endregion

        #region Shapes

        // ERROR: Handles clauses are not supported in C#
        private void btnsquare_Click(object sender, EventArgs e)
        {
            selectedtool = "Square _tool";
            gettoolsettings(pnlsquaretoolsettings);
            txtsquareborderwidth.Text = squarewidth.ToString();
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtsquareborderwidth_TextChanged(object sender, EventArgs e)
        {
            if (txtsquareborderwidth.Text == "")
            {
            }
            else
            {
                squarewidth = (Convert.ToInt32(txtsquareborderwidth.Text));
                setuppreview();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlsquarefillcolour_Click(object sender, EventArgs e)
        {
            pnlsquarefillcolour.BackColor = drawingcolour;
            fillsquarecolor = drawingcolour;
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnsquarefillonoff_Click(object sender, EventArgs e)
        {
            if (squarefillon == true)
            {
                btnsquarefillonoff.Text = "Fill OFF";
                btnsquarefillonoff.BackColor = Color.White;
                btnsquarefillonoff.ForeColor = Color.Black;
                squarefillon = false;
            }
            else
            {
                btnsquarefillonoff.Text = "Fill ON";
                btnsquarefillonoff.BackColor = Color.Black;
                btnsquarefillonoff.ForeColor = Color.White;
                squarefillon = true;
            }
            txtsquareborderwidth.Text = squarewidth.ToString();
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnoval_Click(object sender, EventArgs e)
        {
            selectedtool = "Oval _tool";
            gettoolsettings(pnlovaltoolsettings);
            txtovalborderwidth.Text = ovalwidth.ToString();
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtovalborderwidth_TextChanged(object sender, EventArgs e)
        {
            if (txtovalborderwidth.Text == "")
            {
            }
            else
            {
                ovalwidth = (Convert.ToInt32(txtovalborderwidth.Text));
                setuppreview();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void pnlovalfillcolour_Click(object sender, EventArgs e)
        {
            pnlovalfillcolour.BackColor = drawingcolour;
            fillovalcolor = drawingcolour;
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnovalfillonoff_Click(object sender, EventArgs e)
        {
            if (ovalfillon == true)
            {
                btnovalfillonoff.Text = "Fill OFF";
                btnovalfillonoff.BackColor = Color.White;
                btnovalfillonoff.ForeColor = Color.Black;
                ovalfillon = false;
            }
            else
            {
                btnovalfillonoff.Text = "Fill ON";
                btnovalfillonoff.BackColor = Color.Black;
                btnovalfillonoff.ForeColor = Color.White;
                ovalfillon = true;
            }
            txtovalborderwidth.Text = ovalwidth.ToString();
            setuppreview();
        }
        #endregion

        #region Eraser

        // ERROR: Handles clauses are not supported in C#
        private void btneracer_Click(object sender, EventArgs e)
        {
            selectedtool = "Eracer";
            gettoolsettings(pnleracertoolsettings);
            txteracersize.Text = eracerwidth.ToString();
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void txteracersize_TextChanged(object sender, EventArgs e)
        {
            if (txteracersize.Text == "")
            {
            }
            else
            {
                try
                {
                    eracerwidth = (Convert.ToInt32(txteracersize.Text));
                }
                catch
                {
                    txteracersize.Text = eracerwidth.ToString();
                }
                }
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btneracercircle_Click(object sender, EventArgs e)
        {
            eracertype = "circle";
            btneracercircle.BackgroundImage = Properties.Resources.ArtPadcirclerubberselected;
            btneracersquare.BackgroundImage = Properties.Resources.ArtPadsquarerubber;
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btneracersquare_Click(object sender, EventArgs e)
        {
            eracertype = "square";
            btneracercircle.BackgroundImage = Properties.Resources.ArtPadcirclerubber;
            btneracersquare.BackgroundImage = Properties.Resources.ArtPadsquarerubberselected;
            setuppreview();
        }

        #endregion

        #region Line

        // ERROR: Handles clauses are not supported in C#
        private void btnlinetool_Click(object sender, EventArgs e)
        {
            selectedtool = "Line _tool";
            gettoolsettings(pnllinetoolsettings);
            txtlinewidth.Text = linewidth.ToString();
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtlinewidth_TextChanged(object sender, EventArgs e)
        {
            if (txtlinewidth.Text == "")
            {
            }
            else
            {
                linewidth = (Convert.ToInt32(txtlinewidth.Text));
            }
            setuppreview();
        }

        #endregion

        #region Text

        // ERROR: Handles clauses are not supported in C#
        private void btntexttool_Click(object sender, EventArgs e)
        {
            selectedtool = "Text _tool";
            gettoolsettings(pnltexttoolsettings);
        }

        // ERROR: Handles clauses are not supported in C#
        private void txtdrawtextsize_TextChanged(object sender, EventArgs e)
        {
            if (txtdrawtextsize.Text == "")
            {
            }
            else
            {
                drawtextsize = Convert.ToInt32(txtdrawtextsize.Text);
            }
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void combodrawtextfont_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawtextfontname = combodrawtextfont.Text;
            txtdrawstringtext.Focus();
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void combofontstyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (combofontstyle.Text)
            {
                case "Bold":
                    drawtextfontstyle = FontStyle.Bold;
                    break; //BREAK COME ON HOO OH YEAHE CHECK IT OUT OH YEAH LETS GO SCRUATCHO HEY
                case "Italic":
                    drawtextfontstyle = FontStyle.Italic;
                    break;
                case "Regular":
                    drawtextfontstyle = FontStyle.Regular;
                    break;
                case "Strikeout":
                    drawtextfontstyle = FontStyle.Strikeout;
                    break;
                case "Underline":
                    drawtextfontstyle = FontStyle.Underline;
                    break;
            }
            txtdrawstringtext.Focus();
            setuppreview();
        }
        #endregion

        #region Paintbrush

        // ERROR: Handles clauses are not supported in C#
        private void txtpaintbrushsize_TextChanged(object sender, EventArgs e)
        {
            if (txtpaintbrushsize.Text == "")
            {
            }
            else
            {
                paintbrushwidth = (Convert.ToInt32(txtpaintbrushsize.Text));
            }
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnpaintsquareshape_Click(object sender, EventArgs e)
        {
            paintbrushtype = "square";
            btnpaintcircleshape.BackgroundImage = Properties.Resources.ArtPadcirclerubber;
            btnpaintsquareshape.BackgroundImage = Properties.Resources.ArtPadsquarerubberselected;
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnpaintcircleshape_Click(object sender, EventArgs e)
        {
            paintbrushtype = "circle";
            btnpaintcircleshape.BackgroundImage = Properties.Resources.ArtPadcirclerubberselected;
            btnpaintsquareshape.BackgroundImage = Properties.Resources.ArtPadsquarerubber;
            setuppreview();
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnpaintbrush_Click(object sender, EventArgs e)
        {
            selectedtool = "Paint Brush";
            gettoolsettings(pnlpaintbrushtoolsettings);
            txtpaintbrushsize.Text = paintbrushwidth.ToString();
            setuppreview();
        }

        #endregion

        #region Codepoints

        private void preparecooldown()
        {
            needtosave = true;
            if (codepointscooldown == true)
            {
            }
            else
            {
                codepointsearned = codepointsearned + 1;
                codepointscooldown = true;
                tmrcodepointcooldown.Start();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void tmrcodepointcooldown_Tick(object sender, EventArgs e)
        {
            codepointscooldown = false;
            tmrcodepointcooldown.Stop();
        }

        // ERROR: Handles clauses are not supported in C#
        private void tmrshowearnedcodepoints_Tick(object sender, EventArgs e)
        {
            tmrshowearnedcodepoints.Stop();
        }

        #endregion

        #region More UI stuff

        public void determinevisiblepalettes()
        {
            int panelstoadd = 2;

            if (ShiftoriumFrontend.UpgradeInstalled("artpad_4_color_palettes") == true)
            {
                panelstoadd = 4;
            }
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_8_color_palettes") == true)
            {
                panelstoadd = 8;
            }
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_16_color_palettes") == true)
            {
                panelstoadd = 16;
            }
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_32_color_palettes") == true)
            {
                panelstoadd = 32;
            }
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_64_color_palettes") == true)
            {
                panelstoadd = 64;
            }
            if (ShiftoriumFrontend.UpgradeInstalled("artpad_128_color_palettes") == true)
            {
                panelstoadd = 128;
            }
            flowcolours.Controls.Clear();
            for (int i = 0; i < panelstoadd; i++)
            {
                Panel pnl = new Panel();
                pnl.BackColor = Color.Black;
                pnl.Size = new Size(12, 8);
                flowcolours.Controls.Add(pnl);
                pnl.Margin = new Padding(1, 0, 0, 1);
                pnl.MouseClick += new MouseEventHandler(this.colourpalette1_MouseClick);
                pnl.Show();
            }
        }

        // ERROR: Handles clauses are not supported in C#
        private void btnchangesizecancel_Click(object sender, EventArgs e)
        {
            pnlpalettesize.Hide();
        }

        #endregion

        #region More Text Stuff

        // ERROR: Handles clauses are not supported in C#
        private void txtdrawstringtext_TextChanged(object sender, EventArgs e)
        {
            setuppreview();
        }

        #endregion

        private void tmrsetupui_Tick(object sender, EventArgs e)
        {
        }

        private void btnpixelplacer_Click(object sender, EventArgs e)
        {
            selectedtool = "Pixel Placer";
            gettoolsettings(pnlpixelplacersettings);
        }

        public void OnLoad()
        {
            foreach (Control ctrl in flowcolours.Controls)
            {
                ctrl.Tag = "keepbg";
                ctrl.BackColor = Color.Black;
            }
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

    public class undo
    {
        public Stack<Image> undoStack = new Stack<Image>();
        public Stack<Image> redoStack = new Stack<Image>();

    }
}
