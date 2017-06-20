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

namespace ShiftOS.WinForms.Applications
{
    partial class Artpad
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pgcontents = new System.Windows.Forms.Panel();
            this.pnldrawingbackground = new System.Windows.Forms.Panel();
            this.pnlinitialcanvassettings = new System.Windows.Forms.Panel();
            this.btncreate = new System.Windows.Forms.Button();
            this.Label11 = new System.Windows.Forms.Label();
            this.lbltotalpixels = new System.Windows.Forms.Label();
            this.txtnewcanvasheight = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtnewcanvaswidth = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.pnlpalettesize = new System.Windows.Forms.Panel();
            this.txttopspace = new System.Windows.Forms.TextBox();
            this.Label40 = new System.Windows.Forms.Label();
            this.txtsidespace = new System.Windows.Forms.TextBox();
            this.Label41 = new System.Windows.Forms.Label();
            this.btnchangesizecancel = new System.Windows.Forms.Button();
            this.btnsetsize = new System.Windows.Forms.Button();
            this.txtcolorpalletheight = new System.Windows.Forms.TextBox();
            this.Label42 = new System.Windows.Forms.Label();
            this.txtcolorpalletwidth = new System.Windows.Forms.TextBox();
            this.Label43 = new System.Windows.Forms.Label();
            this.picdrawingdisplay = new System.Windows.Forms.PictureBox();
            this.pnlbottompanel = new System.Windows.Forms.Panel();
            this.pnlpallet = new System.Windows.Forms.Panel();
            this.label44 = new System.Windows.Forms.Label();
            this.flowcolours = new System.Windows.Forms.FlowLayoutPanel();
            this.colourpallet1 = new System.Windows.Forms.Panel();
            this.colourpallet2 = new System.Windows.Forms.Panel();
            this.colourpallet3 = new System.Windows.Forms.Panel();
            this.colourpallet4 = new System.Windows.Forms.Panel();
            this.colourpallet5 = new System.Windows.Forms.Panel();
            this.colourpallet6 = new System.Windows.Forms.Panel();
            this.colourpallet7 = new System.Windows.Forms.Panel();
            this.colourpallet8 = new System.Windows.Forms.Panel();
            this.colourpallet9 = new System.Windows.Forms.Panel();
            this.colourpallet10 = new System.Windows.Forms.Panel();
            this.colourpallet11 = new System.Windows.Forms.Panel();
            this.colourpallet12 = new System.Windows.Forms.Panel();
            this.colourpallet13 = new System.Windows.Forms.Panel();
            this.colourpallet14 = new System.Windows.Forms.Panel();
            this.colourpallet15 = new System.Windows.Forms.Panel();
            this.colourpallet16 = new System.Windows.Forms.Panel();
            this.colourpallet17 = new System.Windows.Forms.Panel();
            this.colourpallet18 = new System.Windows.Forms.Panel();
            this.colourpallet19 = new System.Windows.Forms.Panel();
            this.colourpallet20 = new System.Windows.Forms.Panel();
            this.colourpallet21 = new System.Windows.Forms.Panel();
            this.colourpallet22 = new System.Windows.Forms.Panel();
            this.colourpallet23 = new System.Windows.Forms.Panel();
            this.colourpallet24 = new System.Windows.Forms.Panel();
            this.colourpallet25 = new System.Windows.Forms.Panel();
            this.colourpallet26 = new System.Windows.Forms.Panel();
            this.colourpallet27 = new System.Windows.Forms.Panel();
            this.colourpallet28 = new System.Windows.Forms.Panel();
            this.colourpallet29 = new System.Windows.Forms.Panel();
            this.colourpallet30 = new System.Windows.Forms.Panel();
            this.colourpallet31 = new System.Windows.Forms.Panel();
            this.colourpallet32 = new System.Windows.Forms.Panel();
            this.colourpallet33 = new System.Windows.Forms.Panel();
            this.colourpallet34 = new System.Windows.Forms.Panel();
            this.colourpallet35 = new System.Windows.Forms.Panel();
            this.colourpallet36 = new System.Windows.Forms.Panel();
            this.colourpallet37 = new System.Windows.Forms.Panel();
            this.colourpallet38 = new System.Windows.Forms.Panel();
            this.colourpallet39 = new System.Windows.Forms.Panel();
            this.colourpallet40 = new System.Windows.Forms.Panel();
            this.colourpallet41 = new System.Windows.Forms.Panel();
            this.colourpallet42 = new System.Windows.Forms.Panel();
            this.colourpallet43 = new System.Windows.Forms.Panel();
            this.colourpallet44 = new System.Windows.Forms.Panel();
            this.colourpallet45 = new System.Windows.Forms.Panel();
            this.colourpallet46 = new System.Windows.Forms.Panel();
            this.colourpallet47 = new System.Windows.Forms.Panel();
            this.colourpallet48 = new System.Windows.Forms.Panel();
            this.colourpallet49 = new System.Windows.Forms.Panel();
            this.colourpallet50 = new System.Windows.Forms.Panel();
            this.colourpallet51 = new System.Windows.Forms.Panel();
            this.colourpallet52 = new System.Windows.Forms.Panel();
            this.colourpallet53 = new System.Windows.Forms.Panel();
            this.colourpallet54 = new System.Windows.Forms.Panel();
            this.colourpallet55 = new System.Windows.Forms.Panel();
            this.colourpallet56 = new System.Windows.Forms.Panel();
            this.colourpallet57 = new System.Windows.Forms.Panel();
            this.colourpallet58 = new System.Windows.Forms.Panel();
            this.colourpallet59 = new System.Windows.Forms.Panel();
            this.colourpallet60 = new System.Windows.Forms.Panel();
            this.colourpallet61 = new System.Windows.Forms.Panel();
            this.colourpallet62 = new System.Windows.Forms.Panel();
            this.colourpallet63 = new System.Windows.Forms.Panel();
            this.colourpallet64 = new System.Windows.Forms.Panel();
            this.colourpallet65 = new System.Windows.Forms.Panel();
            this.colourpallet66 = new System.Windows.Forms.Panel();
            this.colourpallet67 = new System.Windows.Forms.Panel();
            this.colourpallet68 = new System.Windows.Forms.Panel();
            this.colourpallet69 = new System.Windows.Forms.Panel();
            this.colourpallet70 = new System.Windows.Forms.Panel();
            this.colourpallet71 = new System.Windows.Forms.Panel();
            this.colourpallet72 = new System.Windows.Forms.Panel();
            this.colourpallet73 = new System.Windows.Forms.Panel();
            this.colourpallet74 = new System.Windows.Forms.Panel();
            this.colourpallet75 = new System.Windows.Forms.Panel();
            this.colourpallet76 = new System.Windows.Forms.Panel();
            this.colourpallet77 = new System.Windows.Forms.Panel();
            this.colourpallet78 = new System.Windows.Forms.Panel();
            this.colourpallet79 = new System.Windows.Forms.Panel();
            this.colourpallet80 = new System.Windows.Forms.Panel();
            this.colourpallet81 = new System.Windows.Forms.Panel();
            this.colourpallet82 = new System.Windows.Forms.Panel();
            this.colourpallet83 = new System.Windows.Forms.Panel();
            this.colourpallet84 = new System.Windows.Forms.Panel();
            this.colourpallet85 = new System.Windows.Forms.Panel();
            this.colourpallet86 = new System.Windows.Forms.Panel();
            this.colourpallet87 = new System.Windows.Forms.Panel();
            this.colourpallet88 = new System.Windows.Forms.Panel();
            this.colourpallet89 = new System.Windows.Forms.Panel();
            this.colourpallet90 = new System.Windows.Forms.Panel();
            this.colourpallet91 = new System.Windows.Forms.Panel();
            this.colourpallet92 = new System.Windows.Forms.Panel();
            this.colourpallet93 = new System.Windows.Forms.Panel();
            this.colourpallet94 = new System.Windows.Forms.Panel();
            this.colourpallet95 = new System.Windows.Forms.Panel();
            this.colourpallet96 = new System.Windows.Forms.Panel();
            this.colourpallet97 = new System.Windows.Forms.Panel();
            this.colourpallet98 = new System.Windows.Forms.Panel();
            this.colourpallet99 = new System.Windows.Forms.Panel();
            this.colourpallet100 = new System.Windows.Forms.Panel();
            this.colourpallet101 = new System.Windows.Forms.Panel();
            this.colourpallet102 = new System.Windows.Forms.Panel();
            this.colourpallet103 = new System.Windows.Forms.Panel();
            this.colourpallet104 = new System.Windows.Forms.Panel();
            this.colourpallet105 = new System.Windows.Forms.Panel();
            this.colourpallet106 = new System.Windows.Forms.Panel();
            this.colourpallet107 = new System.Windows.Forms.Panel();
            this.colourpallet108 = new System.Windows.Forms.Panel();
            this.colourpallet109 = new System.Windows.Forms.Panel();
            this.colourpallet110 = new System.Windows.Forms.Panel();
            this.colourpallet111 = new System.Windows.Forms.Panel();
            this.colourpallet112 = new System.Windows.Forms.Panel();
            this.colourpallet113 = new System.Windows.Forms.Panel();
            this.colourpallet114 = new System.Windows.Forms.Panel();
            this.colourpallet115 = new System.Windows.Forms.Panel();
            this.colourpallet116 = new System.Windows.Forms.Panel();
            this.colourpallet117 = new System.Windows.Forms.Panel();
            this.colourpallet118 = new System.Windows.Forms.Panel();
            this.colourpallet119 = new System.Windows.Forms.Panel();
            this.colourpallet120 = new System.Windows.Forms.Panel();
            this.colourpallet121 = new System.Windows.Forms.Panel();
            this.colourpallet122 = new System.Windows.Forms.Panel();
            this.colourpallet123 = new System.Windows.Forms.Panel();
            this.colourpallet124 = new System.Windows.Forms.Panel();
            this.colourpallet125 = new System.Windows.Forms.Panel();
            this.colourpallet126 = new System.Windows.Forms.Panel();
            this.colourpallet127 = new System.Windows.Forms.Panel();
            this.colourpallet128 = new System.Windows.Forms.Panel();
            this.Label4 = new System.Windows.Forms.Label();
            this.line6 = new System.Windows.Forms.Panel();
            this.pnltoolproperties = new System.Windows.Forms.Panel();
            this.pnlmagnifiersettings = new System.Windows.Forms.Panel();
            this.btnzoomout = new System.Windows.Forms.Button();
            this.btnzoomin = new System.Windows.Forms.Button();
            this.lblzoomlevel = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.pnleracertoolsettings = new System.Windows.Forms.Panel();
            this.Label28 = new System.Windows.Forms.Label();
            this.btneracersquare = new System.Windows.Forms.Button();
            this.btneracercircle = new System.Windows.Forms.Button();
            this.Label24 = new System.Windows.Forms.Label();
            this.txteracersize = new System.Windows.Forms.TextBox();
            this.Label26 = new System.Windows.Forms.Label();
            this.Label27 = new System.Windows.Forms.Label();
            this.pnlpixelplacersettings = new System.Windows.Forms.Panel();
            this.lblpixelplacerhelp = new System.Windows.Forms.Label();
            this.btnpixelplacermovementmode = new System.Windows.Forms.Button();
            this.Label8 = new System.Windows.Forms.Label();
            this.pnlovaltoolsettings = new System.Windows.Forms.Panel();
            this.Label20 = new System.Windows.Forms.Label();
            this.btnovalfillonoff = new System.Windows.Forms.Button();
            this.pnlovalfillcolour = new System.Windows.Forms.Panel();
            this.txtovalborderwidth = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label23 = new System.Windows.Forms.Label();
            this.pnllinetoolsettings = new System.Windows.Forms.Panel();
            this.Label29 = new System.Windows.Forms.Label();
            this.txtlinewidth = new System.Windows.Forms.TextBox();
            this.Label30 = new System.Windows.Forms.Label();
            this.Label31 = new System.Windows.Forms.Label();
            this.pnlpaintbrushtoolsettings = new System.Windows.Forms.Panel();
            this.Label36 = new System.Windows.Forms.Label();
            this.btnpaintsquareshape = new System.Windows.Forms.Button();
            this.btnpaintcircleshape = new System.Windows.Forms.Button();
            this.Label37 = new System.Windows.Forms.Label();
            this.txtpaintbrushsize = new System.Windows.Forms.TextBox();
            this.Label38 = new System.Windows.Forms.Label();
            this.Label39 = new System.Windows.Forms.Label();
            this.pnltexttoolsettings = new System.Windows.Forms.Panel();
            this.Label35 = new System.Windows.Forms.Label();
            this.combofontstyle = new System.Windows.Forms.ComboBox();
            this.txtdrawstringtext = new System.Windows.Forms.TextBox();
            this.combodrawtextfont = new System.Windows.Forms.ComboBox();
            this.Label25 = new System.Windows.Forms.Label();
            this.txtdrawtextsize = new System.Windows.Forms.TextBox();
            this.Label32 = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.pnlpixelsettersettings = new System.Windows.Forms.Panel();
            this.btnpixelsettersetpixel = new System.Windows.Forms.Button();
            this.txtpixelsetterycoordinate = new System.Windows.Forms.TextBox();
            this.txtpixelsetterxcoordinate = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.pnlfloodfillsettings = new System.Windows.Forms.Panel();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.pnlsquaretoolsettings = new System.Windows.Forms.Panel();
            this.Label19 = new System.Windows.Forms.Label();
            this.btnsquarefillonoff = new System.Windows.Forms.Button();
            this.pnlsquarefillcolour = new System.Windows.Forms.Panel();
            this.txtsquareborderwidth = new System.Windows.Forms.TextBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.pnlpencilsettings = new System.Windows.Forms.Panel();
            this.btnpencilsize3 = new System.Windows.Forms.Button();
            this.btnpencilsize2 = new System.Windows.Forms.Button();
            this.btnpencilsize1 = new System.Windows.Forms.Button();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.line5 = new System.Windows.Forms.Panel();
            this.line3 = new System.Windows.Forms.Panel();
            this.pnltools = new System.Windows.Forms.Panel();
            this.pnltoolpositioner = new System.Windows.Forms.FlowLayoutPanel();
            this.btnpixelsetter = new System.Windows.Forms.Button();
            this.btnpixelplacer = new System.Windows.Forms.Button();
            this.btnpencil = new System.Windows.Forms.Button();
            this.btnfloodfill = new System.Windows.Forms.Button();
            this.btnoval = new System.Windows.Forms.Button();
            this.btnsquare = new System.Windows.Forms.Button();
            this.btnlinetool = new System.Windows.Forms.Button();
            this.btnpaintbrush = new System.Windows.Forms.Button();
            this.btntexttool = new System.Windows.Forms.Button();
            this.btneracer = new System.Windows.Forms.Button();
            this.line1 = new System.Windows.Forms.Panel();
            this.pnltoolpreview = new System.Windows.Forms.Panel();
            this.Label13 = new System.Windows.Forms.Label();
            this.picpreview = new System.Windows.Forms.PictureBox();
            this.lbltoolselected = new System.Windows.Forms.Label();
            this.line4 = new System.Windows.Forms.Panel();
            this.line2 = new System.Windows.Forms.Panel();
            this.tmrcodepointcooldown = new System.Windows.Forms.Timer(this.components);
            this.tmrshowearnedcodepoints = new System.Windows.Forms.Timer(this.components);
            this.pullbs = new System.Windows.Forms.Timer(this.components);
            this.pullbottom = new System.Windows.Forms.Timer(this.components);
            this.pullside = new System.Windows.Forms.Timer(this.components);
            this.tmrsetupui = new System.Windows.Forms.Timer(this.components);
            this.msTools = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gENNEWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gENLOADToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gENSAVEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gENEXITToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.moreControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pgcontents.SuspendLayout();
            this.pnldrawingbackground.SuspendLayout();
            this.pnlinitialcanvassettings.SuspendLayout();
            this.pnlpalettesize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picdrawingdisplay)).BeginInit();
            this.pnlbottompanel.SuspendLayout();
            this.pnlpallet.SuspendLayout();
            this.flowcolours.SuspendLayout();
            this.pnltoolproperties.SuspendLayout();
            this.pnlmagnifiersettings.SuspendLayout();
            this.pnleracertoolsettings.SuspendLayout();
            this.pnlpixelplacersettings.SuspendLayout();
            this.pnlovaltoolsettings.SuspendLayout();
            this.pnllinetoolsettings.SuspendLayout();
            this.pnlpaintbrushtoolsettings.SuspendLayout();
            this.pnltexttoolsettings.SuspendLayout();
            this.pnlpixelsettersettings.SuspendLayout();
            this.pnlfloodfillsettings.SuspendLayout();
            this.pnlsquaretoolsettings.SuspendLayout();
            this.pnlpencilsettings.SuspendLayout();
            this.pnltools.SuspendLayout();
            this.pnltoolpositioner.SuspendLayout();
            this.pnltoolpreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picpreview)).BeginInit();
            this.msTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgcontents
            // 
            this.pgcontents.BackColor = System.Drawing.Color.White;
            this.pgcontents.Controls.Add(this.pnldrawingbackground);
            this.pgcontents.Controls.Add(this.pnlbottompanel);
            this.pgcontents.Controls.Add(this.pnltools);
            this.pgcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcontents.Location = new System.Drawing.Point(0, 24);
            this.pgcontents.Name = "pgcontents";
            this.pgcontents.Size = new System.Drawing.Size(802, 574);
            this.pgcontents.TabIndex = 20;
            // 
            // pnldrawingbackground
            // 
            this.pnldrawingbackground.AutoScroll = true;
            this.pnldrawingbackground.BackColor = System.Drawing.Color.Gray;
            this.pnldrawingbackground.Controls.Add(this.pnlinitialcanvassettings);
            this.pnldrawingbackground.Controls.Add(this.pnlpalettesize);
            this.pnldrawingbackground.Controls.Add(this.picdrawingdisplay);
            this.pnldrawingbackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnldrawingbackground.Location = new System.Drawing.Point(120, 0);
            this.pnldrawingbackground.Name = "pnldrawingbackground";
            this.pnldrawingbackground.Size = new System.Drawing.Size(682, 474);
            this.pnldrawingbackground.TabIndex = 2;
            // 
            // pnlinitialcanvassettings
            // 
            this.pnlinitialcanvassettings.BackColor = System.Drawing.Color.White;
            this.pnlinitialcanvassettings.Controls.Add(this.btncreate);
            this.pnlinitialcanvassettings.Controls.Add(this.Label11);
            this.pnlinitialcanvassettings.Controls.Add(this.lbltotalpixels);
            this.pnlinitialcanvassettings.Controls.Add(this.txtnewcanvasheight);
            this.pnlinitialcanvassettings.Controls.Add(this.Label10);
            this.pnlinitialcanvassettings.Controls.Add(this.txtnewcanvaswidth);
            this.pnlinitialcanvassettings.Controls.Add(this.Label9);
            this.pnlinitialcanvassettings.Location = new System.Drawing.Point(144, 137);
            this.pnlinitialcanvassettings.Name = "pnlinitialcanvassettings";
            this.pnlinitialcanvassettings.Size = new System.Drawing.Size(326, 160);
            this.pnlinitialcanvassettings.TabIndex = 1;
            // 
            // btncreate
            // 
            this.btncreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncreate.Location = new System.Drawing.Point(196, 93);
            this.btncreate.Name = "btncreate";
            this.btncreate.Size = new System.Drawing.Size(115, 50);
            this.btncreate.TabIndex = 6;
            this.btncreate.Text = "Create";
            this.btncreate.UseVisualStyleBackColor = true;
            this.btncreate.Click += new System.EventHandler(this.btncreate_Click);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Label11.Location = new System.Drawing.Point(200, 7);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(106, 24);
            this.Label11.TabIndex = 5;
            this.Label11.Text = "Total Pixels";
            // 
            // lbltotalpixels
            // 
            this.lbltotalpixels.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F);
            this.lbltotalpixels.Location = new System.Drawing.Point(181, 31);
            this.lbltotalpixels.Name = "lbltotalpixels";
            this.lbltotalpixels.Size = new System.Drawing.Size(143, 56);
            this.lbltotalpixels.TabIndex = 4;
            this.lbltotalpixels.Text = "0";
            this.lbltotalpixels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtnewcanvasheight
            // 
            this.txtnewcanvasheight.BackColor = System.Drawing.Color.White;
            this.txtnewcanvasheight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtnewcanvasheight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtnewcanvasheight.Location = new System.Drawing.Point(82, 93);
            this.txtnewcanvasheight.MaxLength = 4;
            this.txtnewcanvasheight.Name = "txtnewcanvasheight";
            this.txtnewcanvasheight.Size = new System.Drawing.Size(54, 26);
            this.txtnewcanvasheight.TabIndex = 3;
            this.txtnewcanvasheight.Text = "0";
            this.txtnewcanvasheight.TextChanged += new System.EventHandler(this.txtnewcanvaswidth_TextChanged);
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.Label10.Location = new System.Drawing.Point(7, 91);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(74, 25);
            this.Label10.TabIndex = 2;
            this.Label10.Text = "Height:";
            // 
            // txtnewcanvaswidth
            // 
            this.txtnewcanvaswidth.BackColor = System.Drawing.Color.White;
            this.txtnewcanvaswidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtnewcanvaswidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtnewcanvaswidth.Location = new System.Drawing.Point(82, 41);
            this.txtnewcanvaswidth.MaxLength = 4;
            this.txtnewcanvaswidth.Name = "txtnewcanvaswidth";
            this.txtnewcanvaswidth.Size = new System.Drawing.Size(54, 26);
            this.txtnewcanvaswidth.TabIndex = 1;
            this.txtnewcanvaswidth.Text = "0";
            this.txtnewcanvaswidth.TextChanged += new System.EventHandler(this.txtnewcanvaswidth_TextChanged);
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.Label9.Location = new System.Drawing.Point(7, 39);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(69, 25);
            this.Label9.TabIndex = 0;
            this.Label9.Text = "Width:";
            // 
            // pnlpalettesize
            // 
            this.pnlpalettesize.BackColor = System.Drawing.Color.White;
            this.pnlpalettesize.Controls.Add(this.txttopspace);
            this.pnlpalettesize.Controls.Add(this.Label40);
            this.pnlpalettesize.Controls.Add(this.txtsidespace);
            this.pnlpalettesize.Controls.Add(this.Label41);
            this.pnlpalettesize.Controls.Add(this.btnchangesizecancel);
            this.pnlpalettesize.Controls.Add(this.btnsetsize);
            this.pnlpalettesize.Controls.Add(this.txtcolorpalletheight);
            this.pnlpalettesize.Controls.Add(this.Label42);
            this.pnlpalettesize.Controls.Add(this.txtcolorpalletwidth);
            this.pnlpalettesize.Controls.Add(this.Label43);
            this.pnlpalettesize.Location = new System.Drawing.Point(144, 303);
            this.pnlpalettesize.Name = "pnlpalettesize";
            this.pnlpalettesize.Size = new System.Drawing.Size(259, 100);
            this.pnlpalettesize.TabIndex = 2;
            this.pnlpalettesize.Visible = false;
            // 
            // txttopspace
            // 
            this.txttopspace.BackColor = System.Drawing.Color.White;
            this.txttopspace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txttopspace.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txttopspace.Location = new System.Drawing.Point(198, 38);
            this.txttopspace.Name = "txttopspace";
            this.txttopspace.Size = new System.Drawing.Size(54, 22);
            this.txttopspace.TabIndex = 11;
            // 
            // Label40
            // 
            this.Label40.AutoSize = true;
            this.Label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label40.Location = new System.Drawing.Point(115, 40);
            this.Label40.Name = "Label40";
            this.Label40.Size = new System.Drawing.Size(79, 16);
            this.Label40.TabIndex = 10;
            this.Label40.Text = "Top Space:";
            // 
            // txtsidespace
            // 
            this.txtsidespace.BackColor = System.Drawing.Color.White;
            this.txtsidespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtsidespace.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsidespace.Location = new System.Drawing.Point(198, 10);
            this.txtsidespace.Name = "txtsidespace";
            this.txtsidespace.Size = new System.Drawing.Size(54, 22);
            this.txtsidespace.TabIndex = 9;
            // 
            // Label41
            // 
            this.Label41.AutoSize = true;
            this.Label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label41.Location = new System.Drawing.Point(115, 12);
            this.Label41.Name = "Label41";
            this.Label41.Size = new System.Drawing.Size(82, 16);
            this.Label41.TabIndex = 8;
            this.Label41.Text = "Side Space:";
            // 
            // btnchangesizecancel
            // 
            this.btnchangesizecancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnchangesizecancel.Location = new System.Drawing.Point(7, 66);
            this.btnchangesizecancel.Name = "btnchangesizecancel";
            this.btnchangesizecancel.Size = new System.Drawing.Size(121, 28);
            this.btnchangesizecancel.TabIndex = 7;
            this.btnchangesizecancel.Text = "Close";
            this.btnchangesizecancel.UseVisualStyleBackColor = true;
            // 
            // btnsetsize
            // 
            this.btnsetsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsetsize.Location = new System.Drawing.Point(133, 66);
            this.btnsetsize.Name = "btnsetsize";
            this.btnsetsize.Size = new System.Drawing.Size(119, 28);
            this.btnsetsize.TabIndex = 6;
            this.btnsetsize.Text = "Set Size";
            this.btnsetsize.UseVisualStyleBackColor = true;
            // 
            // txtcolorpalletheight
            // 
            this.txtcolorpalletheight.BackColor = System.Drawing.Color.White;
            this.txtcolorpalletheight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtcolorpalletheight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcolorpalletheight.Location = new System.Drawing.Point(59, 37);
            this.txtcolorpalletheight.Name = "txtcolorpalletheight";
            this.txtcolorpalletheight.Size = new System.Drawing.Size(54, 22);
            this.txtcolorpalletheight.TabIndex = 3;
            // 
            // Label42
            // 
            this.Label42.AutoSize = true;
            this.Label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label42.Location = new System.Drawing.Point(8, 40);
            this.Label42.Name = "Label42";
            this.Label42.Size = new System.Drawing.Size(50, 16);
            this.Label42.TabIndex = 2;
            this.Label42.Text = "Height:";
            // 
            // txtcolorpalletwidth
            // 
            this.txtcolorpalletwidth.BackColor = System.Drawing.Color.White;
            this.txtcolorpalletwidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtcolorpalletwidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcolorpalletwidth.Location = new System.Drawing.Point(59, 9);
            this.txtcolorpalletwidth.Name = "txtcolorpalletwidth";
            this.txtcolorpalletwidth.Size = new System.Drawing.Size(54, 22);
            this.txtcolorpalletwidth.TabIndex = 1;
            // 
            // Label43
            // 
            this.Label43.AutoSize = true;
            this.Label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label43.Location = new System.Drawing.Point(8, 12);
            this.Label43.Name = "Label43";
            this.Label43.Size = new System.Drawing.Size(45, 16);
            this.Label43.TabIndex = 0;
            this.Label43.Text = "Width:";
            // 
            // picdrawingdisplay
            // 
            this.picdrawingdisplay.BackColor = System.Drawing.Color.White;
            this.picdrawingdisplay.Location = new System.Drawing.Point(180, 108);
            this.picdrawingdisplay.Name = "picdrawingdisplay";
            this.picdrawingdisplay.Size = new System.Drawing.Size(100, 50);
            this.picdrawingdisplay.TabIndex = 0;
            this.picdrawingdisplay.TabStop = false;
            this.picdrawingdisplay.Visible = false;
            this.picdrawingdisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.picdrawingdisplay_Paint);
            this.picdrawingdisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picdrawingdisplay_MouseDown);
            this.picdrawingdisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picdrawingdisplay_MouseMove);
            this.picdrawingdisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picdrawingdisplay_MouseUp);
            // 
            // pnlbottompanel
            // 
            this.pnlbottompanel.Controls.Add(this.pnlpallet);
            this.pnlbottompanel.Controls.Add(this.pnltoolproperties);
            this.pnlbottompanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlbottompanel.Location = new System.Drawing.Point(120, 474);
            this.pnlbottompanel.Name = "pnlbottompanel";
            this.pnlbottompanel.Size = new System.Drawing.Size(682, 100);
            this.pnlbottompanel.TabIndex = 1;
            // 
            // pnlpallet
            // 
            this.pnlpallet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlpallet.Controls.Add(this.label44);
            this.pnlpallet.Controls.Add(this.flowcolours);
            this.pnlpallet.Controls.Add(this.Label4);
            this.pnlpallet.Controls.Add(this.line6);
            this.pnlpallet.Location = new System.Drawing.Point(457, 0);
            this.pnlpallet.Name = "pnlpallet";
            this.pnlpallet.Size = new System.Drawing.Size(225, 100);
            this.pnlpallet.TabIndex = 0;
            // 
            // label44
            // 
            this.label44.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label44.Location = new System.Drawing.Point(0, 16);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(225, 11);
            this.label44.TabIndex = 13;
            this.label44.Text = "Left click to select, right click to change color";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowcolours
            // 
            this.flowcolours.Controls.Add(this.colourpallet1);
            this.flowcolours.Controls.Add(this.colourpallet2);
            this.flowcolours.Controls.Add(this.colourpallet3);
            this.flowcolours.Controls.Add(this.colourpallet4);
            this.flowcolours.Controls.Add(this.colourpallet5);
            this.flowcolours.Controls.Add(this.colourpallet6);
            this.flowcolours.Controls.Add(this.colourpallet7);
            this.flowcolours.Controls.Add(this.colourpallet8);
            this.flowcolours.Controls.Add(this.colourpallet9);
            this.flowcolours.Controls.Add(this.colourpallet10);
            this.flowcolours.Controls.Add(this.colourpallet11);
            this.flowcolours.Controls.Add(this.colourpallet12);
            this.flowcolours.Controls.Add(this.colourpallet13);
            this.flowcolours.Controls.Add(this.colourpallet14);
            this.flowcolours.Controls.Add(this.colourpallet15);
            this.flowcolours.Controls.Add(this.colourpallet16);
            this.flowcolours.Controls.Add(this.colourpallet17);
            this.flowcolours.Controls.Add(this.colourpallet18);
            this.flowcolours.Controls.Add(this.colourpallet19);
            this.flowcolours.Controls.Add(this.colourpallet20);
            this.flowcolours.Controls.Add(this.colourpallet21);
            this.flowcolours.Controls.Add(this.colourpallet22);
            this.flowcolours.Controls.Add(this.colourpallet23);
            this.flowcolours.Controls.Add(this.colourpallet24);
            this.flowcolours.Controls.Add(this.colourpallet25);
            this.flowcolours.Controls.Add(this.colourpallet26);
            this.flowcolours.Controls.Add(this.colourpallet27);
            this.flowcolours.Controls.Add(this.colourpallet28);
            this.flowcolours.Controls.Add(this.colourpallet29);
            this.flowcolours.Controls.Add(this.colourpallet30);
            this.flowcolours.Controls.Add(this.colourpallet31);
            this.flowcolours.Controls.Add(this.colourpallet32);
            this.flowcolours.Controls.Add(this.colourpallet33);
            this.flowcolours.Controls.Add(this.colourpallet34);
            this.flowcolours.Controls.Add(this.colourpallet35);
            this.flowcolours.Controls.Add(this.colourpallet36);
            this.flowcolours.Controls.Add(this.colourpallet37);
            this.flowcolours.Controls.Add(this.colourpallet38);
            this.flowcolours.Controls.Add(this.colourpallet39);
            this.flowcolours.Controls.Add(this.colourpallet40);
            this.flowcolours.Controls.Add(this.colourpallet41);
            this.flowcolours.Controls.Add(this.colourpallet42);
            this.flowcolours.Controls.Add(this.colourpallet43);
            this.flowcolours.Controls.Add(this.colourpallet44);
            this.flowcolours.Controls.Add(this.colourpallet45);
            this.flowcolours.Controls.Add(this.colourpallet46);
            this.flowcolours.Controls.Add(this.colourpallet47);
            this.flowcolours.Controls.Add(this.colourpallet48);
            this.flowcolours.Controls.Add(this.colourpallet49);
            this.flowcolours.Controls.Add(this.colourpallet50);
            this.flowcolours.Controls.Add(this.colourpallet51);
            this.flowcolours.Controls.Add(this.colourpallet52);
            this.flowcolours.Controls.Add(this.colourpallet53);
            this.flowcolours.Controls.Add(this.colourpallet54);
            this.flowcolours.Controls.Add(this.colourpallet55);
            this.flowcolours.Controls.Add(this.colourpallet56);
            this.flowcolours.Controls.Add(this.colourpallet57);
            this.flowcolours.Controls.Add(this.colourpallet58);
            this.flowcolours.Controls.Add(this.colourpallet59);
            this.flowcolours.Controls.Add(this.colourpallet60);
            this.flowcolours.Controls.Add(this.colourpallet61);
            this.flowcolours.Controls.Add(this.colourpallet62);
            this.flowcolours.Controls.Add(this.colourpallet63);
            this.flowcolours.Controls.Add(this.colourpallet64);
            this.flowcolours.Controls.Add(this.colourpallet65);
            this.flowcolours.Controls.Add(this.colourpallet66);
            this.flowcolours.Controls.Add(this.colourpallet67);
            this.flowcolours.Controls.Add(this.colourpallet68);
            this.flowcolours.Controls.Add(this.colourpallet69);
            this.flowcolours.Controls.Add(this.colourpallet70);
            this.flowcolours.Controls.Add(this.colourpallet71);
            this.flowcolours.Controls.Add(this.colourpallet72);
            this.flowcolours.Controls.Add(this.colourpallet73);
            this.flowcolours.Controls.Add(this.colourpallet74);
            this.flowcolours.Controls.Add(this.colourpallet75);
            this.flowcolours.Controls.Add(this.colourpallet76);
            this.flowcolours.Controls.Add(this.colourpallet77);
            this.flowcolours.Controls.Add(this.colourpallet78);
            this.flowcolours.Controls.Add(this.colourpallet79);
            this.flowcolours.Controls.Add(this.colourpallet80);
            this.flowcolours.Controls.Add(this.colourpallet81);
            this.flowcolours.Controls.Add(this.colourpallet82);
            this.flowcolours.Controls.Add(this.colourpallet83);
            this.flowcolours.Controls.Add(this.colourpallet84);
            this.flowcolours.Controls.Add(this.colourpallet85);
            this.flowcolours.Controls.Add(this.colourpallet86);
            this.flowcolours.Controls.Add(this.colourpallet87);
            this.flowcolours.Controls.Add(this.colourpallet88);
            this.flowcolours.Controls.Add(this.colourpallet89);
            this.flowcolours.Controls.Add(this.colourpallet90);
            this.flowcolours.Controls.Add(this.colourpallet91);
            this.flowcolours.Controls.Add(this.colourpallet92);
            this.flowcolours.Controls.Add(this.colourpallet93);
            this.flowcolours.Controls.Add(this.colourpallet94);
            this.flowcolours.Controls.Add(this.colourpallet95);
            this.flowcolours.Controls.Add(this.colourpallet96);
            this.flowcolours.Controls.Add(this.colourpallet97);
            this.flowcolours.Controls.Add(this.colourpallet98);
            this.flowcolours.Controls.Add(this.colourpallet99);
            this.flowcolours.Controls.Add(this.colourpallet100);
            this.flowcolours.Controls.Add(this.colourpallet101);
            this.flowcolours.Controls.Add(this.colourpallet102);
            this.flowcolours.Controls.Add(this.colourpallet103);
            this.flowcolours.Controls.Add(this.colourpallet104);
            this.flowcolours.Controls.Add(this.colourpallet105);
            this.flowcolours.Controls.Add(this.colourpallet106);
            this.flowcolours.Controls.Add(this.colourpallet107);
            this.flowcolours.Controls.Add(this.colourpallet108);
            this.flowcolours.Controls.Add(this.colourpallet109);
            this.flowcolours.Controls.Add(this.colourpallet110);
            this.flowcolours.Controls.Add(this.colourpallet111);
            this.flowcolours.Controls.Add(this.colourpallet112);
            this.flowcolours.Controls.Add(this.colourpallet113);
            this.flowcolours.Controls.Add(this.colourpallet114);
            this.flowcolours.Controls.Add(this.colourpallet115);
            this.flowcolours.Controls.Add(this.colourpallet116);
            this.flowcolours.Controls.Add(this.colourpallet117);
            this.flowcolours.Controls.Add(this.colourpallet118);
            this.flowcolours.Controls.Add(this.colourpallet119);
            this.flowcolours.Controls.Add(this.colourpallet120);
            this.flowcolours.Controls.Add(this.colourpallet121);
            this.flowcolours.Controls.Add(this.colourpallet122);
            this.flowcolours.Controls.Add(this.colourpallet123);
            this.flowcolours.Controls.Add(this.colourpallet124);
            this.flowcolours.Controls.Add(this.colourpallet125);
            this.flowcolours.Controls.Add(this.colourpallet126);
            this.flowcolours.Controls.Add(this.colourpallet127);
            this.flowcolours.Controls.Add(this.colourpallet128);
            this.flowcolours.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowcolours.Location = new System.Drawing.Point(0, 27);
            this.flowcolours.Name = "flowcolours";
            this.flowcolours.Padding = new System.Windows.Forms.Padding(1, 1, 0, 0);
            this.flowcolours.Size = new System.Drawing.Size(225, 73);
            this.flowcolours.TabIndex = 12;
            // 
            // colourpallet1
            // 
            this.colourpallet1.BackColor = System.Drawing.Color.Black;
            this.colourpallet1.Location = new System.Drawing.Point(2, 1);
            this.colourpallet1.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet1.Name = "colourpallet1";
            this.colourpallet1.Size = new System.Drawing.Size(12, 8);
            this.colourpallet1.TabIndex = 5;
            this.colourpallet1.Visible = false;
            // 
            // colourpallet2
            // 
            this.colourpallet2.BackColor = System.Drawing.Color.Black;
            this.colourpallet2.Location = new System.Drawing.Point(15, 1);
            this.colourpallet2.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet2.Name = "colourpallet2";
            this.colourpallet2.Size = new System.Drawing.Size(12, 8);
            this.colourpallet2.TabIndex = 9;
            this.colourpallet2.Visible = false;
            // 
            // colourpallet3
            // 
            this.colourpallet3.BackColor = System.Drawing.Color.Black;
            this.colourpallet3.Location = new System.Drawing.Point(28, 1);
            this.colourpallet3.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet3.Name = "colourpallet3";
            this.colourpallet3.Size = new System.Drawing.Size(12, 8);
            this.colourpallet3.TabIndex = 13;
            this.colourpallet3.Visible = false;
            // 
            // colourpallet4
            // 
            this.colourpallet4.BackColor = System.Drawing.Color.Black;
            this.colourpallet4.Location = new System.Drawing.Point(41, 1);
            this.colourpallet4.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet4.Name = "colourpallet4";
            this.colourpallet4.Size = new System.Drawing.Size(12, 8);
            this.colourpallet4.TabIndex = 9;
            this.colourpallet4.Visible = false;
            // 
            // colourpallet5
            // 
            this.colourpallet5.BackColor = System.Drawing.Color.Black;
            this.colourpallet5.Location = new System.Drawing.Point(54, 1);
            this.colourpallet5.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet5.Name = "colourpallet5";
            this.colourpallet5.Size = new System.Drawing.Size(12, 8);
            this.colourpallet5.TabIndex = 18;
            this.colourpallet5.Visible = false;
            // 
            // colourpallet6
            // 
            this.colourpallet6.BackColor = System.Drawing.Color.Black;
            this.colourpallet6.Location = new System.Drawing.Point(67, 1);
            this.colourpallet6.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet6.Name = "colourpallet6";
            this.colourpallet6.Size = new System.Drawing.Size(12, 8);
            this.colourpallet6.TabIndex = 17;
            this.colourpallet6.Visible = false;
            // 
            // colourpallet7
            // 
            this.colourpallet7.BackColor = System.Drawing.Color.Black;
            this.colourpallet7.Location = new System.Drawing.Point(80, 1);
            this.colourpallet7.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet7.Name = "colourpallet7";
            this.colourpallet7.Size = new System.Drawing.Size(12, 8);
            this.colourpallet7.TabIndex = 14;
            this.colourpallet7.Visible = false;
            // 
            // colourpallet8
            // 
            this.colourpallet8.BackColor = System.Drawing.Color.Black;
            this.colourpallet8.Location = new System.Drawing.Point(93, 1);
            this.colourpallet8.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet8.Name = "colourpallet8";
            this.colourpallet8.Size = new System.Drawing.Size(12, 8);
            this.colourpallet8.TabIndex = 13;
            this.colourpallet8.Visible = false;
            // 
            // colourpallet9
            // 
            this.colourpallet9.BackColor = System.Drawing.Color.Black;
            this.colourpallet9.Location = new System.Drawing.Point(106, 1);
            this.colourpallet9.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet9.Name = "colourpallet9";
            this.colourpallet9.Size = new System.Drawing.Size(12, 8);
            this.colourpallet9.TabIndex = 15;
            this.colourpallet9.Visible = false;
            // 
            // colourpallet10
            // 
            this.colourpallet10.BackColor = System.Drawing.Color.Black;
            this.colourpallet10.Location = new System.Drawing.Point(119, 1);
            this.colourpallet10.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet10.Name = "colourpallet10";
            this.colourpallet10.Size = new System.Drawing.Size(12, 8);
            this.colourpallet10.TabIndex = 11;
            this.colourpallet10.Visible = false;
            // 
            // colourpallet11
            // 
            this.colourpallet11.BackColor = System.Drawing.Color.Black;
            this.colourpallet11.Location = new System.Drawing.Point(132, 1);
            this.colourpallet11.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet11.Name = "colourpallet11";
            this.colourpallet11.Size = new System.Drawing.Size(12, 8);
            this.colourpallet11.TabIndex = 12;
            this.colourpallet11.Visible = false;
            // 
            // colourpallet12
            // 
            this.colourpallet12.BackColor = System.Drawing.Color.Black;
            this.colourpallet12.Location = new System.Drawing.Point(145, 1);
            this.colourpallet12.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet12.Name = "colourpallet12";
            this.colourpallet12.Size = new System.Drawing.Size(12, 8);
            this.colourpallet12.TabIndex = 16;
            this.colourpallet12.Visible = false;
            // 
            // colourpallet13
            // 
            this.colourpallet13.BackColor = System.Drawing.Color.Black;
            this.colourpallet13.Location = new System.Drawing.Point(158, 1);
            this.colourpallet13.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet13.Name = "colourpallet13";
            this.colourpallet13.Size = new System.Drawing.Size(12, 8);
            this.colourpallet13.TabIndex = 10;
            this.colourpallet13.Visible = false;
            // 
            // colourpallet14
            // 
            this.colourpallet14.BackColor = System.Drawing.Color.Black;
            this.colourpallet14.Location = new System.Drawing.Point(171, 1);
            this.colourpallet14.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet14.Name = "colourpallet14";
            this.colourpallet14.Size = new System.Drawing.Size(12, 8);
            this.colourpallet14.TabIndex = 20;
            this.colourpallet14.Visible = false;
            // 
            // colourpallet15
            // 
            this.colourpallet15.BackColor = System.Drawing.Color.Black;
            this.colourpallet15.Location = new System.Drawing.Point(184, 1);
            this.colourpallet15.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet15.Name = "colourpallet15";
            this.colourpallet15.Size = new System.Drawing.Size(12, 8);
            this.colourpallet15.TabIndex = 14;
            this.colourpallet15.Visible = false;
            // 
            // colourpallet16
            // 
            this.colourpallet16.BackColor = System.Drawing.Color.Black;
            this.colourpallet16.Location = new System.Drawing.Point(197, 1);
            this.colourpallet16.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet16.Name = "colourpallet16";
            this.colourpallet16.Size = new System.Drawing.Size(12, 8);
            this.colourpallet16.TabIndex = 16;
            this.colourpallet16.Visible = false;
            // 
            // colourpallet17
            // 
            this.colourpallet17.BackColor = System.Drawing.Color.Black;
            this.colourpallet17.Location = new System.Drawing.Point(210, 1);
            this.colourpallet17.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet17.Name = "colourpallet17";
            this.colourpallet17.Size = new System.Drawing.Size(12, 8);
            this.colourpallet17.TabIndex = 11;
            this.colourpallet17.Visible = false;
            // 
            // colourpallet18
            // 
            this.colourpallet18.BackColor = System.Drawing.Color.Black;
            this.colourpallet18.Location = new System.Drawing.Point(2, 10);
            this.colourpallet18.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet18.Name = "colourpallet18";
            this.colourpallet18.Size = new System.Drawing.Size(12, 8);
            this.colourpallet18.TabIndex = 8;
            this.colourpallet18.Visible = false;
            // 
            // colourpallet19
            // 
            this.colourpallet19.BackColor = System.Drawing.Color.Black;
            this.colourpallet19.Location = new System.Drawing.Point(15, 10);
            this.colourpallet19.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet19.Name = "colourpallet19";
            this.colourpallet19.Size = new System.Drawing.Size(12, 8);
            this.colourpallet19.TabIndex = 24;
            this.colourpallet19.Visible = false;
            // 
            // colourpallet20
            // 
            this.colourpallet20.BackColor = System.Drawing.Color.Black;
            this.colourpallet20.Location = new System.Drawing.Point(28, 10);
            this.colourpallet20.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet20.Name = "colourpallet20";
            this.colourpallet20.Size = new System.Drawing.Size(12, 8);
            this.colourpallet20.TabIndex = 12;
            this.colourpallet20.Visible = false;
            // 
            // colourpallet21
            // 
            this.colourpallet21.BackColor = System.Drawing.Color.Black;
            this.colourpallet21.Location = new System.Drawing.Point(41, 10);
            this.colourpallet21.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet21.Name = "colourpallet21";
            this.colourpallet21.Size = new System.Drawing.Size(12, 8);
            this.colourpallet21.TabIndex = 20;
            this.colourpallet21.Visible = false;
            // 
            // colourpallet22
            // 
            this.colourpallet22.BackColor = System.Drawing.Color.Black;
            this.colourpallet22.Location = new System.Drawing.Point(54, 10);
            this.colourpallet22.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet22.Name = "colourpallet22";
            this.colourpallet22.Size = new System.Drawing.Size(12, 8);
            this.colourpallet22.TabIndex = 8;
            this.colourpallet22.Visible = false;
            // 
            // colourpallet23
            // 
            this.colourpallet23.BackColor = System.Drawing.Color.Black;
            this.colourpallet23.Location = new System.Drawing.Point(67, 10);
            this.colourpallet23.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet23.Name = "colourpallet23";
            this.colourpallet23.Size = new System.Drawing.Size(12, 8);
            this.colourpallet23.TabIndex = 19;
            this.colourpallet23.Visible = false;
            // 
            // colourpallet24
            // 
            this.colourpallet24.BackColor = System.Drawing.Color.Black;
            this.colourpallet24.Location = new System.Drawing.Point(80, 10);
            this.colourpallet24.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet24.Name = "colourpallet24";
            this.colourpallet24.Size = new System.Drawing.Size(12, 8);
            this.colourpallet24.TabIndex = 10;
            this.colourpallet24.Visible = false;
            // 
            // colourpallet25
            // 
            this.colourpallet25.BackColor = System.Drawing.Color.Black;
            this.colourpallet25.Location = new System.Drawing.Point(93, 10);
            this.colourpallet25.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet25.Name = "colourpallet25";
            this.colourpallet25.Size = new System.Drawing.Size(12, 8);
            this.colourpallet25.TabIndex = 15;
            this.colourpallet25.Visible = false;
            // 
            // colourpallet26
            // 
            this.colourpallet26.BackColor = System.Drawing.Color.Black;
            this.colourpallet26.Location = new System.Drawing.Point(106, 10);
            this.colourpallet26.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet26.Name = "colourpallet26";
            this.colourpallet26.Size = new System.Drawing.Size(12, 8);
            this.colourpallet26.TabIndex = 23;
            this.colourpallet26.Visible = false;
            // 
            // colourpallet27
            // 
            this.colourpallet27.BackColor = System.Drawing.Color.Black;
            this.colourpallet27.Location = new System.Drawing.Point(119, 10);
            this.colourpallet27.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet27.Name = "colourpallet27";
            this.colourpallet27.Size = new System.Drawing.Size(12, 8);
            this.colourpallet27.TabIndex = 6;
            this.colourpallet27.Visible = false;
            // 
            // colourpallet28
            // 
            this.colourpallet28.BackColor = System.Drawing.Color.Black;
            this.colourpallet28.Location = new System.Drawing.Point(132, 10);
            this.colourpallet28.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet28.Name = "colourpallet28";
            this.colourpallet28.Size = new System.Drawing.Size(12, 8);
            this.colourpallet28.TabIndex = 19;
            this.colourpallet28.Visible = false;
            // 
            // colourpallet29
            // 
            this.colourpallet29.BackColor = System.Drawing.Color.Black;
            this.colourpallet29.Location = new System.Drawing.Point(145, 10);
            this.colourpallet29.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet29.Name = "colourpallet29";
            this.colourpallet29.Size = new System.Drawing.Size(12, 8);
            this.colourpallet29.TabIndex = 22;
            this.colourpallet29.Visible = false;
            // 
            // colourpallet30
            // 
            this.colourpallet30.BackColor = System.Drawing.Color.Black;
            this.colourpallet30.Location = new System.Drawing.Point(158, 10);
            this.colourpallet30.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet30.Name = "colourpallet30";
            this.colourpallet30.Size = new System.Drawing.Size(12, 8);
            this.colourpallet30.TabIndex = 18;
            this.colourpallet30.Visible = false;
            // 
            // colourpallet31
            // 
            this.colourpallet31.BackColor = System.Drawing.Color.Black;
            this.colourpallet31.Location = new System.Drawing.Point(171, 10);
            this.colourpallet31.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet31.Name = "colourpallet31";
            this.colourpallet31.Size = new System.Drawing.Size(12, 8);
            this.colourpallet31.TabIndex = 21;
            this.colourpallet31.Visible = false;
            // 
            // colourpallet32
            // 
            this.colourpallet32.BackColor = System.Drawing.Color.Black;
            this.colourpallet32.Location = new System.Drawing.Point(184, 10);
            this.colourpallet32.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet32.Name = "colourpallet32";
            this.colourpallet32.Size = new System.Drawing.Size(12, 8);
            this.colourpallet32.TabIndex = 17;
            this.colourpallet32.Visible = false;
            // 
            // colourpallet33
            // 
            this.colourpallet33.BackColor = System.Drawing.Color.Black;
            this.colourpallet33.Location = new System.Drawing.Point(197, 10);
            this.colourpallet33.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet33.Name = "colourpallet33";
            this.colourpallet33.Size = new System.Drawing.Size(12, 8);
            this.colourpallet33.TabIndex = 50;
            this.colourpallet33.Visible = false;
            // 
            // colourpallet34
            // 
            this.colourpallet34.BackColor = System.Drawing.Color.Black;
            this.colourpallet34.Location = new System.Drawing.Point(210, 10);
            this.colourpallet34.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet34.Name = "colourpallet34";
            this.colourpallet34.Size = new System.Drawing.Size(12, 8);
            this.colourpallet34.TabIndex = 46;
            this.colourpallet34.Visible = false;
            // 
            // colourpallet35
            // 
            this.colourpallet35.BackColor = System.Drawing.Color.Black;
            this.colourpallet35.Location = new System.Drawing.Point(2, 19);
            this.colourpallet35.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet35.Name = "colourpallet35";
            this.colourpallet35.Size = new System.Drawing.Size(12, 8);
            this.colourpallet35.TabIndex = 53;
            this.colourpallet35.Visible = false;
            // 
            // colourpallet36
            // 
            this.colourpallet36.BackColor = System.Drawing.Color.Black;
            this.colourpallet36.Location = new System.Drawing.Point(15, 19);
            this.colourpallet36.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet36.Name = "colourpallet36";
            this.colourpallet36.Size = new System.Drawing.Size(12, 8);
            this.colourpallet36.TabIndex = 48;
            this.colourpallet36.Visible = false;
            // 
            // colourpallet37
            // 
            this.colourpallet37.BackColor = System.Drawing.Color.Black;
            this.colourpallet37.Location = new System.Drawing.Point(28, 19);
            this.colourpallet37.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet37.Name = "colourpallet37";
            this.colourpallet37.Size = new System.Drawing.Size(12, 8);
            this.colourpallet37.TabIndex = 54;
            this.colourpallet37.Visible = false;
            // 
            // colourpallet38
            // 
            this.colourpallet38.BackColor = System.Drawing.Color.Black;
            this.colourpallet38.Location = new System.Drawing.Point(41, 19);
            this.colourpallet38.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet38.Name = "colourpallet38";
            this.colourpallet38.Size = new System.Drawing.Size(12, 8);
            this.colourpallet38.TabIndex = 26;
            this.colourpallet38.Visible = false;
            // 
            // colourpallet39
            // 
            this.colourpallet39.BackColor = System.Drawing.Color.Black;
            this.colourpallet39.Location = new System.Drawing.Point(54, 19);
            this.colourpallet39.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet39.Name = "colourpallet39";
            this.colourpallet39.Size = new System.Drawing.Size(12, 8);
            this.colourpallet39.TabIndex = 55;
            this.colourpallet39.Visible = false;
            // 
            // colourpallet40
            // 
            this.colourpallet40.BackColor = System.Drawing.Color.Black;
            this.colourpallet40.Location = new System.Drawing.Point(67, 19);
            this.colourpallet40.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet40.Name = "colourpallet40";
            this.colourpallet40.Size = new System.Drawing.Size(12, 8);
            this.colourpallet40.TabIndex = 41;
            this.colourpallet40.Visible = false;
            // 
            // colourpallet41
            // 
            this.colourpallet41.BackColor = System.Drawing.Color.Black;
            this.colourpallet41.Location = new System.Drawing.Point(80, 19);
            this.colourpallet41.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet41.Name = "colourpallet41";
            this.colourpallet41.Size = new System.Drawing.Size(12, 8);
            this.colourpallet41.TabIndex = 31;
            this.colourpallet41.Visible = false;
            // 
            // colourpallet42
            // 
            this.colourpallet42.BackColor = System.Drawing.Color.Black;
            this.colourpallet42.Location = new System.Drawing.Point(93, 19);
            this.colourpallet42.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet42.Name = "colourpallet42";
            this.colourpallet42.Size = new System.Drawing.Size(12, 8);
            this.colourpallet42.TabIndex = 49;
            this.colourpallet42.Visible = false;
            // 
            // colourpallet43
            // 
            this.colourpallet43.BackColor = System.Drawing.Color.Black;
            this.colourpallet43.Location = new System.Drawing.Point(106, 19);
            this.colourpallet43.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet43.Name = "colourpallet43";
            this.colourpallet43.Size = new System.Drawing.Size(12, 8);
            this.colourpallet43.TabIndex = 27;
            this.colourpallet43.Visible = false;
            // 
            // colourpallet44
            // 
            this.colourpallet44.BackColor = System.Drawing.Color.Black;
            this.colourpallet44.Location = new System.Drawing.Point(119, 19);
            this.colourpallet44.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet44.Name = "colourpallet44";
            this.colourpallet44.Size = new System.Drawing.Size(12, 8);
            this.colourpallet44.TabIndex = 51;
            this.colourpallet44.Visible = false;
            // 
            // colourpallet45
            // 
            this.colourpallet45.BackColor = System.Drawing.Color.Black;
            this.colourpallet45.Location = new System.Drawing.Point(132, 19);
            this.colourpallet45.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet45.Name = "colourpallet45";
            this.colourpallet45.Size = new System.Drawing.Size(12, 8);
            this.colourpallet45.TabIndex = 36;
            this.colourpallet45.Visible = false;
            // 
            // colourpallet46
            // 
            this.colourpallet46.BackColor = System.Drawing.Color.Black;
            this.colourpallet46.Location = new System.Drawing.Point(145, 19);
            this.colourpallet46.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet46.Name = "colourpallet46";
            this.colourpallet46.Size = new System.Drawing.Size(12, 8);
            this.colourpallet46.TabIndex = 56;
            this.colourpallet46.Visible = false;
            // 
            // colourpallet47
            // 
            this.colourpallet47.BackColor = System.Drawing.Color.Black;
            this.colourpallet47.Location = new System.Drawing.Point(158, 19);
            this.colourpallet47.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet47.Name = "colourpallet47";
            this.colourpallet47.Size = new System.Drawing.Size(12, 8);
            this.colourpallet47.TabIndex = 28;
            this.colourpallet47.Visible = false;
            // 
            // colourpallet48
            // 
            this.colourpallet48.BackColor = System.Drawing.Color.Black;
            this.colourpallet48.Location = new System.Drawing.Point(171, 19);
            this.colourpallet48.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet48.Name = "colourpallet48";
            this.colourpallet48.Size = new System.Drawing.Size(12, 8);
            this.colourpallet48.TabIndex = 33;
            this.colourpallet48.Visible = false;
            // 
            // colourpallet49
            // 
            this.colourpallet49.BackColor = System.Drawing.Color.Black;
            this.colourpallet49.Location = new System.Drawing.Point(184, 19);
            this.colourpallet49.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet49.Name = "colourpallet49";
            this.colourpallet49.Size = new System.Drawing.Size(12, 8);
            this.colourpallet49.TabIndex = 43;
            this.colourpallet49.Visible = false;
            // 
            // colourpallet50
            // 
            this.colourpallet50.BackColor = System.Drawing.Color.Black;
            this.colourpallet50.Location = new System.Drawing.Point(197, 19);
            this.colourpallet50.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet50.Name = "colourpallet50";
            this.colourpallet50.Size = new System.Drawing.Size(12, 8);
            this.colourpallet50.TabIndex = 40;
            this.colourpallet50.Visible = false;
            // 
            // colourpallet51
            // 
            this.colourpallet51.BackColor = System.Drawing.Color.Black;
            this.colourpallet51.Location = new System.Drawing.Point(210, 19);
            this.colourpallet51.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet51.Name = "colourpallet51";
            this.colourpallet51.Size = new System.Drawing.Size(12, 8);
            this.colourpallet51.TabIndex = 52;
            this.colourpallet51.Visible = false;
            // 
            // colourpallet52
            // 
            this.colourpallet52.BackColor = System.Drawing.Color.Black;
            this.colourpallet52.Location = new System.Drawing.Point(2, 28);
            this.colourpallet52.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet52.Name = "colourpallet52";
            this.colourpallet52.Size = new System.Drawing.Size(12, 8);
            this.colourpallet52.TabIndex = 32;
            this.colourpallet52.Visible = false;
            // 
            // colourpallet53
            // 
            this.colourpallet53.BackColor = System.Drawing.Color.Black;
            this.colourpallet53.Location = new System.Drawing.Point(15, 28);
            this.colourpallet53.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet53.Name = "colourpallet53";
            this.colourpallet53.Size = new System.Drawing.Size(12, 8);
            this.colourpallet53.TabIndex = 44;
            this.colourpallet53.Visible = false;
            // 
            // colourpallet54
            // 
            this.colourpallet54.BackColor = System.Drawing.Color.Black;
            this.colourpallet54.Location = new System.Drawing.Point(28, 28);
            this.colourpallet54.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet54.Name = "colourpallet54";
            this.colourpallet54.Size = new System.Drawing.Size(12, 8);
            this.colourpallet54.TabIndex = 35;
            this.colourpallet54.Visible = false;
            // 
            // colourpallet55
            // 
            this.colourpallet55.BackColor = System.Drawing.Color.Black;
            this.colourpallet55.Location = new System.Drawing.Point(41, 28);
            this.colourpallet55.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet55.Name = "colourpallet55";
            this.colourpallet55.Size = new System.Drawing.Size(12, 8);
            this.colourpallet55.TabIndex = 34;
            this.colourpallet55.Visible = false;
            // 
            // colourpallet56
            // 
            this.colourpallet56.BackColor = System.Drawing.Color.Black;
            this.colourpallet56.Location = new System.Drawing.Point(54, 28);
            this.colourpallet56.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet56.Name = "colourpallet56";
            this.colourpallet56.Size = new System.Drawing.Size(12, 8);
            this.colourpallet56.TabIndex = 42;
            this.colourpallet56.Visible = false;
            // 
            // colourpallet57
            // 
            this.colourpallet57.BackColor = System.Drawing.Color.Black;
            this.colourpallet57.Location = new System.Drawing.Point(67, 28);
            this.colourpallet57.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet57.Name = "colourpallet57";
            this.colourpallet57.Size = new System.Drawing.Size(12, 8);
            this.colourpallet57.TabIndex = 38;
            this.colourpallet57.Visible = false;
            // 
            // colourpallet58
            // 
            this.colourpallet58.BackColor = System.Drawing.Color.Black;
            this.colourpallet58.Location = new System.Drawing.Point(80, 28);
            this.colourpallet58.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet58.Name = "colourpallet58";
            this.colourpallet58.Size = new System.Drawing.Size(12, 8);
            this.colourpallet58.TabIndex = 39;
            this.colourpallet58.Visible = false;
            // 
            // colourpallet59
            // 
            this.colourpallet59.BackColor = System.Drawing.Color.Black;
            this.colourpallet59.Location = new System.Drawing.Point(93, 28);
            this.colourpallet59.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet59.Name = "colourpallet59";
            this.colourpallet59.Size = new System.Drawing.Size(12, 8);
            this.colourpallet59.TabIndex = 45;
            this.colourpallet59.Visible = false;
            // 
            // colourpallet60
            // 
            this.colourpallet60.BackColor = System.Drawing.Color.Black;
            this.colourpallet60.Location = new System.Drawing.Point(106, 28);
            this.colourpallet60.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet60.Name = "colourpallet60";
            this.colourpallet60.Size = new System.Drawing.Size(12, 8);
            this.colourpallet60.TabIndex = 47;
            this.colourpallet60.Visible = false;
            // 
            // colourpallet61
            // 
            this.colourpallet61.BackColor = System.Drawing.Color.Black;
            this.colourpallet61.Location = new System.Drawing.Point(119, 28);
            this.colourpallet61.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet61.Name = "colourpallet61";
            this.colourpallet61.Size = new System.Drawing.Size(12, 8);
            this.colourpallet61.TabIndex = 30;
            this.colourpallet61.Visible = false;
            // 
            // colourpallet62
            // 
            this.colourpallet62.BackColor = System.Drawing.Color.Black;
            this.colourpallet62.Location = new System.Drawing.Point(132, 28);
            this.colourpallet62.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet62.Name = "colourpallet62";
            this.colourpallet62.Size = new System.Drawing.Size(12, 8);
            this.colourpallet62.TabIndex = 37;
            this.colourpallet62.Visible = false;
            // 
            // colourpallet63
            // 
            this.colourpallet63.BackColor = System.Drawing.Color.Black;
            this.colourpallet63.Location = new System.Drawing.Point(145, 28);
            this.colourpallet63.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet63.Name = "colourpallet63";
            this.colourpallet63.Size = new System.Drawing.Size(12, 8);
            this.colourpallet63.TabIndex = 29;
            this.colourpallet63.Visible = false;
            // 
            // colourpallet64
            // 
            this.colourpallet64.BackColor = System.Drawing.Color.Black;
            this.colourpallet64.Location = new System.Drawing.Point(158, 28);
            this.colourpallet64.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet64.Name = "colourpallet64";
            this.colourpallet64.Size = new System.Drawing.Size(12, 8);
            this.colourpallet64.TabIndex = 25;
            this.colourpallet64.Visible = false;
            // 
            // colourpallet65
            // 
            this.colourpallet65.BackColor = System.Drawing.Color.Black;
            this.colourpallet65.Location = new System.Drawing.Point(171, 28);
            this.colourpallet65.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet65.Name = "colourpallet65";
            this.colourpallet65.Size = new System.Drawing.Size(12, 8);
            this.colourpallet65.TabIndex = 57;
            this.colourpallet65.Visible = false;
            // 
            // colourpallet66
            // 
            this.colourpallet66.BackColor = System.Drawing.Color.Black;
            this.colourpallet66.Location = new System.Drawing.Point(184, 28);
            this.colourpallet66.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet66.Name = "colourpallet66";
            this.colourpallet66.Size = new System.Drawing.Size(12, 8);
            this.colourpallet66.TabIndex = 61;
            this.colourpallet66.Visible = false;
            // 
            // colourpallet67
            // 
            this.colourpallet67.BackColor = System.Drawing.Color.Black;
            this.colourpallet67.Location = new System.Drawing.Point(197, 28);
            this.colourpallet67.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet67.Name = "colourpallet67";
            this.colourpallet67.Size = new System.Drawing.Size(12, 8);
            this.colourpallet67.TabIndex = 69;
            this.colourpallet67.Visible = false;
            // 
            // colourpallet68
            // 
            this.colourpallet68.BackColor = System.Drawing.Color.Black;
            this.colourpallet68.Location = new System.Drawing.Point(210, 28);
            this.colourpallet68.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet68.Name = "colourpallet68";
            this.colourpallet68.Size = new System.Drawing.Size(12, 8);
            this.colourpallet68.TabIndex = 62;
            this.colourpallet68.Visible = false;
            // 
            // colourpallet69
            // 
            this.colourpallet69.BackColor = System.Drawing.Color.Black;
            this.colourpallet69.Location = new System.Drawing.Point(2, 37);
            this.colourpallet69.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet69.Name = "colourpallet69";
            this.colourpallet69.Size = new System.Drawing.Size(12, 8);
            this.colourpallet69.TabIndex = 79;
            this.colourpallet69.Visible = false;
            // 
            // colourpallet70
            // 
            this.colourpallet70.BackColor = System.Drawing.Color.Black;
            this.colourpallet70.Location = new System.Drawing.Point(15, 37);
            this.colourpallet70.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet70.Name = "colourpallet70";
            this.colourpallet70.Size = new System.Drawing.Size(12, 8);
            this.colourpallet70.TabIndex = 77;
            this.colourpallet70.Visible = false;
            // 
            // colourpallet71
            // 
            this.colourpallet71.BackColor = System.Drawing.Color.Black;
            this.colourpallet71.Location = new System.Drawing.Point(28, 37);
            this.colourpallet71.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet71.Name = "colourpallet71";
            this.colourpallet71.Size = new System.Drawing.Size(12, 8);
            this.colourpallet71.TabIndex = 71;
            this.colourpallet71.Visible = false;
            // 
            // colourpallet72
            // 
            this.colourpallet72.BackColor = System.Drawing.Color.Black;
            this.colourpallet72.Location = new System.Drawing.Point(41, 37);
            this.colourpallet72.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet72.Name = "colourpallet72";
            this.colourpallet72.Size = new System.Drawing.Size(12, 8);
            this.colourpallet72.TabIndex = 70;
            this.colourpallet72.Visible = false;
            // 
            // colourpallet73
            // 
            this.colourpallet73.BackColor = System.Drawing.Color.Black;
            this.colourpallet73.Location = new System.Drawing.Point(54, 37);
            this.colourpallet73.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet73.Name = "colourpallet73";
            this.colourpallet73.Size = new System.Drawing.Size(12, 8);
            this.colourpallet73.TabIndex = 74;
            this.colourpallet73.Visible = false;
            // 
            // colourpallet74
            // 
            this.colourpallet74.BackColor = System.Drawing.Color.Black;
            this.colourpallet74.Location = new System.Drawing.Point(67, 37);
            this.colourpallet74.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet74.Name = "colourpallet74";
            this.colourpallet74.Size = new System.Drawing.Size(12, 8);
            this.colourpallet74.TabIndex = 66;
            this.colourpallet74.Visible = false;
            // 
            // colourpallet75
            // 
            this.colourpallet75.BackColor = System.Drawing.Color.Black;
            this.colourpallet75.Location = new System.Drawing.Point(80, 37);
            this.colourpallet75.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet75.Name = "colourpallet75";
            this.colourpallet75.Size = new System.Drawing.Size(12, 8);
            this.colourpallet75.TabIndex = 67;
            this.colourpallet75.Visible = false;
            // 
            // colourpallet76
            // 
            this.colourpallet76.BackColor = System.Drawing.Color.Black;
            this.colourpallet76.Location = new System.Drawing.Point(93, 37);
            this.colourpallet76.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet76.Name = "colourpallet76";
            this.colourpallet76.Size = new System.Drawing.Size(12, 8);
            this.colourpallet76.TabIndex = 76;
            this.colourpallet76.Visible = false;
            // 
            // colourpallet77
            // 
            this.colourpallet77.BackColor = System.Drawing.Color.Black;
            this.colourpallet77.Location = new System.Drawing.Point(106, 37);
            this.colourpallet77.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet77.Name = "colourpallet77";
            this.colourpallet77.Size = new System.Drawing.Size(12, 8);
            this.colourpallet77.TabIndex = 64;
            this.colourpallet77.Visible = false;
            // 
            // colourpallet78
            // 
            this.colourpallet78.BackColor = System.Drawing.Color.Black;
            this.colourpallet78.Location = new System.Drawing.Point(119, 37);
            this.colourpallet78.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet78.Name = "colourpallet78";
            this.colourpallet78.Size = new System.Drawing.Size(12, 8);
            this.colourpallet78.TabIndex = 84;
            this.colourpallet78.Visible = false;
            // 
            // colourpallet79
            // 
            this.colourpallet79.BackColor = System.Drawing.Color.Black;
            this.colourpallet79.Location = new System.Drawing.Point(132, 37);
            this.colourpallet79.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet79.Name = "colourpallet79";
            this.colourpallet79.Size = new System.Drawing.Size(12, 8);
            this.colourpallet79.TabIndex = 72;
            this.colourpallet79.Visible = false;
            // 
            // colourpallet80
            // 
            this.colourpallet80.BackColor = System.Drawing.Color.Black;
            this.colourpallet80.Location = new System.Drawing.Point(145, 37);
            this.colourpallet80.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet80.Name = "colourpallet80";
            this.colourpallet80.Size = new System.Drawing.Size(12, 8);
            this.colourpallet80.TabIndex = 75;
            this.colourpallet80.Visible = false;
            // 
            // colourpallet81
            // 
            this.colourpallet81.BackColor = System.Drawing.Color.Black;
            this.colourpallet81.Location = new System.Drawing.Point(158, 37);
            this.colourpallet81.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet81.Name = "colourpallet81";
            this.colourpallet81.Size = new System.Drawing.Size(12, 8);
            this.colourpallet81.TabIndex = 65;
            this.colourpallet81.Visible = false;
            // 
            // colourpallet82
            // 
            this.colourpallet82.BackColor = System.Drawing.Color.Black;
            this.colourpallet82.Location = new System.Drawing.Point(171, 37);
            this.colourpallet82.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet82.Name = "colourpallet82";
            this.colourpallet82.Size = new System.Drawing.Size(12, 8);
            this.colourpallet82.TabIndex = 60;
            this.colourpallet82.Visible = false;
            // 
            // colourpallet83
            // 
            this.colourpallet83.BackColor = System.Drawing.Color.Black;
            this.colourpallet83.Location = new System.Drawing.Point(184, 37);
            this.colourpallet83.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet83.Name = "colourpallet83";
            this.colourpallet83.Size = new System.Drawing.Size(12, 8);
            this.colourpallet83.TabIndex = 88;
            this.colourpallet83.Visible = false;
            // 
            // colourpallet84
            // 
            this.colourpallet84.BackColor = System.Drawing.Color.Black;
            this.colourpallet84.Location = new System.Drawing.Point(197, 37);
            this.colourpallet84.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet84.Name = "colourpallet84";
            this.colourpallet84.Size = new System.Drawing.Size(12, 8);
            this.colourpallet84.TabIndex = 68;
            this.colourpallet84.Visible = false;
            // 
            // colourpallet85
            // 
            this.colourpallet85.BackColor = System.Drawing.Color.Black;
            this.colourpallet85.Location = new System.Drawing.Point(210, 37);
            this.colourpallet85.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet85.Name = "colourpallet85";
            this.colourpallet85.Size = new System.Drawing.Size(12, 8);
            this.colourpallet85.TabIndex = 83;
            this.colourpallet85.Visible = false;
            // 
            // colourpallet86
            // 
            this.colourpallet86.BackColor = System.Drawing.Color.Black;
            this.colourpallet86.Location = new System.Drawing.Point(2, 46);
            this.colourpallet86.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet86.Name = "colourpallet86";
            this.colourpallet86.Size = new System.Drawing.Size(12, 8);
            this.colourpallet86.TabIndex = 59;
            this.colourpallet86.Visible = false;
            // 
            // colourpallet87
            // 
            this.colourpallet87.BackColor = System.Drawing.Color.Black;
            this.colourpallet87.Location = new System.Drawing.Point(15, 46);
            this.colourpallet87.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet87.Name = "colourpallet87";
            this.colourpallet87.Size = new System.Drawing.Size(12, 8);
            this.colourpallet87.TabIndex = 81;
            this.colourpallet87.Visible = false;
            // 
            // colourpallet88
            // 
            this.colourpallet88.BackColor = System.Drawing.Color.Black;
            this.colourpallet88.Location = new System.Drawing.Point(28, 46);
            this.colourpallet88.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet88.Name = "colourpallet88";
            this.colourpallet88.Size = new System.Drawing.Size(12, 8);
            this.colourpallet88.TabIndex = 63;
            this.colourpallet88.Visible = false;
            // 
            // colourpallet89
            // 
            this.colourpallet89.BackColor = System.Drawing.Color.Black;
            this.colourpallet89.Location = new System.Drawing.Point(41, 46);
            this.colourpallet89.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet89.Name = "colourpallet89";
            this.colourpallet89.Size = new System.Drawing.Size(12, 8);
            this.colourpallet89.TabIndex = 73;
            this.colourpallet89.Visible = false;
            // 
            // colourpallet90
            // 
            this.colourpallet90.BackColor = System.Drawing.Color.Black;
            this.colourpallet90.Location = new System.Drawing.Point(54, 46);
            this.colourpallet90.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet90.Name = "colourpallet90";
            this.colourpallet90.Size = new System.Drawing.Size(12, 8);
            this.colourpallet90.TabIndex = 87;
            this.colourpallet90.Visible = false;
            // 
            // colourpallet91
            // 
            this.colourpallet91.BackColor = System.Drawing.Color.Black;
            this.colourpallet91.Location = new System.Drawing.Point(67, 46);
            this.colourpallet91.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet91.Name = "colourpallet91";
            this.colourpallet91.Size = new System.Drawing.Size(12, 8);
            this.colourpallet91.TabIndex = 58;
            this.colourpallet91.Visible = false;
            // 
            // colourpallet92
            // 
            this.colourpallet92.BackColor = System.Drawing.Color.Black;
            this.colourpallet92.Location = new System.Drawing.Point(80, 46);
            this.colourpallet92.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet92.Name = "colourpallet92";
            this.colourpallet92.Size = new System.Drawing.Size(12, 8);
            this.colourpallet92.TabIndex = 82;
            this.colourpallet92.Visible = false;
            // 
            // colourpallet93
            // 
            this.colourpallet93.BackColor = System.Drawing.Color.Black;
            this.colourpallet93.Location = new System.Drawing.Point(93, 46);
            this.colourpallet93.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet93.Name = "colourpallet93";
            this.colourpallet93.Size = new System.Drawing.Size(12, 8);
            this.colourpallet93.TabIndex = 86;
            this.colourpallet93.Visible = false;
            // 
            // colourpallet94
            // 
            this.colourpallet94.BackColor = System.Drawing.Color.Black;
            this.colourpallet94.Location = new System.Drawing.Point(106, 46);
            this.colourpallet94.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet94.Name = "colourpallet94";
            this.colourpallet94.Size = new System.Drawing.Size(12, 8);
            this.colourpallet94.TabIndex = 80;
            this.colourpallet94.Visible = false;
            // 
            // colourpallet95
            // 
            this.colourpallet95.BackColor = System.Drawing.Color.Black;
            this.colourpallet95.Location = new System.Drawing.Point(119, 46);
            this.colourpallet95.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet95.Name = "colourpallet95";
            this.colourpallet95.Size = new System.Drawing.Size(12, 8);
            this.colourpallet95.TabIndex = 85;
            this.colourpallet95.Visible = false;
            // 
            // colourpallet96
            // 
            this.colourpallet96.BackColor = System.Drawing.Color.Black;
            this.colourpallet96.Location = new System.Drawing.Point(132, 46);
            this.colourpallet96.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet96.Name = "colourpallet96";
            this.colourpallet96.Size = new System.Drawing.Size(12, 8);
            this.colourpallet96.TabIndex = 78;
            this.colourpallet96.Visible = false;
            // 
            // colourpallet97
            // 
            this.colourpallet97.BackColor = System.Drawing.Color.Black;
            this.colourpallet97.Location = new System.Drawing.Point(145, 46);
            this.colourpallet97.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet97.Name = "colourpallet97";
            this.colourpallet97.Size = new System.Drawing.Size(12, 8);
            this.colourpallet97.TabIndex = 89;
            this.colourpallet97.Visible = false;
            // 
            // colourpallet98
            // 
            this.colourpallet98.BackColor = System.Drawing.Color.Black;
            this.colourpallet98.Location = new System.Drawing.Point(158, 46);
            this.colourpallet98.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet98.Name = "colourpallet98";
            this.colourpallet98.Size = new System.Drawing.Size(12, 8);
            this.colourpallet98.TabIndex = 93;
            this.colourpallet98.Visible = false;
            // 
            // colourpallet99
            // 
            this.colourpallet99.BackColor = System.Drawing.Color.Black;
            this.colourpallet99.Location = new System.Drawing.Point(171, 46);
            this.colourpallet99.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet99.Name = "colourpallet99";
            this.colourpallet99.Size = new System.Drawing.Size(12, 8);
            this.colourpallet99.TabIndex = 101;
            this.colourpallet99.Visible = false;
            // 
            // colourpallet100
            // 
            this.colourpallet100.BackColor = System.Drawing.Color.Black;
            this.colourpallet100.Location = new System.Drawing.Point(184, 46);
            this.colourpallet100.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet100.Name = "colourpallet100";
            this.colourpallet100.Size = new System.Drawing.Size(12, 8);
            this.colourpallet100.TabIndex = 94;
            this.colourpallet100.Visible = false;
            // 
            // colourpallet101
            // 
            this.colourpallet101.BackColor = System.Drawing.Color.Black;
            this.colourpallet101.Location = new System.Drawing.Point(197, 46);
            this.colourpallet101.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet101.Name = "colourpallet101";
            this.colourpallet101.Size = new System.Drawing.Size(12, 8);
            this.colourpallet101.TabIndex = 111;
            this.colourpallet101.Visible = false;
            // 
            // colourpallet102
            // 
            this.colourpallet102.BackColor = System.Drawing.Color.Black;
            this.colourpallet102.Location = new System.Drawing.Point(210, 46);
            this.colourpallet102.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet102.Name = "colourpallet102";
            this.colourpallet102.Size = new System.Drawing.Size(12, 8);
            this.colourpallet102.TabIndex = 110;
            this.colourpallet102.Visible = false;
            // 
            // colourpallet103
            // 
            this.colourpallet103.BackColor = System.Drawing.Color.Black;
            this.colourpallet103.Location = new System.Drawing.Point(2, 55);
            this.colourpallet103.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet103.Name = "colourpallet103";
            this.colourpallet103.Size = new System.Drawing.Size(12, 8);
            this.colourpallet103.TabIndex = 117;
            this.colourpallet103.Visible = false;
            // 
            // colourpallet104
            // 
            this.colourpallet104.BackColor = System.Drawing.Color.Black;
            this.colourpallet104.Location = new System.Drawing.Point(15, 55);
            this.colourpallet104.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet104.Name = "colourpallet104";
            this.colourpallet104.Size = new System.Drawing.Size(12, 8);
            this.colourpallet104.TabIndex = 112;
            this.colourpallet104.Visible = false;
            // 
            // colourpallet105
            // 
            this.colourpallet105.BackColor = System.Drawing.Color.Black;
            this.colourpallet105.Location = new System.Drawing.Point(28, 55);
            this.colourpallet105.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet105.Name = "colourpallet105";
            this.colourpallet105.Size = new System.Drawing.Size(12, 8);
            this.colourpallet105.TabIndex = 109;
            this.colourpallet105.Visible = false;
            // 
            // colourpallet106
            // 
            this.colourpallet106.BackColor = System.Drawing.Color.Black;
            this.colourpallet106.Location = new System.Drawing.Point(41, 55);
            this.colourpallet106.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet106.Name = "colourpallet106";
            this.colourpallet106.Size = new System.Drawing.Size(12, 8);
            this.colourpallet106.TabIndex = 103;
            this.colourpallet106.Visible = false;
            // 
            // colourpallet107
            // 
            this.colourpallet107.BackColor = System.Drawing.Color.Black;
            this.colourpallet107.Location = new System.Drawing.Point(54, 55);
            this.colourpallet107.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet107.Name = "colourpallet107";
            this.colourpallet107.Size = new System.Drawing.Size(12, 8);
            this.colourpallet107.TabIndex = 102;
            this.colourpallet107.Visible = false;
            // 
            // colourpallet108
            // 
            this.colourpallet108.BackColor = System.Drawing.Color.Black;
            this.colourpallet108.Location = new System.Drawing.Point(67, 55);
            this.colourpallet108.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet108.Name = "colourpallet108";
            this.colourpallet108.Size = new System.Drawing.Size(12, 8);
            this.colourpallet108.TabIndex = 106;
            this.colourpallet108.Visible = false;
            // 
            // colourpallet109
            // 
            this.colourpallet109.BackColor = System.Drawing.Color.Black;
            this.colourpallet109.Location = new System.Drawing.Point(80, 55);
            this.colourpallet109.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet109.Name = "colourpallet109";
            this.colourpallet109.Size = new System.Drawing.Size(12, 8);
            this.colourpallet109.TabIndex = 98;
            this.colourpallet109.Visible = false;
            // 
            // colourpallet110
            // 
            this.colourpallet110.BackColor = System.Drawing.Color.Black;
            this.colourpallet110.Location = new System.Drawing.Point(93, 55);
            this.colourpallet110.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet110.Name = "colourpallet110";
            this.colourpallet110.Size = new System.Drawing.Size(12, 8);
            this.colourpallet110.TabIndex = 99;
            this.colourpallet110.Visible = false;
            // 
            // colourpallet111
            // 
            this.colourpallet111.BackColor = System.Drawing.Color.Black;
            this.colourpallet111.Location = new System.Drawing.Point(106, 55);
            this.colourpallet111.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet111.Name = "colourpallet111";
            this.colourpallet111.Size = new System.Drawing.Size(12, 8);
            this.colourpallet111.TabIndex = 108;
            this.colourpallet111.Visible = false;
            // 
            // colourpallet112
            // 
            this.colourpallet112.BackColor = System.Drawing.Color.Black;
            this.colourpallet112.Location = new System.Drawing.Point(119, 55);
            this.colourpallet112.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet112.Name = "colourpallet112";
            this.colourpallet112.Size = new System.Drawing.Size(12, 8);
            this.colourpallet112.TabIndex = 96;
            this.colourpallet112.Visible = false;
            // 
            // colourpallet113
            // 
            this.colourpallet113.BackColor = System.Drawing.Color.Black;
            this.colourpallet113.Location = new System.Drawing.Point(132, 55);
            this.colourpallet113.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet113.Name = "colourpallet113";
            this.colourpallet113.Size = new System.Drawing.Size(12, 8);
            this.colourpallet113.TabIndex = 116;
            this.colourpallet113.Visible = false;
            // 
            // colourpallet114
            // 
            this.colourpallet114.BackColor = System.Drawing.Color.Black;
            this.colourpallet114.Location = new System.Drawing.Point(145, 55);
            this.colourpallet114.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet114.Name = "colourpallet114";
            this.colourpallet114.Size = new System.Drawing.Size(12, 8);
            this.colourpallet114.TabIndex = 104;
            this.colourpallet114.Visible = false;
            // 
            // colourpallet115
            // 
            this.colourpallet115.BackColor = System.Drawing.Color.Black;
            this.colourpallet115.Location = new System.Drawing.Point(158, 55);
            this.colourpallet115.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet115.Name = "colourpallet115";
            this.colourpallet115.Size = new System.Drawing.Size(12, 8);
            this.colourpallet115.TabIndex = 107;
            this.colourpallet115.Visible = false;
            // 
            // colourpallet116
            // 
            this.colourpallet116.BackColor = System.Drawing.Color.Black;
            this.colourpallet116.Location = new System.Drawing.Point(171, 55);
            this.colourpallet116.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet116.Name = "colourpallet116";
            this.colourpallet116.Size = new System.Drawing.Size(12, 8);
            this.colourpallet116.TabIndex = 97;
            this.colourpallet116.Visible = false;
            // 
            // colourpallet117
            // 
            this.colourpallet117.BackColor = System.Drawing.Color.Black;
            this.colourpallet117.Location = new System.Drawing.Point(184, 55);
            this.colourpallet117.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet117.Name = "colourpallet117";
            this.colourpallet117.Size = new System.Drawing.Size(12, 8);
            this.colourpallet117.TabIndex = 92;
            this.colourpallet117.Visible = false;
            // 
            // colourpallet118
            // 
            this.colourpallet118.BackColor = System.Drawing.Color.Black;
            this.colourpallet118.Location = new System.Drawing.Point(197, 55);
            this.colourpallet118.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet118.Name = "colourpallet118";
            this.colourpallet118.Size = new System.Drawing.Size(12, 8);
            this.colourpallet118.TabIndex = 120;
            this.colourpallet118.Visible = false;
            // 
            // colourpallet119
            // 
            this.colourpallet119.BackColor = System.Drawing.Color.Black;
            this.colourpallet119.Location = new System.Drawing.Point(210, 55);
            this.colourpallet119.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet119.Name = "colourpallet119";
            this.colourpallet119.Size = new System.Drawing.Size(12, 8);
            this.colourpallet119.TabIndex = 100;
            this.colourpallet119.Visible = false;
            // 
            // colourpallet120
            // 
            this.colourpallet120.BackColor = System.Drawing.Color.Black;
            this.colourpallet120.Location = new System.Drawing.Point(2, 64);
            this.colourpallet120.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet120.Name = "colourpallet120";
            this.colourpallet120.Size = new System.Drawing.Size(12, 8);
            this.colourpallet120.TabIndex = 115;
            this.colourpallet120.Visible = false;
            // 
            // colourpallet121
            // 
            this.colourpallet121.BackColor = System.Drawing.Color.Black;
            this.colourpallet121.Location = new System.Drawing.Point(15, 64);
            this.colourpallet121.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet121.Name = "colourpallet121";
            this.colourpallet121.Size = new System.Drawing.Size(12, 8);
            this.colourpallet121.TabIndex = 91;
            this.colourpallet121.Visible = false;
            // 
            // colourpallet122
            // 
            this.colourpallet122.BackColor = System.Drawing.Color.Black;
            this.colourpallet122.Location = new System.Drawing.Point(28, 64);
            this.colourpallet122.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet122.Name = "colourpallet122";
            this.colourpallet122.Size = new System.Drawing.Size(12, 8);
            this.colourpallet122.TabIndex = 113;
            this.colourpallet122.Visible = false;
            // 
            // colourpallet123
            // 
            this.colourpallet123.BackColor = System.Drawing.Color.Black;
            this.colourpallet123.Location = new System.Drawing.Point(41, 64);
            this.colourpallet123.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet123.Name = "colourpallet123";
            this.colourpallet123.Size = new System.Drawing.Size(12, 8);
            this.colourpallet123.TabIndex = 95;
            this.colourpallet123.Visible = false;
            // 
            // colourpallet124
            // 
            this.colourpallet124.BackColor = System.Drawing.Color.Black;
            this.colourpallet124.Location = new System.Drawing.Point(54, 64);
            this.colourpallet124.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet124.Name = "colourpallet124";
            this.colourpallet124.Size = new System.Drawing.Size(12, 8);
            this.colourpallet124.TabIndex = 105;
            this.colourpallet124.Visible = false;
            // 
            // colourpallet125
            // 
            this.colourpallet125.BackColor = System.Drawing.Color.Black;
            this.colourpallet125.Location = new System.Drawing.Point(67, 64);
            this.colourpallet125.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet125.Name = "colourpallet125";
            this.colourpallet125.Size = new System.Drawing.Size(12, 8);
            this.colourpallet125.TabIndex = 119;
            this.colourpallet125.Visible = false;
            // 
            // colourpallet126
            // 
            this.colourpallet126.BackColor = System.Drawing.Color.Black;
            this.colourpallet126.Location = new System.Drawing.Point(80, 64);
            this.colourpallet126.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet126.Name = "colourpallet126";
            this.colourpallet126.Size = new System.Drawing.Size(12, 8);
            this.colourpallet126.TabIndex = 90;
            this.colourpallet126.Visible = false;
            // 
            // colourpallet127
            // 
            this.colourpallet127.BackColor = System.Drawing.Color.Black;
            this.colourpallet127.Location = new System.Drawing.Point(93, 64);
            this.colourpallet127.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet127.Name = "colourpallet127";
            this.colourpallet127.Size = new System.Drawing.Size(12, 8);
            this.colourpallet127.TabIndex = 114;
            this.colourpallet127.Visible = false;
            // 
            // colourpallet128
            // 
            this.colourpallet128.BackColor = System.Drawing.Color.Black;
            this.colourpallet128.Location = new System.Drawing.Point(106, 64);
            this.colourpallet128.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.colourpallet128.Name = "colourpallet128";
            this.colourpallet128.Size = new System.Drawing.Size(12, 8);
            this.colourpallet128.TabIndex = 118;
            this.colourpallet128.Visible = false;
            // 
            // Label4
            // 
            this.Label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(0, 1);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(225, 15);
            this.Label4.TabIndex = 11;
            this.Label4.Text = "Colors";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // line6
            // 
            this.line6.BackColor = System.Drawing.Color.Black;
            this.line6.Dock = System.Windows.Forms.DockStyle.Top;
            this.line6.Location = new System.Drawing.Point(0, 0);
            this.line6.Name = "line6";
            this.line6.Size = new System.Drawing.Size(225, 1);
            this.line6.TabIndex = 4;
            // 
            // pnltoolproperties
            // 
            this.pnltoolproperties.Controls.Add(this.pnlmagnifiersettings);
            this.pnltoolproperties.Controls.Add(this.pnleracertoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnlpixelplacersettings);
            this.pnltoolproperties.Controls.Add(this.pnlovaltoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnllinetoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnlpaintbrushtoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnltexttoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnlpixelsettersettings);
            this.pnltoolproperties.Controls.Add(this.pnlfloodfillsettings);
            this.pnltoolproperties.Controls.Add(this.pnlsquaretoolsettings);
            this.pnltoolproperties.Controls.Add(this.pnlpencilsettings);
            this.pnltoolproperties.Controls.Add(this.Label6);
            this.pnltoolproperties.Controls.Add(this.Label5);
            this.pnltoolproperties.Controls.Add(this.line5);
            this.pnltoolproperties.Controls.Add(this.line3);
            this.pnltoolproperties.Location = new System.Drawing.Point(0, 0);
            this.pnltoolproperties.Name = "pnltoolproperties";
            this.pnltoolproperties.Size = new System.Drawing.Size(457, 100);
            this.pnltoolproperties.TabIndex = 1;
            // 
            // pnlmagnifiersettings
            // 
            this.pnlmagnifiersettings.Controls.Add(this.btnzoomout);
            this.pnlmagnifiersettings.Controls.Add(this.btnzoomin);
            this.pnlmagnifiersettings.Controls.Add(this.lblzoomlevel);
            this.pnlmagnifiersettings.Controls.Add(this.Label7);
            this.pnlmagnifiersettings.Location = new System.Drawing.Point(78, 50);
            this.pnlmagnifiersettings.Name = "pnlmagnifiersettings";
            this.pnlmagnifiersettings.Size = new System.Drawing.Size(67, 44);
            this.pnlmagnifiersettings.TabIndex = 6;
            this.pnlmagnifiersettings.Visible = false;
            // 
            // btnzoomout
            // 
            this.btnzoomout.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnzoomout.Location = new System.Drawing.Point(16, 33);
            this.btnzoomout.Name = "btnzoomout";
            this.btnzoomout.Size = new System.Drawing.Size(129, 56);
            this.btnzoomout.TabIndex = 11;
            this.btnzoomout.Text = "-";
            this.btnzoomout.UseVisualStyleBackColor = true;
            this.btnzoomout.Click += new System.EventHandler(this.btnzoomout_Click);
            // 
            // btnzoomin
            // 
            this.btnzoomin.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnzoomin.Location = new System.Drawing.Point(313, 33);
            this.btnzoomin.Name = "btnzoomin";
            this.btnzoomin.Size = new System.Drawing.Size(129, 56);
            this.btnzoomin.TabIndex = 10;
            this.btnzoomin.Text = "+";
            this.btnzoomin.UseVisualStyleBackColor = true;
            this.btnzoomin.Click += new System.EventHandler(this.btnzoomin_Click);
            // 
            // lblzoomlevel
            // 
            this.lblzoomlevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblzoomlevel.Location = new System.Drawing.Point(151, 33);
            this.lblzoomlevel.Name = "lblzoomlevel";
            this.lblzoomlevel.Size = new System.Drawing.Size(156, 56);
            this.lblzoomlevel.TabIndex = 1;
            this.lblzoomlevel.Text = "1X";
            this.lblzoomlevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label7
            // 
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(6, 3);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(444, 23);
            this.Label7.TabIndex = 0;
            this.Label7.Text = "Magnifier Settings";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnleracertoolsettings
            // 
            this.pnleracertoolsettings.Controls.Add(this.Label28);
            this.pnleracertoolsettings.Controls.Add(this.btneracersquare);
            this.pnleracertoolsettings.Controls.Add(this.btneracercircle);
            this.pnleracertoolsettings.Controls.Add(this.Label24);
            this.pnleracertoolsettings.Controls.Add(this.txteracersize);
            this.pnleracertoolsettings.Controls.Add(this.Label26);
            this.pnleracertoolsettings.Controls.Add(this.Label27);
            this.pnleracertoolsettings.Location = new System.Drawing.Point(0, 1);
            this.pnleracertoolsettings.Name = "pnleracertoolsettings";
            this.pnleracertoolsettings.Size = new System.Drawing.Size(459, 96);
            this.pnleracertoolsettings.TabIndex = 16;
            this.pnleracertoolsettings.Visible = false;
            // 
            // Label28
            // 
            this.Label28.AutoSize = true;
            this.Label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label28.Location = new System.Drawing.Point(40, 71);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(65, 24);
            this.Label28.TabIndex = 19;
            this.Label28.Text = "Shape";
            // 
            // btneracersquare
            // 
            this.btneracersquare.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadsquarerubberselected;
            this.btneracersquare.FlatAppearance.BorderSize = 0;
            this.btneracersquare.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btneracersquare.Location = new System.Drawing.Point(75, 15);
            this.btneracersquare.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btneracersquare.Name = "btneracersquare";
            this.btneracersquare.Size = new System.Drawing.Size(60, 50);
            this.btneracersquare.TabIndex = 17;
            this.btneracersquare.Text = " ";
            this.btneracersquare.UseVisualStyleBackColor = true;
            this.btneracersquare.Click += new System.EventHandler(this.btneracersquare_Click);
            // 
            // btneracercircle
            // 
            this.btneracercircle.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadcirclerubber;
            this.btneracercircle.FlatAppearance.BorderSize = 0;
            this.btneracercircle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btneracercircle.Location = new System.Drawing.Point(9, 15);
            this.btneracercircle.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btneracercircle.Name = "btneracercircle";
            this.btneracercircle.Size = new System.Drawing.Size(60, 50);
            this.btneracercircle.TabIndex = 16;
            this.btneracercircle.Text = " ";
            this.btneracercircle.UseVisualStyleBackColor = true;
            this.btneracercircle.Click += new System.EventHandler(this.btneracercircle_Click);
            // 
            // Label24
            // 
            this.Label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label24.Location = new System.Drawing.Point(141, 25);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(314, 35);
            this.Label24.TabIndex = 15;
            this.Label24.Text = "Choose a shape and size for your eracer then rub out unwanted parts of your drawi" +
    "ng with the mouse.";
            // 
            // txteracersize
            // 
            this.txteracersize.BackColor = System.Drawing.Color.White;
            this.txteracersize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txteracersize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txteracersize.ForeColor = System.Drawing.Color.Black;
            this.txteracersize.Location = new System.Drawing.Point(198, 69);
            this.txteracersize.Name = "txteracersize";
            this.txteracersize.Size = new System.Drawing.Size(73, 26);
            this.txteracersize.TabIndex = 12;
            this.txteracersize.TextChanged += new System.EventHandler(this.txteracersize_TextChanged);
            // 
            // Label26
            // 
            this.Label26.AutoSize = true;
            this.Label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label26.Location = new System.Drawing.Point(141, 71);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(51, 24);
            this.Label26.TabIndex = 10;
            this.Label26.Text = "Size:";
            // 
            // Label27
            // 
            this.Label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label27.Location = new System.Drawing.Point(6, 3);
            this.Label27.Name = "Label27";
            this.Label27.Size = new System.Drawing.Size(444, 18);
            this.Label27.TabIndex = 0;
            this.Label27.Text = "Eraser Tool Settings";
            this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlpixelplacersettings
            // 
            this.pnlpixelplacersettings.Controls.Add(this.lblpixelplacerhelp);
            this.pnlpixelplacersettings.Controls.Add(this.btnpixelplacermovementmode);
            this.pnlpixelplacersettings.Controls.Add(this.Label8);
            this.pnlpixelplacersettings.Location = new System.Drawing.Point(0, 1);
            this.pnlpixelplacersettings.Name = "pnlpixelplacersettings";
            this.pnlpixelplacersettings.Size = new System.Drawing.Size(454, 98);
            this.pnlpixelplacersettings.TabIndex = 7;
            this.pnlpixelplacersettings.Visible = false;
            // 
            // lblpixelplacerhelp
            // 
            this.lblpixelplacerhelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblpixelplacerhelp.Location = new System.Drawing.Point(3, 26);
            this.lblpixelplacerhelp.Name = "lblpixelplacerhelp";
            this.lblpixelplacerhelp.Size = new System.Drawing.Size(307, 67);
            this.lblpixelplacerhelp.TabIndex = 11;
            this.lblpixelplacerhelp.Text = "This tool does not contain any alterable settings. Simply click on the canvas and" +
    " a pixel will be placed in the spot you click.";
            this.lblpixelplacerhelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnpixelplacermovementmode
            // 
            this.btnpixelplacermovementmode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpixelplacermovementmode.Location = new System.Drawing.Point(312, 26);
            this.btnpixelplacermovementmode.Name = "btnpixelplacermovementmode";
            this.btnpixelplacermovementmode.Size = new System.Drawing.Size(138, 67);
            this.btnpixelplacermovementmode.TabIndex = 10;
            this.btnpixelplacermovementmode.Text = "Activate Movement Mode";
            this.btnpixelplacermovementmode.UseVisualStyleBackColor = true;
            this.btnpixelplacermovementmode.Click += new System.EventHandler(this.btnpixelplacermovementmode_Click);
            // 
            // Label8
            // 
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(6, 3);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(444, 23);
            this.Label8.TabIndex = 0;
            this.Label8.Text = "Pixel Placer Settings";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlovaltoolsettings
            // 
            this.pnlovaltoolsettings.Controls.Add(this.Label20);
            this.pnlovaltoolsettings.Controls.Add(this.btnovalfillonoff);
            this.pnlovaltoolsettings.Controls.Add(this.pnlovalfillcolour);
            this.pnlovaltoolsettings.Controls.Add(this.txtovalborderwidth);
            this.pnlovaltoolsettings.Controls.Add(this.Label21);
            this.pnlovaltoolsettings.Controls.Add(this.Label22);
            this.pnlovaltoolsettings.Controls.Add(this.Label23);
            this.pnlovaltoolsettings.Location = new System.Drawing.Point(0, 1);
            this.pnlovaltoolsettings.Name = "pnlovaltoolsettings";
            this.pnlovaltoolsettings.Size = new System.Drawing.Size(459, 96);
            this.pnlovaltoolsettings.TabIndex = 15;
            this.pnlovaltoolsettings.Visible = false;
            // 
            // Label20
            // 
            this.Label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label20.Location = new System.Drawing.Point(211, 29);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(244, 65);
            this.Label20.TabIndex = 15;
            this.Label20.Text = "Set a border width and turn fill on or off then draw the square on the canvas wit" +
    "h the mouse. Click the fill colour box to set it to your currently selected colo" +
    "ur.";
            // 
            // btnovalfillonoff
            // 
            this.btnovalfillonoff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnovalfillonoff.Location = new System.Drawing.Point(150, 64);
            this.btnovalfillonoff.Name = "btnovalfillonoff";
            this.btnovalfillonoff.Size = new System.Drawing.Size(56, 28);
            this.btnovalfillonoff.TabIndex = 14;
            this.btnovalfillonoff.Text = "Fill OFF";
            this.btnovalfillonoff.UseVisualStyleBackColor = true;
            this.btnovalfillonoff.Click += new System.EventHandler(this.btnovalfillonoff_Click);
            // 
            // pnlovalfillcolour
            // 
            this.pnlovalfillcolour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlovalfillcolour.Location = new System.Drawing.Point(109, 64);
            this.pnlovalfillcolour.Name = "pnlovalfillcolour";
            this.pnlovalfillcolour.Size = new System.Drawing.Size(34, 28);
            this.pnlovalfillcolour.TabIndex = 13;
            this.pnlovalfillcolour.Click += new System.EventHandler(this.pnlovalfillcolour_Click);
            // 
            // txtovalborderwidth
            // 
            this.txtovalborderwidth.BackColor = System.Drawing.Color.White;
            this.txtovalborderwidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtovalborderwidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtovalborderwidth.ForeColor = System.Drawing.Color.Black;
            this.txtovalborderwidth.Location = new System.Drawing.Point(133, 33);
            this.txtovalborderwidth.Name = "txtovalborderwidth";
            this.txtovalborderwidth.Size = new System.Drawing.Size(73, 26);
            this.txtovalborderwidth.TabIndex = 12;
            this.txtovalborderwidth.TextChanged += new System.EventHandler(this.txtovalborderwidth_TextChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(4, 66);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(100, 24);
            this.Label21.TabIndex = 11;
            this.Label21.Text = "Fill Colour:";
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label22.Location = new System.Drawing.Point(4, 33);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(125, 24);
            this.Label22.TabIndex = 10;
            this.Label22.Text = "Border Width:";
            // 
            // Label23
            // 
            this.Label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label23.Location = new System.Drawing.Point(6, 3);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(444, 23);
            this.Label23.TabIndex = 0;
            this.Label23.Text = "Oval Tool Settings";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnllinetoolsettings
            // 
            this.pnllinetoolsettings.Controls.Add(this.Label29);
            this.pnllinetoolsettings.Controls.Add(this.txtlinewidth);
            this.pnllinetoolsettings.Controls.Add(this.Label30);
            this.pnllinetoolsettings.Controls.Add(this.Label31);
            this.pnllinetoolsettings.Location = new System.Drawing.Point(168, 32);
            this.pnllinetoolsettings.Name = "pnllinetoolsettings";
            this.pnllinetoolsettings.Size = new System.Drawing.Size(60, 28);
            this.pnllinetoolsettings.TabIndex = 20;
            this.pnllinetoolsettings.Visible = false;
            // 
            // Label29
            // 
            this.Label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label29.Location = new System.Drawing.Point(196, 29);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(257, 38);
            this.Label29.TabIndex = 15;
            this.Label29.Text = "Enter a line width then use the mouse to draw a straight line on the canvas.";
            // 
            // txtlinewidth
            // 
            this.txtlinewidth.BackColor = System.Drawing.Color.White;
            this.txtlinewidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtlinewidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlinewidth.ForeColor = System.Drawing.Color.Black;
            this.txtlinewidth.Location = new System.Drawing.Point(114, 33);
            this.txtlinewidth.Name = "txtlinewidth";
            this.txtlinewidth.Size = new System.Drawing.Size(73, 26);
            this.txtlinewidth.TabIndex = 12;
            this.txtlinewidth.TextAlignChanged += new System.EventHandler(this.txtlinewidth_TextChanged);
            // 
            // Label30
            // 
            this.Label30.AutoSize = true;
            this.Label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label30.Location = new System.Drawing.Point(8, 34);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(104, 24);
            this.Label30.TabIndex = 10;
            this.Label30.Text = "Line Width:";
            // 
            // Label31
            // 
            this.Label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label31.Location = new System.Drawing.Point(6, 3);
            this.Label31.Name = "Label31";
            this.Label31.Size = new System.Drawing.Size(444, 23);
            this.Label31.TabIndex = 0;
            this.Label31.Text = "Line Tool Settings";
            this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlpaintbrushtoolsettings
            // 
            this.pnlpaintbrushtoolsettings.Controls.Add(this.Label36);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.btnpaintsquareshape);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.btnpaintcircleshape);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.Label37);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.txtpaintbrushsize);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.Label38);
            this.pnlpaintbrushtoolsettings.Controls.Add(this.Label39);
            this.pnlpaintbrushtoolsettings.Location = new System.Drawing.Point(0, 1);
            this.pnlpaintbrushtoolsettings.Name = "pnlpaintbrushtoolsettings";
            this.pnlpaintbrushtoolsettings.Size = new System.Drawing.Size(457, 97);
            this.pnlpaintbrushtoolsettings.TabIndex = 21;
            this.pnlpaintbrushtoolsettings.Visible = false;
            // 
            // Label36
            // 
            this.Label36.AutoSize = true;
            this.Label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label36.Location = new System.Drawing.Point(32, 69);
            this.Label36.Name = "Label36";
            this.Label36.Size = new System.Drawing.Size(65, 24);
            this.Label36.TabIndex = 19;
            this.Label36.Text = "Shape";
            // 
            // btnpaintsquareshape
            // 
            this.btnpaintsquareshape.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadsquarerubber;
            this.btnpaintsquareshape.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpaintsquareshape.FlatAppearance.BorderSize = 0;
            this.btnpaintsquareshape.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpaintsquareshape.Location = new System.Drawing.Point(69, 27);
            this.btnpaintsquareshape.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpaintsquareshape.Name = "btnpaintsquareshape";
            this.btnpaintsquareshape.Size = new System.Drawing.Size(48, 40);
            this.btnpaintsquareshape.TabIndex = 17;
            this.btnpaintsquareshape.Text = " ";
            this.btnpaintsquareshape.UseVisualStyleBackColor = true;
            this.btnpaintsquareshape.Click += new System.EventHandler(this.btnpaintsquareshape_Click);
            // 
            // btnpaintcircleshape
            // 
            this.btnpaintcircleshape.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadcirclerubberselected;
            this.btnpaintcircleshape.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpaintcircleshape.FlatAppearance.BorderSize = 0;
            this.btnpaintcircleshape.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpaintcircleshape.Location = new System.Drawing.Point(16, 27);
            this.btnpaintcircleshape.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpaintcircleshape.Name = "btnpaintcircleshape";
            this.btnpaintcircleshape.Size = new System.Drawing.Size(48, 40);
            this.btnpaintcircleshape.TabIndex = 16;
            this.btnpaintcircleshape.Text = " ";
            this.btnpaintcircleshape.UseVisualStyleBackColor = true;
            this.btnpaintcircleshape.Click += new System.EventHandler(this.btnpaintcircleshape_Click);
            // 
            // Label37
            // 
            this.Label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label37.Location = new System.Drawing.Point(137, 29);
            this.Label37.Name = "Label37";
            this.Label37.Size = new System.Drawing.Size(314, 38);
            this.Label37.TabIndex = 15;
            this.Label37.Text = "Choose a shape and size for your paint brush then paint on the canvas by drawing " +
    "with the mouse.";
            // 
            // txtpaintbrushsize
            // 
            this.txtpaintbrushsize.BackColor = System.Drawing.Color.White;
            this.txtpaintbrushsize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpaintbrushsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpaintbrushsize.ForeColor = System.Drawing.Color.Black;
            this.txtpaintbrushsize.Location = new System.Drawing.Point(194, 68);
            this.txtpaintbrushsize.Name = "txtpaintbrushsize";
            this.txtpaintbrushsize.Size = new System.Drawing.Size(73, 26);
            this.txtpaintbrushsize.TabIndex = 12;
            this.txtpaintbrushsize.TextChanged += new System.EventHandler(this.txtpaintbrushsize_TextChanged);
            // 
            // Label38
            // 
            this.Label38.AutoSize = true;
            this.Label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label38.Location = new System.Drawing.Point(137, 69);
            this.Label38.Name = "Label38";
            this.Label38.Size = new System.Drawing.Size(51, 24);
            this.Label38.TabIndex = 10;
            this.Label38.Text = "Size:";
            // 
            // Label39
            // 
            this.Label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label39.Location = new System.Drawing.Point(6, 3);
            this.Label39.Name = "Label39";
            this.Label39.Size = new System.Drawing.Size(444, 23);
            this.Label39.TabIndex = 0;
            this.Label39.Text = "Paint Brush Tool Settings";
            this.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnltexttoolsettings
            // 
            this.pnltexttoolsettings.Controls.Add(this.Label35);
            this.pnltexttoolsettings.Controls.Add(this.combofontstyle);
            this.pnltexttoolsettings.Controls.Add(this.txtdrawstringtext);
            this.pnltexttoolsettings.Controls.Add(this.combodrawtextfont);
            this.pnltexttoolsettings.Controls.Add(this.Label25);
            this.pnltexttoolsettings.Controls.Add(this.txtdrawtextsize);
            this.pnltexttoolsettings.Controls.Add(this.Label32);
            this.pnltexttoolsettings.Controls.Add(this.Label33);
            this.pnltexttoolsettings.Controls.Add(this.Label34);
            this.pnltexttoolsettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnltexttoolsettings.Location = new System.Drawing.Point(0, 1);
            this.pnltexttoolsettings.Name = "pnltexttoolsettings";
            this.pnltexttoolsettings.Size = new System.Drawing.Size(456, 99);
            this.pnltexttoolsettings.TabIndex = 17;
            this.pnltexttoolsettings.Visible = false;
            // 
            // Label35
            // 
            this.Label35.AutoSize = true;
            this.Label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label35.Location = new System.Drawing.Point(117, 33);
            this.Label35.Name = "Label35";
            this.Label35.Size = new System.Drawing.Size(55, 24);
            this.Label35.TabIndex = 19;
            this.Label35.Text = "Style:";
            // 
            // combofontstyle
            // 
            this.combofontstyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.combofontstyle.FormattingEnabled = true;
            this.combofontstyle.Items.AddRange(new object[] {
            "Bold",
            "Italic",
            "Regular",
            "Strikeout",
            "Underline"});
            this.combofontstyle.Location = new System.Drawing.Point(178, 34);
            this.combofontstyle.Name = "combofontstyle";
            this.combofontstyle.Size = new System.Drawing.Size(78, 24);
            this.combofontstyle.TabIndex = 18;
            this.combofontstyle.SelectedIndexChanged += new System.EventHandler(this.combofontstyle_SelectedIndexChanged);
            // 
            // txtdrawstringtext
            // 
            this.txtdrawstringtext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtdrawstringtext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdrawstringtext.Location = new System.Drawing.Point(265, 52);
            this.txtdrawstringtext.Multiline = true;
            this.txtdrawstringtext.Name = "txtdrawstringtext";
            this.txtdrawstringtext.Size = new System.Drawing.Size(185, 41);
            this.txtdrawstringtext.TabIndex = 17;
            this.txtdrawstringtext.Text = "Enter Words Here";
            this.txtdrawstringtext.TextChanged += new System.EventHandler(this.txtdrawstringtext_TextChanged);
            // 
            // combodrawtextfont
            // 
            this.combodrawtextfont.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.combodrawtextfont.FormattingEnabled = true;
            this.combodrawtextfont.Location = new System.Drawing.Point(64, 68);
            this.combodrawtextfont.Name = "combodrawtextfont";
            this.combodrawtextfont.Size = new System.Drawing.Size(192, 24);
            this.combodrawtextfont.TabIndex = 16;
            this.combodrawtextfont.SelectedIndexChanged += new System.EventHandler(this.combodrawtextfont_SelectedIndexChanged);
            // 
            // Label25
            // 
            this.Label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label25.Location = new System.Drawing.Point(262, 26);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(189, 23);
            this.Label25.TabIndex = 15;
            this.Label25.Text = "Click and drag on the canvas!";
            this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtdrawtextsize
            // 
            this.txtdrawtextsize.BackColor = System.Drawing.Color.White;
            this.txtdrawtextsize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtdrawtextsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdrawtextsize.ForeColor = System.Drawing.Color.Black;
            this.txtdrawtextsize.Location = new System.Drawing.Point(61, 33);
            this.txtdrawtextsize.Name = "txtdrawtextsize";
            this.txtdrawtextsize.Size = new System.Drawing.Size(43, 26);
            this.txtdrawtextsize.TabIndex = 12;
            this.txtdrawtextsize.TextChanged += new System.EventHandler(this.txtdrawtextsize_TextChanged);
            // 
            // Label32
            // 
            this.Label32.AutoSize = true;
            this.Label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label32.Location = new System.Drawing.Point(4, 66);
            this.Label32.Name = "Label32";
            this.Label32.Size = new System.Drawing.Size(53, 24);
            this.Label32.TabIndex = 11;
            this.Label32.Text = "Font:";
            // 
            // Label33
            // 
            this.Label33.AutoSize = true;
            this.Label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label33.Location = new System.Drawing.Point(4, 33);
            this.Label33.Name = "Label33";
            this.Label33.Size = new System.Drawing.Size(51, 24);
            this.Label33.TabIndex = 10;
            this.Label33.Text = "Size:";
            // 
            // Label34
            // 
            this.Label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label34.Location = new System.Drawing.Point(6, 3);
            this.Label34.Name = "Label34";
            this.Label34.Size = new System.Drawing.Size(444, 23);
            this.Label34.TabIndex = 0;
            this.Label34.Text = "Text Tool Settings";
            this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlpixelsettersettings
            // 
            this.pnlpixelsettersettings.Controls.Add(this.btnpixelsettersetpixel);
            this.pnlpixelsettersettings.Controls.Add(this.txtpixelsetterycoordinate);
            this.pnlpixelsettersettings.Controls.Add(this.txtpixelsetterxcoordinate);
            this.pnlpixelsettersettings.Controls.Add(this.Label3);
            this.pnlpixelsettersettings.Controls.Add(this.Label2);
            this.pnlpixelsettersettings.Controls.Add(this.Label1);
            this.pnlpixelsettersettings.Location = new System.Drawing.Point(0, 1);
            this.pnlpixelsettersettings.Name = "pnlpixelsettersettings";
            this.pnlpixelsettersettings.Size = new System.Drawing.Size(457, 99);
            this.pnlpixelsettersettings.TabIndex = 5;
            this.pnlpixelsettersettings.Visible = false;
            // 
            // btnpixelsettersetpixel
            // 
            this.btnpixelsettersetpixel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpixelsettersetpixel.Location = new System.Drawing.Point(215, 34);
            this.btnpixelsettersetpixel.Name = "btnpixelsettersetpixel";
            this.btnpixelsettersetpixel.Size = new System.Drawing.Size(228, 56);
            this.btnpixelsettersetpixel.TabIndex = 10;
            this.btnpixelsettersetpixel.Text = "Set Pixel";
            this.btnpixelsettersetpixel.UseVisualStyleBackColor = true;
            this.btnpixelsettersetpixel.Click += new System.EventHandler(this.btnpixelsettersetpixel_Click);
            // 
            // txtpixelsetterycoordinate
            // 
            this.txtpixelsetterycoordinate.BackColor = System.Drawing.Color.White;
            this.txtpixelsetterycoordinate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpixelsetterycoordinate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpixelsetterycoordinate.ForeColor = System.Drawing.Color.Black;
            this.txtpixelsetterycoordinate.Location = new System.Drawing.Point(136, 64);
            this.txtpixelsetterycoordinate.Name = "txtpixelsetterycoordinate";
            this.txtpixelsetterycoordinate.Size = new System.Drawing.Size(73, 26);
            this.txtpixelsetterycoordinate.TabIndex = 9;
            // 
            // txtpixelsetterxcoordinate
            // 
            this.txtpixelsetterxcoordinate.BackColor = System.Drawing.Color.White;
            this.txtpixelsetterxcoordinate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpixelsetterxcoordinate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpixelsetterxcoordinate.ForeColor = System.Drawing.Color.Black;
            this.txtpixelsetterxcoordinate.Location = new System.Drawing.Point(136, 34);
            this.txtpixelsetterxcoordinate.Name = "txtpixelsetterxcoordinate";
            this.txtpixelsetterxcoordinate.Size = new System.Drawing.Size(73, 26);
            this.txtpixelsetterxcoordinate.TabIndex = 8;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(8, 63);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(124, 24);
            this.Label3.TabIndex = 2;
            this.Label3.Text = "Y Coordinate:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(7, 34);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(126, 24);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "X Coordinate:";
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(6, 3);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(444, 23);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Pixel Setter Settings";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlfloodfillsettings
            // 
            this.pnlfloodfillsettings.Controls.Add(this.Label12);
            this.pnlfloodfillsettings.Controls.Add(this.Label15);
            this.pnlfloodfillsettings.Location = new System.Drawing.Point(162, 65);
            this.pnlfloodfillsettings.Name = "pnlfloodfillsettings";
            this.pnlfloodfillsettings.Size = new System.Drawing.Size(71, 31);
            this.pnlfloodfillsettings.TabIndex = 13;
            this.pnlfloodfillsettings.Visible = false;
            // 
            // Label12
            // 
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(3, 28);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(450, 51);
            this.Label12.TabIndex = 11;
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label15
            // 
            this.Label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label15.Location = new System.Drawing.Point(6, 3);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(444, 23);
            this.Label15.TabIndex = 0;
            this.Label15.Text = "Flood Fill Settings";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlsquaretoolsettings
            // 
            this.pnlsquaretoolsettings.Controls.Add(this.Label19);
            this.pnlsquaretoolsettings.Controls.Add(this.btnsquarefillonoff);
            this.pnlsquaretoolsettings.Controls.Add(this.pnlsquarefillcolour);
            this.pnlsquaretoolsettings.Controls.Add(this.txtsquareborderwidth);
            this.pnlsquaretoolsettings.Controls.Add(this.Label16);
            this.pnlsquaretoolsettings.Controls.Add(this.Label18);
            this.pnlsquaretoolsettings.Controls.Add(this.Label17);
            this.pnlsquaretoolsettings.Location = new System.Drawing.Point(16, 39);
            this.pnlsquaretoolsettings.Name = "pnlsquaretoolsettings";
            this.pnlsquaretoolsettings.Size = new System.Drawing.Size(42, 31);
            this.pnlsquaretoolsettings.TabIndex = 14;
            this.pnlsquaretoolsettings.Visible = false;
            // 
            // Label19
            // 
            this.Label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label19.Location = new System.Drawing.Point(211, 29);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(244, 65);
            this.Label19.TabIndex = 15;
            this.Label19.Text = "Set a border width and turn fill on or off then draw the square on the canvas wit" +
    "h the mouse. Click the fill colour box to set it to your currently selected colo" +
    "ur.";
            // 
            // btnsquarefillonoff
            // 
            this.btnsquarefillonoff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnsquarefillonoff.Location = new System.Drawing.Point(150, 64);
            this.btnsquarefillonoff.Name = "btnsquarefillonoff";
            this.btnsquarefillonoff.Size = new System.Drawing.Size(56, 28);
            this.btnsquarefillonoff.TabIndex = 14;
            this.btnsquarefillonoff.Text = "Fill OFF";
            this.btnsquarefillonoff.UseVisualStyleBackColor = true;
            this.btnsquarefillonoff.Click += new System.EventHandler(this.btnsquarefillonoff_Click);
            // 
            // pnlsquarefillcolour
            // 
            this.pnlsquarefillcolour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlsquarefillcolour.Location = new System.Drawing.Point(109, 64);
            this.pnlsquarefillcolour.Name = "pnlsquarefillcolour";
            this.pnlsquarefillcolour.Size = new System.Drawing.Size(34, 28);
            this.pnlsquarefillcolour.TabIndex = 13;
            this.pnlsquarefillcolour.Click += new System.EventHandler(this.pnlsquarefillcolour_Click);
            // 
            // txtsquareborderwidth
            // 
            this.txtsquareborderwidth.BackColor = System.Drawing.Color.White;
            this.txtsquareborderwidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtsquareborderwidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsquareborderwidth.ForeColor = System.Drawing.Color.Black;
            this.txtsquareborderwidth.Location = new System.Drawing.Point(133, 33);
            this.txtsquareborderwidth.Name = "txtsquareborderwidth";
            this.txtsquareborderwidth.Size = new System.Drawing.Size(73, 26);
            this.txtsquareborderwidth.TabIndex = 12;
            this.txtsquareborderwidth.TextChanged += new System.EventHandler(this.txtsquareborderwidth_TextChanged);
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label16.Location = new System.Drawing.Point(4, 66);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(100, 24);
            this.Label16.TabIndex = 11;
            this.Label16.Text = "Fill Colour:";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label18.Location = new System.Drawing.Point(4, 33);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(125, 24);
            this.Label18.TabIndex = 10;
            this.Label18.Text = "Border Width:";
            // 
            // Label17
            // 
            this.Label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.Location = new System.Drawing.Point(6, 3);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(444, 23);
            this.Label17.TabIndex = 0;
            this.Label17.Text = "Rectangle Tool Settings";
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlpencilsettings
            // 
            this.pnlpencilsettings.Controls.Add(this.btnpencilsize3);
            this.pnlpencilsettings.Controls.Add(this.btnpencilsize2);
            this.pnlpencilsettings.Controls.Add(this.btnpencilsize1);
            this.pnlpencilsettings.Controls.Add(this.Label14);
            this.pnlpencilsettings.Location = new System.Drawing.Point(422, 13);
            this.pnlpencilsettings.Name = "pnlpencilsettings";
            this.pnlpencilsettings.Size = new System.Drawing.Size(28, 22);
            this.pnlpencilsettings.TabIndex = 12;
            this.pnlpencilsettings.Visible = false;
            // 
            // btnpencilsize3
            // 
            this.btnpencilsize3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpencilsize3.Location = new System.Drawing.Point(298, 30);
            this.btnpencilsize3.Name = "btnpencilsize3";
            this.btnpencilsize3.Size = new System.Drawing.Size(127, 54);
            this.btnpencilsize3.TabIndex = 12;
            this.btnpencilsize3.Text = "Thickest";
            this.btnpencilsize3.UseVisualStyleBackColor = true;
            this.btnpencilsize3.Click += new System.EventHandler(this.ChangePencilSize);
            // 
            // btnpencilsize2
            // 
            this.btnpencilsize2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpencilsize2.Location = new System.Drawing.Point(165, 30);
            this.btnpencilsize2.Name = "btnpencilsize2";
            this.btnpencilsize2.Size = new System.Drawing.Size(127, 54);
            this.btnpencilsize2.TabIndex = 11;
            this.btnpencilsize2.Text = "Thicker";
            this.btnpencilsize2.UseVisualStyleBackColor = true;
            this.btnpencilsize2.Click += new System.EventHandler(this.ChangePencilSize);
            // 
            // btnpencilsize1
            // 
            this.btnpencilsize1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpencilsize1.Location = new System.Drawing.Point(30, 30);
            this.btnpencilsize1.Name = "btnpencilsize1";
            this.btnpencilsize1.Size = new System.Drawing.Size(127, 54);
            this.btnpencilsize1.TabIndex = 10;
            this.btnpencilsize1.Text = "Thin";
            this.btnpencilsize1.UseVisualStyleBackColor = true;
            this.btnpencilsize1.Click += new System.EventHandler(this.ChangePencilSize);
            // 
            // Label14
            // 
            this.Label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label14.Location = new System.Drawing.Point(6, 3);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(444, 23);
            this.Label14.TabIndex = 0;
            this.Label14.Text = "Pencil Settings";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(13, 40);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(437, 49);
            this.Label6.TabIndex = 9;
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(110, 13);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(269, 24);
            this.Label5.TabIndex = 8;
            this.Label5.Text = "No Tool Currently Selected!";
            // 
            // line5
            // 
            this.line5.BackColor = System.Drawing.Color.Black;
            this.line5.Dock = System.Windows.Forms.DockStyle.Top;
            this.line5.Location = new System.Drawing.Point(0, 0);
            this.line5.Name = "line5";
            this.line5.Size = new System.Drawing.Size(456, 1);
            this.line5.TabIndex = 4;
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.Color.Black;
            this.line3.Dock = System.Windows.Forms.DockStyle.Right;
            this.line3.Location = new System.Drawing.Point(456, 0);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(1, 100);
            this.line3.TabIndex = 2;
            // 
            // pnltools
            // 
            this.pnltools.BackColor = System.Drawing.Color.White;
            this.pnltools.Controls.Add(this.pnltoolpositioner);
            this.pnltools.Controls.Add(this.line1);
            this.pnltools.Controls.Add(this.pnltoolpreview);
            this.pnltools.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnltools.Location = new System.Drawing.Point(0, 0);
            this.pnltools.Name = "pnltools";
            this.pnltools.Size = new System.Drawing.Size(120, 574);
            this.pnltools.TabIndex = 0;
            // 
            // pnltoolpositioner
            // 
            this.pnltoolpositioner.Controls.Add(this.btnpixelsetter);
            this.pnltoolpositioner.Controls.Add(this.btnpixelplacer);
            this.pnltoolpositioner.Controls.Add(this.btnpencil);
            this.pnltoolpositioner.Controls.Add(this.btnfloodfill);
            this.pnltoolpositioner.Controls.Add(this.btnoval);
            this.pnltoolpositioner.Controls.Add(this.btnsquare);
            this.pnltoolpositioner.Controls.Add(this.btnlinetool);
            this.pnltoolpositioner.Controls.Add(this.btnpaintbrush);
            this.pnltoolpositioner.Controls.Add(this.btneracer);
            this.pnltoolpositioner.Controls.Add(this.btntexttool);
            this.pnltoolpositioner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnltoolpositioner.Location = new System.Drawing.Point(0, 0);
            this.pnltoolpositioner.Name = "pnltoolpositioner";
            this.pnltoolpositioner.Size = new System.Drawing.Size(119, 474);
            this.pnltoolpositioner.TabIndex = 2;
            // 
            // btnpixelsetter
            // 
            this.btnpixelsetter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpixelsetter.FlatAppearance.BorderSize = 0;
            this.btnpixelsetter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpixelsetter.Location = new System.Drawing.Point(6, 6);
            this.btnpixelsetter.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpixelsetter.Name = "btnpixelsetter";
            this.btnpixelsetter.Size = new System.Drawing.Size(50, 50);
            this.btnpixelsetter.TabIndex = 2;
            this.btnpixelsetter.Tag = "nobuttonskin";
            this.btnpixelsetter.UseVisualStyleBackColor = true;
            this.btnpixelsetter.Click += new System.EventHandler(this.btnpixelsetter_Click);
            // 
            // btnpixelplacer
            // 
            this.btnpixelplacer.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadpixelplacer;
            this.btnpixelplacer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpixelplacer.FlatAppearance.BorderSize = 0;
            this.btnpixelplacer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpixelplacer.Location = new System.Drawing.Point(62, 6);
            this.btnpixelplacer.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpixelplacer.Name = "btnpixelplacer";
            this.btnpixelplacer.Size = new System.Drawing.Size(50, 50);
            this.btnpixelplacer.TabIndex = 4;
            this.btnpixelplacer.Tag = "nobuttonskin";
            this.btnpixelplacer.UseVisualStyleBackColor = true;
            this.btnpixelplacer.Click += new System.EventHandler(this.btnpixelplacer_Click);
            // 
            // btnpencil
            // 
            this.btnpencil.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadpencil;
            this.btnpencil.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpencil.FlatAppearance.BorderSize = 0;
            this.btnpencil.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpencil.Location = new System.Drawing.Point(6, 62);
            this.btnpencil.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpencil.Name = "btnpencil";
            this.btnpencil.Size = new System.Drawing.Size(50, 50);
            this.btnpencil.TabIndex = 7;
            this.btnpencil.Tag = "nobuttonskin";
            this.btnpencil.Text = " ";
            this.btnpencil.UseVisualStyleBackColor = true;
            this.btnpencil.Click += new System.EventHandler(this.btnpencil_Click);
            // 
            // btnfloodfill
            // 
            this.btnfloodfill.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadfloodfill;
            this.btnfloodfill.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnfloodfill.FlatAppearance.BorderSize = 0;
            this.btnfloodfill.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnfloodfill.Location = new System.Drawing.Point(62, 62);
            this.btnfloodfill.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnfloodfill.Name = "btnfloodfill";
            this.btnfloodfill.Size = new System.Drawing.Size(50, 50);
            this.btnfloodfill.TabIndex = 11;
            this.btnfloodfill.Tag = "nobuttonskin";
            this.btnfloodfill.UseVisualStyleBackColor = true;
            this.btnfloodfill.Click += new System.EventHandler(this.btnfill_Click);
            // 
            // btnoval
            // 
            this.btnoval.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadOval;
            this.btnoval.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnoval.FlatAppearance.BorderSize = 0;
            this.btnoval.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnoval.Location = new System.Drawing.Point(6, 118);
            this.btnoval.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnoval.Name = "btnoval";
            this.btnoval.Size = new System.Drawing.Size(50, 50);
            this.btnoval.TabIndex = 13;
            this.btnoval.Tag = "nobuttonskin";
            this.btnoval.Text = " ";
            this.btnoval.UseVisualStyleBackColor = true;
            this.btnoval.Click += new System.EventHandler(this.btnoval_Click);
            // 
            // btnsquare
            // 
            this.btnsquare.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadRectangle;
            this.btnsquare.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnsquare.FlatAppearance.BorderSize = 0;
            this.btnsquare.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsquare.Location = new System.Drawing.Point(62, 118);
            this.btnsquare.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnsquare.Name = "btnsquare";
            this.btnsquare.Size = new System.Drawing.Size(50, 50);
            this.btnsquare.TabIndex = 12;
            this.btnsquare.Tag = "nobuttonskin";
            this.btnsquare.Text = " ";
            this.btnsquare.UseVisualStyleBackColor = true;
            this.btnsquare.Click += new System.EventHandler(this.btnsquare_Click);
            // 
            // btnlinetool
            // 
            this.btnlinetool.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadlinetool;
            this.btnlinetool.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnlinetool.FlatAppearance.BorderSize = 0;
            this.btnlinetool.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnlinetool.Location = new System.Drawing.Point(6, 174);
            this.btnlinetool.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnlinetool.Name = "btnlinetool";
            this.btnlinetool.Size = new System.Drawing.Size(50, 50);
            this.btnlinetool.TabIndex = 15;
            this.btnlinetool.Tag = "nobuttonskin";
            this.btnlinetool.Text = " ";
            this.btnlinetool.UseVisualStyleBackColor = true;
            this.btnlinetool.Click += new System.EventHandler(this.btnlinetool_Click);
            // 
            // btnpaintbrush
            // 
            this.btnpaintbrush.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadpaintbrush;
            this.btnpaintbrush.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnpaintbrush.FlatAppearance.BorderSize = 0;
            this.btnpaintbrush.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpaintbrush.Location = new System.Drawing.Point(62, 174);
            this.btnpaintbrush.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btnpaintbrush.Name = "btnpaintbrush";
            this.btnpaintbrush.Size = new System.Drawing.Size(50, 50);
            this.btnpaintbrush.TabIndex = 17;
            this.btnpaintbrush.Tag = "nobuttonskin";
            this.btnpaintbrush.Text = " ";
            this.btnpaintbrush.UseVisualStyleBackColor = true;
            this.btnpaintbrush.Click += new System.EventHandler(this.btnpaintbrush_Click);
            // 
            // btntexttool
            // 
            this.btntexttool.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPadtexttool;
            this.btntexttool.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btntexttool.FlatAppearance.BorderSize = 0;
            this.btntexttool.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btntexttool.Location = new System.Drawing.Point(62, 230);
            this.btntexttool.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btntexttool.Name = "btntexttool";
            this.btntexttool.Size = new System.Drawing.Size(50, 50);
            this.btntexttool.TabIndex = 16;
            this.btntexttool.Tag = "nobuttonskin";
            this.btntexttool.Text = " ";
            this.btntexttool.UseVisualStyleBackColor = true;
            this.btntexttool.Click += new System.EventHandler(this.btntexttool_Click);
            // 
            // btneracer
            // 
            this.btneracer.BackgroundImage = global::ShiftOS.WinForms.Properties.Resources.ArtPaderacer;
            this.btneracer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btneracer.FlatAppearance.BorderSize = 0;
            this.btneracer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btneracer.Location = new System.Drawing.Point(6, 230);
            this.btneracer.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.btneracer.Name = "btneracer";
            this.btneracer.Size = new System.Drawing.Size(50, 50);
            this.btneracer.TabIndex = 14;
            this.btneracer.Tag = "nobuttonskin";
            this.btneracer.Text = " ";
            this.btneracer.UseVisualStyleBackColor = true;
            this.btneracer.Click += new System.EventHandler(this.btneracer_Click);
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.Color.Black;
            this.line1.Dock = System.Windows.Forms.DockStyle.Right;
            this.line1.Location = new System.Drawing.Point(119, 0);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(1, 474);
            this.line1.TabIndex = 1;
            // 
            // pnltoolpreview
            // 
            this.pnltoolpreview.Controls.Add(this.Label13);
            this.pnltoolpreview.Controls.Add(this.picpreview);
            this.pnltoolpreview.Controls.Add(this.lbltoolselected);
            this.pnltoolpreview.Controls.Add(this.line4);
            this.pnltoolpreview.Controls.Add(this.line2);
            this.pnltoolpreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnltoolpreview.Location = new System.Drawing.Point(0, 474);
            this.pnltoolpreview.Name = "pnltoolpreview";
            this.pnltoolpreview.Size = new System.Drawing.Size(120, 100);
            this.pnltoolpreview.TabIndex = 0;
            // 
            // Label13
            // 
            this.Label13.BackColor = System.Drawing.Color.Transparent;
            this.Label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(3, 79);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(113, 21);
            this.Label13.TabIndex = 6;
            this.Label13.Text = "Preview";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picpreview
            // 
            this.picpreview.Location = new System.Drawing.Point(24, 27);
            this.picpreview.Name = "picpreview";
            this.picpreview.Size = new System.Drawing.Size(70, 50);
            this.picpreview.TabIndex = 5;
            this.picpreview.TabStop = false;
            // 
            // lbltoolselected
            // 
            this.lbltoolselected.BackColor = System.Drawing.Color.Transparent;
            this.lbltoolselected.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltoolselected.Location = new System.Drawing.Point(3, 3);
            this.lbltoolselected.Name = "lbltoolselected";
            this.lbltoolselected.Size = new System.Drawing.Size(113, 21);
            this.lbltoolselected.TabIndex = 4;
            this.lbltoolselected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // line4
            // 
            this.line4.BackColor = System.Drawing.Color.Black;
            this.line4.Dock = System.Windows.Forms.DockStyle.Top;
            this.line4.Location = new System.Drawing.Point(0, 0);
            this.line4.Name = "line4";
            this.line4.Size = new System.Drawing.Size(119, 1);
            this.line4.TabIndex = 3;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.Black;
            this.line2.Dock = System.Windows.Forms.DockStyle.Right;
            this.line2.Location = new System.Drawing.Point(119, 0);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(1, 100);
            this.line2.TabIndex = 2;
            // 
            // tmrcodepointcooldown
            // 
            this.tmrcodepointcooldown.Interval = 10000;
            // 
            // tmrshowearnedcodepoints
            // 
            this.tmrshowearnedcodepoints.Interval = 3000;
            // 
            // pullbs
            // 
            this.pullbs.Interval = 1;
            // 
            // pullbottom
            // 
            this.pullbottom.Interval = 1;
            // 
            // pullside
            // 
            this.pullside.Interval = 1;
            // 
            // tmrsetupui
            // 
            this.tmrsetupui.Tick += new System.EventHandler(this.tmrsetupui_Tick);
            // 
            // msTools
            // 
            this.msTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.msTools.Location = new System.Drawing.Point(0, 0);
            this.msTools.Name = "msTools";
            this.msTools.Size = new System.Drawing.Size(802, 24);
            this.msTools.TabIndex = 3;
            this.msTools.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gENNEWToolStripMenuItem,
            this.gENLOADToolStripMenuItem,
            this.gENSAVEToolStripMenuItem,
            this.gENEXITToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // gENNEWToolStripMenuItem
            // 
            this.gENNEWToolStripMenuItem.Name = "gENNEWToolStripMenuItem";
            this.gENNEWToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.gENNEWToolStripMenuItem.Text = "{GEN_NEW}";
            this.gENNEWToolStripMenuItem.Click += new System.EventHandler(this.btnnew_Click);
            // 
            // gENLOADToolStripMenuItem
            // 
            this.gENLOADToolStripMenuItem.Name = "gENLOADToolStripMenuItem";
            this.gENLOADToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.gENLOADToolStripMenuItem.Text = "{GEN_LOAD}";
            this.gENLOADToolStripMenuItem.Click += new System.EventHandler(this.btnopen_Click);
            // 
            // gENSAVEToolStripMenuItem
            // 
            this.gENSAVEToolStripMenuItem.Name = "gENSAVEToolStripMenuItem";
            this.gENSAVEToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.gENSAVEToolStripMenuItem.Text = "{GEN_SAVE}";
            this.gENSAVEToolStripMenuItem.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // gENEXITToolStripMenuItem
            // 
            this.gENEXITToolStripMenuItem.Name = "gENEXITToolStripMenuItem";
            this.gENEXITToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.gENEXITToolStripMenuItem.Text = "{GEN_EXIT}";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.btnundo_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.btnredo_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.moreControlsToolStripMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem2.Text = "25%";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem3.Text = "50%";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem4.Text = "100%";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem5.Text = "200%";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // moreControlsToolStripMenuItem
            // 
            this.moreControlsToolStripMenuItem.Name = "moreControlsToolStripMenuItem";
            this.moreControlsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.moreControlsToolStripMenuItem.Text = "More controls";
            this.moreControlsToolStripMenuItem.Click += new System.EventHandler(this.btnmagnify_Click);
            // 
            // Artpad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgcontents);
            this.Controls.Add(this.msTools);
            this.MinimumSize = new System.Drawing.Size(502, 398);
            this.Name = "Artpad";
            this.Size = new System.Drawing.Size(802, 598);
            this.Tag = "nobuttonskin";
            this.Load += new System.EventHandler(this.Template_Load);
            this.pgcontents.ResumeLayout(false);
            this.pnldrawingbackground.ResumeLayout(false);
            this.pnlinitialcanvassettings.ResumeLayout(false);
            this.pnlinitialcanvassettings.PerformLayout();
            this.pnlpalettesize.ResumeLayout(false);
            this.pnlpalettesize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picdrawingdisplay)).EndInit();
            this.pnlbottompanel.ResumeLayout(false);
            this.pnlpallet.ResumeLayout(false);
            this.flowcolours.ResumeLayout(false);
            this.pnltoolproperties.ResumeLayout(false);
            this.pnltoolproperties.PerformLayout();
            this.pnlmagnifiersettings.ResumeLayout(false);
            this.pnleracertoolsettings.ResumeLayout(false);
            this.pnleracertoolsettings.PerformLayout();
            this.pnlpixelplacersettings.ResumeLayout(false);
            this.pnlovaltoolsettings.ResumeLayout(false);
            this.pnlovaltoolsettings.PerformLayout();
            this.pnllinetoolsettings.ResumeLayout(false);
            this.pnllinetoolsettings.PerformLayout();
            this.pnlpaintbrushtoolsettings.ResumeLayout(false);
            this.pnlpaintbrushtoolsettings.PerformLayout();
            this.pnltexttoolsettings.ResumeLayout(false);
            this.pnltexttoolsettings.PerformLayout();
            this.pnlpixelsettersettings.ResumeLayout(false);
            this.pnlpixelsettersettings.PerformLayout();
            this.pnlfloodfillsettings.ResumeLayout(false);
            this.pnlsquaretoolsettings.ResumeLayout(false);
            this.pnlsquaretoolsettings.PerformLayout();
            this.pnlpencilsettings.ResumeLayout(false);
            this.pnltools.ResumeLayout(false);
            this.pnltoolpositioner.ResumeLayout(false);
            this.pnltoolpreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picpreview)).EndInit();
            this.msTools.ResumeLayout(false);
            this.msTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Panel pgcontents;
        internal System.Windows.Forms.Panel pnlbottompanel;
        internal System.Windows.Forms.Panel pnltoolproperties;
        internal System.Windows.Forms.Panel pnlpallet;
        internal System.Windows.Forms.Panel line3;
        internal System.Windows.Forms.Panel line5;
        internal System.Windows.Forms.Panel line6;
        internal System.Windows.Forms.Panel pnldrawingbackground;
        internal System.Windows.Forms.PictureBox picdrawingdisplay;
        internal System.Windows.Forms.Button btnpixelsetter;
        internal System.Windows.Forms.Panel pnlpixelsettersettings;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnpixelsettersetpixel;
        internal System.Windows.Forms.TextBox txtpixelsetterycoordinate;
        internal System.Windows.Forms.TextBox txtpixelsetterxcoordinate;
        internal System.Windows.Forms.Panel colourpallet32;
        internal System.Windows.Forms.Panel colourpallet31;
        internal System.Windows.Forms.Panel colourpallet30;
        internal System.Windows.Forms.Panel colourpallet29;
        internal System.Windows.Forms.Panel colourpallet28;
        internal System.Windows.Forms.Panel colourpallet26;
        internal System.Windows.Forms.Panel colourpallet25;
        internal System.Windows.Forms.Panel colourpallet23;
        internal System.Windows.Forms.Panel colourpallet21;
        internal System.Windows.Forms.Panel colourpallet19;
        internal System.Windows.Forms.Panel colourpallet8;
        internal System.Windows.Forms.Panel colourpallet6;
        internal System.Windows.Forms.Panel colourpallet16;
        internal System.Windows.Forms.Panel colourpallet14;
        internal System.Windows.Forms.Panel colourpallet7;
        internal System.Windows.Forms.Panel colourpallet5;
        internal System.Windows.Forms.Panel colourpallet10;
        internal System.Windows.Forms.Panel colourpallet9;
        internal System.Windows.Forms.Panel colourpallet11;
        internal System.Windows.Forms.Panel colourpallet12;
        internal System.Windows.Forms.Panel colourpallet13;
        internal System.Windows.Forms.Panel colourpallet15;
        internal System.Windows.Forms.Panel colourpallet4;
        internal System.Windows.Forms.Panel colourpallet3;
        internal System.Windows.Forms.Panel colourpallet17;
        internal System.Windows.Forms.Panel colourpallet18;
        internal System.Windows.Forms.Panel colourpallet20;
        internal System.Windows.Forms.Panel colourpallet22;
        internal System.Windows.Forms.Panel colourpallet24;
        internal System.Windows.Forms.Panel colourpallet2;
        internal System.Windows.Forms.Panel colourpallet27;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Panel colourpallet1;
        internal System.Windows.Forms.Panel pnlmagnifiersettings;
        internal System.Windows.Forms.Button btnzoomin;
        internal System.Windows.Forms.Label lblzoomlevel;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Button btnzoomout;
        internal System.Windows.Forms.Button btnpixelplacer;
        internal System.Windows.Forms.Panel pnlpixelplacersettings;
        internal System.Windows.Forms.Button btnpixelplacermovementmode;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label lblpixelplacerhelp;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.FlowLayoutPanel flowcolours;
        internal System.Windows.Forms.Button btnpencil;
        internal System.Windows.Forms.Panel pnlpencilsettings;
        internal System.Windows.Forms.Button btnpencilsize3;
        internal System.Windows.Forms.Button btnpencilsize2;
        internal System.Windows.Forms.Button btnpencilsize1;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.Button btnfloodfill;
        internal System.Windows.Forms.Panel pnlfloodfillsettings;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label Label15;
        internal System.Windows.Forms.Button btnsquare;
        internal System.Windows.Forms.Panel pnlsquaretoolsettings;
        internal System.Windows.Forms.TextBox txtsquareborderwidth;
        internal System.Windows.Forms.Label Label16;
        internal System.Windows.Forms.Label Label18;
        internal System.Windows.Forms.Label Label17;
        internal System.Windows.Forms.Button btnsquarefillonoff;
        internal System.Windows.Forms.Panel pnlsquarefillcolour;
        internal System.Windows.Forms.Label Label19;
        internal System.Windows.Forms.Button btnoval;
        internal System.Windows.Forms.Panel pnlovaltoolsettings;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.Button btnovalfillonoff;
        internal System.Windows.Forms.Panel pnlovalfillcolour;
        internal System.Windows.Forms.TextBox txtovalborderwidth;
        internal System.Windows.Forms.Label Label21;
        internal System.Windows.Forms.Label Label22;
        internal System.Windows.Forms.Label Label23;
        internal System.Windows.Forms.Panel pnltools;
        internal System.Windows.Forms.FlowLayoutPanel pnltoolpositioner;
        internal System.Windows.Forms.Panel line1;
        internal System.Windows.Forms.Panel pnltoolpreview;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.PictureBox picpreview;
        internal System.Windows.Forms.Label lbltoolselected;
        internal System.Windows.Forms.Panel line4;
        internal System.Windows.Forms.Panel line2;
        internal System.Windows.Forms.Button btneracer;
        internal System.Windows.Forms.Button btnlinetool;
        internal System.Windows.Forms.Panel pnllinetoolsettings;
        internal System.Windows.Forms.Label Label29;
        internal System.Windows.Forms.TextBox txtlinewidth;
        internal System.Windows.Forms.Label Label30;
        internal System.Windows.Forms.Label Label31;
        internal System.Windows.Forms.Panel pnleracertoolsettings;
        internal System.Windows.Forms.Label Label28;
        internal System.Windows.Forms.Button btneracersquare;
        internal System.Windows.Forms.Button btneracercircle;
        internal System.Windows.Forms.Label Label24;
        internal System.Windows.Forms.TextBox txteracersize;
        internal System.Windows.Forms.Label Label26;
        internal System.Windows.Forms.Label Label27;
        internal System.Windows.Forms.Button btntexttool;
        internal System.Windows.Forms.Button btnpaintbrush;
        internal System.Windows.Forms.Panel pnltexttoolsettings;
        internal System.Windows.Forms.Label Label25;
        internal System.Windows.Forms.TextBox txtdrawtextsize;
        internal System.Windows.Forms.Label Label32;
        internal System.Windows.Forms.Label Label33;
        internal System.Windows.Forms.Label Label34;
        internal System.Windows.Forms.ComboBox combodrawtextfont;
        internal System.Windows.Forms.TextBox txtdrawstringtext;
        internal System.Windows.Forms.ComboBox combofontstyle;
        internal System.Windows.Forms.Label Label35;
        internal System.Windows.Forms.Panel pnlpaintbrushtoolsettings;
        internal System.Windows.Forms.Label Label36;
        internal System.Windows.Forms.Button btnpaintsquareshape;
        internal System.Windows.Forms.Button btnpaintcircleshape;
        internal System.Windows.Forms.Label Label37;
        internal System.Windows.Forms.TextBox txtpaintbrushsize;
        internal System.Windows.Forms.Label Label38;
        internal System.Windows.Forms.Label Label39;
        internal System.Windows.Forms.Timer tmrcodepointcooldown;
        internal System.Windows.Forms.Timer tmrshowearnedcodepoints;
        internal System.Windows.Forms.Panel pnlpalettesize;
        internal System.Windows.Forms.Button btnchangesizecancel;
        internal System.Windows.Forms.Button btnsetsize;
        internal System.Windows.Forms.TextBox txtcolorpalletheight;
        internal System.Windows.Forms.Label Label42;
        internal System.Windows.Forms.TextBox txtcolorpalletwidth;
        internal System.Windows.Forms.Label Label43;
        internal System.Windows.Forms.Panel pnlinitialcanvassettings;
        internal System.Windows.Forms.Button btncreate;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.Label lbltotalpixels;
        internal System.Windows.Forms.TextBox txtnewcanvasheight;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.TextBox txtnewcanvaswidth;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Panel colourpallet33;
        internal System.Windows.Forms.Panel colourpallet34;
        internal System.Windows.Forms.Panel colourpallet35;
        internal System.Windows.Forms.Panel colourpallet36;
        internal System.Windows.Forms.Panel colourpallet37;
        internal System.Windows.Forms.Panel colourpallet38;
        internal System.Windows.Forms.Panel colourpallet39;
        internal System.Windows.Forms.Panel colourpallet40;
        internal System.Windows.Forms.Panel colourpallet41;
        internal System.Windows.Forms.Panel colourpallet42;
        internal System.Windows.Forms.Panel colourpallet43;
        internal System.Windows.Forms.Panel colourpallet44;
        internal System.Windows.Forms.Panel colourpallet45;
        internal System.Windows.Forms.Panel colourpallet46;
        internal System.Windows.Forms.Panel colourpallet47;
        internal System.Windows.Forms.Panel colourpallet48;
        internal System.Windows.Forms.Panel colourpallet49;
        internal System.Windows.Forms.Panel colourpallet50;
        internal System.Windows.Forms.Panel colourpallet51;
        internal System.Windows.Forms.Panel colourpallet52;
        internal System.Windows.Forms.Panel colourpallet53;
        internal System.Windows.Forms.Panel colourpallet54;
        internal System.Windows.Forms.Panel colourpallet55;
        internal System.Windows.Forms.Panel colourpallet56;
        internal System.Windows.Forms.Panel colourpallet57;
        internal System.Windows.Forms.Panel colourpallet58;
        internal System.Windows.Forms.Panel colourpallet59;
        internal System.Windows.Forms.Panel colourpallet60;
        internal System.Windows.Forms.Panel colourpallet61;
        internal System.Windows.Forms.Panel colourpallet62;
        internal System.Windows.Forms.Panel colourpallet63;
        internal System.Windows.Forms.Panel colourpallet64;
        internal System.Windows.Forms.Panel colourpallet65;
        internal System.Windows.Forms.Panel colourpallet66;
        internal System.Windows.Forms.Panel colourpallet67;
        internal System.Windows.Forms.Panel colourpallet68;
        internal System.Windows.Forms.Panel colourpallet69;
        internal System.Windows.Forms.Panel colourpallet70;
        internal System.Windows.Forms.Panel colourpallet71;
        internal System.Windows.Forms.Panel colourpallet72;
        internal System.Windows.Forms.Panel colourpallet73;
        internal System.Windows.Forms.Panel colourpallet74;
        internal System.Windows.Forms.Panel colourpallet75;
        internal System.Windows.Forms.Panel colourpallet76;
        internal System.Windows.Forms.Panel colourpallet77;
        internal System.Windows.Forms.Panel colourpallet78;
        internal System.Windows.Forms.Panel colourpallet79;
        internal System.Windows.Forms.Panel colourpallet80;
        internal System.Windows.Forms.Panel colourpallet81;
        internal System.Windows.Forms.Panel colourpallet82;
        internal System.Windows.Forms.Panel colourpallet83;
        internal System.Windows.Forms.Panel colourpallet84;
        internal System.Windows.Forms.Panel colourpallet85;
        internal System.Windows.Forms.Panel colourpallet86;
        internal System.Windows.Forms.Panel colourpallet87;
        internal System.Windows.Forms.Panel colourpallet88;
        internal System.Windows.Forms.Panel colourpallet89;
        internal System.Windows.Forms.Panel colourpallet90;
        internal System.Windows.Forms.Panel colourpallet91;
        internal System.Windows.Forms.Panel colourpallet92;
        internal System.Windows.Forms.Panel colourpallet93;
        internal System.Windows.Forms.Panel colourpallet94;
        internal System.Windows.Forms.Panel colourpallet95;
        internal System.Windows.Forms.Panel colourpallet96;
        internal System.Windows.Forms.Panel colourpallet97;
        internal System.Windows.Forms.Panel colourpallet98;
        internal System.Windows.Forms.Panel colourpallet99;
        internal System.Windows.Forms.Panel colourpallet100;
        internal System.Windows.Forms.Panel colourpallet101;
        internal System.Windows.Forms.Panel colourpallet102;
        internal System.Windows.Forms.Panel colourpallet103;
        internal System.Windows.Forms.Panel colourpallet104;
        internal System.Windows.Forms.Panel colourpallet105;
        internal System.Windows.Forms.Panel colourpallet106;
        internal System.Windows.Forms.Panel colourpallet107;
        internal System.Windows.Forms.Panel colourpallet108;
        internal System.Windows.Forms.Panel colourpallet109;
        internal System.Windows.Forms.Panel colourpallet110;
        internal System.Windows.Forms.Panel colourpallet111;
        internal System.Windows.Forms.Panel colourpallet112;
        internal System.Windows.Forms.Panel colourpallet113;
        internal System.Windows.Forms.Panel colourpallet114;
        internal System.Windows.Forms.Panel colourpallet115;
        internal System.Windows.Forms.Panel colourpallet116;
        internal System.Windows.Forms.Panel colourpallet117;
        internal System.Windows.Forms.Panel colourpallet118;
        internal System.Windows.Forms.Panel colourpallet119;
        internal System.Windows.Forms.Panel colourpallet120;
        internal System.Windows.Forms.Panel colourpallet121;
        internal System.Windows.Forms.Panel colourpallet122;
        internal System.Windows.Forms.Panel colourpallet123;
        internal System.Windows.Forms.Panel colourpallet124;
        internal System.Windows.Forms.Panel colourpallet125;
        internal System.Windows.Forms.Panel colourpallet126;
        internal System.Windows.Forms.Panel colourpallet127;
        internal System.Windows.Forms.Panel colourpallet128;
        internal System.Windows.Forms.TextBox txttopspace;
        internal System.Windows.Forms.Label Label40;
        internal System.Windows.Forms.TextBox txtsidespace;
        internal System.Windows.Forms.Label Label41;
        internal System.Windows.Forms.Timer pullbs;
        internal System.Windows.Forms.Timer pullbottom;
        internal System.Windows.Forms.Timer pullside;
        private System.Windows.Forms.Timer tmrsetupui;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.MenuStrip msTools;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gENNEWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gENLOADToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gENSAVEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gENEXITToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem moreControlsToolStripMenuItem;
    }
}