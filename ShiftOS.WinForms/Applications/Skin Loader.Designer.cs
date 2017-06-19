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
    partial class Skin_Loader
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
            this.pnldesktop = new System.Windows.Forms.Panel();
            this.desktoppanel = new System.Windows.Forms.Panel();
            this.sysmenuholder = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.apps = new System.Windows.Forms.ToolStripMenuItem();
            this.item1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.item4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbtime = new System.Windows.Forms.Label();
            this.pnlborder = new System.Windows.Forms.Panel();
            this.pnlcontents = new System.Windows.Forms.Panel();
            this.pnlbottom = new System.Windows.Forms.Panel();
            this.pnlbottomr = new System.Windows.Forms.Panel();
            this.pnlbottoml = new System.Windows.Forms.Panel();
            this.pnlright = new System.Windows.Forms.Panel();
            this.pnlleft = new System.Windows.Forms.Panel();
            this.pnltitlemaster = new System.Windows.Forms.Panel();
            this.pnltitle = new System.Windows.Forms.Panel();
            this.pnlminimize = new System.Windows.Forms.Panel();
            this.pnlmaximize = new System.Windows.Forms.Panel();
            this.pnlclose = new System.Windows.Forms.Panel();
            this.lbtitletext = new System.Windows.Forms.Label();
            this.pnltitleright = new System.Windows.Forms.Panel();
            this.pnltitleleft = new System.Windows.Forms.Panel();
            this.flbuttons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnloaddefault = new System.Windows.Forms.Button();
            this.btnexport = new System.Windows.Forms.Button();
            this.btnimport = new System.Windows.Forms.Button();
            this.btnapply = new System.Windows.Forms.Button();
            this.pnldesktop.SuspendLayout();
            this.desktoppanel.SuspendLayout();
            this.sysmenuholder.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlborder.SuspendLayout();
            this.pnlbottom.SuspendLayout();
            this.pnltitlemaster.SuspendLayout();
            this.pnltitle.SuspendLayout();
            this.flbuttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnldesktop
            // 
            this.pnldesktop.BackColor = System.Drawing.Color.Black;
            this.pnldesktop.Controls.Add(this.desktoppanel);
            this.pnldesktop.Location = new System.Drawing.Point(13, 13);
            this.pnldesktop.Name = "pnldesktop";
            this.pnldesktop.Size = new System.Drawing.Size(522, 251);
            this.pnldesktop.TabIndex = 0;
            // 
            // desktoppanel
            // 
            this.desktoppanel.BackColor = System.Drawing.Color.Green;
            this.desktoppanel.Controls.Add(this.sysmenuholder);
            this.desktoppanel.Controls.Add(this.lbtime);
            this.desktoppanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.desktoppanel.Location = new System.Drawing.Point(0, 0);
            this.desktoppanel.Name = "desktoppanel";
            this.desktoppanel.Size = new System.Drawing.Size(522, 24);
            this.desktoppanel.TabIndex = 1;
            // 
            // sysmenuholder
            // 
            this.sysmenuholder.Controls.Add(this.menuStrip1);
            this.sysmenuholder.Location = new System.Drawing.Point(12, 5);
            this.sysmenuholder.Name = "sysmenuholder";
            this.sysmenuholder.Size = new System.Drawing.Size(68, 24);
            this.sysmenuholder.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.apps});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(68, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // apps
            // 
            this.apps.AutoSize = false;
            this.apps.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.item1ToolStripMenuItem,
            this.item2ToolStripMenuItem,
            this.item3ToolStripMenuItem,
            this.toolStripSeparator1,
            this.item4ToolStripMenuItem});
            this.apps.Name = "apps";
            this.apps.Padding = new System.Windows.Forms.Padding(0);
            this.apps.Size = new System.Drawing.Size(58, 20);
            this.apps.Tag = "applauncherbutton";
            this.apps.Text = "ShiftOS";
            // 
            // item1ToolStripMenuItem
            // 
            this.item1ToolStripMenuItem.Name = "item1ToolStripMenuItem";
            this.item1ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item1ToolStripMenuItem.Text = "Item 1";
            // 
            // item2ToolStripMenuItem
            // 
            this.item2ToolStripMenuItem.Name = "item2ToolStripMenuItem";
            this.item2ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item2ToolStripMenuItem.Text = "Item 2";
            // 
            // item3ToolStripMenuItem
            // 
            this.item3ToolStripMenuItem.Name = "item3ToolStripMenuItem";
            this.item3ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item3ToolStripMenuItem.Text = "Item 3";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // item4ToolStripMenuItem
            // 
            this.item4ToolStripMenuItem.Name = "item4ToolStripMenuItem";
            this.item4ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item4ToolStripMenuItem.Text = "Item 4";
            // 
            // lbtime
            // 
            this.lbtime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbtime.AutoSize = true;
            this.lbtime.Location = new System.Drawing.Point(445, 5);
            this.lbtime.Name = "lbtime";
            this.lbtime.Size = new System.Drawing.Size(35, 13);
            this.lbtime.TabIndex = 0;
            this.lbtime.Text = "label1";
            this.lbtime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlborder
            // 
            this.pnlborder.BackColor = System.Drawing.Color.Black;
            this.pnlborder.Controls.Add(this.pnlcontents);
            this.pnlborder.Controls.Add(this.pnlbottom);
            this.pnlborder.Controls.Add(this.pnlright);
            this.pnlborder.Controls.Add(this.pnlleft);
            this.pnlborder.Controls.Add(this.pnltitlemaster);
            this.pnlborder.Location = new System.Drawing.Point(12, 270);
            this.pnlborder.Name = "pnlborder";
            this.pnlborder.Size = new System.Drawing.Size(522, 251);
            this.pnlborder.TabIndex = 1;
            // 
            // pnlcontents
            // 
            this.pnlcontents.BackColor = System.Drawing.Color.Black;
            this.pnlcontents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlcontents.ForeColor = System.Drawing.Color.White;
            this.pnlcontents.Location = new System.Drawing.Point(2, 30);
            this.pnlcontents.Name = "pnlcontents";
            this.pnlcontents.Size = new System.Drawing.Size(518, 219);
            this.pnlcontents.TabIndex = 9;
            // 
            // pnlbottom
            // 
            this.pnlbottom.BackColor = System.Drawing.Color.Black;
            this.pnlbottom.Controls.Add(this.pnlbottomr);
            this.pnlbottom.Controls.Add(this.pnlbottoml);
            this.pnlbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlbottom.Location = new System.Drawing.Point(2, 249);
            this.pnlbottom.Name = "pnlbottom";
            this.pnlbottom.Size = new System.Drawing.Size(518, 2);
            this.pnlbottom.TabIndex = 6;
            // 
            // pnlbottomr
            // 
            this.pnlbottomr.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlbottomr.Location = new System.Drawing.Point(516, 0);
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
            this.pnlright.Location = new System.Drawing.Point(520, 30);
            this.pnlright.Name = "pnlright";
            this.pnlright.Size = new System.Drawing.Size(2, 221);
            this.pnlright.TabIndex = 8;
            // 
            // pnlleft
            // 
            this.pnlleft.BackColor = System.Drawing.Color.Black;
            this.pnlleft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlleft.Location = new System.Drawing.Point(0, 30);
            this.pnlleft.Name = "pnlleft";
            this.pnlleft.Size = new System.Drawing.Size(2, 221);
            this.pnlleft.TabIndex = 7;
            // 
            // pnltitlemaster
            // 
            this.pnltitlemaster.Controls.Add(this.pnltitle);
            this.pnltitlemaster.Controls.Add(this.pnltitleright);
            this.pnltitlemaster.Controls.Add(this.pnltitleleft);
            this.pnltitlemaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnltitlemaster.Location = new System.Drawing.Point(0, 0);
            this.pnltitlemaster.Name = "pnltitlemaster";
            this.pnltitlemaster.Size = new System.Drawing.Size(522, 30);
            this.pnltitlemaster.TabIndex = 5;
            // 
            // pnltitle
            // 
            this.pnltitle.BackColor = System.Drawing.Color.Black;
            this.pnltitle.Controls.Add(this.pnlminimize);
            this.pnltitle.Controls.Add(this.pnlmaximize);
            this.pnltitle.Controls.Add(this.pnlclose);
            this.pnltitle.Controls.Add(this.lbtitletext);
            this.pnltitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnltitle.Location = new System.Drawing.Point(2, 0);
            this.pnltitle.Name = "pnltitle";
            this.pnltitle.Size = new System.Drawing.Size(518, 30);
            this.pnltitle.TabIndex = 0;
            // 
            // pnlminimize
            // 
            this.pnlminimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlminimize.BackColor = System.Drawing.Color.Green;
            this.pnlminimize.Location = new System.Drawing.Point(437, 3);
            this.pnlminimize.Name = "pnlminimize";
            this.pnlminimize.Size = new System.Drawing.Size(24, 24);
            this.pnlminimize.TabIndex = 3;
            // 
            // pnlmaximize
            // 
            this.pnlmaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlmaximize.BackColor = System.Drawing.Color.Yellow;
            this.pnlmaximize.Location = new System.Drawing.Point(464, 3);
            this.pnlmaximize.Name = "pnlmaximize";
            this.pnlmaximize.Size = new System.Drawing.Size(24, 24);
            this.pnlmaximize.TabIndex = 2;
            // 
            // pnlclose
            // 
            this.pnlclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlclose.BackColor = System.Drawing.Color.Red;
            this.pnlclose.Location = new System.Drawing.Point(491, 3);
            this.pnlclose.Name = "pnlclose";
            this.pnlclose.Size = new System.Drawing.Size(24, 24);
            this.pnlclose.TabIndex = 1;
            // 
            // lbtitletext
            // 
            this.lbtitletext.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbtitletext.AutoSize = true;
            this.lbtitletext.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.lbtitletext.ForeColor = System.Drawing.Color.White;
            this.lbtitletext.Location = new System.Drawing.Point(4, 0);
            this.lbtitletext.Name = "lbtitletext";
            this.lbtitletext.Size = new System.Drawing.Size(77, 14);
            this.lbtitletext.TabIndex = 0;
            this.lbtitletext.Text = "Title text";
            this.lbtitletext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbtitletext.UseMnemonic = false;
            // 
            // pnltitleright
            // 
            this.pnltitleright.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnltitleright.Location = new System.Drawing.Point(520, 0);
            this.pnltitleright.Name = "pnltitleright";
            this.pnltitleright.Size = new System.Drawing.Size(2, 30);
            this.pnltitleright.TabIndex = 5;
            // 
            // pnltitleleft
            // 
            this.pnltitleleft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnltitleleft.Location = new System.Drawing.Point(0, 0);
            this.pnltitleleft.Name = "pnltitleleft";
            this.pnltitleleft.Size = new System.Drawing.Size(2, 30);
            this.pnltitleleft.TabIndex = 4;
            // 
            // flbuttons
            // 
            this.flbuttons.AutoSize = true;
            this.flbuttons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flbuttons.Controls.Add(this.btnclose);
            this.flbuttons.Controls.Add(this.btnloaddefault);
            this.flbuttons.Controls.Add(this.btnexport);
            this.flbuttons.Controls.Add(this.btnimport);
            this.flbuttons.Controls.Add(this.btnapply);
            this.flbuttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flbuttons.Location = new System.Drawing.Point(0, 629);
            this.flbuttons.Name = "flbuttons";
            this.flbuttons.Size = new System.Drawing.Size(547, 29);
            this.flbuttons.TabIndex = 2;
            // 
            // btnclose
            // 
            this.btnclose.AutoSize = true;
            this.btnclose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnclose.Location = new System.Drawing.Point(3, 3);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(89, 23);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "{GEN_CLOSE}";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnloaddefault
            // 
            this.btnloaddefault.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnloaddefault.AutoSize = true;
            this.btnloaddefault.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnloaddefault.Location = new System.Drawing.Point(98, 3);
            this.btnloaddefault.Name = "btnloaddefault";
            this.btnloaddefault.Size = new System.Drawing.Size(132, 23);
            this.btnloaddefault.TabIndex = 1;
            this.btnloaddefault.Text = "{GEN_LOADDEFAULT}";
            this.btnloaddefault.UseVisualStyleBackColor = true;
            this.btnloaddefault.Click += new System.EventHandler(this.btnloaddefault_Click);
            // 
            // btnexport
            // 
            this.btnexport.AutoSize = true;
            this.btnexport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnexport.Location = new System.Drawing.Point(236, 3);
            this.btnexport.Name = "btnexport";
            this.btnexport.Size = new System.Drawing.Size(82, 23);
            this.btnexport.TabIndex = 2;
            this.btnexport.Text = "{GEN_SAVE}";
            this.btnexport.UseVisualStyleBackColor = true;
            this.btnexport.Click += new System.EventHandler(this.btnexport_Click);
            // 
            // btnimport
            // 
            this.btnimport.AutoSize = true;
            this.btnimport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnimport.Location = new System.Drawing.Point(324, 3);
            this.btnimport.Name = "btnimport";
            this.btnimport.Size = new System.Drawing.Size(83, 23);
            this.btnimport.TabIndex = 3;
            this.btnimport.Text = "{GEN_LOAD}";
            this.btnimport.UseVisualStyleBackColor = true;
            this.btnimport.Click += new System.EventHandler(this.btnimport_Click);
            // 
            // btnapply
            // 
            this.btnapply.AutoSize = true;
            this.btnapply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnapply.Location = new System.Drawing.Point(413, 3);
            this.btnapply.Name = "btnapply";
            this.btnapply.Size = new System.Drawing.Size(88, 23);
            this.btnapply.TabIndex = 4;
            this.btnapply.Text = "{GEN_APPLY}";
            this.btnapply.UseVisualStyleBackColor = true;
            this.btnapply.Click += new System.EventHandler(this.btnapply_Click);
            // 
            // Skin_Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flbuttons);
            this.Controls.Add(this.pnlborder);
            this.Controls.Add(this.pnldesktop);
            this.Name = "Skin_Loader";
            this.Size = new System.Drawing.Size(547, 658);
            this.pnldesktop.ResumeLayout(false);
            this.desktoppanel.ResumeLayout(false);
            this.desktoppanel.PerformLayout();
            this.sysmenuholder.ResumeLayout(false);
            this.sysmenuholder.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlborder.ResumeLayout(false);
            this.pnlbottom.ResumeLayout(false);
            this.pnltitlemaster.ResumeLayout(false);
            this.pnltitle.ResumeLayout(false);
            this.pnltitle.PerformLayout();
            this.flbuttons.ResumeLayout(false);
            this.flbuttons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnldesktop;
        private System.Windows.Forms.Panel pnlborder;
        private System.Windows.Forms.FlowLayoutPanel flbuttons;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnloaddefault;
        private System.Windows.Forms.Button btnexport;
        private System.Windows.Forms.Button btnimport;
        private System.Windows.Forms.Button btnapply;
        private System.Windows.Forms.Panel desktoppanel;
        private System.Windows.Forms.Panel sysmenuholder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem apps;
        private System.Windows.Forms.ToolStripMenuItem item1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem item4ToolStripMenuItem;
        private System.Windows.Forms.Label lbtime;
        private System.Windows.Forms.Panel pnlcontents;
        private System.Windows.Forms.Panel pnlbottom;
        private System.Windows.Forms.Panel pnlbottomr;
        private System.Windows.Forms.Panel pnlbottoml;
        private System.Windows.Forms.Panel pnlright;
        private System.Windows.Forms.Panel pnlleft;
        private System.Windows.Forms.Panel pnltitlemaster;
        private System.Windows.Forms.Panel pnltitle;
        private System.Windows.Forms.Panel pnlminimize;
        private System.Windows.Forms.Panel pnlmaximize;
        private System.Windows.Forms.Panel pnlclose;
        private System.Windows.Forms.Label lbtitletext;
        private System.Windows.Forms.Panel pnltitleright;
        private System.Windows.Forms.Panel pnltitleleft;
    }
}