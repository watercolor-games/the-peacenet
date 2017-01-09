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

using ShiftOS.WinForms.Controls;

namespace ShiftOS.WinForms.Applications
{
    partial class VirusScanner
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
            this.btnfullscan = new System.Windows.Forms.Button();
            this.btnhomescan = new System.Windows.Forms.Button();
            this.btnsysscan = new System.Windows.Forms.Button();
            this.grpresults = new System.Windows.Forms.GroupBox();
            this.lbviruses = new System.Windows.Forms.ListBox();
            this.btnremoveviruses = new System.Windows.Forms.Button();
            this.lblresults = new System.Windows.Forms.Label();
            this.grpabout = new System.Windows.Forms.GroupBox();
            this.rtbterm = new TerminalBox();
            this.lblabout = new System.Windows.Forms.Label();
            this.pgcontents = new System.Windows.Forms.Panel();
            this.grpresults.SuspendLayout();
            this.grpabout.SuspendLayout();
            this.pgcontents.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnfullscan
            // 
            this.btnfullscan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnfullscan.Location = new System.Drawing.Point(10, 12);
            this.btnfullscan.Name = "btnfullscan";
            this.btnfullscan.Size = new System.Drawing.Size(175, 23);
            this.btnfullscan.TabIndex = 0;
            this.btnfullscan.Text = "{START_SYSTEM_SCAN}";
            this.btnfullscan.UseVisualStyleBackColor = true;
            this.btnfullscan.Click += new System.EventHandler(this.btnfullscan_Click);
            // 
            // btnhomescan
            // 
            this.btnhomescan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnhomescan.Location = new System.Drawing.Point(10, 41);
            this.btnhomescan.Name = "btnhomescan";
            this.btnhomescan.Size = new System.Drawing.Size(175, 23);
            this.btnhomescan.TabIndex = 1;
            this.btnhomescan.Text = "{SCAN_HOME}";
            this.btnhomescan.UseVisualStyleBackColor = true;
            this.btnhomescan.Click += new System.EventHandler(this.btnhomescan_Click);
            // 
            // btnsysscan
            // 
            this.btnsysscan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnsysscan.Location = new System.Drawing.Point(10, 70);
            this.btnsysscan.Name = "btnsysscan";
            this.btnsysscan.Size = new System.Drawing.Size(175, 23);
            this.btnsysscan.TabIndex = 2;
            this.btnsysscan.Text = "{SCAN_SYSTEM}";
            this.btnsysscan.UseVisualStyleBackColor = true;
            this.btnsysscan.Click += new System.EventHandler(this.btnsysscan_Click);
            // 
            // grpresults
            // 
            this.grpresults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpresults.Controls.Add(this.lbviruses);
            this.grpresults.Controls.Add(this.btnremoveviruses);
            this.grpresults.Controls.Add(this.lblresults);
            this.grpresults.Location = new System.Drawing.Point(3, 168);
            this.grpresults.Name = "grpresults";
            this.grpresults.Size = new System.Drawing.Size(179, 158);
            this.grpresults.TabIndex = 3;
            this.grpresults.TabStop = false;
            this.grpresults.Text = "{RESULTS}";
            // 
            // lbviruses
            // 
            this.lbviruses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbviruses.FormattingEnabled = true;
            this.lbviruses.Location = new System.Drawing.Point(3, 16);
            this.lbviruses.Name = "lbviruses";
            this.lbviruses.Size = new System.Drawing.Size(173, 116);
            this.lbviruses.TabIndex = 2;
            // 
            // btnremoveviruses
            // 
            this.btnremoveviruses.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnremoveviruses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnremoveviruses.Location = new System.Drawing.Point(3, 132);
            this.btnremoveviruses.Name = "btnremoveviruses";
            this.btnremoveviruses.Size = new System.Drawing.Size(173, 23);
            this.btnremoveviruses.TabIndex = 1;
            this.btnremoveviruses.Text = "Remove";
            this.btnremoveviruses.UseVisualStyleBackColor = true;
            this.btnremoveviruses.Visible = false;
            this.btnremoveviruses.Click += new System.EventHandler(this.btnremoveviruses_Click);
            // 
            // lblresults
            // 
            this.lblresults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblresults.Location = new System.Drawing.Point(3, 16);
            this.lblresults.Name = "lblresults";
            this.lblresults.Size = new System.Drawing.Size(173, 139);
            this.lblresults.TabIndex = 0;
            this.lblresults.Text = "{SCAN_NOT_STARTED}";
            // 
            // grpabout
            // 
            this.grpabout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpabout.Controls.Add(this.rtbterm);
            this.grpabout.Controls.Add(this.lblabout);
            this.grpabout.Location = new System.Drawing.Point(191, 12);
            this.grpabout.Name = "grpabout";
            this.grpabout.Size = new System.Drawing.Size(362, 314);
            this.grpabout.TabIndex = 5;
            this.grpabout.TabStop = false;
            this.grpabout.Text = "{ABOUT}";
            // 
            // rtbterm
            // 
            this.rtbterm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbterm.Location = new System.Drawing.Point(3, 16);
            this.rtbterm.Name = "rtbterm";
            this.rtbterm.Size = new System.Drawing.Size(356, 295);
            this.rtbterm.TabIndex = 1;
            this.rtbterm.Text = "";
            // 
            // lblabout
            // 
            this.lblabout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblabout.Location = new System.Drawing.Point(3, 16);
            this.lblabout.Name = "lblabout";
            this.lblabout.Size = new System.Drawing.Size(356, 295);
            this.lblabout.TabIndex = 0;
            this.lblabout.Text = "{VIRUSSCANNER_ABOUT}";
            // 
            // pgcontents
            // 
            this.pgcontents.BackColor = System.Drawing.Color.White;
            this.pgcontents.Controls.Add(this.grpabout);
            this.pgcontents.Controls.Add(this.grpresults);
            this.pgcontents.Controls.Add(this.btnsysscan);
            this.pgcontents.Controls.Add(this.btnhomescan);
            this.pgcontents.Controls.Add(this.btnfullscan);
            this.pgcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcontents.Location = new System.Drawing.Point(0, 0);
            this.pgcontents.Name = "pgcontents";
            this.pgcontents.Size = new System.Drawing.Size(565, 343);
            this.pgcontents.TabIndex = 25;
            // 
            // VirusScanner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgcontents);
            this.Name = "VirusScanner";
            this.Text = "{VIRUS_SCANNER_NAME}";
            this.Size = new System.Drawing.Size(565, 343);
            this.Load += new System.EventHandler(this.VirusScanner_Load);
            this.grpresults.ResumeLayout(false);
            this.grpabout.ResumeLayout(false);
            this.pgcontents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnfullscan;
        internal System.Windows.Forms.Button btnhomescan;
        internal System.Windows.Forms.Button btnsysscan;
        internal System.Windows.Forms.GroupBox grpresults;
        internal System.Windows.Forms.Button btnremoveviruses;
        internal System.Windows.Forms.Label lblresults;
        internal System.Windows.Forms.GroupBox grpabout;
        internal System.Windows.Forms.Label lblabout;
        internal System.Windows.Forms.Panel pgcontents;
        private TerminalBox rtbterm;
        private System.Windows.Forms.ListBox lbviruses;
    }
}