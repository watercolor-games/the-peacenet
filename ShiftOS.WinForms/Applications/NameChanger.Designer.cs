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

namespace ShiftOS.WinForms.Applications {
    partial class NameChanger {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.flnames = new System.Windows.Forms.FlowLayoutPanel();
            this.flbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnloaddefault = new System.Windows.Forms.Button();
            this.btnimport = new System.Windows.Forms.Button();
            this.btnexport = new System.Windows.Forms.Button();
            this.btnapply = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.flbuttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flnames);
            this.panel1.Controls.Add(this.flbuttons);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(354, 349);
            this.panel1.TabIndex = 0;
            // 
            // flnames
            // 
            this.flnames.AutoScroll = true;
            this.flnames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flnames.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flnames.Location = new System.Drawing.Point(0, 0);
            this.flnames.Name = "flnames";
            this.flnames.Size = new System.Drawing.Size(354, 320);
            this.flnames.TabIndex = 0;
            this.flnames.WrapContents = false;
            // 
            // flbuttons
            // 
            this.flbuttons.AutoSize = true;
            this.flbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flbuttons.Controls.Add(this.btnclose);
            this.flbuttons.Controls.Add(this.btnloaddefault);
            this.flbuttons.Controls.Add(this.btnimport);
            this.flbuttons.Controls.Add(this.btnexport);
            this.flbuttons.Controls.Add(this.btnapply);
            this.flbuttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flbuttons.Location = new System.Drawing.Point(0, 320);
            this.flbuttons.Name = "flbuttons";
            this.flbuttons.Size = new System.Drawing.Size(354, 29);
            this.flbuttons.TabIndex = 0;
            // 
            // btnclose
            // 
            this.btnclose.AutoSize = true;
            this.btnclose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnclose.Location = new System.Drawing.Point(3, 3);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(43, 23);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "Close";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnloaddefault
            // 
            this.btnloaddefault.AutoSize = true;
            this.btnloaddefault.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnloaddefault.Location = new System.Drawing.Point(52, 3);
            this.btnloaddefault.Name = "btnloaddefault";
            this.btnloaddefault.Size = new System.Drawing.Size(76, 23);
            this.btnloaddefault.TabIndex = 1;
            this.btnloaddefault.Text = "Load default";
            this.btnloaddefault.UseVisualStyleBackColor = true;
            this.btnloaddefault.Click += new System.EventHandler(this.btnloaddefault_Click);
            // 
            // btnimport
            // 
            this.btnimport.AutoSize = true;
            this.btnimport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnimport.Location = new System.Drawing.Point(134, 3);
            this.btnimport.Name = "btnimport";
            this.btnimport.Size = new System.Drawing.Size(46, 23);
            this.btnimport.TabIndex = 2;
            this.btnimport.Text = "Import";
            this.btnimport.UseVisualStyleBackColor = true;
            this.btnimport.Click += new System.EventHandler(this.btnimport_Click);
            // 
            // btnexport
            // 
            this.btnexport.AutoSize = true;
            this.btnexport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnexport.Location = new System.Drawing.Point(186, 3);
            this.btnexport.Name = "btnexport";
            this.btnexport.Size = new System.Drawing.Size(47, 23);
            this.btnexport.TabIndex = 3;
            this.btnexport.Text = "Export";
            this.btnexport.UseVisualStyleBackColor = true;
            this.btnexport.Click += new System.EventHandler(this.btnexport_Click);
            // 
            // btnapply
            // 
            this.btnapply.AutoSize = true;
            this.btnapply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnapply.Location = new System.Drawing.Point(239, 3);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(43, 23);
            this.btnapply.TabIndex = 4;
            this.btnapply.Text = "Apply";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // NameChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "NameChanger";
            this.Size = new System.Drawing.Size(354, 349);
            this.Load += new System.EventHandler(this.NameChanger_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flbuttons.ResumeLayout(false);
            this.flbuttons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flnames;
        private System.Windows.Forms.FlowLayoutPanel flbuttons;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnloaddefault;
        private System.Windows.Forms.Button btnimport;
        private System.Windows.Forms.Button btnexport;
        private System.Windows.Forms.Button btnapply;
    }
}