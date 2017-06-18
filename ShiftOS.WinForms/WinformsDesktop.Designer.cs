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

namespace ShiftOS.WinForms
{
    partial class WinformsDesktop
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
            this.desktoppanel = new System.Windows.Forms.Panel();
            this.pnlnotifications = new System.Windows.Forms.Panel();
            this.flnotifications = new System.Windows.Forms.FlowLayoutPanel();
            this.ntconnectionstatus = new System.Windows.Forms.PictureBox();
            this.lbtime = new System.Windows.Forms.Label();
            this.panelbuttonholder = new System.Windows.Forms.FlowLayoutPanel();
            this.sysmenuholder = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.apps = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlscreensaver = new System.Windows.Forms.Panel();
            this.pnlssicon = new System.Windows.Forms.Panel();
            this.pnlwidgetlayer = new System.Windows.Forms.Panel();
            this.pnlnotificationbox = new System.Windows.Forms.Panel();
            this.lbnotemsg = new System.Windows.Forms.Label();
            this.lbnotetitle = new System.Windows.Forms.Label();
            this.pnladvancedal = new System.Windows.Forms.Panel();
            this.flapps = new System.Windows.Forms.FlowLayoutPanel();
            this.flcategories = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlalsystemactions = new System.Windows.Forms.Panel();
            this.btnshutdown = new System.Windows.Forms.Button();
            this.pnlstatus = new System.Windows.Forms.Panel();
            this.lbalstatus = new System.Windows.Forms.Label();
            this.desktoppanel.SuspendLayout();
            this.pnlnotifications.SuspendLayout();
            this.flnotifications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntconnectionstatus)).BeginInit();
            this.sysmenuholder.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlscreensaver.SuspendLayout();
            this.pnlnotificationbox.SuspendLayout();
            this.pnladvancedal.SuspendLayout();
            this.pnlalsystemactions.SuspendLayout();
            this.pnlstatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // desktoppanel
            // 
            this.desktoppanel.BackColor = System.Drawing.Color.Green;
            this.desktoppanel.Controls.Add(this.pnlnotifications);
            this.desktoppanel.Controls.Add(this.panelbuttonholder);
            this.desktoppanel.Controls.Add(this.sysmenuholder);
            this.desktoppanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.desktoppanel.Location = new System.Drawing.Point(0, 0);
            this.desktoppanel.Name = "desktoppanel";
            this.desktoppanel.Size = new System.Drawing.Size(1296, 24);
            this.desktoppanel.TabIndex = 0;
            this.desktoppanel.Paint += new System.Windows.Forms.PaintEventHandler(this.desktoppanel_Paint);
            // 
            // pnlnotifications
            // 
            this.pnlnotifications.Controls.Add(this.flnotifications);
            this.pnlnotifications.Controls.Add(this.lbtime);
            this.pnlnotifications.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlnotifications.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlnotifications.Location = new System.Drawing.Point(1096, 0);
            this.pnlnotifications.Name = "pnlnotifications";
            this.pnlnotifications.Size = new System.Drawing.Size(200, 24);
            this.pnlnotifications.TabIndex = 3;
            // 
            // flnotifications
            // 
            this.flnotifications.AutoSize = true;
            this.flnotifications.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flnotifications.Controls.Add(this.ntconnectionstatus);
            this.flnotifications.Dock = System.Windows.Forms.DockStyle.Left;
            this.flnotifications.Location = new System.Drawing.Point(0, 0);
            this.flnotifications.Name = "flnotifications";
            this.flnotifications.Size = new System.Drawing.Size(22, 24);
            this.flnotifications.TabIndex = 1;
            this.flnotifications.WrapContents = false;
            // 
            // ntconnectionstatus
            // 
            this.ntconnectionstatus.Image = global::ShiftOS.WinForms.Properties.Resources.notestate_connection_full;
            this.ntconnectionstatus.Location = new System.Drawing.Point(3, 3);
            this.ntconnectionstatus.Name = "ntconnectionstatus";
            this.ntconnectionstatus.Size = new System.Drawing.Size(16, 16);
            this.ntconnectionstatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ntconnectionstatus.TabIndex = 0;
            this.ntconnectionstatus.TabStop = false;
            this.ntconnectionstatus.Tag = "digitalsociety";
            // 
            // lbtime
            // 
            this.lbtime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbtime.AutoSize = true;
            this.lbtime.Location = new System.Drawing.Point(139, 7);
            this.lbtime.Name = "lbtime";
            this.lbtime.Size = new System.Drawing.Size(49, 14);
            this.lbtime.TabIndex = 0;
            this.lbtime.Text = "label1";
            this.lbtime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbtime.Click += new System.EventHandler(this.lbtime_Click);
            // 
            // panelbuttonholder
            // 
            this.panelbuttonholder.AutoSize = true;
            this.panelbuttonholder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelbuttonholder.Location = new System.Drawing.Point(107, -77);
            this.panelbuttonholder.Name = "panelbuttonholder";
            this.panelbuttonholder.Size = new System.Drawing.Size(0, 0);
            this.panelbuttonholder.TabIndex = 2;
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
            this.apps.Name = "apps";
            this.apps.Padding = new System.Windows.Forms.Padding(0);
            this.apps.Size = new System.Drawing.Size(58, 20);
            this.apps.Tag = "applauncherbutton";
            this.apps.Text = "ShiftOS";
            this.apps.Click += new System.EventHandler(this.apps_Click);
            // 
            // pnlscreensaver
            // 
            this.pnlscreensaver.Controls.Add(this.pnlssicon);
            this.pnlscreensaver.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlscreensaver.Location = new System.Drawing.Point(0, 24);
            this.pnlscreensaver.Name = "pnlscreensaver";
            this.pnlscreensaver.Size = new System.Drawing.Size(1296, 714);
            this.pnlscreensaver.TabIndex = 1;
            this.pnlscreensaver.Visible = false;
            // 
            // pnlssicon
            // 
            this.pnlssicon.Location = new System.Drawing.Point(303, 495);
            this.pnlssicon.Name = "pnlssicon";
            this.pnlssicon.Size = new System.Drawing.Size(200, 100);
            this.pnlssicon.TabIndex = 0;
            // 
            // pnlwidgetlayer
            // 
            this.pnlwidgetlayer.BackColor = System.Drawing.Color.Transparent;
            this.pnlwidgetlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlwidgetlayer.Location = new System.Drawing.Point(0, 24);
            this.pnlwidgetlayer.Name = "pnlwidgetlayer";
            this.pnlwidgetlayer.Size = new System.Drawing.Size(1296, 714);
            this.pnlwidgetlayer.TabIndex = 1;
            // 
            // pnlnotificationbox
            // 
            this.pnlnotificationbox.AutoSize = true;
            this.pnlnotificationbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlnotificationbox.Controls.Add(this.lbnotemsg);
            this.pnlnotificationbox.Controls.Add(this.lbnotetitle);
            this.pnlnotificationbox.Location = new System.Drawing.Point(654, 111);
            this.pnlnotificationbox.Name = "pnlnotificationbox";
            this.pnlnotificationbox.Size = new System.Drawing.Size(69, 68);
            this.pnlnotificationbox.TabIndex = 0;
            this.pnlnotificationbox.Visible = false;
            // 
            // lbnotemsg
            // 
            this.lbnotemsg.AutoSize = true;
            this.lbnotemsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbnotemsg.Location = new System.Drawing.Point(0, 34);
            this.lbnotemsg.MaximumSize = new System.Drawing.Size(350, 0);
            this.lbnotemsg.Name = "lbnotemsg";
            this.lbnotemsg.Padding = new System.Windows.Forms.Padding(10);
            this.lbnotemsg.Size = new System.Drawing.Size(69, 34);
            this.lbnotemsg.TabIndex = 1;
            this.lbnotemsg.Text = "label1";
            // 
            // lbnotetitle
            // 
            this.lbnotetitle.AutoSize = true;
            this.lbnotetitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbnotetitle.Location = new System.Drawing.Point(0, 0);
            this.lbnotetitle.Margin = new System.Windows.Forms.Padding(10);
            this.lbnotetitle.Name = "lbnotetitle";
            this.lbnotetitle.Padding = new System.Windows.Forms.Padding(10);
            this.lbnotetitle.Size = new System.Drawing.Size(69, 34);
            this.lbnotetitle.TabIndex = 0;
            this.lbnotetitle.Tag = "header2";
            this.lbnotetitle.Text = "label1";
            // 
            // pnladvancedal
            // 
            this.pnladvancedal.Controls.Add(this.flapps);
            this.pnladvancedal.Controls.Add(this.flcategories);
            this.pnladvancedal.Controls.Add(this.pnlalsystemactions);
            this.pnladvancedal.Controls.Add(this.pnlstatus);
            this.pnladvancedal.Location = new System.Drawing.Point(0, 24);
            this.pnladvancedal.Name = "pnladvancedal";
            this.pnladvancedal.Size = new System.Drawing.Size(433, 417);
            this.pnladvancedal.TabIndex = 1;
            this.pnladvancedal.Visible = false;
            // 
            // flapps
            // 
            this.flapps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flapps.Location = new System.Drawing.Point(221, 58);
            this.flapps.Name = "flapps";
            this.flapps.Size = new System.Drawing.Size(212, 328);
            this.flapps.TabIndex = 3;
            // 
            // flcategories
            // 
            this.flcategories.Dock = System.Windows.Forms.DockStyle.Left;
            this.flcategories.Location = new System.Drawing.Point(0, 58);
            this.flcategories.Name = "flcategories";
            this.flcategories.Size = new System.Drawing.Size(221, 328);
            this.flcategories.TabIndex = 2;
            // 
            // pnlalsystemactions
            // 
            this.pnlalsystemactions.Controls.Add(this.btnshutdown);
            this.pnlalsystemactions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlalsystemactions.Location = new System.Drawing.Point(0, 386);
            this.pnlalsystemactions.Name = "pnlalsystemactions";
            this.pnlalsystemactions.Size = new System.Drawing.Size(433, 31);
            this.pnlalsystemactions.TabIndex = 1;
            // 
            // btnshutdown
            // 
            this.btnshutdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnshutdown.AutoSize = true;
            this.btnshutdown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnshutdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnshutdown.Location = new System.Drawing.Point(355, 3);
            this.btnshutdown.Name = "btnshutdown";
            this.btnshutdown.Size = new System.Drawing.Size(75, 26);
            this.btnshutdown.TabIndex = 0;
            this.btnshutdown.Text = "Shutdown";
            this.btnshutdown.UseVisualStyleBackColor = true;
            this.btnshutdown.Click += new System.EventHandler(this.btnshutdown_Click);
            // 
            // pnlstatus
            // 
            this.pnlstatus.Controls.Add(this.lbalstatus);
            this.pnlstatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlstatus.Location = new System.Drawing.Point(0, 0);
            this.pnlstatus.Name = "pnlstatus";
            this.pnlstatus.Size = new System.Drawing.Size(433, 58);
            this.pnlstatus.TabIndex = 0;
            // 
            // lbalstatus
            // 
            this.lbalstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbalstatus.Location = new System.Drawing.Point(0, 0);
            this.lbalstatus.Name = "lbalstatus";
            this.lbalstatus.Size = new System.Drawing.Size(433, 58);
            this.lbalstatus.TabIndex = 0;
            this.lbalstatus.Text = "michael@system\r\n0 Codepoints\r\n0 installed, 0 available";
            this.lbalstatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WinformsDesktop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1296, 738);
            this.Controls.Add(this.pnlnotificationbox);
            this.Controls.Add(this.pnlwidgetlayer);
            this.Controls.Add(this.pnladvancedal);
            this.Controls.Add(this.pnlscreensaver);
            this.Controls.Add(this.desktoppanel);
            this.Font = new System.Drawing.Font("Consolas", 9F);
            this.ForeColor = System.Drawing.Color.LightGreen;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "WinformsDesktop";
            this.Text = "Desktop";
            this.Load += new System.EventHandler(this.Desktop_Load);
            this.desktoppanel.ResumeLayout(false);
            this.desktoppanel.PerformLayout();
            this.pnlnotifications.ResumeLayout(false);
            this.pnlnotifications.PerformLayout();
            this.flnotifications.ResumeLayout(false);
            this.flnotifications.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntconnectionstatus)).EndInit();
            this.sysmenuholder.ResumeLayout(false);
            this.sysmenuholder.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlscreensaver.ResumeLayout(false);
            this.pnlnotificationbox.ResumeLayout(false);
            this.pnlnotificationbox.PerformLayout();
            this.pnladvancedal.ResumeLayout(false);
            this.pnlalsystemactions.ResumeLayout(false);
            this.pnlalsystemactions.PerformLayout();
            this.pnlstatus.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel desktoppanel;
        private System.Windows.Forms.Label lbtime;
        private System.Windows.Forms.Panel sysmenuholder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem apps;
        private System.Windows.Forms.FlowLayoutPanel panelbuttonholder;
        private System.Windows.Forms.Panel pnlscreensaver;
        private System.Windows.Forms.Panel pnlssicon;
        private System.Windows.Forms.Panel pnladvancedal;
        private System.Windows.Forms.Panel pnlalsystemactions;
        private System.Windows.Forms.Button btnshutdown;
        private System.Windows.Forms.Panel pnlstatus;
        private System.Windows.Forms.Label lbalstatus;
        private System.Windows.Forms.FlowLayoutPanel flapps;
        private System.Windows.Forms.FlowLayoutPanel flcategories;
        private System.Windows.Forms.Panel pnlwidgetlayer;
        private System.Windows.Forms.Panel pnlnotifications;
        private System.Windows.Forms.FlowLayoutPanel flnotifications;
        private System.Windows.Forms.Panel pnlnotificationbox;
        private System.Windows.Forms.Label lbnotemsg;
        private System.Windows.Forms.Label lbnotetitle;
        private System.Windows.Forms.PictureBox ntconnectionstatus;
    }

}

