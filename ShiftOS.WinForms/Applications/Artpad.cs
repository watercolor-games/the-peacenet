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
using System.Diagnostics;

namespace ShiftOS.WinForms.Applications
{
    [FileHandler("Artpad Picture", ".pic", "fileiconpicture")]
    [FileHandler("JPEG Picture", ".jpg", "fileiconpicture")]
    [FileHandler("PNG Picture", ".png", "fileiconpicture")]
    [FileHandler("Bitmap Picture", ".bmp", "fileiconpicture")]
    [MultiplayerOnly]
    [Launcher("{TIITLE_ARTPAD}", true, "al_artpad", "{AL_GRAPHICS}")]
    [RequiresUpgrade("artpad")]
    [WinOpen("{WO_ARTPAD}")]
    [DefaultIcon("iconArtpad")]
    [DefaultTitle("{TITLE_ARTPAD}")]
    public partial class Artpad : UserControl, IShiftOSWindow, IFileHandler
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
        float magnificationlevel = 1;
        RectangleF magnifyRect;
        Graphics graphicsbitmap;
        public Color drawingcolour = Color.Black;
        string selectedtool = "Pixel Setter";
        bool pixalplacermovable = false;
        public string savelocation;
        System.Drawing.Drawing2D.GraphicsPath mousePath = new System.Drawing.Drawing2D.GraphicsPath();
        int pencilwidth = 1;
        undo undo = new undo();
        PointF thisPoint;
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
            //Moved to the engine "OnLoad" method.
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

        //PHILCODE: I just reduced this function's amount of Windows Forms calls by 66%.
        public void setuptoolbox()
        {
            gENSAVEToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("artpad_save");
            gENLOADToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("artpad_open");
            gENNEWToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("artpad_new");

            undoToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("artpad_undo");
            redoToolStripMenuItem.Visible = Shiftorium.UpgradeInstalled("artpad_redo");

            editToolStripMenuItem.Visible = (undoToolStripMenuItem.Visible || redoToolStripMenuItem.Visible);



            btnpixelplacer.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_pixel_placer") == true);
            btnpencil.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_pencil") == true);
            btnfloodfill.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_fill_tool") == true);
            btnoval.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_oval_tool") == true);
            btnsquare.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_rectangle_tool") == true);
            btnlinetool.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_line_tool") == true);
            btnpaintbrush.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_paintbrush") == true);
            btntexttool.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_text_tool") == true);
            btneracer.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_eraser") == true);
            btnpixelplacermovementmode.Visible = (ShiftoriumFrontend.UpgradeInstalled("artpad_pp_movement_mode") == true);
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


            thisPoint.X = (e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;

            if (selectedtool == "Pixel Placer")
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (thisPoint.X < canvasbitmap.Width && thisPoint.X > -1)
                    {
                        if (thisPoint.Y < canvasbitmap.Height && thisPoint.Y > -1)
                        {
                            canvasbitmap.SetPixel((int)thisPoint.X, (int)thisPoint.Y, drawingcolour);
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
                            SafeFloodFill(canvasbitmap, (int)thisPoint.X, (int)thisPoint.Y, drawingcolour);
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
            PointF lastpoint;

            thisPoint.X = (e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;


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
                            //NO, NO, NO... Please don't tell me you're using SetPixel()... Whatever...
                            canvasbitmap.SetPixel((int)thisPoint.X, (int)thisPoint.Y, drawingcolour);
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
            thisPoint.X = (e.Location.X - (magnificationlevel / 2)) / magnificationlevel;
            thisPoint.Y = (e.Location.Y - (magnificationlevel / 2)) / magnificationlevel;

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
            if (magnificationlevel > 0)
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
            magnifyRect.Width = canvaswidth / magnificationlevel;
            magnifyRect.Height = canvasheight / magnificationlevel;
            picdrawingdisplay.Size = new Size((int)(canvaswidth * magnificationlevel), (int)(canvasheight * magnificationlevel));
            if (picdrawingdisplay.Height > 468 && picdrawingdisplay.Width > 676)
            {
                picdrawingdisplay.Location = new Point(0, 0);
            }
            else
            {
                picdrawingdisplay.CenterParent();
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
            if (txtnewcanvaswidth.Text == "" || txtnewcanvasheight.Text == "")
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
                        lbltotalpixels.ForeColor = SkinEngine.LoadedSkin.ControlTextColor;
                    }
                    else
                    {
                        if ((Convert.ToInt32(txtnewcanvaswidth.Text) * Convert.ToInt32(txtnewcanvasheight.Text)) > GetPixelLimit())
                        {
                            lbltotalpixels.ForeColor = Color.Red;
                        }
                        else
                        {
                            lbltotalpixels.ForeColor = SkinEngine.LoadedSkin.ControlTextColor;
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
            setuppreview();
            settoolcolours();
            AddFonts();
            setuptoolbox();
            determinevisiblepalettes();

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

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            magnificationlevel = 0.25f;
            setmagnification();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            magnificationlevel = 0.5f;
            setmagnification();

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            magnificationlevel = 1;
            setmagnification();

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            magnificationlevel = 2f;
            setmagnification();

        }

        public void OpenFile(string file)
        {
            AppearanceManager.SetupWindow(this);
            LoadPicture(file);
        }
    }

    public class undo
    {
        public Stack<Image> undoStack = new Stack<Image>();
        public Stack<Image> redoStack = new Stack<Image>();

    }
}
