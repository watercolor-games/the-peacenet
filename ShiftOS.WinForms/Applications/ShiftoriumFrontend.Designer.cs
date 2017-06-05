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
    partial class ShiftoriumFrontend
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbupgradedesc = new System.Windows.Forms.Label();
            this.pnlupgradeactions = new System.Windows.Forms.Panel();
            this.btnbuy = new System.Windows.Forms.Button();
            this.lbupgradetitle = new System.Windows.Forms.Label();
            this.pnllist = new System.Windows.Forms.Panel();
            this.lbnoupgrades = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblcategorytext = new System.Windows.Forms.Label();
            this.btncat_forward = new System.Windows.Forms.Button();
            this.btncat_back = new System.Windows.Forms.Button();
            this.lbcodepoints = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pgupgradeprogress = new ShiftOS.WinForms.Controls.ShiftedProgressBar();
            this.lbupgrades = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlupgradeactions.SuspendLayout();
            this.pnllist.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pnllist);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(782, 427);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbupgradedesc);
            this.panel2.Controls.Add(this.pnlupgradeactions);
            this.panel2.Controls.Add(this.lbupgradetitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(406, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(376, 427);
            this.panel2.TabIndex = 1;
            // 
            // lbupgradedesc
            // 
            this.lbupgradedesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbupgradedesc.Location = new System.Drawing.Point(0, 42);
            this.lbupgradedesc.Name = "lbupgradedesc";
            this.lbupgradedesc.Size = new System.Drawing.Size(376, 348);
            this.lbupgradedesc.TabIndex = 2;
            this.lbupgradedesc.Text = "{SHIFTORIUM_EXP}";
            this.lbupgradedesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbupgradedesc.UseCompatibleTextRendering = true;
            // 
            // pnlupgradeactions
            // 
            this.pnlupgradeactions.Controls.Add(this.btnbuy);
            this.pnlupgradeactions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlupgradeactions.Location = new System.Drawing.Point(0, 390);
            this.pnlupgradeactions.Name = "pnlupgradeactions";
            this.pnlupgradeactions.Size = new System.Drawing.Size(376, 37);
            this.pnlupgradeactions.TabIndex = 1;
            // 
            // btnbuy
            // 
            this.btnbuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnbuy.AutoSize = true;
            this.btnbuy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnbuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuy.Location = new System.Drawing.Point(327, 9);
            this.btnbuy.Name = "btnbuy";
            this.btnbuy.Size = new System.Drawing.Size(37, 25);
            this.btnbuy.TabIndex = 0;
            this.btnbuy.Text = "Buy";
            this.btnbuy.UseVisualStyleBackColor = true;
            this.btnbuy.Visible = false;
            this.btnbuy.Click += new System.EventHandler(this.btnbuy_Click);
            // 
            // lbupgradetitle
            // 
            this.lbupgradetitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbupgradetitle.Location = new System.Drawing.Point(0, 0);
            this.lbupgradetitle.Name = "lbupgradetitle";
            this.lbupgradetitle.Size = new System.Drawing.Size(376, 42);
            this.lbupgradetitle.TabIndex = 0;
            this.lbupgradetitle.Text = "{WELCOME_TO_SHIFTORIUM}";
            this.lbupgradetitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbupgradetitle.UseCompatibleTextRendering = true;
            // 
            // pnllist
            // 
            this.pnllist.Controls.Add(this.lbnoupgrades);
            this.pnllist.Controls.Add(this.panel3);
            this.pnllist.Controls.Add(this.lbcodepoints);
            this.pnllist.Controls.Add(this.label1);
            this.pnllist.Controls.Add(this.pgupgradeprogress);
            this.pnllist.Controls.Add(this.lbupgrades);
            this.pnllist.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnllist.Location = new System.Drawing.Point(0, 0);
            this.pnllist.Name = "pnllist";
            this.pnllist.Size = new System.Drawing.Size(406, 427);
            this.pnllist.TabIndex = 0;
            // 
            // lbnoupgrades
            // 
            this.lbnoupgrades.AutoSize = true;
            this.lbnoupgrades.Location = new System.Drawing.Point(69, 183);
            this.lbnoupgrades.Name = "lbnoupgrades";
            this.lbnoupgrades.Size = new System.Drawing.Size(71, 13);
            this.lbnoupgrades.TabIndex = 6;
            this.lbnoupgrades.Tag = "header2";
            this.lbnoupgrades.Text = "No upgrades!";
            this.lbnoupgrades.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblcategorytext);
            this.panel3.Controls.Add(this.btncat_forward);
            this.panel3.Controls.Add(this.btncat_back);
            this.panel3.Location = new System.Drawing.Point(6, 76);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(394, 23);
            this.panel3.TabIndex = 5;
            // 
            // lblcategorytext
            // 
            this.lblcategorytext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblcategorytext.Location = new System.Drawing.Point(29, 0);
            this.lblcategorytext.Name = "lblcategorytext";
            this.lblcategorytext.Size = new System.Drawing.Size(336, 23);
            this.lblcategorytext.TabIndex = 2;
            this.lblcategorytext.Text = "No Upgrades";
            this.lblcategorytext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btncat_forward
            // 
            this.btncat_forward.AutoSize = true;
            this.btncat_forward.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btncat_forward.Dock = System.Windows.Forms.DockStyle.Right;
            this.btncat_forward.Location = new System.Drawing.Point(365, 0);
            this.btncat_forward.Name = "btncat_forward";
            this.btncat_forward.Size = new System.Drawing.Size(29, 23);
            this.btncat_forward.TabIndex = 1;
            this.btncat_forward.Text = "-->";
            this.btncat_forward.UseVisualStyleBackColor = true;
            this.btncat_forward.Click += new System.EventHandler(this.btncat_forward_Click);
            // 
            // btncat_back
            // 
            this.btncat_back.AutoSize = true;
            this.btncat_back.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btncat_back.Dock = System.Windows.Forms.DockStyle.Left;
            this.btncat_back.Location = new System.Drawing.Point(0, 0);
            this.btncat_back.Name = "btncat_back";
            this.btncat_back.Size = new System.Drawing.Size(29, 23);
            this.btncat_back.TabIndex = 0;
            this.btncat_back.Text = "<--";
            this.btncat_back.UseVisualStyleBackColor = true;
            this.btncat_back.Click += new System.EventHandler(this.btncat_back_Click);
            // 
            // lbcodepoints
            // 
            this.lbcodepoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbcodepoints.AutoSize = true;
            this.lbcodepoints.Location = new System.Drawing.Point(128, 357);
            this.lbcodepoints.Name = "lbcodepoints";
            this.lbcodepoints.Size = new System.Drawing.Size(135, 13);
            this.lbcodepoints.TabIndex = 3;
            this.lbcodepoints.Text = "You have: %cp Codepoints";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 399);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "{UPGRADE_PROGRESS}:";
            // 
            // pgupgradeprogress
            // 
            this.pgupgradeprogress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgupgradeprogress.Location = new System.Drawing.Point(146, 390);
            this.pgupgradeprogress.Maximum = 100;
            this.pgupgradeprogress.Name = "pgupgradeprogress";
            this.pgupgradeprogress.Size = new System.Drawing.Size(254, 23);
            this.pgupgradeprogress.TabIndex = 1;
            this.pgupgradeprogress.Value = 25;
            // 
            // lbupgrades
            // 
            this.lbupgrades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbupgrades.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbupgrades.FormattingEnabled = true;
            this.lbupgrades.Location = new System.Drawing.Point(3, 105);
            this.lbupgrades.Name = "lbupgrades";
            this.lbupgrades.Size = new System.Drawing.Size(397, 238);
            this.lbupgrades.TabIndex = 0;
            this.lbupgrades.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbupgrades_DrawItem);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 399);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 2;
            // 
            // ShiftoriumFrontend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.LightGreen;
            this.Name = "ShiftoriumFrontend";
            this.Size = new System.Drawing.Size(782, 427);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlupgradeactions.ResumeLayout(false);
            this.pnlupgradeactions.PerformLayout();
            this.pnllist.ResumeLayout(false);
            this.pnllist.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnllist;
        private System.Windows.Forms.ListBox lbupgrades;
        private System.Windows.Forms.Label lbupgradedesc;
        private System.Windows.Forms.Panel pnlupgradeactions;
        private System.Windows.Forms.Label lbupgradetitle;
        private System.Windows.Forms.Button btnbuy;
        private ShiftedProgressBar pgupgradeprogress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbcodepoints;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblcategorytext;
        private System.Windows.Forms.Button btncat_forward;
        private System.Windows.Forms.Button btncat_back;
        private System.Windows.Forms.Label lbnoupgrades;
    }
}