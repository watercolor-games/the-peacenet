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
    partial class MUDAuthenticator
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
            this.pnlmain = new System.Windows.Forms.Panel();
            this.pnlusers = new System.Windows.Forms.GroupBox();
            this.lbusers = new System.Windows.Forms.ListBox();
            this.fluserbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnview = new System.Windows.Forms.Button();
            this.btnrefreshusers = new System.Windows.Forms.Button();
            this.pnllogin = new System.Windows.Forms.GroupBox();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnlogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnlmain.SuspendLayout();
            this.pnlusers.SuspendLayout();
            this.fluserbuttons.SuspendLayout();
            this.pnllogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlmain);
            this.panel1.Controls.Add(this.pnllogin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(622, 430);
            this.panel1.TabIndex = 0;
            // 
            // pnlmain
            // 
            this.pnlmain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlmain.Controls.Add(this.pnlusers);
            this.pnlmain.Location = new System.Drawing.Point(13, 13);
            this.pnlmain.Name = "pnlmain";
            this.pnlmain.Size = new System.Drawing.Size(597, 405);
            this.pnlmain.TabIndex = 1;
            this.pnlmain.Visible = false;
            // 
            // pnlusers
            // 
            this.pnlusers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlusers.Controls.Add(this.lbusers);
            this.pnlusers.Controls.Add(this.fluserbuttons);
            this.pnlusers.Location = new System.Drawing.Point(16, 27);
            this.pnlusers.Name = "pnlusers";
            this.pnlusers.Size = new System.Drawing.Size(265, 356);
            this.pnlusers.TabIndex = 0;
            this.pnlusers.TabStop = false;
            this.pnlusers.Text = "groupBox1";
            // 
            // lbusers
            // 
            this.lbusers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbusers.FormattingEnabled = true;
            this.lbusers.Location = new System.Drawing.Point(3, 16);
            this.lbusers.Name = "lbusers";
            this.lbusers.Size = new System.Drawing.Size(259, 308);
            this.lbusers.TabIndex = 0;
            // 
            // fluserbuttons
            // 
            this.fluserbuttons.AutoSize = true;
            this.fluserbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fluserbuttons.Controls.Add(this.btnview);
            this.fluserbuttons.Controls.Add(this.btnrefreshusers);
            this.fluserbuttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fluserbuttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.fluserbuttons.Location = new System.Drawing.Point(3, 324);
            this.fluserbuttons.Name = "fluserbuttons";
            this.fluserbuttons.Size = new System.Drawing.Size(259, 29);
            this.fluserbuttons.TabIndex = 1;
            // 
            // btnview
            // 
            this.btnview.AutoSize = true;
            this.btnview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnview.Location = new System.Drawing.Point(173, 3);
            this.btnview.Name = "btnview";
            this.btnview.Size = new System.Drawing.Size(83, 23);
            this.btnview.TabIndex = 0;
            this.btnview.Text = "View user info";
            this.btnview.UseVisualStyleBackColor = true;
            // 
            // btnrefreshusers
            // 
            this.btnrefreshusers.AutoSize = true;
            this.btnrefreshusers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnrefreshusers.Location = new System.Drawing.Point(113, 3);
            this.btnrefreshusers.Name = "btnrefreshusers";
            this.btnrefreshusers.Size = new System.Drawing.Size(54, 23);
            this.btnrefreshusers.TabIndex = 1;
            this.btnrefreshusers.Text = "Refresh";
            this.btnrefreshusers.UseVisualStyleBackColor = true;
            // 
            // pnllogin
            // 
            this.pnllogin.Controls.Add(this.txtpassword);
            this.pnllogin.Controls.Add(this.label2);
            this.pnllogin.Controls.Add(this.btnlogin);
            this.pnllogin.Controls.Add(this.label1);
            this.pnllogin.Location = new System.Drawing.Point(177, 180);
            this.pnllogin.Name = "pnllogin";
            this.pnllogin.Size = new System.Drawing.Size(419, 251);
            this.pnllogin.TabIndex = 0;
            this.pnllogin.TabStop = false;
            this.pnllogin.Text = "Log in";
            // 
            // txtpassword
            // 
            this.txtpassword.Location = new System.Drawing.Point(13, 137);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.Size = new System.Drawing.Size(387, 20);
            this.txtpassword.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "{PASSWORD}:";
            // 
            // btnlogin
            // 
            this.btnlogin.Location = new System.Drawing.Point(325, 209);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(75, 23);
            this.btnlogin.TabIndex = 1;
            this.btnlogin.Text = "Submit";
            this.btnlogin.UseVisualStyleBackColor = true;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(393, 64);
            this.label1.TabIndex = 0;
            this.label1.Text = "{LOGIN_EXP}";
            // 
            // MUDAuthenticator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(622, 430);
            this.Controls.Add(this.panel1);
            this.Name = "MUDAuthenticator";
            this.Text = "Multi-User Domain Admin Panel";
            this.panel1.ResumeLayout(false);
            this.pnlmain.ResumeLayout(false);
            this.pnlusers.ResumeLayout(false);
            this.pnlusers.PerformLayout();
            this.fluserbuttons.ResumeLayout(false);
            this.fluserbuttons.PerformLayout();
            this.pnllogin.ResumeLayout(false);
            this.pnllogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox pnllogin;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlmain;
        private System.Windows.Forms.GroupBox pnlusers;
        private System.Windows.Forms.ListBox lbusers;
        private System.Windows.Forms.FlowLayoutPanel fluserbuttons;
        private System.Windows.Forms.Button btnview;
        private System.Windows.Forms.Button btnrefreshusers;
    }
}