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

using ShiftOS.Engine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    partial class WindowBorder
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            this.pnlleft = new System.Windows.Forms.Panel();
            this.pnlright = new System.Windows.Forms.Panel();
            this.pnlcontents = new System.Windows.Forms.Panel();
            this.pnltitle.SuspendLayout();
            this.pnlbottom.SuspendLayout();
            this.SuspendLayout();
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
            this.pnltitle.Location = new System.Drawing.Point(0, 0);
            this.pnltitle.Name = "pnltitle";
            this.pnltitle.Size = new System.Drawing.Size(730, 30);
            this.pnltitle.TabIndex = 0;
            this.pnltitle.Paint += new System.Windows.Forms.PaintEventHandler(this.pnltitle_Paint);
            this.pnltitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnltitle_MouseMove);
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
            this.pnlminimize.Location = new System.Drawing.Point(649, 3);
            this.pnlminimize.Name = "pnlminimize";
            this.pnlminimize.Size = new System.Drawing.Size(24, 24);
            this.pnlminimize.TabIndex = 3;
            this.pnlminimize.Click += new System.EventHandler(this.pnlminimize_Click);
            this.pnlminimize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlminimize_MouseDown);
            this.pnlminimize.MouseEnter += new System.EventHandler(this.pnlminimize_MouseEnter);
            this.pnlminimize.MouseLeave += new System.EventHandler(this.pnlminimize_MouseLeave);
            this.pnlminimize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlminimize_MouseUp);
            // 
            // pnlmaximize
            // 
            this.pnlmaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlmaximize.BackColor = System.Drawing.Color.Yellow;
            this.pnlmaximize.Location = new System.Drawing.Point(676, 3);
            this.pnlmaximize.Name = "pnlmaximize";
            this.pnlmaximize.Size = new System.Drawing.Size(24, 24);
            this.pnlmaximize.TabIndex = 2;
            this.pnlmaximize.Click += new System.EventHandler(this.pnlmaximize_Click);
            this.pnlmaximize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlmaximize_MouseDown);
            this.pnlmaximize.MouseEnter += new System.EventHandler(this.pnlmaximize_MouseEnter);
            this.pnlmaximize.MouseLeave += new System.EventHandler(this.pnlmaximize_MouseLeave);
            this.pnlmaximize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlmaximize_MouseUp);
            // 
            // pnlclose
            // 
            this.pnlclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlclose.BackColor = System.Drawing.Color.Red;
            this.pnlclose.Location = new System.Drawing.Point(703, 3);
            this.pnlclose.Name = "pnlclose";
            this.pnlclose.Size = new System.Drawing.Size(24, 24);
            this.pnlclose.TabIndex = 1;
            this.pnlclose.Click += new System.EventHandler(this.pnlclose_Click);
            this.pnlclose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlclose_MouseDown);
            this.pnlclose.MouseEnter += new System.EventHandler(this.pnlclose_MouseEnter);
            this.pnlclose.MouseLeave += new System.EventHandler(this.pnlclose_MouseLeave);
            this.pnlclose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlclose_MouseUp);
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
            this.pnltitleright.Location = new System.Drawing.Point(728, 0);
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
            this.lbtitletext.Click += new System.EventHandler(this.lbtitletext_Click);
            this.lbtitletext.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbtitletext_MouseMove);
            // 
            // pnlbottom
            // 
            this.pnlbottom.BackColor = System.Drawing.Color.Black;
            this.pnlbottom.Controls.Add(this.pnlbottomr);
            this.pnlbottom.Controls.Add(this.pnlbottoml);
            this.pnlbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlbottom.Location = new System.Drawing.Point(0, 491);
            this.pnlbottom.Name = "pnlbottom";
            this.pnlbottom.Size = new System.Drawing.Size(730, 2);
            this.pnlbottom.TabIndex = 1;
            this.pnlbottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseDown);
            this.pnlbottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlbottom_MouseMove);
            this.pnlbottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseUp);
            // 
            // pnlbottomr
            // 
            this.pnlbottomr.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlbottomr.Location = new System.Drawing.Point(728, 0);
            this.pnlbottomr.Name = "pnlbottomr";
            this.pnlbottomr.Size = new System.Drawing.Size(2, 2);
            this.pnlbottomr.TabIndex = 3;
            this.pnlbottomr.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseDown);
            this.pnlbottomr.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlbottomr_MouseMove);
            this.pnlbottomr.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseUp);
            // 
            // pnlbottoml
            // 
            this.pnlbottoml.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlbottoml.Location = new System.Drawing.Point(0, 0);
            this.pnlbottoml.Name = "pnlbottoml";
            this.pnlbottoml.Size = new System.Drawing.Size(2, 2);
            this.pnlbottoml.TabIndex = 2;
            this.pnlbottoml.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseDown);
            this.pnlbottoml.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlbottoml_MouseMove);
            this.pnlbottoml.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseUp);
            // 
            // pnlleft
            // 
            this.pnlleft.BackColor = System.Drawing.Color.Black;
            this.pnlleft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlleft.Location = new System.Drawing.Point(0, 30);
            this.pnlleft.Name = "pnlleft";
            this.pnlleft.Size = new System.Drawing.Size(2, 461);
            this.pnlleft.TabIndex = 2;
            this.pnlleft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseDown);
            this.pnlleft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlleft_MouseMove);
            this.pnlleft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseUp);
            // 
            // pnlright
            // 
            this.pnlright.BackColor = System.Drawing.Color.Black;
            this.pnlright.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlright.Location = new System.Drawing.Point(728, 30);
            this.pnlright.Name = "pnlright";
            this.pnlright.Size = new System.Drawing.Size(2, 461);
            this.pnlright.TabIndex = 3;
            this.pnlright.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseDown);
            this.pnlright.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseMove);
            this.pnlright.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlright_MouseUp);
            // 
            // pnlcontents
            // 
            this.pnlcontents.BackColor = System.Drawing.Color.Black;
            this.pnlcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlcontents.ForeColor = System.Drawing.Color.White;
            this.pnlcontents.Location = new System.Drawing.Point(2, 30);
            this.pnlcontents.Name = "pnlcontents";
            this.pnlcontents.Size = new System.Drawing.Size(726, 461);
            this.pnlcontents.TabIndex = 4;
            // 
            // WindowBorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 493);
            this.Controls.Add(this.pnlcontents);
            this.Controls.Add(this.pnlright);
            this.Controls.Add(this.pnlleft);
            this.Controls.Add(this.pnlbottom);
            this.Controls.Add(this.pnltitle);
            this.Name = "WindowBorder";
            this.Load += new System.EventHandler(this.WindowBorder_Load);
            this.pnltitle.ResumeLayout(false);
            this.pnltitle.PerformLayout();
            this.pnlbottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnltitle;
        private System.Windows.Forms.Label lbtitletext;
        private System.Windows.Forms.Panel pnlminimize;
        private System.Windows.Forms.Panel pnlmaximize;
        private System.Windows.Forms.Panel pnlclose;
        private System.Windows.Forms.Panel pnlbottom;
        private System.Windows.Forms.Panel pnlbottomr;
        private System.Windows.Forms.Panel pnlbottoml;
        private System.Windows.Forms.Panel pnlleft;
        private System.Windows.Forms.Panel pnlright;
        private System.Windows.Forms.Panel pnlcontents;
        private System.Windows.Forms.Panel pnltitleright;
        private System.Windows.Forms.Panel pnltitleleft;
        private System.Windows.Forms.Panel pnlicon;
    }
}
