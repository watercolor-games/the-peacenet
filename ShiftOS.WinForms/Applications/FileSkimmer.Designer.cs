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
    partial class FileSkimmer
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
            this.lvitems = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbcurrentfolder = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvitems
            // 
            this.lvitems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvitems.Location = new System.Drawing.Point(0, 0);
            this.lvitems.Name = "lvitems";
            this.lvitems.Size = new System.Drawing.Size(634, 356);
            this.lvitems.TabIndex = 0;
            this.lvitems.UseCompatibleStateImageBehavior = false;
            this.lvitems.DoubleClick += new System.EventHandler(this.lvitems_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvitems);
            this.panel1.Controls.Add(this.lbcurrentfolder);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 369);
            this.panel1.TabIndex = 1;
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
            // FileSkimmer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 369);
            this.Controls.Add(this.panel1);
            this.Name = "FileSkimmer";
            this.Text = "{FILE_SKIMMER_NAME}";
            this.Load += new System.EventHandler(this.FileSkimmer_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvitems;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbcurrentfolder;
    }
}