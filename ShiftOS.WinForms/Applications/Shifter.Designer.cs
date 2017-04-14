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
    partial class Shifter
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shifter));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlpreview = new System.Windows.Forms.Panel();
            this.pnlwindow = new System.Windows.Forms.Panel();
            this.pnlcontents = new System.Windows.Forms.Panel();
            this.pnltitle = new System.Windows.Forms.Panel();
            this.pnlicon = new System.Windows.Forms.Panel();
            this.pnlminimize = new System.Windows.Forms.Panel();
            this.pnlmaximize = new System.Windows.Forms.Panel();
            this.pnlclose = new System.Windows.Forms.Panel();
            this.pnltitleleft = new System.Windows.Forms.Panel();
            this.pnltitleright = new System.Windows.Forms.Panel();
            this.lbtitletext = new System.Windows.Forms.Label();
            this.pnlbottom = new System.Windows.Forms.Panel();
            this.pnlbottomr = new System.Windows.Forms.Panel();
            this.pnlbottoml = new System.Windows.Forms.Panel();
            this.pnlright = new System.Windows.Forms.Panel();
            this.pnlleft = new System.Windows.Forms.Panel();
            this.pnlsettingsholder = new System.Windows.Forms.Panel();
            this.flbody = new System.Windows.Forms.FlowLayoutPanel();
            this.flcategory = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlcategoryholder = new System.Windows.Forms.Panel();
            this.flmeta = new System.Windows.Forms.FlowLayoutPanel();
            this.btnapply = new System.Windows.Forms.Button();
            this.mspreview = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subitem1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subitem2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subitem3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tspreview = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.pnlintro = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnlpreview.SuspendLayout();
            this.pnlwindow.SuspendLayout();
            this.pnlcontents.SuspendLayout();
            this.pnltitle.SuspendLayout();
            this.pnlbottom.SuspendLayout();
            this.pnlsettingsholder.SuspendLayout();
            this.pnlcategoryholder.SuspendLayout();
            this.mspreview.SuspendLayout();
            this.tspreview.SuspendLayout();
            this.pnlintro.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlpreview);
            this.panel1.Controls.Add(this.pnlsettingsholder);
            this.panel1.Controls.Add(this.pnlcategoryholder);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 539);
            this.panel1.TabIndex = 0;
            // 
            // pnlpreview
            // 
            this.pnlpreview.Controls.Add(this.pnlintro);
            this.pnlpreview.Controls.Add(this.pnlwindow);
            this.pnlpreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlpreview.Location = new System.Drawing.Point(136, 0);
            this.pnlpreview.Name = "pnlpreview";
            this.pnlpreview.Size = new System.Drawing.Size(757, 286);
            this.pnlpreview.TabIndex = 1;
            // 
            // pnlwindow
            // 
            this.pnlwindow.Controls.Add(this.pnlcontents);
            this.pnlwindow.Controls.Add(this.pnltitle);
            this.pnlwindow.Controls.Add(this.pnlbottom);
            this.pnlwindow.Controls.Add(this.pnlright);
            this.pnlwindow.Controls.Add(this.pnlleft);
            this.pnlwindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlwindow.Location = new System.Drawing.Point(0, 0);
            this.pnlwindow.Name = "pnlwindow";
            this.pnlwindow.Size = new System.Drawing.Size(757, 286);
            this.pnlwindow.TabIndex = 0;
            // 
            // pnlcontents
            // 
            this.pnlcontents.BackColor = System.Drawing.Color.Black;
            this.pnlcontents.Controls.Add(this.tspreview);
            this.pnlcontents.Controls.Add(this.mspreview);
            this.pnlcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlcontents.ForeColor = System.Drawing.Color.White;
            this.pnlcontents.Location = new System.Drawing.Point(2, 30);
            this.pnlcontents.Name = "pnlcontents";
            this.pnlcontents.Size = new System.Drawing.Size(753, 254);
            this.pnlcontents.TabIndex = 9;
            // 
            // pnltitle
            // 
            this.pnltitle.BackColor = System.Drawing.Color.Black;
            this.pnltitle.Controls.Add(this.pnlicon);
            this.pnltitle.Controls.Add(this.pnlminimize);
            this.pnltitle.Controls.Add(this.pnlmaximize);
            this.pnltitle.Controls.Add(this.pnlclose);
            this.pnltitle.Controls.Add(this.pnltitleleft);
            this.pnltitle.Controls.Add(this.pnltitleright);
            this.pnltitle.Controls.Add(this.lbtitletext);
            this.pnltitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnltitle.Location = new System.Drawing.Point(2, 0);
            this.pnltitle.Name = "pnltitle";
            this.pnltitle.Size = new System.Drawing.Size(753, 30);
            this.pnltitle.TabIndex = 5;
            // 
            // pnlicon
            // 
            this.pnlicon.Location = new System.Drawing.Point(9, -76);
            this.pnlicon.Name = "pnlicon";
            this.pnlicon.Size = new System.Drawing.Size(200, 100);
            this.pnlicon.TabIndex = 6;
            // 
            // pnlminimize
            // 
            this.pnlminimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlminimize.BackColor = System.Drawing.Color.Green;
            this.pnlminimize.Location = new System.Drawing.Point(672, 3);
            this.pnlminimize.Name = "pnlminimize";
            this.pnlminimize.Size = new System.Drawing.Size(24, 24);
            this.pnlminimize.TabIndex = 3;
            // 
            // pnlmaximize
            // 
            this.pnlmaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlmaximize.BackColor = System.Drawing.Color.Yellow;
            this.pnlmaximize.Location = new System.Drawing.Point(699, 3);
            this.pnlmaximize.Name = "pnlmaximize";
            this.pnlmaximize.Size = new System.Drawing.Size(24, 24);
            this.pnlmaximize.TabIndex = 2;
            // 
            // pnlclose
            // 
            this.pnlclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlclose.BackColor = System.Drawing.Color.Red;
            this.pnlclose.Location = new System.Drawing.Point(726, 3);
            this.pnlclose.Name = "pnlclose";
            this.pnlclose.Size = new System.Drawing.Size(24, 24);
            this.pnlclose.TabIndex = 1;
            // 
            // pnltitleleft
            // 
            this.pnltitleleft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnltitleleft.Location = new System.Drawing.Point(0, 0);
            this.pnltitleleft.Name = "pnltitleleft";
            this.pnltitleleft.Size = new System.Drawing.Size(2, 30);
            this.pnltitleleft.TabIndex = 4;
            // 
            // pnltitleright
            // 
            this.pnltitleright.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnltitleright.Location = new System.Drawing.Point(751, 0);
            this.pnltitleright.Name = "pnltitleright";
            this.pnltitleright.Size = new System.Drawing.Size(2, 30);
            this.pnltitleright.TabIndex = 5;
            // 
            // lbtitletext
            // 
            this.lbtitletext.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbtitletext.AutoSize = true;
            this.lbtitletext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbtitletext.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.lbtitletext.ForeColor = System.Drawing.Color.White;
            this.lbtitletext.Location = new System.Drawing.Point(75, 9);
            this.lbtitletext.Name = "lbtitletext";
            this.lbtitletext.Size = new System.Drawing.Size(77, 14);
            this.lbtitletext.TabIndex = 0;
            this.lbtitletext.Text = "Title text";
            this.lbtitletext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbtitletext.UseMnemonic = false;
            // 
            // pnlbottom
            // 
            this.pnlbottom.BackColor = System.Drawing.Color.Black;
            this.pnlbottom.Controls.Add(this.pnlbottomr);
            this.pnlbottom.Controls.Add(this.pnlbottoml);
            this.pnlbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlbottom.Location = new System.Drawing.Point(2, 284);
            this.pnlbottom.Name = "pnlbottom";
            this.pnlbottom.Size = new System.Drawing.Size(753, 2);
            this.pnlbottom.TabIndex = 6;
            // 
            // pnlbottomr
            // 
            this.pnlbottomr.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlbottomr.Location = new System.Drawing.Point(751, 0);
            this.pnlbottomr.Name = "pnlbottomr";
            this.pnlbottomr.Size = new System.Drawing.Size(2, 2);
            this.pnlbottomr.TabIndex = 3;
            // 
            // pnlbottoml
            // 
            this.pnlbottoml.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlbottoml.Location = new System.Drawing.Point(0, 0);
            this.pnlbottoml.Name = "pnlbottoml";
            this.pnlbottoml.Size = new System.Drawing.Size(2, 2);
            this.pnlbottoml.TabIndex = 2;
            // 
            // pnlright
            // 
            this.pnlright.BackColor = System.Drawing.Color.Black;
            this.pnlright.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlright.Location = new System.Drawing.Point(755, 0);
            this.pnlright.Name = "pnlright";
            this.pnlright.Size = new System.Drawing.Size(2, 286);
            this.pnlright.TabIndex = 8;
            // 
            // pnlleft
            // 
            this.pnlleft.BackColor = System.Drawing.Color.Black;
            this.pnlleft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlleft.Location = new System.Drawing.Point(0, 0);
            this.pnlleft.Name = "pnlleft";
            this.pnlleft.Size = new System.Drawing.Size(2, 286);
            this.pnlleft.TabIndex = 7;
            // 
            // pnlsettingsholder
            // 
            this.pnlsettingsholder.Controls.Add(this.flbody);
            this.pnlsettingsholder.Controls.Add(this.flcategory);
            this.pnlsettingsholder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlsettingsholder.Location = new System.Drawing.Point(136, 286);
            this.pnlsettingsholder.Name = "pnlsettingsholder";
            this.pnlsettingsholder.Padding = new System.Windows.Forms.Padding(10);
            this.pnlsettingsholder.Size = new System.Drawing.Size(757, 253);
            this.pnlsettingsholder.TabIndex = 2;
            // 
            // flbody
            // 
            this.flbody.AutoScroll = true;
            this.flbody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flbody.Location = new System.Drawing.Point(136, 10);
            this.flbody.Name = "flbody";
            this.flbody.Size = new System.Drawing.Size(611, 233);
            this.flbody.TabIndex = 3;
            // 
            // flcategory
            // 
            this.flcategory.Dock = System.Windows.Forms.DockStyle.Left;
            this.flcategory.Location = new System.Drawing.Point(10, 10);
            this.flcategory.Name = "flcategory";
            this.flcategory.Size = new System.Drawing.Size(126, 233);
            this.flcategory.TabIndex = 2;
            // 
            // pnlcategoryholder
            // 
            this.pnlcategoryholder.Controls.Add(this.flmeta);
            this.pnlcategoryholder.Controls.Add(this.btnapply);
            this.pnlcategoryholder.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlcategoryholder.Location = new System.Drawing.Point(0, 0);
            this.pnlcategoryholder.Name = "pnlcategoryholder";
            this.pnlcategoryholder.Padding = new System.Windows.Forms.Padding(10);
            this.pnlcategoryholder.Size = new System.Drawing.Size(136, 539);
            this.pnlcategoryholder.TabIndex = 0;
            // 
            // flmeta
            // 
            this.flmeta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flmeta.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flmeta.Location = new System.Drawing.Point(14, 14);
            this.flmeta.Name = "flmeta";
            this.flmeta.Size = new System.Drawing.Size(126, 458);
            this.flmeta.TabIndex = 1;
            // 
            // btnapply
            // 
            this.btnapply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnapply.Location = new System.Drawing.Point(13, 478);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(127, 48);
            this.btnapply.TabIndex = 0;
            this.btnapply.Text = "Apply";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // mspreview
            // 
            this.mspreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.mspreview.Location = new System.Drawing.Point(0, 0);
            this.mspreview.Name = "mspreview";
            this.mspreview.Size = new System.Drawing.Size(753, 24);
            this.mspreview.TabIndex = 0;
            this.mspreview.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem1ToolStripMenuItem,
            this.menuItem2ToolStripMenuItem,
            this.separatorToolStripMenuItem,
            this.menuItem3ToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // menuItem1ToolStripMenuItem
            // 
            this.menuItem1ToolStripMenuItem.Name = "menuItem1ToolStripMenuItem";
            this.menuItem1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.menuItem1ToolStripMenuItem.Text = "Menu Item 1";
            // 
            // menuItem2ToolStripMenuItem
            // 
            this.menuItem2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subitem1ToolStripMenuItem,
            this.subitem2ToolStripMenuItem,
            this.subitem3ToolStripMenuItem});
            this.menuItem2ToolStripMenuItem.Name = "menuItem2ToolStripMenuItem";
            this.menuItem2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.menuItem2ToolStripMenuItem.Text = "Menu Item 2";
            // 
            // subitem1ToolStripMenuItem
            // 
            this.subitem1ToolStripMenuItem.Name = "subitem1ToolStripMenuItem";
            this.subitem1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.subitem1ToolStripMenuItem.Text = "Subitem 1";
            // 
            // subitem2ToolStripMenuItem
            // 
            this.subitem2ToolStripMenuItem.Name = "subitem2ToolStripMenuItem";
            this.subitem2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.subitem2ToolStripMenuItem.Text = "Subitem 2";
            // 
            // subitem3ToolStripMenuItem
            // 
            this.subitem3ToolStripMenuItem.Name = "subitem3ToolStripMenuItem";
            this.subitem3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.subitem3ToolStripMenuItem.Text = "Subitem 3";
            // 
            // separatorToolStripMenuItem
            // 
            this.separatorToolStripMenuItem.Name = "separatorToolStripMenuItem";
            this.separatorToolStripMenuItem.Size = new System.Drawing.Size(149, 6);
            // 
            // menuItem3ToolStripMenuItem
            // 
            this.menuItem3ToolStripMenuItem.Name = "menuItem3ToolStripMenuItem";
            this.menuItem3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.menuItem3ToolStripMenuItem.Text = "Menu Item 3";
            // 
            // tspreview
            // 
            this.tspreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripTextBox1});
            this.tspreview.Location = new System.Drawing.Point(0, 24);
            this.tspreview.Name = "tspreview";
            this.tspreview.Size = new System.Drawing.Size(753, 25);
            this.tspreview.TabIndex = 1;
            this.tspreview.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabel1.Text = "Toolbar Label:";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Enabled = false;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(56, 22);
            this.toolStripButton1.Text = "Disabled";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(51, 22);
            this.toolStripButton2.Text = "Regular";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Checked = true;
            this.toolStripButton3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(57, 22);
            this.toolStripButton3.Text = "Checked";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            // 
            // pnlintro
            // 
            this.pnlintro.Controls.Add(this.label2);
            this.pnlintro.Controls.Add(this.label1);
            this.pnlintro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlintro.Location = new System.Drawing.Point(0, 0);
            this.pnlintro.Name = "pnlintro";
            this.pnlintro.Size = new System.Drawing.Size(757, 286);
            this.pnlintro.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(757, 54);
            this.label1.TabIndex = 0;
            this.label1.Tag = "header2";
            this.label1.Text = "Welcome to the Shifter!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(757, 232);
            this.label2.TabIndex = 1;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Shifter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Shifter";
            this.Size = new System.Drawing.Size(893, 539);
            this.Load += new System.EventHandler(this.Shifter_Load);
            this.panel1.ResumeLayout(false);
            this.pnlpreview.ResumeLayout(false);
            this.pnlwindow.ResumeLayout(false);
            this.pnlcontents.ResumeLayout(false);
            this.pnlcontents.PerformLayout();
            this.pnltitle.ResumeLayout(false);
            this.pnltitle.PerformLayout();
            this.pnlbottom.ResumeLayout(false);
            this.pnlsettingsholder.ResumeLayout(false);
            this.pnlcategoryholder.ResumeLayout(false);
            this.mspreview.ResumeLayout(false);
            this.mspreview.PerformLayout();
            this.tspreview.ResumeLayout(false);
            this.tspreview.PerformLayout();
            this.pnlintro.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlcategoryholder;
        private System.Windows.Forms.FlowLayoutPanel flmeta;
        private System.Windows.Forms.Button btnapply;
        private System.Windows.Forms.Panel pnlsettingsholder;
        private System.Windows.Forms.FlowLayoutPanel flbody;
        private System.Windows.Forms.FlowLayoutPanel flcategory;
        private System.Windows.Forms.Panel pnlpreview;
        private System.Windows.Forms.Panel pnlwindow;
        private System.Windows.Forms.Panel pnlcontents;
        private System.Windows.Forms.Panel pnltitle;
        private System.Windows.Forms.Panel pnlicon;
        private System.Windows.Forms.Panel pnlminimize;
        private System.Windows.Forms.Panel pnlmaximize;
        private System.Windows.Forms.Panel pnlclose;
        private System.Windows.Forms.Panel pnltitleleft;
        private System.Windows.Forms.Panel pnltitleright;
        private System.Windows.Forms.Label lbtitletext;
        private System.Windows.Forms.Panel pnlbottom;
        private System.Windows.Forms.Panel pnlbottomr;
        private System.Windows.Forms.Panel pnlbottoml;
        private System.Windows.Forms.Panel pnlright;
        private System.Windows.Forms.Panel pnlleft;
        private System.Windows.Forms.MenuStrip mspreview;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subitem1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subitem2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subitem3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem3ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip tspreview;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.Panel pnlintro;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}