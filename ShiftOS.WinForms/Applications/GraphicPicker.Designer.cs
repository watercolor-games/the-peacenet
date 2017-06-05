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
    partial class GraphicPicker
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
            this.pgcontents = new System.Windows.Forms.Panel();
            this.btncancel = new System.Windows.Forms.Button();
            this.btnreset = new System.Windows.Forms.Button();
            this.btnapply = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.btnidlebrowse = new System.Windows.Forms.Button();
            this.txtidlefile = new System.Windows.Forms.TextBox();
            this.picidle = new System.Windows.Forms.PictureBox();
            this.btnzoom = new System.Windows.Forms.Button();
            this.btnstretch = new System.Windows.Forms.Button();
            this.btncentre = new System.Windows.Forms.Button();
            this.btntile = new System.Windows.Forms.Button();
            this.pnlgraphicholder = new System.Windows.Forms.Panel();
            this.picgraphic = new System.Windows.Forms.PictureBox();
            this.lblobjecttoskin = new System.Windows.Forms.Label();
            this.pgcontents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picidle)).BeginInit();
            this.pnlgraphicholder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picgraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // pgcontents
            // 
            this.pgcontents.BackColor = System.Drawing.Color.White;
            this.pgcontents.Controls.Add(this.btncancel);
            this.pgcontents.Controls.Add(this.btnreset);
            this.pgcontents.Controls.Add(this.btnapply);
            this.pgcontents.Controls.Add(this.Label2);
            this.pgcontents.Controls.Add(this.btnidlebrowse);
            this.pgcontents.Controls.Add(this.txtidlefile);
            this.pgcontents.Controls.Add(this.picidle);
            this.pgcontents.Controls.Add(this.btnzoom);
            this.pgcontents.Controls.Add(this.btnstretch);
            this.pgcontents.Controls.Add(this.btncentre);
            this.pgcontents.Controls.Add(this.btntile);
            this.pgcontents.Controls.Add(this.pnlgraphicholder);
            this.pgcontents.Controls.Add(this.lblobjecttoskin);
            this.pgcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcontents.Location = new System.Drawing.Point(0, 0);
            this.pgcontents.Name = "pgcontents";
            this.pgcontents.Size = new System.Drawing.Size(487, 383);
            this.pgcontents.TabIndex = 20;
            // 
            // btncancel
            // 
            this.btncancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btncancel.Location = new System.Drawing.Point(21, 335);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(109, 32);
            this.btncancel.TabIndex = 23;
            this.btncancel.Text = "Cancel";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // btnreset
            // 
            this.btnreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnreset.Location = new System.Drawing.Point(136, 335);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(206, 32);
            this.btnreset.TabIndex = 22;
            this.btnreset.Text = "Reset";
            this.btnreset.UseVisualStyleBackColor = true;
            this.btnreset.Click += new System.EventHandler(this.btnreset_Click);
            // 
            // btnapply
            // 
            this.btnapply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnapply.Location = new System.Drawing.Point(348, 335);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(118, 32);
            this.btnapply.TabIndex = 21;
            this.btnapply.Text = "Apply";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // Label2
            // 
            this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(125, 260);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(260, 28);
            this.Label2.TabIndex = 12;
            this.Label2.Tag = "header3";
            this.Label2.Text = "Idle";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnidlebrowse
            // 
            this.btnidlebrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnidlebrowse.Location = new System.Drawing.Point(392, 260);
            this.btnidlebrowse.Name = "btnidlebrowse";
            this.btnidlebrowse.Size = new System.Drawing.Size(73, 60);
            this.btnidlebrowse.TabIndex = 10;
            this.btnidlebrowse.Text = "Browse";
            this.btnidlebrowse.UseVisualStyleBackColor = true;
            this.btnidlebrowse.Click += new System.EventHandler(this.btnidlebrowse_Click);
            // 
            // txtidlefile
            // 
            this.txtidlefile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtidlefile.BackColor = System.Drawing.Color.White;
            this.txtidlefile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtidlefile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtidlefile.Location = new System.Drawing.Point(125, 291);
            this.txtidlefile.Multiline = true;
            this.txtidlefile.Name = "txtidlefile";
            this.txtidlefile.Size = new System.Drawing.Size(260, 29);
            this.txtidlefile.TabIndex = 9;
            this.txtidlefile.Text = "None";
            this.txtidlefile.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picidle
            // 
            this.picidle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picidle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picidle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picidle.Location = new System.Drawing.Point(19, 260);
            this.picidle.Name = "picidle";
            this.picidle.Size = new System.Drawing.Size(100, 60);
            this.picidle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picidle.TabIndex = 8;
            this.picidle.TabStop = false;
            // 
            // btnzoom
            // 
            this.btnzoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnzoom.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnzoom.FlatAppearance.BorderSize = 0;
            this.btnzoom.Location = new System.Drawing.Point(383, 144);
            this.btnzoom.Name = "btnzoom";
            this.btnzoom.Size = new System.Drawing.Size(82, 65);
            this.btnzoom.TabIndex = 7;
            this.btnzoom.Text = "Zoom";
            this.btnzoom.UseVisualStyleBackColor = true;
            this.btnzoom.Click += new System.EventHandler(this.btnzoom_Click);
            // 
            // btnstretch
            // 
            this.btnstretch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnstretch.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnstretch.FlatAppearance.BorderSize = 0;
            this.btnstretch.Location = new System.Drawing.Point(294, 144);
            this.btnstretch.Name = "btnstretch";
            this.btnstretch.Size = new System.Drawing.Size(82, 65);
            this.btnstretch.TabIndex = 6;
            this.btnstretch.Text = "Stretch";
            this.btnstretch.UseVisualStyleBackColor = true;
            this.btnstretch.Click += new System.EventHandler(this.btnstretch_Click);
            // 
            // btncentre
            // 
            this.btncentre.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btncentre.FlatAppearance.BorderSize = 0;
            this.btncentre.Location = new System.Drawing.Point(108, 144);
            this.btncentre.Name = "btncentre";
            this.btncentre.Size = new System.Drawing.Size(82, 65);
            this.btncentre.TabIndex = 5;
            this.btncentre.Text = "Center";
            this.btncentre.UseVisualStyleBackColor = true;
            this.btncentre.Click += new System.EventHandler(this.btncentre_Click);
            // 
            // btntile
            // 
            this.btntile.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btntile.FlatAppearance.BorderSize = 0;
            this.btntile.Location = new System.Drawing.Point(19, 144);
            this.btntile.Name = "btntile";
            this.btntile.Size = new System.Drawing.Size(82, 65);
            this.btntile.TabIndex = 4;
            this.btntile.Text = "Tile";
            this.btntile.UseVisualStyleBackColor = true;
            this.btntile.Click += new System.EventHandler(this.btntile_Click);
            // 
            // pnlgraphicholder
            // 
            this.pnlgraphicholder.Controls.Add(this.picgraphic);
            this.pnlgraphicholder.Location = new System.Drawing.Point(19, 38);
            this.pnlgraphicholder.Name = "pnlgraphicholder";
            this.pnlgraphicholder.Size = new System.Drawing.Size(350, 100);
            this.pnlgraphicholder.TabIndex = 3;
            // 
            // picgraphic
            // 
            this.picgraphic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picgraphic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picgraphic.Location = new System.Drawing.Point(0, 0);
            this.picgraphic.Name = "picgraphic";
            this.picgraphic.Size = new System.Drawing.Size(350, 100);
            this.picgraphic.TabIndex = 0;
            this.picgraphic.TabStop = false;
            // 
            // lblobjecttoskin
            // 
            this.lblobjecttoskin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblobjecttoskin.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblobjecttoskin.Location = new System.Drawing.Point(19, 9);
            this.lblobjecttoskin.Name = "lblobjecttoskin";
            this.lblobjecttoskin.Size = new System.Drawing.Size(447, 23);
            this.lblobjecttoskin.TabIndex = 2;
            this.lblobjecttoskin.Tag = "header1";
            this.lblobjecttoskin.Text = "Close Button";
            this.lblobjecttoskin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // GraphicPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgcontents);
            this.Name = "GraphicPicker";
            this.Size = new System.Drawing.Size(487, 383);
            this.Load += new System.EventHandler(this.Graphic_Picker_Load);
            this.pgcontents.ResumeLayout(false);
            this.pgcontents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picidle)).EndInit();
            this.pnlgraphicholder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picgraphic)).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pgcontents;
        internal System.Windows.Forms.Button btncancel;
        internal System.Windows.Forms.Button btnreset;
        internal System.Windows.Forms.Button btnapply;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Button btnidlebrowse;
        internal System.Windows.Forms.TextBox txtidlefile;
        internal System.Windows.Forms.PictureBox picidle;
        internal System.Windows.Forms.Button btnzoom;
        internal System.Windows.Forms.Button btnstretch;
        internal System.Windows.Forms.Button btncentre;
        internal System.Windows.Forms.Button btntile;
        internal System.Windows.Forms.Panel pnlgraphicholder;
        internal System.Windows.Forms.PictureBox picgraphic;
        internal System.Windows.Forms.Label lblobjecttoskin;
    }
}