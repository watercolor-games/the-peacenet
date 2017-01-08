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
    partial class FileDialog
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
            this.lbcurrentfolder = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlfiletype = new System.Windows.Forms.Panel();
            this.btnok = new System.Windows.Forms.Button();
            this.cbfiletypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtfilename = new System.Windows.Forms.TextBox();
            this.lvitems = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.pnlfiletype.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbcurrentfolder
            // 
            this.lbcurrentfolder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbcurrentfolder.Location = new System.Drawing.Point(0, 356);
            this.lbcurrentfolder.Name = "lbcurrentfolder";
            this.lbcurrentfolder.Size = new System.Drawing.Size(634, 13);
            this.lbcurrentfolder.TabIndex = 1;
            this.lbcurrentfolder.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlfiletype);
            this.panel1.Controls.Add(this.lvitems);
            this.panel1.Controls.Add(this.lbcurrentfolder);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 369);
            this.panel1.TabIndex = 2;
            // 
            // pnlfiletype
            // 
            this.pnlfiletype.Controls.Add(this.btnok);
            this.pnlfiletype.Controls.Add(this.cbfiletypes);
            this.pnlfiletype.Controls.Add(this.label1);
            this.pnlfiletype.Controls.Add(this.txtfilename);
            this.pnlfiletype.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlfiletype.Location = new System.Drawing.Point(0, 330);
            this.pnlfiletype.Name = "pnlfiletype";
            this.pnlfiletype.Size = new System.Drawing.Size(634, 26);
            this.pnlfiletype.TabIndex = 2;
            // 
            // btnok
            // 
            this.btnok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnok.Location = new System.Drawing.Point(551, 2);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(75, 23);
            this.btnok.TabIndex = 3;
            this.btnok.Text = "Open";
            this.btnok.UseVisualStyleBackColor = true;
            // 
            // cbfiletypes
            // 
            this.cbfiletypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbfiletypes.FormattingEnabled = true;
            this.cbfiletypes.Location = new System.Drawing.Point(424, 2);
            this.cbfiletypes.Name = "cbfiletypes";
            this.cbfiletypes.Size = new System.Drawing.Size(121, 21);
            this.cbfiletypes.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtfilename
            // 
            this.txtfilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtfilename.Location = new System.Drawing.Point(47, 3);
            this.txtfilename.Name = "txtfilename";
            this.txtfilename.Size = new System.Drawing.Size(371, 20);
            this.txtfilename.TabIndex = 0;
            // 
            // lvitems
            // 
            this.lvitems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvitems.Location = new System.Drawing.Point(0, 0);
            this.lvitems.Name = "lvitems";
            this.lvitems.Size = new System.Drawing.Size(634, 356);
            this.lvitems.TabIndex = 0;
            this.lvitems.UseCompatibleStateImageBehavior = false;
            // 
            // FileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 369);
            this.Controls.Add(this.panel1);
            this.Name = "FileDialog";
            this.Text = "{FILE_DIALOG_NAME}";
            this.panel1.ResumeLayout(false);
            this.pnlfiletype.ResumeLayout(false);
            this.pnlfiletype.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbcurrentfolder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvitems;
        private System.Windows.Forms.Panel pnlfiletype;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.ComboBox cbfiletypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtfilename;
    }
}